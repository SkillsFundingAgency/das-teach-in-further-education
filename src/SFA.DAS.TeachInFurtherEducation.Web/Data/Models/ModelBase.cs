using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System;

namespace SFA.DAS.TeachInFurtherEducation.Web.Data.Models
{
    /// <summary>
    /// Represents an address with associated location data.
    /// </summary>
    public abstract class ModelBase
    {
        [Required]
        [Key]   // This attribute specifies that the Id is the primary key
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [Required]
        [JsonProperty("lastUpdated")]
        public DateTime LastUpdated { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }
}
