using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services
{
    [ExcludeFromCodeCoverage]
    internal sealed class NavigationMenuProvider : INavigationMenuProvider
    {
        private readonly ContentfulNavigationOptions _options;
        private readonly IContentfulNavigationService _contentfulNav;
        private readonly DropdownMenuService _legacyNav;
        private readonly ILogger<NavigationMenuProvider> _logger;
        private readonly IHttpContextAccessor _http;

        public NavigationMenuProvider(
            IOptions<ContentfulNavigationOptions> options,
            IContentfulNavigationService contentfulNav,
            DropdownMenuService legacyNav,
            ILogger<NavigationMenuProvider> logger,
            IHttpContextAccessor http
            )
        {
            _options = options.Value;
            _contentfulNav = contentfulNav;
            _legacyNav = legacyNav;
            _logger = logger;
            _http = http;
        }

        public async Task<IReadOnlyList<NavigationMenuTreeItem>> GetHeaderMenuItemsAsync(CancellationToken ct = default)
        {
            if (!_options.Enabled)
                return GetLegacyMenuItems();

            if (string.IsNullOrWhiteSpace(_options.HeaderMenuTitle))
            {
                _logger.LogWarning("Navigation Header Menu Title is not configured. Falling back.");
                return GetLegacyMenuItems();
            }

            try
            {
                var menu = await GetContentfulMenuAsync(_options.HeaderMenuTitle, ct);

                if (menu?.Items is { Count: > 0 })
                    return menu.Items;

                _logger.LogWarning("Contentful navigation returned empty menu for {NavigationHeaderMenuTitle}. Falling back.", _options.HeaderMenuTitle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Conetntful navigation failed for {NavigationHeaderMenuTitle}", _options.HeaderMenuTitle);
            }

            return GetLegacyMenuItems();
        }

        private async Task<NavigationMenuTree?> GetContentfulMenuAsync(string title, CancellationToken ct)
        {
            return IsPreviewRequest()
                ? await _contentfulNav.GetPreviewMenuTreeByTitleAsync(title, ct)
                : await _contentfulNav.GetMenuTreeByTitleAsync(title, ct);
        }

        private bool IsPreviewRequest()
        {
            var path = _http.HttpContext?.Request?.Path.Value;
            return path?.StartsWith("/preview", StringComparison.OrdinalIgnoreCase) == true;
        }

        private IReadOnlyList<NavigationMenuTreeItem> GetLegacyMenuItems()
        {
            var legacyItems = _legacyNav.GetDropdownMenuItems();
            if (legacyItems is not { Count: > 0 })
            {
                _logger.LogWarning("Legacy menu source returned no items.");
                return Array.Empty<NavigationMenuTreeItem>();
            }

            return legacyItems.Select(MapLegacy).ToList();
        }

        private static NavigationMenuTreeItem MapLegacy(DropdownMenuItem item)
        {
            return new NavigationMenuTreeItem
            {
                Id = CreateLegacyMenuItemId(item),
                Title = item.Title ?? string.Empty,
                Url = item.Url ?? string.Empty,
                OpenInNewTab = false,
                SortOrder = 0,
                Children = item.Children?.Select(MapLegacy).ToList() ?? []
            };
        }

        private static string CreateLegacyMenuItemId(DropdownMenuItem item)
        {
            var source = item.Title ?? item.Url ?? "item";
            return source.Trim().Replace(" ", "", StringComparison.Ordinal);
        }
    }
}