using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.TeachInFurtherEducation.Web.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class CookieKeys
    {
        public const string DasSeenCookieMessage = "DASSeenCookieMessage";
        public const string AnalyticsConsent = nameof(AnalyticsConsent);
        public const string FunctionalConsent = nameof(FunctionalConsent);
    }
}
