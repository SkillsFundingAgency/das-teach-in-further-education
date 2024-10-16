using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Api;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces;
using ApiPage = SFA.DAS.TeachInFurtherEducation.Contentful.Model.Api.Page;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services.Roots.Base
{
    public class ContentRootService
    {
        private readonly HtmlRenderer _htmlRenderer;
        private readonly ILogger _logger;

        public ContentRootService(HtmlRenderer htmlRenderer, ILogger logger)
        {
            _htmlRenderer = htmlRenderer;
            _logger = logger;
        }

        protected async Task<HtmlString?> ToHtmlString(Document? document)
        {
            if (document == null)
                return null;

            string html = await _htmlRenderer.ToHtml(document);

            return ToNormalisedHtmlString(html);
        }

        /// <remarks>
        /// Should be private, but Contentful's .net library is not very test friendly (HtmlRenderer.ToHtml can't be mocked).
        /// We'd have to introduce a level of indirection to test this, were it private.
        /// </remarks>
        public static HtmlString ToNormalisedHtmlString(string html)
        {
            // replace left/right quotation marks with regular old quotation marks
            html = html.Replace('“', '"').Replace('”', '"');

            // sometimes contentful uses a \r and sometimes a \r\n - nice!
            // we could strip these out instead
            html = html.Replace("\r\n", "\r");
            html = html.Replace("\r", "\r\n");

            return new HtmlString(html);
        }

        protected async Task<IEnumerable<T>> GetContentSync<T>(IContentfulClient contentfulClient, string contentType, Func<ApiPage, Task<T>> toContent)
        {

            _logger.LogInformation("Beginning {MethodName}...", nameof(GetContentSync));

            try
            {
#pragma warning disable S125

                // This is failing for Pre-Prod (getting all pages in once go)
                //var builder = QueryBuilder<ApiPage>.New.ContentTypeIs("page").Include(3);
                //var pages = await contentfulClient.GetEntries(builder);

#pragma warning restore S125 

                var pages = new List<ApiPage>();


                var ids = await GetAllPageContentIds(contentfulClient, _logger);
                var cancellationToken = new CancellationToken();

                foreach (var id in ids)
                {
                    var pageQuery = QueryBuilder<ApiPage>.New.ContentTypeIs(contentType).FieldEquals("sys.id", id).Include(3);

                    var pagesWithContent = await contentfulClient.GetEntries(pageQuery, cancellationToken);
                    LogErrors(pagesWithContent);

                    pages.AddRange(pagesWithContent);
                }

                return await Task.WhenAll(FilterValidUrl(pages, _logger).Select(toContent));
            }
            catch (Exception _Exception)
            {
                _logger.LogError(_Exception, "Unable to get pages.");

                return Enumerable.Empty<T>();
            }
        }

        /// <summary>
        /// Retrieves the content ids for interim pages
        /// </summary>
        /// <param name="contentfulClient">The Contentful client used to retrieve data from the CMS.</param>
        /// <returns>Returns a list of content ids</returns>
        [ExcludeFromCodeCoverage]
        protected async Task<IEnumerable<string>> GetAllPageContentIds(IContentfulClient contentfulClient, ILogger logger)
        {
            logger.LogInformation("Beginning {MethodName}...", nameof(GetAllPageContentIds));

            var retVal = new List<string>();

            try
            {
                var query = QueryBuilder<dynamic>.New.ContentTypeIs("page").Include(3);
                var entries = await contentfulClient.GetEntries(query);

                if (entries != null)
                {
                    retVal.AddRange(entries.Items.Select(entry => (string)entry.sys.id.ToString()));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            return retVal;
        }

        protected static IEnumerable<T> FilterValidUrl<T>(IEnumerable<T> roots, ILogger logger)
            where T : IRootContent
        {
            var rootsWithValidUrl = roots.Where(p => !string.IsNullOrWhiteSpace(p.PageURL));

            int numberExcluded = roots.Count() - rootsWithValidUrl.Count();
            if (numberExcluded > 0)
            {
                logger.LogWarning("Had to exclude {NumberExcluded} root content items for blank urls", numberExcluded);
            }

            return rootsWithValidUrl;
        }

        protected static void LogErrors<T>(ContentfulCollection<T> contentfulCollection)
        {
            //todo: log errors
            //todo: show error when previewing

#if log_errors
            if (!contentfulCollection.Errors.Any())
                return;

            //todo: log SystemProperties.Type?
            _logger.LogWarning($"Errors received fetching {nameof(T)}'s.");

            foreach (var errorDetails in contentfulCollection.Errors.Select(e => e.Details))
            {
                _logger.LogWarning($"Id:{errorDetails.Id}, LinkType:{errorDetails.LinkType}, Type:{errorDetails.Type}");
            }
#endif
        }
    }
}
