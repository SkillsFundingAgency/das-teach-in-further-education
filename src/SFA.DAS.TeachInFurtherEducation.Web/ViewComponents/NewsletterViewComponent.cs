using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace SFA.DAS.TeachInFurtherEducation.Web.ViewComponents
{
    public class NewsletterViewComponent : ViewComponent
    {
        const string _formIdentifier = "newsletter";
        private readonly IMarketingService _marketingService;

        public NewsletterViewComponent(IMarketingService marketingService)
        {
            _marketingService = marketingService;
        }

        public async Task<IViewComponentResult> InvokeAsync(NewsLetter newsLetterContent)
        {
            var model = new NewsLetterViewModel(newsLetterContent);

            // Check if the form has been submitted
            if (Request.Method == "POST")
            {
                var formIdentifier = Request.Form["formIdentifier"].ToString();

                // If token validation is possible (both tokens exist), validate them.
                // If tokens don't exist (either in session or form), skip validation and allow the form to proceed.
                if (formIdentifier == _formIdentifier)
                {
                    // Manually bind the form values to the model
                    model.FirstName = Request.Form["firstName"];
                    model.LastName = Request.Form["lastName"];
                    model.EmailAddress = Request.Form["emailAddress"];
                    model.SelectedLocation = Request.Form["location"];
                    model.SelectedSubject = Request.Form["subject"];

                    model.FirstName = model.FirstName?.Replace("\n", "_").Replace("\r", "_");
                    model.LastName = model.LastName?.Replace("\n", "_").Replace("\r", "_");
                    model.EmailAddress = model.EmailAddress?.Replace("\n", "_").Replace("\r", "_");
                    model.SelectedLocation = model.SelectedLocation?.Replace("\n", "_").Replace("\r", "_");
                    model.SelectedSubject = model.SelectedSubject?.Replace("\n", "_").Replace("\r", "_");

                    // Validate the model manually
                    var validationResults = new List<ValidationResult>();
                    var validationContext = new ValidationContext(model, null, null);
                    bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

                    // Add validation errors to the ModelState
                    if (!isValid)
                    {
                        foreach (var validationResult in validationResults)
                        {
                            foreach (var memberName in validationResult.MemberNames)
                            {
                                if (validationResult!.ErrorMessage != null)
                                {
                                    ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                                }
                            }
                        }
                    }
                    else if (ModelState.IsValid)
                    {
                        string? selectedLocation = null;
                        string? selectedSubject = null;

                        var locationId = Request.Form["location"].ToString();
                        if (!string.IsNullOrEmpty(locationId))
                        {
                            selectedLocation = newsLetterContent.LocationSelectOptions.Find(option => option.OptionValue == int.Parse(locationId))?.OptionText;
                        }

                        var subjectId = Request.Form["subject"].ToString();
                        if (!string.IsNullOrEmpty(subjectId))
                        {
                            selectedSubject = newsLetterContent.SubjectSelectOptions.Find(option => option.OptionValue == int.Parse(subjectId))?.OptionText;
                        }

                        var subscriber = new NewsLetterSubscriberModel()
                        {
                            FirstName = model.FirstName!,
                            LastName = model.LastName!,
                            EmailAddress = model.EmailAddress!,
                            Location = selectedLocation,
                            SubjectArea = selectedSubject
                        };

                        try
                        {
                            await _marketingService.SubscribeUser(subscriber);

                            // If subscription is successful, set a success message
                            model.SuccessMessage = newsLetterContent.SuccessMessage;
                            model.IsSubmitted = true;
                        }
                        catch (HttpRequestException)
                        {
                            // Handle network issue
                            model.ErrorMessage = "Network error occurred. Please try again.";
                        }
                        catch (Exception)
                        {
                            // Handle any other exceptions
                            model.ErrorMessage = "An unexpected error occurred. Please try again.";
                        }
                    }
                }
            }

            return View("Default", model);
        }
    }
}
