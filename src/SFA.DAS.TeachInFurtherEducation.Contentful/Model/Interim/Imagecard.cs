using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class ImageCard
    {
        public required string ImagecardTitle { get; set; }

        public Document? ImageCardDescription { get; set; }

        public int? ImageCardOrder { get; set; } = 0;

        public Asset? CardImage { get; set; }
    }
}
