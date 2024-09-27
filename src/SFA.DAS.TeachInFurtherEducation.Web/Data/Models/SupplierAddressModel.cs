using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.TeachInFurtherEducation.Web.Data.Models
{
    /// <summary>
    /// Represents an address with associated location data.
    /// </summary>
    public class SupplierAddressModel : ModelBase
    {
        [Required]
        [MaxLength(100)]
        [JsonProperty("organisationName")]
        public string OrganisationName { get; set; } = string.Empty;

        [MaxLength(100)]
        [JsonProperty("parentOrganisation")]
        public string ParentOrganisation { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [MaxLength(100)]
        [JsonProperty("area")]
        public string Area { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [JsonProperty("city")]
        public string City { get; set; } = string.Empty;

        [MaxLength(100)]
        [JsonProperty("addressLine1")]
        public string? AddressLine1 { get; set; }

        [MaxLength(100)]
        [JsonProperty("addressLine2")]
        public string? AddressLine2 { get; set; }

        [MaxLength(100)]
        [JsonProperty("addressLine3")]
        public string? AddressLine3 { get; set; }

        [MaxLength(50)]
        [JsonProperty("county")]
        public string? County { get; set; }

        [Required]
        [MaxLength(10)]
        [JsonProperty("postcode")]
        public string Postcode { get; set; } = string.Empty;

        [MaxLength(100)]
        [JsonProperty("telephone")]
        public string Telephone { get; set; } = string.Empty;

        [MaxLength(100)]
        [JsonProperty("website")]
        public string Website { get; set; } = string.Empty;

        [JsonProperty("location")]
        public Point? Location { get; set; }
    }

}
