using Contentful.Core.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Web.Enums;
using SFA.DAS.TeachInFurtherEducation.Web.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.References;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services
{

    public static class ComponentService
    {

        #region Properties

#pragma warning disable CS8618

        private static HtmlRenderer _htmlRenderer;

#pragma warning restore CS8618

        #endregion

        #region Methods

        public static void Initialize(ILogger logger, IViewRenderService viewRenderService, HtmlRenderer htmlRenderer)
        {
            _htmlRenderer = htmlRenderer;
        }

        /// <summary>
        /// Convert a long text html document to a html string.
        /// </summary>
        /// <param name="document">Contentful.Core.Models.Document object with encapsulated html node information.</param>
        /// <returns>Microsoft.AspNetCore.Html.HtmlString.</returns>
        public static HtmlString? ToHtmlString(Document? document)
        {
            if (document == null)
            {

                return null;

            }

            string html = _htmlRenderer.ToHtml(document).Result;

            return ToNormalisedHtmlString(html);

        }

        /// <summary>
        /// Adds an 'id' attribute with a specified value to the first HTML element in the provided <see cref="HtmlString"/> content that does not already have an 'id' attribute.
        /// </summary>
        /// <param name="html">The HTML content wrapped in an <see cref="HtmlString"/>. This is the HTML to be processed.</param>
        /// <param name="id">The ID value to be added to the first HTML element that lacks an 'id' attribute.</param>
        /// <returns>
        /// An <see cref="HtmlString"/> containing the modified HTML with the 'id' attribute added, or null if the input <see cref="HtmlString"/> is null.
        /// </returns>
        public static HtmlString? AddIdToTopElement(HtmlString? html, string id) => AddIdToTopElement(html?.Value, id);

        /// <summary>
        /// Adds an 'id' attribute with a specified value to the first HTML element in the provided HTML content string that does not already have an 'id' attribute.
        /// </summary>
        /// <param name="html">The string containing the HTML content to be processed.</param>
        /// <param name="id">The ID value to be added to the first HTML element that lacks an 'id' attribute.</param>
        /// <returns>
        /// An <see cref="HtmlString"/> containing the modified HTML with the 'id' attribute added, or null if the input string is null.
        /// </returns>
        /// <remarks>
        /// This method uses a regular expression to identify the first HTML element without an 'id' attribute and modifies it to include the specified 'id'.
        /// The operation is case-insensitive and will timeout after 10 seconds if not completed, ensuring that the method does not hang indefinitely on complex HTML strings.
        /// </remarks>
        public static HtmlString? AddIdToTopElement(string? html, string id)
        {
            if (html != null)
            {
                return new HtmlString(Regex.Replace(html, "(^\\s*<)([^/\\s>]+)(?![^>]*?\\bid\\s*=\\s*['\"][^'\"]*['\"])\\s*([^>]*>)", $"$1$2 id=\"{id}\" $3", RegexOptions.IgnoreCase, new TimeSpan(0, 0, 10)));
            }

            return null;
        }

        /// <summary>
        /// Normalize html string removed any new line or quotation marks.
        /// </summary>
        /// <param name="html">Prenormalized html string as string.</param>
        /// <returns>Microsoft.AspNetCore.Html.HtmlString.</returns>
        public static HtmlString ToNormalisedHtmlString(string html)
        {

            html = html.Replace('“', '"').Replace('”', '"');

            html = html.Replace("\r\n", "\r");

            html = html.Replace("\r", "\r\n");

            return new HtmlString(html);

        }

        /// <summary>
        /// Retrieves the source URL of the media asset if available.
        /// </summary>
        /// <param name="mediaAsset">The media asset.</param>
        /// <returns>Returns the source URL of the media asset, or an empty string if the asset is null or the URL is not available.</returns>
        public static string GetMediaImageSource(Asset? mediaAsset)
        {
            if(mediaAsset == null || mediaAsset.File == null || string.IsNullOrWhiteSpace(mediaAsset.File.Url))
            {

                return string.Empty;

            }

            return mediaAsset.File.Url;
        }

        #endregion

    }

}
