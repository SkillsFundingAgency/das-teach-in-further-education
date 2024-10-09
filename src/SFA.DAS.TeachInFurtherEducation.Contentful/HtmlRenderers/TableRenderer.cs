﻿using Contentful.Core.Models;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.GdsHtmlRenderers
{
    /// <summary>
    /// A renderer for a GDS table.
    /// </summary>
    public class TableRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        /// <summary>
        /// Initializes a new TableRenderer
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public TableRenderer(ContentRendererCollection rendererCollection)
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
            return content is Table;

        }

        /// <summary>
        /// Renders the content to an html Table-tag.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The p-tag as a string.</returns>
        public async Task<string> RenderAsync(IContent content)
        {
            if (content is not Table table)
            {
                throw new ArgumentException("Invalid content passed to TableRenderer", nameof(content));
            }

            var sb = new StringBuilder();
             
            sb.Append("<table class=\"govuk-table\">");

            // Render any nested content within the paragraph
            foreach (var subContent in table.Content)
            {
                    var renderer = _rendererCollection.GetRendererForContent(subContent);

                    sb.Append(await renderer.RenderAsync(subContent));

            }

            sb.Append("</table>");

            return sb.ToString();
        }
    }

}
