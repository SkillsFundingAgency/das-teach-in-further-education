using Microsoft.AspNetCore.Html;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Collections.Generic;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{

    public class PageContentModel : LayoutModel
    {

        public required string PageURL { get; set; }

        public required string PageTitle { get; set; }

        public  Preamble? PagePreamble { get; set; }

        public List<TileSection> TileSections { get; set; } = [];

        public HtmlString? Contents { get; set; }

    }

}
