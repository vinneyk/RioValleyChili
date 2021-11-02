function OutputPickerVM(params) {
  if (!(this instanceof OutputPickerVM)) { return new OutputPickerVM(params); }

  var self = this;

  // Data
  var inputData = ko.unwrap(params.input);
  this.options = {
    packagingOptions: ko.unwrap(params.options && params.options.packagingOptions()) || [],
    warehouseOptions: ko.unwrap(params.options && params.options.warehouseOptions()) || []
  };

  this.outputItems = ko.observableArray([]);

  // Computed data
  this.totalWeight = ko.pureComputed(function() {
    var total = 0;

    ko.utils.arrayForEach(self.outputItems(), function(item) {
      var weight = item.Weight();

      total += weight;
    });

    return total;
  });

  // Behaviors
  function mapItem(item) {
    var mappedItem = {
      Quantity: ko.observable(),
      FacilityLocation: ko.observable(),
      Packaging: ko.observable(),
      Weight: ko.pureComputed(function() {
        var packaging = mappedItem.Packaging();
        var quantity = mappedItem.Quantity();

        return packaging && quantity > 0 ?
          packaging.Weight * quantity :
          null;
      })
    };

    if (item) {
      if (item.hasOwnProperty('Location')) {
        mappedItem.FacilityLocation(ko.utils.arrayFirst(self.options.warehouseOptions, function(loc) {
          return item.Location.LocationKey === loc.LocationKey;
        }));
      }
      if (item.hasOwnProperty('PackagingProduct')) {
        mappedItem.Packaging(ko.utils.arrayFirst(self.options.packagingOptions, function(packaging) {
          return item.PackagingProduct.ProductKey === packaging.ProductKey;
        }));
      }
      if (item.hasOwnProperty('QuantityProduced')) {
        mappedItem.Quantity(item.QuantityProduced);
      }
    }

    function isRequired() {
      var location = mappedItem.FacilityLocation();
      var packaging = mappedItem.Packaging();
      var quantity = mappedItem.Quantity();

      if (location || packaging || quantity) {
        return true;
      } else {
        return false;
      }
    }

    mappedItem.validation = ko.validatedObservable({
      Quantity: mappedItem.Quantity.extend({ min: 1,
      required: {
        onlyIf: isRequired
      }}),
      Location: mappedItem.FacilityLocation.extend({ required: {
        onlyIf: isRequired
      }}),
      Packaging: mappedItem.Packaging.extend({ required: {
        onlyIf: isRequired
      }})
    });

    (function() {
      var _complete = false;

      return ko.computed(function() {
        if (!_complete) {
          var items = ko.toJS(mappedItem);

          if (items.Quantity &&
              items.FacilityLocation &&
              items.Packaging) {

            _complete = true;

            var outputItems = self.outputItems.peek();
            var lastItem = outputItems[outputItems.length - 1];
            if (lastItem === mappedItem) {
              addItem({
                Location: lastItem.FacilityLocation()
              });
            }
          }

          return false;
        } else {
          return true;
        }
      });
    })();
    
    return mappedItem;
  }

  function addItem(item) {
    self.outputItems.push(mapItem(item || null));
    self.outputItems.notifySubscribers();
  }

  function addExistingOutputs(items) {
    var currentItems = self.outputItems();
    var newItems = items || [];
    var mappedItems = [];

    ko.utils.arrayForEach(newItems, function(item) {
      mappedItems.push(mapItem(item));
    });

    self.outputItems(currentItems.concat(mappedItems));

    return self.outputItems;
  }

  function buildDto() {
    var isValid = true;
    var items = [];
    
    var pushValidItems = ko.utils.arrayForEach(self.outputItems(), function(item) {
      var quantity = item.Quantity();

      if (quantity > 0) {
        if (item.validation.isValid()) {
          items.push({
            PackagingProductKey: item.Packaging().ProductKey,
            LocationKey: item.FacilityLocation().LocationKey,
            Quantity: quantity,
          });
        } else {
          isValid = false;
        }
      }
    });

    return isValid ? items : null;
  }

  this.addNewItem = function() {
    var outputItems = self.outputItems();
    var lastItem = outputItems[outputItems.length - 1];
    var lastLocation = lastItem && lastItem.FacilityLocation();

    if (lastLocation) {
      addItem({
        Location: lastLocation,
      });
    } else {
      addItem();
    }
  };

  this.removeItem = ko.command({
    execute: function(item, $element) {
      var items = self.outputItems();

      self.outputItems.splice(items.indexOf(item), 1);
    }
  });


  // Code execution
  if (inputData) {
    addExistingOutputs(inputData);
  } else {
    addItem();
  }

  // Exports
  if (params && params.exports) {
    params.exports({
      toDto: buildDto
    });
  }
  return this;
}

module.exports = {
  viewModel: OutputPickerVM,
  template: require('./output-picker.html')
};
