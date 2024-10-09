using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Exceptions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.TeachInFurtherEducation.Web.Controllers;
using SFA.DAS.TeachInFurtherEducation.Web.Models;

namespace SFA.DAS.TeachInFurtherEducation.Web
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        private static readonly LayoutModel LayoutModel = new LayoutModel();

        private readonly IContentService _contentService;

        public ExceptionFilter(ILogger<ExceptionFilter> logger, IContentService contentService)
        {
            _logger = logger;
            _contentService = contentService;
        }

        public void OnException(ExceptionContext context)
        {
            // Log the exception details for debugging purposes
            _logger.LogError(context.Exception, "An unhandled exception occurred.");

            LayoutModel.footerLinks = _contentService.Content.FooterLinks;
            LayoutModel.MenuItems = _contentService.Content.MenuItems;

            var statusCode = 500;
            var viewName = "~/Views/Error/ApplicationError.cshtml";

            if (context.Exception is PageNotFoundException)
            {
                statusCode = 404;
                viewName = "~/Views/Error/PageNotFound.cshtml";
            }

            context.HttpContext.Response.StatusCode = statusCode;

            var viewData = new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                context.ModelState)
                {
                    Model = LayoutModel 
                };

            // Add exception details to ViewData
            viewData["StatusCode"] = statusCode;
            viewData["ErrorMessage"] = context.Exception.Message;

            context.Result = new ViewResult
            {
                ViewName = viewName,
                ViewData = viewData
            };

            context.ExceptionHandled = true;
        }
    }
}
