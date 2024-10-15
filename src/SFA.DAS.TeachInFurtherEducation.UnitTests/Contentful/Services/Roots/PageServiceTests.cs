using Xunit;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Roots;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using Contentful.Core;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Html;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contentful.Core.Models;
using System.Linq;
using System.Threading;
using ApiPage = SFA.DAS.TeachInFurtherEducation.Contentful.Model.Api.Page;
using System.Dynamic;
using NLog;
using System;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.Services.Roots
{
    public class PageServiceTests
    {
        private readonly ILogger<PageService> _logger;
        private readonly HtmlRenderer _htmlRenderer;
        private readonly PageService _pageService;
        private readonly IContentfulClient _contentfulClient;

        public PageServiceTests()
        {
            _logger = A.Fake<ILogger<PageService>>();
            _htmlRenderer = A.Fake<HtmlRenderer>();
            _contentfulClient = A.Fake<IContentfulClient>();

            // Create the service instance
            _pageService = new PageService(_htmlRenderer, _logger);
        }

        [Fact]
        public async Task GetAll_ShouldReturnFilteredPages()
        {
            // Arrange: Mock some pages retrieved from Contentful
            var page1 = new ApiPage()
            {
                PageTitle = "Page 1",
                PageURL = "/page-1"
            };

            var page2 = new ApiPage()
            {
                PageTitle = "Page 2",
                PageURL = "/page-2"
            };

            var pages = new List<ApiPage> { page1, page2 };

            var contentfulCollection = new ContentfulCollection<ApiPage>
            {
                Items = pages
            };

            SetupContentIds(_contentfulClient, "TestPage1", "TestPage2");

            var pagesCollection1 = new ContentfulCollection<ApiPage> { Items = new List<ApiPage> { page1 } };
            var pagesCollection2 = new ContentfulCollection<ApiPage> { Items = new List<ApiPage> { page2 } };

            // Mock GetEntries to return pages based on the sys.id
            A.CallTo(() => _contentfulClient.GetEntries(A<QueryBuilder<ApiPage>>.That.Matches(q => q.Build().Contains("TestPage1")), A<CancellationToken>._))
                .Returns(Task.FromResult(pagesCollection1));

            A.CallTo(() => _contentfulClient.GetEntries(A<QueryBuilder<ApiPage>>.That.Matches(q => q.Build().Contains("TestPage2")), A<CancellationToken>._))
                .Returns(Task.FromResult(pagesCollection2));


            //// Workaround for the optional argument issue
            //A.CallTo(() => _contentfulClient.GetEntries(A<QueryBuilder<ApiPage>>.Ignored, A<CancellationToken>.Ignored))
            //    .Returns(Task.FromResult(contentfulCollection));

            // Act
            var result = await _pageService.GetAll(_contentfulClient);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Title == "Page 1");
            Assert.Contains(result, p => p.Title == "Page 2");
        }

        [Fact(DisplayName = "PageService - GetAll - Handles Exception and Logs Error")]
        public async Task PageService_GetAll_HandlesExceptionAndLogsError()
        {
            // Arrange
            var contentfulClient = A.Fake<IContentfulClient>();

            // Mock GetAllPageContentIds to return some IDs
            var pageIds = new List<string> { "TestPage1", "TestPage2" };
            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<dynamic>>._, A<CancellationToken>._))
                .Returns(Task.FromResult(new ContentfulCollection<dynamic>
                {
                    Items = pageIds.Select(id => new { sys = new { id = id } }).ToList<dynamic>()
                }));

            SetupContentIds(contentfulClient, "TestPage1");

            // Mock GetEntries to throw an exception when retrieving pages
            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<ApiPage>>._, A<CancellationToken>._))
                .Throws(new Exception("Simulated exception"));

            // Act
            var result = await _pageService.GetAll(contentfulClient);

            // Assert
            // Verify the result is an empty collection
            Assert.Empty(result);

            _logger.VerifyLogMustHaveHappened(Microsoft.Extensions.Logging.LogLevel.Error, "Unable to get pages.");
        }

        private void SetupContentIds(IContentfulClient contentfulClient, params string[] ids)
        {
            // Mock page IDs returned from GetAllPageContentIds
            var contentIds = new ContentfulCollection<dynamic>
            {
                Items = new List<dynamic>(ids.Select(id =>
                {
                    dynamic sys = new ExpandoObject();
                    sys.id = id;

                    dynamic entry = new ExpandoObject();
                    entry.sys = sys;

                    return entry;
                }))
            };

            // Mock GetEntries for content IDs
            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<dynamic>>._, A<CancellationToken>._))
                .Returns(Task.FromResult(contentIds));
        }
    }
}
