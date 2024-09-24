using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.TeachInFurtherEducation.Web.Data.Models
{
    /// <summary>
    /// Represents a supplier's address and the distance to a specified location.
    /// </summary>
    public class SupplierAddressDistanceModel
    {
        /// <summary>
        /// Gets or sets the supplier's address information.
        /// </summary>
        public required SupplierAddressModel Supplier { get; set; }

        /// <summary>
        /// Gets or sets the distance to the supplier's address from a specified location, in kilometers.
        /// </summary>
        public required double Distance { get; set; }
    }
}
