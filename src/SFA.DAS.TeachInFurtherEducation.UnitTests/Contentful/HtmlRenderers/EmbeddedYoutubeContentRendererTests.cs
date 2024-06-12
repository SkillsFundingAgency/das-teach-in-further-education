using Contentful.Core.Models;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.GdsHtmlRenderers
{
    public class EmbeddedYoutubeContentRendererTests
    {
        public HtmlRenderer HtmlRenderer { get; set; }

        public EmbeddedYoutubeContentRendererTests()
        {
            HtmlRenderer = ContentService.CreateHtmlRenderer();
        }

        [Fact]
        public async Task ToHtml_GdsEmbeddedYoutubeContentRenderer_WhitespaceAcceptedAndTrimmedTest()
        {
            const string sourceParagraphText = "  <iframe width=\"3840\" height=\"2160\" src=\"https://www.youtube-nocookie.com/embed/03gpzFZadcQ\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>  ";
            const string expectedHtml = "<p class=\"govuk-body\"><div class=\"app-video-container\" style=\"--aspect-ratio: 56.25%\"><iframe width=\"3840\" height=\"2160\" src=\"https://www.youtube-nocookie.com/embed/03gpzFZadcQ\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe></div></p>";

            var document = CreateYouTubeDocument(sourceParagraphText);

            string html = await HtmlRenderer.ToHtml(document);

            Assert.Equal(expectedHtml, html);
        }

        [Fact]
        public async Task ToHtml_GdsEmbeddedYoutubeContentRenderer_ForceNoCookieTest()
        {
            const string sourceParagraphText = "<iframe width=\"3840\" height=\"2160\" src=\"https://www.youtube.com/embed/03gpzFZadcQ\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>";
            const string expectedHtml = "<p class=\"govuk-body\"><div class=\"app-video-container\" style=\"--aspect-ratio: 56.25%\"><iframe width=\"3840\" height=\"2160\" src=\"https://www.youtube-nocookie.com/embed/03gpzFZadcQ\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe></div></p>";

            var document = CreateYouTubeDocument(sourceParagraphText);

            string html = await HtmlRenderer.ToHtml(document);

            Assert.Equal(expectedHtml, html);
        }

        [Theory]
        [InlineData(1280,  720, "56.25")]
        [InlineData(3840, 2160, "56.25")]
        [InlineData(1280,  852, "66.56")]
        [InlineData(1280,  544, "42.50")]
        [InlineData(1280, 1024, "80.00")]
        public async Task ToHtml_GdsEmbeddedYoutubeContentRenderer_AspectRatioTests(int width, int height, string expectedAspectRatio)
        {
            string sourceParagraphText = $"<iframe width=\"{width}\" height=\"{height}\" src=\"https://www.youtube-nocookie.com/embed/03gpzFZadcQ\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>";
            string expectedHtml = $"<p class=\"govuk-body\"><div class=\"app-video-container\" style=\"--aspect-ratio: {expectedAspectRatio}%\"><iframe width=\"{width}\" height=\"{height}\" src=\"https://www.youtube-nocookie.com/embed/03gpzFZadcQ\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe></div></p>";

            var document = CreateYouTubeDocument(sourceParagraphText);

            var html = await HtmlRenderer.ToHtml(document);

            Assert.Equal(expectedHtml, html);
        }

        [Fact]
        public async Task ToHtml_GdsEmbeddedYoutubeContentRenderer_AspectRatio_NoWidthTests()
        {
            string sourceParagraphText = "<iframe height=\"100\" src=\"https://www.youtube-nocookie.com/embed/03gpzFZadcQ\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>";
            string expectedHtml = "<p class=\"govuk-body\"><div class=\"app-video-container\" style=\"--aspect-ratio: 56.25%\"><iframe height=\"100\" src=\"https://www.youtube-nocookie.com/embed/03gpzFZadcQ\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe></div></p>";

            var document = CreateYouTubeDocument(sourceParagraphText);

            var html = await HtmlRenderer.ToHtml(document);

            Assert.Equal(expectedHtml, html);
        }

        [Fact]
        public async Task ToHtml_GdsEmbeddedYoutubeContentRenderer_AspectRatio_NoHeightTests()
        {
            string sourceParagraphText = "<iframe width=\"100\" src=\"https://www.youtube-nocookie.com/embed/03gpzFZadcQ\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>";
            string expectedHtml = "<p class=\"govuk-body\"><div class=\"app-video-container\" style=\"--aspect-ratio: 56.25%\"><iframe width=\"100\" src=\"https://www.youtube-nocookie.com/embed/03gpzFZadcQ\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe></div></p>";

            var document = CreateYouTubeDocument(sourceParagraphText);

            var html = await HtmlRenderer.ToHtml(document);

            Assert.Equal(expectedHtml, html);
        }

        private Document CreateYouTubeDocument(string embeddedYouTube)
        {
            return new Document
            {
                Content = new List<IContent>
                {
                    new Paragraph
                    {
                        Content = new List<IContent>
                        {
                            new Text
                            {
                                Value = embeddedYouTube
                            }
                        }
                    }
                }
            };
        }
    }
}
