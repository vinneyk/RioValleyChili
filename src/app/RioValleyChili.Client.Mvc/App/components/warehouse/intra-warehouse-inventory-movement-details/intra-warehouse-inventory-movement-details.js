var inventoryPickOrderComponent = require('components/inventory/picked-inventory-items-with-destination/picked-inventory-items-with-destination');
ko.components.register('picked-inventory-items-with-destination', inventoryPickOrderComponent);
require('App/helpers/koValidationHelpers');

function IntraWarehouseInventoryMovementViewModel(params) {
    if (!(this instanceof IntraWarehouseInventoryMovementViewModel)) return new IntraWarehouseInventoryMovementViewModel(params);

    // Init
    var model = this,
        config = ko.unwrap(params) || {},
        values = ko.unwrap(config.input) || {};

    // Initial validation
    if (!ko.isObservable(params.input)) throw new Error("Missing or invalid parameter: inputValue. Requires a observable.");
    if (!ko.isWritableObservable(params.exports)) throw new Error("Missing or invalid parameter: value. Requires a writable observable.");
    if (!config.warehouseLocationOptions || !ko.isObservable(config.warehouseLocationOptions)) throw new Error("Missing or invalid parameter from the initial config object: warehouseLocationOptions. Expected an observableArray object.");
    if (!config.inventoryProductOptions || !ko.isObservable(config.inventoryProductOptions)) throw new Error("Missing or invalid parameter from the initial config object: inventoryProductOptions. Expected an observableArray object.");
    if (!config.packagingProductOptions || !ko.isObservable(config.packagingProductOptions)) throw new Error("Missing or invalid parameter from the initial config object: packagingProductOptions. Expected an observableArray object.");

    // Data initialization
    model.warehouseLocationOptions = config.warehouseLocationOptions;
    model.inventoryProductOptions = config.inventoryProductOptions;
    model.packagingProductOptions = config.packagingProductOptions;

    model.IntraWarehouseOrderKey = ko.observable(values.OrderKey);
    model.TrackingSheetNumber = ko.observable(values.TrackingSheetNumber).extend({ required: true });
    model.OperatorName = ko.observable(values.OperatorName).extend({ required: true });
    model.MovementDate = ko.observableDate(config.input ? values.DateCreated : Date.now(), 'm/d/yyyy').extend({ required: true });

    model.PickedInventoryDetails = ko.observable(values.PickedInventoryDetail);
    model.PickedInventoryVm = ko.observable();

    var trackingNumberCache = '';

    this.isNewMovement = ko.pureComputed(function () {
        return model.IntraWarehouseOrderKey() == undefined;
    }, model);

    model.validation = ko.validatedObservable({
        TrackingSheetNumber: model.TrackingSheetNumber,
        OperatorName: model.OperatorName,
        MovementDate: model.MovementDate
    });

    // Manual subscriptions
    params.input.subscribe(function (input) {
        input = input || {};

        model.IntraWarehouseOrderKey(input.OrderKey);
        model.TrackingSheetNumber(input.TrackingSheetNumber);
        model.OperatorName(input.OperatorName);
        model.MovementDate(config.input ? input.DateCreated : Date.now(), 'm/d/yyyy');
        model.PickedInventoryDetails(input.PickedInventoryDetail);
    });
    
    // Exported values
    params.exports(model);

    return this;
}

ko.utils.extend(IntraWarehouseInventoryMovementViewModel, {
    dispose: function() {
        this.IntraWarehouseMovement = null;
    }
});

module.exports = {
    viewModel: IntraWarehouseInventoryMovementViewModel,
    template: require('./intra-warehouse-inventory-movement-details.html')
};
