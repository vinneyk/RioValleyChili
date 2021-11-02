/**
  * @param {Object} input - Initial settings for filters
  * @param {string} input.ingredientType
  * @param {string} input.inventoryType
  * @param {string} input.lotType
  * @param {string} input.lotKey
  * @param {string} input.packagingProductKey
  * @param {string} input.productKey
  * @param {string} input.warehouseLocationKey
  * @param {Object} [options] - Options to populate select boxes with
  * @param {Object} options.ingredients - Ingredients
  * @param {Object} options.products - Products, sorted by lot type
  * @param {Object[]} options.locations - Rincon warehouse locations
  * @param {Object[]} options.packaging - Packaging products
  * @param {boolean} [options.filterProductsWithInventory=true] - If not provided options, lot filters will return products with existing inventory
  * @param {Object} filters - Observable; container for filters
  * @param {string} mode - Sets filtering mode; 'inventory' or 'qualityControl'
  * @param {boolean} [disable=false] - Observable; Disables filter input fields
  * @param {boolean} [lotKeyOnly=false] - Observable; Hide all filters except lotkey
  * @param {boolean} [startingLotKey] - Use startingLotKey instead of lotKey
  */


require('App/koExtensions');
var disposableHelper = require('App/helpers/disposableHelper');

var rvc = require('rvc'),
    lotService = require('App/services/lotService'),
    productsService = require('App/services/productsService'),
    warehouseService = require('App/services/warehouseService'),
    warehouseLocationsService = require('App/services/warehouseLocationsService');

function FiltersViewModel(params) {
  params = params || {};
  params.options = params.options || {};
  var input = ko.unwrap(params.input) || {};
  var filterProductsWithInventory = params.filterProductsWithInventory || false;

  if (!(this instanceof FiltersViewModel)) {
    return new FiltersViewModel(params);
  }

  var self = this;
  var productOptionsByLotType = params.options.products || {};
  var packagingProducts = ko.observableArray(params.options.packaging || []);
  var ingredientsByProductType = ko.observable(params.options.ingredients || []);
  var facilityOptions = ko.observableArray( [] );
  var warehouseLocationOptions = ko.observableArray(params.options.locations || []);
  this.isLotKeyOnly = params.lotKeyOnly ?
    params.lotKeyOnly :
    false;

  this.uiTemplate = ko.pureComputed(function() {
    return ko.unwrap( self.isLotKeyOnly ) ? 'filters-lotkey-only' : 'filters-base';
  });

  this.disposables = [];
  this.includeInventoryFilters = params.mode === 'inventory';
  this.includeQualityControlFilters = params.mode === 'qualityControl';
  this.includeInventoryAdjustmentFilters = params.mode === 'inventoryAdjustment';
  this.enableFacilityFilter = params.enableFacilityFilter || false;
  this.rinconKey = 2;

  // Data Structure
  this.inventoryType = ko.observable(input.inventoryType).extend({ inventoryType: true });

  this.isInventoryChile = ko.pureComputed(function() {
    var _invType = self.inventoryType();

    return _invType && _invType.key === 1;
  });
  this.chileTypeOptions = ko.utils.arrayMap( Object.keys( rvc.lists.chileClassifications ), function( opt ) {
    return rvc.lists.chileClassifications[ opt ];
  });
  this.chileType = ko.observable();

  this.lotType = ko.observable(input.lotType).extend({ lotType: true });
  this.ingredientType = ko.observable(input.ingredientType);
  this.productKey = ko.observable(input.productKey);
  this.packagingProductKey = ko.observable(input.packagingProductKey);
  this.treatmentKey = ko.observable().extend({ treatmentType: true });
  this.lotKey = ko.observable(input.lotKey).extend({ lotKey: setInventoryTypeForLotKey });
  this.facilityKey = ko.observable( input.facilityKey );
  this.warehouseLocationKey = ko.observable(input.warehouseLocationKey);
  this.lotTypeOptions = ko.pureComputed(function () {
    var inventoryType = self.inventoryType();
    return inventoryType && rvc.lists.lotTypesByInventoryType[inventoryType.value] || [];
  });

  this.streetFilter = ko.observable();
  this.productionStatus = ko.observable().extend({ productionStatusType: true });
  this.productionStart = ko.observableDate();
  this.productionEnd = ko.observableDate();
  this.qualityStatus = ko.observable().extend({ lotQualityStatusType: true });

  /** Inventory adjustment filters */
  this.beginningDate = ko.observableDate();
  this.endingDate = ko.observableDate();

  this.enableLotTypeFilter = ko.pureComputed(function () {
    return self.inventoryType() != undefined;
  });
  this.enableProductFilter = ko.pureComputed(function () {
    return self.lotType() != undefined;
  });

  this.isRincon = ko.computed(function() {
    if ( self.facilityKey() === self.rinconKey ) {
      return true;
    } else {
      self.warehouseLocationKey( null );
      return false;
    }
  });
  this.isAllDisabled = params.disable || false;
  this.isDisabled = ko.pureComputed(function() {
    return ko.unwrap( self.isAllDisabled ) || self.lotKey() != null;
  });

  this.disposables.push(this.productionStart.subscribe(function(val) {
    if (val != null && self.productionEnd() == null) {
      self.productionEnd(Date.now().format('m/d/yyyy'));
    }
  }));

  // Filter options
  this.ingredientTypeOptions = ko.pureComputed(function () {
    var productType = self.inventoryType(),
    ingredientTypeDictionary = ingredientsByProductType();

    return (!productType || !ingredientTypeDictionary) ?
        [] :
        ingredientTypeDictionary[productType.value || productType] || [];
  });

  this.productKeyOptions = ko.pureComputed(function () {
    var lotType = self.lotType();
    return lotType && productOptionsByLotType[lotType.value]() || [];
  });

  this.packagingProductKeyOptions = ko.pureComputed(function () {
    return packagingProducts() || [];
  });

  this.facilityOptions = ko.pureComputed(function() {
    return facilityOptions() || [];
  });

  this.warehouseLocationOptions = ko.pureComputed(function () {
    var locations = warehouseLocationOptions() || [];
    var streetFilter = self.streetFilter();
    if ( streetFilter ) {
      return ko.utils.arrayFilter(locations, function(l) { return l.GroupName === streetFilter; });
    } else return locations;
  });
  this.streetFilterOptions = ko.pureComputed(function () {
    var locations = warehouseLocationOptions() || [];
    return ko.utils.arrayGetDistinctValues(
      ko.utils.arrayMap(locations, function(l) { return l.GroupName; })
    ).sort();
  });

  this.hasIngredients = ko.pureComputed(function () {
    return self.ingredientTypeOptions().length > 0;
  });

  // Behaviors
  function loadProductOptions() {
    var productDfds = [];
    var options = {
      filterProductsWithInventory: filterProductsWithInventory
    };

    rvc.helpers.forEachLotType(loadAndPush);

    var checkResults = $.when.apply($, productDfds);

    function loadAndPush(type) {
      if (!ko.isObservable(productOptionsByLotType[type.value])) {
        productOptionsByLotType[type.value] = ko.observableArray([]);
      }

      var dfd = lotService.getProductsByLotType(type.key, options)
          .done(function (data) {
            productOptionsByLotType[type.value](data);
          });

      productDfds.push(dfd);
      return dfd;
    }

    return checkResults;
  }

  this.getFacilityLocations = ko.asyncCommand({
    execute: function( facilityKey, complete ) {
      var getLocations = warehouseLocationsService.getWarehouseLocations( facilityKey ).then(
        function( data, textStatus, jqXHR ) {
        return data;
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Failed to get locations for warehouse', {
          description: errorThrown
        });
      }).always( complete );

      return getLocations;
    }
  });

  function setInventoryTypeForLotKey() {
    var inventoryTypeKey = self.lotKey.InventoryTypeKey();

    var inventoryType = ko.utils.arrayFirst(self.inventoryType.options, function ( opt ) {
      return opt.value.key === inventoryTypeKey;
    });

    self.inventoryType( inventoryType.value );
  }

  if ( params.options.hasOwnProperty('ingredients') &&
             params.options.hasOwnProperty('products') &&
             params.options.hasOwnProperty('locations') &&
             params.options.hasOwnProperty('packaging') ) {
    init(params.input);
  } else {
    loadOptions().then(
    function( ingredients, products, packaging, locations, facilities ) {
      ingredientsByProductType(ingredients[0] || []);
      packagingProducts(packaging[0] || []);
      warehouseLocationOptions(locations[0] || []);

      if ( facilities ) {
        facilityOptions(facilities && facilities[0] || []);

        var rinconKey = ko.utils.arrayFirst( facilities[0], function( facility ) {
          return facility.FacilityName === 'Rincon';
        });

        if ( rinconKey ) {
          self.facilityKey.subscribe(function( newFacilityKey ) {
            if ( newFacilityKey && self.getFacilityLocations.canExecute() ) {
              self.getFacilityLocations.execute( newFacilityKey ).then(
                function( data, textStatus, jqXHR ) {
                warehouseLocationOptions( data );
              });
            }
          });

          self.rinconKey = rinconKey.FacilityKey;
          self.facilityKey( rinconKey.FacilityKey );
        }
      }

      init(params.input);
    });
  }

  function loadOptions() {
    var getIngredients = lotService.getIngredientsByProductType().then( null );
    var getProducts = loadProductOptions().then( null );
    var getPackaging = productsService.getPackagingProducts().then( null );
    var getLocations = self.enableFacilityFilter ?
      $.Deferred().resolve( [] ) :
      warehouseLocationsService.getRinconWarehouseLocations().then( null );

    var checkComplete;

    if ( self.enableFacilityFilter ) {
      var getFacilities = warehouseService.getWarehouses().then( null );

      checkComplete = $.when( getIngredients, getProducts, getPackaging, getLocations, getFacilities );
    } else {
      checkComplete = $.when( getIngredients, getProducts, getPackaging, getLocations );
    }

    return checkComplete;
  }

  function init(input) {
    self.lotType( undefined );
    self.ingredientType( undefined );
    self.productKey( undefined );
    self.warehouseLocationKey( undefined );

    if ( input == undefined ) {
      return;
    }

    var data = ko.toJS( input ) || {};
    var lotReady = $.Deferred();
    var productKeySub = $.Deferred();
    var ingredientTypeSub = $.Deferred();

    self.inventoryType( ko.utils.arrayFirst( self.inventoryType.options, function ( item ) {
      if ( comparator( item.value.key, data.inventoryType ) ) {
        if ( self.lotTypeOptions().length > 0 ) {
          lotReady.resolve();
        } else {
          var lotTypeSub = self.lotTypeOptions.subscribe(function () {
            if ( self.lotTypeOptions().length > 0 ) {
              lotReady.resolve();
              lotTypeSub.dispose();

              ko.utils.arrayRemoveItem( self.disposables, lotTypeSub );
            }
          });
          self.disposables.push( lotTypeSub );
        }
        return true;
      }
    } ) );
    self.packagingProductKey( ko.utils.arrayFirst( self.packagingProductKeyOptions(), function (item) {
      return comparator( item.ProductKey, data.packagingProductKey );
    } ) );
    self.treatmentKey( ko.utils.arrayFirst( self.treatmentKey.options, function (item) {
      return comparator( item.key, data.treatmentKey );
    } ) );

    function comparator( target, key ) {
      var targetKey = keyParse( target ),
          currentKey = keyParse( key );

      if ( currentKey === targetKey ) {
        return true;
      }

      return false;

      function keyParse( input ) {
        return input === Object( input ) ?
            input.key :
            input;
      }
    }

    self.lotKey( data.lotKey );

    var checkLotReady = lotReady.then(
      function() {
        ko.utils.arrayFirst( self.lotTypeOptions(), function ( item ) {
          if ( comparator( item.key, data.lotType ) ) {
            var productSub = self.productKeyOptions.subscribe(function ( data ) {
              productKeySub.resolve( true );
              productSub.dispose();
              ko.utils.arrayRemoveItem( self.disposables, productSub );
            });
            self.disposables.push( productSub );

            if ( self.hasIngredients() ) {
              var ingredientSub = self.productKeyOptions.subscribe(function (data) {
                ingredientTypeSub.resolve( true );
                ingredientSub.dispose();
                ko.utils.arrayRemoveItem( self.disposables, ingredientSub );
              });
              self.disposables.push( ingredientSub );
            } else {
              ingredientTypeSub.resolve( false );
            }

            self.lotType(item);

            return true;
          }
        } );
      }
    );

    var checkProductAndIngredientReady = $.when( productKeySub, ingredientTypeSub ).then(
    function( productsReady, ingredientsReady ) {
      ko.utils.arrayFirst( self.productKeyOptions(), function (item) {
        if ( comparator( item.ProductKey, data.productKey ) ) {
          self.productKey( item );
        }
      } );

      if ( ingredientSub ) {
        if ( typeof data.ingredientType === 'string' ) {
          ko.utils.arrayFirst( self.ingredientTypeOptions(), function (item) {
            if ( item.Description === data.ingredientType ) {
              self.ingredientType( item );

              return true;
            }
          } );
        } else {
          ko.utils.arrayFirst( self.ingredientTypeOptions(), function (item) {
            if ( comparator( item.ProductKey, data.ingredientType ) ) {
              self.ingredientType( item );

              return true;
            }
          } );
        }
      }
    });
  }

  function getOptionDefaultValue(val) {
    if (val == undefined) { return null; }
    if (typeof val === "string") { return val; }
    if ($.isNumeric(val)) { return val.toString(); }

    return val.key && (getOptionDefaultValue(val.key) || null);
  }

  function getInventoryTypeOptionByKey(val, target) {
    var key = val == undefined ?
      null :
      val.key || val;

    if (key == undefined) {
      return null;
    }

    var selected = ko.utils.arrayFirst(ko.unwrap(target.options), function (opt) {
      return opt.value && opt.value.key === key;
    });

    return selected == undefined ?
      null :
      selected.value;
  }

  var filters = {
    inventoryType: ko.pureComputed({
      read: function () {
        var invType = this();
        return invType && invType.key != undefined ? invType.key : (invType || null);
      },
      write: function (val) {
        this(val == undefined ?
            null :
            getInventoryTypeOptionByKey(val, this));
      },
      owner: self.inventoryType
    }),

    productSubType: ko.pureComputed({
      read: function () {
        var _invType = self.inventoryType();
        
        switch(_invType.key) {
          case rvc.lists.inventoryTypes.Chile.key:
            return self.chileType();
          default:
            return null;
        }
      },
      write: function (val) {
        var _invType = self.inventoryType();

        self.chileType(null);

        switch (_invType) {
          case rvc.lists.inventoryTypes.Chile.key:
            self.chileType(val);
            break;
        }
      },
      owner: self
    }),

    lotType: ko.pureComputed({
      read: function () {
        var lotType = this();
        return (lotType && lotType.key) ? lotType.key : (lotType || null);
      },
      write: function (val) {
        this(val != undefined ?
            rvc.lists.lotTypes.findByKey(val.key == undefined ? val : val.key) :
            null);
      },
      owner: self.lotType
    }),

    productKey: ko.pureComputed({
      read: function () {
        var prod = this();
        return (prod && prod.ProductKey) ? prod.ProductKey : (prod || null);
      },
      write: function (val) {
        this(val);
      },
      owner: self.productKey
    })
  };

  if ( params.startingLotKey ) {
    filters.startingLotKey = self.lotKey;
  } else {
    filters.lotKey = self.lotKey;
  }

  if ( this.includeInventoryFilters ) {
    filters.ingredientType = ko.pureComputed({
      read: function () {
        var ingredType = this();
        return (ingredType && ingredType.key) ? ingredType.key : (ingredType || null);
      },
      write: function (val) {
        var opts = self.ingredientTypeOptions();
        var selectedValue = null;
        if (Number(val)) {
          selectedValue = ko.utils.arrayFirst(opts, function (o) {
            return o.Key === val;
          });
        } else {
          selectedValue = ko.utils.arrayFirst(opts, function (o) {
            return o.Description === val;
          });
        }
        this(selectedValue && selectedValue.Key);
      },
      owner: self.ingredientType
    });

    filters.packagingProductKey = ko.pureComputed({
      read: function () {
        var packaging = this();
        return (packaging && packaging.ProductKey) ? packaging.ProductKey : (packaging || null);
      },
      write: function (val) {
        this(getOptionDefaultValue(val));
      },
      owner: self.packagingProductKey
    });

    filters.treatmentKey = ko.pureComputed({
      read: function () {
        var treatment = this();
        return (treatment && treatment.key != undefined) ? treatment.key : (treatment || null);
      },
      write: function (val) {
        this(getOptionDefaultValue(val));
      },
      owner: self.treatmentKey
    });

    filters.warehouseLocationKey = self.warehouseLocationKey;
    filters.locationGroupName = self.streetFilter;
  }

  if ( this.includeQualityControlFilters ) {
    filters.productionStatus = this.productionStatus;
    filters.productionStart = this.productionStart;
    filters.productionEnd = this.productionEnd;
    filters.qualityStatus = this.qualityStatus;
  }

  if ( this.includeInventoryAdjustmentFilters ) {
    filters.beginningDateFilter = this.beginningDate;
    filters.endingDateFilter = this.endingDate;
  }

  if ( this.enableFacilityFilter ) {
    filters.warehouseKey = self.facilityKey;
  }

  if ( params.exports ) {
    params.exports( filters );
  }

  if ( params.filters ) {
    params.filters( filters );
  }

  return self;
}

FiltersViewModel.prototype.dispose = function () {
  ko.utils.arrayForEach(this.disposables, this.disposeOne);
  ko.utils.objectForEach(this, this.disposeOne);
};

FiltersViewModel.prototype.disposeOne = function (propOrValue, value) {
  var disposable = value || propOrValue;

  if (disposable && typeof disposable.dispose === "function") {
    disposable.dispose();
  }
};

module.exports = {
  viewModel: FiltersViewModel,
  template: require('./lot-filters.html')
};
