using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class FullImageCardBanner : Common, IContent
    {
        public required string BannerTitle { get; set; }
        public required Document BannerContents { get; set; }

        public required Asset BannerImage { get; set; }
    }
}
