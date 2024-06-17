using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Content;
using SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class CookiePage : PageRenamed
    {

        public bool ShowMessage { get; }

        public PageRenamed AnalyticsPage { get; }

        public PageRenamed MarketingPage { get; }

        //todo: common const cookies url
        //todo: page interface??
        public CookiePage(
            
            PageRenamed analyticsPage, 
            
            PageRenamed marketingPage, 
            
            bool showMessage, 
            
            Breadcrumbs? interimBreadcrumbs = null,

            Preamble? interimPreamble = null

        )
            : base(analyticsPage.Title, "cookies", analyticsPage.Content, interimPreamble, interimBreadcrumbs)
        {

            ShowMessage = showMessage;

            AnalyticsPage = analyticsPage;

            MarketingPage = marketingPage;

        }

    }

}
