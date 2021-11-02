var warehouseService = require('services/warehouseService');
require('App/koExtensions');

var WarehouseLocationsViewModel = (function () {
    if (!(this instanceof WarehouseLocationsViewModel)) { return new WarehouseLocationViewModel(); }

    var self = this,
        ws = warehouseService;

    self.disposables = [];

    // Data
    self.facilities = ko.observableArray([]);
    self.warehouseSelected = ko.observable();
    self.warehouseDetails = ko.observable();
    self.outputDetails = ko.observable();

    // Editor
    self.isNew = ko.observable(false);
    self.showEditor = ko.computed(function () {
        return self.isNew() || self.warehouseDetails() !== undefined;
    });
    self.disposables.push(self.showEditor);

    // Behaviors
    function buildFacilityList(key) {
        ws.getWarehouses().then(function (data) {
            self.facilities(data);
            selectFacility(key ? key : undefined);
            }, function (xhr, status, message) {
                showUserMessage("Failed to load warehouses", { description: message });
            });
    }

    function selectFacility(key) {
        self.warehouseSelected(key);
    }

    function getFacilityDetails(key) {
        if (key) {
            self.isNew(false);
            ws.getWarehouseDetails(self.warehouseSelected()).then(function (values) {
                self.warehouseDetails(values);
            }, function () {
                showUserMessage('Failed to retrieve facility details');
            });
        } else {
            self.isNew(true);
            self.warehouseSelected(undefined);
            self.warehouseDetails(undefined);
        }
    }

    function showNewFacilityEditor(input) {
        getFacilityDetails();
    }

    // Manual subscriptions to update on value change
    self.disposables.push(self.warehouseSelected.subscribe(function (input) {
        getFacilityDetails(input);
    }));

    self.disposables.push(self.outputDetails.subscribe(function (values) {
        // When outputDetails updates, update drop down navigation appropriately
        var isNewFacility = true;

        function sortByFacilityName(a, b) {
            return a.FacilityName > b.FacilityName ? 1 : (a.FacilityName < b.FacilityName ? -1 : 0);
        }

        updatedDetails = {
            FacilityKey: values.FacilityKey,
            FacilityName: values.FacilityName
        };

        for (var i = 0, targetKey = values.FacilityKey, len = self.facilities().length; i < len; i++) {
            if (self.facilities()[i].FacilityKey === targetKey) {
                self.facilities.splice(i, 1, updatedDetails);
                isNewFacility = false;
                self.facilities.sort(sortByFacilityName);
                self.warehouseSelected(updatedDetails.FacilityKey);
                break;
            }
        }

        if (isNewFacility) {
            self.facilities.push(updatedDetails);
            self.facilities.sort(sortByFacilityName);
            self.warehouseSelected(updatedDetails.FacilityKey);
        }
    }));
    
    // Execute essential functions
    self.facilities(buildFacilityList());

    // Public Methods and Properties
    return {
        facilities: self.facilities,
        showNewFacilityEditor: showNewFacilityEditor,
        isNew: self.isNew,
        showEditor: self.showEditor,
        warehouseSelected: self.warehouseSelected,
        warehouseDetails: self.warehouseDetails,
        outputDetails: self.outputDetails
    }
});

ko.utils.extend(WarehouseLocationsViewModel, {
    dispose: function () {
        ko.utils.arrayForEach(this.disposables, this.disposeOne);
    },

    disposeOne: function(propOrValue, value) {
        var disposable = value || propOrValue;

        if (disposable && typeof disposable.dispose === "function") {
            disposable.dispose();
        }
    }
});

ko.components.register('warehouse-editor', require('App/components/warehouse/warehouse-editor/warehouse-editor')); 
ko.components.register('locations-editor', require('App/components/warehouse/locations-editor/locations-editor')); 

var vm = new WarehouseLocationsViewModel();

ko.applyBindings(vm);
