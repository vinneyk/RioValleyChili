function PickedInventoryItem(input) {
    if (!(this instanceof PickedInventoryItem)) return new PickedInventoryItem(input);

    // Init
    var self = this,
        input = ko.toJS(input || {});

    // Data
    self.ProductName = input.Product.ProductName;
    self.ProductKey = input.Product.ProductKey;
    self.ProductCodeAndName = input.Product.ProductCodeAndName;
    self.PackagingProductName = input.PackagingProduct.ProductName;
    self.PackagingProductKey = input.PackagingProduct.ProductKey;
    self.PickedInventoryItemKey = input.PickedInventoryItemKey;
    self.LocationKey = input.Location.LocationKey;
    self.LocationName = input.Location.Description;
    self.TreatmentKey = input.InventoryTreatment.TreatmentKey;
    self.TreatmentName = input.InventoryTreatment.TreatmentNameShort;
    self.LotNumber = input.LotKey;
    self.CustomerProductCode = ko.observable(input.CustomerProductCode);
    self.CustomerLotCode = ko.observable(input.CustomerLotCode);
    self.QuantityPicked = input.QuantityPicked;
    self.NetWeight = self.QuantityPicked * input.PackagingProduct.Weight;
    self.GrossWeight = self.NetWeight + input.PackagingProduct.PackagingWeight + input.PackagingProduct.PalletWeight;
}

module.exports = PickedInventoryItem;
