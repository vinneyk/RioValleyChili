var rvc = require('rvc'),
  inventoryService = require('App/services/inventoryService'),
  page = require('page');

page.base('/Warehouse/DehydratedMaterialReceiving');

require('Scripts/app/koBindings.js');
require('Scripts/jquery.plugins.stickytableheaders.js');

ko.components.register('dehydrated-materials-summary', require('App/components/inventory/dehydrated-materials-summary/dehydrated-materials-summary'));
ko.components.register('dehydrated-materials-details', require('App/components/inventory/dehydrated-materials-details/dehydrated-materials-details'));

function DehydratedMaterialsViewModel() {
  if (!(this instanceof DehydratedMaterialsViewModel)) { return new DehydratedMaterialsViewModel(params); }

  var self = this;

  self.isInit = ko.observable(false);

  // Data
  var dataPager = inventoryService.getDehydratedMaterialsDataPager();

  self.summaryExports = ko.observable();
  self.detailsExports = ko.observable();
  self.detailsInput = ko.observable();

  self.isEditingEnabled = ko.observable(false);
  self.isEditing = ko.observable(false);
  self.isAddingVariety = ko.observable(false);

  self.searchKey = ko.observable().extend({ lotKey: true });
  self.selectedKey = ko.observable();
  self.newVarietyName = ko.observable();

  self.recentEntries = ko.observableArray();

  // Computed data
  self.isNew = ko.pureComputed(function () {
    return this.selectedKey() === 'new';
  }, this);

  // Behaviors
  // Commands
  self.loadDetailsByKey = function (key) {
    key && page('/' + key);
  };
  self.recentEntryClicked = function (vm, $element) {
    var key = ko.contextFor($element.target).$data;
    if (key) {
      self.loadDetailsByKey(key);
    }
  };

  self.search = function () {
    var key = self.searchKey();
    self.loadDetailsByKey(key);
  };

  function loadDetails(key) {
    return inventoryService.getDehydratedMaterials(key)
      .done(function(data) {
        self.isEditing(false);
        self.selectedKey(key);
        data.ProductionDate = toDate(data.ProductionDate);
        self.detailsInput(data);
      })
      .fail(function(jqXHR, textStatus, errorThrown) {
        console.log(errorThrown);
        showUserMessage('Could not get lot details', { description: errorThrown });
      });
  }

    function toDate(value) {
      var input = new Date(value),
        dateStr = (input.getUTCMonth() + 1) + '/' + input.getUTCDate() + '/' + input.getUTCFullYear();
      return dateStr;
    }

  self.loadDetails = loadDetails;
  this.loadNextPageCommand = ko.asyncCommand({
    execute: function (complete) {
      self.summaryExports().getItems().always(complete);
    },
    canExecute: function (isExecuting) {
      return !isExecuting;
    }
  });

  this.addDetailsItemCommand = ko.command({
    execute: function() {
      var editor = self.detailsExports();
      editor && editor.addItemCommand.execute();
    },
    canExecute: function() {
      var editor = self.detailsExports();
      return editor && editor.addItemCommand.canExecute();
    }
  });

  this.newVariety = ko.command({
    execute: function () {
      self.newVarietyName(null);
      self.isAddingVariety(true);
    },
    canExecute: function () {
      return true;
    }
  });

  this.saveNewVariety = ko.command({
    execute: function () {
      self.detailsExports().addVariety(self.newVarietyName());
      self.isAddingVariety(false);
    },
    canExecute: function () {
      return self.newVarietyName();
    }
  });

  this.cancelNewVariety = ko.command({
    execute: function () {
      self.isAddingVariety(false);
    },
    canExecute: function () {
      return true;
    }
  });

  // Methods
  this.getItem = function (lot) {
    page('/' + lot);
  };

  this.editCommand = ko.command({
    execute: function () {
      self.isEditing(true);
    },
    canExecute: function () {
      return self.isEditingEnabled();
    }
  });

  this.receiveNew = ko.command({
    execute: function () {
      page('/new');
    },
    canExecute: function () {
      return self.isInit();
    }
  });

  this.addMaterial = ko.command({
    execute: function () {
      self.detailsExports().addMaterial();
    },
    canExecute: function () {
      return true;
    }
  });

  this.saveCommand = ko.asyncCommand({
    execute: function (complete) {
      try {
        var details = self.detailsExports(),
        summary = self.summaryExports();

        if (!details.isValid()) {
          showUserMessage('Please correct validation errors.');
          complete();
        } else {
          var values = details.getValues();
          var dfd = values.LotKey == undefined ?
            self.createDehydratedMaterialReceivingRecord(values)
            .done(function(newKey) {
              addRecent(newKey);
              values.LotKey = newKey;
              summary.updateEntry(newKey);

              showUserMessage('Dehydrated Materials Received Successfully', { description: 'The Dehydrated Materials have been recorded and received into inventory with the lot "' + newKey + '".' });
              try {
                var next = {
                  Load: Number(values.Load) + 1,
                  Supplier: values.Supplier,
                  Product: values.Product,
                  ProductionDate: values.ProductionDate,
                };
                self.detailsInput(next);
                self.detailsExports().addItemCommand.execute();
              } catch (ex) { }
            }) :
            self.updateDehydratedMaterialReceivingRecord(values)
            .done(function (data) {
              addRecent(values.LotKey);
              self.isEditing(false);
              self.detailsInput(data);
            });

          dfd.fail(function (jqXHR, textStatus, errorThrown) {
            showUserMessage('Save failed', { description: errorThrown });
          }).always(complete);
        }
      } catch (ex) {
        complete();
        showUserMessage("We messed up.", { description: "Something went wrong and trying again won't fix it this time. Time to call the programmers :(" });
      }
    },
    canExecute: function (isExecuting) {
      return !isExecuting;
    }
  });

  function addRecent(key) {
    var recent = self.recentEntries;

    recent.unshift(key);

    if (recent().length >= 5) {
      self.recentEntries.pop();
    }
  }

  this.cancelEditCommand = ko.command({
    execute: function () {

      if (self.isNew()) {
        self.detailsInput(null);
        page('/');
      } else {
        self.isEditing(false);
        self.detailsExports().revertChanges();
      }
    },
    canExecute: function () {
      return self.isInit();
    }
  });

  this.closePopupCommand = ko.command({
    execute: function () {
      page('/');
    },
    canExecute: function () {
      return self.selectedKey() != undefined;
    }
  });

  // Init
  // Waits for all sub-components to load before initializing
  var detailsInit = $.Deferred();
      //summaryInit = $.Deferred();

  var init = $.when(detailsInit /*, summaryInit */).done(function() {
    self.isInit(true);
    page();
  });

  ko.computed({
    read: function () {
      var details = self.detailsExports(),
        summary = self.summaryExports();

      details && details.init.then(detailsInit.resolve);
      //summary && summary.init.then(summaryInit.resolve);

      //if (details && details.isInit() &&
      //    summary && summary.isInit()) {
      //  self.isInit(true);

      //  page();
      //}
    },
    disposeWhen: function () {
      return self.isInit();
    }
  });

  page('/:key?', navigateToLot);

  function navigateToLot(ctx) {
    var key = ctx.params.key;

    init.then(function() {
      if (!key) {
        self.isAddingVariety(false);
        self.isEditingEnabled(false);
        self.isEditing(false);
        self.selectedKey(null);
        self.loadNextPageCommand.execute();
      } else if (key === 'new') {
        self.isEditingEnabled(true);
        self.isEditing(true);
        self.selectedKey('new');

        var today = (function () {
          var dateString = '',
            currentDate = Date.now();
          dateString = (currentDate.getMonth() + 1) + '/' + currentDate.getDate() + '/' + currentDate.getFullYear();
          return dateString;
        })();

        self.detailsInput({
          ProductionDate: today,
          Load: '1'
        });
        self.detailsExports().addItemCommand.execute();
      } else if (key) {
        self.isEditing(false);
        loadDetails(key)
            .done(function (data) {
              self.isEditingEnabled(data.IsEditingEnabled);
            })
            .fail(function (jqXHR, textStatus, message) {
              showUserMessage("Failed to load Dehydrated Materials record.", { description: message });
            });
      }

    });
  }

  // Exports
  return this;
}

DehydratedMaterialsViewModel.prototype.createDehydratedMaterialReceivingRecord = function(data) {
  var jsonDto = ko.toJSON(data);

  if (data == undefined) {
    var dfd = $.Deferred();
    dfd.reject();
    return dfd;
  }

  return inventoryService.saveDehydratedMaterials(jsonDto);
};

DehydratedMaterialsViewModel.prototype.updateDehydratedMaterialReceivingRecord = function(dehyData) {
  var jsonDto = ko.toJSON(dehyData);
  var lotKey = dehyData.LotKey;
  var self = this;

  if (dehyData == undefined) {
    return $.Deferred().reject(null, null, 'There are no valid materials to save.');
  } else {
    var updateRecords = inventoryService.updateDehydratedMaterials(lotKey, jsonDto)
    .then(function(data, textStatus, jqXHR) {
      showUserMessage('Dehydrated Materials Updated Successfully', {
        description: 'The Dehydrated Materials have been updated into inventory with the lot "' + lotKey + '".'
      });

      return data;
    },
    function(jqXHR, textStatus, errorThrown) {
      showUserMessage(errorThrown);
    });

    var refreshData = updateRecords
    .then(function() {
      return self.loadDetails(lotKey);
    })
    .then(function(data, textStatus, jqXHR) {
      self.isEditing(false);
      self.summaryExports().updateEntry(data);

      return data;
    },
    function(jqXHR, textStatus, errorThrown) {
      showUserMessage(errorThrown);
    });

    return $.when(refreshData);
  }
};

var vm = new DehydratedMaterialsViewModel();

ko.applyBindings(vm);

module.exports = vm;
