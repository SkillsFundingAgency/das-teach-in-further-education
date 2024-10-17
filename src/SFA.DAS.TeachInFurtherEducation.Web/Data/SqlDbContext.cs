using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Data
{
    [ExcludeFromCodeCoverage]
    public class SqlDbContext : DbContext
    {
        private const string AzureResource = "https://database.windows.net/";

        private readonly ChainedTokenCredential _azureServiceTokenProvider;
        private readonly SqlDbContextConfiguration _configuration;
        private readonly IWebHostEnvironment _currentEnvironment;
        private readonly ILogger<SqlDbContext> _logger;

        public DbSet<SupplierAddressModel> SupplierAddresses { get; set; }

        public SqlDbContext(IOptions<SqlDbContextConfiguration> configuration,
                            DbContextOptions<SqlDbContext> options,
                            IWebHostEnvironment currentEnvironment,
                            ChainedTokenCredential azureServiceTokenProvider, 
                            ILogger<SqlDbContext> logger) : base(options)
        {
            this._configuration = configuration.Value;
            this._azureServiceTokenProvider = azureServiceTokenProvider;
            this._currentEnvironment = currentEnvironment;
            this._logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_configuration != null && !string.IsNullOrEmpty(_configuration.ConnectionString))
            {
                var connection = new SqlConnection(_configuration.ConnectionString);

                if (!_currentEnvironment.IsDevelopment())
                {
                    if (_azureServiceTokenProvider != null)
                    {
                        _logger.LogInformation($"Using connection string {_configuration.ConnectionString} with Integrated Authentication");
                        connection.AccessToken = _azureServiceTokenProvider.GetTokenAsync(new TokenRequestContext(new string[] { AzureResource })).GetAwaiter().GetResult().Token;
                    }
                    else
                    {
                        _logger.LogInformation($"Using connection string {_configuration.ConnectionString}");
                    }
                }

                optionsBuilder.UseSqlServer(connection, options =>
                {
                    options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(20), null);
                    options.UseNetTopologySuite();
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SupplierAddressModel>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Location)
                    .HasColumnType("geography");   
            });
        }
    }
}
