using Contentful.Core.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class SupplierSearch : BaseModel, IContent
    {
        [ExcludeFromCodeCoverage]

        public required string SupplierSearchTitle { get; set; }

        public required string Heading { get; set; }

        public required string NoResultMessage { get; set; }

        public required string SuccessMessage { get; set; }

        public required string ButtonText { get; set; }

        public required int SearchWithinMiles { get; set; }
    }
}
