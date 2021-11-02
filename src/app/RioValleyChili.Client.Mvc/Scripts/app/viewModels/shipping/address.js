(function (ko, undefined) {
    this.Address = function(data) {
        var defaults = {
            AddressLine1: '',
            AddressLine2: '',
            AddressLine3: '',
            City: '',
            State: '',
            PostalCode: '',
            Country: '',
        };
        var self = {
            clear: clear,
            update: update,
        };

        init(data);
        return self;
        
        function init(data) {
            var vals = $.extend({}, defaults, data);
            ko.mapping.fromJS(vals, {}, self);

            self.CityStateZip = ko.computed(function () {
                var separater = ', ';
                if (self.City() == undefined || self.State() == undefined) {
                    separater = ' ';
                }
                return self.City() + separater + self.State() + ' ' + self.PostalCode();
            });
        }
        function clear() {
            self.AddressLine1(defaults.AddressLine1);
            self.AddressLine2(defaults.AddressLine2);
            self.AddressLine3(defaults.AddressLine3);
            self.City(defaults.City);
            self.State(defaults.State);
            self.PostalCode(defaults.PostalCode);
        }
        function update(newAddress) {
            newAddress = newAddress || {};
            self.AddressLine1(ko.utils.unwrapObservable(newAddress.AddressLine1));
            self.AddressLine2(ko.utils.unwrapObservable(newAddress.AddressLine2));
            self.AddressLine3(ko.utils.unwrapObservable(newAddress.AddressLine3));
            self.City(ko.utils.unwrapObservable(newAddress.City));
            self.State(ko.utils.unwrapObservable(newAddress.State));
            self.PostalCode(ko.utils.unwrapObservable(newAddress.PostalCode));
        }
    };
}(window.ko));