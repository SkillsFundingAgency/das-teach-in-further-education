using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class SupplierSearchViewModel 
    {
        public string Title { get; init; }
        public string Heading { get; init; }
        public string NoResultMessage { get; init; }
        public string SuccessMessage { get; set; }
        public string ButtonText { get; init; }
        public int SearchWithinMiles { get; init; }
        public string? DataId { get; init; }

        public SupplierSearchViewModel(SupplierSearch supplierSearchContent )
        {
            Title = supplierSearchContent.SupplierSearchTitle;
            Heading = supplierSearchContent.Heading;
            NoResultMessage = supplierSearchContent.NoResultMessage;
            SuccessMessage = supplierSearchContent.SuccessMessage;
            ButtonText = supplierSearchContent.ButtonText;
            SearchWithinMiles = supplierSearchContent.SearchWithinMiles;
            DataId = supplierSearchContent.DataId;
        }

        [Required(ErrorMessage = "Postcode is required.")]
        public string? Postcode { get; set; }
        public string? PostcodeDistrict { get; set; }
        public string? LatLong { get; set; }

        public List<SupplierSearchResultViewModel>? SearchResults { get; set; }  
    }

    public class SupplierSearchResultViewModel
    {
        public string Name { get; init; }
        public string Website { get; init; }
        public string Address { get; init; }
        public string? Parent { get; init; }
        public double Distance { get; init; }

        public SupplierSearchResultViewModel(SupplierAddressDistanceModel searchResult)
        {
            const double milesPerKilometer = 0.621371;

            Name = searchResult.Supplier.OrganisationName;
            Website = searchResult.Supplier.Website;

            Address = AddressHelper.FormatAddress(
                searchResult.Supplier.AddressLine1,
                searchResult.Supplier.AddressLine2,
                searchResult.Supplier.AddressLine3,
                searchResult.Supplier.City,
                searchResult.Supplier.County,
                searchResult.Supplier.Postcode).ReplaceLineEndings("<br>");

            Parent = searchResult.Supplier.ParentOrganisation;

            double miles = searchResult.Distance * milesPerKilometer;

            Distance =Math.Round(miles, 1);
        }
    }
}
