using Contentful.Core;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces.Roots
{

    public interface IPageContentService
    {
        Task<IEnumerable<Page>> GetAllPages(IContentfulClient contentfulClient);

        Task<IEnumerable<MenuItem>> GetMenuItems(IContentfulClient contentfulClient);

        Task<IEnumerable<InterimPage>> GetInterimPages(IContentfulClient contentfulClient);

        Task<BetaBanner?> GetBetaBanner(IContentfulClient contentfulClient);

        Task<IEnumerable<FooterLink>> GetFooterLinks(IContentfulClient contentfulClient);

    }

}
