var EditAddressViewModel = function (params) {
    var self = this,
        values = ko.toJS(params.input) || {};

    self.disposables = [];

    self.isLocked = params.locked || null;

    // Defines initial observables
    self.AddressLine1 = ko.observable(values.AddressLine1);
    self.AddressLine2 = ko.observable(values.AddressLine2);
    self.AddressLine3 = ko.observable(values.AddressLine3);
    self.City = ko.observable(values.City);
    self.State = ko.observable(values.State);
    self.PostalCode = ko.observable(values.PostalCode);
    self.Country = ko.observable(values.Country);

    // Updates observables when input changes
    self.disposables.push(params.input.subscribe(function (input) {
        values = ko.toJS(input) || {};

        self.AddressLine1(values.AddressLine1);
        self.AddressLine2(values.AddressLine2);
        self.AddressLine3(values.AddressLine3);
        self.City(values.City);
        self.State(values.State);
        self.PostalCode(values.PostalCode);
        self.Country(values.Country);
    }));

    // Output all values
    params.output(self);
};

ko.utils.extend(EditAddressViewModel, {
    dispose: function () {
        ko.utils.arrayForEach(this.disposables, this.disposeOne);
        ko.utils.objectForEach(this, this.disposeOne);
    },

    disposeOne: function(propOrValue, value) {
        var disposable = value || propOrValue;

        if (disposable && typeof disposable.dispose === "function") {
            disposable.dispose();
        }
    }
});

module.exports = { viewModel: EditAddressViewModel, template: require('./address-editor.html')};
