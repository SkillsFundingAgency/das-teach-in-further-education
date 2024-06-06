using System;
using System.Linq;
using Contentful.Core;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using System.Net.Http;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.Services
{
    public class ContentfulClientFactoryTests
    {
        public IContentfulClient ContentfulClient { get; set; }
        public IContentfulClient PreviewContentfulClient { get; set; }

        public ContentfulClientFactoryTests()
        {
            var httpClient = new HttpClient();
            ContentfulClient = new ContentfulClient(httpClient, "", "", "", false);
            PreviewContentfulClient = new ContentfulClient(httpClient, "", "", "", true);
        }

        [Fact]
        public void ContentfulClient_ClientAndPreviewClient_ClientReturnedTest()
        {
            var bothTypesOfClient = new[] {ContentfulClient, PreviewContentfulClient};
            var contentfulClientFactory = new ContentfulClientFactory(bothTypesOfClient);

            Assert.Equal(ContentfulClient, contentfulClientFactory.ContentfulClient);
        }

        [Fact]
        public void PreviewContentfulClient_ClientAndPreviewClient_PreviewClientReturnedTest()
        {
            var bothTypesOfClient = new[] { ContentfulClient, PreviewContentfulClient };
            var contentfulClientFactory = new ContentfulClientFactory(bothTypesOfClient);

            Assert.Equal(PreviewContentfulClient, contentfulClientFactory.PreviewContentfulClient);
        }

        [Fact]
        public void ContentfulClient_OnlyClient_ClientReturnedTest()
        {
            var bothTypesOfClient = new[] { ContentfulClient };
            var contentfulClientFactory = new ContentfulClientFactory(bothTypesOfClient);

            Assert.Equal(ContentfulClient, contentfulClientFactory.ContentfulClient);
        }

        [Fact]
        public void PreviewContentfulClient_OnlyPreviewClient_PreviewClientReturnedTest()
        {
            var bothTypesOfClient = new[] { PreviewContentfulClient };
            var contentfulClientFactory = new ContentfulClientFactory(bothTypesOfClient);

            Assert.Equal(PreviewContentfulClient, contentfulClientFactory.PreviewContentfulClient);
        }

        [Fact]
        public void ContentfulClient_MoreThanOneClient_ThrowsTest()
        {
            var bothTypesOfClient = new[] { ContentfulClient, ContentfulClient };

            Assert.ThrowsAny<Exception>(() => new ContentfulClientFactory(bothTypesOfClient));
        }

        [Fact]
        public void ContentfulClient_MoreThanOnePreviewClient_ThrowsTest()
        {
            var bothTypesOfClient = new[] { PreviewContentfulClient, PreviewContentfulClient };

            Assert.ThrowsAny<Exception>(() => new ContentfulClientFactory(bothTypesOfClient));
        }

        [Fact]
        public void ContentfulClient_NoClient_ThrowsTest()
        {
            var bothTypesOfClient = Enumerable.Empty<IContentfulClient>();

            Assert.ThrowsAny<Exception>(() => new ContentfulClientFactory(bothTypesOfClient));
        }
    }
}
