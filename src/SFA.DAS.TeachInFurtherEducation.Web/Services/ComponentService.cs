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

namespace SFA.DAS.TeachInFurtherEducation.Web.Services
{

    public static class ComponentService
    {

        #region Properties

        #pragma warning disable CS8618

        private static ILogger _logger;


        private static HtmlRenderer _htmlRenderer;

        #pragma warning restore CS8618

        #endregion

        #region Methods

        public static void Initialize(ILogger logger, IViewRenderService viewRenderService, HtmlRenderer htmlRenderer)
        {

            _logger = logger;

            _htmlRenderer = htmlRenderer;

        }


        ///// <summary>
        ///// Generate specified partial view via the component type property.
        ///// </summary>
        ///// <param name="component">Encapsulated information regarding the unique component object.</param>
        ///// <param name="tag">Tag to append the rendered partial view to.</param>
        //private static void GenerateComponent(InterimPageComponent component, TagBuilder tag)
        //{

        //    try
        //    {

        //        var result = _viewRenderService.RenderToStringAsync(

        //            MappedTypesToPartials[component.ComponentType ?? string.Empty],

        //            component

        //        ).Result;

        //        tag.InnerHtml.AppendHtml(result);

        //    }
        //    catch(Exception _exception)
        //    {

        //        _logger.LogError(_exception, "Unable to generate componeont for type {ComponentType}.", component.ComponentType);

        //    }

        //}


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

            try
            {

                if(mediaAsset == null || mediaAsset.File == null || string.IsNullOrWhiteSpace(mediaAsset.File.Url))
                {

                    return string.Empty;

                }

                return mediaAsset.File.Url;

            }
            catch(Exception _exception)
            {

                _logger.LogError(_exception, "Unable to get media asset source");

                return string.Empty;

            }

        }

        //public static void Initialize(global::Castle.Core.Logging.ILogger logger, IViewRenderService viewRenderService, HtmlRenderer renderer)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

    }

}
