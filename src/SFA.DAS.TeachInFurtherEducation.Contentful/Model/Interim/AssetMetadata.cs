using Contentful.Core.Models;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{
    /// <summary>
    /// Represents metadata associated with an asset.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AssetMetadata
    {
        /// <summary>
        /// Gets or sets the ID of the asset.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the filename of the asset.
        /// </summary>
        public required string Filename { get; set; }

        /// <summary>
        /// Gets or sets the url of the asset.
        /// </summary>
        public required string Url { get; set; }

        /// <summary>
        /// Gets or sets the date when the asset was created or last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }

}
