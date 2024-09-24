using SFA.DAS.TeachInFurtherEducation.Web.Models;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces
{
    public interface ICspReportService
    {
        void LogReport(CspViolationReport report);
    }
}
