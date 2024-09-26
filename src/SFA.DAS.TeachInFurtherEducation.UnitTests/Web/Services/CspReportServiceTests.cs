// CspReportServiceTests.cs
using FakeItEasy;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Services
{
    public class CspReportServiceTests
    {
        private readonly ILogger<CspReportService> _logger;
        private readonly CspReportService _service;

        public CspReportServiceTests()
        {
            _logger = A.Fake<ILogger<CspReportService>>();
            _service = new CspReportService(_logger);
        }

        [Fact]
        public void LogReport_WithValidCspReport_ShouldLogError()
        {
            // Arrange
            var report = new CspViolationReport
            {
                CspReport = new CspReportDetails
                {
                    DocumentUri = "https://example.com/page",
                    ViolatedDirective = "script-src",
                    OriginalPolicy = "default-src 'self'; script-src 'self'"
                }
            };

            // Act
            _service.LogReport(report);

            // Assert
            _logger.VerifyLogMustHaveHappened(
                LogLevel.Error,
                "CSP Violation: https://example.com/page, script-src, default-src 'self'; script-src 'self'"
            );
        }

        [Fact]
        public void LogReport_WithNullCspReport_ShouldLogWarning()
        {
            // Arrange
            var report = new CspViolationReport
            {
                CspReport = null
            };

            // Act
            _service.LogReport(report);

            // Assert
            _logger.VerifyLogMustHaveHappened(
                LogLevel.Warning,
                "CSP Violation: Null CSP report submitted"
            );
        }
    }
}
