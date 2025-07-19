using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Infrastructure;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.RateLimiting;

namespace SFA.DAS.TeachInFurtherEducation.Web.Controllers
{
    [Route("error")]
    public class ErrorController : Controller
    {
        private const string __SYSTEMERRORVIEW = "ApplicationError";
        private const string __PAGENOTFOUNDVIEW = "PageNotFound";

        private readonly ILogger<ErrorController> _log;

        private static readonly LayoutModel LayoutModel = new LayoutModel();

        private readonly IContentService _contentService;

        public ErrorController(ILogger<ErrorController> logger, IContentService contentService)
        {

            _log = logger;

            _contentService = contentService;

        }

        [Route("{statusCode}")]
        public IActionResult HandleError(int? statusCode)
        {
            try
            {
                LayoutModel.footerLinks = _contentService.Content.FooterLinks;
                LayoutModel.MenuItems = _contentService.Content.MenuItems;

                if (!ModelState.IsValid)
                {
                    return View(__SYSTEMERRORVIEW, LayoutModel);
                }

                if (statusCode.HasValue)
                {
                    switch (statusCode.Value)
                    {
                        case 404:
                            return View(__PAGENOTFOUNDVIEW, LayoutModel);
                        case 500:
                            return View(__SYSTEMERRORVIEW, LayoutModel);
                    }
                }
                return View(__SYSTEMERRORVIEW, LayoutModel);
            }
            catch (Exception _exception)
            {
                _log.LogError(_exception, "Unable to get model with populated footer");

                return View(__SYSTEMERRORVIEW, LayoutModel);
            }
        }
        
        [Route("/Rate-Limit-Exceeded")]
        public IActionResult RateLimitExceeded()
        {
            Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            return View("RateLimitExceeded");
        }
    }
}
