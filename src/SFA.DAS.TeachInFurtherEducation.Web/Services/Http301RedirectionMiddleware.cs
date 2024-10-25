using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetEscapades.AspNetCore.SecurityHeaders.Headers;
using SFA.DAS.TeachInFurtherEducation.Web.Models;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services
{
    public class Http301RedirectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Http301RedirectConfig _config;
        private readonly ILogger<Http301RedirectionMiddleware> _logger;

        TimeSpan _regexTimeout = TimeSpan.FromMilliseconds(500);

        public Http301RedirectionMiddleware(RequestDelegate next, Http301RedirectConfig config, ILogger<Http301RedirectionMiddleware> logger)
        {
            _next = next;
            _config = config;
            _logger = logger;

            PrecompileRegexPatterns();
        }

        [ExcludeFromCodeCoverage]
        private void PrecompileRegexPatterns()
        {
            foreach (var trigger in _config.Triggers)
            {
                trigger.CompiledExp = trigger.CompiledExp ?? new Regex(trigger.Exp, RegexOptions.Compiled | RegexOptions.IgnoreCase, _regexTimeout);
                foreach (var rule in trigger.Rules)
                {
                    rule.CompiledExp = rule.CompiledExp ?? new Regex(rule.Exp, RegexOptions.Compiled | RegexOptions.IgnoreCase, _regexTimeout);
                }
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
            var referrer = context.Request.Headers["Referer"].ToString();

            bool matchExp(Regex? exp)
            {
                if (exp == null) return false;

                try
                {
                    return exp.IsMatch(requestUrl);
                }
                catch (RegexMatchTimeoutException ex)
                {
                    _logger.LogWarning(ex, "Regex match timed out for pattern '{Pattern}' and input '{Input}'", exp!.ToString(), requestUrl);
                    return false;
                }
            }

            var matchingRule = _config.Triggers?
                .Where(t => matchExp(t.CompiledExp))
                .SelectMany(t =>
                    t.Rules
                        .Where(r => matchExp(r.CompiledExp))
                        .Select(r => new { Trigger = t, Rule = r })
                )
                .OrderBy(x => x.Trigger.Seq)
                .ThenBy(x => x.Rule.Seq)
                .FirstOrDefault();

            if (matchingRule != null)
            {
                var match = matchingRule.Rule.CompiledExp!.Match(requestUrl);
                var redirectUrl = match.Result(matchingRule.Rule.SendTo);

                _logger.LogInformation("Redirecting '{RequestUrl}' to '{SendTo}' after Referral from {Referrer}", requestUrl, redirectUrl, referrer);

                if (_config.AppendReferrerOnQueryString)
                {
                    var encodedReferrer = Uri.EscapeDataString(referrer);
                    var separator = redirectUrl.Contains("?") ? "&" : "?";

                    redirectUrl = $"{redirectUrl}{separator}originalReferrer={encodedReferrer}";
                }

                context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
                context.Response.Headers["Location"] = redirectUrl;

                return; // End the middleware pipeline
            }

            await _next(context); // Proceed to the next middleware
        }
    }
}
