using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Web.Logging;

namespace SFA.DAS.TeachInFurtherEducation.Web.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNLog(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var nLogConfiguration = new NLogConfiguration();

            serviceCollection.AddLogging(options => //NOSONAR logging configuration is safe
            {
                options.AddFilter(typeof(Startup).Namespace, LogLevel.Information);
                options.SetMinimumLevel(LogLevel.Trace);
                options.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
                options.AddConsole(); //NOSONAR logging configuration is safe

                nLogConfiguration.ConfigureNLog(configuration["NLog:LogLevel"]!);
            });

            return serviceCollection;
        }
    }
}
