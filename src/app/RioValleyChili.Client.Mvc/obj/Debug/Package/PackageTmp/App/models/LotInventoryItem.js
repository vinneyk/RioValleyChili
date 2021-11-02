function LotInventoryItem ( values, checkOutOfRange ) {
    if (!(this instanceof LotInventoryItem)) { return new LotInventoryItem( values, checkOutOfRange ); }

    var lotSummaryFactory = require('App/models/LotSummary');

    this.disposables = [];

    var base = new lotSummaryFactory( values, checkOutOfRange );
    var lot = this;

    for (var prop in base) {
        if (base.hasOwnProperty(prop))
            lot[prop] = base[prop];
    }

    lot.InventoryKey = values.InventoryKey;
    lot.ToteKey = values.ToteKey;
    lot.QuantityOnHand = values.Quantity;
    lot.PackagingDescription = values.PackagingDescription || values.PackagingProduct.ProductName;
    lot.PackagingProductKey = values.PackagingProductKey || values.PackagingProduct.ProductKey;
    lot.PackagingCapacity = values.PackagingProduct?values.PackagingProduct.Weight : values.PackagingCapacity;
    lot.ReceivedPackagingName = values.ReceivedPackagingName;
    lot.LocationKey = values.LocationKey || values.Location.LocationKey;
    lot.LocationName = values.LocationName || values.Location.Description;
    lot.WarehouseName = values.WarehouseName || values.Location.FacilityName;
    lot.WarehouseKey = values.WarehouseKey || values.Location.FacilityKey;
    lot.TotalWeightOnHand = (values.PackagingProduct ? values.PackagingProduct.Weight : values.PackagingCapacity) * values.Quantity;
    lot.Notes = values.Notes;
    lot.CustomerProductCode = values.CustomerProductCode;
    lot.CustomerLotCode = values.CustomerLotCode;

    return lot;
}

LotInventoryItem.prototype.dispose = function () {
    ko.utils.arrayForEach(this.disposables, this.disposeOne);
    ko.utils.objectForEach(this, this.disposeOne);
};

LotInventoryItem.prototype.disposeOne = function(propOrValue, value) {
    var disposable = value || propOrValue;

    if (disposable && typeof disposable.dispose === "function") {
        disposable.dispose();
    }
};



module.exports = LotInventoryItem;
