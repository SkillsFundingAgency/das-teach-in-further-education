using Contentful.Core.Models;
using FakeItEasy;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.GdsHtmlRenderers
{
    public class TableCellRendererTests
    {
        [Fact]
        public async Task ToHtml_GdsTableCellRendererTests()
        {
            var renderer = ContentService.CreateHtmlRenderer();
            var doc = new Document
            {
                Content = new List<IContent>
                {
                    new TableCell
                    {
                        Content = new List<IContent>
                        {
                            new Text
                            {
                                Value = "TestTableCell",
                            }
                        }
                    }
                }
            };

            var html = await renderer.ToHtml(doc);

            Assert.Equal("<td class=\"govuk-table__cell\">TestTableCell</td>", html);
        }

        [Fact]
        public async Task RenderAsync_InvalidContent_ThrowsArgumentException()
        {
            // Arrange
            var fakeRendererCollection = A.Fake<ContentRendererCollection>();
            var renderer = new SFA.DAS.TeachInFurtherEducation.Contentful.GdsHtmlRenderers.TableCellRenderer(fakeRendererCollection);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => renderer.RenderAsync(null));

            // Verify the exception message
            Assert.Equal("Invalid content passed to TableCellRenderer (Parameter 'content')", exception.Message);
        }


        [Fact]
        public async Task ToHtml_GdsTableCellRenderer_ContainsHtmlDiv_ReturnsRawHtml()
        {
            // Arrange: TableCell with HTML <div> content
            var renderer = ContentService.CreateHtmlRenderer();
            var doc = new Document
            {
                Content = new List<IContent>
                {
                    new TableCell
                    {
                        Content = new List<IContent>
                        {
                            new Text
                            {
                                Value = "<div>TestDivContent</div>",
                            }
                        }
                    }
                }
            };

            // Act: Render the document with raw HTML content
            var html = await renderer.ToHtml(doc);

            // Assert: Ensure the raw HTML is included without encoding
            Assert.Equal("<td class=\"govuk-table__cell\"><div>TestDivContent</div></td>", html);
        }
    }
}
