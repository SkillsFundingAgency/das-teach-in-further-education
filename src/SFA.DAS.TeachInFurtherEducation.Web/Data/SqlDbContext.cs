using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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

        public DbSet<SupplierAddressModel> SupplierAddresses { get; set; }

        public SqlDbContext(IOptions<SqlDbContextConfiguration> configuration,
                            DbContextOptions<SqlDbContext> options,
                            IWebHostEnvironment currentEnvironment,
                            ChainedTokenCredential azureServiceTokenProvider) : base(options)
        {
            this._configuration = configuration.Value;
            this._azureServiceTokenProvider = azureServiceTokenProvider;
            this._currentEnvironment = currentEnvironment;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_configuration == null
                || _azureServiceTokenProvider == null
                || _currentEnvironment.IsDevelopment())
            {
                return;
            }

            var connection = new SqlConnection(_configuration.ConnectionString);
            connection.AccessToken = _azureServiceTokenProvider.GetTokenAsync(new TokenRequestContext(new string[] { AzureResource })).GetAwaiter().GetResult().Token;

            optionsBuilder.UseSqlServer(connection, options =>
                options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(20), null)
            );
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
