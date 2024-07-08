using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class ImageCardBanner
    {
        public Document? BannerContents { get; set; }

        public Asset? BannerImage { get; set; }
    }
}
