function InventoryResults(inventoryItems) {
    if (inventoryItems == null) { return; }
    else if (inventoryItems.length < 1) {
        showUserMessage("This lot does not contain any available inventory items.");
        return;
    } else if (!validateInventoryResults(inventoryItems)) {
        alert("The inventory items is not valid for use with this form. All items in the collection must have the same lot number and product name.")
        return;
    }

    var me = this;
    me.inventoryOptions = inventoryItems;

    me.lotNumber = inventoryItems[0].LotKey,
    me.product = inventoryItems[0].ProductName,
    me.treatment = ko.observable(inventoryItems[0].Treatment),
    me.packagingKey = ko.observable();
    me.inventoryKey = ko.observable();

    me.packagingOptions = ko.observableArray([]);
    me.treatmentOptions = ko.observableArray([{ display: inventoryItems[0].TreatmentName, value: inventoryItems[0].TreatmentKey }]);

    // computed properties
    me.warehouseLocationOptions = ko.computed(function () {
        if (!me.packagingKey()) { return []; }

        var inventoryForPackaging = ko.utils.arrayFilter(me.inventoryOptions, function (item) {
            return item.PackagingProductKey === me.packagingKey();
        });

        var locs = ko.utils.arrayMap(inventoryForPackaging, function (item) {
            return {
                display: item.WarehouseLocationName,
                value: item.InventoryKey
            };
        });

        locs.sort();

        // if more than one option available, include empty option in position 0
        if (locs.length > 1) {
            locs.splice(0, 0, {
                display: '',
                value: ''
            });
        }
        return locs;
    });
    me.hasLot = ko.computed(function () {
        return this.lotNumber && this.lotNumber.length > 0;
    }, me);

    // event handlers
    me.getInventoryByKey = getInventoryByKey;

    init();

    //****************************************
    // private functions

    function init() {
        var packages = [];
        ko.utils.arrayForEach(inventoryItems, function (item) {
            var existing = ko.utils.arrayFirst(packages, function (pkg) {
                return pkg.value === item.PackagingProductKey;
            });

            if (existing == null) {
                packages.push({
                    display: item.PackagingProductName,
                    value: item.PackagingProductKey
                });
            }
        });

        packages.sort();

        if (packages.length > 1) {
            packages.splice(0, 0, {
                display: ' ',
                value: ' '
            });
        }

        me.packagingOptions(packages);
    }
    function validateInventoryResults() {
        if (!(inventoryItems instanceof Array) || inventoryItems.length < 1) {
            return false;
        }

        var key = inventoryItems[0].LotKey;
        var productName = inventoryItems[0].ProductName;

        var firstInvalid = ko.utils.arrayFirst(inventoryItems, function (item) {
            return key !== item.LotKey || productName !== item.ProductName;
        });

        return firstInvalid === null;
    }
    function getInventoryByKey(keyDescriptor) {
        return ko.utils.arrayFirst(me.inventoryOptions, function (inv) {
            return inv.InventoryKey === keyDescriptor;
        });
    }
}