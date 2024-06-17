using Contentful.Core.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model
{

    [ExcludeFromCodeCoverage]
    public class PageComponent
    {

        public string? ComponentID { get; set; }

        public string? ComponentTitle { get; set; }

        public string? ComponentHeading { get; set; }

        public string? ComponentSubHeading { get; set; }

        public string? ComponentType { get; set; }

        public Document? ComponentContent { get; set; }

        public string? ComponentBackgroundColor { get; set; }

        public string? ComponentBorderColor { get; set; }

        public int? ComponentOrder { get; set; } = 0;

        public string? ComponentVideoSource { get; set; }

        public string? ComponentVideoDescription { get; set; }

        public Document? ComponentVideoTranscript { get; set; }

        public List<PageComponent> SubComponents { get; set; } = new List<PageComponent>();

    }

}
