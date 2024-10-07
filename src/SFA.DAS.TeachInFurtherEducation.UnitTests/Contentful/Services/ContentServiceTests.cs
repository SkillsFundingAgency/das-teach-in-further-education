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
using System.Net;
using System.Net.Http;
using System.Threading;
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
        public IAssetDownloader AssetDownloader { get; set; }
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

            AssetDownloader = A.Fake<IAssetDownloader>();
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
        public async Task GetPageByURL_WhenExceptionThrown_LogsErrorAndReturnsNull()
        {
            // Arrange
            var logger = A.Fake<ILogger<ContentService>>();
            var invalidUrl = "invalid-url";

            var contentService = new ContentService(
                ContentfulClientFactory,
                PageService,
                PageContentService,
                AssetDownloader,
                logger);

            A.CallTo(() => PageContentService.GetAllPages(ContentfulClient))
                .Throws<Exception>();

            // Act
            var result = contentService.GetPageByURL(invalidUrl);

            // Assert
            Assert.Null(result);

            // Verify the logger call using your extension
            logger.VerifyLogMustHaveHappened(LogLevel.Error, "Unable to get page by url: invalid-url");
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

        [Fact]
        public async Task GetAssetsByTags_ShouldReturnAssets_WhenValidTagsProvided()
        {
            // Arrange
            var tags = new[] { "tag1", "tag2" };

            var asset1 = new Asset
            {
                SystemProperties = new SystemProperties
                {
                    Id = "asset1",
                    UpdatedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                File = new File
                {
                    Url = "https://example.com/asset1.jpg",
                    FileName = "asset1.jpg"
                }
            };

            var asset2 = new Asset
            {
                SystemProperties = new SystemProperties
                {
                    Id = "asset2",
                    UpdatedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                File = new File
                {
                    Url = "https://example.com/asset2.jpg",
                    FileName = "asset2.jpg"
                }
            };

            var collection1 = new ContentfulCollection<Asset>()
            {
                Items = new List<Asset> { asset1 },
                Total = 1, 
                Skip = 0, 
                Limit = 100
            };

            var collection2 = new ContentfulCollection<Asset>()
            {
                Items = new List<Asset> { asset2 },
                Total = 1,
                Skip = 0,
                Limit = 100
            };

            // Fake GetAssetsByTags for each tag
            A.CallTo(() => ContentfulClient.GetAssets($"?metadata.tags.sys.id[in]={tags[0]}", A<CancellationToken>._))
                .Returns(Task.FromResult(collection1));

            A.CallTo(() => ContentfulClient.GetAssets($"?metadata.tags.sys.id[in]={tags[1]}", A<CancellationToken>._))
                .Returns(Task.FromResult(collection2));

            // Fake GetAsset for each asset ID
            A.CallTo(() => ContentfulClient.GetAsset("asset1", default(string), A<CancellationToken>._)).Returns(Task.FromResult(asset1));
            A.CallTo(() => ContentfulClient.GetAsset("asset2", default(string), A<CancellationToken>._)).Returns(Task.FromResult(asset2));

            // Fake DownloadAssetContentAsync for each asset URL
            var content1 = new byte[] { 1, 2, 3 };
            var content2 = new byte[] { 4, 5, 6 };

            A.CallTo(() => AssetDownloader.DownloadAssetContentAsync("https://example.com/asset1.jpg"))
                .Returns(Task.FromResult<byte[]>(content1));

            A.CallTo(() => AssetDownloader.DownloadAssetContentAsync("https://example.com/asset2.jpg"))
                .Returns(Task.FromResult<byte[]>(content2));

            // Act
            var result = await ContentService.GetAssetsByTags(tags);

            // Assert
            Assert.Equal(2, result.Count);

            var returnedAsset1 = result.FirstOrDefault(a => a.Metadata.Id == "asset1");
            Assert.NotNull(returnedAsset1);
            Assert.Equal("asset1.jpg", returnedAsset1.Metadata.Filename);
            Assert.Equal("https://example.com/asset1.jpg", returnedAsset1.Metadata.Url);
            Assert.Equal(asset1.SystemProperties.UpdatedAt, returnedAsset1.Metadata.LastUpdated);
            Assert.True(returnedAsset1.Content.SequenceEqual(content1));

            var returnedAsset2 = result.FirstOrDefault(a => a.Metadata.Id == "asset2");
            Assert.NotNull(returnedAsset2);
            Assert.Equal("asset2.jpg", returnedAsset2.Metadata.Filename);
            Assert.Equal("https://example.com/asset2.jpg", returnedAsset2.Metadata.Url);
            Assert.Equal(asset2.SystemProperties.UpdatedAt, returnedAsset2.Metadata.LastUpdated);
            Assert.True(returnedAsset2.Content.SequenceEqual(content2));
        }

        [Fact]
        public void GetPreviewPageByURL_PreviewContentIsNull_ReturnsNull()
        {
            // Arrange
            var contentService = new ContentService(ContentfulClientFactory, PageService, PageContentService, AssetDownloader, Logger);

            // Act
            var result = contentService.GetPreviewPageByURL("some-url");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAssetsByTags_NoTagsProvided_ThrowsArgumentException()
        {
            // Arrange
            var contentService = new ContentService(ContentfulClientFactory, PageService, PageContentService, AssetDownloader, Logger);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => contentService.GetAssetsByTags());
            Assert.Equal("At least one tag must be provided. (Parameter 'tags')", exception.Message);
        }

        [Fact]
        public async Task GetAssetsByTags_AssetContentIsNull_ReturnsNoAsset()
        {
            // Arrange
            var tags = new[] { "tag1" };

            // Create an asset with SystemProperties and File but no content
            var asset = new Asset
            {
                SystemProperties = new SystemProperties
                {
                    Id = "asset1",
                    UpdatedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                File = new File { Url = "https://example.com/asset1.jpg" }
            };

            var collection = new ContentfulCollection<Asset>
            {
                Items = new List<Asset> { asset }
            };

            // Fake calls
            A.CallTo(() => ContentfulClient.GetAssets($"?metadata.tags.sys.id[in]=tag1", A<CancellationToken>._))
                .Returns(Task.FromResult(collection));

            // Simulate null content for the asset
            A.CallTo(() => AssetDownloader.DownloadAssetContentAsync("https://example.com/asset1.jpg"))
                .Returns(Task.FromResult<byte[]>(null));

            var contentService = new ContentService(ContentfulClientFactory, PageService, PageContentService, AssetDownloader, Logger);

            // Act
            var result = await contentService.GetAssetsByTags(tags);

            // Assert that no assets are returned since the content is null
            Assert.Empty(result);
        }

        private void CreateContentService()
        {
            ContentService = new ContentService(
                ContentfulClientFactory,
                PageService,
                PageContentService, 
                AssetDownloader,
                Logger);
        }

    }

}
