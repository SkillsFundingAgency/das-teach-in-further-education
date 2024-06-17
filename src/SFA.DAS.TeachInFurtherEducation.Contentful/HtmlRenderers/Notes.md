# Notes

We shouldn't need a GdsTextRenderer, as only bold is supported in GDS (italics and underline are not), and `<strong>` is fine (`govuk-!-font-weight-bold` is only required on other elements).

# todo

contentful's <p> handling is inconsistent - is there a way to switch the rich text box to show the underlying html, like orchard core supports?

contentful adds a bonus empty <p> at the end of every rich text box for some reason (https://github.com/contentful/rich-text/issues/101)
we need to remove or ignore it (eg. in css)