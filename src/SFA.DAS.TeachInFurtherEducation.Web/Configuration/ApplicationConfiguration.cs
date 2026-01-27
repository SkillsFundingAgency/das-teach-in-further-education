using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ApplicationConfiguration
    {
        public string DataProtectionKeysDatabase { get; set; } = string.Empty;
    }
}
