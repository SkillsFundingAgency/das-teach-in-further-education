using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class Breadcrumbs
    {

        public required string InterimBreadcrumbTitle { get; set; }

        public List<BreadcrumbLink> InterimBreadcrumLinks { get; set; } = [];

    }

}
