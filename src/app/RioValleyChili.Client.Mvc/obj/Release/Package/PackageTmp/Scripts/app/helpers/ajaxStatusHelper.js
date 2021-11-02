//todo: replace usage with App/helpers/koHelpers.ajaxStatusHelper

var ajaxStatusHelper = (function () {
    var self = {
        init: init,
    };

    var ajaxStatus = {
        success: 2,
        failure: -1,
        working: 1,
        none: 0,
    };

    function init(target) {
        if (target == undefined) throw new Error("Target cannot be undefined.");

        target.ajaxStatus = ko.observable(ajaxStatus.none);
        target.indicateSuccess = success;
        target.indicateWorking = working;
        target.indicateFailure = failure;
        target.clearStatus = clear;

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
        target.ajaxInactive = ko.computed(function() {
            return this.ajaxStatus() === ajaxStatus.none;
        }, target);

        return target;

        // functions
        function clear() {
            target.ajaxStatus(ajaxStatus.none);
        }
        function success() {
            target.ajaxStatus(ajaxStatus.success);
        }
        function working() {
            target.ajaxStatus(ajaxStatus.working);
        }
        function failure() {
            target.ajaxStatus(ajaxStatus.failure);
        }
    }

    return self;
}());