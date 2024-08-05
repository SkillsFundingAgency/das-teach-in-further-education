using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Html;
using SFA.DAS.TeachInFurtherEducation.Contentful.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Api
{

    [DebuggerDisplay("{PageTitle}")]
    [ExcludeFromCodeCoverage]
    public class Page : IRootContent
    {

        public string? PageTitle { get; set; }

        public string? PageURL { get; set; }

        public string PageTemplate { get; set; } = "";

        public Breadcrumbs? Breadcrumbs { get; set; }

        public Preamble? PagePreamble { get; set; }

        public List<TileSection> TileSections { get; set; } = [];

        public List<ContentBox> ContentBoxSection { get; set; } = [];

        public Document? Contents { get; set; }

        public ImageCardBanner? ImageCardBanner { get; set; }

        public NewsLetter? NewsLetter { get; set; }

        public ContactUs? ContactUs { get; set; }

        public Video? Video { get; set; }

        public List<IContent>? PageComponents { get; set; }
    }

}
