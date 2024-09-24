using Microsoft.EntityFrameworkCore;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Interfaces;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using NetTopologySuite.Geometries;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;

namespace SFA.DAS.TeachInFurtherEducation.Web.Data
{
    /// <summary>
    /// Provides an implementation for managing Address entities in SQL Server.
    /// </summary>
    public class SqlSupplierAddressRepository : SqlRepositoryBase<SupplierAddressModel>, ISupplierAddressRepository
    {
        public SqlSupplierAddressRepository(SqlDbContext context) : base(context) {}

        /// <inheritdoc />
        public async Task<IDictionary<DateTime, int>> GetAddressCountsGroupedByDate()
        {
            return await _context.SupplierAddresses
                .Where(a => a.IsActive)
                .GroupBy(a => a.LastUpdated)
                .Select(g => new { LastUpdated = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.LastUpdated, g => g.Count);
        }

        /// <inheritdoc />
        public async Task MarkOldAddressesAsInactive(DateTime latestDate)
        {
            var oldAddresses = await _context.SupplierAddresses
                .Where(a => a.IsActive && a.LastUpdated != latestDate)
                .ToListAsync();

            foreach (var address in oldAddresses)
            {
                address.IsActive = false;
            }

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<List<SupplierAddressDistanceModel>> GetAddressesWithinDistance(double latitude, double longitude, double distanceInKm)
        {
            // Convert distance from kilometers to meters (SQL Server uses meters for geospatial distance calculations)
            double distanceInMeters = distanceInKm * 1000;

            // Create the Point for the input latitude and longitude using NetTopologySuite
            var targetPoint = new Point(longitude, latitude) { SRID = 4326 };  // SRID 4326 is for WGS 84

            // Query to get the addresses within the given distance
            var addresses = await _context.SupplierAddresses
                .Where(a => a.IsActive && a.Location != null)
                .Where(a => a.Location!.Distance(targetPoint) <= distanceInMeters) 
                .OrderBy(a => a.Location!.Distance(targetPoint))
                .Select(a => new SupplierAddressDistanceModel
                {
                    Supplier = a,
                    Distance = (a.Location!.Distance(targetPoint)) / 1000 
                })
                .ToListAsync();

            return addresses;
        }

    }
}
