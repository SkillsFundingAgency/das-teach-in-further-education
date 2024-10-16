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

        public PageService(HtmlRenderer htmlRenderer, ILogger<PageService> logger) : base(htmlRenderer, logger) { }

        public Task<IEnumerable<PageRenamed>> GetAll(IContentfulClient contentfulClient) => base.GetContentSync<PageRenamed>(contentfulClient, "page", ToContent);

        private async Task<PageRenamed> ToContent(ApiPage apiPage)
        {
            return new PageRenamed(
                apiPage.PageTitle!,
                apiPage.PageURL!,
                (await ToHtmlString(apiPage.Contents))!,
                null,
                apiPage.Breadcrumbs);
        }

        //public async Task<IEnumerable<PageRenamed>> GetAll(IContentfulClient contentfulClient)
        //{
        //    return await GetPagesAndContent(contentfulClient);

        //    //_logger.LogInformation("Beginning {MethodName}...", nameof(GetAll));

        //    //try
        //    //{
        //    //    // This is failing for Pre-Prod (getting all pages in once go)
        //    //    //var builder = QueryBuilder<ApiPage>.New.ContentTypeIs("page").Include(3);
        //    //    //var pages = await contentfulClient.GetEntries(builder);

        //    //    var pages = new List<ApiPage>();

        //    //    var ids = await GetAllPageContentIds(contentfulClient, _logger);

        //    //    var cancellationToken = new CancellationToken();

        //    //    foreach (var id in ids)
        //    //    {
        //    //        var pageQuery = QueryBuilder<ApiPage>.New.ContentTypeIs("page").FieldEquals("sys.id", id).Include(3);

        //    //        var pagesWithContent = await contentfulClient.GetEntries(pageQuery, cancellationToken);
        //    //        LogErrors(pagesWithContent);

        //    //        pages.AddRange(pagesWithContent);
        //    //    }

        //    //    return await Task.WhenAll(FilterValidUrl(pages, _logger).Select(ToContent));
        //    //}
        //    //catch (Exception _Exception)
        //    //{
        //    //    _logger.LogError(_Exception, "Unable to get pages.");

        //    //    return Enumerable.Empty<PageRenamed>();
        //    //}
        //}

        ////todo: ctor on Page?

    }
}
