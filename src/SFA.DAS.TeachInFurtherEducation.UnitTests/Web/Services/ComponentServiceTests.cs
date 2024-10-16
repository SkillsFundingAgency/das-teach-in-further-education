using Xunit;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Contentful.Core.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using System;
using Microsoft.AspNetCore.Html;

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

        [Theory]
        [InlineData("<p class=\"stylish\">some text</p>", "xyz123", "<p id=\"xyz123\" class=\"stylish\">some text</p>")]
        [InlineData("<div class=\"stylish\">some text</div>", "xyz123", "<div id=\"xyz123\" class=\"stylish\">some text</div>")]
        [InlineData("       <div class=\"stylish\">some text</div>", "xyz123", "       <div id=\"xyz123\" class=\"stylish\">some text</div>")]
        [InlineData("<p id=\"existing-id\" class=\"stylish\">some text</p>", "should-not-be-added", "<p id=\"existing-id\" class=\"stylish\">some text</p>")]
        [InlineData("<p class=\"stylish\" id=\"existing-id-at-the-end-of-the-element\">some text</p>", "should-not-be-added", "<p class=\"stylish\" id=\"existing-id-at-the-end-of-the-element\">some text</p>")]
        [InlineData("       <p class=\"stylish\" id=\"existing-id-at-the-end-of-the-element\">some text</p>", "should-not-be-added", "       <p class=\"stylish\" id=\"existing-id-at-the-end-of-the-element\">some text</p>")]

        public void AddIdToTopElement_WithHtmlString_AddsIdWhereNexessary(string source, string id, string expected)
        {
            // Arragne

            // Act
            var result = ComponentService.AddIdToTopElement(new HtmlString(source), id);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData("<p class=\"stylish\">some text</p>", "xyz123", "<p id=\"xyz123\" class=\"stylish\">some text</p>")]
        [InlineData("<div class=\"stylish\">some text</div>", "xyz123", "<div id=\"xyz123\" class=\"stylish\">some text</div>")]
        [InlineData("       <div class=\"stylish\">some text</div>", "xyz123", "       <div id=\"xyz123\" class=\"stylish\">some text</div>")]
        [InlineData("<p id=\"existing-id\" class=\"stylish\">some text</p>", "should-not-be-added", "<p id=\"existing-id\" class=\"stylish\">some text</p>")]
        [InlineData("<p class=\"stylish\" id=\"existing-id-at-the-end-of-the-element\">some text</p>", "should-not-be-added", "<p class=\"stylish\" id=\"existing-id-at-the-end-of-the-element\">some text</p>")]
        [InlineData("       <p class=\"stylish\" id=\"existing-id-at-the-end-of-the-element\">some text</p>", "should-not-be-added", "       <p class=\"stylish\" id=\"existing-id-at-the-end-of-the-element\">some text</p>")]

        public void AddIdToTopElement_WithString_AddsIdWhereNexessary(string source, string id, string expected)
        {
            // Arragne

            // Act
            var result = ComponentService.AddIdToTopElement(source, id);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public void AddIdToTopElement_WithNull_ReturnsNull()
        {
            // Arragne
            string source = null;

            // Act
            var result = ComponentService.AddIdToTopElement(source, "testId");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ToHtmlString_WithNull_ReturnsNull()
        {
            // Act
            var result = ComponentService.ToHtmlString(null);

            // Assert
            Assert.Null(result);
        }
    }
}
