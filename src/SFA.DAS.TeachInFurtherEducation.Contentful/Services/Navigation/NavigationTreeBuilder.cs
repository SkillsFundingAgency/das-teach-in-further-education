using Microsoft.AspNetCore.Mvc.Razor.Internal;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Navigation;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services.Navigation
{
    [ExcludeFromCodeCoverage]
    internal sealed class NavigationTreeBuilder : INavigationTreeBuilder
    {
        public IReadOnlyList<NavigationMenuTreeItem> BuildNavigationTree(IReadOnlyList<NavigationMenuItem> items)
        {
            var validItems = items
                .Where(item => !string.IsNullOrWhiteSpace(item.Sys?.Id))
                .ToList();

            var menuItemsbyId = CreateMenuItemsById(validItems);
            var topLevelItems = AttachChildrenToParents(validItems, menuItemsbyId);

            RemoveDisabledItems(topLevelItems);
            SortMenuItems(topLevelItems);

            return topLevelItems;
        }

        private static Dictionary<string, NavigationMenuTreeItem> CreateMenuItemsById(IReadOnlyList<NavigationMenuItem> items)
        {
            var menuItemsById = new Dictionary<string, NavigationMenuTreeItem>(StringComparer.Ordinal);

            foreach (var item in items)
            {
                var id = item.Sys!.Id;

                menuItemsById.TryAdd(id, new NavigationMenuTreeItem
                {
                    Id = id,
                    Title = item.Title ?? string.Empty,
                    Url = NormaliseUrl(item.Url),
                    OpenInNewTab = item.OpenInNewTab,
                    SortOrder = item.SortOrder,
                    Enabled = item.Enabled,
                    Children = new List<NavigationMenuTreeItem>()
                });
            }

            return menuItemsById;
        }

        private static List<NavigationMenuTreeItem> AttachChildrenToParents(
            IReadOnlyList<NavigationMenuItem> items,
            IReadOnlyDictionary<string, NavigationMenuTreeItem> menuItemsById)
        {
            var topLevelItems = new List<NavigationMenuTreeItem>();
            var addedTopLevelIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var item in items)
            {
                var id = item.Sys!.Id;
                var currentItem = menuItemsById[id];
                var parentId = item.ParentItem?.Sys?.Id;

                if (TryGetParentItem(parentId, id, menuItemsById, out var parentItem))
                {
                    parentItem!.Children.Add(currentItem);
                    continue;
                }

                if (addedTopLevelIds.Add(id))
                    topLevelItems.Add(currentItem);
            }

            return topLevelItems;
        }

        private static bool TryGetParentItem(
            string? parentId,
            string currentItemId,
            IReadOnlyDictionary<string, NavigationMenuTreeItem> menuItemsById,
            out NavigationMenuTreeItem? parentItem)
        {
            parentItem = null;

            if (string.IsNullOrWhiteSpace(parentId))
                return false;

            if (string.Equals(parentId, currentItemId, StringComparison.Ordinal))
                return false;

            return menuItemsById.TryGetValue(parentId, out parentItem);
        }

        private static void RemoveDisabledItems(List<NavigationMenuTreeItem> items)
        {
            for (var i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];

                if (!item.Enabled)
                {
                    items.RemoveAt(i);
                    continue;
                }

                if (item.Children.Count > 0)
                    RemoveDisabledItems(item.Children);
            }
        }

        private static void SortMenuItems(List<NavigationMenuTreeItem> items)
        {
            items.Sort(CompareMenuItems);

            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Children.Count > 0)
                    SortMenuItems(items[i].Children);
            }
        }

        private static int CompareMenuItems(NavigationMenuTreeItem a,  NavigationMenuTreeItem b)
        {
            var sortOrderComparison = a.SortOrder.CompareTo(b.SortOrder);

            return sortOrderComparison != 0
                ? sortOrderComparison
                : string.Compare(a.Title, b.Title, StringComparison.OrdinalIgnoreCase);
        }
                                                                                                                             
        private static string NormaliseUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return "/";

            if (Uri.TryCreate(url, UriKind.Absolute, out _))
                return url;

            return url.StartsWith('/') ? url : "/" + url;
        }

    }
}
