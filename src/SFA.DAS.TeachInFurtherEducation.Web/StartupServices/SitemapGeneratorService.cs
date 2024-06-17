using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.TeachInFurtherEducation.Web.Infrastructure;

namespace SFA.DAS.TeachInFurtherEducation.Web.StartupServices
{
    //https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-3/
    public class SitemapGeneratorService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public SitemapGeneratorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            scope.ServiceProvider.GetRequiredService<ISitemap>().Generate();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
