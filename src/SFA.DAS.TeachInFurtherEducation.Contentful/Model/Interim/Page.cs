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

        public ImageCardBanner? ImageCardBanner { get; set; }

        public NewsLetter? NewsLetter { get; set; }

        public ContactUs? ContactUs { get; set; }

        public Video? Video { get; set; }

        public List<IContent> PageComponents { get; }

        public Page(string pageUrl, string title, string pageTemplate, List<TileSection> tileSections, List<ContentBox> contentBoxSection, HtmlString? contents, NewsLetter? newsLetter, ContactUs? contactUs, Video? video, List<IContent> pageComponents, Preamble? Preamble = null, ImageCardBanner? imageCardBanner = null)
        {

            PageURL = pageUrl;

            PageTitle = title;

            PageTemplate = pageTemplate;

            PagePreamble = Preamble;

            TileSections = tileSections;

            ContentBoxSection = contentBoxSection;

            Contents = contents;

            ImageCardBanner = imageCardBanner;

            NewsLetter = newsLetter;

            ContactUs = contactUs;

            Video = video;

            PageComponents = pageComponents;
        }
    }

}
