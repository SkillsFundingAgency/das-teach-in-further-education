using Contentful.Core.Models;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
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
    }
}