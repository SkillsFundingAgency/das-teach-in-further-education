using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services
{
    public class CspReportService : ICspReportService
    {
        private readonly ILogger<CspReportService> _logger;

        public CspReportService(ILogger<CspReportService> logger)
            {
            _logger = logger;
        }

        public void LogReport(CspViolationReport report)
        {
            if (report.CspReport != null)
            {
                _logger.LogError("CSP Violation: {DocumentUri}, {ViolatedDirective}, {OriginalPolicy}",
                report.CspReport!.DocumentUri, report.CspReport!.ViolatedDirective, report.CspReport!?.OriginalPolicy);
            }
            else
            {
                _logger.LogWarning("CSP Violation: Null CSP report submitted");
            }
            
        }
    }
}
