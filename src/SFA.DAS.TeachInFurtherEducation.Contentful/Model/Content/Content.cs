using Microsoft.AspNetCore.Html;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content
{

    [ExcludeFromCodeCoverage]
    public class Content : IContent
    {
        public Content(

            IEnumerable<PageRenamed> pagesRenamed,

            IEnumerable<Page> Pages,

            IEnumerable<MenuItem> menuItems,

            IEnumerable<FooterLink> footerLinks
        )
        {

            this.Pages = Pages;

            MenuItems = menuItems;

            this.Pagess = pagesRenamed;

            FooterLinks = footerLinks;
        }

        public IEnumerable<Page> Pages { get; }

        public IEnumerable<MenuItem> MenuItems { get; }

        public IEnumerable<PageRenamed> Pagess { get; }

        public IEnumerable<FooterLink> FooterLinks { get; }
    }

}
