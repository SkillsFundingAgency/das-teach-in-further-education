
function nodeListForEach(nodes, callback) {
    if (window.NodeList.prototype.forEach) {
        return nodes.forEach(callback);
    }
    for (var i = 0; i < nodes.length; i++) {
        callback.call(window, nodes[i], i, nodes);
    }
}

function ShowHideElement (module) {
    this.module = module
    this.id = this.module.id
    this.bodyExpandedClass = 'app-filter-layout--show'
    this.sectionExpandedClass = 'app-show-hide__section--show'
}

ShowHideElement.prototype.init = function () {
    var that = this
    var sectionExpanded = this.isExpanded()

    if (!this.id) {
        return
    }

    this.showHideLinks = document.querySelectorAll("a[href='#" + this.id + "']")

    if (this.showHideLinks.length === 0) {
        return
    }

    this.module.classList.add('app-show-hide')

    nodeListForEach(this.showHideLinks, function (showHideLink) {
        showHideLink.setAttribute('aria-controls', that.id)
        showHideLink.setAttribute('aria-expanded', sectionExpanded)
        showHideLink.addEventListener('click', that.showHideTarget.bind(that))
    })
}

ShowHideElement.prototype.showHideTarget = function (e) {
    var sectionExpanded = this.isExpanded()
    var body = document.getElementsByTagName('body')[0]

    if (sectionExpanded) {
        this.module.classList.remove(this.sectionExpandedClass)
        body.classList.remove(this.bodyExpandedClass)
    } else {
        this.module.classList.add(this.sectionExpandedClass)
        body.classList.add(this.bodyExpandedClass)
        this.module.focus()
    }
    nodeListForEach(this.showHideLinks, function (showHideLink) {
        var showText = showHideLink.getAttribute('data-text-show');
        var hideText = showHideLink.getAttribute('data-text-hide');
        showHideLink.innerHTML = (sectionExpanded ? showText : hideText);
        showHideLink.setAttribute('aria-expanded', !sectionExpanded);

        var hideClass = showHideLink.getAttribute('data-class-hide');
        var showClass = showHideLink.getAttribute('data-class-show');

        if (hideClass !== null) {
            if (sectionExpanded) {
                showHideLink.classList.remove(hideClass);
                showHideLink.classList.add(showClass);
            } else {
                showHideLink.classList.add(hideClass);
                showHideLink.classList.remove(showClass);
            }
        }
    })
    if (typeof e.preventDefault === 'function') {
        e.preventDefault()
    }
}

ShowHideElement.prototype.isExpanded = function () {
    return this.module.classList.contains(this.sectionExpandedClass)
}

function compareSchemesSetup() {
    enableCompareButton();
    $(".govuk-checkboxes__input").click(function (e) {
        enableCompareButton();
    });
}

function enableCompareButton() {
    var checked = $(".govuk-checkboxes__input:checked").length;
    $(".govuk-button").prop('disabled', checked < 2);
}

$(document).ready(function () {
    $("details.govuk-details").click(function () {
        $(this).siblings("details.govuk-details").each(function () {
            $(this).removeAttr("open");
        });
    });
});

window.addEventListener("DOMContentLoaded", e =>
{
    document.querySelectorAll('a[href]').forEach(a =>
    {
        if (location.hostname == new URL(a.href).hostname)
            return;

        a.target = "_blank";
        a.rel = "noreferrer nofollow noopener";
    });
});
