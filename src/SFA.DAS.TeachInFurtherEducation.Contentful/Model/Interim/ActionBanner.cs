using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class ActionBanner : BaseModel, IContent
    {
        public required string BannerText { get; set; }

        public required string BannerLinkText { get; set; }

        public required string? BannerLinkSource { get; set; }
    }
}
