using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services
{
    /// <summary>
    /// Configuration options for the MailChimpMarketingService.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MailChimpMarketingServiceOptions
    {
        /// <summary>
        /// Gets or sets the API key used to authenticate requests to the MailChimp API.
        /// </summary>
        public required string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the ID of the audience (list) where subscribers will be added.
        /// </summary>
        public required string ListId { get; set; }

        /// <summary>
        /// Gets or sets the data center associated with the MailChimp account (e.g., "us1", "us19").
        /// </summary>
        public required string DataCenter { get; set; }
    }
}
