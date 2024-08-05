using Contentful.Core.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class TileSection : IContent
    {

        public required string TileSectionTitle { get; set; }

        public string? TileSectionHeading { get; set; }

        public Document? TileSectionDescription { get; set; }

        public List<Tile> Tiles { get; set; } = [];

    }

}
