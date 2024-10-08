using Xunit;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Contentful.Core.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using System;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Services
{
    public class ComponentServiceTests
    {
        private readonly ILogger _logger;

        public ComponentServiceTests()
        {
            _logger = A.Fake<ILogger>();
            ComponentService.Initialize(_logger, null, null);  // Initialize the service
        }

        [Fact]
        public void GetMediaImageSource_WhenMediaAssetIsNull_ReturnsEmptyString()
        {
            // Act
            var result = ComponentService.GetMediaImageSource(null);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetMediaImageSource_WhenMediaFileIsNull_ReturnsEmptyString()
        {
            // Arrange
            var mediaAsset = new Asset
            {
                File = null
            };

            // Act
            var result = ComponentService.GetMediaImageSource(mediaAsset);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetMediaImageSource_WhenUrlIsNullOrWhitespace_ReturnsEmptyString()
        {
            // Arrange
            var mediaAsset = new Asset
            {
                File = new File
                {
                    Url = "   "
                }
            };

            // Act
            var result = ComponentService.GetMediaImageSource(mediaAsset);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetMediaImageSource_WhenValidMediaAsset_ReturnsUrl()
        {
            // Arrange
            var expectedUrl = "https://example.com/media.jpg";
            var mediaAsset = new Asset
            {
                File = new File
                {
                    Url = expectedUrl
                }
            };

            // Act
            var result = ComponentService.GetMediaImageSource(mediaAsset);

            // Assert
            Assert.Equal(expectedUrl, result);
        }
    }
}
