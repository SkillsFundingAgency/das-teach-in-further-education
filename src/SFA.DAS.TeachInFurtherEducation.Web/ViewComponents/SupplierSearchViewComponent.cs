﻿
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Web.Helpers;

namespace SFA.DAS.TeachInFurtherEducation.Web.ViewComponents
{
    public class SupplierSearchViewComponent : ViewComponent
    {
        const string _formIdentifier = "supplier-search";
        private readonly ISupplierAddressService _supplierAddressService;

        public SupplierSearchViewComponent(ISupplierAddressService supplierAddressService)
        {
            _supplierAddressService = supplierAddressService;
        }

        public async Task<IViewComponentResult> InvokeAsync(SupplierSearch supplierSearchContent)
        {
            var model = new SupplierSearchViewModel(supplierSearchContent);

            // Check if the form has been submitted
            if (Request.Method == "POST")
            {
                var formIdentitfier = Request.Form["formIdentifier"]!.ToString();
                if (formIdentitfier == _formIdentifier)
                {
                    // Populate the model from the form values
                    var postcode = Request.Form["postcode"].ToString();
                    postcode = postcode.Replace("\n", "_").Replace("\r", "_");

                    int radiusMiles = int.TryParse(Request.Form["radius"], out var parsedRadius) ? parsedRadius : 25;
                    double radiusKm = radiusMiles * 1.60934;


                    // Perform validation of the postcode
                    if (string.IsNullOrWhiteSpace(postcode))
                    {
                        ModelState.AddModelError("Postcode", "Postcode is required.");
                    }
                    else if (!AddressHelper.ValidateUKPostcode(postcode))
                    {
                        ModelState.AddModelError("Postcode", "Please enter a valid UK postcode.");
                    }

                    // If validation fails, return the view with the model containing errors
                    if (!ModelState.IsValid)
                    {
                        return View("Default", model);
                    }

                    // If valid, proceed with the search
                    var results = await _supplierAddressService.GetSuppliersWithinRadiusOfPostcode(postcode.Trim(), radiusKm);

                    model.Postcode = postcode.ToUpper();

                    model.SearchResults = results
                        .Select(r => new SupplierSearchResultViewModel(r))
                        .OrderBy(r => r.Distance)
                        .ThenBy(r => r.Name)
                        .ToList();
                }
            }

            return View("Default", model);
        }
    }
}
