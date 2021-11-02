webpackJsonp([3],{

/***/ 0:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {var rvc = __webpack_require__(8),
	  inventoryService = __webpack_require__(77),
	  page = __webpack_require__(26);

	page.base('/Warehouse/DehydratedMaterialReceiving');

	__webpack_require__(78);
	__webpack_require__(79);

	ko.components.register('dehydrated-materials-summary', __webpack_require__(80));
	ko.components.register('dehydrated-materials-details', __webpack_require__(82));

	function DehydratedMaterialsViewModel() {
	  if (!(this instanceof DehydratedMaterialsViewModel)) { return new DehydratedMaterialsViewModel(params); }

	  var self = this;

	  self.isInit = ko.observable(false);

	  // Data
	  var dataPager = inventoryService.getDehydratedMaterialsDataPager();

	  self.summaryExports = ko.observable();
	  self.detailsExports = ko.observable();
	  self.detailsInput = ko.observable();

	  self.isEditingEnabled = ko.observable(false);
	  self.isEditing = ko.observable(false);
	  self.isAddingVariety = ko.observable(false);

	  self.searchKey = ko.observable().extend({ lotKey: true });
	  self.selectedKey = ko.observable();
	  self.newVarietyName = ko.observable();

	  self.recentEntries = ko.observableArray();

	  // Computed data
	  self.isNew = ko.pureComputed(function () {
	    return this.selectedKey() === 'new';
	  }, this);

	  // Behaviors
	  // Commands
	  self.loadDetailsByKey = function (key) {
	    key && page('/' + key);
	  };
	  self.recentEntryClicked = function (vm, $element) {
	    var key = ko.contextFor($element.target).$data;
	    if (key) {
	      self.loadDetailsByKey(key);
	    }
	  };

	  self.search = function () {
	    var key = self.searchKey();
	    self.loadDetailsByKey(key);
	  };

	  function loadDetails(key) {
	    return inventoryService.getDehydratedMaterials(key)
	      .done(function(data) {
	        self.isEditing(false);
	        self.selectedKey(key);
	        data.ProductionDate = toDate(data.ProductionDate);
	        self.detailsInput(data);
	      })
	      .fail(function(jqXHR, textStatus, errorThrown) {
	        console.log(errorThrown);
	        showUserMessage('Could not get lot details', { description: errorThrown });
	      });
	  }

	    function toDate(value) {
	      var input = new Date(value),
	        dateStr = (input.getUTCMonth() + 1) + '/' + input.getUTCDate() + '/' + input.getUTCFullYear();
	      return dateStr;
	    }

	  self.loadDetails = loadDetails;
	  this.loadNextPageCommand = ko.asyncCommand({
	    execute: function (complete) {
	      self.summaryExports().getItems().always(complete);
	    },
	    canExecute: function (isExecuting) {
	      return !isExecuting;
	    }
	  });

	  this.addDetailsItemCommand = ko.command({
	    execute: function() {
	      var editor = self.detailsExports();
	      editor && editor.addItemCommand.execute();
	    },
	    canExecute: function() {
	      var editor = self.detailsExports();
	      return editor && editor.addItemCommand.canExecute();
	    }
	  });

	  this.newVariety = ko.command({
	    execute: function () {
	      self.newVarietyName(null);
	      self.isAddingVariety(true);
	    },
	    canExecute: function () {
	      return true;
	    }
	  });

	  this.saveNewVariety = ko.command({
	    execute: function () {
	      self.detailsExports().addVariety(self.newVarietyName());
	      self.isAddingVariety(false);
	    },
	    canExecute: function () {
	      return self.newVarietyName();
	    }
	  });

	  this.cancelNewVariety = ko.command({
	    execute: function () {
	      self.isAddingVariety(false);
	    },
	    canExecute: function () {
	      return true;
	    }
	  });

	  // Methods
	  this.getItem = function (lot) {
	    page('/' + lot);
	  };

	  this.editCommand = ko.command({
	    execute: function () {
	      self.isEditing(true);
	    },
	    canExecute: function () {
	      return self.isEditingEnabled();
	    }
	  });

	  this.receiveNew = ko.command({
	    execute: function () {
	      page('/new');
	    },
	    canExecute: function () {
	      return self.isInit();
	    }
	  });

	  this.addMaterial = ko.command({
	    execute: function () {
	      self.detailsExports().addMaterial();
	    },
	    canExecute: function () {
	      return true;
	    }
	  });

	  this.saveCommand = ko.asyncCommand({
	    execute: function (complete) {
	      try {
	        var details = self.detailsExports(),
	        summary = self.summaryExports();

	        if (!details.isValid()) {
	          showUserMessage('Please correct validation errors.');
	          complete();
	        } else {
	          var values = details.getValues();
	          var dfd = values.LotKey == undefined ?
	            self.createDehydratedMaterialReceivingRecord(values)
	            .done(function(newKey) {
	              addRecent(newKey);
	              values.LotKey = newKey;
	              summary.updateEntry(newKey);

	              showUserMessage('Dehydrated Materials Received Successfully', { description: 'The Dehydrated Materials have been recorded and received into inventory with the lot "' + newKey + '".' });
	              try {
	                var next = {
	                  Load: Number(values.Load) + 1,
	                  Supplier: values.Supplier,
	                  Product: values.Product,
	                  ProductionDate: values.ProductionDate,
	                };
	                self.detailsInput(next);
	                self.detailsExports().addItemCommand.execute();
	              } catch (ex) { }
	            }) :
	            self.updateDehydratedMaterialReceivingRecord(values)
	            .done(function (data) {
	              addRecent(values.LotKey);
	              self.isEditing(false);
	              self.detailsInput(data);
	            });

	          dfd.fail(function (jqXHR, textStatus, errorThrown) {
	            showUserMessage('Save failed', { description: errorThrown });
	          }).always(complete);
	        }
	      } catch (ex) {
	        complete();
	        showUserMessage("We messed up.", { description: "Something went wrong and trying again won't fix it this time. Time to call the programmers :(" });
	      }
	    },
	    canExecute: function (isExecuting) {
	      return !isExecuting;
	    }
	  });

	  function addRecent(key) {
	    var recent = self.recentEntries;

	    recent.unshift(key);

	    if (recent().length >= 5) {
	      self.recentEntries.pop();
	    }
	  }

	  this.cancelEditCommand = ko.command({
	    execute: function () {

	      if (self.isNew()) {
	        self.detailsInput(null);
	        page('/');
	      } else {
	        self.isEditing(false);
	        self.detailsExports().revertChanges();
	      }
	    },
	    canExecute: function () {
	      return self.isInit();
	    }
	  });

	  this.closePopupCommand = ko.command({
	    execute: function () {
	      page('/');
	    },
	    canExecute: function () {
	      return self.selectedKey() != undefined;
	    }
	  });

	  // Init
	  // Waits for all sub-components to load before initializing
	  var detailsInit = $.Deferred();
	      //summaryInit = $.Deferred();

	  var init = $.when(detailsInit /*, summaryInit */).done(function() {
	    self.isInit(true);
	    page();
	  });

	  ko.computed({
	    read: function () {
	      var details = self.detailsExports(),
	        summary = self.summaryExports();

	      details && details.init.then(detailsInit.resolve);
	      //summary && summary.init.then(summaryInit.resolve);

	      //if (details && details.isInit() &&
	      //    summary && summary.isInit()) {
	      //  self.isInit(true);

	      //  page();
	      //}
	    },
	    disposeWhen: function () {
	      return self.isInit();
	    }
	  });

	  page('/:key?', navigateToLot);

	  function navigateToLot(ctx) {
	    var key = ctx.params.key;

	    init.then(function() {
	      if (!key) {
	        self.isAddingVariety(false);
	        self.isEditingEnabled(false);
	        self.isEditing(false);
	        self.selectedKey(null);
	        self.loadNextPageCommand.execute();
	      } else if (key === 'new') {
	        self.isEditingEnabled(true);
	        self.isEditing(true);
	        self.selectedKey('new');

	        var today = (function () {
	          var dateString = '',
	            currentDate = Date.now();
	          dateString = (currentDate.getMonth() + 1) + '/' + currentDate.getDate() + '/' + currentDate.getFullYear();
	          return dateString;
	        })();

	        self.detailsInput({
	          ProductionDate: today,
	          Load: '1'
	        });
	        self.detailsExports().addItemCommand.execute();
	      } else if (key) {
	        self.isEditing(false);
	        loadDetails(key)
	            .done(function (data) {
	              self.isEditingEnabled(data.IsEditingEnabled);
	            })
	            .fail(function (jqXHR, textStatus, message) {
	              showUserMessage("Failed to load Dehydrated Materials record.", { description: message });
	            });
	      }

	    });
	  }

	  // Exports
	  return this;
	}

	DehydratedMaterialsViewModel.prototype.createDehydratedMaterialReceivingRecord = function(data) {
	  var jsonDto = ko.toJSON(data);

	  if (data == undefined) {
	    var dfd = $.Deferred();
	    dfd.reject();
	    return dfd;
	  }

	  return inventoryService.saveDehydratedMaterials(jsonDto);
	};

	DehydratedMaterialsViewModel.prototype.updateDehydratedMaterialReceivingRecord = function(dehyData) {
	  var jsonDto = ko.toJSON(dehyData);
	  var lotKey = dehyData.LotKey;
	  var self = this;

	  if (dehyData == undefined) {
	    return $.Deferred().reject(null, null, 'There are no valid materials to save.');
	  } else {
	    var updateRecords = inventoryService.updateDehydratedMaterials(lotKey, jsonDto)
	    .then(function(data, textStatus, jqXHR) {
	      showUserMessage('Dehydrated Materials Updated Successfully', {
	        description: 'The Dehydrated Materials have been updated into inventory with the lot "' + lotKey + '".'
	      });

	      return data;
	    },
	    function(jqXHR, textStatus, errorThrown) {
	      showUserMessage(errorThrown);
	    });

	    var refreshData = updateRecords
	    .then(function() {
	      return self.loadDetails(lotKey);
	    })
	    .then(function(data, textStatus, jqXHR) {
	      self.isEditing(false);
	      self.summaryExports().updateEntry(data);

	      return data;
	    },
	    function(jqXHR, textStatus, errorThrown) {
	      showUserMessage(errorThrown);
	    });

	    return $.when(refreshData);
	  }
	};

	var vm = new DehydratedMaterialsViewModel();

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

/***/ 79:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(jQuery) {/* OBSOLETE! USE App/scripts/utils/jquery.plugins.stickyTableHeaders.js instead */

	/*! Copyright (c) 2011 by Jonas Mosbech - https://github.com/jmosbech/StickyTableHeaders 
	Adapted by Solutionhead Technologies, LLC 5/30/2012
	MIT license info: https://github.com/jmosbech/StickyTableHeaders/blob/master/license.txt */

	jQuery(function ($) {

	    var _oldShow = $.fn.show;

	    $.fn.show = function(speed, oldCallback) {
	        return $(this).each(function() {
	            var obj = $(this),
	                newCallback = function() {
	                    if ($.isFunction(oldCallback)) {
	                        oldCallback.apply(obj);
	                    }
	                    obj.trigger('afterShow');
	                };

	            // you can trigger a before show if you want
	            obj.trigger('beforeShow');

	            // now use the old function to show the element passing the new callback
	            _oldShow.apply(obj, [speed, newCallback]);
	        });
	    };
	});

	(function ($) {
	    $.StickyTableHeaders = function (el, options) {
	        var self = this;
	        options = options || {};

	        // Access to jQuery and DOM versions of element
	        self.$window = $(window);
	        self.$el = $(el);
	        self.el = el;
	        
	        self.options = $.extend({}, $.StickyTableHeaders.defaultOptions, options);
	        self.$parent = $(options.parent || window);
	        self.$measuredElement = options.parent ? self.$parent : self.$el;
	        self.$clonedHeader = null;
	        self.$originalHeader = null;
	        
	        var parentIsWindow = options.parent == undefined;

	        // Add a reverse reference to the DOM object
	        self.$el.data('StickyTableHeaders', self);
	        
	        // functions
	        self.init = init;
	        
	        self.updateTableHeaders = function () {
	            var scrollTop = self.$parent.scrollTop();
	            var scrollLeft = self.$parent.scrollLeft();
	            var position = self.$measuredElement.offset();
	            var parentPosition = self.$parent.offset() || { top: 0, left: 0 };
	            
	            var visibleHeight = parentIsWindow ? self.$el[0].clientHeight : self.$parent[0].clientHeight;
	            visibleHeight = Math.max((visibleHeight + position.top), 0);
	            
	            var height = Math.min(self.$originalHeader.height(), visibleHeight);
	            var width = self.$measuredElement[0].clientWidth;
	            var top = parentIsWindow ? 0 : Math.max(position.top - $(window).scrollTop(), 0);
	            
	            self.$el.each(function () {
	                if (tableNeedsHeader()) {
	                    if (!isHeaderVisible()) self.updateCloneFromOriginal();
	                    self.$stickyHeaderContainer.css({
	                        'top': top,
	                        'height': height,
	                        'display': 'block',
	                        'left': position.left,
	                        'width': width,
	                    });

	                    self.$clonedHeader.css({
	                        'left': -1 * scrollLeft + "px",
	                    });
	                } else {
	                    self.$stickyHeaderContainer.css('display', 'none');
	                }
	            });
	            
	            function tableNeedsHeader() {
	                return headerIsOffScreen() && tableIsOnScreen();
	                
	                function tableIsOnScreen() {
	                    return height > 0;
	                }

	                function headerIsOffScreen() {
	                    return scrollTop + parentPosition.top > position.top
	                        || !parentIsWindow && $(window).scrollTop() > parentPosition.top;
	                }
	            }
	            function isHeaderVisible() {
	                return self.$stickyHeaderContainer.css('display') !== 'none';
	            }
	        };

	        self.updateCloneFromOriginal = function () {
	            // Copy cell widths and classes from original header
	            $('th', self.$clonedHeader).each(function (index) {
	                var $this = $(this);
	                var origCell = $('th', self.$originalHeader).eq(index);
	                $this.removeClass().addClass(origCell.attr('class'));
	                $this.css('width', origCell.width());
	            });

	            // Copy row width from whole table
	            self.$clonedHeader.css('width', self.$originalHeader.width());
	        };


	        // prevent processing of hidden tab element
	        if (options.tabs) {
	            $(options.tabs).tabs({
	                activate: function (event, ui) {
	                    if (ui.newPanel[0] == $(options.myTab)[0]) self.init();
	                }
	            });
	            return;
	        }


	        self.init();
	        

	        function init() {
	            $("tableFloatingHeader").remove();

	            self.$el.parents().scroll(function () {
	                self.updateTableHeaders();
	            });

	            self.$el.each(function () {
	                var $this = $(this);

	                // remove padding on <table> to fix issue #7
	                $this.css('padding', 0);

	                if (!$(".divTableWithFloatingFooter").length) {
	                    $this.wrap('<div class="divTableWithFloatingHeader"></div>');
	                }

	                self.$originalHeader = $('thead:first', this);
	                self.$stickyHeaderContainer = $("<div></div>");
	                self.$clonedHeader = self.$originalHeader.clone();

	                self.$clonedHeader.addClass('tableFloatingHeader');
	                self.$stickyHeaderContainer.addClass('tableFloatingHeader');
	                
	                self.$clonedHeader.css({
	                    'position': 'absolute',
	                    'top': 0,
	                    'left': 0,
	                });

	                var height;
	                if (parentIsWindow) {
	                    height = self.$parent.height();
	                } else {
	                    height = self.$parent[0].clientHeight;
	                }


	                var parentPosition = self.$parent.offset() || { top: 0, left: 0};
	                var top = Math.max(parentPosition.top, 0);
	                self.$stickyHeaderContainer.css({
	                    'position': 'fixed',
	                    'z-index': 25,
	                    'top': top,
	                    'left': parentPosition.left,
	                    'display': 'none',
	                    overflow: 'hidden',
	                    'height': height,
	                });
	                
	                self.$clonedHeader.appendTo(self.$stickyHeaderContainer);

	                self.$originalHeader.addClass('tableFloatingHeaderOriginal');
	                self.$originalHeader.before(self.$stickyHeaderContainer);
	                
	                // enabling support for jquery.tablesorter plugin
	                // forward clicks on clone to original
	                $('th', self.$clonedHeader).click(function () {
	                    var index = $('th', self.$clonedHeader).index(this);
	                    $('th', self.$originalHeader).eq(index).click();
	                });
	                $this.bind('sortEnd', self.updateCloneFromOriginal);
	            });

	            self.updateTableHeaders();
	            self.$parent.scroll(self.updateTableHeaders);
	            if (!parentIsWindow) self.$window.scroll(self.updateTableHeaders);
	            self.$window.resize(function() {
	                self.updateCloneFromOriginal();
	                self.updateTableHeaders();
	            });
	        };
	    };

	    $.StickyTableHeaders.defaultOptions = {
	        fixedOffset: 0,
	        offsetTop: 0,
	        parent: window,
	    };

	    $.fn.stickyTableHeaders = function (options) {
	        return this.each(function () {
	            (new $.StickyTableHeaders(this, options));
	        });
	    };

	})(jQuery);

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 80:
/***/ (function(module, exports, __webpack_require__) {

	var inventoryService = __webpack_require__(77);

	function DehydratedMaterialsSummaryVM (params) {
	  if (!(this instanceof DehydratedMaterialsSummaryVM)) { return new DehydratedMaterialsSummaryVM(params); }

	  var self = this;

	  this.isInit = ko.observable(false);

	  // Data
	  this.summaryItems = ko.observableArray();

	  // Behaviors
	  function getItems() {
	    return inventoryService.getDehydratedMaterials().done(function(data, textStatus, jqXHR) {
	      var summaryItems = self.summaryItems();

	      self.summaryItems(summaryItems.concat(data));
	    })
	    .fail(function(jqXHR, textStatus, errorThrown) {
	      console.log(errorThrown);
	      showUserMessage('Could not get dehydrated materials', { description: errorThrown });
	    });
	  }

	  function updateEntry(data) {
	    var lotkey = typeof data === 'string' && data;

	    if (lotkey) {
	      return inventoryService.getDehydratedMaterials(lotkey).done(function(data, textStatus, jqXHR) {
	        performUpdate(data);
	      })
	      .fail(function(jqXHR, textStatus, errorThrown) {
	      });
	    } else {
	      performUpdate(data);
	    }

	    function performUpdate(data) {
	      var items = self.summaryItems();
	      var lotKey = data.LotKey;
	      var summaryItem = ko.utils.arrayFirst(items, function(entry) {
	        return entry.LotKey === lotKey;
	      });
	      var isNew = !summaryItem;

	      if (isNew) {
	        self.summaryItems.unshift(data);
	      } else {
	        self.summaryItems.splice(items.indexOf(summaryItem), 1, data);
	      }
	    }
	  }

	  this.selectItem = function(vm, $element) {
	    var context = ko.contextFor($element.target).$data;
	    var key = (context.LotKey && context.LotKey.toString() || '').replace(/ /g, '');

	    if (key && self.summaryItems().indexOf(context) >= 0) {
	      if (params.getItem) {
	        params.getItem(key);
	      }
	    }
	  };

	  // Exports
	  if (params.exports) {
	    params.exports({
	      getItems: getItems,
	      isInit: this.isInit,
	      updateEntry: updateEntry
	    });
	  }

	  self.isInit(true);

	  return this;
	}

	module.exports = {
	  viewModel: DehydratedMaterialsSummaryVM,
	  template: __webpack_require__(81)
	};


/***/ }),

/***/ 81:
/***/ (function(module, exports) {

	module.exports = "<div class=\"panel panel-default\">\r\n    <div class=\"table-responsive\">\r\n        <table class=\"table table-condensed table-hover no-wrap clickable\">\r\n            <thead>\r\n                <tr>\r\n                    <th>Lot Key</th>\r\n                    <th>Production Date</th>\r\n                    <th>Load</th>\r\n                    <th>Dehydrator</th>\r\n                    <th>Product</th>\r\n                    <th>Purchase Order</th>\r\n                    <th>Shipper #</th>\r\n                </tr>\r\n            </thead>\r\n            <tbody data-bind=\"foreach: summaryItems, click: selectItem\">\r\n                <tr>\r\n                    <td data-bind=\"text: LotKey\"></td>\r\n                    <td data-bind=\"text: ProductionDate | toDate\"> </td>\r\n                    <td data-bind=\"text: Load\"> </td>\r\n                    <td data-bind=\"text: Dehydrator.Name\"></td>\r\n                    <td data-bind=\"text: ChileProductName\"></td>\r\n                    <td data-bind=\"text: PurchaseOrder\"> </td>\r\n                    <td data-bind=\"text: ShipperNumber\"> </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n</div>\r\n"

/***/ }),

/***/ 82:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {var inventoryService = __webpack_require__(77),
	  warehouseService = __webpack_require__(6),
	  productsService = __webpack_require__(24),
	  rvc = __webpack_require__(8),
	  DehydratedItemReceived = __webpack_require__(83);

	__webpack_require__(36);
	__webpack_require__(18);
	__webpack_require__(21);
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
	  template: __webpack_require__(84)
	};

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 83:
/***/ (function(module, exports) {

	function DehydratedItemReceived( input ) {
	  if ( !(this instanceof DehydratedItemReceived) ) { return new DehydratedItemReceived( input ); }

	  var self = this;

	  self.Tote = ko.observable( input.Tote );
	  self.Variety = ko.observable( input.Variety );
	  self.Grower = ko.observable( input.Grower );
	  self.Location = ko.observable( input.Location );
	  self.Quantity = ko.observable( input.Quantity );
	  self.PackagingProduct = ko.observable( input.PackagingProduct );

	  // computeds
	  self.Weight = ko.pureComputed(function () {
	    var quantity = self.Quantity();
	    var packaging = self.PackagingProduct();
	    return packaging && !isNaN( packaging.Weight )
	      ? ( quantity > 0 ? quantity * packaging.Weight : 0 )
	      : 0;
	  });
	  self.LocationDescription = ko.pureComputed(function () {
	    var location = self.Location() || {};
	    return location.Description;
	  });
	  self.PackagingName = ko.pureComputed(function () {
	    var packaging = self.PackagingProduct() || {};
	    return packaging.ProductName;
	  });

	  // validation and other extensions
	  self.Tote.extend({
	    toteKey: true,
	    required: { onlyIf: isRequired }
	  });
	  self.Quantity.extend({
	    required: {
	      message: "This field is required.",
	      onlyIf: function () {
	        return self.Tote() != undefined;
	      }
	    }
	  });

	  self.Variety.extend({
	    required: {
	      message: "This field is required.",
	      onlyIf: isRequired
	    }
	  });
	  self.Grower.extend({
	    required: {
	      message: "This field is required.",
	      onlyIf: isRequired
	    }
	  });
	  self.Location.extend({
	    required: {
	      message: "This field is required.",
	      onlyIf: isRequired
	    }
	  });
	  self.PackagingProduct.extend({
	    required: {
	      message: "This field is required.",
	      onlyIf: isRequired
	    }
	  });

	  function isRequired() {
	    return self.Quantity() > 0;
	  }
	}

	module.exports = DehydratedItemReceived;


/***/ }),

/***/ 84:
/***/ (function(module, exports) {

	module.exports = "<div class=\"container-fluid\" dat-bind=\"if: isInit\">\r\n  <h2 data-bind=\"text: headerText\"></h2>\r\n  <div data-bind=\"with: editorData\">\r\n\r\n    <div data-bind=\"template: $parent.editorMode\"></div>\r\n\r\n    <div class=\"panel panel-default\">\r\n      <div class=\"table-responsive\">\r\n        <table class=\"table\" data-bind=\"stickyTableHeaders: true\">\r\n          <thead>\r\n            <tr>\r\n              <th data-bind=\"visible: $parent.isEditing\"></th>\r\n              <th>Tote</th>\r\n              <th>Variety</th>\r\n              <th>Grower</th>\r\n              <th>Warehouse</th>\r\n              <th>Packaging</th>\r\n              <th>Qty</th>\r\n              <th class=\"calc\">Weight</th>\r\n            </tr>\r\n          </thead>\r\n          <tbody data-bind=\"foreach: Items\">\r\n            <tr data-bind=\"ifnot: isEditing\">\r\n              <td data-bind=\"text: Tote | toteKey\"></td>\r\n              <td data-bind=\"text: Variety\"></td>\r\n              <td data-bind=\"text: Grower\"></td>\r\n              <td data-bind=\"text: LocationDescription\"></td>\r\n              <td data-bind=\"text: PackagingName\"></td>\r\n              <td data-bind=\"text: Quantity\"></td>\r\n              <td data-bind=\"text: Weight\" class=\"calc\"></td>\r\n            </tr>\r\n            <tr data-bind=\"if: isEditing\">\r\n              <td><button class=\"btn btn-link btn-xs\" data-bind=\"click: $parentContext.$parent.removeItem\"><i class=\"fa fa-times fa-lg\"></i></button></td>\r\n              <td class=\"form-group\">\r\n                <input class=\"form-control\" type=\"text\" maxlength=\"10\" data-bind=\"value: Tote.formattedTote, valueUpdate: 'input', hasFocus: toteHasFocus\" tabindex=\"10\">\r\n              </td>\r\n              <td class=\"form-group\">\r\n                <select id=\"\" class=\"form-control\" data-bind=\"value: Variety, options: $parentContext.$parent.options.varietyOptions, optionsCaption: ' ', hasFocus: varietyHasFocus\" tabindex=\"10\"></select>\r\n              </td>\r\n              <td class=\"form-group\">\r\n                <input class=\"form-control\" type=\"text\" data-bind=\"value: Grower, hasFocus: growerHasFocus\" tabindex=\"10\">\r\n              </td>\r\n              <td class=\"form-group\">\r\n                <input class=\"form-control\" type=\"text\" data-bind=\"jqAuto: { value: Location, source: $parentContext.$parent.options.warehouseLocations, labelProp: 'Description', options: { minLength: 2, autoFocus: true }}, hasFocus: warehouseHasFocus\" tabindex=\"10\">\r\n              </td>\r\n              <td class=\"form-group\">\r\n                <select id=\"\" class=\"form-control\" data-bind=\"value: PackagingProduct, options: $parentContext.$parent.options.packagingProductOptions, optionsText: 'ProductName', optionsCaption: ' ', hasFocus: packagingHasFocus\" tabindex=\"10\"></select>\r\n              </td>\r\n              <td class=\"form-group\">\r\n                <input class=\"form-control\" type=\"text\" data-bind=\"value: Quantity, hasFocus: quantityHasFocus\" tabindex=\"10\">\r\n              </td>\r\n              <td data-bind=\"text: Weight\" class=\"calc\"></td>\r\n            </tr>\r\n          </tbody>\r\n          <tfoot>\r\n            <tr>\r\n              <td colspan=\"1\" data-bind=\"visible: $parent.isEditing\"></td>\r\n              <td colspan=\"5\"></td>\r\n              <td>Total:</td>\r\n              <td class=\"calc\" data-bind=\"text: totalWeight\"></td>\r\n            </tr>\r\n          </tfoot>\r\n        </table>\r\n      </div>\r\n    </div>\r\n  </div>\r\n</div>\r\n\r\n<script id=\"viewLot\" type=\"text/html\">\r\n  <div>\r\n    <fieldset class=\"tabular\">\r\n      <legend>Product</legend>\r\n      <ol id=\"productView\">\r\n        <li>\r\n          <label>Production Date</label>\r\n          <span data-bind=\"text: ProductionDate | toDate\"></span>\r\n        </li>\r\n        <li>\r\n          <label>Load</label>\r\n          <span data-bind=\"text: Load\"></span>\r\n        </li>\r\n        <li>\r\n          <label>Product</label>\r\n          <span data-bind=\"text: Product\"></span>\r\n        </li>\r\n        <li>\r\n          <label>Purchase Order</label>\r\n          <span data-bind=\"text: PurchaseOrder\"></span>\r\n        </li>\r\n        <li>\r\n          <label>Shipper #</label>\r\n          <span data-bind=\"text: ShipperNumber\"></span>\r\n        </li>\r\n        <li>\r\n          <label>Count of Items</label>\r\n          <span data-bind=\"text: Items().length\"></span>\r\n        </li>\r\n      </ol>\r\n    </fieldset>\r\n  </div>\r\n</script>\r\n\r\n<script id=\"editLot\" type=\"text/html\">\r\n  <div>\r\n    <div class=\"row\">\r\n      <div class=\"form-group col-sm-6 col-md-4 col-lg-3\" data-bind=\"validationElement: ProductionDate\">\r\n          <label class=\"control-label\" for=\"productionDate\">Production Date</label>\r\n          <input id=\"productionDate\" class=\"form-control\" data-bind=\"value: ProductionDate, datePicker: true, hasFocus: productionDateHasFocus\" type=\"text\" tabindex=\"1\" />\r\n      </div>\r\n      <div class=\"form-group col-sm-6 col-md-4 col-lg-3\" data-bind=\"validationElement: Supplier\">\r\n        <label class=\"control-label\">Supplier</label>\r\n        <select class=\"form-control\" data-bind=\"value: Supplier, options: $parent.options.supplierOptions, optionsText: 'Name', optionsValue: 'CompanyKey', optionsCaption: ' '\" tabindex=\"2\"></select>\r\n      </div>\r\n      <div class=\"form-group col-sm-6 col-md-4 col-lg-3\" data-bind=\"validationElement: Load\">\r\n        <label class=\"control-label\">Load</label>\r\n        <input class=\"form-control\" data-bind=\"value: Load, valueUpdate: 'input'\" type=\"text\" tabindex=\"3\" />\r\n      </div>\r\n      <div class=\"form-group col-sm-6 col-md-4 col-lg-3\" data-bind=\"validationElement: Product\">\r\n        <label class=\"control-label\">Product</label>\r\n        <select class=\"form-control\" data-bind=\"value: Product, options: $parent.options.chileProductOptions, optionsText: 'ProductName', optionsValue: 'ProductKey'\" tabindex=\"4\"></select>\r\n      </div>\r\n      <div class=\"form-group col-sm-6 col-md-4 col-lg-3\" data-bind=\"validationElement: PurchaseOrder\">\r\n        <label class=\"control-label\">Purchase Order No.</label>\r\n        <input class=\"form-control\" data-bind=\"value: PurchaseOrder, valueUpdate: 'input'\" type=\"text\" tabindex=\"5\" />\r\n      </div>\r\n      <div class=\"form-group col-sm-6 col-md-4 col-lg-3\" data-bind=\"validationElement: ShipperNumber\">\r\n        <label class=\"control-label\">Shipper No.</label>\r\n        <input class=\"form-control\" data-bind=\"value: ShipperNumber\" type=\"text\" tabindex=\"6\" />\r\n      </div>\r\n    </div>\r\n  </div>\r\n</script>\r\n"

/***/ })

});