using Contentful.Core.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class Breadcrumbs : IContent
    {

        public required string BreadcrumbTitle { get; set; }

        public List<BreadcrumbLink> BreadcrumLinks { get; set; } = [];

    }

}
