using AspNetCoreRateLimit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Security
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCOllectionExtensions
    {
        public static IServiceCollection AddRateLimiting(this IServiceCollection services)
        {
            // Create rate limit options
            var rateLimitOptions = new IpRateLimitOptions
            {
                EnableEndpointRateLimiting = true,
                StackBlockedRequests = false,
                RealIpHeader = "X-Real-IP",
                ClientIdHeader = "X-ClientId",
                HttpStatusCode = 429,
                GeneralRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*",
                    Period = "1m",
                    Limit = 100
                }
            }
            };

            // Create rate limit policies
            var rateLimitPolicies = new IpRateLimitPolicies
            {
                IpRules = new List<IpRateLimitPolicy>
            {
                new IpRateLimitPolicy
                {
                    Ip = "127.0.0.1",
                    Rules = new List<RateLimitRule>
                    {
                        new RateLimitRule
                        {
                            Endpoint = "*",
                            Period = "1m",
                            Limit = 1000
                        }
                    }
                }
            }
            };

            // Register rate limit options
            services.Configure<IpRateLimitOptions>(options =>
            {
                options.EnableEndpointRateLimiting = rateLimitOptions.EnableEndpointRateLimiting;
                options.StackBlockedRequests = rateLimitOptions.StackBlockedRequests;
                options.RealIpHeader = rateLimitOptions.RealIpHeader;
                options.ClientIdHeader = rateLimitOptions.ClientIdHeader;
                options.HttpStatusCode = rateLimitOptions.HttpStatusCode;
                options.GeneralRules = rateLimitOptions.GeneralRules;
            });

            // Register rate limit policies
            services.Configure<IpRateLimitPolicies>(options =>
            {
                options.IpRules = rateLimitPolicies.IpRules;
            });

            // Load IP rate limiting from memory
            services.AddMemoryCache();
            services.AddInMemoryRateLimiting();

            // Add rate limit configuration
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            return services;
        }
    }
}
