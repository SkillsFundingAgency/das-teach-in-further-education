using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces
{
    public interface IPageService
    {
        PageModel? GetPageModel(string pageUrl);
        PageModel? GetCookiePageModel(IContent content, bool showMessage);
        Task<PageModel?> GetPageModelPreview(string pageUrl);
        (string? routeName, object? routeValues) RedirectPreview(string pageUrl);
    }
}
