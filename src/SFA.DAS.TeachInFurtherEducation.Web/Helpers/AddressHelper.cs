using System.Collections.Generic;
using System.Linq;

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
    }

}
