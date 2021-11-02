var ResultStateHelper = (function () {

    return {
        resultStatus: {
            success: 1,
            error: -1,
            none: 0,
        },
        init: function (target) {
            if (target == undefined) throw new Error("Target is undefined.");

            var results = new Results();
            target.pushError = results.pushError;
            target.pushSuccess = results.pushSuccess;
            target.hasErrors = results.hasErrors;
            target.clearResults = results.clear;
            target.results = {
                success: results.success,
                status: results.status,
                errors: results.errors,
                messages: ko.computed(results.results),
            };
            target.getAllErrors = getErrorsFromTarget;

            function getErrorsFromTarget(cmd) {
                cmd = cmd || target;
                var targetErrors = cmd.results.errors() || [];
                ko.utils.arrayMap(cmd.modulesToExecute(), function (module) {
                    targetErrors = targetErrors.concat(getErrorsFromTarget(module));
                });
                return targetErrors;
            }
        }
    };

    function Results() {
        var _results = ko.observableArray([]);

        var result = {
            results: ko.computed(function () { return _results(); }),
            pushError: function (message) {
                pushResult(ResultStateHelper.resultStatus.error, message);
            },
            pushSuccess: function (message) {
                pushResult(ResultStateHelper.resultStatus.success, message);
            },
            clear: function () {
                _results([]);
            },
            errors: ko.computed(function () {
                return ko.utils.arrayFilter(_results(), function (msg) {
                    return msg.state === ResultStateHelper.resultStatus.error;
                });
            }),
        };

        result.hasErrors = ko.computed(function () {
            return this.errors().length > 0;
        }, result);
        result.status = ko.computed(function () {
            return this.hasErrors()
                ? ResultStateHelper.resultStatus.error
                : ResultStateHelper.resultStatus.success;
        }, result);
        result.success = ko.computed(function () {
            return !this.hasErrors();
        }, result);
        
        return result;

        function pushResult(state, message) {
            _results.push({
                state: state,
                message: message,
                success: state === ResultStateHelper.resultStatus.success,
                error: state === ResultStateHelper.resultStatus.error,
            });
        }
    };
}());