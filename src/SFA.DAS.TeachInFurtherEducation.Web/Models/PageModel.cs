using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class PageModel : LayoutModel
    {

        public PageRenamed Page { get; }

        public string? ViewName { get; }

        public PageModel(PageRenamed page, IEnumerable<MenuItem> menuItems, IEnumerable<FooterLink>? links, string? viewName = null)
        {

            Page = page;

            ViewName = viewName;

            MenuItems = menuItems;

            footerLinks = links;

        }

    }

}
