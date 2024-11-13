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
using NUglify.Helpers;
using System.Linq;

#pragma warning disable S6932

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
                    model.FirstName = Request.Form["FirstName"];
                    model.LastName = Request.Form["LastName"];
                    model.EmailAddress = Request.Form["EmailAddress"];
                    model.SelectedLocation = Request.Form["SelectedLocation"];
                    model.SelectedSubject = Request.Form["SelectedSubject"];

                    model.FirstName = model.FirstName?.Replace("\n", "_").Replace("\r", "_").Trim();
                    model.LastName = model.LastName?.Replace("\n", "_").Replace("\r", "_").Trim();
                    model.EmailAddress = model.EmailAddress?.Replace("\n", "_").Replace("\r", "_").Trim();
                    model.SelectedLocation = model.SelectedLocation?.Replace("\n", "_").Replace("\r", "_").Trim();
                    model.SelectedSubject = model.SelectedSubject?.Replace("\n", "_").Replace("\r", "_").Trim();

                    // Validate the model manually
                    var validationResults = new List<ValidationResult>();
                    var validationContext = new ValidationContext(model, null, null);
                    bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

                    if (model.SelectedSubject == model.SubjectSelectOptions.ToArray()[0]?.OptionValue.ToString() || model.SelectedSubject == "Choose a subject area")
                    {
                        ModelState.AddModelError("SelectedSubject", "Select a subject that you are interested in teaching.");
                        isValid = false;
                    }

                    if (model.SelectedLocation == model.LocationSelectOptions.ToArray()[0]?.OptionValue.ToString() || model.SelectedLocation == "Choose a location")
                    {
                        ModelState.AddModelError("SelectedLocation", "Select the location where you would like to teach.");
                        isValid = false;
                    }

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

                        var locationId = Request.Form["SelectedLocation"].ToString();
                        if (!string.IsNullOrEmpty(locationId))
                        {
                            selectedLocation = newsLetterContent.LocationSelectOptions.Find(option => option.OptionValue == int.Parse(locationId))?.OptionText;
                        }

                        var subjectId = Request.Form["SelectedSubject"].ToString();
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
                        catch (HttpRequestException hre)
                        {
                            if (hre.InnerException != null && !string.IsNullOrWhiteSpace(hre.InnerException.Message))
                            {
                                var emailError = hre.InnerException.Message;
                                ModelState.AddModelError("emailAddress", emailError);
                            }
                            else
                            {
                                // Handle network issue
                                model.ErrorMessage = "Network error occurred. Please try again.";
                            }
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

#pragma warning restore S6932
