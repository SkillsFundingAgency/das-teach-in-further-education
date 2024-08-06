using Contentful.Core.Models;
using Microsoft.AspNetCore.Html;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class PageContentModel : LayoutModel
    {

        public required string PageURL { get; set; }

        public required string PageTitle { get; set; }

        public required string PageTemplate { get; set; }

        public  Preamble? PagePreamble { get; set; }

        public List<TileSection> TileSections { get; set; } = [];

        public List<ContentBox> ContentBoxSection { get; set; } = [];

        public HtmlString? Contents { get; set; }

        public ImageCardBanner? ImageCardBanner { get; set; }

        public NewsLetter? NewsLetter { get; set; }

        public ContactUs? ContactUs { get; set; }

        public Video? Video { get; set; }

        public List<IContent>? PageComponents { get; set; }

    }

}
