using Microsoft.Extensions.Primitives;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces
{
    internal interface INavigationMenuProvider
    {
        Task<IReadOnlyList<NavigationMenuTreeItem>> GetHeaderMenuItemsAsync(CancellationToken ct = default);
    }
}
