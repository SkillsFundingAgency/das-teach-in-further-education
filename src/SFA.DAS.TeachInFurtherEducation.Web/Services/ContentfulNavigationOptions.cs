using Microsoft.Identity.Client;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services
{
    [ExcludeFromCodeCoverage]
    public sealed class ContentfulNavigationOptions
    {
        public required bool Enabled { get; set; }
        public required string HeaderMenuTitle { get; set; }
        public required int HeaderMenuCacheTTLMinutes { get; set; }
    }
}
