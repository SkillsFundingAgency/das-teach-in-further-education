using System.Collections.Generic;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces
{

    public interface IContentModelService
    {

        PageContentModel? GetPageContentModel(string pageUrl, bool isLandingPage = false);

        Task<PageContentModel?> GetPagePreviewModel(string pageUrl, bool isLandingPage = false);
        IEnumerable<MenuItem> GetMenuItems(ref IEnumerable<MenuItem> menuItems, string pageUrl, bool isLandingPage = false);
        Breadcrumbs GetBreadcrumbs(ref Breadcrumbs breadcrumbs, bool isLandingPage = false);
    }

}
