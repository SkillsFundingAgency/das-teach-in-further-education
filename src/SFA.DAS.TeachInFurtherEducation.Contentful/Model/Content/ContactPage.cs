using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content
{

    [ExcludeFromCodeCoverage]
    public class ContactPage
    {

        public required string ContactPageTitle { get; set; }

        public Preamble? InterimPreamble { get; set; }

        public Breadcrumbs? InterimBreadcrumbs { get; set; }

        public List<Contact> Contacts { get; set; } = [];

    }

}
