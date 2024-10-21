using Xunit;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Web.Interfaces;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Tests.Services
{
    public class MailChimpMarketingServiceTests
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly MailChimpMarketingServiceOptions _options;
        private readonly ILogger<MailChimpMarketingService> _logger;
        private readonly MailChimpMarketingService _service;

        public MailChimpMarketingServiceTests()
        {
            _httpClientWrapper = A.Fake<IHttpClientWrapper>();
            _options = new MailChimpMarketingServiceOptions
            {
                ApiKey = "test-api-key",
                ListId = "test-list-id",
                DataCenter = "us1"
            };
            _logger = A.Fake<ILogger<MailChimpMarketingService>>();

            // Create the service instance
            _service = new MailChimpMarketingService(_httpClientWrapper, Options.Create(_options), _logger);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WhenHttpClientWrapperIsNull_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MailChimpMarketingService(null, Options.Create(_options), _logger));
        }

        [Fact]
        public void Constructor_WhenOptionsIsNull_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MailChimpMarketingService(_httpClientWrapper, null, _logger));
        }

        [Fact]
        public void Constructor_WhenLoggerIsNull_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MailChimpMarketingService(_httpClientWrapper, Options.Create(_options), null));
        }

        #endregion

        #region SubscribeUser Tests

        [Fact]
        public async Task SubscribeUser_WhenUserDoesNotExist_AddsUserSuccessfully()
        {
            // Arrange
            var subscriber = new NewsLetterSubscriberModel { EmailAddress = "test@example.com", FirstName = "John", LastName = "Doe" };

            var getResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.That.Matches(r => r.Method == HttpMethod.Get)))
                .Returns(getResponse);

            // Act
            await _service.SubscribeUser(subscriber);

            // Assert
            _logger.VerifyLogMustHaveHappened(LogLevel.Information, $"Subscriber {subscriber.EmailAddress} does not exist. Proceeding to add.");
            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.That.Matches(r => r.Method == HttpMethod.Put))).MustHaveHappened();
        }

        [Fact]
        public async Task SubscribeUser_WhenUserExists_UpdatesUserSuccessfully()
        {
            // Arrange
            var subscriber = new NewsLetterSubscriberModel { EmailAddress = "test@example.com", FirstName = "John", LastName = "Doe" };

            var getResponse = new HttpResponseMessage(HttpStatusCode.OK);
            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.That.Matches(r => r.Method == HttpMethod.Get)))
                .Returns(getResponse);

            // Act
            await _service.SubscribeUser(subscriber);

            // Assert
            _logger.VerifyLogMustHaveHappened(LogLevel.Information, $"Subscriber {subscriber.EmailAddress} exists. Proceeding to update.");
            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.That.Matches(r => r.Method == HttpMethod.Put))).MustHaveHappened();
        }

        [Fact]
        public async Task SubscribeUser_WhenGetRequestFails_ThrowsHttpRequestException()
        {
            // Arrange
            var subscriber = new NewsLetterSubscriberModel { EmailAddress = "test@example.com", FirstName = "John", LastName = "Doe" };

            var getResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.That.Matches(r => r.Method == HttpMethod.Get)))
                .Returns(getResponse);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(() => _service.SubscribeUser(subscriber));
            _logger.VerifyLogMustHaveHappened(LogLevel.Error, $"Failed to check subscriber status: {getResponse.StatusCode}, {await getResponse.Content.ReadAsStringAsync()}");
        }

        #endregion

        #region AddOrUpdateSubscriber Tests

        [Fact]
        public async Task AddOrUpdateSubscriber_WhenResponseIsSuccessful_LogsSuccessMessage()
        {
            // Arrange
            var subscriber = new NewsLetterSubscriberModel { EmailAddress = "test@example.com", FirstName = "John", LastName = "Doe" };
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.Ignored)).Returns(response);

            // Act
            await _service.SubscribeUser(subscriber);

            // Assert
            _logger.VerifyLogMustHaveHappened(LogLevel.Information, $"Successfully updated subscriber: {subscriber.EmailAddress}");
        }

        [Fact]
        public async Task AddOrUpdateSubscriber_WhenResponseIsNotSuccessful_LogsErrorAndThrowsHttpRequestException()
        {
            // Arrange
            var subscriber = new NewsLetterSubscriberModel { EmailAddress = "test@example.com", FirstName = "John", LastName = "Doe" };
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Error details", Encoding.UTF8, "application/json")
            };

            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.Ignored)).Returns(response);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(() => _service.SubscribeUser(subscriber));

            _logger.VerifyLogMustHaveHappened(LogLevel.Error, $"Network error when subscribing user: {ex.InnerException.Message}");
        }

        [Fact]
        public async Task SubscribeUser_WhenMailChimpApiReturnsErrorWithDetail_ThrowsHttpRequestExceptionWithDetailMessage()
        {
            // Arrange
            var subscriber = new NewsLetterSubscriberModel
            {
                FirstName = "Alice",
                LastName = "Johnson",
                EmailAddress = "alice.johnson@example.com",
                Location = "1", // New York
                SubjectArea = "Math" // Subject area
            };

            var getResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.That.Matches(r => r.Method == HttpMethod.Get)))
                .Returns(getResponse);

            var errorResponseContent = "{\"detail\": \"Email address already exists.\"}";
            var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(errorResponseContent, Encoding.UTF8, "application/json")
            };

            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.That.Matches(r => r.Method == HttpMethod.Put)))
                .Returns(errorResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(async () => await _service.SubscribeUser(subscriber));

            // Assert that the exception message is as expected
            Assert.NotNull(exception.InnerException);
            Assert.Equal("Email address already exists.", exception.InnerException.Message); // Ensure the detail is correctly thrown
        }

        [Fact]
        public async Task SubscribeUser_WhenMailChimpApiReturnsErrorWithDetailButWithInvalidJSONInTheMessage_ThrowsHttpRequestExceptionWithDetailMessage()
        {
            // Arrange
            var subscriber = new NewsLetterSubscriberModel
            {
                FirstName = "Alice",
                LastName = "Johnson",
                EmailAddress = "alice.johnson@example.com",
                Location = "1", // New York
                SubjectArea = "Math" // Subject area
            };

            var getResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.That.Matches(r => r.Method == HttpMethod.Get)))
                .Returns(getResponse);

            var errorResponseContent = "invalid json";
            var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(errorResponseContent, Encoding.UTF8, "application/json")
            };

            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.That.Matches(r => r.Method == HttpMethod.Put)))
                .Returns(errorResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(async () => await _service.SubscribeUser(subscriber));

            // Assert that the exception message is as expected
            Assert.NotNull(exception.InnerException);
            Assert.Equal("There was a problem processing your request. Please try again later.", exception.Message); // Ensure the detail is correctly thrown
        }

        [Fact]
        public async Task SubscribeUser_WhenUnexpectedExceptionOccurs_ThrowsHttpRequestException()
        {
            // Arrange
            var subscriber = new NewsLetterSubscriberModel
            {
                FirstName = "Charlie",
                LastName = "Smith",
                EmailAddress = "charlie.smith@example.com",
                Location = "1", // New York
                SubjectArea = "Art"
            };

            var getResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.That.Matches(r => r.Method == HttpMethod.Get)))
                .Returns(getResponse);

            // Simulate an unexpected exception during the PUT request
            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.That.Matches(r => r.Method == HttpMethod.Put)))
                .ThrowsAsync(new Exception("Unexpected error occurred during the process"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(async () => await _service.SubscribeUser(subscriber));

            // Assert that the exception message is as expected
            Assert.Equal("An unexpected error occurred while subscribing the user.", exception.Message);
            _logger.VerifyLogMustHaveHappened(LogLevel.Error, "Unexpected error when subscribing user: Unexpected error occurred during the process");
        }

        [Fact]
        public async Task SubscribeUser_WhenNetworkErrorOccurs_ThrowsHttpRequestException()
        {
            // Arrange
            var subscriber = new NewsLetterSubscriberModel
            {
                FirstName = "Bob",
                LastName = "Brown",
                EmailAddress = "bob.brown@example.com",
                Location = "1", // New York
                SubjectArea = "Science" // Subject area
            };

            var getResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.That.Matches(r => r.Method == HttpMethod.Get)))
                .Returns(getResponse);

            var httpRequestException = new HttpRequestException("Network issue");
            A.CallTo(() => _httpClientWrapper.SendAsync(A<HttpRequestMessage>.That.Matches(r => r.Method == HttpMethod.Put)))
                .ThrowsAsync(httpRequestException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(async () => await _service.SubscribeUser(subscriber));

            // Assert that the exception message is as expected
            Assert.Equal("There was a problem processing your request. Please try again later.", exception.Message);
        }

        #endregion
    }
}
