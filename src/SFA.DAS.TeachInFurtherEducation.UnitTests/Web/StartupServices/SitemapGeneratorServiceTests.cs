using SFA.DAS.TeachInFurtherEducation.Web.StartupServices;
using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.TeachInFurtherEducation.Web.Infrastructure;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.StartupServices
{
    public class SitemapGeneratorServiceTests
    {
        [Fact]
        public async Task StartAsyncTest()
        {
            // really, we're too deep in .net core internals here

            var serviceProvider = A.Fake<IServiceProvider>(o => o.Implements<ISupportRequiredService>());
            var sitemap = A.Fake<ISitemap>();
            A.CallTo(() => ((ISupportRequiredService)serviceProvider).GetRequiredService(A<Type>.That.Matches(t => t.Name == nameof(ISitemap))))
                .Returns(sitemap);

            var serviceScopeFactory = A.Fake<IServiceScopeFactory>();
            A.CallTo(() => ((ISupportRequiredService)serviceProvider).GetRequiredService(A<Type>.That.Matches(t => t.Name == nameof(IServiceScopeFactory))))
                .Returns(serviceScopeFactory);

            var serviceScope = A.Fake<IServiceScope>();
            A.CallTo(() => serviceScopeFactory.CreateScope())
                .Returns(serviceScope);

            // reuse the global service provider for the scoped version
            A.CallTo(() => serviceScope.ServiceProvider)
                .Returns(serviceProvider);

            var sitemapGeneratorService = new SitemapGeneratorService(serviceProvider);

            await sitemapGeneratorService.StartAsync(CancellationToken.None);

            A.CallTo(() => sitemap.Generate())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task StopAsync_ShouldCompleteSuccessfully()
        {
            // Arrange
            var serviceProvider = A.Fake<IServiceProvider>();
            var sitemapGeneratorService = new SitemapGeneratorService(serviceProvider);

            // Act
            await sitemapGeneratorService.StopAsync(CancellationToken.None);
        }
    }
}
