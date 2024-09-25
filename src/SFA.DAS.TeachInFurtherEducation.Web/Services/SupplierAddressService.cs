using Contentful.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services
{

    /// <summary>
    /// Service for managing supplier addresses, syncing them with Cosmos DB, and providing location information.
    /// </summary>
    public class SupplierAddressService : ISupplierAddressService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ISupplierAddressRepository _supplierAddressRepository;
        private readonly IContentService _contentService;
        private readonly ISpreadsheetParser _spreadsheetParser;
        private readonly IGeoLocationProvider _geoLocationProvider;
        private readonly ICompositeKeyGenerator<SupplierAddressModel> _compositeKeyGenerator;


        private readonly ILogger<SupplierAddressService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupplierAddressService"/> class.
        /// </summary>
        /// <param name="contentService">The service responsible for retrieving content from Contentful.</param>
        /// <param name="spreadsheetParser">The service responsible for parsing the source spreadsheet.</param>
        /// <param name="logger">The logger used for logging errors and information.</param>
        public SupplierAddressService(
            IServiceScopeFactory serviceScopeFactory,
            ISupplierAddressRepository supplierAddressRepository,
            IContentService contentService,
            IGeoLocationProvider geoLocationProvider,
            ISpreadsheetParser spreadsheetParser,
            ICompositeKeyGenerator<SupplierAddressModel> compositeKeyGenerator,
            ILogger<SupplierAddressService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _supplierAddressRepository = supplierAddressRepository;
            _contentService = contentService;
            _geoLocationProvider = geoLocationProvider;
            _spreadsheetParser = spreadsheetParser;
            _compositeKeyGenerator = compositeKeyGenerator;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves supplier addresses from Contentful, parses them into a list of DTOs, and returns the list along with the last updated date of the asset.
        /// </summary>
        /// <returns>
        /// A tuple containing the last updated <see cref="DateTime"/> of the asset and a list of <see cref="SupplierAddressModel"/> containing parsed supplier addresses.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Thrown when no asset or multiple assets with the 'supplier-addresses' tag are found.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the supplier address spreadsheet asset is unexpectedly null.
        /// </exception>
        /// <remarks>
        /// This method assumes that exactly one asset tagged with 'supplierAddresses' exists in Contentful.
        /// The addresses are parsed from a spreadsheet, and the last updated date of the asset is returned alongside the list of addresses.
        /// </remarks>

        public async Task<List<SupplierAddressModel>> GetSourceSupplierAddresses()
        {
            _logger.LogInformation("Retrieving supplier address asset.");

            var assets = await _contentService.GetAssetsByTags("supplierAddresses");

            if (assets.Count != 1)
            {
                throw new FileNotFoundException("Expected exactly one asset with the 'supplier-addresses' tag, but found none or multiple.");
            }

            var supplierAddressesSpreadsheet = assets.SingleOrDefault();

            if (supplierAddressesSpreadsheet != null && supplierAddressesSpreadsheet.Content != null)
            {
                var rawData = await _spreadsheetParser.ParseAsync(supplierAddressesSpreadsheet.Content);
                var addresses = new List<SupplierAddressModel>();

                if (rawData.Count != 0)
                {
                    var getSafeKey = (string exp, string defaultKey) =>
                    {
                        return rawData[0].Keys.FirstOrDefault(k => Regex.IsMatch(k, exp, RegexOptions.IgnoreCase), defaultKey);
                    };

                    var orgsNameKey = getSafeKey(@"^.*?\sName\s*$", "SupplierName");
                    var parentOrgNameKey = getSafeKey(@"^.*?Parent.*?$", "ParentOrganisation");
                    var cityKey = getSafeKey(@"^.*?\scity\s*$", "City");
                    var areaKey = getSafeKey(@"^.*?\sarea\s*$", "Area");
                    var orgTypeKey = getSafeKey(@"^.*?\stype\s*$", "OrganisationType");
                    var addressLine1Key = getSafeKey(@"^.*?\saddress\s?(?:line\s)?1\s*$", "AddressLine1");
                    var addressLine2Key = getSafeKey(@"^.*?\saddress\s?(?:line\s)?2\s*$", "AddressLine2");
                    var addressLine3Key = getSafeKey(@"^.*?\saddress\s?(?:line\s)?3\s*$", "AddressLine3");
                    var countyKey = getSafeKey(@"^.*?\scounty\s*$", "County");
                    var postcodeKey = getSafeKey(@"^.*?\spost(?:al)?\s*code\s*$", "Postcode");
                    var telephoneKey = getSafeKey(@"^.*?\sswitch\s?board\s*$", "mainSwitchboard");
                    var websiteKey = getSafeKey(@".*?web\s*$", "Website");

                    foreach (var row in rawData)
                    {
                        var supplierName = row.GetValueOrDefault(orgsNameKey);

                        if (string.IsNullOrEmpty(supplierName)) continue;

                        var address = new SupplierAddressModel
                        {
                            Type = row.GetValueOrDefault(orgTypeKey)!,
                            OrganisationName = row.GetValueOrDefault(orgsNameKey)!,
                            ParentOrganisation = row.GetValueOrDefault(parentOrgNameKey)!,
                            AddressLine1 = row.GetValueOrDefault(addressLine1Key),
                            AddressLine2 = row.GetValueOrDefault(addressLine2Key),
                            AddressLine3 = row.GetValueOrDefault(addressLine3Key),
                            Area = row.GetValueOrDefault(areaKey)!,
                            City = row.GetValueOrDefault(cityKey)!,
                            County = row.GetValueOrDefault(countyKey),
                            Postcode = row.GetValueOrDefault(postcodeKey)!,
                            Telephone = row.GetValueOrDefault(telephoneKey)!,
                            Website = row.GetValueOrDefault(websiteKey)!,
                        };

                        address.Id = _compositeKeyGenerator.GenerateKey(address);

                        addresses.Add(address);
                    }
                }

                return addresses;
            }

            // In case the supplierAddressesSpreadsheet is null (although highly unlikely based on the logic)
            throw new InvalidOperationException("The supplier address spreadsheet asset was null.");
        }

        /// <summary>
        /// Retrieves the last published date of the supplier address asset from Contentful.
        /// </summary>
        /// <returns>
        /// The <see cref="DateTime"/> representing the last time the supplier address asset was published or updated.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Thrown when no asset or multiple assets with the 'supplier-addresses' tag are found.
        /// </exception>
        /// <remarks>
        /// This method assumes that there is exactly one asset tagged with 'supplierAddresses' in Contentful. 
        /// If the asset is not found or multiple assets are found, an exception will be thrown.
        /// </remarks>
        public async Task<DateTime> GetSupplierAddressAssetLastPublishedDate()
        {
            _logger.LogInformation("Retrieving supplier address asset.");

            var assets = await _contentService.GetAssetsByTags("supplierAddresses");

            if (assets.Count != 1)
            {
                throw new FileNotFoundException("Expected exactly one asset with the 'supplier-addresses' tag, but found none or multiple.");
            }

            var supplierAddressesSpreadsheet = assets.Single();

            return supplierAddressesSpreadsheet.Metadata.LastUpdated;
        }

        /// <summary>
        /// Creates supplier address models from the source supplier addresses.
        /// </summary>
        /// <param name="sourceAddresses">The source addresses to create models from.</param>
        /// <param name="assetDate">The last updated date of the source asset.</param>
        /// <returns>A list of supplier address models.</returns>
        public async Task<List<SupplierAddressModel>> CreateSupplierAddresses(List<SupplierAddressModel> sourceAddresses, DateTime assetDate)
        {
            var supplierAddressTasks = sourceAddresses.Select(async sourceAddress =>
            {
                try
                {
                    // Resolve a new DbContext for each task to ensure thread safety
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var repository = scope.ServiceProvider.GetRequiredService<ISupplierAddressRepository>();

                        var existingAddress = await repository.GetById(sourceAddress.Id, sourceAddress.Postcode);

                        var supplierAddress = new SupplierAddressModel
                        {
                            Id = sourceAddress.Id,
                            OrganisationName = sourceAddress.OrganisationName,
                            ParentOrganisation = sourceAddress.ParentOrganisation,
                            Type = sourceAddress.Type,
                            AddressLine1 = sourceAddress.AddressLine1,
                            AddressLine2 = sourceAddress.AddressLine2,
                            AddressLine3 = sourceAddress.AddressLine3,
                            Area = sourceAddress.Area,
                            City = sourceAddress.City,
                            County = sourceAddress.County,
                            Postcode = sourceAddress.Postcode,
                            Telephone = sourceAddress.Telephone,
                            Website = sourceAddress.Website,
                            LastUpdated = assetDate,
                            Location = existingAddress?.Location,
                            IsActive = true
                        };

                        if (supplierAddress.Location == null)
                        {
                            var latLong = await _geoLocationProvider.GetLocationByPostcode(sourceAddress.Postcode);
                            if (latLong != null)
                            {
                                supplierAddress.Location = new NetTopologySuite.Geometries.Point(latLong.Longitude, latLong.Latitude)
                                {
                                    SRID = 4326 // Standard SRID for geographic coordinates (WGS84)
                                };
                            }
                        }

                        return supplierAddress;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return null;
                }
            }).ToList();

            var supplierAddresses = await Task.WhenAll(supplierAddressTasks);

            return supplierAddresses.Where(addr => addr != null).ToList()!;
        }

        /// <summary>
        /// Retrieves a list of supplier addresses within a specified distance from a given postcode.
        /// </summary>
        /// <param name="postcode">The postcode to use as the reference point for the search.</param>
        /// <param name="distanceKm">The radius (in kilometers) within which to search for supplier addresses.</param>
        /// <returns>A <see cref="SupplierAddressResponseDto"/> containing the reference point and a list of supplier addresses within the specified distance, or null if the postcode is invalid.</returns>
        public async Task<List<SupplierAddressDistanceModel>> GetSuppliersWithinRadiusOfPostcode(string postcode, double distanceKm)
        {
            var retVal = new List<SupplierAddressDistanceModel>();

            var postcodeLocation = await _geoLocationProvider.GetLocationByPostcode(postcode);
            if (postcodeLocation != null)
            {
                var supplierAddresses = await _supplierAddressRepository.GetAddressesWithinDistance(postcodeLocation.Latitude, postcodeLocation.Longitude, distanceKm);
                retVal.AddRange(supplierAddresses);
            }

            return retVal;
        }
    }
}
