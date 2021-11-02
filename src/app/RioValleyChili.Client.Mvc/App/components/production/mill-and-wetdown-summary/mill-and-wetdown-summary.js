var inventoryService = require('App/services/inventoryService');

require('App/helpers/koPunchesFilters.js');

ko.punches.enableAll();

function millWetdownSummaryVM(params) {
  if (!(this instanceof millWetdownSummaryVM)) { return new millWetdownSummaryVM(params); }

  var self = this;

  this.isInit = params.isInit || ko.observable(false);

  // Data
  this.summaryData = ko.computed(function() {
    var input = ko.unwrap(params.input) || {};

    return {
      lots: input
    };
  });

  this.lots = ko.pureComputed(function() {
    var input = ko.unwrap(input) || {};

    return input.lots;
  });

  // Behaviors
  function init() {
    self.isInit(true);
  }

  this.select = function(vm, $element) {
    var context = ko.contextFor($element.target).$data;
    var key = context ? context.MillAndWetdownKey : null;

    if (key && params.getKey) {
      params.getKey(key);
    }
  };

  init();

  // Exports
  if (params && params.exports) {
    params.exports({
      isInit: self.isInit,
    });
  }

  return this;
}

module.exports = {
  viewModel: millWetdownSummaryVM,
  template: require('./mill-and-wetdown-summary.html'),
  synchronous: true
};
