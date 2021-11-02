var warehouseService = require('services/warehouseService'),
    productsService = require('services/productsService'),
    directoryService = require('services/directoryService'),
    PickOrderItemFactory = require('App/models/PickOrderItemModel');

function PickOrderItem( item, options ) {
  item.customerOptions = options.customerOptions;
  item.packageOptions = options.packageOptions;
  item.productOptions = options.productOptions;

  var mappedItem = new PickOrderItemFactory( item );

  var _dirtyCheckProps = [ mappedItem.ProductKey, mappedItem.CustomerKey, mappedItem.CustomerProductCode, mappedItem.CustomerLotCode, mappedItem.PackagingKey, mappedItem.TreatmentKey, mappedItem.Quantity ];


  var _quantity = mappedItem.Quantity();
  mappedItem.Quantity( _quantity != null ? "" + _quantity : "" );

  var _lotCode = mappedItem.CustomerLotCode();
  mappedItem.CustomerLotCode( _lotCode != null ? "" + _lotCode : "" );

  var _productCode = mappedItem.CustomerProductCode();
  mappedItem.CustomerProductCode( _productCode != null ? "" + _productCode : "" );

  mappedItem.dirtyFlag = (function( root, isInitiallyDirty ) {
    var result = function() {},
    _initialState = ko.observable( ko.toJSON( root ) ),
    _isInitiallyDirty = ko.observable( isInitiallyDirty );

    result.isDirty = ko.computed(function() {
      return _isInitiallyDirty() || _initialState() !== ko.toJSON( root );
    });

    result.reset = function() {
      _initialState(ko.toJSON( root ));
      _isInitiallyDirty( false );
    };

    return result;
  })( _dirtyCheckProps, false );

  return mappedItem;
}

InventoryPickOrderViewModel.prototype.setInitialPickOrderItems = function (values) {
    var self = this;
    self.init.then(function() {
        self.PickOrderItems(self.mapPickOrderItems((values && values.PickOrderItems) || []));
    });
};

InventoryPickOrderViewModel.prototype.mapPickOrderItems = function (input) {
    var self = this;

    var options = {
      customerOptions: self.customerOptions,
      packageOptions: self.packageOptions,
      productOptions: self.productOptions
    };

    var items = ko.utils.arrayMap( input, function( item ) {
      return new PickOrderItem( item, options );
    });

    return items;
};

function InventoryPickOrderViewModel(params) {
    if(!(this instanceof InventoryPickOrderViewModel)) { return new InventoryPickOrderViewModel(params); }

    var self = this;

    self.disposables = [];

    // Initial bindings
    self.isInit = ko.observable(false);

    self.customerOptions = [];
    self.packageOptions = [];
    self.productOptions = [];

    self.isLocked = params.locked || null;

    self.PickOrderItems = ko.observableArray();

    self.TotalQuantityOnOrder = ko.pureComputed(function () {
        var total = 0;
        ko.utils.arrayForEach(self.PickOrderItems(), function (item) {
            total += Number(item.Quantity());
        });
        return total;
    });
    self.TotalWeightOnOrder = ko.pureComputed(function () {
        var total = 0;
        if (self.PickOrderItems().length > 0) {
            ko.utils.arrayForEach(self.PickOrderItems(), function (item) {
                total += Number(item.TotalWeight());
            });
        }
        return total;
    });

   self.init = $.when(directoryService.getCustomers(), productsService.getPackagingProducts(), productsService.getChileProducts())
        .done(function(customers, packaging, products) {
            if (customers[1] !== "success" || packaging[1] !== "success" || products[1] !== "success") {
                throw new Error('Failed to load Inventory Pick Order\'s option arrays.');
            }

            self.customerOptions = customers[0];
            self.packageOptions = packaging[0];
            self.productOptions = products[0];

            self.isInit(true);
        });

    // Behaviors
    self.addNewItem = function () {
      var options = {
          customerOptions: self.customerOptions,
          packageOptions: self.packageOptions,
          productOptions: self.productOptions,
      };

      var newItem = new PickOrderItem( {}, options );

      self.PickOrderItems.push( newItem );

      var items = $(".pick-order-item");
      var itemsCount = items.length;
      $(items[itemsCount - 1]).find(".product-select")[0].focus();
    };

    self.removeItem = ko.command({
        execute: function () {
            self.PickOrderItems.splice(self.PickOrderItems.indexOf(this), 1);
        },
        canExecute: function () {
            return !self.isLocked();
        }
    });

    self.isValid = function () {
        var isValid = true;
        for (var i = self.PickOrderItems().length, list = self.PickOrderItems() ; i--;) {
            if (list[i].validation.isValid()) {
                isValid = false;
                break;
            }
        }
        return isValid;
    };

    self.toDto = function () {
      var items = self.PickOrderItems();
      var dto = ko.utils.arrayMap( items, mapItem );

      function mapItem( item ) {
        var itemData = {
          ProductKey: item.ProductKey,
          PackagingKey: item.PackagingKey,
          Quantity: item.Quantity,
          TreatmentKey: item.TreatmentKey,
          CustomerLotCode: item.CustomerLotCode,
          CustomerProductCode: item.CustomerProductCode,
          CustomerKey: item.CustomerKey
        };

        return itemData;
      }

      return ko.toJS( dto );
    };

    self.disposables.push(self.TotalQuantityOnOrder, self.TotalWeightOnOrder);
    self.pickForItem = ko.asyncCommand({
        execute: function ( data, complete ) {
            var product = this.Product() || {},
            packaging = this.Packaging() || {},
            targetProduct = ko.toJS({
                targetProduct: product,
                targetWeight: data.TotalWeight,
                filters: {
                    inventoryType: product.ProductType,
                    lotType: product.ChileState,
                    productKey: this.ProductKey,
                    packagingProductKey: packaging.ProductKey,
                    treatmentKey: this.TreatmentKey
                },
                customerLotCode: this.CustomerLotCode,
                customerProductCode: this.CustomerProductCode,
                orderItemKey: this.OrderItemKey
            });

            var getProductSpec = productsService.getProductDetails( product.ProductType, product.ProductKey ).then(
            function( data, textStatus, jqXHR ) {
              targetProduct.targetProduct.AttributeRanges = data.AttributeRanges;
              params.pickForItem(targetProduct);
            },
            function() {
              targetProduct.targetProduct.AttributeRanges = null;
              params.pickForItem(targetProduct);
            }).always( complete );
        },
        canExecute: function ( isExecuting ) {
            var enablePicking = ko.unwrap(params.enablePicking);
            if (enablePicking == undefined) { enablePicking = true; }

            return !isExecuting &&
              enablePicking &&
              typeof params.pickForItem === "function" &&
              !self.isLocked();
        }
    });

    if (ko.isObservable(params.data)) {
        self.disposables.push(params.data.subscribe(function(values) {
            self.setInitialPickOrderItems.call(self, values);
        }));
    }
    self.setInitialPickOrderItems(ko.unwrap(params.data));

    params.exports(self);
}

// Custom disposal logic
InventoryPickOrderViewModel.prototype.dispose = function () {
    ko.utils.arrayForEach(this.disposables, this.disposeOne);
    ko.utils.objectForEach(this, this.disposeOne);
};

InventoryPickOrderViewModel.prototype.disposeOne = function (propOrValue, value) {
    var disposable = value || propOrValue;

    if (disposable && typeof disposable.dispose === "function") {
        disposable.dispose();
    }
};

// Webpack
module.exports = {
    viewModel: InventoryPickOrderViewModel,
    template: require('./inventory-pick-order.html')
};
