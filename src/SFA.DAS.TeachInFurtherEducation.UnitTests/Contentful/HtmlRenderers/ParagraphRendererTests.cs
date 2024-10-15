using Contentful.Core.Models;
using SFA.DAS.TeachInFurtherEducation.Contentful.GdsHtmlRenderers;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.GdsHtmlRenderers
{
    public class ParagraphContentRendererTests
    {
        [Fact]
        public async Task ToHtml_GdsParagraphRendererTests()
        {
            var renderer = ContentService.CreateHtmlRenderer();
            var doc = new Document
            {
                Content = new List<IContent>
                {
                    new Paragraph
                    {
                        Content = new List<IContent>
                        {
                            new Text
                            {
                                Value = "Gobble",
                            }
                        }
                    }
                }
            };

            var html = await renderer.ToHtml(doc);

            Assert.Equal("<p class=\"govuk-body govuk-!-margin-bottom-6\">Gobble</p>", html);
        }

        [Fact]
        public async Task RenderAsync_ReturnsEmptyString_WhenContentIsNotParagraph()
        {
            // Arrange
            var renderer = new GdsParagraphRenderer(new ContentRendererCollection());
            var headingContent = new Heading1 // Content other than Paragraph
            {
                Content = new List<IContent>
                {
                    new Text { Value = "Heading" }
                }
            };

            // Act
            var html = await renderer.RenderAsync(headingContent);

            // Assert
            Assert.Equal(string.Empty, html);
        }


        [Fact]
        public async Task RenderAsync_AppendsHtmlDirectly_WhenTextContainsDivTag()
        {
            // Arrange
            var renderer = ContentService.CreateHtmlRenderer();
            var doc = new Document
            {
                Content = new List<IContent>
                {
                    new Paragraph
                    {
                        Content = new List<IContent>
                        {
                            new Text
                            {
                                Value = "<div>This is a div content</div>"
                            }
                        }
                    }
                }
            };

            // Act
            var html = await renderer.ToHtml(doc);

            // Assert
            Assert.Equal("<p class=\"govuk-body govuk-!-margin-bottom-6\"><div>This is a div content</div></p>", html);
        }


    }
}