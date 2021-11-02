var inventoryService = require('App/services/inventoryService'),
  warehouseService = require('App/services/warehouseService'),
  productsService = require('App/services/productsService'),
  rvc = require('app'),
  DehydratedItemReceived = require('App/models/DehydratedItemReceived');

require('App/helpers/koValidationHelpers.js');
require('App/helpers/koPunchesFilters.js');
require('node_modules/knockout-jqautocomplete/build/knockout-jqAutocomplete');
ko.punches.enableAll();

function DehydratedMaterialsDetailsVM(params) {
  if (!(this instanceof DehydratedMaterialsDetailsVM)) { return new DehydratedMaterialsDetailsVM(params); }

  var self = this;

  this.isInit = ko.observable(false);

  // Data
  this.isEditing = ko.computed(function() {
    var editing = ko.unwrap(params.isEditing);
    return editing == null ? true : editing;
  });
  this.lotKey = ko.observable();
  this.headerText = ko.observable();

  var originalValues = ko.computed(function() {
    return ko.unwrap(params.input);
  });

  self.editorData = ko.computed(function () {
    var input = ko.unwrap(params.input) || {};
    self.lotKey(input.LotKey);
    self.headerText(input.LotKey || 'Receive New Dehydrated Material');

    var model = {
      User: input.User,
      LotKey: input.LotKey,
      ProductionDate: ko.observable(input.ProductionDate).extend({ required: true }),
      Supplier: ko.observable(input.Dehydrator && input.Dehydrator.CompanyKey).extend({ required: true }),
      Load: ko.numericObservable(input.Load).extend({ required: true }),
      Product: ko.observable(input.ChileProductName).extend({ required: true }),
      PurchaseOrder: ko.observable(input.PurchaseOrder).extend({ required: true }),
      ShipperNumber: ko.observable(input.ShipperNumber).extend({ required: true }),
      productionDateHasFocus: true,
      Items: ko.observableArray(ko.utils.arrayMap(input.Items || [], function(val) {
        return self.buildMaterialReceivedItem(val);
      }))
    };

    model.totalWeight = ko.pureComputed(function () {
      var sumOfWeight = 0;
      ko.utils.arrayForEach(model.Items(), function (item) {
        sumOfWeight += ko.unwrap(item.Weight);
      });
      return sumOfWeight;
    });

    return model;
  });

  this.options = {
    chileProductOptions: ko.observableArray([]),
    packagingProductOptions: ko.observableArray([]),
    supplierOptions: ko.observableArray([]),
    varietyOptions: ko.observableArray([]),
    warehouseLocations: ko.observableArray([])
  };

  this.defaults = {
    packaging: rvc.constants.ThousandPoundTotePackaging.ProductKey,
    location: rvc.constants.DehyLocation.LocationKey
  };

  // Computed Data
  this.isNew = ko.pureComputed(function () {
    return self.lotKey() === 'new' || self.lotKey() == null;
  });


  this.editorMode = ko.computed(function () {
    var editing = self.isEditing();

    if (self.isNew()) {
      return 'editLot';
    }
    return editing ? 'editLot' : 'viewLot';
  });

  // Validation
  this.isValid = null;

  // Behaviors
  this.removeItem = function (itemContext, element) {
    if (!(itemContext instanceof DehydratedItemReceived)) return;

    var items = self.editorData().Items;
    items.splice(items.indexOf(itemContext), 1);
  };

  var init = $.when(
          productsService.getChileProducts(),
          productsService.getPackagingProducts(),
          warehouseService.getWarehouseLocations(rvc.constants.rinconWarehouse.WarehouseKey),
          inventoryService.getDehydrators(), productsService.getChileVarieties())
      .done(function (chile, packaging, warehouses, dehydrators, varieties) {
        var productOptions = ko.utils.arrayFilter(chile[0], function (product) {
          return product.ChileState === 1;
        });
        self.options.chileProductOptions(productOptions);
        self.options.packagingProductOptions(packaging[0]);
        self.options.warehouseLocations(warehouses[0]);
        self.options.supplierOptions(dehydrators);
        self.options.varietyOptions(varieties[0]);

        self.isInit(true);
      })
      .fail(function (jqXHR, textStatus, errorThrown) {
        showUserMessage('Initialization error occurred. Please refresh to try again.');
      });

  function buildDirtyChecker(type) {
    var _initialized = false;

    var result = ko.computed(function () {
      if (!_initialized) {
        ko.toJS(self.editorData);

        _initialized = true;

        return false;
      }

      return true;
    });

    return result;
  }

  function addVariety(name) {
    self.options.varietyOptions.unshift(name);
  }

  self.revertChanges = function() {
    params.input(originalValues());
  };

  self.addItemCommand = ko.command({
    execute: function () {
      init.done(function() {
        var items = self.editorData().Items(), values = {};
        if (items.length) {
          var previousItem = items[items.length - 1];
          values.Tote = previousItem.Tote.getNextTote();
          values.Variety = previousItem.Variety();
          values.Grower = previousItem.Grower();
          values.Location = previousItem.Location();
          values.Quantity = previousItem.Quantity();
          values.PackagingProduct = previousItem.PackagingProduct();
        } else {
          values.PackagingKey = self.defaults.packaging;
          values.LocationKey = self.defaults.location;
          values.Quantity = 1;
        }

        var item = self.buildMaterialReceivedItem(values);
        self.editorData().Items.push(item);
      });
    },
    canExecute: function() {
      return self.isInit() && self.editorData() != null;
    }
  });

  function toDto() {
    var dto = ko.toJS(self.editorData());

    dto.DehydratorKey = dto.Supplier;
    dto.ChileProductKey = dto.Product;

    dto.ItemsReceived = ko.utils.arrayFilter(dto.Items, function (item) {
      if (item.Quantity > 0) {
        item.PackagingProductKey = item.PackagingProduct.ProductKey;
        item.GrowerCode = item.Grower;
        item.WarehouseLocationKey = item.Location.LocationKey;
        item.ToteKey = item.Tote;

        return true;
      }
    });

    return dto;
  }

  // Exports
  if (ko.isObservable(params.exports)) {
    params.exports({
      addItemCommand: self.addItemCommand,
      addVariety: addVariety,
      isDirty: this.isDirty,
      init: init,
      getValues: toDto,
      revertChanges: self.revertChanges,
      isValid: function () {
        return true;
        if (self.isValid().length > 0) {
          self.isValid.showAllMessages(true);
        }
      }
    });
  }
  return self;
}

DehydratedMaterialsDetailsVM.prototype.buildMaterialReceivedItem = function (values) {
  values.Tote = values.Tote || values.ToteKey;
  values.Grower = values.Grower || values.GrowerCode;
  values.Location = values.Location || this.findLocationByKey(values.LocationKey);
  values.PackagingProduct = values.PackagingProduct || this.findPackagingProductByKey(values.PackagingKey);

  var item = new DehydratedItemReceived(values),
    self = this;
  item.isEditing = self.isEditing;

  var isFirstItem = self.editorData() && self.editorData().Items().length === 0;
  item.toteHasFocus = values.Tote == null && !isFirstItem;
  item.varietyHasFocus = !(item.toteHasFocus) && values.Variety == null && !isFirstItem;
  item.growerHasFocus = !(item.toteHasFocus || item.varietyHasFocus) && (values.Grower == null || values.Grower === '') && !isFirstItem;
  item.warehouseHasFocus = !(item.toteHasFocus || item.varietyHasFocus || item.growerHasFocus) && (values.Location == null || values.Location === '') && !isFirstItem;
  item.packagingHasFocus = !(item.toteHasFocus || item.varietyHasFocus || item.growerHasFocus || item.warehouseHasFocus) && values.PackagingProduct == null && !isFirstItem;
  item.quantityHasFocus = !(item.toteHasFocus || item.varietyHasFocus || item.growerHasFocus || item.warehouseHasFocus || item.packagingHasFocus) && !isFirstItem;
  if (isFirstItem) { self.productionDateHasFocus = true; }

  item.Tote.extend({
    validation: {
      message: 'This tote has already been used!',
      validator: function () {
        return !isDuplicateTote();
      }
    }
  });

  return item;

  function isDuplicateTote(tote) {
    var results = 0;
    var editor = self.editorData();
    if (editor == null || editor.Items == null) {
      return false;
    }

    return ko.utils.arrayFirst(editor.Items(), function (item) {
      if (item.Tote() === tote) {
        results += 1;
      }
      return results > 1;
    }) !== null;
  }
};

DehydratedMaterialsDetailsVM.prototype.findPackagingProductByKey = function (keyValue) {
  var self = this;
  return ko.utils.arrayFirst(self.options.packagingProductOptions(), function (p) {
    if (p.ProductKey === keyValue) {
      return p;
    }
  });
};

DehydratedMaterialsDetailsVM.prototype.findLocationByKey = function (keyValue) {
  var self = this;
  return ko.utils.arrayFirst(self.options.warehouseLocations(), function (loc) {
    if (loc.LocationKey === keyValue) {
      return loc;
    }
  });
};

module.exports = {
  viewModel: DehydratedMaterialsDetailsVM,
  template: require('./dehydrated-materials-details.html')
};
