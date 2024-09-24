using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System.Security.Cryptography;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services
{
    public class MailChimpMarketingService : IMarketingService
    {
        private readonly HttpClient _httpClient;
        private readonly MailChimpMarketingServiceOptions _options;
        private readonly ILogger<MailChimpMarketingService> _logger;

        public MailChimpMarketingService(
            HttpClient httpClient,
            IOptions<MailChimpMarketingServiceOptions> options,
            ILogger<MailChimpMarketingService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SubscribeUser(NewsLetterSubscriberModel subscriber)
        {
            try
            {
                var subscriberHash = CreateMd5Hash(subscriber.EmailAddress.ToLower());
                var url = $"https://{_options.DataCenter}.api.mailchimp.com/3.0/lists/{_options.ListId}/members/{subscriberHash}";
                var base64ApiKey = Convert.ToBase64String(Encoding.UTF8.GetBytes($"anystring:{_options.ApiKey}"));
                var authHeader = $"Basic {base64ApiKey}";

                // Check if the subscriber already exists (GET request)
                var getRequest = new HttpRequestMessage(HttpMethod.Get, url);
                getRequest.Headers.Add("Authorization", authHeader);

                var getResponse = await _httpClient.SendAsync(getRequest);

                if (getResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Subscriber does not exist, proceed to add
                    _logger.LogInformation($"Subscriber {subscriber.EmailAddress} does not exist. Proceeding to add.");
                    await AddOrUpdateSubscriber(url, authHeader, subscriber, "added");
                }
                else if (getResponse.IsSuccessStatusCode)
                {
                    // Subscriber exists, proceed to update
                    _logger.LogInformation($"Subscriber {subscriber.EmailAddress} exists. Proceeding to update.");
                    await AddOrUpdateSubscriber(url, authHeader, subscriber, "updated");
                }
                else
                {
                    var errorResponse = await getResponse.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to check subscriber status: {getResponse.StatusCode}, {errorResponse}");
                    throw new HttpRequestException($"MailChimp request failed: {errorResponse}");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Network error when subscribing user: {ex.Message}");
                throw new HttpRequestException("There was a problem processing your request. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error when subscribing user: {ex.Message}");
                throw new HttpRequestException("An unexpected error occurred while subscribing the user.", ex);
            }
        }

        private async Task AddOrUpdateSubscriber(string url, string authHeader, NewsLetterSubscriberModel subscriber, string action)
        {
            var memberData = new
            {
                email_address = subscriber.EmailAddress,
                status_if_new = "subscribed",
                merge_fields = new
                {
                    FNAME = subscriber.FirstName,
                    LNAME = subscriber.LastName,
                    SUBJECT = subscriber.SubjectArea,
                    //MMERGE16 = subscriber.SubjectArea, // Switch to this for production
                    LOCATION = subscriber.Location
                    //MMERGE4 = subscriber.Location // Switch to this for production
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(memberData), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = content
            };
            request.Headers.Add("Authorization", authHeader);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to {action} subscriber: {response.StatusCode}, {errorResponse}");
                throw new HttpRequestException($"MailChimp {action} failed: {response.StatusCode} - {errorResponse}");
            }
            else
            {
                _logger.LogInformation($"Successfully {action} subscriber: {subscriber.EmailAddress}");
            }
        }

        private static string CreateMd5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                foreach (var t in hashBytes)
                {
                    sb.Append(t.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
