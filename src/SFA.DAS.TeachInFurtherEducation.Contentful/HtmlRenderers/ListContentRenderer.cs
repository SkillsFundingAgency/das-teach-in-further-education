using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.HtmlRenderers
{
    /// <summary>
    /// A renderer for a GDS list.
    /// </summary>
    public class ListContentRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        /// <summary>
        /// Initializes a new GdsListContentRenderer.
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public ListContentRenderer(ContentRendererCollection rendererCollection)
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
        /// <returns>Returns true if the content is a list, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is List;
        }

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The list as a ul or ol HTML string.</returns>
        public async Task<string> RenderAsync(IContent content)
        {
            var list = content as List;
            string listTagType = "ul";
            string gdsClasses = "govuk-list govuk-list--bullet";
            if (list!.NodeType == "ordered-list")
            {
                listTagType = "ol";
                gdsClasses = "govuk-list govuk-list--number";
            }

            var sb = new StringBuilder();

            sb.Append($"<{listTagType} class=\"{gdsClasses}\">");

            foreach (var subContent in list.Content)
            {
                var renderer = _rendererCollection.GetRendererForContent(subContent);
                sb.Append(await renderer.RenderAsync(subContent));
            }

            sb.Append($"</{listTagType}>");

            return sb.ToString();
        }
    }
}