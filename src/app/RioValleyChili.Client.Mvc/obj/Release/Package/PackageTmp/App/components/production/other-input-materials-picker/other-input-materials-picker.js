var inventoryService = require('App/services/inventoryService');

function otherInputMatPickerVM(params) {
  if (!(this instanceof otherInputMatPickerVM)) { return new otherInputMatPickerVM(params); }

  var self = this;
  var _defaults = params.defaults || {};

  // Data
  var defaultEthoxLot = ko.pureComputed(function() {
    return ko.toJS(_defaults).ethoxLot;
  });
  var defaultSipernatLot = ko.pureComputed(function() {
    return ko.toJS(_defaults).sipernatLot;
  });

  this.inventoryItems = ko.observableArray([]);

  function getPickedItems() {
    return ko.utils.arrayFilter(self.inventoryItems(), isPicked);

    function isPicked(item) {
      return ko.unwrap(item.QuantityPicked) > 0;
    }
  }

  function buildDto() {
    var isValid = true;
    var pickedItems = ko.utils.arrayMap(getPickedItems(), function (item) {
      if (item.validation.isValid()) {
        return {
          Tote: item.ToteKey,
          InventoryKey: item.InventoryKey,
          Quantity: item.QuantityPicked
        };
      } else {
        isValid = false;
      }
    });

    return isValid ? pickedItems : null;
  }

  this.lotToAdd = ko.observable();
  this.inventoryItems = ko.observableArray([]);
  this.range = function(max) {
    var r = [];

    for (var i = 0; i < max; i++) { 
      r.push(i);
    }

    return r;
  };

  // computed properties
  this.TotalWeightPicked = ko.pureComputed(function() {
    var sum = 0;
    ko.utils.arrayForEach(this.inventoryItems(), function(item) {
      sum += item.WeightPicked();
    });
    return sum;
  }, this);

  // commands
  this.getLotInventoryCommand = ko.asyncCommand({
    execute: function(complete) {
      getLotData(self.lotToAdd.formattedLot()).done(function(data, textStatus, jqXHR) {
        //TODO: set focus: if treatment is selected then quantity else packaging
        addLotInventory(data);
        self.lotToAdd('');
      })
      .fail(function(jqXHR, textStatus, errorThrown) {
          showUserMessage("Unable to load inventory for lot", { description: errorThrown });
      })
      .always(complete);
    },
    canExecute: function(isExecuting) {
      return !isExecuting;
    }
  });

  this.removeItemCommand = ko.command({
    execute: function (item) {
      var index = ko.utils.arrayIndexOf(self.inventoryItems(), item);

      if (item) {
        self.inventoryItems.splice(index, 1);
      }
    }
  });

  function getLotData(lotKey) {
    return inventoryService.getInventoryByLot(lotKey);
  }

  this.addEthoxCommand = ko.command({
    execute: function () {
      getLotData(defaultEthoxLot()).then(
      function(data, textStatus, jqXHR) {
        addLotInventory(data);
      },
      function(jqXHR, textStatus, errorThrown) {
        showUserMessage("Could not add ethox", { description: errorThrown });
      });
    },
    canExecute: function () {
      return defaultEthoxLot();
    }
  });

  this.addSipernatCommand = ko.command({
    execute: function () {
      getLotData(defaultSipernatLot()).then(
      function(data, textStatus, jqXHR) {
        addLotInventory(data);
      },
      function(jqXHR, textStatus, errorThrown) {
        showUserMessage("Could not add sipernat", { description: errorThrown });
      });
    },
    canExecute: function() {
      return defaultSipernatLot();
    }
  });

  this.containsPickedItem = function(inventoryKey) {
    return ko.utils.arrayFirst(self.inventoryItems(), function(selected) {
      return selected.InventoryKey() === inventoryKey;
    }) !== null;
  };

  // init
  this.lotToAdd.extend({ lotKey: this.getLotInventoryCommand.execute });

  function mapLot(data) {
    var inventory = data.InventoryItems;

    var packagingOptions = {};
    var locationOptions = {};
    var treatmentOptions = {};

    ko.utils.arrayForEach(inventory, function(item) {
      if (!packagingOptions[item.PackagingProduct.ProductKey]) {
        packagingOptions[item.PackagingProduct.ProductKey] = {
          Key: item.PackagingProduct.ProductKey,
          DisplayText: item.PackagingProduct.ProductName,
          Capacity: item.PackagingProduct.Weight,
        };
      }

      if (!locationOptions[item.Location.LocationKey]) {
        locationOptions[item.Location.LocationKey] = {
          Key: item.Location.LocationKey,
          DisplayText: item.Location.Description,
        };
      }

      if (!item.InventoryTreatment) {
        item.InventoryTreatment = {
          TreatmentKey: '0',
          TreatmentName: 'Not Treated',
          TreatmentNameShort: 'NA'
        };
      }

      if (item.InventoryTreatment && !treatmentOptions[item.InventoryTreatment]) {
        treatmentOptions[item.InventoryTreatment.TreatmentKey] = {
          Key: item.InventoryTreatment.TreatmentKey,
          DisplayText: item.InventoryTreatment.TreatmentNameShort,
        };
      }
    });

    var lotKey = inventory[0].LotKey;
    var packagingOptionsArray = toArray(packagingOptions);
    var locationOptionsArray = toArray(locationOptions);
    var treatmentOptionsArray = toArray(treatmentOptions);

    var inventorySelector = {
      LotKey: lotKey,
      ProductType: inventory[0].Product.ProductType,
      ProductName: inventory[0].Product.ProductName,
      ProductKey: inventory[0].Product.ProductKey,
      QuantityPicked: ko.numericObservable(inventory[0].QuantityPicked || 0),
      selectedPackaging: ko.observable(),
      selectedFacilityLocation: ko.observable(),
      selectedTreatment: ko.observable(),
      InventoryKey: ko.observable(),
    };

    (function() {
      var key = inventory[0].PackagingProduct.ProductKey || null;
      var match = ko.utils.arrayFirst(packagingOptionsArray, function(item) {
        return key === item.Key;
      });

      if (match) {
        inventorySelector.selectedPackaging(match);
      }
    })();

    inventorySelector.WeightPicked = ko.pureComputed(function() {
      var quantityPicked = this.QuantityPicked();
      var selectedPackaging = this.selectedPackaging();
      return quantityPicked && selectedPackaging ?
        quantityPicked * selectedPackaging.Capacity :
        0;
    }, inventorySelector);

    inventorySelector.packagingOptions = ko.pureComputed(function() {
      return packagingOptionsArray;
    }, inventorySelector);

    inventorySelector.locationOptions = ko.pureComputed(function() {
      return this.selectedPackaging() ?
        locationOptionsArray :
        [];
    }, inventorySelector);

    inventorySelector.treatmentOptions = ko.pureComputed(function() {
      return this.selectedFacilityLocation() ?
        treatmentOptionsArray :
        [];
    }, inventorySelector);

    inventorySelector.selectedInventoryItem = ko.pureComputed(function() {
      return this.selectedTreatment() ?
        getInventoryKey(this) :
        null;

        function getInventoryKey(selections) {
          return ko.utils.arrayFirst(inventory, function(item) {
            return item.PackagingProduct.ProductKey === selections.selectedPackaging().Key &&
              item.Location.LocationKey === selections.selectedFacilityLocation().Key &&
              item.InventoryTreatment.TreatmentKey === selections.selectedTreatment().Key;
          });
        }

    }, inventorySelector);

    inventorySelector.InventoryKey = ko.pureComputed(function() {
      return this.selectedInventoryItem() ?
        this.selectedInventoryItem().InventoryKey :
        null;
    }, inventorySelector).extend({
      isUnique: {
        array: self.inventoryItems,
        predicate: function (opt, val) {
          return opt.InventoryKey() === val;
        }
      }
    });
    var inventoryKeyValidator = ko.validatedObservable({
      isDup: inventorySelector.InventoryKey
    });
    inventorySelector.isDuplicate = ko.pureComputed(function() {
      return !inventoryKeyValidator.isValid();
    });
    inventorySelector.quantityAvailable = ko.pureComputed(function() {
      var inventoryItem = this.selectedInventoryItem();

      return inventoryItem ?
        inventoryItem.Quantity - (this.QuantityPicked() || 0) :
        0;
    }, inventorySelector);

    // subscribers
    inventorySelector.locationOptions.subscribe(function(val) {
      if (val && val.length && val.length > 1) {
        var defaultSelection = ko.utils.arrayFirst(val, function(item) {
          return item.DisplayText === 'Ing00';
        });
        if (defaultSelection) {
          inventorySelector.selectedFacilityLocation(defaultSelection);
        }
      }
    });

    var maxQuantity = ko.pureComputed(function() {
      return inventorySelector.selectedInventoryItem() ?
        inventorySelector.selectedInventoryItem().Quantity :
        0;
    }, this);

    inventorySelector.QuantityPicked.extend({ min: 0, max: maxQuantity });
    inventorySelector.validation = ko.validatedObservable({
      key: inventorySelector.InventoryKey,
      quantity: inventorySelector.QuantityPicked
    });

    function toArray(obj) {
      var a = [];
      for (var prop in obj) {
        a.push(obj[prop]);
      }
      return a;
    }

    return inventorySelector;
  }

  function addLotInventory(data) {
    if (!data.InventoryItems.length) {
      showUserMessage("There is no available inventory for the Lot \"" + self.lotToAdd.formattedLot() + "\".");
      return;
    }

    var mappedLot = mapLot(data);

    self.inventoryItems.splice(0, 0, mappedLot);
    self.inventoryItems.notifySubscribers(mappedLot, 'itemAdded');
  }

  function getUpdatedInventory(items) {
    var inventoryItems = items || [];

    ko.utils.arrayForEach(inventoryItems, function(item) {
      self.getLot(ko.unwrap(item.LotKey), { update: true }).done(function(data, textStatus, jqXHR) {
        if (!!data.InventoryItems.length) {
          updateLotInventory(item.LotKey, data);
        }
      });
    });
  }

  function updateLotInventory(lotKey, data) {
    var newData = {
      InventoryItems: [],
    };
    
    function matchOption(options, matchKey, propName) {
      return ko.utils.arrayFirst(options, function(opt) {
        return opt.hasOwnProperty('Key') ? opt[propName] === matchKey : false;
      });
    }

    /** Update API data to include exisiting picks */
    var lot = ko.utils.arrayFirst(self.inventoryItems(), function(item) {
      return item.LotKey === lotKey;
    });
    var lotData = ko.toJS(lot);

    ko.utils.arrayForEach(data.InventoryItems, function(inv) {
      var matchesCurrentLot = inv.InventoryKey === lotData.InventoryKey;

      if (matchesCurrentLot) {
        inv.Quantity += lotData.QuantityPicked;
      } 

      newData.InventoryItems.push(inv);
    });

    /** Map updated data for use in picker and restore pick data */
    var mappedData = mapLot(newData);
    mappedData.selectedPackaging(matchOption(mappedData.packagingOptions(), 'Key', lotData.selectedPackaging.Key));
    mappedData.selectedFacilityLocation(matchOption(mappedData.locationOptions(), 'Key', lotData.selectedFacilityLocation.Key));
    mappedData.selectedTreatment(matchOption(mappedData.treatmentOptions(), 'Key', lotData.selectedTreatment.Key));
    mappedData.QuantityPicked(lotData.QuantityPicked);

    /** Replace existing entry with updated entry */
    var i = self.inventoryItems().indexOf(lot);
    self.inventoryItems.splice(i, 1, mappedData);
  }

  function addExistingLotInventory(inv) {
    ko.utils.arrayForEach(ko.toJS(inv), function(item) {
      item.Quantity = item.QuantityPicked;
      addLotInventory({ InventoryItems: [item] });
    });

    getUpdatedInventory(self.inventoryItems());
  }

  if (ko.unwrap(params.input)) {
    addExistingLotInventory(params.input);
  }

  // Exports
  if (params && params.exports) {
    params.exports({
      inventoryItems: self.inventoryItems,
      toDto: buildDto,
      getPickedItems: getPickedItems,
    });
  }
  return this;
}

otherInputMatPickerVM.prototype.getLot = function(lotKey, opts) {
  var key = ko.observable(lotKey).extend({ lotKey: true });
  var lot = inventoryService.getInventoryByLot(lotKey).fail(function(jqXHR, textStatus, errorThrown) {
    if (jqXHR.status === 404 && opts && opts.update === true) {
      return lot;
    } else if (jqXHR.status === 404) {
      showUserMessage("Lot not found", { description: "There is no available inventory with Lot \"<strong>" + key.formattedLot() + "</strong>\"." });
    } else {
      showUserMessage("Error Getting Lot Inventory", { description: errorThrown || "Please refresh the page to try again. If the problem persists, please contact the system administrator." });
    }
  });

  return lot;
};

module.exports = {
  viewModel: otherInputMatPickerVM,
  template: require('./other-input-materials-picker.html')
};
