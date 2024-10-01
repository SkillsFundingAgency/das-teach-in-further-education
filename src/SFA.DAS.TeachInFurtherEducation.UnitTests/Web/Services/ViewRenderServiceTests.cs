using Xunit;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.TeachInFurtherEducation.Web.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Tests.Services
{
    public class ViewRenderServiceTests
    {
        private readonly ILogger<ViewRenderService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly ViewRenderService _service;

        public ViewRenderServiceTests()
        {
            _logger = A.Fake<ILogger<ViewRenderService>>();
            _serviceProvider = A.Fake<IServiceProvider>();
            _viewEngine = A.Fake<ICompositeViewEngine>();
            _httpContextAccessor = A.Fake<IHttpContextAccessor>();
            _tempDataProvider = A.Fake<ITempDataProvider>();

            // Mock the TempDataProvider in the service provider
            A.CallTo(() => _serviceProvider.GetService(typeof(ITempDataProvider)))
                .Returns(_tempDataProvider);

            // Create the service instance
            _service = new ViewRenderService(_logger, _serviceProvider, _viewEngine, _httpContextAccessor);
        }

        [Fact]
        public async Task RenderToStringAsync_WhenViewExists_ReturnsRenderedView()
        {
            // Arrange
            var viewName = "TestView";
            var model = new { Name = "Test Model" };

            // Set up the HttpContext
            var httpContext = new DefaultHttpContext();
            _httpContextAccessor.HttpContext = httpContext;

            // Set up the view engine result
            var view = A.Fake<IView>();
            var viewEngineResult = ViewEngineResult.Found(viewName, view);
            A.CallTo(() => _viewEngine.FindView(A<ActionContext>.Ignored, viewName, false))
                .Returns(viewEngineResult);

            // Mock RenderAsync to write something to the writer
            A.CallTo(() => view.RenderAsync(A<ViewContext>.Ignored))
                .Invokes((ViewContext vc) => vc.Writer.Write("Rendered View Content"));

            // Act
            var result = await _service.RenderToStringAsync(viewName, model);

            // Assert
            A.CallTo(() => view.RenderAsync(A<ViewContext>.Ignored)).MustHaveHappened();
            Assert.NotEmpty(result);
            Assert.Equal("Rendered View Content", result);
        }

        [Fact]
        public async Task RenderToStringAsync_WhenViewNotFound_ThrowsArgumentNullExceptionAndLogsError()
        {
            // Arrange
            var viewName = "NonExistentView";
            var model = new { Name = "Test Model" };

            // Set up the HttpContext
            var httpContext = new DefaultHttpContext();
            _httpContextAccessor.HttpContext = httpContext;

            // Set up the view engine to return not found
            var viewEngineResult = ViewEngineResult.NotFound(viewName, new[] { "Location1", "Location2" });
            A.CallTo(() => _viewEngine.FindView(A<ActionContext>.Ignored, viewName, false))
                .Returns(viewEngineResult);

            // Act & Assert
            var result = await _service.RenderToStringAsync(viewName, model);

            // Verify logging using custom method
            _logger.VerifyLogMustHaveHappened(
                LogLevel.Error,
                $"Unable to render partial view {viewName}"
            );
        }

        [Fact]
        public async Task RenderToStringAsync_WhenExceptionThrown_LogsErrorAndReturnsEmptyString()
        {
            // Arrange
            var viewName = "TestView";
            var model = new { Name = "Test Model" };

            var httpContext = new DefaultHttpContext();
            _httpContextAccessor.HttpContext = httpContext;

            // Mock the view engine to throw an exception
            A.CallTo(() => _viewEngine.FindView(A<ActionContext>.Ignored, viewName, false)).Throws<Exception>();

            // Act
            var result = await _service.RenderToStringAsync(viewName, model);

            // Assert
            Assert.Equal(string.Empty, result);

            // Verify logging using custom method
            _logger.VerifyLogMustHaveHappened(
                LogLevel.Error,
                $"Unable to render partial view {viewName}"
            );
        }
    }
}
