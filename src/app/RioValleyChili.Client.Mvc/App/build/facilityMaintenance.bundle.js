webpackJsonp([4],{

/***/ 0:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {/** Required libraries */
	var warehouseService = __webpack_require__(6);
	var rvc = __webpack_require__(8);
	var page = __webpack_require__(26);

	__webpack_require__(32);
	__webpack_require__(36);
	__webpack_require__(30);

	/** KO Components*/
	ko.components.register( 'fm-summary', __webpack_require__(85) );
	ko.components.register( 'fm-editor', __webpack_require__(87) );
	ko.components.register( 'fm-location-editor', __webpack_require__(89) );
	ko.components.register( 'loading-screen', __webpack_require__(91) );

	/** Facility Maintenance view model */
	function FacilityMaintenanceVM() {
	  if ( !(this instanceof FacilityMaintenanceVM) ) { return new FacilityMaintenanceVM(); }

	  var self = this;

	  this.isInit = ko.observable( false );

	  this.options = {
	    facilities: ko.observableArray([]),
	    facilityTypes: ko.utils.arrayMap( Object.keys( rvc.lists.facilityTypes ), function( opt ) {
	      return rvc.lists.facilityTypes[ opt ];
	    })
	  };

	  // Summary view
	  this.summaryUI = {
	    input: this.options.facilities,
	    selected: ko.observable(),
	    exports: ko.observable(),
	  };

	  this.summaryUI.selected.subscribe(function( selectedFacility ) {
	    if ( selectedFacility ) {
	      editLocations( selectedFacility );
	    }
	  });

	  // Facility info editor
	  this.editorUI = {
	    input: ko.observable(),
	    exports: ko.observable()
	  };

	  // Facility location editor
	  this.locationEditorUI = {
	    input: ko.observable(),
	    exports: ko.observable()
	  };


	  this.isEditingInfo = ko.pureComputed(function() {
	    return !!self.editorUI.input();
	  });

	  this.startNewFacility = ko.command({
	    execute: function() {
	      page('/new');
	    }
	  });

	  this.editFacilityInfo = ko.command({
	    execute: function() {
	      var _facility = self.currentFacility();
	      var _facilityKey = _facility && _facility.FacilityKey;

	      if ( _facilityKey != null ) {
	        page( '/' + _facilityKey + '/edit' );
	      }
	    },
	    canExecute: function () {
	      var _editor = self.locationEditorUI.exports();
	      return _editor && !_editor.isDirty();
	    }
	  });

	  this.isSavingInfo = ko.observable( false );
	  this.saveFacilityInfo = ko.asyncCommand({
	    execute: function( complete ) {
	      var _editor = self.editorUI.exports();
	      var _data = _editor.toDto();
	      var _isNew = _data.FacilityKey == null;

	      self.isSavingInfo( true );
	      if ( !_isNew ) {
	        var updateData = warehouseService.updateFacility( _data.FacilityKey, _data ).then(
	        function( data ) {
	          return data;
	        },
	        function( jqXHR, textStatus, errorThrown ) {
	          showUserMessage( 'Could not save facility info', {
	            description: errorThrown
	          });
	        });

	        var reloadUpdatedSummaries = updateData.then(
	        function() {
	          return loadFacilities();
	        })
	        .always(function() {
	          self.isSavingInfo( false );
	          complete();
	        });

	        return reloadUpdatedSummaries;
	      } else {
	        var createFacility = warehouseService.createFacility( _data ).then(
	        function( data ) {
	          return data;
	        },
	        function( jqXHR, textStatus, errorThrown ) {
	          showUserMessage( 'Could not create new facility', {
	            description: errorThrown
	          });
	        });

	        var updateFacilityEditor = createFacility
	        .done(function( data ) {
	          self.editorUI.input( null );
	          page( '/' + data + '/locations' );
	        });

	        var reloadSummaries = updateFacilityEditor.then(
	        function() {
	          return loadFacilities();
	        })
	        .always(function() {
	          self.isSavingInfo( false );
	          complete();
	        });

	        return reloadSummaries;
	      }
	    },
	    canExecute: function( isExecuting ) {
	      var _editor = self.editorUI.exports();

	      return !isExecuting && _editor && _editor.isValid();
	    }
	  });

	  this.closeEditorUI = ko.command({
	    execute: function() {
	      if ( self.isEditingLocations() ) {
	        self.editorUI.input( null );
	        return history.back();
	      }

	      page('/');
	    }
	  });

	  this.isEditingLocations = ko.pureComputed(function() {
	    return !!self.locationEditorUI.input();
	  });

	  function editLocations( facility ) {
	    var _facilityKey = facility && facility.FacilityKey;

	    page( '/' + _facilityKey + '/locations' );
	  }

	  this.isSavingAllLocations = ko.observable( false );
	  this.saveAllLocations = ko.asyncCommand({
	    execute: function( complete ) {
	      var _saves = self.locationEditorUI.exports().saveAll();

	      self.isSavingAllLocations( true );
	      var checkSaves = _saves.then(
	      null,
	      function( jqXHR, textStatus, errorThrown ) {
	        showUserMessage( 'Location saving failed', {
	          description: 'Some locations could not be saved. ' + errorThrown
	        });
	      })
	      .always(function() {
	        self.isSavingAllLocations( false );
	        complete();
	      });

	      return checkSaves;
	    },
	    canExecute: function( isExecuting ) {
	      var _editor = self.locationEditorUI.exports();

	      return !isExecuting && _editor && _editor.isDirty();
	    }
	  });

	  this.currentFacility = ko.observable();

	  this.isFreezing = ko.observable( false );
	  this.freezeStreet = ko.asyncCommand({
	    execute: function( complete ) {
	      var _editor = self.locationEditorUI.exports();
	      var _facilityKey = self.currentFacility().FacilityKey;
	      var _street = _editor && _editor.currentStreet();

	      self.isFreezing( true );
	      var freezeStreet = warehouseService.freezeStreet( _facilityKey, _street )
	      .done(function( data ) {
	        self.locationEditorUI.exports().loadUpdatedData({
	          locations: data,
	          facilityKey: _facilityKey
	        });
	      })
	      .fail(function( jqXHR, textStatus, errorThrown ) {
	        showUserMessage( 'Could not freeze street', {
	          description: errorThrown
	        });
	      })
	      .always(function() {
	        self.isFreezing( false );
	        complete();
	      });

	      return freezeStreet;
	    },
	    canExecute: function( isExecuting ) {
	      return !isExecuting && self.currentFacility() != null;
	    }
	  });

	  this.isUnfreezing = ko.observable( false );
	  this.unfreezeStreet = ko.asyncCommand({
	    execute: function( complete ) {
	      var _editor = self.locationEditorUI.exports();
	      var _facilityKey = self.currentFacility().FacilityKey;
	      var _street = _editor && _editor.currentStreet();

	      self.isUnfreezing( true );
	      var unfreezeStreet = warehouseService.unfreezeStreet( _facilityKey, _street )
	      .done(function( data ) {
	        self.locationEditorUI.exports().loadUpdatedData({
	          locations: data,
	          facilityKey: _facilityKey
	        });
	      })
	      .fail(function( jqXHR, textStatus, errorThrown ) {
	        showUserMessage( 'Could not unfreeze street', {
	          description: errorThrown
	        });
	      })
	      .always(function() {
	        complete();
	        self.isUnfreezing( false );
	      });

	      return unfreezeStreet;
	    },

	    canExecute: function( isExecuting ) {
	      return !isExecuting && self.currentFacility() != null;
	    }
	  });
	  this.closeLocationEditorUI = ko.command({
	    execute: function() {
	      page('/');
	    }
	  });

	  // New street modal
	  this.newStreetName = ko.observable('');

	  this.startNewStreet = ko.command({
	    execute: function() {
	      $('#new-street-modal').modal('show');
	    }
	  });

	  this.saveNewStreet = ko.asyncCommand({
	    execute: function( complete ) {
	      var streetName = self.newStreetName();

	      return self.locationEditorUI.exports().createStreet( streetName ).then(
	      function() {
	        self.cancelNewStreet.execute();
	      }).always( complete );
	    },
	    canExecute: function( isExecuting ) {
	      return !isExecuting && self.newStreetName();
	    }
	  });

	  this.cancelNewStreet = ko.command({
	    execute: function() {
	      $('#new-street-modal').modal('hide');
	    }
	  });

	  $('#new-street-modal').on('hidden.bs.modal', function() {
	    self.newStreetName('');
	  });

	  this.isShowingSummaries = ko.pureComputed(function() {
	    return self.editorUI.input() == null && self.locationEditorUI.input() == null;
	  });

	  // Page routing
	  page.base('/Warehouse/FacilityMaintenance');

	  function showNewFacility( ctx, next ) {
	    var _mode = ctx.params.mode;

	    if ( _mode === 'new' ) {
	      self.editorUI.input({});
	      return;
	    }

	    next();
	  }
	  page('/:mode', showNewFacility );

	  function getDetails( facilityKey ) {
	    return warehouseService.getFacilityDetails( facilityKey )
	    .fail(function( jqXHR, textStatus, errorThrown ) {
	      showUserMessage( 'Could not load facility data', {
	        description: errorThrown
	      });
	    });
	  }

	  this.getFacilityDetails = ko.asyncCommand({
	    execute: function( facilityKey, complete ) {
	      return getDetails( facilityKey )
	      .done(function( data ) {
	        self.editorUI.input( data );
	      })
	      .always( complete );
	    }
	  });

	  this.getFacilityLocations = ko.asyncCommand({
	    execute: function( facilityKey, complete ) {
	      var getFacilityDetails = getDetails( facilityKey ).then(
	      function( data ) {
	        self.currentFacility( data );
	      });

	      var getLocations = getFacilityDetails.then(function() {
	        return warehouseService.getFacilityLocations( facilityKey )
	        .done(function( data ) {
	          self.locationEditorUI.input({
	            locations: data,
	            facilityKey: facilityKey
	          });
	        })
	        .fail(function( jqXHR, textStatus, errorThrown ) {
	          showUserMessage( 'Could not load facility data', {
	            description: errorThrown
	          });
	        })
	        .always( complete );
	      });

	      return getLocations;
	    }
	  });

	  function loadFacilityDetails( ctx, next ) {
	    var _facilityKey = ctx.params.facilityKey;
	    var _mode = ctx.params.mode;

	    var _currentFacility = self.currentFacility();
	    if ( _mode === 'locations' && (!_currentFacility || _currentFacility && _currentFacility.FacilityKey != _facilityKey) ) {
	      return self.getFacilityLocations.execute( _facilityKey );
	    }

	    if ( _mode === 'edit' ) {
	      return self.getFacilityDetails.execute( _facilityKey );
	    }

	    next();
	  }
	  page('/:facilityKey/:mode', loadFacilityDetails );

	  function returnToSummaries() {
	    self.editorUI.input( null );
	    self.locationEditorUI.input( null );
	    self.summaryUI.selected( null );
	    self.currentFacility( null );
	    self.cancelNewStreet.execute();
	  }
	  page('/', returnToSummaries );

	  function loadFacilities() {
	    return warehouseService.getFacilities()
	    .done(function( data ) {
	      self.options.facilities( data );
	    });
	  }

	  (function init() {
	    var getFacilities = loadFacilities();

	    var checkOpts = $.when( getFacilities )
	    .done(function() {
	      self.isInit( true );
	      page();
	    })
	    .fail(function( jqXHR, textStatus, errorThrown ) {
	      showUserMessage( 'Could not load Facility Maintenance UI', {
	        description: errorThrown
	      } );
	    });

	    return checkOpts;
	  })();

	  // Exports
	  return this;
	}

	var vm = new FacilityMaintenanceVM();

	ko.applyBindings( vm );

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

/***/ 12:
/***/ (function(module, exports, __webpack_require__) {

	var EditAddressViewModel = function (params) {
	    var self = this,
	        values = ko.toJS(params.input) || {};

	    self.disposables = [];

	    self.isLocked = params.locked || null;

	    // Defines initial observables
	    self.AddressLine1 = ko.observable(values.AddressLine1);
	    self.AddressLine2 = ko.observable(values.AddressLine2);
	    self.AddressLine3 = ko.observable(values.AddressLine3);
	    self.City = ko.observable(values.City);
	    self.State = ko.observable(values.State);
	    self.PostalCode = ko.observable(values.PostalCode);
	    self.Country = ko.observable(values.Country);

	    // Updates observables when input changes
	    self.disposables.push(params.input.subscribe(function (input) {
	        values = ko.toJS(input) || {};

	        self.AddressLine1(values.AddressLine1);
	        self.AddressLine2(values.AddressLine2);
	        self.AddressLine3(values.AddressLine3);
	        self.City(values.City);
	        self.State(values.State);
	        self.PostalCode(values.PostalCode);
	        self.Country(values.Country);
	    }));

	    // Output all values
	    params.output(self);
	};

	ko.utils.extend(EditAddressViewModel, {
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

	module.exports = { viewModel: EditAddressViewModel, template: __webpack_require__(13)};


/***/ }),

/***/ 13:
/***/ (function(module, exports) {

	module.exports = "<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">Address line 1: </label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"Address\" data-bind=\"value: AddressLine1, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">Address line 2: </label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"Address\" data-bind=\"value: AddressLine2, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">Address line 3: </label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"Address\" data-bind=\"value: AddressLine3, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">City: </label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"City\" data-bind=\"value: City, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">State: </label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"State\" data-bind=\"value: State, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">Postal Code: </label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"Postal Code\" data-bind=\"value: PostalCode, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">Country: </label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"Country\" data-bind=\"value: Country, disable: isLocked\" />\r\n</div>\r\n"

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

/***/ 30:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(jQuery) {/*!
	 * Bootstrap v3.3.7 (http://getbootstrap.com)
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under the MIT license
	 */

	if (typeof jQuery === 'undefined') {
	  throw new Error('Bootstrap\'s JavaScript requires jQuery')
	}

	+function ($) {
	  'use strict';
	  var version = $.fn.jquery.split(' ')[0].split('.')
	  if ((version[0] < 2 && version[1] < 9) || (version[0] == 1 && version[1] == 9 && version[2] < 1) || (version[0] > 3)) {
	    throw new Error('Bootstrap\'s JavaScript requires jQuery version 1.9.1 or higher, but lower than version 4')
	  }
	}(jQuery);

	/* ========================================================================
	 * Bootstrap: transition.js v3.3.7
	 * http://getbootstrap.com/javascript/#transitions
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // CSS TRANSITION SUPPORT (Shoutout: http://www.modernizr.com/)
	  // ============================================================

	  function transitionEnd() {
	    var el = document.createElement('bootstrap')

	    var transEndEventNames = {
	      WebkitTransition : 'webkitTransitionEnd',
	      MozTransition    : 'transitionend',
	      OTransition      : 'oTransitionEnd otransitionend',
	      transition       : 'transitionend'
	    }

	    for (var name in transEndEventNames) {
	      if (el.style[name] !== undefined) {
	        return { end: transEndEventNames[name] }
	      }
	    }

	    return false // explicit for ie8 (  ._.)
	  }

	  // http://blog.alexmaccaw.com/css-transitions
	  $.fn.emulateTransitionEnd = function (duration) {
	    var called = false
	    var $el = this
	    $(this).one('bsTransitionEnd', function () { called = true })
	    var callback = function () { if (!called) $($el).trigger($.support.transition.end) }
	    setTimeout(callback, duration)
	    return this
	  }

	  $(function () {
	    $.support.transition = transitionEnd()

	    if (!$.support.transition) return

	    $.event.special.bsTransitionEnd = {
	      bindType: $.support.transition.end,
	      delegateType: $.support.transition.end,
	      handle: function (e) {
	        if ($(e.target).is(this)) return e.handleObj.handler.apply(this, arguments)
	      }
	    }
	  })

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: alert.js v3.3.7
	 * http://getbootstrap.com/javascript/#alerts
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // ALERT CLASS DEFINITION
	  // ======================

	  var dismiss = '[data-dismiss="alert"]'
	  var Alert   = function (el) {
	    $(el).on('click', dismiss, this.close)
	  }

	  Alert.VERSION = '3.3.7'

	  Alert.TRANSITION_DURATION = 150

	  Alert.prototype.close = function (e) {
	    var $this    = $(this)
	    var selector = $this.attr('data-target')

	    if (!selector) {
	      selector = $this.attr('href')
	      selector = selector && selector.replace(/.*(?=#[^\s]*$)/, '') // strip for ie7
	    }

	    var $parent = $(selector === '#' ? [] : selector)

	    if (e) e.preventDefault()

	    if (!$parent.length) {
	      $parent = $this.closest('.alert')
	    }

	    $parent.trigger(e = $.Event('close.bs.alert'))

	    if (e.isDefaultPrevented()) return

	    $parent.removeClass('in')

	    function removeElement() {
	      // detach from parent, fire event then clean up data
	      $parent.detach().trigger('closed.bs.alert').remove()
	    }

	    $.support.transition && $parent.hasClass('fade') ?
	      $parent
	        .one('bsTransitionEnd', removeElement)
	        .emulateTransitionEnd(Alert.TRANSITION_DURATION) :
	      removeElement()
	  }


	  // ALERT PLUGIN DEFINITION
	  // =======================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this = $(this)
	      var data  = $this.data('bs.alert')

	      if (!data) $this.data('bs.alert', (data = new Alert(this)))
	      if (typeof option == 'string') data[option].call($this)
	    })
	  }

	  var old = $.fn.alert

	  $.fn.alert             = Plugin
	  $.fn.alert.Constructor = Alert


	  // ALERT NO CONFLICT
	  // =================

	  $.fn.alert.noConflict = function () {
	    $.fn.alert = old
	    return this
	  }


	  // ALERT DATA-API
	  // ==============

	  $(document).on('click.bs.alert.data-api', dismiss, Alert.prototype.close)

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: button.js v3.3.7
	 * http://getbootstrap.com/javascript/#buttons
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // BUTTON PUBLIC CLASS DEFINITION
	  // ==============================

	  var Button = function (element, options) {
	    this.$element  = $(element)
	    this.options   = $.extend({}, Button.DEFAULTS, options)
	    this.isLoading = false
	  }

	  Button.VERSION  = '3.3.7'

	  Button.DEFAULTS = {
	    loadingText: 'loading...'
	  }

	  Button.prototype.setState = function (state) {
	    var d    = 'disabled'
	    var $el  = this.$element
	    var val  = $el.is('input') ? 'val' : 'html'
	    var data = $el.data()

	    state += 'Text'

	    if (data.resetText == null) $el.data('resetText', $el[val]())

	    // push to event loop to allow forms to submit
	    setTimeout($.proxy(function () {
	      $el[val](data[state] == null ? this.options[state] : data[state])

	      if (state == 'loadingText') {
	        this.isLoading = true
	        $el.addClass(d).attr(d, d).prop(d, true)
	      } else if (this.isLoading) {
	        this.isLoading = false
	        $el.removeClass(d).removeAttr(d).prop(d, false)
	      }
	    }, this), 0)
	  }

	  Button.prototype.toggle = function () {
	    var changed = true
	    var $parent = this.$element.closest('[data-toggle="buttons"]')

	    if ($parent.length) {
	      var $input = this.$element.find('input')
	      if ($input.prop('type') == 'radio') {
	        if ($input.prop('checked')) changed = false
	        $parent.find('.active').removeClass('active')
	        this.$element.addClass('active')
	      } else if ($input.prop('type') == 'checkbox') {
	        if (($input.prop('checked')) !== this.$element.hasClass('active')) changed = false
	        this.$element.toggleClass('active')
	      }
	      $input.prop('checked', this.$element.hasClass('active'))
	      if (changed) $input.trigger('change')
	    } else {
	      this.$element.attr('aria-pressed', !this.$element.hasClass('active'))
	      this.$element.toggleClass('active')
	    }
	  }


	  // BUTTON PLUGIN DEFINITION
	  // ========================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.button')
	      var options = typeof option == 'object' && option

	      if (!data) $this.data('bs.button', (data = new Button(this, options)))

	      if (option == 'toggle') data.toggle()
	      else if (option) data.setState(option)
	    })
	  }

	  var old = $.fn.button

	  $.fn.button             = Plugin
	  $.fn.button.Constructor = Button


	  // BUTTON NO CONFLICT
	  // ==================

	  $.fn.button.noConflict = function () {
	    $.fn.button = old
	    return this
	  }


	  // BUTTON DATA-API
	  // ===============

	  $(document)
	    .on('click.bs.button.data-api', '[data-toggle^="button"]', function (e) {
	      var $btn = $(e.target).closest('.btn')
	      Plugin.call($btn, 'toggle')
	      if (!($(e.target).is('input[type="radio"], input[type="checkbox"]'))) {
	        // Prevent double click on radios, and the double selections (so cancellation) on checkboxes
	        e.preventDefault()
	        // The target component still receive the focus
	        if ($btn.is('input,button')) $btn.trigger('focus')
	        else $btn.find('input:visible,button:visible').first().trigger('focus')
	      }
	    })
	    .on('focus.bs.button.data-api blur.bs.button.data-api', '[data-toggle^="button"]', function (e) {
	      $(e.target).closest('.btn').toggleClass('focus', /^focus(in)?$/.test(e.type))
	    })

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: carousel.js v3.3.7
	 * http://getbootstrap.com/javascript/#carousel
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // CAROUSEL CLASS DEFINITION
	  // =========================

	  var Carousel = function (element, options) {
	    this.$element    = $(element)
	    this.$indicators = this.$element.find('.carousel-indicators')
	    this.options     = options
	    this.paused      = null
	    this.sliding     = null
	    this.interval    = null
	    this.$active     = null
	    this.$items      = null

	    this.options.keyboard && this.$element.on('keydown.bs.carousel', $.proxy(this.keydown, this))

	    this.options.pause == 'hover' && !('ontouchstart' in document.documentElement) && this.$element
	      .on('mouseenter.bs.carousel', $.proxy(this.pause, this))
	      .on('mouseleave.bs.carousel', $.proxy(this.cycle, this))
	  }

	  Carousel.VERSION  = '3.3.7'

	  Carousel.TRANSITION_DURATION = 600

	  Carousel.DEFAULTS = {
	    interval: 5000,
	    pause: 'hover',
	    wrap: true,
	    keyboard: true
	  }

	  Carousel.prototype.keydown = function (e) {
	    if (/input|textarea/i.test(e.target.tagName)) return
	    switch (e.which) {
	      case 37: this.prev(); break
	      case 39: this.next(); break
	      default: return
	    }

	    e.preventDefault()
	  }

	  Carousel.prototype.cycle = function (e) {
	    e || (this.paused = false)

	    this.interval && clearInterval(this.interval)

	    this.options.interval
	      && !this.paused
	      && (this.interval = setInterval($.proxy(this.next, this), this.options.interval))

	    return this
	  }

	  Carousel.prototype.getItemIndex = function (item) {
	    this.$items = item.parent().children('.item')
	    return this.$items.index(item || this.$active)
	  }

	  Carousel.prototype.getItemForDirection = function (direction, active) {
	    var activeIndex = this.getItemIndex(active)
	    var willWrap = (direction == 'prev' && activeIndex === 0)
	                || (direction == 'next' && activeIndex == (this.$items.length - 1))
	    if (willWrap && !this.options.wrap) return active
	    var delta = direction == 'prev' ? -1 : 1
	    var itemIndex = (activeIndex + delta) % this.$items.length
	    return this.$items.eq(itemIndex)
	  }

	  Carousel.prototype.to = function (pos) {
	    var that        = this
	    var activeIndex = this.getItemIndex(this.$active = this.$element.find('.item.active'))

	    if (pos > (this.$items.length - 1) || pos < 0) return

	    if (this.sliding)       return this.$element.one('slid.bs.carousel', function () { that.to(pos) }) // yes, "slid"
	    if (activeIndex == pos) return this.pause().cycle()

	    return this.slide(pos > activeIndex ? 'next' : 'prev', this.$items.eq(pos))
	  }

	  Carousel.prototype.pause = function (e) {
	    e || (this.paused = true)

	    if (this.$element.find('.next, .prev').length && $.support.transition) {
	      this.$element.trigger($.support.transition.end)
	      this.cycle(true)
	    }

	    this.interval = clearInterval(this.interval)

	    return this
	  }

	  Carousel.prototype.next = function () {
	    if (this.sliding) return
	    return this.slide('next')
	  }

	  Carousel.prototype.prev = function () {
	    if (this.sliding) return
	    return this.slide('prev')
	  }

	  Carousel.prototype.slide = function (type, next) {
	    var $active   = this.$element.find('.item.active')
	    var $next     = next || this.getItemForDirection(type, $active)
	    var isCycling = this.interval
	    var direction = type == 'next' ? 'left' : 'right'
	    var that      = this

	    if ($next.hasClass('active')) return (this.sliding = false)

	    var relatedTarget = $next[0]
	    var slideEvent = $.Event('slide.bs.carousel', {
	      relatedTarget: relatedTarget,
	      direction: direction
	    })
	    this.$element.trigger(slideEvent)
	    if (slideEvent.isDefaultPrevented()) return

	    this.sliding = true

	    isCycling && this.pause()

	    if (this.$indicators.length) {
	      this.$indicators.find('.active').removeClass('active')
	      var $nextIndicator = $(this.$indicators.children()[this.getItemIndex($next)])
	      $nextIndicator && $nextIndicator.addClass('active')
	    }

	    var slidEvent = $.Event('slid.bs.carousel', { relatedTarget: relatedTarget, direction: direction }) // yes, "slid"
	    if ($.support.transition && this.$element.hasClass('slide')) {
	      $next.addClass(type)
	      $next[0].offsetWidth // force reflow
	      $active.addClass(direction)
	      $next.addClass(direction)
	      $active
	        .one('bsTransitionEnd', function () {
	          $next.removeClass([type, direction].join(' ')).addClass('active')
	          $active.removeClass(['active', direction].join(' '))
	          that.sliding = false
	          setTimeout(function () {
	            that.$element.trigger(slidEvent)
	          }, 0)
	        })
	        .emulateTransitionEnd(Carousel.TRANSITION_DURATION)
	    } else {
	      $active.removeClass('active')
	      $next.addClass('active')
	      this.sliding = false
	      this.$element.trigger(slidEvent)
	    }

	    isCycling && this.cycle()

	    return this
	  }


	  // CAROUSEL PLUGIN DEFINITION
	  // ==========================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.carousel')
	      var options = $.extend({}, Carousel.DEFAULTS, $this.data(), typeof option == 'object' && option)
	      var action  = typeof option == 'string' ? option : options.slide

	      if (!data) $this.data('bs.carousel', (data = new Carousel(this, options)))
	      if (typeof option == 'number') data.to(option)
	      else if (action) data[action]()
	      else if (options.interval) data.pause().cycle()
	    })
	  }

	  var old = $.fn.carousel

	  $.fn.carousel             = Plugin
	  $.fn.carousel.Constructor = Carousel


	  // CAROUSEL NO CONFLICT
	  // ====================

	  $.fn.carousel.noConflict = function () {
	    $.fn.carousel = old
	    return this
	  }


	  // CAROUSEL DATA-API
	  // =================

	  var clickHandler = function (e) {
	    var href
	    var $this   = $(this)
	    var $target = $($this.attr('data-target') || (href = $this.attr('href')) && href.replace(/.*(?=#[^\s]+$)/, '')) // strip for ie7
	    if (!$target.hasClass('carousel')) return
	    var options = $.extend({}, $target.data(), $this.data())
	    var slideIndex = $this.attr('data-slide-to')
	    if (slideIndex) options.interval = false

	    Plugin.call($target, options)

	    if (slideIndex) {
	      $target.data('bs.carousel').to(slideIndex)
	    }

	    e.preventDefault()
	  }

	  $(document)
	    .on('click.bs.carousel.data-api', '[data-slide]', clickHandler)
	    .on('click.bs.carousel.data-api', '[data-slide-to]', clickHandler)

	  $(window).on('load', function () {
	    $('[data-ride="carousel"]').each(function () {
	      var $carousel = $(this)
	      Plugin.call($carousel, $carousel.data())
	    })
	  })

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: collapse.js v3.3.7
	 * http://getbootstrap.com/javascript/#collapse
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */

	/* jshint latedef: false */

	+function ($) {
	  'use strict';

	  // COLLAPSE PUBLIC CLASS DEFINITION
	  // ================================

	  var Collapse = function (element, options) {
	    this.$element      = $(element)
	    this.options       = $.extend({}, Collapse.DEFAULTS, options)
	    this.$trigger      = $('[data-toggle="collapse"][href="#' + element.id + '"],' +
	                           '[data-toggle="collapse"][data-target="#' + element.id + '"]')
	    this.transitioning = null

	    if (this.options.parent) {
	      this.$parent = this.getParent()
	    } else {
	      this.addAriaAndCollapsedClass(this.$element, this.$trigger)
	    }

	    if (this.options.toggle) this.toggle()
	  }

	  Collapse.VERSION  = '3.3.7'

	  Collapse.TRANSITION_DURATION = 350

	  Collapse.DEFAULTS = {
	    toggle: true
	  }

	  Collapse.prototype.dimension = function () {
	    var hasWidth = this.$element.hasClass('width')
	    return hasWidth ? 'width' : 'height'
	  }

	  Collapse.prototype.show = function () {
	    if (this.transitioning || this.$element.hasClass('in')) return

	    var activesData
	    var actives = this.$parent && this.$parent.children('.panel').children('.in, .collapsing')

	    if (actives && actives.length) {
	      activesData = actives.data('bs.collapse')
	      if (activesData && activesData.transitioning) return
	    }

	    var startEvent = $.Event('show.bs.collapse')
	    this.$element.trigger(startEvent)
	    if (startEvent.isDefaultPrevented()) return

	    if (actives && actives.length) {
	      Plugin.call(actives, 'hide')
	      activesData || actives.data('bs.collapse', null)
	    }

	    var dimension = this.dimension()

	    this.$element
	      .removeClass('collapse')
	      .addClass('collapsing')[dimension](0)
	      .attr('aria-expanded', true)

	    this.$trigger
	      .removeClass('collapsed')
	      .attr('aria-expanded', true)

	    this.transitioning = 1

	    var complete = function () {
	      this.$element
	        .removeClass('collapsing')
	        .addClass('collapse in')[dimension]('')
	      this.transitioning = 0
	      this.$element
	        .trigger('shown.bs.collapse')
	    }

	    if (!$.support.transition) return complete.call(this)

	    var scrollSize = $.camelCase(['scroll', dimension].join('-'))

	    this.$element
	      .one('bsTransitionEnd', $.proxy(complete, this))
	      .emulateTransitionEnd(Collapse.TRANSITION_DURATION)[dimension](this.$element[0][scrollSize])
	  }

	  Collapse.prototype.hide = function () {
	    if (this.transitioning || !this.$element.hasClass('in')) return

	    var startEvent = $.Event('hide.bs.collapse')
	    this.$element.trigger(startEvent)
	    if (startEvent.isDefaultPrevented()) return

	    var dimension = this.dimension()

	    this.$element[dimension](this.$element[dimension]())[0].offsetHeight

	    this.$element
	      .addClass('collapsing')
	      .removeClass('collapse in')
	      .attr('aria-expanded', false)

	    this.$trigger
	      .addClass('collapsed')
	      .attr('aria-expanded', false)

	    this.transitioning = 1

	    var complete = function () {
	      this.transitioning = 0
	      this.$element
	        .removeClass('collapsing')
	        .addClass('collapse')
	        .trigger('hidden.bs.collapse')
	    }

	    if (!$.support.transition) return complete.call(this)

	    this.$element
	      [dimension](0)
	      .one('bsTransitionEnd', $.proxy(complete, this))
	      .emulateTransitionEnd(Collapse.TRANSITION_DURATION)
	  }

	  Collapse.prototype.toggle = function () {
	    this[this.$element.hasClass('in') ? 'hide' : 'show']()
	  }

	  Collapse.prototype.getParent = function () {
	    return $(this.options.parent)
	      .find('[data-toggle="collapse"][data-parent="' + this.options.parent + '"]')
	      .each($.proxy(function (i, element) {
	        var $element = $(element)
	        this.addAriaAndCollapsedClass(getTargetFromTrigger($element), $element)
	      }, this))
	      .end()
	  }

	  Collapse.prototype.addAriaAndCollapsedClass = function ($element, $trigger) {
	    var isOpen = $element.hasClass('in')

	    $element.attr('aria-expanded', isOpen)
	    $trigger
	      .toggleClass('collapsed', !isOpen)
	      .attr('aria-expanded', isOpen)
	  }

	  function getTargetFromTrigger($trigger) {
	    var href
	    var target = $trigger.attr('data-target')
	      || (href = $trigger.attr('href')) && href.replace(/.*(?=#[^\s]+$)/, '') // strip for ie7

	    return $(target)
	  }


	  // COLLAPSE PLUGIN DEFINITION
	  // ==========================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.collapse')
	      var options = $.extend({}, Collapse.DEFAULTS, $this.data(), typeof option == 'object' && option)

	      if (!data && options.toggle && /show|hide/.test(option)) options.toggle = false
	      if (!data) $this.data('bs.collapse', (data = new Collapse(this, options)))
	      if (typeof option == 'string') data[option]()
	    })
	  }

	  var old = $.fn.collapse

	  $.fn.collapse             = Plugin
	  $.fn.collapse.Constructor = Collapse


	  // COLLAPSE NO CONFLICT
	  // ====================

	  $.fn.collapse.noConflict = function () {
	    $.fn.collapse = old
	    return this
	  }


	  // COLLAPSE DATA-API
	  // =================

	  $(document).on('click.bs.collapse.data-api', '[data-toggle="collapse"]', function (e) {
	    var $this   = $(this)

	    if (!$this.attr('data-target')) e.preventDefault()

	    var $target = getTargetFromTrigger($this)
	    var data    = $target.data('bs.collapse')
	    var option  = data ? 'toggle' : $this.data()

	    Plugin.call($target, option)
	  })

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: dropdown.js v3.3.7
	 * http://getbootstrap.com/javascript/#dropdowns
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // DROPDOWN CLASS DEFINITION
	  // =========================

	  var backdrop = '.dropdown-backdrop'
	  var toggle   = '[data-toggle="dropdown"]'
	  var Dropdown = function (element) {
	    $(element).on('click.bs.dropdown', this.toggle)
	  }

	  Dropdown.VERSION = '3.3.7'

	  function getParent($this) {
	    var selector = $this.attr('data-target')

	    if (!selector) {
	      selector = $this.attr('href')
	      selector = selector && /#[A-Za-z]/.test(selector) && selector.replace(/.*(?=#[^\s]*$)/, '') // strip for ie7
	    }

	    var $parent = selector && $(selector)

	    return $parent && $parent.length ? $parent : $this.parent()
	  }

	  function clearMenus(e) {
	    if (e && e.which === 3) return
	    $(backdrop).remove()
	    $(toggle).each(function () {
	      var $this         = $(this)
	      var $parent       = getParent($this)
	      var relatedTarget = { relatedTarget: this }

	      if (!$parent.hasClass('open')) return

	      if (e && e.type == 'click' && /input|textarea/i.test(e.target.tagName) && $.contains($parent[0], e.target)) return

	      $parent.trigger(e = $.Event('hide.bs.dropdown', relatedTarget))

	      if (e.isDefaultPrevented()) return

	      $this.attr('aria-expanded', 'false')
	      $parent.removeClass('open').trigger($.Event('hidden.bs.dropdown', relatedTarget))
	    })
	  }

	  Dropdown.prototype.toggle = function (e) {
	    var $this = $(this)

	    if ($this.is('.disabled, :disabled')) return

	    var $parent  = getParent($this)
	    var isActive = $parent.hasClass('open')

	    clearMenus()

	    if (!isActive) {
	      if ('ontouchstart' in document.documentElement && !$parent.closest('.navbar-nav').length) {
	        // if mobile we use a backdrop because click events don't delegate
	        $(document.createElement('div'))
	          .addClass('dropdown-backdrop')
	          .insertAfter($(this))
	          .on('click', clearMenus)
	      }

	      var relatedTarget = { relatedTarget: this }
	      $parent.trigger(e = $.Event('show.bs.dropdown', relatedTarget))

	      if (e.isDefaultPrevented()) return

	      $this
	        .trigger('focus')
	        .attr('aria-expanded', 'true')

	      $parent
	        .toggleClass('open')
	        .trigger($.Event('shown.bs.dropdown', relatedTarget))
	    }

	    return false
	  }

	  Dropdown.prototype.keydown = function (e) {
	    if (!/(38|40|27|32)/.test(e.which) || /input|textarea/i.test(e.target.tagName)) return

	    var $this = $(this)

	    e.preventDefault()
	    e.stopPropagation()

	    if ($this.is('.disabled, :disabled')) return

	    var $parent  = getParent($this)
	    var isActive = $parent.hasClass('open')

	    if (!isActive && e.which != 27 || isActive && e.which == 27) {
	      if (e.which == 27) $parent.find(toggle).trigger('focus')
	      return $this.trigger('click')
	    }

	    var desc = ' li:not(.disabled):visible a'
	    var $items = $parent.find('.dropdown-menu' + desc)

	    if (!$items.length) return

	    var index = $items.index(e.target)

	    if (e.which == 38 && index > 0)                 index--         // up
	    if (e.which == 40 && index < $items.length - 1) index++         // down
	    if (!~index)                                    index = 0

	    $items.eq(index).trigger('focus')
	  }


	  // DROPDOWN PLUGIN DEFINITION
	  // ==========================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this = $(this)
	      var data  = $this.data('bs.dropdown')

	      if (!data) $this.data('bs.dropdown', (data = new Dropdown(this)))
	      if (typeof option == 'string') data[option].call($this)
	    })
	  }

	  var old = $.fn.dropdown

	  $.fn.dropdown             = Plugin
	  $.fn.dropdown.Constructor = Dropdown


	  // DROPDOWN NO CONFLICT
	  // ====================

	  $.fn.dropdown.noConflict = function () {
	    $.fn.dropdown = old
	    return this
	  }


	  // APPLY TO STANDARD DROPDOWN ELEMENTS
	  // ===================================

	  $(document)
	    .on('click.bs.dropdown.data-api', clearMenus)
	    .on('click.bs.dropdown.data-api', '.dropdown form', function (e) { e.stopPropagation() })
	    .on('click.bs.dropdown.data-api', toggle, Dropdown.prototype.toggle)
	    .on('keydown.bs.dropdown.data-api', toggle, Dropdown.prototype.keydown)
	    .on('keydown.bs.dropdown.data-api', '.dropdown-menu', Dropdown.prototype.keydown)

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: modal.js v3.3.7
	 * http://getbootstrap.com/javascript/#modals
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // MODAL CLASS DEFINITION
	  // ======================

	  var Modal = function (element, options) {
	    this.options             = options
	    this.$body               = $(document.body)
	    this.$element            = $(element)
	    this.$dialog             = this.$element.find('.modal-dialog')
	    this.$backdrop           = null
	    this.isShown             = null
	    this.originalBodyPad     = null
	    this.scrollbarWidth      = 0
	    this.ignoreBackdropClick = false

	    if (this.options.remote) {
	      this.$element
	        .find('.modal-content')
	        .load(this.options.remote, $.proxy(function () {
	          this.$element.trigger('loaded.bs.modal')
	        }, this))
	    }
	  }

	  Modal.VERSION  = '3.3.7'

	  Modal.TRANSITION_DURATION = 300
	  Modal.BACKDROP_TRANSITION_DURATION = 150

	  Modal.DEFAULTS = {
	    backdrop: true,
	    keyboard: true,
	    show: true
	  }

	  Modal.prototype.toggle = function (_relatedTarget) {
	    return this.isShown ? this.hide() : this.show(_relatedTarget)
	  }

	  Modal.prototype.show = function (_relatedTarget) {
	    var that = this
	    var e    = $.Event('show.bs.modal', { relatedTarget: _relatedTarget })

	    this.$element.trigger(e)

	    if (this.isShown || e.isDefaultPrevented()) return

	    this.isShown = true

	    this.checkScrollbar()
	    this.setScrollbar()
	    this.$body.addClass('modal-open')

	    this.escape()
	    this.resize()

	    this.$element.on('click.dismiss.bs.modal', '[data-dismiss="modal"]', $.proxy(this.hide, this))

	    this.$dialog.on('mousedown.dismiss.bs.modal', function () {
	      that.$element.one('mouseup.dismiss.bs.modal', function (e) {
	        if ($(e.target).is(that.$element)) that.ignoreBackdropClick = true
	      })
	    })

	    this.backdrop(function () {
	      var transition = $.support.transition && that.$element.hasClass('fade')

	      if (!that.$element.parent().length) {
	        that.$element.appendTo(that.$body) // don't move modals dom position
	      }

	      that.$element
	        .show()
	        .scrollTop(0)

	      that.adjustDialog()

	      if (transition) {
	        that.$element[0].offsetWidth // force reflow
	      }

	      that.$element.addClass('in')

	      that.enforceFocus()

	      var e = $.Event('shown.bs.modal', { relatedTarget: _relatedTarget })

	      transition ?
	        that.$dialog // wait for modal to slide in
	          .one('bsTransitionEnd', function () {
	            that.$element.trigger('focus').trigger(e)
	          })
	          .emulateTransitionEnd(Modal.TRANSITION_DURATION) :
	        that.$element.trigger('focus').trigger(e)
	    })
	  }

	  Modal.prototype.hide = function (e) {
	    if (e) e.preventDefault()

	    e = $.Event('hide.bs.modal')

	    this.$element.trigger(e)

	    if (!this.isShown || e.isDefaultPrevented()) return

	    this.isShown = false

	    this.escape()
	    this.resize()

	    $(document).off('focusin.bs.modal')

	    this.$element
	      .removeClass('in')
	      .off('click.dismiss.bs.modal')
	      .off('mouseup.dismiss.bs.modal')

	    this.$dialog.off('mousedown.dismiss.bs.modal')

	    $.support.transition && this.$element.hasClass('fade') ?
	      this.$element
	        .one('bsTransitionEnd', $.proxy(this.hideModal, this))
	        .emulateTransitionEnd(Modal.TRANSITION_DURATION) :
	      this.hideModal()
	  }

	  Modal.prototype.enforceFocus = function () {
	    $(document)
	      .off('focusin.bs.modal') // guard against infinite focus loop
	      .on('focusin.bs.modal', $.proxy(function (e) {
	        if (document !== e.target &&
	            this.$element[0] !== e.target &&
	            !this.$element.has(e.target).length) {
	          this.$element.trigger('focus')
	        }
	      }, this))
	  }

	  Modal.prototype.escape = function () {
	    if (this.isShown && this.options.keyboard) {
	      this.$element.on('keydown.dismiss.bs.modal', $.proxy(function (e) {
	        e.which == 27 && this.hide()
	      }, this))
	    } else if (!this.isShown) {
	      this.$element.off('keydown.dismiss.bs.modal')
	    }
	  }

	  Modal.prototype.resize = function () {
	    if (this.isShown) {
	      $(window).on('resize.bs.modal', $.proxy(this.handleUpdate, this))
	    } else {
	      $(window).off('resize.bs.modal')
	    }
	  }

	  Modal.prototype.hideModal = function () {
	    var that = this
	    this.$element.hide()
	    this.backdrop(function () {
	      that.$body.removeClass('modal-open')
	      that.resetAdjustments()
	      that.resetScrollbar()
	      that.$element.trigger('hidden.bs.modal')
	    })
	  }

	  Modal.prototype.removeBackdrop = function () {
	    this.$backdrop && this.$backdrop.remove()
	    this.$backdrop = null
	  }

	  Modal.prototype.backdrop = function (callback) {
	    var that = this
	    var animate = this.$element.hasClass('fade') ? 'fade' : ''

	    if (this.isShown && this.options.backdrop) {
	      var doAnimate = $.support.transition && animate

	      this.$backdrop = $(document.createElement('div'))
	        .addClass('modal-backdrop ' + animate)
	        .appendTo(this.$body)

	      this.$element.on('click.dismiss.bs.modal', $.proxy(function (e) {
	        if (this.ignoreBackdropClick) {
	          this.ignoreBackdropClick = false
	          return
	        }
	        if (e.target !== e.currentTarget) return
	        this.options.backdrop == 'static'
	          ? this.$element[0].focus()
	          : this.hide()
	      }, this))

	      if (doAnimate) this.$backdrop[0].offsetWidth // force reflow

	      this.$backdrop.addClass('in')

	      if (!callback) return

	      doAnimate ?
	        this.$backdrop
	          .one('bsTransitionEnd', callback)
	          .emulateTransitionEnd(Modal.BACKDROP_TRANSITION_DURATION) :
	        callback()

	    } else if (!this.isShown && this.$backdrop) {
	      this.$backdrop.removeClass('in')

	      var callbackRemove = function () {
	        that.removeBackdrop()
	        callback && callback()
	      }
	      $.support.transition && this.$element.hasClass('fade') ?
	        this.$backdrop
	          .one('bsTransitionEnd', callbackRemove)
	          .emulateTransitionEnd(Modal.BACKDROP_TRANSITION_DURATION) :
	        callbackRemove()

	    } else if (callback) {
	      callback()
	    }
	  }

	  // these following methods are used to handle overflowing modals

	  Modal.prototype.handleUpdate = function () {
	    this.adjustDialog()
	  }

	  Modal.prototype.adjustDialog = function () {
	    var modalIsOverflowing = this.$element[0].scrollHeight > document.documentElement.clientHeight

	    this.$element.css({
	      paddingLeft:  !this.bodyIsOverflowing && modalIsOverflowing ? this.scrollbarWidth : '',
	      paddingRight: this.bodyIsOverflowing && !modalIsOverflowing ? this.scrollbarWidth : ''
	    })
	  }

	  Modal.prototype.resetAdjustments = function () {
	    this.$element.css({
	      paddingLeft: '',
	      paddingRight: ''
	    })
	  }

	  Modal.prototype.checkScrollbar = function () {
	    var fullWindowWidth = window.innerWidth
	    if (!fullWindowWidth) { // workaround for missing window.innerWidth in IE8
	      var documentElementRect = document.documentElement.getBoundingClientRect()
	      fullWindowWidth = documentElementRect.right - Math.abs(documentElementRect.left)
	    }
	    this.bodyIsOverflowing = document.body.clientWidth < fullWindowWidth
	    this.scrollbarWidth = this.measureScrollbar()
	  }

	  Modal.prototype.setScrollbar = function () {
	    var bodyPad = parseInt((this.$body.css('padding-right') || 0), 10)
	    this.originalBodyPad = document.body.style.paddingRight || ''
	    if (this.bodyIsOverflowing) this.$body.css('padding-right', bodyPad + this.scrollbarWidth)
	  }

	  Modal.prototype.resetScrollbar = function () {
	    this.$body.css('padding-right', this.originalBodyPad)
	  }

	  Modal.prototype.measureScrollbar = function () { // thx walsh
	    var scrollDiv = document.createElement('div')
	    scrollDiv.className = 'modal-scrollbar-measure'
	    this.$body.append(scrollDiv)
	    var scrollbarWidth = scrollDiv.offsetWidth - scrollDiv.clientWidth
	    this.$body[0].removeChild(scrollDiv)
	    return scrollbarWidth
	  }


	  // MODAL PLUGIN DEFINITION
	  // =======================

	  function Plugin(option, _relatedTarget) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.modal')
	      var options = $.extend({}, Modal.DEFAULTS, $this.data(), typeof option == 'object' && option)

	      if (!data) $this.data('bs.modal', (data = new Modal(this, options)))
	      if (typeof option == 'string') data[option](_relatedTarget)
	      else if (options.show) data.show(_relatedTarget)
	    })
	  }

	  var old = $.fn.modal

	  $.fn.modal             = Plugin
	  $.fn.modal.Constructor = Modal


	  // MODAL NO CONFLICT
	  // =================

	  $.fn.modal.noConflict = function () {
	    $.fn.modal = old
	    return this
	  }


	  // MODAL DATA-API
	  // ==============

	  $(document).on('click.bs.modal.data-api', '[data-toggle="modal"]', function (e) {
	    var $this   = $(this)
	    var href    = $this.attr('href')
	    var $target = $($this.attr('data-target') || (href && href.replace(/.*(?=#[^\s]+$)/, ''))) // strip for ie7
	    var option  = $target.data('bs.modal') ? 'toggle' : $.extend({ remote: !/#/.test(href) && href }, $target.data(), $this.data())

	    if ($this.is('a')) e.preventDefault()

	    $target.one('show.bs.modal', function (showEvent) {
	      if (showEvent.isDefaultPrevented()) return // only register focus restorer if modal will actually get shown
	      $target.one('hidden.bs.modal', function () {
	        $this.is(':visible') && $this.trigger('focus')
	      })
	    })
	    Plugin.call($target, option, this)
	  })

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: tooltip.js v3.3.7
	 * http://getbootstrap.com/javascript/#tooltip
	 * Inspired by the original jQuery.tipsy by Jason Frame
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // TOOLTIP PUBLIC CLASS DEFINITION
	  // ===============================

	  var Tooltip = function (element, options) {
	    this.type       = null
	    this.options    = null
	    this.enabled    = null
	    this.timeout    = null
	    this.hoverState = null
	    this.$element   = null
	    this.inState    = null

	    this.init('tooltip', element, options)
	  }

	  Tooltip.VERSION  = '3.3.7'

	  Tooltip.TRANSITION_DURATION = 150

	  Tooltip.DEFAULTS = {
	    animation: true,
	    placement: 'top',
	    selector: false,
	    template: '<div class="tooltip" role="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>',
	    trigger: 'hover focus',
	    title: '',
	    delay: 0,
	    html: false,
	    container: false,
	    viewport: {
	      selector: 'body',
	      padding: 0
	    }
	  }

	  Tooltip.prototype.init = function (type, element, options) {
	    this.enabled   = true
	    this.type      = type
	    this.$element  = $(element)
	    this.options   = this.getOptions(options)
	    this.$viewport = this.options.viewport && $($.isFunction(this.options.viewport) ? this.options.viewport.call(this, this.$element) : (this.options.viewport.selector || this.options.viewport))
	    this.inState   = { click: false, hover: false, focus: false }

	    if (this.$element[0] instanceof document.constructor && !this.options.selector) {
	      throw new Error('`selector` option must be specified when initializing ' + this.type + ' on the window.document object!')
	    }

	    var triggers = this.options.trigger.split(' ')

	    for (var i = triggers.length; i--;) {
	      var trigger = triggers[i]

	      if (trigger == 'click') {
	        this.$element.on('click.' + this.type, this.options.selector, $.proxy(this.toggle, this))
	      } else if (trigger != 'manual') {
	        var eventIn  = trigger == 'hover' ? 'mouseenter' : 'focusin'
	        var eventOut = trigger == 'hover' ? 'mouseleave' : 'focusout'

	        this.$element.on(eventIn  + '.' + this.type, this.options.selector, $.proxy(this.enter, this))
	        this.$element.on(eventOut + '.' + this.type, this.options.selector, $.proxy(this.leave, this))
	      }
	    }

	    this.options.selector ?
	      (this._options = $.extend({}, this.options, { trigger: 'manual', selector: '' })) :
	      this.fixTitle()
	  }

	  Tooltip.prototype.getDefaults = function () {
	    return Tooltip.DEFAULTS
	  }

	  Tooltip.prototype.getOptions = function (options) {
	    options = $.extend({}, this.getDefaults(), this.$element.data(), options)

	    if (options.delay && typeof options.delay == 'number') {
	      options.delay = {
	        show: options.delay,
	        hide: options.delay
	      }
	    }

	    return options
	  }

	  Tooltip.prototype.getDelegateOptions = function () {
	    var options  = {}
	    var defaults = this.getDefaults()

	    this._options && $.each(this._options, function (key, value) {
	      if (defaults[key] != value) options[key] = value
	    })

	    return options
	  }

	  Tooltip.prototype.enter = function (obj) {
	    var self = obj instanceof this.constructor ?
	      obj : $(obj.currentTarget).data('bs.' + this.type)

	    if (!self) {
	      self = new this.constructor(obj.currentTarget, this.getDelegateOptions())
	      $(obj.currentTarget).data('bs.' + this.type, self)
	    }

	    if (obj instanceof $.Event) {
	      self.inState[obj.type == 'focusin' ? 'focus' : 'hover'] = true
	    }

	    if (self.tip().hasClass('in') || self.hoverState == 'in') {
	      self.hoverState = 'in'
	      return
	    }

	    clearTimeout(self.timeout)

	    self.hoverState = 'in'

	    if (!self.options.delay || !self.options.delay.show) return self.show()

	    self.timeout = setTimeout(function () {
	      if (self.hoverState == 'in') self.show()
	    }, self.options.delay.show)
	  }

	  Tooltip.prototype.isInStateTrue = function () {
	    for (var key in this.inState) {
	      if (this.inState[key]) return true
	    }

	    return false
	  }

	  Tooltip.prototype.leave = function (obj) {
	    var self = obj instanceof this.constructor ?
	      obj : $(obj.currentTarget).data('bs.' + this.type)

	    if (!self) {
	      self = new this.constructor(obj.currentTarget, this.getDelegateOptions())
	      $(obj.currentTarget).data('bs.' + this.type, self)
	    }

	    if (obj instanceof $.Event) {
	      self.inState[obj.type == 'focusout' ? 'focus' : 'hover'] = false
	    }

	    if (self.isInStateTrue()) return

	    clearTimeout(self.timeout)

	    self.hoverState = 'out'

	    if (!self.options.delay || !self.options.delay.hide) return self.hide()

	    self.timeout = setTimeout(function () {
	      if (self.hoverState == 'out') self.hide()
	    }, self.options.delay.hide)
	  }

	  Tooltip.prototype.show = function () {
	    var e = $.Event('show.bs.' + this.type)

	    if (this.hasContent() && this.enabled) {
	      this.$element.trigger(e)

	      var inDom = $.contains(this.$element[0].ownerDocument.documentElement, this.$element[0])
	      if (e.isDefaultPrevented() || !inDom) return
	      var that = this

	      var $tip = this.tip()

	      var tipId = this.getUID(this.type)

	      this.setContent()
	      $tip.attr('id', tipId)
	      this.$element.attr('aria-describedby', tipId)

	      if (this.options.animation) $tip.addClass('fade')

	      var placement = typeof this.options.placement == 'function' ?
	        this.options.placement.call(this, $tip[0], this.$element[0]) :
	        this.options.placement

	      var autoToken = /\s?auto?\s?/i
	      var autoPlace = autoToken.test(placement)
	      if (autoPlace) placement = placement.replace(autoToken, '') || 'top'

	      $tip
	        .detach()
	        .css({ top: 0, left: 0, display: 'block' })
	        .addClass(placement)
	        .data('bs.' + this.type, this)

	      this.options.container ? $tip.appendTo(this.options.container) : $tip.insertAfter(this.$element)
	      this.$element.trigger('inserted.bs.' + this.type)

	      var pos          = this.getPosition()
	      var actualWidth  = $tip[0].offsetWidth
	      var actualHeight = $tip[0].offsetHeight

	      if (autoPlace) {
	        var orgPlacement = placement
	        var viewportDim = this.getPosition(this.$viewport)

	        placement = placement == 'bottom' && pos.bottom + actualHeight > viewportDim.bottom ? 'top'    :
	                    placement == 'top'    && pos.top    - actualHeight < viewportDim.top    ? 'bottom' :
	                    placement == 'right'  && pos.right  + actualWidth  > viewportDim.width  ? 'left'   :
	                    placement == 'left'   && pos.left   - actualWidth  < viewportDim.left   ? 'right'  :
	                    placement

	        $tip
	          .removeClass(orgPlacement)
	          .addClass(placement)
	      }

	      var calculatedOffset = this.getCalculatedOffset(placement, pos, actualWidth, actualHeight)

	      this.applyPlacement(calculatedOffset, placement)

	      var complete = function () {
	        var prevHoverState = that.hoverState
	        that.$element.trigger('shown.bs.' + that.type)
	        that.hoverState = null

	        if (prevHoverState == 'out') that.leave(that)
	      }

	      $.support.transition && this.$tip.hasClass('fade') ?
	        $tip
	          .one('bsTransitionEnd', complete)
	          .emulateTransitionEnd(Tooltip.TRANSITION_DURATION) :
	        complete()
	    }
	  }

	  Tooltip.prototype.applyPlacement = function (offset, placement) {
	    var $tip   = this.tip()
	    var width  = $tip[0].offsetWidth
	    var height = $tip[0].offsetHeight

	    // manually read margins because getBoundingClientRect includes difference
	    var marginTop = parseInt($tip.css('margin-top'), 10)
	    var marginLeft = parseInt($tip.css('margin-left'), 10)

	    // we must check for NaN for ie 8/9
	    if (isNaN(marginTop))  marginTop  = 0
	    if (isNaN(marginLeft)) marginLeft = 0

	    offset.top  += marginTop
	    offset.left += marginLeft

	    // $.fn.offset doesn't round pixel values
	    // so we use setOffset directly with our own function B-0
	    $.offset.setOffset($tip[0], $.extend({
	      using: function (props) {
	        $tip.css({
	          top: Math.round(props.top),
	          left: Math.round(props.left)
	        })
	      }
	    }, offset), 0)

	    $tip.addClass('in')

	    // check to see if placing tip in new offset caused the tip to resize itself
	    var actualWidth  = $tip[0].offsetWidth
	    var actualHeight = $tip[0].offsetHeight

	    if (placement == 'top' && actualHeight != height) {
	      offset.top = offset.top + height - actualHeight
	    }

	    var delta = this.getViewportAdjustedDelta(placement, offset, actualWidth, actualHeight)

	    if (delta.left) offset.left += delta.left
	    else offset.top += delta.top

	    var isVertical          = /top|bottom/.test(placement)
	    var arrowDelta          = isVertical ? delta.left * 2 - width + actualWidth : delta.top * 2 - height + actualHeight
	    var arrowOffsetPosition = isVertical ? 'offsetWidth' : 'offsetHeight'

	    $tip.offset(offset)
	    this.replaceArrow(arrowDelta, $tip[0][arrowOffsetPosition], isVertical)
	  }

	  Tooltip.prototype.replaceArrow = function (delta, dimension, isVertical) {
	    this.arrow()
	      .css(isVertical ? 'left' : 'top', 50 * (1 - delta / dimension) + '%')
	      .css(isVertical ? 'top' : 'left', '')
	  }

	  Tooltip.prototype.setContent = function () {
	    var $tip  = this.tip()
	    var title = this.getTitle()

	    $tip.find('.tooltip-inner')[this.options.html ? 'html' : 'text'](title)
	    $tip.removeClass('fade in top bottom left right')
	  }

	  Tooltip.prototype.hide = function (callback) {
	    var that = this
	    var $tip = $(this.$tip)
	    var e    = $.Event('hide.bs.' + this.type)

	    function complete() {
	      if (that.hoverState != 'in') $tip.detach()
	      if (that.$element) { // TODO: Check whether guarding this code with this `if` is really necessary.
	        that.$element
	          .removeAttr('aria-describedby')
	          .trigger('hidden.bs.' + that.type)
	      }
	      callback && callback()
	    }

	    this.$element.trigger(e)

	    if (e.isDefaultPrevented()) return

	    $tip.removeClass('in')

	    $.support.transition && $tip.hasClass('fade') ?
	      $tip
	        .one('bsTransitionEnd', complete)
	        .emulateTransitionEnd(Tooltip.TRANSITION_DURATION) :
	      complete()

	    this.hoverState = null

	    return this
	  }

	  Tooltip.prototype.fixTitle = function () {
	    var $e = this.$element
	    if ($e.attr('title') || typeof $e.attr('data-original-title') != 'string') {
	      $e.attr('data-original-title', $e.attr('title') || '').attr('title', '')
	    }
	  }

	  Tooltip.prototype.hasContent = function () {
	    return this.getTitle()
	  }

	  Tooltip.prototype.getPosition = function ($element) {
	    $element   = $element || this.$element

	    var el     = $element[0]
	    var isBody = el.tagName == 'BODY'

	    var elRect    = el.getBoundingClientRect()
	    if (elRect.width == null) {
	      // width and height are missing in IE8, so compute them manually; see https://github.com/twbs/bootstrap/issues/14093
	      elRect = $.extend({}, elRect, { width: elRect.right - elRect.left, height: elRect.bottom - elRect.top })
	    }
	    var isSvg = window.SVGElement && el instanceof window.SVGElement
	    // Avoid using $.offset() on SVGs since it gives incorrect results in jQuery 3.
	    // See https://github.com/twbs/bootstrap/issues/20280
	    var elOffset  = isBody ? { top: 0, left: 0 } : (isSvg ? null : $element.offset())
	    var scroll    = { scroll: isBody ? document.documentElement.scrollTop || document.body.scrollTop : $element.scrollTop() }
	    var outerDims = isBody ? { width: $(window).width(), height: $(window).height() } : null

	    return $.extend({}, elRect, scroll, outerDims, elOffset)
	  }

	  Tooltip.prototype.getCalculatedOffset = function (placement, pos, actualWidth, actualHeight) {
	    return placement == 'bottom' ? { top: pos.top + pos.height,   left: pos.left + pos.width / 2 - actualWidth / 2 } :
	           placement == 'top'    ? { top: pos.top - actualHeight, left: pos.left + pos.width / 2 - actualWidth / 2 } :
	           placement == 'left'   ? { top: pos.top + pos.height / 2 - actualHeight / 2, left: pos.left - actualWidth } :
	        /* placement == 'right' */ { top: pos.top + pos.height / 2 - actualHeight / 2, left: pos.left + pos.width }

	  }

	  Tooltip.prototype.getViewportAdjustedDelta = function (placement, pos, actualWidth, actualHeight) {
	    var delta = { top: 0, left: 0 }
	    if (!this.$viewport) return delta

	    var viewportPadding = this.options.viewport && this.options.viewport.padding || 0
	    var viewportDimensions = this.getPosition(this.$viewport)

	    if (/right|left/.test(placement)) {
	      var topEdgeOffset    = pos.top - viewportPadding - viewportDimensions.scroll
	      var bottomEdgeOffset = pos.top + viewportPadding - viewportDimensions.scroll + actualHeight
	      if (topEdgeOffset < viewportDimensions.top) { // top overflow
	        delta.top = viewportDimensions.top - topEdgeOffset
	      } else if (bottomEdgeOffset > viewportDimensions.top + viewportDimensions.height) { // bottom overflow
	        delta.top = viewportDimensions.top + viewportDimensions.height - bottomEdgeOffset
	      }
	    } else {
	      var leftEdgeOffset  = pos.left - viewportPadding
	      var rightEdgeOffset = pos.left + viewportPadding + actualWidth
	      if (leftEdgeOffset < viewportDimensions.left) { // left overflow
	        delta.left = viewportDimensions.left - leftEdgeOffset
	      } else if (rightEdgeOffset > viewportDimensions.right) { // right overflow
	        delta.left = viewportDimensions.left + viewportDimensions.width - rightEdgeOffset
	      }
	    }

	    return delta
	  }

	  Tooltip.prototype.getTitle = function () {
	    var title
	    var $e = this.$element
	    var o  = this.options

	    title = $e.attr('data-original-title')
	      || (typeof o.title == 'function' ? o.title.call($e[0]) :  o.title)

	    return title
	  }

	  Tooltip.prototype.getUID = function (prefix) {
	    do prefix += ~~(Math.random() * 1000000)
	    while (document.getElementById(prefix))
	    return prefix
	  }

	  Tooltip.prototype.tip = function () {
	    if (!this.$tip) {
	      this.$tip = $(this.options.template)
	      if (this.$tip.length != 1) {
	        throw new Error(this.type + ' `template` option must consist of exactly 1 top-level element!')
	      }
	    }
	    return this.$tip
	  }

	  Tooltip.prototype.arrow = function () {
	    return (this.$arrow = this.$arrow || this.tip().find('.tooltip-arrow'))
	  }

	  Tooltip.prototype.enable = function () {
	    this.enabled = true
	  }

	  Tooltip.prototype.disable = function () {
	    this.enabled = false
	  }

	  Tooltip.prototype.toggleEnabled = function () {
	    this.enabled = !this.enabled
	  }

	  Tooltip.prototype.toggle = function (e) {
	    var self = this
	    if (e) {
	      self = $(e.currentTarget).data('bs.' + this.type)
	      if (!self) {
	        self = new this.constructor(e.currentTarget, this.getDelegateOptions())
	        $(e.currentTarget).data('bs.' + this.type, self)
	      }
	    }

	    if (e) {
	      self.inState.click = !self.inState.click
	      if (self.isInStateTrue()) self.enter(self)
	      else self.leave(self)
	    } else {
	      self.tip().hasClass('in') ? self.leave(self) : self.enter(self)
	    }
	  }

	  Tooltip.prototype.destroy = function () {
	    var that = this
	    clearTimeout(this.timeout)
	    this.hide(function () {
	      that.$element.off('.' + that.type).removeData('bs.' + that.type)
	      if (that.$tip) {
	        that.$tip.detach()
	      }
	      that.$tip = null
	      that.$arrow = null
	      that.$viewport = null
	      that.$element = null
	    })
	  }


	  // TOOLTIP PLUGIN DEFINITION
	  // =========================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.tooltip')
	      var options = typeof option == 'object' && option

	      if (!data && /destroy|hide/.test(option)) return
	      if (!data) $this.data('bs.tooltip', (data = new Tooltip(this, options)))
	      if (typeof option == 'string') data[option]()
	    })
	  }

	  var old = $.fn.tooltip

	  $.fn.tooltip             = Plugin
	  $.fn.tooltip.Constructor = Tooltip


	  // TOOLTIP NO CONFLICT
	  // ===================

	  $.fn.tooltip.noConflict = function () {
	    $.fn.tooltip = old
	    return this
	  }

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: popover.js v3.3.7
	 * http://getbootstrap.com/javascript/#popovers
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // POPOVER PUBLIC CLASS DEFINITION
	  // ===============================

	  var Popover = function (element, options) {
	    this.init('popover', element, options)
	  }

	  if (!$.fn.tooltip) throw new Error('Popover requires tooltip.js')

	  Popover.VERSION  = '3.3.7'

	  Popover.DEFAULTS = $.extend({}, $.fn.tooltip.Constructor.DEFAULTS, {
	    placement: 'right',
	    trigger: 'click',
	    content: '',
	    template: '<div class="popover" role="tooltip"><div class="arrow"></div><h3 class="popover-title"></h3><div class="popover-content"></div></div>'
	  })


	  // NOTE: POPOVER EXTENDS tooltip.js
	  // ================================

	  Popover.prototype = $.extend({}, $.fn.tooltip.Constructor.prototype)

	  Popover.prototype.constructor = Popover

	  Popover.prototype.getDefaults = function () {
	    return Popover.DEFAULTS
	  }

	  Popover.prototype.setContent = function () {
	    var $tip    = this.tip()
	    var title   = this.getTitle()
	    var content = this.getContent()

	    $tip.find('.popover-title')[this.options.html ? 'html' : 'text'](title)
	    $tip.find('.popover-content').children().detach().end()[ // we use append for html objects to maintain js events
	      this.options.html ? (typeof content == 'string' ? 'html' : 'append') : 'text'
	    ](content)

	    $tip.removeClass('fade top bottom left right in')

	    // IE8 doesn't accept hiding via the `:empty` pseudo selector, we have to do
	    // this manually by checking the contents.
	    if (!$tip.find('.popover-title').html()) $tip.find('.popover-title').hide()
	  }

	  Popover.prototype.hasContent = function () {
	    return this.getTitle() || this.getContent()
	  }

	  Popover.prototype.getContent = function () {
	    var $e = this.$element
	    var o  = this.options

	    return $e.attr('data-content')
	      || (typeof o.content == 'function' ?
	            o.content.call($e[0]) :
	            o.content)
	  }

	  Popover.prototype.arrow = function () {
	    return (this.$arrow = this.$arrow || this.tip().find('.arrow'))
	  }


	  // POPOVER PLUGIN DEFINITION
	  // =========================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.popover')
	      var options = typeof option == 'object' && option

	      if (!data && /destroy|hide/.test(option)) return
	      if (!data) $this.data('bs.popover', (data = new Popover(this, options)))
	      if (typeof option == 'string') data[option]()
	    })
	  }

	  var old = $.fn.popover

	  $.fn.popover             = Plugin
	  $.fn.popover.Constructor = Popover


	  // POPOVER NO CONFLICT
	  // ===================

	  $.fn.popover.noConflict = function () {
	    $.fn.popover = old
	    return this
	  }

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: scrollspy.js v3.3.7
	 * http://getbootstrap.com/javascript/#scrollspy
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // SCROLLSPY CLASS DEFINITION
	  // ==========================

	  function ScrollSpy(element, options) {
	    this.$body          = $(document.body)
	    this.$scrollElement = $(element).is(document.body) ? $(window) : $(element)
	    this.options        = $.extend({}, ScrollSpy.DEFAULTS, options)
	    this.selector       = (this.options.target || '') + ' .nav li > a'
	    this.offsets        = []
	    this.targets        = []
	    this.activeTarget   = null
	    this.scrollHeight   = 0

	    this.$scrollElement.on('scroll.bs.scrollspy', $.proxy(this.process, this))
	    this.refresh()
	    this.process()
	  }

	  ScrollSpy.VERSION  = '3.3.7'

	  ScrollSpy.DEFAULTS = {
	    offset: 10
	  }

	  ScrollSpy.prototype.getScrollHeight = function () {
	    return this.$scrollElement[0].scrollHeight || Math.max(this.$body[0].scrollHeight, document.documentElement.scrollHeight)
	  }

	  ScrollSpy.prototype.refresh = function () {
	    var that          = this
	    var offsetMethod  = 'offset'
	    var offsetBase    = 0

	    this.offsets      = []
	    this.targets      = []
	    this.scrollHeight = this.getScrollHeight()

	    if (!$.isWindow(this.$scrollElement[0])) {
	      offsetMethod = 'position'
	      offsetBase   = this.$scrollElement.scrollTop()
	    }

	    this.$body
	      .find(this.selector)
	      .map(function () {
	        var $el   = $(this)
	        var href  = $el.data('target') || $el.attr('href')
	        var $href = /^#./.test(href) && $(href)

	        return ($href
	          && $href.length
	          && $href.is(':visible')
	          && [[$href[offsetMethod]().top + offsetBase, href]]) || null
	      })
	      .sort(function (a, b) { return a[0] - b[0] })
	      .each(function () {
	        that.offsets.push(this[0])
	        that.targets.push(this[1])
	      })
	  }

	  ScrollSpy.prototype.process = function () {
	    var scrollTop    = this.$scrollElement.scrollTop() + this.options.offset
	    var scrollHeight = this.getScrollHeight()
	    var maxScroll    = this.options.offset + scrollHeight - this.$scrollElement.height()
	    var offsets      = this.offsets
	    var targets      = this.targets
	    var activeTarget = this.activeTarget
	    var i

	    if (this.scrollHeight != scrollHeight) {
	      this.refresh()
	    }

	    if (scrollTop >= maxScroll) {
	      return activeTarget != (i = targets[targets.length - 1]) && this.activate(i)
	    }

	    if (activeTarget && scrollTop < offsets[0]) {
	      this.activeTarget = null
	      return this.clear()
	    }

	    for (i = offsets.length; i--;) {
	      activeTarget != targets[i]
	        && scrollTop >= offsets[i]
	        && (offsets[i + 1] === undefined || scrollTop < offsets[i + 1])
	        && this.activate(targets[i])
	    }
	  }

	  ScrollSpy.prototype.activate = function (target) {
	    this.activeTarget = target

	    this.clear()

	    var selector = this.selector +
	      '[data-target="' + target + '"],' +
	      this.selector + '[href="' + target + '"]'

	    var active = $(selector)
	      .parents('li')
	      .addClass('active')

	    if (active.parent('.dropdown-menu').length) {
	      active = active
	        .closest('li.dropdown')
	        .addClass('active')
	    }

	    active.trigger('activate.bs.scrollspy')
	  }

	  ScrollSpy.prototype.clear = function () {
	    $(this.selector)
	      .parentsUntil(this.options.target, '.active')
	      .removeClass('active')
	  }


	  // SCROLLSPY PLUGIN DEFINITION
	  // ===========================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.scrollspy')
	      var options = typeof option == 'object' && option

	      if (!data) $this.data('bs.scrollspy', (data = new ScrollSpy(this, options)))
	      if (typeof option == 'string') data[option]()
	    })
	  }

	  var old = $.fn.scrollspy

	  $.fn.scrollspy             = Plugin
	  $.fn.scrollspy.Constructor = ScrollSpy


	  // SCROLLSPY NO CONFLICT
	  // =====================

	  $.fn.scrollspy.noConflict = function () {
	    $.fn.scrollspy = old
	    return this
	  }


	  // SCROLLSPY DATA-API
	  // ==================

	  $(window).on('load.bs.scrollspy.data-api', function () {
	    $('[data-spy="scroll"]').each(function () {
	      var $spy = $(this)
	      Plugin.call($spy, $spy.data())
	    })
	  })

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: tab.js v3.3.7
	 * http://getbootstrap.com/javascript/#tabs
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // TAB CLASS DEFINITION
	  // ====================

	  var Tab = function (element) {
	    // jscs:disable requireDollarBeforejQueryAssignment
	    this.element = $(element)
	    // jscs:enable requireDollarBeforejQueryAssignment
	  }

	  Tab.VERSION = '3.3.7'

	  Tab.TRANSITION_DURATION = 150

	  Tab.prototype.show = function () {
	    var $this    = this.element
	    var $ul      = $this.closest('ul:not(.dropdown-menu)')
	    var selector = $this.data('target')

	    if (!selector) {
	      selector = $this.attr('href')
	      selector = selector && selector.replace(/.*(?=#[^\s]*$)/, '') // strip for ie7
	    }

	    if ($this.parent('li').hasClass('active')) return

	    var $previous = $ul.find('.active:last a')
	    var hideEvent = $.Event('hide.bs.tab', {
	      relatedTarget: $this[0]
	    })
	    var showEvent = $.Event('show.bs.tab', {
	      relatedTarget: $previous[0]
	    })

	    $previous.trigger(hideEvent)
	    $this.trigger(showEvent)

	    if (showEvent.isDefaultPrevented() || hideEvent.isDefaultPrevented()) return

	    var $target = $(selector)

	    this.activate($this.closest('li'), $ul)
	    this.activate($target, $target.parent(), function () {
	      $previous.trigger({
	        type: 'hidden.bs.tab',
	        relatedTarget: $this[0]
	      })
	      $this.trigger({
	        type: 'shown.bs.tab',
	        relatedTarget: $previous[0]
	      })
	    })
	  }

	  Tab.prototype.activate = function (element, container, callback) {
	    var $active    = container.find('> .active')
	    var transition = callback
	      && $.support.transition
	      && ($active.length && $active.hasClass('fade') || !!container.find('> .fade').length)

	    function next() {
	      $active
	        .removeClass('active')
	        .find('> .dropdown-menu > .active')
	          .removeClass('active')
	        .end()
	        .find('[data-toggle="tab"]')
	          .attr('aria-expanded', false)

	      element
	        .addClass('active')
	        .find('[data-toggle="tab"]')
	          .attr('aria-expanded', true)

	      if (transition) {
	        element[0].offsetWidth // reflow for transition
	        element.addClass('in')
	      } else {
	        element.removeClass('fade')
	      }

	      if (element.parent('.dropdown-menu').length) {
	        element
	          .closest('li.dropdown')
	            .addClass('active')
	          .end()
	          .find('[data-toggle="tab"]')
	            .attr('aria-expanded', true)
	      }

	      callback && callback()
	    }

	    $active.length && transition ?
	      $active
	        .one('bsTransitionEnd', next)
	        .emulateTransitionEnd(Tab.TRANSITION_DURATION) :
	      next()

	    $active.removeClass('in')
	  }


	  // TAB PLUGIN DEFINITION
	  // =====================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this = $(this)
	      var data  = $this.data('bs.tab')

	      if (!data) $this.data('bs.tab', (data = new Tab(this)))
	      if (typeof option == 'string') data[option]()
	    })
	  }

	  var old = $.fn.tab

	  $.fn.tab             = Plugin
	  $.fn.tab.Constructor = Tab


	  // TAB NO CONFLICT
	  // ===============

	  $.fn.tab.noConflict = function () {
	    $.fn.tab = old
	    return this
	  }


	  // TAB DATA-API
	  // ============

	  var clickHandler = function (e) {
	    e.preventDefault()
	    Plugin.call($(this), 'show')
	  }

	  $(document)
	    .on('click.bs.tab.data-api', '[data-toggle="tab"]', clickHandler)
	    .on('click.bs.tab.data-api', '[data-toggle="pill"]', clickHandler)

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: affix.js v3.3.7
	 * http://getbootstrap.com/javascript/#affix
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // AFFIX CLASS DEFINITION
	  // ======================

	  var Affix = function (element, options) {
	    this.options = $.extend({}, Affix.DEFAULTS, options)

	    this.$target = $(this.options.target)
	      .on('scroll.bs.affix.data-api', $.proxy(this.checkPosition, this))
	      .on('click.bs.affix.data-api',  $.proxy(this.checkPositionWithEventLoop, this))

	    this.$element     = $(element)
	    this.affixed      = null
	    this.unpin        = null
	    this.pinnedOffset = null

	    this.checkPosition()
	  }

	  Affix.VERSION  = '3.3.7'

	  Affix.RESET    = 'affix affix-top affix-bottom'

	  Affix.DEFAULTS = {
	    offset: 0,
	    target: window
	  }

	  Affix.prototype.getState = function (scrollHeight, height, offsetTop, offsetBottom) {
	    var scrollTop    = this.$target.scrollTop()
	    var position     = this.$element.offset()
	    var targetHeight = this.$target.height()

	    if (offsetTop != null && this.affixed == 'top') return scrollTop < offsetTop ? 'top' : false

	    if (this.affixed == 'bottom') {
	      if (offsetTop != null) return (scrollTop + this.unpin <= position.top) ? false : 'bottom'
	      return (scrollTop + targetHeight <= scrollHeight - offsetBottom) ? false : 'bottom'
	    }

	    var initializing   = this.affixed == null
	    var colliderTop    = initializing ? scrollTop : position.top
	    var colliderHeight = initializing ? targetHeight : height

	    if (offsetTop != null && scrollTop <= offsetTop) return 'top'
	    if (offsetBottom != null && (colliderTop + colliderHeight >= scrollHeight - offsetBottom)) return 'bottom'

	    return false
	  }

	  Affix.prototype.getPinnedOffset = function () {
	    if (this.pinnedOffset) return this.pinnedOffset
	    this.$element.removeClass(Affix.RESET).addClass('affix')
	    var scrollTop = this.$target.scrollTop()
	    var position  = this.$element.offset()
	    return (this.pinnedOffset = position.top - scrollTop)
	  }

	  Affix.prototype.checkPositionWithEventLoop = function () {
	    setTimeout($.proxy(this.checkPosition, this), 1)
	  }

	  Affix.prototype.checkPosition = function () {
	    if (!this.$element.is(':visible')) return

	    var height       = this.$element.height()
	    var offset       = this.options.offset
	    var offsetTop    = offset.top
	    var offsetBottom = offset.bottom
	    var scrollHeight = Math.max($(document).height(), $(document.body).height())

	    if (typeof offset != 'object')         offsetBottom = offsetTop = offset
	    if (typeof offsetTop == 'function')    offsetTop    = offset.top(this.$element)
	    if (typeof offsetBottom == 'function') offsetBottom = offset.bottom(this.$element)

	    var affix = this.getState(scrollHeight, height, offsetTop, offsetBottom)

	    if (this.affixed != affix) {
	      if (this.unpin != null) this.$element.css('top', '')

	      var affixType = 'affix' + (affix ? '-' + affix : '')
	      var e         = $.Event(affixType + '.bs.affix')

	      this.$element.trigger(e)

	      if (e.isDefaultPrevented()) return

	      this.affixed = affix
	      this.unpin = affix == 'bottom' ? this.getPinnedOffset() : null

	      this.$element
	        .removeClass(Affix.RESET)
	        .addClass(affixType)
	        .trigger(affixType.replace('affix', 'affixed') + '.bs.affix')
	    }

	    if (affix == 'bottom') {
	      this.$element.offset({
	        top: scrollHeight - height - offsetBottom
	      })
	    }
	  }


	  // AFFIX PLUGIN DEFINITION
	  // =======================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.affix')
	      var options = typeof option == 'object' && option

	      if (!data) $this.data('bs.affix', (data = new Affix(this, options)))
	      if (typeof option == 'string') data[option]()
	    })
	  }

	  var old = $.fn.affix

	  $.fn.affix             = Plugin
	  $.fn.affix.Constructor = Affix


	  // AFFIX NO CONFLICT
	  // =================

	  $.fn.affix.noConflict = function () {
	    $.fn.affix = old
	    return this
	  }


	  // AFFIX DATA-API
	  // ==============

	  $(window).on('load', function () {
	    $('[data-spy="affix"]').each(function () {
	      var $spy = $(this)
	      var data = $spy.data()

	      data.offset = data.offset || {}

	      if (data.offsetBottom != null) data.offset.bottom = data.offsetBottom
	      if (data.offsetTop    != null) data.offset.top    = data.offsetTop

	      Plugin.call($spy, data)
	    })
	  })

	}(jQuery);

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

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

/***/ 85:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {function FacilityItem( data ) {
	  this.FacilityName = data.FacilityName;
	  this.ShippingLabel = data.ShippingLabel;

	  this.FacilityKey = data.FacilityKey;
	  this.Active = data.Active;
	  this.COName = data.ShippingLabel.Name;
	  this.PhoneNumber = data.ShippingLabel.PhoneNumber;
	  this.Email = data.ShippingLabel.Email;

	  var _addr = data.ShippingLabel.Address;
	  var _addrItems = [];
	  _addr.AddressLine1 != null && _addrItems.push( _addr.AddressLine1 );
	  _addr.AddressLine2 != null && _addrItems.push( _addr.AddressLine2 );
	  _addr.AddressLine3 != null && _addrItems.push( _addr.AddressLine3 );
	  _addr.City != null && _addrItems.push( _addr.City );
	  _addr.State != null && _addrItems.push( _addr.State );
	  _addr.PostalCode != null && _addrItems.push( _addr.PostalCode );

	  this.FullAddress = _addrItems.join(', ');
	}

	/** Facility Maintenance summaries
	  * @param {Object} input - Observable, input data for summary table
	  * @param {Object} selected - Observable, container for the selected facility
	  * @param {Object} exports - Observable, Public methods
	  */
	function FacilityMaintenanceSummaryVM( params ) {
	  if ( !(this instanceof FacilityMaintenanceSummaryVM) ) { return new FacilityMaintenanceSummaryVM( params ); }

	  var self = this;

	  this.facilities = ko.pureComputed(function() {
	    var _facilities = ko.unwrap( params.input ) || [];

	    return _facilities.map(function( facility ) {
	      return new FacilityItem( facility );
	    });
	  });

	  this.selectedFacility = params.selected;
	  this.selectFacility = function( data ) {
	    if ( data === self.selectedFacility() ) {
	      self.selectedFacility( null );
	      return;
	    }

	    self.selectedFacility( data );
	  };

	  function updateFacility( facilityData ) {
	    var _facilityKey = facilityData.FacilityKey;
	    var _facilities = self.facilities();

	    var _facility = ko.utils.arrayFirst( _facilities, function( facility ) {
	      return facility.FacilityKey === _facilityKey;
	    });

	    if ( _facility ) {
	      var i = _facilities.indexOf( _facility );

	      if ( i > -1 ) {
	        self.facilities.splice( i, 1, new FacilityItem( facilityData ) );

	        return $.Deferred().resolve();
	      }
	    }

	    return addFacility( facilityData );
	  }

	  function addFacility( facilityData ) {
	    self.facilities.unshift( new FacilityItem( facilityData ) );

	    return $.Deferred().resolve();
	  }

	  // Exports
	  if ( params && params.exports ) {
	    params.exports({
	      updateFacility: updateFacility,
	      addFacility: addFacility
	    });
	  }

	  return this;
	}

	module.exports = {
	  viewModel: FacilityMaintenanceSummaryVM,
	  template: __webpack_require__(86)
	};

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 86:
/***/ (function(module, exports) {

	module.exports = "<div class=\"panel panel-default\">\r\n  <div class=\"table-responsive\">\r\n    <table class=\"table table-hover\">\r\n      <thead>\r\n        <tr>\r\n          <th>Facility Name</th>\r\n          <th>Active</th>\r\n          <th>C/O Name</th>\r\n          <th>Phone #</th>\r\n          <th>Email</th>\r\n          <th>Address</th>\r\n        </tr>\r\n      </thead>\r\n      <tbody class=\"clickable\" data-bind=\"foreach: facilities\">\r\n        <tr data-bind=\"css: { 'info': $parent.selectedFacility() === $data }, click: $parent.selectFacility\">\r\n          <td data-bind=\"text: FacilityName\"></td>\r\n          <td><input type=\"checkbox\" data-bind=\"checked: Active, disable: true\"></td>\r\n          <td data-bind=\"text: COName\"></td>\r\n          <td data-bind=\"text: PhoneNumber\"></td>\r\n          <td data-bind=\"text: Email\"></td>\r\n          <td data-bind=\"text: FullAddress\"></td>\r\n        </tr>\r\n      </tbody>\r\n    </table>\r\n  </div>\r\n</div>\r\n"

/***/ }),

/***/ 87:
/***/ (function(module, exports, __webpack_require__) {

	ko.components.register( 'address-editor', __webpack_require__(12) );

	function FacilityEditor( facilityData ) {
	  this.cache = {
	    facilityName: facilityData.FacilityName
	  };

	  this.FacilityKey = facilityData.FacilityKey;
	  this.FacilityName = ko.observable( facilityData.FacilityName ).extend({ required: true });
	  this.FacilityType = ko.observable( facilityData.FacilityType ).extend({ required: true });
	  this.Active = ko.observable( facilityData.Active );

	  var _shipLabel = facilityData.ShippingLabel || {};
	  this.ShippingLabelName = ko.observable( _shipLabel.Name );
	  this.Locations = ko.observableArray( facilityData.Locations );
	  this.Phone = ko.observable( _shipLabel.Phone );
	  this.EMail = ko.observable( _shipLabel.EMail );

	  this.address = {
	    input: ko.observable( _shipLabel.Address ),
	    exports: ko.observable(),
	  };

	  this.validation = ko.validatedObservable({
	    type: this.FacilityType,
	    name: this.FacilityName
	  });
	}

	FacilityEditor.prototype.toDto = function() {
	  return ko.toJS({
	    isNew: this.FacilityKey != null,
	    FacilityKey: this.FacilityKey,
	    FacilityType: this.FacilityType,
	    FacilityName: this.FacilityName,
	    Active: this.Active,
	    PhoneNumber: this.Phone,
	    EMailAddress: this.EMail,
	    ShippingLabelName: this.ShippingLabelName,
	    Address: this.address.exports(),
	  });
	};

	function FacilityMaintenanceEditorVM( params ) {
	  if ( !(this instanceof FacilityMaintenanceEditorVM) ) { return new FacilityMaintenanceEditorVM( params ); }

	  var self = this;
	  this.disposables = [];

	  this.options = params.options;

	  function buildEditor( facilityData ) {
	    return new FacilityEditor( facilityData || {} );
	  }

	  var _editor = buildEditor( ko.toJS( params.input ) );
	  this.editor = ko.observable( _editor );

	  if ( ko.isObservable( params.input ) ) {
	    this.disposables.push( params.input.subscribe(function( newData ) {
	      if ( newData ) {
	        self.editor( buildEditor( newData ) );
	      }
	    }) );
	  }


	  var isValid = ko.pureComputed(function() {
	    var _editor = self.editor();

	    return _editor && _editor.validation.isValid();
	  });

	  function toDto() {
	    var _editor = self.editor();

	    return _editor && _editor.toDto();
	  }

	  // Exports
	  if ( params && params.exports ) {
	    params.exports({
	      isValid: isValid,
	      toDto: toDto
	    });
	  }

	  return this;
	}

	ko.utils.extend(FacilityMaintenanceEditorVM.prototype, {
	  dispose: function() {
	    ko.utils.arrayForEach( this.disposables, this.disposeOne );
	    ko.utils.objectForEach( this, this.disposeOne );
	  },

	  disposeOne: function( propOrValue, value ) {
	    var disposable = value || propOrValue;

	    if ( disposable && typeof disposable.dispose === 'function' ) {
	      disposable.dispose();
	    }
	  }
	});

	module.exports = {
	  viewModel: FacilityMaintenanceEditorVM,
	  template: __webpack_require__(88)
	};


/***/ }),

/***/ 88:
/***/ (function(module, exports) {

	module.exports = "<section data-bind=\"with: editor\">\r\n  <section>\r\n    <div class=\"row\">\r\n      <div class=\"col-sm-6\">\r\n        <div class=\"form-group\" data-bind=\"validationElement: FacilityType\">\r\n          <label class=\"control-label\" for=\"facility-editor-type\">Facility Type</label>\r\n          <select id=\"facility-editor-type\" class=\"form-control\" data-bind=\"value: FacilityType, options: $parent.options.facilityTypes, optionsText: 'value', optionsValue: 'key', optionsCaption: ' '\"></select>\r\n        </div>\r\n      </div>\r\n      <div class=\"col-sm-6\">\r\n        <div class=\"form-group\" data-bind=\"validationElement: FacilityName\">\r\n          <label class=\"control-label\" for=\"facility-editor-name\">Facility Name</label>\r\n          <input id=\"facility-editor-name\" class=\"form-control\" type=\"text\" data-bind=\"textinput: FacilityName\">\r\n        </div>\r\n      </div>\r\n    </div>\r\n    <div class=\"row\">\r\n      <div class=\"col-sm-6\">\r\n        <div class=\"checkbox\">\r\n          <label>\r\n            <input type=\"checkbox\" data-bind=\"checked: Active\"> Active\r\n          </label>\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </section>\r\n  <section>\r\n    <div class=\"row\">\r\n      <div class=\"col-xs-12\">\r\n        <div class=\"panel panel-default\">\r\n          <div class=\"panel-heading\">\r\n            <h4 class=\"panel-title\">Ship To</h4>\r\n          </div>\r\n          <div class=\"panel-body\">\r\n            <div class=\"form-group\">\r\n              <label class=\"control-label\" for=\"facility-editor-co\">C/O Name</label>\r\n              <input id=\"facility-editor-co\" class=\"form-control\" type=\"text\" data-bind=\"textinput: ShippingLabelName\">\r\n            </div>\r\n            <div class=\"form-group\">\r\n              <label class=\"control-label\" for=\"facility-editor-email\">Email</label>\r\n              <input id=\"facility-editor-email\" class=\"form-control\" type=\"email\" data-bind=\"textinput: EMail\">\r\n            </div>\r\n            <div class=\"form-group\">\r\n              <label class=\"control-label\" for=\"facility-editor-phone\">Phone Number</label>\r\n              <input id=\"facility-editor-phone\" class=\"form-control\" type=\"tel\" data-bind=\"textinput: Phone\">\r\n            </div>\r\n            <label class=\"control-label\">Address</label>\r\n            <address-editor params=\"input: address.input, output: address.exports\">\r\n            </address-editor>\r\n          </div>\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </section>\r\n</section>\r\n"

/***/ }),

/***/ 89:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {var warehouseService = __webpack_require__(6);

	function Location( locationData ) {
	  this.cache = locationData;

	  // Static Data
	  this.LocationKey = locationData.LocationKey;
	  this.FacilityKey = locationData.FacilityKey;
	  this.FacilityName = locationData.FacilityName;
	  this.Active = locationData.Active;
	  this.Description = locationData.Description;

	  // Editable data
	  this.GroupName = ko.observable( locationData.GroupName );
	  this.Row = ko.observable( locationData.Row );
	  this.Status = ko.observable( locationData.Status );

	  this.statusIcon = ko.pureComputed(function() {
	    var _status = this.Status();

	    var _statusOption = ko.utils.arrayFirst( this.statusOptions, function( opt ) {
	      return opt.key === _status;
	    } );

	    return _statusOption && _statusOption.icon;
	  }, this );

	  // State
	  this.isEditing = ko.observable( locationData.isEditing || locationData.FacilityKey == null );
	}

	Location.prototype.statusOptions = [{
	  icon: 'fa-check',
	  key: 1,
	  value: 'Available'
	},
	{
	  icon: 'fa-lock',
	  key: 2,
	  value: 'Locked'
	},
	{
	  icon: 'fa-times',
	  key: 3,
	  value: 'Inactive'
	}];

	Location.prototype.toDto = function() {
	  return ko.toJS({
	    FacilityKey: this.FacilityKey,
	    GroupName: this.GroupName,
	    Row: this.Row,
	    Status: this.Status
	  });
	};

	function LocationEditor( locationsData ) {
	  var self = this;

	  this.FacilityKey = locationsData.facilityKey;

	  var locations = (locationsData.locations || []).map(function( location ) {
	    return new Location( location );
	  });

	  this.sortLocations = function( locations ) {
	    locations.sort(function( a, b ) {
	      var _a = a.Row();
	      var _b = b.Row();

	      if ( _a > _b ) {
	        return 1;
	      } else if ( _a < _b ) {
	        return -1;
	      } else {
	        return 0;
	      }
	    });
	  };
	  this.sortLocations( locations );

	  this.locations = ko.observableArray( locations );

	  var _streets = [];
	  ko.utils.arrayForEach( self.locations(), function( location ) {
	    var _street = ko.unwrap( location.GroupName );

	    if ( _street && _streets.indexOf( _street ) === -1 ) {
	      _streets.push( _street );
	    }

	    _streets.sort();
	  });
	  this.streetNames = ko.observableArray( _streets );

	  this.street = ko.observable( this.streetNames()[0] );
	  this.streetLocations = ko.pureComputed(function() {
	    var _locations = self.locations().filter(function( location ) {
	      return location.cache.GroupName === self.street();
	    });

	    return _locations;
	  });

	  this.createStreet = function( streetName ) {
	    var initLocation = {
	      Row: 1,
	      GroupName: streetName,
	      Status: 1,
	      FacilityKey: self.FacilityKey,
	    };

	    return self.createLocation( initLocation ).then(
	      function( data ) {
	      var newLocation = new Location({
	        Row: initLocation.Row,
	        GroupName: initLocation.GroupName,
	        Status: initLocation.Status,
	        FacilityKey: initLocation.FacilityKey,
	        isEditing: true,
	        LocationKey: data
	      });
	      self.locations.push( newLocation );
	      self.streetNames.push( streetName );
	      self.streetNames.sort();
	      self.street( streetName );
	    },
	    function( jqXHR, textStatus, errorThrown ) {
	      showUserMessage( 'Could not create new street', {
	        description: errorThrown
	      });
	    });
	  };

	  this.save = ko.asyncCommand({
	    execute: function( locationData, complete ) {
	      var onSuccess = function() {
	        this.isEditing( false );
	        this.cache.Row = this.Row();
	        this.cache.GroupName = this.GroupName();
	      }.bind( locationData );

	      if ( locationData.FacilityKey != null ) {
	        var _updateData = locationData.toDto();

	        var updateLocation = self.updateLocation( locationData.LocationKey, _updateData, onSuccess ).always(function() {
	          complete();
	          this.locations.valueHasMutated();
	        }.bind( self ));

	        return updateLocation;
	      } else {
	        var _createData = locationData.toDto();
	        _createData.FacilityKey = self.FacilityKey;

	        var createLocation = self.createLocation( _createData, onSuccess ).always(function( data ) {
	          locationData.LocationKey = data;
	          this.locations.valueHasMutated();
	          complete();
	        }.bind( self ));

	        return createLocation;
	      }
	    }
	  });

	  this.addLocation = ko.command({
	    execute: function() {
	      self.locations.push( new Location({
	        GroupName: self.street(),
	      }) );
	    }
	  });

	  this.saveLocation = function( location ) {
	    self.save.execute( location )
	    .done(function() {
	      self.sortLocations( self.locations );
	    });
	  };

	  this.editLocation = ko.command({
	    execute: function( location ) {
	      location.isEditing( true );
	    }
	  });

	  this.cancelEditLocation = ko.command({
	    execute: function( location ) {
	      if ( location.FacilityKey == null ) {
	        var i = self.locations().indexOf( location );

	        self.locations.splice( i, 1 );
	      } else {
	        location.isEditing( false );
	        location.Row( location.cache.Row );
	        location.GroupName( location.cache.GroupName );
	        location.Status( location.cache.Status );
	      }
	    }
	  });
	}

	LocationEditor.prototype.createLocation = function( locationData, successCallback ) {
	  var createLocation = warehouseService.createLocation( locationData ).then(function( data ) {
	    if ( successCallback ) {
	       successCallback();
	    }

	    return data;
	  },
	  function( jqXHR, textStatus, errorThrown ) {
	    showUserMessage( 'Could not save location', {
	      description: errorThrown
	    });
	  });

	  return createLocation;
	};

	LocationEditor.prototype.updateLocation = function( locationKey, locationData, successCallback ) {
	  var updateLocation = warehouseService.updateLocation( locationKey, locationData )
	  .then(function( data ) {
	    if ( successCallback ) {
	       successCallback();
	    }

	    return data;
	  },
	  function( jqXHR, textStatus, errorThrown ) {
	    showUserMessage( 'Could not save location', {
	      description: errorThrown
	    });
	  });

	  return updateLocation;
	};

	LocationEditor.prototype.saveAll = function() {
	  var saves = [];

	  var onSuccess = function() {
	    this.isEditing( false );
	    this.cache.Row = this.Row();
	    this.cache.GroupName = this.GroupName();
	  };

	  var saveLocation = function( location ) {
	    if ( location.FacilityKey != null ) {
	      var _updateData = location.toDto();

	      return this.updateLocation( location.LocationKey, _updateData, onSuccess.bind( location ) );
	    } else {
	      var _createData = location.toDto();
	      _createData.FacilityKey = this.FacilityKey;

	      return this.createLocation( _createData, onSuccess.bind( location ) );
	    }
	  }.bind( this );

	  ko.utils.arrayForEach( this.locations(), function( location ) {
	    if ( location.isEditing() ) {
	      saves.push( saveLocation( location ) );
	    }
	  });

	  var checkSaves = $.when.apply( $, saves ).always(function() {
	    this.locations.valueHasMutated();
	  }.bind( this ));

	  return checkSaves;
	};

	function FacilityMaintenanceLocationEditorVM( params ) {
	  if ( !(this instanceof FacilityMaintenanceLocationEditorVM) ) { return new FacilityMaintenanceLocationEditorVM( params ); }

	  var self = this;

	  function buildEditor( locationsData ) {
	    return new LocationEditor( locationsData || [] );
	  }

	  this.editor = ko.observable( buildEditor( ko.toJS( params.input ) ) );
	  var currentStreet = ko.pureComputed(function() {
	    var _editor = self.editor();

	    return _editor && _editor.street();
	  });

	  var isDirty = ko.pureComputed(function() {
	    var _editor = self.editor();

	    return _editor && ko.utils.arrayFirst( ko.unwrap( _editor.locations() ), function( location ) {
	      return location.isEditing();
	    } );
	  });

	  function loadUpdatedData( newLocationsData ) {
	    var _editor = self.editor();
	    var _street = _editor && _editor.street();

	    self.editor( buildEditor( newLocationsData ) );
	    self.editor().street( _street );
	  }

	  function saveAll() {
	    var _editor = self.editor();

	    return _editor && _editor.saveAll();
	  }

	  function createStreet( streetName ) {
	    return self.editor().createStreet( streetName );
	  }

	  // Exports
	  if ( params && params.exports ) {
	    params.exports({
	      isDirty: isDirty,
	      saveAll: saveAll,
	      currentStreet: currentStreet,
	      loadUpdatedData: loadUpdatedData,
	      createStreet: createStreet,
	    });
	  }

	  return this;
	}

	module.exports = {
	  viewModel: FacilityMaintenanceLocationEditorVM,
	  template: __webpack_require__(90)
	};

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 90:
/***/ (function(module, exports) {

	module.exports = "<section data-bind=\"with: editor\">\r\n  <div class=\"row\">\r\n    <div class=\"col-sm-6\">\r\n      <div class=\"form-group\">\r\n        <label class=\"control-label\" for=\"\">Street</label>\r\n        <select id=\"\" class=\"form-control\" data-bind=\"value: street, options: streetNames\"></select>\r\n      </div>\r\n    </div>\r\n  </div>\r\n  <div class=\"panel panel-default\">\r\n    <div class=\"table-responsive\">\r\n      <table class=\"table table-hover\">\r\n        <thead>\r\n          <tr>\r\n            <th>\r\n              <section class=\"invisible no-wrap\">\r\n                <button class=\"btn btn-link\"><i class=\"fa fa-save fa-fw\"></i></button>\r\n                <button class=\"btn btn-link\"><i class=\"fa fa-times fa-fw\"></i></button>\r\n              </section>\r\n            </th>\r\n            <th style=\"width: 33.333%\">Group</th>\r\n            <th style=\"width: 33.333%\">Row</th>\r\n            <th style=\"width: 33.333%\">Status</th>\r\n          </tr>\r\n        </thead>\r\n        <tbody data-bind=\"foreach: streetLocations\">\r\n          <!-- ko template: isEditing() ? 'location-edit' : 'location-view' --><!-- /ko -->\r\n        </tbody>\r\n        <tfoot>\r\n          <tr>\r\n            <td colspan=\"4\"><button class=\"btn btn-primary btn-block\" data-bind=\"command: addLocation\"><i class=\"fa fa-plus\"></i> Add Location</button></td>\r\n          </tr>\r\n        </tfoot>\r\n      </table>\r\n    </div>\r\n  </div>\r\n</section>\r\n\r\n<script id=\"location-edit\" type=\"text/html\">\r\n  <tr class=\"no-wrap\">\r\n    <td>\r\n      <button class=\"btn btn-link\" title=\"Save edit\" data-bind=\"click: $parent.saveLocation, enable: $parent.save.canExecute\"><i class=\"fa fa-save fa-fw\"></i></button>\r\n      <button class=\"btn btn-link\" title=\"Cancel edit\" data-bind=\"command: $parent.cancelEditLocation\"><i class=\"fa fa-times fa-fw\"></i></button>\r\n    </td>\r\n    <td>\r\n      <select class=\"form-control\" data-bind=\"value: GroupName, options: $parent.streetNames\">\r\n    </td>\r\n    <td>\r\n      <input class=\"form-control\" type=\"number\" data-bind=\"textinput: Row\">\r\n    </td>\r\n    <td>\r\n      <select class=\"form-control\" data-bind=\"value: Status, options: statusOptions, optionsValue: 'key', optionsText: 'value'\">\r\n    </td>\r\n  </tr>\r\n</script>\r\n\r\n<script id=\"location-view\" type=\"text/html\">\r\n  <tr>\r\n    <td>\r\n      <button class=\"btn btn-link\" title=\"Edit location\" data-bind=\"command: $parent.editLocation\"><i class=\"fa fa-edit\"></i></button>\r\n    </td>\r\n    <td><p class=\"form-control-static\" data-bind=\"text: GroupName\"></p></td>\r\n    <td><p class=\"form-control-static\" data-bind=\"text: Row\"></p></td>\r\n    <td><p class=\"form-control-static\"><i class=\"fa fa-fw\" data-bind=\"css: statusIcon\"></i></p></td>\r\n  </tr>\r\n</script>\r\n"

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

/***/ })

});