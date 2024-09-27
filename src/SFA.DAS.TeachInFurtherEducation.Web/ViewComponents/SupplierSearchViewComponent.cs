using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

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
                    var postcode = Request.Form["postcode"].ToString();
                    postcode = postcode.Replace("\n", "_").Replace("\r", "_");

                    int radiusMiles = int.TryParse(Request.Form["radius"], out var parsedRadius) ? parsedRadius : 25;
                    double radiusKm = radiusMiles * 1.60934;

                    if (!string.IsNullOrWhiteSpace(postcode))
                    {
                        var results = await _supplierAddressService.GetSuppliersWithinRadiusOfPostcode(postcode.Trim(), radiusKm);

                        model.Postcode = postcode;

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
