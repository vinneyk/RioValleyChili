/*! Copyright (c) 2012 by Solutionhead Technologies, LLC */

(function ($) {
    $.sh = $.sh || {};


    $.extend($.sh, {

    });

    //    $.sh = function () {
    //    };

    $.shPopup = function (el, options) {
        // To avoid scope issues, use 'base' instead of 'this'
        // to reference this class from internal events and functions.
        var base = this;

        // Access to jQuery and DOM versions of element
        base.$el = $(el);

        // Cache DOM refs for performance reasons
        base.$window = $(window);
        base.$popupTrigger = null;
        base.$popupWindow = null;
        base.isTriggerVisible = false;
        base.isPopupDisplayed = false;

        // if > 0 the trigger element will be displayed only when the scrollTop > the value
        base.scrollPositionTarget = 0;

        base.init = function () {
            base.options = $.extend({}, $.shPopup.defaultOptions, options);

            if (base.$el == null) {
                $.error("Popup window can not be null.");
            }

            if (base.options.trigger == null) {
                $.error('The \'trigger\' option is required.');
                return false;
            }

            if (!$.isFunction(base.options.showFunction)) {
                base.options.showFunction = $.shPopup.defaultOptions.showFunction;
            }

            if (!$.isFunction(base.options.hideFunction)) {
                base.options.hideFunction = $.shPopup.defaultOptions.hideFunction;
            }


            //set scrollPositionTarget
            var scrollPositionValue = parseInt(parseInt(base.options.scrollPositionTarget));
            if (!isNaN(scrollPositionValue) && scrollPositionValue > 0) {
                base.scrollPositionTarget = base.options.scrollPositionTarget;
            }
            else if (base.options.scrollPositionTargetElement != null) {
                var $element = $(base.options.scrollPositionTargetElement);
                base.scrollPositionTarget = $element.offset().top + $element.height();
            }

            base.$el.each(function () {
                var $this = $(this);

                // create close button is needed
                if (base.options.closeButton == null) {
                    $this.append("<a href=\"#\" class=\"close\">close</a>").addClass("close");
                    var closeButton = $(".close", $this).first();
                    base.options.closeButton = closeButton;
                }

                $this.wrapAll('<div id="popupWindow-container"></div>');
                base.$popupWindow = $("#popupWindow-container");
                base.hidePopup();

                base.$popupTrigger = base.options.trigger;
                base.$popupTrigger.hide();
                base.$popupTrigger.click(function () {
                    base.showPopup();
                });
            });

            if (base.options.closeButton != null) {
                $(base.options.closeButton).each(function () {
                    $(this).click(function () {
                        base.hidePopup();
                    });
                });
            }

            if (!isNaN(base.scrollPositionTarget) && base.scrollPositionTarget > 0) {
                base.$window.scroll(base.updateTrigger);
                base.$window.resize(base.updateTrigger);
            }
        };

        base.updateTrigger = function () {
            base.$el.each(function () {
                if (base.isTriggerVisible) {
                    if (base.$window.scrollTop() < base.scrollPositionTarget) {
                        base.$popupTrigger.slideUp();
                        base.isTriggerVisible = false;
                    }
                } else {
                    if (base.$window.scrollTop() >= base.scrollPositionTarget) {
                        base.$popupTrigger.hide().slideDown();
                        base.isTriggerVisible = true;
                    }
                }
            });
        };

        base.showPopup = function () {
            if (base.options.beforeDisplay != null && $.isFunction(base.options.beforeDisplay)) {
                base.options.beforeDisplay();
            }

            base.options.showFunction(base.$popupWindow);
            if (base.scrollPositionTarget > 0) { base.$popupTrigger.hide(); }
            base.isPopupDisplayed = true;
        };

        base.hidePopup = function () {
            //todo: something is running after this function which is causing the page to scroll back to the top. this is not the desired outcome.
            base.options.hideFunction(base.$popupWindow);
            base.isPopupDisplayed = false;
            
            if (base.options.afterDisplay != null && $.isFunction(base.options.afterDisplay)) {
                base.options.afterDisplay();
            }
        };

        // Run initializer
        base.init();
    };

    $.shPopup.defaultOptions = {
        trigger: null,
        scrollPositionTarget: 0, // determines the scrollTop value at which the trigger element will be displayed
        scrollPositionTargetElement: null, // enables the use of an element to determine at which the trigger will be displayed
        closeButton: null,
        showFunction: function (popup) { $(popup).show(); },
        hideFunction: function (popup) { $(popup).hide(); },
        beforeDisplay: null,
        afterDisplay: null
    };

    $.fn.shPopup = function (options) {
        return this.each(function () {
            (new $.shPopup(this, options));
        });
    };
})(jQuery);
