using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.GoogleAnalytics
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EnableGoogleAnalyticsAttribute : ResultFilterAttribute
    {
        public GoogleAnalyticsConfiguration GoogleAnalyticsConfiguration { get; }

        public EnableGoogleAnalyticsAttribute(GoogleAnalyticsConfiguration configuration)
        {
            GoogleAnalyticsConfiguration = configuration;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Controller is Controller controller)
                SetViewData(controller.ViewData);

            void SetViewData(ViewDataDictionary viewData)
                => viewData[ViewDataKeys.GoogleAnalyticsConfigurationKey] = GoogleAnalyticsConfiguration;
        }
    }
}
