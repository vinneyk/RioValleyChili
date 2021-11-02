(function() {
    //if (require)
        define(['jquery', 'App/Scripts/utils/domGlue'], factory);
    //else factory(jQuery, domGlueWidget);

    function factory($, domGlueWidget) {
        $.StickyTableHeaders = function(el, options) {

            if (options.format) return reformatClone(el, options);
            if (options.refresh) return refreshClone(el, options);
            if (options.show) return setVisibility(el, true);
            if (options.hide) return setVisibility(el, false);

            var domGlue = getDomGlueWidget(el);
            if (domGlue) domGlue.cleanup.call(el);

            var self = this;
            self.options = $.extend({}, $.StickyTableHeaders.defaultOptions, options);
            //prepend element ID in attempt to prevent multiple instance with the same floating element id.
            if (el.id) { self.options.floatingElementId = el.id + '_' + self.options.floatingElementId; }
            self.options.onParentScroll = curryParentScrolledHandler($(el));
            self.options.formatClone = updateCloneLayout;

            self.domGlue = domGlueWidget.init(el, self.options);

            $(el).data('__stickyTableHeader', self);
        };

        $.StickyTableHeaders.defaultOptions = {
            fixedOffset: 0,
            offsetTop: 0,
            parent: window,
            floatingElementId: 'stickyTableHeader',
            target: 'thead:first',
            position: { top: 0, left: 0 }
        };

        $.fn.stickyTableHeaders = function (options) {
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
                (new $.StickyTableHeaders(this, optParam));
            });
        };
        
        function curryParentScrolledHandler($element) {
            return function (parent, target, clone) {
                var displayClone = displayFloatingClone(parent);
                var $parent = $(parent);

                var display = displayClone ? 'block' : 'hidden';

                $(clone).css({
                    'display': display,
                    'left': -1 * $parent.scrollLeft() + "px",
                });
            }

            function displayFloatingClone(parent) {
                return headerIsOffScreen(parent) && tableIsOnScreen();
            }

            function headerIsOffScreen(parent) {
                var $parent = $(parent);
                var parentPosition = $parent.offset() || { top: 0, left: 0 };
                var position = $element.offset() || { top: 0, left: 0 };
                var scrollTop = $parent.scrollTop();
                var $window = $(window);
                var parentIsWindow = $parent == $window;

                return scrollTop + parentPosition.top > position.top
                    || !parentIsWindow && $window.scrollTop() > parentPosition.top;
            }

            function tableIsOnScreen() {
                //return height > 0;
                return true;
            }
        }
        function updateCloneLayout(orig, clone) {
            // Copy cell widths and classes from original header
            $('th', clone).each(function (index) {
                var $this = $(this);
                var origCell = $('th', orig).eq(index);
                $this.removeClass().addClass(origCell.attr('class'));
                $this.css('width', origCell.width());
            });

            // Copy row width from whole table
            clone.css('width', orig.width());
        }

        function reformatClone(el) {
            var domGlue = getDomGlueWidget(el);
            if (!domGlue) return;

            var target = domGlue.getTargetElement();
            updateCloneLayout(target, domGlue.getCloneElement());
            domGlue.updateBoundaryPosition();
        }
        function refreshClone(el) {
            var domGlue = getDomGlueWidget(el);
            if (!domGlue) return;

            domGlue.rebuildClone();
            reformatClone(el);
        }
        function setVisibility(el, show) {
            var domGlue = getDomGlueWidget(el);
            if (!domGlue) return;
            var clone = domGlue.getCloneElement();
            if (!clone) return;
            if (show) clone.show();
            else cline.hide();
        }
        function cleanup() {
            var widget = getDomGlueWidget(this);
            widget && widget.cleanup();
            $.removeData(this, '__stickyTableHeader');
        }

        function getDomGlueWidget(el) {
            var widget = $(el).data('__stickyTableHeader');
            return widget && widget.domGlue || undefined;
        }
    }
}());