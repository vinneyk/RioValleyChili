webpackJsonp([11],{

/***/ 0:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {var rvc = __webpack_require__(8);
	var inventoryService = __webpack_require__(77);
	var page = __webpack_require__(26);

	page.base('/Production/MillAndWetdown');

	__webpack_require__(78);

	ko.components.register('mw-summary', __webpack_require__(145));
	ko.components.register('mw-editor', __webpack_require__(147));
	ko.components.register('loading-screen', __webpack_require__(91));

	function MillAndWetdownVM() {
	  if (!(this instanceof MillAndWetdownVM)) { return new MillAndWetdownVM(params); }

	  var self = this;

	  this.isInit = ko.observable(false);

	  // Data
	  var dataPager = inventoryService.getMillAndWetdownPager();

	  this.moreDetailsItem = ko.observable();
	  this.searchKey = ko.observable().extend({ lotKey: true });

	  this.isEditing = ko.observable();
	  this.isEditingEnabled = ko.observable();

	  this.recentLots = ko.observableArray();
	  this.currentLot = ko.observable();
	  this.isCurrentLot = ko.pureComputed(function(key) {
	    return key === self.currentLot();
	  });
	  this.defaults = {
	    ethoxLot: ko.observable(),
	    sipernatLot: ko.observable(),
	  };

	  var scrollToItem = ko.observable(false);
	  var recentLotsBacking = {};
	  var lastMillAndWetdownRecord = null;
	  var sipernatPattern = /sipernat/i;
	  var ethoxPattern = /ethoxiquin/i;

	  this.filters = {
	    startDate: ko.observable(),
	    endDate: ko.observable(),
	  };

	  this.isSummaryInit = ko.observable(false);
	  this.summaryData = ko.observableArray([]);
	  this.summaryDataPager = inventoryService.getMillAndWetdownPager({
	    onNewPageSet: function() {
	      self.summaryData([]);
	    },
	  });
	  this.summaryDataPager.addParameters(self.filters);
	  this.summaryExports = ko.observable();

	  this.isEditorInit = ko.observable(false);
	  this.editorData = ko.observable(null);
	  this.editorExports = ko.observable();
	  this.isEditorShowing = ko.pureComputed(function() {
	    return this.editorData() !== null;
	  }, this);


	  this.goToKey = function(lotKey) {
	    var key = typeof lotKey === 'string' ? lotKey : self.searchKey();

	    key = key.replace(/\s/g, '');

	    page('/' + key);
	  };

	  this.applyFiltersCommand = ko.asyncCommand({
	    execute: function(complete) {
	      self.summaryDataPager.resetCursor();
	      getNextSummaryPage().always(complete);
	    },
	    canExecute: function(isExecuting) {
	      return self.isSummaryInit() && !isExecuting;
	    }
	  });

	  this.clearFiltersCommand = ko.command({
	    execute: function() {
	      self.filters.startDate(null);
	      self.filters.endDate(null);
	    },
	    canExecute: function() {
	      return true;
	    }
	  });

	  // Computed Data
	  this.isNewRecord = ko.pureComputed(function() {
	    var lot = this.currentLot();

	    return lot === 'new';
	  }, this);

	  // Behaviors
	  function getDefaults() {
	    var lots = self.summaryData;

	    var getRecentEntries = getNextSummaryPage().then(
	    function(data, textStatus, jqXHR) {
	      if (data.length) {
	        setDefaultsFromEntry(data[0]);
	      }
	    });

	    function setDefaultsFromEntry(entry) {
	      inventoryService.getMillAndWetdownDetails(entry.OutputChileLotKey).then(
	        function(data, textStatus, jqXHR) {
	        var result = ko.utils.arrayFirst(data.PickedItems || [], function(pickedItem) {
	          if (!self.defaults.ethoxLot() && isEthoxiquin(pickedItem)) {
	            self.defaults.ethoxLot(pickedItem.LotKey);
	          } else if (!self.defaults.sipernatLot() && isSipernat(pickedItem)) {
	            self.defaults.sipernatLot(pickedItem.LotKey);
	          }

	          return self.defaults.ethoxLot() && self.defaults.sipernatLot();
	        });
	      });

	      function isSipernat(item) {
	        return isDefaultProduct.call(item, additiveTypeSelector, sipernatNameSelector, sipernatPattern);

	        function sipernatNameSelector() {
	          return this.Product.ProductName;
	        }
	      }
	      function isEthoxiquin(item) {
	        return isDefaultProduct.call(item, additiveTypeSelector, ethoxNameSelector, ethoxPattern);

	        function ethoxNameSelector() {
	          return this.Product.ProductName;
	        }
	      }
	      function additiveTypeSelector() {
	        return this.Product.ProductType;
	      }
	    }
	  }

	  function isDefaultProduct(productTypeSelector, productNameSelector, expression) {
	    return productTypeSelector.call(this) === rvc.lists.inventoryTypes.Additive.key &&
	      productNameSelector.call(this).match(expression);
	  }
	  function checkPickedItemForDefaults(pickedItem) {
	    if (!pickedItem) return;

	    if (isDefaultProduct.call(pickedItem, productTypeSelector, productNameSelector, sipernatPattern)) {
	      replaceDefault(defaultSipernatLot, "Sipernat", pickedItem.LotKey);
	    } else if (isDefaultProduct.call(pickedItem, productTypeSelector, productNameSelector, ethoxPattern)) {
	      replaceDefault(defaultEthoxLot, "Ethoxiquin", pickedItem.LotKey);
	    }

	    function productTypeSelector() {
	      return this.ProductType;
	    }
	    function productNameSelector() {
	      return this.ProductName;
	    }
	    function replaceDefault(defaultVariable, name, lotNumber) {
	      var lastDefault = defaultVariable();
	      if (!lastDefault) {
	        defaultVariable(lotNumber);
	        return;
	      }
	      if (lastDefault === lotNumber) { return; }

	      showUserMessage("Update default " + name + " lot?", {
	        description: "Would you like to update the default <strong>" + name + "</strong> lot to <strong>" + lotNumber + "</strong>? Click \"Yes\" to update the default value. Click \"No\" to leave the default lot as " + lastDefault + ".",
	        type: 'yesno',
	        onYesClick: function () {
	          defaultVariable(lotNumber);
	        }
	      });
	    }

	  }
	  function getNextSummaryPage() {
	    return self.summaryDataPager.nextPage().then(
	    function(data, textStatus, jqXHR) {
	      var lots = self.summaryData;

	      lots(lots().concat(data));

	      return lots();
	    },
	    function(jqXHR, textStatus, errorThrown) {
	      showUserMessage('Could not fetch next page', { description: errorThrown });
	    });
	  }

	  function toDto() {
	    return {
	      ProductionDate: model.OutputChileLotKey.Date(),
	      LotSequence: model.OutputChileLotKey.Sequence(),
	      ShiftKey: model.ShiftKey(),
	      ChileProductKey: model.ChileProductKey(),
	      ProductionLineKey: model.ProductionLineKey(),
	      ProductionBegin: model.ProductionBegin(),
	      ProductionEnd: model.ProductionEnd(),
	      ResultItems: model.outputInventoryViewModel.toDto(),
	      PickedItems: inputs,
	    };
	  }

	  this.loadDetailsStatus = {
	    loading: ko.observable(false),
	    message: "Loading lot details...",
	  };

	  function getDetails(key) {
	    self.loadDetailsStatus.loading(true);
	    var fetchDetails = inventoryService.getMillAndWetdownDetails(key).then(function(data, textStatus, jqXHR) {
	      return data;
	    })
	    .fail(function(jqXHR, textStatus, errorThrown) {
	      showUserMessage('Unable to load lot ' + key, { description: errorThrown });
	      return arguments;
	    })
	    .always(function() {
	      self.loadDetailsStatus.loading(false);
	    });

	    return fetchDetails;
	  }

	  function addToRecentLots(newLotKey) {
	    var lots = self.recentLots();
	    var lotIndex = lots.indexOf(newLotKey);

	    if (lotIndex > -1) {
	      self.recentLots.splice(lotIndex, 1);
	    }

	    self.recentLots([newLotKey].concat(lots));

	    if (lots.length >= 5) {
	      self.recentLots.pop();
	    }
	  }

	  this.getSummaryItemsCommand = ko.asyncCommand({
	    execute: function(complete) {
	      getNextSummaryPage().done(function(data, textStatus, jqXHR) {
	      })
	      .always(complete);
	    },
	    canExecute: function(isExecuting) {
	      return self.isSummaryInit() && !isExecuting;
	    }
	  });

	  this.saveEntry = ko.asyncCommand({
	    execute: function(complete) {
	      var editor = self.editorExports();
	      var editorData = editor.toDto();
	      var isNew = self.isNewRecord();

	      var saveData = self.save(editorData).then(
	      function(data, textStatus, jqXHR) {
	        if (isNew) {
	          addToRecentLots(editorData.LotKey);
	          editor.complete();
	        }
	      })
	      .always(complete);
	    },
	    canExecute: function(isExecuting) {
	      return true;
	    }
	  });

	  this.deleteEntry = ko.asyncCommand({
	    execute: function(complete) {
	      var editor = self.editorData();
	      var lotKey = editor && editor.MillAndWetdownKey;

	      showUserMessage( 'Delete Mill & Wetdown record?', {
	        description: 'Are you sure you want to delete this record? This action cannot be undone.',
	        type: 'yesno',
	        onYesClick: function() {
	          var deleteData = self.deleteLot( lotKey )
	          .done(function( data, textStatus, jqXHR ) {
	            page('/');
	          })
	          .fail(function( jqXHR, textStatus, errorThrown ) {
	            showUserMessage( 'Delete failed', {
	              description: errorThrown
	            });
	          })
	          .always( complete );

	          return deleteData;
	        },
	        onNoClick: function() {
	          complete();
	        },
	      });
	    },
	    canExecute: function(isExecuting) {
	      return !isExecuting;
	    }
	  });

	  this.initNewRecordCommand = ko.command({
	    execute: function() {
	      page('/new');
	    },
	    canExecute: function() {
	      return true;
	    }
	  });

	  this.editCommand = ko.command({
	    execute: function() {
	      self.isEditing(true);
	    },
	    canExecute: function() {
	      return self.isEditingEnabled();
	    }
	  });

	  this.cancelEditCommand = ko.command({
	    execute: function() {
	      self.isEditing(false);
	    },
	    canExecute: function() {
	      return self.isEditing();
	    }
	  });

	  this.closePopupCommand = ko.command({
	    execute: function() {
	      page('/');
	    },
	    canExecute: function() {
	      return true;
	    }
	  });

	  page('/:key?', navigateToLot);
	  function navigateToLot(ctx) {
	    var key = ctx.params.key;
	    var editorData = self.editorData;
	    var editor = self.editorExports();

	    self.currentLot(key);
	    self.isEditingEnabled(false);

	    if (!key) {
	      self.searchKey(null);
	      editorData(null);
	      self.isEditing(false);
	    } else if (key === 'new') {
	      self.searchKey(null);
	      self.isEditingEnabled(true);
	      self.isEditing(true);
	      editorData({
	        MillAndWetdownKey: 'new'
	      });
	    } else if (key) {
	      self.isEditing(false);
	      getDetails(key).done(function(data) {
	        addToRecentLots(key);
	        self.isEditingEnabled(data.enabledEditing);
	        self.isEditingEnabled(true);
	        editorData(data);
	      });
	    }
	  }

	  this.addSummaryItem = function(data) {
	    var summaryItems = self.summaryData();
	    var matchingEntry = ko.utils.arrayFirst(summaryItems, function(item) {
	      return item.OutputChileLotKey === data.OutputChileLotKey;
	    });

	    if (matchingEntry) {
	      var i = summaryItems.indexOf(matchingEntry);
	      self.summaryData.splice(i, 1, data);
	    } else {
	      self.summaryData([data].concat(summaryItems));
	    }
	  };

	  this.removeSummaryItem = function( lotKey ) {
	    var summaryItems = self.summaryData();

	    var matchingEntry = ko.utils.arrayFirst( summaryItems, function( item ) {
	      return item.OutputChileLotKey === lotKey;
	    });

	    if ( matchingEntry ) {
	      var i = summaryItems.indexOf( matchingEntry );

	      self.summaryData.splice( i, 1 );
	    }
	  }

	  this.saveStatus = {
	    saving: ko.observable(false),
	    message: "Saving Mill & Wetdown record..."
	  };

	  // Init
	  // Fetches from summary items, loads inital page for summary view
	  getDefaults();

	  // Waits for all sub-components to load before initializing
	  ko.computed({
	    read: function() {
	      var editor = self.isEditorInit();
	      var summary = self.isSummaryInit();

	      if (editor && summary) {
	        self.isInit(true);
	        page();
	      }
	    },
	    disposeWhen: function() {
	      return self.isInit();
	    }
	  });

	  // Exports
	  return this;
	}

	MillAndWetdownVM.prototype.save = function(data) {
	  var self = this;

	  var isNew = this.isNewRecord();
	  var lotKey = data && data.LotKey || "";
	  var lotKeyFormatted = [lotKey.substr(0,2), lotKey.substr(2,2), lotKey.substr(4,3), lotKey.substr(7)].join(' ');

	  if (data) {
	    self.saveStatus.saving(true);
	    if (isNew) {
	      var saveNewData = inventoryService.createMillAndWetdownEntry(data).then(
	        function(data, textStatus, jqXHR) {
	        showUserMessage("Save Successful", { description: "".concat("<b>", lotKeyFormatted, "</b> has been created.") });
	      },
	      function(jqXHR, textStatus, errorThrown) {
	        showUserMessage("Save Failed", { description: errorThrown });
	      });

	      var addNewSummary = saveNewData.then(function() {
	        return inventoryService.getMillAndWetdownDetails(lotKey).then(
	          function(data, textStatus, jqXHR) {
	            self.addSummaryItem(data);
	        });
	      });

	      saveNewData.always(function() {
	        self.saveStatus.saving(false);
	      });

	      return saveNewData;
	    } else {
	      var saveData = inventoryService.updateMillAndWetdownEntry(lotKey, data).then(
	        function(data, textStatus, jqXHR) {
	        showUserMessage("Save Successful", { description: "".concat("<b>", lotKeyFormatted, "</b> has been updated.") });
	      },
	      function(jqXHR, textStatus, errorThrown) {
	        showUserMessage("Save Failed", { description: errorThrown });
	      });

	      var addSummary = saveData.then(function() {
	        return inventoryService.getMillAndWetdownDetails(lotKey).then(
	          function(data, textStatus, jqXHR) {
	            self.addSummaryItem(data);
	        });
	      });

	      saveData.always(function() {
	        self.saveStatus.saving(false);
	      });

	      return saveData;
	    }
	  } else {
	    showUserMessage("Save Failed", { description: "Please fill out all required fields and try again" });

	    return $.Deferred().reject();
	  }
	};

	MillAndWetdownVM.prototype.deleteLot = function( lotKey ) {
	  var self = this;

	  var deleteData = inventoryService.deleteMillAndWetdownEntry( lotKey ).then(
	  function( data, textStatus, jqXHR ) {
	    // Remove from summary table
	    self.removeSummaryItem( lotKey );
	  });

	  return deleteData;
	};

	var vm = new MillAndWetdownVM();

	ko.applyBindings(vm);

	module.exports = vm;

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 6:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {var warehouseService = (function () {
	    var serviceCore = __webpack_require__(7);
	    var dataPager = __webpack_require__(10);

	    function getIntraWarehouseMovement() {
	        return serviceCore.ajax(buildIntraWarehouseAPIUrl(arguments[0]));
	    }
	    function createIntraWarehouseMovement(values) {
	        return serviceCore.ajaxPost(buildIntraWarehouseAPIUrl(), values);
	    }
	    function updateIntraWarehouseMovement(key, values) {
	        return serviceCore.ajaxPut(buildIntraWarehouseAPIUrl(key), values);
	    }

	    // Facilities
	    var FACILITY_BASE_URL = '/api/facilities/';
	    function getWarehouseDetails(key) {
	        return serviceCore.ajax( FACILITY_BASE_URL + key);
	    }
	    function getWarehouses() {
	        return serviceCore.ajax( FACILITY_BASE_URL );
	    }
	    function getFacilities() {
	      return serviceCore.ajax( FACILITY_BASE_URL );
	    }
	    function getFacilityDetails( facilityKey ) {
	      return serviceCore.ajax( FACILITY_BASE_URL + facilityKey );
	    }
	    function createFacility( facilityData ) {
	      return serviceCore.ajaxPost( FACILITY_BASE_URL, facilityData );
	    }
	    function updateFacility( facilityKey, facilityData ) {
	      if ( !facilityKey ) {
	        throw new Error('Updating facilities requires a key');
	      }

	      return serviceCore.ajaxPut( FACILITY_BASE_URL + facilityKey, facilityData );
	    }
	    function getFacilityLocations( facilityKey ) {
	      return serviceCore.ajax( FACILITY_BASE_URL + facilityKey + '/locations' );
	    }
	    function freezeStreet( facilityKey, street ) {
	      return serviceCore.ajaxPut( FACILITY_BASE_URL + facilityKey + '/lock?streetname=' + street );
	    }
	    function unfreezeStreet( facilityKey, street ) {
	      return serviceCore.ajaxPut( FACILITY_BASE_URL + facilityKey + '/unlock?streetname=' + street );
	    }

	    var FACILITY_LOCATION_BASE_URL = '/api/facilityLocations/';
	    function createLocation( locationData ) {
	      return serviceCore.ajaxPost( FACILITY_LOCATION_BASE_URL, locationData );
	    }
	    function updateLocation( locationKey, locationData ) {
	      return serviceCore.ajaxPut( FACILITY_LOCATION_BASE_URL + locationKey, locationData );
	    }
	    function getInterWarehouseMovementsDataPager(options) {
	        options = options || {};
	        options.urlBase = buildInterWarhouseUrl();
	        return dataPager.init(options);
	    }
	    function getInterWarehouseDetails(key) {
	        return serviceCore.ajax(buildInterWarhouseUrl(key));
	    }
	    function createInterWarehouseMovement(values) {
	        return serviceCore.ajaxPost(buildInterWarhouseUrl(), values);
	    }
	    function updateInterWarehouseMovement(key, values) {
	        return serviceCore.ajaxPut(buildInterWarhouseUrl(key), values);
	    }
	    function postAndCloseShipmentOrder(key, values) {
	        if(key == undefined) { throw new Error("Invalid argument. Expected key value."); }
	        return serviceCore.ajaxPost(buildShipmentOrderUrl(key) + '/postandclose', values);
	    }

	    function buildIntraWarehouseAPIUrl(key) {
	        return ["/api/IntraWarehouseInventoryMovements/", key || ''].join('');
	    }
	    function buildInterWarhouseUrl(key) {
	        return ['/api/InterWarehouseInventoryMovements/', key || ''].join('');
	    }
	    function buildShipmentOrderUrl(key) {
	        return ['/api/InventoryShipmentOrders/', key || ''].join('');
	    }

	    return {
	        getIntraWarehouseMovementDetails: getIntraWarehouseMovement,
	        getIntraWarehouseMovements: getIntraWarehouseMovement,
	        createIntraWarehouseMovement: createIntraWarehouseMovement,
	        updateIntraWarehouseMovement: updateIntraWarehouseMovement,
	        getWarehouseDetails: getWarehouseDetails,
	        getWarehouses: getWarehouses,
	        getFacilities: getFacilities,
	        getFacilityDetails: getFacilityDetails,
	        createFacility: createFacility,
	        updateFacility: updateFacility,
	        getFacilityLocations: getFacilityLocations,
	        createLocation: createLocation,
	        updateLocation: updateLocation,
	        freezeStreet: freezeStreet,
	        unfreezeStreet: unfreezeStreet,
	        getInterWarehouseMovementsDataPager: getInterWarehouseMovementsDataPager,
	        getInterWarehouseDetails: getInterWarehouseDetails,
	        createInterWarehouseMovement: createInterWarehouseMovement,
	        updateInterWarehouseMovement: updateInterWarehouseMovement,
	        postAndCloseShipmentOrder: postAndCloseShipmentOrder,
	    };
	})();

	module.exports = $.extend(warehouseService, __webpack_require__(11));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 11:
/***/ (function(module, exports, __webpack_require__) {

	var warehouseLocationsService = (function () {
	    var serviceCore = __webpack_require__(7);
	    var rvc = __webpack_require__(8);

	    function getWarehouseLocations(warehouseKey, options) {
	      return serviceCore.ajax(["/api/facilities/", warehouseKey, "/locations/"].join(''), options);
	    }

	    function updateWarehouseLocation(locationKey, values) {
	        return serviceCore.ajaxPut(buildWarehouseLocationsUrl(locationKey), values);
	    }

	    function createWarehouseLocation(values) {
	        return serviceCore.ajaxPost(buildWarehouseLocationsUrl(), values);
	    }

	    function getRinconWarehouseLocations() {
	        return getWarehouseLocations(rvc.lists.rinconWarehouse.WarehouseKey, arguments[0]);
	    }

	    function createWarehouse(values) {
	        return updateWarehouse(null, values);
	    }

	    function updateWarehouse(key, values) {
	        if (key) {
	            return serviceCore.ajaxPut(buildWarehouseUrl(key), values);
	        }
	        return serviceCore.ajaxPost(buildWarehouseUrl(key), values);
	    }

	    function buildWarehouseLocationsUrl(locationKey) {
	      return '/api/facilities/' + (locationKey || '');
	    }

	    function buildWarehouseUrl(key) {
	      return '/api/facilities/' + (key || '');
	    }

	    function freezeFacilityLocationsGroup(facilityKey, groupName) {
	        return serviceCore.ajaxPut(buildWarehouseUrl(facilityKey) + '/street/' + encodeURI(groupName) + '/lock');
	    }

	    function unfreezeFacilityLocationsGroup(facilityKey, groupName) {
	        return serviceCore.ajaxPut(buildWarehouseUrl(facilityKey) + '/street/' + encodeURI(groupName) + '/unlock');
	    }

	    return {
	        getRinconWarehouseLocations: getRinconWarehouseLocations,
	        getWarehouseLocations: getWarehouseLocations,
	        updateWarehouseLocation: updateWarehouseLocation,
	        createWarehouseLocation: createWarehouseLocation,
	        createWarehouse: createWarehouse,
	        updateWarehouse: updateWarehouse,
	        freezeFacilityLocationsGroup: freezeFacilityLocationsGroup,
	        unfreezeFacilityLocationsGroup: unfreezeFacilityLocationsGroup
	    }
	})();

	module.exports = warehouseLocationsService;

/***/ }),

/***/ 18:
/***/ (function(module, exports, __webpack_require__) {

	var rvc = __webpack_require__(8);

	var koPunches = (function () {
	  var self = this;

	  // Behaviors
	  ko.filters.toDate = function (value) {
	    if ( value == null ) {
	      return null;
	    }

	    var dateStr = null,
	      input = new Date(ko.unwrap(value));

	      var month = (input.getUTCMonth() + 1).toString();
	      month = month.length === 2 ? month : "0" + month;

	      var day = input.getUTCDate().toString();
	      day = day.length === 2 ? day : "0" + day;

	    dateStr = month + '/' + day + '/' + input.getUTCFullYear();

	    return dateStr;
	  };

	  ko.filters.toDateTime = function ( value, format ) {
	    var dateObj;

	    if (typeof value === "string") {
	      dateObj = new Date( value );
	    }

	    if ( !(dateObj instanceof Date) ) {
	      throw new Error( 'Invalid input. Expected date but encountered ' + (typeof input) + '.' );
	    }

	    return dateObj.format( format || 'm/d/yyyy hh:MM TT' );
	  };

	  ko.filters.lotKey = function (input) {
	    var value = ko.unwrap(input);

	    if (value == undefined) {
	      return '';
	    }

	    var key = value.toString().replace(/ /g, '');
	    var keyLength = key.length;

	    if (keyLength === 0) {
	      return '';
	    } else if (keyLength <= 2) {
	      return key;
	    } else if (keyLength <= 4) {
	      return [key.substr(0, 2), key.substr(2)].join(' ');
	    } else if (keyLength <= 7) {
	      return [key.substr(0, 2), key.substr(2, 2), key.substr(4,3)].join(' ');
	    } else {
	      return [key.substr(0, 2), key.substr(2, 2), key.substr(4,3), key.substr(7)].join(' ');
	    }
	  };

	  ko.filters.toteKey = function (input) {
	    var value = ko.unwrap(input);

	    if (value == undefined) {
	      return '';
	    }

	    var key = value.toString().replace(/ /g, '');
	    if (key.length === 0) {
	      return '';
	    } else if (key.length <= 2) {
	      return key;
	    } else if (key.length <= 4) {
	      return [key.substr(0, 2), key.substr(2)].join(' ');
	    } else {
	      return [key.substr(0, 2), key.substr(2, 2), key.substr(4)].join(' ');
	    }
	  };

	    ko.filters.secToHrMin = function(value) {
	      var valueNum = +ko.unwrap(value);
	      var isNegative = valueNum < 0;

	      // Parse as positive number, Math.floor rounds negative numbers down
	      // Ex: -0.2 = -1
	      if (!isNaN(valueNum)) {
	        var secondsTotal = isNegative ? -valueNum : valueNum;
	        var hours = Math.floor(secondsTotal / 3600);
	        var minutes = Math.floor((secondsTotal - (3600 * hours)) / 60);

	        return isNegative ?
	          "".concat('-', hours, 'h ', minutes, 'm') :
	          "".concat(hours, 'h ', minutes, 'm');
	      } else {
	        return '0m';
	      }
	    };

	  ko.filters.USD = function(value) {
	    var amt = parseFloat(ko.unwrap(value));

	    return typeof amt === 'number' ?
	      '$' + amt.toFixed(2) :
	      '';
	  };

	  ko.filters.toFixed = function(value, numOfDigits) {
	    var amt = parseFloat(ko.unwrap(value));

	    return typeof amt === 'number' ?
	      amt.toFixed(numOfDigits || 2) :
	      '';
	  };

	  ko.filters.toNumber = function( value ) {
	    var numValue = value != null ? +value : null;

	    if ( numValue != null ) {
	      return numValue.toLocaleString();
	    } else {
	      return null;
	    }
	  };

	  ko.filters.contractStatus = function ( value ) {
	    var input = ko.unwrap( value );
	    var statuses = rvc.lists.contractStatuses;
	    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
	      return statuses[ status ].key === input;
	    });

	    return statuses[ statusKey ].value;
	  };

	  ko.filters.orderStatus = function ( value ) {
	    var input = ko.unwrap( value );
	    var statuses = rvc.lists.orderStatus;
	    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
	      return statuses[ status ].key === input;
	    });

	    return statuses[ statusKey ].value;
	  };

	  ko.filters.sampleStatus = function( value ) {
	    var input = ko.unwrap( value );
	    var statuses = rvc.lists.sampleStatusTypes;
	    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
	      return statuses[ status ].key === input;
	    });

	    return statuses[ statusKey ].value;
	  };

	  ko.filters.statusType = function( value, statusName ) {
	    var input = ko.unwrap( value );

	    if ( input == null ) {
	      return;
	    }

	    var statuses = rvc.lists[ statusName ];
	    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
	      return statuses[ status ].key === input;
	    });

	    return statusKey != null ? statuses[ statusKey ].value : null;
	  };

	  ko.filters.name = function( value ) {
	    var input = ko.unwrap( value );

	    return input && input.Name;
	  };

	  ko.filters.length = function( value ) {
	    if ( typeof value === 'string' || Array.isArray( value ) ) {
	      return value.length;
	    }

	    return 0;
	  };

	  // Exports
	  return this;
	})();

	module.exports = koPunches;


/***/ }),

/***/ 20:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {/**
	  * @param {Object[]} productsSource - Observable, Product options
	  * @param {string} selectedValue - Observable, Selected option
	  * @param {string} optionsDisplay - Property to use for text display
	  * @param {string} optionsValue - Property to use for object value
	  * @param {string} loading - Observable, Toggles input based on status
	  * @param {string} lotType - Observable, optional, enables use of the component without the requirement of providing the source products list and configurations.
	  * @param {bool} disabled - Observable, optional, enables control of the disabled state
	  */

	__webpack_require__(21);
	__webpack_require__(22);
	__webpack_require__(23);
	var productsService = __webpack_require__(24);
	var productsCache = {};

	function ProductSelectorVM(params) {
	  if (!(this instanceof ProductSelectorVM)) { return new ProductSelectorVM(params); }

	  var self = this;
	  var disposables = [];

	  var options = $.extend({}, self.DEFAULT_OPTIONS, params);

	  // Data
	  /** Init options and data */
	  this.options = ko.isObservable( options.productsSource ) ?
	    options.productsSource :
	    ko.observableArray( options.productsSource || [] );
	  this.optionsDisplay = options.optionsDisplay;
	  this.optionsValue = options.optionsValue;
	  this.selectorValue = options.selectedValue;
	  this.loading = options.loading;
	  this.controlId = options.controlId;
	  this.disabled = options.disabled || false;
	  this.enabled = options.enabled || true;

	  var init = this.initAsync(options);

	  init.done(function () {
	    // convert initially selected object into JS object instance
	    var initialValue = options.selectedValue.peek() || null;
	    var valueMember = ko.unwrap(options.optionsValue);
	    if (initialValue != null && valueMember == null) {
	      initialValue = ko.utils.arrayFirst(self.options(), function findItem(opt) {
	        return opt.ProductKey === initialValue.ProductKey;
	      });
	    }
	    self.selectorValue(initialValue);
	  });

	  /** Toggles loading state */
	  this.isLoading = ko.pureComputed(function() {
	    return ko.unwrap(self.loading) || false;
	  });

	  if (ko.isObservable(params.lotType)) {
	    params.lotType.subscribe(function(val) {
	      self.loadProductsByType(val);
	    });
	  }
	  self.loadProductsByType(ko.unwrap(params.lotType));

	  if ( ko.isObservable( options.selectedValue )) {
	    disposables.push( options.selectedValue.subscribe(function(val) {
	      if (ko.utils.arrayIndexOf(self.options(), val) > -1) {
	        return;
	      }

	      var valueMember = ko.unwrap(options.optionsValue);
	      if (valueMember == null && typeof val === "string") {
	        val = ko.utils.arrayFirst(self.options(), function(o) {
	          return o.ProductKey === val;
	        });
	      } else if (valueMember != null) {
	        val = ko.utils.arrayFirst(self.options(), function (o) {
	          return o[valueMember] === val;
	        });
	      } else {
	        val = null;
	      }

	      self.selectorValue(val);
	    }) );
	  }

	  this.dispose = function() {
	    ko.utils.arrayForEach(disposables, function(d) {
	      d.dispose();
	    });
	  };

	  return this;
	}

	module.exports = {
	  viewModel: ProductSelectorVM,
	  template: __webpack_require__(25)
	};

	ProductSelectorVM.prototype.DEFAULT_OPTIONS = {
	  optionsDisplay: 'ProductNameFull',
	  optionsValue: null,
	  loading: ko.observable(false),
	  controlId: null,
	  disabled: false
	};

	ProductSelectorVM.prototype.initAsync = function(options) {
	  var self = this;

	  if (options.productsSource == null) {
	    return self.loadProductsByType(options.lotType);
	  }

	  return $.Deferred().resolve();
	};

	ProductSelectorVM.prototype.loadProductsByType = function (lotType) {
	  lotType = ko.unwrap(lotType);
	  if (lotType == null) {
	    return $.Deferred().reject();
	  }

	  var self = this;
	  self.loading(true);

	  var cache = productsCache[lotType];
	  if (cache != null) {
	    self.options(cache);
	    self.loading(false);
	    return $.Deferred().resolve(cache);
	  }

	  return productsService.getProductsByLotType(ko.unwrap(lotType))
	    .done(function(data) {
	      self.options(data);
	      productsCache[lotType] = data;
	    })
	    .always(function() {
	      self.loading(false);
	    });
	};

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 21:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(jQuery) {/*** IMPORTS FROM imports-loader ***/
	var define = false;
	(function() {

	// knockout-jqautocomplete 0.4.4 | (c) 2016 Ryan Niemeyer |  http://www.opensource.org/licenses/mit-license
	;(function(factory) {
	    if (typeof define === "function" && define.amd) {
	        // AMD anonymous module
	        define(["knockout", "jquery", "jquery-ui/autocomplete"], factory);
	    } else {
	        // No module loader - put directly in global namespace
	        factory(window.ko, jQuery);
	    }
	})(function(ko, $) {
	    var JqAuto = function() {
	        var self = this,
	            unwrap = ko.utils.unwrapObservable; //support older KO versions that did not have ko.unwrap

	        //binding's init function
	        this.init = function(element, valueAccessor, allBindings, data, context) {
	            var existingSelect, existingChange,
	                options = unwrap(valueAccessor()),
	                config = {},
	                filter = typeof options.filter === "function" ? options.filter : self.defaultFilter;

	            //extend with global options
	            ko.utils.extend(config, self.options);
	            //override with options passed in binding
	            ko.utils.extend(config, options.options);

	            //get source from a function (can be remote call)
	            if (typeof options.source === "function" && !ko.isObservable(options.source)) {
	                config.source = function(request, response) {
	                    //provide a wrapper to the normal response callback
	                    var callback = function(data) {
	                        self.processOptions(valueAccessor, null, data, request, response);
	                    };

	                    //call the provided function for retrieving data
	                    options.source.call(context.$data, request.term, callback);
	                };
	            }
	            else {
	                //process local data
	                config.source = self.processOptions.bind(self, valueAccessor, filter, options.source);
	            }

	            //save any passed in select/change calls
	            existingSelect = typeof config.select === "function" && config.select;
	            existingChange = typeof config.change === "function" && config.change;

	            //handle updating the actual value
	            config.select = function(event, ui) {
	                if (ui.item && ui.item.actual) {
	                    options.value(ui.item.actual);

	                    if (ko.isWriteableObservable(options.dataValue)) {
	                        options.dataValue(ui.item.data);
	                    }
	                }

	                if (existingSelect) {
	                    existingSelect.apply(this, arguments);
	                }
	            };

	            //user made a change without selecting a value from the list
	            config.change = function(event, ui) {
	                if (!ui.item || !ui.item.actual) {
	                    options.value(event.target && event.target.value);

	                    if (ko.isWriteableObservable(options.dataValue)) {
	                        options.dataValue(null);
	                    }
	                }

	                if (existingChange) {
	                    existingChange.apply(this, arguments);
	                }
	            };

	            //initialize the widget
	            var widget = $(element).autocomplete(config).data("ui-autocomplete");

	            //render a template for the items
	            if (options.template) {
	                widget._renderItem = self.renderItem.bind(self, options.template, context);
	            }

	            //destroy the widget if KO removes the element
	            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
	                if (widget && typeof widget.destroy === "function") {
	                    widget.destroy();
	                    widget = null;
	                }
	            });
	        };

	        //the binding's update function. keep value in sync with model
	        this.update = function(element, valueAccessor) {
	            var propNames, sources,
	                options = unwrap(valueAccessor()),
	                value = unwrap(options && options.value);

	            if (!value && value !== 0) {
	                value = "";
	            }

	            // find the appropriate value for the input
	            sources = unwrap(options.source);
	            propNames = self.getPropertyNames(valueAccessor);

	            // if there is local data, then try to determine the appropriate value for the input
	            if ($.isArray(sources) && propNames.value) {
	                value = ko.utils.arrayFirst(sources, function (opt) {
	                        return opt[propNames.value] == value;
	                    }
	                ) || value;
	            }

	            if (propNames.input && value && typeof value === "object") {
	                element.value = value[propNames.input];
	            }
	            else {
	                element.value = value;
	            }
	        };

	        //if dealing with local data, the default filtering function
	        this.defaultFilter = function(item, term) {
	            term = term && term.toLowerCase();
	            return (item || item === 0) && ko.toJSON(item).toLowerCase().indexOf(term) > -1;
	        };

	        //filter/map options to be in a format that autocomplete requires
	        this.processOptions = function(valueAccessor, filter, data, request, response) {
	            var item, index, length,
	                items = unwrap(data) || [],
	                results = [],
	                props = this.getPropertyNames(valueAccessor);

	            //filter/map items
	            for (index = 0, length = items.length; index < length; index++) {
	                item = items[index];

	                if (!filter || filter(item, request.term)) {
	                    results.push({
	                        label: props.label ? item[props.label] : item.toString(),
	                        value: props.input ? item[props.input] : item.toString(),
	                        actual: props.value ? item[props.value] : item,
	                        data: item
	                    });
	                }
	            }

	            //call autocomplete callback to display list
	            response(results);
	        };

	        //if specified, use a template to render an item
	        this.renderItem = function(templateName, context, ul, item) {
	            var $li = $("<li></li>").appendTo(ul),
	                itemContext = context.createChildContext(item.data);

	            //apply the template binding
	            ko.applyBindingsToNode($li[0], { template: templateName }, itemContext);

	            //clean up
	            $li.one("remove", ko.cleanNode.bind(ko, $li[0]));

	            return $li;
	        };

	        //retrieve the property names to use for the label, input, and value
	        this.getPropertyNames = function(valueAccessor) {
	            var options = ko.toJS(valueAccessor());

	            return {
	                label: options.labelProp || options.valueProp,
	                input: options.inputProp || options.labelProp || options.valueProp,
	                value: options.valueProp
	            };
	        };

	        //default global options passed into autocomplete widget
	        this.options = {
	            autoFocus: true,
	            delay: 50
	        };
	    };

	    ko.bindingHandlers.jqAuto = new JqAuto();
	});

	}.call(window));
	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 22:
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_FACTORY__, __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/*** IMPORTS FROM imports-loader ***/
	var require = false;

	/* http://www.reddnet.net/knockout-js-extender-for-dates-in-iso-8601-format/ 
	 * Knockout extender for dates that are round-tripped in ISO 8601 format
	 *  Depends on knockout.js and date.format.js 
	 *  Includes extensions for the date object that: 
	 *      add Date.toISOString() for browsers that do not nativly implement it
	 *      replaces Date.parse() with version to supports ISO 8601 (IE and Safari do not)
	 *  Includes example of how to use the extended binding
	 */

	(function () {
	    if (require) {
	        !(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9)], __WEBPACK_AMD_DEFINE_FACTORY__ = (extendKo), __WEBPACK_AMD_DEFINE_RESULT__ = (typeof __WEBPACK_AMD_DEFINE_FACTORY__ === 'function' ? (__WEBPACK_AMD_DEFINE_FACTORY__.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__)) : __WEBPACK_AMD_DEFINE_FACTORY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));
	    } else {
	        extendKo(ko);
	    }
	    
	    function extendKo(ko) {
	        ko.extenders.isoDate = function (target, formatString) {
	            target.formattedDate = ko.computed({
	                read: function () {
	                    if (!target()) {
	                        return;
	                    }
	                    var dt = new Date(Date.parse(target()));
	                    // Modified to not change to UTC. Should not effect 
	                    // anything NOT using time formatting.
	                    return dt.format(formatString);
	                    //return dt.format(formatString, true);
	                },
	                write: function (value) {
	                    // Modified from original to enable setting value to null
	                    // previously, when the value was "falsey", assignment was 
	                    // skipped all together. -- VK 5/27/13
	                    if (!value) {
	                        target(null);
	                    } else {
	                        target(new Date(Date.parse(value)).toISOString());
	                    }
	                }
	            });

	            target.asDate = ko.computed(function () {
	                return new Date(target.formattedDate());
	            });

	            //initialize with current value
	            target.formattedDate(target());

	            //return the computed observable
	            return target;
	        };
	    }
	}());


	/** from the mozilla documentation (before they implemented the function in the browser)
	 * https://developer.mozilla.org/index.php?title=en/JavaScript/Reference/Global_Objects/Date&revision=65
	 */
	(function(Date) {
	    if (!Date.prototype.toISOString) {
	        Date.prototype.toISOString = function() {
	            function pad(n) {
	                return n < 10 ? '0' + n : n;
	            }
	            return this.getUTCFullYear() + '-' + pad(this.getUTCMonth() + 1) + '-' + pad(this.getUTCDate()) + 'T' + pad(this.getUTCHours()) + ':' + pad(this.getUTCMinutes()) + ':' + pad(this.getUTCSeconds()) + 'Z';
	        };
	    }
	}(Date));

	/**
	 * Date.parse with progressive enhancement for ISO 8601 <https://github.com/csnover/js-iso8601>
	 * © 2011 Colin Snover <http://zetafleet.com>
	 * Released under MIT license.
	 */
	(function(Date) {
	    var origParse = Date.parse,
	        numericKeys = [1, 4, 5, 6, 7, 10, 11];
	    Date.parse = function(date) {
	        var timestamp, struct, minutesOffset = 0;

	        // ES5 §15.9.4.2 states that the string should attempt to be parsed as a Date Time String Format string
	        // before falling back to any implementation-specific date parsing, so that’s what we do, even if native
	        // implementations could be faster
	        //              1 YYYY                2 MM       3 DD           4 HH    5 mm       6 ss        7 msec        8 Z 9 ±    10 tzHH    11 tzmm
	        if ((struct = /^(\d{4}|[+\-]\d{6})(?:-(\d{2})(?:-(\d{2}))?)?(?:T(\d{2}):(\d{2})(?::(\d{2})(?:\.(\d{3}))?)?(?:(Z)|([+\-])(\d{2})(?::(\d{2}))?)?)?$/.exec(date))) {
	            // avoid NaN timestamps caused by “undefined” values being passed to Date.UTC
	            for (var i = 0, k;
	            (k = numericKeys[i]); ++i) {
	                struct[k] = +struct[k] || 0;
	            }

	            // allow undefined days and months
	            struct[2] = (+struct[2] || 1) - 1;
	            struct[3] = +struct[3] || 1;

	            if (struct[8] !== 'Z' && struct[9] !== 'undefined') {
	                minutesOffset = struct[10] * 60 + struct[11];

	                if (struct[9] === '+') {
	                    minutesOffset = 0 - minutesOffset;
	                }
	            }

	            timestamp = Date.UTC(struct[1], struct[2], struct[3], struct[4], struct[5] + minutesOffset, struct[6], struct[7]);
	        }
	        else {
	            timestamp = origParse ? origParse(date) : NaN;
	        }

	        return timestamp;
	    };
	}(Date));


/***/ }),

/***/ 23:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {(function() {
	  ko.bindingHandlers.datePicker = {
	    init: function (element, valueAccessor, allBindings) {
	      $(element).wrap('<div class="input-group"></div>');
	      $(element).datepicker({
	        showOn: 'button',
	        buttonText: '<i class="fa fa-calendar"></i>',
	        changeMonth: true,
	        changeYear: true
	      }).next(".ui-datepicker-trigger")
	          .addClass("btn btn-default")
	          .attr( 'tabindex', '-1' )
	          .wrap('<span class="input-group-btn"></span>');

	      ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	        //todo: cleanup wrapper element
	        $(element).datepicker('destroy');
	      });

	      var value = valueAccessor();
	      if (ko.isObservable(value)) {
	        ko.bindingHandlers.value.init(element, valueAccessor, allBindings);
	      }
	    }
	  };
	}());

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 24:
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(7), __webpack_require__(8)], __WEBPACK_AMD_DEFINE_RESULT__ = function(core, app) {
	    function getProductsByInventoryType(inventoryType, options) {
	      options = options || {};
	      options = $.extend({
	        filterProductsWithInventory: false,
	        includeInactive: false
	      }, options);

	      var url = ['/api/products/', inventoryType].join('');

	      var qs = [];
	      if (options.lotType != null) {
	        qs.push('lotType=' + options.lotType);
	      }
	      if (options.filterProductsWithInventory) {
	        qs.push('filterProductsWithInventory=true');
	      }
	      if (options.includeInactive) {
	        qs.push('includeInactive=true');
	      }

	      if (qs.length > 0) {
	        url += '?' + qs.join('&');
	      }

	      return core.ajax(url, options);
	    }

	    function getProductsByLotType(lotType, options) {
	      var inventoryType = app.lists.lotTypes.findByKey(lotType).inventoryType.key;
	      options = options || {};
	      options.lotType = lotType;
	      return getProductsByInventoryType(inventoryType, options);
	    }

	    return {
	        getChileProducts: function (chileState) {
	            if (chileState && typeof chileState === "object") chileState = chileState.key;
	            return core.ajax(core.buildUrl(buildChileProductsUrl, chileState));
	        },
	        getPackagingProducts: function (options) { return getProductsByLotType(app.lists.lotTypes.Packaging.key, options); },
	        getProductTypeAttributes: function () {
	            return core.ajax("/api/productTypeAttributes");
	        },
	        getCustomerProducts: function( customerKey ) {
	          return core.ajax( '/api/customers/' + customerKey + '/productspecs' );
	        },
	        getCustomerProductDetails: function( customerKey, productKey ) {
	          return core.ajax( '/api/customers/' + customerKey + '/productSpecs/' + productKey );
	        },
	        createCustomerProductOverride: function( customerKey, productKey, overrideData ) {
	          return core.ajaxPost( '/api/customers/' + customerKey + '/productSpecs/' + productKey, overrideData );
	        },
	        deleteCustomerProductOverride: function( customerKey, productKey ) {
	          return core.ajaxDelete( '/api/customers/' + customerKey + '/productSpecs/' + productKey );
	        },
	        getProductDetails: core.setupFn(getProductDetails, buildProductUrl),
	        getProductsByLotType: getProductsByLotType,
	        getProductsByInventoryType: getProductsByInventoryType,
	        getChileVarieties: function() {
	          return core.ajax('/api/chilevarities');
	        },
	        getChileTypes: function () {
	            return core.ajax("/api/chileTypes");
	        },
	        getAdditiveTypes: function () {
	            return core.ajax("/api/additiveTypes");
	        },
	        getProductionLocations: function() {
	          return core.ajax('/api/productionlines');
	        },
	        createProduct: function( data ) {
	          return core.ajaxPost( '/api/products', data );
	        },
	        updateProduct: function( productCode, data ) {
	          return core.ajaxPut( '/api/products/' + productCode, data );
	        },
	        setProductIngredients: function( productKey, data ) {
	          return core.ajaxPost( '/api/products/' + productKey + '/ingredients', data );
	        },
	        setProductAttributes: function( productKey, data ) {
	          return core.ajaxPost( '/api/products/' + productKey + '/specs', data );
	        },
	        getProductionSchedulesDataPager: function( options ) {
	          options = options || {};

	          return core.pagedDataHelper.init({
	              urlBase: options.baseUrl || "/api/productionschedules",
	              pageSize: options.pageSize || 50,
	              parameters: options.parameters,
	              onNewPageSet: options.onNewPageSet,
	              onEndOfResults: options.onEndOfResults
	          });
	        },
	        getProductionScheduleDetails: function( key ) {
	          return core.ajax( '/api/productionschedules/' + key );
	        },
	        createProductionSchedule: function( data ) {
	          return core.ajaxPost( '/api/productionschedules/', data );
	        },
	        updateProductionSchedule: function( key, data ) {
	          return core.ajaxPut( '/api/productionschedules/' + key, data );
	        },
	        deleteProductionSchedule: function( key ) {
	          return core.ajaxDelete( '/api/productionschedules/' + key );
	        }
	    };

	    //#region function delegates
	    function getProductDetails(lotType, key) {
	        return core.ajax(buildProductUrl(lotType, key));
	    }
	    //#endregion

	    function buildProductUrl(lotType, key) {
	        key = key || '';
	        return '/api/products/' + lotType + (key ? '/' + key : '');
	    }
	    function buildChileProductsUrl(chileState) {
	        return '/api/chileproducts?chileState=' + chileState;
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 25:
/***/ (function(module, exports) {

	module.exports = "<input type=\"text\" class=\"form-control\" data-bind=\"jqAuto: { value: selectorValue, source: options, labelProp: optionsDisplay, valueProp: optionsValue }, visible: !isLoading(), attr: { 'id': controlId }, enable: enabled, disable: disabled\">\n\n<div class=\"well well-sm\" data-bind=\"visible: isLoading\">\n  <i class=\"fa fa-spinner fa-pulse\"></i>\n</div>\n\n"

/***/ }),

/***/ 26:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(process) {  /* globals require, module */

	  'use strict';

	  /**
	   * Module dependencies.
	   */

	  var pathtoRegexp = __webpack_require__(28);

	  /**
	   * Module exports.
	   */

	  module.exports = page;

	  /**
	   * Detect click event
	   */
	  var clickEvent = ('undefined' !== typeof document) && document.ontouchstart ? 'touchstart' : 'click';

	  /**
	   * To work properly with the URL
	   * history.location generated polyfill in https://github.com/devote/HTML5-History-API
	   */

	  var location = ('undefined' !== typeof window) && (window.history.location || window.location);

	  /**
	   * Perform initial dispatch.
	   */

	  var dispatch = true;


	  /**
	   * Decode URL components (query string, pathname, hash).
	   * Accommodates both regular percent encoding and x-www-form-urlencoded format.
	   */
	  var decodeURLComponents = true;

	  /**
	   * Base path.
	   */

	  var base = '';

	  /**
	   * Running flag.
	   */

	  var running;

	  /**
	   * HashBang option
	   */

	  var hashbang = false;

	  /**
	   * Previous context, for capturing
	   * page exit events.
	   */

	  var prevContext;

	  /**
	   * Register `path` with callback `fn()`,
	   * or route `path`, or redirection,
	   * or `page.start()`.
	   *
	   *   page(fn);
	   *   page('*', fn);
	   *   page('/user/:id', load, user);
	   *   page('/user/' + user.id, { some: 'thing' });
	   *   page('/user/' + user.id);
	   *   page('/from', '/to')
	   *   page();
	   *
	   * @param {string|!Function|!Object} path
	   * @param {Function=} fn
	   * @api public
	   */

	  function page(path, fn) {
	    // <callback>
	    if ('function' === typeof path) {
	      return page('*', path);
	    }

	    // route <path> to <callback ...>
	    if ('function' === typeof fn) {
	      var route = new Route(/** @type {string} */ (path));
	      for (var i = 1; i < arguments.length; ++i) {
	        page.callbacks.push(route.middleware(arguments[i]));
	      }
	      // show <path> with [state]
	    } else if ('string' === typeof path) {
	      page['string' === typeof fn ? 'redirect' : 'show'](path, fn);
	      // start [options]
	    } else {
	      page.start(path);
	    }
	  }

	  /**
	   * Callback functions.
	   */

	  page.callbacks = [];
	  page.exits = [];

	  /**
	   * Current path being processed
	   * @type {string}
	   */
	  page.current = '';

	  /**
	   * Number of pages navigated to.
	   * @type {number}
	   *
	   *     page.len == 0;
	   *     page('/login');
	   *     page.len == 1;
	   */

	  page.len = 0;

	  /**
	   * Get or set basepath to `path`.
	   *
	   * @param {string} path
	   * @api public
	   */

	  page.base = function(path) {
	    if (0 === arguments.length) return base;
	    base = path;
	  };

	  /**
	   * Bind with the given `options`.
	   *
	   * Options:
	   *
	   *    - `click` bind to click events [true]
	   *    - `popstate` bind to popstate [true]
	   *    - `dispatch` perform initial dispatch [true]
	   *
	   * @param {Object} options
	   * @api public
	   */

	  page.start = function(options) {
	    options = options || {};
	    if (running) return;
	    running = true;
	    if (false === options.dispatch) dispatch = false;
	    if (false === options.decodeURLComponents) decodeURLComponents = false;
	    if (false !== options.popstate) window.addEventListener('popstate', onpopstate, false);
	    if (false !== options.click) {
	      document.addEventListener(clickEvent, onclick, false);
	    }
	    if (true === options.hashbang) hashbang = true;
	    if (!dispatch) return;
	    var url = (hashbang && ~location.hash.indexOf('#!')) ? location.hash.substr(2) + location.search : location.pathname + location.search + location.hash;
	    page.replace(url, null, true, dispatch);
	  };

	  /**
	   * Unbind click and popstate event handlers.
	   *
	   * @api public
	   */

	  page.stop = function() {
	    if (!running) return;
	    page.current = '';
	    page.len = 0;
	    running = false;
	    document.removeEventListener(clickEvent, onclick, false);
	    window.removeEventListener('popstate', onpopstate, false);
	  };

	  /**
	   * Show `path` with optional `state` object.
	   *
	   * @param {string} path
	   * @param {Object=} state
	   * @param {boolean=} dispatch
	   * @param {boolean=} push
	   * @return {!Context}
	   * @api public
	   */

	  page.show = function(path, state, dispatch, push) {
	    var ctx = new Context(path, state);
	    page.current = ctx.path;
	    if (false !== dispatch) page.dispatch(ctx);
	    if (false !== ctx.handled && false !== push) ctx.pushState();
	    return ctx;
	  };

	  /**
	   * Goes back in the history
	   * Back should always let the current route push state and then go back.
	   *
	   * @param {string} path - fallback path to go back if no more history exists, if undefined defaults to page.base
	   * @param {Object=} state
	   * @api public
	   */

	  page.back = function(path, state) {
	    if (page.len > 0) {
	      // this may need more testing to see if all browsers
	      // wait for the next tick to go back in history
	      history.back();
	      page.len--;
	    } else if (path) {
	      setTimeout(function() {
	        page.show(path, state);
	      });
	    }else{
	      setTimeout(function() {
	        page.show(base, state);
	      });
	    }
	  };


	  /**
	   * Register route to redirect from one path to other
	   * or just redirect to another route
	   *
	   * @param {string} from - if param 'to' is undefined redirects to 'from'
	   * @param {string=} to
	   * @api public
	   */
	  page.redirect = function(from, to) {
	    // Define route from a path to another
	    if ('string' === typeof from && 'string' === typeof to) {
	      page(from, function(e) {
	        setTimeout(function() {
	          page.replace(/** @type {!string} */ (to));
	        }, 0);
	      });
	    }

	    // Wait for the push state and replace it with another
	    if ('string' === typeof from && 'undefined' === typeof to) {
	      setTimeout(function() {
	        page.replace(from);
	      }, 0);
	    }
	  };

	  /**
	   * Replace `path` with optional `state` object.
	   *
	   * @param {string} path
	   * @param {Object=} state
	   * @param {boolean=} init
	   * @param {boolean=} dispatch
	   * @return {!Context}
	   * @api public
	   */


	  page.replace = function(path, state, init, dispatch) {
	    var ctx = new Context(path, state);
	    page.current = ctx.path;
	    ctx.init = init;
	    ctx.save(); // save before dispatching, which may redirect
	    if (false !== dispatch) page.dispatch(ctx);
	    return ctx;
	  };

	  /**
	   * Dispatch the given `ctx`.
	   *
	   * @param {Context} ctx
	   * @api private
	   */
	  page.dispatch = function(ctx) {
	    var prev = prevContext,
	      i = 0,
	      j = 0;

	    prevContext = ctx;

	    function nextExit() {
	      var fn = page.exits[j++];
	      if (!fn) return nextEnter();
	      fn(prev, nextExit);
	    }

	    function nextEnter() {
	      var fn = page.callbacks[i++];

	      if (ctx.path !== page.current) {
	        ctx.handled = false;
	        return;
	      }
	      if (!fn) return unhandled(ctx);
	      fn(ctx, nextEnter);
	    }

	    if (prev) {
	      nextExit();
	    } else {
	      nextEnter();
	    }
	  };

	  /**
	   * Unhandled `ctx`. When it's not the initial
	   * popstate then redirect. If you wish to handle
	   * 404s on your own use `page('*', callback)`.
	   *
	   * @param {Context} ctx
	   * @api private
	   */
	  function unhandled(ctx) {
	    if (ctx.handled) return;
	    var current;

	    if (hashbang) {
	      current = base + location.hash.replace('#!', '');
	    } else {
	      current = location.pathname + location.search;
	    }

	    if (current === ctx.canonicalPath) return;
	    page.stop();
	    ctx.handled = false;
	    location.href = ctx.canonicalPath;
	  }

	  /**
	   * Register an exit route on `path` with
	   * callback `fn()`, which will be called
	   * on the previous context when a new
	   * page is visited.
	   */
	  page.exit = function(path, fn) {
	    if (typeof path === 'function') {
	      return page.exit('*', path);
	    }

	    var route = new Route(path);
	    for (var i = 1; i < arguments.length; ++i) {
	      page.exits.push(route.middleware(arguments[i]));
	    }
	  };

	  /**
	   * Remove URL encoding from the given `str`.
	   * Accommodates whitespace in both x-www-form-urlencoded
	   * and regular percent-encoded form.
	   *
	   * @param {string} val - URL component to decode
	   */
	  function decodeURLEncodedURIComponent(val) {
	    if (typeof val !== 'string') { return val; }
	    return decodeURLComponents ? decodeURIComponent(val.replace(/\+/g, ' ')) : val;
	  }

	  /**
	   * Initialize a new "request" `Context`
	   * with the given `path` and optional initial `state`.
	   *
	   * @constructor
	   * @param {string} path
	   * @param {Object=} state
	   * @api public
	   */

	  function Context(path, state) {
	    if ('/' === path[0] && 0 !== path.indexOf(base)) path = base + (hashbang ? '#!' : '') + path;
	    var i = path.indexOf('?');

	    this.canonicalPath = path;
	    this.path = path.replace(base, '') || '/';
	    if (hashbang) this.path = this.path.replace('#!', '') || '/';

	    this.title = document.title;
	    this.state = state || {};
	    this.state.path = path;
	    this.querystring = ~i ? decodeURLEncodedURIComponent(path.slice(i + 1)) : '';
	    this.pathname = decodeURLEncodedURIComponent(~i ? path.slice(0, i) : path);
	    this.params = {};

	    // fragment
	    this.hash = '';
	    if (!hashbang) {
	      if (!~this.path.indexOf('#')) return;
	      var parts = this.path.split('#');
	      this.path = parts[0];
	      this.hash = decodeURLEncodedURIComponent(parts[1]) || '';
	      this.querystring = this.querystring.split('#')[0];
	    }
	  }

	  /**
	   * Expose `Context`.
	   */

	  page.Context = Context;

	  /**
	   * Push state.
	   *
	   * @api private
	   */

	  Context.prototype.pushState = function() {
	    page.len++;
	    history.pushState(this.state, this.title, hashbang && this.path !== '/' ? '#!' + this.path : this.canonicalPath);
	  };

	  /**
	   * Save the context state.
	   *
	   * @api public
	   */

	  Context.prototype.save = function() {
	    history.replaceState(this.state, this.title, hashbang && this.path !== '/' ? '#!' + this.path : this.canonicalPath);
	  };

	  /**
	   * Initialize `Route` with the given HTTP `path`,
	   * and an array of `callbacks` and `options`.
	   *
	   * Options:
	   *
	   *   - `sensitive`    enable case-sensitive routes
	   *   - `strict`       enable strict matching for trailing slashes
	   *
	   * @constructor
	   * @param {string} path
	   * @param {Object=} options
	   * @api private
	   */

	  function Route(path, options) {
	    options = options || {};
	    this.path = (path === '*') ? '(.*)' : path;
	    this.method = 'GET';
	    this.regexp = pathtoRegexp(this.path,
	      this.keys = [],
	      options);
	  }

	  /**
	   * Expose `Route`.
	   */

	  page.Route = Route;

	  /**
	   * Return route middleware with
	   * the given callback `fn()`.
	   *
	   * @param {Function} fn
	   * @return {Function}
	   * @api public
	   */

	  Route.prototype.middleware = function(fn) {
	    var self = this;
	    return function(ctx, next) {
	      if (self.match(ctx.path, ctx.params)) return fn(ctx, next);
	      next();
	    };
	  };

	  /**
	   * Check if this route matches `path`, if so
	   * populate `params`.
	   *
	   * @param {string} path
	   * @param {Object} params
	   * @return {boolean}
	   * @api private
	   */

	  Route.prototype.match = function(path, params) {
	    var keys = this.keys,
	      qsIndex = path.indexOf('?'),
	      pathname = ~qsIndex ? path.slice(0, qsIndex) : path,
	      m = this.regexp.exec(decodeURIComponent(pathname));

	    if (!m) return false;

	    for (var i = 1, len = m.length; i < len; ++i) {
	      var key = keys[i - 1];
	      var val = decodeURLEncodedURIComponent(m[i]);
	      if (val !== undefined || !(hasOwnProperty.call(params, key.name))) {
	        params[key.name] = val;
	      }
	    }

	    return true;
	  };


	  /**
	   * Handle "populate" events.
	   */

	  var onpopstate = (function () {
	    var loaded = false;
	    if ('undefined' === typeof window) {
	      return;
	    }
	    if (document.readyState === 'complete') {
	      loaded = true;
	    } else {
	      window.addEventListener('load', function() {
	        setTimeout(function() {
	          loaded = true;
	        }, 0);
	      });
	    }
	    return function onpopstate(e) {
	      if (!loaded) return;
	      if (e.state) {
	        var path = e.state.path;
	        page.replace(path, e.state);
	      } else {
	        page.show(location.pathname + location.hash, undefined, undefined, false);
	      }
	    };
	  })();
	  /**
	   * Handle "click" events.
	   */

	  function onclick(e) {

	    if (1 !== which(e)) return;

	    if (e.metaKey || e.ctrlKey || e.shiftKey) return;
	    if (e.defaultPrevented) return;



	    // ensure link
	    // use shadow dom when available
	    var el = e.path ? e.path[0] : e.target;
	    while (el && 'A' !== el.nodeName) el = el.parentNode;
	    if (!el || 'A' !== el.nodeName) return;



	    // Ignore if tag has
	    // 1. "download" attribute
	    // 2. rel="external" attribute
	    if (el.hasAttribute('download') || el.getAttribute('rel') === 'external') return;

	    // ensure non-hash for the same path
	    var link = el.getAttribute('href');
	    if (!hashbang && el.pathname === location.pathname && (el.hash || '#' === link)) return;



	    // Check for mailto: in the href
	    if (link && link.indexOf('mailto:') > -1) return;

	    // check target
	    if (el.target) return;

	    // x-origin
	    if (!sameOrigin(el.href)) return;



	    // rebuild path
	    var path = el.pathname + el.search + (el.hash || '');

	    // strip leading "/[drive letter]:" on NW.js on Windows
	    if (typeof process !== 'undefined' && path.match(/^\/[a-zA-Z]:\//)) {
	      path = path.replace(/^\/[a-zA-Z]:\//, '/');
	    }

	    // same page
	    var orig = path;

	    if (path.indexOf(base) === 0) {
	      path = path.substr(base.length);
	    }

	    if (hashbang) path = path.replace('#!', '');

	    if (base && orig === path) return;

	    e.preventDefault();
	    page.show(orig);
	  }

	  /**
	   * Event button.
	   */

	  function which(e) {
	    e = e || window.event;
	    return null === e.which ? e.button : e.which;
	  }

	  /**
	   * Check if `href` is the same origin.
	   */

	  function sameOrigin(href) {
	    var origin = location.protocol + '//' + location.hostname;
	    if (location.port) origin += ':' + location.port;
	    return (href && (0 === href.indexOf(origin)));
	  }

	  page.sameOrigin = sameOrigin;

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(27)))

/***/ }),

/***/ 27:
/***/ (function(module, exports) {

	// shim for using process in browser
	var process = module.exports = {};

	// cached from whatever global is present so that test runners that stub it
	// don't break things.  But we need to wrap it in a try catch in case it is
	// wrapped in strict mode code which doesn't define any globals.  It's inside a
	// function because try/catches deoptimize in certain engines.

	var cachedSetTimeout;
	var cachedClearTimeout;

	function defaultSetTimout() {
	    throw new Error('setTimeout has not been defined');
	}
	function defaultClearTimeout () {
	    throw new Error('clearTimeout has not been defined');
	}
	(function () {
	    try {
	        if (typeof setTimeout === 'function') {
	            cachedSetTimeout = setTimeout;
	        } else {
	            cachedSetTimeout = defaultSetTimout;
	        }
	    } catch (e) {
	        cachedSetTimeout = defaultSetTimout;
	    }
	    try {
	        if (typeof clearTimeout === 'function') {
	            cachedClearTimeout = clearTimeout;
	        } else {
	            cachedClearTimeout = defaultClearTimeout;
	        }
	    } catch (e) {
	        cachedClearTimeout = defaultClearTimeout;
	    }
	} ())
	function runTimeout(fun) {
	    if (cachedSetTimeout === setTimeout) {
	        //normal enviroments in sane situations
	        return setTimeout(fun, 0);
	    }
	    // if setTimeout wasn't available but was latter defined
	    if ((cachedSetTimeout === defaultSetTimout || !cachedSetTimeout) && setTimeout) {
	        cachedSetTimeout = setTimeout;
	        return setTimeout(fun, 0);
	    }
	    try {
	        // when when somebody has screwed with setTimeout but no I.E. maddness
	        return cachedSetTimeout(fun, 0);
	    } catch(e){
	        try {
	            // When we are in I.E. but the script has been evaled so I.E. doesn't trust the global object when called normally
	            return cachedSetTimeout.call(null, fun, 0);
	        } catch(e){
	            // same as above but when it's a version of I.E. that must have the global object for 'this', hopfully our context correct otherwise it will throw a global error
	            return cachedSetTimeout.call(this, fun, 0);
	        }
	    }


	}
	function runClearTimeout(marker) {
	    if (cachedClearTimeout === clearTimeout) {
	        //normal enviroments in sane situations
	        return clearTimeout(marker);
	    }
	    // if clearTimeout wasn't available but was latter defined
	    if ((cachedClearTimeout === defaultClearTimeout || !cachedClearTimeout) && clearTimeout) {
	        cachedClearTimeout = clearTimeout;
	        return clearTimeout(marker);
	    }
	    try {
	        // when when somebody has screwed with setTimeout but no I.E. maddness
	        return cachedClearTimeout(marker);
	    } catch (e){
	        try {
	            // When we are in I.E. but the script has been evaled so I.E. doesn't  trust the global object when called normally
	            return cachedClearTimeout.call(null, marker);
	        } catch (e){
	            // same as above but when it's a version of I.E. that must have the global object for 'this', hopfully our context correct otherwise it will throw a global error.
	            // Some versions of I.E. have different rules for clearTimeout vs setTimeout
	            return cachedClearTimeout.call(this, marker);
	        }
	    }



	}
	var queue = [];
	var draining = false;
	var currentQueue;
	var queueIndex = -1;

	function cleanUpNextTick() {
	    if (!draining || !currentQueue) {
	        return;
	    }
	    draining = false;
	    if (currentQueue.length) {
	        queue = currentQueue.concat(queue);
	    } else {
	        queueIndex = -1;
	    }
	    if (queue.length) {
	        drainQueue();
	    }
	}

	function drainQueue() {
	    if (draining) {
	        return;
	    }
	    var timeout = runTimeout(cleanUpNextTick);
	    draining = true;

	    var len = queue.length;
	    while(len) {
	        currentQueue = queue;
	        queue = [];
	        while (++queueIndex < len) {
	            if (currentQueue) {
	                currentQueue[queueIndex].run();
	            }
	        }
	        queueIndex = -1;
	        len = queue.length;
	    }
	    currentQueue = null;
	    draining = false;
	    runClearTimeout(timeout);
	}

	process.nextTick = function (fun) {
	    var args = new Array(arguments.length - 1);
	    if (arguments.length > 1) {
	        for (var i = 1; i < arguments.length; i++) {
	            args[i - 1] = arguments[i];
	        }
	    }
	    queue.push(new Item(fun, args));
	    if (queue.length === 1 && !draining) {
	        runTimeout(drainQueue);
	    }
	};

	// v8 likes predictible objects
	function Item(fun, array) {
	    this.fun = fun;
	    this.array = array;
	}
	Item.prototype.run = function () {
	    this.fun.apply(null, this.array);
	};
	process.title = 'browser';
	process.browser = true;
	process.env = {};
	process.argv = [];
	process.version = ''; // empty string to avoid regexp issues
	process.versions = {};

	function noop() {}

	process.on = noop;
	process.addListener = noop;
	process.once = noop;
	process.off = noop;
	process.removeListener = noop;
	process.removeAllListeners = noop;
	process.emit = noop;
	process.prependListener = noop;
	process.prependOnceListener = noop;

	process.listeners = function (name) { return [] }

	process.binding = function (name) {
	    throw new Error('process.binding is not supported');
	};

	process.cwd = function () { return '/' };
	process.chdir = function (dir) {
	    throw new Error('process.chdir is not supported');
	};
	process.umask = function() { return 0; };


/***/ }),

/***/ 28:
/***/ (function(module, exports, __webpack_require__) {

	var isarray = __webpack_require__(29)

	/**
	 * Expose `pathToRegexp`.
	 */
	module.exports = pathToRegexp
	module.exports.parse = parse
	module.exports.compile = compile
	module.exports.tokensToFunction = tokensToFunction
	module.exports.tokensToRegExp = tokensToRegExp

	/**
	 * The main path matching regexp utility.
	 *
	 * @type {RegExp}
	 */
	var PATH_REGEXP = new RegExp([
	  // Match escaped characters that would otherwise appear in future matches.
	  // This allows the user to escape special characters that won't transform.
	  '(\\\\.)',
	  // Match Express-style parameters and un-named parameters with a prefix
	  // and optional suffixes. Matches appear as:
	  //
	  // "/:test(\\d+)?" => ["/", "test", "\d+", undefined, "?", undefined]
	  // "/route(\\d+)"  => [undefined, undefined, undefined, "\d+", undefined, undefined]
	  // "/*"            => ["/", undefined, undefined, undefined, undefined, "*"]
	  '([\\/.])?(?:(?:\\:(\\w+)(?:\\(((?:\\\\.|[^()])+)\\))?|\\(((?:\\\\.|[^()])+)\\))([+*?])?|(\\*))'
	].join('|'), 'g')

	/**
	 * Parse a string for the raw tokens.
	 *
	 * @param  {String} str
	 * @return {Array}
	 */
	function parse (str) {
	  var tokens = []
	  var key = 0
	  var index = 0
	  var path = ''
	  var res

	  while ((res = PATH_REGEXP.exec(str)) != null) {
	    var m = res[0]
	    var escaped = res[1]
	    var offset = res.index
	    path += str.slice(index, offset)
	    index = offset + m.length

	    // Ignore already escaped sequences.
	    if (escaped) {
	      path += escaped[1]
	      continue
	    }

	    // Push the current path onto the tokens.
	    if (path) {
	      tokens.push(path)
	      path = ''
	    }

	    var prefix = res[2]
	    var name = res[3]
	    var capture = res[4]
	    var group = res[5]
	    var suffix = res[6]
	    var asterisk = res[7]

	    var repeat = suffix === '+' || suffix === '*'
	    var optional = suffix === '?' || suffix === '*'
	    var delimiter = prefix || '/'
	    var pattern = capture || group || (asterisk ? '.*' : '[^' + delimiter + ']+?')

	    tokens.push({
	      name: name || key++,
	      prefix: prefix || '',
	      delimiter: delimiter,
	      optional: optional,
	      repeat: repeat,
	      pattern: escapeGroup(pattern)
	    })
	  }

	  // Match any characters still remaining.
	  if (index < str.length) {
	    path += str.substr(index)
	  }

	  // If the path exists, push it onto the end.
	  if (path) {
	    tokens.push(path)
	  }

	  return tokens
	}

	/**
	 * Compile a string to a template function for the path.
	 *
	 * @param  {String}   str
	 * @return {Function}
	 */
	function compile (str) {
	  return tokensToFunction(parse(str))
	}

	/**
	 * Expose a method for transforming tokens into the path function.
	 */
	function tokensToFunction (tokens) {
	  // Compile all the tokens into regexps.
	  var matches = new Array(tokens.length)

	  // Compile all the patterns before compilation.
	  for (var i = 0; i < tokens.length; i++) {
	    if (typeof tokens[i] === 'object') {
	      matches[i] = new RegExp('^' + tokens[i].pattern + '$')
	    }
	  }

	  return function (obj) {
	    var path = ''
	    var data = obj || {}

	    for (var i = 0; i < tokens.length; i++) {
	      var token = tokens[i]

	      if (typeof token === 'string') {
	        path += token

	        continue
	      }

	      var value = data[token.name]
	      var segment

	      if (value == null) {
	        if (token.optional) {
	          continue
	        } else {
	          throw new TypeError('Expected "' + token.name + '" to be defined')
	        }
	      }

	      if (isarray(value)) {
	        if (!token.repeat) {
	          throw new TypeError('Expected "' + token.name + '" to not repeat, but received "' + value + '"')
	        }

	        if (value.length === 0) {
	          if (token.optional) {
	            continue
	          } else {
	            throw new TypeError('Expected "' + token.name + '" to not be empty')
	          }
	        }

	        for (var j = 0; j < value.length; j++) {
	          segment = encodeURIComponent(value[j])

	          if (!matches[i].test(segment)) {
	            throw new TypeError('Expected all "' + token.name + '" to match "' + token.pattern + '", but received "' + segment + '"')
	          }

	          path += (j === 0 ? token.prefix : token.delimiter) + segment
	        }

	        continue
	      }

	      segment = encodeURIComponent(value)

	      if (!matches[i].test(segment)) {
	        throw new TypeError('Expected "' + token.name + '" to match "' + token.pattern + '", but received "' + segment + '"')
	      }

	      path += token.prefix + segment
	    }

	    return path
	  }
	}

	/**
	 * Escape a regular expression string.
	 *
	 * @param  {String} str
	 * @return {String}
	 */
	function escapeString (str) {
	  return str.replace(/([.+*?=^!:${}()[\]|\/])/g, '\\$1')
	}

	/**
	 * Escape the capturing group by escaping special characters and meaning.
	 *
	 * @param  {String} group
	 * @return {String}
	 */
	function escapeGroup (group) {
	  return group.replace(/([=!:$\/()])/g, '\\$1')
	}

	/**
	 * Attach the keys as a property of the regexp.
	 *
	 * @param  {RegExp} re
	 * @param  {Array}  keys
	 * @return {RegExp}
	 */
	function attachKeys (re, keys) {
	  re.keys = keys
	  return re
	}

	/**
	 * Get the flags for a regexp from the options.
	 *
	 * @param  {Object} options
	 * @return {String}
	 */
	function flags (options) {
	  return options.sensitive ? '' : 'i'
	}

	/**
	 * Pull out keys from a regexp.
	 *
	 * @param  {RegExp} path
	 * @param  {Array}  keys
	 * @return {RegExp}
	 */
	function regexpToRegexp (path, keys) {
	  // Use a negative lookahead to match only capturing groups.
	  var groups = path.source.match(/\((?!\?)/g)

	  if (groups) {
	    for (var i = 0; i < groups.length; i++) {
	      keys.push({
	        name: i,
	        prefix: null,
	        delimiter: null,
	        optional: false,
	        repeat: false,
	        pattern: null
	      })
	    }
	  }

	  return attachKeys(path, keys)
	}

	/**
	 * Transform an array into a regexp.
	 *
	 * @param  {Array}  path
	 * @param  {Array}  keys
	 * @param  {Object} options
	 * @return {RegExp}
	 */
	function arrayToRegexp (path, keys, options) {
	  var parts = []

	  for (var i = 0; i < path.length; i++) {
	    parts.push(pathToRegexp(path[i], keys, options).source)
	  }

	  var regexp = new RegExp('(?:' + parts.join('|') + ')', flags(options))

	  return attachKeys(regexp, keys)
	}

	/**
	 * Create a path regexp from string input.
	 *
	 * @param  {String} path
	 * @param  {Array}  keys
	 * @param  {Object} options
	 * @return {RegExp}
	 */
	function stringToRegexp (path, keys, options) {
	  var tokens = parse(path)
	  var re = tokensToRegExp(tokens, options)

	  // Attach keys back to the regexp.
	  for (var i = 0; i < tokens.length; i++) {
	    if (typeof tokens[i] !== 'string') {
	      keys.push(tokens[i])
	    }
	  }

	  return attachKeys(re, keys)
	}

	/**
	 * Expose a function for taking tokens and returning a RegExp.
	 *
	 * @param  {Array}  tokens
	 * @param  {Array}  keys
	 * @param  {Object} options
	 * @return {RegExp}
	 */
	function tokensToRegExp (tokens, options) {
	  options = options || {}

	  var strict = options.strict
	  var end = options.end !== false
	  var route = ''
	  var lastToken = tokens[tokens.length - 1]
	  var endsWithSlash = typeof lastToken === 'string' && /\/$/.test(lastToken)

	  // Iterate over the tokens and create our regexp string.
	  for (var i = 0; i < tokens.length; i++) {
	    var token = tokens[i]

	    if (typeof token === 'string') {
	      route += escapeString(token)
	    } else {
	      var prefix = escapeString(token.prefix)
	      var capture = token.pattern

	      if (token.repeat) {
	        capture += '(?:' + prefix + capture + ')*'
	      }

	      if (token.optional) {
	        if (prefix) {
	          capture = '(?:' + prefix + '(' + capture + '))?'
	        } else {
	          capture = '(' + capture + ')?'
	        }
	      } else {
	        capture = prefix + '(' + capture + ')'
	      }

	      route += capture
	    }
	  }

	  // In non-strict mode we allow a slash at the end of match. If the path to
	  // match already ends with a slash, we remove it for consistency. The slash
	  // is valid at the end of a path match, not in the middle. This is important
	  // in non-ending mode, where "/test/" shouldn't match "/test//route".
	  if (!strict) {
	    route = (endsWithSlash ? route.slice(0, -2) : route) + '(?:\\/(?=$))?'
	  }

	  if (end) {
	    route += '$'
	  } else {
	    // In non-ending mode, we need the capturing groups to match as much as
	    // possible by using a positive lookahead to the end or next path segment.
	    route += strict && endsWithSlash ? '' : '(?=\\/|$)'
	  }

	  return new RegExp('^' + route, flags(options))
	}

	/**
	 * Normalize the given path string, returning a regular expression.
	 *
	 * An empty array can be passed in for the keys, which will hold the
	 * placeholder key descriptions. For example, using `/user/:id`, `keys` will
	 * contain `[{ name: 'id', delimiter: '/', optional: false, repeat: false }]`.
	 *
	 * @param  {(String|RegExp|Array)} path
	 * @param  {Array}                 [keys]
	 * @param  {Object}                [options]
	 * @return {RegExp}
	 */
	function pathToRegexp (path, keys, options) {
	  keys = keys || []

	  if (!isarray(keys)) {
	    options = keys
	    keys = []
	  } else if (!options) {
	    options = {}
	  }

	  if (path instanceof RegExp) {
	    return regexpToRegexp(path, keys, options)
	  }

	  if (isarray(path)) {
	    return arrayToRegexp(path, keys, options)
	  }

	  return stringToRegexp(path, keys, options)
	}


/***/ }),

/***/ 29:
/***/ (function(module, exports) {

	module.exports = Array.isArray || function (arr) {
	  return Object.prototype.toString.call(arr) == '[object Array]';
	};


/***/ }),

/***/ 31:
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9), __webpack_require__(8), __webpack_require__(32), __webpack_require__(34), __webpack_require__(22), __webpack_require__(35)], __WEBPACK_AMD_DEFINE_RESULT__ = function(ko, app) {
	    ko.extenders.timeEntry = function (target) {
	        var pattern = /^(\d)?(\d)?:?(\d)?(\d)?$/;
	        target.formattedTime = ko.computed({
	            read: function () {
	                if (!target()) return '00:00';
	                var val = target();
	                var formatted = val;
	                switch (val.length) {
	                    case 1:
	                        formatted = val.replace(pattern, "0$1");
	                    case 2:
	                        formatted += ":00";
	                        break;
	                    case 3:
	                        formatted = val.replace(pattern, "0$1:$2$3");
	                        break;
	                    case 4:
	                        formatted = val.replace(pattern, "$1$2:$3$4");
	                        break;
	                }
	                return formatted;
	            },
	            write: function (value) {
	                if (typeof value === "string") {
	                    var parsed = Date.parse(value);
	                    if (parsed) {
	                        var d = new Date(parsed);
	                        var hours = d.getHours();
	                        hours = hours < 10 ? ('0' + hours) : hours;
	                        var minutes = d.getMinutes();
	                        minutes = minutes < 10 ? ('0' + minutes) : minutes;
	                        value = hours + ":" + minutes;
	                    }
	                }
	                target(value);
	            }
	        });

	        target.extend({ pattern: { message: "Invalid Date", params: /^([01]\d|2[0-3]):?([0-5]\d)$/ } });
	        target.Hours = ko.computed(function () {
	            if (!target.formattedTime()) return 0;
	            return target.formattedTime().split(":")[0];
	        });
	        target.Mins = ko.computed(function () {
	            if (!target.formattedTime()) return 0;
	            return target.formattedTime().split(":")[1];
	        });
	        target.formattedTime(target());
	        return target;
	    };

	    ko.extenders.toteKey = function (target, callback) {
	        var pattern = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{4})?$/;
	        var isComplete = ko.observable(false);
	        target.formattedTote = ko.computed({
	            read: function () {
	                var value = target();
	                return formatTote(value);
	            },
	            write: function (input) {
	                var value = cleanInput(input);
	                if (target() === value) return;

	                target(value);
	                if (value && value.match(pattern)) {
	                    var formatted = formatTote(value);
	                    if (formatted.length === 10) {
	                        isComplete(true);
	                        if (typeof callback === "function") callback(formatted);
	                    }
	                }
	            },
	        });
	        target.isComplete = ko.computed(function () {
	            return isComplete();
	        });
	        target.getNextTote = function () {
	            var formatted = target.formattedTote();
	            var sequence = parseSequence();
	            if (isNaN(sequence)) return null;
	            sequence++;

	            var sequenceString = formatSequence();
	            return formatted.replace(pattern, '0$2 $4 ' + sequenceString);

	            function parseSequence() {
	                var sections = formatted.split(" ");
	                if (sections.length !== 3) return null;
	                return parseInt(sections[2]);
	            }
	            function formatSequence() {
	                var val = sequence.toString();
	                while (val.length < 4) {
	                    val = "0" + val;
	                }
	                return val;
	            }
	        };
	        target.isMatch = function (val) {
	            var formattedVal = formatTote(ko.utils.unwrapObservable(val));
	            if (!formattedVal) return false;
	            var p = new RegExp("^" + target.formattedTote() + "$");
	            return formattedVal.match(p);
	        };

	        target.extend({ throttle: 800 });
	        target.formattedTote(target());
	        return target;

	        function formatTote(input) {
	            if (input == undefined) return '';
	            if (!input.match(pattern)) return input;
	            input = input.trim();
	            return input.replace(pattern, '0$2 $4 $6').trim().replace("  ", " ");
	        }
	        function cleanInput(input) {
	            if (typeof input == "number") input = input.toString();
	            if (typeof input !== "string") return undefined;
	            return input.replace(/\s/g, '');
	        }
	    };
	    
	    ko.extenders.contractType = function (target) {
	        return new TypeExtension(target, app.lists.contractTypes.toDictionary());
	    };

	    ko.extenders.contractStatus = function (target) {
	        return new TypeExtension(target, app.lists.contractStatuses.toDictionary());
	    };

	    ko.extenders.defectResolutionType = function (target) {
	        return new TypeExtension(target, app.lists.defectResolutionTypes.toDictionary());
	    };

	    ko.extenders.defectType = function (target) {
	        return new TypeExtension(target, app.lists.defectTypes.toDictionary());
	    };
	    
	    ko.extenders.facilityType = function (target) {
	        return new TypeExtension(target, app.lists.facilityTypes.toDictionary());
	    };

	    ko.extenders.inventoryType = function (target) {
	        return new TypeExtension(target, app.lists.inventoryTypes.toObjectDictionary());
	    };

	    ko.extenders.productType = function (target) {
	      var extension = new TypeExtension(target, app.lists.inventoryTypes.toDictionary());
	      extension.trueValue = ko.pureComputed(function() {
	        var raw = target();
	        if (raw == null) return null;
	        return parseInt(raw);
	      });
	      return extension;
	    };

	    ko.extenders.locationStatusType = function (target) {
	        return new TypeExtension(target, app.lists.locationStatusTypes.toDictionary());
	    }

	    ko.extenders.lotHoldType = function (target) {
	        return new TypeExtension(target, app.lists.lotHoldTypes.toDictionary());
	    };

	    ko.extenders.lotQualityStatusType = function (target) {
	        return new TypeExtension(target, app.lists.lotQualityStatusTypes.toDictionary());
	    };

	    ko.extenders.lotType = function (target) {
	        return new TypeExtension(target, app.lists.lotTypes.toDictionary());
	    };

	    ko.extenders.lotType2 = function (target) {
	        return new TypeExtension(target, app.lists.lotTypes.toObjectDictionary());
	    };

	    ko.extenders.productionStatusType = function (target) {
	        return new TypeExtension(target, app.lists.productionStatusTypes.toDictionary());
	    };

	    ko.extenders.chileType = function (target) {
	        var options = {
	            0: 'Other Raw',
	            1: 'Dehydrated',
	            2: 'WIP',
	            3: 'Finished Goods'
	        };
	        return new TypeExtension(target, options);
	    };

	    ko.extenders.treatmentType = function (target) {
	        return new TypeExtension(target, app.lists.treatmentTypes.toDictionary());
	    };

	    ko.extenders.shipmentStatusType = function (target) {
	        return new TypeExtension(target, app.lists.shipmentStatus.toDictionary());
	    };
	    ko.extenders.orderStatusType = function (target) {
	        return new TypeExtension(target, app.lists.orderStatus.toDictionary());
	    };
	    ko.extenders.customerOrderStatusType = function (target) {
	        return new TypeExtension(target, app.lists.customerOrderStatus.toDictionary());
	    };

	    ko.extenders.movementTypes = function (target) {
	        var options = {
	            0: 'Same Warehouse',
	            1: 'Between Warehouses',
	        };
	        return new TypeExtension(target, options);
	    };

	    ko.extenders.inventoryOrderTypes = function (target, defaultOption) {
	        return new TypeExtension(target, app.lists.inventoryOrderTypes, defaultOption);
	    };

	    // Data input binding extension. Converts input to numeric values.
	    ko.extenders.numeric = function (target, precision) {
	        console.warn('Replace numeric binding extender with numericObservable object');
	        var mode = 'readonly', isWriteable = false;
	        if (!ko.isWriteableObservable(target)) {
	            mode = 'writeable';
	            isWriteable = true;
	            //throw new Error('Object must be a writableObservable in order to be used with the numeric binding. For read-only binding, use formatNumber instead.');
	        }

	        target.numericMode = mode;
	        if (isWriteable) return writable();
	        else return readonly();

	        function writable() {
	            applyFormatting(target());
	            target.subscribe(applyFormatting, target);
	            return target;

	            function applyFormatting(value) {
	                value = formatValue(value);
	                if (value === target()) return;
	                setValue(value);
	            }
	            function setValue(value) {
	                target(value);
	            }
	        }

	        function readonly() {
	            target.formattedNumber = ko.computed({
	                read: function () {
	                    return formatValue(target()) || undefined;
	                },
	                write: function (val) {
	                    target(formatValue(val) || undefined);
	                }
	            }, target);
	            return target;
	        }

	        function formatValue(input) {
	            var numVal = parseFloat(input);
	            if (isNaN(numVal)) return undefined;
	            else return parseFloat(numVal.toFixed(precision));
	        }
	    };

	    //Read-only binding for displaying numeric values with a specific decimal precision.
	    //For numeric input bindings, use the numeric binding instead.
	    ko.extenders.formatNumber = function (target, precision) {
	        function formatValue(input) {
	            precision = parseInt(precision) || 0;
	            return precision > 0 ? parseFloat(input).toFixed(precision) : parseInt(input);
	        }

	        target.formattedNumber = ko.computed(function () {
	            return formatValue(target()) || 0;
	        }, target);
	        return target;
	    };

	    //******************************
	    // MAPPING HELPERS

	    ko.mappings = ko.mappings || {};
	    ko.mappings.formattedDate = function (options, format) {
	        var dateString = options.data;
	        var date = null;
	        if (typeof dateString == "string" && dateString.length > 0) {
	            if (dateString.match(/^\/Date\(\d*\)\/$/)) {
	                dateString = dateString.replace(/[^0-9 +]/g, '');
	                dateString = parseInt(dateString);
	            }
	            date = new Date(dateString).toISOString();
	        }
	        var result = ko.observable(date).extend({ isoDate: format || 'm/d/yyyy' });
	        return result;
	    };

	    //****************************************
	    // validation rules
	    ko.validation.rules['isUnique'] = {
	        validator: function (newVal, options) {
	            if (options.predicate && typeof options.predicate !== "function")
	                throw new Error("Invalid option for isUnique validator. The 'predicate' option must be a function.");

	            var array = options.array || options;
	            var count = 0;
	            ko.utils.arrayMap(ko.utils.unwrapObservable(array), function (existingVal) {
	                if (equalityDelegate()(existingVal, newVal)) count++;
	            });
	            return count < 2;

	            function equalityDelegate() {
	                return options.predicate ? options.predicate : function (v1, v2) { return v1 === v2; };
	            }
	        },
	        message: 'This value is a duplicate',
	    };

	    /*
	     * Determines if a field is required or not based on a function or value
	     * Parameter: boolean function, or boolean value
	     * Example
	     *
	     * viewModel = {
	     *   var vm = this;     
	     *   vm.isRequired = ko.observable(false);
	     *   vm.requiredField = ko.observable().extend({ conditional_required: vm.isRequired});
	     * }   
	    */
	    ko.validation.rules['conditional_required'] = {
	        validator: function (val, condition) {
	            var required;
	            if (typeof condition == 'function') {
	                required = condition();
	            } else {
	                required = condition;
	            }

	            if (required) {
	                return !(val == undefined || val.length == 0);
	            } else {
	                return true;
	            }
	        },
	        message: "Field is required"
	    };

	    ko.validation.rules['doesNotEqual'] = {
	        validator: function (v1, v2) {
	            ko.validation.rules['doesNotEqual'].message = "\"" + v1 + "\" is not valid.";
	            return v1 !== v2;
	        },
	    };

	    ko.validation.rules['isValidTreatment'] = {
	        validator: function (val) {
	            return val !== app.lists.treatmentTypes.NotTreated.key
	                && val !== app.lists.treatmentTypes.LowBac.key;
	        },
	        message: "Invalid Treatment"
	    };

	    ko.validation.rules['isTrue'] = {
	        validator: function (value, fnInvalid) {
	            return fnInvalid.apply(value) === true;
	        },
	        message: "The new location is the same as the previous location. There is no need to create a movement if the items don't change location.",
	    };

	    ko.validation.registerExtenders();


	    //******************************************
	    // private functions

	    function TypeExtension(target, options, defaultOption) {
	        if (defaultOption === undefined && options.length) defaultOption = options[0];
	        target.displayValue = ko.computed({
	            read: function () {
	                if (target() == undefined) return '';
	                return getTypeOption(target()) || defaultOption;
	            }
	        });
	        target.options = buildSelectListOptions(options);
	        return target;

	        function buildSelectListOptions(source) {
	            var selectListOptions = [];
	            for (var opt in source) {
	                selectListOptions.push({
	                    key: opt,
	                    value: source[opt]
	                });
	            }
	            return selectListOptions;
	        }
	        function getTypeOption(val) {
	            switch (typeof val) {
	                case "string": return fromString(val);
	                case "number": return fromNumber(val);
	                case "object": return fromObject(val);
	                default: return undefined;
	            }

	            function fromString(s) {
	                return fromNumber(parseInt(s))
	                    || findOptionByName();

	                function findOptionByName() {
	                    for (var prop in options) {
	                        if (options[prop] === s) {
	                            return fromString(prop);
	                        }
	                    }
	                    return undefined;
	                }
	            }
	            function fromNumber(num) {
	                if (isNaN(num)) return undefined;
	                return options[num + ''];
	            }
	            function fromObject(o) {
	                if (!o || o.value == undefined) return undefined;
	                return o.value;
	            }
	        }
	    }

	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),

/***/ 32:
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {__webpack_require__(33);
	__webpack_require__(23);

	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9)], __WEBPACK_AMD_DEFINE_RESULT__ = function(ko) {
	    ko.bindingHandlers.hidden = {
	        update: function (element, valueAccessor) {
	            ko.bindingHandlers.visible.update(element, function () {
	                return !ko.utils.unwrapObservable(valueAccessor());
	            });
	        }
	    }
	    ko.bindingHandlers.preventBubble = {
	        init: function (element, valueAccessor) {
	            var eventName = ko.utils.unwrapObservable(valueAccessor());
	            ko.utils.registerEventHandler(element, eventName, function (event) {
	                event.cancelBubble = true;
	                if (event.stopPropagation) {
	                    event.stopPropagation();
	                }
	            });
	        }
	    };

	    ko.bindingHandlers.fixed = {
	      init: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
	        var options = ko.unwrap( allBindings().decimalOptions ) || {};
	        var precision = options.precision || ko.bindingHandlers.fixed.defaultPrecision;

	        if ( $( element ).is('input') ) {
	          var hiddenObservable = valueAccessor();

	          if ( ko.isObservable( hiddenObservable ) && hiddenObservable() === '' ) {
	            hiddenObservable( null );
	          }

	          var transform = ko.pureComputed({
	            read: hiddenObservable,
	            write: function( value ) {
	              if ( value === '' ) {
	                hiddenObservable( null );
	              } else {
	                var num = parseFloat( value.replace( /[^\d\.\-]/g, '' ) );
	                hiddenObservable( num.toFixed( precision ) );
	              }
	            }
	          });

	          ko.bindingHandlers.value.init( element, function() { return transform; }, allBindings );
	        }
	      },
	      update: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
	        var value = ko.unwrap( valueAccessor() );
	        var options = ko.unwrap( allBindings().decimalOptions );
	        var precision = options.precision || ko.bindingHandlers.fixed.defaultPrecision;
	        var formattedValue = parseFloat( value ).toFixed( precision );

	        if ( !isNaN( formattedValue ) ) {
	          $( element ).val( options.commas ? ko.bindingHandlers.fixed.withComma( formattedValue ) : formattedValue );
	        } else {
	          $( element ).val( '' );
	        }
	      },
	      defaultPrecision: 2,
	      withComma: function( value ) {
	        var vals = value.split('.');
	        vals[0] = Number( vals[0] ).toLocaleString();

	        return vals.join('.');
	      }
	    };

	    /**
	      * Bootstrap Modal Binding
	      * Bind to modal wrapper with class `.modal`
	      * Refer to http://getbootstrap.com/javascript/#modals for modal structure
	      *
	      * @param {boolean} valueAccessor - Toggles modal visibility
	      */
	    ko.bindingHandlers.modal = {
	      init: function (element, valueAccessor) {
	        $(body).append($(element));
	        $(element).remove();
	        $(element).modal({
	          show: false
	        });

	        var value = valueAccessor();
	        if (ko.isObservable(value)) {
	          $(element).on('hide.bs.modal', function() {
	            value(false);
	          });
	        }
	      },
	      update: function (element, valueAccessor) {
	        var value = valueAccessor();
	        if (ko.utils.unwrapObservable(value)) {
	          $(element).modal('show');
	        } else {
	          $(element).modal('hide');
	        }
	      }
	    };

	    /**
	      * @param {Object} valueAccessor - Value to monitor for update function
	      */
	    ko.bindingHandlers.floatThead = {
	      init: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
	        if ( element.nodeName.toLowerCase() !== "table" ) {
	          throw new Error('The floatThead binding must be set on a table element');
	        }

	        var $tableSelector = $( element );
	        var value = valueAccessor();
	        var valueUnwrapped = ko.unwrap( value );

	        $tableSelector.parent().addClass('sticky-head-container');
	        $tableSelector.addClass('sticky-head');

	        if ( ko.isObservable( value ) && (valueUnwrapped == null || (Array.isArray( valueUnwrapped) && value.length === 0)) ) {
	          var valueSubscription = value.subscribe( function( newValue ) {
	            floatThead( $tableSelector );
	            valueSubscription.dispose();
	          });
	        } else {
	          floatThead( $tableSelector );
	        }

	        ko.utils.domNodeDisposal.addDisposeCallback( element, function() {
	          $tableSelector.floatThead('destroy');
	        });

	        function floatThead( $tableSelector ) {
	          $tableSelector.floatThead({
	            scrollContainer: function ( $table ) {
	              return $table.closest('.sticky-head-container');
	            }
	          });
	        }
	      },
	      update: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
	        var value = valueAccessor();
	        var valueUnwrapped = ko.unwrap(value);

	        $( element ).floatThead('reflow');
	      }
	    };

	    ko.bindingHandlers.loadingMessage = {
	      init: function (element, valueAccessor) {
	        $('body').append($(element));
	        $(element).hide();
	      },
	      update: function(element, valueAccessor) {
	        var value = valueAccessor();
	        var valueUnwrapped = ko.unwrap(value);

	        if (!!valueUnwrapped) {
	          $(element).fadeIn();
	        } else {
	          $(element).fadeOut();
	        }
	      }
	    };

	    /**
	      * @deprecated Use modal instead
	      */
	    ko.bindingHandlers.dialog = {
	        init: function (element, valueAccessor, allBindings, bindingContext) {
	            console.debug('dialog has been deprecated, use modal instead');
	            var defaultConfig = {
	                    modal: true,
	                },
	                $element = $(element),
	                value = valueAccessor(),
	                commands = parseCommands();

	            initDialog();


	            // prevent duplicate binding error?
	            ko.cleanNode($element);
	            $element.removeAttr('data-bind');
	            $element.children(function() {
	                this.removeAttr('data-bind');
	            });

	            var dialogDestroyed = false;
	            if (ko.isObservable(value)) {
	                var valueSubscriber = value.subscribe(function (val) {
	                    if (!dialogDestroyed) $element.dialog(val ? "open" : "close");
	                });

	                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                    if ($element.dialog('option')) $element.dialog("destroy");
	                    dialogDestroyed = true;
	                    valueSubscriber.dispose && valueSubscriber.dispose();
	                });
	            }

	            attachKoCommands();

	            function initDialog() {
	                // Creates empty function for any configured buttons.
	                // This will cause the buttons to be displayed while allowing
	                // execution to be deferred to the supplied command object.

	                var options = ko.utils.extend(allBindings() || {}, defaultConfig);

	                var config = {
	                    modal: options.modal,
	                    height: options.height,
	                    width: options.width,
	                    position: options.position,
	                    buttons: { },
	                    close: options.close || options.cancelCommand,
	                    title: options.title,
	                    autoOpen: ko.utils.peekObservable(value) && true || false,
	                    dialogClass: options.cancelCommand ? 'no-close' : '',
	                };

	                for (var prop in commands) {
	                    if (commands.hasOwnProperty(prop))
	                        config.buttons[prop] = empty;
	                }

	                $element.dialog(config);

	                function empty() { }
	            }

	            function parseCommands() {
	                var exports = {},
	                    bindings = allBindings() || {};

	                parseCommand(bindings['cancelCommand'], 'cancelCommand', 'Cancel');
	                parseCommand(bindings['okCommand'], 'okCommand', 'Ok');

	                var customCommands = getCustomCommands();
	                for (var prop in customCommands) {
	                    if (customCommands.hasOwnProperty(prop))
	                        parseCommand(customCommands[prop], prop, prop);
	                }
	                return exports;

	                function getCustomCommands() {
	                    return allBindings().customCommands || allBindings().customCommand || [];
	                }
	                function parseCommand(cmd, bindingName, mapToButtonName) {
	                    if (!cmd) return;
	                    if (!cmd.execute) {
	                        cmd = ko.command({
	                            execute: cmd
	                        });
	                    }
	                    exports[mapToButtonName || bindingName] = cmd;
	                }


	            }

	            function attachKoCommands() {
	                var buttonFunctions = $element.dialog("option", "buttons");
	                var newButtonsConfig = [];
	                for (var funcName in buttonFunctions) {
	                    for (var cmdName in commands) {
	                        if (cmdName == funcName) {
	                            var buttons = $(".ui-dialog-buttonpane button:contains('" + cmdName + "')");

	                            $.each(buttons, function (index) {
	                                var command = commands[cmdName];

	                                ko.bindingHandlers.command.init(
	                                    buttons[index],
	                                    ko.utils.wrapAccessor(command),
	                                    allBindings,
	                                    null,
	                                    bindingContext);

	                                // remove click functionality from the jQuery UI element
	                                newButtonsConfig.push({
	                                    text: cmdName,
	                                    click: empty,
	                                });
	                            });
	                            break;
	                        }
	                    }
	                }

	                function empty() { }
	            }
	        },
	    };

	    ko.bindingHandlers.cancelKey = {
	        init: function (element, valueAccessor, allBindings, viewModel) {
	            var delegate = ko.utils.unwrapObservable(valueAccessor());

	            if (delegate && typeof delegate !== 'function' && typeof delegate.execute === "function") {
	                delegate = delegate.execute;
	            }
	            if (delegate == undefined) return;

	            var cancelKeyCode = 27;
	            var elementToRegister = element;

	            if (attachToWindow()) {
	                var conditionFn = function () { return $(element).is(':visible'); };
	                elementToRegister = window;
	            }
	            ko.utils.registerEventHandler(elementToRegister, 'keydown', buildEventHandler(conditionFn));

	            function attachToWindow() {
	                var bindings = ko.utils.unwrapObservable(allBindings);
	                return bindings && ko.utils.unwrapObservable(bindings.attachToWindow) === true;
	            }

	            function buildEventHandler(conditionalFn) {
	                conditionalFn = conditionalFn || function () { return true; };
	                return function (event) {
	                    if (event.keyCode == cancelKeyCode && conditionalFn()) {
	                        executeCancel(event);
	                        event.cancelBubble = true;
	                        if (event.stopPropagation) event.stopPropagation();
	                    }
	                };
	            }

	            function executeCancel(event) {
	                event.preventDefault();
	                event.target.blur();
	                delegate.call(viewModel);
	            }
	        }
	    };

	    ko.bindingHandlers.onblur = {
	        init: function (element, valueAccessor) {
	            var fn = valueAccessor();
	            if (fn && fn.execute) fn = commandWrapper.bind(fn);
	            ko.utils.registerEventHandler(element, 'blur', fn);

	            function commandWrapper() {
	                this.execute();
	            }
	        }
	    };

	    ko.bindingHandlers.growToWindowHeight = {
	        init: function (element, valueAccessor) {
	            var $element = $(element);
	            var bindings = $.extend({}, ko.bindingHandlers.growToWindowHeight.DEFAULT_OPTIONS, ko.unwrap(valueAccessor()) || {});

	            var $windowHeight = $(window).height();
	            var viewportHeight = document.body && document.body.clientHeight
	                ? Math.min(document.body.clientHeight, $windowHeight)
	                : $windowHeight;
	            var windowHeight = viewportHeight - bindings.offset;
	            $element.height(windowHeight);
	            $element.css('overflow', 'auto');
	        },
	        DEFAULT_OPTIONS: {
	            offset: 0
	        }
	    };

	    ko.bindingHandlers.maxHeight = {
	        init: function (element, valueAccessor, allBindings) {
	            var $element = $(element);
	            $element.addClass('fullWindowHeight');
	            constrainHeight.call($element);

	            var value = valueAccessor();
	            if (ko.isObservable(value)) {
	                var sub = value.subscribe(function() {
	                    setupStickyTableElements();
	                    sub.dispose();
	                });
	            } else setupStickyTableElements();

	            function setupStickyTableElements() {
	                prepareTables(element, valueAccessor(), allBindings());
	            }
	        },
	    };

	    function constrainHeight() {
	        var windowHeight = $(window).height();
	        this.height(windowHeight);
	        //this.css('max-height', windowHeight);
	        this.css('overflow-y', 'scroll');
	        this.css('overflow-x', 'scroll');
	    }

	    function prepareTables(element, value, allBindings) {
	        var $element = $(element);
	        var opts = allBindings || {};

	        if ($element.is('table')) {
	            $element = $(element).wrap("<div><div>").parent();
	        }

	        initStickyTableBinding($element, value, opts, 'stickyTableHeaders');
	        //initStickyTableBinding($element, value, opts, 'stickyTableFooters');
	    }

	    function initStickyTableBinding(element, value, opts, bindingName) {
	        var valueAccessor = opts[bindingName]
	                ? ko.utils.wrapAccessor(opts[bindingName])
	                : ko.utils.wrapAccessor(value || true);

	        var $element = $(element);

	        var template = getTemplatedChild();
	        if (template) {
	            if (!opts[bindingName]) return;
	            opts.dependsOn = template;
	            var bindings = ko.utils.wrapAccessor(opts);
	            ko.bindingHandlers[bindingName].init(template, valueAccessor, bindings);
	        } else {
	            opts.parent = $element;
	            $element.find(opts[bindingName] || 'table').each(function () {
	                var $this = $(this);
	                ko.bindingHandlers[bindingName].init($this, valueAccessor, ko.utils.wrapAccessor(opts));
	                removeBinding($this, bindingName);
	            });
	        }

	        function getTemplatedChild() {
	            var child = getChild();
	            if (!child) return null;
	            var childContext = ko.contextFor(child);
	            if (!childContext) return null;

	            var childBindings = ko.bindingProvider.instance.getBindings(child, childContext);
	            return childBindings && childBindings.template ? child : null;

	            function getChild() {
	                return $element.children(':first')[0]
	                    || getVirtualElementChild();
	            }
	            function getVirtualElementChild() {
	                var vChild = ko.virtualElements.firstChild($element[0]);
	                return vChild && ko.virtualElements.nextSibling(vChild);
	            }
	        }

	        function removeBinding(table, binding) {
	            var dataBind = table.attr('data-bind');
	            if (dataBind) {
	                var regex = new RegExp(binding + "\:\s?\w+\W?\s?", "i");
	                dataBind = dataBind.replace(regex, "");
	                table.attr('data-bind', dataBind);
	            }
	        }
	    }

	    ko.bindingHandlers.fixCvpOverlay = {
	        init: function (element, valueAccessor) {
	            var $container = $(element).wrap('<div />').parent();
	            //ko.bindingHandlers.fixCvpOverlay.update(element,valueAccessor);
	        },
	        update: function (element, valueAccessor) {
	            valueAccessor().notifySubscribers(); // fix initial overlay
	            ko.utils.unwrapObservable(valueAccessor());
	            var $cvp = $("#cvp");
	            var cvpWidth = $cvp.outerWidth();
	            var $element = $(element);
	            var inventoryTableWidth = $element.width();

	            // When element contains an enumerated child (such as a foreach binding), the
	            // width function returns 0. This hacky little fix will set a default width.
	            if (inventoryTableWidth == 0) {
	                inventoryTableWidth = 5000; // default width
	            }

	            var $container = $element.parent();
	            $container.width(inventoryTableWidth).css({ "padding-right": cvpWidth + 85 });
	        }
	    };

	    ko.bindingHandlers.undo = {
	        init: function (element, valueAccessor, allBindings, viewModel) {
	            var bindings = {};
	            var trackedBindingNames = ['value'];
	            var isEditing = ko.computed(function () {
	                return ko.utils.unwrapObservable(valueAccessor());
	            });
	            var elementBindings = ko.bindingProvider.instance.getBindings(element, ko.contextFor(element));
	            ko.utils.arrayForEach(trackedBindingNames, function (binding) {
	                if (elementBindings[binding]) {
	                    bindings[binding] = elementBindings[binding];
	                }
	            });

	            for (var boundProp in bindings) {
	                initializeTracking(bindings[boundProp]);
	            }

	            function revert(propAccessor) {
	                var initalValue = propAccessor.changeHistory()[0];
	                propAccessor(initalValue);
	            }

	            function initializeTracking(propAccessor) {
	                propAccessor.changeHistory = ko.observableArray([propAccessor()]);

	                propAccessor.subscribe(function (newVal) {
	                    propAccessor.changeHistory.push(newVal);
	                });

	                setupRevertTrigger(propAccessor);
	            }

	            isEditing.subscribe(function (newVal) {
	                $cancelButton.each(function (index, button) {
	                    ko.bindingHandlers.visible.update(
	                        button,
	                        ko.utils.wrapAccessor(newVal),
	                        allBindings,
	                        data
	                    );
	                });
	            });

	            function setupRevertTrigger(propAccessor) {
	                // eventually, we'll enable actual undo/redo stepping but for now, we just
	                // handle both as a revert function.
	                var revertCommand = ko.command({
	                    execute: function () {
	                        revert(propAccessor);
	                    },
	                    canExecute: function () {
	                        return propAccessor.changeHistory().length > 1;
	                    }
	                });
	                propAccessor.revertCommand = revertCommand;
	                var trigger = allBindings().undoTrigger || allBindings().revertTrigger;

	                $(trigger).each(function (index, button) {
	                    ko.bindingHandlers.command.init(
	                        button,
	                        ko.utils.wrapAccessor(revertCommand),
	                        allBindings,
	                        viewModel
	                    );
	                });
	            }
	        }
	    };

	    ko.bindingHandlers.pageData = {
	        update: function (element, valueAccessor) {
	            ko.utils.unwrapObservable(valueAccessor());
	            $(element).hide().fadeIn(500);
	        }
	    };

	    ko.bindingHandlers.editableContent = {
	        init: function (element, valueAccessor, allBindings, data) {
	            var savedState = ko.observable();
	            var isEditing = ko.computed(function () {
	                return ko.utils.unwrapObservable(valueAccessor());
	            });
	            var $element = $(element);
	            var $cancelButton = $(allBindings().cancelTrigger);
	            var $masterCancelButton = $(allBindings().masterCancelTrigger);

	            if (!isEditing()) { $element.attr("readonly", "readonly"); }

	            ko.bindingHandlers.undo.init(
	                element,
	                ko.utils.wrapAccessor(function () { return true; }),
	                ko.utils.wrapAccessor({ revertTrigger: $cancelButton ? $cancelButton[0] : undefined }),
	                data);

	            ko.bindingHandlers.click.init(
	                element,
	                ko.utils.wrapAccessor(beginEdit),
	                ko.utils.wrapAccessor({}),
	                data);

	            //todo: handle blur events (and allow disabling the blur handlers)

	            //todo: 1. prevent bubbling, 2. enable canceling when cancel button is not supplied
	            ko.bindingHandlers.cancelKey.init(
	                element,
	                ko.utils.wrapAccessor(function () { $cancelButton.click(); }),
	                ko.utils.wrapAccessor({ keydownBubble: false }),
	                data
	            );


	            setupCancelButtons();

	            function setupCancelButtons() {
	                var cancelCommand = allBindings().cancelEditsCommand;
	                if ($cancelButton.length > 0) {
	                    $cancelButton.each(function (index, button) {
	                        ko.bindingHandlers.command.init(
	                            button,
	                            ko.utils.wrapAccessor(cancelCommand),
	                            function () { return { clickBubble: false }; },
	                            data);
	                    });
	                }

	                $masterCancelButton.each(function (index, button) {
	                    var context = ko.contextFor(button);
	                    if (context) {
	                        var commandBinding = ko.bindingProvider.instance.getBindings(button, context).command;
	                        if (commandBinding) {
	                            if (typeof commandBinding.addCommand != "function") {
	                                throw new Error('The masterCancelCommand is only supported with a composableCommand instance.');
	                            }

	                            commandBinding.addCommand(ko.command({
	                                execute: function () { $cancelButton.click(); }
	                            }));
	                        }
	                    }
	                });
	            }

	            function beginEdit() {
	                if (isEditing()) return;
	                valueAccessor()(true);
	                var bindings = ko.bindingProvider.instance.getBindings(element, ko.contextFor(element));
	                savedState(ko.utils.unwrapObservable(bindings.value));
	            }
	        },
	        update: function (element, valueAccessor) {
	            var isEditing = ko.utils.unwrapObservable(valueAccessor());
	            if (isEditing === false) {
	                $(element).attr("readonly", "readonly");
	            } else {
	                $(element).removeAttr("readonly");
	            }
	        }
	    };

	    ko.bindingHandlers.editableContentArea = {
	        init: function (element, valueAccessor, allBindings, data) {
	            var $element = $(element);
	            var isEditing = ko.computed(function () {
	                return ko.utils.unwrapObservable(valueAccessor());
	            });

	            function setIsEditingValue(val) {
	                valueAccessor()(val);
	            }
	            var inputElements = $('input', $element).not('[type="button"], [type="submit"]');
	            var allChildrenEmpty = ko.computed(function () {
	                var firstDefinedValue = ko.utils.arrayFirst(inputElements, function (e) {
	                    var ctx = ko.contextFor(e);
	                    var binding = ctx ? ko.bindingProvider.instance.getBindings(e, ctx) : undefined;
	                    var value = binding ? binding.value() : undefined;
	                    return value != undefined;
	                });
	                return firstDefinedValue === null;
	            });
	            var isCancelVisible = ko.computed(function () {
	                return isEditing() && !allChildrenEmpty();
	            });

	            valueAccessor().__editableContentArea__inputElements = inputElements;

	            var cancelValueAccessor = ko.utils.wrapAccessor(function () { return false; });
	            cancelValueAccessor().__editableContentArea__inputElements = inputElements;

	            var cancelCommand = ko.command({
	                execute: function () {
	                    setIsEditingValue(false);
	                },
	            });

	            // currently requires cancelTrigger binding to be provided
	            var $cancelButton = $element.find(allBindings().cancelTrigger);

	            // initialize visibility
	            updateCancelButtonVisibility();

	            // update visibility
	            isCancelVisible.subscribe(function () {
	                updateCancelButtonVisibility();
	            });

	            allBindings().cancelTrigger = $cancelButton ? $cancelButton[0] : undefined;
	            allBindings().cancelEditsCommand = cancelCommand;

	            $.each(inputElements, function (index, elem) {
	                ko.bindingHandlers.editableContent.init(elem, valueAccessor, allBindings, data);
	            });

	            function updateCancelButtonVisibility() {
	                $cancelButton.each(function (index, button) {
	                    ko.bindingHandlers.visible.update(
	                        button,
	                        function () { return isCancelVisible(); },
	                        allBindings,
	                        data
	                    );
	                });
	            }
	        },
	        update: function (element, valueAccessor, allBindings, data) {
	            var inputElements = valueAccessor().__editableContentArea__inputElements;
	            var isEditing = ko.utils.unwrapObservable(valueAccessor());

	            $.each(inputElements, function (index, elem) {
	                ko.bindingHandlers.editableContent.update(
	                    elem,
	                    ko.utils.wrapAccessor(isEditing),
	                    allBindings,
	                    data
	                );
	            });
	        },
	    };



	    function initVisibleToggle(element, dataValue, isNot) {
	        var $el = $(element);
	        if (isNot ? !dataValue : (dataValue && true)) {
	            $el.hide();
	            $el.slideDown();
	        } else {
	            $el.slideUp();
	        }

	        $el = null;
	    }

	    function updateVisibleToggle(element, dataValue, opts, isNot) {
	        var defaults = {
	            showDuration: "slow",
	            hideDuration: "slow",
	            speed: false,
	            direction: "down",
	        };

	        var options = $.extend({}, defaults, opts);
	        var $el = $(element);
	        if (options && options.speed) options.showDuration = options.hideDuration = options.speed;

	        if (isNot ? !dataValue : (dataValue && true)) {
	            $el.slideDown(options.showDuration);
	        } else {
	            $el.slideUp(options.hideDuration);
	        }
	    }

	    ko.bindingHandlers.slideVisible = {
	        init: function (element, valueAccessor) {
	            initVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), false);
	        },
	        update: function (element, valueAccessor, allBindings) {
	            updateVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), allBindings(), false);
	        }
	    };

	    ko.bindingHandlers.slideCollapsed = {
	        init: function(element, valueAccessor) {
	            initVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), true);
	        },
	        update: function (element, valueAccessor, allBindings) {
	            updateVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), allBindings(), true);
	        }
	    }

	    ko.bindingHandlers.popup = {
	        init: function (element, valueAccessor, allBindings) {
	            var $element = $(element);
	            $element.addClass('popupWindow');

	            var defaults = {
	                attachCancelCommandToWindow: true,
	            };
	            var options = $.extend({}, defaults, allBindings());
	            var borderWidth = parseInt($element.css('border-left-width'), 10) || 10; // parseInt trims the 'px' and returns base-10 value

	            $(element).on('click', onCloseEvent);

	            if (options.closePopupCommand) {
	                var cancelKeyOptions = options;
	                cancelKeyOptions.attachToWindow = options.attachCancelCommandToWindow;
	                ko.bindingHandlers.cancelKey.init(element, ko.utils.wrapAccessor(options.closePopupCommand), ko.utils.wrapAccessor(cancelKeyOptions));
	            }

	            ko.bindingHandlers.slideIn.init(element, valueAccessor);

	            // handle cleanup
	            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                $element.off('click', onCloseEvent);
	            });

	            // private functions
	            function onCloseEvent(e) {
	                var hitAreaX = borderWidth + $element.position().left;
	                // Reece - Replaced pageX with screenX b/c when
	                // clicking a selectBox,  pageX value is relative to
	                // selectbox -- not the page.
	                if (e.pageX && e.pageX <= hitAreaX) {
	                    if (options.closePopupCommand && typeof options.closePopupCommand.execute == "function") {
	                        options.closePopupCommand.execute();
	                        return;
	                    }
	                    if (ko.isWriteableObservable(valueAccessor())) {
	                        valueAccessor()(false);
	                        return;
	                    }
	                    ko.bindingHandlers.popup.update(element, ko.utils.wrapAccessor(false), allBindings);
	                }
	            }
	        },
	        update: function (element, valueAccessor, allBindings) {
	            ko.bindingHandlers.slideIn.update(element, valueAccessor, allBindings);
	        }
	    };

	    ko.bindingHandlers.slideIn = {
	        init: function (element, valueAccessor) {
	            var display = ko.utils.unwrapObservable(valueAccessor());

	            var $element = $(element);
	            $element.show();
	            if (!display) {
	                $element.hide();
	                //$element.css({ left: $(window).width() });
	                $element.css({ left: "100%" });
	            }
	        },
	        update: function (element, valueAccessor) {
	            var $element = $(element);
	            var display = ko.utils.unwrapObservable(valueAccessor());
	            if (display) {
	                $element.show();
	                $element.animate({ left: 0 });
	            } else {
	                $element.animate({ left: "100%" });
	                $element.hide();
	            }
	        }
	    };

	    ko.bindingHandlers.fadeVisible = {
	        init: function (element) {
	            $(element).hide();
	        },
	        update: function (element, valueAccessor) {
	            var value = ko.utils.unwrapObservable(valueAccessor());
	            if (value) $(element).fadeIn();
	            else $(element).fadeOut();
	        }
	    };

	    ko.bindingHandlers.stickyTableHeaders = {
	        init: function (element, valueAccessor, allBindings) {
	            var value = ko.utils.unwrapObservable(valueAccessor());
	            var bindings = allBindings();
	            var $table;
	            var options = {};
	            var completed = false;
	            var $element = $(element);

	            options.tabs = bindings.tabbedParent;
	            options.myTab = bindings.myTab;

	            if (typeof value === "string") {
	                $table = $element.find(value);
	                options.parent = $element;
	            } else {
	                $table = $(element);
	                options.parent = bindings.parent;
	            }

	            if ($table == undefined) throw new Error("The table element can not be found. Selector: '" + value + "'.");

	            bindTable();

	            function bindTable() {
	                //Enables the jQuery transformation to be deferred until after the dependent object has data
	                var dependsOn = bindings['dependsOn'];
	                if (dependsOn && deferToDependency()) {
	                    return;
	                }
	                stickyHeaders($table, options);

	                function deferToDependency() {
	                    var $dependency = typeof dependsOn === "string"
	                        ? $element.children(':first')
	                        : $(dependsOn);
	                    if (!$dependency) return false;

	                    var dependencyElement = $dependency[0];
	                    var dependencyContext = ko.contextFor(dependencyElement);
	                    var dependencyBindings = ko.bindingProvider.instance.getBindings(dependencyElement, dependencyContext);

	                    if (dependencyHasTemplate()) {
	                        var fnName = '__stickyTableHeaders__updateHeaders__';
	                        if (isVirtualElement()) {
	                            dependsOn.data = attachAfterRenderBinding.call(dependsOn.data);
	                            dependencyContext.$data[fnName] = function () {
	                                var table = typeof (value) === "string"
	                                    ? $(arguments[0]).filter(value) || $element.find(value)
	                                    : value;

	                                if (!table.length) {
	                                    console.error("The table element could not be found. When attaching stickyTableHeaders within template, the value parameter should contain a selector for the table.");
	                                    return;
	                                }

	                                var context = ko.contextFor(dependsOn);
	                                var theadDependency = bindings.stickyTableHeaderDependency;
	                                if (typeof theadDependency === "string") theadDependency = context.$data[theadDependency];
	                                options.parent = table;
	                                if (ko.isObservable(theadDependency)) {
	                                    theadDependency.subscribe(function () {
	                                        stickyHeaders(table, options);
	                                    });
	                                } else {
	                                    stickyHeaders(table, options);
	                                }
	                            };
	                        } else {
	                            var binding = attachAfterRenderBinding.call($dependency.attr('data-bind'));
	                            $dependency.attr('data-bind', binding);
	                            dependencyContext.$data[fnName] = function () {
	                                stickyHeaders($element.find(value), options);
	                            };
	                        }
	                        return true;

	                    }

	                    return false;

	                    function isVirtualElement() {
	                        return dependencyElement.nodeType === 8;
	                            //virtualNoteBindingValue is apparently only available to the debug version of KO.
	                            //&& ko.virtualElements.virtualNodeBindingValue
	                            //&& ko.virtualElements.virtualNodeBindingValue(dependsOn);
	                    }
	                    function dependencyHasTemplate() {
	                        return dependencyBindings && dependencyBindings.template;
	                    }
	                    function attachAfterRenderBinding() {
	                        return this.replace(/(template\:\s?\{)/, "$1" + 'afterRender:' + fnName + ',');
	                    }
	                }
	            }

	            function stickyHeaders(table, opts) {
	                table.each(function () {
	                    if (!this.tagName || this.tagName.toLowerCase() !== 'table') {
	                        throw new Error("The bound element is not a table element. Element selector: '" + value + "'");
	                    }
	                });

	                opts.floatingElementId = 'stickyTableHeader';
	                opts.target = 'thead:first';

	                table.stickyTableHeaders(opts);

	                table.each(function () { rebind.call(this, opts); });

	                var valueSubscription;
	                if (ko.isObservable(valueAccessor())) {
	                    valueSubscription = valueAccessor().subscribe(function () {
	                        table.stickyTableHeaders('option', 'format');
	                    });
	                }

	                var rebuildSubscription;
	                if (bindings.rebuildTrigger) {
	                    if (!ko.isObservable(bindings.rebuildTrigger))
	                        throw new Error("Invalid binding: \"rebuildTrigger\". Must be observable object.");

	                    rebuildSubscription = bindings.rebuildTrigger.subscribe(function () {
	                        table.stickyTableHeaders("option", "refresh");
	                        table.each(function () { rebind.call(this, opts); });
	                    });
	                }

	                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                    table.stickyTableHeaders('destroy');
	                    valueSubscription && valueSubscription.dispose();
	                    rebuildSubscription && rebuildSubscription.dispose();
	                });

	                completed = true;
	            }

	            function rebind(opts) {
	                var floatingElements = $('.' + opts.floatingElementId, this);
	                var floatingClone = floatingElements[0];

	                var context = ko.contextFor(this);
	                if (!context || !floatingClone) return;

	                ko.cleanNode(floatingClone);
	                ko.applyBindings(context.$data, floatingClone);

	                var $clone = $(floatingClone);
	                var bindings = ko.bindingProvider.instance.getBindings(this, ko.contextFor(this));

	                // reformat elements if the clone was templated
	                if (bindings.template) {
	                    $element.stickyTableHeaders('option', 'format');
	                }

	                if (bindings.sortableTable) {
	                    ko.bindingHandlers.sortableTable.init($clone.parent()[0], ko.utils.wrapAccessor(bindings.sortableTable), ko.utils.wrapAccessor(bindings));
	                }

	            }
	        },
	    };

	    var templateRegEx = /(?:^|,|\s)template\s*:\s*(?:(?:(?:'|\")([^(?:'|"|\s|\{)]+)\s*(?:'|"))|(?:\{.*name\s*:\s*(?:(?:'|\")([^(?:'|"|\s|\{)]+)(?:'|"|\s))))/i;
	    ko.bindingHandlers.stickyTableFooters = {
	        init: function (element, valueAccessor, allBindingsAccessor) {
	            var opts = ko.utils.unwrapObservable(allBindingsAccessor());
	            opts.floatingElementId = 'stickyTableFooter';
	            opts.target = 'tfoot:first';

	            var $element = $(element);

	            $element.stickyTableFooters(opts);

	            var table = element;

	            $(opts.target, table).each(function () {
	                var floatingElements = $('.' + opts.floatingElementId, table);
	                if (!floatingElements.length) return;
	                var floatingClone = floatingElements[0];


	                var context = ko.contextFor(this);
	                if (!context) return;

	                ko.cleanNode(floatingClone);
	                ko.applyBindings(context.$data, floatingClone);

	                // reformat elements if the clone was templated
	                var $clone = $(floatingClone);
	                var dataBind = $clone.attr('data-bind');
	                if (dataBind) {
	                    var matches = dataBind.match(templateRegEx);
	                    if (matches && matches.length) {
	                        $element.stickyTableFooters('option', 'format');
	                    }
	                }
	            });

	            var value = valueAccessor();
	            if (ko.isObservable(value)) {
	                value.subscribe(function() {
	                    $element.stickyTableFooters('option', 'format');
	                });
	            }

	            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                $element.stickyTableFooters('destroy');
	            });
	        },
	    };

	    ko.bindingHandlers.tooltip = {
	        init: function (element, bindingAccessor, allBindings) {
	            var value = ko.utils.unwrapObservable(bindingAccessor()),
	                bindings = allBindings && allBindings() || {};

	            if (typeof value == "number") value = value.toString();
	            if (!value || value.length == 0) return;

	            var $element = $(element);
	            $element.attr('title', value);
	            $element.tooltip({
	                track: bindings.tooltipTrack,
	            });
	            //todo: enable updates to the tooltip value
	        },
	    };

	    ko.bindingHandlers.datePickerSm = {
	        init: function (element, valueAccessor, allBindings) {
	            $(element).wrap('<div class="input-group input-group-sm"></div>');
	            $(element).datepicker({
	                showOn: 'button',
	                buttonText: '<i class="fa fa-calendar"></i>',
	                changeMonth: true,
	                changeYear: true
	            }).next(".ui-datepicker-trigger")
	                .addClass("btn btn-default")
	                .wrap('<span class="input-group-btn"></span>');

	            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                //todo: cleanup wrapper element
	                $(element).datepicker('destroy');
	            });

	            var value = valueAccessor();
	            if (ko.isObservable(value)) {
	                ko.bindingHandlers.value.init(element, valueAccessor, allBindings);
	            }
	        }
	    };

	    ko.bindingHandlers.autoHeightTextarea = {
	        init: function (element, valueAccessor) {
	        },
	        update: function (element, valueAccessor) {
	            element.style.height = '0';
	            element.style.height = element.scrollHeight + 'px';
	        }
	    };

	    // autocomplete: listOfCompletions
	    ko.bindingHandlers.autocomplete = {
	        init: function (element, valueAccessor) {
	            var disposables = [];
	            var value = ko.utils.unwrapObservable(valueAccessor());
	            var opts = {
	                //minLength: 0,
	                change: onChange
	            };
	            $( element ).wrap('<div class="ui-front"></div>');

	            function buildSourceOptions( value ) {
	              if ( value.length ) {
	                opts.source = ko.utils.arrayMap( value, function(c) {
	                  if ( c.Name && !c.label ) {
	                    c.label = c.Name;
	                  }

	                  return c;
	                });
	              } else {
	                opts = $.extend( opts, value );

	                if ( !value.source ) {
	                  console.log("Invalid parameters for the autocomplete binding. Value must be either an array or and object with a \"source\" property containing an array.");
	                  return;

	                  //the following line was causing an error when closing a pack schedule after it's been in edit mode.
	                  throw new Error("Invalid parameters for the autocomplete binding. Value must be either an array or and object with a \"source\" property containing an array.");
	                }

	                if ( value.label || value.value ) {
	                  var labelProjector = buildProjector( value.label ),
	                  valueProjector = value.value ? buildProjector( value.value ) : function() { return value; };

	                  opts.source = ko.utils.arrayMap( ko.utils.unwrapObservable( value.source ), function ( item ) {
	                    return {
	                      label: labelProjector(item),
	                      value: valueProjector(item),
	                    };
	                  });
	                } else {
	                  opts.source = ko.utils.unwrapObservable(value.source);
	                }
	              }
	            }

	            function buildProjector( src ) {
	              var prop = ko.utils.unwrapObservable(src);

	              if (prop == undefined) {
	                throw new Error("Projector property is undefined.");
	              }

	              return typeof prop === "function" ?
	                function (object) { return prop(object); } :
	                function(object) { return object[prop]; };
	            }

	            if ( ko.isObservable( value ) ) {
	              disposables.push( value.subscribe(function( optionsSource ) {
	                buildSourceOptions( optionsSource );
	                $( element ).autocomplete( opts );
	              }));
	            } else if ( ko.isObservable( value.source ) ) {
	              disposables.push( value.source.subscribe(function( optionsSource ) {
	                buildSourceOptions( optionsSource );
	                $( element ).autocomplete( opts );
	              }));
	            }

	            buildSourceOptions( value );
	            $( element ).autocomplete( opts );

	            function onChange (e, ui) {
	                var bindingContext = ko.contextFor(element);
	                if (!bindingContext) return;
	                var bindings = ko.bindingProvider.instance.getBindings(element, bindingContext) || {};
	                if (!bindings.value) return;

	                if (ui.item && ui.item.value) {
	                    bindings.value(ui.item.value);
	                }
	                    // enable new elements to be added to the list
	                else if (opts.allowNewValues) bindings.value($(this).val());
	                else {
	                    bindings.value(null);
	                    if (ko.DEBUG) {
	                        console.log('The selected value was not found in the options list. To allow new values, include the \"allowNewValues=\'true\'\" value in the \"autocompleteOptions\" binding attribute.');
	                    }
	                }
	            }

	            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
	              $( element ).autocomplete( 'destroy' );
	              ko.utils.arrayForEach( disposables, function( disposable ) {
	                disposable.dispose();
	              });
	            });
	        },
	    };

	    ko.bindingHandlers.tabs = {
	        init: function (element, valueAccessor, allBindings) {
	            var $element = $(element);
	            var value = ko.utils.unwrapObservable(valueAccessor());
	            $(element).val(value);

	            var options = ko.utils.unwrapObservable(allBindings().tabOptions) || {};

	            $element.on("tabsactivate", onTabActivate);
	            $element.on("tabscreate", onTabCreate);

	            $element.tabs(options);

	            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                $element.tabs("destroy");
	                $element.off('tabsactivate', onTabActivate);
	                $element.off('tabscreate', onTabCreate);
	            });


	            function onTabActivate(event, ui) {
	                bindValueFromUI(ui.newTab.text(), ui.newPanel[0]);
	            }

	            function onTabCreate(event, ui) {
	                bindValueFromUI(ui.tab.text(), ui.panel[0]);
	            }

	            function bindValueFromUI(name, panel) {
	                if (!ko.isObservable(valueAccessor())) return;
	                valueAccessor()({
	                    name: name,
	                    data: getDataBoundObjectFor(panel)
	                });
	            }

	            function getDataBoundObjectFor(tabPanel) {
	                if (!tabPanel) return undefined;
	                var panelContext = ko.contextFor(tabPanel);
	                if (!panelContext) return undefined;
	                var bindings = ko.bindingProvider.instance.getBindings(tabPanel, panelContext) || {};
	                return bindings.with || panelContext.$data;
	            }
	        },
	    };

	    ko.bindingHandlers.ajaxStatus = {
	        init: function (element, valueAccessor) {
	            var value = valueAccessor();
	            if (value.ajaxSuccess == undefined
	                || value.ajaxFailure == undefined
	                || value.ajaxWorking == undefined) throw new Error("The bound value is not valid for use with the ajaxStatus binding.");

	            ko.applyBindingsToNode(element, {
	                css: {
	                    working: value.ajaxWorking,
	                    success: value.ajaxSuccess,
	                    fail: value.ajaxFailure,
	                    ajaxStatus: true,
	                }
	            });
	        }
	    };


	    // Dragons be here...
	    // allows up/down arrows, mouse-click dragging,
	    // and mouse-click wheel
	    // accepts property 'negative'in allBindings to allow negative numbers
	    ko.bindingHandlers.numValue = {
	        init: function (element, valueAccessor, allBindings) {
	            console.warn("numValue binding handler is being used! This should be replaced with the numericObservable.");
	            var num = valueAccessor();
	            var bindings = ko.utils.unwrapObservable(allBindings());
	            var isChar = function (key) { return key >= 65 && key <= 90; };
	            var up = 38, down = 40;
	            $(element).keydown(function (evt) {
	                var key = evt.keyCode;
	                var iVal = parseInt(element.value);
	                if (key === up || key === down) {
	                    if (key === up) iVal++;
	                    else if (key == down && (bindings.negative || iVal > 0)) iVal--;
	                }
	                else if (isChar(key) && !evt.ctrlKey) evt.preventDefault();
	                if (!isNaN(iVal) && iVal != null) num(iVal);
	                else num(null);

	                return true;
	            });
	            var isDown = false;
	            var lastY = 0;
	            var buffer = 10;
	            $(element).mousedown(function (e) { isDown = true; return true; });
	            $(document).mouseup(function (e) { isDown = false; return true; });
	            $(document).mousemove(function (e) {
	                if (isDown) {
	                    var y = e.pageY;
	                    if (!lastY) lastY = y;
	                    if (y > lastY + buffer && (bindings.negative || num() > 0)) {
	                        num(num() - 1);
	                        lastY = y;
	                    }
	                    else if (y + buffer < lastY) {
	                        num(num() + 1);
	                        lastY = y;
	                    }
	                }
	            });
	            $(document).on("mousewheel", function (evt) {
	                if (isDown) {
	                    var delta = evt.originalEvent.wheelDelta;
	                    if (delta > 0) {
	                        num(num() + 1);
	                    }
	                    else if (delta < 0 && (bindings.negative || num() > 0)) {
	                        num(num() - 1);
	                    }
	                }
	            });

	            // show validations as well
	            return ko.bindingHandlers['validationCore'].init(element, valueAccessor, allBindings);
	        },
	        update: function (element, valueAccessor, allBindings) {
	            var val = ko.utils.unwrapObservable(valueAccessor());
	            if (!isNaN(val)) element.value = val;
	        }
	    };

	    ko.bindingHandlers.resizable = {
	        init: function (element, valueAccessor) {
	            var alsoResizeSelector = ko.unwrap(valueAccessor());
	            if (typeof alsoResizeSelector != "string") alsoResizeSelector = '';
	            $(element).resizable({
	                alsoResize: alsoResizeSelector,
	                minWidth: 300,
	                minHeight: 100
	            });
	        }
	    };

	    ko.bindingHandlers.accordion = {
	        init: function (element, valueAccessor) {
	        },
	        update: function (element, valueAccessor) {
	            var opts = ko.utils.unwrapObservable(valueAccessor());
	            $(element).accordion(opts);
	        }
	    };

	    ko.bindingHandlers.slimscroll = {
	        init: function (element) {
	            var $el = $(element);
	            $el.slimscroll({
	                //alwaysVisible: true,
	                railColor: '#222',
	                height: "100%"
	                //railVisible: true
	            });
	        }
	    };

	    // Focuses next .form-control when Enter is pressed
	    ko.bindingHandlers.tabOnEnter = {
	        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
	            var allBindings = allBindingsAccessor();
	            $(element).keypress(function (event) {
	                var keyCode = (event.which ? event.which : event.keyCode);
	                if (keyCode === 13) {
	                    var index = $('.form-control').index(event.target) + 1;
	                    var $next = $('.form-control').eq(index);

	                    $next.focus();
	                    $next.select();
	                    return false;
	                }
	                return true;
	            });
	        }
	    };

	    /** Trigger valueAccessor on Enter keypress
	      * @param {function} valueAccessor - Function to call
	      */
	    ko.bindingHandlers.onEnter = {
	      init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
	        var allBindings = allBindingsAccessor();
	        var value = valueAccessor();
	        $(element).keypress(function (event) {
	          var keyCode = (event.which ? event.which : event.keyCode);
	          if (keyCode === 13) {
	            value.call(viewModel);
	            return false;
	          }
	          return true;
	        });
	      }
	    };
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 33:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(jQuery) {// @preserve jQuery.floatThead 1.4.5 - http://mkoryak.github.io/floatThead/ - Copyright (c) 2012 - 2016 Misha Koryak
	// @license MIT

	/* @author Misha Koryak
	 * @projectDescription lock a table header in place while scrolling - without breaking styles or events bound to the header
	 *
	 * Dependencies:
	 * jquery 1.9.0 + [required] OR jquery 1.7.0 + jquery UI core
	 *
	 * http://mkoryak.github.io/floatThead/
	 *
	 * Tested on FF13+, Chrome 21+, IE8, IE9, IE10, IE11
	 *
	 */
	(function( $ ) {
	  /**
	   * provides a default config object. You can modify this after including this script if you want to change the init defaults
	   * @type {Object}
	   */
	  $.floatThead = $.floatThead || {};
	  $.floatThead.defaults = {
	    headerCellSelector: 'tr:visible:first>*:visible', //thead cells are this.
	    zIndex: 1001, //zindex of the floating thead (actually a container div)
	    position: 'auto', // 'fixed', 'absolute', 'auto'. auto picks the best for your table scrolling type.
	    top: 0, //String or function($table) - offset from top of window where the header should not pass above
	    bottom: 0, //String or function($table) - offset from the bottom of the table where the header should stop scrolling
	    scrollContainer: function($table) { // or boolean 'true' (use offsetParent) | function -> if the table has horizontal scroll bars then this is the container that has overflow:auto and causes those scroll bars
	      return $([]);
	    },
	    responsiveContainer: function($table) { // only valid if scrollContainer is not used (ie window scrolling). this is the container which will control y scrolling at some mobile breakpoints
	      return $([]);
	    },
	    getSizingRow: function($table, $cols, $fthCells){ // this is only called when using IE,
	      // override it if the first row of the table is going to contain colgroups (any cell spans greater than one col)
	      // it should return a jquery object containing a wrapped set of table cells comprising a row that contains no col spans and is visible
	      return $table.find('tbody tr:visible:first>*:visible');
	    },
	    floatTableClass: 'floatThead-table',
	    floatWrapperClass: 'floatThead-wrapper',
	    floatContainerClass: 'floatThead-container',
	    copyTableClass: true, //copy 'class' attribute from table into the floated table so that the styles match.
	    autoReflow: false, //(undocumented) - use MutationObserver api to reflow automatically when internal table DOM changes
	    debug: false, //print possible issues (that don't prevent script loading) to console, if console exists.
	    support: { //should we bind events that expect these frameworks to be present and/or check for them?
	      bootstrap: true,
	      datatables: true,
	      jqueryUI: true,
	      perfectScrollbar: true
	    }
	  };

	  var util = window._;

	  var canObserveMutations = typeof MutationObserver !== 'undefined';


	  //browser stuff
	  var ieVersion = function(){for(var a=3,b=document.createElement("b"),c=b.all||[];a = 1+a,b.innerHTML="<!--[if gt IE "+ a +"]><i><![endif]-->",c[0];);return 4<a?a:document.documentMode}();
	  var isFF = /Gecko\//.test(navigator.userAgent);
	  var isWebkit = /WebKit\//.test(navigator.userAgent);

	  if(!(ieVersion || isFF || isWebkit)){
	    ieVersion = 11; //yey a hack!
	  }

	  //safari 7 (and perhaps others) reports table width to be parent container's width if max-width is set on table. see: https://github.com/mkoryak/floatThead/issues/108
	  var isTableWidthBug = function(){
	    if(isWebkit) {
	      var $test = $('<div>').css('width', 0).append(
	        $('<table>').css('max-width', '100%').append(
	          $('<tr>').append(
	            $('<th>').append(
	              $('<div>').css('min-width', 100).text('X')
	            )
	          )
	        )
	      );
	      $("body").append($test);
	      var ret = ($test.find("table").width() == 0);
	      $test.remove();
	      return ret;
	    }
	    return false;
	  };

	  var createElements = !isFF && !ieVersion; //FF can read width from <col> elements, but webkit cannot

	  var $window = $(window);

	  if(!window.matchMedia) {
	    var _beforePrint = window.onbeforeprint;
	    var _afterPrint = window.onafterprint;
	    window.onbeforeprint = function () {
	      _beforePrint && _beforePrint();
	      $window.triggerHandler("beforeprint");
	    };
	    window.onafterprint = function () {
	      _afterPrint && _afterPrint();
	      $window.triggerHandler("afterprint");
	    };
	  }

	  /**
	   * @param debounceMs
	   * @param cb
	   */
	  function windowResize(eventName, cb){
	    if(ieVersion == 8){ //ie8 is crap: https://github.com/mkoryak/floatThead/issues/65
	      var winWidth = $window.width();
	      var debouncedCb = util.debounce(function(){
	        var winWidthNew = $window.width();
	        if(winWidth != winWidthNew){
	          winWidth = winWidthNew;
	          cb();
	        }
	      }, 1);
	      $window.on(eventName, debouncedCb);
	    } else {
	      $window.on(eventName, util.debounce(cb, 1));
	    }
	  }

	  function getClosestScrollContainer($elem) {
	    var elem = $elem[0];
	    var parent = elem.parentElement;

	    do {
	      var pos = window
	          .getComputedStyle(parent)
	          .getPropertyValue('overflow');

	      if (pos != 'visible') break;

	    } while (parent = parent.parentElement);

	    if(parent == document.body){
	      return $([]);
	    }
	    return $(parent);
	  }


	  function debug(str){
	    window && window.console && window.console.error && window.console.error("jQuery.floatThead: " + str);
	  }

	  //returns fractional pixel widths
	  function getOffsetWidth(el) {
	    var rect = el.getBoundingClientRect();
	    return rect.width || rect.right - rect.left;
	  }

	  /**
	   * try to calculate the scrollbar width for your browser/os
	   * @return {Number}
	   */
	  function scrollbarWidth() {
	    var $div = $('<div>').css({ //borrowed from anti-scroll
	      'width': 50,
	      'height': 50,
	      'overflow-y': 'scroll',
	      'position': 'absolute',
	      'top': -200,
	      'left': -200
	    }).append(
	      $('<div>').css({
	        'height': 100,
	        'width': '100%'
	      })
	    );
	    $('body').append($div);
	    var w1 = $div.innerWidth();
	    var w2 = $('div', $div).innerWidth();
	    $div.remove();
	    return w1 - w2;
	  }
	  /**
	   * Check if a given table has been datatableized (http://datatables.net)
	   * @param $table
	   * @return {Boolean}
	   */
	  function isDatatable($table){
	    if($table.dataTableSettings){
	      for(var i = 0; i < $table.dataTableSettings.length; i++){
	        var table = $table.dataTableSettings[i].nTable;
	        if($table[0] == table){
	          return true;
	        }
	      }
	    }
	    return false;
	  }

	  function tableWidth($table, $fthCells, isOuter){
	    // see: https://github.com/mkoryak/floatThead/issues/108
	    var fn = isOuter ? "outerWidth": "width";
	    if(isTableWidthBug && $table.css("max-width")){
	      var w = 0;
	      if(isOuter) {
	        w += parseInt($table.css("borderLeft"), 10);
	        w += parseInt($table.css("borderRight"), 10);
	      }
	      for(var i=0; i < $fthCells.length; i++){
	        w += $fthCells.get(i).offsetWidth;
	      }
	      return w;
	    } else {
	      return $table[fn]();
	    }
	  }
	  $.fn.floatThead = function(map){
	    map = map || {};
	    if(!util){ //may have been included after the script? lets try to grab it again.
	      util = window._ || $.floatThead._;
	      if(!util){
	        throw new Error("jquery.floatThead-slim.js requires underscore. You should use the non-lite version since you do not have underscore.");
	      }
	    }

	    if(ieVersion < 8){
	      return this; //no more crappy browser support.
	    }

	    var mObs = null; //mutation observer lives in here if we can use it / make it

	    if(util.isFunction(isTableWidthBug)) {
	      isTableWidthBug = isTableWidthBug();
	    }

	    if(util.isString(map)){
	      var command = map;
	      var args = Array.prototype.slice.call(arguments, 1);
	      var ret = this;
	      this.filter('table').each(function(){
	        var $this = $(this);
	        var opts = $this.data('floatThead-lazy');
	        if(opts){
	          $this.floatThead(opts);
	        }
	        var obj = $this.data('floatThead-attached');
	        if(obj && util.isFunction(obj[command])){
	          var r = obj[command].apply(this, args);
	          if(r !== undefined){
	            ret = r;
	          }
	        }
	      });
	      return ret;
	    }
	    var opts = $.extend({}, $.floatThead.defaults || {}, map);

	    $.each(map, function(key, val){
	      if((!(key in $.floatThead.defaults)) && opts.debug){
	        debug("Used ["+key+"] key to init plugin, but that param is not an option for the plugin. Valid options are: "+ (util.keys($.floatThead.defaults)).join(', '));
	      }
	    });
	    if(opts.debug){
	      var v = $.fn.jquery.split(".");
	      if(parseInt(v[0], 10) == 1 && parseInt(v[1], 10) <= 7){
	        debug("jQuery version "+$.fn.jquery+" detected! This plugin supports 1.8 or better, or 1.7.x with jQuery UI 1.8.24 -> http://jqueryui.com/resources/download/jquery-ui-1.8.24.zip")
	      }
	    }

	    this.filter(':not(.'+opts.floatTableClass+')').each(function(){
	      var floatTheadId = util.uniqueId();
	      var $table = $(this);
	      if($table.data('floatThead-attached')){
	        return true; //continue the each loop
	      }
	      if(!$table.is('table')){
	        throw new Error('jQuery.floatThead must be run on a table element. ex: $("table").floatThead();');
	      }
	      canObserveMutations = opts.autoReflow && canObserveMutations; //option defaults to false!
	      var $header = $table.children('thead:first');
	      var $tbody = $table.children('tbody:first');
	      if($header.length == 0 || $tbody.length == 0){
	        $table.data('floatThead-lazy', opts);
	        $table.unbind("reflow").one('reflow', function(){
	          $table.floatThead(opts);
	        });
	        return;
	      }
	      if($table.data('floatThead-lazy')){
	        $table.unbind("reflow");
	      }
	      $table.data('floatThead-lazy', false);

	      var headerFloated = true;
	      var scrollingTop, scrollingBottom;
	      var scrollbarOffset = {vertical: 0, horizontal: 0};
	      var scWidth = scrollbarWidth();
	      var lastColumnCount = 0; //used by columnNum()

	      if(opts.scrollContainer === true){
	        opts.scrollContainer = getClosestScrollContainer;
	      }

	      var $scrollContainer = opts.scrollContainer($table) || $([]); //guard against returned nulls
	      var locked = $scrollContainer.length > 0;
	      var $responsiveContainer = locked ? $([]) : opts.responsiveContainer($table) || $([]);
	      var responsive = isResponsiveContainerActive();

	      var useAbsolutePositioning = null;
	      if(typeof opts.useAbsolutePositioning !== 'undefined'){
	        opts.position = 'auto';
	        if(opts.useAbsolutePositioning){
	          opts.position = opts.useAbsolutePositioning ? 'absolute' : 'fixed';
	        }
	        debug("option 'useAbsolutePositioning' has been removed in v1.3.0, use `position:'"+opts.position+"'` instead. See docs for more info: http://mkoryak.github.io/floatThead/#options")
	      }
	      if(typeof opts.scrollingTop !== 'undefined'){
	        opts.top = opts.scrollingTop;
	        debug("option 'scrollingTop' has been renamed to 'top' in v1.3.0. See docs for more info: http://mkoryak.github.io/floatThead/#options");
	      }
	      if(typeof opts.scrollingBottom !== 'undefined'){
	        opts.bottom = opts.scrollingBottom;
	        debug("option 'scrollingBottom' has been renamed to 'bottom' in v1.3.0. See docs for more info: http://mkoryak.github.io/floatThead/#options");
	      }


	      if (opts.position == 'auto') {
	        useAbsolutePositioning = null;
	      } else if (opts.position == 'fixed') {
	        useAbsolutePositioning = false;
	      } else if (opts.position == 'absolute'){
	        useAbsolutePositioning = true;
	      } else if (opts.debug) {
	        debug('Invalid value given to "position" option, valid is "fixed", "absolute" and "auto". You passed: ', opts.position);
	      }

	      if(useAbsolutePositioning == null){ //defaults: locked=true, !locked=false
	        useAbsolutePositioning = locked;
	      }
	      var $caption = $table.find("caption");
	      var haveCaption = $caption.length == 1;
	      if(haveCaption){
	        var captionAlignTop = ($caption.css("caption-side") || $caption.attr("align") || "top") === "top";
	      }

	      var $fthGrp = $('<fthfoot>').css({
	        'display': 'table-footer-group',
	        'border-spacing': 0,
	        'height': 0,
	        'border-collapse': 'collapse',
	        'visibility': 'hidden'
	      });

	      var wrappedContainer = false; //used with absolute positioning enabled. did we need to wrap the scrollContainer/table with a relative div?
	      var $wrapper = $([]); //used when absolute positioning enabled - wraps the table and the float container
	      var absoluteToFixedOnScroll = ieVersion <= 9 && !locked && useAbsolutePositioning; //on IE using absolute positioning doesn't look good with window scrolling, so we change position to fixed on scroll, and then change it back to absolute when done.
	      var $floatTable = $("<table/>");
	      var $floatColGroup = $("<colgroup/>");
	      var $tableColGroup = $table.children('colgroup:first');
	      var existingColGroup = true;
	      if($tableColGroup.length == 0){
	        $tableColGroup = $("<colgroup/>");
	        existingColGroup = false;
	      }
	      var $fthRow = $('<fthtr>').css({ //created unstyled elements (used for sizing the table because chrome can't read <col> width)
	        'display': 'table-row',
	        'border-spacing': 0,
	        'height': 0,
	        'border-collapse': 'collapse'
	      });
	      var $floatContainer = $('<div>').css('overflow', 'hidden').attr('aria-hidden', 'true');
	      var floatTableHidden = false; //this happens when the table is hidden and we do magic when making it visible
	      var $newHeader = $("<thead/>");
	      var $sizerRow = $('<tr class="size-row" aria-hidden="true"/>');
	      var $sizerCells = $([]);
	      var $tableCells = $([]); //used for sizing - either $sizerCells or $tableColGroup cols. $tableColGroup cols are only created in chrome for borderCollapse:collapse because of a chrome bug.
	      var $headerCells = $([]);
	      var $fthCells = $([]); //created elements

	      $newHeader.append($sizerRow);
	      $table.prepend($tableColGroup);
	      if(createElements){
	        $fthGrp.append($fthRow);
	        $table.append($fthGrp);
	      }

	      $floatTable.append($floatColGroup);
	      $floatContainer.append($floatTable);
	      if(opts.copyTableClass){
	        $floatTable.attr('class', $table.attr('class'));
	      }
	      $floatTable.attr({ //copy over some deprecated table attributes that people still like to use. Good thing people don't use colgroups...
	                         'cellpadding': $table.attr('cellpadding'),
	                         'cellspacing': $table.attr('cellspacing'),
	                         'border': $table.attr('border')
	                       });
	      var tableDisplayCss = $table.css('display');
	      $floatTable.css({
	                        'borderCollapse': $table.css('borderCollapse'),
	                        'border': $table.css('border'),
	                        'display': tableDisplayCss
	                      });
	      if(!locked){
	        $floatTable.css('width', 'auto');
	      }
	      if(tableDisplayCss == 'none'){
	        floatTableHidden = true;
	      }

	      $floatTable.addClass(opts.floatTableClass).css({'margin': 0, 'border-bottom-width': 0}); //must have no margins or you won't be able to click on things under floating table

	      if(useAbsolutePositioning){
	        var makeRelative = function($container, alwaysWrap){
	          var positionCss = $container.css('position');
	          var relativeToScrollContainer = (positionCss == "relative" || positionCss == "absolute");
	          var $containerWrap = $container;
	          if(!relativeToScrollContainer || alwaysWrap){
	            var css = {"paddingLeft": $container.css('paddingLeft'), "paddingRight": $container.css('paddingRight')};
	            $floatContainer.css(css);
	            $containerWrap = $container.data('floatThead-containerWrap') || $container.wrap(
	              $('<div>').addClass(opts.floatWrapperClass).css({
	                'position': 'relative',
	                'clear': 'both'
	              })
	            ).parent();
	            $container.data('floatThead-containerWrap', $containerWrap); //multiple tables inside one scrolling container - #242
	            wrappedContainer = true;
	          }
	          return $containerWrap;
	        };
	        if(locked){
	          $wrapper = makeRelative($scrollContainer, true);
	          $wrapper.prepend($floatContainer);
	        } else {
	          $wrapper = makeRelative($table);
	          $table.before($floatContainer);
	        }
	      } else {
	        $table.before($floatContainer);
	      }


	      $floatContainer.css({
	                            position: useAbsolutePositioning ? 'absolute' : 'fixed',
	                            marginTop: 0,
	                            top:  useAbsolutePositioning ? 0 : 'auto',
	                            zIndex: opts.zIndex,
	                            willChange: 'transform'
	                          });
	      $floatContainer.addClass(opts.floatContainerClass);
	      updateScrollingOffsets();

	      var layoutFixed = {'table-layout': 'fixed'};
	      var layoutAuto = {'table-layout': $table.css('tableLayout') || 'auto'};
	      var originalTableWidth = $table[0].style.width || ""; //setting this to auto is bad: #70
	      var originalTableMinWidth = $table.css('minWidth') || "";

	      function eventName(name){
	        return name+'.fth-'+floatTheadId+'.floatTHead'
	      }

	      function setHeaderHeight(){
	        var headerHeight = 0;
	        $header.children("tr:visible").each(function(){
	          headerHeight += $(this).outerHeight(true);
	        });
	        if($table.css('border-collapse') == 'collapse') {
	          var tableBorderTopHeight = parseInt($table.css('border-top-width'), 10);
	          var cellBorderTopHeight = parseInt($table.find("thead tr:first").find(">*:first").css('border-top-width'), 10);
	          if(tableBorderTopHeight > cellBorderTopHeight) {
	            headerHeight -= (tableBorderTopHeight / 2); //id love to see some docs where this magic recipe is found..
	          }
	        }
	        $sizerRow.outerHeight(headerHeight);
	        $sizerCells.outerHeight(headerHeight);
	      }


	      function setFloatWidth(){
	        var tw = tableWidth($table, $fthCells, true);
	        var $container = responsive ? $responsiveContainer : $scrollContainer;
	        var width = $container.width() || tw;
	        var floatContainerWidth = $container.css("overflow-y") != 'hidden' ? width - scrollbarOffset.vertical : width;
	        $floatContainer.width(floatContainerWidth);
	        if(locked){
	          var percent = 100 * tw / (floatContainerWidth);
	          $floatTable.css('width', percent+'%');
	        } else {
	          $floatTable.outerWidth(tw);
	        }
	      }

	      function updateScrollingOffsets(){
	        scrollingTop = (util.isFunction(opts.top) ? opts.top($table) : opts.top) || 0;
	        scrollingBottom = (util.isFunction(opts.bottom) ? opts.bottom($table) : opts.bottom) || 0;
	      }

	      /**
	       * get the number of columns and also rebuild resizer rows if the count is different than the last count
	       */
	      function columnNum(){
	        var count;
	        var $headerColumns = $header.find(opts.headerCellSelector);
	        if(existingColGroup){
	          count = $tableColGroup.find('col').length;
	        } else {
	          count = 0;
	          $headerColumns.each(function () {
	            count += parseInt(($(this).attr('colspan') || 1), 10);
	          });
	        }
	        if(count != lastColumnCount){
	          lastColumnCount = count;
	          var cells = [], cols = [], psuedo = [], content;
	          for(var x = 0; x < count; x++){
	            content = $headerColumns.eq(x).text();
	            cells.push('<th class="floatThead-col" aria-label="'+content+'"/>');
	            cols.push('<col/>');
	            psuedo.push(
	              $('<fthtd>').css({
	                'display': 'table-cell',
	                'height': 0,
	                'width': 'auto'
	              })
	            );
	          }

	          cols = cols.join('');
	          cells = cells.join('');

	          if(createElements){
	            $fthRow.empty();
	            $fthRow.append(psuedo);
	            $fthCells = $fthRow.find('fthtd');
	          }

	          $sizerRow.html(cells);
	          $sizerCells = $sizerRow.find("th");
	          if(!existingColGroup){
	            $tableColGroup.html(cols);
	          }
	          $tableCells = $tableColGroup.find('col');
	          $floatColGroup.html(cols);
	          $headerCells = $floatColGroup.find("col");

	        }
	        return count;
	      }

	      function refloat(){ //make the thing float
	        if(!headerFloated){
	          headerFloated = true;
	          if(useAbsolutePositioning){ //#53, #56
	            var tw = tableWidth($table, $fthCells, true);
	            var wrapperWidth = $wrapper.width();
	            if(tw > wrapperWidth){
	              $table.css('minWidth', tw);
	            }
	          }
	          $table.css(layoutFixed);
	          $floatTable.css(layoutFixed);
	          $floatTable.append($header); //append because colgroup must go first in chrome
	          $tbody.before($newHeader);
	          setHeaderHeight();
	        }
	      }
	      function unfloat(){ //put the header back into the table
	        if(headerFloated){
	          headerFloated = false;
	          if(useAbsolutePositioning){ //#53, #56
	            $table.width(originalTableWidth);
	          }
	          $newHeader.detach();
	          $table.prepend($header);
	          $table.css(layoutAuto);
	          $floatTable.css(layoutAuto);
	          $table.css('minWidth', originalTableMinWidth); //this looks weird, but it's not a bug. Think about it!!
	          $table.css('minWidth', tableWidth($table, $fthCells)); //#121
	        }
	      }
	      var isHeaderFloatingLogical = false; //for the purpose of this event, the header is/isnt floating, even though the element
	                                           //might be in some other state. this is what the header looks like to the user
	      function triggerFloatEvent(isFloating){
	        if(isHeaderFloatingLogical != isFloating){
	          isHeaderFloatingLogical = isFloating;
	          $table.triggerHandler("floatThead", [isFloating, $floatContainer])
	        }
	      }
	      function changePositioning(isAbsolute){
	        if(useAbsolutePositioning != isAbsolute){
	          useAbsolutePositioning = isAbsolute;
	          $floatContainer.css({
	                                position: useAbsolutePositioning ? 'absolute' : 'fixed'
	                              });
	        }
	      }
	      function getSizingRow($table, $cols, $fthCells, ieVersion){
	        if(createElements){
	          return $fthCells;
	        } else if(ieVersion) {
	          return opts.getSizingRow($table, $cols, $fthCells);
	        } else {
	          return $cols;
	        }
	      }

	      /**
	       * returns a function that updates the floating header's cell widths.
	       * @return {Function}
	       */
	      function reflow(){
	        var i;
	        var numCols = columnNum(); //if the tables columns changed dynamically since last time (datatables), rebuild the sizer rows and get a new count

	        return function(){
	          //Cache the current scrollLeft value so that it can be reset post reflow
	          var scrollLeft = $floatContainer.scrollLeft();
	          $tableCells = $tableColGroup.find('col');
	          var $rowCells = getSizingRow($table, $tableCells, $fthCells, ieVersion);

	          if($rowCells.length == numCols && numCols > 0){
	            if(!existingColGroup){
	              for(i=0; i < numCols; i++){
	                $tableCells.eq(i).css('width', '');
	              }
	            }
	            unfloat();
	            var widths = [];
	            for(i=0; i < numCols; i++){
	              widths[i] = getOffsetWidth($rowCells.get(i));
	            }
	            for(i=0; i < numCols; i++){
	              $headerCells.eq(i).width(widths[i]);
	              $tableCells.eq(i).width(widths[i]);
	            }
	            refloat();
	          } else {
	            $floatTable.append($header);
	            $table.css(layoutAuto);
	            $floatTable.css(layoutAuto);
	            setHeaderHeight();
	          }
	          //Set back the current scrollLeft value on floatContainer
	          $floatContainer.scrollLeft(scrollLeft);
	          $table.triggerHandler("reflowed", [$floatContainer]);
	        };
	      }

	      function floatContainerBorderWidth(side){
	        var border = $scrollContainer.css("border-"+side+"-width");
	        var w = 0;
	        if (border && ~border.indexOf('px')) {
	          w = parseInt(border, 10);
	        }
	        return w;
	      }

	      function isResponsiveContainerActive(){
	        return $responsiveContainer.css("overflow-x") == 'auto';
	      }
	      /**
	       * first performs initial calculations that we expect to not change when the table, window, or scrolling container are scrolled.
	       * returns a function that calculates the floating container's top and left coords. takes into account if we are using page scrolling or inner scrolling
	       * @return {Function}
	       */
	      function calculateFloatContainerPosFn(){
	        var scrollingContainerTop = $scrollContainer.scrollTop();

	        //this floatEnd calc was moved out of the returned function because we assume the table height doesn't change (otherwise we must reinit by calling calculateFloatContainerPosFn)
	        var floatEnd;
	        var tableContainerGap = 0;
	        var captionHeight = haveCaption ? $caption.outerHeight(true) : 0;
	        var captionScrollOffset = captionAlignTop ? captionHeight : -captionHeight;

	        var floatContainerHeight = $floatContainer.height();
	        var tableOffset = $table.offset();
	        var tableLeftGap = 0; //can be caused by border on container (only in locked mode)
	        var tableTopGap = 0;
	        if(locked){
	          var containerOffset = $scrollContainer.offset();
	          tableContainerGap = tableOffset.top - containerOffset.top + scrollingContainerTop;
	          if(haveCaption && captionAlignTop){
	            tableContainerGap += captionHeight;
	          }
	          tableLeftGap = floatContainerBorderWidth('left');
	          tableTopGap = floatContainerBorderWidth('top');
	          tableContainerGap -= tableTopGap;
	        } else {
	          floatEnd = tableOffset.top - scrollingTop - floatContainerHeight + scrollingBottom + scrollbarOffset.horizontal;
	        }
	        var windowTop = $window.scrollTop();
	        var windowLeft = $window.scrollLeft();
	        var scrollContainerLeft = (
	            isResponsiveContainerActive() ?  $responsiveContainer :
	            (locked ? $scrollContainer : $window)
	        ).scrollLeft();

	        return function(eventType){
	          responsive = isResponsiveContainerActive();

	          var isTableHidden = $table[0].offsetWidth <= 0 && $table[0].offsetHeight <= 0;
	          if(!isTableHidden && floatTableHidden) {
	            floatTableHidden = false;
	            setTimeout(function(){
	              $table.triggerHandler("reflow");
	            }, 1);
	            return null;
	          }
	          if(isTableHidden){ //it's hidden
	            floatTableHidden = true;
	            if(!useAbsolutePositioning){
	              return null;
	            }
	          }

	          if(eventType == 'windowScroll'){
	            windowTop = $window.scrollTop();
	            windowLeft = $window.scrollLeft();
	          } else if(eventType == 'containerScroll'){
	            if($responsiveContainer.length){
	              if(!responsive){
	                return; //we dont care about the event if we arent responsive right now
	              }
	              scrollContainerLeft = $responsiveContainer.scrollLeft();
	            } else {
	              scrollingContainerTop = $scrollContainer.scrollTop();
	              scrollContainerLeft = $scrollContainer.scrollLeft();
	            }
	          } else if(eventType != 'init') {
	            windowTop = $window.scrollTop();
	            windowLeft = $window.scrollLeft();
	            scrollingContainerTop = $scrollContainer.scrollTop();
	            scrollContainerLeft =  (responsive ? $responsiveContainer : $scrollContainer).scrollLeft();
	          }
	          if(isWebkit && (windowTop < 0 || windowLeft < 0)){ //chrome overscroll effect at the top of the page - breaks fixed positioned floated headers
	            return;
	          }

	          if(absoluteToFixedOnScroll){
	            if(eventType == 'windowScrollDone'){
	              changePositioning(true); //change to absolute
	            } else {
	              changePositioning(false); //change to fixed
	            }
	          } else if(eventType == 'windowScrollDone'){
	            return null; //event is fired when they stop scrolling. ignore it if not 'absoluteToFixedOnScroll'
	          }

	          tableOffset = $table.offset();
	          if(haveCaption && captionAlignTop){
	            tableOffset.top += captionHeight;
	          }
	          var top, left;
	          var tableHeight = $table.outerHeight();

	          if(locked && useAbsolutePositioning){ //inner scrolling, absolute positioning
	            if (tableContainerGap >= scrollingContainerTop) {
	              var gap = tableContainerGap - scrollingContainerTop + tableTopGap;
	              top = gap > 0 ? gap : 0;
	              triggerFloatEvent(false);
	            } else {
	              top = wrappedContainer ? tableTopGap : scrollingContainerTop;
	              //headers stop at the top of the viewport
	              triggerFloatEvent(true);
	            }
	            left = tableLeftGap;
	          } else if(!locked && useAbsolutePositioning) { //window scrolling, absolute positioning
	            if(windowTop > floatEnd + tableHeight + captionScrollOffset){
	              top = tableHeight - floatContainerHeight + captionScrollOffset; //scrolled past table
	            } else if (tableOffset.top >= windowTop + scrollingTop) {
	              top = 0; //scrolling to table
	              unfloat();
	              triggerFloatEvent(false);
	            } else {
	              top = scrollingTop + windowTop - tableOffset.top + tableContainerGap + (captionAlignTop ? captionHeight : 0);
	              refloat(); //scrolling within table. header floated
	              triggerFloatEvent(true);
	            }
	            left =  scrollContainerLeft;
	          } else if(locked && !useAbsolutePositioning){ //inner scrolling, fixed positioning
	            if (tableContainerGap > scrollingContainerTop || scrollingContainerTop - tableContainerGap > tableHeight) {
	              top = tableOffset.top - windowTop;
	              unfloat();
	              triggerFloatEvent(false);
	            } else {
	              top = tableOffset.top + scrollingContainerTop  - windowTop - tableContainerGap;
	              refloat();
	              triggerFloatEvent(true);
	              //headers stop at the top of the viewport
	            }
	            left = tableOffset.left + scrollContainerLeft - windowLeft;
	          } else if(!locked && !useAbsolutePositioning) { //window scrolling, fixed positioning
	            if(windowTop > floatEnd + tableHeight + captionScrollOffset){
	              top = tableHeight + scrollingTop - windowTop + floatEnd + captionScrollOffset;
	              //scrolled past the bottom of the table
	            } else if (tableOffset.top > windowTop + scrollingTop) {
	              top = tableOffset.top - windowTop;
	              refloat();
	              triggerFloatEvent(false); //this is a weird case, the header never gets unfloated and i have no no way to know
	              //scrolled past the top of the table
	            } else {
	              //scrolling within the table
	              top = scrollingTop;
	              triggerFloatEvent(true);
	            }
	            left = tableOffset.left + scrollContainerLeft - windowLeft;
	          }
	          return {top: Math.round(top), left: Math.round(left)};
	        };
	      }
	      /**
	       * returns a function that caches old floating container position and only updates css when the position changes
	       * @return {Function}
	       */
	      function repositionFloatContainerFn(){
	        var oldTop = null;
	        var oldLeft = null;
	        var oldScrollLeft = null;
	        return function(pos, setWidth, setHeight){
	          if(pos != null && (oldTop != pos.top || oldLeft != pos.left)){
	            if(ieVersion === 8){
	              $floatContainer.css({
	                top: pos.top,
	                left: pos.left
	              });
	            } else {
	              var transform = 'translateX(' + pos.left + 'px) translateY(' + pos.top + 'px)';
	              $floatContainer.css({
	                '-webkit-transform' : transform,
	                '-moz-transform'    : transform,
	                '-ms-transform'     : transform,
	                '-o-transform'      : transform,
	                'transform'         : transform,
	                'top': 0,
	                'left': 0
	              });
	            }
	            oldTop = pos.top;
	            oldLeft = pos.left;
	          }
	          if(setWidth){
	            setFloatWidth();
	          }
	          if(setHeight){
	            setHeaderHeight();
	          }
	          var scrollLeft = (responsive ? $responsiveContainer : $scrollContainer).scrollLeft();
	          if(!useAbsolutePositioning || oldScrollLeft != scrollLeft){
	            $floatContainer.scrollLeft(scrollLeft);
	            oldScrollLeft = scrollLeft;
	          }
	        }
	      }

	      /**
	       * checks if THIS table has scrollbars, and finds their widths
	       */
	      function calculateScrollBarSize(){ //this should happen after the floating table has been positioned
	        if($scrollContainer.length){
	          if(opts.support && opts.support.perfectScrollbar && $scrollContainer.data().perfectScrollbar){
	            scrollbarOffset = {horizontal:0, vertical:0};
	          } else {
	            if($scrollContainer.css('overflow-x') == 'scroll'){
	              scrollbarOffset.horizontal = scWidth;
	            } else {
	              var sw = $scrollContainer.width(), tw = tableWidth($table, $fthCells);
	              var offsetv = sh < th ? scWidth : 0;
	              scrollbarOffset.horizontal = sw - offsetv < tw ? scWidth : 0;
	            }
	            if($scrollContainer.css('overflow-y') == 'scroll'){
	              scrollbarOffset.vertical = scWidth;
	            } else {
	              var sh = $scrollContainer.height(), th = $table.height();
	              var offseth = sw < tw ? scWidth : 0;
	              scrollbarOffset.vertical = sh - offseth < th ? scWidth : 0;
	            }
	          }
	        }
	      }
	      //finish up. create all calculation functions and bind them to events
	      calculateScrollBarSize();

	      var flow;

	      var ensureReflow = function(){
	        flow = reflow();
	        flow();
	      };

	      ensureReflow();

	      var calculateFloatContainerPos = calculateFloatContainerPosFn();
	      var repositionFloatContainer = repositionFloatContainerFn();

	      repositionFloatContainer(calculateFloatContainerPos('init'), true); //this must come after reflow because reflow changes scrollLeft back to 0 when it rips out the thead

	      var windowScrollDoneEvent = util.debounce(function(){
	        repositionFloatContainer(calculateFloatContainerPos('windowScrollDone'), false);
	      }, 1);

	      var windowScrollEvent = function(){
	        repositionFloatContainer(calculateFloatContainerPos('windowScroll'), false);
	        if(absoluteToFixedOnScroll){
	          windowScrollDoneEvent();
	        }
	      };
	      var containerScrollEvent = function(){
	        repositionFloatContainer(calculateFloatContainerPos('containerScroll'), false);
	      };


	      var windowResizeEvent = function(){
	        if($table.is(":hidden")){
	          return;
	        }
	        updateScrollingOffsets();
	        calculateScrollBarSize();
	        ensureReflow();
	        calculateFloatContainerPos = calculateFloatContainerPosFn();
	        repositionFloatContainer = repositionFloatContainerFn();
	        repositionFloatContainer(calculateFloatContainerPos('resize'), true, true);
	      };
	      var reflowEvent = util.debounce(function(){
	        if($table.is(":hidden")){
	          return;
	        }
	        calculateScrollBarSize();
	        updateScrollingOffsets();
	        ensureReflow();
	        calculateFloatContainerPos = calculateFloatContainerPosFn();
	        repositionFloatContainer(calculateFloatContainerPos('reflow'), true);
	      }, 1);

	      /////// printing stuff
	      var beforePrint = function(){
	        unfloat();
	      };
	      var afterPrint = function(){
	        refloat();
	      };
	      var printEvent = function(mql){
	        //make printing the table work properly on IE10+
	        if(mql.matches) {
	          beforePrint();
	        } else {
	          afterPrint();
	        }
	      };

	      var matchMediaPrint;
	      if(window.matchMedia && window.matchMedia('print').addListener){
	        matchMediaPrint = window.matchMedia("print");
	        matchMediaPrint.addListener(printEvent);
	      } else {
	        $window.on('beforeprint', beforePrint);
	        $window.on('afterprint', afterPrint);
	      }
	      ////// end printing stuff


	      if(locked){ //internal scrolling
	        if(useAbsolutePositioning){
	          $scrollContainer.on(eventName('scroll'), containerScrollEvent);
	        } else {
	          $scrollContainer.on(eventName('scroll'), containerScrollEvent);
	          $window.on(eventName('scroll'), windowScrollEvent);
	        }
	      } else { //window scrolling
	        $responsiveContainer.on(eventName('scroll'), containerScrollEvent);
	        $window.on(eventName('scroll'), windowScrollEvent);
	      }

	      $window.on(eventName('load'), reflowEvent); //for tables with images

	      windowResize(eventName('resize'), windowResizeEvent);
	      $table.on('reflow', reflowEvent);
	      if(opts.support && opts.support.datatables && isDatatable($table)){
	        $table
	            .on('filter', reflowEvent)
	            .on('sort',   reflowEvent)
	            .on('page',   reflowEvent);
	      }

	      if(opts.support && opts.support.bootstrap) {
	        $window.on(eventName('shown.bs.tab'), reflowEvent); // people cant seem to figure out how to use this plugin with bs3 tabs... so this :P
	      }
	      if(opts.support && opts.support.jqueryUI) {
	        $window.on(eventName('tabsactivate'), reflowEvent); // same thing for jqueryui
	      }


	      if (canObserveMutations) {
	        var mutationElement = null;
	        if(util.isFunction(opts.autoReflow)){
	          mutationElement = opts.autoReflow($table, $scrollContainer)
	        }
	        if(!mutationElement) {
	          mutationElement = $scrollContainer.length ? $scrollContainer[0] : $table[0]
	        }
	        mObs = new MutationObserver(function(e){
	          var wasTableRelated = function(nodes){
	            return nodes && nodes[0] && (nodes[0].nodeName == "THEAD" || nodes[0].nodeName == "TD"|| nodes[0].nodeName == "TH");
	          };
	          for(var i=0; i < e.length; i++){
	            if(!(wasTableRelated(e[i].addedNodes) || wasTableRelated(e[i].removedNodes))){
	              reflowEvent();
	              break;
	            }
	          }
	        });
	        mObs.observe(mutationElement, {
	          childList: true,
	          subtree: true
	        });
	      }

	      //attach some useful functions to the table.
	      $table.data('floatThead-attached', {
	        destroy: function(){
	          var ns = '.fth-'+floatTheadId;
	          unfloat();
	          $table.css(layoutAuto);
	          $tableColGroup.remove();
	          createElements && $fthGrp.remove();
	          if($newHeader.parent().length){ //only if it's in the DOM
	            $newHeader.replaceWith($header);
	          }
	          triggerFloatEvent(false);
	          if(canObserveMutations){
	            mObs.disconnect();
	            mObs = null;
	          }
	          $table.off('reflow reflowed');
	          $scrollContainer.off(ns);
	          $responsiveContainer.off(ns);
	          if (wrappedContainer) {
	            if ($scrollContainer.length) {
	              $scrollContainer.unwrap();
	            }
	            else {
	              $table.unwrap();
	            }
	          }
	          if(locked){
	            $scrollContainer.data('floatThead-containerWrap', false);
	          } else {
	            $table.data('floatThead-containerWrap', false);
	          }
	          $table.css('minWidth', originalTableMinWidth);
	          $floatContainer.remove();
	          $table.data('floatThead-attached', false);
	          $window.off(ns);
	          if (matchMediaPrint) {
	            matchMediaPrint.removeListener(printEvent);
	          }
	          beforePrint = afterPrint = function(){};

	          return function reinit(){
	            return $table.floatThead(opts);
	          }
	        },
	        reflow: function(){
	          reflowEvent();
	        },
	        setHeaderHeight: function(){
	          setHeaderHeight();
	        },
	        getFloatContainer: function(){
	          return $floatContainer;
	        },
	        getRowGroups: function(){
	          if(headerFloated){
	            return $floatContainer.find('>table>thead').add($table.children("tbody,tfoot"));
	          } else {
	            return $table.children("thead,tbody,tfoot");
	          }
	        }
	      });
	    });
	    return this;
	  };
	})((function(){
	  var $ = window.jQuery;
	  if(typeof module !== 'undefined' && module.exports && !$) {
	    // only use cjs if they dont have a jquery for me to use, and we have commonjs
	    $ = __webpack_require__(1);
	  }
	  return $;
	})());

	/* jQuery.floatThead.utils - http://mkoryak.github.io/floatThead/ - Copyright (c) 2012 - 2016 Misha Koryak
	 * License: MIT
	 *
	 * This file is required if you do not use underscore in your project and you want to use floatThead.
	 * It contains functions from underscore that the plugin uses.
	 *
	 * YOU DON'T NEED TO INCLUDE THIS IF YOU ALREADY INCLUDE UNDERSCORE!
	 *
	 */

	(function($){

	  $.floatThead = $.floatThead || {};

	  $.floatThead._  = window._ || (function(){
	    var that = {};
	    var hasOwnProperty = Object.prototype.hasOwnProperty, isThings = ['Arguments', 'Function', 'String', 'Number', 'Date', 'RegExp'];
	    that.has = function(obj, key) {
	      return hasOwnProperty.call(obj, key);
	    };
	    that.keys = function(obj) {
	      if (obj !== Object(obj)) throw new TypeError('Invalid object');
	      var keys = [];
	      for (var key in obj) if (that.has(obj, key)) keys.push(key);
	      return keys;
	    };
	    var idCounter = 0;
	    that.uniqueId = function(prefix) {
	      var id = ++idCounter + '';
	      return prefix ? prefix + id : id;
	    };
	    $.each(isThings, function(){
	      var name = this;
	      that['is' + name] = function(obj) {
	        return Object.prototype.toString.call(obj) == '[object ' + name + ']';
	      };
	    });
	    that.debounce = function(func, wait, immediate) {
	      var timeout, args, context, timestamp, result;
	      return function() {
	        context = this;
	        args = arguments;
	        timestamp = new Date();
	        var later = function() {
	          var last = (new Date()) - timestamp;
	          if (last < wait) {
	            timeout = setTimeout(later, wait - last);
	          } else {
	            timeout = null;
	            if (!immediate) result = func.apply(context, args);
	          }
	        };
	        var callNow = immediate && !timeout;
	        if (!timeout) {
	          timeout = setTimeout(later, wait);
	        }
	        if (callNow) result = func.apply(context, args);
	        return result;
	      };
	    };
	    return that;
	  })();
	})(jQuery);


	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 34:
/***/ (function(module, exports) {

	/*** IMPORTS FROM imports-loader ***/
	var require = false;
	var module = false;

	/*=============================================================================
		Author:			Eric M. Barnard - @ericmbarnard								
		License:		MIT (http://opensource.org/licenses/mit-license.php)		
																					
		Description:	Validation Library for KnockoutJS							
		Version:		2.0.2											
	===============================================================================
	*/
	/*globals require: false, exports: false, define: false, ko: false */

	(function (factory) {
		// Module systems magic dance.

		if (typeof require === "function" && typeof exports === "object" && typeof module === "object") {
			// CommonJS or Node: hard-coded dependency on "knockout"
			factory(require("knockout"), exports);
		} else if (typeof define === "function" && define["amd"]) {
			// AMD anonymous module with hard-coded dependency on "knockout"
			define(["knockout", "exports"], factory);
		} else {
			// <script> tag: use the global `ko` object, attaching a `mapping` property
			factory(ko, ko.validation = {});
		}
	}(function ( ko, exports ) {

		if (typeof (ko) === 'undefined') {
			throw new Error('Knockout is required, please ensure it is loaded before loading this validation plug-in');
		}

		// create our namespace object
		ko.validation = exports;

		var kv = ko.validation,
			koUtils = ko.utils,
			unwrap = koUtils.unwrapObservable,
			forEach = koUtils.arrayForEach,
			extend = koUtils.extend;
	;/*global ko: false*/

	var defaults = {
		registerExtenders: true,
		messagesOnModified: true,
		errorsAsTitle: true,            // enables/disables showing of errors as title attribute of the target element.
		errorsAsTitleOnModified: false, // shows the error when hovering the input field (decorateElement must be true)
		messageTemplate: null,
		insertMessages: true,           // automatically inserts validation messages as <span></span>
		parseInputAttributes: false,    // parses the HTML5 validation attribute from a form element and adds that to the object
		writeInputAttributes: false,    // adds HTML5 input validation attributes to form elements that ko observable's are bound to
		decorateInputElement: false,         // false to keep backward compatibility
		decorateElementOnModified: true,// true to keep backward compatibility
		errorClass: null,               // single class for error message and element
		errorElementClass: 'validationElement',  // class to decorate error element
		errorMessageClass: 'validationMessage',  // class to decorate error message
		allowHtmlMessages: false,		// allows HTML in validation messages
		grouping: {
			deep: false,        //by default grouping is shallow
			observable: true,   //and using observables
			live: false		    //react to changes to observableArrays if observable === true
		},
		validate: {
			// throttle: 10
		}
	};

	// make a copy  so we can use 'reset' later
	var configuration = extend({}, defaults);

	configuration.html5Attributes = ['required', 'pattern', 'min', 'max', 'step'];
	configuration.html5InputTypes = ['email', 'number', 'date'];

	configuration.reset = function () {
		extend(configuration, defaults);
	};

	kv.configuration = configuration;
	;kv.utils = (function () {
		var seedId = new Date().getTime();

		var domData = {}; //hash of data objects that we reference from dom elements
		var domDataKey = '__ko_validation__';

		return {
			isArray: function (o) {
				return o.isArray || Object.prototype.toString.call(o) === '[object Array]';
			},
			isObject: function (o) {
				return o !== null && typeof o === 'object';
			},
			isNumber: function(o) {
				return !isNaN(o);	
			},
			isObservableArray: function(instance) {
				return !!instance &&
						typeof instance["remove"] === "function" &&
						typeof instance["removeAll"] === "function" &&
						typeof instance["destroy"] === "function" &&
						typeof instance["destroyAll"] === "function" &&
						typeof instance["indexOf"] === "function" &&
						typeof instance["replace"] === "function";
			},
			values: function (o) {
				var r = [];
				for (var i in o) {
					if (o.hasOwnProperty(i)) {
						r.push(o[i]);
					}
				}
				return r;
			},
			getValue: function (o) {
				return (typeof o === 'function' ? o() : o);
			},
			hasAttribute: function (node, attr) {
				return node.getAttribute(attr) !== null;
			},
			getAttribute: function (element, attr) {
				return element.getAttribute(attr);
			},
			setAttribute: function (element, attr, value) {
				return element.setAttribute(attr, value);
			},
			isValidatable: function (o) {
				return !!(o && o.rules && o.isValid && o.isModified);
			},
			insertAfter: function (node, newNode) {
				node.parentNode.insertBefore(newNode, node.nextSibling);
			},
			newId: function () {
				return seedId += 1;
			},
			getConfigOptions: function (element) {
				var options = kv.utils.contextFor(element);

				return options || kv.configuration;
			},
			setDomData: function (node, data) {
				var key = node[domDataKey];

				if (!key) {
					node[domDataKey] = key = kv.utils.newId();
				}

				domData[key] = data;
			},
			getDomData: function (node) {
				var key = node[domDataKey];

				if (!key) {
					return undefined;
				}

				return domData[key];
			},
			contextFor: function (node) {
				switch (node.nodeType) {
					case 1:
					case 8:
						var context = kv.utils.getDomData(node);
						if (context) { return context; }
						if (node.parentNode) { return kv.utils.contextFor(node.parentNode); }
						break;
				}
				return undefined;
			},
			isEmptyVal: function (val) {
				if (val === undefined) {
					return true;
				}
				if (val === null) {
					return true;
				}
				if (val === "") {
					return true;
				}
			},
			getOriginalElementTitle: function (element) {
				var savedOriginalTitle = kv.utils.getAttribute(element, 'data-orig-title'),
					currentTitle = element.title,
					hasSavedOriginalTitle = kv.utils.hasAttribute(element, 'data-orig-title');

				return hasSavedOriginalTitle ?
					savedOriginalTitle : currentTitle;
			},
			async: function (expr) {
				if (window.setImmediate) { window.setImmediate(expr); }
				else { window.setTimeout(expr, 0); }
			},
			forEach: function (object, callback) {
				if (kv.utils.isArray(object)) {
					return forEach(object, callback);
				}
				for (var prop in object) {
					if (object.hasOwnProperty(prop)) {
						callback(object[prop], prop);
					}
				}
			}
		};
	}());;var api = (function () {

		var isInitialized = 0,
			configuration = kv.configuration,
			utils = kv.utils;

		function cleanUpSubscriptions(context) {
			forEach(context.subscriptions, function (subscription) {
				subscription.dispose();
			});
			context.subscriptions = [];
		}

		function dispose(context) {
			if (context.options.deep) {
				forEach(context.flagged, function (obj) {
					delete obj.__kv_traversed;
				});
				context.flagged.length = 0;
			}

			if (!context.options.live) {
				cleanUpSubscriptions(context);
			}
		}

		function runTraversal(obj, context) {
			context.validatables = [];
			cleanUpSubscriptions(context);
			traverseGraph(obj, context);
			dispose(context);
		}

		function traverseGraph(obj, context, level) {
			var objValues = [],
				val = obj.peek ? obj.peek() : obj;

			if (obj.__kv_traversed === true) {
				return;
			}

			if (context.options.deep) {
				obj.__kv_traversed = true;
				context.flagged.push(obj);
			}

			//default level value depends on deep option.
			level = (level !== undefined ? level : context.options.deep ? 1 : -1);

			// if object is observable then add it to the list
			if (ko.isObservable(obj)) {
				// ensure it's validatable but don't extend validatedObservable because it
				// would overwrite isValid property.
				if (!obj.errors && !utils.isValidatable(obj)) {
					obj.extend({ validatable: true });
				}
				context.validatables.push(obj);

				if (context.options.live && utils.isObservableArray(obj)) {
					context.subscriptions.push(obj.subscribe(function () {
						context.graphMonitor.valueHasMutated();
					}));
				}
			}

			//get list of values either from array or object but ignore non-objects
			// and destroyed objects
			if (val && !val._destroy) {
				if (utils.isArray(val)) {
					objValues = val;
				}
				else if (utils.isObject(val)) {
					objValues = utils.values(val);
				}
			}

			//process recursively if it is deep grouping
			if (level !== 0) {
				utils.forEach(objValues, function (observable) {
					//but not falsy things and not HTML Elements
					if (observable && !observable.nodeType && (!ko.isComputed(observable) || observable.rules)) {
						traverseGraph(observable, context, level + 1);
					}
				});
			}
		}

		function collectErrors(array) {
			var errors = [];
			forEach(array, function (observable) {
				// Do not collect validatedObservable errors
				if (utils.isValidatable(observable) && !observable.isValid()) {
					// Use peek because we don't want a dependency for 'error' property because it
					// changes before 'isValid' does. (Issue #99)
					errors.push(observable.error.peek());
				}
			});
			return errors;
		}

		return {
			//Call this on startup
			//any config can be overridden with the passed in options
			init: function (options, force) {
				//done run this multiple times if we don't really want to
				if (isInitialized > 0 && !force) {
					return;
				}

				//because we will be accessing options properties it has to be an object at least
				options = options || {};
				//if specific error classes are not provided then apply generic errorClass
				//it has to be done on option so that options.errorClass can override default
				//errorElementClass and errorMessage class but not those provided in options
				options.errorElementClass = options.errorElementClass || options.errorClass || configuration.errorElementClass;
				options.errorMessageClass = options.errorMessageClass || options.errorClass || configuration.errorMessageClass;

				extend(configuration, options);

				if (configuration.registerExtenders) {
					kv.registerExtenders();
				}

				isInitialized = 1;
			},

			// resets the config back to its original state
			reset: kv.configuration.reset,

			// recursively walks a viewModel and creates an object that
			// provides validation information for the entire viewModel
			// obj -> the viewModel to walk
			// options -> {
			//	  deep: false, // if true, will walk past the first level of viewModel properties
			//	  observable: false // if true, returns a computed observable indicating if the viewModel is valid
			// }
			group: function group(obj, options) { // array of observables or viewModel
				options = extend(extend({}, configuration.grouping), options);

				var context = {
					options: options,
					graphMonitor: ko.observable(),
					flagged: [],
					subscriptions: [],
					validatables: []
				};

				var result = null;

				//if using observables then traverse structure once and add observables
				if (options.observable) {
					result = ko.computed(function () {
						context.graphMonitor(); //register dependency
						runTraversal(obj, context);
						return collectErrors(context.validatables);
					});
				}
				else { //if not using observables then every call to error() should traverse the structure
					result = function () {
						runTraversal(obj, context);
						return collectErrors(context.validatables);
					};
				}

				result.showAllMessages = function (show) { // thanks @heliosPortal
					if (show === undefined) {//default to true
						show = true;
					}

					result.forEach(function (observable) {
						if (utils.isValidatable(observable)) {
							observable.isModified(show);
						}
					});
				};

				result.isAnyMessageShown = function () {
					var invalidAndModifiedPresent;

					invalidAndModifiedPresent = !!result.find(function (observable) {
						return utils.isValidatable(observable) && !observable.isValid() && observable.isModified();
					});
					return invalidAndModifiedPresent;
				};

				result.filter = function(predicate) {
					predicate = predicate || function () { return true; };
					// ensure we have latest changes
					result();

					return koUtils.arrayFilter(context.validatables, predicate);
				};

				result.find = function(predicate) {
					predicate = predicate || function () { return true; };
					// ensure we have latest changes
					result();

					return koUtils.arrayFirst(context.validatables, predicate);
				};

				result.forEach = function(callback) {
					callback = callback || function () { };
					// ensure we have latest changes
					result();

					forEach(context.validatables, callback);
				};

				result.map = function(mapping) {
					mapping = mapping || function (item) { return item; };
					// ensure we have latest changes
					result();

					return koUtils.arrayMap(context.validatables, mapping);
				};

				/**
				 * @private You should not rely on this method being here.
				 * It's a private method and it may change in the future.
				 *
				 * @description Updates the validated object and collects errors from it.
				 */
				result._updateState = function(newValue) {
					if (!utils.isObject(newValue)) {
						throw new Error('An object is required.');
					}
					obj = newValue;
					if (options.observable) {
						context.graphMonitor.valueHasMutated();
					}
					else {
						runTraversal(newValue, context);
						return collectErrors(context.validatables);
					}
				};
				return result;
			},

			formatMessage: function (message, params, observable) {
				if (utils.isObject(params) && params.typeAttr) {
					params = params.value;
				}
				if (typeof (message) === 'function') {
					return message(params, observable);
				}
				var replacements = unwrap(params) || [];
				if (!utils.isArray(replacements)) {
					replacements = [replacements];
				}
				return message.replace(/{(\d+)}/gi, function(match, index) {
					if (typeof replacements[index] !== 'undefined') {
						return replacements[index];
					}
					return match;
				});
			},

			// addRule:
			// This takes in a ko.observable and a Rule Context - which is just a rule name and params to supply to the validator
			// ie: kv.addRule(myObservable, {
			//		  rule: 'required',
			//		  params: true
			//	  });
			//
			addRule: function (observable, rule) {
				observable.extend({ validatable: true });

				var hasRule = !!koUtils.arrayFirst(observable.rules(), function(item) {
					return item.rule && item.rule === rule.rule;
				});

				if (!hasRule) {
					//push a Rule Context to the observables local array of Rule Contexts
					observable.rules.push(rule);
				}
				return observable;
			},

			// addAnonymousRule:
			// Anonymous Rules essentially have all the properties of a Rule, but are only specific for a certain property
			// and developers typically are wanting to add them on the fly or not register a rule with the 'kv.rules' object
			//
			// Example:
			// var test = ko.observable('something').extend{(
			//	  validation: {
			//		  validator: function(val, someOtherVal){
			//			  return true;
			//		  },
			//		  message: "Something must be really wrong!',
			//		  params: true
			//	  }
			//  )};
			addAnonymousRule: function (observable, ruleObj) {
				if (ruleObj['message'] === undefined) {
					ruleObj['message'] = 'Error';
				}

				//make sure onlyIf is honoured
				if (ruleObj.onlyIf) {
					ruleObj.condition = ruleObj.onlyIf;
				}

				//add the anonymous rule to the observable
				kv.addRule(observable, ruleObj);
			},

			addExtender: function (ruleName) {
				ko.extenders[ruleName] = function (observable, params) {
					//params can come in a few flavors
					// 1. Just the params to be passed to the validator
					// 2. An object containing the Message to be used and the Params to pass to the validator
					// 3. A condition when the validation rule to be applied
					//
					// Example:
					// var test = ko.observable(3).extend({
					//	  max: {
					//		  message: 'This special field has a Max of {0}',
					//		  params: 2,
					//		  onlyIf: function() {
					//					  return specialField.IsVisible();
					//				  }
					//	  }
					//  )};
					//
					if (params && (params.message || params.onlyIf)) { //if it has a message or condition object, then its an object literal to use
						return kv.addRule(observable, {
							rule: ruleName,
							message: params.message,
							params: utils.isEmptyVal(params.params) ? true : params.params,
							condition: params.onlyIf
						});
					} else {
						return kv.addRule(observable, {
							rule: ruleName,
							params: params
						});
					}
				};
			},

			// loops through all kv.rules and adds them as extenders to
			// ko.extenders
			registerExtenders: function () { // root extenders optional, use 'validation' extender if would cause conflicts
				if (configuration.registerExtenders) {
					for (var ruleName in kv.rules) {
						if (kv.rules.hasOwnProperty(ruleName)) {
							if (!ko.extenders[ruleName]) {
								kv.addExtender(ruleName);
							}
						}
					}
				}
			},

			//creates a span next to the @element with the specified error class
			insertValidationMessage: function (element) {
				var span = document.createElement('SPAN');
				span.className = utils.getConfigOptions(element).errorMessageClass;
				utils.insertAfter(element, span);
				return span;
			},

			// if html-5 validation attributes have been specified, this parses
			// the attributes on @element
			parseInputValidationAttributes: function (element, valueAccessor) {
				forEach(kv.configuration.html5Attributes, function (attr) {
					if (utils.hasAttribute(element, attr)) {

						var params = element.getAttribute(attr) || true;

						if (attr === 'min' || attr === 'max')
						{
							// If we're validating based on the min and max attributes, we'll
							// need to know what the 'type' attribute is set to
							var typeAttr = element.getAttribute('type');
							if (typeof typeAttr === "undefined" || !typeAttr)
							{
								// From http://www.w3.org/TR/html-markup/input:
								//   An input element with no type attribute specified represents the
								//   same thing as an input element with its type attribute set to "text".
								typeAttr = "text";
							}
							params = {typeAttr: typeAttr, value: params};
						}

						kv.addRule(valueAccessor(), {
							rule: attr,
							params: params
						});
					}
				});

				var currentType = element.getAttribute('type');
				forEach(kv.configuration.html5InputTypes, function (type) {
					if (type === currentType) {
						kv.addRule(valueAccessor(), {
							rule: (type === 'date') ? 'dateISO' : type,
							params: true
						});
					}
				});
			},

			// writes html5 validation attributes on the element passed in
			writeInputValidationAttributes: function (element, valueAccessor) {
				var observable = valueAccessor();

				if (!observable || !observable.rules) {
					return;
				}

				var contexts = observable.rules(); // observable array

				// loop through the attributes and add the information needed
				forEach(kv.configuration.html5Attributes, function (attr) {
					var ctx = koUtils.arrayFirst(contexts, function (ctx) {
						return ctx.rule && ctx.rule.toLowerCase() === attr.toLowerCase();
					});

					if (!ctx) {
						return;
					}

					// we have a rule matching a validation attribute at this point
					// so lets add it to the element along with the params
					ko.computed({
						read: function() {
							var params = ko.unwrap(ctx.params);

							// we have to do some special things for the pattern validation
							if (ctx.rule === "pattern" && params instanceof RegExp) {
								// we need the pure string representation of the RegExpr without the //gi stuff
								params = params.source;
							}

							element.setAttribute(attr, params);
						},
						disposeWhenNodeIsRemoved: element
					});
				});

				contexts = null;
			},

			//take an existing binding handler and make it cause automatic validations
			makeBindingHandlerValidatable: function (handlerName) {
				var init = ko.bindingHandlers[handlerName].init;

				ko.bindingHandlers[handlerName].init = function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

					init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);

					return ko.bindingHandlers['validationCore'].init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
				};
			},

			// visit an objects properties and apply validation rules from a definition
			setRules: function (target, definition) {
				var setRules = function (target, definition) {
					if (!target || !definition) { return; }

					for (var prop in definition) {
						if (!definition.hasOwnProperty(prop)) { continue; }
						var ruleDefinitions = definition[prop];

						//check the target property exists and has a value
						if (!target[prop]) { continue; }
						var targetValue = target[prop],
							unwrappedTargetValue = unwrap(targetValue),
							rules = {},
							nonRules = {};

						for (var rule in ruleDefinitions) {
							if (!ruleDefinitions.hasOwnProperty(rule)) { continue; }
							if (kv.rules[rule]) {
								rules[rule] = ruleDefinitions[rule];
							} else {
								nonRules[rule] = ruleDefinitions[rule];
							}
						}

						//apply rules
						if (ko.isObservable(targetValue)) {
							targetValue.extend(rules);
						}

						//then apply child rules
						//if it's an array, apply rules to all children
						if (unwrappedTargetValue && utils.isArray(unwrappedTargetValue)) {
							for (var i = 0; i < unwrappedTargetValue.length; i++) {
								setRules(unwrappedTargetValue[i], nonRules);
							}
							//otherwise, just apply to this property
						} else {
							setRules(unwrappedTargetValue, nonRules);
						}
					}
				};
				setRules(target, definition);
			}
		};

	}());

	// expose api publicly
	extend(ko.validation, api);
	;//Validation Rules:
	// You can view and override messages or rules via:
	// kv.rules[ruleName]
	//
	// To implement a custom Rule, simply use this template:
	// kv.rules['<custom rule name>'] = {
	//      validator: function (val, param) {
	//          <custom logic>
	//          return <true or false>;
	//      },
	//      message: '<custom validation message>' //optionally you can also use a '{0}' to denote a placeholder that will be replaced with your 'param'
	// };
	//
	// Example:
	// kv.rules['mustEqual'] = {
	//      validator: function( val, mustEqualVal ){
	//          return val === mustEqualVal;
	//      },
	//      message: 'This field must equal {0}'
	// };
	//
	kv.rules = {};
	kv.rules['required'] = {
		validator: function (val, required) {
			var testVal;

			if (val === undefined || val === null) {
				return !required;
			}

			testVal = val;
			if (typeof (val) === 'string') {
				if (String.prototype.trim) {
					testVal = val.trim();
				}
				else {
					testVal = val.replace(/^\s+|\s+$/g, '');
				}
			}

			if (!required) {// if they passed: { required: false }, then don't require this
				return true;
			}

			return ((testVal + '').length > 0);
		},
		message: 'This field is required.'
	};

	function minMaxValidatorFactory(validatorName) {
	    var isMaxValidation = validatorName === "max";

	    return function (val, options) {
	        if (kv.utils.isEmptyVal(val)) {
	            return true;
	        }

	        var comparisonValue, type;
	        if (options.typeAttr === undefined) {
	            // This validator is being called from javascript rather than
	            // being bound from markup
	            type = "text";
	            comparisonValue = options;
	        } else {
	            type = options.typeAttr;
	            comparisonValue = options.value;
	        }

	        // From http://www.w3.org/TR/2012/WD-html5-20121025/common-input-element-attributes.html#attr-input-min,
	        // if the value is parseable to a number, then the minimum should be numeric
	        if (!isNaN(comparisonValue) && !(comparisonValue instanceof Date)) {
	            type = "number";
	        }

	        var regex, valMatches, comparisonValueMatches;
	        switch (type.toLowerCase()) {
	            case "week":
	                regex = /^(\d{4})-W(\d{2})$/;
	                valMatches = val.match(regex);
	                if (valMatches === null) {
	                    throw new Error("Invalid value for " + validatorName + " attribute for week input.  Should look like " +
	                        "'2000-W33' http://www.w3.org/TR/html-markup/input.week.html#input.week.attrs.min");
	                }
	                comparisonValueMatches = comparisonValue.match(regex);
	                // If no regex matches were found, validation fails
	                if (!comparisonValueMatches) {
	                    return false;
	                }

	                if (isMaxValidation) {
	                    return (valMatches[1] < comparisonValueMatches[1]) || // older year
	                        // same year, older week
	                        ((valMatches[1] === comparisonValueMatches[1]) && (valMatches[2] <= comparisonValueMatches[2]));
	                } else {
	                    return (valMatches[1] > comparisonValueMatches[1]) || // newer year
	                        // same year, newer week
	                        ((valMatches[1] === comparisonValueMatches[1]) && (valMatches[2] >= comparisonValueMatches[2]));
	                }
	                break;

	            case "month":
	                regex = /^(\d{4})-(\d{2})$/;
	                valMatches = val.match(regex);
	                if (valMatches === null) {
	                    throw new Error("Invalid value for " + validatorName + " attribute for month input.  Should look like " +
	                        "'2000-03' http://www.w3.org/TR/html-markup/input.month.html#input.month.attrs.min");
	                }
	                comparisonValueMatches = comparisonValue.match(regex);
	                // If no regex matches were found, validation fails
	                if (!comparisonValueMatches) {
	                    return false;
	                }

	                if (isMaxValidation) {
	                    return ((valMatches[1] < comparisonValueMatches[1]) || // older year
	                        // same year, older month
	                        ((valMatches[1] === comparisonValueMatches[1]) && (valMatches[2] <= comparisonValueMatches[2])));
	                } else {
	                    return (valMatches[1] > comparisonValueMatches[1]) || // newer year
	                        // same year, newer month
	                        ((valMatches[1] === comparisonValueMatches[1]) && (valMatches[2] >= comparisonValueMatches[2]));
	                }
	                break;

	            case "number":
	            case "range":
	                if (isMaxValidation) {
	                    return (!isNaN(val) && parseFloat(val) <= parseFloat(comparisonValue));
	                } else {
	                    return (!isNaN(val) && parseFloat(val) >= parseFloat(comparisonValue));
	                }
	                break;

	            default:
	                if (isMaxValidation) {
	                    return val <= comparisonValue;
	                } else {
	                    return val >= comparisonValue;
	                }
	        }
	    };
	}

	kv.rules['min'] = {
		validator: minMaxValidatorFactory("min"),
		message: 'Please enter a value greater than or equal to {0}.'
	};

	kv.rules['max'] = {
		validator: minMaxValidatorFactory("max"),
		message: 'Please enter a value less than or equal to {0}.'
	};

	kv.rules['minLength'] = {
		validator: function (val, minLength) {
			if(kv.utils.isEmptyVal(val)) { return true; }
			var normalizedVal = kv.utils.isNumber(val) ? ('' + val) : val;
			return normalizedVal.length >= minLength;
		},
		message: 'Please enter at least {0} characters.'
	};

	kv.rules['maxLength'] = {
		validator: function (val, maxLength) {
			if(kv.utils.isEmptyVal(val)) { return true; }
			var normalizedVal = kv.utils.isNumber(val) ? ('' + val) : val;
			return normalizedVal.length <= maxLength;
		},
		message: 'Please enter no more than {0} characters.'
	};

	kv.rules['pattern'] = {
		validator: function (val, regex) {
			return kv.utils.isEmptyVal(val) || val.toString().match(regex) !== null;
		},
		message: 'Please check this value.'
	};

	kv.rules['step'] = {
		validator: function (val, step) {

			// in order to handle steps of .1 & .01 etc.. Modulus won't work
			// if the value is a decimal, so we have to correct for that
			if (kv.utils.isEmptyVal(val) || step === 'any') { return true; }
			var dif = (val * 100) % (step * 100);
			return Math.abs(dif) < 0.00001 || Math.abs(1 - dif) < 0.00001;
		},
		message: 'The value must increment by {0}.'
	};

	kv.rules['email'] = {
		validator: function (val, validate) {
			if (!validate) { return true; }

			//I think an empty email address is also a valid entry
			//if one want's to enforce entry it should be done with 'required: true'
			return kv.utils.isEmptyVal(val) || (
				// jquery validate regex - thanks Scott Gonzalez
				validate && /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$/i.test(val)
			);
		},
		message: 'Please enter a proper email address.'
	};

	kv.rules['date'] = {
		validator: function (value, validate) {
			if (!validate) { return true; }
			return kv.utils.isEmptyVal(value) || (validate && !/Invalid|NaN/.test(new Date(value)));
		},
		message: 'Please enter a proper date.'
	};

	kv.rules['dateISO'] = {
		validator: function (value, validate) {
			if (!validate) { return true; }
			return kv.utils.isEmptyVal(value) || (validate && /^\d{4}[-/](?:0?[1-9]|1[012])[-/](?:0?[1-9]|[12][0-9]|3[01])$/.test(value));
		},
		message: 'Please enter a proper date.'
	};

	kv.rules['number'] = {
		validator: function (value, validate) {
			if (!validate) { return true; }
			return kv.utils.isEmptyVal(value) || (validate && /^-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$/.test(value));
		},
		message: 'Please enter a number.'
	};

	kv.rules['digit'] = {
		validator: function (value, validate) {
			if (!validate) { return true; }
			return kv.utils.isEmptyVal(value) || (validate && /^\d+$/.test(value));
		},
		message: 'Please enter a digit.'
	};

	kv.rules['phoneUS'] = {
		validator: function (phoneNumber, validate) {
			if (!validate) { return true; }
			if (kv.utils.isEmptyVal(phoneNumber)) { return true; } // makes it optional, use 'required' rule if it should be required
			if (typeof (phoneNumber) !== 'string') { return false; }
			phoneNumber = phoneNumber.replace(/\s+/g, "");
			return validate && phoneNumber.length > 9 && phoneNumber.match(/^(1-?)?(\([2-9]\d{2}\)|[2-9]\d{2})-?[2-9]\d{2}-?\d{4}$/);
		},
		message: 'Please specify a valid phone number.'
	};

	kv.rules['equal'] = {
		validator: function (val, params) {
			var otherValue = params;
			return val === kv.utils.getValue(otherValue);
		},
		message: 'Values must equal.'
	};

	kv.rules['notEqual'] = {
		validator: function (val, params) {
			var otherValue = params;
			return val !== kv.utils.getValue(otherValue);
		},
		message: 'Please choose another value.'
	};

	//unique in collection
	// options are:
	//    collection: array or function returning (observable) array
	//              in which the value has to be unique
	//    valueAccessor: function that returns value from an object stored in collection
	//              if it is null the value is compared directly
	//    external: set to true when object you are validating is automatically updating collection
	kv.rules['unique'] = {
		validator: function (val, options) {
			var c = kv.utils.getValue(options.collection),
				external = kv.utils.getValue(options.externalValue),
				counter = 0;

			if (!val || !c) { return true; }

			koUtils.arrayFilter(c, function (item) {
				if (val === (options.valueAccessor ? options.valueAccessor(item) : item)) { counter++; }
			});
			// if value is external even 1 same value in collection means the value is not unique
			return counter < (!!external ? 1 : 2);
		},
		message: 'Please make sure the value is unique.'
	};


	//now register all of these!
	(function () {
		kv.registerExtenders();
	}());
	;// The core binding handler
	// this allows us to setup any value binding that internally always
	// performs the same functionality
	ko.bindingHandlers['validationCore'] = (function () {

		return {
			init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
				var config = kv.utils.getConfigOptions(element);
				var observable = valueAccessor();

				// parse html5 input validation attributes, optional feature
				if (config.parseInputAttributes) {
					kv.utils.async(function () { kv.parseInputValidationAttributes(element, valueAccessor); });
				}

				// if requested insert message element and apply bindings
				if (config.insertMessages && kv.utils.isValidatable(observable)) {

					// insert the <span></span>
					var validationMessageElement = kv.insertValidationMessage(element);

					// if we're told to use a template, make sure that gets rendered
					if (config.messageTemplate) {
						ko.renderTemplate(config.messageTemplate, { field: observable }, null, validationMessageElement, 'replaceNode');
					} else {
						ko.applyBindingsToNode(validationMessageElement, { validationMessage: observable });
					}
				}

				// write the html5 attributes if indicated by the config
				if (config.writeInputAttributes && kv.utils.isValidatable(observable)) {

					kv.writeInputValidationAttributes(element, valueAccessor);
				}

				// if requested, add binding to decorate element
				if (config.decorateInputElement && kv.utils.isValidatable(observable)) {
					ko.applyBindingsToNode(element, { validationElement: observable });
				}
			}
		};

	}());

	// override for KO's default 'value', 'checked', 'textInput' and selectedOptions bindings
	kv.makeBindingHandlerValidatable("value");
	kv.makeBindingHandlerValidatable("checked");
	if (ko.bindingHandlers.textInput) {
		kv.makeBindingHandlerValidatable("textInput");
	}
	kv.makeBindingHandlerValidatable("selectedOptions");


	ko.bindingHandlers['validationMessage'] = { // individual error message, if modified or post binding
		update: function (element, valueAccessor) {
			var obsv = valueAccessor(),
				config = kv.utils.getConfigOptions(element),
				val = unwrap(obsv),
				msg = null,
				isModified = false,
				isValid = false;

			if (obsv === null || typeof obsv === 'undefined') {
				throw new Error('Cannot bind validationMessage to undefined value. data-bind expression: ' +
					element.getAttribute('data-bind'));
			}

			isModified = obsv.isModified && obsv.isModified();
			isValid = obsv.isValid && obsv.isValid();

			var error = null;
			if (!config.messagesOnModified || isModified) {
				error = isValid ? null : obsv.error;
			}

			var isVisible = !config.messagesOnModified || isModified ? !isValid : false;
			var isCurrentlyVisible = element.style.display !== "none";

			if (config.allowHtmlMessages) {
				koUtils.setHtml(element, error);
			} else {
				ko.bindingHandlers.text.update(element, function () { return error; });
			}

			if (isCurrentlyVisible && !isVisible) {
				element.style.display = 'none';
			} else if (!isCurrentlyVisible && isVisible) {
				element.style.display = '';
			}
		}
	};

	ko.bindingHandlers['validationElement'] = {
		update: function (element, valueAccessor, allBindingsAccessor) {
			var obsv = valueAccessor(),
				config = kv.utils.getConfigOptions(element),
				val = unwrap(obsv),
				msg = null,
				isModified = false,
				isValid = false;

			if (obsv === null || typeof obsv === 'undefined') {
				throw new Error('Cannot bind validationElement to undefined value. data-bind expression: ' +
					element.getAttribute('data-bind'));
			}

			isModified = obsv.isModified && obsv.isModified();
			isValid = obsv.isValid && obsv.isValid();

			// create an evaluator function that will return something like:
			// css: { validationElement: true }
			var cssSettingsAccessor = function () {
				var css = {};

				var shouldShow = ((!config.decorateElementOnModified || isModified) ? !isValid : false);

				// css: { validationElement: false }
				css[config.errorElementClass] = shouldShow;

				return css;
			};

			//add or remove class on the element;
			ko.bindingHandlers.css.update(element, cssSettingsAccessor, allBindingsAccessor);
			if (!config.errorsAsTitle) { return; }

			ko.bindingHandlers.attr.update(element, function () {
				var
					hasModification = !config.errorsAsTitleOnModified || isModified,
					title = kv.utils.getOriginalElementTitle(element);

				if (hasModification && !isValid) {
					return { title: obsv.error, 'data-orig-title': title };
				} else if (!hasModification || isValid) {
					return { title: title, 'data-orig-title': null };
				}
			});
		}
	};

	// ValidationOptions:
	// This binding handler allows you to override the initial config by setting any of the options for a specific element or context of elements
	//
	// Example:
	// <div data-bind="validationOptions: { insertMessages: true, messageTemplate: 'customTemplate', errorMessageClass: 'mySpecialClass'}">
	//      <input type="text" data-bind="value: someValue"/>
	//      <input type="text" data-bind="value: someValue2"/>
	// </div>
	ko.bindingHandlers['validationOptions'] = (function () {
		return {
			init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
				var options = unwrap(valueAccessor());
				if (options) {
					var newConfig = extend({}, kv.configuration);
					extend(newConfig, options);

					//store the validation options on the node so we can retrieve it later
					kv.utils.setDomData(element, newConfig);
				}
			}
		};
	}());
	;// Validation Extender:
	// This is for creating custom validation logic on the fly
	// Example:
	// var test = ko.observable('something').extend{(
	//      validation: {
	//          validator: function(val, someOtherVal){
	//              return true;
	//          },
	//          message: "Something must be really wrong!',
	//          params: true
	//      }
	//  )};
	ko.extenders['validation'] = function (observable, rules) { // allow single rule or array
		forEach(kv.utils.isArray(rules) ? rules : [rules], function (rule) {
			// the 'rule' being passed in here has no name to identify a core Rule,
			// so we add it as an anonymous rule
			// If the developer is wanting to use a core Rule, but use a different message see the 'addExtender' logic for examples
			kv.addAnonymousRule(observable, rule);
		});
		return observable;
	};

	//This is the extender that makes a Knockout Observable also 'Validatable'
	//examples include:
	// 1. var test = ko.observable('something').extend({validatable: true});
	// this will ensure that the Observable object is setup properly to respond to rules
	//
	// 2. test.extend({validatable: false});
	// this will remove the validation properties from the Observable object should you need to do that.
	ko.extenders['validatable'] = function (observable, options) {
		if (!kv.utils.isObject(options)) {
			options = { enable: options };
		}

		if (!('enable' in options)) {
			options.enable = true;
		}

		if (options.enable && !kv.utils.isValidatable(observable)) {
			var config = kv.configuration.validate || {};
			var validationOptions = {
				throttleEvaluation : options.throttle || config.throttle
			};

			observable.error = ko.observable(null); // holds the error message, we only need one since we stop processing validators when one is invalid

			// observable.rules:
			// ObservableArray of Rule Contexts, where a Rule Context is simply the name of a rule and the params to supply to it
			//
			// Rule Context = { rule: '<rule name>', params: '<passed in params>', message: '<Override of default Message>' }
			observable.rules = ko.observableArray(); //holds the rule Contexts to use as part of validation

			//in case async validation is occurring
			observable.isValidating = ko.observable(false);

			//the true holder of whether the observable is valid or not
			observable.__valid__ = ko.observable(true);

			observable.isModified = ko.observable(false);

			// a semi-protected observable
			observable.isValid = ko.computed(observable.__valid__);

			//manually set error state
			observable.setError = function (error) {
				var previousError = observable.error.peek();
				var previousIsValid = observable.__valid__.peek();

				observable.error(error);
				observable.__valid__(false);

				if (previousError !== error && !previousIsValid) {
					// if the observable was not valid before then isValid will not mutate,
					// hence causing any grouping to not display the latest error.
					observable.isValid.notifySubscribers();
				}
			};

			//manually clear error state
			observable.clearError = function () {
				observable.error(null);
				observable.__valid__(true);
				return observable;
			};

			//subscribe to changes in the observable
			var h_change = observable.subscribe(function () {
				observable.isModified(true);
			});

			// we use a computed here to ensure that anytime a dependency changes, the
			// validation logic evaluates
			var h_obsValidationTrigger = ko.computed(extend({
				read: function () {
					var obs = observable(),
						ruleContexts = observable.rules();

					kv.validateObservable(observable);

					return true;
				}
			}, validationOptions));

			extend(h_obsValidationTrigger, validationOptions);

			observable._disposeValidation = function () {
				//first dispose of the subscriptions
				observable.isValid.dispose();
				observable.rules.removeAll();
				h_change.dispose();
				h_obsValidationTrigger.dispose();

				delete observable['rules'];
				delete observable['error'];
				delete observable['isValid'];
				delete observable['isValidating'];
				delete observable['__valid__'];
				delete observable['isModified'];
	            delete observable['setError'];
	            delete observable['clearError'];
	            delete observable['_disposeValidation'];
			};
		} else if (options.enable === false && observable._disposeValidation) {
			observable._disposeValidation();
		}
		return observable;
	};

	function validateSync(observable, rule, ctx) {
		//Execute the validator and see if its valid
		if (!rule.validator(observable(), (ctx.params === undefined ? true : unwrap(ctx.params)))) { // default param is true, eg. required = true

			//not valid, so format the error message and stick it in the 'error' variable
			observable.setError(kv.formatMessage(
						ctx.message || rule.message,
						unwrap(ctx.params),
						observable));
			return false;
		} else {
			return true;
		}
	}

	function validateAsync(observable, rule, ctx) {
		observable.isValidating(true);

		var callBack = function (valObj) {
			var isValid = false,
				msg = '';

			if (!observable.__valid__()) {

				// since we're returning early, make sure we turn this off
				observable.isValidating(false);

				return; //if its already NOT valid, don't add to that
			}

			//we were handed back a complex object
			if (valObj['message']) {
				isValid = valObj.isValid;
				msg = valObj.message;
			} else {
				isValid = valObj;
			}

			if (!isValid) {
				//not valid, so format the error message and stick it in the 'error' variable
				observable.error(kv.formatMessage(
					msg || ctx.message || rule.message,
					unwrap(ctx.params),
					observable));
				observable.__valid__(isValid);
			}

			// tell it that we're done
			observable.isValidating(false);
		};

		//fire the validator and hand it the callback
		rule.validator(observable(), unwrap(ctx.params || true), callBack);
	}

	kv.validateObservable = function (observable) {
		var i = 0,
			rule, // the rule validator to execute
			ctx, // the current Rule Context for the loop
			ruleContexts = observable.rules(), //cache for iterator
			len = ruleContexts.length; //cache for iterator

		for (; i < len; i++) {

			//get the Rule Context info to give to the core Rule
			ctx = ruleContexts[i];

			// checks an 'onlyIf' condition
			if (ctx.condition && !ctx.condition()) {
				continue;
			}

			//get the core Rule to use for validation
			rule = ctx.rule ? kv.rules[ctx.rule] : ctx;

			if (rule['async'] || ctx['async']) {
				//run async validation
				validateAsync(observable, rule, ctx);

			} else {
				//run normal sync validation
				if (!validateSync(observable, rule, ctx)) {
					return false; //break out of the loop
				}
			}
		}
		//finally if we got this far, make the observable valid again!
		observable.clearError();
		return true;
	};
	;
	var _locales = {};
	var _currentLocale;

	kv.defineLocale = function(name, values) {
		if (name && values) {
			_locales[name.toLowerCase()] = values;
			return values;
		}
		return null;
	};

	kv.locale = function(name) {
		if (name) {
			name = name.toLowerCase();

			if (_locales.hasOwnProperty(name)) {
				kv.localize(_locales[name]);
				_currentLocale = name;
			}
			else {
				throw new Error('Localization ' + name + ' has not been loaded.');
			}
		}
		return _currentLocale;
	};

	//quick function to override rule messages
	kv.localize = function (msgTranslations) {
		var rules = kv.rules;

		//loop the properties in the object and assign the msg to the rule
		for (var ruleName in msgTranslations) {
			if (rules.hasOwnProperty(ruleName)) {
				rules[ruleName].message = msgTranslations[ruleName];
			}
		}
	};

	// Populate default locale (this will make en-US.js somewhat redundant)
	(function() {
		var localeData = {};
		var rules = kv.rules;

		for (var ruleName in rules) {
			if (rules.hasOwnProperty(ruleName)) {
				localeData[ruleName] = rules[ruleName].message;
			}
		}
		kv.defineLocale('en-us', localeData);
	})();

	// No need to invoke locale because the messages are already defined along with the rules for en-US
	_currentLocale = 'en-us';
	;/**
	 * Possible invocations:
	 * 		applyBindingsWithValidation(viewModel)
	 * 		applyBindingsWithValidation(viewModel, options)
	 * 		applyBindingsWithValidation(viewModel, rootNode)
	 *		applyBindingsWithValidation(viewModel, rootNode, options)
	 */
	ko.applyBindingsWithValidation = function (viewModel, rootNode, options) {
		var node = document.body,
			config;

		if (rootNode && rootNode.nodeType) {
			node = rootNode;
			config = options;
		}
		else {
			config = rootNode;
		}

		kv.init();

		if (config) {
			config = extend(extend({}, kv.configuration), config);
			kv.utils.setDomData(node, config);
		}

		ko.applyBindings(viewModel, node);
	};

	//override the original applyBindings so that we can ensure all new rules and what not are correctly registered
	var origApplyBindings = ko.applyBindings;
	ko.applyBindings = function (viewModel, rootNode) {

		kv.init();

		origApplyBindings(viewModel, rootNode);
	};

	ko.validatedObservable = function (initialValue, options) {
		if (!options && !kv.utils.isObject(initialValue)) {
			return ko.observable(initialValue).extend({ validatable: true });
		}

		var obsv = ko.observable(initialValue);
		obsv.errors = kv.group(kv.utils.isObject(initialValue) ? initialValue : {}, options);
		obsv.isValid = ko.observable(obsv.errors().length === 0);

		if (ko.isObservable(obsv.errors)) {
			obsv.errors.subscribe(function(errors) {
				obsv.isValid(errors.length === 0);
			});
		}
		else {
			ko.computed(obsv.errors).subscribe(function (errors) {
				obsv.isValid(errors.length === 0);
			});
		}

		obsv.subscribe(function(newValue) {
			if (!kv.utils.isObject(newValue)) {
				/*
				 * The validation group works on objects.
				 * Since the new value is a primitive (scalar, null or undefined) we need
				 * to create an empty object to pass along.
				 */
				newValue = {};
			}
			// Force the group to refresh
			obsv.errors._updateState(newValue);
			obsv.isValid(obsv.errors().length === 0);
		});

		return obsv;
	};
	;}));


/***/ }),

/***/ 35:
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;var rvc = __webpack_require__(8);
	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9), __webpack_require__(22)], __WEBPACK_AMD_DEFINE_RESULT__ = function () {
	    ko.extenders.lotKey = function (target, options) {
	        var matchCallback, changedCallback;

	        if (typeof options === "function") {
	            matchCallback = options;
	            changedCallback = null;
	        } else if(options != undefined && typeof options === "object") {
	            matchCallback = options.matchCallback;
	            changedCallback = typeof options.changedCallback === "function" ? options.changedCallback : undefined;
	        }

		    var pattern = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{3})?(\s?)(\d{2}\d*)?$/,
		        newPattern = /^([01])?([0-9]{1})\s{0,}(\d{1,2})?\s{0,}(\d{1,3})?\s{0,}(\d{1,})?(.*)$/,
	            completePattern = /^(\d{2})(\s)(\d{2})(\s)(\d{3})(\s)(\d{2}\d*)$/,
	            isComplete = ko.observable(false);

		    target.formattedLot = ko.pureComputed({
				read: function () {
					return target();
				},
				write: function (value) {
					value = cleanInput(value);
					if (target.peek() === value) return;

					var formatted = formatAsLot(value);
					target(formatted);
				    if (formatted && formatted.match(completePattern)) {
				        isComplete(true);
				        if (typeof matchCallback === "function") matchCallback(formatted);
				        if (typeof changedCallback === "function") changedCallback(value, formatted);
				    } else changedCallback && changedCallback(value, undefined); // the second argument of the changedCallback is only returned when the lot key is complete

				}
			});
	        
			function cleanInput(input) {
				if (typeof input == "number") input = input.toString();
				if (typeof input !== "string") return undefined;
				return input.trim();
			}
			function formatAsLot(input) {
	            var re = /\d+/g,
	            newInput = input.match(re);

				if (newInput === undefined || newInput === null) { return; }
				newInput = newInput.join('');

				return newInput.replace(newPattern, function(match, p1, p2, p3, p4, p5, p6) {
				    if (p1) {
				        return [String(p1) + p2, p3, p4, p5].join(' ');
	                } else if (!p1 && (p2 === "0" || p2 === "1")) {
	                    return [p2, p3, p4, p5].join(' ');
	                } else {
	                    return [String(0) + p2, p3, p4, p5].join(' ');
	                }
	            }).trim();
			}

			target.match = function (valueToCompare) {
				var partialPattern = new RegExp('^' + target.formattedLot() + '$');
				return valueToCompare.match(partialPattern);
			};
			target.isComplete = ko.pureComputed(function () {
				return isComplete();
			}, target);
			target.Date = ko.pureComputed(function () {
			    var formattedLot = this.formattedLot();
				if (formattedLot) {
					var sections = formattedLot.split(" ");
					var days = parseInt(sections[2]);
					var defDate = "1/1/" + (parseInt(sections[1]) >= 90 ? "19" : "20");
					var date = new Date(defDate + sections[1]).addDays(days - 1);
					date.addMinutes(date.getTimezoneOffset());

					return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
				}
			}, target);
			target.formattedDate = ko.pureComputed(function () {
				var date = this();
				if (date && date != 'Invalid Date') return date.format("UTC:m/d/yyyy");
				return '';
			}, target.Date);
			target.LotType = ko.pureComputed(function () {
			  var lot = target.formattedLot();
			  if( lot ) {
			    var sections = lot.split(" ");
			    return Number(sections[0]);
			  }
			});
			target.InventoryTypeKey = ko.pureComputed(function () {
			  var lotType = target.LotType();
	      switch (lotType) {
	        case 1:
	        case 2:
	        case 3:
	        case 11:
	        case 12:
	          return rvc.lists.inventoryTypes.Chile.key;
	        case 4:
	          return rvc.lists.inventoryTypes.Additive.key;
	        case 5:
	          return rvc.lists.inventoryTypes.Packaging.key;
	        default:
	          return null;
			  }
			});
			target.Sequence = ko.pureComputed({
				read: function () {
					if (this.formattedLot()) {
						var sections = this.formattedLot().split(" ");
						if (sections.length === 4)
							return sections[3];
					}
				},
				write: function (newSeq) {
					var val = '';
					if (isComplete()) {
						var reg = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{3})?(\s?)/;
						val = this.formattedLot().match(reg)[0];
						val += newSeq < 10 ? '0' : '';
						val += newSeq;
						this.formattedLot(val);
					}
				}
			}, target);
			target.getNextLot = function () {
				var sequence = parseInt(target.Sequence());
				sequence++;
				if (sequence < 10) sequence = '0' + sequence;
				return target.formattedLot().replace(pattern, '0$2 $4 $6 ' + sequence);
			};

			target.extend({ throttle: 800 });

			target.formattedLot(target.peek());
			return target;
		};
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),

/***/ 36:
/***/ (function(module, exports, __webpack_require__) {

	(function (ko) {
	    ko.validation.init({
	        insertMessages: false,
	        decorateInputElement: true,
	        errorElementClass: 'has-error',
	        errorMessageClass: 'help-block'
	    });
	}(__webpack_require__(9)));

/***/ }),

/***/ 77:
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(7), __webpack_require__(8)], __WEBPACK_AMD_DEFINE_RESULT__ = function (core, app) {
	    var warehouseLocationsService = __webpack_require__(11);

	    var buildUrl = core.buildUrl,
	        ajax = core.ajax,
	        pagedDataHelper = core.pagedDataHelper;

	    return {
	        getInventoryPager: getInventoryPager,
	        getInventoryPagerWithTotals: getInventoryPagerWithTotals,
	        getPickableInventoryPager: getPickableInventoryPager,
	        getPickableInventory: getPickableInventory,
	        getInventory: getInventory,
	        getInventoryByLot: getInventoryByLot,
	        getInventoryByLotAndWarehouse: getInventoryByLotAndWarehouse,
	        getInventoryByTote: getInventoryByTote,
	        getChileProductsDataPager: getChileProductsDataPager,
	        getChileMaterials: getChileMaterials,
	        createChileProductReceivedRecord: createChileProductReceivedRecord,
	        updateChileProductReceivedRecord: updateChileProductReceivedRecord,
	        getDehydratedMaterials: getDehydratedMaterials,
	        getDehydratedMaterialsDataPager: getDehydratedMaterialsDataPager,
	        getDehydratedMaterialsReceived: getDehydratedMaterialsReceived,
	        getDehydrators: getDehydrators,
	        getMillAndWetdownPager: getMillAndWetdownPager,
	        getMillAndWetdownDetails: getMillAndWetdownDetails,
	        createMillAndWetdownEntry: createMillAndWetdownEntry,
	        updateMillAndWetdownEntry: updateMillAndWetdownEntry,
	        deleteMillAndWetdownEntry: deleteMillAndWetdownEntry,
	        getRinconWarehouseLocations: warehouseLocationsService.getRinconWarehouseLocations,
	        getWarehouseLocations: warehouseLocationsService.getWarehouseLocations,
	        receiveInventory: receiveInventory,
	        saveDehydratedMaterials: saveDehydratedMaterials,
	        updateDehydratedMaterials: updateDehydratedMaterials,
	        savePickedInventory: savePickedInventoryPublic,
	    };

	    //#region exports
	    function getInventoryPager(options) {
	        options = options || {};

	        return pagedDataHelper.init({
	            urlBase: options.baseUrl || "/api/inventory",
	            pageSize: options.pageSize || 50,
	            parameters: options.parameters,
	            onNewPageSet: options.onNewPageSet,
	            onEndOfResults: options.onEndOfResults
	        });
	    }
	    function getInventoryPagerWithTotals(options) {
	      options = options || {};

	      return pagedDataHelper.init({
	        urlBase: options.baseUrl || "/api/inventorytotals",
	        pageSize: options.pageSize || 50,
	        parameters: options.parameters,
	        onNewPageSet: options.onNewPageSet,
	        onEndOfResults: options.onEndOfResults,
	        resultCounter: function (data) {
	          data = data || {};
	          data.Items = data.Items || [];
	          return data.Items.length || 0;
	        }
	      });
	    }
	    function getPickableInventoryPager(pickingContext, contextKey, options) {
	        options = options || {};
	        if (!options.pageSize) options.pageSize = 100;
	        options.urlBase = buildInventoryPickingUrl(pickingContext, contextKey);
	        return pagedDataHelper.init(options);
	    }
	    function getPickableInventory(pickingContext, contextKey, params) {
	        var qs = $.param(params);
	        return core.ajax(buildInventoryPickingUrl(pickingContext, contextKey) + '?' + qs);
	    }
	    function savePickedInventoryPublic(pickingContext, contextKey, values) {
	        return core.ajaxPost(buildInventoryPickingUrl(pickingContext, contextKey), values);
	    }
	    function buildInventoryPickingUrl(pickingContext, contextKey) {
	        return '/api/inventory/pick-' + pickingContext.value.toLowerCase() + '/' + contextKey;
	    }
	    function getInventory(/* productType, lotType, productSubType, productKey, warehouseKey */) {
	        var args = [];
	        for (var a in arguments) {
	            args.push(arguments[a]);
	        }
	        var options = args.pop();
	        return ajax(buildUrl(buildInventoryUrl, args), options);
	    }
	    function getInventoryByLot(lotNumber, options) {
	        return ajax(buildUrl(buildInventoryUrl, lotNumber), options);
	    }
	    function getInventoryByLotAndWarehouse(lotNumber, warehouseKey, options) {
	        return ajax(buildUrl(buildLotInventoryByWarehouseUrl, lotNumber, warehouseKey), options);
	    }
	    function getInventoryByTote(toteKey, options) {
	        return ajax("/api/toteinventory/" + toteKey, options);
	    }
	    function getMillAndWetdownPager(options) {
	      options = options || {};

	      return pagedDataHelper.init({
	        urlBase: options.baseUrl || "/api/millwetdown",
	        pageSize: options.pageSize || 50,
	        parameters: options.parameters,
	        onNewPageSet: options.onNewPageSet,
	      });
	    }
	    function getMillAndWetdownDetails(key) {
	      return ajax('/api/millwetdown/' + (key || ''));
	    }
	    function createMillAndWetdownEntry(entryData) {
	      return core.ajaxPost("/api/millwetdown/", entryData);
	    }
	    function updateMillAndWetdownEntry(lotNumber, entryData) {
	      return core.ajaxPut("/api/millwetdown/" + lotNumber, entryData);
	    }
	    function deleteMillAndWetdownEntry(lotNumber, entryData) {
	      return core.ajaxDelete( "/api/millwetdown/" + lotNumber );
	    }
	    function getDehydrators() {
	      var dfd = $.Deferred();

	      ajax('/api/companies').done(function(data, textStatus, jqXHR) {
	        var dehydrators = ko.utils.arrayFilter(data, function(company) {
	          if (company.CompanyTypes.indexOf(app.lists.companyTypes.Dehydrator.key) >= 0) {
	            return true;
	          }
	        });

	        dfd.resolve(dehydrators);
	      })
	      .fail(function(jqXHR, textStatus, errorThrown) {
	      });

	      return dfd;
	    }
	    function getChileProductsDataPager(options) {
	        options = options || {};

	        return pagedDataHelper.init({
	            urlBase: options.baseUrl || "/api/chilereceived",
	            pageSize: options.pageSize || 50,
	            parameters: options.parameters,
	            onNewPageSet: options.onNewPageSet,
	        });
	    }
	    function getDehydratedMaterialsDataPager(options) {
	        options = options || {};

	        return pagedDataHelper.init({
	            urlBase: options.baseUrl || "/api/dehydratedmaterialsreceived",
	            pageSize: options.pageSize || 50,
	            parameters: options.parameters,
	            onNewPageSet: options.onNewPageSet,
	        });
	    }
	    function getChileMaterials( lotKey ) {
	      return core.ajax( '/api/chilereceived/' + lotKey );
	    }
	    function createChileProductReceivedRecord( productData ) {
	      return core.ajaxPost( '/api/chilereceived', productData );
	    }
	    function updateChileProductReceivedRecord( productKey, productData ) {
	      return core.ajaxPut( '/api/chilereceived/' + productKey, productData );
	    }
	    function getDehydratedMaterials(lot) {
	      return ajax('/api/dehydratedmaterialsreceived/' + (lot || ''));
	    }
	    function getDehydratedMaterialsReceived() { /* optional parameters: startDate, endDate */
	        var args = [];
	        for (var a in arguments) {
	            args.push(arguments[a]);
	        }
	        var options = args.pop();
	        var url = "/api/dehydratedmaterialsreceived";
	        if (args.length > 0) {
	            url += buildQueryString(args[0]);
	        }

	        ajax(url, options);
	    }
	    function saveDehydratedMaterials(data) {
	      return core.ajaxPost('/api/dehydratedmaterialsreceived', data);
	    }
	    function updateDehydratedMaterials(key, data) {
	      return core.ajaxPut('/api/dehydratedmaterialsreceived/' + key, data);
	    }
	    function receiveInventory(values) {
	        var url = buildLotInventoryUrl();
	        return core.ajaxPost(url, values);
	    }
	    //#endregion exports

	    function buildLotInventoryUrl(lotNumber) {
	        return "/api/inventory/" + lotNumber;
	    }
	    function buildLotInventoryByWarehouseUrl(lotNumber, warehouseKey) {
	        return buildLotInventoryUrl(lotNumber) + "?warehouseKey=" + warehouseKey;
	    }
	    function buildInventoryUrl(productType) {
	        return "/api/inventory/" + (productType || 2);
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 78:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {ko.bindingHandlers.hidden = {
	    update: function(element, valueAccessor) {
	        ko.bindingHandlers.visible.update(element, function() {
	            return !ko.utils.unwrapObservable(valueAccessor());
	        });
	    }
	}
	ko.bindingHandlers.preventBubble = {
	    init: function (element, valueAccessor) {
	        var eventName = ko.utils.unwrapObservable(valueAccessor());
	        ko.utils.registerEventHandler(element, eventName, function (event) {
	            event.cancelBubble = true;
	            if (event.stopPropagation) {
	                event.stopPropagation();
	            }
	        });
	    }
	};

	ko.bindingHandlers.dialog = {
	    init: function (element, valueAccessor, allBindings, bindingContext) {
	        var $element = $(element),
	            commands = {};

	        var defaultConfig = {
	            modal: true,
	        };

	        commands = parseCommands(allBindings());
	        var value = typeof valueAccessor === "function" ? valueAccessor() : valueAccessor;
	        var config = buildConfiguration(allBindings());
	        $element.dialog(config);
	        var dialogDestroyed = false;

	        if (ko.isObservable(value)) {
	            var valueSubscriber = value.subscribe(function (val) {
	                if (val == undefined) ko.cleanNode(element);
	                else if(!dialogDestroyed) $element.dialog(val ? "open" : "close");
	            });

	            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                if($element.dialog('option')) $element.dialog("destroy");
	                dialogDestroyed = true;
	                valueSubscriber.dispose && valueSubscriber.dispose();
	            });
	        }

	        attachKoCommands($element, commands);
	        ko.bindingHandlers.cancelKey.init(element, ko.utils.wrapAccessor(function () {
	            if (commands['Cancel']) commands['Cancel'].execute();
	            else $element.dialog('close');
	        }));
	        
	        function buildConfiguration(bindings) {
	            var modal = bindings.modal || defaultConfig.modal;

	            // Creates empty function for any configured buttons. 
	            // This will cause the buttons to be displayed while allowing
	            // execution to be deferred to the supplied command object.

	            var config = {
	                modal: modal,
	                height: bindings.height || defaultConfig.height,
	                width: bindings.width || defaultConfig.width,
	                position: bindings.position || defaultConfig.position,
	                buttons: {
	                    Ok: bindings.okCommand ? function() {} : undefined,
	                    Cancel: bindings.cancelCommand ? function() {} : undefined,
	                },
	                close: bindings.close || bindings.cancelCommand,
	                title: ko.utils.unwrapObservable(bindings.title),
	                autoOpen: ko.utils.unwrapObservable(value) && true || false,
	                dialogClass: bindings.cancelCommand ? 'no-close' : '',
	            };

	            if (bindings.customCommands) {
	                var customCommands = getCustomCommands(bindings);
	                ko.utils.arrayForEach(customCommands, function (command) {
	                    for (var prop in command) {
	                        config.buttons[prop] = empty;
	                    }
	                });
	            }

	            function empty() {}

	            return config;
	        }

	        function getCustomCommands(bindings) {
	            var customCommands = bindings.customCommands || [];
	            if (customCommands.length == undefined) {
	                var temp = customCommands;
	                customCommands = [];
	                customCommands.push(temp);
	            }
	            return customCommands;
	        }

	        function parseCommands(bindings) {
	            bindings = bindings || {};
	            var commands = {};
	            commands = parseCommand(bindings, 'cancelCommand', commands, 'Cancel');
	            commands = parseCommand(bindings, 'okCommand', commands, 'Ok');

	            var customCommands = getCustomCommands(bindings);
	            $.each(customCommands, function (index) {
	                for (var cmdName in customCommands[index]) {
	                    commands = parseCommand(customCommands[index], cmdName, commands);
	                }
	            });
	            return commands;
	        }

	        function parseCommand(bindings, bindingName, commandBindings, mapToCommandName) {
	            mapToCommandName = mapToCommandName || bindingName;
	            if (bindings[bindingName]) {
	                var cmd = bindings[bindingName];
	                if (cmd.execute == undefined) {
	                    cmd = ko.command({
	                        execute: cmd
	                    });
	                }
	                commandBindings[mapToCommandName] = cmd;
	            }
	            return commandBindings;
	        }

	        function attachKoCommands($e, commands) {
	            var buttonFunctions = $e.dialog("option", "buttons");
	            var newButtonsConfig = [];
	            for (var funcName in buttonFunctions) {
	                for (var cmdName in commands) {
	                    if (cmdName == funcName) {
	                        //todo: replace contains with eq? 
	                        var buttons = $(".ui-dialog-buttonpane button:contains('" + cmdName + "')");

	                        $.each(buttons, function (index) {
	                            var command = commands[cmdName];

	                            ko.bindingHandlers.command.init(
	                                buttons[index],
	                                ko.utils.wrapAccessor(command),
	                                allBindings,
	                                bindingContext);

	                            // remove click functionality from the jQuery UI element
	                            newButtonsConfig.push({
	                                text: cmdName,
	                                click: empty,
	                            });
	                        });
	                        break;
	                    }
	                }
	            }

	            function empty() {}
	        }
	    }
	};

	ko.bindingHandlers.cancelKey = {
	    init: function (element, valueAccessor, allBindings, viewModel) {
	        var delegate = ko.utils.unwrapObservable(valueAccessor());

	        if (delegate && typeof delegate !== 'function' && typeof delegate.execute === "function") {
	            delegate = delegate.execute;
	        }
	        if (delegate == undefined) return;

	        var cancelKeyCode = 27;
	        var elementToRegister = element;

	        if (attachToWindow()) {
	            var conditionFn = function () { return $(element).is(':visible'); };
	            elementToRegister = window;
	        }
	        ko.utils.registerEventHandler(elementToRegister, 'keydown', buildEventHandler(conditionFn));
	        
	        function attachToWindow() {
	            var bindings = ko.utils.unwrapObservable(allBindings);
	            return bindings && ko.utils.unwrapObservable(bindings.attachToWindow) === true;
	        }

	        function buildEventHandler(conditionalFn) {
	            conditionalFn = conditionalFn || function() { return true; };
	            return function (event) {
	                if (event.keyCode == cancelKeyCode && conditionalFn()) {
	                    executeCancel(event);
	                    event.cancelBubble = true;
	                    if (event.stopPropagation) event.stopPropagation();
	                }
	            };
	        }

	        function executeCancel(event) {
	            event.preventDefault();
	            event.target.blur();
	            delegate.call(viewModel);
	        }
	    }
	};

	ko.bindingHandlers.onblur = {
	    init: function(element, valueAccessor) {
	        var fn = valueAccessor();
	        if (fn && fn.execute) fn = commandWrapper.bind(fn);
	        ko.utils.registerEventHandler(element, 'blur', fn);
	        
	        function commandWrapper() {
	            this.execute();
	        }
	    }
	};

	ko.bindingHandlers.maxHeight = {
	    init: function (element, valueBinding, allBindings) {
	        var $element = $(element);
	        $element.addClass('maxHeight-container');
	        var resize = ko.bindingHandlers.maxHeight.constrainHeight.bind($element);
	        resize();
	        //$('body *').scroll(resize); // this was added to catch scrolling events on the popupWindow and should be done more efficiently
	        $(window).scroll(resize);

	        // display full container when the user starts to scroll content
	        var lastScrollTop = 0; 
	        $element.scroll(function () {
	            var scrollTop = $element.scrollTop();
	            if (respondToScroll()) {
	                var offset = $element.offset();
	                // warning: this will not work correctly with popupWindow because of the window's scroll position which is irrelevant to the popup window - vk 1/1/2014
	                if (offset.top !== $(window).scrollTop()) {
	                    $('html, body').animate({
	                        scrollTop: offset.top
	                    });
	                }
	            }
	            lastScrollTop = scrollTop;

	            function respondToScroll() {
	                return scrollTop !== lastScrollTop;
	            }
	        });


	        ko.bindingHandlers.maxHeight.addStickyHeaderToTables($element, allBindings());
	    },
	    update: function (element) {
	        ko.bindingHandlers.maxHeight.constrainHeight.call($(element));
	    },
	    constrainHeight: function() {
	        var windowHeight = $(window).height();
	        var top = this[0].getBoundingClientRect().top;
	        var elHeight = Math.max(top, 0);
	        this.css('max-height', windowHeight - elHeight);
	        this.css('overflow-y', 'scroll');
	        this.css('overflow-x', 'scroll');
	    },
	    addStickyHeaderToTables: function (element, allBindings) {
	        var $element = $(element);
	        var opts = allBindings || {};

	        var template = getTemplatedChild();
	        if (template) {
	            if (!allBindings.stickyTableHeaders) return; 
	            var value = ko.utils.wrapAccessor(allBindings.stickyTableHeaders);
	            opts.dependsOn = template;
	            var bindings = ko.utils.wrapAccessor(opts);
	            ko.bindingHandlers.stickyTableHeaders.init(template, value, bindings);
	        } else {
	            opts.parent = $element;
	            $element.find(allBindings.stickyTableHeaders || 'table').each(function () {
	                var $this = $(this);
	                ko.bindingHandlers.stickyTableHeaders.init($this, ko.utils.wrapAccessor(true), ko.utils.wrapAccessor(opts));
	                removeStickyTableBinding($this);
	            });
	        }
	        
	        function getTemplatedChild() {
	            var child = getChild();
	            if (!child) return null;
	            var childContext = ko.contextFor(child);
	            if (!childContext) return null;

	            var childBindings = ko.bindingProvider.instance.getBindings(child, childContext);
	            return childBindings && childBindings.template
	                ? child : null;

	            function getChild() {
	                return $element.children(':first')[0]
	                    || getVirtualElementChild();
	            }
	            function getVirtualElementChild() {
	                var vChild = ko.virtualElements.firstChild($element[0]);
	                return vChild && ko.virtualElements.nextSibling(vChild);
	            }
	        }

	        function removeStickyTableBinding(table) {
	            var dataBind = table.attr('data-bind');
	            if (dataBind) {
	                dataBind = dataBind.replace(/stickyTableHeaders\:\s?\w+\W?\s?/, "");
	                table.attr('data-bind', dataBind);
	            }
	        }
	    }
	};

	ko.bindingHandlers.fixCvpOverlay = {
	    init: function (element, valueAccessor) {
	        var $container = $(element).wrap('<div />').parent();
	        //ko.bindingHandlers.fixCvpOverlay.update(element,valueAccessor);
	    },
	    update: function (element, valueAccessor) {
	        valueAccessor().notifySubscribers(); // fix initial overlay
	        ko.utils.unwrapObservable(valueAccessor());
	        var $cvp = $("#cvp");
	        var cvpWidth = $cvp.outerWidth();
	        var $element = $(element);
	        var inventoryTableWidth = $element.width();

	        // When element contains an enumerated child (such as a foreach binding), the
	        // width function returns 0. This hacky little fix will set a default width.
	        if (inventoryTableWidth == 0) {
	            inventoryTableWidth = 5000; // default width
	        }

	        var $container = $element.parent();
	        $container.width(inventoryTableWidth).css({ "padding-right": cvpWidth + 85 });
	    }
	};

	ko.bindingHandlers.undo = {
	    init: function (element, valueAccessor, allBindings, viewModel) {
	        var bindings = {};
	        var trackedBindingNames = ['value'];
	        var isEditing = ko.computed(function () {
	            return ko.utils.unwrapObservable(valueAccessor());
	        });
	        var elementBindings = ko.bindingProvider.instance.getBindings(element, ko.contextFor(element));
	        ko.utils.arrayForEach(trackedBindingNames, function (binding) {
	            if (elementBindings[binding]) {
	                bindings[binding] = elementBindings[binding];
	            }
	        });

	        for (var boundProp in bindings) {
	            initializeTracking(bindings[boundProp]);
	        }

	        function revert(propAccessor) {
	            var initalValue = propAccessor.changeHistory()[0];
	            propAccessor(initalValue);
	        }

	        function initializeTracking(propAccessor) {
	            propAccessor.changeHistory = ko.observableArray([propAccessor()]);

	            propAccessor.subscribe(function (newVal) {
	                propAccessor.changeHistory.push(newVal);
	            });

	            setupRevertTrigger(propAccessor);
	        }

	        isEditing.subscribe(function (newVal) {
	            $cancelButton.each(function (index, button) {
	                ko.bindingHandlers.visible.update(
	                    button,
	                    ko.utils.wrapAccessor(newVal),
	                    allBindings,
	                    data
	                );
	            });
	        });

	        function setupRevertTrigger(propAccessor) {
	            // eventually, we'll enable actual undo/redo stepping but for now, we just 
	            // handle both as a revert function.
	            var revertCommand = ko.command({
	                execute: function () {
	                    revert(propAccessor);
	                },
	                canExecute: function () {
	                    return propAccessor.changeHistory().length > 1;
	                }
	            });
	            propAccessor.revertCommand = revertCommand;
	            var trigger = allBindings().undoTrigger || allBindings().revertTrigger;

	            $(trigger).each(function (index, button) {
	                ko.bindingHandlers.command.init(
	                    button,
	                    ko.utils.wrapAccessor(revertCommand),
	                    allBindings,
	                    viewModel
	                );
	            });
	        }
	    }
	};

	ko.bindingHandlers.pageData = {
	    update: function (element, valueAccessor) {
	        ko.utils.unwrapObservable(valueAccessor());
	        $(element).hide().fadeIn(500);
	    }
	};

	ko.bindingHandlers.editableContent = {
	    init: function (element, valueAccessor, allBindings, data) {
	        var savedState = ko.observable();
	        var isEditing = ko.computed(function () {
	            return ko.utils.unwrapObservable(valueAccessor());
	        });
	        var $element = $(element);
	        var $cancelButton = $(allBindings().cancelTrigger);
	        var $masterCancelButton = $(allBindings().masterCancelTrigger);

	        if (!isEditing()) { $element.attr("readonly", "readonly"); }

	        ko.bindingHandlers.undo.init(
	            element,
	            ko.utils.wrapAccessor(function () { return true; }),
	            ko.utils.wrapAccessor({ revertTrigger: $cancelButton ? $cancelButton[0] : undefined }),
	            data);

	        ko.bindingHandlers.click.init(
	            element,
	            ko.utils.wrapAccessor(beginEdit),
	            ko.utils.wrapAccessor({}),
	            data);

	        //todo: handle blur events (and allow disabling the blur handlers)

	        //todo: 1. prevent bubbling, 2. enable canceling when cancel button is not supplied
	        ko.bindingHandlers.cancelKey.init(
	            element,
	            ko.utils.wrapAccessor(function () { $cancelButton.click(); }),
	            ko.utils.wrapAccessor({ keydownBubble: false }),
	            data
	        );


	        setupCancelButtons();

	        function setupCancelButtons() {
	            var cancelCommand = allBindings().cancelEditsCommand;
	            if ($cancelButton.length > 0) {
	                $cancelButton.each(function (index, button) {
	                    ko.bindingHandlers.command.init(
	                        button,
	                        ko.utils.wrapAccessor(cancelCommand),
	                        function () { return { clickBubble: false }; },
	                        data);
	                });
	            }

	            $masterCancelButton.each(function (index, button) {
	                var context = ko.contextFor(button);
	                if (context) {
	                    var commandBinding = ko.bindingProvider.instance.getBindings(button, context).command;
	                    if (commandBinding) {
	                        if (typeof commandBinding.addCommand != "function") {
	                            throw new Error('The masterCancelCommand is only supported with a composableCommand instance.');
	                        }

	                        commandBinding.addCommand(ko.command({
	                            execute: function () { $cancelButton.click(); }
	                        }));
	                    }
	                }
	            });
	        }

	        function beginEdit() {
	            if (isEditing()) return;
	            valueAccessor()(true);
	            var bindings = ko.bindingProvider.instance.getBindings(element, ko.contextFor(element));
	            savedState(ko.utils.unwrapObservable(bindings.value));
	        }
	    },
	    update: function (element, valueAccessor) {
	        var isEditing = ko.utils.unwrapObservable(valueAccessor());
	        if (isEditing === false) {
	            $(element).attr("readonly", "readonly");
	        } else {
	            $(element).removeAttr("readonly");
	        }
	    }
	};

	ko.bindingHandlers.editableContentArea = {
	    init: function (element, valueAccessor, allBindings, data) {
	        var $element = $(element);
	        var isEditing = ko.computed(function () {
	            return ko.utils.unwrapObservable(valueAccessor());
	        });

	        function setIsEditingValue(val) {
	            valueAccessor()(val);
	        }
	        var inputElements = $('input', $element).not('[type="button"], [type="submit"]');
	        var allChildrenEmpty = ko.computed(function () {
	            var firstDefinedValue = ko.utils.arrayFirst(inputElements, function (e) {
	                var ctx = ko.contextFor(e);
	                var binding = ctx ? ko.bindingProvider.instance.getBindings(e, ctx) : undefined;
	                var value = binding ? binding.value() : undefined;
	                return value != undefined;
	            });
	            return firstDefinedValue === null;
	        });
	        var isCancelVisible = ko.computed(function () {
	            return isEditing() && !allChildrenEmpty();
	        });

	        valueAccessor().__editableContentArea__inputElements = inputElements;

	        var cancelValueAccessor = ko.utils.wrapAccessor(function () { return false; });
	        cancelValueAccessor().__editableContentArea__inputElements = inputElements;

	        var cancelCommand = ko.command({
	            execute: function () {
	                setIsEditingValue(false);
	            },
	        });

	        // currently requires cancelTrigger binding to be provided
	        var $cancelButton = $element.find(allBindings().cancelTrigger);

	        // initialize visibility
	        updateCancelButtonVisibility();

	        // update visibility
	        isCancelVisible.subscribe(function () {
	            updateCancelButtonVisibility();
	        });

	        allBindings().cancelTrigger = $cancelButton ? $cancelButton[0] : undefined;
	        allBindings().cancelEditsCommand = cancelCommand;

	        $.each(inputElements, function (index, elem) {
	            ko.bindingHandlers.editableContent.init(elem, valueAccessor, allBindings, data);
	        });

	        function updateCancelButtonVisibility() {
	            $cancelButton.each(function (index, button) {
	                ko.bindingHandlers.visible.update(
	                    button,
	                    function () { return isCancelVisible(); },
	                    allBindings,
	                    data
	                );
	            });
	        }
	    },
	    update: function (element, valueAccessor, allBindings, data) {
	        var inputElements = valueAccessor().__editableContentArea__inputElements;
	        var isEditing = ko.utils.unwrapObservable(valueAccessor());

	        $.each(inputElements, function (index, elem) {
	            ko.bindingHandlers.editableContent.update(
	                elem,
	                ko.utils.wrapAccessor(isEditing),
	                allBindings,
	                data
	            );
	        });
	    },
	};

	ko.bindingHandlers.slideVisible = {
	    init: function (element, valueAccessor) {
	        var display = ko.utils.unwrapObservable(valueAccessor());
	        if (display) {
	            $(element).hide();
	            $(element).slideDown();
	        } else {
	            $(element).slideUp();
	        }
	    },
	    update: function (element, valueAccessor, allBindings) {
	        var display = ko.utils.unwrapObservable(valueAccessor());
	        var defaults = {
	            showDuration: "slow",
	            hideDuration: "slow",
	            speed: false,
	            direction: "down",
	        };
	        var options = $.extend(defaults, allBindings());
	        if (options.speed) options.showDuration = options.hideDuration = options.speed;

	        if (display) {
	            $(element).slideDown(options.showDuration);
	        } else {
	            $(element).slideUp(options.hideDuration);
	        }
	    }
	};

	ko.bindingHandlers.popup = {
	    init: function(element, valueAccessor, allBindings) {
	        var $element = $(element);
	        $element.addClass('popupWindow');

	        var defaults = {
	            attachCancelCommandToWindow: true,
	        };
	        var options = $.extend({}, defaults, allBindings());
	        var borderWidth = parseInt($element.css('border-left-width'), 10) || 10; // parseInt trims the 'px' and returns base-10 value

	        $(element).on('click', onCloseEvent);

	        if (options.closePopupCommand) {
	            var cancelKeyOptions = options;
	            cancelKeyOptions.attachToWindow = options.attachCancelCommandToWindow;
	            ko.bindingHandlers.cancelKey.init(element, ko.utils.wrapAccessor(options.closePopupCommand), ko.utils.wrapAccessor(cancelKeyOptions));
	        }

	        ko.bindingHandlers.slideIn.init(element, valueAccessor);
	        
	        // handle cleanup
	        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	            $element.off('click', onCloseEvent);
	        });
	        
	        // private functions
	        function onCloseEvent(e) {
	            var hitAreaX = borderWidth + $element.position().left;
	            // Reece - Replaced pageX with screenX b/c when
	            // clicking a selectBox,  pageX value is relative to 
	            // selectbox -- not the page.
	            if (e.pageX && e.pageX <= hitAreaX) {
	                if (options.closePopupCommand && typeof options.closePopupCommand.execute == "function") {
	                    options.closePopupCommand.execute();
	                    return;
	                }
	                if (ko.isWriteableObservable(valueAccessor())) {
	                    valueAccessor()(false);
	                    return;
	                }
	                ko.bindingHandlers.popup.update(element, ko.utils.wrapAccessor(false), allBindings);
	            }
	        }
	    },
	    update: function(element, valueAccessor, allBindings) {
	        ko.bindingHandlers.slideIn.update(element, valueAccessor, allBindings);
	    }
	};

	ko.bindingHandlers.slideIn = {
	    init: function (element, valueAccessor) {
	        var display = ko.utils.unwrapObservable(valueAccessor());

	        var $element = $(element);
	        $element.show();
	        if (!display) {
	            $(element).css({ left: $(window).width()});
	        }
	    },
	    update: function (element, valueAccessor) {
	        var $element = $(element);
	        var display = ko.utils.unwrapObservable(valueAccessor());
	        if (display) $element.animate({ left: 0 });
	        else $element.animate({ left: "100%" });
	    }
	};

	ko.bindingHandlers.fadeVisible = {
	    init: function (element) {
	        $(element).hide();
	    },
	    update: function (element, valueAccessor) {
	        var value = ko.utils.unwrapObservable(valueAccessor());
	        if (value) $(element).fadeIn();
	        else  $(element).fadeOut();
	    }
	};

	ko.bindingHandlers.stickyTableHeaders = {
	    init: function (element, valueAccessor, allBindings) {
	        var value = ko.utils.unwrapObservable(valueAccessor());
	        var bindings = allBindings();
	        var $table;
	        var options = {};
	        var completed = false;
	        var $element = $(element);

	        options.tabs = bindings.tabbedParent;
	        options.myTab = bindings.myTab;

	        if (typeof value === "string") {
	            $table = $element.find(value);
	            options.parent = $element;
	        } else {
	            $table = $(element);
	            options.parent = bindings.parent;
	        }

	        if ($table == undefined) throw new Error("The table element can not be found. Selector: '" + value + "'.");
	        
	        if (bindings.rebuildTrigger) {
	            if (!ko.isObservable(bindings.rebuildTrigger))
	                throw new Error("Invalid binding: \"rebuildTrigger\". Must be observable object.");

	            bindings.rebuildTrigger.subscribe(function () {
	                stickyHeaders($element.find(value), options);
	            }, null, 'rendered');
	        }

	        bindTable();
	        
	        function bindTable() {
	            //Enables the jQuery transformation to be deferred until after the dependent object has data
	            var dependsOn = bindings['dependsOn'];
	            if (dependsOn && deferToDependency()) {
	                return;
	            }

	            function deferToDependency() {
	                var $dependency = typeof dependsOn === "string"
	                    ? $element.children(':first')
	                    : $(dependsOn);
	                if (!$dependency) return false;

	                var dependencyElement = $dependency[0];
	                var dependencyContext = ko.contextFor(dependencyElement);
	                var dependencyBindings = ko.bindingProvider.instance.getBindings(dependencyElement, dependencyContext);

	                if (dependencyHasTemplate()) {
	                    var fnName = '__stickyTableHeaders__updateHeaders__';
	                    if (isVirtualElement()) {
	                        dependsOn.data = attachAfterRenderBinding.call(dependsOn.data);
	                        dependencyContext.$data[fnName] = function () {
	                            var table = typeof (value) === "string" 
	                                ? $(arguments[0]).filter(value) || $element.find(value)
	                                : value;

	                            if (!table.length) {
	                                console.error("The table element could not be found. When attaching stickyTableHeaders within template, the value parameter should contain a selector for the table.");
	                                return;
	                            }

	                            var context = ko.contextFor(dependsOn);
	                            var theadDependency = bindings.stickyTableHeaderDependency;
	                            if (typeof theadDependency === "string") theadDependency = context.$data[theadDependency];
	                            options.parent = table;
	                            if (ko.isObservable(theadDependency)) {
	                                theadDependency.subscribe(function() {
	                                    stickyHeaders(table, options);
	                                });
	                            } else {
	                                stickyHeaders(table, options);
	                            }
	                        };
	                    } else {
	                        var binding = $dependency.attr('data-bind').each(attachAfterRenderBinding);
	                        $dependency.attr('data-bind', binding);
	                        dependencyContext.$data[fnName] = function () {
	                            stickyHeaders($element.find(value), options);
	                        };
	                    }
	                    return true;

	                }

	                return false;

	                function isVirtualElement() {
	                    return dependencyElement.nodeType === 8
	                        && ko.virtualElements.virtualNodeBindingValue(dependsOn);
	                }
	                function dependencyHasTemplate() {
	                    return dependencyBindings && dependencyBindings.template;
	                }
	                function attachAfterRenderBinding() {
	                    return this.replace(/(template\:\s?\{)/, "$1" + 'afterRender:' + fnName + ',');
	                }
	            }
	            
	            stickyHeaders($table, options);
	        }
	        
	        function stickyHeaders(table, opts) {
	            table.each(function () {
	                if (!this.tagName || this.tagName.toLowerCase() !== 'table') {
	                    throw new Error("The bound element is not a table element. Element selector: '" + value + "'");
	                }
	            });
	            table.stickyTableHeaders(opts);
	            completed = true;
	        }
	    }
	};

	ko.bindingHandlers.stickyTableFooters = {
	    init: function (element) {
	        $(element).stickyTableFooters();
	    },
	    update: function (element) {
	        $(element).stickyTableFooters();
	    }
	};

	ko.bindingHandlers.tooltip = {
	    init: function (element, bindingAccessor, allBindings) {
	        var value = ko.utils.unwrapObservable(bindingAccessor()),
	            bindings = allBindings && allBindings() || {};

	        if (typeof value == "number") value = value.toString();
	        if (!value || value.length == 0) return;

	        var $element = $(element);
	        $element.attr('title', value);
	        $element.tooltip({
	            track: bindings.tooltipTrack,
	        });
	        //todo: enable updates to the tooltip value
	    },
	};

	ko.bindingHandlers.datePicker = {
	    init: function (element, valueAccessor, allBindings) {
	        $(element).wrap('<div class="input-group"></div>');
	        $(element).datepicker({
	            showOn: 'button',
	            buttonText: '<i class="fa fa-calendar"></i>',
	            changeMonth: true,
	            changeYear: true
	        }).next(".ui-datepicker-trigger")
	            .addClass("btn btn-default")
	            .wrap('<span class="input-group-btn"></span>');

	        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	            //todo: cleanup wrapper element
	            $(element).datepicker('destroy');
	        });
	    }
	};;

	ko.bindingHandlers.sortableTable = {
	    init: function(element, onMoved) {
	        $(element).sortable({
	            placeholder: "ui-sortable-highlight",
	            forcePlaceHolderSize: true,
	            start: function(event, ui) {
	                var colspan = ui.item.parents(1).find("th").length;
	                ui.placeholder.html("<td colspan='" + colspan + "'></td>");
	            },
	            stop: onMoved(),
	            helper: function(e, ui) {
	                ui.children().each(function() {
	                    $(this).width($(this).width());
	                });
	                return ui;
	            }
	        }).disableSelection();
	    }
	};


	ko.bindingHandlers.autoHeightTextarea = {
	    init: function(element, valueAccessor) {
	    },
	    update: function(element, valueAccessor) {
	        element.style.height = '0';
	        element.style.height = element.scrollHeight + 'px';
	    }
	};

	// autocomplete: listOfCompletions
	ko.bindingHandlers.autocomplete = {
	    init: function (element, valueAccessor, allBindings) {
	        var optionValues = ko.utils.arrayMap(ko.unwrap(valueAccessor()), function (c) {
	            if (c.Name && !c.label) c.label = c.Name;
	            return c;
	        });
	        var opts = ko.unwrap(allBindings().autocompleteOptions || {});
	        opts = $.extend(opts, {
	            source: optionValues,
	            minLength: 0,
	            change: function (e, ui) {
	                var bindingContext = ko.contextFor(element);
	                if (!bindingContext) return;
	                var bindings = ko.bindingProvider.instance.getBindings(element, bindingContext) || {};
	                if (!bindings.value) return;

	                if (ui.item && ui.item.value) {
	                    bindings.value(ui.item.value);
	                }
	                    // enable new elements to be added to the list
	                else if (opts.allowNewValues) bindings.value($(this).val());
	                else {
	                    bindings.value(null);
	                    if (ko.DEBUG) {
	                        console.log('The selected value was not found in the options list. To allow new values, include the \"allowNewValues=\'true\'\" value in the \"autocompleteOptions\" binding attribute.');
	                    }
	                }
	            }
	        });
	        $(element).autocomplete(opts);
	    },
	    update: function (element, valueAccessor) {
	        var completions = ko.utils.arrayMap(ko.utils.unwrapObservable(valueAccessor()), function (c) {
	            if (c.Name && !c.label) c.label = c.Name;
	            return c;
	        });
	        $(element).autocomplete("option",{
	            source: completions
	        });
	    }
	};

	ko.bindingHandlers.tabs = {
	    init: function (element, valueAccessor, allBindings) {
	        var $element = $(element);
	        var value = ko.utils.unwrapObservable(valueAccessor());
	        $(element).val(value);
	        
	        var options = ko.utils.unwrapObservable(allBindings().tabOptions) || {};
	        
	        $element.on("tabsactivate", onTabActivate);
	        $element.on("tabscreate", onTabCreate);
	        
	        $element.tabs(options);

	        ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
	            $element.tabs("destroy");
	            $element.off('tabsactivate', onTabActivate);
	            $element.off('tabscreate', onTabCreate);
	        });


	        function onTabActivate(event, ui) {
	            bindValueFromUI(ui.newTab.text(), ui.newPanel[0]);
	        }
	        
	        function onTabCreate(event, ui) {
	            bindValueFromUI(ui.tab.text(), ui.panel[0]);
	        }

	        function bindValueFromUI(name, panel) {
	            if (!ko.isObservable(valueAccessor())) return;
	            valueAccessor()({
	                name: name,
	                data: getDataBoundObjectFor(panel)
	            });
	        }

	        function getDataBoundObjectFor(tabPanel) {
	            if (!tabPanel) return undefined;
	            var panelContext = ko.contextFor(tabPanel);
	            if (!panelContext) return undefined;
	            var bindings = ko.bindingProvider.instance.getBindings(tabPanel, panelContext) || {};
	            return bindings.with || panelContext.$data;
	        }
	    },
	};

	ko.bindingHandlers.ajaxStatus = {
	    init: function(element, valueAccessor) {
	        var value = valueAccessor();
	        if (value.ajaxSuccess == undefined
	            || value.ajaxFailure == undefined
	            || value.ajaxWorking == undefined) throw new Error("The bound value is not valid for use with the ajaxStatus binding.");

	        ko.applyBindingsToNode(element, {
	            css: {
	                working: value.ajaxWorking,
	                success: value.ajaxSuccess,
	                fail: value.ajaxFailure,
	                ajaxStatus: true,
	            }
	        });
	    }
	};


	// Dragons be here...
	// allows up/down arrows, mouse-click dragging, 
	// and mouse-click wheel
	// accepts property 'negative'in allBindings to allow negative numbers
	ko.bindingHandlers.numValue = {
	    init: function (element, valueAccessor, allBindings) {
	        console.warn("numValue binding handler is being used! This should be replaced with the numericObservable.");
	        var num = valueAccessor();
	        var bindings = ko.utils.unwrapObservable(allBindings());
	        var isChar = function(key) { return key >= 65 && key <= 90; };
	        var up = 38, down = 40;
	        $(element).keydown(function(evt) {
	            var key = evt.keyCode;
	            var iVal = parseInt(element.value);
	            if (key === up || key === down) {
	                if (key === up) iVal++;
	                else if (key == down && (bindings.negative || iVal > 0)) iVal--;
	            }
	            else if (isChar(key) && !evt.ctrlKey) evt.preventDefault();
	            if (!isNaN(iVal) && iVal != null) num(iVal);
	            else num(null);

	            return true;
	        });
	        var isDown = false;
	        var lastY = 0;
	        var buffer = 10;
	        $(element).mousedown(function (e) { isDown = true; return true; });
	        $(document).mouseup(function (e) { isDown = false; return true; });
	        $(document).mousemove(function (e) {
	            if (isDown) {
	                var y = e.pageY;
	                if (!lastY) lastY = y;
	                if (y > lastY + buffer && (bindings.negative || num() > 0)) {
	                    num(num() - 1);
	                    lastY = y;
	                }
	                else if (y + buffer < lastY) {
	                    num(num() + 1);
	                    lastY = y;
	                }
	            }
	        });
	        $(document).on("mousewheel", function (evt) {
	            if (isDown) {
	                var delta = evt.originalEvent.wheelDelta;
	                if (delta > 0) {
	                    num(num() + 1);
	                }
	                else if (delta < 0 && (bindings.negative || num() > 0)) {
	                    num(num() - 1);
	                }
	            }
	        });

	        // show validations as well
	        return ko.bindingHandlers['validationCore'].init(element, valueAccessor, allBindings);
	    },
	    update: function (element, valueAccessor, allBindings) {
	        var val = ko.utils.unwrapObservable(valueAccessor());
	        if (!isNaN(val)) element.value = val;
	    }
	};

	ko.bindingHandlers.resizable = {
	    init: function (element, valueAccessor) {
	        var alsoResizeSelector = ko.unwrap(valueAccessor());
	        if (typeof alsoResizeSelector != "string") alsoResizeSelector = '';
	        $(element).resizable({
	            alsoResize: alsoResizeSelector,
	            minWidth: 300,
	            minHeight: 100
	        });
	    }
	};

	ko.bindingHandlers.accordion = {
	    init: function (element, valueAccessor) {
	    },
	    update: function (element, valueAccessor) {
	        var opts = ko.utils.unwrapObservable(valueAccessor());
	        $(element).accordion(opts);
	    }
	};

	ko.bindingHandlers.slimscroll = {
	    init: function (element) {
	        var $el = $(element);
	        $el.slimscroll({
	            //alwaysVisible: true,
	            railColor: '#222',
	            height: "100%"
	            //railVisible: true
	        });
	    }
	};

	//******************************
	// EXTENDERS 

	ko.extenders.timeEntry = function (target) {
	    var pattern = /^(\d)?(\d)?:?(\d)?(\d)?$/;
	    target.formattedTime = ko.computed({
	        read: function () {
	            if (!target()) return '00:00';
	            var val = target();
	            var formatted = val;
	            switch (val.length) {
	                case 1:
	                    formatted = val.replace(pattern, "0$1");
	                case 2:
	                    formatted += ":00";
	                    break;
	                case 3:
	                    formatted = val.replace(pattern, "0$1:$2$3");
	                    break;
	                case 4:
	                    formatted = val.replace(pattern, "$1$2:$3$4");
	                    break;
	            }
	            return formatted;
	        },
	        write: function (value) {
	            if (typeof value === "string") {
	                var parsed = Date.parse(value);
	                if (parsed) {
	                    var d = new Date(parsed);
	                    var hours = d.getHours();
	                    hours = hours < 10 ? ('0' + hours) : hours;
	                    var minutes = d.getMinutes();
	                    minutes = minutes < 10 ? ('0' + minutes) : minutes;
	                    value = hours + ":" + minutes;
	                }
	            }
	            target(value);
	        }
	    });

	    target.extend({ pattern: { message: "Invalid Date", params: /^([01]\d|2[0-3]):?([0-5]\d)$/ } });
	    target.Hours = ko.computed(function () {
	        if (!target.formattedTime()) return 0;
	        return target.formattedTime().split(":")[0];
	    });
	    target.Mins = ko.computed(function () {
	        if (!target.formattedTime()) return 0;
	        return target.formattedTime().split(":")[1];
	    });
	    target.formattedTime(target());
	    return target;
	};

	ko.extenders.toteKey = function (target, callback) {
	    var pattern = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{4})?$/;
	    var isComplete = ko.observable(false);
	    target.formattedTote = ko.computed({
	        read: function () {
	            var value = target();
	            return formatTote(value);
	        },
	        write: function (input) {
	            var value = cleanInput(input);
	            if (target() === value) return;

	            target(value);
	            if (value && value.match(pattern)) {
	                var formatted = formatTote(value);
	                if (formatted.length === 10) {
	                    isComplete(true);
	                    if (typeof callback === "function") callback(formatted);
	                }
	            }
	        },
	    });
	    target.isComplete = ko.computed(function () {
	        return isComplete();
	    });
	    target.getNextTote = function () {
	        var formatted = target.formattedTote();
	        var sequence = parseSequence();
	        if (isNaN(sequence)) return null;
	        sequence++;

	        var sequenceString = formatSequence();
	        return formatted.replace(pattern, '0$2 $4 ' + sequenceString);

	        function parseSequence() {
	            var sections = formatted.split(" ");
	            if (sections.length !== 3) return null;
	            return parseInt(sections[2]);
	        }
	        function formatSequence() {
	            var val = sequence.toString();
	            while (val.length < 4) {
	                val = "0" + val;
	            }
	            return val;
	        }
	    };
	    target.isMatch = function (val) {
	        var formattedVal = formatTote(ko.utils.unwrapObservable(val));
	        if (!formattedVal) return false;
	        var p = new RegExp("^" + target.formattedTote() + "$");
	        return formattedVal.match(p);
	    };

	    target.extend({ throttle: 800 });
	    target.formattedTote(target());
	    return target;

	    function formatTote(input) {
	        if (input == undefined) return '';
	        if (!input.match(pattern)) return input;
	        input = input.trim();
	        return input.replace(pattern, '0$2 $4 $6').trim().replace("  ", " ");
	    }
	    function cleanInput(input) {
	        if (typeof input == "number") input = input.toString();
	        if (typeof input !== "string") return undefined;
	        return input.replace(/\s/g, '');
	    }
	};

	ko.extenders.lotKey = function (target, matchCallback) {
	    var pattern = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{3})?(\s?)(\d{2}\d*)?$/;
	    var completePattern = /^(\d{2})(\s)(\d{2})(\s)(\d{3})(\s)(\d{2}\d*)$/;
	    var isComplete = ko.observable(false);
	    target.formattedLot = ko.computed({
	        read: function () {
	            return target();
	            //var value = target();
	            //if (!value) return '';
	            //return value.match(pattern)
	            //    ? formatAsLot(value)
	            //    : value;
	        },
	        write: function (value) {
	            value = cleanInput(value);
	            if (target() === value) return;

	            var formatted = formatAsLot(value);
	            target(formatted);
	            if (formatted && formatted.match(completePattern)) {
	                isComplete(true);
	                if (typeof matchCallback === "function") matchCallback(formatted);
	            }

	        }
	    });

	    function cleanInput(input) {
	        if (typeof input == "number") input = input.toString();
	        if (typeof input !== "string") return undefined;
	        return input.trim();
	    }
	    function formatAsLot(input) {
	        if (input == undefined) return undefined;
	        input = input.trim();
	        return input.replace(pattern, '0$2 $4 $6 $8').trim().replace("  ", " ");
	    }

	    target.match = function (valueToCompare) {
	        var partialPattern = new RegExp('^' + target.formattedLot() + '$');
	        return valueToCompare.match(partialPattern);
	    };
	    target.isComplete = ko.computed(function () {
	        return isComplete();
	    }, target);
	    target.Date = ko.computed(function () {
	        if (this.formattedLot()) {
	            var sections = this.formattedLot().split(" ");
	            var days = parseInt(sections[2]);
	            var defDate = "1/1/" + (parseInt(sections[1]) >= 90 ? "19" : "20");
	            var date = new Date(defDate + sections[1]).addDays(days - 1);
	            date.addMinutes(date.getTimezoneOffset());

	            return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
	        }
	    }, target);
	    target.formattedDate = ko.computed(function () {
	        var date = this();
	        if (date && date != 'Invalid Date') return date.format("UTC:m/d/yyyy");
	        return '';
	    }, target.Date);
	    target.Sequence = ko.computed({
	        read: function () {
	            if (this.formattedLot()) {
	                var sections = this.formattedLot().split(" ");
	                if (sections.length === 4)
	                    return sections[3];
	            }
	        },
	        write: function (newSeq) {
	            var val = '';
	            if (isComplete()) {
	                var reg = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{3})?(\s?)/;
	                val = this.formattedLot().match(reg)[0];
	                val += newSeq < 10 ? '0' : '';
	                val += newSeq;
	                this.formattedLot(val);
	            }
	        }
	    }, target);
	    target.getNextLot = function () {
	        var sequence = parseInt(target.Sequence());
	        sequence++;
	        if (sequence < 10) sequence = '0' + sequence;
	        return target.formattedLot().replace(pattern, '0$2 $4 $6 ' + sequence);
	    };

	    target.extend({ throttle: 800 });

	    target.formattedLot(target());
	    return target;
	};

	ko.extenders.contractType = function (target) {
	    var options = {
	        0: "Contract",
	        1: "Quote",
	        2: "Spot",
	        3: "Interim"
	    };
	    return new TypeExtension(target, options, options[0]);
	};

	ko.extenders.contractStatus = function (target) {
	    var options = {
	        0: "Pending",
	        1: "Rejected",
	        2: "Confirmed",
	        3: "Completed"
	    };
	    return new TypeExtension(target, options, options[0]);
	};

	ko.extenders.defectType = function (target) {
	    return new TypeExtension(target, rvc.helpers.defectTypeOptions, rvc.helpers.defectTypeOptions[0]);
	};

	ko.extenders.lotHoldType = function (target) {
	    return new TypeExtension(target, rvc.helpers.lotHoldTypeOptions, rvc.helpers.lotHoldTypeOptions[0]);
	};

	ko.extenders.defectResolutionType = function (target) {
	    return new TypeExtension(target, rvc.helpers.defectResolutionOptions, rvc.helpers.defectResolutionOptions[0]);
	};

	ko.extenders.productionStatusType = function (target) {
	    return new TypeExtension(target, rvc.helpers.productionStatusTypeOptions, rvc.helpers.productionStatusTypes[0]);
	};

	ko.extenders.lotQualityStatusType = function (target) {
	    return new TypeExtension(target, rvc.helpers.lotQualityStatusTypeOptions, rvc.helpers.lotQualityStatusTypes[0]);
	};

	ko.extenders.inventoryType = function (target) {
	    return new TypeExtension(target, rvc.helpers.inventoryTypeOptions, rvc.helpers.inventoryTypeOptions[0]);
	};

	ko.extenders.productType = function (target) {
	    return new TypeExtension(target, rvc.helpers.lotTypeOptions, rvc.helpers.lotTypeOptions[0]);
	};
	ko.extenders.lotType = function (target) {
	    return new TypeExtension(target, rvc.helpers.lotTypeOptions, rvc.helpers.lotTypeOptions[0]);
	};

	ko.extenders.chileType = function (target) {
	    var options = {
	        0: 'Other Raw',
	        1: 'Dehydrated',
	        2: 'WIP',
	        3: 'Finished Goods'
	    };
	    return new TypeExtension(target, options, options[0]);
	};

	ko.extenders.treatmentType = function (target) {
	    return new TypeExtension(target, rvc.helpers.treatmentTypeOptions, rvc.helpers.treatmentTypeOptions[0]);
	};

	ko.extenders.shipmentStatusType = function (target) {
	    return new TypeExtension(target, rvc.helpers.shipmentStatusOptions, rvc.helpers.shipmentStatusOptions[0]);
	};

	ko.extenders.movementTypes = function (target) {
	    var options = {
	        0: 'Same Warehouse',
	        1: 'Between Warehouses',
	    };
	    return new TypeExtension(target, options, options[0]);
	};

	ko.extenders.inventoryOrderTypes = function (target, defaultOption) {
	    return new TypeExtension(target, rvc.helpers.inventoryOrderTypeOptions, defaultOption);
	};

	// Data input binding extension. Converts input to numeric values.
	ko.extenders.numeric = function (target, precision) {
	    console.warn('Replace numeric binding extender with numericObservable object');
	    var mode = 'readonly', isWriteable = false;
	    if (!ko.isWriteableObservable(target)) {
	        mode = 'writeable';
	        isWriteable = true;
	        //throw new Error('Object must be a writableObservable in order to be used with the numeric binding. For read-only binding, use formatNumber instead.');
	    }

	    target.numericMode = mode;
	    if (isWriteable) return writable();
	    else return readonly();

	    function writable() {
	        applyFormatting(target());
	        target.subscribe(applyFormatting, target);
	        return target;

	        function applyFormatting(value) {
	            value = formatValue(value);
	            if (value === target()) return;
	            setValue(value);
	        }
	        function setValue(value) {
	            target(value);
	        }
	    }

	    function readonly() {
	        target.formattedNumber = ko.computed({
	            read: function () {
	                return formatValue(target()) || undefined;
	            },
	            write: function (val) {
	                target(formatValue(val) || undefined);
	            }
	        }, target);
	        return target;
	    }

	    function formatValue(input) {
	        var numVal = parseFloat(input);
	        if (isNaN(numVal)) return undefined;
	        else return parseFloat(numVal.toFixed(precision));
	    }
	};

	//Read-only binding for displaying numeric values with a specific decimal precision.
	//For numeric input bindings, use the numeric binding instead.
	ko.extenders.formatNumber = function (target, precision) {
	    function formatValue(input) {
	        precision = parseInt(precision) || 0;
	        return precision > 0 ? parseFloat(input).toFixed(precision) : parseInt(input);
	    }

	    target.formattedNumber = ko.computed(function () {
	        return formatValue(target()) || 0;
	    }, target);
	    return target;
	};

	//******************************
	// MAPPING HELPERS

	ko.mappings = ko.mappings || {};
	ko.mappings.formattedDate = function (options, format) {
	    var dateString = options.data;
	    var date = null;
	    if (typeof dateString == "string" && dateString.length > 0) {
	        if (dateString.match(/^\/Date\(\d*\)\/$/)) {
	            dateString = dateString.replace(/[^0-9 +]/g, '');
	            dateString = parseInt(dateString);
	        }
	        date = new Date(dateString).toISOString();
	    }
	    var result = ko.observable(date).extend({ isoDate: format || 'm/d/yyyy' });
	    return result;
	};


	//****************************************
	// validation rules
	ko.validation.rules['isUnique'] = {
	    validator: function (newVal, options) {
	        if (options.predicate && typeof options.predicate !== "function")
	            throw new Error("Invalid option for isUnique validator. The 'predicate' option must be a function.");

	        var array = options.array || options;
	        var count = 0;
	        ko.utils.arrayMap(ko.utils.unwrapObservable(array), function (existingVal) {
	            if (equalityDelegate()(existingVal, newVal)) count++;
	        });
	        return count < 2;

	        function equalityDelegate() {
	            return options.predicate ? options.predicate : function (v1, v2) { return v1 === v2; };
	        }
	    },
	    message: 'This value is a duplicate',
	};

	/*
	 * Determines if a field is required or not based on a function or value
	 * Parameter: boolean function, or boolean value
	 * Example
	 *
	 * viewModel = {
	 *   var vm = this;     
	 *   vm.isRequired = ko.observable(false);
	 *   vm.requiredField = ko.observable().extend({ conditional_required: vm.isRequired});
	 * }   
	*/
	ko.validation.rules['conditional_required'] = {
	    validator: function (val, condition) {
	        var required;
	        if (typeof condition == 'function') {
	            required = condition();
	        } else {
	            required = condition;
	        }

	        if (required) {
	            return !(val == undefined || val.length == 0);
	        } else {
	            return true;
	        }
	    },
	    message: "Field is required"
	};

	ko.validation.rules['doesNotEqual'] = {
	    validator: function (v1, v2) {
	        ko.validation.rules['doesNotEqual'].message = "\"" + v1 + "\" is not valid.";
	        return v1 !== v2;
	    },
	};

	ko.validation.rules['isValidTreatment'] = {
	    validator: function (val) {
	        return val !== rvc.helpers.treatmentTypes.NotTreated.key
	            && val !== rvc.helpers.treatmentTypes.LowBac.key;
	    },
	    message: "Invalid Treatment"
	};

	ko.validation.rules['isTrue'] = {
	    validator: function (value, fnInvalid) {
	        return fnInvalid.apply(value) === true;
	    },
	    message: "The new location is the same as the previous location. There is no need to create a movement if the items don't change location.",
	};

	ko.validation.registerExtenders();


	//******************************************
	// private functions

	function TypeExtension(target, options, defaultOption) {
	    target.displayValue = ko.computed({
	        read: function () {
	            if (target() == undefined) return '';
	            return getTypeOption(target()) || defaultOption;
	        }
	    });
	    target.options = buildSelectListOptions(options);
	    return target;

	    function buildSelectListOptions(source) {
	        var selectListOptions = [];
	        for (var opt in source) {
	            selectListOptions.push({
	                key: opt,
	                value: source[opt]
	            });
	        }
	        return selectListOptions;
	    }
	    function getTypeOption(val) {
	        switch (typeof val) {
	            case "string": return fromString(val);
	            case "number": return fromNumber(val);
	            case "object": return fromObject(val);
	            default: return undefined;
	        }

	        function fromString(s) {
	            return fromNumber(parseInt(s))
	                || findOptionByName();

	            function findOptionByName() {
	                for (var prop in options) {
	                    if (options[prop] === s) {
	                        return fromString(prop);
	                    }
	                }
	                return undefined;
	            }
	        }
	        function fromNumber(num) {
	            if (isNaN(num)) return undefined;
	            return options[num + ''];
	        }
	        function fromObject(o) {
	            if (!o || o.value == undefined) return undefined;
	            return o.value;
	        }
	    }
	}
	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 91:
/***/ (function(module, exports, __webpack_require__) {

	function LoadScreenViewModel(params) {
	    var self = this;

	    self.isVisible = params.isVisible;
	    self.loadMessage = params.displayMessage;
	}

	// Webpack
	module.exports = {
	    viewModel: LoadScreenViewModel,
	    template: __webpack_require__(92)
	};



/***/ }),

/***/ 92:
/***/ (function(module, exports) {

	module.exports = "<section class=\"modal-message load-message\" data-bind=\"loadingMessage: isVisible\">\r\n  <span>\r\n    <i class=\"fa fa-spinner fa-2x fa-pulse\"></i><br>\r\n    <!-- ko text: loadMessage --><!-- /ko -->\r\n  </span>\r\n</section>\r\n"

/***/ }),

/***/ 145:
/***/ (function(module, exports, __webpack_require__) {

	var inventoryService = __webpack_require__(77);

	__webpack_require__(18);

	ko.punches.enableAll();

	function millWetdownSummaryVM(params) {
	  if (!(this instanceof millWetdownSummaryVM)) { return new millWetdownSummaryVM(params); }

	  var self = this;

	  this.isInit = params.isInit || ko.observable(false);

	  // Data
	  this.summaryData = ko.computed(function() {
	    var input = ko.unwrap(params.input) || {};

	    return {
	      lots: input
	    };
	  });

	  this.lots = ko.pureComputed(function() {
	    var input = ko.unwrap(input) || {};

	    return input.lots;
	  });

	  // Behaviors
	  function init() {
	    self.isInit(true);
	  }

	  this.select = function(vm, $element) {
	    var context = ko.contextFor($element.target).$data;
	    var key = context ? context.MillAndWetdownKey : null;

	    if (key && params.getKey) {
	      params.getKey(key);
	    }
	  };

	  init();

	  // Exports
	  if (params && params.exports) {
	    params.exports({
	      isInit: self.isInit,
	    });
	  }

	  return this;
	}

	module.exports = {
	  viewModel: millWetdownSummaryVM,
	  template: __webpack_require__(146),
	  synchronous: true
	};


/***/ }),

/***/ 146:
/***/ (function(module, exports) {

	module.exports = "<div class=\"panel\">\r\n  <div class=\"panel-body table-responsive\">\r\n    <div class=\"text-center\" data-bind=\"ifnot: isInit\">\r\n      <i class=\"fa fa-spinner fa-pulse fa-3x\"></i>\r\n    </div>\r\n    <table class=\"clickable no-wrap table table-condensed table-hover reset\" data-bind=\"if: isInit\">\r\n      <thead>\r\n        <tr>\r\n          <th>Lot</th>\r\n          <th>Shift</th>\r\n          <th>Product</th>\r\n          <th>Line</th>\r\n          <th>Production Start</th>\r\n          <th>Production End</th>\r\n          <th class=\"calc\">Total Time</th>\r\n          <th class=\"calc\">Total Weight</th>\r\n        </tr>\r\n      </thead>\r\n      <!-- ko with: summaryData -->\r\n      <tbody data-bind=\"foreach: lots, click: $parent.select\">\r\n        <tr>\r\n          <td data-bind=\"text: OutputChileLotKey\"></td>\r\n          <td data-bind=\"text: ShiftKey\">  </td>\r\n          <td data-bind=\"text: ChileProductName\"></td>\r\n          <td data-bind=\"text: ProductionLineDescription\"></td>\r\n          <td data-bind=\"text: ProductionBegin | toDate\"></td>\r\n          <td data-bind=\"text: ProductionEnd | toDate\"></td>\r\n          <td data-bind=\"text: TotalProductionTimeMinutes\" class=\"calc\"></td>\r\n          <td data-bind=\"text: TotalWeightProduced\" class=\"calc\"></td>\r\n        </tr>\r\n      </tbody>\r\n      <!-- /ko -->\r\n    </table>\r\n  </div>\r\n</div>\r\n\r\n"

/***/ }),

/***/ 147:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {var productsService = __webpack_require__(24);
	var warehouseService = __webpack_require__(6);
	var rvc = __webpack_require__(8);

	ko.components.register('tote-picker', __webpack_require__(148));
	ko.components.register('other-materials-picker', __webpack_require__(150));
	ko.components.register('output-picker', __webpack_require__(152));
	ko.components.register('product-selector', __webpack_require__(20));

	__webpack_require__(78);
	__webpack_require__(31);
	__webpack_require__(18);
	__webpack_require__(36);

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
	  template: __webpack_require__(154),
	  synchronous: true
	};

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 148:
/***/ (function(module, exports, __webpack_require__) {

	var inventoryService = __webpack_require__(77);

	function toteInventoryPickerVM(params) {
	  if (!(this instanceof toteInventoryPickerVM)) { return new toteInventoryPickerVM(params); }

	  var self = this;
	  var input = ko.toJS(params.input);

	  // Data
	  var selectedTote = ko.observable();
	  this.tote = ko.observable();
	  this.totes = ko.observableArray([]);
	  this.inventoryItems = ko.observableArray([]);

	  // Behaviors
	  function getPickedItems() {
	    return ko.utils.arrayFilter(ko.toJS(self.inventoryItems()), isPicked);

	    function isPicked(item) {
	      return item.QuantityPicked > 0;
	    }
	  }

	  function buildDto() {
	    var pickedItems = ko.utils.arrayMap(getPickedItems(), function (item) {
	      return {
	        Tote: item.ToteKey,
	        InventoryKey: item.InventoryKey,
	        Quantity: item.QuantityPicked
	      };
	    });

	    return pickedItems;
	  }

	  // computed properties
	  this.totalWeightPicked = ko.computed(function () {
	    var sum = 0;
	    ko.utils.arrayForEach(this.inventoryItems(), function (item) {
	      if (item.QuantityPicked()) {
	        sum += item.WeightPicked();
	      }
	    });
	    return sum;
	  }, this);

	  // commands
	  this.addToteCommand = ko.asyncCommand({
	    execute: function (complete) {
	      var tote = self.tote();

	      self.getTote(tote).done(function(data, textStatus, jqXHR) {
	        addToteInventory(data);
	      })
	      .always(complete);
	    },
	    canExecute: function (isExecuting) {
	      return !isExecuting && self.tote();
	    }
	  });

	  this.removeToteCommand = ko.command({
	    execute: function(inventory) {
	      removeToteInventory.bind(self)(inventory.ToteKey());
	    }
	  });

	  // methods 
	  this.range = function(max) {
	    var r = [];

	    for (var i = 0; i < max; i++) {
	      r.push(i);
	    }

	    return r;
	  };

	  this.toggleSelection = toggleSelectedInventory;

	  this.isToteSelected = function(tote) {
	    return tote && tote === selectedTote();
	  };

	  this.tote.extend({ toteKey: self.addToteCommand.execute });

	  function addToteInventory(data) {
	    var previousInventoryCount = self.inventoryItems().length;

	    pushToteInventoryResults.call(self, data);
	    self.totes().pushAllWithoutDuplicates([self.tote], function(i) { return i.formattedTote(); });
	    self.totes.notifySubscribers();

	    if (previousInventoryCount > 0) {
	      toggleSelectedInventory(self.tote());
	    }

	    self.tote.formattedTote(self.tote.getNextTote());
	  }

	  function addExistingToteInventory(inv) {
	    addToteInventory({
	      Inventory: inv,
	      isExistingPick: true
	    });

	    getUpdatedInventory(inv);
	  }

	  function updateToteInventory(data) {
	    /** Get matching tote */
	    var tote = ko.utils.arrayFirst(self.inventoryItems(), function(item) {
	      return ko.unwrap(item.ToteKey) === data.ToteKey;
	    });
	    var updatedTote = ko.utils.arrayFirst(data.Inventory, function(item) {
	      return item.InventoryKey === tote.InventoryKey;
	    });

	    /** Update quantity */
	    tote.Quantity(updatedTote.Quantity);
	  }

	  function toggleSelectedInventory(toteKey) {
	    ko.utils.arrayForEach(self.inventoryItems(), function (item) {
	      item.Selected(item.ToteKey === toteKey && !item.Selected());
	    });

	    if (toteKey && selectedTote() !== toteKey) {
	      selectedTote(toteKey);
	    } else {
	      selectedTote(null);
	    }
	  }

	  function getUpdatedInventory(items) {
	    var inventoryItems = items || [];

	    ko.utils.arrayForEach(inventoryItems, function(item) {
	      self.getTote(ko.unwrap(item.ToteKey), { update: true }).done(function(data, textStatus, jqXHR) {
	        updateToteInventory(data);
	      });
	    });
	  }

	  function pushToteInventoryResults(data) {
	    var inventoryResults = data.Inventory || [];
	    var newInventoryItems = this.inventoryItems().pushAllWithoutDuplicates(inventoryResults, function(item) {
	      return item.InventoryKey;
	    }) || [];
	    var isExistingPick = data.isExistingPick;

	    ko.utils.arrayForEach(newInventoryItems, function (item) {
	      item.ToteKey = ko.observable(item.ToteKey).extend({ toteKey: true });

	      if (isExistingPick && !item.InventoryTreatment) {
	        // TODO NJH: Remove debug data for inventorytreatment
	        item.InventoryTreatment = {
	          TreatmentKey: '0',
	          TreatmentName: 'Not Treated',
	          TreatmentNameShort: 'NA'
	        };
	        item.Quantity = ko.observable(item.QuantityPicked);
	        item.QuantityPicked = ko.observable(item.QuantityPicked);
	      } else {
	        item.Quantity = ko.observable(item.Quantity);
	        item.QuantityPicked = ko.observable(inventoryResults.length > 1 ?
	          0 :
	          1);
	      }
	      item.WeightPicked = ko.computed(function () {
	        return item.QuantityPicked() * item.PackagingProduct.Weight;
	      });

	      item.Selected = ko.observable(false);
	    });

	    if (newInventoryItems.length) {
	      this.inventoryItems.notifySubscribers(this.inventoryItems());
	    }
	  }

	  function removeToteInventory(toteKey) {
	    var hasPickedInventory = ko.utils.arrayFirst(this.inventoryItems(), function (item) {
	      return item.QuantityPicked() > 0;
	    });

	    var remove = removeDelegate.bind(this);

	    if (hasPickedInventory) {
	      showUserMessage("Are you sure you want to remove this tote?", {
	        type: "yesno",
	        onYesClick: remove,
	        description: "Some of this tote's inventory items have been picked as input. If you want to remove all inventory items from this tote from the input materials, click \"Yes\" otherwise click \"No\"."
	      });
	    } else {
	      remove();
	    }

	    function removeDelegate() {
	      var index = 0;
	      var items = ko.toJS(this.inventoryItems()); //prevent notifications
	      ko.utils.arrayForEach(items, function (item) {
	        if (item.ToteKey !== toteKey) {
	          index++;
	          return;
	        }
	        self.inventoryItems.splice(index, 1);
	      });

	      var toteIndex = ko.utils.arrayIndexOf(this.totes(), toteKey, this);
	      if (toteIndex > -1) {
	        this.totes.splice(toteIndex, 1);
	      }
	    }
	  }    

	  if (input) {
	    resetUI();

	    addExistingToteInventory(input);
	  }

	  function resetUI() {
	    selectedTote('');
	    self.inventoryItems([]);
	    self.totes([]);
	    self.tote('');
	  }

	  // Exports
	  if (params && params.exports) {
	    params.exports({
	      getPickedItems: getPickedItems,
	      inventoryItems: self.inventoryItems,
	      toDto: buildDto
	    });
	  }

	  return this;
	}

	toteInventoryPickerVM.prototype.getTote = function(toteKey, opts) {
	  var key = ko.observable(toteKey).extend({ toteKey: true });
	    var tote = inventoryService.getInventoryByTote(toteKey).fail(function(jqXHR, textStatus, errorThrown) {
	      if (jqXHR.status === 404 && opts && opts.update === true) {
	        return tote;
	      } else if (jqXHR.status === 404) {
	        showUserMessage("Tote not found", { description: "There is no available inventory with Tote \"<strong>" + key.formattedTote() + "</strong>\"." });
	      } else {
	        showUserMessage("Error Getting Tote Inventory", { description: errorThrown || "Please refresh the page to try again. If the problem persists, please contact the system administrator." });
	      }
	    });

	    return tote;
	};

	module.exports = {
	  viewModel: toteInventoryPickerVM,
	  template: __webpack_require__(149)
	};


/***/ }),

/***/ 149:
/***/ (function(module, exports) {

	module.exports = "<form data-bind=\"submit: addToteCommand.execute\">\r\n  <h3>Dehy Materials</h3>\r\n  <div class=\"form-inline\">\r\n    <div class=\"form-group\">\r\n      <label class=\"sr-only\" for=\"toteText\">Tote Number</label>\r\n      <input class=\"form-control\" id=\"toteText\" type=\"text\" data-bind=\"value: tote.formattedTote, valueUpdate: 'input'\" placeholder=\"Tote\" maxlength=\"10\" />\r\n    </div>\r\n    <div class=\"form-group\">\r\n      <button type=\"submit\" data-bind=\"enable: addToteCommand.canExecute\" class=\"btn btn-default reset\">Add Tote</button>\r\n    </div>\r\n  </div>\r\n</form>\r\n\r\n<table class=\"table table-striped reset\" data-bind=\"fadeVisible: inventoryItems().length > 0\">\r\n  <thead>\r\n    <tr>\r\n      <th></th>\r\n      <th>Tote</th>\r\n      <th>Lot</th>\r\n      <th>Product</th>\r\n      <th>Packaging</th>\r\n      <th>Location</th>\r\n      <th>Treatment</th>\r\n      <th>Quantity</th>\r\n      <th class=\"info\">Weight (lbs)</th>\r\n    </tr>\r\n  </thead>\r\n  <tbody data-bind=\"foreach: inventoryItems\">\r\n    <tr data-bind=\"css: { 'active': QuantityPicked() === 0 }\">\r\n      <td>\r\n        <button type=\"button\" class=\"btn btn-link btn-xs btn-block\" data-bind=\"command: $parent.removeToteCommand\">\r\n          <i class=\"fa fa-times fa-lg\" tabindex=\"-1\"></i>\r\n        </button>\r\n      </td>\r\n      <td><span data-bind=\"text: ToteKey.formattedTote\"></span></td>\r\n      <td><span data-bind=\"text: LotKey\"></span></td>\r\n      <td><span data-bind=\"text: Product.ProductName\"></span></td>\r\n      <td><span data-bind=\"text: PackagingProduct.ProductName\"></span></td>\r\n      <td><span data-bind=\"text: Location.Description\"></span></td>\r\n      <td><span data-bind=\"text: InventoryTreatment.TreatmentNameShort\"></span></td>\r\n      <td><select data-bind=\"value: QuantityPicked, options: $parent.range(Quantity()+1)\" tabindex=\"-1\" class=\"form-control\"></select></td>\r\n      <td class=\"info\"><span data-bind=\"text: WeightPicked\"></span></td>\r\n    </tr>\r\n  </tbody>\r\n  <tfoot>\r\n    <tr>\r\n      <td colspan=\"8\"></td>\r\n      <td data-bind=\"text: 'Total: ' + totalWeightPicked()\" class=\"info\"></td>\r\n    </tr>\r\n  </tfoot>\r\n</table>\r\n\r\n"

/***/ }),

/***/ 150:
/***/ (function(module, exports, __webpack_require__) {

	var inventoryService = __webpack_require__(77);

	function otherInputMatPickerVM(params) {
	  if (!(this instanceof otherInputMatPickerVM)) { return new otherInputMatPickerVM(params); }

	  var self = this;
	  var _defaults = params.defaults || {};

	  // Data
	  var defaultEthoxLot = ko.pureComputed(function() {
	    return ko.toJS(_defaults).ethoxLot;
	  });
	  var defaultSipernatLot = ko.pureComputed(function() {
	    return ko.toJS(_defaults).sipernatLot;
	  });

	  this.inventoryItems = ko.observableArray([]);

	  function getPickedItems() {
	    return ko.utils.arrayFilter(self.inventoryItems(), isPicked);

	    function isPicked(item) {
	      return ko.unwrap(item.QuantityPicked) > 0;
	    }
	  }

	  function buildDto() {
	    var isValid = true;
	    var pickedItems = ko.utils.arrayMap(getPickedItems(), function (item) {
	      if (item.validation.isValid()) {
	        return {
	          Tote: item.ToteKey,
	          InventoryKey: item.InventoryKey,
	          Quantity: item.QuantityPicked
	        };
	      } else {
	        isValid = false;
	      }
	    });

	    return isValid ? pickedItems : null;
	  }

	  this.lotToAdd = ko.observable();
	  this.inventoryItems = ko.observableArray([]);
	  this.range = function(max) {
	    var r = [];

	    for (var i = 0; i < max; i++) { 
	      r.push(i);
	    }

	    return r;
	  };

	  // computed properties
	  this.TotalWeightPicked = ko.pureComputed(function() {
	    var sum = 0;
	    ko.utils.arrayForEach(this.inventoryItems(), function(item) {
	      sum += item.WeightPicked();
	    });
	    return sum;
	  }, this);

	  // commands
	  this.getLotInventoryCommand = ko.asyncCommand({
	    execute: function(complete) {
	      getLotData(self.lotToAdd.formattedLot()).done(function(data, textStatus, jqXHR) {
	        //TODO: set focus: if treatment is selected then quantity else packaging
	        addLotInventory(data);
	        self.lotToAdd('');
	      })
	      .fail(function(jqXHR, textStatus, errorThrown) {
	          showUserMessage("Unable to load inventory for lot", { description: errorThrown });
	      })
	      .always(complete);
	    },
	    canExecute: function(isExecuting) {
	      return !isExecuting;
	    }
	  });

	  this.removeItemCommand = ko.command({
	    execute: function (item) {
	      var index = ko.utils.arrayIndexOf(self.inventoryItems(), item);

	      if (item) {
	        self.inventoryItems.splice(index, 1);
	      }
	    }
	  });

	  function getLotData(lotKey) {
	    return inventoryService.getInventoryByLot(lotKey);
	  }

	  this.addEthoxCommand = ko.command({
	    execute: function () {
	      getLotData(defaultEthoxLot()).then(
	      function(data, textStatus, jqXHR) {
	        addLotInventory(data);
	      },
	      function(jqXHR, textStatus, errorThrown) {
	        showUserMessage("Could not add ethox", { description: errorThrown });
	      });
	    },
	    canExecute: function () {
	      return defaultEthoxLot();
	    }
	  });

	  this.addSipernatCommand = ko.command({
	    execute: function () {
	      getLotData(defaultSipernatLot()).then(
	      function(data, textStatus, jqXHR) {
	        addLotInventory(data);
	      },
	      function(jqXHR, textStatus, errorThrown) {
	        showUserMessage("Could not add sipernat", { description: errorThrown });
	      });
	    },
	    canExecute: function() {
	      return defaultSipernatLot();
	    }
	  });

	  this.containsPickedItem = function(inventoryKey) {
	    return ko.utils.arrayFirst(self.inventoryItems(), function(selected) {
	      return selected.InventoryKey() === inventoryKey;
	    }) !== null;
	  };

	  // init
	  this.lotToAdd.extend({ lotKey: this.getLotInventoryCommand.execute });

	  function mapLot(data) {
	    var inventory = data.InventoryItems;

	    var packagingOptions = {};
	    var locationOptions = {};
	    var treatmentOptions = {};

	    ko.utils.arrayForEach(inventory, function(item) {
	      if (!packagingOptions[item.PackagingProduct.ProductKey]) {
	        packagingOptions[item.PackagingProduct.ProductKey] = {
	          Key: item.PackagingProduct.ProductKey,
	          DisplayText: item.PackagingProduct.ProductName,
	          Capacity: item.PackagingProduct.Weight,
	        };
	      }

	      if (!locationOptions[item.Location.LocationKey]) {
	        locationOptions[item.Location.LocationKey] = {
	          Key: item.Location.LocationKey,
	          DisplayText: item.Location.Description,
	        };
	      }

	      if (!item.InventoryTreatment) {
	        item.InventoryTreatment = {
	          TreatmentKey: '0',
	          TreatmentName: 'Not Treated',
	          TreatmentNameShort: 'NA'
	        };
	      }

	      if (item.InventoryTreatment && !treatmentOptions[item.InventoryTreatment]) {
	        treatmentOptions[item.InventoryTreatment.TreatmentKey] = {
	          Key: item.InventoryTreatment.TreatmentKey,
	          DisplayText: item.InventoryTreatment.TreatmentNameShort,
	        };
	      }
	    });

	    var lotKey = inventory[0].LotKey;
	    var packagingOptionsArray = toArray(packagingOptions);
	    var locationOptionsArray = toArray(locationOptions);
	    var treatmentOptionsArray = toArray(treatmentOptions);

	    var inventorySelector = {
	      LotKey: lotKey,
	      ProductType: inventory[0].Product.ProductType,
	      ProductName: inventory[0].Product.ProductName,
	      ProductKey: inventory[0].Product.ProductKey,
	      QuantityPicked: ko.numericObservable(inventory[0].QuantityPicked || 0),
	      selectedPackaging: ko.observable(),
	      selectedFacilityLocation: ko.observable(),
	      selectedTreatment: ko.observable(),
	      InventoryKey: ko.observable(),
	    };

	    (function() {
	      var key = inventory[0].PackagingProduct.ProductKey || null;
	      var match = ko.utils.arrayFirst(packagingOptionsArray, function(item) {
	        return key === item.Key;
	      });

	      if (match) {
	        inventorySelector.selectedPackaging(match);
	      }
	    })();

	    inventorySelector.WeightPicked = ko.pureComputed(function() {
	      var quantityPicked = this.QuantityPicked();
	      var selectedPackaging = this.selectedPackaging();
	      return quantityPicked && selectedPackaging ?
	        quantityPicked * selectedPackaging.Capacity :
	        0;
	    }, inventorySelector);

	    inventorySelector.packagingOptions = ko.pureComputed(function() {
	      return packagingOptionsArray;
	    }, inventorySelector);

	    inventorySelector.locationOptions = ko.pureComputed(function() {
	      return this.selectedPackaging() ?
	        locationOptionsArray :
	        [];
	    }, inventorySelector);

	    inventorySelector.treatmentOptions = ko.pureComputed(function() {
	      return this.selectedFacilityLocation() ?
	        treatmentOptionsArray :
	        [];
	    }, inventorySelector);

	    inventorySelector.selectedInventoryItem = ko.pureComputed(function() {
	      return this.selectedTreatment() ?
	        getInventoryKey(this) :
	        null;

	        function getInventoryKey(selections) {
	          return ko.utils.arrayFirst(inventory, function(item) {
	            return item.PackagingProduct.ProductKey === selections.selectedPackaging().Key &&
	              item.Location.LocationKey === selections.selectedFacilityLocation().Key &&
	              item.InventoryTreatment.TreatmentKey === selections.selectedTreatment().Key;
	          });
	        }

	    }, inventorySelector);

	    inventorySelector.InventoryKey = ko.pureComputed(function() {
	      return this.selectedInventoryItem() ?
	        this.selectedInventoryItem().InventoryKey :
	        null;
	    }, inventorySelector).extend({
	      isUnique: {
	        array: self.inventoryItems,
	        predicate: function (opt, val) {
	          return opt.InventoryKey() === val;
	        }
	      }
	    });
	    var inventoryKeyValidator = ko.validatedObservable({
	      isDup: inventorySelector.InventoryKey
	    });
	    inventorySelector.isDuplicate = ko.pureComputed(function() {
	      return !inventoryKeyValidator.isValid();
	    });
	    inventorySelector.quantityAvailable = ko.pureComputed(function() {
	      var inventoryItem = this.selectedInventoryItem();

	      return inventoryItem ?
	        inventoryItem.Quantity - (this.QuantityPicked() || 0) :
	        0;
	    }, inventorySelector);

	    // subscribers
	    inventorySelector.locationOptions.subscribe(function(val) {
	      if (val && val.length && val.length > 1) {
	        var defaultSelection = ko.utils.arrayFirst(val, function(item) {
	          return item.DisplayText === 'Ing00';
	        });
	        if (defaultSelection) {
	          inventorySelector.selectedFacilityLocation(defaultSelection);
	        }
	      }
	    });

	    var maxQuantity = ko.pureComputed(function() {
	      return inventorySelector.selectedInventoryItem() ?
	        inventorySelector.selectedInventoryItem().Quantity :
	        0;
	    }, this);

	    inventorySelector.QuantityPicked.extend({ min: 0, max: maxQuantity });
	    inventorySelector.validation = ko.validatedObservable({
	      key: inventorySelector.InventoryKey,
	      quantity: inventorySelector.QuantityPicked
	    });

	    function toArray(obj) {
	      var a = [];
	      for (var prop in obj) {
	        a.push(obj[prop]);
	      }
	      return a;
	    }

	    return inventorySelector;
	  }

	  function addLotInventory(data) {
	    if (!data.InventoryItems.length) {
	      showUserMessage("There is no available inventory for the Lot \"" + self.lotToAdd.formattedLot() + "\".");
	      return;
	    }

	    var mappedLot = mapLot(data);

	    self.inventoryItems.splice(0, 0, mappedLot);
	    self.inventoryItems.notifySubscribers(mappedLot, 'itemAdded');
	  }

	  function getUpdatedInventory(items) {
	    var inventoryItems = items || [];

	    ko.utils.arrayForEach(inventoryItems, function(item) {
	      self.getLot(ko.unwrap(item.LotKey), { update: true }).done(function(data, textStatus, jqXHR) {
	        if (!!data.InventoryItems.length) {
	          updateLotInventory(item.LotKey, data);
	        }
	      });
	    });
	  }

	  function updateLotInventory(lotKey, data) {
	    var newData = {
	      InventoryItems: [],
	    };
	    
	    function matchOption(options, matchKey, propName) {
	      return ko.utils.arrayFirst(options, function(opt) {
	        return opt.hasOwnProperty('Key') ? opt[propName] === matchKey : false;
	      });
	    }

	    /** Update API data to include exisiting picks */
	    var lot = ko.utils.arrayFirst(self.inventoryItems(), function(item) {
	      return item.LotKey === lotKey;
	    });
	    var lotData = ko.toJS(lot);

	    ko.utils.arrayForEach(data.InventoryItems, function(inv) {
	      var matchesCurrentLot = inv.InventoryKey === lotData.InventoryKey;

	      if (matchesCurrentLot) {
	        inv.Quantity += lotData.QuantityPicked;
	      } 

	      newData.InventoryItems.push(inv);
	    });

	    /** Map updated data for use in picker and restore pick data */
	    var mappedData = mapLot(newData);
	    mappedData.selectedPackaging(matchOption(mappedData.packagingOptions(), 'Key', lotData.selectedPackaging.Key));
	    mappedData.selectedFacilityLocation(matchOption(mappedData.locationOptions(), 'Key', lotData.selectedFacilityLocation.Key));
	    mappedData.selectedTreatment(matchOption(mappedData.treatmentOptions(), 'Key', lotData.selectedTreatment.Key));
	    mappedData.QuantityPicked(lotData.QuantityPicked);

	    /** Replace existing entry with updated entry */
	    var i = self.inventoryItems().indexOf(lot);
	    self.inventoryItems.splice(i, 1, mappedData);
	  }

	  function addExistingLotInventory(inv) {
	    ko.utils.arrayForEach(ko.toJS(inv), function(item) {
	      item.Quantity = item.QuantityPicked;
	      addLotInventory({ InventoryItems: [item] });
	    });

	    getUpdatedInventory(self.inventoryItems());
	  }

	  if (ko.unwrap(params.input)) {
	    addExistingLotInventory(params.input);
	  }

	  // Exports
	  if (params && params.exports) {
	    params.exports({
	      inventoryItems: self.inventoryItems,
	      toDto: buildDto,
	      getPickedItems: getPickedItems,
	    });
	  }
	  return this;
	}

	otherInputMatPickerVM.prototype.getLot = function(lotKey, opts) {
	  var key = ko.observable(lotKey).extend({ lotKey: true });
	  var lot = inventoryService.getInventoryByLot(lotKey).fail(function(jqXHR, textStatus, errorThrown) {
	    if (jqXHR.status === 404 && opts && opts.update === true) {
	      return lot;
	    } else if (jqXHR.status === 404) {
	      showUserMessage("Lot not found", { description: "There is no available inventory with Lot \"<strong>" + key.formattedLot() + "</strong>\"." });
	    } else {
	      showUserMessage("Error Getting Lot Inventory", { description: errorThrown || "Please refresh the page to try again. If the problem persists, please contact the system administrator." });
	    }
	  });

	  return lot;
	};

	module.exports = {
	  viewModel: otherInputMatPickerVM,
	  template: __webpack_require__(151)
	};


/***/ }),

/***/ 151:
/***/ (function(module, exports) {

	module.exports = "<form data-bind=\"submit: getLotInventoryCommand.execute\" class=\"form-inline\">\r\n  <h3>Other Materials</h3>\r\n  <div class=\"form-group\">\r\n    <label class=\"sr-only\">Enter Lot Number</label>\r\n    <input class=\"form-control\" type=\"text\" data-bind=\"value: lotToAdd.formattedLot, valueUpdate: 'afterkeydown'\" placeholder=\"Lot\" maxlength=\"12\" />\r\n  </div>\r\n  <div class=\"form-group\">\r\n    <button data-bind=\"enable: getLotInventoryCommand.canExecute\" class=\"btn btn-default\">Add Lot</button>\r\n  </div>\r\n  <div class=\"form-group\">\r\n    <button data-bind=\"command: addEthoxCommand\" class=\"btn btn-link\" type=\"button\">Add Ethox.</button>\r\n  </div>\r\n  <div class=\"form-group\">\r\n    <button data-bind=\"command: addSipernatCommand\" class=\"btn btn-link\" type=\"button\">Add Sipernat</button>\r\n  </div>\r\n</form>\r\n\r\n<table class=\"table table-striped reset\" data-bind=\"fadeVisible: inventoryItems().length > 0\">\r\n  <thead>\r\n    <tr>\r\n      <th></th>\r\n      <th>Lot</th>\r\n      <th>Product</th>\r\n      <th>Packaging</th>\r\n      <th>Location</th>\r\n      <th>Treatment</th>\r\n      <th>Quantity Picked</th>\r\n      <th>Quantity Avail.</th>\r\n      <th class=\"info\">Weight (lbs)</th>\r\n    </tr>\r\n  </thead>\r\n  <tbody data-bind=\"foreach: inventoryItems\">\r\n    <tr data-bind=\"fadeVisible: true, css: { danger: isDuplicate }\">\r\n      <td>\r\n        <button type=\"button\" class=\"btn btn-link btn-xs btn-block\" data-bind=\"command: $parent.removeItemCommand\">\r\n          <i class=\"fa fa-times fa-lg\" tabindex=\"-1\"></i>\r\n        </button>\r\n      </td>\r\n      <td style=\"white-space: nowrap\" data-bind=\"text: LotKey\"></td>\r\n      <td data-bind=\"text: ProductName\"></td>\r\n      <td><select data-bind=\"value: selectedPackaging, options: packagingOptions, optionsText: 'DisplayText'\" class=\"form-control\"></select></td>\r\n      <td><select data-bind=\"value: selectedFacilityLocation, options: locationOptions, optionsText: 'DisplayText'\" class=\"form-control\"></select></td>\r\n      <td><select data-bind=\"value: selectedTreatment, options: treatmentOptions, optionsText: 'DisplayText'\" class=\"form-control\"></select></td>\r\n      <td>\r\n        <div class=\"form-group\" data-bind=\"validationElement: QuantityPicked\"><input type=\"number\" data-bind=\"value: QuantityPicked, valueUpdate: 'afterkeydown'\" class=\"form-control\" /></div>\r\n      </td>\r\n      <td data-bind=\"text: quantityAvailable\"></td>\r\n      <td class=\"info\">\r\n        <span data-bind=\"text: WeightPicked\"></span>\r\n        <input type=\"hidden\" data-bind=\"value: InventoryKey\" />\r\n      </td>\r\n    </tr>\r\n  </tbody>\r\n  <tfoot>\r\n    <tr>\r\n      <td colspan=\"8\"></td>\r\n      <td data-bind=\"text: 'Total: ' + TotalWeightPicked()\" class=\"info\"></td>\r\n    </tr>\r\n  </tfoot>\r\n</table>\r\n\r\n"

/***/ }),

/***/ 152:
/***/ (function(module, exports, __webpack_require__) {

	function OutputPickerVM(params) {
	  if (!(this instanceof OutputPickerVM)) { return new OutputPickerVM(params); }

	  var self = this;

	  // Data
	  var inputData = ko.unwrap(params.input);
	  this.options = {
	    packagingOptions: ko.unwrap(params.options && params.options.packagingOptions()) || [],
	    warehouseOptions: ko.unwrap(params.options && params.options.warehouseOptions()) || []
	  };

	  this.outputItems = ko.observableArray([]);

	  // Computed data
	  this.totalWeight = ko.pureComputed(function() {
	    var total = 0;

	    ko.utils.arrayForEach(self.outputItems(), function(item) {
	      var weight = item.Weight();

	      total += weight;
	    });

	    return total;
	  });

	  // Behaviors
	  function mapItem(item) {
	    var mappedItem = {
	      Quantity: ko.observable(),
	      FacilityLocation: ko.observable(),
	      Packaging: ko.observable(),
	      Weight: ko.pureComputed(function() {
	        var packaging = mappedItem.Packaging();
	        var quantity = mappedItem.Quantity();

	        return packaging && quantity > 0 ?
	          packaging.Weight * quantity :
	          null;
	      })
	    };

	    if (item) {
	      if (item.hasOwnProperty('Location')) {
	        mappedItem.FacilityLocation(ko.utils.arrayFirst(self.options.warehouseOptions, function(loc) {
	          return item.Location.LocationKey === loc.LocationKey;
	        }));
	      }
	      if (item.hasOwnProperty('PackagingProduct')) {
	        mappedItem.Packaging(ko.utils.arrayFirst(self.options.packagingOptions, function(packaging) {
	          return item.PackagingProduct.ProductKey === packaging.ProductKey;
	        }));
	      }
	      if (item.hasOwnProperty('QuantityProduced')) {
	        mappedItem.Quantity(item.QuantityProduced);
	      }
	    }

	    function isRequired() {
	      var location = mappedItem.FacilityLocation();
	      var packaging = mappedItem.Packaging();
	      var quantity = mappedItem.Quantity();

	      if (location || packaging || quantity) {
	        return true;
	      } else {
	        return false;
	      }
	    }

	    mappedItem.validation = ko.validatedObservable({
	      Quantity: mappedItem.Quantity.extend({ min: 1,
	      required: {
	        onlyIf: isRequired
	      }}),
	      Location: mappedItem.FacilityLocation.extend({ required: {
	        onlyIf: isRequired
	      }}),
	      Packaging: mappedItem.Packaging.extend({ required: {
	        onlyIf: isRequired
	      }})
	    });

	    (function() {
	      var _complete = false;

	      return ko.computed(function() {
	        if (!_complete) {
	          var items = ko.toJS(mappedItem);

	          if (items.Quantity &&
	              items.FacilityLocation &&
	              items.Packaging) {

	            _complete = true;

	            var outputItems = self.outputItems.peek();
	            var lastItem = outputItems[outputItems.length - 1];
	            if (lastItem === mappedItem) {
	              addItem({
	                Location: lastItem.FacilityLocation()
	              });
	            }
	          }

	          return false;
	        } else {
	          return true;
	        }
	      });
	    })();
	    
	    return mappedItem;
	  }

	  function addItem(item) {
	    self.outputItems.push(mapItem(item || null));
	    self.outputItems.notifySubscribers();
	  }

	  function addExistingOutputs(items) {
	    var currentItems = self.outputItems();
	    var newItems = items || [];
	    var mappedItems = [];

	    ko.utils.arrayForEach(newItems, function(item) {
	      mappedItems.push(mapItem(item));
	    });

	    self.outputItems(currentItems.concat(mappedItems));

	    return self.outputItems;
	  }

	  function buildDto() {
	    var isValid = true;
	    var items = [];
	    
	    var pushValidItems = ko.utils.arrayForEach(self.outputItems(), function(item) {
	      var quantity = item.Quantity();

	      if (quantity > 0) {
	        if (item.validation.isValid()) {
	          items.push({
	            PackagingProductKey: item.Packaging().ProductKey,
	            LocationKey: item.FacilityLocation().LocationKey,
	            Quantity: quantity,
	          });
	        } else {
	          isValid = false;
	        }
	      }
	    });

	    return isValid ? items : null;
	  }

	  this.addNewItem = function() {
	    var outputItems = self.outputItems();
	    var lastItem = outputItems[outputItems.length - 1];
	    var lastLocation = lastItem && lastItem.FacilityLocation();

	    if (lastLocation) {
	      addItem({
	        Location: lastLocation,
	      });
	    } else {
	      addItem();
	    }
	  };

	  this.removeItem = ko.command({
	    execute: function(item, $element) {
	      var items = self.outputItems();

	      self.outputItems.splice(items.indexOf(item), 1);
	    }
	  });


	  // Code execution
	  if (inputData) {
	    addExistingOutputs(inputData);
	  } else {
	    addItem();
	  }

	  // Exports
	  if (params && params.exports) {
	    params.exports({
	      toDto: buildDto
	    });
	  }
	  return this;
	}

	module.exports = {
	  viewModel: OutputPickerVM,
	  template: __webpack_require__(153)
	};


/***/ }),

/***/ 153:
/***/ (function(module, exports) {

	module.exports = "<table class=\"table table-striped reset\" data-bind=\"validationOptions: {\r\n        insertMessages: false,\r\n        decorateInputElement: true,\r\n        decorateElementOnModified: false,\r\n        errorElementClass: 'has-error',\r\n        errorMessageClass: 'help-block'\r\n    }\">\r\n  <thead>\r\n    <tr>\r\n      <th></th>\r\n      <th>Location</th>\r\n      <th>Packaging</th>\r\n      <th>Quantity</th>\r\n      <th class=\"info\">Weight (lbs)</th>\r\n    </tr>\r\n  </thead>\r\n  <tbody data-bind=\"foreach: outputItems\">\r\n    <tr data-bind=\"fadeVisible: true\">\r\n      <td>\r\n        <button class=\"btn btn-link btn-xs btn-block\" data-bind=\"command: $parent.removeItem\">\r\n          <i class=\"fa fa-times fa-lg\" tabindex=\"-1\"></i>\r\n        </button>\r\n      </td>\r\n      <td data-bind=\"validationElement: FacilityLocation\">\r\n        <select class=\"form-control\" data-bind=\"value: FacilityLocation, options: $parent.options.warehouseOptions, optionsText: 'Description', optionsCaption: ' '\"></select>\r\n      </td>\r\n      <td data-bind=\"validationElement: Packaging\">\r\n        <select class=\"form-control\" data-bind=\"value: Packaging, options: $parent.options.packagingOptions, optionsText: 'ProductName', optionsCaption: ' '\"></select>\r\n      </td>\r\n      <td data-bind=\"validationElement: Quantity\">\r\n        <input class=\"form-control\" type=\"number\" data-bind=\"value: Quantity, valueUpdate: 'afterkeydown'\" />\r\n      </td>\r\n      <td class=\"info\" data-bind=\"text: Weight\"></td>\r\n    </tr>\r\n  </tbody>\r\n  <tfoot>\r\n    <tr>\r\n      <td colspan=\"4\">\r\n        <button class=\"btn btn-default btn-block\" data-bind=\"click: addNewItem\"><i class=\"fa fa-plus-square-o\"></i> Add Item</button>\r\n      </td>\r\n      <td class=\"info\">\r\n        Total: <!-- ko text: totalWeight --><!-- /ko -->\r\n      </td>\r\n    </tr>\r\n  </tfoot>\r\n</table>\r\n"

/***/ }),

/***/ 154:
/***/ (function(module, exports) {

	module.exports = "<!-- ko if: hasData -->\r\n<h2 data-bind=\"text: headerText\"></h2>\r\n<!-- ko with: displayData -->\r\n<div class=\"panel panel-default\" data-bind=\"if: !$parent.isEditing()\">\r\n  <!-- ko template: 'template-mw-display' --><!-- /ko -->\r\n</div>\r\n<!-- /ko -->\r\n<!-- ko with: editorData -->\r\n<div class=\"panel panel-default\" data-bind=\"if: $parent.isEditing() && $parent.isShowingComponents()\">\r\n  <!-- ko template: 'template-mw-edit' --><!-- /ko -->\r\n</div>\r\n<!-- /ko -->\r\n<!-- /ko -->\r\n\r\n<script id=\"template-mw-display\" type=\"text/html\">\r\n  <div class=\"panel-heading\">\r\n    <div class=\"container-fluid\">\r\n      <section data-bind=\"template: 'mill-wetdown-header-display'\">\r\n      </section>\r\n    </div>\r\n  </div>\r\n  <div class=\"panel-body\">\r\n    <section data-bind=\"template: 'template-mw-input-materials-display'\">\r\n    </section>\r\n\r\n    <h3>Inventory Outputs</h3>\r\n    <section data-bind=\"template: 'template-mw-output-inventory-display'\">\r\n    </section>\r\n  </div>\r\n</script>\r\n\r\n<script id=\"template-mw-edit\" type=\"text/html\">\r\n  <div class=\"panel-heading\">\r\n    <div class=\"container-fluid\">\r\n      <section data-bind=\"template: 'mill-wetdown-header-edit'\">\r\n      </section>\r\n    </div>\r\n  </div>\r\n  <div class=\"panel-body\">\r\n    <section data-bind=\"template: 'template-mw-input-materials-edit'\">\r\n    </section>\r\n\r\n    <h3>Inventory Outputs</h3>\r\n    <output-picker params=\"input: outputInput,\r\n    options: $parent.options,\r\n    exports: $parent.exports.outputs\">\r\n    </output-picker>\r\n  </div>\r\n</script>\r\n\r\n<script id=\"mill-wetdown-header-display\" type=\"text/html\">\r\n  <div class=\"form-horizontal\">\r\n      <div class=\"row\">\r\n          <div class=\"col-sm-6 col-md-4\">\r\n              <div class=\"form-group\">\r\n                <label class=\"control-label col-sm-4\">Product</label>\r\n                <div class=\"col-sm-8 form-control-static\" data-bind=\"text: ChileProductName\"></div>\r\n              </div>\r\n          </div>\r\n          <div class=\"col-sm-6 col-md-4\">\r\n            <div class=\"form-group\">\r\n                <label class=\"control-label col-sm-4\">Shift</label>\r\n                <div class=\"form-control-static col-sm-8\" data-bind=\"text: ShiftKey\"></div>\r\n            </div>\r\n          </div>\r\n          <div class=\"col-sm-6 col-md-4\">\r\n            <div class=\"form-group\">\r\n              <label class=\"control-label col-sm-4\">Line</label>\r\n              <span class=\"form-control-static col-sm-8\" data-bind=\"text: ProductionLineDescription\"></span>\r\n            </div>\r\n          </div>\r\n          <div class=\"col-sm-6 col-md-4\">\r\n            <div class=\"form-group\">\r\n              <label class=\"control-label col-sm-4\">Production Start</label>\r\n              <span class=\"form-control-static col-sm-8\" data-bind=\"text: ProductionBegin | toDate\"></span>\r\n            </div>\r\n          </div>\r\n          <div class=\"col-sm-6 col-md-4\">\r\n            <div class=\"form-group\">\r\n              <label class=\"control-label col-sm-4\">Production End</label>\r\n              <span class=\"form-control-static col-sm-8\" data-bind=\"text: ProductionEnd | toDate\"></span>\r\n            </div>\r\n          </div>\r\n          <div class=\"col-sm-6 col-md-4\">\r\n            <div class=\"form-group\">\r\n              <label class=\"control-label col-sm-4\">Total Time</label>\r\n              <span class=\"form-control-static col-sm-8\" data-bind=\"text: $parent.totalTime | secToHrMin\"></span>\r\n            </div>\r\n          </div>\r\n      </div>\r\n  </div>\r\n</script>\r\n\r\n<script id=\"template-mw-input-materials-display\" type=\"text/html\">\r\n    <h3>Material Inputs</h3>\r\n    <div>\r\n        <table class=\"table table-striped reset\">\r\n            <thead>\r\n                <tr>\r\n                    <th>Lot</th>\r\n                    <th>Tote</th>\r\n                    <th>Product</th>\r\n                    <th>Quantity</th>\r\n                    <th>Packaging</th>\r\n                    <th class=\"calc\">Weight</th>\r\n                </tr>\r\n            </thead>\r\n            <tbody data-bind=\"foreach: PickedItems\">\r\n                <tr>\r\n                    <td data-bind=\"text: LotKey\"></td>\r\n                    <td data-bind=\"text: ToteKey\"></td>\r\n                    <td data-bind=\"text: Product.ProductName\"></td>\r\n                    <td data-bind=\"text: QuantityPicked\"></td>\r\n                    <td data-bind=\"text: PackagingProduct.ProductName\"></td>\r\n                    <td data-bind=\"text: TotalWeightPicked\" class=\"calc\"></td>\r\n                </tr>\r\n            </tbody>\r\n            <tfoot>\r\n                <tr>\r\n                    <td colspan=\"5\" style=\"text-align: right\">Total Weight:</td>\r\n                    <td data-bind=\"text: $parent.totalPickedWeight\" class=\"calc\"></td>\r\n                </tr>\r\n            </tfoot>\r\n        </table>\r\n    </div>\r\n</script>\r\n\r\n<script id=\"template-mw-output-inventory-display\" type=\"text/html\">\r\n    <div>\r\n        <table class=\"table table-striped reset\">\r\n            <thead>\r\n                <tr>\r\n                    <th>Packaging</th>\r\n                    <th>Quantity</th>\r\n                    <th>Location</th>\r\n                    <th class=\"info\">Weight</th>\r\n                </tr>\r\n            </thead>\r\n            <tbody data-bind=\"foreach: ResultItems\">\r\n                <tr>\r\n                    <td data-bind=\"text: PackagingProduct.ProductName\"></td>\r\n                    <td data-bind=\"text: QuantityProduced\"></td>\r\n                    <td data-bind=\"text: Location.Description\"></td>\r\n                    <td data-bind=\"text: TotalWeightProduced\" class=\"info\"></td>\r\n                </tr>\r\n            </tbody>\r\n            <tfoot>\r\n                <tr>\r\n                    <td colspan='3' style=\"text-align: right\">Total Weight:</td>\r\n                    <td data-bind=\"text: $parent.totalOutputWeight\" class=\"info\"></td>\r\n                </tr>\r\n            </tfoot>\r\n        </table>\r\n    </div>\r\n</script>\r\n\r\n<script id=\"mill-wetdown-header-edit\" type=\"text/html\">\r\n  <div class=\"form-horizontal\">\r\n      <div class=\"form-group\" data-bind=\"validationElement: OutputChileLotKey\">\r\n          <label class=\"control-label col-sm-4 col-md-3 col-lg-2\" for=\"lotKey\">Lot</label>\r\n          <div class=\"col-sm-4 col-md-3 col-lg-2\">\r\n              <input id=\"lotKey\" type=\"text\" data-bind=\"value: OutputChileLotKey.formattedLot, valueUpdate: 'input'\" class=\"form-control\" />\r\n              <input type=\"hidden\" data-bind=\"value: OutputChileLotKey\" />\r\n          </div>\r\n          <!-- <div data-bind=\"ajaxStatus: checkDuplicateLotKeyCommand\" class=\"col-sm-8 bg-warning\"></div> -->\r\n      </div>\r\n      <div class=\"form-group\">\r\n          <label class=\"control-label col-sm-4 col-md-3 col-lg-2\">Production Date</label>\r\n          <div class=\"col-sm-4 col-md-3 col-lg-2\">\r\n              <p class=\"form-control-static\" data-bind=\"text: OutputChileLotKey.formattedDate\"></p>\r\n          </div>\r\n      </div>\r\n      <div class=\"form-group\" data-bind=\"validationElement: ShiftKey\">\r\n          <label class=\"control-label col-sm-4 col-md-3 col-lg-2\" for=\"shift\">Shift</label>\r\n          <div class=\"col-sm-4 col-md-3 col-lg-2\">\r\n              <input id=\"shift\" type=\"text\" data-bind=\"value: ShiftKey\" class=\"form-control\" />\r\n          </div>\r\n      </div>\r\n      <div class=\"form-group\" data-bind=\"validationElement: ProductionLineKey\">\r\n          <label class=\"control-label col-sm-4 col-md-3 col-lg-2\" for=\"produtionLineSelect\">Line</label>\r\n          <div class=\"col-sm-4 col-md-3 col-lg-2\">\r\n              <select data-bind=\"value: ProductionLineKey, options: $parent.options.productionLineOptions, optionsText: 'Description', optionsValue: 'LocationKey', optionsCaption: ' '\" class=\"form-control\" id=\"produtionLineSelect\"></select>\r\n          </div>\r\n      </div>\r\n      <div class=\"form-group\" data-bind=\"css: { 'has-error': !ProductionBeginDate.isValid() || !ProductionBeginTime.isValid() }\">\r\n          <label class=\"control-label col-sm-4 col-md-3 col-lg-2\" for=\"productionBeginDate\">Production Start</label>\r\n          <div class=\"col-sm-4 col-md-3 col-lg-2\">\r\n              <input type=\"text\" data-bind=\"value: ProductionBeginDate, datePicker: true\" class=\"form-control\" id=\"productionBeginDate\" />\r\n          </div>\r\n          <div class=\"col-sm-3 col-md-2\">\r\n              <input type=\"text\" data-bind=\"value: ProductionBeginTime\" class=\"form-control\" maxlength=\"5\" />\r\n          </div>\r\n      </div>\r\n      <div class=\"form-group\" data-bind=\"css: { 'has-error': !ProductionEndDate.isValid() || !ProductionEndTime.isValid() }\">\r\n          <label class=\"control-label col-sm-4 col-md-3 col-lg-2\" for=\"productionEndDate\">Production End</label>\r\n          <div class=\"col-sm-4 col-md-3 col-lg-2\">\r\n              <input type=\"text\" data-bind=\"value: ProductionEndDate, datePicker: true\" class=\"form-control\" id=\"productionEndDate\" />\r\n          </div>\r\n          <div class=\"col-sm-3 col-md-2\">\r\n              <input type=\"text\" data-bind=\"value: ProductionEndTime\" class=\"form-control\" maxlength=\"5\" />\r\n          </div>\r\n      </div>\r\n      <div class=\"form-group\" data-bind=\"validationElement: TotalTime\">\r\n          <label class=\"control-label col-sm-4 col-md-3 col-lg-2\">Total Time:</label>\r\n          <div class=\"col-sm-4 col-md-3 col-lg-2\">\r\n            <p class=\"form-control-static\" data-bind=\"css: { 'text-danger': !TotalTime.isValid() }, text: TotalTime | secToHrMin\"></p>\r\n          </div>\r\n      </div>\r\n      <div class=\"form-group\" data-bind=\"validationElement: ChileProductKey\">\r\n          <label class=\"control-label col-sm-4 col-md-3 col-lg-2\" for=\"productSelect\">Product</label>\r\n          <div class=\"col-sm-4\">\r\n              <product-selector params=\"productsSource: $parent.options.chileProductOptions,\r\n              selectedValue: ChileProductKey,\r\n              optionsDisplay: 'ProductCodeAndName'\">\r\n              </product-selector>\r\n          </div>\r\n      </div>\r\n  </div>\r\n</script>\r\n\r\n<script id=\"template-mw-input-materials-edit\" type=\"text/html\">\r\n    <div class=\"input-materials reset\">\r\n      <tote-picker params=\"input: dehyInput,\r\n      exports: $parent.exports.totes\">\r\n      </tote-picker>\r\n      <other-materials-picker params=\"input: otherMatsInput,\r\n      defaults: $parent.defaults,\r\n      exports: $parent.exports.otherMats\">\r\n      </other-materials-picker>\r\n    </div>\r\n</script>\r\n\r\n"

/***/ })

});