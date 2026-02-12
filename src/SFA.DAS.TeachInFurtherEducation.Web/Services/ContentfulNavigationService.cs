using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System.Collections.Generic;

// namespace SFA.DAS.TeachInFurtherEducation.Web.Services
// {
    // TODOS: 
    // Fetch menu structure from Contentful
    // Build hierarchical tree from flat entries
    // Cache results?
    // public class ContentfulNavigationService : IContentfulNavigationService
    // {}

// TODO: Tree Building Logic:
//     private List<NavigationMenuItem> BuildMenuTree(List<NavigationMenuItem> allItems, string parentId = null)
//     {
//         return allItems
//             .Where(item => item.ParentItem?.Sys?.Id == parentId)
//             .OrderBy(item => item.SortOrder)
//             .Select(item => new NavigationMenuItem
//             {
//                 ...item, Children = BuildMenuTree(allItems, item.Id)
//             })
//             .ToList();
//     }
// }
