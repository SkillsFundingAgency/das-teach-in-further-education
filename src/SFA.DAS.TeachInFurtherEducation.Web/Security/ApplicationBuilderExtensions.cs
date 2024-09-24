using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using static System.Net.WebRequestMethods;

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

#pragma warning disable S1481 // Unused local variables should be removed

        public static IApplicationBuilder UseAppSecurityHeaders(
            this IApplicationBuilder app,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            string cdnUrl = configuration["cdn:url"]!;
            string cspViolationReportUrl = configuration["csp:violationReportUrl"]!;

            _ = app.UseSecurityHeaders(policies =>
            policies.AddDefaultSecurityHeaders()
            .AddContentSecurityPolicy(builder =>
            {
                builder.AddReportUri()
                    .To(cspViolationReportUrl);

                builder.AddUpgradeInsecureRequests();

                var defaultSrc = builder.AddDefaultSrc()
                    .Self()
                    .From(cdnUrl);

                builder.AddImgSrc()
                    .Self()
                    .From(new[] { cdnUrl, "https://ssl.gstatic.com", "https://www.gstatic.com", "https://www.google-analytics.com", "https://images.ctfassets.net" });

                var connectSrc = builder.AddConnectSrc()
                    .Self()
                    .From(new[] { "http://localhost", "https://localhost", "ws://localhost", "wss://localhost", "http://localhost:64212", "ws://localhost:64212", "https://localhost:44350", "wss://localhost:44350" });

                builder.AddFormAction()
                    .Self();

                builder.AddFrameAncestors()
                    .Self()
                    .From("https://app.contentful.com");

                builder.AddFrameSrc()
                    .Self()
                    .From(new[] { "https://videos.ctfassets.net" });

                builder.AddFontSrc()
                    .Self()
                    .From(new[] { cdnUrl, "https://fonts.gstatic.com", "https://rsms.me" });

                var scriptSrc = builder.AddScriptSrc()
                    .Self()
                    .From(new[] { "https://das-at-frnt-end.azureedge.net", "https://localhost" })
                    .WithNonce();

                var styleSrc = builder.AddStyleSrc()
                        .Self()
                        //.UnsafeInline()
                        .From(new[] { cdnUrl, "https://rsms.me", "https://rsms.me" });



                // Allow inline styles and scripts for development
                if (env.IsDevelopment())
                {
                    scriptSrc
                        //.UnsafeInline()
                        .From(new[] { "https://das-at-frnt-end.azureedge.net" });

                    //builder.AddStyleSrc()
                    //    .Self()
                    //    .UnsafeInline()
                    //    .From(new[] { cdnUrl });
                }
            })
            .AddCustomHeader("X-Permitted-Cross-Domain-Policies", "none")
            .AddXssProtectionBlock());

            return app;
        }

#pragma warning restore S1481 // Unused local variables should be removed




        //        public static IApplicationBuilder UseAppSecurityHeaders(
        //            this IApplicationBuilder app,
        //            IWebHostEnvironment env,
        //            IConfiguration configuration)
        //        {
        //            string cdnUrl = configuration["cdn:url"]!;

        //#pragma warning disable S1075
        //#pragma warning disable CA1861

        //            app.UseSecurityHeaders(policies =>
        //                policies.AddDefaultSecurityHeaders()
        //                .AddContentSecurityPolicy(builder =>
        //                {
        //                    builder.AddUpgradeInsecureRequests();

        //                    var defaultSrc = builder.AddDefaultSrc()
        //                     .Self()
        //                     .From(cdnUrl);

        //                    builder.AddImgSrc()
        //                        //.OverHttps()
        //                        .Self()
        //                        .From(new[] { cdnUrl, "data:", "https://ssl.gstatic.com", "https://www.gstatic.com", "https://www.google-analytics.com" });

        //                    var connectSrc = builder.AddConnectSrc()
        //                        .Self();

        //                    builder.AddFormAction()
        //                        .Self();
        //                    //.From(new[]
        //                    //{
        //                    //    //"https://www.facebook.com",
        //                    //    ////"*.qualtrics.com",
        //                    //    ////"*.clarity.ms",
        //                    //    ////"https://td.doubleclick.net"
        //                    //});


        //                    builder.AddFrameAncestors()
        //                        .Self()
        //                        .From("https://app.contentful.com");

        //                    builder.AddFontSrc()
        //                         .Self()
        //                         .From(new[] { cdnUrl, "https://fonts.gstatic.com" });

        //                    var scriptSrc = builder.AddScriptSrc()
        //                         .Self();

        //                    if (env.IsDevelopment())
        //                    {
        //                        // open up for browserlink
        //                        defaultSrc.From(new[] { "http://localhost" });
        //                        //defaultSrc.From(new[] { "http://localhost:*", "ws://localhost:*" });

        //                        connectSrc.From(new[] { "https://localhost" });
        //                        //connectSrc.From(new[] { "https://localhost:*", "ws://localhost:*", "wss://localhost:*" });

        //                        scriptSrc.From(new[] { "https://localhost" });
        //                    }


        //                })
        //                //.AddCustomHeader("X-Frame-Options", "ALLOW-FROM https://app.contentful.com/")
        //                .AddCustomHeader("X-Permitted-Cross-Domain-Policies", "none")

        //                // this is called in AddDefaultSecurityHeaders(), but without this, we get AddXssProtectionDisabled() instead
        //                .AddXssProtectionBlock());



        //            return app;
        //        }

        //        //public static IApplicationBuilder UseCspHeaders(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        //        //{
        //        //    app.Use(async (context, next) =>
        //        //    {
        //        //        context.Response.Headers.Append("Content-Security-Policy",
        //        //            "img-src 'self' data: https://das-at-frnt-end.azureedge.net https://ssl.gstatic.com https://www.gstatic.com https://www.google-analytics.com; " +
        //        //            "report-uri /csp-violation-report-endpoint/");

        //        //        // Disable caching
        //        //        context.Response.Headers.Append("Cache-Control", "no-store, no-cache, must-revalidate, proxy-revalidate");
        //        //        context.Response.Headers.Append("Pragma", "no-cache");
        //        //        context.Response.Headers.Append("Expires", "0");

        //        //        await next();
        //        //    });

        //        //    return app;
        //        //}
    }
}


//.AddContentSecurityPolicy(builder =>
// {
//     builder.AddUpgradeInsecureRequests();

//     var defaultSrc = builder.AddDefaultSrc()
//         .Self()
//         .From(cdnUrl);

//     var connectSrc = builder.AddConnectSrc()
//         .Self()
//         .From(new[]
//         {
//                                //"https://consent-api-bgzqvpmbyq-nw.a.run.app/api/v1/consent/",
//                                //"https://stats.g.doubleclick.net/j/collect",
//                                //"https://region1.google-analytics.com/g/collect",
//                                "https://www.google-analytics.com",
//                                "https://www.youtube-nocookie.com",
//                                ////"*.qualtrics.com", 
//                                ///* application insights*/ 
//                                //"https://dc.services.visualstudio.com/v2/track", 
//                                "rt.services.visualstudio.com/v2/track",
//                                "cdn.linkedin.oribi.io",
//             //"*.clarity.ms",
//             //"https://td.doubleclick.net",
//             //"https://px.ads.linkedin.com/wa/"
//         });

//     builder.AddFontSrc()
//         .Self()
//         .From(new[] { cdnUrl, "https://fonts.gstatic.com" });

//     builder.AddObjectSrc()
//         .None();

//     builder.AddFormAction()
//         .Self()
//         .From(new[]
//         {
//                                "https://www.facebook.com",
//             //"*.qualtrics.com",
//             //"*.clarity.ms",
//             //"https://td.doubleclick.net"
//         });



//     var scriptSrc = builder.AddScriptSrc()
//         .Self()
//         .From(new[]
//         {
//                                cdnUrl,
//             //"https://tagmanager.google.com",
//             //"https://www.google-analytics.com/",
//             //"https://www.googletagmanager.com",
//             //"https://www.googleadservices.com",
//             //"https://ssl.google-analytics.com",
//             //"https://googleads.g.doubleclick.net",
//             //"https://acdn.adnxs.com",
//             //"https://www.youtube-nocookie.com",
//             //"https://www.youtube.com",
//             //"https://snap.licdn.com",
//             //"https://analytics.twitter.com",
//             //"https://static.ads-twitter.com",
//             //"https://connect.facebook.net",
//             ////"*.qualtrics.com",
//             ////"*.clarity.ms",
//             //"https://td.doubleclick.net"
//         })
//         // this is needed for GTM and YouTube embedding
//         .UnsafeEval()
//         .UnsafeInline();
//     // if we wanted the nonce back, we'd add `.WithNonce();` here

//     builder.AddStyleSrc()
//         .Self()
//         .From(new[]
//         {
//                                cdnUrl,
//                                "https://www.googletagmanager.com",
//                                "https://tagmanager.google.com",
//                                "https://fonts.googleapis.com"
//         })
//         .StrictDynamic()
//         .UnsafeInline();

//     builder.AddMediaSrc()
//         .None();

//     builder.AddFrameAncestors()
//         .None();

//     builder.AddBaseUri()
//         .Self();

//     builder.AddFrameSrc()
//         .From(new[]
//         {
//                                "https://www.googletagmanager.com",
//                                "https://www.youtube-nocookie.com",
//                                "https://2673654.fls.doubleclick.net",
//                                "https://www.facebook.com",
//                                //"*.qualtrics.com",
//                                //"*.clarity.ms",
//                                "https://td.doubleclick.net",
//                                "https://videos.ctfassets.net/",
//                                "https://www.youtube.com/"
//         });

//     // Add frame-ancestors directive allowing embedding from specific domain(s)
//     builder.AddFrameAncestors()
//            .From("https://app.contentful.com");


// })


#pragma warning restore CA1861
#pragma warning restore S1075