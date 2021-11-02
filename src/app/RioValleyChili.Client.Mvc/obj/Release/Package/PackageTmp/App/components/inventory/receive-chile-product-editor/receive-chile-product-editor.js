/* KO Components */
ko.components.register( 'product-selector', require('App/components/common/product-selector/product-selector') );

/* Required libraries */
var rvc = require('app');
var warehouseService = require('App/services/warehouseService');

function ItemReceived( input, isDehydrated ) {
  var self = this;

  // Editor data
  this.ItemKey = input.ItemKey;
  this.Variety = ko.observable( input.Variety );
  this.ToteKey = ko.observable( input.ToteKey );
  this.Quantity = ko.numericObservable( input.Quantity );
  this.GrowerCode = ko.observable( input.GrowerCode );

  this.Location = ko.observable( input.Location );
  this.PackagingProduct = ko.observable(input.PackagingProduct);
  
  // Computed values
  this.TotalWeight = ko.pureComputed(function () {
    var quantity = self.Quantity();
    var packaging = self.PackagingProduct();
    var weight = packaging && +packaging.Weight;

    return !isNaN( weight ) && quantity > 0 ?
      quantity * weight :
      0;
  });

  // Validation
  function isRequired() {
    return self.Quantity() > 0 || self.ToteKey() != null;
  }

  this.ToteKey.extend({
    toteKey: true,
    required: {
      message: 'This field is required.',
      onlyIf: function() {
        return isDehydrated() && isRequired();
      }
    }
  });
  this.Quantity.extend({
    required: {
      message: 'This field is required.',
      onlyIf: isRequired
    }
  });
  this.Variety.extend({
    required: {
      message: "This field is required.",
      onlyIf: isRequired
    }
  });
  this.GrowerCode.extend({
    required: {
      message: "This field is required.",
      onlyIf: function() {
        return isDehydrated() && isRequired();
      }
    }
  });
  this.Location.extend({
    required: {
      message: "This field is required.",
      onlyIf: isRequired
    }
  });
  this.PackagingProduct.extend({
    required: {
      message: "This field is required.",
      onlyIf: isRequired
    }
  });
}

ItemReceived.prototype.toDto = function() {
  return {
    GrowerCode: this.GrowerCode,
    ToteKey: this.ToteKey,
    Quantity: this.Quantity,
    PackagingProductKey: this.PackagingProduct().ProductKey,
    Variety: this.Variety,
    LocationKey: this.Location().LocationKey,
  };
};

function Editor( input, options ) {
  var self = this;

  // Static data and states
  this.LotKey = input.LotKey;
  this.isNew = this.LotKey === 'new' || this.LotKey == null;
  this.isEditingEnabled = input.isEditingEnabled;

  this.ChileMaterialsReceivedType = ko.observable( input.ChileMaterialsReceivedType );
  this.isDehydrated = ko.pureComputed(function() {
    return self.ChileMaterialsReceivedType() === 0;
  });
  this.editorTemplateName = ko.pureComputed(function() {
    return self.isDehydrated() ? 'table-dehydrated' : 'table-other-raw';
  });

  var _defaults = {
    packaging: rvc.constants.ThousandPoundTotePackaging.ProductKey,
    location: rvc.constants.DehyLocation.LocationKey,
    warehouse: rvc.constants.rinconWarehouse.WarehouseKey
  };

  // Product header info
  this.DateReceived = ko.observableDate( input.DateReceived ).extend({ required: true });
  this.Supplier = ko.observable( input.Supplier && input.Supplier.CompanyKey ).extend({ required: true });
  this.LoadNumber = ko.numericObservable( input.LoadNumber );
  this.Product = ko.observable( input.ChileProduct && input.ChileProduct.ProductKey ).extend({ required: true });
  this.Treatment = ko.observable( input.Treatment && input.Treatment.TreatmentKey ).extend({ required: true, treatmentType: true });
  this.PurchaseOrder = ko.observable( input.PurchaseOrder );
  this.ShipperNumber = ko.observable( input.ShipperNumber );
  this.Warehouse = ko.observable( _defaults.warehouse );
  if ( input.Items && input.Items.length ) {
    this.Warehouse( input.Items[0].Location.FacilityKey );
  }

  this.options = options;

  this.chileState = ko.observable( input.ChileProduct &&
    typeof input.ChileProduct.ChileState === 'number' ?
      input.ChileProduct.ChileState :
      1 ).extend({ chileType: true });
  this.chileProductOptions = ko.pureComputed(function() {
    var _chileState = +self.chileState();
    var _products = self.options.chileProducts();
    var _filteredProducts = ko.utils.arrayFilter( _products, function( product ) {
      return product.ChileState === _chileState;
    });

    return _filteredProducts;
  });
  this.isDehydrated.subscribe(function( isDehy ) {
    if ( isDehy ) {
      self.Warehouse( "2" );
      self.chileState( "1" );
    }
  });
  this.isMaterialsTypeEnabled = ko.pureComputed(function() {
    return self.chileState() === "1";
  });

  this.loadWarehouseLocations = ko.asyncCommand({
    execute: function( complete ) {
      var _warehouse = self.Warehouse();
      var getWarehouseLocations = warehouseService.getWarehouseLocations( _warehouse ).then(
      function( data, textStatus, jqXHR ) {
        var _warehouseLocs = self.options.warehouseLocations();

        _warehouseLocs[ _warehouse ] = data;

        self.options.warehouseLocations( _warehouseLocs );
      }).always( complete );

    }
  });

  this.locationOptions = ko.pureComputed(function() {
    var _warehouse = self.Warehouse();
    var _options = ko.unwrap( options.warehouseLocations )[ _warehouse ];

    if ( !_options ) {
      self.loadWarehouseLocations.execute();
    }

    return _options || [];
  });

  // Item editor
  var _items = (input.Items || []).map( function( item ) {
    return this.buildMaterialReceivedItem( item, this.isDehydrated );
  }, this );
  this.Items = ko.observableArray( _items || [] );

  this.totalWeight = ko.pureComputed(function () {
    var sumOfWeight = 0;
    ko.utils.arrayForEach( self.Items(), function ( item ) {
      sumOfWeight += ko.unwrap( item.TotalWeight );
    } );
    return sumOfWeight;
  });

  // Item list manipulation
  function scrollToLastMaterial( Parameters ) {
    var $lastItem = $('#editor-item-list').find('tr').last();
    if ( $lastItem.length ) {
      $lastItem.find('input').first().focus();

      var $popup = $lastItem.closest('.popupWindow');

      if ( $popup.length ) {
        $popup.scrollTop( $popup[0].scrollHeight );
      }
    }
  }

  function addItem() {
    var initialValues = {};
    var items = self.Items();

    // If items exist, get data from last item in list
    if ( items.length ) {
      var previousItem = items[items.length - 1];
      initialValues.ToteKey = self.isDehydrated() ? previousItem.ToteKey.getNextTote() : '';
      initialValues.Variety = previousItem.Variety();
      initialValues.Location = previousItem.Location();
      initialValues.Quantity = previousItem.Quantity();
      initialValues.PackagingProduct = previousItem.PackagingProduct();
      initialValues.GrowerCode = previousItem.GrowerCode();
      // Else, use default values
    } else {
      initialValues.PackagingKey = _defaults.packaging;
      initialValues.LocationKey = _defaults.location;
      initialValues.Quantity = 1;

      if ( self.isDehydrated() ) {
        var _supplier = self.Supplier();
        var _grower = ko.utils.arrayFirst( self.options.suppliers(), function( grower ) {
          return grower.CompanyKey === _supplier;
        });
        initialValues.GrowerCode = _grower && _grower.Name;
      }
    }

    var newItem = self.buildMaterialReceivedItem( initialValues );
    self.Items.push( newItem );

  }
  this.addItemCommand = ko.command({
    execute: function () {
      addItem();

      scrollToLastMaterial();
    }
  });

  this.removeItem = function ( data, element ) {
    // Find index and remove item
    var items = self.Items();
    var i;
    var len = items.length;
    for ( i = 0; i < len; i++ ) {
      if ( items[i] === data ) {
        self.Items.splice( i, 1 );
        return;
      }
    }
  };

  // Default actions on new receiving requests
  if ( this.isNew ) {
    this.addItemCommand.execute();
  }

  // Validation
  this.validation = ko.validatedObservable({
    DateReceived: this.DateReceived,
    supplier: this.Supplier,
    product: this.Product,
    items: this.Items
  });

}

Editor.prototype.buildMaterialReceivedItem = function ( values, isDehydrated ) {
  values.ToteKey = values.Tote || values.ToteKey;
  values.GrowerCode = values.Grower || values.GrowerCode;
  values.Location = values.Location || this.findLocationByKey( values.LocationKey );
  values.PackagingProduct = this.findPackagingProductByKey( values.PackagingProduct && values.PackagingProduct.ProductKey );

  var item = new ItemReceived( values, this.isDehydrated );

  var isDuplicateTote = function( tote ) {
    var results = 0;

    if ( this.Items == null ) {
      return false;
    }

    var items = this.Items();

    var i;
    var len = items.length;
    for ( i = 0; i < len; i++ ) {
      if ( items[i].ToteKey() === tote ) {
        results += 1;
      }

      if ( results > 1 ) {
        return true;
      }
    }
  }.bind( this );

  item.Location.extend({
    validation: {
      validator: function( location ) {
        var _locationKey = location && location.LocationKey;
        var _locations = this.locationOptions();

        return ko.utils.arrayFirst( _locations, function( location ) {
          return location.LocationKey === _locationKey;
        });
      }.bind( this ),
      message: 'Location does not exist for the selected Warehouse.'
    }
  });

  item.ToteKey.extend({
    validation: {
      message: 'This tote has already been used!',
      validator: function ( tote ) {
        return tote == null || !isDuplicateTote( tote );
      }
    }
  });

  return item;
};

Editor.prototype.findPackagingProductByKey = function ( keyValue ) {
  return ko.utils.arrayFirst( this.options.packaging(), function ( p ) {
    if ( p.ProductKey === keyValue ) {
      return p;
    }
  } );
};

Editor.prototype.findLocationByKey = function ( keyValue ) {
  return ko.utils.arrayFirst( this.options.warehouseLocations(), function ( loc ) {
    if ( loc.LocationKey === keyValue ) {
      return loc;
    }
  } );
};

Editor.prototype.toDto = function() {
  var dto = {
    LotKey: this.LotKey,
    ChileMaterialsReceivedType: this.ChileMaterialsReceivedType,
    DateReceived: this.DateReceived,
    LoadNumber: this.LoadNumber,
    PurchaseOrder: this.PurchaseOrder,
    ShipperNumber: this.ShipperNumber,
    ChileProductKey: this.Product,
    SupplierKey: this.Supplier,
    TreatmentKey: this.Treatment,

    Items: this.Items().map( function( item ) {
      return item.toDto();
    }, this ),
  };

  return ko.toJS( dto );
};

function ReceiveChileProductEditorVM( params ) {
  if ( !(this instanceof ReceiveChileProductEditorVM) ) { return new ReceiveChileProductEditorVM( params ); }

  var self = this;
  this.disposables = [];

  // Exposed methods and properties
  function toDto() {
    var _editor = self.editor();

    return _editor && _editor.toDto();
  }

  var isValid = ko.pureComputed(function() {
    var _editor = self.editor();

    return _editor && _editor.validation.isValid();
  });

  function showErrors() {
    var _editor = self.editor();

    return _editor && _editor.validation.errors.showAllMessages();
  }

  function next() {
    var _editor = self.editor().toDto();
    var _data = {
      LoadNumber: _editor.LoadNumber + 1,
      Supplier: { CompanyKey: _editor.DehydratorKey },
      Product: _editor.ChileProductKey,
      DateReceived: _editor.DateReceived
    };

    self.editor( buildEditor( _data ) );
  }

  // Editor construction
  var options = params.options;
  function buildEditor( editorData ) {
    var _editor = new Editor( ko.unwrap( editorData ), options );

    function serialize() {
      return ko.toJSON({
        LotKey: _editor.LotKey,
        ChileMaterialsReceivedType: _editor.ChileMaterialsReceivedType,
        DateReceived: _editor.DateReceived,
        LoadNumber: _editor.LoadNumber,
        PurchaseOrder: _editor.PurchaseOrder,
        ShipperNumber: _editor.ShipperNumber,
        ChileProductKey: _editor.Product,
        SupplierKey: _editor.Supplier,
        TreatmentKey: _editor.Treatment,
        Items: _editor.Items
      });
    }

    _editor.dirtyFlag = (function () {
      var result = function() {},
        _initialState = ko.observable();
      result.isDirty = ko.computed(function () {
        var initialState = _initialState();
        return initialState == null ? false : initialState !== serialize();
      });

      result.reset = function () {
        _initialState(serialize());
      };


      setTimeout(result.reset);

      return result;
    })();

    if ( ko.isObservable( params.exports ) ) {
      params.exports({
        toDto: toDto,
        isValid: isValid,
        showErrors: showErrors,
        next: next,
        materialsType: _editor.ChileMaterialsReceivedType,
        addItemCommand: _editor.addItemCommand,
        isDirty: _editor.dirtyFlag.isDirty,
        resetDirtyFlag: _editor.dirtyFlag.reset
      });
    }

    return _editor;
  }

  var values = $.extend({}, ReceiveChileProductEditorVM.prototype.DEFAULT_VALUES, params.input());
  this.recapReportLink = values.Links['report-recap'];
  this.recapReportLink = this.recapReportLink && this.recapReportLink.HRef;
  this.editor = ko.observable( buildEditor( values ) );
  var inputSub = params.input.subscribe(function( newProduct ) {
    if ( newProduct ) {
      self.editor( buildEditor( newProduct ) );
    }
  });
  this.disposables.push(inputSub);

  this.displayReportLinks = ko.pureComputed(function() {
    var editor = this.editor();
    if (editor == null) return null;
    return !editor.isNew && !editor.dirtyFlag.isDirty();
  }, this);

  // Exports
  return self;
}

ReceiveChileProductEditorVM.prototype.DEFAULT_VALUES = {
  Links: {}
}

ko.utils.extend(ReceiveChileProductEditorVM.prototype, {
    dispose: function() {
        ko.utils.arrayForEach(this.disposables, this.disposeOne);
        ko.utils.objectForEach(this, this.disposeOne);
    },

    // little helper that handles being given a value or prop + value
    disposeOne: function(propOrValue, value) {
        var disposable = value || propOrValue;

        if (disposable && typeof disposable.dispose === "function") {
            disposable.dispose();
        }
    }
});

module.exports = {
  viewModel: ReceiveChileProductEditorVM,
  template: require('./receive-chile-product-editor.html')
};
