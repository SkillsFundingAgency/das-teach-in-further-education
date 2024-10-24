using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{
    [ExcludeFromCodeCoverage]
    public class Navigation : BaseModel, IContent
    {
        public string? PreviousText { get; init; }
        public string? PreviousTextSource { get; init; }
        public string? NextText { get; init; }
        public string? NextTextSource { get; init; }
    }
}