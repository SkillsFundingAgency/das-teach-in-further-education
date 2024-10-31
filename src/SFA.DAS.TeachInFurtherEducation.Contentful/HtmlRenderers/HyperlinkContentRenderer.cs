using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.GdsHtmlRenderers
{
    /// <summary>
    /// A renderer for a GDS compliant hyperlink.
    /// </summary>
    public partial class HyperlinkContentRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        private Regex _newTabExp = NewTabExpression();

        /// <summary>
        /// Initializes a new GdsHyperlinkContentRenderer.
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public HyperlinkContentRenderer(ContentRendererCollection rendererCollection)
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
        /// <returns>Returns true if the content is a hyperlink, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is Hyperlink;
        }

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The GDS compliant a tag as a string.</returns>
        public async Task<string> RenderAsync(IContent content)
        {
            var link = content as Hyperlink;
            var sb = new StringBuilder();

            // we assume we only get asked to render what we've said we support
            sb.Append($"<a href=\"{link!.Data.Uri}\" title=\"{link.Data.Title}\" class=\"govuk-link\"");

            // if the text content of the link ends with "(opens in new tab)", then we make the link open in a new tab
            string? firstTextValue = link.Content.OfType<Text>().FirstOrDefault()?.Value;

            if (firstTextValue != null && _newTabExp.IsMatch(firstTextValue))
                sb.Append(" rel=\"noreferrer noopener\" target=\"_blank\"");

            sb.Append('>');

            // this common code could go in a base class
            foreach (var subContent in link.Content)
            {
                var renderer = _rendererCollection.GetRendererForContent(subContent);
                sb.Append(await renderer.RenderAsync(subContent));
            }

            sb.Append("</a>");

            return sb.ToString();
        }

        [ExcludeFromCodeCoverage]
        [GeneratedRegex("[(]\\s*open[^)]+new\\stab\\s*[)]", RegexOptions.IgnoreCase, 200)]
        private static partial Regex NewTabExpression();
    }
}
