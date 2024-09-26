using Microsoft.EntityFrameworkCore;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Data
{
    [ExcludeFromCodeCoverage]
    public class SqlDbContext : DbContext
    {
        public DbSet<SupplierAddressModel> SupplierAddresses { get; set; }

        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
        {
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
