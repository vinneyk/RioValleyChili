require('App/ko.filters.dateTime');

function LotInventoryInputsViewModel (params) {
    var self = this;

    // Data
    this.inputMaterialsData = params.input;

    this.inputMaterialsTotalWeight = ko.pureComputed(function () {
      var materials = ko.unwrap( self.inputMaterialsData );

      var total = 0;

      for (var i = 0, max = materials.length; i < max; i += 1) {
        total += Number(materials[i].Weight);
      }

      return total;
    });

    // Behaviors

    // Exports
    if (params.hasOwnProperty('exports')) {
        params.exports(this);
    }

    return this;
}

module.exports = {
    viewModel: LotInventoryInputsViewModel,
    template: require('./lot-inventory-inputs.html')
};
