( () => {
        var o = {
            621: () => {
                NodeList.prototype.forEach || (NodeList.prototype.forEach = Array.prototype.forEach),
                Array.prototype.includes || Object.defineProperty(Array.prototype, "includes", {
                    enumerable: !1,
                    value: function(t) {
                        return 0 < this.filter(function(e) {
                            return e === t
                        }).length
                    }
                }),
                Element.prototype.matches || (Element.prototype.matches = Element.prototype.msMatchesSelector || Element.prototype.webkitMatchesSelector),
                Element.prototype.closest || (Element.prototype.closest = function(e) {
                        var t = this;
                        do {
                            if (Element.prototype.matches.call(t, e))
                                return t
                        } while (null !== (t = t.parentElement || t.parentNode) && 1 === t.nodeType);
                        return null
                    }
                )
            }
        }
            , r = {};
        function n(e) {
            var t = r[e];
            return void 0 !== t || (t = r[e] = {
                exports: {}
            },
                o[e](t, t.exports, n)),
                t.exports
        }
        ( () => {
                "use strict";
                function l(e, t) {
                    var o;
                    e && t && (o = "true" === e.getAttribute(t) ? "false" : "true",
                        e.setAttribute(t, o))
                }
                n(621),
                    document.addEventListener("DOMContentLoaded", function() {
                        var t, o, r, n, e, c, s;
                        function a(e) {
                            e.preventDefault(),
                                l(n, "aria-expanded"),
                                n.classList.toggle("is-active"),
                                c.classList.toggle("js-show"),
                                s.classList.toggle("js-show")
                        }
                        t = document.querySelector("#toggle-menu"),
                            e = document.querySelector("#close-menu"),
                            o = document.querySelector("#header-navigation"),
                            r = function(e) {
                                e.preventDefault(),
                                    l(t, "aria-expanded"),
                                    t.classList.toggle("is-active"),
                                    o.classList.toggle("js-show")
                            }
                            ,
                        t && e && o && [t, e].forEach(function(e) {
                            e.addEventListener("click", r)
                        }),
                            n = document.querySelector("#toggle-search"),
                            e = document.querySelector("#close-search"),
                            c = document.querySelector("#wrap-search"),
                            s = document.querySelector("#content-header"),
                        n && e && [n, e].forEach(function(e) {
                            e.addEventListener("click", a)
                        })
                    })
            }
        )()
    }
)();