var warehouseService = require('services/warehouseService');
require('App/helpers/koValidationHelpers');

//ko.validation.init({
//    insertMessages: false
//});

function EditWarehouseViewModel(params) {
    var self = this,
        values = params.input() || {};

    self.disposables = [];

    // Build cache for restoring values on cancel
    self.cache = ko.observable(values);

    // Initial bindings
    self.isNew = params.isNew;

    self.facilityInfo = {
        FacilityKey: ko.observable(values.FacilityKey),
        FacilityName: ko.observable(values.FacilityName).extend({ required: true }),
        PhoneNumber: ko.observable(values.PhoneNumber),
        EmailAddress: ko.observable(values.EmailAddress),
        Active: ko.observable(values.Active),
        Locations: ko.observableArray(values.Locations),
        Address: ko.observable(values.Address || {})
    };

    // Output values to push data upstream
    self.output = params.output;
    self.AddressOutput = ko.observable();
    self.LocationsOutput = ko.observable();

    // Values to validate
    self.errors = ko.validatedObservable({
        FacilityName: self.facilityInfo.FacilityName
    });

    // Behaviors
    function saveFacilityData() {
        // Package data for saving and sending upstream
        var preparedData = ko.toJS({
                FacilityKey: self.facilityInfo.FacilityKey,
                FacilityName: self.facilityInfo.FacilityName,

                Active: self.facilityInfo.Active,
                PhoneNumber: self.facilityInfo.PhoneNumber,
                EmailAddress: self.facilityInfo.EmailAddress,
                Address: self.AddressOutput,
                Locations: self.LocationsOutput
            });

        // Validate form
        if (self.errors.isValid()) {
            // Creates new facility if isNew is set, else updates the facility provided
            if (self.isNew()) {
                warehouseService.createWarehouse(preparedData).then(function (newKey) {
                    preparedData.FacilityKey = newKey;

                    self.output(preparedData)
                    self.isNew(false);
                }, function (xhr, status, message) {
                    showUserMessage("Failed to create the facility", { description: message });
                });
            } else {
                warehouseService.updateWarehouse(preparedData.FacilityKey, preparedData).then(function () {
                }, function (xhr, status, message) {
                    showUserMessage("Failed to update the facility", { description: message });
                });
                self.output(preparedData);
            }
            self.cache(preparedData);
        } else {
            showUserMessage("Failed to save facility data", { description: "Please fill in all required fields." });
        }
    }

    function cancelFacilityEdit() {
        // Restore previous values on cancel 
        // Hide editor if creating a new facility
        if (!self.isNew()) {
            self.facilityInfo.FacilityName(self.cache().FacilityName);
            self.facilityInfo.PhoneNumber(self.cache().PhoneNumber);
            self.facilityInfo.EmailAddress(self.cache().EmailAddress);
            self.facilityInfo.Active(self.cache().Active);
            self.facilityInfo.Address(self.cache().Address);
        } else {
            self.facilityInfo.FacilityName(undefined);
            self.facilityInfo.PhoneNumber(undefined);
            self.facilityInfo.EmailAddress(undefined);
            self.facilityInfo.Active(undefined);
            self.facilityInfo.Address({});
        }

        self.isNew(false);
    }

    // Manual subscriptions
    self.disposables.push(params.input.subscribe(function (input) {
        input = input || {};

        self.cache(input);

        self.facilityInfo.FacilityKey(input.FacilityKey);
        self.facilityInfo.FacilityName(input.FacilityName);
        self.facilityInfo.PhoneNumber(input.PhoneNumber);
        self.facilityInfo.EmailAddress(input.EmailAddress);
        self.facilityInfo.Active(input.Active);
        self.facilityInfo.Locations(input.Locations);
        self.facilityInfo.Address(input.Address || {});
    }));

    // Public properties and methods
    return {
        isNew: self.isNew,                      // Data   
        facilityInfo: self.facilityInfo,
        LocationsOutput: self.LocationsOutput,
        Address: self.Address,
        AddressOutput: self.AddressOutput,
        saveFacilityData: saveFacilityData,     // Behaviors
        cancelFacilityEdit: cancelFacilityEdit,
        output: self.output                     // Output
    }
}

ko.utils.extend(EditWarehouseViewModel, {
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

ko.components.register('address-editor', require('App/components/warehouse/address-editor/address-editor')); 

module.exports = { viewModel: EditWarehouseViewModel, template: require('./warehouse-editor.html')};
