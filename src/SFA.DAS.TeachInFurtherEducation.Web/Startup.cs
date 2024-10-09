using System.Diagnostics.CodeAnalysis;
using AspNetCore.SEOHelper;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
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
using System;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using NetEscapades.AspNetCore.SecurityHeaders;
using System.Security.Cryptography;
using SFA.DAS.TeachInFurtherEducation.Web.MicrosoftClarity;

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

                //.AddAzureTableStorage(options =>
                //{
                //    options.ConfigurationKeys = configuration["ConfigNames"]?.Split(",");
                //    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                //    options.EnvironmentName = configuration["EnvironmentName"];
                //    options.PreFixConfigurationKeys = false;
                //})
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

            // Enable Google Analytics
            services.AddControllersWithViews(options =>
            {
                var googleAnalyticsConfiguration = Configuration.GetSection("GoogleAnalytics").Get<GoogleAnalyticsConfiguration>()!;
                options.Filters.Add(new EnableGoogleAnalyticsAttribute(googleAnalyticsConfiguration));
            });

            // Enable Microsoft Clarity
            services.AddControllersWithViews(options =>
            {
                var microsoftClarityConfiguration = Configuration.GetSection("MicrosoftClarity").Get<MicrosoftClarityConfiguration>()!;
                options.Filters.Add(new EnableMicrosoftClarityAttribute(microsoftClarityConfiguration));
            });

            services.AddRateLimiting(Configuration);

            services.AddNLog(Configuration).AddHealthChecks();

            services.AddApplicationInsightsTelemetry();

            // Register CSPReportService
            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, new CspReportInputFormatter());
            });

            // Add Exception Filter
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ExceptionFilter>();
            });

            services.AddControllersWithViews();



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

            services.AddHsts(options =>
            {
                options.Preload = true; 
                options.IncludeSubDomains = true;  
                options.MaxAge = TimeSpan.FromDays(365); 
            });

            services.AddAntiforgery(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IHttpClientWrapper, HttpClientWrapper>();

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
            services.Configure<MicrosoftClarityConfiguration>(Configuration.GetSection("GoogleAnalytics"));

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
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {
            app.UseAppSecurityHeaders(env, configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseStatusCodePagesWithReExecute("/error/{0}");
            }

            app.UseHttpsRedirection();
            app.UseWebOptimizer();
            app.UseCachingAndCompression();
            app.UseStaticFiles();

            app.UseXMLSitemap(env.ContentRootPath);
            app.UseRouting();
            app.UseAuthorization();
            app.UseHealthCheckEndPoint();
            
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
