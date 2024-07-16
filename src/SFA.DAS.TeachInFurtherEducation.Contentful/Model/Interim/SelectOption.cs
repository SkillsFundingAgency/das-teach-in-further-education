using Contentful.Core.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class SelectOption
    {
        public required string OptionTitle { get; set; }

        public required string OptionText { get; set; }

        public int OptionValue { get; set; }
    }

}
