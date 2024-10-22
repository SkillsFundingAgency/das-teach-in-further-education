using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class Navigation : BaseModel, IContent
    {
        public required string PreviousText { get; set; }

        public required string PreviousTextSource { get; set; }

        public required string? NextText { get; set; }
        public string? NextTextSource { get; set; }
    }
}
