var PickedInventoryItemFactory = require('App/models/PickedInventoryItemModel');

function PickedInventoryItemsViewModel (params) {
    // Init
    var self = this;
    
    self.disposables = [];

    self.isLocked = params.locked || null;
    self.isInit = ko.observable(false);

    // Data
    self.PickedInventoryItems = ko.pureComputed(function () {
        var data = ko.unwrap(params.input) || {};
        return ko.utils.arrayMap(ko.unwrap(data.PickedInventoryItems) || [], PickedInventoryItemFactory);
    });
    self.TotalQuantityPicked = ko.observable();
    self.TotalNetWeightOfPickedItems = ko.observable();
    self.TotalGrossWeightOfPickedItems = ko.observable();

    self.isInit(true);

    var calculationsWatcher = ko.computed(function () {
        if (self.PickedInventoryItems()) {
            var totalQuantity = 0,
                totalNetWeight = 0,
                totalGrossWeight = 0;

            ko.utils.arrayForEach(self.PickedInventoryItems(), function (item) {
                totalQuantity += Number(item.QuantityPicked);
                totalNetWeight += Number(item.NetWeight);
                totalGrossWeight += Number(item.GrossWeight);
            });

            self.TotalQuantityPicked(totalQuantity);
            self.TotalNetWeightOfPickedItems(totalNetWeight);
            self.TotalGrossWeightOfPickedItems(totalGrossWeight);
        }
    });
    self.disposables.push(calculationsWatcher);
    
    function toDto() {
        return ko.toJS(self.PickedInventoryItems);
    }
    
    // Exports
    params.exports({
        isInit: self.isInit,
        PickedItems: self.PickedInventoryItems,
        TotalQuantityPicked: self.TotalQuantityPicked,
        TotalNetWeightOfPickedItems: self.TotalNetWeightOfPickedItems,
        TotalGrossWeightOfPickedItems: self.TotalGrossWeightOfPickedItems,
        toDto: toDto
    });
}

ko.utils.extend(PickedInventoryItemsViewModel, {
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

module.exports = {
    viewModel: PickedInventoryItemsViewModel,
    template: require('./picked-inventory-items.html')
};
