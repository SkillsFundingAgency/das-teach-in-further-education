using Contentful.Core;
using Contentful.Core.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Exceptions;
using SFA.DAS.TeachInFurtherEducation.Contentful.GdsHtmlRenderers;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces.Roots;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IContent = SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces.IContent;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services
{
    public class ContentService : IContentService
    {
        private readonly IContentfulClient? _contentfulClient;
        private readonly IContentfulClient? _previewContentfulClient;
        private readonly IPageService _pageService;
        private readonly IPageContentService _pageContentService;
        private readonly IAssetDownloader _assetDownloader;
        private readonly ILogger<ContentService> _logger;

        public event EventHandler<EventArgs>? ContentUpdated;
        public event EventHandler<EventArgs>? PreviewContentUpdated;

        public ContentService(
            IContentfulClientFactory contentfulClientFactory,
            IPageService pageService,
            IPageContentService interimService,
            IAssetDownloader assetDownloader, 
            ILogger<ContentService> logger)
        {
            _contentfulClient = contentfulClientFactory.ContentfulClient;
            _previewContentfulClient = contentfulClientFactory.PreviewContentfulClient;
            _pageService = pageService;
            _pageContentService = interimService;
            _assetDownloader = assetDownloader;
            _logger = logger;
        }

        public static readonly IContent GeneratedContent = new GeneratedContent();

        public IContent Content { get; private set; } = GeneratedContent;
        public IContent? PreviewContent { get; private set; }

        public async Task<IContent> Update()
        {
            _logger.LogInformation("Updating content");

            if (_contentfulClient == null)
                throw new ContentServiceException("Can't update content without a ContentfulClient.");

            var content = await Update(_contentfulClient);
            Content = content;

            _logger.LogInformation("Publishing ContentUpdated event");
            ContentUpdated?.Invoke(this, EventArgs.Empty);

            return content;
        }

        public async Task<IContent> UpdatePreview()
        {
            _logger.LogInformation("Updating preview content");

            if (_previewContentfulClient == null)
                throw new ContentServiceException("Can't update preview content without a preview ContentfulClient.");

            IContent previewContent = await Update(_previewContentfulClient);
            PreviewContent = previewContent;

            _logger.LogInformation("Publishing PreviewContentUpdated event");
            PreviewContentUpdated?.Invoke(this, EventArgs.Empty);
            return previewContent;
        }

        private async Task<IContent> Update(IContentfulClient contentfulClient)
        {

            var pages = await _pageContentService.GetAllPages(contentfulClient);

            var menuItems = await _pageContentService.GetMenuItems(contentfulClient);

            var footerLinks = await _pageContentService.GetFooterLinks(contentfulClient);

            return new Model.Content.Content(

                await _pageService.GetAll(contentfulClient),

                pages,

                menuItems,

                footerLinks

            );

        }

        public static HtmlRenderer CreateHtmlRenderer()
        {
            var htmlRendererOptions = new HtmlRendererOptions
            {
                ListItemOptions =
                {
                    OmitParagraphTagsInsideListItems = true
                }
            };
            var htmlRenderer = new HtmlRenderer(htmlRendererOptions);
            htmlRenderer.AddRenderer(new EmbeddedYoutubeContentRenderer());
            htmlRenderer.AddRenderer(new CtaContentRenderer(htmlRenderer.Renderers));
            htmlRenderer.AddRenderer(new GdsHtmlRenderers.HeadingRenderer(htmlRenderer.Renderers));
            htmlRenderer.AddRenderer(new GdsHtmlRenderers.HorizontalRulerContentRenderer());
            htmlRenderer.AddRenderer(new GdsHtmlRenderers.HyperlinkContentRenderer(htmlRenderer.Renderers));
            htmlRenderer.AddRenderer(new GdsHtmlRenderers.ListContentRenderer(htmlRenderer.Renderers));
            htmlRenderer.AddRenderer(new GdsParagraphRenderer(htmlRenderer.Renderers));
            htmlRenderer.AddRenderer(new BlockQuoteRenderer(htmlRenderer.Renderers));
            htmlRenderer.AddRenderer(new GdsHtmlRenderers.TableCellRenderer(htmlRenderer.Renderers));
            htmlRenderer.AddRenderer(new GdsHtmlRenderers.TableHeaderRenderer(htmlRenderer.Renderers));
            htmlRenderer.AddRenderer(new GdsHtmlRenderers.TableRenderer(htmlRenderer.Renderers));

            return htmlRenderer;
        }

        /// <summary>
        /// Retrieves a page from the content by its URL.
        /// </summary>
        /// <param name="url">The URL of the interim page to retrieve.</param>
        /// <returns>Returns the interim page with the specified URL if found, otherwise returns null.</returns>
        public Page? GetPageByURL(string url)
        {

            try
            {

                return this.Content.Pages.FirstOrDefault(a => a.PageURL == url);

            }
            catch (Exception _exception)
            {

                _logger.LogError(_exception, "Unable to get page by url: {URL}", url);

                return null;

            }

        }

        /// <summary>
        /// Retrieves an interim page from the preview content by its URL.
        /// </summary>
        /// <param name="url">The URL of the interim page to retrieve.</param>
        /// <returns>Returns the interim page with the specified URL if found, otherwise returns null.</returns>
        public Page? GetPreviewPageByURL(string url)
        {
            return PreviewContent?.Pages.FirstOrDefault(a => a.PageURL == url);
        }

        /// <summary>
        /// Retrieves an asset by its tags along with metadata.
        /// </summary>
        /// <param name="tasg">The tags associated with the asset to retrieve.</param>
        /// <returns>An collection of assets with content and metadata where those assets have been tagged with one or more of the specified strings.</returns>
        public async Task<List<Asset<byte[]>>> GetAssetsByTags(params string[] tags)
        {
            if (tags == null || tags.Length == 0)
            {
                throw new ArgumentException("At least one tag must be provided.", nameof(tags));
            }

            var assets = await GetAssetIdsByTags(tags);

            var assetTasks = assets.Select(async asset => {

                var content = await GetAssetContent(asset.File?.Url);

                if (content != null)
                {
                    return new Asset<byte[]>
                    {
                        Content = content,
                        Metadata = new AssetMetadata
                        {
                            Id = asset.SystemProperties.Id,
                            Filename = asset.File!.FileName,
                            Url = asset.File!.Url,
                            LastUpdated = asset.SystemProperties.UpdatedAt.GetValueOrDefault(asset.SystemProperties.CreatedAt.GetValueOrDefault()),
                        }
                    };
                }

                return null;
            });

            var assetResults = (await Task.WhenAll(assetTasks)).Where(result => result != null).Cast<Asset<byte[]>>().ToList();

            return assetResults;
        }
                
        private async Task<List<Asset>> GetAssetIdsByTags(string[] tags)
        {
            var assetIds = new HashSet<string>();

            foreach (var tag in tags)
            {
                var cancellationToken = new CancellationToken();
                var result = await _contentfulClient!.GetAssets($"?metadata.tags.sys.id[in]={tag}", cancellationToken);
                assetIds.UnionWith(result.Items.Select(a => a.SystemProperties.Id));
            }

            var assets = new List<Asset>();

            foreach (var assetId in assetIds)
            {
                var asset = await _contentfulClient!.GetAsset(assetId);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        private async Task<byte[]?> GetAssetContent(string? assetUrl)
        {
            if (!string.IsNullOrEmpty(assetUrl))
            {
                return await _assetDownloader.DownloadAssetContentAsync(assetUrl);
            }

            return null;
        }
    }
}
