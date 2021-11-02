/**
  * @param {Object[]} productsSource - Observable, Product options
  * @param {string} selectedValue - Observable, Selected option
  * @param {string} optionsDisplay - Property to use for text display
  * @param {string} optionsValue - Property to use for object value
  * @param {string} loading - Observable, Toggles input based on status
  * @param {string} lotType - Observable, optional, enables use of the component without the requirement of providing the source products list and configurations.
  * @param {bool} disabled - Observable, optional, enables control of the disabled state
  */

require('node_modules/knockout-jqautocomplete/build/knockout-jqAutocomplete');
require('scripts/ko.extenders.date');
require('App/bindings/ko.bindingHandlers.datePicker');
var productsService = require('services/productsService');
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
  template: require('./product-selector.html')
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
