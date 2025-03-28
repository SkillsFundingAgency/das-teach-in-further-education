﻿using Contentful.Core.Models;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.GdsHtmlRenderers
{
    public class HyperlinkContentRendererTests
    {
        public HtmlRenderer HtmlRenderer { get; set; }
        public Document Document { get; set; }
        public Hyperlink Hyperlink { get; set; }

        public HyperlinkContentRendererTests()
        {
            HtmlRenderer = ContentService.CreateHtmlRenderer();

            Hyperlink = new Hyperlink
            {
                Data = new HyperlinkData
                {
                    Title = "title",
                    Uri = "https://example.com"
                }
            };

            Document = new Document
            {
                Content = new List<IContent>
                {
                    Hyperlink                    
                }
            };
        }

        [Fact]
        public async Task ToHtml_GdsParagraphRenderer_SameTabTest()
        {
            Hyperlink.Content = new List<IContent>
            {
                new Text
                {
                    Value = "text",
                }
            };

            var html = await HtmlRenderer.ToHtml(Document);

            Assert.Equal("<a href=\"https://example.com\" title=\"title\" class=\"govuk-link\">text</a>", html);
        }

        [Theory]
        [InlineData("(opens in a new tab)")]
        [InlineData("opens in a new tab")]
        [InlineData("this is a link - opens in a new tab")]
        [InlineData("this is a link-opens in a new tab")]
        [InlineData("this is a link opens in a new tab")]
        [InlineData("(opens in new tab)")]
        [InlineData("(   opens in a new tab   )")]
        [InlineData("(OpEnS In A nEw TaB)")]
        [InlineData("OpEnS In A nEw TaB")]
        public async Task ToHtml_GdsParagraphRenderer_NewTabTest(string suffix)
        {
            Hyperlink.Content = new List<IContent>
            {
                new Text
                {
                    Value = $"text {suffix}",
                }
            };

            var html = await HtmlRenderer.ToHtml(Document);

            Assert.Equal($"<a href=\"https://example.com\" title=\"title\" class=\"govuk-link\" rel=\"noreferrer noopener\" target=\"_blank\">text {suffix}</a>", html);
        }
    }
}
