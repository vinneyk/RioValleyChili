var lotInventoryItemFactory = require('App/models/LotInventoryItem'),
    rvc = require('App/rvc');

function PickableInventoryItem( input, checkOutOfRange ) {
    if (!(this instanceof PickableInventoryItem)) return new PickableInventoryItem( input, checkOutOfRange );

    if (input instanceof lotInventoryItemFactory && ko.isObservable(input.QuantityPicked)) {
        return input;
    }

    var inventoryItem = lotInventoryItemFactory( input, checkOutOfRange ),
        qtyInput = input.QuantityPicked == undefined ? undefined : Number(input.QuantityPicked),
        quantityPicked = ko.observable(qtyInput),
        preservedQuantityPicked = ko.observable(Number(input.QuantityPicked) || undefined),
        quantityOnHand = ko.observable(input.Quantity || 0),
        maxQuantityPickedValue = ko.pureComputed(function () {
            return quantityOnHand() + (preservedQuantityPicked() || 0);
        });

    inventoryItem.LotType = rvc.lists.lotTypes.fromLotKey(input.LotKey);
    inventoryItem.isInitiallyPicked = ko.observable(input.isPicked || false);
    inventoryItem.ValidForPicking = input.ValidForPicking;
    inventoryItem.checkOutOfRange = checkOutOfRange;
    inventoryItem.OrderItemKey = input.OrderItemKey;

    inventoryItem.QuantityPicked = ko.pureComputed({
        read: function () {
            return quantityPicked();
        },
        write: function (value) {
            var oldValue = quantityPicked();
            if (value === oldValue) { return; }

            var numVal = Number(value);

            if (isNaN(numVal)) { quantityPicked(null); }
            else quantityPicked(numVal);

            ko.postbox.publish('pickedQuantityChanged', { value: numVal, item: inventoryItem, oldValue: oldValue });
        },
        owner: inventoryItem
    });

    inventoryItem.isPicked = ko.pureComputed(function () {
        return inventoryItem.isInitiallyPicked() || inventoryItem.QuantityPicked() > 0;
    });

    inventoryItem.WeightPicked = ko.pureComputed(function () {
        var qtyPicked = inventoryItem.QuantityPicked() || 0;
        return qtyPicked > 0
            ? calculatePoundsForQuantity(qtyPicked)
            : '';
    });
    inventoryItem.QuantityAvailable = ko.pureComputed(function () {
        return inventoryItem.QuantityPicked() >= 0 ?
            maxQuantityPickedValue() - (inventoryItem.QuantityPicked() || 0) :
            maxQuantityPickedValue();
    });
    inventoryItem.TotalWeightAvailable = ko.pureComputed(function () {
        return (inventoryItem.QuantityAvailable() || 0) * inventoryItem.PackagingCapacity;
    });
    inventoryItem.isChanged = ko.computed(function () {
        return inventoryItem.QuantityPicked() !== preservedQuantityPicked();
    });

    inventoryItem.validation = ko.validatedObservable({
        quantityPicked: inventoryItem.QuantityPicked.extend({
            min: 0,
            max: maxQuantityPickedValue
        })
    });

    inventoryItem.setInitialQuantityPicked = function (value) {
        preservedQuantityPicked(value);
        quantityPicked(value);
        inventoryItem.QuantityPicked(value);
        inventoryItem.commit();
    };
    inventoryItem.revert = function () {
        inventoryItem.QuantityPicked(preservedQuantityPicked());
    };
    inventoryItem.commit = function () {
        quantityOnHand(inventoryItem.QuantityAvailable());
        preservedQuantityPicked(inventoryItem.QuantityPicked());
        if (quantityPicked() > 0 && inventoryItem.validation.isValid()) {
            inventoryItem.isInitiallyPicked(true);
        } else if (quantityPicked() <= 0 || isNaN(quantityPicked())) {
            inventoryItem.isInitiallyPicked(false);
        }
    };

    function calculatePoundsForQuantity(quantity) {
        var qty = parseInt(quantity);
        if (!qty) return 0;
        return qty * inventoryItem.PackagingCapacity;
    }

    return inventoryItem;
}

module.exports = PickableInventoryItem;
