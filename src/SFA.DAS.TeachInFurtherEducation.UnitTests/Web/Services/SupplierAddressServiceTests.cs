using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class SupplierAddressServiceTests
{
    private readonly ISupplierAddressRepository _supplierAddressRepository;
    private readonly IContentService _contentService;
    private readonly IGeoLocationProvider _geoLocationProvider;
    private readonly ISpreadsheetParser _spreadsheetParser;
    private readonly ICompositeKeyGenerator<SupplierAddressModel> _compositeKeyGenerator;
    private readonly ILogger<SupplierAddressService> _logger;
    private readonly SupplierAddressService _service;

    public SupplierAddressServiceTests()
    {
        _supplierAddressRepository = A.Fake<ISupplierAddressRepository>();
        _contentService = A.Fake<IContentService>();
        _geoLocationProvider = A.Fake<IGeoLocationProvider>();
        _spreadsheetParser = A.Fake<ISpreadsheetParser>();
        _compositeKeyGenerator = A.Fake<ICompositeKeyGenerator<SupplierAddressModel>>();
        _logger = A.Fake<ILogger<SupplierAddressService>>();

        // Create a mock service provider
        var serviceProvider = A.Fake<IServiceProvider>();

        // Return the fake repository when GetRequiredService is called
        A.CallTo(() => serviceProvider.GetService(typeof(ISupplierAddressRepository)))
            .Returns(_supplierAddressRepository);

        var serviceScopeFactory = A.Fake<IServiceScopeFactory>();
        var serviceScope = A.Fake<IServiceScope>();

        A.CallTo(() => serviceScopeFactory.CreateScope()).Returns(serviceScope);
        A.CallTo(() => serviceScope.ServiceProvider).Returns(serviceProvider);

        _service = new SupplierAddressService(
            serviceScopeFactory,
            _supplierAddressRepository,
            _contentService,
            _geoLocationProvider,
            _spreadsheetParser,
            _compositeKeyGenerator,
            _logger
        );
    }


    [Fact]
    public async Task GetSourceSupplierAddresses_ShouldReturnAddresses_WhenAssetExists()
    {
        var asset = new Asset<byte[]>
        {
            Content = Encoding.UTF8.GetBytes("<xml></xml>"),
            Metadata = new AssetMetadata
            {
                Id = "asset-123",
                Filename = "data.xmls",
                Url = "https://example.com/assets/data.xmls",
                LastUpdated = DateTime.UtcNow
            }
        };

        A.CallTo(() => _contentService.GetAssetsByTags("supplierAddresses"))
            .Returns(Task.FromResult(new List<Asset<byte[]>> { asset }));

        var rawData = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string>
            {
                { "SupplierName", "Test Supplier" },
                { "AddressLine1", "123 Test St" },
                { "City", "Test City" },
                { "Postcode", "TST 1NG" },
                { "OrganisationType", "Type A" }
            }
        };

        A.CallTo(() => _spreadsheetParser.ParseAsync(asset.Content))
            .Returns(rawData);

        A.CallTo(() => _compositeKeyGenerator.GenerateKey(A<SupplierAddressModel>._))
            .Returns("unique-key");

        var result = await _service.GetSourceSupplierAddresses();

        Assert.Single(result);
        Assert.Equal("Test Supplier", result[0].OrganisationName);
        Assert.Equal("123 Test St", result[0].AddressLine1);
    }

    [Fact]
    public async Task GetSourceSupplierAddresses_ShouldThrowFileNotFoundException_WhenNoAssetFound()
    {
        A.CallTo(() => _contentService.GetAssetsByTags("supplierAddresses"))
            .Returns(Task.FromResult(new List<Asset<byte[]>>()));

        await Assert.ThrowsAsync<FileNotFoundException>(() => _service.GetSourceSupplierAddresses());
    }

    [Fact]
    public async Task GetSupplierAddressAssetLastPublishedDate_ShouldReturnLastUpdatedDate()
    {
        var asset = new Asset<byte[]>
        {
            Content = Encoding.UTF8.GetBytes("<xml></xml>"),
            Metadata = new AssetMetadata
            {
                Id = "asset-123",
                Filename = "data.xmls",
                Url = "https://example.com/assets/data.xmls",
                LastUpdated = DateTime.UtcNow
            }
        };

        A.CallTo(() => _contentService.GetAssetsByTags("supplierAddresses"))
            .Returns(Task.FromResult(new List<Asset<byte[]>> { asset }));

        var result = await _service.GetSupplierAddressAssetLastPublishedDate();

        Assert.Equal(asset.Metadata.LastUpdated, result);
    }

    [Fact]
    public async Task GetSupplierAddressAssetLastPublishedDate_ShouldThrowFileNotFoundException_WhenNoAssetFound()
    {
        A.CallTo(() => _contentService.GetAssetsByTags("supplierAddresses"))
            .Returns(Task.FromResult(new List<Asset<byte[]>>()));

        await Assert.ThrowsAsync<FileNotFoundException>(() => _service.GetSupplierAddressAssetLastPublishedDate());
    }

    [Fact]
    public async Task CreateSupplierAddresses_ShouldReturnAddresses_WhenCalledWithSourceAddresses()
    {
        var sourceAddresses = new List<SupplierAddressModel>
        {
            new SupplierAddressModel
            {
                OrganisationName = "Test Org",
                Postcode = "TST 1NG",
                City = "Test City",
                AddressLine1 = "123 Test St",
                Type = "Type A"
            }
        };

        var location = new Point(1.0, 1.0) { SRID = 4326 }; // Ensure you use the correct GeoPoint type
        A.CallTo(() => _geoLocationProvider.GetLocationByPostcode("TST 1NG"))
            .Returns(new LocationModel { Latitude = 1.0, Longitude = 1.0 });

        var result = await _service.CreateSupplierAddresses(sourceAddresses, DateTime.UtcNow);

        Assert.Single(result);
        Assert.NotNull(result[0].Location);
    }

    [Fact]
    public async Task GetSuppliersWithinRadiusOfPostcode_ShouldReturnAddresses_WhenPostcodeIsValid()
    {
        var postcodeLocation = new LocationModel { Latitude = 1.0, Longitude = 1.0 };
        A.CallTo(() => _geoLocationProvider.GetLocationByPostcode("TST 1NG"))
            .Returns(postcodeLocation);

        var supplierAddresses = new List<SupplierAddressDistanceModel>
        {
            new SupplierAddressDistanceModel
            {
                Supplier = new SupplierAddressModel()
                {
                    OrganisationName = "Test Org", 
                    Postcode = "TST 1NG",
                    City = "Test City",
                    AddressLine1 = "123 Test St",
                    Type = "Type A"
                },
                Distance = 5.0
            }
        };

        A.CallTo(() => _supplierAddressRepository.GetAddressesWithinDistance(1.0, 1.0, 10))
            .Returns(supplierAddresses);

        var result = await _service.GetSuppliersWithinRadiusOfPostcode("TST 1NG", 10);

        Assert.Single(result);
        Assert.Equal("Test Org", result[0].Supplier.OrganisationName);
    }

    [Fact]
    public async Task GetSourceSupplierAddresses_ShouldThrowInvalidOperationException_WhenAssetContentIsNull()
    {
        var asset = new Asset<byte[]>
        {
            Content = null,
            Metadata = null
        };

        A.CallTo(() => _contentService.GetAssetsByTags("supplierAddresses"))
            .Returns(Task.FromResult(new List<Asset<byte[]>> { asset }));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetSourceSupplierAddresses());
    }

    [Fact]
    public async Task GetSuppliersWithinRadiusOfPostcode_ShouldReturnEmptyList_WhenPostcodeIsInvalid()
    {
        A.CallTo(() => _geoLocationProvider.GetLocationByPostcode("INVALID"))
            .Returns((LocationModel)null);

        var result = await _service.GetSuppliersWithinRadiusOfPostcode("INVALID", 10);

        Assert.Empty(result);
    }
}
