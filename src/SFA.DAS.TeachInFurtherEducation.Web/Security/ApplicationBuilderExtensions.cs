using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SFA.DAS.TeachInFurtherEducation.Web.Security
{
#pragma warning disable S1075
#pragma warning disable CA1861

    [ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions
    {
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
        ///
        /// Note: we _may_ need the other google domains from the das ga doc,
        /// but there were no violations reported without them, so we leave them out for now
        ///
        /// Allowing unsafe-inline scripts
        /// ------------------------------
        /// Google's nonce-aware tag manager code has an issue with custom html tags (which we use).
        /// https://stackoverflow.com/questions/65100704/gtm-not-propagating-nonce-to-custom-html-tags
        /// https://dev.to/matijamrkaic/using-google-tag-manager-with-a-content-security-policy-9ai
        ///
        /// We tried the given solution (above), but the last piece of the puzzle to make it work,
        /// would involve self hosting a modified version of google's gtm.js script.
        ///
        /// In gtm.js, where it's creating customScripts, we'd have to change...
        /// var n = C.createElement("script");
        /// to
        /// var n=C.createElement("script");n.nonce=[get nonce from containing script block];
        ///
        /// The problems with self hosting a modified gtm.js are (from https://stackoverflow.com/questions/45615612/is-it-possible-to-host-google-tag-managers-script-locally)
        /// * we wouldn't automatically pick up any new tags or triggers that Steve added
        /// * we would need a version of the script that worked across all browsers and versions (and wouldn't have a browser optimised version)
        /// * we wouldn't pick up new versions of the script
        /// For these reasons, the only way to get the campaign tracking working, is to open up the CSP to allow unsafe-inline scripts.
        /// This will make our site less secure, but is a trade-off between security and tracking functionality.
        /// </summary>

        public static IApplicationBuilder UseAppSecurityHeaders(
            this IApplicationBuilder app,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            string cdnUrl = configuration["cdn:url"]!;
            string cspViolationReportUrl = configuration["csp:violationReportUrl"]!;
            string[] clarity = new[] { "https://a.clarity.ms", "https://b.clarity.ms", "https://c.clarity.ms", "https://d.clarity.ms", "https://e.clarity.ms", "https://f.clarity.ms", "https://g.clarity.ms", "https://h.clarity.ms", "https://i.clarity.ms", "https://j.clarity.ms", "https://k.clarity.ms", "https://l.clarity.ms", "https://m.clarity.ms", "https://n.clarity.ms", "https://o.clarity.ms", "https://p.clarity.ms", "https://q.clarity.ms", "https://r.clarity.ms", "https://s.clarity.ms", "https://t.clarity.ms", "https://u.clarity.ms", "https://v.clarity.ms", "https://w.clarity.ms", "https://x.clarity.ms", "https://y.clarity.ms", "https://z.clarity.ms" };

            _ = app.UseSecurityHeaders(policies =>
            policies.AddDefaultSecurityHeaders()
            .AddContentSecurityPolicy(builder =>
            {
                builder.AddReportUri()
                    .To(cspViolationReportUrl);

                builder.AddUpgradeInsecureRequests();

                builder.AddDefaultSrc()
                    .Self()
                    .From(cdnUrl);

                builder.AddImgSrc()
                    .Self()
                    .From(new[] { cdnUrl, "https://ssl.gstatic.com", "https://www.gstatic.com", "https://www.google-analytics.com", "https://images.ctfassets.net" });

                var connectSrc = builder.AddConnectSrc()
                    .Self()
                    .From(clarity);

                builder.AddFormAction()
                    .Self()
                    .From(clarity);

                builder.AddFrameAncestors()
                    .Self()
                    .From("https://app.contentful.com");

                builder.AddFrameSrc()
                    .Self()
                    .From(new[] { "https://videos.ctfassets.net" })
                    .From(clarity);

                builder.AddFontSrc()
                    .Self()
                    .From(new[] { cdnUrl, "https://fonts.gstatic.com", "https://rsms.me" });

                var scriptSrc = builder.AddScriptSrc()
                    .Self()
                    .From(new[] { "https://das-at-frnt-end.azureedge.net" })
                    .WithNonce();

                builder.AddStyleSrc()
                        .Self()
                        .From(new[] { cdnUrl, "https://rsms.me", "https://rsms.me" });

                // Allow inline styles and scripts for development
                if (env.IsDevelopment())
                {
                    scriptSrc
                        .From(new[] { "https://localhost" });

                    connectSrc
                        .From(new[] { "http://localhost", "https://localhost", "ws://localhost", "wss://localhost", "http://localhost:64212", "ws://localhost:64212", "https://localhost:44350", "wss://localhost:44350" });
                }
            })
            .AddCustomHeader("X-Permitted-Cross-Domain-Policies", "none")
            .AddXssProtectionBlock());

            return app;
        }
    }
}

#pragma warning restore CA1861
#pragma warning restore S1075