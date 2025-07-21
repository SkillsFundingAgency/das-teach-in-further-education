using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TeachInFurtherEducation.Web.Exceptions;
using SFA.DAS.TeachInFurtherEducation.Web.Infrastructure;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace SFA.DAS.TeachInFurtherEducation.Web.Controllers
{
    [EnableRateLimiting("fixed")]
    public class LandingController : Controller
    {
        private readonly IContentModelService _contentModelService;

        public LandingController(IContentModelService contentModelService)
        {
            _contentModelService = contentModelService;
        }

        public IActionResult Landing(string pageUrl = RouteNames.Home)
        {
            const string navigationSettingsCookieName = "NavigationSettings";

            if (pageUrl.ToLower().Trim() == RouteNames.LandingPage || pageUrl.ToLower().Trim() == RouteNames.Home)
            {
                var navigationPage = pageUrl.ToLower().Trim() == RouteNames.LandingPage
                    ? RouteNames.LandingPage
                    : RouteNames.Home;
                
                HttpContext.Response.Cookies.Append(navigationSettingsCookieName, navigationPage, new CookieOptions
                {
                    Path = "/", // Ensure the cookie is available site-wide
                    HttpOnly = true, // Accessible only by the server
                    IsEssential = true, // Required for GDPR compliance
                    Secure = true, // Ensure the cookie is sent only over HTTPS
                    SameSite = SameSiteMode.Strict // Prevent the cookie from being sent in cross-site requests
                });
            }

            var landingPageStr = HttpContext.Request.Cookies[navigationSettingsCookieName];
            var isLandingPage = !string.IsNullOrEmpty(landingPageStr) && landingPageStr.Trim() == RouteNames.LandingPage;
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