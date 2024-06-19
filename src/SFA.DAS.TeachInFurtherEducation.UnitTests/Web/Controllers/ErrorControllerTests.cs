using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Web.Controllers;
using FakeItEasy;
using Xunit;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;

namespace SFA.DAS.FindEmploymentSchemes.UnitTests.Web.Controllers
{
    public class ErrorControllerTests
    {
        public ILogger<ErrorController> Logger { get; set; }
        private ErrorController ErrorController { get; set; }

        private IContentService IContentService { get; set; }

        public ErrorControllerTests()
        {
            Logger = A.Fake<ILogger<ErrorController>>();

            IContentService = A.Fake<IContentService>();

            ErrorController = new ErrorController(Logger, IContentService)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
        }

        [Fact]
        public void PageNotFound_UsesPageNotFoundViewTest()
        {
            var result = ErrorController.PageNotFound();

            var viewResult = Assert.IsType<ViewResult>(result);

            // ViewName == null means use Error/PageNotFound.cshtml, which is what we want
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ApplicationError_UsesApplicationErrorViewTest()
        {
            var result = ErrorController.ApplicationError();

            var viewResult = Assert.IsType<ViewResult>(result);

            // ViewName == null means use Error/ApplicationError.cshtml, which is what we want
            Assert.Null(viewResult.ViewName);
        }
        [Fact]
        public void ApplicationError_LogsErrorTest()
        {
            ErrorController.ApplicationError();

            A.CallTo(Logger)
                .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Error)
                .MustHaveHappened();
        }
    }
}