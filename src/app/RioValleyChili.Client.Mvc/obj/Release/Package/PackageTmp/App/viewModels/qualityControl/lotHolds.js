var LotHold;

define(['ko', 'services/lotService'], function (ko, service) {
    LotHold = lotHoldFactory(ko);
    return { init: init };

    function init(target) {
        if (!target.LotKey) throw new Error("LotKey property is required.");

        var self = target;

        self.LotHold = ko.observable();
        self.AntiForgeryToken = ko.observable();
        self.displayHoldView = ko.observable(false);

        self.openHoldView = function () {
            self.LotHold(new LotHold(target));
            self.displayHoldView(true);
        };
        self.closeHoldView = function () {
            self.LotHold(null);
            self.displayHoldView(false);
        };
        self.removeHoldCommand = ko.asyncCommand({
            execute: function (complete) {
                service.removeLotHold(ko.utils.unwrapObservable(self.LotKey), {
                    successCallback: function () {
                        complete();
                        updateTarget({});
                        self.displayHoldView(false);
                        showUserMessage("Hold Removed Successfully");
                    },
                    errorCallback: function (xhr, status, message) {
                        complete();
                        showUserMessage("Unable to remove lot hold.", { description: message });
                    },
                });
            },
            canExecute: function (isExecuting) {
                return !isExecuting;
            }
        });
        self.saveHoldCommand = ko.asyncCommand({
            execute: function (complete) {
                var data = self.LotHold();
                var results = ko.validation.group(data);
                if (results() && results().length) {
                    complete();
                    return;
                }

                service.setLotHold(ko.utils.unwrapObservable(self.LotKey), data.toDto(), {
                    successCallback: function () {
                        complete();
                        updateTarget(data);
                        self.displayHoldView(false);
                    },
                    errorCallback: function () {

                    }
                });
            },
            canExecute: function (isExecuting) {
                return !isExecuting && !!self.LotHold();
            }
        });

        function updateTarget(hold) {
            if (ko.isObservable(target.HoldType)) {
                target.HoldType(ko.utils.unwrapObservable(hold.HoldType));
            }
            if (ko.isObservable(target.HoldDescription)) {
                target.HoldDescription(ko.utils.unwrapObservable(hold.HoldDescription));
            }
        }

        return self;
    }
    function lotHoldFactory(ko) {
        return function LotHold(input) {
            var values = input || {};
            if (!(this instanceof arguments.callee)) return new LotHold(values);

            this.HoldType = ko.observable(ko.utils.unwrapObservable(values.HoldType)).extend({
                required: true,
                lotHoldType: true,
            });
            this.HoldDescription = ko.observable(ko.utils.unwrapObservable(values.HoldDescription)).extend({ required: true });
            this.toDto = buildDto.bind(this);

            return this;

            function buildDto() {
                return {
                    HoldType: this.HoldType(),
                    Description: this.HoldDescription(),
                };
            }
        }
    }
});