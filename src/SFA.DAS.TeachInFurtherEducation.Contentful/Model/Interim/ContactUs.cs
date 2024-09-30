using Contentful.Core.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class ContactUs : BaseModel, IContent
    {
        [ExcludeFromCodeCoverage]

        public required string ContactUsTitle    { get; set; }

        public required string Heading { get; set; }

        public required Document ContactUsContents { get; set; }

        public required string Email { get; set; }

        public required string Phone { get; set; }

    }
}
