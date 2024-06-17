using Contentful.Core.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class TileSection
    {

        public required string InterimTileSectionTitle { get; set; }

        public string? InterimTileSectionHeading { get; set; }

        public Document? InterimTileSectionDescription { get; set; }

        public List<Tile> InterimTiles { get; set; } = [];

    }

}
