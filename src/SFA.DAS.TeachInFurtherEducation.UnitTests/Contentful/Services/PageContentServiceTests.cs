using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FakeItEasy;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using Xunit;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Roots;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Linq;
using ApiPage = SFA.DAS.TeachInFurtherEducation.Contentful.Model.Api.Page;
using SFA.DAS.TeachInFurtherEducation.Contentful.Interfaces;
using System.Dynamic;
using KellermanSoftware.CompareNetObjects;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.Services
{

    public class PageContentServiceTests
    {

        public HtmlRenderer htmlRenderer { get; set; }

        public ILogger<PageContentService> Logger { get; set; }

        public PageContentServiceTests()
        {

            htmlRenderer = A.Fake<HtmlRenderer>();

            Logger = A.Fake<ILogger<PageContentService>>();

        }

        #region Get Page Tests

        [Fact(DisplayName = "PageContentService - GetLandingPage - WithMatchingPage - ReturnsLandingPage")]
        public async Task PageContentService_GetLandingPage_WithMatchingPage_ReturnsLandingPage()
        {
            var contentfulClient = A.Fake<IContentfulClient>();

            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var entries = new ContentfulCollection<ApiPage>();

            entries.Items = [
                
                new ApiPage()
                {
                    PageTitle = "Test Page",
                    PageURL = "Home"
                }
            ];

            SetupContentIds(contentfulClient, "TestPage");

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<ApiPage>>._, A<CancellationToken>._)).Returns(entries);

            var result = await pageContentService.GetAllPages(contentfulClient);

            Assert.NotNull(result);

            Assert.Equal("Test Page", result.FirstOrDefault().PageTitle);

        }

        [Fact(DisplayName = "PageContentService - GetLandingPage - WithNoMatchingPage - ReturnsNull")]
        public async Task PageContentService_GetLandingPage_WithNoMatchingPage_ReturnsNull()
        {

            var contentfulClient = A.Fake<IContentfulClient>();

            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var fakeResult = new ContentfulCollection<Page> { Items = new List<Page>() };

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<Page>>._, A<CancellationToken>._)).Returns(fakeResult);

            var result = await pageContentService.GetAllPages(contentfulClient);

            Assert.Empty(result);

        }

        [Fact(DisplayName = "PageContentService - GetLandingPage - WithException - ReturnsNull")]
        public async Task PageContentService_GetLandingPage_WithException_ReturnsNull()
        {

            var contentfulClient = A.Fake<IContentfulClient>();

            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var fakeResult = new ContentfulCollection<Page> { Items = new List<Page>() };

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<Page>>._, A<CancellationToken>._)).Throws(new Exception());

            var result = await pageContentService.GetAllPages(contentfulClient);

            Assert.Empty(result);

        }

        #endregion

        #region Get Menu Items Tests

        [Fact(DisplayName = "PageContentService - GetMenuItems - WithMatchingMenuItems - ReturnsMenuItems")]
        public async Task PageContentService_GetMenuItems_WithMatchingMenuItems_ReturnsMenuItems()
        {

            var contentfulClient = A.Fake<IContentfulClient>();

            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var entries = new ContentfulCollection<MenuItem>();

            entries.Items = [

                new MenuItem()
                {

                    MenuItemOrder = 1,

                    MenuItemTitle = "Menu Item Title",

                    MenuItemText = "Menu Item Text",

                    MenuItemSource = "Menu Item Source",

                    TopLevelMenuItem = false

                }

            ];

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<MenuItem>>._, A<CancellationToken>._)).Returns(entries);

            var result = await pageContentService.GetMenuItems(contentfulClient);

            Assert.NotNull(result);

            Assert.Single(result);

            Assert.Equal("Menu Item Title", result.First().MenuItemTitle);

            Assert.Equal("Menu Item Text", result.First().MenuItemText);

            Assert.Equal("Menu Item Source", result.First().MenuItemSource);

        }

        [Fact(DisplayName = "PageContentService - GetMenuItems - WithNoMatchingMenuItems - Returns Empty")]
        public async Task PageContentService_GetMenuItems_WithNoMatchingMenuItems_ReturnsEmpty()
        {

            var contentfulClient = A.Fake<IContentfulClient>();

            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var fakeResult = new ContentfulCollection<MenuItem> { Items = new List<MenuItem>() };

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<MenuItem>>._, A<CancellationToken>._)).Returns(fakeResult);

            var result = await pageContentService.GetMenuItems(contentfulClient);

            Assert.Empty(result);

        }

        [Fact(DisplayName = "PageContentService - GetMenuItems - WithException - ReturnsEmpty")]
        public async Task PageContentService_GetMenuItems_WithException_ReturnsEmpty()
        {

            var contentfulClient = A.Fake<IContentfulClient>();

            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var fakeResult = new ContentfulCollection<MenuItem> { Items = new List<MenuItem>() };

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<MenuItem>>._, A<CancellationToken>._)).Throws(new Exception());

            var result = await pageContentService.GetMenuItems(contentfulClient);

            Assert.Empty(result);

        }

        #endregion

        #region Get Footer

        [Fact(DisplayName = "PageContentService - GetFooter - WithMatchingFooter - ReturnsFooterPage")]
        public async Task PageContentService_GetFooter_WithMatchingFooter_ReturnsFooterPage()
        {

            var contentfulClient = A.Fake<IContentfulClient>();

            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var entries = new ContentfulCollection<FooterLink>();

            entries.Items = [

                new FooterLink()
                {

                  FooterLinkTitle = "FooterLinksTitle",
                  FooterLinkSource = "Test URL",
                  FooterLinkText = "Test Link Text",
                  FooterLinkOrder = 1
                }

            ];

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<FooterLink>>._, A<CancellationToken>._)).Returns(entries);

            var result = await pageContentService.GetFooterLinks(contentfulClient);

            Assert.NotNull(result);

            Assert.Equal("FooterLinksTitle", result.First().FooterLinkTitle);

            Assert.Equal("Test URL", result.First().FooterLinkSource);

            Assert.Equal("Test Link Text", result.First().FooterLinkText);

            Assert.Equal(1, result.First().FooterLinkOrder);

        }

        [Fact(DisplayName = "PageContentService - GetFooter - WithNoMatchingFooter - Returns Empty")]
        public async Task PageContentService_GetFooter_WithNoMatchingFooter_ReturnsEmpty()
        {

            var contentfulClient = A.Fake<IContentfulClient>();

            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var fakeResult = new ContentfulCollection<FooterLink> { Items = new List<FooterLink>() };

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<FooterLink>>._, A<CancellationToken>._)).Returns(fakeResult);

            var result = await pageContentService.GetFooterLinks(contentfulClient);

            Assert.Empty(result);

        }

        [Fact(DisplayName = "PageContentService - GetFooter - WithException - ReturnsEmpty")]
        public async Task PageContentService_GetFooter_WithException_ReturnsEmpty()
        {

            var contentfulClient = A.Fake<IContentfulClient>();

            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var fakeResult = new ContentfulCollection<FooterLink> { Items = new List<FooterLink>() };

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<FooterLink>>._, A<CancellationToken>._)).Throws(new Exception());

            var result = await pageContentService.GetFooterLinks(contentfulClient);

            Assert.Empty(result);

        }

        #endregion

        [Fact(DisplayName = "PageContentService - GetInterimPages - WithMatchingInterimPages - ReturnsInterimPages")]
        public async Task PageContentService_GetInterimPages_WithMatchingInterimPages_ReturnsInterimPages()
        {
            var contentfulClient = A.Fake<IContentfulClient>();
            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var entries = new ContentfulCollection<InterimPage>
            {
                Items = new List<InterimPage>
                {
                    new InterimPage
                    {
                        InterimPageTitle = "Interim Page",
                        InterimPageURL = "interim-page",
                        InterimPagePreamble = null,
                        InterimPageBreadcrumbs = null,
                        InterimPageTileSections = new List<TileSection>()
                    }
                }
            };

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<InterimPage>>._, A<CancellationToken>._))
                .Returns(entries);

            var result = await pageContentService.GetInterimPages(contentfulClient);

            Assert.NotNull(result);
            var page = result.First();
            Assert.Equal("Interim Page", page.InterimPageTitle);
            Assert.Equal("interim-page", page.InterimPageURL);
        }

        [Fact(DisplayName = "PageContentService - GetInterimPages - WithNoMatchingInterimPages - ReturnsEmpty")]
        public async Task PageContentService_GetInterimPages_WithNoMatchingInterimPages_ReturnsEmpty()
        {
            var contentfulClient = A.Fake<IContentfulClient>();
            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var fakeResult = new ContentfulCollection<InterimPage> { Items = new List<InterimPage>() };

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<InterimPage>>._, A<CancellationToken>._)).Returns(fakeResult);

            var result = await pageContentService.GetInterimPages(contentfulClient);

            Assert.Empty(result);
        }

        [Fact(DisplayName = "PageContentService - GetInterimPages - WithException - ReturnsEmpty")]
        public async Task PageContentService_GetInterimPages_WithException_ReturnsEmpty()
        {
            var contentfulClient = A.Fake<IContentfulClient>();
            var pageContentService = new PageContentService(htmlRenderer, Logger);

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<InterimPage>>._, A<CancellationToken>._)).Throws(new Exception());

            var result = await pageContentService.GetInterimPages(contentfulClient);

            Assert.Empty(result);
        }

        [Fact(DisplayName = "PageContentService - GetBetaBanner - WithMatchingBanner - ReturnsBanner")]
        public async Task PageContentService_GetBetaBanner_WithMatchingBanner_ReturnsBanner()
        {
            var contentfulClient = A.Fake<IContentfulClient>();
            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var entries = new ContentfulCollection<BetaBanner>
            {
                Items = new List<BetaBanner>
                {
                    new BetaBanner
                    {
                        BetaBannerID = "banner-1",
                        BetaBannerTitle = "Test Beta Banner"
                    }
                }
            };

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<BetaBanner>>._, A<CancellationToken>._)).Returns(entries);

            var result = await pageContentService.GetBetaBanner(contentfulClient);

            Assert.NotNull(result);
            Assert.Equal("banner-1", result.BetaBannerID);
            Assert.Equal("Test Beta Banner", result.BetaBannerTitle);
        }

        [Fact(DisplayName = "PageContentService - GetBetaBanner - WithNoMatchingBanner - ReturnsNull")]
        public async Task PageContentService_GetBetaBanner_WithNoMatchingBanner_ReturnsNull()
        {
            var contentfulClient = A.Fake<IContentfulClient>();
            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var fakeResult = new ContentfulCollection<BetaBanner> { Items = new List<BetaBanner>() };

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<BetaBanner>>._, A<CancellationToken>._)).Returns(fakeResult);

            var result = await pageContentService.GetBetaBanner(contentfulClient);

            Assert.Null(result);
        }

        [Fact(DisplayName = "PageContentService - GetBetaBanner - WithException - ReturnsNull")]
        public async Task PageContentService_GetBetaBanner_WithException_ReturnsNull()
        {
            var contentfulClient = A.Fake<IContentfulClient>();
            var pageContentService = new PageContentService(htmlRenderer, Logger);

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<BetaBanner>>._, A<CancellationToken>._)).Throws(new Exception());

            var result = await pageContentService.GetBetaBanner(contentfulClient);

            Assert.Null(result);
        }

        [Fact(DisplayName = "PageContentService - GetAllPages - Null Contents - CallsToHtmlString")]
        public async Task PageContentService_GetAllPages_NullContents_CallsToHtmlString()
        {
            // Arrange
            var contentfulClient = A.Fake<IContentfulClient>();
            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var apiPage = new ApiPage
            {
                PageURL = "TestPage",
                PageTitle = "Test Page",
                Contents = null // Set contents to null to trigger the path in ToHtmlString
            };

            var entries = new ContentfulCollection<ApiPage>
            {
                Items = new List<ApiPage> { apiPage }
            };

            SetupContentIds(contentfulClient, "TestPage");

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<ApiPage>>._, A<CancellationToken>._)).Returns(entries);

            // Act
            var result = await pageContentService.GetAllPages(contentfulClient);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Page", result.First().PageTitle);
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

        [Fact(DisplayName = "PageContentService - GetAllPages - Handles Exception and Logs Error")]
        public async Task PageContentService_GetAllPages_HandlesExceptionAndLogsError()
        {
            // Arrange
            var contentfulClient = A.Fake<IContentfulClient>();
            var pageContentService = new PageContentService(htmlRenderer, Logger);

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
            var result = await pageContentService.GetAllPages(contentfulClient);

            // Assert
            // Verify the result is an empty collection
            Assert.Empty(result);

            Logger.VerifyLogMustHaveHappened(LogLevel.Error, "Unable to get pages.");
        }

        [Fact(DisplayName = "PageContentService - GetAllPages - Retrieves Pages Correctly")]
        public async Task PageContentService_GetAllPages_RetrievesPagesCorrectly()
        {
            // Arrange
            var contentfulClient = A.Fake<IContentfulClient>();
            var pageContentService = new PageContentService(htmlRenderer, Logger);
                       
            SetupContentIds(contentfulClient, "TestPage1");

            // Mock GetEntries for actual pages
            var apiPage1 = new ApiPage { PageURL = "TestPage1", PageTitle = "Page 1 Title", PageTemplate = "template1" };

            var pagesCollection1 = new ContentfulCollection<ApiPage> { Items = new List<ApiPage> { apiPage1 } };

            // Mock GetEntries to return pages based on the sys.id
            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<ApiPage>>.That.Matches(q => q.Build().Contains("TestPage1")), A<CancellationToken>._))
                .Returns(Task.FromResult(pagesCollection1));

            // Act
            var result = await pageContentService.GetAllPages(contentfulClient);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(result, r => r.PageTitle == "Page 1 Title");
        }

        [Fact(DisplayName = "PageContentService - GetAllPages - CallsToHtmlString with Valid Document")]
        public async Task PageContentService_GetAllPages_CallsToHtmlString_WithValidDocument()
        {
            // Arrange
            var contentfulClient = A.Fake<IContentfulClient>();
            var pageContentService = new PageContentService(htmlRenderer, Logger);

            var apiPage = new ApiPage
            {
                PageURL = "TestPage",
                PageTitle = "Test Page",
                PageTemplate = "template",
                Breadcrumbs = null,
                PageComponents = null,
                Contents = new Document // Create a valid Document here
                {
                    NodeType = "document",
                    Data = new GenericStructureData(),
                    Content = new List<IContent>() // Add any necessary content if needed
                }
            };

            var entries = new ContentfulCollection<ApiPage>
            {
                Items = new List<ApiPage> { apiPage }
            };

            SetupContentIds(contentfulClient, "TestPage");

            // Mock GetEntries to return pages based on the sys.id
            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<ApiPage>>.That.Matches(q => q.Build().Contains("TestPage")), A<CancellationToken>._))
                .Returns(Task.FromResult(entries));

            // Act
            var result = await pageContentService.GetAllPages(contentfulClient);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Page", result.First().PageTitle);
        }

        [Fact(DisplayName = "PageContentService - GetAllPages - LogsWarningForExcludedUrls")]
        public async Task PageContentService_GetAllPages_LogsWarningForExcludedUrls()
        {
            var contentfulClient = A.Fake<IContentfulClient>();
            var logger = A.Fake<ILogger<PageContentService>>();
            var pageContentService = new PageContentService(htmlRenderer, logger);

            // Arrange: Mock some pages retrieved from Contentful
            var page1 = new ApiPage()
            {
                PageTitle = "Page 1",
                PageURL = "valid-url"
            };

            var page2 = new ApiPage()
            {
                PageTitle = "Page 2",
                PageURL = null
            };

            var page3= new ApiPage()
            {
                PageTitle = "Page 3",
                PageURL = ""
            };

            var pages = new List<ApiPage> { page1, page2 };

            var contentfulCollection = new ContentfulCollection<ApiPage>
            {
                Items = pages
            };

            SetupContentIds(contentfulClient, "TestPage1", "TestPage2", "TestPage3");

            // Mock GetEntries for actual pages
            var pagesCollection1 = new ContentfulCollection<ApiPage> { Items = new List<ApiPage> { page1 } };
            var pagesCollection2 = new ContentfulCollection<ApiPage> { Items = new List<ApiPage> { page2 } };
            var pagesCollection3 = new ContentfulCollection<ApiPage> { Items = new List<ApiPage> { page3 } };

            // Mock GetEntries to return pages based on the sys.id
            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<ApiPage>>.That.Matches(q => q.Build().Contains("TestPage1")), A<CancellationToken>._))
                .Returns(Task.FromResult(pagesCollection1));

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<ApiPage>>.That.Matches(q => q.Build().Contains("TestPage2")), A<CancellationToken>._))
                .Returns(Task.FromResult(pagesCollection2));

            A.CallTo(() => contentfulClient.GetEntries(A<QueryBuilder<ApiPage>>.That.Matches(q => q.Build().Contains("TestPage3")), A<CancellationToken>._))
                .Returns(Task.FromResult(pagesCollection3));

            // Act
            var result = await pageContentService.GetAllPages(contentfulClient);

            // Assert
            Assert.Single(result); // Only one valid page should remain
        }


        // Mock class for IRootContent
        private class MockRootContent : IRootContent
        {
            public string PageURL { get; set; }
        }

    }

}
