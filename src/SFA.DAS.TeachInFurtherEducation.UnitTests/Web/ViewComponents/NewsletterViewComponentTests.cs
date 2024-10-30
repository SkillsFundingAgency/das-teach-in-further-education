using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Contentful.Core.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using SFA.DAS.TeachInFurtherEducation.Web.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.ViewComponents;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Tests.ViewComponents
{
    public class NewsletterViewComponentTests
    {
        private IMarketingService _marketingServiceFake;
        private NewsletterViewComponent _viewComponent;
        private NewsLetter _newsLetterContent;

        public NewsletterViewComponentTests()
        {
            Initialize();
        }

        private void Initialize()
        {
            _marketingServiceFake = A.Fake<IMarketingService>();
            _viewComponent = new NewsletterViewComponent(_marketingServiceFake);

            _newsLetterContent = new NewsLetter
            {
                NewsLetterHeading = "Subscribe to our Newsletter",
                NewsLetterDescription = GetDocument("Testing Testing 123"),
                EmailIdLabel = "Email Address",
                SubjectFieldLabel = "Select Subject",
                SubjectSelectOptions = new List<SelectOption>
                {
                    new SelectOption { OptionValue = 1, OptionText = "Choose a Subject", OptionTitle = "Choose a Subject" },
                    new SelectOption { OptionValue = 2, OptionText = "Math", OptionTitle = "Math" },
                    new SelectOption { OptionValue = 3, OptionText = "Science", OptionTitle = "Science" }
                },
                LocationFieldLabel = "Select Location",
                LocationSelectOptions = new List<SelectOption>
                {
                    new SelectOption { OptionValue = 1, OptionText = "Choose a Location", OptionTitle = "Choose a Location" },
                    new SelectOption { OptionValue = 2, OptionText = "New York", OptionTitle = "New York" },
                    new SelectOption { OptionValue = 3, OptionText = "London", OptionTitle = "London" }
                },
                SuccessMessage = "Thank you for subscribing!",
                BackgroundColor = "#FFEBDB"
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

            _viewComponent.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = viewContext
            };
        }

        private Dictionary<string, Microsoft.Extensions.Primitives.StringValues> ConvertToStringValues(IDictionary<string, string> dict)
        {
            var result = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            foreach (var kvp in dict)
            {
                result[kvp.Key] = kvp.Value;
            }
            return result;
        }

        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_SubscribeUserThrowsHttpRequestExceptionWithDetail_ReturnsViewWithErrors()
        {
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", "Alice" },
                { "lastName", "Johnson" },
                { "emailAddress", "alice.johnson@example.com" },
                { "location", "2" },
                { "subject", "3" }
            };

            var innerException = new Exception("Email address already exists.");
            var httpRequestException = new HttpRequestException("Network error", innerException);

            // Mocking the marketing service to throw the exception
            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._))
                .ThrowsAsync(httpRequestException);

            // Setup the ViewComponentContext for the POST request
            SetupViewComponentContext("POST", formData);

            var result = await _viewComponent.InvokeAsync(_newsLetterContent) as ViewViewComponentResult;

            Assert.NotNull(result);
            var model = Assert.IsType<NewsLetterViewModel>(result.ViewData.Model);
            Assert.False(model.IsSubmitted);
            Assert.Null(model.SuccessMessage);
            Assert.Null(model.ErrorMessage);

            var modelState = _viewComponent.ViewComponentContext.ViewData.ModelState;
            Assert.False(modelState.IsValid);
            Assert.True(modelState.ContainsKey("emailAddress")); // Check for email address error
            Assert.Equal("Email address already exists.", modelState["emailAddress"].Errors[0].ErrorMessage); // Assert the inner exception message
        }

        [Fact]
        public async Task InvokeAsync_GetRequest_ReturnsViewWithInitialModel()
        {
            SetupViewComponentContext("GET");

            var result = await _viewComponent.InvokeAsync(_newsLetterContent) as ViewViewComponentResult;

            var expectedDesc = ComponentService.ToHtmlString(_newsLetterContent.NewsLetterDescription);

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

        [Fact]
        public async Task InvokeAsync_PostRequest_InvalidFormIdentifier_ReturnsViewWithInitialModel()
        {
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "invalid" }
            };
            SetupViewComponentContext("POST", formData);

            var result = await _viewComponent.InvokeAsync(_newsLetterContent) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.Equal("Default", result.ViewName);
            var model = Assert.IsType<NewsLetterViewModel>(result.ViewData.Model);
            Assert.False(model.IsSubmitted);
            Assert.Null(model.SuccessMessage);
            Assert.Null(model.ErrorMessage);

            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_InvalidModel_ReturnsViewWithErrors()
        {
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", "" }, // Missing first name
                { "lastName", "Doe" },
                { "emailAddress", "invalid-email" }, // Invalid email
                { "location", "1" }, // Not selected location
                { "subject", "1" } // Not selected subject
            };
            SetupViewComponentContext("POST", formData);

            var result = await _viewComponent.InvokeAsync(_newsLetterContent) as ViewViewComponentResult;

            Assert.NotNull(result);
            var model = Assert.IsType<NewsLetterViewModel>(result.ViewData.Model);
            Assert.False(model.IsSubmitted);
            Assert.Null(model.SuccessMessage);
            Assert.Null(model.ErrorMessage);

            var modelState = _viewComponent.ViewComponentContext.ViewData.ModelState;
            Assert.False(modelState.IsValid);
            Assert.True(modelState.ContainsKey("FirstName"));
            Assert.True(modelState.ContainsKey("EmailAddress"));
            Assert.True(modelState.ContainsKey("SelectedLocation"));
            Assert.True(modelState.ContainsKey("SelectedSubject"));
            Assert.Equal(4, modelState.ErrorCount);

            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_NullValues_ReturnsViewWithErrors()
        {
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", null }, // Missing first name
                { "lastName", null },
                { "emailAddress",  null }, // Invalid email
                { "location", null }, // Not selected location
                { "subject", null } // Not selected subject
            };
            SetupViewComponentContext("POST", formData);

            var result = await _viewComponent.InvokeAsync(_newsLetterContent) as ViewViewComponentResult;

            Assert.NotNull(result);
            var model = Assert.IsType<NewsLetterViewModel>(result.ViewData.Model);
            Assert.False(model.IsSubmitted);
            Assert.Null(model.SuccessMessage);
            Assert.Null(model.ErrorMessage);

            var modelState = _viewComponent.ViewComponentContext.ViewData.ModelState;
            Assert.False(modelState.IsValid);
            Assert.True(modelState.ContainsKey("FirstName"));
            Assert.True(modelState.ContainsKey("LastName"));
            Assert.True(modelState.ContainsKey("EmailAddress"));
            Assert.True(modelState.ContainsKey("SelectedLocation"));
            Assert.True(modelState.ContainsKey("SelectedSubject"));
            Assert.Equal(5, modelState.ErrorCount);

            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_ValidModel_SuccessfulSubscription_ReturnsViewWithSuccess()
        {
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", "John" },
                { "lastName", "Doe" },
                { "emailAddress", "john.doe@example.com" },
                { "location", "2" }, // New York
                { "subject", "3" } // Science
            };
            SetupViewComponentContext("POST", formData);

            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>.That.Matches(s =>
                s.FirstName == "John" &&
                s.LastName == "Doe" &&
                s.EmailAddress == "john.doe@example.com" &&
                s.Location == "New York" && // locationId=1
                s.SubjectArea == "Science" // subjectId=2
            ))).Returns(Task.CompletedTask);

            var result = await _viewComponent.InvokeAsync(_newsLetterContent) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.Equal("Default", result.ViewName);
            var model = Assert.IsType<NewsLetterViewModel>(result.ViewData.Model);
            Assert.True(model.IsSubmitted);
            Assert.Equal(_newsLetterContent.SuccessMessage, model.SuccessMessage);
            Assert.Null(model.ErrorMessage);

            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_SubscribeUserThrowsHttpRequestException_ReturnsViewWithError()
        {
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", "Jane" },
                { "lastName", "Smith" },
                { "emailAddress", "jane.smith@example.com" },
                { "location", "3" }, // London
                { "subject", "2" } // Math
            };
            SetupViewComponentContext("POST", formData);

            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._))
                .ThrowsAsync(new HttpRequestException("Network error"));

            var result = await _viewComponent.InvokeAsync(_newsLetterContent) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.Equal("Default", result.ViewName);
            var model = Assert.IsType<NewsLetterViewModel>(result.ViewData.Model);
            Assert.False(model.IsSubmitted);
            Assert.Equal("Network error occurred. Please try again.", model.ErrorMessage);
            Assert.Null(model.SuccessMessage);

            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_SubscribeUserThrowsException_ReturnsViewWithError()
        {
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", "Bob" },
                { "lastName", "Brown" },
                { "emailAddress", "bob.brown@example.com" },
                { "location", "2" }, // New York
                { "subject", "2" } // Math
            };
            SetupViewComponentContext("POST", formData);

            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._))
                .ThrowsAsync(new Exception("General error"));

            var result = await _viewComponent.InvokeAsync(_newsLetterContent) as ViewViewComponentResult;

            Assert.NotNull(result);
            Assert.Equal("Default", result.ViewName);
            var model = Assert.IsType<NewsLetterViewModel>(result.ViewData.Model);
            Assert.False(model.IsSubmitted);
            Assert.Equal("An unexpected error occurred. Please try again.", model.ErrorMessage);
            Assert.Null(model.SuccessMessage);

            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_InvalidLocationId_ThrowsFormatException()
        {
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", "Eve" },
                { "lastName", "Black" },
                { "emailAddress", "eve.black@example.com" },
                { "location", "invalid" }, // Non-integer
                { "subject", "2" } // Math
            };
            SetupViewComponentContext("POST", formData);

            await Assert.ThrowsAsync<FormatException>(async () => await _viewComponent.InvokeAsync(_newsLetterContent));

            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_InvalidSubjectId_ThrowsFormatException()
        {
            var formData = new Dictionary<string, string>
            {
                { "formIdentifier", "newsletter" },
                { "firstName", "Frank" },
                { "lastName", "Green" },
                { "emailAddress", "frank.green@example.com" },
                { "location", "2" }, // New York
                { "subject", "invalid" } // Non-integer
            };
            SetupViewComponentContext("POST", formData);

            await Assert.ThrowsAsync<FormatException>(async () => await _viewComponent.InvokeAsync(_newsLetterContent));

            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task InvokeAsync_PostRequest_ValidFormIdentifier_InvalidLocationAndSubjectIds_ThrowsFormatException()
        {
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

            await Assert.ThrowsAsync<FormatException>(async () => await _viewComponent.InvokeAsync(_newsLetterContent));

            A.CallTo(() => _marketingServiceFake.SubscribeUser(A<NewsLetterSubscriberModel>._)).MustNotHaveHappened();
        }
    }

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
