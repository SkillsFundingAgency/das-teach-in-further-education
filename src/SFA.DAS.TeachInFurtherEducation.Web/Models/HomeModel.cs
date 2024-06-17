using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class HomeModel : LayoutModel
    {
        public IEnumerable<FilterSectionModel> Filters { get; }
        public Preamble? PagePreamble { get; }
        public Breadcrumbs? PageBreadcrumbs { get; }
        public bool EnsureSchemesAreVisible { get; }
        public string SelectedFilters { get; }

        public HomeModel(
            IEnumerable<FilterSectionModel> filters,
            IEnumerable<MenuItem> menuItems,
            Preamble? pagePreamble,
            Breadcrumbs? pageBreadcrumbs,
            BetaBanner? banner,
            IEnumerable<FooterLink>? footerLinks,
            bool ensureSchemesAreVisible = false,
            string selectedFilters = "")
        {
            PagePreamble = pagePreamble;
            PageBreadcrumbs = pageBreadcrumbs;
            Filters = filters;
            MenuItems = menuItems;
            EnsureSchemesAreVisible = ensureSchemesAreVisible;
            SelectedFilters = selectedFilters;
            BetaBanner = banner;
            base.footerLinks = footerLinks;
        }
    }
}