using Contentful.Core.Models;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.GdsHtmlRenderers
{
    public class HeadingRendererTests
    {
        public HtmlRenderer HtmlRenderer { get; set; }
        public Document Document { get; set; }

        public HeadingRendererTests()
        {
            HtmlRenderer = ContentService.CreateHtmlRenderer();
            Document = new Document
            {
                Content = new List<IContent>()
            };
        }

        [Fact]
        public async Task ToHtml_GdsHeadingRenderer_Heading1Test()
        {
            Document.Content.Add(new Heading1
            {
                Content = new List<IContent> {new Text {Value = "Gobble"}}
            });

            var html = await HtmlRenderer.ToHtml(Document);

            Assert.Equal("<h1 class=\"govuk-heading-xl\">Gobble</h1>", html);
        }

        [Fact]
        public async Task ToHtml_GdsHeadingRenderer_Heading2Test()
        {
            Document.Content.Add(new Heading2
            {
                Content = new List<IContent> { new Text { Value = "Gobble" } }
            });

            var html = await HtmlRenderer.ToHtml(Document);

            Assert.Equal("<h2 class=\"govuk-heading-l\">Gobble</h2>", html);
        }

        [Fact]
        public async Task ToHtml_GdsHeadingRenderer_Heading3Test()
        {
            Document.Content.Add(new Heading3
            {
                Content = new List<IContent> { new Text { Value = "Gobble" } }
            });

            var html = await HtmlRenderer.ToHtml(Document);

            Assert.Equal("<h3 class=\"govuk-heading-m\">Gobble</h3>", html);
        }

        [Fact]
        public async Task ToHtml_GdsHeadingRenderer_Heading4Test()
        {
            Document.Content.Add(new Heading4
            {
                Content = new List<IContent> { new Text { Value = "Gobble" } }
            });

            var html = await HtmlRenderer.ToHtml(Document);

            Assert.Equal("<h4 class=\"govuk-heading-s\">Gobble</h4>", html);
        }

        [Fact]
        public async Task ToHtml_GdsHeadingRenderer_Heading5NotHandledByGdsHeadingRendererTest()
        {
            Document.Content.Add(new Heading5
            {
                Content = new List<IContent> { new Text { Value = "Gobble" } }
            });

            var html = await HtmlRenderer.ToHtml(Document);

            // 
            Assert.Equal("<h5>Gobble</h5>", html);
        }

        [Fact]
        public async Task ToHtml_GdsHeadingRenderer_Heading6NotHandledByGdsHeadingRendererTest()
        {
            Document.Content.Add(new Heading6
            {
                Content = new List<IContent> { new Text { Value = "Gobble" } }
            });

            var html = await HtmlRenderer.ToHtml(Document);

            // 
            Assert.Equal("<h6>Gobble</h6>", html);
        }
    }
}
