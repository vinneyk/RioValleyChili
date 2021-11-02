var warehouseService = require('services/warehouseService');
    
var WarehouseLocationModel = function (input) {
    if (!(this instanceof WarehouseLocationModel)) { return new WarehouseLocationModel(input); }

    // Data initialization
    var self = this,
        isNewLocation = ('input' in input),
        locationData = isNewLocation ? (ko.toJS(input.input.facilityData)) : (ko.toJS(input) || {});

    self.disposables = [];

    self.isCreatingGroup = isNewLocation ? input.input.isCreatingGroup : false;
    self.statusCssClass = ko.observable();

    self.FacilityKey = ko.observable(locationData.FacilityKey);
    self.FacilityName = ko.observable(locationData.FacilityName);     // Ignored
    self.GroupName = ko.observable(('input' in input) ? input.input.selectedGroup : locationData.GroupName);
    self.LocationKey = ko.observable(locationData.LocationKey);
    self.Row = ko.observable(locationData.Row).extend({ required: true, insertMessages: false });
    self.Status = ko.observable(locationData.Status).extend({ required: true, locationStatusType: true })
    
    self.disposables.push(locationData);

    // Validation
    self.errors = ko.validatedObservable({
        Row: self.Row,
        Status: self.Status
    });

    // Boolean states
    self.isNew = ko.observable();
    self.disposables.push(ko.computed(function () {
        if (!ko.toJS(locationData.LocationKey)) {
            self.isNew(true);
        } else {
            self.isNew(false);
        }
    }));
    self.isEditing = ko.observable(self.isNew.peek());

    if (isNewLocation) {
        self.output = input.input.output;
        self.removeLocation = input.input.removeLocation;

        self.disposables.push(self.isEditing.subscribe(function (input) {
            if (self.isEditing && !input) {
                self.isCreatingGroup(false);
                self.removeLocation(self);
            }
        }));

        self.disposables.push(input.input.isCreatingGroup.subscribe(function (input) {
            if (!input && self.isEditing) {
                self.removeLocation(self);
            }
        }));
    }
};

WarehouseLocationModel.prototype.statusIcon = function () {
    var self = this;

    self.disposables.push(ko.computed(function () {
        var status = self.Status();

        switch(status) {
            case "1":
                self.statusCssClass("fa-circle-o");
                break;
            case "2":
                self.statusCssClass("fa-lock");
                break;
            case "3":
                self.statusCssClass("fa-times-circle");
                break;
            default:
                break;
        }
    }));

    return self.statusCssClass;
}

WarehouseLocationModel.prototype.saveLocationData = function (locationData) {
    var self = this,
        ws = warehouseService,
        data = locationData;

    // Run validation check and initiate save if validation passes
    if (!this.errors.isValid()) {
        showUserMessage("Failed to save location data", { description: "Please fill in all required fields" });
    } else {
        if (this.isNew()) {
            ws.createWarehouseLocation(locationData).then(function (value) {
                self.LocationKey(value);
                data.LocationKey = value;

                self.isNew(false);
                self.isEditing(false);

                if (self.isCreatingGroup) {
                    self.isCreatingGroup(false);
                }
            }, function (xhr, status, message) {
                showUserMessage("Failed to create location", { description: message });
            }).done(function() {
                if (self.LocationKey()) {
                    self.output(data);
                }
            });
        } else {
            ws.updateWarehouseLocation(self.LocationKey(), locationData).then(function(value) {
                self.isEditing(false);
            }, function(xhr, status, message) {
                showUserMessage("Failed to update location", { description: message });
            });
        }
    }
}

WarehouseLocationModel.prototype.editLocation = function () { 
    // Cache data before editing
    this.cachedName = this.Row();
    this.cachedStatus = this.Status();

    // Enters editing mode
    this.isEditing(true);
};

WarehouseLocationModel.prototype.saveLocation = function () {
    // Package data for saving
    var preparedData = ko.toJS({
        FacilityKey: this.FacilityKey,
        FacilityName: this.FacilityName,
        LocationKey: this.LocationKey,
        GroupName: ko.toJS(this.GroupName), 
        LocationKey: this.LocationKey,
        Row: this.Row,
        Status: this.Status
    });

    // Initiates save
    this.saveLocationData(preparedData);
}

WarehouseLocationModel.prototype.undoEdit = function () {
    this.Row(this.cachedName);
    this.Status(this.cachedStatus);

    this.isEditing(false);
};

WarehouseLocationModel.prototype.removeNewLocation = function () {
    this.removeLocation(this);
    this.isCreatingGroup(false);
};

ko.utils.extend(WarehouseLocationModel, {
    dispose: function () {
        ko.utils.arrayForEach(this.disposables, this.disposeOne)
        ko.utils.objectForEach(this, this.disposeOne);
    },

    disposeOne: function(propOrValue, value) {
        var disposable = value || propOrValue;

        if (disposable && typeof disposable.dispose === "function") {
            disposable.dispose();
        }
    }
});

module.exports = WarehouseLocationModel;