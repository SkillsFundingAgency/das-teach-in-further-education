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

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services.Roots
{
    public class PageService : ContentRootService, IPageService
    {

        private readonly ILogger<PageService> _logger;

        public PageService(HtmlRenderer htmlRenderer, ILogger<PageService> logger) : base(htmlRenderer)
        {

            _logger = logger;

        }

        public async Task<IEnumerable<PageRenamed>> GetAll(IContentfulClient contentfulClient)
        {
            var builder = QueryBuilder<ApiPage>.New.ContentTypeIs("page").Include(3);
            var pages = await contentfulClient.GetEntries(builder);
            LogErrors(pages);

            return await Task.WhenAll(FilterValidUrl(pages, _logger).Select(ToContent));
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
