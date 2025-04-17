using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.TeachInFurtherEducation.Web.Configuration;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.StartupServices
{
    [ExcludeFromCodeCoverage]
    public static class DataProtectionStartupExtensions
    {
        public static IServiceCollection AddDataProtection(this IServiceCollection services, ApplicationConfiguration config)
        {
            if (string.IsNullOrEmpty(config.DataProtectionKeysDatabase)
                || string.IsNullOrEmpty(config.RedisConnectionString)) return services;

            var redisConnectionString = config.RedisConnectionString;
            var dataProtectionKeysDatabase = config.DataProtectionKeysDatabase;

            var configurationOptions = ConfigurationOptions.Parse($"{redisConnectionString},{dataProtectionKeysDatabase}");
            var redis = ConnectionMultiplexer
                .Connect(configurationOptions);

            services.AddDataProtection()
                .SetApplicationName("das-teach-in-further-education")
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

            return services;
        }
    }
}
