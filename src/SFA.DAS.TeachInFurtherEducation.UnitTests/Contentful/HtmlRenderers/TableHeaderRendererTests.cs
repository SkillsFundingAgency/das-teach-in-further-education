using Contentful.Core.Models;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
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
    }
}