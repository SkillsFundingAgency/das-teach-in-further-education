using AutoFixture;
using AutoFixture.Kernel;
using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using FakeItEasy;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Exceptions;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces.Roots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.Services
{
    public class ContentServiceTests
    {
        public Fixture Fixture { get; }
        public Document Document { get; set; }
        public HtmlString ExpectedContent { get; set; }
        public IContentfulClientFactory ContentfulClientFactory { get; set; }
        public IContentfulClient ContentfulClient { get; set; }
        public IContentfulClient PreviewContentfulClient { get; set; }
        public HtmlRenderer HtmlRenderer { get; set; }
        public ILogger<ContentService> Logger { get; set; }
        public IPageService PageService { get; set; }
        public IPageContentService PageContentService { get; set; }

        public ContentService ContentService { get; set; }
        public CompareLogic CompareLogic { get; set; }

        public ContentServiceTests()
        {
            Fixture = new Fixture();

            (Document, ExpectedContent) = SampleDocumentAndExpectedContent();

            Fixture.Inject(Document);

            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            Fixture.Customizations.Add(
                new TypeRelay(
                    typeof(IContent),
                    typeof(Paragraph)));

            Fixture.Customizations.Add(
            new TypeRelay(
            typeof(IFieldValidator),
            typeof(Asset)));

            ContentfulClientFactory = A.Fake<IContentfulClientFactory>();
            ContentfulClient = A.Fake<IContentfulClient>();
            PreviewContentfulClient = A.Fake<IContentfulClient>();
            HtmlRenderer = A.Fake<HtmlRenderer>();
            Logger = A.Fake<ILogger<ContentService>>();

            A.CallTo(() => ContentfulClientFactory.ContentfulClient)
                .Returns(ContentfulClient);

            A.CallTo(() => ContentfulClientFactory.PreviewContentfulClient)
                .Returns(PreviewContentfulClient);

            PageService = A.Fake<IPageService>();
            PageContentService = A.Fake<IPageContentService>();

            CreateContentService();

            CompareLogic = new CompareLogic();
        }

        private (Document, HtmlString) SampleDocumentAndExpectedContent(int differentiator = 0)
        {
            return (new Document
            {
                NodeType = "heading-2",
                Data = new GenericStructureData(),
                Content = new List<IContent>
                {
                    new Heading2
                    {
                        Content = new List<IContent> {new Text {Value = $"Gobble{differentiator}" } }
                    }
                }
            }, new HtmlString($"<h2>Gobble{differentiator}</h2>"));
        }

        [Fact]
        public async Task Update_MissingContentfulClientThrowsExceptionTest()
        {
            A.CallTo(() => ContentfulClientFactory.ContentfulClient)
                .Returns(null);

            CreateContentService();

            await Assert.ThrowsAsync<ContentServiceException>(() => ContentService.Update());
        }

        [Fact]
        public async Task Update_ContentUpdatedEventIsRaisedTest()
        {
            bool eventWasRaised = false;
            ContentService.ContentUpdated += (sender, args) => { eventWasRaised = true; };

            var content = await ContentService.Update();

            Assert.True(eventWasRaised);
        }

        [Fact]
        public async Task Update_SameNumberOfPagesTest()
        {
            const int numberOfPages = 3;

            var contentPages = Fixture.CreateMany<PageRenamed>(numberOfPages);
            A.CallTo(() => PageService.GetAll(ContentfulClient))
                .Returns(contentPages);

            var content = await ContentService.Update();

            Assert.NotNull(content.Pagess);
            Assert.Equal(numberOfPages, content.Pagess.Count());
        }

        [Fact]
        public async Task Update_PageTest()
        {
            const int numberOfPages = 1;

            Fixture.Inject(ExpectedContent);
            var contentPages = Fixture.CreateMany<PageRenamed>(numberOfPages).ToArray();
            A.CallTo(() => PageService.GetAll(ContentfulClient))
                .Returns(contentPages);

            var content = await ContentService.Update();

            var actualPage = content.Pagess.FirstOrDefault();
            Assert.NotNull(actualPage);

            var expectedSourcePage = contentPages.First();
            Assert.Equal(expectedSourcePage.Title, actualPage.Title);
            Assert.Equal(expectedSourcePage.Url, actualPage.Url);
            Assert.Equal(ExpectedContent.Value, actualPage.Content.Value);
        }

        [Fact]
        public async Task UpdatePreview_MissingContentfulClientThrowsExceptionTest()
        {
            A.CallTo(() => ContentfulClientFactory.PreviewContentfulClient)
                .Returns(null);

            CreateContentService();

            await Assert.ThrowsAsync<ContentServiceException>(() => ContentService.UpdatePreview());
        }

        [Fact]
        public async Task UpdatePreview_PreviewContentUpdatedEventIsRaisedTest()
        {
            bool eventWasRaised = false;
            ContentService.PreviewContentUpdated += (sender, args) => { eventWasRaised = true; };

            await ContentService.UpdatePreview();

            Assert.True(eventWasRaised);
        }

        [Fact]
        public async Task UpdatePreview_SameNumberOfPagesTest()
        {
            const int numberOfPages = 3;

            var contentPages = Fixture.CreateMany<PageRenamed>(numberOfPages);
            A.CallTo(() => PageService.GetAll(PreviewContentfulClient))
                .Returns(contentPages);

            var content = await ContentService.UpdatePreview();

            Assert.NotNull(content.Pagess);
            Assert.Equal(numberOfPages, content.Pagess.Count());
        }

        [Fact]
        public async Task UpdatePreview_PageTest()
        {
            const int numberOfPages = 1;

            Fixture.Inject(ExpectedContent);
            var contentPages = Fixture.CreateMany<PageRenamed>(numberOfPages).ToArray();
            A.CallTo(() => PageService.GetAll(PreviewContentfulClient))
                .Returns(contentPages);

            var content = await ContentService.UpdatePreview();

            var actualPage = content.Pagess.FirstOrDefault();
            Assert.NotNull(actualPage);

            var expectedSourcePage = contentPages.First();
            Assert.Equal(expectedSourcePage.Title, actualPage.Title);
            Assert.Equal(expectedSourcePage.Url, actualPage.Url);
            Assert.Equal(ExpectedContent.Value, actualPage.Content.Value);
        }

        [Fact]
        public async Task Content_GetPageByURL()
        {

            var Pages = Fixture.CreateMany<Page>(3).ToArray();

            A.CallTo(() => PageContentService.GetAllPages(ContentfulClient))

                .Returns(Pages);

            var content = await ContentService.Update();

            var interimPage = ContentService.GetPageByURL(Pages.First().PageURL);

            var actualInterimPage = content.Pages.FirstOrDefault();

            Assert.NotNull(actualInterimPage);

            Assert.Equal(interimPage.PageTitle, actualInterimPage.PageTitle);

            Assert.Equal(interimPage.PageURL, actualInterimPage.PageURL);

        }

        [Fact]
        public void Content_IsGeneratedContentBeforeUpdate()
        {
            var compareResult = CompareLogic.Compare(new GeneratedContent(), ContentService.Content);

            Assert.True(compareResult.AreEqual);
        }

        [Fact]
        public async Task Content_IsNotGeneratedContentAfterUpdate()
        {
            await ContentService.Update();

            var compareResult = CompareLogic.Compare(new GeneratedContent(), ContentService.Content);

            Assert.False(compareResult.AreEqual);
        }

        [Fact]
        public void PreviewContent_IsNullBeforeUpdate()
        {
            Assert.Null(ContentService.PreviewContent);
        }

        [Fact]
        public async Task CreateHtmlRenderer_RenderingNullContent()
        {
            var renderer = ContentService.CreateHtmlRenderer();

            var nullResult = await renderer.ToHtml(null);

            Assert.Equal(string.Empty, nullResult);
        }

        [Fact]
        public async Task CreateHtmlRenderer_RenderingEmptyContent()
        {
            var renderer = ContentService.CreateHtmlRenderer();
            var emptyDocument = new Document();

            var emptyResult = await renderer.ToHtml(emptyDocument);

            Assert.Equal(string.Empty, emptyResult);
        }

        private void CreateContentService()
        {
            ContentService = new ContentService(
                ContentfulClientFactory,
                PageService,
                PageContentService,
                Logger);
        }

    }

}
