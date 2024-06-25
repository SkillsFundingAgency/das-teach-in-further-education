using Contentful.Core.Models;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class Page
    {
        public  string PageURL { get; set; }

        public  string PageTitle { get; set; }

        public string PageTemplate { get; set; }

        public  Preamble? PagePreamble { get; set; }

        public List<TileSection> TileSections { get; set; }

        public List<ContentBox> ContentBoxSection { get; set; }

        public HtmlString? Contents { get; set; }

        public Page(string pageUrl, string title, string pageTemplate, List<TileSection> tileSections, List<ContentBox> contentBoxSection, HtmlString? contents, Preamble? Preamble = null)
        {

            PageURL = pageUrl;

            PageTitle = title;

            PageTemplate = pageTemplate;

            PagePreamble = Preamble;

            TileSections = tileSections;

            ContentBoxSection = contentBoxSection;

            Contents = contents;

        }
    }

}
