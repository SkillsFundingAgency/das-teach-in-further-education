using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Roots.Base;
using Contentful.Core.Models;
using Contentful.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using Contentful.Core.Search;
using System.Linq;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces.Roots;
using ApiPage = SFA.DAS.TeachInFurtherEducation.Contentful.Model.Api.Page;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services.Roots
{
    public class PageService : ContentRootService, IPageService
    {

        private readonly ILogger<PageService> _logger;

        public PageService(HtmlRenderer htmlRenderer, ILogger<PageService> logger) : base(htmlRenderer)
        {

            _logger = logger;

        }

        /// <summary>
        /// Retrieves the content ids for interim pages
        /// </summary>
        /// <param name="contentfulClient">The Contentful client used to retrieve data from the CMS.</param>
        /// <returns>Returns a list of content ids</returns>
        [ExcludeFromCodeCoverage]
        public async Task<IEnumerable<string>> GetAllPageContentIds(IContentfulClient contentfulClient)
        {
            _logger.LogInformation("Beginning {MethodName}...", nameof(GetAllPageContentIds));

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
                _logger.LogError(ex.Message);
            }

            return retVal;
        }

        public async Task<IEnumerable<PageRenamed>> GetAll(IContentfulClient contentfulClient)
        {

            _logger.LogInformation("Beginning {MethodName}...", nameof(GetAll));

            try
            {
                // This is failing for Pre-Prod (getting all pages in once go)
                //var builder = QueryBuilder<ApiPage>.New.ContentTypeIs("page").Include(3);
                //var pages = await contentfulClient.GetEntries(builder);

                var pages = new List<ApiPage>();

                var ids = await GetAllPageContentIds(contentfulClient);

                var cancellationToken = new CancellationToken();

                foreach (var id in ids)
                {
                    var pageQuery = QueryBuilder<ApiPage>.New.ContentTypeIs("page").FieldEquals("sys.id", id).Include(3);

                    var pagesWithContent = await contentfulClient.GetEntries(pageQuery, cancellationToken);
                    LogErrors(pagesWithContent);

                    pages.AddRange(pagesWithContent);
                }

                return await Task.WhenAll(FilterValidUrl(pages, _logger).Select(ToContent));
            }
            catch (Exception _Exception)
            {
                _logger.LogError(_Exception, "Unable to get pages.");

                return Enumerable.Empty<PageRenamed>();
            }
        }

        //todo: ctor on Page?
        private async Task<PageRenamed> ToContent(ApiPage apiPage)
        {
            return new PageRenamed(
                apiPage.PageTitle!,
                apiPage.PageURL!,
                (await ToHtmlString(apiPage.Contents))!,
                null,
                apiPage.Breadcrumbs);
        }
    }
}
