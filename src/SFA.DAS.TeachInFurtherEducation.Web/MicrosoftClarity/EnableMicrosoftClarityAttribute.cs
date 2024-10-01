using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.MicrosoftClarity
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EnableMicrosoftClarityAttribute : ResultFilterAttribute
    {
        public MicrosoftClarityConfiguration MicrosoftClarityConfiguration { get; }

        public EnableMicrosoftClarityAttribute(MicrosoftClarityConfiguration configuration)
        {
           MicrosoftClarityConfiguration = configuration;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Controller is Controller controller)
                SetViewData(controller.ViewData);

            void SetViewData(ViewDataDictionary viewData)
                => viewData[ViewDataKeys.MicrosoftClarityConfigurationKey] = MicrosoftClarityConfiguration;
        }
    }
}
