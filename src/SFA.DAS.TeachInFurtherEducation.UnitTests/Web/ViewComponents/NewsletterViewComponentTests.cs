using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Contentful.Core.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using SFA.DAS.TeachInFurtherEducation.Web.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.ViewComponents;
using Xunit;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Tests.ViewComponents
{
    public class NewsletterViewComponentTests
    {
        private readonly IMarketingService _marketingServiceFake;
        private readonly NewsletterViewComponent _viewComponent;
        private readonly NewsLetter _newsLetterContent;

        public NewsletterViewComponentTests()
        {
            // Create a fake IMarketingService using FakeItEasy
            _marketingServiceFake = A.Fake<IMarketingService>();

            // Initialize the ViewComponent with the fake service
            _viewComponent = new NewsletterViewComponent(_marketingServiceFake);

            // Sample NewsLetter content
            _newsLetterContent = new NewsLetter
            {
                NewsLetterHeading = "Subscribe to our Newsletter",
                NewsLetterDescription = GetDocument("Testing Testing 123"),
                EmailIdLabel = "Email Address",
                SubjectFieldLabel = "Select Subject",
                SubjectSelectOptions = new List<SelectOption>
                {
                    new SelectOption { OptionValue = 1, OptionText = "Math", OptionTitle = "Math" },
                    new SelectOption { OptionValue = 2, OptionText = "Science", OptionTitle = "Science" }
                },
                LocationFieldLabel = "Select Location",
                LocationSelectOptions = new List<SelectOption>
                {
                    new SelectOption { OptionValue = 1, OptionText = "New York", OptionTitle = "New York" },
                    new SelectOption { OptionValue = 2, OptionText = "London", OptionTitle = "London" }
                },
                SuccessMessage = "Thank you for subscribing!"
            };

            var renderer = ContentService.CreateHtmlRenderer();
            ComponentService.Initialize(A.Fake<Microsoft.Extensions.Logging.ILogger>(),
                                        A.Fake<IViewRenderService>(),
                                        renderer);
        }

        private Document GetDocument(string content)
        {
            return new Document
            {
                Content = new List<IContent>
                {
                    new Quote
                    {
                        Content = new List<IContent>
                        {
                            new Paragraph
                            {
                                Content = new List<IContent>
                                {
                                    new Text
                                    {
                                        Value = content
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Helper method to set up the ViewComponentContext with specified HTTP method and form data.
        /// </summary>
        /// <param name="method">HTTP method (e.g., "GET", "POST").</param>
        /// <param name="formData">Form data as a dictionary.</param>
        private void SetupViewComponentContext(string method, IDictionary<string, string> formData = null)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = method;

            if (formData != null)
            {
                var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(
                    ConvertToStringValues(formData)));
                httpContext.Request.Form = formCollection;
            }

            var viewData = new ViewDataDictionary<NewsLetterViewModel>(
                metadataProvider: new EmptyModelMetadataProvider(),
                modelState: new ModelStateDictionary());

            var viewContext = new ViewContext
            {
                HttpContext = httpContext,
                ViewData = viewData,
                Writer = System.IO.TextWriter.Null // Not necessary for this test
            };

            var viewComponentContext = new ViewComponentContext
            {
                ViewContext = viewContext,
            };

            _viewComponent.ViewComponentContext = viewComponentContext;
        }

        /// <summary>
        /// Converts a Dictionary<string, string> to Dictionary<string, StringValues>
        /// required for FormCollection.
        /// </summary>
        /// <param name="dict">The input dictionary.</param>
        /// <returns>A dictionary with StringValues.</returns>
        private Dictionary<string, Microsoft.Extensions.Primitives.StringValues> ConvertToStringValues(IDictionary<string, string> dict)
        {
            var result = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            foreach (var kvp in dict)
            {
                result[kvp.Key] = kvp.Value;
            }
            return result;
        }

        /// <summary>
        /// Tests that a GET request returns the view with the initial model.
        /// </summary>
        [Fact]
        public async Task InvokeAsync_GetRequest_ReturnsViewWithInitialModel()
        {
            // Arrange
            SetupViewComponentContext("GET");

            // Act
            var result = await _viewComponent.InvokeAsync(_newsLetterContent) as ViewViewComponentResult;

            var expectedDesc = ComponentService.ToHtmlString(_newsLetterContent.NewsLetterDescription);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Default", result.ViewName);
            var model = Assert.IsType<NewsLetterViewModel>(result.ViewData.Model);
            Assert.Equal(_newsLetterContent.NewsLetterHeading, model.NewsLetterHeading);
            Assert.Equal(expectedDesc.Value, model.NewsLetterDescription.Value);
            Assert.Equal(_newsLetterContent.EmailIdLabel, model.EmailIdLabel);
            Assert.Equal(_newsLetterContent.SubjectFieldLabel, model.SubjectFieldLabel);
            Assert.Equal(_newsLetterContent.SubjectSelectOptions, model.SubjectSelectOptions);
            Assert.Equal(_newsLetterContent.LocationFieldLabel, model.LocationFieldLabel);
            Assert.Equal(_newsLetterContent.LocationSelectOptions, model.LocationSelectOptions);
            Assert.False(model.IsSubmitted);
            Assert.Null(model.SuccessMessage);
            Assert.Null(model.ErrorMessage);
        }

        /// <summary>
        /// Tests that a POST request with an invalid form identifier returns the view with the initial model.
        /// </summary>
        [Fact]
        public async Task InvokeAsync_PostRequest_InvalidFormIdentifier_ReturnsViewWithInitialModel()
        {
            // Arrange
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "invalid" }
            };
            SetupViewComponentContext("POST", formData);

            // Act
            var result = await _viewComponent.InvokeAsync(_newsLetterContent) as ViewViewComponentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Default", result.ViewName);
            var model = Assert.IsType<NewsLetterViewModel>(result.ViewData.Model);
            Assert.False(model.IsSubmitted);
            Assert.Null(model.SuccessMessage);
            Assert.Null(model.ErrorMessage);

            // Verify that SubscribeUser was never called
            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustNotHaveHappened();
        }

        /// <summary>
        /// Tests that a POST request with a valid form identifier but invalid model data returns the view with errors.
        /// </summary>
        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_InvalidModel_ReturnsViewWithErrors()
        {
            // Arrange
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", "" }, // Missing first name
                { "lastName", "Doe" },
                { "emailAddress", "invalid-email" }, // Invalid email
                { "location", "1" },
                { "subject", "2" }
            };
            SetupViewComponentContext("POST", formData);

            // Act
            var result = await _viewComponent.InvokeAsync(_newsLetterContent) as ViewViewComponentResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<NewsLetterViewModel>(result.ViewData.Model);
            Assert.False(model.IsSubmitted);
            Assert.Null(model.SuccessMessage);
            Assert.Null(model.ErrorMessage);

            // Check that ModelState contains errors for FirstName and EmailAddress
            var modelState = _viewComponent.ViewComponentContext.ViewData.ModelState;
            Assert.False(modelState.IsValid);
            Assert.True(modelState.ContainsKey("FirstName"));
            Assert.True(modelState.ContainsKey("EmailAddress"));
            Assert.Equal(2, modelState.ErrorCount);

            // Verify that SubscribeUser was never called due to validation failure
            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustNotHaveHappened();
        }

        /// <summary>
        /// Tests that a POST request with a valid form identifier and valid model data successfully subscribes the user and returns the view with a success message.
        /// </summary>
        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_ValidModel_SuccessfulSubscription_ReturnsViewWithSuccess()
        {
            // Arrange
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", "John" },
                { "lastName", "Doe" },
                { "emailAddress", "john.doe@example.com" },
                { "location", "1" }, // New York
                { "subject", "2" } // Science
            };
            SetupViewComponentContext("POST", formData);

            // Configure the fake service to not throw exceptions (simulate successful subscription)
            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>.That.Matches(s =>
                s.FirstName == "John" &&
                s.LastName == "Doe" &&
                s.EmailAddress == "john.doe@example.com" &&
                s.Location == "New York" && // locationId=1
                s.SubjectArea == "Science" // subjectId=2
            ))).Returns(Task.CompletedTask);

            // Act
            var result = await _viewComponent.InvokeAsync(_newsLetterContent) as ViewViewComponentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Default", result.ViewName);
            var model = Assert.IsType<NewsLetterViewModel>(result.ViewData.Model);
            Assert.True(model.IsSubmitted);
            Assert.Equal(_newsLetterContent.SuccessMessage, model.SuccessMessage);
            Assert.Null(model.ErrorMessage);

            // Verify that SubscribeUser was called once with correct parameters
            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// Tests that a POST request with a valid form identifier and valid model data where SubscribeUser throws an HttpRequestException returns the view with an appropriate error message.
        /// </summary>
        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_SubscribeUserThrowsHttpRequestException_ReturnsViewWithError()
        {
            // Arrange
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", "Jane" },
                { "lastName", "Smith" },
                { "emailAddress", "jane.smith@example.com" },
                { "location", "2" }, // London
                { "subject", "1" } // Math
            };
            SetupViewComponentContext("POST", formData);

            // Configure the fake service to throw HttpRequestException
            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._))
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await _viewComponent.InvokeAsync(_newsLetterContent) as ViewViewComponentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Default", result.ViewName);
            var model = Assert.IsType<NewsLetterViewModel>(result.ViewData.Model);
            Assert.False(model.IsSubmitted);
            Assert.Equal("Network error occurred. Please try again.", model.ErrorMessage);
            Assert.Null(model.SuccessMessage);

            // Verify that SubscribeUser was called once
            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// Tests that a POST request with a valid form identifier and valid model data where SubscribeUser throws a general Exception returns the view with an appropriate error message.
        /// </summary>
        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_SubscribeUserThrowsException_ReturnsViewWithError()
        {
            // Arrange
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", "Bob" },
                { "lastName", "Brown" },
                { "emailAddress", "bob.brown@example.com" },
                { "location", "1" }, // New York
                { "subject", "1" } // Math
            };
            SetupViewComponentContext("POST", formData);

            // Configure the fake service to throw a general Exception
            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._))
                .ThrowsAsync(new Exception("General error"));

            // Act
            var result = await _viewComponent.InvokeAsync(_newsLetterContent) as ViewViewComponentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Default", result.ViewName);
            var model = Assert.IsType<NewsLetterViewModel>(result.ViewData.Model);
            Assert.False(model.IsSubmitted);
            Assert.Equal("An unexpected error occurred. Please try again.", model.ErrorMessage);
            Assert.Null(model.SuccessMessage);

            // Verify that SubscribeUser was called once
            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// Tests that a POST request with a valid form identifier but non-integer location ID throws a FormatException.
        /// </summary>
        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_InvalidLocationId_ThrowsFormatException()
        {
            // Arrange
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", "Eve" },
                { "lastName", "Black" },
                { "emailAddress", "eve.black@example.com" },
                { "location", "invalid" }, // Non-integer
                { "subject", "1" } // Math
            };
            SetupViewComponentContext("POST", formData);

            // Act & Assert
            await Assert.ThrowsAsync<FormatException>(async () => await _viewComponent.InvokeAsync(_newsLetterContent));

            // Verify that SubscribeUser was never called due to exception
            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustNotHaveHappened();
        }

        /// <summary>
        /// Tests that a POST request with a valid form identifier but non-integer subject ID throws a FormatException.
        /// </summary>
        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_InvalidSubjectId_ThrowsFormatException()
        {
            // Arrange
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", "Frank" },
                { "lastName", "Green" },
                { "emailAddress", "frank.green@example.com" },
                { "location", "1" }, // New York
                { "subject", "invalid" } // Non-integer
            };
            SetupViewComponentContext("POST", formData);

            // Act & Assert
            await Assert.ThrowsAsync<FormatException>(async () => await _viewComponent.InvokeAsync(_newsLetterContent));

            // Verify that SubscribeUser was never called due to exception
            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustNotHaveHappened();
        }

        /// <summary>
        /// Tests that a POST request with a valid form identifier but both location and subject IDs are non-integer throws a FormatException.
        /// </summary>
        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_InvalidLocationAndSubjectIds_ThrowsFormatException()
        {
            // Arrange
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", "Grace" },
                { "lastName", "Hopper" },
                { "emailAddress", "grace.hopper@example.com" },
                { "location", "invalid" }, // Non-integer
                { "subject", "invalid" } // Non-integer
            };
            SetupViewComponentContext("POST", formData);

            // Act & Assert
            await Assert.ThrowsAsync<FormatException>(async () => await _viewComponent.InvokeAsync(_newsLetterContent));

            // Verify that SubscribeUser was never called due to exception
            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustNotHaveHappened();
        }
    }

    /// <summary>
    /// Extension methods for converting Dictionary<string, string> to Dictionary<string, StringValues>.
    /// </summary>
    public static class DictionaryExtensions
    {
        public static Dictionary<string, Microsoft.Extensions.Primitives.StringValues> ToStringValues(this IDictionary<string, string> dict)
        {
            var result = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            foreach (var kvp in dict)
            {
                result[kvp.Key] = kvp.Value;
            }
            return result;
        }
    }
}
