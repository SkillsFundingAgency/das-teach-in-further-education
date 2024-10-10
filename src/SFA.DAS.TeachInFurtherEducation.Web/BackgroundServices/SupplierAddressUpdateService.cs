using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using Cronos;
using Microsoft.Extensions.Options;
using SFA.DAS.TeachInFurtherEducation.Contentful.Exceptions;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using Contentful.Core.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;

namespace SFA.DAS.TeachInFurtherEducation.Web.BackgroundServices
{
    /// <summary>
    /// Updates content on a schedule, given in a configurable cron expression.
    /// It uses a cron expression, as it's standard, well understood, supported by libraries and provides flexibility,
    /// but also because we want all instances of the app services to update as one,
    /// so the site isn't serving different versions of the content, depending on the instance that happens to service the request.
    /// </summary>
    public class SupplierAddressUpdateService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<SupplierAddressUpdateService> _logger;

        private Timer? _timer;
        private int _executionCount;
        private DateTime? _lastAssetPublishedDate;

        private readonly bool _enabled;
        private readonly CronExpression _cronExpression;

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public SupplierAddressUpdateService(
            IServiceScopeFactory serviceScopeFactory,
            IOptions<SupplierAddressUpdateServiceOptions> supplierAddressUpdateServiceOptions,
            ILogger<SupplierAddressUpdateService> logger, 
            DateTime? lastAssetPublishedDate = null)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;

            var options = supplierAddressUpdateServiceOptions.Value;

            _enabled = options.Enabled;

            // Obtain cron schedule from config or default
            var cronSchedule = options.CronSchedule ?? "* * * * *";
            _cronExpression = CronExpression.Parse(cronSchedule);

            _lastAssetPublishedDate = lastAssetPublishedDate;
        }

        [ExcludeFromCodeCoverage]
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Supplier Address Update Service running.");

            if (!_enabled)
            {
                _logger.Log(LogLevel.Information, "Online Supplier Address Updates disabled.");
                return;
            }

            await PerformSupplierAddressUpdate();

            var delay = TimeToNextInvocation(DateTime.UtcNow);

            _timer = new Timer(UpdateSupplierAddresses, null, delay, Timeout.InfiniteTimeSpan);
        }

        private async Task PerformSupplierAddressUpdate()
        {
            // Attempt to enter the semaphore
            if (!await _semaphore.WaitAsync(0))
            {
                _logger.LogInformation("Supplier address update is already running. Skipping this invocation.");
                return;
            }

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var supplierAddressRepository = scope.ServiceProvider.GetRequiredService<ISupplierAddressRepository>();
                    var supplierAddressService = scope.ServiceProvider.GetRequiredService<ISupplierAddressService>();

                    // Get the date and time the supplier address asset was last published in the CMS
                    var lastUpdated = await supplierAddressService.GetSupplierAddressAssetLastPublishedDate();

                    if (_lastAssetPublishedDate == null || _lastAssetPublishedDate != lastUpdated)
                    {

                        // Obtain the list of supplier addresses from the asset in Contentful, along with when it was last published.
                        var supplierAddresses = await supplierAddressService.GetSourceSupplierAddresses();

                        // Check if the addresses list is null or empty
                        if (supplierAddresses == null || !supplierAddresses.Any())
                        {
                            _logger.LogWarning("No supplier addresses were returned from the service.");
                            return;
                        }

                        // Proceed with the update logic using the retrieved supplier addresses
                        _logger.LogInformation($"{supplierAddresses.Count} supplier addresses retrieved. Last updated at: {lastUpdated}");

                        var existingData = await supplierAddressRepository.GetAddressCountsGroupedByDate();

                        // Determine if there is data in the target database for the publication date of the asset, and how many rows exist
                        var matchingEntry = existingData.SingleOrDefault(entry => entry.Key.ToString() == lastUpdated.ToString());
                        var proceedWithUpdate = (matchingEntry.Equals(default(KeyValuePair<DateTime, int>)) || matchingEntry.Value != supplierAddresses.Count);

                        if (proceedWithUpdate)
                        {
                            var supplierAddressModels = await supplierAddressService.CreateSupplierAddresses(supplierAddresses, lastUpdated);

                            await UpdateAddresses(supplierAddressRepository, supplierAddressModels);
                            await DeleteOutdatedAddresses(supplierAddressRepository, lastUpdated);

                            _logger.LogInformation($"Supplier address data has been successfully updated with data published at {lastUpdated}");
                        }
                        else
                        {
                            _logger.LogInformation($"The supplier data retrieved matches the supplier data in the database. Skipping update.");
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Supplier addresses have not been published since the last update at: {_lastAssetPublishedDate}");
                    }

                    _lastAssetPublishedDate = lastUpdated;
                }
            }
            catch (FileNotFoundException ex)
            {
                // Handle specific case where asset was not found
                _logger.LogError(ex, "Supplier addresses asset not found.");
            }
            catch (InvalidOperationException ex)
            {
                // Handle specific case where there is a logic error in the response
                _logger.LogError(ex, "Invalid operation while retrieving supplier addresses.");
            }
            catch (Exception ex)
            {
                // Handle any other general exceptions
                _logger.LogError(ex, "An error occurred while retrieving supplier addresses.");
            }
            finally
            {
                _semaphore.Release();
                _logger.LogInformation("Supplier address update completed.");
            }
        }

        /// <summary>
        /// Updates the supplier addresses in Cosmos DB.
        /// </summary>
        /// <param name="supplierAddresses">The list of supplier addresses to update.</param>
        private async Task UpdateAddresses(ISupplierAddressRepository supplierAddressRepository, List<SupplierAddressModel> supplierAddresses)
        {
            var maxDegreeOfParallelism = 1;

            // Create a semaphore to limit the degree of parallelism
            using var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

            // Create a list to hold the upsert tasks
            var upsertTasks = supplierAddresses.Select(async address =>
            {
                // Wait until a slot is available
                await semaphore.WaitAsync();
                try
                {
                    // Perform the upsert
                    await supplierAddressRepository.AddOrUpdate(address, address.Postcode);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                finally
                {
                    // Release the semaphore slot when the task is done
                    semaphore.Release();
                }
            }).ToList();

            // Wait for all tasks to complete
            await Task.WhenAll(upsertTasks);
        }

        /// <summary>
        /// Marks outdated supplier addresses as inactive.
        /// </summary>
        /// <param name="latestDate">The latest date of the source asset.</param>
        private Task DeleteOutdatedAddresses(ISupplierAddressRepository supplierAddressRepository, DateTime latestDate)
        {
            return supplierAddressRepository.MarkOldAddressesAsInactive(latestDate);
        }

        // should be private, but public for easier testing
        public TimeSpan TimeToNextInvocation(DateTime utcNow)
        {
            DateTime? next = _cronExpression.GetNextOccurrence(utcNow);
            if (next == null)
                throw new ContentUpdateServiceException("Next invocation time is unreachable.");

            return next.Value - utcNow;
        }

        // event handler, so ok to use async void, as per sonar's/asyncfixer's warning descriptions (and also given the thumbs up by Stephen Clearly)
        // we also catch and consume all exceptions
#pragma warning disable S3168
#pragma warning disable AsyncFixer03
        [ExcludeFromCodeCoverage] // difficult to test
        private async void UpdateSupplierAddresses(object? state)
        {
            try
            {
                var count = Interlocked.Increment(ref _executionCount);

                _logger.LogInformation("Supplier Address Update Service is updating content. Count: {Count}", count);

                var delay = TimeToNextInvocation(DateTime.UtcNow);

                _timer!.Change(delay, Timeout.InfiniteTimeSpan);

                await PerformSupplierAddressUpdate();
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, "Update content failed!");
            }
            finally
            {
                Interlocked.Decrement(ref _executionCount);
            }
        }
#pragma warning restore AsyncFixer03
#pragma warning restore S3168

        [ExcludeFromCodeCoverage]
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Supplier Address Update is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [ExcludeFromCodeCoverage]
        protected virtual void Dispose(bool disposing)
        {
            _timer?.Dispose();
        }
    }
}
