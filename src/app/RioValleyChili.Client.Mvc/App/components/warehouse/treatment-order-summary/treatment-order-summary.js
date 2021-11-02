var treatmentService = require('services/treatmentService'),
    TreatmentOrderSummaryFactory = require('App/models/InterWarehouseSummaryModel'),
    productsService = require('services/productsService'),
    helpers = require('App/helpers/koHelpers');

function TreatmentOrderSummaryViewModel(params) {
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
        return item.Shipment.Status() === 10 && item.OrderStatus() === 0;
    });
    self.unshippedOrders = self.returnedMovementArray.filter(function (item) {
        return item.Shipment.Status() !== 10;
    });
    self.receivedOrders = self.returnedMovementArray.filter(function (item) {
        return item.Shipment.Status() === 10 && item.OrderStatus() === 1;
    });

    // Behaviors
    var pager = treatmentService.getTreatmentOrdersDataPager();
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
        pager.nextPage()
            .then(function (pagedItems) {
                var array = self.returnedMovementArray();
                ko.utils.arrayPushAll(array, buildMovementSummary(pagedItems));
                self.returnedMovementArray.valueHasMutated();
            })
            .always(function () {
                self.isLoading(false);
            });
    }

    self.setSelectedMovement = function () {
        var context = ko.contextFor(arguments[1].originalEvent.target) || {};
        context = context.$data || null;
        if (!(context instanceof TreatmentOrderSummaryFactory)) { return; }
        
        selectedItem && selectedItem.isSelected(false);
        context.isSelected(true);
        selectedItem = context;
        self.selectedMovement(context);
    };

    self.setTreatmentModal = function (value) {
        var data = {
            orderNumber: value.MovementKey,
            treatmentFacility: value.DestinationFacility.FacilityName,
            treatmentApplied: value.InventoryTreatment.TreatmentNameShort
        };

        params.setTreatmentModal(data);
    };

    self.getNextResultsPage = function () {
        getNextPage();
    };

    self.getNewResultsPage = function () {
        pager.resetCursor();
        getNextPage();
    };

    self.getNextResultsPage();

    self.addSummaryItem = function (key) {
        treatmentService.getTreatmentOrderDetails(key)
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
                    treatmentService.getTreatmentOrderDetails(movement.MovementKey)
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

    self.deleteSummaryItem = function(key) {
      var treatmentOrders = self.returnedMovementArray();

      var treatmentIndex = treatmentOrders.indexOf(ko.utils.arrayFirst(treatmentOrders, function(treatmentOrder) {
        return key === treatmentOrder.MovementKey;
      }));

      self.returnedMovementArray.splice(treatmentIndex, 1);
    };

    params.exports(self);
}

TreatmentOrderSummaryViewModel.prototype.mapSummaryItem = function(values) {
    var summaryItem = new TreatmentOrderSummaryFactory(values);
    summaryItem.isSelected = ko.observable(false);
    return summaryItem;
};

TreatmentOrderSummaryViewModel.prototype.dispose = function () {
    ko.utils.arrayForEach(this.disposables, this.disposeOne);
    ko.utils.objectForEach(this, this.disposeOne);
};

TreatmentOrderSummaryViewModel.prototype.disposeOne = function (propOrValue, value) {
    var disposable = value || propOrValue;

    if (disposable && typeof disposable.dispose === "function") {
        disposable.dispose();
    }
};

module.exports = {
    viewModel: TreatmentOrderSummaryViewModel,
    template: require('./treatment-order-summary.html')
};
