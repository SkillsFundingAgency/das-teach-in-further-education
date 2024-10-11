using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SFA.DAS.TeachInFurtherEducation.Web.Security
{

#pragma warning disable S1075
#pragma warning disable CA1861

    [ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions
    {
        private static void BuildCsp(CspBuilder builder, IWebHostEnvironment env, IConfiguration configuration)
        {
            string cdnUrl = configuration["cdn:url"]!;

            string clarityId = configuration["MicrosoftClarity:MsClarityId"]!;

            var clarityMS = Enumerable.Range('A', 26)
                .SelectMany(c => new[] { $"https://{(char)c}.clarity.ms" })
                .ToArray();

            var clarityImg = Enumerable.Range('A', 26)
                .SelectMany(c => new[] { $"https://{(char)c}.bing.com" })
                .ToArray();

            var violationReportUrl = configuration["csp:violationReportUrl"];

            if (!string.IsNullOrEmpty(violationReportUrl))
            {
                builder.AddReportUri().To(violationReportUrl!);
            }
            
            builder.AddReportTo("csp-violations");            

            builder.AddUpgradeInsecureRequests();

            builder.AddDefaultSrc()
                .Self()
                .From(cdnUrl);

            builder.AddImgSrc()
                .Self()
                .From(clarityMS)
                .From(clarityImg)
                .From(new[] { cdnUrl, "https://ssl.gstatic.com", "https://www.gstatic.com", "https://www.google-analytics.com", "https://images.ctfassets.net" });

            var connectSrc = builder.AddConnectSrc()
                .Self()
                .From(clarityMS)
                .From("https://region1.analytics.google.com");

            builder.AddFormAction()
                .Self()
                .From(clarityMS);

            builder.AddFrameAncestors()
                .Self()
                .From("https://app.contentful.com");

            builder.AddFrameSrc()
                .Self()
                .From(new[] { "https://videos.ctfassets.net", "https://td.doubleclick.net" })
                .From(clarityMS);

            builder.AddFontSrc()
                .Self()
                .From(new[] { cdnUrl, "https://fonts.gstatic.com", "https://rsms.me" });

            var scriptSrc = builder.AddScriptSrc()
                .Self()
                .WithNonce()
                .From(new[] { "https://das-at-frnt-end.azureedge.net", "https://das-at-frnt-end.azureedge.net/libs/jquery/jquery.min.js", $"https://www.clarity.ms/tag/{clarityId}", "https://www.clarity.ms/s/0.7.47/clarity.js", "https://www.googletagmanager.com" });

            builder.AddStyleSrc()
                .Self()
                .WithNonce()
                .From(new[] { cdnUrl, "https://rsms.me" });

            // Allow inline styles and scripts for development
            if (env.IsDevelopment())
            {
                scriptSrc
                    .From(new[] { "https://localhost" });

                connectSrc
                    .From(new[] { "http://localhost", "https://localhost", "ws://localhost", "wss://localhost", "http://localhost:*", "ws://localhost:*", "https://localhost:*", "wss://localhost:*" });
            }
        }

        /// <summary>
        /// nuget documentation
        /// https://github.com/andrewlock/NetEscapades.AspNetCore.SecurityHeaders
        /// csp introduction
        /// https://scotthelme.co.uk/content-security-policy-an-introduction/
        /// google analytics tag manager required csp
        /// https://developers.google.com/tag-platform/tag-manager/web/csp
        /// jquery csp
        /// https://content-security-policy.com/examples/jquery/
        /// das ga
        /// https://skillsfundingagency.atlassian.net/wiki/spaces/DAS/pages/3249700873/Adding+Google+Analytics
        /// </summary>
        public static IApplicationBuilder UseAppSecurityHeaders(
            this IApplicationBuilder app,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            _ = app.UseSecurityHeaders(policies =>
            {
                policies
                    .AddDefaultSecurityHeaders()
                    .AddContentSecurityPolicy(builder => BuildCsp(builder, env, configuration))
                    .AddCustomHeader("X-Permitted-Cross-Domain-Policies", "none")
                    .AddXssProtectionBlock()
                    .RemoveServerHeader()
                    .RemoveCustomHeader("X-Powered-By");

                var violationReportUrl = configuration["csp:violationReportUrl"];

                if (!string.IsNullOrEmpty(violationReportUrl))
                {
                    policies
                        .AddReportingEndpoints(builder => builder.AddEndpoint("csp-violations", configuration["csp:violationReportUrl"]!));
                }
            });

            return app;
        }

        public static IApplicationBuilder UseCachingAndCompression(
            this IApplicationBuilder app)
        {

            _ = app.Use(async (context, next) =>
            {
                if (!context.Request.Path.Value!.StartsWith("/css") &&
                    !context.Request.Path.Value!.StartsWith("/js") &&
                    !context.Request.Path.Value!.StartsWith("/images"))
                {
                    context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
                }
                else
                {
                    context.Response.Headers.CacheControl = "public, max-age=86400, immutable";
                }
                await next();
            });

            return app;
        }

        public static IApplicationBuilder UseHealthCheckEndPoint(
            this IApplicationBuilder app)
        {
            _ = app.UseHealthChecks("/ping", new HealthCheckOptions
            {
                //By returning false to the Predicate option we ensure that none of the health checks registered in ConfigureServices are ran for this endpoint
                Predicate = (_) => false,
                ResponseWriter = (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("whiff-whaff");
                }
            });

            _ = app.UseHealthChecks("/health");

            return app;
        }
    }
}

#pragma warning restore CA1861
#pragma warning restore S1075

