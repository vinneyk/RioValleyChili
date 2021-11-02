var page = require('page');
var queryBase = require('services/serviceCore');
ko.components.register('inventory-receiving', require('components/warehouse/inventory-receiving/inventory-receiving'));
ko.components.register('inventory-received', require('components/warehouse/inventory-received/inventory-received'));
require('App/koBindings');

(function() {
  var vm = {
    inventoryReceivingViewModel: ko.observable(),
    currentLot: ko.observable(),
    searchLot: ko.observable().extend({ lotKey: true })
  };

  vm.showReceiving = ko.pureComputed(function() {
    return vm.currentLot() == null;
  });
  vm.showArchived = ko.pureComputed(function() {
    return vm.currentLot() != null;
  });

  vm.searchLotCommand = ko.command({
    execute: function () {
      var lot = vm.searchLot();
      page('/' + lot);
    },
    canExecute: function() {
      return vm.searchLot();
    }
  });

  vm.closeLotCommand = ko.command({
    execute: function() {
      page('/');
    },
    canExecute: function() {
      return vm.currentLot() != null;
    }
});

  vm.saveAsyncCommand = ko.asyncCommand({
    execute: function(done) {
      var receivingVm = vm.inventoryReceivingViewModel();
      return receivingVm.saveAsyncCommand.execute()
        .done(function(lot) {
          showUserMessage('Inventory Created', { description: 'The inventory has been created successfully. The new inventory lot is <strong>' + lot + '</strong>.' });
          page( '/' + lot );
        })
        .always(done);
    },
    canExecute: function(isExecuting) {
      return !isExecuting && vm.inventoryReceivingViewModel() != null;
    }
  });

  page.base('/Warehouse/Receiving');
  page('/:lot?', function (ctx) {
    var lot = ctx.params.lot;
    if (lot) {
      loadLotByKey(lot);
    } else {
      clearSelection();
    }
  });

  ko.applyBindings(vm);
  page();

  function loadLotByKey(lot) {
    return fetchInventoryReceivedByLot(lot)
      .done(function (data) {
        vm.currentLot(data);
      });
  }

  function clearSelection() {
    vm.currentLot(null);
  }
}());


function fetchInventoryReceivedByLot(lot) {
  return queryBase.ajax('/api/inventory-received/' + lot);
}
