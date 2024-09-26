using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Web.Controllers;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System.Collections.Generic;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Controllers
{
    public class CspReportControllerTests
    {
        [Fact]
        public void Post_ValidRefererHeader_ShouldProcessReportAndReturnOk()
        {
            // Arrange
            var fakeLogger = A.Fake<ILogger<CspReportController>>();
            var fakeCspReportService = A.Fake<ICspReportService>();

            var controller = new CspReportController(fakeCspReportService, fakeLogger);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("example.com");
            httpContext.Request.Headers["Referer"] = "https://example.com/page";

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var report = new CspViolationReport
            {
                CspReport = new CspReportDetails
                {
                    DocumentUri = "https://example.com/page",
                    Referrer = "https://example.com/referrer",
                    ViolatedDirective = "script-src",
                    EffectiveDirective = "script-src",
                    OriginalPolicy = "default-src 'self'; script-src 'self'",
                    Disposition = "enforce",
                    BlockedUri = "https://malicious.com/script.js",
                    StatusCode = 200,
                    ScriptSample = "alert('XSS');"
                }
            };

            // Act
            var result = controller.Post(report);

            // Assert
            Assert.IsType<OkResult>(result);
            A.CallTo(() => fakeCspReportService.LogReport(report)).MustHaveHappenedOnceExactly();
            fakeLogger.VerifyLogMustHaveHappened(
                LogLevel.Information,
                "CSP Violation Report Received"
            );
        }

        [Fact]
        public void Post_ValidOriginHeader_ShouldProcessReportAndReturnOk()
        {
            // Arrange
            var fakeLogger = A.Fake<ILogger<CspReportController>>();
            var fakeCspReportService = A.Fake<ICspReportService>();

            var controller = new CspReportController(fakeCspReportService, fakeLogger);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("example.com");
            httpContext.Request.Headers["Origin"] = "https://example.com";

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var report = new CspViolationReport
            {
                CspReport = new CspReportDetails
                {
                    DocumentUri = "https://example.com/page",
                    Referrer = "https://example.com/referrer",
                    ViolatedDirective = "script-src",
                    EffectiveDirective = "script-src",
                    OriginalPolicy = "default-src 'self'; script-src 'self'",
                    Disposition = "enforce",
                    BlockedUri = "https://malicious.com/script.js",
                    StatusCode = 200,
                    ScriptSample = "alert('XSS');"
                }
            };

            // Act
            var result = controller.Post(report);

            // Assert
            Assert.IsType<OkResult>(result);
            A.CallTo(() => fakeCspReportService.LogReport(report)).MustHaveHappenedOnceExactly();
            fakeLogger.VerifyLogMustHaveHappened(
                LogLevel.Information,
                "CSP Violation Report Received"
            );
        }

        [Fact]
        public void Post_InvalidHeaders_ShouldReturnUnauthorized()
        {
            // Arrange
            var fakeLogger = A.Fake<ILogger<CspReportController>>();
            var fakeCspReportService = A.Fake<ICspReportService>();

            var controller = new CspReportController(fakeCspReportService, fakeLogger);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("example.com");
            // Set invalid Referer and Origin headers that do not start with the current host
            httpContext.Request.Headers["Referer"] = "https://malicious.com/page";
            httpContext.Request.Headers["Origin"] = "https://malicious.com";

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var report = new CspViolationReport
            {
                CspReport = new CspReportDetails
                {
                    DocumentUri = "https://example.com/page",
                    Referrer = "https://malicious.com/referrer",
                    ViolatedDirective = "script-src",
                    EffectiveDirective = "script-src",
                    OriginalPolicy = "default-src 'self'; script-src 'self'",
                    Disposition = "enforce",
                    BlockedUri = "https://malicious.com/script.js",
                    StatusCode = 200,
                    ScriptSample = "alert('XSS');"
                }
            };

            // Act
            var result = controller.Post(report);

            // Assert
            // Verify that the response is Unauthorized
            Assert.IsType<UnauthorizedResult>(result);

            // Verify that LogReport was never called
            A.CallTo(() => fakeCspReportService.LogReport(A<CspViolationReport>._)).MustNotHaveHappened();

            // Verify that the "CSP Violation Report Received" log was created
            fakeLogger.VerifyLogMustHaveHappened(
                LogLevel.Information,
                "CSP Violation Report Received"
            );
        }
    }
}
