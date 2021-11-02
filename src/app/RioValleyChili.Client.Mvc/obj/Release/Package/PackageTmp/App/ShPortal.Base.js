(function (ko, $) {
    "use strict";

    var dataTag = "koJqueryAppend";
    ko.bindingHandlers.jqueryAppend = {
        init: function () { return { controlsDescendantBindings: true } },
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor()),
                $element = $(element),
                appendedId = $element.data(dataTag);

            appendedId && $(appendedId).remove();
            $element.append(value);
            $element.data(dataTag, value);
        }
    };
}(ko, jQuery));


///*!
// * Viva Controls
// * Revision: 4eea9751e611844a5f4f53d14905ec307e325f4c
// * Compiled: 2014-06-03 15:00:11.2464127
// */
//var MsPortalFx, _azcPrivate, MsPortal;
//(function (n) {
//    (function () { "use strict" })(n.Base || (n.Base = {})); var
//    t = n.Base
//})(MsPortalFx || (MsPortalFx = {})), function (n) {
//    (function (n) {
//        "use strict"; (function (n) {
//            n[n.None =
//            0] = "None"; n[n.White = 1] = "White"; n[n.Black = 2] = "Black"; n[n.Blue = 3] = "Blue"
//        })(n.ImagePalette || (n.ImagePalette =
//        {})); var t = n.ImagePalette
//    })(n.Base || (n.Base = {})); var t = n.Base
//}(MsPortalFx || (MsPortalFx = {})), function (
//n) {
//    (function (n) {
//        (function (n) {
//            "use strict"; (function (n) {
//                n[n.Blank = 0] = "Blank"; n[n.Custom = 1] = "Custom"; n[
//                n.ImageUri = 2] = "ImageUri"; n[n.Add = 4] = "Add"; n[n.Book = 10] = "Book"; n[n.Check = 12] = "Check"; n[n.Delete = 17] = "Delete";
//                n[n.Disabled = 18] = "Disabled"; n[n.Discard = 19] = "Discard"; n[n.Download = 20] = "Download"; n[n.Filter = 22] = "Filter";
//                n[n.Gear = 27] = "Gear"; n[n.Guide = 29] = "Guide"; n[n.Hyperlink = 31] = "Hyperlink"; n[n.Info = 32] = "Info"; n[n.Link =
//                35] = "Link"; n[n.Lock = 37] = "Lock"; n[n.Monitoring = 41] = "Monitoring"; n[n.Pending = 43] = "Pending"; n[n.Person =
//                44] = "Person"; n[n.PersonWithFriend = 45] = "PersonWithFriend"; n[n.Pin = 46] = "Pin"; n[n.Properties = 48] = "Properties";
//                n[n.Question = 49] = "Question"; n[n.QuickStart = 50] = "QuickStart"; n[n.Refresh = 51] = "Refresh"; n[n.Save = 52] = "Save";
//                n[n.Start = 61] = "Start"; n[n.Stop = 62] = "Stop"; n[n.Subtract = 65] = "Subtract"; n[n.Swap = 66] = "Swap"; n[n.Unlock =
//                69] = "Unlock"; n[n.Unpin = 70] = "Unpin"; n[n.Canceled = 74] = "Canceled"; n[n.Clock = 75] = "Clock"; n[n.Clone = 76] = "Clone";
//                n[n.Error = 77] = "Error"; n[n.Paused = 79] = "Paused"; n[n.Queued = 80] = "Queued"; n[n.Warning = 81] = "Warning"; n[n.
//                AddTeamMember = 82] = "AddTeamMember"; n[n.Attachment = 83] = "Attachment"; n[n.AvatarDefault = 84] = "AvatarDefault";
//                n[n.AvatarUnknown = 85] = "AvatarUnknown"; n[n.Backlog = 86] = "Backlog"; n[n.Code = 94] = "Code"; n[n.Commit = 96] = "Commit";
//                n[n.Disable = 98] = "Disable"; n[n.Edit = 99] = "Edit"; n[n.Favorite = 101] = "Favorite"; n[n.File = 102] = "File"; n[n.
//                GearFlat = 103] = "GearFlat"; n[n.GetMoreLicense = 104] = "GetMoreLicense"; n[n.GetStarted = 105] = "GetStarted"; n[
//                n.Go = 108] = "Go"; n[n.History = 109] = "History"; n[n.Inactive = 110] = "Inactive"; n[n.Log = 112] = "Log"; n[n.Postpone =
//                114] = "Postpone"; n[n.Release = 115] = "Release"; n[n.Request = 116] = "Request"; n[n.Retain = 117] = "Retain"; n[n.Tasks =
//                119] = "Tasks"; n[n.Triangle = 120] = "Triangle"; n[n.Upload = 121] = "Upload"; n[n.Connect = 132] = "Connect"; n[n.Disconnect =
//                133] = "Disconnect"; n[n.Redo = 157] = "Redo"; n[n.ShellChevron = 158] = "ShellChevron"; n[n.Tools = 159] = "Tools"; n[
//                n.Wrench = 160] = "Wrench"; n[n.AzureQuickstart = 161] = "AzureQuickstart"; n[n.Publish = 162] = "Publish"; n[n.ThisWeek =
//                163] = "ThisWeek"; n[n.DownloadFlat = 174] = "DownloadFlat"; n[n.Ellipsis = 176] = "Ellipsis"; n[n.ForPlacementOnly =
//                177] = "ForPlacementOnly"; n[n.MonitoringFlat = 193] = "MonitoringFlat"; n[n.TrendDown = 204] = "TrendDown"; n[n.
//                TrendUp = 205] = "TrendUp"; n[n.Variables = 206] = "Variables"; n[n.Commits = 210] = "Commits"; n[n.HeartPulse = 211] =
//                "HeartPulse"; n[n.PowerUp = 216] = "PowerUp"; n[n.GuideFlat = 218] = "GuideFlat"; n[n.Support = 219] = "Support"; n[
//                n.InfoFlat = 225] = "InfoFlat"; n[n.Signout = 226] = "Signout"; n[n.LaunchCurrent = 227] = "LaunchCurrent"; n[n.Feedback =
//                228] = "Feedback"
//            })(n.SvgType || (n.SvgType = {})); var i = n.SvgType, t = function () {
//                function n(n, t) {
//                    this.type =
//                    n; this.palette = t || 0
//                } return n
//            }(); n.ImageData = t
//        })(n.Image || (n.Image = {})); var t = n.Image
//    })(n.Services || (n.
//    Services = {})); var t = n.Services
//}(MsPortalFx || (MsPortalFx = {})), function (n) {
//    (function (t) {
//        (function (t) {
//            "use strict";
//            function r(n) { return { type: 2, data: n } } function u(n) { return new i(4, n) } function f(n) { return new i(82, n) }
//            function e(n) { return new i(83, n) } function o(n) { return new i(84, n) } function s(n) { return new i(85, n) } function h(
//            n) { return new i(161, n) } function c(n) { return new i(86, n) } function l(n) { return new i(0, n) } function a(n)
//            { return new i(10, n) } function v(n) { return new i(74, n) } function y(n) { return new i(12, n) } function p(n) {
//                return new i(75, n)
//            } function w(n) { return new i(76, n) } function b(n) { return new i(94, n) } function k(n) {
//                return new
//                i(96, n)
//            } function d(n) { return new i(210, n) } function g(n) { return new i(132, n) } function nt(n) {
//                return new
//                i(17, n)
//            } function tt(n) { return new i(98, n) } function it(n) { return new i(18, n) } function rt(n) {
//                return new
//                i(19, n)
//            } function ut(n) { return new i(133, n) } function ft(n) { return new i(174, n) } function et(n) {
//                return new
//                i(99, n)
//            } function ot(n) { return new i(176, n) } function st(n) { return new i(77, n) } function ht(n) {
//                return new
//                i(101, n)
//            } function ct(n) { return new i(228, n) } function lt(n) { return new i(102, n) } function at(n) {
//                return new
//                i(22, n)
//            } function vt(n) { return new i(177, n) } function yt(n) { return new i(103, n) } function pt(n) {
//                return new
//                i(104, n)
//            } function wt(n) { return new i(105, n) } function bt(n) { return new i(108, n) } function kt(n) {
//                return new
//                i(218, n)
//            } function dt(n) { return new i(211, n) } function gt(n) { return new i(109, n) } function ni(n) {
//                return new
//                i(31, n)
//            } function ti(n) { return new i(110, n) } function ii(n) { return new i(227, n) } function ri(n) {
//                return new
//                i(35, n)
//            } function ui(n) { return new i(37, n) } function fi(n) { return new i(112, n) } function ei(n) {
//                return new
//                i(193, n)
//            } function oi(n) { return new i(79, n) } function si(n) { return new i(43, n) } function hi(n) {
//                return new
//                i(44, n)
//            } function ci(n) { return new i(45, n) } function li(n) { return new i(46, n) } function ai(n) {
//                return new
//                i(114, n)
//            } function vi(n) { return new i(216, n) } function yi(n) { return new i(48, n) } function pi(n) {
//                return new
//                i(162, n)
//            } function wi(n) { return new i(49, n) } function bi(n) { return new i(80, n) } function ki(n) {
//                return new
//                i(157, n)
//            } function di(n) { return new i(51, n) } function gi(n) { return new i(115, n) } function nr(n) {
//                return new
//                i(116, n)
//            } function tr(n) { return new i(117, n) } function ir(n) { return new i(52, n) } function rr(n) {
//                return new
//                i(226, n)
//            } function ur(n) { return new i(61, n) } function fr(n) { return new i(62, n) } function er(n) {
//                return new
//                i(65, n)
//            } function or(n) { return new i(219, n) } function sr(n) { return new i(66, n) } function hr(n) {
//                return new
//                i(119, n)
//            } function cr(n) { return new i(163, n) } function lr(n) { return new i(159, n) } function ar(n) {
//                return new
//                i(204, n)
//            } function vr(n) { return new i(205, n) } function yr(n) { return new i(120, n) } function pr(n) {
//                return new
//                i(69, n)
//            } function wr(n) { return new i(70, n) } function br(n) { return new i(121, n) } function kr(n) {
//                return new
//                i(206, n)
//            } function dr(n) { return new i(81, n) } function gr(n) { return new i(160, n) } function nu(n) {
//                return new
//                i(225, n)
//            } var iu = n.Services.Image.SvgType, i = n.Services.Image.ImageData, tu; t.ImageUri = r; t.Add = u; t.AddTeamMember =
//            f; t.Attachment = e; t.AvatarDefault = o; t.AvatarUnknown = s; t.AzureQuickstart = h; t.Backlog = c; t.Blank = l; t.Book =
//            a; t.Canceled = v; t.Check = y; t.Clock = p; t.Clone = w; t.Code = b; t.Commit = k; t.Commits = d; t.Connect = g; t.Delete = nt;
//            t.Disable = tt; t.Disabled = it; t.Discard = rt; t.Disconnect = ut; t.Download = ft; t.Edit = et; t.Ellipsis = ot; t.Error =
//            st; t.Favorite = ht; t.Feedback = ct; t.File = lt; t.Filter = at; t.ForPlacementOnly = vt; t.Gear = yt; t.GetMoreLicense =
//            pt; t.GetStarted = wt; t.Go = bt; t.Guide = kt; t.HeartPulse = dt; t.History = gt; t.Hyperlink = ni; t.Inactive = ti; t.LaunchCurrent =
//            ii; t.Link = ri; t.Lock = ui; t.Log = fi; t.Monitoring = ei; t.Paused = oi; t.Pending = si; t.Person = hi; t.PersonWithFriend =
//            ci; t.Pin = li; t.Postpone = ai; t.PowerUp = vi; t.Properties = yi; t.Publish = pi; t.Question = wi; t.Queued = bi; t.Redo =
//            ki; t.Refresh = di; t.Release = gi; t.Request = nr; t.Retain = tr; t.Save = ir; t.Signout = rr; t.Start = ur; t.Stop = fr; t.
//            Subtract = er; t.Support = or; t.Swap = sr; t.Tasks = hr; t.ThisWeek = cr; t.Tools = lr; t.TrendDown = ar; t.TrendUp = vr; t.
//            Triangle = yr; t.Unlock = pr; t.Unpin = wr; t.Upload = br; t.Variables = kr; t.Warning = dr; t.Wrench = gr; t.Info = nu, function (
//            n) { function t() { return new i(158) } n.Chevron = t }(t.Shell || (t.Shell = {})); tu = t.Shell
//        })(t.Images || (t.Images =
//        {})); var i = t.Images
//    })(n.Base || (n.Base = {})); var t = n.Base
//}(MsPortalFx || (MsPortalFx = {})), function (n) {
//    (function (
//    n) {
//        (function () {
//            "use strict";
//            var u = window,
//                n = jQuery,
//                i = "koJqueryAppend",
//                t = "###knockoutArrayEditFixupTokenPropertyName###",
//                r;

//            ko.bindingHandlers.activateWidget = {
//                init: function (t, i, r) {
//                    var u = i(), f = r()["fxcontrol-options"] || {}; n(
//                    t)[u](f)
//                }, update: function () { }
//            }; ko.bindingHandlers.jqueryAppend = {
//                init: function () {
//                    return {
//                        controlsDescendantBindings:
//                        !0
//                    }
//                }, update: function (t, r) {
//                    var f = ko.utils.unwrapObservable(r()), u = n(t), e = u.data(i); e && n(e).remove(); u.
//                    append(f); u.data(i, f)
//                }
//            }; ko.bindingHandlers.commands = {
//                update: function (t, i) {
//                    var u = n(t), r = i(), f, e; if (r &&
//                    r.group) {
//                        u.on("contextmenu", f = function (t) {
//                            var f, i; if (t.which === 3) {
//                                if (t.ctrlKey) return; f = ko.utils.unwrapObservable(
//                                r.viewModel); i = n.Event("rightclick"); i.clientX = t.clientX; i.clientY = t.clientY; u.trigger(i, {
//                                    commandGroup:
//                                    r.group, viewModel: f, entityType: 2
//                                }); t.preventDefault(); t.stopPropagation()
//                            }
//                        }); ko.utils.domNodeDisposal.
//                        addDisposeCallback(t, e = function () { u.off("contextmenu", f) })
//                    }
//                }
//            }; ko.utils.fixupArrayEdits = function (n) {
//                if (
//                n) {
//                    if (n[t]) throw new Error("Knockout array edits have already been fixed up."); n[t] = t; var i = 0; n.forEach(
//                    function (n) { switch (n.status) { case "added": i++; break; case "deleted": n.index += i; i-- } })
//                } return n
//            }; ko.utils.
//            applyArrayEdits = function (n, t, i) {
//                t && (i || (i = function (n) { return n }), t.forEach(function (t) {
//                    switch (t.status)
//                    { case "added": n.splice(t.index, 0, i(t.value)); break; case "deleted": n.splice(t.index, 1) }
//                }))
//            }; ko.utils.observableArraySubscribe =
//            function (n, t) {
//                return n.subscribe(function (i) {
//                    var r = i.slice(0), u = n.subscribe(function (n) {
//                        var i = ko.utils.
//                        compareArrays(r, n); t(i); u.dispose()
//                    })
//                }, null, "beforeChange")
//            }; ko.utils.twoWayBinding = function (n, t) {
//                var
//                r = n(), u = t(), i = !1, f = function (n, t) { if (!i) try { i = !0; n(t) } finally { i = !1 } }, e, o; return r !== undefined ? t(r) : u !==
//                undefined && n(u), e = n.subscribe(function (n) { f(t, n) }), o = t.subscribe(function (t) { f(n, t) }), [e, o]
//            }; ko.subscribable.
//            fn.getChangeNumber = function () { return this._changeNumber || 0 }; r = ko.subscribable.fn.notifySubscribers; ko.
//            subscribable.fn.notifySubscribers = function (n, t) {
//                return (t === undefined || t === "change") && (this._changeNumber =
//                this.getChangeNumber() + 1), r.apply(this, arguments)
//            }
//        })(n.KnockoutExtensions || (n.KnockoutExtensions = {})
//        ); var t = n.KnockoutExtensions
//    })(n.Base || (n.Base = {})); var t = n.Base
//}(MsPortalFx || (MsPortalFx = {})), function (
//n) {
//    (function () {
//        "use strict"; function n(n) {
//            var r = this.length, t, i; if (r === 0) return null; if (n === undefined)
//                return this[0]; for (t = 0; t < r; t++) if (i = this[t], n(i) === !0) return i; return null
//        } function t(n) {
//            var t = this.
//            map(function (n, t) { return { i: t, v: n } }); return t.sort(function (t, i) {
//                var r = n(t.v, i.v); return r === 0 ? t.i - i.
//                    i : r
//            }), t.map(function (n) { return n.v })
//        } Array.prototype.first = n; Array.prototype.stableSort = t
//    })(n.Polyfills ||
//    (n.Polyfills = {})); var t = n.Polyfills
//}(MsPortal || (MsPortal = {})), function (n) {
//    (function () {
//        "use strict"; function n(
//        ) {
//            var t, n = arguments; return n && n.length === 1 && n[0] && typeof n[0] == "object" ? (n = n[0], this.replace(/\{[_a-zA-Z\d]+\}/g,
//            function (i) { return t = i.substring(1, i.length - 1), n.hasOwnProperty(t) ? n[t] : i })) : this.replace(/\{(\d+)\}/g,
//            function (t, i) { return n[i] !== undefined ? n[i] : t })
//        } String.prototype.format = n
//    })(n.Polyfills || (n.Polyfills =
//    {})); var t = n.Polyfills
//}(MsPortal || (MsPortal = {})), function (n) {
//    (function () {
//        "use strict"; var b = Date.prototype.
//        toString, k = Date.parse, n = {
//            days: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
//            daysAbbr: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"], months: ["January", "February", "March", "April", "May",
//            "June", "July", "August", "September", "October", "November", "December"], monthsAbbr: ["Jan", "Feb", "Mar", "Apr",
//            "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], ampm: ["AM", "PM"], ampmAbbr: [], dateSeparator: "/", timeSeparator:
//            ":", standard: {
//                d: "M/d/yyyy", D: "dddd, MMMM dd, yyyy", F: "dddd, MMMM dd, yyyy h:mm:ss tt", m: "MMMM dd", M:
//                "MMMM dd", r: "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", R: "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", s: "yyyy'-'MM'-'dd'T'HH':'mm':'ss",
//                t: "h:mm tt", T: "h:mm:ss tt", u: "yyyy'-'MM'-'dd HH':'mm':'ss'Z'", y: "MMMM, yyyy", Y: "MMMM, yyyy"
//            }, firstDayOfWeek:
//            0, agoStrings: {
//                lessThanAMinute: "Just now", aMinute: "1 min ago", minutes: "{0} min ago", anHour: "1 h ago",
//                hours: "{0} h ago", aDay: "1 d ago", days: "{0} d ago", aWeek: "1 wk ago", weeks: "{0} wk ago", aMonth: "1 mo ago",
//                months: "{0} mo ago", aYear: "1 yr ago", years: "{0} yr ago"
//            }
//        }, f = 60, e = f * 60, u = e * 24, h = u * 7, c = u * 365, l = c / 12, it =
//        f * 1e3, rt = e * 1e3, ut = u * 1e3, ft = h * 1e3, v = [{ limit: f, format: n.agoStrings.lessThanAMinute, factorSeconds: 0 }, {
//            limit:
//            f * 1.5, format: n.agoStrings.aMinute, factorSeconds: 0
//        }, {
//            limit: e, format: n.agoStrings.minutes, factorSeconds:
//            f
//        }, { limit: e * 1.5, format: n.agoStrings.anHour, factorSeconds: 0 }, {
//            limit: u, format: n.agoStrings.hours, factorSeconds:
//            e
//        }, { limit: u * 1.5, format: n.agoStrings.aDay, factorSeconds: 0 }, {
//            limit: h, format: n.agoStrings.days, factorSeconds:
//            u
//        }, { limit: h * 1.5, format: n.agoStrings.aWeek, factorSeconds: 0 }, {
//            limit: l, format: n.agoStrings.weeks, factorSeconds:
//            h
//        }, { limit: l * 1.5, format: n.agoStrings.aMonth, factorSeconds: 0 }, {
//            limit: c, format: n.agoStrings.months, factorSeconds:
//            l
//        }, { limit: c * 1.5, format: n.agoStrings.aYear, factorSeconds: 0 }, {
//            limit: Number.POSITIVE_INFINITY, format: n.
//            agoStrings.years, factorSeconds: c
//        }], d = function (n, t, i) {
//            return i = i ? i : "0", t + 1 >= n.length && (n = Array(t + 1 - n.
//            length).join(i) + n), n
//        }, o = function (n, t, i) { return d(n + "", t, i) }, y = function (n, t) {
//            return n.substr(n.length -
//            t)
//        }, i = function (n, t) { return y(n + "", t) }, r = function (n, t) { return y(o(n, t, "0"), t) }, p = function (n) {
//            return n =
//            n % 12, n === 0 ? 12 : n
//        }, a = function (n) { return n.split("").reverse().join("") }, s = function (n) {
//            return "\\" + n.split(
//            "").join("\\")
//        }, t, g = function (u) {
//            var f = u.length, e, h, c, l; switch (u[0]) {
//                case "y": switch (f) {
//                    case 3: return o(
//                    t.getFullYear(), f); case 1: return parseInt(i(t.getFullYear(), 2), 10) + ""; default: return r(t.getFullYear(
//                    ), f)
//                } case "M": switch (f) {
//                    case 4: return s(n.months[t.getMonth()]); case 3: return s(n.monthsAbbr[t.getMonth(
//                    )]); case 2: return r(t.getMonth() + 1, 2); default: return i(t.getMonth() + 1, 2)
//                } case "d": switch (f) {
//                    case 4: return s(
//                    n.days[t.getDay()]); case 3: return s(n.daysAbbr[t.getDay()]); case 2: return r(t.getDate(), 2); default: return i(
//                    t.getDate(), 2)
//                } case "h": return f === 2 ? r(p(t.getHours()), 2) : i(p(t.getHours()), 2); case "H": return f === 2 ? r(
//                t.getHours(), 2) : i(t.getHours(), 2); case "m": return f === 2 ? r(t.getMinutes(), 2) : i(t.getMinutes(), 2); case "s":
//                    return f === 2 ? r(t.getSeconds(), 2) : i(t.getSeconds(), 2); case "t": return f === 2 ? n.ampm[t.getHours() < 12 ? 0 : 1] :
//                    n.ampmAbbr[t.getHours() < 12 ? 0 : 1]; case "z": return (e = -t.getTimezoneOffset() / 60, h = e < 0, h && (e = -e), c = parseInt(
//                    e + "", 10), l = (e - c) * 60, f === 3) ? (h ? "-" : "+") + o(c, 2) + ":" + o(l, 2) : (h ? "-" : "+") + o(c, f); case "/": return n.dateSeparator
//                case ":": return n.timeSeparator
//            } return ""
//        }, nt = function (n) { return a(g(n)) }, w = function (n, t) {
//            var r, i, u, f;
//            for (t = t || new Date, u = t.getTime() - n.getTime(), u /= 1e3, r = 0; r < v.length; r++) if (i = v[r], u < i.limit) break; return f =
//            i.format, i.factorSeconds && (f = f.format(Math.round(u / i.factorSeconds))), f
//        }, tt = function (t, i) {
//            var r, e, u,
//            f, o, s = t.toString(); for (i = i || new Date, o = new Date(i.getFullYear(), i.getMonth(), i.getDate() - i.getDay() +
//            n.firstDayOfWeek), f = [{ limit: 86400, format: function (n) { return w(n, i) } }, {
//                limit: (i.getTime() - o.getTime())
//            / 1e3, format: function (n) { return n.toString("dddd") }
//            }, {
//                limit: Number.POSITIVE_INFINITY, format: function (
//            n) { return n.toString("d") }
//            }], e = (i.getTime() - t.getTime()) / 1e3, r = 0; r < f.length; r++) if (u = f[r], e < u.limit &&
//            u.limit > 0) break; return u.format(t)
//        }; Date.getLocaleValues = function () { return n }; Date.setLocaleValues = function (
//        t) {
//            var i; n = t; n.ampmAbbr = [n.ampm[0][0] || "", n.ampm[1][0] || ""]; i = n.standard; i.f = i.D + " " + i.t; i.g = i.d + " " +
//            i.t; i.G = i.d + " " + i.T
//        }; Date.prototype.toRelativeString = function (n, t) {
//            var i; t = t || new Date; n = n || "difference";
//            switch (n) { case "difference": i = w(this, t); break; case "timestamp": i = tt(this, t) } return i
//        }; Date.prototype.toString =
//        function () {
//            if (arguments.length) {
//                var i = arguments[0]; return i = n.standard[i] || i, i = i.replace(/'([^']*)'/g,
//                function (n, t) { return s(t) }), t = this, a(a(i).replace(/(y{1,5}|M{1,4}|d{1,4}|h{1,2}|H{1,2}|m{1,2}|s{1,2}|t{1,2}|z{1,3}|\/|:)(?!\\)/g,
//                nt).replace(/\\(?!\\)/g, ""))
//            } return b.apply(this, arguments)
//        }; Date.parse = function (n) {
//            return (n + "").substr(
//            0, 6) === "/Date(" ? parseInt(n.substr(6), 10) : k.apply(this, arguments)
//        }; Date.setLocaleValues(n)
//    })(n.Polyfills ||
//    (n.Polyfills = {})); var t = n.Polyfills
//}(MsPortal || (MsPortal = {})), function (n) {
//    "use strict"; var t = null, i,
//    r = window, u = function () {
//        function n(n) { this._prefixCode = ";;;"; this._dataTransfer = n } return Object.defineProperty(
//        n.prototype, "dropEffect", {
//            get: function () { return this._dataTransfer.dropEffect }, set: function (n) {
//                if (n !==
//                "none" && n !== "copy" && n !== "link" && n !== "move") throw new Error("Drop effect must be either none, copy, link, or move.");
//                this._dataTransfer.dropEffect = n
//            }, enumerable: !0, configurable: !0
//        }), Object.defineProperty(n.prototype, "effectAllowed",
//        {
//            get: function () { return this._dataTransfer.effectAllowed }, set: function (n) {
//                if (n !== "none" && n !== "copy" &&
//                n !== "copyLink" && n !== "copyMove" && n !== "link" && n !== "linkMove" && n !== "move" && n !== "all" && n !== "uninitialized")
//                    throw new Error("Effect allowed must be either none, copy, copyLink, copyMove, link, linkMove, move, all, or uninitialized.");
//                this._dataTransfer.effectAllowed = n
//            }, enumerable: !0, configurable: !0
//        }), Object.defineProperty(n.prototype,
//        "types", {
//            get: function () {
//                var n = this._dataTransfer.types, u, f, r, e, o; if (t === null && this.getData(""), t) {
//                    f =
//                    !1; r = this._getLegacyData(); for (o in r) if (r.hasOwnProperty(o) && r[o] !== "") { f = !0; break } return Object.keys(
//                    f ? r : i)
//                } if (n && !n.forEach) for (e = n, n = [], u = 0; u < e.length; u++) n.push(e[u]); return n
//            }, enumerable: !0, configurable:
//            !0
//        }), Object.defineProperty(n.prototype, "files", {
//            get: function () { return this._dataTransfer.files }, enumerable:
//            !0, configurable: !0
//        }), n.isLegacyDataTransfer = function () {
//            if (t === null) {
//                var n = r.DataTransfer || r.Clipboard;
//                return !n.prototype.setDragImage
//            } return t
//        }, n.prototype.setDragImage = function (n, t, i) {
//            this._dataTransfer.
//            setDragImage && this._dataTransfer.setDragImage(n, t, i)
//        }, n.prototype.addElement = function (n) {
//            this._dataTransfer.
//            addElement && this._dataTransfer.addElement(n)
//        }, n.prototype.getData = function (n) {
//            if (this._checkFormat(n),
//            t) return this._getLegacyDataTransfer(n); try { return t = !1, this._dataTransfer.getData(n) } catch (i) {
//                return t =
//                !0, this.getData(n)
//            }
//        }, n.prototype.setData = function (n, i) {
//            if (this._checkFormat(n), t) this._setLegacyDataTransfer(
//            n, i); else try { t = !1; this._dataTransfer.setData(n, i) } catch (r) { t = !0; this.setData(n, i) }
//        }, n.prototype.clearData =
//        function (n) {
//            if (this._checkFormat(n), t) this._clearLegacyDataTransfer(n); else try {
//                t = !1; this._dataTransfer.
//                clearData(n)
//            } catch (i) { t = !0; this.clearData(n) }
//        }, n.prototype._checkFormat = function (n) {
//            if (n && n.toLowerCase(
//            ) !== n) throw new Error("Format must be in lowercase.");
//        }, n.prototype._getLegacyData = function () {
//            var n =
//            this._dataTransfer.getData("Text"); return n && n.substr(0, this._prefixCode.length) === this._prefixCode ?
//            JSON.parse(n.substr(this._prefixCode.length)) : { Text: n }
//        }, n.prototype._stringifyLegacyData = function (n)
//        { return this._prefixCode + JSON.stringify(n) }, n.prototype._getLegacyDataTransfer = function (n) {
//            return this.
//            _getLegacyData()[n] || ""
//        }, n.prototype._setLegacyDataTransfer = function (n, t) {
//            var r = this._getLegacyData(
//            ); r[n] = t; i = r; this._dataTransfer.setData("Text", this._stringifyLegacyData(r))
//        }, n.prototype._clearLegacyDataTransfer =
//        function (n) {
//            if (i = {}, n) { var t = this._getLegacyData(); delete t[n]; i = t } this._dataTransfer.setData("Text",
//            this._stringifyLegacyData(i))
//        }, n
//    }(); n.DataTransfer2 = u; document.addEventListener("dragend", function ()
//    { i = {} }, !1)
//}(_azcPrivate || (_azcPrivate = {})), function (n) {
//    (function (n) {
//        "use strict"; function r() {
//            for (var
//            u, i = "", n, r = 0; r < 4; r++) n = 4294967296 * Math.random() | 0, i += t[n & 15] + t[n >> 4 & 15] + t[n >> 8 & 15] + t[n >> 12 & 15] + t[n >>
//            16 & 15] + t[n >> 20 & 15] + t[n >> 24 & 15] + t[n >> 28 & 15]; return u = t[8 + Math.random() * 4 | 0], i.substr(0, 8) + "-" + i.substr(
//            9, 4) + "-4" + i.substr(13, 3) + "-" + u + i.substr(16, 3) + "-" + i.substr(19, 12)
//        } function u(n, t) {
//            return n === undefined ||
//            t === undefined || n > t ? undefined : Math.floor(Math.random() * (t - n + 1)) + n
//        } var t = ["0", "1", "2", "3", "4", "5", "6",
//        "7", "8", "9", "A", "B", "C", "D", "E", "F"], i; n.blankGif = "data:image/gif;base64,R0lGODlhAQABAJEAAAAAAP///////wAAACH5BAEAAAIALAAAAAABAAEAAAICVAEAOw==";
//        n.newGuid = r; n.random = u; i = _azcPrivate.DataTransfer2; n.DataTransfer = i
//    })(n.Util || (n.Util = {})); var t = n.Util
//}
//(MsPortal || (MsPortal = {}))





//s.prototype._afterAddChild=function(n) {
//    this._callAnimateCallback(this.options.afterAdd, n);
//}
//s.prototype._callAnimateCallback=function(n,i){
//    var r;
//    t.isFunction(n) && (r=t(i).children()[0], r&&n(t(r)))
//},s}(i.Widget);

//n.Widget=s;u=function(){function n(){this._items=[]}return n.prototype.append=function(
//n){var t=this._getExistingItem(n);return!t?(this._insert(n),this._items):this._items},n.prototype.insert=
//function(n,t){var i=this._getExistingItem(n);return!i||this._items.splice(this._items.indexOf(i),1),
//this._insert(n,t),this._items},n.prototype.move=function(n,t){var i=this._getExistingItem(n),r=this.
//_items.indexOf(i);return r===-1?this._items:(this._items.splice(r,1),this._items.splice(t,0,i),this.
//_items)},n.prototype.remove=function(n){var i=this._getExistingItem(n),t;return(t=this._items.indexOf(
//i),t===-1)?this._items:(this._items.splice(t,1),this._items)},