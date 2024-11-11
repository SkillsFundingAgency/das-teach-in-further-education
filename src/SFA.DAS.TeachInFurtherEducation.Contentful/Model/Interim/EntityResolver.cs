using Contentful.Core.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
/// <summary>
/// Resolves ContentTypeId strings to their POCO representations.
/// Used by the Contentful client side library to serialize JSON objects from the response into our Models.
/// </summary>
[ExcludeFromCodeCoverage]
public class EntityResolver : IContentTypeResolver
{
    public Type Resolve(string contentTypeId)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return contentTypeId switch
        {
            "premable" => typeof(Preamble),
            "tileSection" => typeof(TileSection),
            "tile" => typeof(Tile),
            "imageCardBanner" => typeof(ImageCardBanner),
            "contentBox" => typeof(ContentBox),
            "video" => typeof(Video),
            "contactUs" => typeof(ContactUs),
            "newsLetter" => typeof(NewsLetter),
            "richTextContents" => typeof(RichTextContents),
            "breadcrumbs" => typeof(Breadcrumbs),
            "breadcrumbLink" => typeof(BreadcrumbLink),
            "supplierSearch" => typeof(SupplierSearch),
            "fullImageCardBanner" => typeof(FullImageCardBanner),
            "headerFullImage" => typeof(HeaderFullImage),
            "lineSeparator" => typeof(LineSeparator),
            "userJourney" => typeof(UserJourney),
            "actionBanner" => typeof(ActionBanner),
            "navigation" => typeof(Navigation),
            "cookieConsentOptions" => typeof(CookieConsentOptions),
            _ => null
        };
#pragma warning restore CS8603 // Possible null reference return.
    }
}

