using Contentful.Core.Models;
using Newtonsoft.Json.Linq;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.GdsHtmlRenderers
{
    public class EntryStructureRendererTests
    {
        public HtmlRenderer HtmlRenderer { get; set; }
        public Document Document { get; set; }
        public EntryStructure EntryStructure { get; set; }

        public EntryStructureRendererTests()
        {
            HtmlRenderer = ContentService.CreateHtmlRenderer();

            var jObject = new JObject
            {
                ["pageTitle"] = "title",
                ["pageUrl"] = "https://example.com"
            };

            EntryStructure = new EntryStructure
            {
                NodeType = "entry-hyperlink",
                Data = new EntryStructureData
                {
                    Target = new CustomNode
                    {
                        JObject = jObject
                    }
                }
            };
            
            Document = new Document
            {
                Content = new List<IContent>
                {
                    EntryStructure
                }
            };
        }

        [Fact]
        public async Task ToHtml_GdsParagraphRenderer_SameTabTest()
        {
            EntryStructure.Content = new List<IContent>
            {
                new Text
                {
                    Value = "text",
                }
            };

            var html = await HtmlRenderer.ToHtml(Document);

            Assert.Equal("<a href=\"https://example.com\" title=\"title\" class=\"govuk-link\">text</a>", html);
        }
    }
}
