using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;

namespace SFA.DAS.TeachInFurtherEducation.Web.Data.Interfaces
{
    /// <summary>
    /// Defines the contract for a repository that manages Address entities.
    /// </summary>
    public interface ISupplierAddressRepository : IRepository<SupplierAddressModel>
    {
        /// <summary>
        /// Retrieves all addresses grouped by their date.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, with a result containing a dictionary where the key is the date and the value is the count of addresses for that date.</returns>
        Task<IDictionary<DateTime, int>> GetAddressCountsGroupedByDate();

        /// <summary>
        /// Retrieves all addresses within a certain radius of the latitude and longitude provided
        /// </summary>
        /// <param name="latitude">The latitude of the reference point</param>
        /// <param name="longitude">The longitude of the reference point</param>
        /// <param name="distanceInKm">The radius from the reference point in which to search defined as km.</param>
        /// <returns>A collection of supplier addresses that are within the specified radius of the reference point</returns>
        Task<List<SupplierAddressDistanceModel>> GetAddressesWithinDistance(double latitude, double longitude, double distanceInKm);

        /// <summary>
        /// Marks all active addresses as inactive if they have not been updated to the latest date.
        /// This ensures that only addresses updated in the current batch remain active.
        /// </summary>
        /// <param name="latestDate">The latest date when addresses were updated.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task MarkOldAddressesAsInactive(DateTime latestDate);
    }
}
