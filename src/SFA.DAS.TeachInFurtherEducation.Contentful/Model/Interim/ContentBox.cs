using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class ContentBox : BaseModel, IContent
    {
        public string? ContentTitle { get; set; }

        public Document? ContentBoxContents { get; set; }
        public string? BackgroundColor { get; set; }
    }

}
