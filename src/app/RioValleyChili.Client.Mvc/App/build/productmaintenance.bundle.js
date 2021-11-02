webpackJsonp([15],{

/***/ 0:
/***/ (function(module, exports, __webpack_require__) {

	/** Knockout components */
	ko.components.register( 'product-maintenance-editor', __webpack_require__(170));
	ko.components.register( 'product-maintenance-summary', __webpack_require__(172));

	/** Service libraries */
	var productsService = __webpack_require__(24);
	var lotService = __webpack_require__(72);

	/** Extensions */
	__webpack_require__(31);
	__webpack_require__(30);
	__webpack_require__(37);

	ko.validation.init({
	  insertMessages: false,
	  decorateInputElement: true,
	  errorElementClass: 'has-error',
	  errorMessageClass: 'help-block',
	  grouping: {
	    deep: true,
	    live: true,
	    observable: true
	  }
	});

	/** Page routing */
	var page = __webpack_require__(26);

	/** Product Maintenance root view model */
	function ProductMaintenanceVM() {
	  if ( !(this instanceof ProductMaintenanceVM) ) { return new ProductMaintenanceVM( params ); }

	  var self = this;

	  // Data
	  this.isRedirecting = ko.observable( false );
	  this.currentKey = ko.observable(  );

	  this.attrs = {};
	  function getAttributes() {
	    return lotService.getAttributeNames().then(
	    function( data, textStatus, jqXHR ) {
	      self.attrs = data;
	    },
	    function( jqXHR, textStatus, errorThrown ) {
	      // Fail
	    });
	  }

	  this.isInit = ko.observable( false );
	  function init() {
	    var getAttrs = getAttributes().then(
	    function( data, textStatus, jqXHR ) {
	      self.isInit( true );
	      page();
	    },
	    function( jqXHR, textStatus, errorThrown ) {
	      showUserMessage( 'Could not load Product Maintenance', {
	        description: errorThrown
	      });
	    });
	  }

	  init();

	  // Set default inventory type as Chile
	  this.inventoryType = ko.observable().extend({ inventoryType: true });
	  this.inventoryType.subscribe(function( invType ) {
	    if ( !invType ) {
	      return;
	    }

	    // Init cache if not found
	    var _invCache = _productsCache[ invType ];

	    if ( !_invCache && invType !== 7 ) {
	      _productsCache[ invType ] = {};
	    } else if ( !_invCache && invType === 7 ) {
	      // Load misc items
	      _productsCache['7'] = ko.observableArray( [] );
	      getMiscProducts.execute();
	    }
	  });

	  this.lotType = ko.observable().extend({ lotType2: true });
	  this.lotType.subscribe(function( data ) {
	    if ( !data ) {
	      return;
	    }

	    var _inventoryType = data.inventoryType.key;
	    var _invCache = _productsCache[ _inventoryType ];

	    if ( _inventoryType !== 7 && !_invCache[ data.key ] ) {
	      _invCache[ data.key ] = ko.observableArray( [] );
	      getProducts.execute( _inventoryType, data.key );
	    }
	  });

	  this.isMisc = ko.pureComputed(function() {
	    var _invType = self.inventoryType();

	    return _invType === 7;
	  });

	  this.selected = ko.observable();
	  this.selected.subscribe(function( productKey ) {
	    if ( productKey == null ) {
	      return;
	    }

	    page( '/' + self.inventoryType() + '/' + productKey );
	  });

	  var _lotTypes = ko.utils.arrayMap( this.lotType.options, function( opt ) {
	    return opt && opt.value;
	  });
	  // _lotTypes.push({
	  //   inventoryType: {
	  //     key: 7,
	  //     value: 'Misc. Inventory',
	  //   },
	  //   key: null,
	  //   value: 'Misc. Inventory'
	  // });

	  this.options = {
	    lotType: ko.pureComputed(function() {
	      var _invType = self.inventoryType();

	      if ( _invType ) {
	        return ko.utils.arrayFilter( _lotTypes, function( lotType ) {
	          return lotType.inventoryType.key === _invType;
	        });
	      }

	      return _lotTypes;
	    }),
	    inventoryType: ko.utils.arrayMap( this.inventoryType.options, function( opt ) {
	      return opt && opt.value;
	    })
	  };
	  // Product Mainteance Summary data
	  // Product cache
	  var _productsCache = {};
	  var getProducts = ko.asyncCommand({
	    execute: function( inventoryKey, lotTypeKey, complete ) {
	      var getProds = productsService.getProductsByLotType( lotTypeKey, { includeInactive: true }).then(
	        function( data, textStatus, jqXHR ) {
	        _productsCache[ inventoryKey ][ lotTypeKey ]( data );
	      },
	      function( jqXHR, textStatus, errorThrown ) {
	        showUserMessage( 'Could not get products', {
	          description: errorThrown
	        });
	      }).always( complete );

	      return getProds;
	    },
	    canExecute: function( isExecuting ) {
	      return !isExecuting;
	    }
	  });
	  var getMiscProducts = ko.asyncCommand({
	    execute: function( complete ) {
	      var getProds = productsService.getProductsByInventoryType( '7', { includeInactive: true }).then(
	        function( data, textStatus, jqXHR ) {
	        _productsCache['7']( data );
	      },
	      function( jqXHR, textStatus, errorThrown ) {
	        showUserMessage( 'Could not get products', {
	          description: errorThrown
	        });
	      }).always( complete );

	      return getProds;
	    },
	    canExecute: function( isExecuting ) {
	      return !isExecuting;
	    }
	  });

	  this.products = ko.computed({
	    read: function() {
	      // Check inventory type
	      var _invType = self.inventoryType();

	      if ( !_invType ) {
	        return;
	      }

	      if ( _invType === 7 ) {
	        return _productsCache['7']();
	      }

	      // Check lot type
	      var lotType = self.lotType();

	      if ( !lotType ) {
	        return;
	      }

	      var prodCache = _productsCache[ _invType ];
	      var cache = prodCache && prodCache[ lotType.key ];

	      return cache && cache();
	    },
	    write: function( items ) {
	      // Check inventory type
	      var _invType = self.inventoryType();

	      if ( !_invType ) {
	        return;
	      }

	      if ( _invType === 7 ) {
	        return _productsCache['7']( items );
	      }

	      // Check lot type
	      var lotType = self.lotType();

	      if ( !lotType ) {
	        return;
	      }

	      var prodCache = _productsCache[ _invType ];
	      var cache = prodCache && prodCache[ lotType.key ];

	      // Modify cache
	      if ( cache ) {
	        cache( items );
	      }
	    }
	  });
	  this.summaryData = {
	    input: self.products,
	    selected: self.selected,
	    exports: ko.observable()
	  };

	  // Product maintenance editor data
	  // Editor data
	  this.editorData = {
	    input: ko.observable(  ),
	    exports: ko.observable(  )
	  };

	  this.isDirty = ko.pureComputed(function() {
	    var editor = self.editorData.exports();

	    return editor && editor.isDirty();
	  });

	  // Save maintenance data
	  this.save = ko.asyncCommand({
	    execute: function( complete ) {
	      // Compile data from editor
	      var editor = self.editorData.exports();
	      var dto = editor && editor.toDto();

	      // If data is invalid
	      if ( dto === null ) {
	        // Display error message
	        showUserMessage( 'Save failed', {
	          description: 'There was an error in the form. Please ensure all required fields have been filled out correctly.'
	        });

	        // End save call
	        complete();
	        return;
	      }

	      var _lotType = dto.LotType;
	      var _productType = dto.ProductType;
	      var _productKey = dto.ProductKey;

	      var isNew = dto.isNew;
	      var dfd;

	      // If creating a product, use POST to save
	      if ( isNew ) {
	        var createProduct = productsService.createProduct( dto ).then(
	        function( data, textStatus, jqXHR ) {
	          _productKey = data;

	          // Display user message on successful save
	          showUserMessage( 'Save successful', {
	            description: '<b>' + dto.ProductName + '</b> has been created.'
	          });
	        },
	        function( jqXHR, textStatus, errorThrown ) {
	          // Display user message on failure
	          showUserMessage( 'Could not create product', {
	            description: errorThrown
	          });
	        });

	        dfd = createProduct;

	      // Else if updating a product, use PUT to save
	      } else {
	        var updateProduct = productsService.updateProduct( _productKey, dto ).then(
	        null,
	        function( jqXHR, textStatus, errorThrown ) {
	          // Display user message on failure
	          showUserMessage( 'Could not update product', {
	            description: errorThrown
	          });
	        });

	        dfd = updateProduct;
	      }

	      var finishSave = dfd.then(function( data ) {
	        // Update summary table if it contains the product
	        updateSummaryViaKey( _productType, _productKey );

	        editor.resetDirtyChecker();

	        // Display user message on successful save
	        if ( isNew ) {
	          showUserMessage( 'Save successful', {
	            description: '<b>' + dto.ProductName + '</b> has been created.'
	          });

	          page.redirect( '/' + _productType + '/' + _productKey );
	        } else {
	          showUserMessage( 'Save successful', {
	            description: '<b>' + dto.ProductName + '</b> has been updated.'
	          });
	        }
	      }).always( complete );

	      return finishSave;

	      function updateSummaryViaKey( invType, productKey ) {
	        var summaries = self.summaryData.exports();

	        productsService.getProductDetails( invType, productKey ).then(
	        function( data, textStatus, jqXHR ) {
	          var update = summaries && summaries.updateItem( data );
	        },
	        function( jqXHR, textStatus, errorThrown ) {
	          showUserMessage( 'Could not update summary', {
	            description: errorThrown
	          });
	        });
	      }
	    },
	    canExecute: function( isExecuting ) {
	      return !isExecuting && self.isDirty();
	    }
	  });

	  // Start new product creation
	  this.createNewProduct = ko.command({
	    execute: function() {
	      page( '/new' );
	    }
	  });

	  function clearEditorData() {
	    self.editorData.input( null );
	    self.editorData.exports( null );
	    self.selected( null );
	  }

	  // Close the editor popover
	  this.closeEditor = ko.asyncCommand({
	    execute: function( complete ) {
	      // Confirm before closing
	      if ( self.isDirty() ) {
	        showUserMessage( 'Save changes before closing?', {
	          description: 'The current product has unsaved changes. Would you like to save before closing?',
	          type: 'yesnocancel',
	          onYesClick: function() {
	            var saveData = self.save.execute().then(
	            function( data, textStatus, jqXHR ) {
	              clearEditorData();
	              page( '/' );
	            }).always( complete );
	          },
	          onNoClick: function() {
	            clearEditorData();
	            page( '/' );
	            complete();
	          },
	          onCancelClick: complete
	        });
	      } else {
	        // Disable the editor component to clear memory
	        clearEditorData();
	        page( '/' );
	        complete();
	      }
	    },
	    canExecute: function( isExecuting ) {
	      return !isExecuting;
	    }
	  });

	  // Page routing
	  // Set page routing callbacks
	  page.base('/QualityControl/ProductMaintenance');

	  // On routing callback
	  function checkIfDirty( ctx, next ) {
	    if ( self.isRedirecting() ) {
	      self.isRedirecting( false );

	      return;
	    }

	    var currentKey = self.currentKey();
	    if ( self.isDirty() ) {
	      showUserMessage( 'Save changes before navigating?', {
	        description: 'The current product has unsaved changes. Would you like to save before navigating?',
	        type: 'yesnocancel',
	        onYesClick: function() {
	          var saveData = self.save.execute().then(
	            function( data, textStatus, jqXHR ) {
	            next();
	          },
	          function( jqXHR, textStatus, errorThrown ) {
	            self.isRedirecting( true );
	            page( '/' + currentKey );
	          });
	        },
	        onNoClick: function() {
	          next();
	        },
	        onCancelClick: function() {
	          // Cancel navigation
	          self.isRedirecting( true );
	          page( '/' + currentKey );
	        }
	      });
	    } else {
	      next();
	    }
	  }
	  page( checkIfDirty );

	  function startNewProduct( ctx, next ) {
	    var _lotType = self.lotType();
	    var _inventoryType = ctx.params.inventoryKey;

	    if ( _inventoryType === 'new' ) {
	      // Display creation ui
	      self.editorData.input({
	        attrs: JSON.parse( ko.toJSON( self.attrs ) ),
	        LotType: _lotType && _lotType.key,
	        ProductCodeAndName: 'New'
	      });
	    } else {
	      next();
	    }
	  }
	  page( '/:inventoryKey', startNewProduct );

	  function navigateToDetails( ctx, next ) {
	    // Check if url has key
	    var _lotType = self.lotType();
	    var _inventoryType = +ctx.params.inventoryKey;
	    var _productKey = ctx.params.productKey;
	    self.currentKey( _productKey );

	    if ( _inventoryType && _productKey ) {
	      // Fetch product details
	      var getDetails = productsService.getProductDetails( _inventoryType, _productKey ).then(
	      function( data, textStatus, jqXHR ) {
	        data.attrs = JSON.parse( ko.toJSON( self.attrs ) );
	        data.InventoryType = _inventoryType;

	        // Display editor ui
	        self.inventoryType( _inventoryType );
	        if ( _inventoryType === 7 ) {
	          self.editorData.input( data );

	          return;
	        }

	        var _lotTypeObject = ko.utils.arrayFirst( ko.unwrap( self.options.lotType ), function( lotType ) {
	          return lotType.key === data.LotType;
	        });

	        self.lotType( _lotTypeObject );
	        self.editorData.input( data );
	      },
	      function( jqXHR, textStatus, errorThrown ) {
	        page.redirect( '/' );
	        showUserMessage( 'Could not load details', {
	          description: errorThrown
	        });
	      });
	    } else {
	      next();
	    }
	  }
	  page( '/:inventoryKey/:productKey', navigateToDetails );

	  function showSummary( ctx ) {
	    clearEditorData();
	  }
	  page( showSummary );

	  // Exports
	  return this;
	}

	var vm = new ProductMaintenanceVM();

	ko.applyBindings( vm );

	module.exports = vm;


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

/***/ 37:
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {__webpack_require__(37);

	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(38), __webpack_require__(39), __webpack_require__(9)], __WEBPACK_AMD_DEFINE_RESULT__ = function (tableHeaderClickHelper, propertyGetter, ko) {
	    var sortOption = {};

	    ko.bindingHandlers.sortableTable = {
	        init: function (element, valueAccessor, allBindingsAccessor) {
	            var allBindings = allBindingsAccessor();
	            allBindings.enableClick = canBeSorted;

	            ko.bindingHandlers.clickableTableHeaders.init(
	                element,
	                ko.utils.wrapAccessor(sort),
	                ko.utils.wrapAccessor(allBindings));

	            function sort(th) {
	                $(element).find('thead .' + ko.bindingHandlers.sortableTable.options.sortedCssClass)
	                    .removeClass(ko.bindingHandlers.sortableTable.options.sortedCssClass + ' ' +
	                        ko.bindingHandlers.sortableTable.options.sortedAscCssClass + ' ' +
	                        ko.bindingHandlers.sortableTable.options.sortedDescCssClass);

	                sortData(th, valueAccessor());
	            }
	        }
	    }

	    ko.bindingHandlers.sortableTable.options = {
	        sortedCssClass: 'sorted',
	        sortedAscCssClass: 'asc',
	        sortedDescCssClass: 'desc'
	    }

	    function sortData(thElement, data) {
	        var $th = $(thElement);
	        var sort = $th.attr('data-sort');
	        if (!sort) return;

	        var previousSort = sortOption;
	        var sortDirection = previousSort && previousSort.propertyName === sort
	            ? previousSort.direction * -1
	            : 1;

	        sortOption = {
	            propertyName: sort,
	            direction: sortDirection
	        };

	        // todo: get context for table's tbody in order to prevent duplicate declaration source property as valueAccessor for the body and clickable header

	        var sortFn = dynamicSortFn(sort, sortDirection);
	        if (ko.isObservable(data)) {
	          var dataCache = data();

	            data(dataCache.sort(sortFn));
	        }

	        $th.addClass(ko.bindingHandlers.sortableTable.options.sortedCssClass + ' ' +
	            (sortDirection > 0
	                ? ko.bindingHandlers.sortableTable.options.sortedAscCssClass
	                : ko.bindingHandlers.sortableTable.options.sortedDescCssClass));
	    };

	    function dynamicSortFn( sort, direction ) {
	        if ( !sort ) { return null; }

	        // 1 = Ascending, -1 = Descending
	        if (direction != -1) { direction = 1; }

	        var lt = -1 * direction,
	            gt = 1 * direction;

	        function sortFn( a, b ) {
	            var _a = ko.utils.unwrapObservable( propertyGetter.getValue( a, sort ) );
	            var _b = ko.utils.unwrapObservable( propertyGetter.getValue( b, sort ) );

	            if ( _a < _b || _a == null ) {
	              return lt;
	            } else if ( _a > _b ) {
	              return gt;
	            }

	            return 0;
	        }

	        return sortFn;
	    }

	    function canBeSorted(element) {
	        var $th = $(element);
	        var sort = $th.attr('data-sort');
	        return sort && true;
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__))

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 38:
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(1), __webpack_require__(9)], __WEBPACK_AMD_DEFINE_RESULT__ = function($, ko) {
	    ko.bindingHandlers.clickableTableHeaders = {
	        init: function (element, valueAccessor, allBindings) {
	            setTimeout(function () { // allow templated bindings to be rendered
	                setupHandlers(element, valueAccessor, allBindings);
	            }, 0);
	        }
	    }

	    function setupHandlers(element, valueAccessor, allBindings) {
	        setupTableClickElements(element, valueAccessor, allBindings);
	        setupRebindTrigger(element, valueAccessor, allBindings);
	    }

	    var defaultEnableClick = function () { return true; };

	    function setupTableClickElements(element, valueAccessor, allBindings) {
	        var options = allBindings() || {};
	        var enableClick = options.enableClick || defaultEnableClick;
	        var $table = $(element);

	        $table.find('thead th').each(function (index, thElem) {
	            thElem.clickEnabled = enableClick(thElem);
	            
	            if (thElem.clickEnabled) {
	                var $th = $(thElem);
	                $th.css({
	                    cursor: 'pointer'
	                });
	            }

	            thElem = null;
	            $th = null;
	        });

	        var $thead = $table.find('thead');
	        $thead.off('click');
	        $thead.click(function (args) {
	            if (args.target.nodeType !== 1 || args.target.nodeName !== 'TH') return;
	            if (args.target.clickEnabled) valueAccessor()(args.target);
	            args.stopPropagation();
	        });

	        ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
	            $thead.off('click');
	            $thead = null;
	            $table = null;
	        });
	    }
	    function setupRebindTrigger(element, valueAccessor, allBindings) {
	        var options = allBindings() || {};
	        if (options.rebindTrigger) {
	            if (!ko.isObservable(options.rebindTrigger)) throw new Error("The \"rebindTrigger\" binding option is invalid. Expected observable.");
	            options.rebindTrigger.subscribe(function () {
	                setupTableClickElements(element, valueAccessor, allBindings);
	                //todo: clean up old bindings?
	            });
	        }
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

/***/ }),

/***/ 39:
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_RESULT__ = function() {
	    var arrayIndexRegex = /^(\w+)?\[(\d+)\]\.?(.*)/;

	    //Object.prototype.deepSearch = function(propertyName) {
	    //    return getValueFromPropertyName(this, propertyName);
	    //}

	    return {
	        getValue: getValueFromPropertyName
	    }

	    function getValueFromPropertyName(obj, propName) {
	        if (obj == undefined) return undefined;
	        var matches = arrayIndexRegex.exec(propName);
	        if (matches && matches.length) {
	            var index = parseInt(matches[2]);
	            obj = matches[1]
	                ? obj[matches[1]][index]
	                : obj[index];

	            return matches[3]
	                ? getValueFromPropertyName(obj, matches[3])
	                : obj;
	        }

	        var paths = propName.split('.');
	        if (paths.length > 1) {
	            var p = paths.shift();
	            return getValueFromPropertyName(obj[p], paths.join('.'));
	        }

	        return obj[propName];
	    }
	}.call(exports, __webpack_require__, exports, module), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__))


/***/ }),

/***/ 72:
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(7), __webpack_require__(8)], __WEBPACK_AMD_DEFINE_RESULT__ = function (core, rvc) {
	  var attributeOrder = {
	    'AstaC': 0,
	    'Asta': 1,
	    'H2O': 2,
	    'Scan': 3,
	    'AB': 4,
	    'Gran': 5,
	    'Scov': 6,
	    'TPC': 7,
	    'Yeast': 8,
	    'Mold': 9,
	    'ColiF': 10,
	    'EColi': 11,
	    'Sal': 12,
	    'Rod Hrs': 13,
	    'InsP': 14,
	    'Ash': 15,
	    'AIA': 16,
	    'Ethox': 17,
	    'BI': 18,
	    'Lead': 19,
	    'AToxin': 20,
	    'Gluten': 21
	  };

	  function sortAttributesLists(values) {
	    var data = values || {};
	    rvc.helpers.forEachInventoryType(sortAttributesForType);
	    return data;

	    function sortAttributesForType(type) {
	      var attrs = data[type.key] || [];
	      if (type.key === rvc.lists.inventoryTypes.Chile.key) {
	        attrs.push({ Key: 'AstaC', Value: 'Asta Calc' });
	      }

	      var attrOrder = attributeOrder || {};
	      var unorderedIndex = Object.keys(attrOrder).length + 1;
	      ko.utils.arrayMap(attrs, function (attr) {
	        var order = attrOrder[attr.Key];
	        var index = order === undefined ? unorderedIndex++ : order;
	        attr.__index = index;
	      });

	      data[type.key] = attrs.sort(function (a, b) {
	        return a.__index === b.__index ?
	          0 :
	          a.__index < b.__index ?
	            -1 :
	            1;
	      });
	    }
	  }

	  function sortAttributes( attrs ) {
	    var sortedAttributes = [];
	    var attrOrder = attributeOrder || {};
	    var unorderedIndex = Object.keys(attrOrder).length + 1;

	    ko.utils.arrayMap( attrs, function (attr) {
	      var order = attrOrder[attr.Key];
	      var index = order === undefined ?
	        unorderedIndex++ :
	        order;

	      attr.__index = index;
	    } );

	    sortedAttributes = attrs.sort(function (a, b) {
	      return a.__index === b.__index ?
	        0 :
	        a.__index < b.__index ?
	          -1 :
	          1;
	    });

	    return sortedAttributes;
	  }

	  return {
	    buildLotPager: function( options ) {
	      options = options || {};
	      return core.pagedDataHelper.init({
	        urlBase: "/api/lots",
	        pageSize: options.pageSize || 50,
	        parameters: options.parameters,
	        resultCounter: function (data) {
	          return data.LotSummaries.length;
	        },
	        onNewPageSet: options.onNewPageSet
	      });
	    },
	    compositeLots: function( data ) {
	      return core.ajaxPut( '/api/lots/addattributes/', data );
	    },
	    sortAttributes: sortAttributes,
	    buildLotUrl: function (key) {
	      return ["/api/Lots/", key || ''].join('');
	    },
	    getAttributeNames: function () {
	      var $dfd = $.Deferred();

	      core.ajax('/api/attributeNames')
	      .done(function (data) {
	        try {
	          $dfd.resolve(sortAttributesLists(data));
	        } catch (e) {
	          $dfd.reject();
	        }
	      })
	      .fail($dfd.reject);

	      // support compatability with the core.ajax return object
	      $dfd.error = $dfd.fail;

	      return $dfd;
	    },
	    getLotData: function( lotKey ) {
	      return core.ajax( ''.concat( '/api/lots/' + lotKey.replace(/ /g, '') ) );
	    },
	    getLotHistory: function( lotKey ) {
	      return core.ajax( '/api/lots/' + lotKey + '/history' );
	    },
	    getIngredientsByProductType: function () {
	      return core.ajax('/api/ingredients');
	    },
	    getProductsByLotType: function (lotType, options) {
	      var inventoryType = rvc.lists.lotTypes.findByKey(lotType).inventoryType.key;
	      var url = ['/api/products/', inventoryType, '?lotType=', lotType].join('');

	      if (options && options.filterProductsWithInventory) {
	        url = url.concat("&filterProductsWithInventory=true");
	      }

	      return core.ajax(url, options);
	    },
	    getLotsByKey: function( lotKey ) {
	      return core.ajax( ''.concat( '/api/lots?startingLotKey=', lotKey ) );
	    },
	    setLotStatus: function( lotKey, status ) {
	      return core.ajaxPut( ''.concat( '/api/lots/', lotKey, '/qualityStatus' ), status );
	    },
	    removeLotHold: function (lotKey, optionsCallback) {
	      return core.ajaxPut("/api/lots/" + lotKey + "/holds", null, optionsCallback);
	    },
	    setLotHold: function (lotKey, data, optionsCallback) {
	      return core.ajaxPut("/api/lots/" + lotKey + "/holds", data, optionsCallback);
	    },
	    getInputMaterialsDetails: function (key) {
	      if (!key) { return; }
	      return core.ajax(this.buildLotUrl(key) + '/input');
	    },
	    getTransactionsDetails: function (key) {
	      if (!key) { return; }
	      return core.ajax(this.buildLotUrl(key) + '/inventory/transactions');
	    },
	    saveLabResult: function( lotKey, data ) {
	      return core.ajaxPut( ('/api/lots/' + lotKey), data );
	    },
	    createAllowance: function( lotKey, type, key ) {
	      return core.ajaxPut( '/api/lots/' + lotKey.replace( /\s+/g, '' ) + '/allowances/' + type + '/' + key );
	    },
	    deleteAllowance: function( lotKey, type, key ) {
	      return core.ajaxDelete( '/api/lots/' + lotKey.replace( /\s+/g, '' ) + '/allowances/' + type + '/' + key );
	    },
	    getLotInputTrace: function( lotKey ) {
	      if ( lotKey == null ) { throw new Error('Lot trace requires a key'); }

	      return core.ajax( '/api/lots/' + lotKey + '/input-trace' );
	    },
	    getLotOutputTrace: function( lotKey ) {
	      if ( lotKey == null ) { throw new Error('Lot trace requires a key'); }

	      return core.ajax( '/api/lots/' + lotKey + '/output-trace' );
	    },
	  };
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 170:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {var rvc = __webpack_require__(8);
	/** Constructor functions */
	function ProductAttribute( data ) {
	  this.Key = data.Key;

	  this.MinValue = ko.observable( data.MinValue != null ? data.MinValue : '' );
	  this.MaxValue = ko.observable( data.MaxValue != null ? data.MaxValue : '' );
	  var _isRequired = ko.pureComputed(function() {
	    return this.MinValue() || this.MaxValue();
	  }, this);

	  this.MinValue.extend({
	    required: {
	      onlyIf: _isRequired,
	      message: 'Both min and max values are required.',
	    },
	    number: true
	  });
	  this.MaxValue.extend({
	    required: {
	      onlyIf: _isRequired,
	      message: 'Both min and max values are required.',
	    },
	    min: {
	      params: this.MinValue,
	      message: 'Max value must be greater than or equal to min value'
	    },
	    number: true
	  });
	}

	ProductAttribute.prototype.toDto = function() {
	  var _min = this.MinValue();
	  var _max = this.MaxValue();

	  return _min != null && _min !== '' ? {
	    AttributeNameKey: this.Key,
	    RangeMin: _min,
	    RangeMax: _max
	  } :
	    null;
	};

	function ProductIngredient( data ) {
	  this.AdditiveTypeKey = ko.observable( data.AdditiveTypeKey ).extend({
	    required: true
	  });

	  this.Percent = ko.observable( (+data.Percent * 100) || null ).extend({
	    required: true,
	    number: true,
	    min: 0
	  });
	}

	ProductIngredient.prototype.toDto = function() {
	  var percentage = (new Number(this.Percent()) / 100).toFixed(4);

	  return {
	    AdditiveTypeKey: this.AdditiveTypeKey,
	    Percentage: percentage
	  };
	};

	/** Required services */
	var productsService = __webpack_require__(24);

	/** Constructor for editor */
	function ProductEditor( productData, options ) {
	  // Static data
	  this.ProductKey = productData.ProductKey;
	  this.isNew = productData.ProductKey == null;
	  this.options = options;

	  // Assign all editable fields to observables
	  this.IsActive = ko.observable( productData.IsActive == null ? true : productData.IsActive );
	  this.ProductType = ko.observable( productData.ProductType ).extend({ productType: true });
	  this.ProductCode = ko.observable( productData.ProductCode ).extend({ number: true });
	  this.ProductName = ko.observable( productData.ProductName ).extend({ required: true });

	  this.ChileState = ko.observable(productData.ChileState).extend({ chileType: true });
	  this.ChileTypeKey = ko.observable( productData.ChileTypeKey );
	  this.AdditiveTypeKey = ko.observable(productData.AdditiveTypeKey).extend({ additiveType: true });
	  this.Mesh = ko.observable( productData.Mesh );
	  this.IngredientsDescription = ko.observable( productData.IngredientsDescription );

	  this.PackagingWeight = ko.observable( productData.PackagingWeight );
	  this.PalletWeight = ko.observable( productData.PalletWeight );

	  this.editorMode = ko.pureComputed(function() {
	    if ( this.isChile() ) {
	      return 'chile';
	    } else if ( this.isPackaging() ) {
	      return 'packaging';
	    } else if ( this.isAdditive() ) {
	      return 'additive';
	    }

	    return null;
	  }, this);

	  this.isChile = ko.pureComputed(function() {
	    return this.ProductType.trueValue() === rvc.lists.inventoryTypes.Chile.key;
	  }, this);
	  this.isPackaging = ko.pureComputed(function() {
	    return this.ProductType.trueValue() === rvc.lists.inventoryTypes.Packaging.key;
	  }, this);
	  this.isAdditive = ko.pureComputed(function() {
	    return this.ProductType.trueValue() === rvc.lists.inventoryTypes.Additive.key;
	  }, this);
	  this.isMisc = ko.pureComputed(function() {
	    return this.ProductType.trueValue() === rvc.lists.inventoryTypes.NonInventory.key;
	  }, this);

	  this.ChileState.extend({
	    required: {
	      onlyIf: function() {
	        return !this.isChile();
	      }.bind(this),
	      message: 'Please select a chile state.'
	    }
	  });

	  // Editable attributes
	  var attrsCache = productData.attrs;

	  // Map attribute cache items using attr constructor
	  ko.utils.arrayForEach( Object.keys( attrsCache ), function( invType ) {
	    var _mappedAttrs = ko.utils.arrayMap( attrsCache[ invType ], function( attr ) {
	      return new ProductAttribute( attr );
	    } );

	    attrsCache[ invType ] = _mappedAttrs;
	  }, this );

	  var mapAttrs = function( attrs, attributeRanges ) {
	    var _attrRanges = {};

	    // Map attr ranges by attr key
	    ko.utils.arrayForEach( attributeRanges, function( attrRange ) {
	      _attrRanges[ attrRange.AttributeNameKey ] = attrRange;
	    });

	    // Set attr ranges using attr ranges object
	    var productType = this.ProductType();
	    if ( attrs[ productType ] ) {
	      ko.utils.arrayForEach( attrs[ productType ], function( attr ) {
	        var _attrRange = _attrRanges[ attr.Key ];

	        if ( _attrRange ) {
	          attr.MinValue( _attrRange.MinValue );
	          attr.MaxValue( _attrRange.MaxValue );
	        }
	      });
	    }
	  }.bind( this );

	  if ( productData.AttributeRanges ) {
	    mapAttrs( productData.attrs, productData.AttributeRanges );
	  }

	  this.Attributes = ko.pureComputed(function() {
	    var _invType = this.ProductType();

	    return attrsCache[ _invType ];
	  }, this);

	  this.editableAttrs = ko.pureComputed(function() {
	    var attrs = this.Attributes();

	    return ko.utils.arrayFilter( attrs, function( attr ) {
	      return attr.Key !== 'AstaC';
	    });
	  }, this );

	  // Packaging
	  this.Weight = ko.observable( productData.Weight );
	  this.PackagingWeight = ko.observable( productData.PackagingWeight );
	  this.PalletWeight = ko.observable( productData.PalletWeight );

	  // Product formulation
	  var _mappedIngredients = ko.utils.arrayMap( productData.ProductIngredients, function( ingredient ) {
	    return new ProductIngredient( ingredient );
	  });
	  this.ProductIngredients = ko.observableArray( _mappedIngredients );

	  this.wBasePercent = ko.pureComputed(function() {
	    var total = 100;
	    var ingredients = this.ProductIngredients();

	    ko.utils.arrayForEach( ingredients, function( ingredient ) {
	      total -= ingredient.Percent() || 0;
	    });

	    return total.toFixed( 2 );
	  }, this);

	  this.addProductIngredient = ko.command({
	    execute: function() {
	      this.ProductIngredients.push( new ProductIngredient( {} ) );
	    }.bind( this ),
	    canExecute: function() {
	      return true;
	    }
	  });

	  this.removeProductIngredient = ko.command({
	    execute: function( item ) {
	      var items = this.ProductIngredients();
	      var index = items.indexOf( item );

	      this.ProductIngredients.splice( index, 1 );
	    }.bind( this ),
	    canExecute: function() {
	      return true;
	    }
	  });

	  this.validation = ko.validatedObservable({
	    ProductCode: this.ProductCode,
	    ProductName: this.ProductName,
	    LotType: this.LotType,
	    Attributes: this.Attributes,
	    Ingredients: this.ProductIngredients
	  });
	}

	ProductEditor.prototype.toDto = function() {
	  var _isValid = this.validation.isValid();
	  var _inventoryType = this.ProductType();

	  if ( !_isValid ) {
	    this.validation.errors.showAllMessages();
	    return null;
	  }

	  var dto = {
	    isNew: this.isNew,
	    IsActive: this.IsActive,
	    ProductCode: this.ProductCode,
	    ProductName: this.ProductName,
	    ProductType: _inventoryType,
	  };

	  // Compile Header data
	  if ( !this.isNew ) {
	    dto.ProductKey = this.ProductKey;
	  }

	  if ( this.isChile() ) {
	    dto.ChileProductParameters = {
	      ChileState: this.ChileState,
	      ChileTypeKey: this.ChileTypeKey,
	      IngredientsDescription: this.IngredientsDescription,
	      Mesh: this.Mesh,

	      // Attribute range data
	      AttributeRanges: [],

	      // Formulation data
	      Ingredients: ko.utils.arrayMap( this.ProductIngredients(), function( ingredient ) {
	        return ingredient.toDto();
	      })
	    };

	    ko.utils.arrayForEach( this.Attributes(), function( attr ) {
	      var data = attr.toDto();

	      if ( data ) {
	        dto.ChileProductParameters.AttributeRanges.push( data );
	      }
	    });
	  }

	  if ( this.isAdditive() ) {
	    dto.AdditiveProductParameters = {
	      AdditiveTypeKey: this.AdditiveTypeKey
	    };
	  }

	  if ( this.isPackaging() ) {
	    dto.PackagingProductParameters = {
	      Weight: this.Weight,
	      PackagingWeight: this.PackagingWeight,
	      PalletWeight: this.PalletWeight
	    };
	  }

	  return ko.toJS( dto );
	};

	/** Product maintenance display and editor
	  * @param {Object} input - Observable, Input data to populate editor fields
	  * @param {Object} options - Object containing observable arrays of options
	  * @param {Object} exports - Observable, Container for methods and properties revealed by this editor
	  */
	function ProductMaintenanceEditorVM( params ) {
	  if ( !(this instanceof ProductMaintenanceEditorVM) ) { return new ProductMaintenanceEditorVM( params ); }

	  var self = this;
	  this.disposables = [];

	  // Data
	  var _product = params.input;
	  this.options = {
	    additives: ko.observableArray( [] ),
	    chiles: ko.observableArray( [] )
	  };

	  // Behaviors
	  // Map product data
	  this.productEditor = ko.observable();

	  var _isDirty = ko.observable(  );
	  this.isDirty = ko.pureComputed(function() {
	    return ko.unwrap( _isDirty() );
	  });
	  function buildDirtyChecker() {
	    var root = self.productEditor();
	    var _initialized;

	    var result = ko.computed(function() {
	      if ( !_initialized ) {
	        ko.toJS( root );
	        _initialized = true;
	        return false;
	      }

	      return true;
	    });

	    _isDirty( result );
	  }

	  function init() {
	    function loadAdditivesAsync() {
	      return productsService.getAdditiveTypes()
	        .done(
	          function(data) {
	            self.options.additives(data);
	          });
	    }

	    function loadChileTypesAsync() {
	      return productsService.getChileTypes()
	        .done(function(data) {
	          self.options.chiles(data);
	        });
	    }

	    var loadData = $.when( loadAdditivesAsync(), loadChileTypesAsync() )
	      .done(function() {
	        var editor = new ProductEditor(ko.unwrap(_product), self.options);
	        self.productEditor(editor);
	        buildDirtyChecker();
	      }).fail(function( jqXHR, textStatus, errorThrown ) {
	        showUserMessage( 'Could not load editor', {
	          description: errorThrown
	        });
	      });

	    var isValid = ko.pureComputed(function() {
	      var editor = self.productEditor();

	      return editor && editor.validation.isValid();
	    });

	    var buildExports = loadData.then(function() {
	      // Exports
	      if ( params && params.exports ) {
	        params.exports({
	          toDto: toDto,
	          isValid: isValid,
	          isDirty: self.isDirty,
	          resetDirtyChecker: buildDirtyChecker
	        });
	      }
	    });

	    return buildExports;
	  }

	  init();

	  if ( ko.isObservable( params.input ) ) {
	    var inputSub = params.input.subscribe( function( data ) {
	      if ( data ) {
	        init();
	      }
	    });

	    this.disposables.push( inputSub );
	  }

	  // Compile data for save
	  function toDto() {
	    var editor = self.productEditor();

	    return editor.toDto();
	  }

	  return this;
	}

	ko.utils.extend( ProductMaintenanceEditorVM.prototype, {
	    dispose: function() {
	        ko.utils.arrayForEach( this.disposables, this.disposeOne );
	        ko.utils.objectForEach( this, this.disposeOne );
	    },

	    // little helper that handles being given a value or prop + value
	    disposeOne: function( propOrValue, value ) {
	        var disposable = value || propOrValue;

	        if ( disposable && typeof disposable.dispose === "function" ) {
	            disposable.dispose();
	        }
	    }
	});

	module.exports = {
	  viewModel: ProductMaintenanceEditorVM,
	  template: __webpack_require__(171)
	};

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 171:
/***/ (function(module, exports) {

	module.exports = "<section data-bind=\"ifnot: productEditor\">\n  <div class=\"well text-center\">\n    <i class=\"fa fa-spinner fa-pulse fa-2x\"></i>\n  </div>\n</section>\n<section data-bind=\"with: productEditor\">\n  <div class=\"container-fluid\">\n    <div class=\"row\">\n      <section>\n        <div class=\"form-group col-sm-6\" data-bind=\"validationElement: ProductType\">\n          <label class=\"control-label\" for=\"\">Product Type</label>\n          <select class=\"form-control\" data-bind=\"value: ProductType, options: ProductType.options, optionsText: 'value', optionsValue: 'key', optionsCaption: ' ', enable: isNew\"></select>\n        </div>\n        <div class=\"form-group col-sm-6\" data-bind=\"validationElement: ProductCode\">\n          <label class=\"control-label\" for=\"\">Product Code</label>\n          <input class=\"form-control\" type=\"text\" data-bind=\"textinput: ProductCode, enable: isNew\">\n        </div>\n        <div class=\"form-group col-sm-6\" data-bind=\"validationElement: ProductName\">\n          <label class=\"control-label\" for=\"\">Product Name</label>\n          <input class=\"form-control\" type=\"text\" data-bind=\"textinput: ProductName\">\n        </div>\n      </section>\r\n      <section data-bind=\"visible: isChile\">\r\n        <div class=\"form-group col-sm-6\" data-bind=\"validationElement: ChileState\">\r\n          <label class=\"control-label\" for=\"\">Lot Type</label>\r\n          <select class=\"form-control\" data-bind=\"value: ChileState, options: ChileState.options, optionsText: 'value', optionsValue: 'key', optionsCaption: ' ', enable: isNew\"></select>\r\n        </div>\r\n        <div class=\"form-group col-sm-6\" data-bind=\"validationElement: ChileTypeKey\">\r\n          <label class=\"control-label\" for=\"\">Chile Type</label>\r\n          <select class=\"form-control\" data-bind=\"value: ChileTypeKey, options: $parent.options.chiles, optionsText: 'ChileTypeDescription', optionsValue: 'ChileTypeKey', optionsCaption: ' '\"></select>\r\n        </div>\r\n        <div class=\"form-group col-sm-6\" data-bind=\"validationElement: Mesh\">\r\n          <label class=\"control-label\" for=\"\">Mesh</label>\r\n          <input class=\"form-control\" type=\"text\" data-bind=\"textinput: Mesh\">\r\n        </div>\r\n      </section>\n      <section data-bind=\"visible: isAdditive\">\n        <div class=\"form-group col-sm-6\" data-bind=\"validationElement: AdditiveTypeKey\">\n          <label class=\"control-label\" for=\"\">Additive Type</label>\n          <select class=\"form-control\" data-bind=\"value: AdditiveTypeKey, options: $parent.options.additives, optionsText: 'Description', optionsValue: 'Key', optionsCaption: ' '\"></select>\n        </div>\n      </section>\n    </div>\n    <div class=\"row\" data-bind=\"visible: isChile\">\n      <div class=\"form-group col-xs-12\" data-bind=\"validationElement: IngredientsDescription\">\n        <label class=\"control-label\" for=\"\">Ingredients Description</label>\n        <textarea class=\"form-control\" data-bind=\"textinput: IngredientsDescription\"></textarea>\n      </div>\n    </div>\n    <div class=\"row\">\n      <div class=\"checkbox col-xs-12\">\n        <label class=\"control-label\">\n          <input type=\"checkbox\" data-bind=\"checked: IsActive\"> Active\n        </label>\n      </div>\n    </div>\n  </div>\n  <div class=\"panel panel-default\" data-bind=\"visible: isChile() || isPackaging()\">\n    <div class=\"panel-body\">\n      <ul class=\"nav nav-tabs\">\n          <li class=\"active\" role=\"presentation\" data-bind=\"visible: isChile\">\n          <a href=\"#editor-attributes\" role=\"tab\" data-toggle=\"tab\" aria-expanded=\"true\">Attributes</a>\n          </li>\n          <li role=\"presentation\" data-bind=\"visible: isChile\">\n          <a href=\"#editor-formulation\" role=\"tab\" data-toggle=\"tab\" aria-expanded=\"false\">Formulation</a>\n          </li>\n          <li class=\"active\" role=\"presentation\" data-bind=\"visible: isPackaging\">\n          <a href=\"#editor-attributes\" role=\"tab\" data-toggle=\"tab\" aria-expanded=\"true\">Packaging</a>\n          </li>\n      </ul>\n      <br>\n\n      <section class=\"tab-content\">\n        <section id=\"editor-attributes\" class=\"tab-pane active\" role=\"tabpanel\" data-bind=\"visible: isChile\">\n          <div class=\"panel panel-primary\" data-bind=\"visible: Attributes\">\n            <div class=\"panel-heading\">\n              <h5 class=\"panel-title\">Included Attributes</h5>\n            </div>\n            <div class=\"panel-body\">\n              <table class=\"table table-condensed table-hover\">\n                <thead>\n                  <tr>\n                    <th>Attr</th>\n                    <th>Min</th>\n                    <th>Max</th>\n                  </tr>\n                </thead>\n                <tbody data-bind=\"foreach: editableAttrs\">\n                  <tr>\n                    <td data-bind=\"text: Key\"></td>\n                    <td data-bind=\"validationElement: MinValue\">\n                      <input class=\"form-control\" type=\"text\" data-bind=\"textinput: MinValue\">\n                    </td>\n                    <td data-bind=\"validationElement: MaxValue\">\n                      <input class=\"form-control\" type=\"text\" data-bind=\"textinput: MaxValue\">\n                    </td>\n                  </tr>\n                </tbody>\n              </table>\n            </div>\n          </div>\n        </section>\n        <section id=\"editor-packaging\" class=\"tab-pane active\" role=\"tabpanel\" data-bind=\"visible: isPackaging\">\n          <div class=\"panel panel-primary\">\n            <div class=\"panel-heading\">\n              <h4>Packaging</h4>\n            </div>\n            <div class=\"panel-body\">\n              <div class=\"row\">\n                <div class=\"col-sm-6 col-md-4\">\n                  <div class=\"form-group\">\n                    <label class=\"control-label\" for=\"\">Weight</label>\n                    <input class=\"form-control\" type=\"number\" data-bind=\"textinput: Weight\">\n                  </div>\n                </div>\n                <div class=\"col-sm-6 col-md-4\">\n                  <div class=\"form-group\">\n                    <label class=\"control-label\" for=\"\">Packaging Weight</label>\n                    <input class=\"form-control\" type=\"number\" data-bind=\"textinput: PackagingWeight\">\n                  </div>\n                </div>\n                <div class=\"col-sm-6 col-md-4\">\n                  <div class=\"form-group\">\n                    <label class=\"control-label\" for=\"\">Pallet Weight</label>\n                    <input class=\"form-control\" type=\"number\" data-bind=\"textinput: PalletWeight\">\n                  </div>\n                </div>\n              </div>\n            </div>\n          </div>\n        </section>\n        <section id=\"editor-formulation\" class=\"tab-pane\" role=\"tabpanel\" data-bind=\"if: isChile\">\n          <div class=\"panel panel-primary\">\n            <div class=\"panel-heading\">\n              <h4 class=\"panel-title\">Product Formulation</h4>\n            </div>\n            <div class=\"panel-body\">\n              <table class=\"table table-condensed\">\n                <thead>\n                  <tr>\n                    <th style=\"width: 50px\"></th>\n                    <th>Additive</th>\n                    <th>%</th>\n                  </tr>\n                </thead>\n                <tbody>\n                  <tr>\n                    <td>\n                      <button class=\"btn btn-link\" disabled><i class=\"fa fa-times fa-fw\"></i></button>\n                    </td>\n                    <td>\n                      <span class=\"form-control-static\">WBase</span>\n                    </td>\n                    <td>\n                      <span class=\"form-control-static\" data-bind=\"text: wBasePercent\"></span>\n                    </td>\n                  </tr>\n                  <!-- ko foreach: ProductIngredients -->\n                  <tr>\n                    <td>\n                      <button class=\"btn btn-link\" data-bind=\"command: $parent.removeProductIngredient\"><i class=\"fa fa-times fa-fw\"></i></button>\n                    </td>\n                    <td data-bind=\"validationElement: AdditiveTypeKey\">\r\n                      <select class=\"form-control\" data-bind=\"value: AdditiveTypeKey, options: $parents[1].options.additives, optionsValue: 'Key', optionsText: 'Description', optionsCaption: ' '\"></select>\n                    </td>\n                    <td data-bind=\"validationElement: Percent\">\n                      <input type=\"number\" class=\"form-control\" data-bind=\"fixed: Percent, decimalOptions:{ precision: 2 }\">\n                    </td>\n                  </tr>\n                  <!-- /ko -->\n                </tbody>\n                <tbody>\n                  <tr class=\"clickable text-primary\" data-bind=\"command: addProductIngredient\">\n                    <td class=\"text-center\"><i class=\"fa fa-plus-square fa-fw\"></i></td>\n                    <td colspan=\"2\">Add Item</td>\n                  </tr>\n                </tbody>\n                <tfoot>\n                  <tr>\n                    <td class=\"text-right\" colspan=\"2\"><b>Total Percentage:</b></td>\n                    <td>100%</td>\n                  </tr>\n                </tfoot>\n              </table>\n            </div>\n          </div>\n        </section>\n      </section>\n    </div>\n  </div>\n</section>\n"

/***/ }),

/***/ 172:
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {// TODO NJH: Add sticky headers and column sorting

	/** Product maintenance summary table that displays input data
	  *
	  * @param {Type} input - Observable, Products to display in the summary table
	  * @param {Type} selected - Observable, used to monitor which item has been selected
	  */
	function ProductMaintenanceSummaryVM( params ) {
	  if ( !(this instanceof ProductMaintenanceSummaryVM) ) { return new ProductMaintenanceSummaryVM( params ); }

	  var self = this;

	  // Data
	  this.products = params.input;
	  this.selected = params.selected;

	  this.isTableVisible = ko.pureComputed(function() {
	    var products = self.products();

	    return products && products.length > 0;
	  });

	  // Behaviors
	  this.selectItem = function( data, element ) {
	    // Verify clicked element is or is a child of a tr element
	    var $tr = $( element.target ).closest('tr');

	    // If element has a tr parent
	    if ( $tr.length ) {
	      // Get context of element
	      var context = ko.contextFor( $tr[0] ).$data;

	      // Change selected item to the selected element
	      self.selected( context.ProductKey );
	    }
	  };

	   function updateItem( data ) {
	    var item = ko.utils.arrayFirst( ko.unwrap( self.products ), function( product ) {
	      return product.ProductKey === data.ProductKey;
	    });

	    // Products data is computed, not observableArray
	    var prodData = self.products();

	    if ( item ) {
	      var index = self.products().indexOf( item );

	      // Update item info if in table
	      if ( index > -1 ) {
	        prodData.splice( index, 1, data );
	        self.products( prodData );

	      // Else append to table
	      } else {
	        self.products( [ data ].concat( prodData ) );
	      }
	    } else {
	      // Append item to table
	      self.products( [ data ].concat( prodData ) );
	    }
	  }

	  // Exports
	  if ( params && params.exports ) {
	    params.exports({
	      updateItem: updateItem
	    });
	  }

	  return this;
	}

	module.exports = {
	  viewModel: ProductMaintenanceSummaryVM,
	  template: __webpack_require__(173)
	};



	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),

/***/ 173:
/***/ (function(module, exports) {

	module.exports = "<section>\r\n  <div class=\"panel panel-primary\">\r\n    <div class=\"panel-heading\">\r\n      <h2 class=\"panel-title\">Products</h2>\r\n    </div>\r\n    <div class=\"table-responsive\" style=\"max-height: 500px\">\r\n      <table class=\"table table-condensed table-hover\" data-bind=\"floatThead: products, sortableTable: products\">\r\n        <thead>\r\n          <tr>\r\n            <th data-sort=\"ProductCode\">Prod. Code</th>\r\n            <th data-sort=\"ProductName\">Product</th>\r\n            <th data-sort=\"IsActive\">Active</th>\r\n          </tr>\r\n        </thead>\r\n        <tbody class=\"clickable\" data-bind=\"foreach: products, click: selectItem\">\r\n          <tr>\r\n            <td data-bind=\"text: ProductCode\"></td>\r\n            <td data-bind=\"text: ProductName\"></td>\r\n            <td data-bind=\"text: IsActive\"></td>\r\n          </tr>\r\n        </tbody>\r\n      </table>\r\n    </div>\r\n    <br>\r\n  </div>\r\n</section>\r\n"

/***/ })

});