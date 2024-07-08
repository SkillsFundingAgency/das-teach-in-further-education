using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class Tile
    {

        public required string TileID { get; set; }

        public required string TileTitle { get; set; }

        public required string TileSource { get; set; }

        public required string TileHeading { get; set; }

        public Document? TileDescription { get; set; }

        public int? TileOrder { get; set; } = 0;

        public Asset? TileImage { get; set; }

    }

}
