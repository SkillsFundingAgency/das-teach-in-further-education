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
    }
}
