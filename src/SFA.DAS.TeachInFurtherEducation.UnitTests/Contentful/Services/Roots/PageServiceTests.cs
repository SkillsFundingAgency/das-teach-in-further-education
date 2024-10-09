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

            // Workaround for the optional argument issue
            A.CallTo(() => _contentfulClient.GetEntries(A<QueryBuilder<ApiPage>>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(contentfulCollection));

            // Act
            var result = await _pageService.GetAll(_contentfulClient);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Title == "Page 1");
            Assert.Contains(result, p => p.Title == "Page 2");
        }
    }
}
