using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Services
{
    public class Http301RedirectionMiddlewareTests
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<Http301RedirectionMiddleware> _logger;

        public Http301RedirectionMiddlewareTests()
        {
            _next = A.Fake<RequestDelegate>();
            _logger = A.Fake<ILogger<Http301RedirectionMiddleware>>();
        }

        [Fact]
        public async Task InvokeAsync_ShouldProceedToNextMiddleware_WhenNoMatchingRule()
        {
            // Arrange
            var config = new Http301RedirectConfig
            {
                Triggers = new List<Http301RedirectTrigger>(),
                AppendReferrerOnQueryString = false
            };
            var middleware = new Http301RedirectionMiddleware(_next, config, _logger);

            var context = new DefaultHttpContext();
            context.Request.Scheme = "https";
            context.Request.Host = new HostString("example.com");
            context.Request.Path = "/no-match";

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            A.CallTo(() => _next(context)).MustHaveHappened();
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_ShouldRedirect_WhenMatchingRuleFound()
        {
            // Arrange
            var config = new Http301RedirectConfig
            {
                Triggers = new List<Http301RedirectTrigger>
                {
                    new Http301RedirectTrigger
                    {
                        Seq = 1,
                        Exp = "^.*example\\.com.*$",
                        Rules = new List<Http301RedirectRule>
                        {
                            new Http301RedirectRule
                            {
                                Seq = 1,
                                Exp = "^https?://example\\.com/match$",
                                SendTo = "https://example.org/new-path"
                            }
                        }
                    }
                }
            };
            var middleware = new Http301RedirectionMiddleware(_next, config, _logger);

            var context = new DefaultHttpContext();
            context.Request.Scheme = "https";
            context.Request.Host = new HostString("example.com");
            context.Request.Path = "/match";

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status301MovedPermanently, context.Response.StatusCode);
            Assert.Equal("https://example.org/new-path", context.Response.Headers["Location"]);
            A.CallTo(() => _next(context)).MustNotHaveHappened();
        }

        [Fact]
        public async Task InvokeAsync_ShouldRedirectAndAppendReferrer_WhenConfigured()
        {
            // Arrange
            var config = new Http301RedirectConfig
            {
                AppendReferrerOnQueryString = true,
                Triggers = new List<Http301RedirectTrigger>
                {
                    new Http301RedirectTrigger
                    {
                        Seq = 1,
                        Exp = "^.*example\\.com.*$",
                        Rules = new List<Http301RedirectRule>
                        {
                            new Http301RedirectRule
                            {
                                Seq = 1,
                                Exp = "^https?://example\\.com/match$",
                                SendTo = "https://example.org/new-path"
                            }
                        }
                    }
                }
            };
            var middleware = new Http301RedirectionMiddleware(_next, config, _logger);

            var context = new DefaultHttpContext();
            context.Request.Scheme = "https";
            context.Request.Host = new HostString("example.com");
            context.Request.Path = "/match";
            context.Request.Headers["Referer"] = "https://referrer.com/page";

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status301MovedPermanently, context.Response.StatusCode);
            Assert.Equal(
                "https://example.org/new-path?originalReferrer=https%3A%2F%2Freferrer.com%2Fpage",
                context.Response.Headers["Location"]
            );
            A.CallTo(() => _next(context)).MustNotHaveHappened();
        }

        [Fact]
        public void Constructor_ShouldInitializeAndPrecompileRegexPatterns()
        {
            // Arrange
            var triggerRegex = "^.*example\\.com.*$";
            var ruleRegex = "^https?://example\\.com/match$";

            var trigger = new Http301RedirectTrigger
            {
                Seq = 1,
                Exp = triggerRegex,
                Rules = new List<Http301RedirectRule>
                {
                    new Http301RedirectRule
                    {
                        Seq = 1,
                        Exp = ruleRegex,
                        SendTo = "https://example.org/new-path"
                    }
                }
            };

            var config = new Http301RedirectConfig
            {
                Triggers = new List<Http301RedirectTrigger> { trigger }
            };

            // Act
            var middleware = new Http301RedirectionMiddleware(_next, config, _logger);

            // Assert
            Assert.NotNull(trigger.CompiledExp);
            Assert.NotNull(trigger.Rules[0].CompiledExp);
        }

        [Fact]
        public async Task InvokeAsync_ShouldHandleRegexMatchTimeoutException()
        {
            // Arrange
            var catastrophicPattern = @"(a+)+$";
            var timeout = TimeSpan.FromMilliseconds(1); // Very short timeout

            // This input will cause catastrophic backtracking
            var longInput = new string('a', 10000) + "X";

            var regexWithTimeout = new Regex(catastrophicPattern, RegexOptions.None, timeout);

            var trigger = new Http301RedirectTrigger
            {
                Seq = 1,
                Exp = ".*",
                CompiledExp = regexWithTimeout,
                Rules = new List<Http301RedirectRule>
                {
                    new Http301RedirectRule
                    {
                        Seq = 1,
                        Exp = ".*",
                        CompiledExp = regexWithTimeout,
                        SendTo = "https://example.org/new-path"
                    }
                }
            };

            var config = new Http301RedirectConfig
            {
                Triggers = new List<Http301RedirectTrigger> { trigger },
                AppendReferrerOnQueryString = false
            };

            var middleware = new Http301RedirectionMiddleware(_next, config, _logger);

            var context = new DefaultHttpContext();
            context.Request.Scheme = "https";
            context.Request.Host = new HostString("example.com");
            context.Request.Path = "/" + longInput;

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            _logger.VerifyLogMustHaveHappened(
                Microsoft.Extensions.Logging.LogLevel.Warning,
                "Regex match timed out for pattern");

            A.CallTo(() => _next(context)).MustHaveHappened();
        }
             
        [Fact]
        public async Task InvokeAsync_ShouldUseCaptureGroupsInRedirectUrl()
        {
            // Arrange
            var config = new Http301RedirectConfig
            {
                Triggers = new List<Http301RedirectTrigger>
                {
                    new Http301RedirectTrigger
                    {
                        Seq = 1,
                        Exp = "^.*example\\.com.*$",
                        Rules = new List<Http301RedirectRule>
                        {
                            new Http301RedirectRule
                            {
                                Seq = 1,
                                Exp = "^https?://example\\.com/(match)/(\\d+)$",
                                SendTo = "https://example.org/$1?id=$2"
                            }
                        }
                    }
                }
            };
            var middleware = new Http301RedirectionMiddleware(_next, config, _logger);

            var context = new DefaultHttpContext();
            context.Request.Scheme = "https";
            context.Request.Host = new HostString("example.com");
            context.Request.Path = "/match/123";

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status301MovedPermanently, context.Response.StatusCode);
            Assert.Equal("https://example.org/match?id=123", context.Response.Headers["Location"]);
            A.CallTo(() => _next(context)).MustNotHaveHappened();
        }
    }
}
