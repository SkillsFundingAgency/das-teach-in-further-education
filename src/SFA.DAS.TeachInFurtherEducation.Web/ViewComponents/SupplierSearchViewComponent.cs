
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Web.Helpers;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace SFA.DAS.TeachInFurtherEducation.Web.ViewComponents
{
    public class SupplierSearchViewComponent : ViewComponent
    {
        const string _formIdentifier = "supplier-search";
        private readonly ISupplierAddressService _supplierAddressService;
        private readonly ILogger<SupplierSearchViewComponent> _logger;

        public SupplierSearchViewComponent(ISupplierAddressService supplierAddressService, ILogger<SupplierSearchViewComponent> logger)
        {
            _supplierAddressService = supplierAddressService;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(SupplierSearch supplierSearchContent)
        {
            var postcodeDistrictExp = new Regex(@"^\s*([a-zA-Z0-9]+)", RegexOptions.IgnoreCase, new TimeSpan(0, 0, 2));

            var model = new SupplierSearchViewModel(supplierSearchContent);

            // Check if the form has been submitted
            if (Request.Method == "POST")
            {
                var formIdentitfier = Request.Form["formIdentifier"]!.ToString();
                if (formIdentitfier == _formIdentifier)
                {
                    // Populate the model from the form values
                    var postcode = Request.Form["postcode"].ToString().Trim();
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

                    var results = new List<SupplierAddressDistanceModel>();

                    try
                    {
                        results.AddRange(await _supplierAddressService.GetSuppliersWithinRadiusOfPostcode(postcode.Trim(), radiusKm));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occured while performing a search for supplier addresses");
                    }
                    finally
                    {
                        model.Postcode = postcode.ToUpper();
                        if (postcodeDistrictExp.IsMatch(model.Postcode!))
                        {
                            model.PostcodeDistrict = postcodeDistrictExp.Match(model.Postcode!).Groups[1].Value;
                        }

                        model.SuccessMessage = model.SuccessMessage ?? $"Nearest colleges to {model.Postcode}";

                        model.SearchResults = results
                            .Select(r => new SupplierSearchResultViewModel(r))
                            .OrderBy(r => r.Distance)
                            .ThenBy(r => r.Name)
                            .ToList();
                    }
                }
            }

            return View("Default", model);
        }
    }
}
