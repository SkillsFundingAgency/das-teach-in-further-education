using Contentful.Core.Configuration;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System;

namespace SFA.DAS.TeachInFurtherEducation.Contentful;

/// <summary>
/// Resolves ContentTypeId strings to their POCO representations.
/// Used by the Contentful client side library to serialize JSON objects from the response into our Models.
/// </summary>
public class EntityResolver : IContentTypeResolver
{
    public Type Resolve(string contentTypeId)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return contentTypeId switch
        {
            "page" => typeof(Page),
            "interimPremable" => typeof(Preamble),
            "interimTileSection" => typeof(TileSection),
            "interimTile" => typeof(Tile),
            _ => null
        };
#pragma warning restore CS8603 // Possible null reference return.
    }
}

