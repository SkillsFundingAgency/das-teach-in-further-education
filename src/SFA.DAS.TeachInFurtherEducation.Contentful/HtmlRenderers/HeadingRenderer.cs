using System;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.GdsHtmlRenderers
{
    /// <summary>
    /// A renderer for a heading.
    /// </summary>
    public class HeadingRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        /// <summary>
        /// Initializes a new HeadingRenderer.
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public HeadingRenderer(ContentRendererCollection rendererCollection)
        {
            _rendererCollection = rendererCollection;
        }

        /// <summary>
        /// The order of this renderer in the collection.
        /// </summary>
        public int Order { get; set; } = 50;

        /// <summary>
        /// Whether or not this renderer supports the provided content.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>Returns true if the content is a heading, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is Heading1 || content is Heading2 || content is Heading3 || content is Heading4;
        }

        // a sensible recommendation, but we want to stick as closely as possible to the renderer we're replacing,
        // for maximum compatibility within Contentful's HtmlRenderer
#pragma warning disable S4457
        /// <summary>
        /// Renders the content to a GDS compliant html h-tag.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The h-tag as a string.</returns>
        public async Task<string> RenderAsync(IContent content)
        {
            int headingSize;
            string gdsHeadingClassSize;

            switch (content)
            {
                case Heading1 _:
                    gdsHeadingClassSize = "xl-home";
                    headingSize = 1;
                    break;
                case Heading2 _:
                    gdsHeadingClassSize = "l";
                    headingSize = 2;
                    break;
                case Heading3 _:
                    gdsHeadingClassSize = "m";
                    headingSize = 3;
                    break;
                case Heading4 _:
                    gdsHeadingClassSize = "s";
                    headingSize = 4;
                    break;
                default:
                    throw new ArgumentException("Only H1-H4 are supported", nameof(content));
            }

            var heading = content as IHeading;

            var sb = new StringBuilder();
            sb.Append($"<h{headingSize} class=\"govuk-heading-{gdsHeadingClassSize}\">");

            // we assume HeadingN implements IHeading
            foreach (var subContent in heading!.Content)
            {
                var renderer = _rendererCollection.GetRendererForContent(subContent);
                sb.Append(await renderer.RenderAsync(subContent));
            }

            sb.Append($"</h{headingSize}>");
            return sb.ToString();
        }
#pragma warning restore S4457
    }
}
