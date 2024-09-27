using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using System.Diagnostics.CodeAnalysis;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Web;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services
{
    /// <summary>
    /// Provides geocoding services using the Postcodes.io API with exponential backoff for resilience.
    /// </summary>
    [ExcludeFromCodeCoverage] // Soon to be replaced
    public class PostcodesIoGeoLocationService : IGeoLocationProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PostcodesIoGeoLocationService> _logger;

        private const int MaxRetries = 5;
        private const int InitialDelayMilliseconds = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostcodesIoGeoLocationService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to send requests.</param>
        public PostcodesIoGeoLocationService(HttpClient httpClient, ILogger<PostcodesIoGeoLocationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Gets the geographical location (latitude and longitude) for a given postcode.
        /// </summary>
        /// <param name="postcode">The postcode for which to retrieve the location.</param>
        /// <returns>A <see cref="LocationDto"/> representing the geographical location.</returns>
        /// <exception cref="Exception">Thrown when no location is found or if all retry attempts fail.</exception>
        public async Task<LocationModel?> GetLocationByPostcode(string postcode)
        {
            var requestUri = $"https://api.postcodes.io/postcodes/{HttpUtility.UrlEncode(postcode)}";

            for (int retry = 0; retry < MaxRetries; retry++)
            {
                try
                {
                    var response = await _httpClient.GetAsync(requestUri);

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Return null if a 404 is returned, no retries needed
                        _logger.LogInformation($"Postcode {postcode} not found.");
                        return null;
                    }

                    response.EnsureSuccessStatusCode(); // Throws if status code is not 2xx

                    var content = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(content);

                    var result = json["result"];

                    if (result != null)
                    {
                        var latitude = result["latitude"]!.Value<double>();
                        var longitude = result["longitude"]!.Value<double>();

                        return new LocationModel
                        {
                            Latitude = latitude,
                            Longitude = longitude
                        };
                    }
                }
                catch (HttpRequestException ex) when (retry < MaxRetries - 1)
                {
                    // Log the exception and apply exponential backoff for non-404 errors
                    _logger.LogWarning(ex, $"Error retrieving location for postcode {postcode}. Retrying...");
                    await Task.Delay(InitialDelayMilliseconds * (int)Math.Pow(2, retry));
                }
            }

            return null;
        }
    }
}
