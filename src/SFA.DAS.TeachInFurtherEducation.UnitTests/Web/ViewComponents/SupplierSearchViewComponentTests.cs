// SupplierSearchViewComponentTests.cs
using Castle.Core.Logging;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.ViewComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.ViewComponents
{
    public class SupplierSearchViewComponentTests
    {
        private readonly ISupplierAddressService _fakeSupplierAddressService;
        private readonly ILogger<SupplierSearchViewComponent> _fakeLogger;
        private readonly SupplierSearchViewComponent _viewComponent;

        public SupplierSearchViewComponentTests()
        {
            _fakeSupplierAddressService = A.Fake<ISupplierAddressService>();
            _fakeLogger = A.Fake<ILogger<SupplierSearchViewComponent>>();
            _viewComponent = new SupplierSearchViewComponent(_fakeSupplierAddressService, _fakeLogger);
        }

        [Fact]
        public async Task InvokeAsync_PostInvalidFormIdentifier_ShouldReturnViewWithModelWithoutSearchResults()
        {
            // Arrange
            var supplierSearchContent = new SupplierSearch
            {
                SupplierSearchTitle = "Search Title",
                Heading = "Search Heading",
                NoResultsMessage = "No suppliers found.",
                ButtonText = "Search",
                SearchWithinMiles = 25
            };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = "POST";
            httpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "formIdentifier", "supplier-search" },
                { "postcode", "SW1A 1AA" },
                { "radius", "10" }
            });

            _viewComponent.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    HttpContext = httpContext
                }
            };

            var fakeSuppliers = new List<SupplierAddressDistanceModel>
            {
                new SupplierAddressDistanceModel
                {
                    Supplier = new SupplierAddressModel
                    {
                        OrganisationName = "Supplier A",
                        Postcode = "SW1A 1AA",
                    },
                    Distance = 4.4
                },
                new SupplierAddressDistanceModel
                {
                    Supplier = new SupplierAddressModel
                    {
                        OrganisationName = "Supplier B",
                        Postcode = "SW1B 1BB",
                    },
                    Distance = 2.2
                },
            };

            A.CallTo(() => _fakeSupplierAddressService.GetSuppliersWithinRadiusOfPostcode("SW1A 1AA", 16.0934))
                .Returns(Task.FromResult(fakeSuppliers));

            // Act
            var result = await _viewComponent.InvokeAsync(supplierSearchContent);

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            var model = Assert.IsType<SupplierSearchViewModel>(viewResult.ViewData.Model);
            Assert.Equal(supplierSearchContent.Heading, model.Heading);
            Assert.Equal(supplierSearchContent.ButtonText, model.ButtonText);
            Assert.Equal(supplierSearchContent.SupplierSearchTitle, model.Title);
            Assert.Equal(supplierSearchContent.SearchWithinMiles, model.SearchWithinMiles);
            Assert.Equal(supplierSearchContent.NoResultsMessage, model.NoResultsMessage);

            Assert.Equal("SW1A 1AA", model.Postcode);
            Assert.NotNull(model.SearchResults);
            Assert.Equal(2, model.SearchResults.Count);

            Assert.Equal("Supplier B", model.SearchResults[0].Name);
            Assert.Equal(Math.Round(2.2 / 1.60934, 1), model.SearchResults[0].Distance);

            Assert.Equal("Supplier A", model.SearchResults[1].Name);
            Assert.Equal(Math.Round(4.4 / 1.60934, 1), model.SearchResults[1].Distance);
        }

        [Fact]
        public async Task InvokeAsync_EmptyPostcode_ValidationMessageUpdated()
        {
            // Arrange
            var supplierSearchContent = new SupplierSearch
            {
                SupplierSearchTitle = "Search Title",
                Heading = "Search Heading",
                NoResultsMessage = "No suppliers found.",
                ButtonText = "Search",
                SearchWithinMiles = 25
            };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = "POST";
            httpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "formIdentifier", "supplier-search" },
                { "postcode", "" },
                { "radius", "10" }
            });

            _viewComponent.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    HttpContext = httpContext
                }
            };
            
            // Act
            var result = await _viewComponent.InvokeAsync(supplierSearchContent);

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            var modelState = Assert.IsType<ModelStateDictionary>(viewResult.ViewData.ModelState);
            
            Assert.False(modelState.IsValid);
            Assert.True(modelState.ContainsKey("PostCode"));
            Assert.Equal(1, modelState.ErrorCount);
        }

        [Fact]
        public async Task InvokeAsync_InvalidPostcode_ValidationMessageUpdated()
        {
            // Arrange
            var supplierSearchContent = new SupplierSearch
            {
                SupplierSearchTitle = "Search Title",
                Heading = "Search Heading",
                NoResultsMessage = "No suppliers found.",
                ButtonText = "Search",
                SearchWithinMiles = 25
            };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = "POST";
            httpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "formIdentifier", "supplier-search" },
                { "postcode", "NOT A VALID POSTCODE" },
                { "radius", "10" }
            });

            _viewComponent.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    HttpContext = httpContext
                }
            };

            // Act
            var result = await _viewComponent.InvokeAsync(supplierSearchContent);

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            var modelState = Assert.IsType<ModelStateDictionary>(viewResult.ViewData.ModelState);

            Assert.False(modelState.IsValid);
            Assert.True(modelState.ContainsKey("PostCode"));
            Assert.Equal(1, modelState.ErrorCount);
        }
    }
}
