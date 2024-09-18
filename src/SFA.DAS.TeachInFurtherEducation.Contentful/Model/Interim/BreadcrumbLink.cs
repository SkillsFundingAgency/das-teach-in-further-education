using Contentful.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class BreadcrumbLink : IContent
    {

        public required string BreadcrumbLinkTitle { get; set; }

        public required string BreadcrumbLinkText { get; set; }

        public required string? BreadcrumbLinkSource { get; set; }

        public required bool BreadcrumbLinkActive { get; set; }
        
        public required int BreadcrumbLinkOrder { get; set; }

    }

}
