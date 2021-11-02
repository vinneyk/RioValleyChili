/** KO Components */
ko.components.register( 'contact-label', require('App/components/warehouse/contact-label-editor/contact-label-editor') );
ko.components.register( 'product-selector', require('App/components/common/product-selector/product-selector') );
ko.components.register( 'label-helper', require('App/components/common/address-book/company-address-label-helper') );

/** Libraries */
var salesService = require('App/services/salesService');
require('bootstrap');

/** Quote Item for Items table */
function Item( itemData ) {
  this.SalesQuoteItemKey = itemData.SalesQuoteItemKey;

  this.CustomerProductCode = ko.observable( itemData.CustomerProductCode || '' );
  this.Product = ko.observable( itemData.Product && itemData.Product.ProductKey ).extend({ required: true });
  this.Packaging = ko.observable( itemData.Packaging ).extend({ required: true });
  this.TreatmentKey = ko.observable( Number(itemData.TreatmentKey) ).extend({ required: true });

  this.Quantity = ko.observable( itemData.Quantity || 0 ).extend({ required: true, min: 1 });
  this.PriceBase = this.buildUSDComputed( itemData.PriceBase ).extend({ min: 0 });
  this.PriceFreight = this.buildUSDComputed( itemData.PriceFreight ).extend({ min: 0 });
  this.PriceTreatment = this.buildUSDComputed( itemData.PriceTreatment ).extend({ min: 0 });
  this.PriceWarehouse = this.buildUSDComputed( itemData.PriceWarehouse ).extend({ min: 0 });
  this.PriceRebate = this.buildUSDComputed( itemData.PriceRebate ).extend({ min: 0 });

  this.TotalWeight = ko.pureComputed(function() {
    var packaging = this.Packaging();
    var quantity = Number( this.Quantity() );

    if ( !packaging || quantity < 1 ) {
      return;
    }

    return packaging.Weight * quantity;
  }, this);

  this.TotalCostPerLb = ko.pureComputed(function() {
    var cost = Number( this.PriceBase() ) +
      Number( this.PriceFreight() ) +
      Number( this.PriceTreatment() ) +
      Number( this.PriceWarehouse() ) +
      Number( this.PriceRebate() );

    if ( isNaN( cost ) ) {
      return 0;
    }

    return cost;
  }, this);

  this.TotalCost = ko.pureComputed(function() {
    var cost = Number( this.TotalCostPerLb() ) * Number( this.TotalWeight() );

    if ( isNaN( cost ) ) {
      return 0;
    }

    return cost;
  }, this);
}

Item.prototype.buildUSDComputed = function(value) {
  var _usdValue = ko.observable("0.00").extend({ notify: 'always' });
  var computed = ko.computed({
    read: function() {
      return _usdValue();
    },
    write: function(newValue) {
      var sanitizedValue = String(newValue).replace(/[^0-9.-]/g, '');

      _usdValue((+sanitizedValue || 0).toFixed(2));
    }
  }).extend({ notify: 'always' });

  if (value) {
    computed(value);
  }

  return computed;
};

Item.prototype.toDto = function() {
  var packaging = this.Packaging() || {};

  return ko.toJS({
    SalesQuoteItemKey: this.SalesQuoteItemKey,

    Quantity: this.Quantity,
    CustomerProductCode: this.CustomerProductCode,
    PriceBase: this.PriceBase,
    PriceFreight: this.PriceFreight,
    PriceTreatment: this.PriceTreatment,
    PriceWarehouse: this.PriceWarehouse,
    PriceRebate: this.PriceRebate,

    ProductKey: this.Product,
    PackagingKey: packaging.ProductKey,
    TreatmentKey: this.TreatmentKey
  });
};

/** Quote Editor data */
function Editor( editorData, options ) {
  var self = this;

  // Static Data
  this.SalesQuoteKey = editorData.SalesQuoteKey;
  this.options = options;

  this.isNew = this.SalesQuoteKey == null;

  var DEFAULTS = {
    SOURCE_FACILITY: ko.utils.arrayFirst( options.facilities(), function( facility ) {
      return facility.FacilityKey === '2';
    }),
    DATE: Date.now()
  };

  if ( this.SalesQuoteKey == null ) {
    editorData.QuoteDate = DEFAULTS.DATE;
    editorData.SourceFacility = DEFAULTS.SOURCE_FACILITY;
  }

  // Dynamic data
  this.QuoteNumber = editorData.QuoteNumber;
  this.QuoteDate = ko.observableDate( editorData.QuoteDate ).extend({ required: true });
  this.DateReceived = ko.observableDate( editorData.DateReceived );
  this.CalledBy = ko.observable( editorData.CalledBy );
  this.TakenBy = ko.observable( editorData.TakenBy );
  this.PaymentTerms = ko.observable( editorData.PaymentTerms || null );
  this.SourceFacility = ko.observable( editorData.SourceFacility && editorData.SourceFacility.FacilityKey );
  this.Broker = ko.observable( editorData.Broker && editorData.Broker.CompanyKey || undefined );
  this.Customer = ko.observable( editorData.Customer ).extend({ rateLimit: { timeout: 500, method: 'notifyWhenChangesStop' } });
  this.customerKey = ko.pureComputed(function() {
    var _customer = self.Customer();

    if ( typeof _customer === 'string' ) {
      return _customer;
    } else if ( typeof _customer === 'object' ) {
      return _customer && _customer.CompanyKey;
    }

    return;
  });

  function setDefaultsForCustomer( customerKey ) {
    var getContracts = salesService.getContractsForCustomer( customerKey )
    .done(function( data ) {
      if ( data.length ) {
        var contract = data[ data.length - 1 ];

        self.Broker( contract.BrokerCompanyKey );
        self.PaymentTerms( contract.PaymentTerms );
      } else {
        self.Broker( undefined );
        self.PaymentTerms( null );
      }

      return data;
    })
    .fail(function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not load customer contracts', {
        description: errorThrown
      } );
    });

    return getContracts;
  }

  this.Customer.subscribe(function( customerKey ) {
    if ( typeof customerKey === 'string' ) {
      setDefaultsForCustomer( customerKey );
    }
  });

  // Shipping Info
  var _shipment = editorData.Shipment || {};
  var _instructions = _shipment.ShippingInstructions || {};
  this.SoldTo = ko.observable( _instructions.ShipFromOrSoldTo );
  this.ShipTo = ko.observable( _instructions.ShipTo );

  this.ScheduledShipDate = ko.observableDate( _instructions.ScheduledShipDateTime );
  this.RequiredDeliveryDate = ko.observableDate( _instructions.RequiredDeliveryDateTime );

  this.isPickingAddress = ko.observable( false );
  this.showAddressPicker = ko.command({
    execute: function() {
      self.isPickingAddress( true );
    }
  });

  this.closeAddressPicker = ko.command({
    execute: function() {
      self.isPickingAddress( false );
    }
  });

  this.shippingButtons = [{
    callback: function( data ) {
      self.SoldTo( data );
    },
    text: 'Sold To'
  },
  {
    callback: function( data ) {
      self.ShipTo( data );
    },
    text: 'Ship To'
  }];

  this.soldToExports = ko.observable();
  this.shipToExports = ko.observable();

  // Notes
  this.SpecialInstructions = ko.observable( _instructions.SpecialInstructions );
  this.InternalNotes = ko.observable( _instructions.InternalNotes );
  this.ExternalNotes = ko.observable( _instructions.ExternalNotes );

  var _transit = _shipment.Transit || {};
  this.FreightType = ko.observable( _transit.FreightType );
  this.ShipmentMethod = ko.observable( _transit.ShipmentMethod );

  // Items
  var items = (editorData.Items || []).map(function( item ) {
    item.Packaging = ko.utils.arrayFirst( self.options.packaging(), function( product ) {
      return product.ProductKey === item.Packaging.ProductKey;
    });

    return new Item( item );
  });
  this.Items = ko.observableArray( items || [] );

  this.addItem = ko.command({
    execute: function() {
      self.Items.push( new Item({}) );
    }
  });

  this.removeItem = ko.command({
    execute: function( item ) {
      var itemIndex = self.Items().indexOf( item );

      self.Items.splice( itemIndex, 1 );
    }
  });

  this.validation = ko.validatedObservable({
    QuoteDate: this.QuoteDate,
    Items: this.Items
  });

  this.buildDirtyFlag();
}

Editor.prototype.buildDirtyFlag = function() {
  this.dirtyFlag = (function(root, isInitiallyDirty) {
    var result = function() {},
        _initialState = ko.observable(ko.toJSON(root)),
        _isInitiallyDirty = ko.observable(isInitiallyDirty);

    result.isDirty = ko.computed(function() {
        return _isInitiallyDirty() || _initialState() !== ko.toJSON(root);
    });

    result.reset = function() {
        _initialState(ko.toJSON(root));
        _isInitiallyDirty(false);
    };

    return result;
  })({
    QuoteNumber: this.QuoteNumber,

    SourceFacility: this.SourceFacility,
    Customer: this.Customer,
    Broker: this.Broker,
    QuoteDate: this.QuoteDate,
    DateReceived: this.DateReceived,

    CalledBy: this.CalledBy,
    TakenBy: this.TakenBy,
    PaymentTerms: this.PaymentTerms,

    ShipmentInformation: {
      ShippingInstructions: {
        RequiredDeliveryDateTime: this.RequiredDeliveryDate,
        ScheduledShipDateTime: this.ScheduledShipDate,

        InternalNotes: this.InternalNotes,
        ExternalNotes: this.ExternalNotes,
        SpecialInstructions: this.SpecialInstructions,

        ShipFromOrSoldTo: this.SoldTo,
        ShipTo: this.ShipTo
      },
      Transit: {
        FreightBillType: this.FreightType,
        ShipmentMethod: this.ShipmentMethod
      }
    },

    Items: this.Items
  });
};

Editor.prototype.toDto = function() {
  var _customer = this.Customer();
  var customerKey = typeof _customer === 'string' ? _customer : _customer && _customer.CompanyKey;

  return ko.toJS({
    QuoteNumber: this.QuoteNumber,

    SourceFacilityKey: this.SourceFacility,
    CustomerKey: customerKey,
    BrokerKey: this.Broker,
    QuoteDate: this.QuoteDate,
    DateReceived: this.DateReceived,

    CalledBy: this.CalledBy,
    TakenBy: this.TakenBy,
    PaymentTerms: this.PaymentTerms,

    ShipmentInformation: {
      ShippingInstructions: {
        RequiredDeliveryDateTime: this.RequiredDeliveryDate,
        ScheduledShipDateTime: this.ScheduledShipDate,

        InternalNotes: this.InternalNotes,
        ExternalNotes: this.ExternalNotes,
        SpecialInstructions: this.SpecialInstructions,

        ShipFromOrSoldTo: this.soldToExports,
        ShipTo: this.shipToExports
      },
      Transit: {
        FreightBillType: this.FreightType,
        ShipmentMethod: this.ShipmentMethod
      }
    },

    Items: this.Items().map(function( item ) {
      return item.toDto();
    })
  });
};

/** Quote Editor VM */
function QuotesEditorVM( params ) {
  if ( !(this instanceof QuotesEditorVM) ) { return new QuotesEditorVM( params ); }

  var self = this;

  var options = params.options || {};

  this.APP_CONSTANTS = {
    INVOICE_NOTE_LENGTH: 600
  };

  // Editor construction
  function buildEditor( quote ) {
    var quoteData = ko.unwrap( quote );

    if ( quoteData.Customer != null ) {
      var customer = ko.utils.arrayFirst( options.customers(), function( customer ) {
        return customer.CompanyKey === quoteData.Customer.CompanyKey;
      });

      quoteData.Customer = customer;
    }

    var editor = new Editor( ko.unwrap( quoteData ), options );

    return editor;
  }

  this.editor = ko.observable( buildEditor( params.input ) );

  this.disposables = [];
  if (  ko.isObservable( params.input ) ) {
    this.disposables.push( params.input.subscribe(function( newData ) {
      if ( newData ) {
        self.editor( buildEditor( newData ) );
      }
    }) );
  }

  // Editor state
  var isPickingAddress = ko.pureComputed(function() {
    var _editor = self.editor();

    return _editor && _editor.isPickingAddress();
  });

  var isValid = ko.pureComputed(function() {
    var _editor = self.editor();

    return _editor && _editor.validation.isValid();
  });

  var isDirty = ko.pureComputed(function() {
    var _editor = self.editor();

    return _editor && _editor.dirtyFlag.isDirty();
  });

  function resetDirtyFlag() {
    var _editor = self.editor();

    _editor && _editor.dirtyFlag.reset();
  }

  // Editor methods
  function closeAddressPicker() {
    var _editor = self.editor();

    _editor && _editor.closeAddressPicker.execute();
  }

  function toDto() {
    var _editor = self.editor();

    return _editor && _editor.toDto();
  }

  // Exports
  if ( params && params.exports ) {
    params.exports({
      isPickingAddress: isPickingAddress,
      closeAddressPicker: closeAddressPicker,
      toDto: toDto,
      isValid: isValid,
      isDirty: isDirty,
      resetDirtyFlag: resetDirtyFlag
    });
  }

  return this;
}

ko.utils.extend(QuotesEditorVM.prototype, {
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
  viewModel: QuotesEditorVM,
  template: require('./quotes-editor.html')
};
