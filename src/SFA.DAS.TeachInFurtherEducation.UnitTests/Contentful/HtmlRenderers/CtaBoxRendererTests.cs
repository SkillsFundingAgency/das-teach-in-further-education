using Contentful.Core.Models;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Contentful.GdsHtmlRenderers
{
    public class CtaBoxRendererTests
    {
        [Fact]
        public async Task ToHtml_GdsGdsCtaBoxRendererTests()
        {
            var renderer = ContentService.CreateHtmlRenderer();
            var doc = new Document
            {
                Content = new List<IContent>
                {
                    new Quote
                    {
                        Content = new List<IContent>
                        {
                            new Paragraph
                            {
                                Content = new List<IContent>
                                {
                            new Text
                            {
                                        Value = "<cta>This is a"
                                    }
                                }
                            },
                            new Paragraph
                            {
                                Content = new List<IContent>
                                {
                                    new Text
                                    {
                                        Value = "Call To Action"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var html = await renderer.ToHtml(doc);

            Assert.Equal("<section class=\"cx-cta-box\"><p class=\"govuk-body-l govuk-!-margin-bottom-6\">This is a</p><p class=\"govuk-body-l govuk-!-margin-bottom-6\">Call To Action</p></section>", html);
        }
    }
}
