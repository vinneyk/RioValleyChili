var rvc = require('rvc'),
    productService = require('services/productsService'),
    inventoryService = require('services/inventoryService');

/**
* auto-inventory-picker
* 
* (All params optional)
*  
* @param enable         {Boolean}  Enable `Pick` button (default=true).
* @param pickedItems    {array}    Existing picked inventory for lot. IMPORTANT: omitting this property will cause existing items to be unpicked!
* @param inventoryType  {Key}      Setting this makes it read-only.
* @param product        {Key}      Default value for `product` input.
* @param quantity       {Integer}  Default value for `quantity` input
* @param sortFn         {Function} Function to sort inventory by.
* @param onSuccess      {Function} Success callback.
* @param onFail         {Function} Error callback.
* @param pickingContext {string}   REQUIRED. Determines route param value.
* @param lotKey         {string}   REQUIRED. The lot for which inventory will be picked.
*/
function AutoInventoryPicker(params) {
    if (!(this instanceof AutoInventoryPicker)) { return new AutoInventoryPicker(params); }

    var self = this;

    this.sortFn = params.sortFn || sortByDate;
    this.staticType = params.inventoryType;
    this.enablePick = params.enable || ko.observable(true);
    this.lotKey = ko.pureComputed(function() {
        return ko.unwrap(params.lotKey);
    });
    this.pickingContext = ko.pureComputed(function() {
        return ko.unwrap(params.pickingContext);
    });
    this.existingPickedItems = ko.pureComputed(function() {
        return ko.unwrap(params.pickedItems);
    });
    this.isSaving = ko.observable(false);

    // Inputs
    this.inventoryType = ko.observable(this.staticType ?
        rvc.lists.inventoryTypes.findByKey(this.staticType) :
        null);
    this.product = ko.observable();
    this.quantity = ko.numericObservable(params.quantity || 1);

    // Load productOpts
    this.inventoryType.subscribe(loadProducts);

    // Select options
    this.inventoryOpts = rvc.lists.inventoryTypes.buildSelectListOptions();
    this.productOpts = ko.observableArray();

    // Set default product
    if (ko.unwrap(params.product)) {
        // Silly timeout for select options binding
        var s = this.productOpts.subscribe(setTimeout.bind(null, function (items) {
            var prod = getProductByKey(params.product);
            self.product(prod);
            s.dispose(); // only once!
        }), 0);
    }

    loadProducts();

    // Commands/Behaviors
    this.getInventoryPicks = ko.asyncCommand({
        execute: function (complete) {
            return self.pickInventoryAndSave().always(complete);
        },
        canExecute: function (isExecuting) {
            return !isExecuting && self.enablePick() && self.inventoryType() && self.product() && self.quantity() > 0;
        }
    });

    ko.isObservable(params.exports) && params.exports(self);

    function loadProducts() {
        var lotType = self.inventoryType();
        if (lotType) {
            productService.getProductsByLotType(lotType.key)
                .done(self.productOpts)
                .fail(console.error.bind(console));
        }
    }

    /** Get product from productOpts */
    function getProductByKey(productKey) {
        var key = ko.unwrap(productKey),
            items = self.productOpts();

        for (var i = 0, max = items.length; i < max; i += 1) {
            if (items[i].ProductKey == key) {
                return items[i];
            }
        }

        throw 'Could not find given product by key:' + key;
    }

    /** Default sort function */
    function sortByDate(a, b) {
        //TODO: if same lot, subtract different (sorting) property instead
        return new Date(a.LotDateCreated) - new Date(b.LotDateCreated);
    }
}

AutoInventoryPicker.prototype.findItemByKeyDelegate = function(key) {
     return function(item) {
         return item.InventoryKey === key;
     }
}
AutoInventoryPicker.prototype.pickInventoryAndSave = function () {
    var self = this,
        context = ko.unwrap(self.pickingContext),
        key = ko.unwrap(self.lotKey),
        pickedItems = ko.toJS(self.existingPickedItems),
        $dfd = $.Deferred();

    self.isSaving(true);

    self.getInventoryItemPicks()
        .then(function(data) {
            ko.utils.arrayForEach(data, function(item) {
                var existingPick = ko.utils.arrayFirst(pickedItems, self.findItemByKeyDelegate(item.InventoryKey));
                if (existingPick) {
                    existingPick.QuantityPicked += item.QuantityPicked;
                } else {
                    pickedItems.push(item);
                }
            });

            inventoryService.savePickedInventory(context, key, pickedItems)
                .done(function () {
                    showUserMessage('Save successful', { description: 'Products have been successfully picked' });
                    ko.postbox.publish('AutoPickedItemsSaved', pickedItems);
                    $dfd.resolve(data, pickedItems);
                })
                .fail(function (promise, status, message) {
                    ko.postbox.publish('AutoPickedItemsSaveFailed', pickedItems);
                    showUserMessage('Failed to save items', { description: 'Server gave error: \n' + message });
                    $dfd.reject();
                })
                .always(function () {
                    self.isSaving(false);
                });
            
        })
        .fail(function() {
            $dfd.reject();
        });

    return $dfd;
}

AutoInventoryPicker.prototype.getInventoryItemPicks = function () {
    var self = this;
    var context = ko.unwrap(self.pickingContext),
        key = ko.unwrap(self.lotKey),
        params = {
            productKey: self.product().ProductKey,
            productType: self.inventoryType().key
        },
        dfd = $.Deferred();

    inventoryService.getPickableInventory(context, key, params)
        .fail(function(xhr, result, msg) {
            showUserMessage("Failed to fetch inventory", {
                description: msg,
                mode: 'error',
                autoclose: true
            });
            dfd.reject();
        })
        .done(function(data) {
            var picks = [],
                quantityRequested = Number(self.quantity()),
                totalQuantityPicked = 0,
                diff = quantityRequested;

            data.sort(self.sortFn);

            ko.utils.arrayFirst(data, function(item) {
                item.QuantityPicked = diff > item.Quantity ? item.Quantity : diff;
                picks.push(item);

                totalQuantityPicked += item.QuantityPicked;
                diff = quantityRequested - totalQuantityPicked;
                return totalQuantityPicked >= quantityRequested;
            });

            dfd.resolve(picks);
        });

    return dfd;
}

module.exports = {
    viewModel: AutoInventoryPicker,
    template: require('./auto-inventory-picker.html')
};