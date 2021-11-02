var ShippingLabelViewModel = (function (addressBase, esmHelper, undefined) {
    return {
        init: init
    };
    
    function init(data) {
        data = data || {};
        var self = new addressBase(data.Address);
        self.Description = ko.observable(data.Name);
        self.AdditionalInfo = ko.observable(data.AdditionalInfo);

        var esm = new esmHelper(self, { name: 'ShippingLabelViewModel' });
        
        // computed observables
        self.isEmpty = ko.computed(function () {
            var fields = [
                this.Name,
                this.AddressLine1,
                this.AddressLine2,
                this.AddressLine3,
                this.City,
                this.State,
                this.PostalCode,
                this.Country
            ];
            return !anyExisty(unwrapAll(fields));
        }, self);
        self.mode = ko.computed(function () {
            return this() ? 'editable' : 'readonly';
        }, esm.isEditing);

        // methods
        var baseClear = self.clear;
        var baseUpdate = self.update;
        self.clear = clear;
        self.update = update;
        self.toDto = getShippingLabelDto;

        return self;

        //private functions
        function clear() {
            self.Name(null);
            self.AdditionalInfo(null);
            baseClear();
        }
        function update(newLabel) {
            newLabel = newLabel || {};
            self.Description(ko.utils.unwrapObservable(newLabel.Name) || null);
            self.AdditionalInfo(ko.utils.unwrapObservable(newLabel.AdditionalInfo) || null);
            baseUpdate(newLabel.Address);
        }
        function getShippingLabelDto() {
            return {
                Name: ko.utils.unwrapObservable(this.Description),
                AdditionalInfo: ko.utils.unwrapObservable(this.AdditionalInfo),
                Address: getAddressDto.call(this),
            };

            function getAddressDto() {
                return {
                    AddressLine1: ko.utils.unwrapObservable(this.AddressLine1),
                    AddressLine2: ko.utils.unwrapObservable(this.AddressLine2),
                    AddressLine3: ko.utils.unwrapObservable(this.AddressLine3),
                    City: ko.utils.unwrapObservable(this.City),
                    State: ko.utils.unwrapObservable(this.State),
                    PostalCode: ko.utils.unwrapObservable(this.PostalCode),
                    Country: ko.utils.unwrapObservable(this.Country)
                };
            }
        }

        // helper functions
        function any(fnEval) {
            return function (input) {
                var first = ko.utils.arrayFirst(input, function (item) {
                    return fnEval.apply(null, [item]) === true;
                });
                return first != null;
            };
        }
        function existy(val) {
            return val != undefined && val.length > 0;
        }
        function anyExisty(input) {
            return any(existy)(input);
        }
        function unwrapAll(observables) {
            return ko.utils.arrayMap(
                ko.utils.unwrapObservable(observables),
                function (o) { return ko.utils.unwrapObservable(o); }
            );
        }
    }
}(Address, EsmHelper));