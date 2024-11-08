using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{
    [ExcludeFromCodeCoverage]
    public class LineSeparator : BaseModel, IContent
    {
        public required string Title { get; set; }
        public string? TopMargin { get; set; }
        public string? BottomMargin { get; set; }
    }
}