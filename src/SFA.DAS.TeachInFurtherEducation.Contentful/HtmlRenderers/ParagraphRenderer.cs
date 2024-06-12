using Contentful.Core.Models;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.HtmlRenderers
{
    /// <summary>
    /// A renderer for a GDS paragraph.
    /// </summary>
    public class ParagraphRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        /// <summary>
        /// Initializes a new GdsParagraphRenderer
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public ParagraphRenderer(ContentRendererCollection rendererCollection)
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
        /// <returns>Returns true if the content is a paragraph, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is Paragraph;

        }

        /// <summary>
        /// Renders the content to an html p-tag.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The p-tag as a string.</returns>
        public async Task<string> RenderAsync(IContent content)
        {
            var paragraph = content as Paragraph;

            var sb = new StringBuilder();

            if(paragraph == null)
            {

                return string.Empty;

            }

            sb.Append("<p class=\"govuk-body\">");

            // Render any nested content within the paragraph
            foreach (var subContent in paragraph.Content)
            {

                // Check if the sub content contains html - process accordingly.
                if (subContent is Text && ContainsDivHtml(((Text)subContent).Value))
                {

                    // If it contains HTML, append it without HTML encoding

                    sb.Append(((Text)subContent).Value);

                }
                else
                {

                    

                    var renderer = _rendererCollection.GetRendererForContent(subContent);

                    sb.Append(await renderer.RenderAsync(subContent));

                }

            }

            sb.Append("</p>");

            return sb.ToString();

        }

        private static bool ContainsDivHtml(string input)
        {

            var doc = new HtmlDocument();

            doc.LoadHtml(input);

            return doc.DocumentNode.DescendantsAndSelf().Any(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase));

        }

    }

}
