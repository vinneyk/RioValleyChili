var inventoryService = require('App/services/inventoryService.js');

function DehydratedMaterialsSummaryVM (params) {
  if (!(this instanceof DehydratedMaterialsSummaryVM)) { return new DehydratedMaterialsSummaryVM(params); }

  var self = this;

  this.isInit = ko.observable(false);

  // Data
  this.summaryItems = ko.observableArray();

  // Behaviors
  function getItems() {
    return inventoryService.getDehydratedMaterials().done(function(data, textStatus, jqXHR) {
      var summaryItems = self.summaryItems();

      self.summaryItems(summaryItems.concat(data));
    })
    .fail(function(jqXHR, textStatus, errorThrown) {
      console.log(errorThrown);
      showUserMessage('Could not get dehydrated materials', { description: errorThrown });
    });
  }

  function updateEntry(data) {
    var lotkey = typeof data === 'string' && data;

    if (lotkey) {
      return inventoryService.getDehydratedMaterials(lotkey).done(function(data, textStatus, jqXHR) {
        performUpdate(data);
      })
      .fail(function(jqXHR, textStatus, errorThrown) {
      });
    } else {
      performUpdate(data);
    }

    function performUpdate(data) {
      var items = self.summaryItems();
      var lotKey = data.LotKey;
      var summaryItem = ko.utils.arrayFirst(items, function(entry) {
        return entry.LotKey === lotKey;
      });
      var isNew = !summaryItem;

      if (isNew) {
        self.summaryItems.unshift(data);
      } else {
        self.summaryItems.splice(items.indexOf(summaryItem), 1, data);
      }
    }
  }

  this.selectItem = function(vm, $element) {
    var context = ko.contextFor($element.target).$data;
    var key = (context.LotKey && context.LotKey.toString() || '').replace(/ /g, '');

    if (key && self.summaryItems().indexOf(context) >= 0) {
      if (params.getItem) {
        params.getItem(key);
      }
    }
  };

  // Exports
  if (params.exports) {
    params.exports({
      getItems: getItems,
      isInit: this.isInit,
      updateEntry: updateEntry
    });
  }

  self.isInit(true);

  return this;
}

module.exports = {
  viewModel: DehydratedMaterialsSummaryVM,
  template: require('./dehydrated-materials-summary.html')
};
