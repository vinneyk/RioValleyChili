/** Required Libraries */
var productsService = require('App/services/productsService');

/** Constructors */
function SpecAttribute( attrData, defaultAttrs ) {
  this.AttributeNameKey = attrData.Key;

  // Editable data
  this.RangeMin = ko.observable( attrData.RangeMin != null ? attrData.RangeMin : '' );
  this.RangeMax = ko.observable( attrData.RangeMax != null ? attrData.RangeMax : '' );
  this.defaultRangeMin = ko.pureComputed(function() {
    var _defaultAttrs = defaultAttrs()[ attrData.Key ];

    return _defaultAttrs && _defaultAttrs.RangeMin;
  });
  this.defaultRangeMax = ko.pureComputed(function() {
    var _defaultAttrs = defaultAttrs()[ attrData.Key ];

    return _defaultAttrs && _defaultAttrs.RangeMax;
  });

  // Validation
  var _isRequired = ko.pureComputed(function() {
    return this.RangeMin() || this.RangeMax();
  }, this);

  this.RangeMin.extend({
    required: {
      onlyIf: _isRequired,
      message: 'Both min and max values are required.',
    },
    number: true
  });
  this.RangeMax.extend({
    required: {
      onlyIf: _isRequired,
      message: 'Both min and max values are required.',
    },
    min: {
      params: this.RangeMax,
      message: 'Max value must be greater than or equal to min value'
    },
    number: true
  });
}

function CustomerSpecEditor( data, options ) {
  this.options = options;
  this.CustomerKey = data.CustomerKey;

  // Editable data
  // Map attribute name options with existing ranges
  var defaultAttributes = ko.observable( {} );
  this.getDefaultAttributes = ko.asyncCommand({
    execute: function( productKey, complete ) {
      var getAttributes = productsService.getProductDetails( '1', productKey ).then(
      function( data, textStatus, jqXHR ) {
        var _attrs = {};
        var attributeRanges = data.AttributeRanges;

        ko.utils.arrayForEach( attributeRanges, function( attr ) {
          _attrs[ attr.AttributeNameKey ] = attr;
        });

        defaultAttributes( _attrs );
        defaultAttributes.valueHasMutated();
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not get default attributes for product', {
          description: errorThrown
        });
      }).always( complete );
    }.bind( this ),
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  var ranges = ko.utils.arrayMap( options.attributes(), function( attr ) {
    var _attrRanges = data.AttributeRanges;
    var _attr = JSON.parse( ko.toJSON( attr ) );
    var matchedAttrRange;

    if ( _attrRanges ) {
      matchedAttrRange = ko.utils.arrayFirst( _attrRanges, function( attrRange ) {
        return attrRange.AttributeShortName === _attr.Key;
      });
    }

    if ( matchedAttrRange ) {
      _attr.RangeMin = matchedAttrRange.RangeMin;
      _attr.RangeMax = matchedAttrRange.RangeMax;
    }

    return new SpecAttribute( _attr, defaultAttributes );
  });

  this.AttributeRanges = ko.observableArray( ranges ).extend({
    validation: {
      validator: function( attrs ) {
        return ko.utils.arrayFirst( attrs, function( attr ) {
          return attr.RangeMin() !== '' && attr.RangeMax() !== '';
        });
      },
    }
  });

  this.ChileProduct = ko.observable().extend({ required: true });
  this.ChileProduct.subscribe(function( productData ) {
    if ( productData ) {
      var _productKey = typeof productData === 'string' ? productData : productData.ProductKey;

      this.getDefaultAttributes.execute( _productKey );
    }
  }, this );

  var _initialProductValue = ko.utils.arrayFirst( this.options.products(), function( product ) {
    return product.ProductKey === data.ChileProductKey;
  });
  this.ChileProduct( _initialProductValue || data.ChileProductKey );

  this.isNew = !data.ChileProductKey;

  // Validation
  this.validation = ko.validatedObservable({
    AttributeRanges: this.AttributeRanges,
    Product: this.ChileProductKey
  });
}

// Compile editor data to dto
CustomerSpecEditor.prototype.toDto = function() {
  var dto = {
    CustomerKey: this.CustomerKey,
    ChileProduct: this.ChileProduct,
    AttributeRanges: []
  };

  ko.utils.arrayForEach( this.AttributeRanges(), function( attr ) {
    var _min = attr.RangeMin();
    var _max = attr.RangeMin();

    if ( _min !== '' && _max !== '' ) {
      dto.AttributeRanges.push( attr );
    }
  });

  return ko.toJS( dto );
};

/** Customer Product Spec Editor view model
  *
  * @param {Object} input - Description
  * @param {Object} options - Description
  * @param {Object} exports - Description
  */
function CustomerProductSpecEditorVM( params ) {
  if ( !(this instanceof CustomerProductSpecEditorVM) ) { return new CustomerProductSpecEditorVM( params ); }

  var self = this;

  // Data
  // Editor data
  this.productSpecEditor = ko.observable();

  // Behaviors
  // Build editor data
  function init( data ) {
    var _data = ko.unwrap( data );

    var specEditor = new CustomerSpecEditor( _data, ko.unwrap( params.options ) );

    self.productSpecEditor( specEditor );
  }

  init( params.input );

  // Compile data into a DTO
  function toDto() {
    return self.productSpecEditor().toDto();
  }

  var isValid = ko.pureComputed(function() {
    var _editor = self.productSpecEditor();

    return _editor && _editor.validation.isValid();
  });

  // Exports
  if ( params && params.exports ) {
    params.exports({
      toDto: toDto,
      isValid: isValid
    });
  }

  return this;
}

module.exports = {
  viewModel: CustomerProductSpecEditorVM,
  template: require('./customer-product-spec-editor.html')
};
