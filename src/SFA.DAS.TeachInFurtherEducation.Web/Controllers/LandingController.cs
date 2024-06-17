using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Landing(string pageURL = "/home")
        {

            PageContentModel? landingModel = _contentModelService.GetPageContentModel(pageURL);

            return View(nameof(Landing), landingModel);

        }

        public async Task<IActionResult> LandingPreview(string pageURL = "/home")
        {

            PageContentModel? landingModel = await _contentModelService.GetPagePreviewModel(pageURL);

            return View(nameof(Landing), landingModel);

        }

    }

}
