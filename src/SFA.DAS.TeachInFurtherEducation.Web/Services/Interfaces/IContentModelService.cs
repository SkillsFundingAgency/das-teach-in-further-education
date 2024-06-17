using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces
{

    public interface IContentModelService
    {

        PageContentModel? GetPageContentModel(string pageURL);

        Task<PageContentModel?> GetPagePreviewModel(string pageURL);

    }

}
