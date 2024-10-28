using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TeachInFurtherEducation.Web.Exceptions;
using SFA.DAS.TeachInFurtherEducation.Web.Infrastructure;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Controllers
{

    public class LandingController : Controller
    {
        private readonly IContentModelService _contentModelService;

        public LandingController(IContentModelService contentModelService)
        {
            _contentModelService = contentModelService;
        }

        //[ResponseCache(Duration = 60 * 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult Landing(string pageUrl = RouteNames.Home)
        {
            var pageModel = _contentModelService.GetPageContentModel(pageUrl);

            if (!ModelState.IsValid || pageModel == null)
            {
                throw new PageNotFoundException($"The requested url {pageUrl} could not be found");
            }

            return View(nameof(Landing), pageModel);
        }

        public async Task<IActionResult> PagePreview(string pageUrl = RouteNames.Home)
        {

            
            PageContentModel? pageModel = await _contentModelService.GetPagePreviewModel(pageUrl);

            if (!ModelState.IsValid || pageModel == null)
            {
                throw new PageNotFoundException($"The requested url {pageUrl} could not be found");
            }

            return View(nameof(Landing), pageModel);

        }
    }
}
