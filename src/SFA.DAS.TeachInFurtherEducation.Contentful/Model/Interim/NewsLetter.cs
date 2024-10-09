using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class NewsLetter : BaseModel, IContent
    {
        [ExcludeFromCodeCoverage]

        public required string NewsLetterHeading { get; set; }

        public required Document NewsLetterDescription { get; set; }

        public required string EmailIdLabel { get; set; }

        public required string SubjectFieldLabel { get; set; }

        public List<SelectOption> SubjectSelectOptions { get; set; } = [];

        public required string LocationFieldLabel { get; set; }

        public List<SelectOption> LocationSelectOptions { get; set; } = [];

        public string? SelectedOption { get; set; }

        public required string SuccessMessage { get; set; } = "You have successfully subscribed to the newsletter!";
    }
}
