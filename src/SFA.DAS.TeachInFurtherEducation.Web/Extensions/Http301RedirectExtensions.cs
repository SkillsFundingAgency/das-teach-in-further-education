using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.TeachInFurtherEducation.Web.Data;
using SFA.DAS.TeachInFurtherEducation.Web.Data.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class Http301RedirectExtensions
    {
        public static void Add301Redirection(this IServiceCollection services, IConfiguration configuration, string? environmentName)
        {
            var redirectConfigSection = configuration.GetSection("301Redirection");
            var redirectConfig = redirectConfigSection.Get<Http301RedirectConfig>() ?? new Http301RedirectConfig();

            services.AddSingleton(redirectConfig);
        }

        public static void Use301Redirection(this IApplicationBuilder app)
        {
            app.UseMiddleware<Http301RedirectionMiddleware>();
        }
    }
}