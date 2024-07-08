using Contentful.Core.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class ImageCardSection
    {
        public required string ImageCardSectionTitle { get; set; }

        public required string ImageCardHeading { get; set; }

        public string? ImageCardSectionHeading { get; set; }

        public Document? ImageCardSectionDescription { get; set; }

        public List<ImageCard> ImageCards { get; set; } = [];
    }

}
