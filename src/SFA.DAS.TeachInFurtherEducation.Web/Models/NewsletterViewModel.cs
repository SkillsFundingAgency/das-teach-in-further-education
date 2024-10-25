using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Html;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Web.Services;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    /// <summary>
    /// ViewModel for the Newsletter Subscription form. 
    /// It holds the content for display as well as the user input fields for subscription.
    /// </summary>
    public class NewsLetterViewModel
    {
        /// <summary>
        /// Gets or sets the heading for the newsletter.
        /// This content is retrieved from the CMS and is not user input.
        /// </summary>
        public string NewsLetterHeading { get; set; }

        /// <summary>
        /// Gets or sets the description for the newsletter.
        /// This content is HTML, retrieved from the CMS.
        /// </summary>
        public HtmlString NewsLetterDescription { get; set; }

        /// <summary>
        /// Gets or sets the label for the email input field.
        /// This content is provided by the CMS.
        /// </summary>
        public string EmailIdLabel { get; set; }

        /// <summary>
        /// Gets or sets the label for the subject select field.
        /// This content is provided by the CMS.
        /// </summary>
        public string SubjectFieldLabel { get; set; }

        /// <summary>
        /// Gets or sets the list of subject options for the subject select dropdown.
        /// This list is populated by the CMS content.
        /// </summary>
        public List<SelectOption> SubjectSelectOptions { get; set; }

        /// <summary>
        /// Gets or sets the label for the location select field.
        /// This content is provided by the CMS.
        /// </summary>
        public string LocationFieldLabel { get; set; }

        /// <summary>
        /// Gets or sets the list of location options for the location select dropdown.
        /// This list is populated by the CMS content.
        /// </summary>
        public List<SelectOption> LocationSelectOptions { get; set; }

        /// <summary>
        /// Gets or sets the email address provided by the user.
        /// This field is required and must be a valid email format.
        /// </summary>
        [Required(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        public string? EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the first name provided by the user.
        /// This field is required and must not exceed 50 characters.
        /// </summary>
        [Required(ErrorMessage = "Enter your first name")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name provided by the user.
        /// This field is required and must not exceed 50 characters.
        /// </summary>
        [Required(ErrorMessage = "Enter your last name")]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the subject area selected by the user from the dropdown.
        /// This field is required and must be selected by the user.
        /// </summary>
        [Required(ErrorMessage = "Select a subject")]
        public string? SelectedSubject { get; set; }

        /// <summary>
        /// Gets or sets the location selected by the user from the dropdown.
        /// This field is required and must be selected by the user.
        /// </summary>
        [Required(ErrorMessage = "Select a location")]
        public string? SelectedLocation { get; set; }

        /// <summary>
        /// Gets or sets an error message if form submission fails.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a success message when the user successfully subscribes.
        /// </summary>
        public string? SuccessMessage { get; set; }

        /// <summary>
        /// Indicates whether the form has been submitted.
        /// </summary>
        public bool IsSubmitted { get; set; }
        public string? BackgroundColor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsLetterViewModel"/> class
        /// and populates it with content retrieved from the CMS.
        /// </summary>
        /// <param name="newsLetterContent">The content retrieved from the CMS.</param>
        public NewsLetterViewModel(NewsLetter newsLetterContent)
        {
            NewsLetterHeading = newsLetterContent.NewsLetterHeading;
            NewsLetterDescription = ComponentService.ToHtmlString(newsLetterContent.NewsLetterDescription)!;
            EmailIdLabel = newsLetterContent.EmailIdLabel;
            SubjectFieldLabel = newsLetterContent.SubjectFieldLabel;
            SubjectSelectOptions = newsLetterContent.SubjectSelectOptions;
            LocationFieldLabel = newsLetterContent.LocationFieldLabel;
            LocationSelectOptions = newsLetterContent.LocationSelectOptions;
            BackgroundColor = newsLetterContent.BackgroundColor;
        }
    }
}
