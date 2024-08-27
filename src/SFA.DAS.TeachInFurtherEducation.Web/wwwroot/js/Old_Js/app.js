// function nodeListForEach(nodes, callback) {
//     if (window.NodeList.prototype.forEach) {
//         return nodes.forEach(callback)
//     }
//     for (var i = 0; i < nodes.length; i++) {
//         callback.call(window, nodes[i], i, nodes);
//     }
// }
//
// var showHideElements = document.querySelectorAll('[data-module="app-show-hide"]')
//
// var showHideEls = {};
//
// nodeListForEach(showHideElements, function (showHideElement) {
//     var showHideEl = new ShowHideElement(showHideElement);
//     showHideEl.init();
//
//     if (showHideElement.hasAttribute('id')) {
//         showHideEls[showHideElement.id] = showHideEl;
//     }
// })