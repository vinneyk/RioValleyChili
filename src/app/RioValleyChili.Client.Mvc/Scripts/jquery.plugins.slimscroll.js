/*! Copyright (c) 2011 Piotr Rochala (http://rocha.la)
 * Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php)
 * and GPL (http://www.opensource.org/licenses/gpl-license.php) licenses.
 *
 * Version: 0.2.5
 * 
 */
//(function ($) {

//    jQuery.fn.extend({
//        slimScroll: function (o) {

//            var plane = function (top, right, bottom, left)
//            {
//                this.top = Math.min(top, bottom);
//                this.right = Math.max(right, left);
//                this.bottom = Math.max(top, bottom);
//                this.left = Math.min(right, left);

//                this.height = bottom - top;
//                this.width = right - left;

//                this.alert = function() {
//                    alert("top: " + top + "\n" + "right: " + right + "\n" + "bottom: " + bottom + "\n" + "left: " + left);
//                };
//            };

//            var ops = o;

//            var isOverPanel, isOverBar, isDragg, queueHide, barHeight;
//            var divS = '<div></div>';
//            var minBarHeight = 30;
//            var wheelStep = 30;
//            var o = ops || { };
//            var size = o.size || '7px';
//            var color = o.color || '#000';
//            var position = o.position || 'right';
//            var opacity = o.opacity || .95;
//            var railOpacity = o.railOpacity || .8;
//            var verticalPadding = o.verticalPadding || 10;
//            var horizontalPadding = o.horizontalPadding || 3;
//            var alwaysVisible = o.alwaysVisible === true;
//            var width = parseInt(size.replace('px', ''));
            
//            this.each(function () {

//                var me = $(this)
//                    .css({ overflow: 'hidden' })
//                    .hover(function () {
//                    isOverPanel = true;
//                    showBar({ overElement: me });
//                }, function () {
//                    isOverPanel = false;
//                    if (!isDragg) {
//                        hideBar();
//                    }
//                });
                
//                var isFixed = me.css('position') === 'fixed';
                
//                var offset;
//                var scrollContentArea, scrollbarArea;
//                var wrapper, rail, bar;
                
//                var barFocus = function () {
//                    showBar({ overElement: bar });
//                    rail.fadeIn(200);
//                };

//                var _onWheel = function (e) {
//                    //use mouse wheel only when mouse is over
//                    if (!isOverPanel) {
//                        return;
//                    }

//                    var e = e || window.event;

//                    var delta = 0;
//                    if (e.wheelDelta) {
//                        delta = -e.wheelDelta / 120;
//                    }
//                    if (e.detail) {
//                        delta = e.detail / 3;
//                    }

//                    //scroll content
//                    scrollContent(0, delta, true);

//                    //stop window scroll
//                    if (e.preventDefault) {
//                        e.preventDefault();
//                    }
//                    e.returnValue = false;
//                };

//                var scrollContent = function (x, y, isWheel) {
//                    var delta = y;

//                    rail.fadeIn(100);
//                    showBar({ overElement: bar });

//                    if (isWheel) {
//                        //move bar with mouse wheel
//                        delta = bar.position().top + y * wheelStep;

//                        //move bar, make sure it doesn't go out
//                        delta = Math.max(delta, 0);
//                        var maxTop = me.outerHeight() - bar.outerHeight() - (verticalPadding * 2);
//                        delta = Math.min(delta, maxTop);

//                        //scroll the scrollbar
//                        bar.css({ top: delta + 'px' });
//                    }

//                    var percentScroll = parseInt(bar.position().top) / ((me.outerHeight() - (verticalPadding * 2)) - bar.outerHeight());
//                    delta = percentScroll * (me[0].scrollHeight - me.outerHeight());

//                    me.scrollTop(delta);
//                };
                
//                var showBar = function (args) {
//                    clearTimeout(queueHide);

//                    //show only when required
//                    if (barHeight >= me.outerHeight()) {
//                        return;
//                    }

//                    var barOpacity = args.overElement == bar ? opacity : Math.max(opacity / 2, .4);
//                    bar.css({ opacity: barOpacity });
//                    bar.fadeIn('fast');
//                };

//                var hideBar = function () {
//                    if (!alwaysVisible && !isDragg) {
//                        queueHide = setTimeout(function () {
//                            if (!isOverBar && !isDragg) {
//                                bar.fadeOut('slow');
//                                rail.hide();
//                            }
//                        }, 500);
//                    }
//                };

//                var hideRail = function () {
//                    if (!alwaysVisible && !isDragg) {
//                        rail.fadeOut(400);
//                    }
//                };
                
//                function initialize() {
//                    if(bar != null) bar.remove();
//                    if(rail != null) rail.remove();
//                    if(wrapper != null) wrapper.remove();
                    
//                    measure();
//                    wrapper = buildWrapper();
//                    rail = createRail();
//                    bar = createScrollbar();
                    
//                    bar.draggable({
//                        axis: 'y',
//                        containment: 'parent',
//                        start: function() { isDragg = true; },
//                        stop: function() {
//                            isDragg = false;
//                            if (!isOverBar) { hideRail(); }
//                            if (!isOverPanel) {
//                                hideBar();
//                            } else {
//                                showBar({ overElement: me });
//                            }
//                        },
//                        drag: function(e) {
//                            scrollContent(0, $(me).position().top, false);
//                        }
//                    }).hover(function() {
//                        isOverBar = true;
//                        barFocus();
//                    }, function() {
//                        isOverBar = false;
//                        hideRail();
//                    });

//                    me.wrap(wrapper);
//                    me.parent().append(bar);
//                    me.parent().append(rail);
                    
//                    attachMouseWheel();
//                }
                
//                function measure() {
//                    offset = me.offset();
//                    barHeight = Math.max((me.outerHeight() / me[0].scrollHeight) * me.outerHeight(), minBarHeight);
                    
//                    scrollContentArea = new plane(
//                        offset.top,
//                        offset.left + me.outerWidth(),
//                        offset.top + me.outerHeight(),
//                        offset.left
//                    );

//                    var sbaRight = position === 'right' ? scrollContentArea.right - horizontalPadding : scrollContentArea.left + width + horizontalPadding;
//                    var sbaLeft = position === 'right' ? scrollContentArea.right - width - horizontalPadding : scrollContentArea.left + horizontalPadding;

//                    scrollbarArea = new plane(
//                        scrollContentArea.top + verticalPadding,
//                        sbaRight,
//                        scrollContentArea.bottom - verticalPadding,
//                        sbaLeft
//                    );
//                }

//                function buildWrapper() {
//                    return $(divS).css({
//                        position: isFixed ? 'fixed' : 'absolute',
//                        width: scrollbarArea.width,
//                        height: scrollbarArea.height,
//                        top: scrollbarArea.top,
//                        bottom: scrollbarArea.bottom,
//                        left: scrollbarArea.left,
//                        right: scrollbarArea.right,
//                        zIndex: 15000,
//                    }).attr({ 'class': 'slimScrollDiv' });
//                }

//                function createRail() {
//                    return $(divS).attr({
//                        style: 'border-radius: ' + size
//                    }).css({
//                        width: scrollbarArea.width,
//                        height: scrollbarArea.height,
//                        position: 'fixed',
//                        background: '#ccc',
//                        display: alwaysVisible ? 'block' : 'none',
//                        top: scrollbarArea.top,
//                        bottom: scrollbarArea.bottom,
//                        right: scrollbarArea.right,
//                        left: scrollbarArea.left,
//                        opacity: railOpacity,
//                        zIndex: 15000
//                    });
//                }
                
//                function createScrollbar() {
//                    var barPosCss = (position == 'right') ? { right: '0' } : { left: '0' };
                    
//                    return $(divS).attr({
//                        'class': 'slimScrollBar',
//                        style: 'border-radius: ' + size
//                    })
//                    .css({
//                        background: color,
//                        width: size,
//                        position: 'absolute',
//                        top: 0,
//                        opacity: opacity,
//                        display: alwaysVisible ? 'block' : 'none',
//                        MozBorderRadius: size,
//                        WebkitBorderRadius: size,
//                        BorderRadius: size,
//                        zIndex: 15001
//                    })
//                    .css({ height: barHeight + 'px' })
//                    .css(barPosCss);
//                }

//                function attachMouseWheel() {
//                    if (window.addEventListener) {
//                        this.addEventListener('DOMMouseScroll', _onWheel, false);
//                        this.addEventListener('mousewheel', _onWheel, false);
//                    } else {
//                        document.attachEvent("onmousewheel", _onWheel)
//                    }
//                };
                
                
//                $(window).resize(function () {
//                    initialize();
//                });

//                // don't initialize until the page has been loaded.
//                // this is in response to an issue where the page was loading

//                me.change(function () {
//                    initialize();
//                });

//                $(function () {
//                    //alert(me.html());
//                    initialize();
//                });
//            });
            

//            return this;
//        }
        

//    });

    
//    jQuery.fn.extend({
//        slimscroll: jQuery.fn.slimScroll
//    });

//})(jQuery);

// Reece: Updating the slimscroll fixed the issue of having
//        to click a button twice when an input form is focused in #CVP.
//        Also fixes some tabindexing issues when tabbing through inputs in #CVP.

/*! Copyright (c) 2011 Piotr Rochala (http://rocha.la)
 * Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php)
 * and GPL (http://www.opensource.org/licenses/gpl-license.php) licenses.
 *
 * Version: 1.2.0
 *
 */
(function ($) {

    jQuery.fn.extend({
        slimScroll: function (options) {

            var defaults = {

                // width in pixels of the visible scroll area
                width: 'auto',

                // height in pixels of the visible scroll area
                height: '250px',

                // width in pixels of the scrollbar and rail
                size: '7px',

                // scrollbar color, accepts any hex/color value
                color: '#000',

                // scrollbar position - left/right
                position: 'right',

                // distance in pixels between the side edge and the scrollbar
                distance: '1px',

                // default scroll position on load - top / bottom / $('selector')
                start: 'top',

                // sets scrollbar opacity
                opacity: .4,

                // enables always-on mode for the scrollbar
                alwaysVisible: false,

                // check if we should hide the scrollbar when user is hovering over
                disableFadeOut: false,

                // sets visibility of the rail
                railVisible: false,

                // sets rail color
                railColor: '#333',

                // sets rail opacity
                railOpacity: .2,

                // whether  we should use jQuery UI Draggable to enable bar dragging
                railDraggable: true,

                // defautlt CSS class of the slimscroll rail
                railClass: 'slimScrollRail',

                // defautlt CSS class of the slimscroll bar
                barClass: 'slimScrollBar',

                // defautlt CSS class of the slimscroll wrapper
                wrapperClass: 'slimScrollDiv',

                // check if mousewheel should scroll the window if we reach top/bottom
                allowPageScroll: false,

                // scroll amount applied to each mouse wheel step
                wheelStep: 20,

                // scroll amount applied when user is using gestures
                touchScrollStep: 200
            };

            var o = $.extend(defaults, options);

            // do it for every element that matches selector
            this.each(function () {

                var isOverPanel, isOverBar, isDragg, queueHide, touchDif,
                  barHeight, percentScroll, lastScroll,
                  divS = '<div></div>',
                  minBarHeight = 30,
                  releaseScroll = false;

                // used in event handlers and for better minification
                var me = $(this);

                // ensure we are not binding it again
                if (me.parent().hasClass(o.wrapperClass)) {
                    // start from last bar position
                    var offset = me.scrollTop();

                    // find bar and rail
                    bar = me.parent().find('.' + o.barClass);
                    rail = me.parent().find('.' + o.railClass);

                    getBarHeight();

                    // check if we should scroll existing instance
                    if ($.isPlainObject(options)) {
                        // Pass height: auto to an existing slimscroll object to force a resize after contents have changed
                        if ('height' in options && options.height == 'auto') {
                            me.parent().css('height', 'auto');
                            me.css('height', 'auto');
                            var height = me.parent().parent().height();
                            me.parent().css('height', height);
                            me.css('height', height);
                        }

                        if ('scrollTo' in options) {
                            // jump to a static point
                            offset = parseInt(o.scrollTo);
                        }
                        else if ('scrollBy' in options) {
                            // jump by value pixels
                            offset += parseInt(o.scrollBy);
                        }
                        else if ('destroy' in options) {
                            // remove slimscroll elements
                            bar.remove();
                            rail.remove();
                            me.unwrap();
                            return;
                        }

                        // scroll content by the given offset
                        scrollContent(offset, false, true);
                    }

                    return;
                }

                // optionally set height to the parent's height
                o.height = (o.height == 'auto') ? me.parent().height() : o.height;

                // wrap content
                var wrapper = $(divS)
                  .addClass(o.wrapperClass)
                  .css({
                      // Reece: this fixes visibility issues
                      //position: 'relative',
                      overflow: 'hidden',
                      width: o.width,
                      height: o.height
                  });

                // update style for the div
                me.css({
                    overflow: 'hidden',
                    width: o.width,
                    height: o.height
                });

                // create scrollbar rail
                var rail = $(divS)
                  .addClass(o.railClass)
                  .css({
                      width: o.size,
                      height: '100%',
                      position: 'absolute',
                      top: 0,
                      display: (o.alwaysVisible && o.railVisible) ? 'block' : 'none',
                      'border-radius': o.size,
                      background: o.railColor,
                      opacity: o.railOpacity,
                      zIndex: 15000
                  });

                // create scrollbar
                var bar = $(divS)
                  .addClass(o.barClass)
                  .css({
                      background: o.color,
                      width: o.size,
                      position: 'absolute',
                      top: 0,
                      opacity: o.opacity,
                      display: o.alwaysVisible ? 'block' : 'none',
                      'border-radius': o.size,
                      BorderRadius: o.size,
                      MozBorderRadius: o.size,
                      WebkitBorderRadius: o.size,
                      zIndex: 15001
                  });

                // set position
                var posCss = (o.position == 'right') ? { right: o.distance } : { left: o.distance };
                rail.css(posCss);
                bar.css(posCss);

                // wrap it
                me.wrap(wrapper);

                // append to parent div
                me.parent().append(bar);
                me.parent().append(rail);

                // make it draggable
                if (o.railDraggable && $.ui && typeof ($.ui.draggable) == 'function') {
                    bar.draggable({
                        axis: 'y',
                        containment: 'parent',
                        start: function () { isDragg = true; },
                        stop: function () { isDragg = false; hideBar(); },
                        drag: function (e) {
                            // scroll content
                            scrollContent(0, $(this).position().top, false);
                        }
                    });
                }

                // on rail over
                rail.hover(function () {
                    showBar();
                }, function () {
                    hideBar();
                });

                // on bar over
                bar.hover(function () {
                    isOverBar = true;
                }, function () {
                    isOverBar = false;
                });

                // show on parent mouseover
                me.hover(function () {
                    isOverPanel = true;
                    showBar();
                    hideBar();
                }, function () {
                    isOverPanel = false;
                    hideBar();
                });

                // support for mobile
                me.bind('touchstart', function (e, b) {
                    if (e.originalEvent.touches.length) {
                        // record where touch started
                        touchDif = e.originalEvent.touches[0].pageY;
                    }
                });

                me.bind('touchmove', function (e) {
                    // prevent scrolling the page
                    e.originalEvent.preventDefault();
                    if (e.originalEvent.touches.length) {
                        // see how far user swiped
                        var diff = (touchDif - e.originalEvent.touches[0].pageY) / o.touchScrollStep;
                        // scroll content
                        scrollContent(diff, true);
                    }
                });

                // check start position
                if (o.start === 'bottom') {
                    // scroll content to bottom
                    bar.css({ top: me.outerHeight() - bar.outerHeight() });
                    scrollContent(0, true);
                }
                else if (o.start !== 'top') {
                    // assume jQuery selector
                    scrollContent($(o.start).position().top, null, true);

                    // make sure bar stays hidden
                    if (!o.alwaysVisible) { bar.hide(); }
                }

                // attach scroll events
                attachWheel();

                // set up initial height
                getBarHeight();

                function _onWheel(e) {
                    // use mouse wheel only when mouse is over
                    if (!isOverPanel) { return; }

                    var e = e || window.event;

                    var delta = 0;
                    if (e.wheelDelta) { delta = -e.wheelDelta / 120; }
                    if (e.detail) { delta = e.detail / 3; }

                    var target = e.target || e.srcTarget || e.srcElement;
                    if ($(target).closest('.' + o.wrapperClass).is(me.parent())) {
                        // scroll content
                        scrollContent(delta, true);
                    }

                    // stop window scroll
                    if (e.preventDefault && !releaseScroll) { e.preventDefault(); }
                    if (!releaseScroll) { e.returnValue = false; }
                }

                function scrollContent(y, isWheel, isJump) {
                    var delta = y;
                    var maxTop = me.outerHeight() - bar.outerHeight();

                    if (isWheel) {
                        // move bar with mouse wheel
                        delta = parseInt(bar.css('top')) + y * parseInt(o.wheelStep) / 100 * bar.outerHeight();

                        // move bar, make sure it doesn't go out
                        delta = Math.min(Math.max(delta, 0), maxTop);

                        // if scrolling down, make sure a fractional change to the
                        // scroll position isn't rounded away when the scrollbar's CSS is set
                        // this flooring of delta would happened automatically when
                        // bar.css is set below, but we floor here for clarity
                        delta = (y > 0) ? Math.ceil(delta) : Math.floor(delta);

                        // scroll the scrollbar
                        bar.css({ top: delta + 'px' });
                    }

                    // calculate actual scroll amount
                    percentScroll = parseInt(bar.css('top')) / (me.outerHeight() - bar.outerHeight());
                    delta = percentScroll * (me[0].scrollHeight - me.outerHeight());

                    if (isJump) {
                        delta = y;
                        var offsetTop = delta / me[0].scrollHeight * me.outerHeight();
                        offsetTop = Math.min(Math.max(offsetTop, 0), maxTop);
                        bar.css({ top: offsetTop + 'px' });
                    }

                    // scroll content
                    me.scrollTop(delta);

                    // fire scrolling event
                    me.trigger('slimscrolling', ~~delta);

                    // ensure bar is visible
                    showBar();

                    // trigger hide when scroll is stopped
                    hideBar();
                }

                function attachWheel() {
                    if (window.addEventListener) {
                        this.addEventListener('DOMMouseScroll', _onWheel, false);
                        this.addEventListener('mousewheel', _onWheel, false);
                    }
                    else {
                        document.attachEvent("onmousewheel", _onWheel)
                    }
                }

                function getBarHeight() {
                    // calculate scrollbar height and make sure it is not too small
                    barHeight = Math.max((me.outerHeight() / me[0].scrollHeight) * me.outerHeight(), minBarHeight);
                    bar.css({ height: barHeight + 'px' });

                    // hide scrollbar if content is not long enough
                    var display = barHeight == me.outerHeight() ? 'none' : 'block';
                    bar.css({ display: display });
                }

                function showBar() {
                    // recalculate bar height
                    getBarHeight();
                    clearTimeout(queueHide);

                    // when bar reached top or bottom
                    if (percentScroll == ~~percentScroll) {
                        //release wheel
                        releaseScroll = o.allowPageScroll;

                        // publish approporiate event
                        if (lastScroll != percentScroll) {
                            var msg = (~~percentScroll == 0) ? 'top' : 'bottom';
                            me.trigger('slimscroll', msg);
                        }
                    }
                    else {
                        releaseScroll = false;
                    }
                    lastScroll = percentScroll;

                    // show only when required
                    if (barHeight >= me.outerHeight()) {
                        //allow window scroll
                        releaseScroll = true;
                        return;
                    }
                    bar.stop(true, true).fadeIn('fast');
                    if (o.railVisible) { rail.stop(true, true).fadeIn('fast'); }
                }

                function hideBar() {
                    // only hide when options allow it
                    if (!o.alwaysVisible) {
                        queueHide = setTimeout(function () {
                            if (!(o.disableFadeOut && isOverPanel) && !isOverBar && !isDragg) {
                                bar.fadeOut('slow');
                                rail.fadeOut('slow');
                            }
                        }, 1000);
                    }
                }

            });

            // maintain chainability
            return this;
        }
    });

    jQuery.fn.extend({
        slimscroll: jQuery.fn.slimScroll
    });

})(jQuery);