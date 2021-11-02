require('App/helpers/koPunchesFilters');

ko.punches.enableAll();

function LotInventoryTransactionsViewModel (params) {
    var self = this;

    // Data
    this.transactionsData = params.input;

    this.transactionTotals = ko.pureComputed(function () {
        var totalWeight = 0,
        totalQuantity = 0;

        for (var i = 0, list = self.transactionsData(), max = list.length; i < max; i += 1) {
            totalQuantity += Number(list[i].Quantity);
            totalWeight += Number(list[i].Weight);
        }

        return {
            totalQuantity: totalQuantity,
            totalWeight: totalWeight
        };
    });

    // Behaviors

    // Exports
    if (params.hasOwnProperty('exports')) {
        params.exports(this);
    }

    return this;
}

module.exports = {
    viewModel: LotInventoryTransactionsViewModel,
    template: require('./lot-inventory-transactions.html')
};
