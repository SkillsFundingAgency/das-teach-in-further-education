// SupplierAddressUpdateServiceTests.cs
using AutoFixture;
using Cronos;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.TeachInFurtherEducation.Contentful.Exceptions;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.BackgroundServices;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.BackgroundServices
{
    public class SupplierAddressUpdateServiceTests : IDisposable
    {
        private readonly Fixture _fixture;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceScope _serviceScope;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SupplierAddressUpdateService> _logger;
        private readonly IOptions<SupplierAddressUpdateServiceOptions> _options;
        private readonly SupplierAddressUpdateService _service;
        private readonly ISupplierAddressRepository _supplierAddressRepository;
        private readonly ISupplierAddressService _supplierAddressService;
        private readonly CronExpression _cronExpression;

        public SupplierAddressUpdateServiceTests()
        {
            _fixture = new Fixture();

            // Create mocks
            _serviceScopeFactory = A.Fake<IServiceScopeFactory>();
            _serviceScope = A.Fake<IServiceScope>();
            _serviceProvider = A.Fake<IServiceProvider>();
            _logger = A.Fake<ILogger<SupplierAddressUpdateService>>();
            _supplierAddressRepository = A.Fake<ISupplierAddressRepository>();
            _supplierAddressService = A.Fake<ISupplierAddressService>();

            // Setup IServiceProvider to return required services
            A.CallTo(() => _serviceScope.ServiceProvider).Returns(_serviceProvider);
            A.CallTo(() => _serviceScopeFactory.CreateScope()).Returns(_serviceScope);
            A.CallTo(() => _serviceProvider.GetService(typeof(ISupplierAddressRepository)))
                .Returns(_supplierAddressRepository);
            A.CallTo(() => _serviceProvider.GetService(typeof(ISupplierAddressService)))
                .Returns(_supplierAddressService);

            // Setup options
            var options = new SupplierAddressUpdateServiceOptions
            {
                Enabled = true,
                CronSchedule = "*/5 * * * *" // Every 5 minutes
            };
            _options = Options.Create(options);

            // Initialize CronExpression
            _cronExpression = CronExpression.Parse(options.CronSchedule);

            // Initialize the service
            _service = new SupplierAddressUpdateService(
                _serviceScopeFactory,
                _options,
                _logger
            );
        }

        public void Dispose()
        {
            _service.Dispose();
        }

        [Fact]
        public async Task StartAsync_ServiceEnabled_ShouldPerformInitialUpdateAndSetTimer()
        {

            var sourceSuppliers = new List<SupplierAddressModel>();
            sourceSuppliers.Add(new SupplierAddressModel()
            {
                Id = "B22408D8-DAA3-4043-9AA8-C55FE30EE4CE",
                OrganisationName = "Test Supplier",
                ParentOrganisation = "Parent Organisation",
                AddressLine1 = "Address Line 1",
                AddressLine2 = "Address Line 2",
                AddressLine3 = "Address Line 3",
                Area = "Midlands",
                City = "City",
                County = "County",
                Postcode = "TE5 T1G",
                Telephone = "_44 (0)1221 122 121",
                Type = "College",
                Website = "http://TestSupplier.com",
                IsActive = true,
                Location = new NetTopologySuite.Geometries.Point(1.0001, 32.0001),
                LastUpdated = DateTime.Parse("01 January 2024 12:00:00"),
            });

            // Arrange
            A.CallTo(() => _supplierAddressService.GetSupplierAddressAssetLastPublishedDate())
                .Returns(Task.FromResult(DateTime.UtcNow));
            A.CallTo(() => _supplierAddressService.GetSourceSupplierAddresses())
                .Returns(Task.FromResult(sourceSuppliers));
            A.CallTo(() => _supplierAddressRepository.GetAddressCountsGroupedByDate())
                .Returns(Task.FromResult((IDictionary<DateTime, int>)new Dictionary<DateTime, int>()));
            A.CallTo(() => _supplierAddressService.CreateSupplierAddresses(A<List<SupplierAddressModel>>._, A<DateTime>._))
                .Returns(Task.FromResult(sourceSuppliers));
            A.CallTo(() => _supplierAddressRepository.MarkOldAddressesAsInactive(A<DateTime>._))
                .Returns(Task.CompletedTask);

            // Act
            await _service.StartAsync(CancellationToken.None);

            // Assert
            A.CallTo(() => _supplierAddressService.GetSupplierAddressAssetLastPublishedDate())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _supplierAddressService.GetSourceSupplierAddresses())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _supplierAddressRepository.GetAddressCountsGroupedByDate())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _supplierAddressService.CreateSupplierAddresses(A<List<SupplierAddressModel>>._, A<DateTime>._))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _supplierAddressRepository.MarkOldAddressesAsInactive(A<DateTime>._))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        public async Task PerformSupplierAddressUpdate_NullOrEmptySourceSupplierList_ShouldLogWarning(List<SupplierAddressModel> sourceSuppliers)
        {
            // Arrange
            A.CallTo(() => _supplierAddressService.GetSupplierAddressAssetLastPublishedDate())
                .Returns(Task.FromResult(DateTime.UtcNow));
            A.CallTo(() => _supplierAddressService.GetSourceSupplierAddresses())
                .Returns(Task.FromResult(sourceSuppliers));
            A.CallTo(() => _supplierAddressRepository.GetAddressCountsGroupedByDate())
                .Returns(Task.FromResult((IDictionary<DateTime, int>)new Dictionary<DateTime, int>()));
            A.CallTo(() => _supplierAddressService.CreateSupplierAddresses(A<List<SupplierAddressModel>>._, A<DateTime>._))
                .Returns(Task.FromResult(sourceSuppliers));
            A.CallTo(() => _supplierAddressRepository.MarkOldAddressesAsInactive(A<DateTime>._))
                .Returns(Task.CompletedTask);

            // Act
            await _service.StartAsync(CancellationToken.None);

            _logger.VerifyLogMustHaveHappened(LogLevel.Warning, "No supplier addresses were returned from the service.");
        }

        [Fact]
        public async Task PerformSupplierAddressUpdate_DataAlreadyUpToDate_ShouldLogInformation()
        {
            var lastUpdated = DateTime.Parse("01 January 2024 12:00:00");

            var sourceSuppliers = new List<SupplierAddressModel>();
            sourceSuppliers.Add(new SupplierAddressModel()
            {
                Id = "B22408D8-DAA3-4043-9AA8-C55FE30EE4CE",
                OrganisationName = "Test Supplier",
                ParentOrganisation = "Parent Organisation",
                AddressLine1 = "Address Line 1",
                AddressLine2 = "Address Line 2",
                AddressLine3 = "Address Line 3",
                Area = "Midlands",
                City = "City",
                County = "County",
                Postcode = "TE5 T1G",
                Telephone = "_44 (0)1221 122 121",
                Type = "College",
                Website = "http://TestSupplier.com",
                IsActive = true,
                Location = new NetTopologySuite.Geometries.Point(1.0001, 32.0001),
                LastUpdated = lastUpdated,
            });

            var addressCountsGroupedByDate = new Dictionary<DateTime, int>();
            addressCountsGroupedByDate.Add(lastUpdated, 1);


            // Arrange
            A.CallTo(() => _supplierAddressService.GetSupplierAddressAssetLastPublishedDate())
                .Returns(Task.FromResult(lastUpdated));
            A.CallTo(() => _supplierAddressService.GetSourceSupplierAddresses())
                .Returns(Task.FromResult(sourceSuppliers));
            A.CallTo(() => _supplierAddressRepository.GetAddressCountsGroupedByDate())
                .Returns(Task.FromResult((IDictionary<DateTime, int>)addressCountsGroupedByDate));
            A.CallTo(() => _supplierAddressService.CreateSupplierAddresses(A<List<SupplierAddressModel>>._, A<DateTime>._))
                .Returns(Task.FromResult(sourceSuppliers));
            A.CallTo(() => _supplierAddressRepository.MarkOldAddressesAsInactive(A<DateTime>._))
                .Returns(Task.CompletedTask);

            // Act
            await _service.StartAsync(CancellationToken.None);

            _logger.VerifyLogMustHaveHappened(LogLevel.Information, "The supplier data retrieved matches the supplier data in the database. Skipping update.");
        }

        [Fact]
        public async Task StartAsync_ServiceDisabled_ShouldNotPerformUpdateAndLogDisabled()
        {
            // Arrange
            var disabledOptions = Options.Create(new SupplierAddressUpdateServiceOptions
            {
                Enabled = false,
                CronSchedule = "*/5 * * * *" // Valid cron schedule
            });

            var disabledService = new SupplierAddressUpdateService(
                _serviceScopeFactory,
                disabledOptions,
                _logger
            );

            // Act
            await disabledService.StartAsync(CancellationToken.None);

            _logger.VerifyLogMustNotHaveHappened(LogLevel.Information, "Supplier address update completed.");
        }

        [Fact]
        public async Task PerformSupplierAddressUpdate_NoChangesSinceLastPublicationDate_ShouldLogInformation()
        {
            var lastUpdated = DateTime.Parse("01 January 2024 12:00:00");

            var sourceSuppliers = new List<SupplierAddressModel>();
            sourceSuppliers.Add(new SupplierAddressModel()
            {
                Id = "B22408D8-DAA3-4043-9AA8-C55FE30EE4CE",
                OrganisationName = "Test Supplier",
                ParentOrganisation = "Parent Organisation",
                AddressLine1 = "Address Line 1",
                AddressLine2 = "Address Line 2",
                AddressLine3 = "Address Line 3",
                Area = "Midlands",
                City = "City",
                County = "County",
                Postcode = "TE5 T1G",
                Telephone = "_44 (0)1221 122 121",
                Type = "College",
                Website = "http://TestSupplier.com",
                IsActive = true,
                Location = new NetTopologySuite.Geometries.Point(1.0001, 32.0001),
                LastUpdated = lastUpdated,
            });

            var addressCountsGroupedByDate = new Dictionary<DateTime, int>();
            addressCountsGroupedByDate.Add(lastUpdated, 1);

            // Setup options
            var options = new SupplierAddressUpdateServiceOptions
            {
                Enabled = true,
                CronSchedule = "*/5 * * * *" // Every 5 minutes
            };
            var serviceOptions = Options.Create(options);

            var service = new SupplierAddressUpdateService(
                _serviceScopeFactory,
                serviceOptions,
                _logger,
                lastUpdated
            );

            // Arrange
            A.CallTo(() => _supplierAddressService.GetSupplierAddressAssetLastPublishedDate())
                .Returns(Task.FromResult(lastUpdated));
            A.CallTo(() => _supplierAddressService.GetSourceSupplierAddresses())
                .Returns(Task.FromResult(sourceSuppliers));
            A.CallTo(() => _supplierAddressRepository.GetAddressCountsGroupedByDate())
                .Returns(Task.FromResult((IDictionary<DateTime, int>)addressCountsGroupedByDate));
            A.CallTo(() => _supplierAddressService.CreateSupplierAddresses(A<List<SupplierAddressModel>>._, A<DateTime>._))
                .Returns(Task.FromResult(sourceSuppliers));
            A.CallTo(() => _supplierAddressRepository.MarkOldAddressesAsInactive(A<DateTime>._))
                .Returns(Task.CompletedTask);
            
            // Act
            await service.StartAsync(CancellationToken.None);

            _logger.VerifyLogMustHaveHappened(LogLevel.Information, $"Supplier addresses have not been published since the last update at: {lastUpdated}");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task PerformSupplierAddressUpdate_NoCronScheduleProvided_ShouldLogInformation(string cronSchedule)
        {
            var lastUpdated = DateTime.Parse("01 January 2024 12:00:00");

            var sourceSuppliers = new List<SupplierAddressModel>();
            sourceSuppliers.Add(new SupplierAddressModel()
            {
                Id = "B22408D8-DAA3-4043-9AA8-C55FE30EE4CE",
                OrganisationName = "Test Supplier",
                ParentOrganisation = "Parent Organisation",
                AddressLine1 = "Address Line 1",
                AddressLine2 = "Address Line 2",
                AddressLine3 = "Address Line 3",
                Area = "Midlands",
                City = "City",
                County = "County",
                Postcode = "TE5 T1G",
                Telephone = "_44 (0)1221 122 121",
                Type = "College",
                Website = "http://TestSupplier.com",
                IsActive = true,
                Location = new NetTopologySuite.Geometries.Point(1.0001, 32.0001),
                LastUpdated = lastUpdated,
            });

            var addressCountsGroupedByDate = new Dictionary<DateTime, int>();
            addressCountsGroupedByDate.Add(lastUpdated, 1);

            // Setup options
            var options = new SupplierAddressUpdateServiceOptions
            {
                Enabled = true,
                CronSchedule = cronSchedule
            };
            var serviceOptions = Options.Create(options);

            var service = new SupplierAddressUpdateService(
                _serviceScopeFactory,
                serviceOptions,
                _logger,
                lastUpdated
            );

            // Arrange
            A.CallTo(() => _supplierAddressService.GetSupplierAddressAssetLastPublishedDate())
                .Returns(Task.FromResult(lastUpdated));
            A.CallTo(() => _supplierAddressService.GetSourceSupplierAddresses())
                .Returns(Task.FromResult(sourceSuppliers));
            A.CallTo(() => _supplierAddressRepository.GetAddressCountsGroupedByDate())
                .Returns(Task.FromResult((IDictionary<DateTime, int>)addressCountsGroupedByDate));
            A.CallTo(() => _supplierAddressService.CreateSupplierAddresses(A<List<SupplierAddressModel>>._, A<DateTime>._))
                .Returns(Task.FromResult(sourceSuppliers));
            A.CallTo(() => _supplierAddressRepository.MarkOldAddressesAsInactive(A<DateTime>._))
                .Returns(Task.CompletedTask);

            // Act
            await service.StartAsync(CancellationToken.None);

            _logger.VerifyLogMustHaveHappened(LogLevel.Information, $"Supplier addresses have not been published since the last update at: {lastUpdated}");
        }

        [Fact]
        public async Task PerformSupplierAddressUpdate_FileNotFoundException_ShouldLogError()
        {
            // Arrange
            var exception = new FileNotFoundException("Asset not found");

            // Mock the method to return a valid lastUpdated date
            A.CallTo(() => _supplierAddressService.GetSupplierAddressAssetLastPublishedDate())
                .Returns(Task.FromResult(DateTime.UtcNow));

            // Mock the method to throw FileNotFoundException
            A.CallTo(() => _supplierAddressService.GetSourceSupplierAddresses())
                .ThrowsAsync(exception);

            // Act
            await _service.StartAsync(CancellationToken.None);

            // Assert
            _logger.VerifyLogMustHaveHappened(
                LogLevel.Error,
                "Supplier addresses asset not found."
            );
        }

        [Fact]
        public async Task PerformSupplierAddressUpdate_InvalidOperationException_ShouldLogError()
        {
            // Arrange
            var exception = new InvalidOperationException("Invalid operation");

            // Mock the method to return a valid lastUpdated date
            A.CallTo(() => _supplierAddressService.GetSupplierAddressAssetLastPublishedDate())
                .Returns(Task.FromResult(DateTime.UtcNow));

            // Mock the method to throw InvalidOperationException
            A.CallTo(() => _supplierAddressService.GetSourceSupplierAddresses())
                .ThrowsAsync(exception);

            // Act
            await _service.StartAsync(CancellationToken.None);

            // Assert
            _logger.VerifyLogMustHaveHappened(
                LogLevel.Error,
                "Invalid operation while retrieving supplier addresses."
            );
        }

        [Fact]
        public async Task PerformSupplierAddressUpdate_GeneralException_ShouldLogError()
        {
            // Arrange
            var exception = new Exception("General error occurred");

            // Mock the method to return a valid lastUpdated date
            A.CallTo(() => _supplierAddressService.GetSupplierAddressAssetLastPublishedDate())
                .Returns(Task.FromResult(DateTime.UtcNow));

            // Mock the method to throw a general Exception
            A.CallTo(() => _supplierAddressService.GetSourceSupplierAddresses())
                .ThrowsAsync(exception);

            // Act
            await _service.StartAsync(CancellationToken.None);

            // Assert
            _logger.VerifyLogMustHaveHappened(
                LogLevel.Error,
                "An error occurred while retrieving supplier addresses."
            );
        }

        [Fact]
        public async Task PerformSupplierAddressUpdate_WhenAddOrUpdateThrowsException_ShouldLogError()
        {
            // Arrange
            var supplierAddresses = new List<SupplierAddressModel>
            {
                new SupplierAddressModel
                {
                    Id = "B22408D8-DAA3-4043-9AA8-C55FE30EE4CE",
                    OrganisationName = "Test Supplier",
                    Postcode = "TE5 T1G"
                }
            };

            var lastUpdated = DateTime.UtcNow;
            var exception = new Exception("An error occurred during AddOrUpdate");

            A.CallTo(() => _supplierAddressService.GetSupplierAddressAssetLastPublishedDate())
                .Returns(lastUpdated);

            A.CallTo(() => _supplierAddressService.GetSourceSupplierAddresses())
                .Returns(supplierAddresses);

            A.CallTo(() => _supplierAddressRepository.GetAddressCountsGroupedByDate())
                .Returns(new Dictionary<DateTime, int>());

            // Ensure CreateSupplierAddresses returns a non-empty list
            A.CallTo(() => _supplierAddressService.CreateSupplierAddresses(A<List<SupplierAddressModel>>._, A<DateTime>._))
                .Returns(supplierAddresses);

            // Mock the repository to throw an exception during the AddOrUpdate call
            A.CallTo(() => _supplierAddressRepository.AddOrUpdate(A<SupplierAddressModel>._, A<string>._))
                .Throws(exception);

            // Act
            await _service.StartAsync(CancellationToken.None);

            // Assert
            A.CallTo(() => _supplierAddressRepository.AddOrUpdate(A<SupplierAddressModel>._, A<string>._))
                .MustHaveHappened();

            _logger.VerifyLogMustHaveHappened(LogLevel.Error, exception.Message);
        }
             
    }
}
