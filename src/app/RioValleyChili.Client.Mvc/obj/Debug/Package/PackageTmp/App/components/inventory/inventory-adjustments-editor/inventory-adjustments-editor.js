var inventoryService = require('App/services/inventoryService');
var warehouseLocationsService = require('App/services/warehouseLocationsService');
var lotService = require('App/services/lotService');

require('App/helpers/koValidationHelpers');
require('koProjections');

ko.components.register( 'product-selector', require('App/components/common/product-selector/product-selector'));

/** Sorts select element options based on existing inventory */
function OptionsFilters( options, optionValue, keysWithInventory ) {
  this.optionsWithInventory = ko.pureComputed(function() {
    var keys = ko.unwrap( keysWithInventory );
    var opts = ko.unwrap( options );

    return ko.utils.arrayFilter( opts, function( opt ) {
      return keys.indexOf( opt[ optionValue ] ) > -1;
    });
  });

  this.optionsWithoutInventory = ko.pureComputed(function() {
    var keys = ko.unwrap( keysWithInventory );
    var opts = ko.unwrap( options );

    return ko.utils.arrayFilter( opts, function( opt ) {
      return keys.indexOf( opt[ optionValue ] ) === -1;
    });
  });
}

/** Single adjustment row in adjustments table */
function AdjustmentItem( options ) {
  var self = this;

  this.working = ko.observable( false );

  /** Options to populate autocomplete */
  this.options = options;

  /** Display/Save data */
  this.LotKey = ko.observable().extend({ lotKey: searchLot, required: true });
  this.ProductName = ko.observable();
  this.WarehouseKey = ko.observable('').extend({ required: true });
  this.WarehouseLocationKey = ko.observable('').extend({ required: true });
  this.PackagingProductKey = ko.observable('').extend({ required: true });
  this.TreatmentKey = ko.observable('').extend({ required: true });
  this.ToteKey = ko.observable();
  this.Adjustment = ko.observable().extend({ required: true });

  this.existingInventory = ko.observable( {} );

  /** State variables */
  this.lotExists = ko.observable( false );
  this.LotKey.subscribe(function() {
    self.lotExists( false );
    self.ProductName( null );
  });

  /** Hidden data */
  var _quantity = ko.pureComputed(function() {
    var inv = self.existingInventory();
    var warehouse = self.WarehouseKey();
    var location = self.WarehouseLocationKey();
    var packaging = self.PackagingProductKey();
    var treatment = self.TreatmentKey();

    if ( warehouse !== null &&
          location !== null &&
          packaging !== null &&
          treatment !== null ) {
      var match = inv[warehouse] && inv[warehouse][location] && inv[warehouse][location][packaging] && inv[warehouse][location][packaging][treatment];

      return match && match.Quantity;
    }

    return null;
  });

  /** Packaging product for calculating weight */
  var packagingProduct = ko.pureComputed(function() {
    var key = self.PackagingProductKey();

    return ko.utils.arrayFirst( self.options.packaging(), function( packaging ) {
      return packaging.ProductKey === key;
    });
  });

  /** Read only data */
  this.adjustmentWeight = ko.pureComputed(function() {
    var packaging = packagingProduct();
    var adjustment = +self.Adjustment();
    var weight = packaging && +packaging.Weight;

    return adjustment * weight || null;
  });

  this.newQuantity = ko.pureComputed(function() {
    var qty = +_quantity() || 0;
    var adjustment = +self.Adjustment() || 0;

    return qty + adjustment;
  }).extend({
    validation: {
      validator: function( value ) {
        return value >= 0;
      },
      message: "New quantity can't be negative"
    }
  });

  this.newWeight = ko.pureComputed(function() {
    var packaging = packagingProduct();
    var weight = packaging && +packaging.Weight;
    var qty = +self.newQuantity();

    return qty * weight;
  });

  /** Options to populate locations list */
  this.allLocationOptions = ko.computed(function() {
    var facilityKey = self.WarehouseKey();

    if ( typeof facilityKey !== 'string' || typeof facilityKey === 'string' && facilityKey.length === 0 ) {
      return [];
    }

    var locations = self.options.locations;
    var opts = locations()[ self.WarehouseKey() ];

    if ( opts ) {
      return opts;
    }

    self.working( true );
    var getOpts = warehouseLocationsService.getWarehouseLocations( facilityKey ).then(
    function( data, textStatus, jqXHR ) {
      var locs = locations();
      locs[ facilityKey ] = data;
      locations( locs );
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not fetch locations', {
        description: errorThrown
      } );
    }).always( function() {
      self.working( false );
    });
  });

  /** Cascading sort to display what options have existing inventory */
  this.facilityOptions = ko.observable(  );
  this.locationOptions = ko.observable(  );
  this.packagingOptions = ko.observable(  );
  this.treatmentOptions = ko.observable(  );

  function buildOptions( inventoryMap ) {
    var facilityOptKeys = ko.pureComputed(function() {
      return Object.keys( inventoryMap );
    });
    self.facilityOptions( new OptionsFilters( self.options.facilities, 'FacilityKey', facilityOptKeys ) );

    var locationOptKeys = ko.pureComputed(function() {
      var inventory = self.existingInventory();

      var keysObj = inventory && inventory[ self.WarehouseKey() ];

      return keysObj && Object.keys( keysObj ) || [];
    });
    var locationOpts = ko.pureComputed(function() {
      return self.options.locations()[ self.WarehouseKey() ];
    });
    self.locationOptions( new OptionsFilters( locationOpts, 'LocationKey', locationOptKeys ) );

    var packagingOptKeys = ko.pureComputed(function() {
      var inventory = self.existingInventory();
      var warehouse = inventory && inventory[ self.WarehouseKey() ];

      var keysObj = warehouse && warehouse[ self.WarehouseLocationKey() ];

      return keysObj && Object.keys( keysObj ) || [];
    });
    self.packagingOptions( new OptionsFilters( self.options.packaging, 'ProductKey', packagingOptKeys ) );

    var treatmentOptKeys = ko.pureComputed(function() {
      var inventory = self.existingInventory();
      var warehouse = inventory && inventory[ self.WarehouseKey() ];
      var warehouseLocation = warehouse && warehouse[ self.WarehouseLocationKey() ];

      var keysObj = warehouseLocation && warehouseLocation[ self.PackagingProductKey() ];

      return keysObj && Object.keys( keysObj ) || [];
    });
    self.treatmentOptions( new OptionsFilters( self.options.treatments, 'key', treatmentOptKeys ) );
  }

  function mapExistingInventory( data ) {
    var inventoryMap = {};

    var inventory = ko.utils.arrayForEach( data.InventoryItems, function( item ) {
      var inventoryFacility = inventoryMap[ item.Location.FacilityKey ];
      var inventoryLocation = null;
      var inventoryPackaging = null;
      var inventoryTreatment = null;

      if ( !inventoryFacility ) {
        inventoryMap[ item.Location.FacilityKey ] = {};
        inventoryFacility = inventoryMap[ item.Location.FacilityKey ];
      }

      inventoryLocation = inventoryFacility[ item.Location.LocationKey ];

      if ( !inventoryLocation ) {
        inventoryFacility[ item.Location.LocationKey ] = {};
        inventoryLocation = inventoryFacility[ item.Location.LocationKey ];
      }

      inventoryPackaging = inventoryLocation[ item.PackagingProduct.ProductKey ];

      if ( !inventoryPackaging ) {
        inventoryLocation[ item.PackagingProduct.ProductKey ] = {};
        inventoryPackaging = inventoryLocation[ item.PackagingProduct.ProductKey ];
      }

      inventoryTreatment = inventoryPackaging[ item.InventoryTreatment.TreatmentKey ];

      if ( !inventoryTreatment ) {
        inventoryPackaging[ item.InventoryTreatment.TreatmentKey ] = item;
      }
    } );

    return inventoryMap;
  }

  /** Fetches data for lots and populates `self.existingData` */
  function searchLot( lotKey ) {
    self.working( true );

    var searchLotKey = inventoryService.getInventoryByLot( lotKey ).then(
    function( data, textStatus, jqXHR ) {
      var inventoryMap = mapExistingInventory( data );

      self.existingInventory( inventoryMap );
      buildOptions(inventoryMap);

      if (data.InventoryItems.length) {
        self.ProductName(data.InventoryItems[0].Product.ProductName);
      }
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Lot does not exists', {
        description: 'The lot <b>' + lotKey + '</b> could not be found.'
      });
    }).always(function Name( Parameters ) {
      self.working( false );
    });

    var getProductName = searchLotKey.then(
    function( data, textStatus, jqXHR ) {
      if ( !self.ProductName() ) {
        var getLotData = lotService.getLotData( lotKey ).then(
        function( data, textStatus, jqXHR ) {
          self.ProductName( data.LotSummary.Product.ProductName );
          self.lotExists( true );
        },
        function( jqXHR, textStatus, errorThrown ) {
          showUserMessage( 'Could not get product type for lot', {
            description: errorThrown
          });
        });

        return getLotData;
      }

      self.lotExists( true );
      return $.Deferred().resolve( data );
    });

    return getProductName;
  }

  this.validation = ko.validatedObservable({
    LotKey: self.LotKey,
    WarehouseKey: self.WarehouseKey,
    WarehouseLocationKey: self.WarehouseLocationKey,
    PackagingProductKey: self.PackagingProductKey,
    TreatmentKey: self.TreatmentKey,
    Quantity: self.newQuantity
  });

  /** Used to build DTO. Avoids options lists, which improves performance */
  this.editableData = {
    lotKey: self.LotKey,
    facility: self.WarehouseKey,
    location: self.WarehouseLocationKey,
    packaging: self.PackagingProductKey,
    treatment: self.TreatmentKey,
    adjustment: self.Adjustment,
  };
}

/** Adjustment Editor data model */
function InventoryAdjustmentsEditor( data, options ) {
  var _data = typeof data === "object" ? data : {};

  /** Static Data */
  this.InventoryAdjustmentKey = _data.InventoryAdjustmentKey;
  this.AdjustmentDate = ko.observable( _data.AdjustmentDate || '' );
  this.User = _data.User;
  this.Notebook = _data.Notebook && _data.Notebook.Notes[0];
  this.TimeStamp = _data.TimeStamp;

  /** Editor data */
  this.Comment = ko.observable( '' );

  if ( data === 'new' ) {
    this.InventoryAdjustments = ko.observableArray( [] );
  } else {
    this.InventoryAdjustments = _data.Items;
  }

  this.canRemove = ko.pureComputed(function() {
    return ko.unwrap( this.InventoryAdjustments ).length > 1;
  }, this);

  // Behaviors
  this.removeItem = function( data, element ) {
    var index = this.InventoryAdjustments().indexOf( data );

    this.InventoryAdjustments.splice( index, 1 );
  }.bind( this );

  this.addAdjustment = function() {
    this.InventoryAdjustments.push( new AdjustmentItem( options ) );
  }.bind( this );

  if ( data === 'new' ) {
    this.addAdjustment();
    this.totalAdjustments = ko.pureComputed(function() {
      var total = 0;

      ko.utils.arrayForEach( this.InventoryAdjustments(), function( adjustment ) {
        total += +adjustment.Adjustment();
      } );

      return total || 0;
    }, this);

    this.isValid = ko.pureComputed(function() {
      return !ko.utils.arrayFirst( this.InventoryAdjustments(), function( adjustment ) {
        return !adjustment.validation.isValid();
      });
    }, this);
  }
}

/** Adjustments VM */
function InventoryAdjustmentsEditorVM( params ) {
  if ( !(this instanceof InventoryAdjustmentsEditorVM) ) { return new InventoryAdjustmentsEditorVM( params ); }

  var self = this;

  // Data
  var _options = params.options;
  this.isNew = ko.observable( true );
  this.adjustmentEditor = ko.observable( null );

  this.dirtyFlag = (function( data ) {
    var root = ko.pureComputed(function() {
      var rootData = data();
      return rootData && {
        notes: rootData.Notes,
        items: ko.utils.arrayMap( ko.unwrap( rootData.InventoryAdjustments ), function( item ) {
          return item.editableData;
        })
      };
    });

    var result = function() {};
    var _initialState = ko.observable( ko.toJSON( root ) );

    result.isDirty = ko.pureComputed(function() {
      return _initialState() !== ko.toJSON( root );
    });

    result.reset = function() {
      _initialState( ko.toJSON( root ) );
    };

    return result;
  })( this.adjustmentEditor );

  params.input.subscribe( function( inputData ) {
    if ( inputData ) {
      initEditor( inputData );
    }
  });

  function initEditor( inputData ) {
    self.adjustmentEditor( null );
    self.isNew( inputData === 'new' );
    self.adjustmentEditor( new InventoryAdjustmentsEditor( inputData, _options ) );
    self.dirtyFlag.reset();
  }

  // Behaviors
  function toDto() {
    var editorData = self.adjustmentEditor();

    if ( editorData.isValid() ) {
      var dto = {
        Comment: editorData.Comment,
        InventoryAdjustments: ko.utils.arrayMap( editorData.InventoryAdjustments(), function( adjustment ) {
          return {
            LotKey: adjustment.LotKey,
            WarehouseKey: adjustment.WarehouseKey,
            WarehouseLocationKey: adjustment.WarehouseLocationKey,
            PackagingProductKey: adjustment.PackagingProductKey,
            TreatmentKey: adjustment.TreatmentKey,
            ToteKey: adjustment.ToteKey,
            Adjustment: adjustment.Adjustment
          };
        }),
      };

      return ko.toJS( dto );
    }

    return null;
  }

  initEditor( params.input() );

  // Exports
  if ( params && params.exports ) {
    params.exports({
      dirtyFlag: this.dirtyFlag,
      toDto: toDto
    });
  }

  return this;
}

module.exports = {
  viewModel: InventoryAdjustmentsEditorVM,
  template: require('./inventory-adjustments-editor.html')
};

