ko.components.register('product-selector', require('components/common/product-selector/product-selector'));

ko.punches.enableAll();
var rvc = require('rvc');
var directoryService = require('services/directoryService');
var warehouseService = require('services/warehouseLocationsService');
var productsService = require('services/productsService');
var inventoryService = require('services/inventoryService');
require('App/scripts/ko.extenders.lotKey');
var lotTypes = rvc.lists.lotTypes.toObjectDictionary();

function InventoryReceiving(params) {
  if (!(this instanceof InventoryReceiving)) { return new InventoryReceiving(params); }

  var self = this;

  // Data
  this.lotTypeOptions = this.buildLotTypeOptions();
  this.vendorOptions = ko.observableArray([]);
  this.locationOptions = ko.observableArray([]);

  this.lotType = ko.observable().extend({ required: true });
  this.lotKey = ko.observable().extend();
  this.product = ko.observable().extend({ required: true });
  this.packagingReceived = ko.observable().extend({ required: true });
  this.quantity = ko.observable().extend({ required: true });
  this.dateReceived = ko.observableDate(Date.now().format('m/d/yyyy')).extend({ required: true });
  this.vendor = ko.observable();
  this.disablePackagingReceived = ko.observable(false);
  this.newVendorName = ko.observable().extend({
    required: {
      message: "A name is required to add new vendors",
      onlyIf: function() {
        return self.vendor() === 'new';
      }
    }
  });
  this.purchaseOrderNumber = ko.observable();
  this.shipperNumber = ko.observable();
  this.items = ko.observableArray([]);

  this.totalWeightReceived = ko.pureComputed(function() {
    var packaging = self.packagingReceived();
    var quantity = self.quantity();

    return (packaging && packaging.Weight) * quantity || 0;
  });
  this.totalWeightOfCreated = ko.pureComputed(function() {
    var total = 0;
    var items = self.items();

    ko.utils.arrayForEach( items, function( item ) {
      var quantity = item.quantity();
      var packaging = item.inventoryUnits();

      if ( quantity > 0 && packaging ) {
        total += packaging.Weight * quantity;
      }
    });

    return total || 0;
  }).extend({
    validation: {
      validator: function( val ) {
        var targetWeight = self.totalWeightReceived();
        return val > (targetWeight - 1) && val < (targetWeight + 1);
      }
    }
  });

  this.lotType.subscribe(function( lotKey ) {
    self.product( null );
    if ( lotKey === 5 || lotKey === 12 ) {
      self.packagingReceived("810");
      self.disablePackagingReceived(true);
    } else if ( lotKey === 4 ) {
      self.disablePackagingReceived(false);
    }
  });

  this.headerText = ko.pureComputed(function() {
    var lotKey = self.lotKey();
    return lotKey == null ? 'Receive New Inventory' : lotKey;
  });
  this.productLabelProp = ko.pureComputed(function() {
    var lotType = self.lotType();
    switch(lotType) {
      case rvc.lists.lotTypes.Additive.key:
      case rvc.lists.lotTypes.GRP.key:
        return "ProductCodeAndName";
      case rvc.lists.lotTypes.Packaging.key:
        return "ProductName";
    }
  });
  this.showTotalWeight = ko.pureComputed(function() {
    var lotType = self.lotType();
    return lotType === 4 || lotType === 12;
  });


  // Behaviors
  this.addItemCommand = ko.command({
    execute: function (values) {
      var item = new ReceivingItem(values);
      item.totalWeight = ko.pureComputed(function() {
        var quantity = item.quantity() || 0;
        var inventoryUnits = item.inventoryUnits() || { Weight: 0 };
        return quantity * inventoryUnits.Weight;
      });
      self.items.push(item);
    }
  });
  this.removeItemCommand = ko.command({
    execute: function (item) {
      if (!item || !(item instanceof ReceivingItem)) { return; }
      console.log(item);
      self.items.remove(item);
    }
  });
  this.saveInventoryCommand = ko.asyncCommand({
    execute: function(complete) {
      if (!self.validateModel()) {
        showUserMessage('Please correct validation errors.');
        complete();

        return $.Deferred().reject();
      }

      var dto = self.buildDto();
      return self.commitChangesAsync(dto)
        .done(self.reset)
        .always(complete);
    }
  });
  this.reset = function () {
    self.lotType(null);
    self.lotKey(null);
    self.product(null);
    self.packagingReceived(null);
    self.quantity(null);
    self.vendor(null);
    self.disablePackagingReceived(null);
    self.newVendorName(null);
    self.purchaseOrderNumber(null);
    self.shipperNumber(null);
    self.items([]);
    self.addItemCommand.execute({});
  }

  // Validation
  this.errors = ko.validation.group( [ this.lotType,
                                        this.product,
                                        this.packagingReceived,
                                        this.quantity,
                                        this.dateReceived,
                                        this.totalWeightOfPicked,
                                        this.items
                                     ],
                                     { deep: true } );

  // Exports
  if (params && ko.isObservable(params.exports)) {
    params.exports({
      saveAsyncCommand: this.saveInventoryCommand
    });
  }

  var init = $.when([
    this.buildVendorOptions(),
    this.loadWarehouseLocationOptions()
  ]).then(function() {
    self.addItemCommand.execute({});
  });

  return this;
}

module.exports = {
  viewModel: InventoryReceiving,
  template: require('./inventory-receiving.html')
};

InventoryReceiving.prototype.buildLotTypeOptions = function() {
  return [
    lotTypes[rvc.lists.lotTypes.Additive.value],
    lotTypes[rvc.lists.lotTypes.Packaging.value],
    lotTypes[rvc.lists.lotTypes.GRP.value]
  ];
};

InventoryReceiving.prototype.buildVendorOptions = function () {
  var self = this;
  return directoryService.getVendors(rvc.lists.companyTypes.Supplier.key)
    .done(function(data) {
      self.vendorOptions(data);
    })
    .fail(function() {
      showUserMessage('Unable to load vendors.');
    });
};

InventoryReceiving.prototype.loadWarehouseLocationOptions = function () {
  var self = this;
  return warehouseService.getRinconWarehouseLocations()
    .done(function(data) {
      self.locationOptions(data);
    })
    .fail(function() {
      showUserMessage('Unable to load warehouse locations.');
    });
};

InventoryReceiving.prototype.validateModel = function() {
  var errs = this.errors;

  if ( errs.length ) {
    errs.showAllMessages();

    return false;
  }

  return true;
};

InventoryReceiving.prototype.commitChangesAsync = function (dto) {
  var self = this;
  if (dto.VendorKey === 'new') {
    var $dfd = $.Deferred();
    var vendorName = self.newVendorName();

    directoryService.createVendor({
      CompanyName: vendorName,
      Active: true
    }).done(function (data, textStatus, jqXHR) {
      dto.VendorKey = data;
      inventoryService.receiveInventory(dto)
        .done(function (responseData) {
          $dfd.resolve(responseData);
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
          //todo: update vendors list and set the newly created vendor
          showUserMessage('Failed to save inventory', {
            description: 'The new vendor has been create. Please refresh the page before retrying in order to prevent creating duplicate vendors.'
          });
          $dfd.reject();
        });
    }).fail(function (jqXHR, textStatus, message) {
      showUserMessage('Vendor creation failed', {
        description: message
      });
      $dfd.reject();
    });
    return $dfd;

  } else {
    return inventoryService.receiveInventory(dto)
      .fail(function (jqXHR, textStatus, errorThrown) {
        showUserMessage('Failed to save inventory');
      });
  }
}

InventoryReceiving.prototype.buildDto = function () {
  var packagingReceived = this.packagingReceived() || {};
  return {
    LotType: this.lotType(),
    ProductKey: this.product().ProductKey,
    PackagingReceivedKey: packagingReceived.ProductKey,
    VendorKey: this.vendor(),
    PurchaseOrderNumber: this.purchaseOrderNumber(),
    ShipperNumber: this.shipperNumber(),
    Items: ko.utils.arrayMap(this.items(), function (item) { return item.buildDto(); }),
  };
};

function ReceivingItem(input) {
  if (!(this instanceof ReceivingItem)) { return new ReceivingItem(values); }

  var me = this;
  var values = input || {};

  this.quantity = ko.numericObservable(values.Quantity);
  this.inventoryUnits = ko.observable(values.PackagingProduct).extend({ required: true });
  this.location = ko.observable(values.LocationKey).extend({ required: true });

  this.totalWeight = ko.pureComputed(function() {
    var q = me.quantity() || 0;
    var pkg = me.inventoryUnits() || { Weight: 0 };
    return q * pkg.Weight;
  });

  return me;
}

ReceivingItem.prototype.buildDto = function() {
  return {
    Quantity: this.quantity(),
    PackagingProductKey: this.inventoryUnits().ProductKey,
    WarehouseLocationKey: this.location(),
    TreatmentKey: rvc.lists.treatmentTypes.NotTreated.key,
    ToteKey: ''
  };
};
