using System;
using AutoFixture;
using AutoFixture.Kernel;
using ContentfulModels = Contentful.Core.Models;
using FakeItEasy;
using System.Linq;
using Xunit;
using IContent = SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces.IContent;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Collections.Generic;


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

            var TestPage = new Page("TestURL", "TestTitle", "TestTemplate",null, new  List<ContentfulModels.IContent>(), new HtmlString(""));
            var HomePage = new Page("HomePageURL", "HolePageTitle", "HomeTemplate", null, new List<ContentfulModels.IContent>(), new HtmlString(""));

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
    }
}
