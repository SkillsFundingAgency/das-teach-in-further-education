using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class HeaderFullImage : BaseModel, IContent
    {
        public required string Title { get; set; }

        public required Asset HeaderImage { get; set; }

        public string? FirstLineTitle { get; set; }
        public string? SecondLineTitle { get; set; }
        public string? ThirdLineTitle { get; set; }
        public string? TitleBackgroundColour { get; set; }
    }
}
