using Contentful.Core.Models;
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

            Assert.Equal("<p class=\"govuk-body\">Gobble</p>", html);
        }
    }
}