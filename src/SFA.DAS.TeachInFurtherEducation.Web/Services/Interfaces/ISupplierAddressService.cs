using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces;
using System.Collections.Generic;
using System;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces
{
    public interface ISupplierAddressService
    {
        Task<List<SupplierAddressModel>> CreateSupplierAddresses(List<SupplierAddressModel> sourceAddresses, DateTime assetDate);
        Task<List<SupplierAddressModel>> GetSourceSupplierAddresses();

        Task<DateTime> GetSupplierAddressAssetLastPublishedDate();
        Task<List<SupplierAddressDistanceModel>> GetSuppliersWithinRadiusOfPostcode(string postcode, double distanceKm);

        Task<LocationModel?> GetSupplierPostcodeLocation(string postcode);
    }
}
