using Contentful.Core.Models;
using Microsoft.AspNetCore.Html;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class RichTextContents : IContent
    {
        public string? Title { get; set; }

        public required Document PageContents { get; set; }

    }
}
