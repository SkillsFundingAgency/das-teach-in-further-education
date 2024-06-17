using SFA.DAS.TeachInFurtherEducation.Contentful.Model;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Collections.Generic;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{

    public class InterimPageModel : LayoutModel
    {

        public required string InterimPageTitle { get; set; }

        public required string InterimPageURL { get; set; }

        public Preamble? InterimPagePreamble { get; set; }

        public Breadcrumbs? InterimPageBreadcrumbs { get; set; }

        public List<PageComponent> InterimPageComponents { get; set; } = [];

        public List<TileSection> InterimPageTileSections { get; set; } = [];

    }

}
