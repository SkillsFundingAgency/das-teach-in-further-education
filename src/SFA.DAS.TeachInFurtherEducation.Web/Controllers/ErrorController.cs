using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Infrastructure;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Controllers
{
    [Route("error")]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _log;

        private static readonly LayoutModel LayoutModel = new LayoutModel();

        private readonly IContentService _contentService;

        public ErrorController(ILogger<ErrorController> logger, IContentService contentService)
        {

            _log = logger;

            _contentService = contentService;

        }

        [Route("404", Name = RouteNames.Error404)]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult PageNotFound()
        {

            try
            {

                LayoutModel.footerLinks = _contentService.Content.FooterLinks;
                LayoutModel.MenuItems = _contentService.Content.MenuItems;

                return View(LayoutModel);

            }
            catch(Exception _exception)
            {

                _log.LogError(_exception, "Unable to get model with populated footer");

                return View(LayoutModel);

            }

        }

        [Route("500", Name = RouteNames.Error500)]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ApplicationError()
        {
            LayoutModel.footerLinks = _contentService.Content.FooterLinks;
            LayoutModel.MenuItems = _contentService.Content.MenuItems;

            IExceptionHandlerPathFeature? feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            _log.LogError($"500 result at {feature?.Path ?? "{unknown}"}", feature?.Error);
            return View(LayoutModel);
        }
    }
}
