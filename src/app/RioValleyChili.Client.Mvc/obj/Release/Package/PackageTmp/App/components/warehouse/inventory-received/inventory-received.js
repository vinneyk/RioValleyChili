ko.punches.enableAll();
var rvc = require('rvc');

function InventoryReceived(params) {
  if (!(this instanceof InventoryReceived)) { return new InventoryReceived(params); }

  var self = this;

  this.data = ko.pureComputed(function() {
    return params.values();
  });

  this.showTotalWeight = ko.pureComputed(function () {
    return true;
  });

  this.totalWeight = ko.pureComputed(function() {
    var data = self.data() || { InventoryItems: [] };
    var total = 0;
    ko.utils.arrayForEach(data.InventoryItems, function(i) {
      total += i.Weight;
    });
    return total;
  });


  // Behaviors

  // Exports
  if (params && ko.isObservable(params.exports)) {
    params.exports({ });
  }

  return this;
}

module.exports = {
  viewModel: InventoryReceived,
  template: require('./inventory-received.html')
};