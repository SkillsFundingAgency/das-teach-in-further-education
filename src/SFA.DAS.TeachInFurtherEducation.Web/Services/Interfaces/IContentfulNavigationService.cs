using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Web.Models;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces
{
    public interface IContentfulNavigationService
    {
        Task<IEnumerable<NavigationMenu>> GetMenusAsync();
        Task<NavigationMenu> GetMenuBySlugAsync(string slug);
        Task<IEnumerable<NavigationMenuItem>> GetMenuTreeAsync(string menuId);
    }
}
