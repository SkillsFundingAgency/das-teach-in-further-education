using System.Diagnostics.CodeAnalysis;
using AspNetCore.SEOHelper;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using ContentfulExample.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.TeachInFurtherEducation.Contentful.Extensions;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.BackgroundServices;
using SFA.DAS.TeachInFurtherEducation.Web.Data;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Extensions;
using SFA.DAS.TeachInFurtherEducation.Web.GoogleAnalytics;
using SFA.DAS.TeachInFurtherEducation.Web.Helpers;
using SFA.DAS.TeachInFurtherEducation.Web.Infrastructure;
using SFA.DAS.TeachInFurtherEducation.Web.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Security;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.StartupServices;

namespace SFA.DAS.TeachInFurtherEducation.Web
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IWebHostEnvironment _currentEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _currentEnvironment = env;

            Configuration = new ConfigurationBuilder()
                .AddConfiguration(configuration)

                .AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"]?.Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                })
                .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var formOptionsConfig = Configuration.GetSection("FormOptions").Get<FormOptionsConfig>();

            // Configure a maxium submission size for security purposes
            services.Configure<FormOptions>(options =>
            {
                // Limit the request body to 4 MB (adjust as necessary)
                options.MultipartBodyLengthLimit = formOptionsConfig!.MaxRequestBodySize;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.AddRateLimiting(Configuration);

            services.AddNLog(Configuration).AddHealthChecks();

            services.AddApplicationInsightsTelemetry();


#if DEBUG
            services.AddControllersWithViews();
#else
            var googleAnalyticsConfiguration = Configuration.GetSection("GoogleAnalytics").Get<GoogleAnalyticsConfiguration>()!;
            services.AddControllersWithViews(options => options.Filters.Add(new EnableGoogleAnalyticsAttribute(googleAnalyticsConfiguration)));
#endif

            services.AddWebOptimizer(assetPipeline =>
            {
                if (!_currentEnvironment.IsDevelopment())
                {
                    assetPipeline.AddJavaScriptBundle("/js/site.js",
                        "/js/show_hide.js",
                        "/js/app.js",
                        "/js/filter.js",
                        "/js/cookies/utils.js",
                        "/js/cookies/consent.js",
                        "/js/cookies/cookie-banner.js",
                        "/js/cookies/cookies-page.js");
                    assetPipeline.AddCssBundle("/css/site.css", "/css/site.css");
                }
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IViewRenderService, ViewRenderService>();

            services.AddSingleton<HtmlRenderer>(a => ContentService.CreateHtmlRenderer());

#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
            var serviceProvider = services.BuildServiceProvider();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

            services.AddLogging(builder => builder.AddConsole());

            var viewRenderService = serviceProvider.GetRequiredService<IViewRenderService>();

            var htmlRenderer = serviceProvider.GetRequiredService<HtmlRenderer>();

            var logger = serviceProvider.GetRequiredService<ILogger<object>>();

            ComponentService.Initialize(logger, viewRenderService, htmlRenderer);

            services.AddSingleton<IAssetDownloader, AssetDownloader>();

            services.AddSingleton<IContentModelService, ContentModelService>();
            services.AddSingleton<Contentful.Services.Interfaces.Roots.IPageContentService, SFA.DAS.TeachInFurtherEducation.Contentful.Services.Roots.PageContentService>();

            services.AddSingleton<IPageService, PageService>();
            services.Configure<ContentUpdateServiceOptions>(Configuration.GetSection("ContentUpdates"));
            services.AddSingleton<IContentTypeResolver, EntityResolver>();
            services.AddContentService(Configuration).AddHostedService<ContentUpdateService>();

            services.Configure<SupplierAddressUpdateServiceOptions>(Configuration.GetSection("SupplierAddressUpdates"));
            services.AddHostedService<SupplierAddressUpdateService>();

            services.Configure<EndpointsOptions>(Configuration.GetSection("Endpoints"));
            services.Configure<GoogleAnalyticsConfiguration>(Configuration.GetSection("GoogleAnalytics"));

            // Register geolocation service (Postcodes.io in this case)
            services.AddHttpClient<PostcodesIoGeoLocationService>();
            services.AddSingleton<IGeoLocationProvider, PostcodesIoGeoLocationService>();

            // Register default Open XML Spreadsheet parser
            services.AddSingleton<ISpreadsheetParser, OpenXmlSpreadsheetParser>();
            
            // Register composite key generator for SupplierAddress
            services.AddSingleton<ICompositeKeyGenerator<SupplierAddressModel>, SupplierAddressCompositeKeyGenerator>();

            // Register MailChimp service
            services.Configure<MailChimpMarketingServiceOptions>(Configuration.GetSection("MailChimp"));
            services.AddSingleton<IMarketingService, MailChimpMarketingService>();

            // Register CSPReportService
            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, new CspReportInputFormatter());
            });

            services.AddSingleton<ICspReportService, CspReportService>();

            // Register IHttpContextAccessor for accessing HttpContext
            services.AddHttpContextAccessor();

            // Register the DbContext for SQL Server with NetTopologySuite for geospatial support
            services.AddDbContext<SqlDbContext>(options =>
                options.UseSqlServer(
                    Configuration["SqlDB:ConnectionString"],
                    sqlOptions => sqlOptions.UseNetTopologySuite() 
                ));

            services.AddScoped<ISupplierAddressRepository, SqlSupplierAddressRepository>();
            
            services.AddScoped<ISupplierAddressService, SupplierAddressService>();

            services.AddTransient<ISitemap, Sitemap>()
                .AddHostedService<SitemapGeneratorService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {
            app.UseAppSecurityHeaders(env, configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/500");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseWebOptimizer();
            app.UseStaticFiles();

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                {
                    //Re-execute the request so the user gets the error page
                    var originalPath = context.Request.Path.Value;
                    context.Items["originalPath"] = originalPath;
                    context.Request.Path = "/error/404";
                    await next();
                }
            });

            app.UseXMLSitemap(env.ContentRootPath);
            app.UseRouting();
            app.UseAuthorization();

            app.UseHealthChecks("/ping", new HealthCheckOptions
            {
                //By returning false to the Predicate option we ensure that none of the health checks registered in ConfigureServices are ran for this endpoint
                Predicate = (_) => false,
                ResponseWriter = (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("whiff-whaff");
                }
            });

            app.UseHealthChecks("/health");

            app.UseEndpoints(endpoints =>
            {
                MapControllerRoute(endpoints,
                     "landing",
                    "/{pageUrl=home}",
                    "Landing", "Landing");

                // Preview Route
                MapControllerRoute(endpoints,
                    "page-preview",
                    "preview/{pageUrl=home}",
                    "Landing", "PagePreview");

                endpoints.MapControllers();
            });
        }

        /// <remarks>
        /// Work around the over enthusiastic duplicate code quality gate in SonarCloud
        /// </remarks>
        private static void MapControllerRoute(IEndpointRouteBuilder builder, string name, string pattern, string controller, string action)
        {
            builder.MapControllerRoute(name, pattern, new { controller, action });
        }
    }
}
