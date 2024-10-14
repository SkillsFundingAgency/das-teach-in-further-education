using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class SqlDbContextConfiguration
    {
        public string SqlConnectionString { get; set; } = "";
        public string ServerName { get; set; } = "";
        public string DatabaseName { get; set; } = "";
    }
}
