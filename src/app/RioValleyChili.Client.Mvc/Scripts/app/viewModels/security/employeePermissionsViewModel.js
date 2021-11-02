var employeePermissionsViewModel = (function(api) {
    return {
        init: init
    };

    function init(data) {
        var self = {
            employeeKey: data.EmployeeKey,
            claims: ko.observableArray(parseClaims(data.Claims, mapClaim)),

            // methods
            savePermissions: saveClaims,
            toDto: buildDto
        };

        // commands
        self.saveCommand = ko.composableCommand({
            execute: function(complete) {
                saveClaims({
                    successCallback: function() {
                        showUserMessage("Claims saved successfully");
                    },
                    errorCallback: function() {
                        showUserMessage("Unable to save claims");
                    },
                    completeCallback: complete
                });
            },
            canExecute: function(isExecuting) {
                return !isExecuting; // todo: add validation
            },
            //shouldExecute: esm.hasChanges,
        });
        self.removeClaimCommand = ko.composableCommand({
            execute: function(claim, complete) {
                var index = ko.utils.arrayIndexOf(self.claims(), claim);
                if (index > -1) {
                    self.claims.splice(index, 1);
                }
                complete();
            },
            canExecute: function (isExecuting) { return !isExecuting; },
        });

        //init
        addNewClaim();
        ko.applyBindings(self);


        // private functions
        function buildDto() {
            var claims = ko.utils.arrayFilter(self.claims(), function(claim) {
                return claim.hasValue();
            });
            return ko.utils.arrayMap(claims, function(p) {
                return {
                    Key: p.type(),
                    Value: p.value(),
                };
            });
        }
        function saveClaims(options) {
            api.security.setEmployeeClaims(self.employeeKey, self.toDto(), options);
        }
        function addNewClaim() {
            self.claims.push(mapClaim());
        }
        function mapClaim(input) {
            var claim = new Claim(input);
            claim.isLastItem = isLastItem.bind(claim);
            registerClaimSubscriptions(claim);
            return claim;

            function isLastItem() {
                return self.claims.indexOf(this) === self.claims().length - 1;
            }
        }
        function registerClaimSubscriptions(claim)
        {
            claim.type.subscribe(claimTypeSubscriptionCallback);
            function claimTypeSubscriptionCallback(value) {
                if (value && claim.isLastItem()) {
                    addNewClaim();
                }
            }

        }
    }
    function parseClaims(claims, mapFn) {
        var claimsArray = [];
        for (var claim in claims) {
            claimsArray.push(mapFn({
                Key: claim,
                Value: claims[claim]
            }));
        }
        return claimsArray;
    }

    
    // types
    function Claim(input) {
        input = input || {};
        var model = {
            type: ko.observable(input.Key),
            value: ko.observable(input.Value),

            // methods
            hasValue: isValueRequired,
        };

        model.value.extend({ required: { onlyIf: isValueRequired } });

        return model;

        function isValueRequired() {
            return model.type() && model.type().length;
        }
    }
}(api))