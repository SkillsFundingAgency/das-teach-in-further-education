using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TeachInFurtherEducation.Web.Exceptions;
using SFA.DAS.TeachInFurtherEducation.Web.Infrastructure;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
            const string homeLinkIsLandingPage = "HomeLinkIsLandingPage";
            bool isLandingPage;
            if (pageUrl.ToLower().Trim() == RouteNames.LandingPage || pageUrl.ToLower().Trim() == RouteNames.Home)
            {
                isLandingPage = pageUrl.ToLower().Trim() == RouteNames.LandingPage;
                HttpContext.Session.SetString(homeLinkIsLandingPage, isLandingPage.ToString());
            }

            var sessionIsLandingPage = HttpContext.Session.GetString(homeLinkIsLandingPage);
            isLandingPage =  !string.IsNullOrEmpty(sessionIsLandingPage) && bool.Parse(sessionIsLandingPage) ;
            var pageModel = _contentModelService.GetPageContentModel(pageUrl, isLandingPage);
            
            
            
            if (!ModelState.IsValid || pageModel == null)
            {
                throw new PageNotFoundException($"The requested url {pageUrl} could not be found");
            }

            return View(nameof(Landing), pageModel);
        }

        public async Task<IActionResult> PagePreview(string pageUrl = RouteNames.Home)
        {

            
            PageContentModel? pageModel = await _contentModelService.GetPagePreviewModel(pageUrl, false);

            if (!ModelState.IsValid || pageModel == null)
            {
                throw new PageNotFoundException($"The requested url {pageUrl} could not be found");
            }

            return View(nameof(Landing), pageModel);

        }
    }
}
