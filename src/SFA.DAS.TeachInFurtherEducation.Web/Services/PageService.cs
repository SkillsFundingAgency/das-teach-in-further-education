using SFA.DAS.TeachInFurtherEducation.Web.Models;
using System;
using System.Linq;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces;
using Microsoft.AspNetCore.Html;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services
{
    //todo: enforce lowercase url in contentful and contentservice

    public class PageService : IPageService
    {
        private const string HomePageUrl = "home";
        private const string CookiesPageUrl = "cookies";
        private const string AnalyticsCookiesPageUrl = "analyticscookies";
        private const string MarketingCookiesPageUrl = "marketingcookies";

        private readonly IContentService _contentService;
        private IReadOnlyDictionary<string, PageModel> _pageModels;

#pragma warning disable CS8618
        public PageService(IContentService contentService)
        {
            _contentService = contentService;
            contentService.ContentUpdated += OnContentUpdated;

            BuildModels();
        }
#pragma warning restore CS8618

        private void BuildModels()
        {
            _pageModels = BuildPageModelsDictionary();
        }

        private void OnContentUpdated(object? sender, EventArgs args)
        {
            BuildModels();
        }

        private ReadOnlyDictionary<string, PageModel> BuildPageModelsDictionary()
        {

            var standardPages = _contentService.Content.Pagess

                .Where(p =>    p.Url != AnalyticsCookiesPageUrl

                            && p.Url != MarketingCookiesPageUrl

                            && p.Url != HomePageUrl
                            
            );

            var pageModels = new Dictionary<string, PageModel>();

            foreach (PageRenamed page in standardPages)
            {

                pageModels.Add(
                    
                    page.Url.ToLowerInvariant(), 
                    
                    new PageModel(
                        
                        page, 
                        
                        _contentService.Content.MenuItems, 

                        _contentService.Content.FooterLinks

                    )
                    
                );

            }

            var cookiePageModel = GetCookiePageModel(_contentService.Content, false);
            if (cookiePageModel != null)
            {
                pageModels.Add(CookiesPageUrl, cookiePageModel);
            }

            return new ReadOnlyDictionary<string, PageModel>(pageModels);
        }

        public PageModel? GetPageModel(string pageUrl)
        {
            pageUrl = pageUrl.ToLowerInvariant();

            if (pageUrl == "error-check")
                throw new NotImplementedException("DEADBEEF-DEAD-BEEF-DEAD-BAAAAAAAAAAD");

            _pageModels.TryGetValue(pageUrl, out PageModel? pageModel);
            return pageModel;
        }

        public PageModel? GetCookiePageModel(IContent content, bool showMessage)
        {

            PageRenamed? analyticsPage = content.Pagess.FirstOrDefault(p => p.Url.ToLowerInvariant() == AnalyticsCookiesPageUrl);

            PageRenamed? marketingPage = content.Pagess.FirstOrDefault(p => p.Url.ToLowerInvariant() == MarketingCookiesPageUrl);

            if (analyticsPage == null || marketingPage == null)
            {

                return null;

            }

            Preamble? preamble = analyticsPage.InterimPreamble ?? marketingPage.InterimPreamble;

            Breadcrumbs? breadcrumbs = analyticsPage.InterimBreadcrumbs ?? marketingPage.InterimBreadcrumbs;

            return new PageModel(
                
                new CookiePage(analyticsPage, marketingPage, showMessage, breadcrumbs, preamble), 
                
                content.MenuItems, 

                content.FooterLinks,

                "Cookies"
                
            );

        }

        public async Task<PageModel?> GetPageModelPreview(string pageUrl)
        {

            IContent previewContent = await _contentService.UpdatePreview();

            pageUrl = pageUrl.ToLowerInvariant();

            if (pageUrl == CookiesPageUrl)
            {

                var pageModel = GetCookiePageModel(previewContent, false);

                if (pageModel == null)
                {

                    return null;

                }
                    
                pageModel.Preview = new PreviewModel(GetErrors((CookiePage)pageModel.Page));

                return pageModel;

            }

            var page = previewContent.Pagess.FirstOrDefault(p => p.Url.ToLowerInvariant() == pageUrl);

            if (page == null)
            {

                return null;

            }
                
            return new PageModel(page, previewContent.MenuItems, previewContent.FooterLinks, "Page")
            {

                Preview = new PreviewModel(GetErrors(page))

            };

        }

        private IEnumerable<HtmlString> GetErrors(PageRenamed page)
        {

            var errors = new List<HtmlString>();

            if (string.IsNullOrWhiteSpace(page.Title))
            {

                errors.Add(new HtmlString("Title must not be blank"));

            }

            if (page.Content == null)
            {

                errors.Add(new HtmlString("Content must not be blank"));

            }

            return errors;

        }

        private IEnumerable<HtmlString> GetErrors(CookiePage page)
        {

            var errors = new List<HtmlString>();

            if (page.AnalyticsPage.Content == null)
            {

                errors.Add(new HtmlString("AnalyticsPage content must not be blank"));

            }

            if (page.MarketingPage.Content == null)
            {

                errors.Add(new HtmlString("MarketingPage content must not be blank"));

            }

            return errors;

        }

        public (string? routeName, object? routeValues) RedirectPreview(string pageUrl)
        {

            pageUrl = pageUrl.ToLowerInvariant();

            switch (pageUrl)
            {

                case AnalyticsCookiesPageUrl:
                case MarketingCookiesPageUrl:
                    {

                        return ("page-preview", new { pageUrl = CookiesPageUrl });

                    }
                case HomePageUrl:
                    {

                        return ("home-preview", null);

                    }
                    
            }

            return default;

        }

    }

}
