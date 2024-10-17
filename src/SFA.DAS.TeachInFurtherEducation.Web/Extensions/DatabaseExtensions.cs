using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.TeachInFurtherEducation.Web.Data;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using System;

namespace SFA.DAS.TeachInFurtherEducation.Web.Extensions
{
    public static class DatabaseExtensions
    {
        public static void AddDatabaseRegistration(this IServiceCollection services, IConfiguration configuration, string? environmentName)
        {
            // Setup DB Configuration
            services.Configure<SqlDbContextConfiguration>(configuration.GetSection("SqlDB"));

            // Define DB Context
            services.AddDbContext<SqlDbContext>(ServiceLifetime.Transient);

            //services.AddScoped(provider => new Lazy<SqlDbContext>(provider.GetService<EducationalOrganisationDataContext>()!));
            services.AddSingleton(new ChainedTokenCredential(
                new ManagedIdentityCredential(),
                new AzureCliCredential(),
                new VisualStudioCodeCredential(),
                new VisualStudioCredential())
            );

        }
    }
}