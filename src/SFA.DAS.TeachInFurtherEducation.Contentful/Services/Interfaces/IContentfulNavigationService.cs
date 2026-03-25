using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Navigation;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces
{
    public interface IContentfulNavigationService
    {
        Task<NavigationMenuTree?> GetMenuTreeByTitleAsync(string navigationMenuTitle, CancellationToken ct =  default);
        Task<NavigationMenuTree?> GetPreviewMenuTreeByTitleAsync(string navigationMenuTitle, CancellationToken ct = default);
    }
}