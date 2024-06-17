using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content
{

    [ExcludeFromCodeCoverage]
    public class Contact
    {

        public string? SectionName { get; set; }

        public int? Order { get; set; }

        public List<ContactInformation> ContactInformation { get; set; } = new List<ContactInformation>();

        public List<ContactLink> ContactLinks { get; set; } = new List<ContactLink>();

    }

}
