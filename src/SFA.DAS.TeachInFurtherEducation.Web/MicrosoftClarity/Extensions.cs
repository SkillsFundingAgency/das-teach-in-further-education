using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.MicrosoftClarity
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static bool MicrosoftClarityIsEnabled(this ViewDataDictionary viewData)
            => !string.IsNullOrWhiteSpace(GetConfiguration(viewData)?.MsClarityId);

        public static string? GetMicrosoftClarityId(this ViewDataDictionary viewData)
            => GetConfiguration(viewData)?.MsClarityId;

        private static MicrosoftClarityConfiguration? GetConfiguration(ViewDataDictionary viewData)
            => viewData.TryGetValue(ViewDataKeys.MicrosoftClarityConfigurationKey, out var section)
                ? section as MicrosoftClarityConfiguration
                : null;

        public static IServiceCollection EnableMicrosoftClarity(this IServiceCollection services, MicrosoftClarityConfiguration microsoftClaritykConfiguration)
        {
            services.Configure<MvcOptions>(options =>
                options.Filters.Add(new EnableMicrosoftClarityAttribute(microsoftClaritykConfiguration)));
            return services;
        }
    }
}
