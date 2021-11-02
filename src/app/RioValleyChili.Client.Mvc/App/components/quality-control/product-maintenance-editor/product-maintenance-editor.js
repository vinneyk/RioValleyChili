var rvc = require('rvc');
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
var productsService = require('App/services/productsService');

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
  template: require('./product-maintenance-editor.html')
};
