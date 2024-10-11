using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Data
{
    [ExcludeFromCodeCoverage]
    public class SqlDBContext : DbContext
    {
        private const string AzureResource = "https://database.windows.net/";

        private readonly ChainedTokenCredential _azureServiceTokenProvider;
        private readonly SqlDbContextConfiguration _configuration;

        public DbSet<SupplierAddressModel> SupplierAddresses { get; set; }

        public SqlDBContext(IOptions<SqlDbContextConfiguration> configuration, DbContextOptions<SqlDBContext> options, ChainedTokenCredential azureServiceTokenProvider): base(options)
        {
            this._configuration = configuration.Value;
            this._azureServiceTokenProvider = azureServiceTokenProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SupplierAddressModel>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Map the Location property to SQL Server GEOGRAPHY type using NetTopologySuite
                entity.Property(e => e.Location)
                    .HasColumnType("geography");   // Ensure Location is stored as a GEOGRAPHY type in SQL Server
            });
        }
    }
}
