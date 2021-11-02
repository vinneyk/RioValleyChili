var warehouseService = require('services/warehouseService'),
    warehouseLocationsService = require('services/warehouseLocationsService');

require('App/koExtensions');

ko.validation.init({
    insertMessages: false,
    decorateInputElement: true,
    decorateElementOnModified: false,
    errorElementClass: 'has-error',
    errorMessageClass: 'help-block'
});

function PostCloseInventoryViewModel(params) {
    if (!(this instanceof PostCloseInventoryViewModel)) { return new PostCloseInventoryViewModel(params); }

    var self = this;

    // Imports
    self.pickedInventoryItems = params.pickedInventoryItems;
    self.orderKey = params.orderKey;
    self.destinationLocationOptions = params.destinationLocationOptions;
    self.requiresDestinationLocation = params.requiresDestinationLocation || false;
    self.validation = ko.validatedObservable();

    // Data
    self.mappedInventoryItems = ko.computed(function () {
        var i,
            isFirst = true,
            validation = ko.unwrap(self.requiresDestinationLocation) === true ? {} : null;

        var mapped = ko.utils.arrayMap(self.pickedInventoryItems() || [], function(item) {
            item.isFirstItem = isFirst;
            item.DestinationLocation = ko.observable().extend({ required: validation != undefined });
            if (validation) validation['item' + i] = item.DestinationLocation;
            if (isFirst) isFirst = false;
            return item;
        });
        self.validation(validation);
        return mapped;
    });

    // Behaviors
    self.isSingleItemList = ko.pureComputed(function () {
        return self.destinationLocationOptions().length === 1;
    });

    self.setDestinationForAllItemsCommand = ko.command({
        execute: function() {
            var destination = this.DestinationLocation();

            for (var i = self.pickedInventoryItems().length, list = self.pickedInventoryItems(); i--;) {
                list[i].DestinationLocation(destination);
            }
        },
        canExecute: function() {
            return true;
        }
    });

    self.setDestinationFromPreviousItemCommand = ko.command({
        execute: function() {
            var i = self.pickedInventoryItems().indexOf(this),
            copyDestination = self.pickedInventoryItems()[i-1].DestinationLocation();

            if (i > 0) {
                self.pickedInventoryItems()[i].DestinationLocation(copyDestination);
            }
        },
        canExecute: function() {
            return true;
        }
    });

    self.disposables = [ self.mappedInventoryItems ];

    // Exports
    params.exports({
        postAndCloseAsync: self.postAndCloseAsync.bind(self)
    });
}

PostCloseInventoryViewModel.prototype.postAndCloseAsync = function () {
    var self = this,
        values = { PickedItemDestinations: [] },
        key = ko.unwrap(self.orderKey);

    if (self.requiresDestinationLocation && !self.validation.isValid()) {
        self.validation.errors.showAllMessages();
        showUserMessage("Failed to Post", {
            description: 'Please fill in all required fields'
        });
        return $.Deferred().reject('Validation errors encountered.');
    }

    for (var i = 0, list = self.pickedInventoryItems(), len = list.length; i < len; i++) {
        values.PickedItemDestinations.push({
            "PickedInventoryItemKey": list[i].PickedInventoryItemKey,
            "DestinationLocationKey": list[i].DestinationLocation() ?
                list[i].DestinationLocation() : null
        });
    }

    return warehouseService.postAndCloseShipmentOrder(key, values);
}

PostCloseInventoryViewModel.prototype.dispose = function () {
    ko.utils.arrayForEach(this.disposables, this.disposeOne);
    ko.utils.objectForEach(this, this.disposeOne);
};

PostCloseInventoryViewModel.prototype.disposeOne = function(propOrValue, value) {
    var disposable = value || propOrValue;

    if (disposable && typeof disposable.dispose === "function") {
        disposable.dispose();
    }
};

module.exports = {
    viewModel: PostCloseInventoryViewModel,
    template: require('App/components/warehouse/post-and-close-inventory-order/post-and-close-inventory-order.html')
};
