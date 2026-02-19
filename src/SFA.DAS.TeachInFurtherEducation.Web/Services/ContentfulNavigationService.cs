using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Web.Models;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services
{
    public class ContentNavigationService : IContentfulNavigationService
    {
        private readonly IContentfulClient _client;

        public ContentNavigationService(IContentfulClient client)
        {
            _client = client;
        }

        public async Task<NavigationMenu> GetMainNavigationAsync()
        {
            var builder = new QueryBuilder<dynamic>()
                .ContentTypeIs("navigationMenu")
                .Include(3); // Important for nested children

            var entries = await _client.GetEntries(builder);

            var menuEntry = entries.Items.FirstOrDefault();
            if (menuEntry == null)
                return new NavigationMenu();

            return MapNavigationMenu(menuEntry);
        }

        private NavigationMenu MapNavigationMenu(dynamic entry)
        {
            var menu = new NavigationMenu
            {
                Title = entry.title
            };

            foreach (var item in entry.menuItems)
            {
                menu.Items.Add(MapMenuItem(item));
            }

            return menu;
        }

        private NavigationMenuItem MapMenuItem(dynamic entry)
        {
            var menuItem = new NavigationMenuItem
            {
                Id = entry.sys.id,
                Title = entry.title,
                Url = entry.url,
                OpenInNewTab = entry.openInNewTab ?? false
            };

            if (entry.children != null)
            {
                foreach (var child in entry.children)
                {
                    menuItem.Children.Add(MapMenuItem(child));
                }
            }

            return menuItem;
        }
    }
}