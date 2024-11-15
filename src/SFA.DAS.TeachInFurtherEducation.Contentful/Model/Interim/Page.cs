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

        public string MetaDescription { get; set; }
        public string PageTemplate { get; set; }

        public Breadcrumbs? Breadcrumbs { get; set; }

        public List<IContent> PageComponents { get; }

        public HtmlString? Contents { get; set; }

        public Page(string pageUrl, string title, string pageTemplate, Breadcrumbs? breadcrumbs, List<IContent> pageComponents, HtmlString? contents, string metaDescription = "")
        {
            PageURL = pageUrl;

            PageTitle = title;

            PageTemplate = pageTemplate;

            PageComponents = pageComponents;

            MetaDescription = metaDescription;

            Contents = contents;

            Breadcrumbs = breadcrumbs;
        }
    }

}
