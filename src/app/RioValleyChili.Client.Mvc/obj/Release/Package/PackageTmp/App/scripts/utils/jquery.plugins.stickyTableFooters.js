(function () {
    //if (require)
        define(['jquery', 'App/Scripts/utils/domGlue'], factory);
    //else factory(jQuery, domGlueWidget);
    

    function factory($, domGlueWdiget) {
        $.StickyTableFooters = function(el, options) {
            if (options.format) reformatClone(el, options);
            else if (options.show) setVisibility(el, true);
            else if (options.hide) setVisibility(el, false);
            else {
                var self = this;
                var $el = $(el);

                $el.data('__stickyTableFooter', self);

                self.options = $.extend({}, $.StickyTableFooters.defaultOptions, options);
                self.options.onParentScroll = curryParentScrolledHandler(el);
                self.options.formatClone = formatClone;

                self.domGlue = domGlueWdiget.init(el, self.options);

            }

            return this;
        };

        $.StickyTableFooters.defaultOptions = {
            fixedOffset: 0,
            parent: window,
            floatingElementId: 'stickyTableFooter',
            target: 'tfoot:first',
            position: { bottom: 0, left: 0 }
        };

        $.fn.stickyTableFooters = function (options) {
            var optParam = options;
            if (typeof arguments[0] === "string") {
                var arg0 = arguments[0].toLowerCase();
                if (arg0 === "option") {
                    optParam = {};
                    optParam[arguments[1]] = arguments[2] || true;
                } else if (arg0 === "destroy") {
                    return cleanup.call(this);
                }
            }

            return this.each(function () {
                (new $.StickyTableFooters(this, optParam));
            });
        };

        function curryParentScrolledHandler(element) {
            var widget = $(element).data('__stickyTableFooter');

            return function (parent, target, clone) {
                var displayClone = displayFloatingClone(parent);
                var $parent = $(parent);

                var display = displayClone ? 'block' : 'hidden';

                $(clone).css({
                    'display': display,
                    'left': -1 * $parent.scrollLeft() + "px",
                    'bottom': widget.options.position.bottom,
                });
            }

            function displayFloatingClone(parent) {
                var $parent = $(parent);
                var display = $parent.css('display') !== 'none';

                return footerIsOffScreen(parent) && tableIsOnScreen();
            }

            function footerIsOffScreen(parent) {
                return true;
                //var parentPosition = $parent.offset() || { top: 0, left: 0 };
                //var position = $element.offset() || { top: 0, left: 0 };
                //var scrollTop = $parent.scrollTop();
                //var $window = $(window);
                //var parentIsWindow = $parent == $window;

                //return scrollTop + parentPosition.top > position.top
                //    || !parentIsWindow && $window.scrollTop() > parentPosition.top;
            }

            function tableIsOnScreen() {
                //return height > 0;
                return true;
            }
        }

        function formatClone(original, clone) {
            // Copy cell widths and classes from original header
            $('td', clone).each(function (index) {
                var $clonedTd = $(this);
                var $originalTd = $('td', original).eq(index);
                $clonedTd.removeClass().addClass($originalTd.attr('class'));
                $clonedTd.css('width', $originalTd.width());
            });

            // Copy row width from whole table
            clone.css('width', original.width());
        }

        function reformatClone(el) {
            var widget = $(el).data('__stickyTableFooter');
            var domGlue = widget.domGlue;
            formatClone(domGlue.$originalTarget, domGlue.$clonedTarget);
        }

        function setVisibility(el, options) {
            var widget = $(el).data('__stickyTableFooter');
            var domGlue = widget.domGlue;
            if (!domGlue.$clonedTarget) return;
            if (options) domGlue.$clonedTarget.show();
            else domGlue.$clonedTarget.hide();
        }

        function cleanup() {
            var widget = $(el).data('__stickyTableFooter');
            var domGlue = widget.domGlue;
            domGlue.destroy();
        }
    }
}());