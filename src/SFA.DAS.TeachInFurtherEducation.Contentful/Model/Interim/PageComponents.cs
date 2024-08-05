using Contentful.Core.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class PageComponents : IContent
    {
        public Preamble? PagePreamble { get; set; }

        public List<TileSection> TileSections { get; set; } = [];
    }

}
