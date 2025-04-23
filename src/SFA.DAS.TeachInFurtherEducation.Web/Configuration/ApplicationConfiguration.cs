using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ApplicationConfiguration
    {
        public string RedisConnectionString { get; set; } = string.Empty;
        public string DataProtectionKeysDatabase { get; set; } = string.Empty;
    }
}
