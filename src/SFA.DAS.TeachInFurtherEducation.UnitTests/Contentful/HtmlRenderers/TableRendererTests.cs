using Contentful.Core.Models;
using FakeItEasy;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.GdsHtmlRenderers
{
    public class TableRendererTests
    {
        [Fact]
        public async Task ToHtml_GdsTableRendererTests()
        {
            var renderer = ContentService.CreateHtmlRenderer();
            var doc = new Document
            {
                Content = new List<IContent>
                {
                    new Table
                    {
                        Content = new List<IContent>
                        {
                            new TableRow
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
                            }
                        }
                    }
               }
            };

            var html = await renderer.ToHtml(doc);
            Assert.Equal("<table class=\"govuk-table\"><tr><td class=\"govuk-table__cell\">TestTableCell</td></tr></table>", html);
        }

        [Fact]
        public async Task RenderAsync_InvalidContent_ThrowsArgumentException()
        {
            // Arrange
            var fakeRendererCollection = A.Fake<ContentRendererCollection>();
            var renderer = new SFA.DAS.TeachInFurtherEducation.Contentful.GdsHtmlRenderers.TableRenderer(fakeRendererCollection);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => renderer.RenderAsync(new Paragraph()));

            // Verify the exception message
            Assert.Equal("Invalid content passed to TableRenderer (Parameter 'content')", exception.Message);
        }
    }
}