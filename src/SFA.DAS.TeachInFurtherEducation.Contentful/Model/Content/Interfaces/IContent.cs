using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Collections.Generic;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces
{

    public interface IContent
    {

        IEnumerable<PageRenamed> Pagess { get; }

        /// <summary>
        /// Guaranteed in descending Size order
        /// </summary>

        IEnumerable<Page> Pages { get; }

        IEnumerable<MenuItem> MenuItems { get; }

        IEnumerable<FooterLink> FooterLinks { get; }

    }

}
