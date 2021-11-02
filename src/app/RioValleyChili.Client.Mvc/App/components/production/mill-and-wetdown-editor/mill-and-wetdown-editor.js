var productsService = require('App/services/productsService');
var warehouseService = require('App/services/warehouseService');
var rvc = require('rvc');

ko.components.register('tote-picker', require('App/components/production/tote-inventory-picker/tote-inventory-picker'));
ko.components.register('other-materials-picker', require('App/components/production/other-input-materials-picker/other-input-materials-picker'));
ko.components.register('output-picker', require('App/components/production/output-picker/output-picker'));
ko.components.register('product-selector', require('App/components/common/product-selector/product-selector'));

require('Scripts/app/koBindings.js');
require('App/koExtensions.js');
require('App/helpers/koPunchesFilters.js');
require('App/helpers/koValidationHelpers.js');

ko.punches.enableAll();

function millWetdownEditorVM(params) {
  if (!(this instanceof millWetdownEditorVM)) { return new millWetdownEditorVM(params); }

  var self = this;

  this.isInit = params.isInit || ko.observable();

  var input = ko.toJS(params.input) || {};

  // Data
  this.lotKey = ko.observable(input.MillAndWetdownKey);
  this.defaults = params.defaults || {};
  this.isShowingComponents = ko.observable(true);
  this.headerText = ko.pureComputed(function() {
    var lot = this.lotKey();

    if (!lot) {
      return;
    } else if (lot === 'new') {
      return 'Enter New Mill & Wetdown Production';
    } else {
      return 'Mill & Wetdown for Lot ' + lot;
    }
  }, this);

  // Editor model
  this.exports = {
    totes: ko.observable(),
    otherMats: ko.observable(),
    outputs: ko.observable(),
  };

  this.options = {
    productionLineOptions: ko.observableArray([]),
    chileProductOptions: ko.observableArray([]),
    packagingOptions: ko.observableArray([]),
    warehouseOptions: ko.observableArray([])
  };

  this.displayData = params.input;
  this.totalTime = ko.pureComputed(function() {
    var minutes = ko.unwrap(params.input).TotalProductionTimeMinutes;

    return minutes * 60;
  });

  function getTime() {
    var currentTime = Date.now();
    currentTime.setHours(0);
    currentTime.setMinutes(0);
    currentTime.setSeconds(0);
    currentTime.setMilliseconds(0);

    return currentTime;
  }

  function buildShiftKeyComputed(initialStr) {
    var _key = ko.observable(initialStr).extend({ notify: 'always' });
    var computed = ko.computed({
      read: function() {
        return _key();
      },
      write: function(val) {
        _key(String(val || "").toUpperCase());
      }
    }).extend({ notify: 'always' });

    return computed;
  }

  function parseDate(dateStr, timeStr) {
    if (dateStr && timeStr) {
      var dateVals = dateStr.split('/');
      var timeVals = timeStr.split(":");

      return new Date(+dateVals[2], (+dateVals[0]) - 1, +dateVals[1], +timeVals[0], +timeVals[1], 0);
    }
  }
  var timeAtInit = getTime().toISOString();
  this.editorData = {
    OutputChileLotKey: ko.observable(input.OutputChileLotKey).extend({ lotKey: true }),
    ShiftKey: buildShiftKeyComputed(input.ShiftKey),
    ProductionLineKey: ko.observable(input.ProductionLineKey),

    ProductionBeginDate: ko.observableDate(timeAtInit),
    ProductionBeginTime: ko.observableTime(timeAtInit),
    ProductionBegin: ko.pureComputed(function() {
      var dateObj = parseDate(self.editorData.ProductionBeginDate(), self.editorData.ProductionBeginTime());

      return dateObj;
    }),

    ProductionEndDate: ko.observableDate(timeAtInit),
    ProductionEndTime: ko.observableTime(timeAtInit),
    ProductionEnd: ko.pureComputed(function() {
      var dateObj = parseDate(self.editorData.ProductionEndDate(), self.editorData.ProductionEndTime());

      return dateObj;
    }),

    ChileProductKey: ko.observable(input.ChileProductKey),

    TotalTime: ko.pureComputed(function() {
      var start = new Date(this.editorData.ProductionBegin());
      var end = new Date(this.editorData.ProductionEnd());
      var startInSeconds = Math.floor(start.getTime() / 1000 / 60) * 60;
      var endInSeconds = Math.floor(end.getTime() / 1000 / 60) * 60;
      var timeDiff = endInSeconds - startInSeconds;

      return (endInSeconds - startInSeconds);

    }, self),
    dehyInput: ko.observableArray([]),
    otherMatsInput: ko.observableArray([]),
    outputInput: ko.observableArray([])
  };

  function getChileProductOption( productKey ) {
    var _product = ko.utils.arrayFirst( self.options.chileProductOptions(), function( opt ) {
      return opt.ProductKey === productKey;
    });

    return _product;
  }

  if ( input.ChileProductKey ) {
    self.editorData.ChileProductKey( getChileProductOption( input.ChileProductKey ) );
  }

  this.validation = ko.validatedObservable({
    OutputChileLotKey: self.editorData.OutputChileLotKey.extend({ required: true }),
    ShiftKey: self.editorData.ShiftKey.extend({ required: true }),
    ProductionLineKey: self.editorData.ProductionLineKey.extend({ required: true }),
    ChileProductKey: self.editorData.ChileProductKey.extend({ required: true }),
    TotalTime: self.editorData.TotalTime.extend({ min: 0 }),
    ProductionBeginDate: self.editorData.ProductionBeginDate.extend({ required: true }),
    ProductionBeginTime: self.editorData.ProductionBeginTime.extend({ required: true }),
    ProductionEndDate: self.editorData.ProductionEndDate.extend({ required: true }),
    ProductionEndTime: self.editorData.ProductionEndTime.extend({ required: true }),
  });

  this.isNew = ko.pureComputed(function() {
    return ko.unwrap(self.lotKey) === 'new';
  });

  // Computed data
  this.isEditing = ko.computed(function() {
    var editing = ko.unwrap(params.isEditing);

    return editing == undefined ? true : editing;
  });

  this.totalPickedWeight = ko.pureComputed(function() {
    var total = 0;
    var isEditing = self.isEditing();
    var pickedItems = isEditing ?
      ko.unwrap(this.editorData().PickedItems) :
      ko.unwrap(this.displayData().PickedItems);

    ko.utils.arrayForEach(pickedItems, function(item) {
      total += ko.unwrap(item.TotalWeightPicked);
    });

    return total;
  }, this);

  this.totalOutputWeight = ko.pureComputed(function() {
    var total = 0;
    var isEditing = self.isEditing();
    var resultItems = isEditing ? ko.unwrap(this.editorData().ResultItems) : ko.unwrap(this.displayData().ResultItems);

    ko.utils.arrayForEach(resultItems, function(item) {
      total += ko.unwrap(item.TotalWeightProduced);
    });

    return total;
  }, this);

  this.hasData = ko.pureComputed(function() {
    return !!(this.lotKey());
  }, this);

  // Behaviors
  function init() {
    var getChileProductOptions = productsService.getChileProducts().then(
      function(data, textStatus, jqXHR) {
        self.options.chileProductOptions(data);
    });

    var getProductLineOptions = productsService.getProductionLocations().then(
      function(data, textStatus, jqXHR) {
        self.options.productionLineOptions(data);
    });

    var getPackagingProductOptions = productsService.getPackagingProducts().then(
      function(data, textStatus, jqXHR) {
        self.options.packagingOptions(data);
    });

    var getWarehouseOptions = warehouseService.getWarehouseDetails('2').then(
      function(data, textStatus, jqXHR) {
        self.options.warehouseOptions(data.Locations);
    });

    var checkOptions = $.when(getChileProductOptions,
                              getProductLineOptions,
                              getPackagingProductOptions,
                              getWarehouseOptions)
    .done(function(chileProducts, productionLines, packaging, warehouses) {
      self.isInit(true);
    });

    return checkOptions;
  }

  function buildDto() {
    var isValid = self.validation.isValid();
    var totes = self.exports.totes().toDto();
    var otherMats = self.exports.otherMats().toDto();
    var outputs = self.exports.outputs().toDto();
    var lotKey = self.editorData.OutputChileLotKey();
    var chileProductKey = (self.editorData.ChileProductKey() || {}).ProductKey;

    if (isValid && totes && otherMats && outputs) {
      var data = {
        LotKey: lotKey.replace(/\s/g, ''),
        ProductionDate: new Date(self.editorData.OutputChileLotKey.formattedDate()).toISOString(),
        LotSequence: lotKey.split(' ')[3],
        ShiftKey: self.editorData.ShiftKey,
        ChileProductKey: chileProductKey,
        ProductionLineKey: self.editorData.ProductionLineKey,
        ProductionBegin: new Date(self.editorData.ProductionBegin()).toISOString(),
        ProductionEnd: new Date(self.editorData.ProductionEnd()).toISOString(),
      };

      data.PickedItems = totes.concat(otherMats);
      data.ResultItems = outputs;

      return ko.toJS(data);
    } else {
      return null;
    }
  }

  function reset() {
    self.editorData.OutputChileLotKey(null);
    self.editorData.ShiftKey(null);
    self.editorData.ProductionLineKey(null);
    self.editorData.ProductionBeginDate(getTime());
    self.editorData.ProductionBeginTime(getTime());
    self.editorData.ProductionEndDate(getTime());
    self.editorData.ProductionEndTime(getTime());
    self.editorData.ChileProductKey(null);
    self.editorData.dehyInput(null);
    self.editorData.otherMatsInput(null);
    self.editorData.outputInput(null);
  }

  function complete() {
    var lotKey = self.editorData.OutputChileLotKey();
    var lotKeySplit = lotKey.split(' ');
    var newSequence = (+lotKeySplit[3] + 1);

    if (newSequence < 10) {
      newSequence = "".concat("0", newSequence);
    } else {
      newSequence = "".concat(newSequence);
    }

    var newLotKey = [lotKeySplit[0], lotKeySplit[1], lotKeySplit[2], newSequence].join(' ');
    self.editorData.OutputChileLotKey(newLotKey);
    self.editorData.ProductionBeginTime("00:00");
    self.editorData.ProductionEndTime("00:00");

    self.isShowingComponents(false);
    self.isShowingComponents(true);
  }

  init();

  // Update editor data when input param changes
  var loadNewInput = params.input.subscribe(function(input) {
    var data = input ? ko.toJS(input) : {};
    var key = data.MillAndWetdownKey;

    reset();

    this.lotKey(key);

    if (data && key && key !== 'new') {
      this.editorData.OutputChileLotKey(key);
      this.editorData.ShiftKey(data.ShiftKey);
      this.editorData.ProductionLineKey(data.ProductionLineKey);
      this.editorData.ProductionBeginDate(data.ProductionBegin);
      this.editorData.ProductionBeginTime(data.ProductionBegin);
      this.editorData.ProductionEndDate(data.ProductionEnd);
      this.editorData.ProductionEndTime(data.ProductionEnd);
      this.editorData.ChileProductKey( getChileProductOption( data.ChileProductKey ) );
      this.editorData.dehyInput(ko.utils.arrayFilter(data.PickedItems, function(item) {
        return !!(item.ToteKey);
      }));
      this.editorData.otherMatsInput(ko.utils.arrayFilter(data.PickedItems, function(item) {
        return item.hasOwnProperty('ToteKey') && item.ToteKey === '';
      }));
      this.editorData.outputInput(data.ResultItems);

      // Forces computed to update
      this.editorData.OutputChileLotKey.formattedDate();
    }
  }, this);

  // Exports
  if (params && params.exports) {
    params.exports({
      isInit: self.isInit,
      toDto: buildDto,
      complete: complete,
      resetUI: reset
    });
  }

  return this;
}

module.exports = {
  viewModel: millWetdownEditorVM,
  template: require('./mill-and-wetdown-editor.html'),
  synchronous: true
};
