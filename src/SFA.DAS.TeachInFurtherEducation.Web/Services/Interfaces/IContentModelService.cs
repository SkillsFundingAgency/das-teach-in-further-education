using System.Collections.Generic;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces
{

    public interface IContentModelService
    {

        PageContentModel? GetPageContentModel(string pageUrl);

        Task<PageContentModel?> GetPagePreviewModel(string pageUrl);
        IEnumerable<MenuItem> GetMenuItems(ref IEnumerable<MenuItem> menuItems, string pageUrl);

    }

}
