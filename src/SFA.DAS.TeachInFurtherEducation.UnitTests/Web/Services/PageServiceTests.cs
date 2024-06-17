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
    }
}
