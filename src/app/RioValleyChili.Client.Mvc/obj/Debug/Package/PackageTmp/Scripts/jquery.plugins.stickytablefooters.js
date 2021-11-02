(function ($) {
    $.StickyTableFooters = function (table, options) {

        var self = this;

        self.$window = $(window);
        self.$inventoryTable = $(table);
        self.$originalFooter = null;
        self.$clonedFooter = null;

        self.$inventoryTable.data('StickyTableFooters', self);

        self.init = function () {

            $(".tableFloatingFooter").remove();

            self.$inventoryTable.each(function() {
                var $this = $(this);
                $this.css('padding', 0);

                if (!$(".divTableWithFloatingFooter").length) {
                    $this.wrap('<div class="divTableWithFloatingFooter"></div>');
                }

                self.$originalFooter = $("tfoot:first", this);
                self.$clonedFooter = self.$originalFooter.clone();

                self.$clonedFooter.addClass('tableFloatingFooter');
                self.$clonedFooter.css({
                    'position': 'fixed',
                    'bottom': 0,
                    'left': $this.css('margin-left'),
                    'display': 'none'
                });

                self.$originalFooter.addClass('tableFloatingFooterOriginal');
                self.$originalFooter.before(self.$clonedFooter);
            });


            self.updateTableFooters();
            self.$window.scroll(self.updateTableFooters);
            self.$window.resize(self.updateTableFooters);
        };

        self.updateTableFooters = function () {
            var scrollTop = self.$window.scrollTop();
            var scrollLeft = self.$window.scrollLeft();
            var windowBottom = self.$window.height() + scrollTop;
            
            self.$inventoryTable.each(function () {
                var $this = $(this);

                var offset = $this.offset();
                var tableHeight = $this.height();
                var tableBottom = offset.top + tableHeight;

                var isTableOnScreen = (offset.top < windowBottom && tableBottom > scrollTop);
                if (isTableOnScreen && tableBottom > windowBottom) {
                    // only update clone if the table footer is not already displayed
                    var isHidden = self.$clonedFooter.css('display') === 'none';
                    
                    if (isHidden) {
                        self.updateCloneFromOriginal();
                    }

                    var currentLeft = self.$clonedFooter.css('left');
                    var newLeft = offset.left - scrollLeft + "px";

                    if (isHidden || currentLeft != newLeft) {
                        self.$clonedFooter.css({
                            'bottom': 0,
                            'left': newLeft,
                            'display': 'block'
                        });
                    }
                } else {
                    self.$clonedFooter.css('display', 'none');
                }
            });
        };

        self.updateCloneFromOriginal = function () {
            // Copy cell widths and classes from original header
            $('td', self.$clonedFooter).each(function (index) {
                var $this = $(this);
                var origCell = $('td', self.$originalFooter).eq(index);
                $this.removeClass().addClass(origCell.attr('class'));
                $this.css('width', origCell.width());
            });

            // Copy row width from whole table
            self.$clonedFooter.css({
                'width': self.$originalFooter.width()
            });
        };


        // initialize state
        self.init();
    };

    $.fn.stickyTableFooters = function (options) {
        return this.each(function () {
            (new $.StickyTableFooters(this, options));
        });
    };

})(jQuery);