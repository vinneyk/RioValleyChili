ko.components.register('shipment-editor', require('App/components/warehouse/shipment-editor/shipment-editor'));
ko.components.register('inventory-pick-order', require('App/components/warehouse/inventory-pick-order/inventory-pick-order'));
ko.components.register('picked-inventory-items', require('App/components/inventory/picked-inventory-items/picked-inventory-items'));

var treatmentService = require('services/treatmentService'),
    app = require('app');

function TreatmentOrderEditorViewModel(params) {
    // Init
    var self = this,
    movement = ko.observable();

    // Cleanup
    self.disposables = [];

    // Initial bindings
    self.warehouses = ko.isObservable(params.warehouseOptions) ? params.warehouseOptions : ko.observableArray(params.warehouseOptions || []);
    self.shipmentExports = ko.observable();
    self.details = ko.observable();
    self.inventoryPickOrderExports = ko.observable();
    self.pickedInventoryItemsExports = ko.observable();

    self.CustomerPO = ko.observable();
    self.DateOrderReceived = ko.observableDate();
    self.OrderPlacedBy = ko.observable();
    self.OrderTakenBy = ko.observable();

    self.isLocked = ko.observable();
    self.isNewMovement = false;

    // Subscriptions and Computed Values
    self.currentMovement = ko.pureComputed({
        read: function () {
            return movement();
        },
        write: function (values) {
            if (values == undefined) {
                setMovement(null);
                return;
            }

            var warehouseList = self.warehouses(),
                i = 0,
                data = ko.toJS(values) || {},
                originKey,
                destinationKey;

            if (!data.MovementKey) {
                self.isNewMovement = true;
            } else {
                originKey = data.OriginFacilityKey ?
                    data.OriginFacilityKey :
                    data.OriginFacility.FacilityKey;
                destinationKey = data.DestinationFacilityKey ?
                    data.DestinationFacilityKey :
                    data.DestinationFacility.FacilityKey;
                self.isNewMovement = false;

                for (i = warehouseList.length; i--;) {
                    if (warehouseList[i].FacilityKey === originKey) {
                        data.OriginDetails = warehouseList[i];
                    }
                    if (warehouseList[i].FacilityKey === destinationKey) {
                        data.DestinationDetails = warehouseList[i];
                    }
                    if (data.OriginDetails && data.DestinationDetails) {
                        break;
                    }
                }

                if (!data.OriginDetails && !data.DestinationDetails) {
                    throw new Error ("Details data was not set");
                }
            }

            setMovement(data);
        }
    });

    if (ko.isObservable(params.values)) {
        self.disposables.push(params.values.subscribe(function(vals) {
            self.currentMovement(vals);
        }));
    }

    function setMovement(input) {
        if (input === null || input === undefined) {
            movement(null);
            self.shipmentExports(null);
            self.inventoryPickOrderExports(null);
            self.pickedInventoryItemsExports(null);
            return;
        }

        var data = input || {},
            shipment = data.Shipment || {},
            instructions = shipment.ShippingInstructions || {};

        var m = {
            CustomerOrder: ko.observable(data.CustomerOrder),
            DateCreated: ko.observableDate(data.DateCreated),
            DateOrderReceived: ko.observableDate(data.DateOrderReceived),
            DestinationFacilityDetails: ko.observable(data.DestinationDetails),
            EnableReturnFromTreatment: ko.observable(data.EnableReturnFromTreatment),
            InventoryTreatment: ko.observable(data.InventoryTreatment && data.InventoryTreatment.TreatmentKey || 0).extend({ treatmentType: true, min: 1 }),
            MovementKey: ko.observable(data.MovementKey),
            OrderRequestedBy: ko.observable(data.OrderRequestedBy),
            OrderTakenBy: ko.observable(data.OrderTakenBy),
            OrderStatus: data.OrderStatus,
            StatusDisplayText: data.StatusDisplayText,
            OriginFacilityDetails: ko.observable(data.OriginDetails),
            PickOrder: ko.observable(data.PickOrder),
            PurchaseOrderNumber: ko.observable(data.PurchaseOrderNumber),
            Shipment: ko.observable(shipment),
            ShippingInstructions: ko.observable({
                SpecialInstructions: ko.observable(instructions.SpecialInstructions),
                InternalNotes: ko.observable(instructions.InternalNotes),
                ExternalNotes: ko.observable(instructions.ExternalNotes),
            }),
            MoveNum: data.MoveNum,
            ShipmentDate: ko.observableDate(data.ShipmentDate).extend({ required: false }),

            PickedInventoryInput: data.PickedInventory,
            reports: [{
                name: 'Order Acknowledgement',
                url: data.Links && data.Links['report-wh-acknowledgement'] && data.Links['report-wh-acknowledgement'].HRef
            }, {
                name: 'Inventory Pick List',
                url: data.Links && data.Links['report-pick-list'] && data.Links['report-pick-list'].HRef
            }, {
                name: 'Bill of Lading',
                url: data.Links && data.Links['report-bol'] && data.Links['report-bol'].HRef
            }, {
                name: 'Packing List',
                url: data.Links && data.Links['report-packing-list'] && data.Links['report-packing-list'].HRef
            }]
        }
        m.PickedInventory = ko.pureComputed(function() {
            var pickedInventoryVm = self.pickedInventoryItemsExports();
            return (pickedInventoryVm && pickedInventoryVm.PickedItems()) || [];
        });
        m.hasPickedInventory = ko.pureComputed(function() {
            return m.PickedInventory().length;
        });

        movement(m);

        self.isLocked(input.IsLocked || false);

        movement().OriginFacilityKey = ko.computed(function () {
            return movement() ? movement().OriginFacilityDetails() && movement().OriginFacilityDetails().FacilityKey : null;
        });
        movement().DestinationFacilityKey = ko.computed(function () {
            return movement() ? movement().DestinationFacilityDetails() && movement().DestinationFacilityDetails().FacilityKey : null;
        });
    }

    self.disposables.push(ko.computed(function () {
        var movement = self.currentMovement();
        var shipment = self.shipmentExports();
        if (movement && shipment) {
            shipment.setShipFromAddress(
                (movement.OriginFacilityDetails() || {}).ShippingLabel || null
            );
            shipment.setFreightBillAddress(
                (movement.OriginFacilityDetails() || {}).ShippingLabel || null
            );
            shipment.setShipToAddress(
                (movement.DestinationFacilityDetails() || {}).ShippingLabel || null
            );
        }
    }));

    self.pickCommand = params.pickCommand;

    // Behaviors
    self.showReceiveInventory = function () {
        $('#treatmentModal').modal('show');
    };

    self.currentMovement.isValid = function() {
        var movement = self.currentMovement();
        if (!movement) return false;
        var validation = ko.validatedObservable({
            ShipmentDate: movement.ShipmentDate,
            Treatment: movement.InventoryTreatment
        });

        if (!validation.isValid()) {
            validation.errors.showAllMessages();
            return false;
        }
        return true;
    }

    self.currentMovement.asDto = function() {
        var values = ko.toJS(self.currentMovement());
        if (!values) return null;

        values.Shipment = self.shipmentExports().toDto();
        values.InventoryPickOrderItems = self.inventoryPickOrderExports().toDto();
        values.PickedInventoryItemCodes = self.pickedInventoryItemsExports().toDto();
        values.Shipment.ShippingInstructions.SpecialInstructions = values.ShippingInstructions.SpecialInstructions;
        values.Shipment.ShippingInstructions.InternalNotes = values.ShippingInstructions.InternalNotes;
        values.Shipment.ShippingInstructions.ExternalNotes = values.ShippingInstructions.ExternalNotes;
        values.SourceFacilityKey = values.OriginFacilityKey;
        values.TreatmentKey = values.InventoryTreatment;
        values.HeaderParameters = {
            CustomerPurchaseOrderNumber: values.PurchaseOrderNumber,
            ShipmentDate: values.ShipmentDate,
            DateOrderReceived: values.DateOrderReceived,
            OrderRequestedBy: values.OrderRequestedBy,
            OrderTakenBy: values.OrderTakenBy
        };
        values.SetShipmentInformation = values.Shipment;
        return values;
    };

    self.disposables.push(self.currentMovement);

    // Exports
    params.exports({
        currentMovement: self.currentMovement,
        showReceiveInventory: self.showReceiveInventory,
        isLocked: self.isLocked,
        dispose: self.dispose
    });
}

// Custom disposal logic
TreatmentOrderEditorViewModel.prototype.dispose = function () {
    ko.utils.arrayForEach(this.disposables, this.disposeOne);
    ko.utils.objectForEach(this, this.disposeOne);
};

TreatmentOrderEditorViewModel.prototype.disposeOne = function (propOrValue, value) {
    var disposable = value || propOrValue;

    if (disposable && typeof disposable.dispose === "function") {
        disposable.dispose();
    }
};

// Webpack
module.exports = {
    viewModel: TreatmentOrderEditorViewModel,
    template: require('./treatment-order-editor.html')
};
