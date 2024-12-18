﻿using Contentful.Core.Models;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.GdsHtmlRenderers
{
    /// <summary>
    /// A renderer for a block quote
    /// </summary>
    public class EntryStructureRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        /// <summary>
        /// Initializes a new GdsBlockQuoteRenderer
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public EntryStructureRenderer(ContentRendererCollection rendererCollection)
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
        /// <returns>Returns true if the content is a quote, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is EntryStructure;
        }

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The block quote as a quote HTML string.</returns>
        public async Task<string> RenderAsync(IContent content)
        {
            var sb = new StringBuilder();

            var entryStructure = content as EntryStructure;

            if (entryStructure != null && entryStructure.NodeType == "entry-hyperlink")
            {
                CustomNode? target = entryStructure.Data?.Target as CustomNode;

                if (target != null)
                { 
                    var pageUrl = target.JObject["pageUrl"];
                    var pageTitle = target.JObject["pageTitle"];

                    sb.Append($"<a href=\"{pageUrl}\" title=\"{pageTitle}\" class=\"govuk-link\">");

                    foreach (var subContent in entryStructure.Content)
                    {
                        var renderer = _rendererCollection.GetRendererForContent(subContent);
                        sb.Append(await renderer.RenderAsync(subContent));
                    }

                    sb.Append("</a>");
                }
            }

            return sb.ToString();
        }
    }
}
