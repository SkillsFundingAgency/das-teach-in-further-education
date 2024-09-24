using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class FormOptionsConfig
    {
        public long MaxRequestBodySize { get; set; } = 4194304; // Default to 4 MB
    }

}
