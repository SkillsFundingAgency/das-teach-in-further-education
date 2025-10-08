using Microsoft.AspNetCore.Html;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content
{
    [ExcludeFromCodeCoverage]
    public class PageRenamed
    {
        /// <summary>
        /// Should always be valid. We filter out pages with invalid urls, as they aren't navigable.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Custom page URL entered by users
        /// </summary>
        public string PageUrl { get; }

        public Preamble? InterimPreamble { get; }

        public Breadcrumbs? InterimBreadcrumbs { get; }

        public string? Title { get; }

        public HtmlString? Content { get; }

        public PageRenamed(string? title, string url, HtmlString? content, Preamble? interimPreamble = null, Breadcrumbs? interimPageBreadcrumbs = null)
        {
            Title = title;
            Url = url;
            PageUrl = url; // Set PageUrl to the same value, or modify constructor to accept separate parameter
            InterimPreamble = interimPreamble;
            InterimBreadcrumbs = interimPageBreadcrumbs;
            Content = content;
        }
    }

}
