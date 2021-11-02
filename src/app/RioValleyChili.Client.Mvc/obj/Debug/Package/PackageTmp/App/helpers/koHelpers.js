define(['jquery', 'ko', 'Scripts/knockout.editStateManager'], function ($, ko) {
    var ajaxStatus = {
        success: 2,
        failure: -1,
        working: 1,
        none: 0,
    };

    setupKoEditStateManager();

    return {
        ajaxStatusHelper: initAjaxStatusHelper,
        animateNewListElement: animateNewListElement,
        esmHelper: initEsmHelper,
        getDataForClickedElement: function (options) {
            options = options || {};
            options.isDesiredTarget = options.isDesiredTarget || function () { return true; }

            var data = options.clickArguments[0];

            if (options.isDesiredTarget(data)) return data;
            if (options.clickArguments.length < 2) throw new Error("Incorrect number of arguments for click handler.");

            var targetElement = options.clickArguments[1].originalEvent.target;

            if (!targetElement) throw new Error("Target element could not be determined.");


            var context = ko.contextFor(targetElement);
            if (context && options.isDesiredTarget(context.$data)) {
                return context.$data;
            }

            throw new Error("Unable to identify data for target element.");
        },
    };
   

    //#region ajaxStatusHelper
    function initAjaxStatusHelper(target) {
        if (target == undefined) throw new Error("Target cannot be undefined.");

        target.ajaxStatus = ko.observable(ajaxStatus.none);
        target.indicateSuccess = success.bind(target);
        target.indicateWorking = working.bind(target);
        target.indicateFailure = failure.bind(target);
        target.clearStatus = clear.bind(target);

        // computed properties
        target.ajaxSuccess = ko.computed(function () {
            return this.ajaxStatus() === ajaxStatus.success;
        }, target);
        target.ajaxFailure = ko.computed(function () {
            return this.ajaxStatus() === ajaxStatus.failure;
        }, target);
        target.ajaxWorking = ko.computed(function () {
            return this.ajaxStatus() === ajaxStatus.working;
        }, target);
        target.ajaxInactive = ko.computed(function () {
            return this.ajaxStatus() === ajaxStatus.none;
        }, target);

        
        return target;
    }
    
    // functions
    function clear() {
        this.ajaxStatus(ajaxStatus.none);
    }
    function success() {
        this.ajaxStatus(ajaxStatus.success);
    }
    function working() {
        this.ajaxStatus(ajaxStatus.working);
    }
    function failure() {
        this.ajaxStatus(ajaxStatus.failure);
    }
    //#endregion ajaxStatusHelper

    //#region animateNewListItem
    function animateNewListElement(options) {
        options = options || {};
        options.paddingTop = options.paddingTop == undefined ? 120 : options.paddingTop;
        return function(elem) {
            if (elem.nodeType === 1) {
                var $elem = $(elem);
                var origBg = $elem.css('background-color');


                if (doScroll()) {
                    var maxHeightContainer = $('.maxHeight-container');
                    var floatingHeader = maxHeightContainer.find('.tableFloatingHeader');
                    if (floatingHeader) {
                        options.paddingTop = floatingHeader.height() + 100; // the 100 shouldn't be necessary but without it, the scrollTop goes off screen...
                    }

                    if (maxHeightContainer) {
                        maxHeightContainer.animate({
                            scrollTop: (maxHeightContainer.scrollTop() + $elem.position().top) - options.paddingTop // need to allow for floating headers
                        }, 2000);
                    } else {
                        $('html, body').animate({
                            scrollTop: $elem.offset().top - 100 // need to allow for floating headers
                        }, 2000);
                    }

                    if (options.afterScrollCallback) options.afterScrollCallback();
                }

                $elem.css('opacity', 0);
                $elem.animate({ backgroundColor: "#a6dbed", opacity: 1 }, 800)
                    .delay(2500)
                    .animate({ backgroundColor: origBg || 'rgb(255, 255, 255)' }, 1000, function() {
                        $elem.css('background-color', ''); // remove style from element to allow css to regain control
                    });
            };
        };

        function doScroll() {
            var scrollOption = options.scrollToItem;
            return ko && ko.isObservable(scrollOption)
                ? scrollOption()
                : scrollOption;
        }
    }
    //#endregion animateNewListItem

    //#region Edit State Manager
    function initEsmHelper(objectToTrack, options) {
        if (!objectToTrack) throw new Error("Must provide an objectToTrack.");
        return setup(options);

        function setup() {
            var esm = ko.EditStateManager(objectToTrack, options);
            var propertiesToCopy = ['toggleEditingCommand', 'beginEditingCommand', 'endEditingCommand', 'revertEditsCommand', 'cancelEditsCommand', 'saveEditsCommand', 'isEditing', 'hasChanges'];
            for (var prop in propertiesToCopy) {
                if (propertiesToCopy.hasOwnProperty(prop)) {
                    var propName = propertiesToCopy[prop];
                    objectToTrack[propName] = esm[propName];
                }
            }
            return esm;
        }
    }
    function setupKoEditStateManager() {
        ko.EditStateManager.defaultOptions = (function () {
            var defaultOptions = {
                include: [],
                ignore: ['__ko_mapping__'],
                initializeAsEditing: false,
                isInitiallyDirty: false,
                canSave: function () { return true; },
                name: "[unnamed_esm]",
                canEdit: function () { return true; },
                canEndEditing: function () { return true; },
            };

            return defaultOptions;
        })();
    }
    //#endregion
});
