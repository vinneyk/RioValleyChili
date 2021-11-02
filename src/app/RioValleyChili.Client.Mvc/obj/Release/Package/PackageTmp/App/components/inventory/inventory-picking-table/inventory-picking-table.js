require('koProjections');
var rvc = require('rvc');
var LotAttribute = require('App/models/LotAttribute');

ko.filters.tryNumber = function (value, fallbackValue) {
  var val = ko.unwrap(value);
  return val == null || isNaN(val) ? fallbackValue : val.toLocaleString();
};
ko.filters.tryRoundedNumber = function (value, fallbackValue) {
  var val = Number(ko.unwrap(value));
  var opts = val >= 10 ?
    {
    maximumFractionDigits: 0
  } :
    {
    maximumFractionDigits: 2
  };
  return val == null || isNaN(val) ?
    fallbackValue :
    val.toLocaleString('en-US', opts);
};

require('App/ko.bindingHandlers.sortableTable');

/**
  * @param {object} params Object containing argument parameters
  * @param {object[]} [attributes] array of attribute ojbects to be displayed in the table header attribute objects can be retrieved from lotService.getAttributeNames
  * @param {number} [targetWeight] - The target total weight for a pick
  * @param {object} [targetProduct] the product for which to display product spec information product object is expected to contain AttributeRanges. When target product is undefined, the product spec is not displayed.
  * @param {bool} [isReadOnly=false] when true, disables the QuantiyPicked field
  * @param {boolean} hideTheoretical - Disables the calulcation and display of theoretical attrs
  * @param {object[]} inventoryItems array of items to display (table rows). Values should be in the shape of PickableInventoryItem (app/models)
  * @param {object} [inventoryType] InventoryType ID to enable filtering display of picked items by type
  * @param {boolean} [viewOnly=false] - Disables the picker controls and modifies view to be text-only
  * @param {observable} [exports] Receives the exported model.
  * @exports {InventoryPickingTable}
  */
function InventoryPickingTable(params) {
  if (!(this instanceof InventoryPickingTable)) { return new InventoryPickingTable(params); }

  var self = this,
      input = params.input,
      attributesToDisplay = ko.computed(function () {
        return ko.unwrap(input.attributes) || [];
      });
  self.inventoryType = ko.pureComputed(function () {
    var type = ko.unwrap(input.inventoryType);
    return type == null ?
      undefined :
      type.key == null ?
        type :
        type.key;
  });

  self.disposables = [];

  var useCustomerSpec = input.useCustomerSpec || ko.observable( false );
  var targetProductSpec = ko.computed(function () {
    var tProduct = ko.unwrap(input.targetProduct);
    if (tProduct == null) {
      return {};
    }

    var productSpec = {};

    ko.utils.arrayMap( tProduct.AttributeRanges, function (attrRange) {
      productSpec[attrRange.AttributeNameKey || attrRange.Key] = attrRange;
    });

    return productSpec;
  });

  self.isReadOnly = input.isReadOnly == null ? false : input.isReadOnly === true;
  self.isViewOnly = params.viewOnly || false;
  self.hideTheoretical = params.hideTheoretical;

  self.targetWeight = ko.pureComputed(function() {
    return ko.unwrap( input.targetWeight );
  });
  self.targetProductName = ko.pureComputed(function () {
    var tProduct = ko.unwrap(input.targetProduct);

    return tProduct ? tProduct.ProductCodeAndName : tProduct;
  });
  self.attributeHeader = ko.observableArray( [] );

  var attributeHeaderBuilder = ko.computed(function () {
    var attrs = attributesToDisplay() || [];
    var spec = JSON.parse( ko.toJSON( targetProductSpec ) );

    var _useCustomerSpec = ko.unwrap( useCustomerSpec );
    var _customerSpecs = _useCustomerSpec ? ko.unwrap( input.customerSpecs ) : null;

    self.attributeHeader( [] );

    if ( _customerSpecs && attrs.length ) {

      var mappedCustomerAttrs = attrs.map(function ( attr ) {
        var productSpec = spec[ attr.Key ];
        var customerSpec = _customerSpecs[ attr.Key ];

        attr.overridden = false;
        attr.productMinTargetValue = null;
        attr.productMaxTargetValue = null;
        attr.minTargetValue = null;
        attr.maxTargetValue = null;

        if ( customerSpec ) {
          attr.productMinTargetValue = productSpec &&
            (productSpec.hasOwnProperty('MinValue') ?
             productSpec.MinValue :
             productSpec.minTargetValue);

          attr.productMaxTargetValue = productSpec &&
            (productSpec.hasOwnProperty('MaxValue') ?
             productSpec.MaxValue :
             productSpec.maxTargetValue);

          attr.minTargetValue = customerSpec && customerSpec.MinValue;
          attr.maxTargetValue = customerSpec && customerSpec.MaxValue;

          attr.overridden = customerSpec && customerSpec.overridden;
        }

        return attr;
      });

      self.attributeHeader( mappedCustomerAttrs );

      return mappedCustomerAttrs;
    } else if ( spec && attrs.length ) {
      var mappedSpecAttrs = ko.utils.arrayMap( attrs, function ( attr ) {
        var currentTarget = spec[ attr.Key ];

        attr.overridden = false;
        attr.productMinTargetValue = null;
        attr.productMaxTargetValue = null;

        attr.minTargetValue = currentTarget ?
          (currentTarget.hasOwnProperty('MinValue') ?
           currentTarget.MinValue :
           currentTarget.minTargetValue) : undefined;

        attr.maxTargetValue = currentTarget ?
          (currentTarget.hasOwnProperty('MaxValue') ?
           currentTarget.MaxValue :
           currentTarget.maxTargetValue) : undefined;

        return attr;
      });

      self.attributeHeader( mappedSpecAttrs );

      return mappedSpecAttrs;
    }

    self.attributeHeader( attrs );
    return attrs;
  });
  self.hasProductSpec = ko.computed(function () {
    return self.attributeHeader().length > 0;
  });

  function filterByProductTypeDelegate(item) {
    return self.inventoryType() == null || ko.unwrap(item.Product.ProductType) === self.inventoryType();
  }

  self.allInventoryItems = input.inventoryItems;
  self.allPickedItems = input.inventoryItems.filter(function (i) { return ko.unwrap(i.isPicked) === true; });
  self.inventoryItems = input.inventoryItems
      .filter(filterByProductTypeDelegate)
      .map(function (item) {
        item.orderedAttributes = (function () {
          var itemAttributes = ko.unwrap(item.Attributes);
          var attrs = (attributesToDisplay());
          if (!attrs) return [];

          return ko.utils.arrayMap(attrs, function (attrName) {
            return ko.utils.arrayFirst(itemAttributes || [], function (attr) {
              return attr.Key === attrName.Key;
            }) || new LotAttribute({
              Key: attrName.Key,
              Name: attrName.Value,
              Value: null,
              formattedValue: '',
              Defect: {},
              isValueComputed: false
            });
          });
        })();
        if (ko.isObservable(item.HoldType) && ko.isObservable(item.HoldType.displayValue)) {
          item.holdDescription = item.HoldType.displayValue();
        } else {
          item.holdDescription = rvc.lists.lotHoldTypes.findByKey(ko.unwrap(item.HoldType));
          item.holdDescription = item.holdDescription && item.holdDescription.value || '';
        }
        return item;
      });
  self.pickedItems = self.inventoryItems.filter(function (i) { return ko.unwrap(i.isPicked) === true; });
  self.initiallyPickedItems = self.pickedItems.filter(function (item) {
    var orderKey = ko.unwrap( input.orderItemKey );

    if ( orderKey ) {
      return ko.unwrap( item.isInitiallyPicked ) === true && item.OrderItemKey === orderKey;
    }

    return ko.unwrap(item.isInitiallyPicked) === true;
  });
  self.pickableItems = self.inventoryItems.filter(function (item) { return !ko.unwrap(item.isInitiallyPicked); });

  self.totalPoundsPicked = ko.observable();
  self.totalQuantityPicked = ko.observable();
  self.isPickedWeightOverTarget = ko.pureComputed(function() {
    var picked = +self.totalPoundsPicked();
    var target = +ko.unwrap( self.targetWeight );

    return picked > target;
  });
  self.isShowingHeader = ko.pureComputed(function () {
    return ko.unwrap(input.targetProduct) != null;
  });

  self.theoreticalAttributeValues = ko.observableArray([]);
  self.attributesOutOfSpec = ko.computed(function() {
    var theoreticals = self.theoreticalAttributeValues() || [],
      pickedItems = self.pickedItems() || [];

    if (!pickedItems.length || !theoreticals.length) {
      return [];
    }

    var oos = [], index = 0;
    ko.utils.arrayForEach(self.attributeHeader(), function(attr) {
      var tValue = theoreticals[index];
      if (tValue > attr.maxTargetValue || tValue < attr.minTargetValue) {
        oos.push(attr);
      }
      index++;
    });

    return oos;
  });
  self.totalPoundsOnScreen = ko.pureComputed(function() {
    var totalPounds = 0;
    ko.utils.arrayForEach(self.pickableItems(), function (i) {
      totalPounds += ko.unwrap(i.TotalWeightAvailable);
    });
    return totalPounds;
  });

  // Subscriptions
  if (!self.hideTheoretical) {
    self.disposables.push([
      self.initiallyPickedItems.subscribe(function() {
        self.updateTheoreticalAttributeValues();
      })
    ]);

    ko.postbox.subscribe('pickedQuantityChanged', function(item) {
      self.updateTheoreticalAttributeValues();
    });

    self.updateTheoreticalAttributeValues();
  }

  if (ko.isObservable(params.exports)) {
    params.exports({
      attributesOutOfSpec: self.attributesOutOfSpec,
      totalPoundsOnScreen: self.totalPoundsOnScreen
    });
  }
  return self;
}

InventoryPickingTable.prototype.updateTheoreticalAttributeValues = function () {
  var self = this,
      totalQuantity = 0,
      totalWeight = 0,
      attributeNames = this.attributeHeader(),
      theoreticalAttributesContainer = initializeAttributeContainer(attributeNames);

  var allPickedItems = self.allPickedItems() || [];
  ko.utils.arrayForEach(allPickedItems, function (item) {
    totalWeight += ko.unwrap(item.WeightPicked) || 0;
    totalQuantity += ko.unwrap(item.QuantityPicked);

    ko.utils.arrayForEach(item.Attributes, function (attr) {
      if (theoreticalAttributesContainer[attr.Key] != null) {
        theoreticalAttributesContainer[attr.Key] += ((attr.Value || 0) * ko.unwrap(item.WeightPicked));
      }
    });
  });

  // Averages all attributes
  var theoreticalArray = ko.utils.arrayMap(attributeNames, function (currentAttribute) {
    return totalQuantity > 0 ?
        (theoreticalAttributesContainer[currentAttribute.Key] / totalWeight) :
        undefined;
  });

  self.totalQuantityPicked(totalQuantity);
  self.totalPoundsPicked(totalWeight);
  self.theoreticalAttributeValues(theoreticalArray);

  function initializeAttributeContainer(attributeNames) {
    var container = {};
    ko.utils.arrayForEach(attributeNames, function (attr) {
      container[attr.Key] = 0;
    });
    return container;
  }
};

InventoryPickingTable.prototype.dispose = function () {
  ko.utils.arrayForEach(this.disposables, this.disposeOne);
  ko.utils.objectForEach(this, this.disposeOne);
};

InventoryPickingTable.prototype.disposeOne = function (propOrValue, value) {
  var disposable = value || propOrValue;

  if (disposable && typeof disposable.dispose === "function") {
    disposable.dispose();
  }
};

module.exports = {
  viewModel: InventoryPickingTable,
  template: require('./inventory-picking-table.html')
};
