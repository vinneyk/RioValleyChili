/* OBSOLETE! USE App/scripts/utils/jquery.plugins.stickyTableHeaders.js instead */

/*! Copyright (c) 2011 by Jonas Mosbech - https://github.com/jmosbech/StickyTableHeaders 
Adapted by Solutionhead Technologies, LLC 5/30/2012
MIT license info: https://github.com/jmosbech/StickyTableHeaders/blob/master/license.txt */

jQuery(function ($) {

    var _oldShow = $.fn.show;

    $.fn.show = function(speed, oldCallback) {
        return $(this).each(function() {
            var obj = $(this),
                newCallback = function() {
                    if ($.isFunction(oldCallback)) {
                        oldCallback.apply(obj);
                    }
                    obj.trigger('afterShow');
                };

            // you can trigger a before show if you want
            obj.trigger('beforeShow');

            // now use the old function to show the element passing the new callback
            _oldShow.apply(obj, [speed, newCallback]);
        });
    };
});

(function ($) {
    $.StickyTableHeaders = function (el, options) {
        var self = this;
        options = options || {};

        // Access to jQuery and DOM versions of element
        self.$window = $(window);
        self.$el = $(el);
        self.el = el;
        
        self.options = $.extend({}, $.StickyTableHeaders.defaultOptions, options);
        self.$parent = $(options.parent || window);
        self.$measuredElement = options.parent ? self.$parent : self.$el;
        self.$clonedHeader = null;
        self.$originalHeader = null;
        
        var parentIsWindow = options.parent == undefined;

        // Add a reverse reference to the DOM object
        self.$el.data('StickyTableHeaders', self);
        
        // functions
        self.init = init;
        
        self.updateTableHeaders = function () {
            var scrollTop = self.$parent.scrollTop();
            var scrollLeft = self.$parent.scrollLeft();
            var position = self.$measuredElement.offset();
            var parentPosition = self.$parent.offset() || { top: 0, left: 0 };
            
            var visibleHeight = parentIsWindow ? self.$el[0].clientHeight : self.$parent[0].clientHeight;
            visibleHeight = Math.max((visibleHeight + position.top), 0);
            
            var height = Math.min(self.$originalHeader.height(), visibleHeight);
            var width = self.$measuredElement[0].clientWidth;
            var top = parentIsWindow ? 0 : Math.max(position.top - $(window).scrollTop(), 0);
            
            self.$el.each(function () {
                if (tableNeedsHeader()) {
                    if (!isHeaderVisible()) self.updateCloneFromOriginal();
                    self.$stickyHeaderContainer.css({
                        'top': top,
                        'height': height,
                        'display': 'block',
                        'left': position.left,
                        'width': width,
                    });

                    self.$clonedHeader.css({
                        'left': -1 * scrollLeft + "px",
                    });
                } else {
                    self.$stickyHeaderContainer.css('display', 'none');
                }
            });
            
            function tableNeedsHeader() {
                return headerIsOffScreen() && tableIsOnScreen();
                
                function tableIsOnScreen() {
                    return height > 0;
                }

                function headerIsOffScreen() {
                    return scrollTop + parentPosition.top > position.top
                        || !parentIsWindow && $(window).scrollTop() > parentPosition.top;
                }
            }
            function isHeaderVisible() {
                return self.$stickyHeaderContainer.css('display') !== 'none';
            }
        };

        self.updateCloneFromOriginal = function () {
            // Copy cell widths and classes from original header
            $('th', self.$clonedHeader).each(function (index) {
                var $this = $(this);
                var origCell = $('th', self.$originalHeader).eq(index);
                $this.removeClass().addClass(origCell.attr('class'));
                $this.css('width', origCell.width());
            });

            // Copy row width from whole table
            self.$clonedHeader.css('width', self.$originalHeader.width());
        };


        // prevent processing of hidden tab element
        if (options.tabs) {
            $(options.tabs).tabs({
                activate: function (event, ui) {
                    if (ui.newPanel[0] == $(options.myTab)[0]) self.init();
                }
            });
            return;
        }


        self.init();
        

        function init() {
            $("tableFloatingHeader").remove();

            self.$el.parents().scroll(function () {
                self.updateTableHeaders();
            });

            self.$el.each(function () {
                var $this = $(this);

                // remove padding on <table> to fix issue #7
                $this.css('padding', 0);

                if (!$(".divTableWithFloatingFooter").length) {
                    $this.wrap('<div class="divTableWithFloatingHeader"></div>');
                }

                self.$originalHeader = $('thead:first', this);
                self.$stickyHeaderContainer = $("<div></div>");
                self.$clonedHeader = self.$originalHeader.clone();

                self.$clonedHeader.addClass('tableFloatingHeader');
                self.$stickyHeaderContainer.addClass('tableFloatingHeader');
                
                self.$clonedHeader.css({
                    'position': 'absolute',
                    'top': 0,
                    'left': 0,
                });

                var height;
                if (parentIsWindow) {
                    height = self.$parent.height();
                } else {
                    height = self.$parent[0].clientHeight;
                }


                var parentPosition = self.$parent.offset() || { top: 0, left: 0};
                var top = Math.max(parentPosition.top, 0);
                self.$stickyHeaderContainer.css({
                    'position': 'fixed',
                    'z-index': 25,
                    'top': top,
                    'left': parentPosition.left,
                    'display': 'none',
                    overflow: 'hidden',
                    'height': height,
                });
                
                self.$clonedHeader.appendTo(self.$stickyHeaderContainer);

                self.$originalHeader.addClass('tableFloatingHeaderOriginal');
                self.$originalHeader.before(self.$stickyHeaderContainer);
                
                // enabling support for jquery.tablesorter plugin
                // forward clicks on clone to original
                $('th', self.$clonedHeader).click(function () {
                    var index = $('th', self.$clonedHeader).index(this);
                    $('th', self.$originalHeader).eq(index).click();
                });
                $this.bind('sortEnd', self.updateCloneFromOriginal);
            });

            self.updateTableHeaders();
            self.$parent.scroll(self.updateTableHeaders);
            if (!parentIsWindow) self.$window.scroll(self.updateTableHeaders);
            self.$window.resize(function() {
                self.updateCloneFromOriginal();
                self.updateTableHeaders();
            });
        };
    };

    $.StickyTableHeaders.defaultOptions = {
        fixedOffset: 0,
        offsetTop: 0,
        parent: window,
    };

    $.fn.stickyTableHeaders = function (options) {
        return this.each(function () {
            (new $.StickyTableHeaders(this, options));
        });
    };

})(jQuery);
