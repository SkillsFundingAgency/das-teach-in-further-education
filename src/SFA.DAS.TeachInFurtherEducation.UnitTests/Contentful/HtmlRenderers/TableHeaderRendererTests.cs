using Contentful.Core.Models;
using FakeItEasy;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.GdsHtmlRenderers
{
    public class TableHeaderRendererTests
    {
        [Fact]
        public async Task ToHtml_GdsTableHeaderRendererTests()
        {
            var renderer = ContentService.CreateHtmlRenderer();
            var doc = new Document
            {
                Content = new List<IContent>
                {
                    new TableHeader
                    {
                        Content = new List<IContent>
                                    {
                                        new Text
                                        {
                                            Value = "TestTableHeadValue",
                                        }
                                    }
                    }
               }
            };

            var html = await renderer.ToHtml(doc);
            Assert.Equal("<th class=\"govuk-table__header\">TestTableHeadValue</th>", html);
        }


        [Fact]
        public async Task RenderAsync_InvalidContent_ThrowsArgumentException()
        {
            // Arrange
            var fakeRendererCollection = A.Fake<ContentRendererCollection>();
            var renderer = new SFA.DAS.TeachInFurtherEducation.Contentful.GdsHtmlRenderers.TableHeaderRenderer(fakeRendererCollection);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => renderer.RenderAsync(new Paragraph()));

            // Verify the exception message
            Assert.Equal("Invalid content passed to TableHeaderRenderer (Parameter 'content')", exception.Message);
        }

        [Fact]
        public async Task RenderAsync_TableHeaderWithDiv_IncludesHtmlWithoutEncoding()
        {
            // Arrange
            var renderer = ContentService.CreateHtmlRenderer();
            var doc = new Document
            {
                Content = new List<IContent>
                {
                    new TableHeader
                    {
                        Content = new List<IContent>
                        {
                            new Text
                            {
                                Value = "<div>TestTableHeaderDivContent</div>"
                            }
                        }
                    }
                }
            };

            // Act
            var html = await renderer.ToHtml(doc);

            // Assert
            Assert.Equal("<th class=\"govuk-table__header\"><div>TestTableHeaderDivContent</div></th>", html);
        }

    }
}