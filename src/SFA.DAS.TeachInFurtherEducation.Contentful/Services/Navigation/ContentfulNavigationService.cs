using Contentful.Core;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.TeachInFurtherEducation.Contentful.Exceptions;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Navigation;
using SFA.DAS.TeachInFurtherEducation.Contentful.Options;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services.Navigation
{
    [ExcludeFromCodeCoverage]
    internal sealed class ContentfulNavigationService : IContentfulNavigationService
    {
        private readonly ContentfulNavigationCacheOptions _cacheOptions;
        private readonly IContentfulClient? _client;
        private readonly IContentfulClient? _previewClient;
        private readonly INavigationTreeBuilder _treeBuilder;
        private readonly IDistributedCache _cache;
        private readonly ILogger<ContentfulNavigationService> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public ContentfulNavigationService(
            IOptions<ContentfulNavigationCacheOptions> cacheOptions,
            IContentfulClientFactory client,
            INavigationTreeBuilder treeBuilder,
            IDistributedCache cache,
            ILogger<ContentfulNavigationService> logger)
        {
            _cacheOptions = cacheOptions.Value;
            _client = client.ContentfulClient;
            _previewClient = client.PreviewContentfulClient;
            _treeBuilder = treeBuilder;
            _cache = cache;
            _logger = logger;
        }

        public async Task<NavigationMenuTree?> GetMenuTreeByTitleAsync(string navigationMenuTitle, CancellationToken ct = default)
        {

            var navigationCacheKey = $"navigation:{navigationMenuTitle}";

            var cachedNavigation = await TryGetNavigationFromCacheAsync(navigationCacheKey, ct);
            if (cachedNavigation is not null)
                return cachedNavigation;

            if (_client is null)
                throw new ContentServiceException("Can't update content without a ContentfulClient.");
             

            var navigationMenu = await FetchMenuByTitleAsync(_client, navigationMenuTitle, ct);
            if (navigationMenu is null)
                return null;

            var result = BuildNavigationMenuTree(navigationMenuTitle, navigationMenu);

            await TrySetCacheWithNavigationAsync(navigationCacheKey, result, ct);

            return result;
        }
        private async Task<NavigationMenuTree?> TryGetNavigationFromCacheAsync(string cacheKey, CancellationToken ct = default)
        {
            try
            {
                var cachedJson = await _cache.GetStringAsync(cacheKey, ct);
                if (cachedJson is null)
                    return null;

                return JsonSerializer.Deserialize<NavigationMenuTree>(cachedJson, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Navigation cache read failed for key {CacheKey}", cacheKey);
                return null;
            }
        }

        private async Task<NavigationMenu?> FetchMenuByTitleAsync(IContentfulClient client, string menuTitle, CancellationToken ct)
        {
            var qb = QueryBuilder<NavigationMenu>.New
                .ContentTypeIs("navigationMenu")
                .FieldEquals("fields.title", menuTitle)
                .Include(10)
                .Limit(1);

            var result = await client.GetEntries(qb, ct);
            return result.Items.FirstOrDefault();
        }

        private NavigationMenuTree BuildNavigationMenuTree(string menuTitle, NavigationMenu menu)
        {
            var menuTree = _treeBuilder.BuildNavigationTree(menu.MenuItems);

            return new NavigationMenuTree
            {
                MenuTitle = menuTitle,
                Items = menuTree
            };
        }

        private async Task TrySetCacheWithNavigationAsync(string cacheKey, NavigationMenuTree value, CancellationToken ct = default)
        {
            TimeSpan navigationMenuCacheTTL = TimeSpan.FromMinutes(_cacheOptions.HeaderMenuCacheTTLMinutes);

            try
            {
                var json = JsonSerializer.Serialize(value, JsonOptions);

                await _cache.SetStringAsync(
                    cacheKey,
                    json,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = navigationMenuCacheTTL
                    },
                    ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Navigation cache write failed for key: {CacheKey}", cacheKey);
            }
        }

        public async Task<NavigationMenuTree?> GetPreviewMenuTreeByTitleAsync(string navigationMenuTitle, CancellationToken ct = default)
        {
            if (_previewClient is null)
                throw new ContentServiceException("Can't update preview content without a PreviewContentfulClient.");

            var navigationMenu = await FetchMenuByTitleAsync(_previewClient, navigationMenuTitle, ct);
            if (navigationMenu is null)
                return null;

            var result = BuildNavigationMenuTree(navigationMenuTitle, navigationMenu);

            return result;
        }
    }
}
