var warehouseService = require('services/warehouseService');
require('bootstrap');

function MovementModel(input) {
    if (!(this instanceof MovementModel)) { return new MovementModel(input); }

    var self = this;

    self.disposables = [];
    
    self.MovementKey = input.MovementKey;
    self.MoveNum = input.MoveNum;
    self.DateCreated = new Date(input.DateCreated).format('m/d/yyyy', true);
    self.InventoryTreatment = input.InventoryTreatment || {};
    self.PickOrder = input.PickOrder;
    self.PickedInventory = input.PickedInventory;
    self.Shipment = input.Shipment;
    self.ShipmentDate = new Date(input.ShipmentDate).format('m/d/yyyy', true);
    self.DestinationFacility = input.DestinationFacility;
    self.OriginFacility = input.OriginFacility;
    self.OrderStatus = ko.observable(input.OrderStatus).extend({ orderStatusType: true });
    self.StatusDisplayText = input.StatusDisplayText;
    self.EnableReturnFromTreatment = input.EnableReturnFromTreatment;

    self.Shipment.Status = ko.observable(input.Shipment.Status || 0).extend({ shipmentStatusType: true });
        
    if (self.PickOrder && 
            !self.PickOrder.hasOwnProperty("PoundsOnOrder") &&
            self.PickOrder.hasOwnProperty("PickOrderItems")) {
        calculateTotals();
    }

    function calculateTotals () {
        var pickOrderWeight = 0,
        pickOrderQuantity = 0,
        pickedWeight = 0,
        pickedQuantity = 0;

        (function calculatePickOrderTotalWeight() {
            for (var i = 0, list = self.PickOrder.PickOrderItems, max = list.length;
                i < max; i += 1) {
                if (list[i].Quantity > 0) {
                    pickOrderWeight += list[i].TotalWeight;
                    pickOrderQuantity += list[i].Quantity;
                }
            }
        })();

        (function calculatePickedTotalWeight() {
            for (var i = 0, list = self.PickedInventory.PickedInventoryItems, max = list.length;
                    i < max; i += 1) {
                if (list[i].QuantityPicked > 0) {
                    pickedQuantity += list[i].QuantityPicked;
                    pickedWeight += (list[i].QuantityPicked * list[i].PackagingProduct.Weight);
                }
            }
        })();
        self.PickOrder.TotalQuantity = pickOrderQuantity;
        self.PickOrder.PoundsOnOrder = pickOrderWeight;
        self.PickedInventory.TotalQuantityPicked = pickedQuantity;
        self.PickedInventory.PoundsPicked = pickedWeight;
    }
}

ko.utils.extend(MovementModel, {
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

module.exports = MovementModel;
