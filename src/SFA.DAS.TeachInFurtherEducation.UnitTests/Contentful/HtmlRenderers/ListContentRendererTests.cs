using Contentful.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.GdsHtmlRenderers
{
    // https://github.com/contentful/contentful.net/blob/master/Contentful.Core.Tests/Models/Rendering/HtmlRenderTests.cs
    public class GdsListContentRendererTests
    {
        [Theory]
        [InlineData("<ul class=\"govuk-list govuk-list--bullet\"><li>testing</li></ul>", "unordered-list")]
        [InlineData("<ol class=\"govuk-list govuk-list--number\"><li>testing</li></ol>", "ordered-list")]
        public async Task ToHtml_GdsListTests(string expectedHtml, string listType)
        {
            //Arrange
            var renderer = ContentService.CreateHtmlRenderer();
            var doc = new Document();
            var list = new List();
            list.NodeType = listType;
            var listItem = new ListItem();
            var paragraph = new Paragraph();
            var text = new Text
            {
                Value = "testing"
            };
            paragraph.Content = new List<IContent> { text };
            listItem.Content = new List<IContent> { paragraph };
            list.Content = new List<IContent> { listItem };
            doc.Content = new List<IContent> { list };

            //Act
            var result = await renderer.ToHtml(doc);

            //Assert
            Assert.Equal(expectedHtml, result);
        }

    }
}
