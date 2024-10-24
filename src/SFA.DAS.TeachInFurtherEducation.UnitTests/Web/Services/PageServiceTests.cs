using System;
using AutoFixture;
using AutoFixture.Kernel;
using Contentful.Core.Models;
using FakeItEasy;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using System.Linq;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using Xunit;
using IContent = SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces.IContent;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using SFA.DAS.TeachInFurtherEducation.Web.Models;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Services
{
    public class PageServiceTests
    {
        public const string CookiePageUrl = "Cookies";

        public Fixture Fixture { get; set; }
        public PageRenamed[] Pages { get; set; }
        public PageRenamed AnalyticsPage { get; set; }
        public PageRenamed MarketingPage { get; set; }
        public IContentService ContentService { get; set; }
        public IContent Content { get; set; }
        public PageService PageService { get; set; }

        public PageServiceTests()
        {
            Fixture = new Fixture();

            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            Fixture.Customizations.Add(
                new TypeRelay(
                    typeof(global::Contentful.Core.Models.IContent),
                    typeof(Paragraph)));

            Content = A.Fake<IContent>();

            ContentService = A.Fake<IContentService>();

            A.CallTo(() => ContentService.Content)
                .Returns(Content);

            AnalyticsPage = new PageRenamed("", "analyticscookies", new HtmlString(""));
            MarketingPage = new PageRenamed("", "marketingcookies", new HtmlString(""));

            Pages = new[] { AnalyticsPage }
                .Concat(Fixture.CreateMany<PageRenamed>(3))
                .Concat(new[] { MarketingPage }).ToArray();

            A.CallTo(() => Content.Pagess)
                .Returns(Pages);

            PageService = new PageService(ContentService);
        }


        [Fact]
        public void GetPageModel_IsPreviewIsFalseTest()
        {
            // act
            var model = PageService.GetPageModel(Pages[1].Url);

            Assert.False(model.Preview.IsPreview);
        }

        [Fact]
        public void GetPageModel_CookiePageUrlReturnsCookiesViewNameTest()
        {
            var model = PageService.GetPageModel(CookiePageUrl);

            Assert.Equal("Cookies", model.ViewName);
        }

        [Fact]
        public void GetPageModel_CookiePageUrlReturnsCookiePageModelTest()
        {
            var model = PageService.GetPageModel(CookiePageUrl);

            var cookiePage = Assert.IsType<CookiePage>(model.Page);
            Assert.Equal(AnalyticsPage, cookiePage.AnalyticsPage);
            Assert.Equal(MarketingPage, cookiePage.MarketingPage);
        }

        [Fact]
        public void GetPageModel_UnknownPageUrlReturnsNullModelTest()
        {
            string unknowUrl = nameof(unknowUrl);

            var model = PageService.GetPageModel(unknowUrl);

            Assert.Null(model);
        }

        [Fact]
        public void GetPageModel_ErrorCheckPageUrlThrowsExceptionTest()
        {
            string errorCheckUrl = "error-check";

            Assert.Throws<NotImplementedException>(() => PageService.GetPageModel(errorCheckUrl));
        }

        [Fact]
        public void RedirectPreview_NonRedirectUrlReturnsNullRouteNameTest()
        {
            const string nonRedirectUrl = "dont-redirect";

            var (routeName, _) = PageService.RedirectPreview(nonRedirectUrl);

            Assert.Null(routeName);
        }

        [Fact]
        public void RedirectPreview_AnalyticsUrlReturnsPagePreviewRouteNameTest()
        {
            const string analyticsCookiesUrl = "analyticscookies";

            var (routeName, _) = PageService.RedirectPreview(analyticsCookiesUrl);

            Assert.Equal("page-preview", routeName);
        }

        [Fact]
        public void RedirectPreview_AnalyticsUrlReturnsCookiesPageUrlRouteValueTest()
        {
            const string analyticsCookiesUrl = "analyticscookies";

            var (_, routeValues) = PageService.RedirectPreview(analyticsCookiesUrl);

            Assert.Equal("cookies", routeValues.GetType().GetProperty("pageUrl").GetValue(routeValues, null));
        }

        [Fact]
        public void RedirectPreview_MarketingUrlReturnsPagePreviewRouteNameTest()
        {
            const string marketingCookiesUrl = "marketingcookies";

            var (routeName, _) = PageService.RedirectPreview(marketingCookiesUrl);

            Assert.Equal("page-preview", routeName);
        }

        [Fact]
        public void RedirectPreview_MarketingUrlReturnsCookiesPageUrlRouteValueTest()
        {
            const string marketingCookiesUrl = "marketingcookies";

            var (_, routeValues) = PageService.RedirectPreview(marketingCookiesUrl);

            Assert.Equal("cookies", routeValues.GetType().GetProperty("pageUrl").GetValue(routeValues, null));
        }

        [Fact]
        public void RedirectPreview_HomeUrlReturnsPagePreviewRouteNameTest()
        {
            const string homeCookiesUrl = "home";

            var (routeName, _) = PageService.RedirectPreview(homeCookiesUrl);

            Assert.Equal("home-preview", routeName);
        }

        [Fact]
        public async Task GetPageModelPreview_IsPreviewIsTrueTest()
        {
            A.CallTo(() => ContentService.UpdatePreview())
                .Returns(Content);

            // act
            var model = await PageService.GetPageModelPreview(Pages.First().Url);

            Assert.True(model.Preview.IsPreview);
        }

        [Fact]
        public async Task GetPageModelPreview_ContentNull_PreviewErrorTest()
        {
            A.CallTo(() => ContentService.UpdatePreview())
                .Returns(Content);

            var page = new PageRenamed("title", "url", null);

            var pages = new[] {page};

            A.CallTo(() => Content.Pagess)
                .Returns(pages);

            // act
            var model = await PageService.GetPageModelPreview(page.Url);

            Assert.Collection(model.Preview.PreviewErrors,
                e => Assert.Equal("Content must not be blank", e.Value));
        }

        [Fact]
        public async Task GetPageModelPreview_TitleNull_PreviewErrorTest()
        {
            A.CallTo(() => ContentService.UpdatePreview())
                .Returns(Content);

            var page = new PageRenamed(null, "url", new HtmlString("content"));

            var pages = new[] { page };

            A.CallTo(() => Content.Pagess)
                .Returns(pages);

            // act
            var model = await PageService.GetPageModelPreview(page.Url);

            Assert.Collection(model.Preview.PreviewErrors,
                e => Assert.Equal("Title must not be blank", e.Value));
        }

        [Fact]
        public async Task GetPageModelPreview_CookiePageAnalyticsPageContentNull_PreviewErrorTest()
        {
            A.CallTo(() => ContentService.UpdatePreview())
                .Returns(Content);

            AnalyticsPage = new PageRenamed("", "analyticscookies", null);

            var pages = new[] { AnalyticsPage, MarketingPage };

            A.CallTo(() => Content.Pagess)
                .Returns(pages);

            // act
            var model = await PageService.GetPageModelPreview("cookies");

            Assert.Collection(model.Preview.PreviewErrors,
                e => Assert.Equal("AnalyticsPage content must not be blank", e.Value));
        }


        [Fact]
        public async Task GetPageModelPreview_CookiePageMarketingPageContentNull_PreviewErrorTest()
        {
            A.CallTo(() => ContentService.UpdatePreview())
                .Returns(Content);

            MarketingPage = new PageRenamed("", "marketingcookies", null);

            var pages = new[] { AnalyticsPage, MarketingPage };

            A.CallTo(() => Content.Pagess)
                .Returns(pages);

            // act
            var model = await PageService.GetPageModelPreview("cookies");

            Assert.Collection(model.Preview.PreviewErrors,
                e => Assert.Equal("MarketingPage content must not be blank", e.Value));
        }

        [Fact]
        public void GetPageModel_CookiePageUrlWhenMarketingPageMissing_ReturnsNull()
        {
            // Arrange: Remove the MarketingPage from the Pages collection
            Pages = Pages.Where(p => p != MarketingPage).ToArray();

            A.CallTo(() => Content.Pagess)
                .Returns(Pages);

            // Re-initialize the PageService to rebuild the page models
            PageService = new PageService(ContentService);

            // Act
            var model = PageService.GetPageModel(CookiePageUrl);

            // Assert
            Assert.Null(model);
        }

        [Fact]
        public async Task GetPageModelPreview_WhenCookiePageModelIsNull_ReturnsNull()
        {
            // Arrange
            // Create a fake preview content without AnalyticsPage and MarketingPage
            var previewContent = A.Fake<IContent>();

            // Remove AnalyticsPage and MarketingPage from the Pages collection
            var pagesWithoutCookiePages = Pages
                .Where(p => p.Url != "analyticscookies" && p.Url != "marketingcookies")
                .ToArray();

            A.CallTo(() => previewContent.Pagess)
                .Returns(pagesWithoutCookiePages);

            A.CallTo(() => ContentService.UpdatePreview())
                .Returns(previewContent);

            // Act
            var model = await PageService.GetPageModelPreview("cookies");

            // Assert
            Assert.Null(model);
        }

        [Fact]
        public async Task GetPageModelPreview_UnknownPageUrl_ReturnsNull()
        {
            // Arrange
            // Create a fake preview content with existing pages
            var previewContent = A.Fake<IContent>();

            // Assume the Pages collection contains some pages
            var existingPages = Fixture.CreateMany<PageRenamed>(3).ToArray();

            A.CallTo(() => previewContent.Pagess)
                .Returns(existingPages);

            A.CallTo(() => ContentService.UpdatePreview())
                .Returns(previewContent);

            // Use a pageUrl that does not exist in the Pages collection
            string unknownPageUrl = "unknown-page-url";

            // Act
            var model = await PageService.GetPageModelPreview(unknownPageUrl);

            // Assert
            Assert.Null(model);
        }

        [Fact]
        public void OnContentUpdated_WhenContentIsUpdated_PageModelsAreRebuilt()
        {
            // Arrange
            // Initial content with PageA
            var initialContent = A.Fake<IContent>();
            var initialPage = new PageRenamed("Initial Title", "page-a", new HtmlString("Initial Content"));
            A.CallTo(() => initialContent.Pagess).Returns(new[] { initialPage });
            A.CallTo(() => ContentService.Content).Returns(initialContent);

            // Create the PageService with the initial content
            PageService = new PageService(ContentService);

            // Verify that the initial page is available
            var initialModel = PageService.GetPageModel("page-a");
            Assert.NotNull(initialModel);
            Assert.Equal("Initial Title", initialModel.Page.Title);

            // Act
            // Update the content to have PageB instead of PageA
            var updatedContent = A.Fake<IContent>();
            var updatedPage = new PageRenamed("Updated Title", "page-b", new HtmlString("Updated Content"));
            A.CallTo(() => updatedContent.Pagess).Returns(new[] { updatedPage });
            A.CallTo(() => ContentService.Content).Returns(updatedContent);

            // Raise the ContentUpdated event to trigger OnContentUpdated
            ContentService.ContentUpdated += Raise.WithEmpty();

            // Assert
            // The initial page should no longer be available
            var modelAfterUpdate = PageService.GetPageModel("page-a");
            Assert.Null(modelAfterUpdate);

            // The updated page should now be available
            var updatedModel = PageService.GetPageModel("page-b");
            Assert.NotNull(updatedModel);
            Assert.Equal("Updated Title", updatedModel.Page.Title);
        }



    }
}
