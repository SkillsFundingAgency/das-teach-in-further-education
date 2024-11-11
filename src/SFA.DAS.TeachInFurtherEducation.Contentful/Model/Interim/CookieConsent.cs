using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Contentful.Model.Interim
{

    [ExcludeFromCodeCoverage]
    public class CookieConsentOptions : BaseModel, IContent
    {
        [ExcludeFromCodeCoverage]

        public required string Title { get; set; }

        public required bool ConsentToUseAnalyticsCookies { get; set; }
        public required bool ConsentToUseFunctionalCookies { get; set; }
    }
}
