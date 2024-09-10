using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class ImageCardBanner : IContent
    {
        public Document? BannerContents { get; set; }

        public string? BannerText { get; set; }

        public string? BannerLinkText { get; set; }

        public string? BannerLinkSource { get; set; }
        public Asset? BannerImage { get; set; }
    }
}
