// cookies
function saveCookieSettings() {
    ["Analytics", "Functional"].forEach(type => {
        let value = document.querySelector(`input[name=Consent${type}Cookie]:checked`).value
        createCookie(`${type}Consent`, value)
    })
    showBanner("confirmation-banner")
}
function acceptCookies(acceptAll) {
    createCookie("DASSeenCookieMessage", true)
    ["Analytics", "Functional"].forEach(type => createCookie(`${type}Consent`, acceptAll))
    showBanner(acceptAll ? "cookieAccept" : "cookieReject")
    document.getElementById("cookieConsent").style.display = "none"
}
function createCookie(name, value) {
    let expires = new Date(Date.now() + 31536000000).toGMTString() // 1 year
    document.cookie = `${name}=${value};expires=${expires};path=/;Secure`
}
function showBanner(id) {
    document.getElementById(id).removeAttribute("hidden")
    window.scrollTo({ top: 0, behavior: "instant" })
}
function hideBanner(id) {
    document.getElementById(id).setAttribute("hidden", "")
}
function deleteCookie(name) {
    document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;Secure`
}
function deleteAllCookies() {
    ["DASSeenCookieMessage", "AnalyticsConsent", "FunctionalConsent"].forEach(deleteCookie)
}
