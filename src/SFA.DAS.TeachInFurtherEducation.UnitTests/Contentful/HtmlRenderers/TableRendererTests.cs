using Contentful.Core.Models;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
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
    }
}