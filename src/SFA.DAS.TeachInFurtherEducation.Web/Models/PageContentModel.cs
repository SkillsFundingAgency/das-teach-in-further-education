using Contentful.Core.Models;
using Microsoft.AspNetCore.Html;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class PageContentModel : LayoutModel
    {
        public required string PageURL { get; set; }

        public required string PageTitle { get; set; }

        public required string PageTemplate { get; set; }

        public string MetaDescription { get; set; } = "";

        public Breadcrumbs? Breadcrumbs { get; set; }

        public List<IContent>? PageComponents { get; set; }
    }
}
