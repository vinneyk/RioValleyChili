var warehouseService = require('services/warehouseService'),
    page = require('page');

ko.bindingHandlers.onEnter = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var allBindings = allBindingsAccessor();
        $(element).keypress(function (event) {
            var keyCode = (event.which ?
                    event.which :
                    event.keyCode);
            if (keyCode === 13) {
                allBindings.onEnter.call(viewModel);
                return false;
            }
            return true;
        });
    }
};

function InterWarehouseEditorViewModel(params) {
    // Init
    var self = this,
        movement = ko.observable();

    self.validation = ko.validatedObservable({});
    self.disposables = [];

    self.warehouses = ko.observableArray();
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

    self.warehouses = ko.isObservable(params.warehouseOptions) ? params.warehouseOptions : ko.observableArray(params.warehouseOptions || []);

    // Subscriptions and Computed Values
    self.currentMovement = ko.computed({
        read: function() {
            return movement();
        },
        write: function(values) {
            if (values === null || values === undefined) {
                setMovement(null);
                return;
            }

            var warehouseList = self.warehouses(),
                data = ko.toJS(values) || {},
                originKey,
                destinationKey;

            if (!data.MovementKey) {
                self.isNewMovement = true;
            } else {
                originKey = data.OriginFacilityKey
                    ? data.OriginFacilityKey
                    : data.OriginFacility.FacilityKey;
                destinationKey = data.DestinationFacilityKey
                    ? data.DestinationFacilityKey
                    : data.DestinationFacility.FacilityKey;
                self.isNewMovement = false;

                ko.utils.arrayFirst(warehouseList, function(wh) {
                    if (wh.FacilityKey === originKey) {
                        data.OriginDetails = wh;
                    }
                    if (wh.FacilityKey === destinationKey) {
                        data.DestinationDetails = wh;
                    }
                    return data.OriginDetails && data.DestinationDetails;
                });

                if (!data.OriginDetails || !data.DestinationDetails) {
                    throw new Error("Details data was not set");
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
            self.validation(null);
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
            MovementKey: ko.observable(data.MovementKey),
            OrderRequestedBy: ko.observable(data.OrderRequestedBy),
            OrderTakenBy: ko.observable(data.OrderTakenBy),
            OrderStatus: ko.observable(data.OrderStatus),
            OriginFacilityDetails: ko.observable(data.OriginDetails),
            PickOrder: ko.observable(data.PickOrder),
            PickedInventory: ko.observable(data.PickedInventory || {}),
            PurchaseOrderNumber: ko.observable(data.PurchaseOrderNumber),
            Shipment: ko.observable(shipment),
            ShippingInstructions: ko.observable({
                    SpecialInstructions: ko.observable(instructions.SpecialInstructions),
                    InternalNotes: ko.observable(instructions.InternalNotes),
                    ExternalNotes: ko.observable(instructions.ExternalNotes),
                }
            ),
            MoveNum: data.MoveNum,
            ShipmentDate: ko.observableDate(data.ShipmentDate).extend({ required: false }),
            PickedInventoryInput: data.PickedInventory,
            reports: [
                {
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
                }, {
                    name: 'Certificate of Analysis',
                    url: data.Links && data.Links['report-coa'] && data.Links['report-coa'].HRef
                }
            ]
        };

        m.OriginFacilityKey = ko.pureComputed(function() {
            return m ? m.OriginFacilityDetails() && m.OriginFacilityDetails().FacilityKey : null;
        });
        m.DestinationFacilityKey = ko.pureComputed(function() {
            return m ? m.DestinationFacilityDetails() && m.DestinationFacilityDetails().FacilityKey : null;
        });
        m.PickedInventory = ko.pureComputed(function() {
            var pickedInventoryVm = self.pickedInventoryItemsExports();
            return (pickedInventoryVm && pickedInventoryVm.PickedItems()) || [];
        });
        m.hasPickedInventory = ko.pureComputed(function() {
            return m.PickedInventory().length;
        });

        movement(m);

        self.isLocked(data.IsLocked);
    }

    self.disposables.push(ko.computed(function() {
        var movement = self.currentMovement(),
            shipment = self.shipmentExports();
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

    self.isLoaded = ko.computed(function() {
        if (self.currentMovement() === null) {
            return true;
        }

        var requiredData = {
            shipment: ko.toJS(self.shipmentExports()) || {},
            pickedInventory: ko.toJS(self.pickedInventoryItemsExports()) || {},
            pickOrder: ko.toJS(self.inventoryPickOrderExports()) || {}
        };

        if (self.currentMovement() &&
            requiredData.shipment.isInit &&
            requiredData.pickedInventory.isInit &&
            requiredData.pickOrder.isInit) {
            return true;
        }
        return false;
    });


    self.currentMovement.isValid = function() {
        var movement = self.currentMovement();
        if (!movement) return false;
        var validation = ko.validatedObservable({
            ShipmentDate: movement.ShipmentDate
        });

        if (!validation.isValid()) {
            validation.errors.showAllMessages();
            return false;
        }
        return true;
    };

    self.currentMovement.asDto = function() {
      var movementData = self.currentMovement();

      if ( !movementData ) {
        return null;
      }

      var dto = {
        OriginFacilityKey: movementData.OriginFacilityKey,
        DestinationFacilityKey: movementData.DestinationFacilityKey,
        ShipmentDate: movementData.ShipmentDate,
        PurchaseOrderNumber: movementData.PurchaseOrderNumber,
        DateOrderReceived: movementData.DateOrderReceived,
        OrderRequestedBy: movementData.OrderRequestedBy,
        OrderTakenBy: movementData.OrderTakenBy,
        Shipment: null,
        InventoryPickOrderItems: self.inventoryPickOrderExports().toDto(),
        PickedInventoryItemCodes: self.pickedInventoryItemsExports().toDto(),
      };

      var _shippingInstructions = movementData.ShippingInstructions();
      dto.Shipment = self.shipmentExports().toDto();
      dto.Shipment.ShippingInstructions.SpecialInstructions = _shippingInstructions.SpecialInstructions;
      dto.Shipment.ShippingInstructions.InternalNotes = _shippingInstructions.InternalNotes;
      dto.Shipment.ShippingInstructions.ExternalNotes = _shippingInstructions.ExternalNotes;

      return ko.toJS( dto );
    };

    // Exports
    params.exports({
        currentMovement: self.currentMovement,
        isLocked: self.isLocked,
        dispose: self.dispose
    });

    self.exported = params.exports;
}

// Custom disposal logic
InterWarehouseEditorViewModel.prototype.dispose = function () {
    ko.utils.arrayForEach(this.disposables, this.disposeOne);
    ko.utils.objectForEach(this, this.disposeOne);
    this.exported(null);
};

InterWarehouseEditorViewModel.prototype.disposeOne = function (propOrValue, value) {
    var disposable = value || propOrValue;

    if (disposable && typeof disposable.dispose === "function") {
        disposable.dispose();
    }
};

ko.components.register('shipment-editor', require('App/components/warehouse/shipment-editor/shipment-editor'));
ko.components.register('inventory-pick-order', require('App/components/warehouse/inventory-pick-order/inventory-pick-order'));
ko.components.register('picked-inventory-items', require('App/components/inventory/picked-inventory-items/picked-inventory-items'));

// Webpack
module.exports = {
    viewModel: InterWarehouseEditorViewModel,
    template: require('./inter-warehouse-movement-editor.html')
};
