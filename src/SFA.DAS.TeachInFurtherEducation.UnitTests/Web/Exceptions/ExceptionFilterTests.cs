using Xunit;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Web;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Exceptions;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using Microsoft.AspNetCore.Http;
using SFA.DAS.TeachInFurtherEducation.Web.Controllers;
using System.Collections.Generic;
using System;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Tests
{
    public class ExceptionFilterTests
    {
        private readonly ILogger<ExceptionFilter> _logger;
        private readonly IContentService _contentService;
        private readonly ExceptionFilter _exceptionFilter;
        private readonly ExceptionContext _context;

        public ExceptionFilterTests()
        {
            _logger = A.Fake<ILogger<ExceptionFilter>>();
            _contentService = A.Fake<IContentService>();

            // Mock content for LayoutModel
            A.CallTo(() => _contentService.Content.FooterLinks).Returns(new List<FooterLink>());
            A.CallTo(() => _contentService.Content.MenuItems).Returns(new List<MenuItem>());

            // Create the exception filter
            _exceptionFilter = new ExceptionFilter(_logger, _contentService);

            // Set up the context with an exception
            var httpContext = new DefaultHttpContext();
            _context = new ExceptionContext(
                new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()),
                new List<IFilterMetadata>()
            );
        }

        [Fact]
        public void OnException_WhenGeneralExceptionOccurs_LogsErrorAndSets500StatusCode()
        {
            // Arrange
            _context.Exception = new Exception("General error occurred");

            // Act
            _exceptionFilter.OnException(_context);

            // Assert: Log the error
            _logger.VerifyLogMustHaveHappened(
                LogLevel.Error,
                "An unhandled exception occurred."
            );

            // Assert: Check if status code is 500
            Assert.Equal(500, _context.HttpContext.Response.StatusCode);

            // Assert: View result is ApplicationError
            var result = Assert.IsType<ViewResult>(_context.Result);
            Assert.Equal("~/Views/Error/ApplicationError.cshtml", result.ViewName);

            // Assert: ViewData contains status code and error message
            Assert.Equal(500, result.ViewData["StatusCode"]);
            Assert.Equal("General error occurred", result.ViewData["ErrorMessage"]);
        }

        [Fact]
        public void OnException_WhenPageNotFoundExceptionOccurs_LogsErrorAndSets404StatusCode()
        {
            // Arrange
            _context.Exception = new PageNotFoundException("Page not found");

            // Act
            _exceptionFilter.OnException(_context);

            // Assert: Log the error
            _logger.VerifyLogMustHaveHappened(
                LogLevel.Error,
                "An unhandled exception occurred."
            );

            // Assert: Check if status code is 404
            Assert.Equal(404, _context.HttpContext.Response.StatusCode);

            // Assert: View result is PageNotFound
            var result = Assert.IsType<ViewResult>(_context.Result);
            Assert.Equal("~/Views/Error/PageNotFound.cshtml", result.ViewName);

            // Assert: ViewData contains status code and error message
            Assert.Equal(404, result.ViewData["StatusCode"]);
            Assert.Equal("Page not found", result.ViewData["ErrorMessage"]);
        }

        [Fact]
        public void OnException_SetsLayoutModelInViewData()
        {
            // Arrange
            _context.Exception = new Exception("General error occurred");

            // Act
            _exceptionFilter.OnException(_context);

            // Assert: Check that LayoutModel is populated correctly
            var result = Assert.IsType<ViewResult>(_context.Result);
            var layoutModel = Assert.IsType<LayoutModel>(result.ViewData.Model);
            Assert.NotNull(layoutModel);
            Assert.NotNull(layoutModel.footerLinks);
            Assert.NotNull(layoutModel.MenuItems);
        }
    }
}
