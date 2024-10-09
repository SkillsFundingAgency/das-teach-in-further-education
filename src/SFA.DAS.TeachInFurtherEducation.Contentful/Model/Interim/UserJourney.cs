using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class UserJourney : BaseModel, IContent
    {
        public required string Name { get; set; }
        public required string CareerStageOne { get; set; }
        public required string CareerStageTwo { get; set; }
        public required string CareerStageThree { get; set; }
        public required string UserJourneyDescription { get; set; }
        public required string ArticleLinkText { get; set; }

        public required string ArticleLinkSource { get; set; }
        public required string ArticleReadTimeInMinutes { get; set; }
        public required Asset ProfilePhoto { get; set; }
    }
}
