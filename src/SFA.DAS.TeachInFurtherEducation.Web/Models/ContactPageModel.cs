using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Collections.Generic;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{

    public class ContactPageModel : LayoutModel
    {

        public required string ContactPageTitle { get; set; }

        public Preamble? InterimPreamble { get; set; }

        public Breadcrumbs? InterimBreadcrumbs { get; set; }

        public List<Contact> Contacts { get; set; } = [];

    }

}
