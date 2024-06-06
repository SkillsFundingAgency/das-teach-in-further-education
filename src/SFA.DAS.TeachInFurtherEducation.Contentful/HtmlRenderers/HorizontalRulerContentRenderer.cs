using System.Threading.Tasks;
using Contentful.Core.Models;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.HtmlRenderers
{
    /// <summary>
    /// A content renderer that renders a GDS horizontal ruler.
    /// </summary>
    public class HorizontalRulerContentRenderer : IContentRenderer
    {
        /// <summary>
        /// The order of this renderer in the collection.
        /// </summary>
        public int Order { get; set; } = 50;

        /// <summary>
        /// Whether or not this renderer supports the provided content.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>Returns true if the content is a horizontal ruler, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is HorizontalRuler;
        }

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>A GDS horizontal ruler HTML tag.</returns>
        public Task<string> RenderAsync(IContent content)
        {
            return Task.FromResult("<hr class=\"govuk-section-break govuk-section-break--visible\">");
        }
    }
}
