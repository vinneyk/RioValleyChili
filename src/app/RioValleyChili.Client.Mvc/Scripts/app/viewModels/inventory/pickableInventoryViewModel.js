function PickableInventory(vals) {
    var me = this;

    var defaultVals = {
        quantityPicked: 0,
        quantityAvailable: 0,
        treatment: 'NA',
    };

    var values = $.extend(defaultVals, vals);

    var quantityAvailable = ko.observable(values.quantityAvailable);
    var quantityPicked = ko.observable(values.quantityPicked);

    var _validatedModel = ko.validatedObservable({
        quantityPicked: quantityPicked.extend({
            required: true,
            min: 1,
            max: values.quantityAvailable,
        }),
        remainingQuantityAvailable: ko.computed(function () {
            return quantityAvailable() - parseInt(quantityPicked());
        }).extend({ min: 0 }),
    });


    me.quantityPicked = _validatedModel().quantityPicked;
    me.lotNumber = values.lotNumber;
    me.product = values.product;
    me.treatment = values.treatment;
    me.productType = values.productType;
    me.productKey = values.productKey;

    me.quantityAvailable = quantityAvailable;
    me.location = values.location;
    me.locationKey = values.locationKey;
    me.warehouseKey = values.warehouseKey;
    me.warehouseName = values.warehouseName;
    
    me.packaging = values.packaging;
    me.packagingKey = ko.observable(values.packagingKey);
    me.packagingCapacity = values.packagingCapacity;

    me.destinationWarehouse = ko.observable(values.destinationWarehouse);
    me.destinationWarehouseLocation = ko.observable(values.destinationWarehouseLocation);

    me.inventoryKey = ko.observable(values.inventoryKey);
    me.focused = ko.observable(false);

    // computed properties
    me.remainingQuantityAvailable = _validatedModel().remainingQuantityAvailable;
    me.isValid = ko.computed(function () {
        return _validatedModel().isValid();
    }, me);
}

function PickableInventoryWithDestination(vals) {
    var model = new PickableInventory(vals);
    
    return model;
}