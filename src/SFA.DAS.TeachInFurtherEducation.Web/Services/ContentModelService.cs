using Contentful.Core.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Logging;
using NUglify.Helpers;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Roots.Base;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using SFA.DAS.TeachInFurtherEducation.Web.Infrastructure;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services;

public class ContentModelService(
    ILogger<ContentModelService> logger,
    IContentService contentService,
    HtmlRenderer htmlRenderer)
    : ContentRootService(htmlRenderer, logger), IContentModelService
{
    public PageContentModel? LandingModel { get; set; }

    /// <summary>
    /// Retrieves the landing model containing data from various content sources.
    /// </summary>
    /// <returns>Returns the model if available, otherwise returns null.</returns>
    public PageContentModel? GetPageContentModel(string pageUrl)
    {
        try
        {
            var page = contentService.GetPageByURL(pageUrl);
            var menus = contentService.Content.MenuItems;
            var menuItems = GetMenuItems(ref menus, pageUrl);
            if (page == null)
            {
                return null;
            }

            return new PageContentModel()
            {
                PageURL = page.PageURL,
                PageTitle = page.PageTitle,
                PageTemplate = page.PageTemplate,
                Breadcrumbs = page.Breadcrumbs,
                MenuItems = menuItems,
                footerLinks = contentService.Content.FooterLinks,
                PageComponents = page.PageComponents,
            };
        }
        catch (Exception exception)
        {
            logger.LogError(exception, " - Unable to get a page.");

            return null;
        }
    }


    /// <summary>
    /// Retrieves the landing model containing data from various content sources in preview mode.
    /// </summary>
    /// <returns>Returns the model if available, otherwise returns null.</returns>
    public async Task<PageContentModel?> GetPagePreviewModel(string pageUrl)
    {
        try
        {
            var previewContent = await contentService.UpdatePreview();
            var previewPage = contentService.GetPreviewPageByURL(pageUrl);
            var menus = previewContent.MenuItems;
            var menuItems = GetMenuItems(ref menus, pageUrl);

            if (previewPage == null)
            {
                return null;
            }

            return new PageContentModel()
            {
                PageURL = previewPage.PageURL,
                PageTitle = previewPage.PageTitle,
                PageTemplate = previewPage.PageTemplate,
                Breadcrumbs = previewPage.Breadcrumbs,
                PageComponents = previewPage.PageComponents,
                MenuItems = menuItems,
                footerLinks = previewContent.FooterLinks,
                Preview = new PreviewModel(Enumerable.Empty<HtmlString>())
            };
        }
        catch (Exception exception)
        {
            logger.LogError(exception, " - Unable to get interim preview landing page.");
            return null;
        }
    }

    public IEnumerable<MenuItem> GetMenuItems(ref IEnumerable<MenuItem> menuItems, string pageUrl)
    {
        menuItems.ForEach(x => x.IsCurrentPage = false);
        var pagePath = pageUrl == RouteNames.Home ? "/" : $"/{pageUrl}";
        var currentMenu = menuItems.FirstOrDefault(x => x.MenuItemSource == pagePath);
        if (currentMenu != null)
        {
            currentMenu.IsCurrentPage = true;
        }
        return menuItems;
    }
}