using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{
    [ExcludeFromCodeCoverage]
    public class Preamble : IContent
    {
        public string? PreambleTitle { get; set; }

        public string? BannerHeader { get; set; }

        public Document? PrimarySection { get; set; }

        public Document? SecondarySection { get; set; }
        public string? BackgroundColor { get; set; }
        public string? SecondarySectionBackgroundColour { get; set; }
        public string? SecondarySectionBorderColour { get; set; }
    }
}