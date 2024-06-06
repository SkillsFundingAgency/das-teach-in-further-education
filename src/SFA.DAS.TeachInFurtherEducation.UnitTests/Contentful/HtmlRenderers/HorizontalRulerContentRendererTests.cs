using Contentful.Core.Models;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.GdsHtmlRenderers
{
    public class GdsHorizontalRulerContentRendererTests
    {
        [Fact]
        public async Task ToHtml_GdsHorizontalRulerContentRendererTests()
        {
            var renderer = ContentService.CreateHtmlRenderer();
            var doc = new Document
            {
                Content = new List<IContent>
                {
                    new HorizontalRuler()
                }
            };

            var html = await renderer.ToHtml(doc);

            Assert.Equal("<hr class=\"govuk-section-break govuk-section-break--visible\">", html);
        }
    }
}
