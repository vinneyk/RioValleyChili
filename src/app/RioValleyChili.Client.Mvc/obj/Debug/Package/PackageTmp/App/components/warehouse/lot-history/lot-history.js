var lotService = require('App/services/lotService'),
    page = require('page');

require('App/koExtensions');

function LotHistoryViewModel (params) {
    var self = this;

    // Data
    this.inputMaterialsData = ko.observableArray([]);
    this.transactionsData = ko.observableArray([]);

    this.searchKey = ko.observable();

    this.isLoading = ko.observable(false);
    this.loadingMessage = ko.observable('');
    this.isInit = false;

    this.inputMaterialsTotalWeight = ko.pureComputed(function () {
        var total = 0;

        for (var i = 0, list = self.inputMaterialsData(), max = list.length; i < max; i += 1) {
            total += list[i].Weight;
        }

        return total;
    });
    this.transactionsTotals = ko.pureComputed(function () {
        var totalWeight = 0,
        totalQuantity = 0;

        for (var i = 0, list = self.transactionsData(), max = list.length; i < max; i += 1) {
            totalQuantity += list[i].Quantity;
            totalWeight += list[i].Weight;
        }

        return {
            totalQuantity: totalQuantity,
            totalWeight: totalWeight
        };
    });

    // Behaviors
    this.searchLotNumber = function () {
        var key = self.searchKey();

        self.isLoading(true);
        self.loadingMessage('Loading Lot #' + key);
        $.when(lotService.getInputMaterialsDetails(key), lotService.getTransactionsDetails(key))
            .done(function (inputMaterials, transactions) {
                self.inputMaterialsData(inputerMaterials);
                self.inputMaterialsData.valueHasMutated();
                self.transactionsData(transactions);
                self.transactionsData.valueHasMutated();
                self.isInit = true;
            })
            .fail(function (xhr, status, message) {
                showUserMessage('Failed to load data', { 
                    description: message
                });
            })
            .always(function () {
                self.isLoading(false);
                self.loadingMessage('');
            });
    };

    // Exports
    return self;
}

ko.components.register('loading-screen', require('App/components/common/loading-screen/loading-screen')); 

var vm = new LotHistoryViewModel();

ko.applyBindings(vm);

