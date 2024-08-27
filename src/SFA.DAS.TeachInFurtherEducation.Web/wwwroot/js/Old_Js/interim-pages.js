// document.addEventListener("DOMContentLoaded", function () {
//
//     InitializeInterimContentLinks();
//
// });
//
// function InitializeInterimContentLinks() {
//
//     $("[data-type='interim-content-section'] ul li").click(function () {
//
//         let anchor = $(this).find("a");
//
//             if (anchor.length > 0) {
//
//                 let targetId = anchor.attr("href");
//
//                 scrollToElement(targetId);
//
//             }
//
//         }
//
//     );
//
// }
//
// function scrollToElement(targetId) {
//
//     let targetElement = $("[data-id='" + targetId + "']");
//
//     if (targetElement.length > 0) {
//
//         $('html, body').animate({
//
//             scrollTop: targetElement.offset().top
//
//         }, 1000); // Adjust the duration as needed
//
//     }
//
// }