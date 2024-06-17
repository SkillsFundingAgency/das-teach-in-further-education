using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Interfaces
{

    public interface IViewRenderService
    {

        Task<string> RenderToStringAsync<TModel>(string viewName, TModel model);

    }

}
