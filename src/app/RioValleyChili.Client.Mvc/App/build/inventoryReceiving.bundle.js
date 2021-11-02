webpackJsonp([7],{

/***/ 0:
/***/ (function(module, exports, __webpack_require__) {

	var page = __webpack_require__(26);
	var queryBase = __webpack_require__(7);
	ko.components.register('inventory-receiving', __webpack_require__(126));
	ko.components.register('inventory-received', __webpack_require__(128));
	__webpack_require__(32);

	(function() {
	  var vm = {
	    inventoryReceivingViewModel: ko.observable(),
	    currentLot: ko.observable(),
	    searchLot: ko.observable().extend({ lotKey: true })
	  };

	  vm.showReceiving = ko.pureComputed(function() {
	    return vm.currentLot() == null;
	  });
	  vm.showArchived = ko.pureComputed(function() {
	    return vm.currentLot() != null;
	  });

	  vm.searchLotCommand = ko.command({
	    execute: function () {
	      var lot = vm.searchLot();
	      page('/' + lot);
	    },
	    canExecute: function() {
	      return vm.searchLot();
	    }
	  });

	  vm.closeLotCommand = ko.command({
	    execute: function() {
	      page('/');
	    },
	    canExecute: function() {
	      return vm.currentLot() != null;
	    }
	});

	  vm.saveAsyncCommand = ko.asyncCommand({
	    execute: function(done) {
	      var receivingVm = vm.inventoryReceivingViewModel();
	      return receivingVm.saveAsyncCommand.execute()
	        .done(function(lot) {
	          showUserMessage('Inventory Created', { description: 'The inventory has been created successfully. The new inventory lot is <strong>' + lot + '</strong>.' });
	          page( '/' + lot );
	        })
	        .always(done);
	    },
	    canExecute: function(isExecuting) {
	      return !isExecuting && vm.inventoryReceivingViewModel() != null;
	    }
	  });

	  page.base('/Warehouse/Receiving');
	  page('/:lot?', function (ctx) {
	    var lot = ctx.params.lot;
	    if (lot) {
	      loadLotByKey(lot);
	    } else {
	      clearSelection();
	    }
	  });

	  ko.applyBindings(vm);
	  page();

	  function loadLotByKey(lot) {
	    return fetchInventoryReceivedByLot(lot)
	      .done(function (data) {
	        vm.currentLot(data);
	      });
	  }

	  function clearSelection() {
	    vm.currentLot(null);
	  }
	}());


	function fetchInventoryReceivedByLot(lot) {
	  return queryBase.ajax('/api/inventory-received/' + lot);
	}


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

/***/ 16:
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(7), __webpack_require__(8)], __WEBPACK_AMD_DEFINE_RESULT__ = function(core, app) {
	    return {
	        getCompanies: curryGetCompaniesDelegate(),
	        getCompaniesDataPager: getCompaniesDataPager,
	        getCompanyDetails: curryGetCompanyByKeyDelegate,
	        getCompanyData: getCompanyData,
	        getCustomers: curryGetCompaniesDelegate(app.lists.companyTypes.Customer.key),
	        getDehydrators: curryGetCompaniesDelegate(app.lists.companyTypes.Dehydrator.key),
	        getBrokers: curryGetCompaniesDelegate(app.lists.companyTypes.Broker.key),
	        getVendors: function (vendorType) { return core.ajax(getCompaniesUrlBuilder(vendorType)); },
	        getVendorDetails: curryGetCompanyByKeyDelegate,
	        createVendor: createVendor,
	        createCompany: createCompany,
	        updateCompany: updateCompany,
	        getContacts: getContacts,
	        createContact: createContact,
	        updateContact: updateContact,
	        deleteContact: deleteContact,
	        getNoteTypes: getNoteTypes,
	        createNote: createNote,
	        updateNote: updateNote,
	        deleteNote: deleteNote
	    };

	    function createCompany( companyData ) {
	      return core.ajaxPost( '/api/companies/', companyData );
	    }

	    function updateCompany( companyKey, companyData ) {
	      return core.ajaxPut( '/api/companies/' + companyKey, companyData );
	    }

	    function createVendor( data ) {
	      var _data = data;

	      // "1" = Supplier
	      _data.VendorTypes = [1];

	      return core.ajaxPost( '/api/vendors/', _data );
	    }

	    function getCompanyData( companyKey ) {
	      return core.ajax( '/api/companies/' + companyKey );
	    }

	    function getCompaniesDataPager( options ) {
	      options = options || {};
	      return core.pagedDataHelper.init({
	        urlBase: "/api/companies",
	        pageSize: options.pageSize || 50,
	        parameters: options.parameters,
	        resultCounter: function (data) {
	          return data.length;
	        },
	        onNewPageSet: options.onNewPageSet
	      });
	    }
	    function curryGetCompaniesDelegate (companyType) {
	        return function () { return core.ajax(getCompaniesUrlBuilder(companyType)); };
	    }

	    function curryGetCompanyByKeyDelegate(companyKey) {
	        return function () {
	          return core.ajax(getCompanyByKeyUrlBuilder(companyKey));
	        };
	    }
	    function getCompanyByKeyUrlBuilder(companyKey) {
	        return function () {
	            return '/api/companies/' + companyKey;
	        };
	    }
	    function getCompaniesUrlBuilder(companyType) {
	        return function () {
	            return '/api/companies' + (companyType == null ? "" : "?companyType=" + companyType);
	        };
	    }

	    function getContacts( companyKey ) {
	      return core.ajax( '/api/companies/' + companyKey + '/contacts' );
	    }

	    function createContact( companyKey, contactData ) {
	      return core.ajaxPost('/api/companies/' + companyKey + '/contacts', contactData );
	    }

	    function updateContact( contactKey, contactData ) {
	      return core.ajaxPut('/api/contacts/' + contactKey, contactData );
	    }

	    function deleteContact( contactKey ) {
	      return core.ajaxDelete('/api/contacts/' + contactKey );
	    }

	    function getNoteTypes() {
	      return core.ajax('/api/profilenotes/types');
	    }
	    function createNote( companyKey, note ) {
	      return core.ajaxPost( '/api/companies/' + companyKey + '/notes/', note );
	    }
	    function updateNote( companyKey, noteId, note ) {
	      return core.ajaxPut( '/api/companies/' + companyKey + '/notes/' + noteId, note );
	    }
	    function deleteNote( companyKey, noteId ) {
	      return core.ajaxDelete( '/api/companies/' + companyKey + '/notes/' + noteId );
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


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

/***/ 126:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {ko.components.register('product-selector', __webpack_require__(20));

	ko.punches.enableAll();
	var rvc = __webpack_require__(8);
	var directoryService = __webpack_require__(16);
	var warehouseService = __webpack_require__(11);
	var productsService = __webpack_require__(24);
	var inventoryService = __webpack_require__(77);
	__webpack_require__(35);
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
	  template: __webpack_require__(127)
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

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 127:
/***/ (function(module, exports) {

	module.exports = "<div class=\"panel panel-default\">\r\n  <div class=\"panel-heading\">{{ headerText }}</div>\r\n  <div class=\"panel-body\">\r\n    <div class=\"form-horizontal\">\r\n      <div class=\"form-group\">\r\n        <label class=\"col-sm-2 control-label\">What are you receiving?</label>\r\n\r\n        <div class=\"radio col-sm-10\">\r\n          {{ #foreach: lotTypeOptions }}\r\n          <label>\r\n            <input type=\"radio\" id=\"invnetory-type\" value=\"{{ key }}\" data-bind=\"checked: $parent.lotType\"> {{ value }}\r\n          </label>\r\n          {{ /foreach }}\r\n        </div>\r\n      </div>\r\n\r\n\r\n      <div class=\"form-group\">\r\n        <label class=\"col-sm-2 control-label\" for=\"date-received\">Date Received</label>\r\n        <div class=\"col-sm-4\">\r\n          <input type=\"text\" class=\"form-control\" id=\"date-received\" data-bind=\"datePicker: dateReceived\" />\r\n        </div>\r\n      </div>\r\n\r\n      <div class=\"form-group\">\r\n        <label class=\"col-sm-2 control-label\" for=\"product\">Product</label>\r\n        <div class=\"col-sm-10\">\r\n          <product-selector params=\"selectedValue: product, lotType: lotType, optionsDisplay: productLabelProp, controlId: 'product'\"></product-selector>\r\n        </div>\r\n      </div>\r\n\r\n      <div class=\"form-group\">\r\n        <label class=\"col-sm-2 control-label\" for=\"packaging-received\">Packaging Received</label>\r\n        <div class=\"col-sm-10\">\r\n          <product-selector params=\"selectedValue: packagingReceived, lotType: 5, optionsDisplay: 'ProductName', controlId: 'packaging-received', disabled: disablePackagingReceived\"></product-selector>\r\n          <span class=\"help-block\">What packaging was this product in when it was received?</span>\r\n        </div>\r\n      </div>\r\n\r\n      <div class=\"form-group\">\r\n        <label class=\"col-sm-2 control-label\" for=\"packaging-received\">Quantity Received</label>\r\n        <div class=\"col-sm-10\">\r\n          <input type=\"number\" class=\"form-control\" data-bind=\"textinput: quantity\">\r\n        </div>\r\n      </div>\r\n\r\n      <div class=\"form-group\">\r\n        <label class=\"col-sm-2 control-label\" for=\"vendor\">Vendor</label>\r\n        <div class=\"col-sm-10\">\r\n          <select class=\"form-control\" id=\"vendor\" data-bind=\"value: vendor\">\r\n            <option> </option>\r\n            <option data-bind=\"value: 'new'\">[Add New Vendor]</option>\r\n            {{ #foreach: vendorOptions }}\r\n            <option data-bind=\"value: CompanyKey\">{{Name}}</option>\r\n            {{ /foreach }}\r\n        </select>\r\n        </div>\r\n      </div>\r\n      \r\n      <div class=\"form-group\" data-bind=\"visible: vendor() === 'new'\">\r\n        <label class=\"col-sm-2 control-label\" for=\"\">Vendor Name</label>\r\n        <div class=\"col-sm-10\">\r\n          <input class=\"form-control\" type=\"text\" data-bind=\"textinput: newVendorName\">\r\n        </div>\r\n      </div>\r\n\r\n      <div class=\"form-group\">\r\n        <label class=\"col-sm-2 control-label\" for=\"purchase-order-num\">Purchase Order</label>\r\n        <div class=\"col-sm-4\">\r\n          <input type=\"text\" class=\"form-control\" id=\"purchase-order-num\" data-bind=\"value: purchaseOrderNumber\">\r\n        </div>\r\n\r\n        <label class=\"col-sm-2 control-label\" for=\"shipper-number\">Shipper Number</label>\r\n        <div class=\"col-sm-4\">\r\n          <input type=\"text\" class=\"form-control\" id=\"shipper-number\" data-bind=\"value: shipperNumber\">\r\n        </div>\r\n      </div>\r\n\r\n      <div class=\"panel panel-primary\">\r\n        <div class=\"panel-heading\"><h4>Inventory To Create</h4></div>\r\n        <div class=\"panel-body\">\r\n          <table class=\"table table-striped\">\r\n            <thead>\r\n              <tr>\r\n                <th></th>\r\n                <th>Packaging</th>\r\n                <th>Quantity</th>\r\n                <th data-bind=\"visible: showTotalWeight\">Total Weight</th>\r\n                <th>Warehouse</th>\r\n                <th>Location</th>\r\n            </tr>\r\n            </thead>\r\n            <tbody data-bind=\"foreach: items\">\r\n            <tr>\r\n              <td>\r\n                <button type=\"button\" class=\"btn btn-link\" data-bind=\"command: $parent.removeItemCommand\"><i class=\"fa fa-times\"></i></button>\r\n              </td>\r\n              <td><product-selector params=\"selectedValue: inventoryUnits, lotType: 5, optionsDisplay: 'ProductName'\"></product-selector></td>\r\n              <td><input type=\"text\" data-bind=\"textInput: quantity\" class=\"form-control\"/></td>\r\n              <td data-bind=\"visible: $parent.showTotalWeight, text: totalWeight\"></td>\r\n              <td>Rincon</td>\r\n              <td><select data-bind=\"value: location, options: $parent.locationOptions, optionsText: 'Description', optionsValue: 'LocationKey', optionsCaption: ' '\"></select></td>\r\n            </tr>\r\n            </tbody>\r\n            <tfoot data-bind=\"visible: showTotalWeight\">\r\n              <tr>\r\n                <td class=\"text-right\" colspan=\"3\"><b>Total Weight:</b></td>\r\n                <td colspan=\"3\"><span data-bind=\"text: totalWeightOfCreated, css: { 'text-danger': !totalWeightOfCreated.isValid() }\"></span> / {{totalWeightReceived}}</td>\r\n              </tr>\r\n            </tfoot>\r\n          </table>\r\n          <button type=\"button\" data-bind=\"command: addItemCommand\" class=\"btn btn-default\" tabindex=\"-1\">\r\n            <i class=\"fa fa-plus\" /> Add Item\r\n          </button>\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </div>\r\n</div>\r\n"

/***/ }),

/***/ 128:
/***/ (function(module, exports, __webpack_require__) {

	ko.punches.enableAll();
	var rvc = __webpack_require__(8);

	function InventoryReceived(params) {
	  if (!(this instanceof InventoryReceived)) { return new InventoryReceived(params); }

	  var self = this;

	  this.data = ko.pureComputed(function() {
	    return params.values();
	  });

	  this.showTotalWeight = ko.pureComputed(function () {
	    return true;
	  });

	  this.totalWeight = ko.pureComputed(function() {
	    var data = self.data() || { InventoryItems: [] };
	    var total = 0;
	    ko.utils.arrayForEach(data.InventoryItems, function(i) {
	      total += i.Weight;
	    });
	    return total;
	  });


	  // Behaviors

	  // Exports
	  if (params && ko.isObservable(params.exports)) {
	    params.exports({ });
	  }

	  return this;
	}

	module.exports = {
	  viewModel: InventoryReceived,
	  template: __webpack_require__(129)
	};

/***/ }),

/***/ 129:
/***/ (function(module, exports) {

	module.exports = "<div class=\"panel panel-default\" data-bind=\"with: data\">\r\n  <div class=\"panel-heading\"><h3>{{ LotKey }}</h3></div>\r\n  <div class=\"panel-body\">\r\n    <div class=\"form-horizontal\">\r\n\r\n      <div class=\"form-group\">\r\n        <label class=\"col-sm-2 control-label\">Date Received</label>\r\n        <div class=\"col-sm-4\">\r\n          <p class=\"form-control-static\">{{ DateEntered }}</p>\r\n        </div>\r\n      </div>\r\n\r\n      <div class=\"form-group\">\r\n        <label class=\"col-sm-2 control-label\">Product</label>\r\n        <div class=\"col-sm-4\">\r\n          <p class=\"form-control-static\">{{ ProductName }}</p>\r\n        </div>\r\n\r\n        <label class=\"col-sm-2 control-label\" for=\"packaging-received\">Packaging Received</label>\r\n        <div class=\"col-sm-4\">\r\n          <p class=\"form-control-static\">{{ PackagingReceived }}</p>\r\n        </div>\r\n      </div>\r\n\r\n      <div class=\"form-group\">\r\n        <label class=\"col-sm-2 control-label\" for=\"vendor\">Vendor</label>\r\n        <div class=\"col-sm-10\">\r\n          <p class=\"form-control-static\">{{ VendorName }}</p>\r\n        </div>\r\n      </div>\r\n\r\n      <div class=\"form-group\">\r\n        <label class=\"col-sm-2 control-label\" for=\"purchase-order-num\">Purchase Oder</label>\r\n        <div class=\"col-sm-4\">\r\n          <p class=\"form-control-static\">{{ PurchaseOrderNumber }}</p>\r\n        </div>\r\n\r\n        <label class=\"col-sm-2 control-label\" for=\"shipper-number\">Shipper Number</label>\r\n        <div class=\"col-sm-4\">\r\n          <p class=\"form-control-static\">{{ ShipperNumber }}</p>\r\n        </div>\r\n      </div>\r\n\r\n      <div class=\"panel panel-primary\">\r\n        <div class=\"panel-heading\"><h4>Invnetory Created</h4></div>\r\n        <div class=\"panel-body\">\r\n\r\n          <table class=\"table table-striped\">\r\n            <thead>\r\n              <tr>\r\n                <th>Packaging</th>\r\n                <th>Quantity</th>\r\n                <th data-bind=\"visible: $parent.showTotalWeight\">Total Weight</th>\r\n                <th>Warehouse</th>\r\n                <th>Location</th>\r\n              </tr>\r\n            </thead>\r\n            <tbody data-bind=\"foreach: InventoryItems\">\r\n            <tr>\r\n              <td>{{ InventoryUnits }}</td>\r\n              <td>{{ Quantity }}</td>\r\n              <td data-bind=\"visible: $parents[1].showTotalWeight\">{{ Weight }}</td>\r\n              <td>{{ FacilityName }}</td>\r\n              <td>{{ Location }}</td>\r\n            </tr>\r\n            </tbody>\r\n            <tfoot>\r\n              <tr>\r\n                <td colspan=\"2\" class=\"text-right\"><strong>Total Weight:</strong></td>\r\n                <td data-bind=\"visible: $parent.showTotalWeight\"><strong>{{ $parent.totalWeight }}</strong></td>\r\n                <td></td>\r\n                <td></td>\r\n              </tr>\r\n            </tfoot>\r\n          </table>\r\n\r\n        </div>\r\n      </div>\r\n\r\n    </div>\r\n  </div>\r\n</div>\r\n"

/***/ })

});