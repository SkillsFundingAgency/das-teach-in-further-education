using Xunit;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TeachInFurtherEducation.Web.Controllers;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System;
using System.Collections.Generic;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;


namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Controllers
{
    public class ErrorControllerTests
    {
        private readonly ILogger<ErrorController> _logger;
        private readonly IContentService _contentService;
        private readonly ErrorController _controller;

        public ErrorControllerTests()
        {
            _logger = A.Fake<ILogger<ErrorController>>();
            _contentService = A.Fake<IContentService>();

            // Mock content for LayoutModel
            A.CallTo(() => _contentService.Content.FooterLinks).Returns(new List<FooterLink>());
            A.CallTo(() => _contentService.Content.MenuItems).Returns(new List<MenuItem>());

            // Create the controller instance
            _controller = new ErrorController(_logger, _contentService);
        }

        [Fact]
        public void HandleError_WhenStatusCodeIs404_ReturnsPageNotFoundView()
        {
            // Arrange
            int statusCode = 404;

            // Act
            var result = _controller.HandleError(statusCode) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("PageNotFound", result.ViewName);
            Assert.IsType<LayoutModel>(result.Model);

            // Verify that the layout model is populated
            var layoutModel = result.Model as LayoutModel;
            Assert.NotNull(layoutModel);
            Assert.NotNull(layoutModel.footerLinks);
         
            Assert.NotNull(layoutModel.MenuItems);
        }

        [Fact]
        public void HandleError_WhenUnhandledStatusCodeIsProvided_ReturnsApplicationErrorView()
        {
            // Arrange
            int statusCode = 418; // Status code that does not have a specific case

            // Act
            var result = _controller.HandleError(statusCode) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ApplicationError", result.ViewName);
            Assert.IsType<LayoutModel>(result.Model);
        }

        [Fact]
        public void HandleError_WhenModelStateIsInvalid_ReturnsSystemErrorView()
        {
            // Arrange
            // Simulate invalid ModelState
            _controller.ModelState.AddModelError("key", "error message");
            int statusCode = 500; // Status code is not important in this case

            // Act
            var result = _controller.HandleError(statusCode) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ApplicationError", result.ViewName);
            Assert.IsType<LayoutModel>(result.Model);

            // Verify that the layout model is populated
            var layoutModel = result.Model as LayoutModel;
            Assert.NotNull(layoutModel);
            Assert.NotNull(layoutModel.footerLinks);
            Assert.NotNull(layoutModel.MenuItems);
        }


        [Fact]
        public void HandleError_WhenStatusCodeIs500_ReturnsApplicationErrorView()
        {
            // Arrange
            int statusCode = 500;

            // Act
            var result = _controller.HandleError(statusCode) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ApplicationError", result.ViewName);
            Assert.IsType<LayoutModel>(result.Model);

            // Verify that the layout model is populated
            var layoutModel = result.Model as LayoutModel;
            Assert.NotNull(layoutModel);
            Assert.NotNull(layoutModel.footerLinks);
            Assert.NotNull(layoutModel.MenuItems);
        }

        [Fact]
        public void HandleError_WhenNoStatusCodeIsProvided_ReturnsApplicationErrorView()
        {
            // Act
            var result = _controller.HandleError(null) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ApplicationError", result.ViewName);
            Assert.IsType<LayoutModel>(result.Model);

            // Verify that the layout model is populated
            var layoutModel = result.Model as LayoutModel;
            Assert.NotNull(layoutModel);
            Assert.NotNull(layoutModel.footerLinks);
            Assert.NotNull(layoutModel.MenuItems);
        }

        [Fact]
        public void HandleError_WhenExceptionIsThrown_LogsErrorAndReturnsApplicationErrorView()
        {
            // Arrange
            A.CallTo(() => _contentService.Content.FooterLinks).Throws(new Exception("Service failure"));

            // Act
            var result = _controller.HandleError(500) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ApplicationError", result.ViewName);

            // Verify that the error was logged
            _logger.VerifyLogMustHaveHappened(
                LogLevel.Error,
                "Unable to get model with populated footer"
            );
        }
        
       
        [Fact]
        public void RateLimitExceeded_SetsStatusCodeTo429()
        {
            // Arrange
            SetupControllerContext();  // Required for accessing HttpContext

            // Act
            var result = _controller.RateLimitExceeded();

            // Assert
            Assert.Equal(StatusCodes.Status429TooManyRequests, _controller.HttpContext.Response.StatusCode);
        }

        [Fact]
        public void RateLimitExceeded_ReturnsCorrectView()
        {
            // Arrange
            SetupControllerContext();  // Required for HttpContext

            // Act
            var result = _controller.RateLimitExceeded() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("RateLimitExceeded", result.ViewName);
        }

        [Fact]
        public void RateLimitExceeded_DoesNotUseDependencies()
        {
            // Arrange
            SetupControllerContext();

            // Act
            var result = _controller.RateLimitExceeded();

            // Assert: Verify dependencies weren't accessed
            A.CallTo(() => _contentService.Content.FooterLinks).MustNotHaveHappened();
            A.CallTo(() => _contentService.Content.MenuItems).MustNotHaveHappened();
        }

        private void SetupControllerContext()
        {
            // Create mock HTTP context
            var httpContext = new DefaultHttpContext();
            
            // Create controller context
            var controllerContext = new ControllerContext(
                new ActionContext(
                    httpContext,
                    new RouteData(),
                    new ControllerActionDescriptor()
                )
            );
            
            _controller.ControllerContext = controllerContext;
        }
    }
}