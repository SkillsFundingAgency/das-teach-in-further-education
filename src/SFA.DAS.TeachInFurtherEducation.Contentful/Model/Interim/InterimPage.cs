using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class InterimPage
    {

        public required string InterimPageTitle { get; set; }

        public required string InterimPageURL { get; set; }

        public Preamble? InterimPagePreamble { get; set; }

        public Breadcrumbs? InterimPageBreadcrumbs { get; set; }

        public List<PageComponent> InterimPageComponents { get; set; } = [];

        public List<TileSection> InterimPageTileSections { get; set; } = [];

    }

}
