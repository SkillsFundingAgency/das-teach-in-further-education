// cookies
function saveCookieSettings() {
    let consentAnalyticsCookieRadioValue = document.querySelector(
        "input[name=ConsentAnalyticsCookie]:checked"
    ).value;
    let consentFunctionalCookieRadioValue = document.querySelector(
        "input[name=ConsentFunctionalCookie]:checked"
    ).value;

    createCookie("AnalyticsConsent", consentAnalyticsCookieRadioValue);
    createCookie("FunctionalConsent", consentFunctionalCookieRadioValue);

    document.getElementById("confirmation-banner").removeAttribute("hidden");
    window.scrollTo({ top: 0, behavior: "instant" });
}
function acceptCookies(args) {
    createCookie("DASSeenCookieMessage", true);
    document.getElementById("cookieConsent").style.display = "none";
    if (args === true) {
        createCookie("AnalyticsConsent", true);
        createCookie("FunctionalConsent", true);
        document.getElementById("cookieAccept").removeAttribute("hidden");
    } else {
        createCookie("AnalyticsConsent", false);
        createCookie("FunctionalConsent", false);
        document.getElementById("cookieReject").removeAttribute("hidden");
    }
}
function createCookie(cookiname, cookivalue) {
    let date = new Date();
    date.setFullYear(date.getFullYear() + 1);
    let expires = "expires=" + date.toGMTString();
    document.cookie =
        cookiname + "=" + cookivalue + ";" + expires + ";path=/;Secure";
}
function hideAcceptBanner() {
    document.getElementById("cookieAccept").setAttribute("hidden", "");
}
function hideRejectBanner() {
    document.getElementById("cookieReject").setAttribute("hidden", "");
}
function deleteCookie(cookiname) {
    document.cookie =
        cookiname + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; Secure"
}

function deleteAllCookies() {
    deleteCookie("DASSeenCookieMessage")
    deleteCookie("AnalyticsConsent")
    deleteCookie("FunctionalConsent")
}