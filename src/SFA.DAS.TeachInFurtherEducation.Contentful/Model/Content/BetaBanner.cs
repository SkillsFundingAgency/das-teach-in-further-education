using Contentful.Core.Models;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content
{

    public class BetaBanner
    {

        public required string BetaBannerID { get; set; }

        public required string BetaBannerTitle { get; set; }

        public Document? BetaBannerContent { get; set; }

    }

}
