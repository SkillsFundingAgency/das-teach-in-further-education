using AutoFixture.Kernel;
using AutoFixture;
using ContentfulModels = Contentful.Core.Models;
using FakeItEasy;
using IContent = SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces.IContent;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Infrastructure;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Xunit;


namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Services
{
    public class ContentModelServiceTests
    {
        public Fixture Fixture { get; set; }
        public Page[] Pages { get; set; }

        public IContentService ContentService { get; set; }
        public ILogger<ContentModelService> LoggerService;
        public IContent Content { get; set; }
        public ContentModelService ContentModelService { get; set; }
        public ContentfulModels.HtmlRenderer htmlRenderer { get; set; }

        private IEnumerable<MenuItem> _menus = new List<MenuItem>
        {
            new()
            {
                MenuItemOrder = 1,
                MenuItemSource = "/",
                MenuItemText = "Home",
                MenuItemTitle = "home",
                TopLevelMenuItem = false
            },
            new()
            {
                MenuItemOrder = 2,
                MenuItemSource = "/find-funding-and-training",
                MenuItemText = "Find funding and training",
                MenuItemTitle = "Find funding and training",
                TopLevelMenuItem = false
            }
        };

        public ContentModelServiceTests()
        {
            Fixture = new Fixture();

            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            Fixture.Customizations.Add(
                new TypeRelay(
                    typeof(global::Contentful.Core.Models.IContent),
                    typeof(ContentfulModels.Paragraph)));

            Content = A.Fake<IContent>();
            LoggerService = A.Fake<ILogger<ContentModelService>>();
            htmlRenderer = A.Fake<ContentfulModels.HtmlRenderer>();

            ContentService = A.Fake<IContentService>();

            A.CallTo(() => ContentService.Content)
                .Returns(Content);

            A.CallTo(() => ContentService.GetPageByURL("unknowUrl"))
                .Returns(null);

            var TestPage = new Page("TestURL", "TestTitle", "TestTemplate", null, new List<ContentfulModels.IContent>(),
                new HtmlString(""));
            var HomePage = new Page("HomePageURL", "HolePageTitle", "HomeTemplate", null,
                new List<ContentfulModels.IContent>(), new HtmlString(""));

            Pages = new[] { TestPage, HomePage };

            A.CallTo(() => Content.Pages)
                .Returns(Pages);

            ContentModelService = new ContentModelService(LoggerService, ContentService, htmlRenderer);
        }

        [Fact]
        public void GetPageContentModel_IsPreviewIsFalseTest()
        {
            // act
            var model = ContentModelService.GetPageContentModel(Pages[1].PageURL);

            Assert.False(model.Preview.IsPreview);
        }

        [Fact]
        public void GetPageContentModel_UnknownPageUrlReturnsNullModelTest()
        {
            string unknowUrl = nameof(unknowUrl);

            var model = ContentModelService.GetPageContentModel(unknowUrl);

            Assert.Null(model);
        }

        [Fact]
        public async Task GetPagePreviewModel_IsPreviewIsTrueTest()
        {
            A.CallTo(() => ContentService.UpdatePreview())
                .Returns(Content);

            // act
            var model = await ContentModelService.GetPagePreviewModel(Pages[0].PageURL);

            Assert.True(model.Preview.IsPreview);
        }

        [Fact]
        public void GetPageContentModel_WhenExceptionIsThrown_LogsErrorAndReturnsNull()
        {
            // Arrange
            string pageURL = "unknownPageUrl";
            A.CallTo(() => ContentService.GetPageByURL(pageURL))
                .Throws(new Exception("Service failure"));

            // Act
            var result = ContentModelService.GetPageContentModel(pageURL);

            // Assert
            Assert.Null(result); // Should return null
            LoggerService.VerifyLogMustHaveHappened(LogLevel.Error,
                "Unable to get a page."); // Verify logging with your logger extension
        }

        [Fact]
        public async Task GetPagePreviewModel_WhenExceptionIsThrown_LogsErrorAndReturnsNull()
        {
            // Arrange
            string pageURL = "unknownPageUrl";
            A.CallTo(() => ContentService.UpdatePreview()).Returns(Content);
            A.CallTo(() => ContentService.GetPreviewPageByURL(pageURL))
                .Throws(new Exception("Preview service failure"));

            // Act
            var result = await ContentModelService.GetPagePreviewModel(pageURL);

            // Assert
            Assert.Null(result); // Should return null
            LoggerService.VerifyLogMustHaveHappened(LogLevel.Error,
                "Unable to get interim preview landing page."); // Verify logging with your logger extension
        }

        [Fact]
        public async Task GetPagePreviewModel_WhenPreviewPageIsNull_ReturnsNull()
        {
            // Arrange
            string pageURL = "unknownPageUrl";
            A.CallTo(() => ContentService.UpdatePreview()).Returns(Content);
            A.CallTo(() => ContentService.GetPreviewPageByURL(pageURL)).Returns(null); // Simulating a null preview page

            // Act
            var result = await ContentModelService.GetPagePreviewModel(pageURL);

            // Assert
            Assert.Null(result); // Should return null
        }

        [Fact]
        public void LandingModel_WhenInitialized_ShouldBeNull()
        {
            // Arrange
            // Create an instance of the ContentModelService
            var service = new ContentModelService(LoggerService, ContentService, htmlRenderer);

            // Act
            var landingModel = service.LandingModel;

            // Assert
            Assert.Null(landingModel); // Verify that it is null upon initialization
        }

        [Fact]
        public void GetMenuItems_Should_Return_Current_Menu_Selection_For_Home_page()
        {
            // Arrange
            // Create an instance of the ContentModelService
            var service = new ContentModelService(LoggerService, ContentService, htmlRenderer);

            // Act
            var currentMenus = service.GetMenuItems(ref _menus, RouteNames.Home);
            
            // Assert
            Assert.Equal(1, currentMenus.Count(x => x.IsCurrentPage));
        }
        [Fact]
        public void GetMenuItems_Should_Return_Current_Menu_Selection_For_Training_page()
        {
            // Arrange
            // Create an instance of the ContentModelService
            var service = new ContentModelService(LoggerService, ContentService, htmlRenderer);

            // Act
            var currentMenus = service.GetMenuItems(ref _menus, "find-funding-and-training");
            
            // Assert
            Assert.Equal(1, currentMenus.Count(x => x.IsCurrentPage));
        }
        [Fact]
        public void GetMenuItems_Should_Return_Current_Menu_Selection_For_Page_Not_Found_page()
        {
            // Arrange
            // Create an instance of the ContentModelService
            var service = new ContentModelService(LoggerService, ContentService, htmlRenderer);

            // Act
            var currentMenus = service.GetMenuItems(ref _menus, "find-funding");
            
            // Assert
            Assert.Equal(0, currentMenus.Count(x => x.IsCurrentPage));
        }
    }
}