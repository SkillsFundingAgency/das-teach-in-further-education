﻿using Contentful.Core.Models;
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

        public  Preamble? PagePreamble { get; set; }

        public List<TileSection> TileSections { get; set; }

        public HtmlString? Contents { get; set; }

        public Page(string pageUrl, string title, List<TileSection> tileSections, HtmlString? contents, Preamble? Preamble = null)
        {

            PageURL = pageUrl;

            PageTitle = title;

            PagePreamble = Preamble;

            TileSections = tileSections;

            Contents = contents;

        }
    }

}