/** Knockout components */
ko.components.register( 'product-maintenance-editor', require('App/components/quality-control/product-maintenance-editor/product-maintenance-editor'));
ko.components.register( 'product-maintenance-summary', require('App/components/quality-control/product-maintenance-summary/product-maintenance-summary'));

/** Service libraries */
var productsService = require('App/services/productsService');
var lotService = require('App/services/lotService');

/** Extensions */
require('App/koExtensions');
require('bootstrap');
require('App/ko.bindingHandlers.sortableTable');

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
var page = require('page');

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
