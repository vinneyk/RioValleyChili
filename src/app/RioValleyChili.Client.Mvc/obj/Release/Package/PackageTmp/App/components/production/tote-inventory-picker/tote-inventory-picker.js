var inventoryService = require('App/services/inventoryService');

function toteInventoryPickerVM(params) {
  if (!(this instanceof toteInventoryPickerVM)) { return new toteInventoryPickerVM(params); }

  var self = this;
  var input = ko.toJS(params.input);

  // Data
  var selectedTote = ko.observable();
  this.tote = ko.observable();
  this.totes = ko.observableArray([]);
  this.inventoryItems = ko.observableArray([]);

  // Behaviors
  function getPickedItems() {
    return ko.utils.arrayFilter(ko.toJS(self.inventoryItems()), isPicked);

    function isPicked(item) {
      return item.QuantityPicked > 0;
    }
  }

  function buildDto() {
    var pickedItems = ko.utils.arrayMap(getPickedItems(), function (item) {
      return {
        Tote: item.ToteKey,
        InventoryKey: item.InventoryKey,
        Quantity: item.QuantityPicked
      };
    });

    return pickedItems;
  }

  // computed properties
  this.totalWeightPicked = ko.computed(function () {
    var sum = 0;
    ko.utils.arrayForEach(this.inventoryItems(), function (item) {
      if (item.QuantityPicked()) {
        sum += item.WeightPicked();
      }
    });
    return sum;
  }, this);

  // commands
  this.addToteCommand = ko.asyncCommand({
    execute: function (complete) {
      var tote = self.tote();

      self.getTote(tote).done(function(data, textStatus, jqXHR) {
        addToteInventory(data);
      })
      .always(complete);
    },
    canExecute: function (isExecuting) {
      return !isExecuting && self.tote();
    }
  });

  this.removeToteCommand = ko.command({
    execute: function(inventory) {
      removeToteInventory.bind(self)(inventory.ToteKey());
    }
  });

  // methods 
  this.range = function(max) {
    var r = [];

    for (var i = 0; i < max; i++) {
      r.push(i);
    }

    return r;
  };

  this.toggleSelection = toggleSelectedInventory;

  this.isToteSelected = function(tote) {
    return tote && tote === selectedTote();
  };

  this.tote.extend({ toteKey: self.addToteCommand.execute });

  function addToteInventory(data) {
    var previousInventoryCount = self.inventoryItems().length;

    pushToteInventoryResults.call(self, data);
    self.totes().pushAllWithoutDuplicates([self.tote], function(i) { return i.formattedTote(); });
    self.totes.notifySubscribers();

    if (previousInventoryCount > 0) {
      toggleSelectedInventory(self.tote());
    }

    self.tote.formattedTote(self.tote.getNextTote());
  }

  function addExistingToteInventory(inv) {
    addToteInventory({
      Inventory: inv,
      isExistingPick: true
    });

    getUpdatedInventory(inv);
  }

  function updateToteInventory(data) {
    /** Get matching tote */
    var tote = ko.utils.arrayFirst(self.inventoryItems(), function(item) {
      return ko.unwrap(item.ToteKey) === data.ToteKey;
    });
    var updatedTote = ko.utils.arrayFirst(data.Inventory, function(item) {
      return item.InventoryKey === tote.InventoryKey;
    });

    /** Update quantity */
    tote.Quantity(updatedTote.Quantity);
  }

  function toggleSelectedInventory(toteKey) {
    ko.utils.arrayForEach(self.inventoryItems(), function (item) {
      item.Selected(item.ToteKey === toteKey && !item.Selected());
    });

    if (toteKey && selectedTote() !== toteKey) {
      selectedTote(toteKey);
    } else {
      selectedTote(null);
    }
  }

  function getUpdatedInventory(items) {
    var inventoryItems = items || [];

    ko.utils.arrayForEach(inventoryItems, function(item) {
      self.getTote(ko.unwrap(item.ToteKey), { update: true }).done(function(data, textStatus, jqXHR) {
        updateToteInventory(data);
      });
    });
  }

  function pushToteInventoryResults(data) {
    var inventoryResults = data.Inventory || [];
    var newInventoryItems = this.inventoryItems().pushAllWithoutDuplicates(inventoryResults, function(item) {
      return item.InventoryKey;
    }) || [];
    var isExistingPick = data.isExistingPick;

    ko.utils.arrayForEach(newInventoryItems, function (item) {
      item.ToteKey = ko.observable(item.ToteKey).extend({ toteKey: true });

      if (isExistingPick && !item.InventoryTreatment) {
        // TODO NJH: Remove debug data for inventorytreatment
        item.InventoryTreatment = {
          TreatmentKey: '0',
          TreatmentName: 'Not Treated',
          TreatmentNameShort: 'NA'
        };
        item.Quantity = ko.observable(item.QuantityPicked);
        item.QuantityPicked = ko.observable(item.QuantityPicked);
      } else {
        item.Quantity = ko.observable(item.Quantity);
        item.QuantityPicked = ko.observable(inventoryResults.length > 1 ?
          0 :
          1);
      }
      item.WeightPicked = ko.computed(function () {
        return item.QuantityPicked() * item.PackagingProduct.Weight;
      });

      item.Selected = ko.observable(false);
    });

    if (newInventoryItems.length) {
      this.inventoryItems.notifySubscribers(this.inventoryItems());
    }
  }

  function removeToteInventory(toteKey) {
    var hasPickedInventory = ko.utils.arrayFirst(this.inventoryItems(), function (item) {
      return item.QuantityPicked() > 0;
    });

    var remove = removeDelegate.bind(this);

    if (hasPickedInventory) {
      showUserMessage("Are you sure you want to remove this tote?", {
        type: "yesno",
        onYesClick: remove,
        description: "Some of this tote's inventory items have been picked as input. If you want to remove all inventory items from this tote from the input materials, click \"Yes\" otherwise click \"No\"."
      });
    } else {
      remove();
    }

    function removeDelegate() {
      var index = 0;
      var items = ko.toJS(this.inventoryItems()); //prevent notifications
      ko.utils.arrayForEach(items, function (item) {
        if (item.ToteKey !== toteKey) {
          index++;
          return;
        }
        self.inventoryItems.splice(index, 1);
      });

      var toteIndex = ko.utils.arrayIndexOf(this.totes(), toteKey, this);
      if (toteIndex > -1) {
        this.totes.splice(toteIndex, 1);
      }
    }
  }    

  if (input) {
    resetUI();

    addExistingToteInventory(input);
  }

  function resetUI() {
    selectedTote('');
    self.inventoryItems([]);
    self.totes([]);
    self.tote('');
  }

  // Exports
  if (params && params.exports) {
    params.exports({
      getPickedItems: getPickedItems,
      inventoryItems: self.inventoryItems,
      toDto: buildDto
    });
  }

  return this;
}

toteInventoryPickerVM.prototype.getTote = function(toteKey, opts) {
  var key = ko.observable(toteKey).extend({ toteKey: true });
    var tote = inventoryService.getInventoryByTote(toteKey).fail(function(jqXHR, textStatus, errorThrown) {
      if (jqXHR.status === 404 && opts && opts.update === true) {
        return tote;
      } else if (jqXHR.status === 404) {
        showUserMessage("Tote not found", { description: "There is no available inventory with Tote \"<strong>" + key.formattedTote() + "</strong>\"." });
      } else {
        showUserMessage("Error Getting Tote Inventory", { description: errorThrown || "Please refresh the page to try again. If the problem persists, please contact the system administrator." });
      }
    });

    return tote;
};

module.exports = {
  viewModel: toteInventoryPickerVM,
  template: require('./tote-inventory-picker.html')
};
