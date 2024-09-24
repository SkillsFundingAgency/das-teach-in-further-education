using Contentful.Core.Models;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{
    /// <summary>
    /// Represents an asset with its content and metadata.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Asset<T>
    {
        /// <summary>
        /// Gets or sets the content of the asset.
        /// </summary>
        public required T Content { get; set; }

        /// <summary>
        /// Gets or sets the metadata associated with the asset.
        /// </summary>
        public required AssetMetadata Metadata { get; set; }
    }

}
