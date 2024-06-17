using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{

    [ExcludeFromCodeCoverage]
    public class LayoutModel
    {

        public PreviewModel Preview { get; set; }

        public IEnumerable<MenuItem> MenuItems { get; set; } = Enumerable.Empty<MenuItem>();

        public BetaBanner? BetaBanner { get; set; }

        public IEnumerable<FooterLink>? footerLinks { get; set; }

        public LayoutModel(IEnumerable<MenuItem> menuItems, BetaBanner? betaBanner, IEnumerable<FooterLink> interimFooterLinks)
        {

            Preview = PreviewModel.NotPreviewModel;

            MenuItems = menuItems.Any() ? menuItems : Enumerable.Empty<MenuItem>();

            BetaBanner = betaBanner;

            footerLinks = interimFooterLinks;

        }

        public LayoutModel()
        {

            Preview = PreviewModel.NotPreviewModel;

        }

    }

}
