using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;

// TODO - Example Controller Usage
public class LayoutController : Controller
{
    private readonly IContentNavigationService _navigationService;

    public LayoutController(IContentNavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    [ChildActionOnly]
    public async Task<ActionResult> DropdownMenu()
    {
        var model = await _navigationService.GetMainNavigationAsync();
        return PartialView("LayoutDropdownMenu", model);
    }
}