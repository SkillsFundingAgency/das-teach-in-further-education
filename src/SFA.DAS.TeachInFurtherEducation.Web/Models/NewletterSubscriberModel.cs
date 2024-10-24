using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class NewsLetterSubscriberModel
    {
       
        public required string EmailAddress { get; set; }

        [Required(ErrorMessage = "Enter your first name")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Enter your last name")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Select a subject")]
        public string? SubjectArea { get; set; }

        [Required(ErrorMessage = "Select a location")]
        public string? Location { get; set; }
    }
}
