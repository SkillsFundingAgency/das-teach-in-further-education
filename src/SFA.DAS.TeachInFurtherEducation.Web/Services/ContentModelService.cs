using Contentful.Core.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Roots.Base;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services
{

    public class ContentModelService : ContentRootService, IContentModelService
    {

        #region Properties

        private readonly ILogger<ContentModelService> _logger;

        private readonly IContentService _contentService;

        public PageContentModel? LandingModel { get; set; }

        #endregion

        #region Constructors

        public ContentModelService(ILogger<ContentModelService> logger, IContentService contentService, HtmlRenderer htmlRenderer) :base(htmlRenderer)
        {

            _logger = logger;

            _contentService = contentService;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the landing model containing data from various content sources.
        /// </summary>
        /// <returns>Returns the landing model if available, otherwise returns null.</returns>
        public PageContentModel? GetPageContentModel(string pageURL)
        {
            try
            {
                Page? page = _contentService.GetPageByURL(pageURL);


                if (page == null)
                {

                    return null;

                }

                return new PageContentModel()
                {

                    PageURL = page.PageURL,

                    PageTitle = page.PageTitle,

                    PageTemplate = page.PageTemplate,

                    PagePreamble = page.PagePreamble,

                    TileSections = page.TileSections,

                    MenuItems = _contentService.Content.MenuItems,

                    footerLinks = _contentService.Content.FooterLinks,

                    Contents = page.Contents,

                    ContentBoxSection = page.ContentBoxSection,

                    ImageCardBanner = page.ImageCardBanner,

                    NewsLetter = page.NewsLetter,

                    ContactUs = page.ContactUs,
                };

            }
            catch(Exception _exception)
            {

                _logger.LogError(_exception, "Unable to get a page.");

                return null;

            }

        }

        #region Preview Models

        //public async Task<InterimPageModel?> GetPagePreviewModel(string interimURL)
        //{

        //    try
        //    {

        //        IContent previewContent = await _contentService.UpdatePreview();

        //        InterimPage? interimPage = _contentService.GetPreviewPageByURL(interimURL);
                    
        //        if (interimPage == null)
        //        {

        //            return null;

        //        }

        //        return new InterimPageModel()
        //        {

        //            InterimPageTitle = interimPage.InterimPageTitle,

        //            InterimPageURL = interimPage.InterimPageURL,

        //            InterimPagePreamble = interimPage.InterimPagePreamble,

        //            InterimPageComponents = interimPage.InterimPageComponents,

        //            InterimPageTileSections = interimPage.InterimPageTileSections,

        //            InterimPageBreadcrumbs = interimPage.InterimPageBreadcrumbs,

        //            MenuItems = previewContent.MenuItems,

        //            BetaBanner = previewContent.BetaBanner,

        //            InterimFooterLinks = previewContent.FooterLinks,

        //            Preview = new PreviewModel(Enumerable.Empty<HtmlString>())

        //        };

        //    }
        //    catch (Exception _exception)
        //    {

        //        _logger.LogError(_exception, "Unable to get interim page.");

        //        return null;

        //    }

        //}

        public async Task<PageContentModel?> GetPagePreviewModel(string pageURL)
        {

            try
            {

                Contentful.Model.Content.Interfaces.IContent previewContent = await _contentService.UpdatePreview();

                Page? previewPage = _contentService.GetPreviewPageByURL(pageURL);

                if (previewPage == null)
                {

                    return null;

                }

                return new PageContentModel()
                {

                    PageURL = previewPage.PageURL,

                    PageTitle = previewPage.PageTitle,

                    PageTemplate = previewPage.PageTemplate,

                    PagePreamble = previewPage.PagePreamble,

                    TileSections = previewPage.TileSections,

                    MenuItems = previewContent.MenuItems,

                    footerLinks = previewContent.FooterLinks,

                    Preview = new PreviewModel(Enumerable.Empty<HtmlString>())

                };

            }
            catch (Exception _exception)
            {

                _logger.LogError(_exception, "Unable to get interim preview landing page.");

                return null;

            }

        }

        #endregion

        #endregion

    }

}
