var warehouseService = require('services/warehouseService'),
    InterWarehouseSummaryFactory = require('App/models/InterWarehouseSummaryModel'),
    productsService = require('services/productsService'),
    helpers = require('App/helpers/koHelpers');

function InterWarehouseMovementSummaryViewModel(params) {
    var self = this, 
        selectedItem;

    // Methods and properties to dispose via .dispose()
    self.disposables = [];

    // Data
    self.isLoading = ko.observable();
    self.loadingMessage = ko.observable('');

    self.returnedMovementArray = ko.observableArray();
    self.selectedMovement = ko.observable();
    self.shippedOrders = self.returnedMovementArray.filter(function (item) {
        return item.Shipment.Status() === 10;
    });
    self.unshippedOrders = self.returnedMovementArray.filter(function (item) {
        return item.Shipment.Status() !== 10;
    });


    // Behaviors
    var pager = warehouseService.getInterWarehouseMovementsDataPager();
    pager.addParameters(params.filters);
    pager.addNewPageSetCallback(function () {
        self.returnedMovementArray([]);
    });

    function buildMovementSummary(input) {
        return ko.utils.arrayMap(input, self.mapSummaryItem);
    }

    function getNextPage() {
        self.isLoading(true);
        self.loadingMessage('Loading...');
        return pager.nextPage()
            .then(function (values) {
                var array = self.returnedMovementArray();
                ko.utils.arrayPushAll(array, buildMovementSummary(values));
                self.returnedMovementArray.valueHasMutated();
            })
            .always(function () {
                self.isLoading(false);
            });
    }

    self.setSelectedMovement = function () {
        var context = ko.contextFor(arguments[1].originalEvent.target) || {};
        context = context.$data || null;
        if (!(context instanceof InterWarehouseSummaryFactory)) { return; }

        selectedItem && selectedItem.isSelected(false);
        context.isSelected(true);
        selectedItem = context;
        self.selectedMovement(context);
    };

    self.getNextResultsPageCommand = ko.asyncCommand({
        execute: function(complete) { getNextPage().always(complete); },
        canExecute: function(isExecuting) { return !isExecuting; }
    });
    
    self.addSummaryItem = function (key) {
        warehouseService.getInterWarehouseDetails(key)
            .done(function (data) {
                var updatedMovement = self.mapSummaryItem(data);
                self.returnedMovementArray.unshift(updatedMovement);
            }
        );
    };

    self.updateSummaryItem = function (key, values) {
        var movement;

        for (var i = 0, list = self.returnedMovementArray(), max = list.length;
                i < max; i += 1) {
            if (list[i].MovementKey === key) {
                movement = list[i];
                if (!values) {
                    warehouseService.getInterWarehouseDetails(movement.MovementKey)
                        .done(function (data) {
                            update(i, data);
                        });
                } else {
                    update(i, values);
                }
                break;
            }
        }

        function update (i, data) {
            var mappedData = self.mapSummaryItem(data);
            self.returnedMovementArray.splice(i, 1, mappedData);
        }
    };

    params.exports(self);

    getNextPage();
}

InterWarehouseMovementSummaryViewModel.prototype.mapSummaryItem = function(values) {
    var summary = InterWarehouseSummaryFactory(values);
    summary.isSelected = ko.observable(false);
    return summary;
}
InterWarehouseMovementSummaryViewModel.prototype.dispose = function () {
    ko.utils.arrayForEach(this.disposables, this.disposeOne);
    ko.utils.objectForEach(this, this.disposeOne);
};

InterWarehouseMovementSummaryViewModel.prototype.disposeOne = function (propOrValue, value) {
    var disposable = value || propOrValue;

    if (disposable && typeof disposable.dispose === "function") {
        disposable.dispose();
    }
};

module.exports = {
    viewModel: InterWarehouseMovementSummaryViewModel,
    template: require('./inter-warehouse-movement-summary.html')
};
