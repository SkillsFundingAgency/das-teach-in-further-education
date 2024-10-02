using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SFA.DAS.TeachInFurtherEducation.Web.Helpers
{
    public static class AddressHelper
    {
        /// <summary>
        /// Formats the address components into a multi-line string, skipping any empty values.
        /// </summary>
        /// <param name="addressLine1">Address Line 1</param>
        /// <param name="addressLine2">Address Line 2</param>
        /// <param name="addressLine3">Address Line 3</param>
        /// <param name="city">City</param>
        /// <param name="county">County</param>
        /// <param name="postcode">Postcode</param>
        /// <returns>Formatted address string with each component on a separate line.</returns>
        public static string FormatAddress(
            string? addressLine1,
            string? addressLine2,
            string? addressLine3,
            string? city,
            string? county,
            string? postcode)
        {
            // Use a StringBuilder for efficient string concatenation
            var addressComponents = new List<string>();

            if (!string.IsNullOrWhiteSpace(addressLine1)) addressComponents.AddRange(addressLine1.Split(",").Select(a => a.Trim()));
            if (!string.IsNullOrWhiteSpace(addressLine2)) addressComponents.AddRange(addressLine2.Split(",").Select(a => a.Trim()));
            if (!string.IsNullOrWhiteSpace(addressLine3)) addressComponents.AddRange(addressLine3.Split(",").Select(a => a.Trim()));
            if (!string.IsNullOrWhiteSpace(city)) addressComponents.Add(city);
            if (!string.IsNullOrWhiteSpace(county)) addressComponents.Add(county);
            if (!string.IsNullOrWhiteSpace(postcode)) addressComponents.Add(postcode);

            // Join non-empty components with a newline to format them as multi-line
            return string.Join("\n", addressComponents.Where(a => !string.IsNullOrWhiteSpace(a)));
        }

        /// <summary>
        /// Validates whether the provided string is a valid UK postcode.
        /// </summary>
        /// <param name="postcode">The postcode to validate.</param>
        /// <returns>
        /// <c>true</c> if the postcode is valid according to UK postcode rules; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method checks the format of the postcode against common UK postcode patterns,
        /// including the removal of spaces and conversion to uppercase. A timeout is applied
        /// to prevent excessive evaluation time on complex input strings.
        /// </remarks>
        public static bool ValidateUKPostcode(string postcode, int timeoutMs = 500)
        {
            if (string.IsNullOrWhiteSpace(postcode))
            {
                return false;
            }

            // Regular expression for UK postcodes
            string postcodePattern = @"^([A-Z]{1,2}\d[A-Z\d]?|\d[A-Z]{2})\s*\d[A-Z]{2}$";

            // Remove spaces and convert to uppercase for consistency
            string cleanedPostcode = postcode.Replace(" ", "").ToUpper();

            try
            {
                // Set a reasonable timeout for the regex match (e.g., 500 milliseconds)
                if (timeoutMs <= 0) throw new RegexMatchTimeoutException();
                TimeSpan regexTimeout = TimeSpan.FromMilliseconds(timeoutMs);

                Regex regex = new Regex(postcodePattern, RegexOptions.None, regexTimeout);

                // Return true if the postcode matches the pattern, false otherwise
                return regex.IsMatch(cleanedPostcode);
            }
            catch (RegexMatchTimeoutException)
            {
                // If a timeout occurs, return false to indicate an invalid postcode
                return false;
            }
        }
    }
}
