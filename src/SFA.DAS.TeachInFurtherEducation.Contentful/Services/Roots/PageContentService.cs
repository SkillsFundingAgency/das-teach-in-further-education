using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Interfaces.Roots;
using SFA.DAS.TeachInFurtherEducation.Contentful.Services.Roots.Base;
using ApiPage = SFA.DAS.TeachInFurtherEducation.Contentful.Model.Api.Page;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Services.Roots
{

    public class PageContentService : ContentRootService, IPageContentService
    {

        private readonly ILogger<PageContentService> _logger;

        public PageContentService(HtmlRenderer htmlRenderer, ILogger<PageContentService> logger) : base(htmlRenderer)
        {

            _logger = logger;

        }

        /// <summary>
        /// Retrieves the interim landing page from the Contentful CMS using the provided Contentful client.
        /// </summary>
        /// <param name="contentfulClient">The Contentful client used to retrieve data from the CMS.</param>
        /// <returns>Returns an interim landing page object if found, otherwise returns null.</returns>
        public async Task<IEnumerable<Page>> GetAllPages(IContentfulClient contentfulClient)
        {

            _logger.LogInformation("Beginning {MethodName}...", nameof(GetAllPages));

            try
            {

                var builder = QueryBuilder<ApiPage>.New.ContentTypeIs("page").Include(3);
                var pages = await contentfulClient.GetEntries(builder);
                LogErrors(pages);

                return await Task.WhenAll(FilterValidUrl(pages, _logger).Select(ToContent));
            }
            catch(Exception _Exception)
            {

                _logger.LogError(_Exception, "Unable to get pages.");

                return Enumerable.Empty<Page>();

            }

        }

        //todo: ctor on Page?
        private async Task<Page> ToContent(ApiPage apiPage)
        {
            return new Page(
                apiPage.PageURL!,
                 apiPage.PageTitle!,
                 apiPage.PageTemplate,
                  apiPage.TileSections,
                  apiPage.ContentBoxSection,
                  (await ToHtmlString(apiPage.Contents)),
                    apiPage.NewsLetter!,
                    apiPage.ContactUs!,
                    apiPage.Video!,
                    apiPage.PageComponents!,
                  apiPage.PagePreamble,
                  apiPage.ImageCardBanner
                  );
        }

        /// <summary>
        /// Retrieves Menu items from the Contentful CMS using the provided Contentful client.
        /// </summary>
        /// <param name="contentfulClient">The Contentful client used to retrieve data from the CMS.</param>
        /// <returns>Returns a collection of Menu items if found, otherwise returns an empty collection.</returns>
        public async Task<IEnumerable<MenuItem>> GetMenuItems(IContentfulClient contentfulClient)
        {

            _logger.LogInformation("Beginning {MethodName}...", nameof(GetMenuItems));

            try
            {

                var builder = QueryBuilder<MenuItem>.New.ContentTypeIs("headerMenuItem");

                var menuItems = await contentfulClient.GetEntries(builder);

                if (menuItems.Any())
                {

                    _logger.LogInformation("Retrieved landing page: {MenuItemCount}", menuItems.Count());

                    return menuItems.OrderBy(t => t.MenuItemOrder);

                }
                else
                {

                    _logger.LogInformation("No menu items found.");

                    return Enumerable.Empty<MenuItem>();

                }

            }
            catch (Exception _Exception)
            {

                _logger.LogError(_Exception, "Unable to get the interim menu items.");

                return Enumerable.Empty<MenuItem>();

            }

        }

        /// <summary>
        /// Retrieves interim pages from the Contentful CMS using the provided Contentful client.
        /// </summary>
        /// <param name="contentfulClient">The Contentful client used to retrieve data from the CMS.</param>
        /// <returns>Returns a collection of interim pages if found, otherwise returns an empty collection.</returns>
        public async Task<IEnumerable<InterimPage>> GetInterimPages(IContentfulClient contentfulClient)
        {

            _logger.LogInformation("Beginning {MethodName}...", nameof(GetInterimPages));

            try
            {

                var builder = QueryBuilder<InterimPage>.New.ContentTypeIs("interimPage").Include(2);

                var interimPages = await contentfulClient.GetEntries(builder);

                if (interimPages.Any())
                {

                    _logger.LogInformation("Retrieved interim pages: {MenuItemCount}", interimPages.Count());

                    return interimPages;

                }
                else
                {

                    _logger.LogInformation("No interim pages found.");

                    return Enumerable.Empty<InterimPage>();

                }

            }
            catch (Exception _Exception)
            {

                _logger.LogError(_Exception, "Unable to get the interim pages.");

                return Enumerable.Empty<InterimPage>();

            }

        }

        /// <summary>
        /// Retrieves the footer links from the Contentful CMS using the provided Contentful client.
        /// </summary>
        /// <param name="contentfulClient">The Contentful client used to retrieve data from the CMS.</param>
        /// <returns>Returns the interim footer links if found, otherwise returns null.</returns>
        public async Task<IEnumerable<FooterLink>> GetFooterLinks(IContentfulClient contentfulClient)
        {

            _logger.LogInformation("Beginning {MethodName}...", nameof(GetFooterLinks));

            try
            {

                var query = new QueryBuilder<FooterLink>()

                .ContentTypeIs("footerLink")

                .Include(2);

                var results = await contentfulClient.GetEntries(query);

                List<FooterLink> resultList = results.Items.ToList();

                if (resultList.Any())
                {

                    FooterLink footer = resultList[0];

                    _logger.LogInformation("Retrieved footer: {Title}", footer.FooterLinkTitle);

                    return resultList;

                }
                else
                {

                    _logger.LogInformation("No matching footer.");

                    return Enumerable.Empty<FooterLink>();

                }

            }
            catch (Exception _Exception)
            {

                _logger.LogError(_Exception, "Unable to get the footer.");

                return Enumerable.Empty<FooterLink>();

            }

        }

        /// <summary>
        /// Retrieves the beta banner information from the Contentful CMS using the provided Contentful client.
        /// </summary>
        /// <param name="contentfulClient">The Contentful client used to retrieve data from the CMS.</param>
        /// <returns>Returns the beta banner if found, otherwise returns null.</returns>
        public async Task<BetaBanner?> GetBetaBanner(IContentfulClient contentfulClient)
        {

            _logger.LogInformation("Beginning {MethodName}...", nameof(GetBetaBanner));

            try
            {

                var query = new QueryBuilder<BetaBanner>()

                .FieldEquals(a => a.BetaBannerID, "employer-schemes-beta-banner")

                .ContentTypeIs("betaBanner")

                .Include(2);

                var results = await contentfulClient.GetEntries(query);

                List<BetaBanner> resultList = results.Items.ToList();

                if (resultList.Any())
                {

                    BetaBanner banner = resultList[0];

                    _logger.LogInformation("Retrieved beta banner: {Title}", banner.BetaBannerTitle);

                    return banner;

                }
                else
                {

                    _logger.LogInformation("No matching beta banner.");

                    return null;

                }

            }
            catch (Exception _Exception)
            {

                _logger.LogError(_Exception, "Unable to get the beta banner.");

                return null;

            }

        }

    }

}
