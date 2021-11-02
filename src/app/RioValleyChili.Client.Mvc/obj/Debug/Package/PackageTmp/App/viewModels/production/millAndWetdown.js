var rvc = require('rvc');
var inventoryService = require('App/services/inventoryService');
var page = require('page');

page.base('/Production/MillAndWetdown');

require('Scripts/app/koBindings.js');

ko.components.register('mw-summary', require('App/components/production/mill-and-wetdown-summary/mill-and-wetdown-summary'));
ko.components.register('mw-editor', require('App/components/production/mill-and-wetdown-editor/mill-and-wetdown-editor'));
ko.components.register('loading-screen', require('App/components/common/loading-screen/loading-screen'));

function MillAndWetdownVM() {
  if (!(this instanceof MillAndWetdownVM)) { return new MillAndWetdownVM(params); }

  var self = this;

  this.isInit = ko.observable(false);

  // Data
  var dataPager = inventoryService.getMillAndWetdownPager();

  this.moreDetailsItem = ko.observable();
  this.searchKey = ko.observable().extend({ lotKey: true });

  this.isEditing = ko.observable();
  this.isEditingEnabled = ko.observable();

  this.recentLots = ko.observableArray();
  this.currentLot = ko.observable();
  this.isCurrentLot = ko.pureComputed(function(key) {
    return key === self.currentLot();
  });
  this.defaults = {
    ethoxLot: ko.observable(),
    sipernatLot: ko.observable(),
  };

  var scrollToItem = ko.observable(false);
  var recentLotsBacking = {};
  var lastMillAndWetdownRecord = null;
  var sipernatPattern = /sipernat/i;
  var ethoxPattern = /ethoxiquin/i;

  this.filters = {
    startDate: ko.observable(),
    endDate: ko.observable(),
  };

  this.isSummaryInit = ko.observable(false);
  this.summaryData = ko.observableArray([]);
  this.summaryDataPager = inventoryService.getMillAndWetdownPager({
    onNewPageSet: function() {
      self.summaryData([]);
    },
  });
  this.summaryDataPager.addParameters(self.filters);
  this.summaryExports = ko.observable();

  this.isEditorInit = ko.observable(false);
  this.editorData = ko.observable(null);
  this.editorExports = ko.observable();
  this.isEditorShowing = ko.pureComputed(function() {
    return this.editorData() !== null;
  }, this);


  this.goToKey = function(lotKey) {
    var key = typeof lotKey === 'string' ? lotKey : self.searchKey();

    key = key.replace(/\s/g, '');

    page('/' + key);
  };

  this.applyFiltersCommand = ko.asyncCommand({
    execute: function(complete) {
      self.summaryDataPager.resetCursor();
      getNextSummaryPage().always(complete);
    },
    canExecute: function(isExecuting) {
      return self.isSummaryInit() && !isExecuting;
    }
  });

  this.clearFiltersCommand = ko.command({
    execute: function() {
      self.filters.startDate(null);
      self.filters.endDate(null);
    },
    canExecute: function() {
      return true;
    }
  });

  // Computed Data
  this.isNewRecord = ko.pureComputed(function() {
    var lot = this.currentLot();

    return lot === 'new';
  }, this);

  // Behaviors
  function getDefaults() {
    var lots = self.summaryData;

    var getRecentEntries = getNextSummaryPage().then(
    function(data, textStatus, jqXHR) {
      if (data.length) {
        setDefaultsFromEntry(data[0]);
      }
    });

    function setDefaultsFromEntry(entry) {
      inventoryService.getMillAndWetdownDetails(entry.OutputChileLotKey).then(
        function(data, textStatus, jqXHR) {
        var result = ko.utils.arrayFirst(data.PickedItems || [], function(pickedItem) {
          if (!self.defaults.ethoxLot() && isEthoxiquin(pickedItem)) {
            self.defaults.ethoxLot(pickedItem.LotKey);
          } else if (!self.defaults.sipernatLot() && isSipernat(pickedItem)) {
            self.defaults.sipernatLot(pickedItem.LotKey);
          }

          return self.defaults.ethoxLot() && self.defaults.sipernatLot();
        });
      });

      function isSipernat(item) {
        return isDefaultProduct.call(item, additiveTypeSelector, sipernatNameSelector, sipernatPattern);

        function sipernatNameSelector() {
          return this.Product.ProductName;
        }
      }
      function isEthoxiquin(item) {
        return isDefaultProduct.call(item, additiveTypeSelector, ethoxNameSelector, ethoxPattern);

        function ethoxNameSelector() {
          return this.Product.ProductName;
        }
      }
      function additiveTypeSelector() {
        return this.Product.ProductType;
      }
    }
  }

  function isDefaultProduct(productTypeSelector, productNameSelector, expression) {
    return productTypeSelector.call(this) === rvc.lists.inventoryTypes.Additive.key &&
      productNameSelector.call(this).match(expression);
  }
  function checkPickedItemForDefaults(pickedItem) {
    if (!pickedItem) return;

    if (isDefaultProduct.call(pickedItem, productTypeSelector, productNameSelector, sipernatPattern)) {
      replaceDefault(defaultSipernatLot, "Sipernat", pickedItem.LotKey);
    } else if (isDefaultProduct.call(pickedItem, productTypeSelector, productNameSelector, ethoxPattern)) {
      replaceDefault(defaultEthoxLot, "Ethoxiquin", pickedItem.LotKey);
    }

    function productTypeSelector() {
      return this.ProductType;
    }
    function productNameSelector() {
      return this.ProductName;
    }
    function replaceDefault(defaultVariable, name, lotNumber) {
      var lastDefault = defaultVariable();
      if (!lastDefault) {
        defaultVariable(lotNumber);
        return;
      }
      if (lastDefault === lotNumber) { return; }

      showUserMessage("Update default " + name + " lot?", {
        description: "Would you like to update the default <strong>" + name + "</strong> lot to <strong>" + lotNumber + "</strong>? Click \"Yes\" to update the default value. Click \"No\" to leave the default lot as " + lastDefault + ".",
        type: 'yesno',
        onYesClick: function () {
          defaultVariable(lotNumber);
        }
      });
    }

  }
  function getNextSummaryPage() {
    return self.summaryDataPager.nextPage().then(
    function(data, textStatus, jqXHR) {
      var lots = self.summaryData;

      lots(lots().concat(data));

      return lots();
    },
    function(jqXHR, textStatus, errorThrown) {
      showUserMessage('Could not fetch next page', { description: errorThrown });
    });
  }

  function toDto() {
    return {
      ProductionDate: model.OutputChileLotKey.Date(),
      LotSequence: model.OutputChileLotKey.Sequence(),
      ShiftKey: model.ShiftKey(),
      ChileProductKey: model.ChileProductKey(),
      ProductionLineKey: model.ProductionLineKey(),
      ProductionBegin: model.ProductionBegin(),
      ProductionEnd: model.ProductionEnd(),
      ResultItems: model.outputInventoryViewModel.toDto(),
      PickedItems: inputs,
    };
  }

  this.loadDetailsStatus = {
    loading: ko.observable(false),
    message: "Loading lot details...",
  };

  function getDetails(key) {
    self.loadDetailsStatus.loading(true);
    var fetchDetails = inventoryService.getMillAndWetdownDetails(key).then(function(data, textStatus, jqXHR) {
      return data;
    })
    .fail(function(jqXHR, textStatus, errorThrown) {
      showUserMessage('Unable to load lot ' + key, { description: errorThrown });
      return arguments;
    })
    .always(function() {
      self.loadDetailsStatus.loading(false);
    });

    return fetchDetails;
  }

  function addToRecentLots(newLotKey) {
    var lots = self.recentLots();
    var lotIndex = lots.indexOf(newLotKey);

    if (lotIndex > -1) {
      self.recentLots.splice(lotIndex, 1);
    }

    self.recentLots([newLotKey].concat(lots));

    if (lots.length >= 5) {
      self.recentLots.pop();
    }
  }

  this.getSummaryItemsCommand = ko.asyncCommand({
    execute: function(complete) {
      getNextSummaryPage().done(function(data, textStatus, jqXHR) {
      })
      .always(complete);
    },
    canExecute: function(isExecuting) {
      return self.isSummaryInit() && !isExecuting;
    }
  });

  this.saveEntry = ko.asyncCommand({
    execute: function(complete) {
      var editor = self.editorExports();
      var editorData = editor.toDto();
      var isNew = self.isNewRecord();

      var saveData = self.save(editorData).then(
      function(data, textStatus, jqXHR) {
        if (isNew) {
          addToRecentLots(editorData.LotKey);
          editor.complete();
        }
      })
      .always(complete);
    },
    canExecute: function(isExecuting) {
      return true;
    }
  });

  this.deleteEntry = ko.asyncCommand({
    execute: function(complete) {
      var editor = self.editorData();
      var lotKey = editor && editor.MillAndWetdownKey;

      showUserMessage( 'Delete Mill & Wetdown record?', {
        description: 'Are you sure you want to delete this record? This action cannot be undone.',
        type: 'yesno',
        onYesClick: function() {
          var deleteData = self.deleteLot( lotKey )
          .done(function( data, textStatus, jqXHR ) {
            page('/');
          })
          .fail(function( jqXHR, textStatus, errorThrown ) {
            showUserMessage( 'Delete failed', {
              description: errorThrown
            });
          })
          .always( complete );

          return deleteData;
        },
        onNoClick: function() {
          complete();
        },
      });
    },
    canExecute: function(isExecuting) {
      return !isExecuting;
    }
  });

  this.initNewRecordCommand = ko.command({
    execute: function() {
      page('/new');
    },
    canExecute: function() {
      return true;
    }
  });

  this.editCommand = ko.command({
    execute: function() {
      self.isEditing(true);
    },
    canExecute: function() {
      return self.isEditingEnabled();
    }
  });

  this.cancelEditCommand = ko.command({
    execute: function() {
      self.isEditing(false);
    },
    canExecute: function() {
      return self.isEditing();
    }
  });

  this.closePopupCommand = ko.command({
    execute: function() {
      page('/');
    },
    canExecute: function() {
      return true;
    }
  });

  page('/:key?', navigateToLot);
  function navigateToLot(ctx) {
    var key = ctx.params.key;
    var editorData = self.editorData;
    var editor = self.editorExports();

    self.currentLot(key);
    self.isEditingEnabled(false);

    if (!key) {
      self.searchKey(null);
      editorData(null);
      self.isEditing(false);
    } else if (key === 'new') {
      self.searchKey(null);
      self.isEditingEnabled(true);
      self.isEditing(true);
      editorData({
        MillAndWetdownKey: 'new'
      });
    } else if (key) {
      self.isEditing(false);
      getDetails(key).done(function(data) {
        addToRecentLots(key);
        self.isEditingEnabled(data.enabledEditing);
        self.isEditingEnabled(true);
        editorData(data);
      });
    }
  }

  this.addSummaryItem = function(data) {
    var summaryItems = self.summaryData();
    var matchingEntry = ko.utils.arrayFirst(summaryItems, function(item) {
      return item.OutputChileLotKey === data.OutputChileLotKey;
    });

    if (matchingEntry) {
      var i = summaryItems.indexOf(matchingEntry);
      self.summaryData.splice(i, 1, data);
    } else {
      self.summaryData([data].concat(summaryItems));
    }
  };

  this.removeSummaryItem = function( lotKey ) {
    var summaryItems = self.summaryData();

    var matchingEntry = ko.utils.arrayFirst( summaryItems, function( item ) {
      return item.OutputChileLotKey === lotKey;
    });

    if ( matchingEntry ) {
      var i = summaryItems.indexOf( matchingEntry );

      self.summaryData.splice( i, 1 );
    }
  }

  this.saveStatus = {
    saving: ko.observable(false),
    message: "Saving Mill & Wetdown record..."
  };

  // Init
  // Fetches from summary items, loads inital page for summary view
  getDefaults();

  // Waits for all sub-components to load before initializing
  ko.computed({
    read: function() {
      var editor = self.isEditorInit();
      var summary = self.isSummaryInit();

      if (editor && summary) {
        self.isInit(true);
        page();
      }
    },
    disposeWhen: function() {
      return self.isInit();
    }
  });

  // Exports
  return this;
}

MillAndWetdownVM.prototype.save = function(data) {
  var self = this;

  var isNew = this.isNewRecord();
  var lotKey = data && data.LotKey || "";
  var lotKeyFormatted = [lotKey.substr(0,2), lotKey.substr(2,2), lotKey.substr(4,3), lotKey.substr(7)].join(' ');

  if (data) {
    self.saveStatus.saving(true);
    if (isNew) {
      var saveNewData = inventoryService.createMillAndWetdownEntry(data).then(
        function(data, textStatus, jqXHR) {
        showUserMessage("Save Successful", { description: "".concat("<b>", lotKeyFormatted, "</b> has been created.") });
      },
      function(jqXHR, textStatus, errorThrown) {
        showUserMessage("Save Failed", { description: errorThrown });
      });

      var addNewSummary = saveNewData.then(function() {
        return inventoryService.getMillAndWetdownDetails(lotKey).then(
          function(data, textStatus, jqXHR) {
            self.addSummaryItem(data);
        });
      });

      saveNewData.always(function() {
        self.saveStatus.saving(false);
      });

      return saveNewData;
    } else {
      var saveData = inventoryService.updateMillAndWetdownEntry(lotKey, data).then(
        function(data, textStatus, jqXHR) {
        showUserMessage("Save Successful", { description: "".concat("<b>", lotKeyFormatted, "</b> has been updated.") });
      },
      function(jqXHR, textStatus, errorThrown) {
        showUserMessage("Save Failed", { description: errorThrown });
      });

      var addSummary = saveData.then(function() {
        return inventoryService.getMillAndWetdownDetails(lotKey).then(
          function(data, textStatus, jqXHR) {
            self.addSummaryItem(data);
        });
      });

      saveData.always(function() {
        self.saveStatus.saving(false);
      });

      return saveData;
    }
  } else {
    showUserMessage("Save Failed", { description: "Please fill out all required fields and try again" });

    return $.Deferred().reject();
  }
};

MillAndWetdownVM.prototype.deleteLot = function( lotKey ) {
  var self = this;

  var deleteData = inventoryService.deleteMillAndWetdownEntry( lotKey ).then(
  function( data, textStatus, jqXHR ) {
    // Remove from summary table
    self.removeSummaryItem( lotKey );
  });

  return deleteData;
};

var vm = new MillAndWetdownVM();

ko.applyBindings(vm);

module.exports = vm;
