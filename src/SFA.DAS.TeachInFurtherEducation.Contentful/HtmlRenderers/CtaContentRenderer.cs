using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.GdsHtmlRenderers
{
    /// <summary>
    /// A renderer for a call to action box.
    /// </summary>
    public class CtaContentRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        /// <summary>
        /// Initializes a new GdsCtaContentRenderer.
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public CtaContentRenderer(ContentRendererCollection rendererCollection)
        {
            _rendererCollection = rendererCollection;
        }

        /// <summary>
        /// The order of this renderer in the collection.
        /// </summary>
        public int Order { get; set; } = 40;

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The list as a quote HTML string.</returns>
        public async Task<string> RenderAsync(IContent content)
        {
            var quote = content as Quote;
            StringBuilder sb = new StringBuilder("<section class=\"cx-cta-box\">");

            foreach (IContent subContent in quote!.Content)
            {
                sb.Append(await _rendererCollection
                    .GetRendererForContent(subContent)
                    .RenderAsync(subContent));
            }

            sb.Append("</section>");
            return sb.ToString()
                     .Replace("<cta>", "")
                     .Replace("</cta>", "")
                     .Replace("&lt;cta&gt;", "")
                     .Replace("&lt;/cta&gt;", "");
        }

        /// <summary>
        /// Whether or not this renderer supports the provided content.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>Returns true if the content is a quote, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            if (!(content is Quote))
                return false;

            var quote = content as Quote;
            var para = quote!.Content[0] as Paragraph;
            return para!.Content[0] is Text && ((Text)para!.Content[0]).Value.Trim().StartsWith("<cta>");
        }
    }
}
