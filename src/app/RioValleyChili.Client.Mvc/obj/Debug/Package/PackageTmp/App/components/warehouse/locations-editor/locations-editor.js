var warehouseService = require('services/warehouseService'),
    WarehouseLocationFactory = require('App/models/WarehouseLocationModel');

var EditLocationsViewModel = function (params) {
    var self = this,
        disposables = [],
        groupData = {},
        newLocationData = {},
        values = ko.toJS(params.input.Locations) || [];

    // Initialize data structure
    self.Locations = ko.observableArray();
    self.filteredLocations = ko.observableArray();

    self.Groups = ko.observableArray();
    self.isCreatingGroup = ko.observable(false);
    self.newGroupName = ko.observable();
    self.selectedGroup = ko.observable();
    self.locationOutput = ko.observable();

    // Behaviors
    function mapLocations(locationsArray) {
        self.Locations([]);
        self.Groups([]);

        self.Locations(ko.utils.arrayMap(locationsArray || [], WarehouseLocationFactory));
        mapGroupNames(self.Locations());
    }

    function buildFilteredLocationsList(targetGroup) {
        var allLocations = ko.toJS(self.Locations),
            filteredList = self.Locations().filter(function (location) {
                return location.GroupName() === targetGroup;
            });

        self.filteredLocations(filteredList.sort(caseInsensitiveSort));
    }

    function caseInsensitiveSort(a, b) {
        var a = typeof a === 'string' ? a.toLowerCase() : undefined,
            b = typeof b === 'string' ? b.toLowerCase() : undefined;

        return a > b ? 1 : (a < b ? -1 : 0);
    }

    function mapGroupNames(locations) {
        var i,
            j,
            currentItem,
            availableGroupNames = [];


        for (i = locations.length; i--;) {
            currentItem = locations[i].GroupName();
            if (availableGroupNames.indexOf(currentItem) === -1) {
                availableGroupNames.push(currentItem);
            }
        }
        self.Groups(availableGroupNames.sort(caseInsensitiveSort));
    }

    function createGroup() {
        self.selectedGroup(undefined);
        self.isCreatingGroup(true);
        addLocation();
    }

    function cancelCreateGroup() {
        self.isCreatingGroup(false);
    }

    function addLocation() {
        var newLocation = new WarehouseLocationFactory({
            input: newLocationData,
        });

        self.filteredLocations.push(newLocation);
    }

    function removeLocation(location) {
        self.Locations.remove(location);
        self.filteredLocations.remove(location);
    }

    function freezeGroup() {
        var previousGroupSelection = self.selectedGroup();

        warehouseService.freezeFacilityLocationsGroup(params.facilityInfo.FacilityKey(), self.selectedGroup()).then(function (locationsArray) {
            mapLocations(locationsArray);
        }).done(function () {
            self.selectedGroup(previousGroupSelection);
        });
    }

    function unfreezeGroup() {
        var previousGroupSelection = self.selectedGroup();

        warehouseService.unfreezeFacilityLocationsGroup(params.facilityInfo.FacilityKey(), self.selectedGroup()).then(function (locationsArray) {
            mapLocations(locationsArray);
        }).done(function () {
            self.selectedGroup(previousGroupSelection);
        });
    }

    // Bundled values to export
    newLocationData = {
        facilityData: params.facilityInfo,
        selectedGroup: ko.pureComputed(function () {
            return self.isCreatingGroup() ? self.newGroupName : self.selectedGroup;
        }),
        isCreatingGroup: self.isCreatingGroup,
        output: self.locationOutput,
        removeLocation: removeLocation
    };

    // Manual subscriptions
    disposables.push(params.input.Locations.subscribe(function (input) {
        self.isCreatingGroup(false);
        mapLocations(input);
    }));

    disposables.push(self.selectedGroup.subscribe(function (currentGroup) {
        buildFilteredLocationsList(currentGroup);
    }));

    disposables.push(self.isCreatingGroup.subscribe(function (input) {
        if (!input) {
            mapLocations(self.Locations());
        } else {
            self.newGroupName('');
            self.filteredLocations([]);
        }
    }));

    disposables.push(self.locationOutput.subscribe(function (input) {
        var previousGroupSelection = ko.toJS(self.newGroupName);

        self.filteredLocations([]);
        self.Groups([]);
        self.Locations.push(input);
        mapLocations(self.Locations());

        self.selectedGroup(previousGroupSelection);
    }));

    // Computed values
    disposables.push(newLocationData.selectedGroup);

    // Execute essential functions
    if (values) {
        mapLocations(values);
    }

    // Export this instance to the output parameter
    params.output(self);

    // Public methods and properties
    return {
        filteredLocations: self.filteredLocations,  // Data
        Groups: self.Groups,
        selectedGroup: self.selectedGroup,
        isCreatingGroup: self.isCreatingGroup,
        newGroupName: self.newGroupName,
        createGroup: createGroup,                   // Behaviors
        cancelCreateGroup: cancelCreateGroup,
        freezeGroup: freezeGroup,
        unfreezeGroup: unfreezeGroup,
        addLocation: addLocation,
        removeLocation: removeLocation
    }
};

ko.utils.extend(EditLocationsViewModel, {
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

module.exports = { viewModel: EditLocationsViewModel, template: require('./locations-editor.html') };