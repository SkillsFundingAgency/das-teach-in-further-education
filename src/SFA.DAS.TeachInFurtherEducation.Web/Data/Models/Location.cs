namespace SFA.DAS.TeachInFurtherEducation.Web.Data.Models
{
    /// <summary>
    /// Represents a geographical point for geospatial queries.
    /// </summary>
    public class LocationModel
    {
        /// <summary>
        /// Gets or sets the latitude of the point.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the point.
        /// </summary>
        public double Longitude { get; set; }
    }
}
