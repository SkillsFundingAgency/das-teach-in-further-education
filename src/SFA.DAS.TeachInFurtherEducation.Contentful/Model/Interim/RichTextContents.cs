using Contentful.Core.Models;
using Microsoft.AspNetCore.Html;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class RichTextContents : BaseModel, IContent
    {
        public string? Title { get; set; }

        public required Document PageContents { get; set; }

        public string? BackgroundColor { get; set; }
    }
}
