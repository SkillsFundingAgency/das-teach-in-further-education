using System;
using System.Text;
using System.Text.RegularExpressions;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.Services
{
    public class ViewRenderServiceTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("Hello World", "Hello World")]
        [InlineData("No tags here!", "No tags here!")]
        public void ReturnsOriginal_WhenNoHtmlTags(string input, string expected)
        {
            var result = ViewRenderService.StripHTML(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("<b>Hello</b>", "Hello")]
        [InlineData("<div>Content</div>", "Content")]
        [InlineData("Before<span>Middle</span>After", "BeforeMiddleAfter")]
        public void RemovesSimpleTags(string input, string expected)
        {
            var result = ViewRenderService.StripHTML(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("<p><em>Nested</em> Tags</p>", "Nested Tags")]
        [InlineData("<div><div>Deep</div></div>", "Deep")]
        public void HandlesNestedTags(string input, string expected)
        {
            var result = ViewRenderService.StripHTML(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Line<br>Break", "LineBreak")]
        [InlineData("Image<img src='test.png'>", "Image")]
        [InlineData("Self-closing<hr />Tag", "Self-closingTag")]
        public void HandlesSelfClosingAndMalformedTags(string input, string expected)
        {
            var result = ViewRenderService.StripHTML(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HandlesMixedContentWithMultipleTags()
        {
            var input = "<h1>Title</h1><p>Paragraph <strong>with</strong> text</p>";
            var expected = "TitleParagraph with text";
            var result = ViewRenderService.StripHTML(input);
            Assert.Equal(expected, result);
        }
        
        [Fact]
        public void ReturnsEmptyString_WhenInputIsOnlyTags()
        {
            var input = "<div></div><p></p>";
            var result = ViewRenderService.StripHTML(input);
            Assert.Equal("", result);
        }

        [Fact]
        public void ThrowsArgumentNullException_WhenInputIsNull()
        {
            string input = null;
            Assert.Throws<ArgumentNullException>(() => ViewRenderService.StripHTML(input));
        }

        [Fact]
        public void CompletesWithinTimeout_ForLargeInput()
        {
            // Arrange - Build large but safe input (1MB)
            var sb = new StringBuilder();
            for (int i = 0; i < 10000; i++)
            {
                sb.Append($"<tag{i}>Text</tag{i}>");
            }
            var input = sb.ToString();

            // Act & Assert (Should complete within 1 second timeout)
            var result = ViewRenderService.StripHTML(input);
            Assert.NotNull(result);
            Assert.Contains("Text", result);
        }
    }
}