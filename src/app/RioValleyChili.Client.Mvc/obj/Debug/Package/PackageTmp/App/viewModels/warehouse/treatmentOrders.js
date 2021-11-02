var rvc = require('app'),
    treatmentService = require('services/treatmentService'),
    warehouseLocationsService = require('services/warehouseLocationsService'),
    page = require('page');

ko.components.register('loading-screen', require('App/components/common/loading-screen/loading-screen'));
ko.components.register('treatment-order-editor', require('App/components/warehouse/treatment-order-editor/treatment-order-editor'));
ko.components.register('treatment-order-summary', require('App/components/warehouse/treatment-order-summary/treatment-order-summary'));
ko.components.register('post-and-close-inventory-order', require('App/components/warehouse/post-and-close-inventory-order/post-and-close-inventory-order'));
ko.components.register('inventory-picker', require('App/components/inventory/inventory-picker/inventory-picker'));
ko.components.register('inventory-filters', require('App/components/common/lot-filters/lot-filters'));

require('App/koExtensions');

ko.bindingHandlers.onEnter = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var allBindings = allBindingsAccessor();
        $(element).keypress(function (event) {
            var keyCode = (event.which ? event.which : event.keyCode);
            if (keyCode === 13) {
                allBindings.onEnter.call(viewModel);
                return false;
            }
            return true;
        });
    }
};

function TreatmentOrdersViewModel() {
    if (!(this instanceof TreatmentOrdersViewModel)) return new TreatmentOrdersViewModel();

    var self = this,
    inventoryPickerData = ko.observable();

    // Methods and properties to dispose via .dispose()
    self.disposables = [];

    // Initial bindings
    self.currentMovement = ko.observable();
    self.warehouses = ko.observableArray();
    self.destinationWarehouseLocations = ko.observableArray();
    self.searchBoxValue = ko.observable();
    self.searchKey = ko.observable();
    self.filtersInput = ko.observable();
    self.filterHoldForTreatment = ko.observable(true);

    self.treatmentOrderEditor = ko.observable();
    self.summaryExports = ko.observable();
    self.inventoryPickerExports = ko.computed({
        read: function () {
            return inventoryPickerData();
        },
        write: function (data) {
            inventoryPickerData(data());
        }
    });
    self.postAndCloseExports = ko.observable();
    self.filtersExports = ko.observable();

    self.isNewMovement = ko.computed(function () {
        var val = self.currentMovement();
        return (!val || !ko.unwrap(val.MovementKey));
    });
    self.isPostAndCloseShowing = ko.observable(false);
    self.isPickerShowing = ko.observable(false);

    self.filters = {
        originFacilityKeyFilter: ko.observable(),
        destinationFacilityKeyFilter: ko.observableDate(),
        beginningShipmentDateFilter: ko.observableDate(),
        endingShipmentDateFilter: ko.observable(),
        orderStatusFilter: ko.observable(),
        shipmentStatusFilter: ko.observable().extend({ shipmentStatusType: true })
    };


    self.isWorking = ko.observable(false);
    self.loadingMessage = ko.observable('');

    self.movementKey = ko.observable();
    self.currentEditorItem = ko.computed(function() {
        var editor = self.treatmentOrderEditor();
        var movement = editor && editor.currentMovement();
        // creating self.movementKey as a computed observable
        // was resulting in an unexpected behavior in which
        // the value stopped being updated after loading and closing
        // a treatment order -- VK 9/25/2015
        self.movementKey(movement ? movement.MovementKey() : null);
        return movement;
    });

    self.treatmentFacility = ko.pureComputed(function () {
        var current = self.currentEditorItem();
        return current &&
            current.DestinationFacilityDetails() &&
            current.DestinationFacilityDetails().FacilityName;
    });

    // Load data
    treatmentService.getWarehouses()
        .then(function (values) {
            self.warehouses(values);
        });

    // Subscriptions and computed functions
    self.disposables.push(ko.computed(function() {
        if (self.summaryExports() && self.summaryExports().selectedMovement()) {
            var key = self.summaryExports().selectedMovement().MovementKey;
            if (key !== self.movementKey()) self.setSearchKey(key);
        }
    }));

    self.updateItem = function (key) {
        var dfd = $.Deferred();

        treatmentService.getTreatmentOrderDetails(key)
            .done(function (data) {
                self.summaryExports().updateSummaryItem(key, data);
                self.currentMovement(data);
            })
            .always(function () {
                dfd.resolve();
            });

        return dfd;
    };

    self.loadInventoryCommand = ko.asyncCommand({
        execute: function (complete) {
            var picker = self.inventoryPickerExports();
            return picker.loadInventoryItemsCommand.execute()
                .always(complete);
        },
        canExecute: function (isExecuting) {
            if (isExecuting) return;
            var picker = self.inventoryPickerExports();
            return picker && picker.loadInventoryItemsCommand.canExecute();
        }
    });

    self.isFiltersShowing = ko.pureComputed(function () {
        return self.isPickerLoaded() && !self.isPostAndCloseShowing();
    });
    self.isReceiveEnabled = ko.pureComputed(function () {
        var movementEditor = self.treatmentOrderEditor(),
            currentMovement = movementEditor && movementEditor.currentMovement();
        return currentMovement && currentMovement.EnableReturnFromTreatment() && !movementEditor.isLocked();
    });
    self.isEditorControlsShowing = ko.pureComputed(function () {
        return !self.isPickerShowing() && !self.isPostAndCloseShowing();
    });
    self.isShowingReports = ko.pureComputed(function () {
        return self.currentEditorItem() && !self.isPickerShowing() && !self.isPostAndCloseShowing();
    });

    self.isPickerLoaded = ko.pureComputed(function () {
        var picker = self.inventoryPickerExports();
        return picker && picker.isInit() && self.isPickerShowing();
    });

    var dataPagerFilters = ko.computed(function() {
        var filters = self.filtersExports() || {};
        filters.holdType = ko.computed(function() {
            return self.filterHoldForTreatment() === true ?
              rvc.lists.lotHoldTypes.HoldForTreatment.key :
              '';
            }
        );

        return filters;
    });

    // Bundled data
    self.inventoryPickerInputs = {
        args: ko.observable( {} ),
        pickedInventoryItems: ko.computed(function () {
            var currentMovement = self.currentMovement(); //raw data
            return currentMovement && currentMovement.PickedInventory && currentMovement.PickedInventory.PickedInventoryItems;
        }),
        pickingContext: rvc.lists.inventoryPickingContexts.Treatments,
        pickingContextKey: self.movementKey,
        filters: dataPagerFilters,
        pageSize: 50,
        customerLotCode: ko.observable(),
        customerProductCode: ko.observable()
    };
    self.disposables.push(self.inventoryPickerInputs.pickedInventoryItems);

    // Behaviors
    self.createNewMovement = ko.command({
        execute: function () {
            page('/new');
        },
        canExecute: function () {
            return true;
        }
    });
    self.rinconLocations = ko.observableArray([]);
    (function fetchRinconLocations() {

        var key = rvc.lists.rinconWarehouse.WarehouseKey;
        warehouseLocationsService.getWarehouseLocations(key)
            .done(function (data) {
                self.rinconLocations(data);
                self.rinconLocations.valueHasMutated();
            })
            .fail(function (xhr, status, message) {
                showUserMessage("Failed to load Rincon facility locations", {
                    description: 'Server returned "' + message + '"'
                });
        });
    })();
    self.treatmentModal = {
        orderNumber: ko.observable(),
        treatmentFacility: ko.observable(),
        treatmentApplied: ko.observable(),
        placedInRow: ko.observable()
    };
    self.setTreatmentModal = function (values) {
        var treatmentKey,
            movementEditor = values.treatmentOrderEditor(),
            currentMovement = movementEditor && movementEditor.currentMovement();

        if (currentMovement) {
            treatmentKey = currentMovement.InventoryTreatment();
            self.treatmentModal.orderNumber(currentMovement.MovementKey());
            self.treatmentModal.treatmentFacility(currentMovement.DestinationFacilityDetails().FacilityName);
            self.treatmentModal.treatmentApplied(currentMovement.InventoryTreatment.options[treatmentKey].value);
            $('#treatmentModal').modal('show');
        } else {
            self.treatmentModal.orderNumber(values.orderNumber);
            self.treatmentModal.treatmentFacility(values.treatmentFacility);
            self.treatmentModal.treatmentApplied(values.treatmentApplied);
            $('#treatmentModal').modal('show');
        }
    };

    self.receiveTreatmentOrderCommand = ko.asyncCommand({
        execute: function (complete) {
            var key = self.treatmentModal.orderNumber(),
            values = {
                DestinationLocationKey: self.treatmentModal.placedInRow()
            };

            treatmentService.receiveTreatmentOrderMovement(key, values)
                .done(function (data) {
                    self.updateItem(key).always(function () {
                        $('#treatmentModal').modal('hide');
                    });
                })
                .fail(function (xhr, status, message) {
                    showUserMessage('Failed to receive treatment', { description: message });
                })
                .always(complete);
            return true;
        },
        canExecute: function (isExecuting) {
            return !isExecuting &&
                self.treatmentModal.placedInRow() &&
                (self.movementKey() ? !self.treatmentOrderEditor().isLocked() : true);
        }
    });

    self.showPostAndCloseCommand = ko.command({
        execute: function() {
            var key = self.treatmentOrderEditor().currentMovement().DestinationFacilityKey();
            self.getWarehouseLocationsByFacilityAsync(key)
                .done(function (data) {
                    self.destinationWarehouseLocations(data);
                    self.isPostAndCloseShowing(true);
                })
                .fail(function (xhr, status, message) {
                    showUserMessage("Failed to load locations for destination facility.", {
                        description: 'Error: "' + message + '"'
                    });
                });
        },
        canExecute: function () {
            var editor = self.treatmentOrderEditor(),
                movement = self.currentEditorItem();

            return movement && !self.isNewMovement() &&
                (editor && !editor.isLocked()) &&
                (movement.Shipment() && movement.Shipment().Status < rvc.lists.shipmentStatus.Shipped.key) &&
                (movement.OrderStatus !== 1 && movement.hasPickedInventory());
        }
    });
    self.showReceiveInventoryCommand = ko.command({
        execute: function () {
            self.treatmentOrderEditor().showReceiveInventory();
        },
        canExecute: function () {
            return self.currentEditorItem() && !self.treatmentOrderEditor().isLocked();
        }
    });
    self.postAndCloseCommand = ko.asyncCommand({
        execute: function (complete) {
            self.postAndCloseExports().postAndCloseAsync()
                .done(function() {
                    showUserMessage("Post & close successful", { description: "Inventory has been added to the specified destinations." });
                    self.updateItem(self.movementKey());
                    self.cancelPostAndCloseCommand.execute();
                })
                .fail(function(xhr, status, message) {
                    showUserMessage("Failed to post treatment order.", { description: 'Error: "' + message + '"' });
                })
                .always(function() {
                    complete();
                    self.isWorking(false);
                });

        },
        canExecute: function () {
            var movement = self.currentEditorItem();
            return movement &&
              (movement.Shipment() &&
               movement.Shipment().Status < rvc.lists.shipmentStatus.Shipped.key) &&
               !self.treatmentOrderEditor().isLocked();
        }
    });
    self.cancelPostAndCloseCommand = ko.command({
        execute: function () {
            self.isPostAndCloseShowing(false);
        }
    });
    self.searchForKey = function () {
        var targetKey = self.searchBoxValue();

        if (targetKey) {
            page('/' + targetKey);
        }
    };
    self.setSearchKey = function (key) {
        if (key === undefined) {
            page('/');
            return;
        }

        if (self.summaryExports() && self.summaryExports().selectedMovement()) {
            self.summaryExports().selectedMovement(undefined);
        }

        self.searchKey(key);
        self.isWorking(key ? true : false);

        if (currentMovementKey != key) {
            page('/' + (key || ''));
        }
    };
    self.getNextResultsPageCommand = ko.command({
        execute: function() {
            if (self.summaryExports()) {
                self.summaryExports().getNextResultsPage();
            }
        },

        canExecute: function () { return true; }
    });
    self.getNewResultsPageCommand = ko.command({
        execute: function() {
            if (self.summaryExports()) {
                self.summaryExports().getNewResultsPage();
            }
        },

        canExecute: function () { return true; }
    });

    self.disposables.push(self.movementKey, self.isWorking);

    self.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            var key = self.isNewMovement() ? null : self.movementKey();
            if (!self.treatmentOrderEditor().currentMovement.isValid()) {
                showUserMessage('Validation Errors', { description: 'The Treatment Order contains validation errors. Please correct the errors and retry.' });
                complete();
                return;
            }

            self.isWorking(true);
            self.loadingMessage('Saving Treatment Order...');

            var values = self.treatmentOrderEditor().currentMovement.asDto(),
                isNew = self.isNewMovement(),
                dfd = isNew ?
                  treatmentService.createTreatmentOrderMovement(values) :
                  treatmentService.updateTreatmentOrderMovement(key, values);

            dfd.then(function(data) {
                    var movementKey = isNew ? data : key;
                    showUserMessage("Save Successful", {
                        description: 'Movement ' + movementKey + ' has been ' +
                        (isNew ? 'created' : 'updated') + '.'
                    });
                    if (isNew) {
                        self.summaryExports().addSummaryItem(movementKey);
                        page('/' + data);
                    } else {
                        self.updateItem(movementKey);
                    }
                })
                .fail(function(xhr, status, message) {
                    showUserMessage('Failed to save movement', {
                        description: message
                    });
                })
                .always(function() {
                    complete();
                    self.isWorking(false);
                });

            self.isPostAndCloseShowing(false);
        },
        canExecute: function (isExecuting) {
            return !isExecuting && self.currentEditorItem() && !self.treatmentOrderEditor().isLocked();
        }
    });

    self.cancelCommand = ko.command({
        execute: function () {
            page('/');
        },
        canExecute: function () {
            return self.currentEditorItem() && true;
        }
    });

    self.deleteOrder = ko.asyncCommand({
      execute: function(complete) {
        showUserMessage('Delete treatment order', {
          description: 'Proceeding will permanently remove this record. Are you sure?',
          type: 'yesno',
          onYesClick: function() {
            var key = self.movementKey();
            var deleteOrder = treatmentService.deleteTreatmentOrder(key).then(
            function(data, textStatus, jqXHR) {
              self.cancelCommand.execute();
              self.summaryExports().deleteSummaryItem(key);
            },
            function(jqXHR, textStatus, errorThrown) {
              showUserMessage('Could not delete treatment order', {
                description: errorThrown
              });
            });
          },
          onNoClick: function() {},
        });
        return true;
      },
      canExecute: function(isExecuting) {
        return true;
      }
    });
    self.pickForItemCommand = ko.command({
        execute: function(targetProduct) {
            var input = targetProduct || {};
            input.filters = input.filters || {};
            self.isPickerShowing(true);

            var filters = self.filtersExports();
            filters.inventoryType(input.filters.inventoryType);
            filters.lotType(input.filters.lotType);
            filters.productKey(input.filters.productKey);
            filters.treatmentKey(input.filters.treatmentKey);
            filters.packagingProductKey(input.filters.packagingProductKey);
            filters.warehouseLocationKey(null);

            //NOTE: Treatment Orders are not currently able to maintain their orderItemKey due to limitations in Access data models.
            //      When the web system is decoupled form Access, this functionality can be restored (pending a refactoring of the association in the web sys models).
            //self.inventoryPickerInputs.args( { orderItemKey: targetProduct.orderItemKey });
            //self.inventoryPickerInputs.customerLotCode(input.customerLotCode);
            //self.inventoryPickerInputs.customerProductCode(input.customerProductCode);
            self.inventoryPickerExports().loadInventoryItemsCommand.execute();
        },
        canExecute: function() {
          var editor = self.treatmentOrderEditor(),
          movement = editor && editor.currentMovement(),
          shipment = movement && movement.Shipment();

          return shipment && !self.isNewMovement() &&
            movement.OrderStatus < rvc.lists.orderStatus.Fulfilled.key &&
            shipment.Status < rvc.lists.shipmentStatus.Shipped.key;
        }
    });

    function clearMovements() {
        var summary = self.summaryExports();
        if (summary && ko.isObservable(summary.selectedMovement)) {
            summary.selectedMovement(null);
        }

        self.currentMovement(null);
        self.isPostAndCloseShowing(false);
        self.isPickerShowing(false);
        self.searchBoxValue(null);
        self.searchKey(null);
    }
    self.cancelPickerCommand = ko.command({
        execute: function () {
            self.isPickerShowing(false);
            self.inventoryPickerExports().revertCommand.execute();
        },
        canExecute: function () {
            return self.inventoryPickerExports() && true;
        }
    });

    ko.postbox.subscribe('PickedItemsSaved', function () {
        self.isWorking(true);
        self.loadingMessage('Updating Treatment Order...');
        self.updateItem(self.movementKey()).always(function () {
            self.isPickerShowing(false);
            self.isWorking(false);
        });
    });

    // Routing
    var currentMovementKey;

    page.base('/Warehouse/TreatmentOrders');
    page('/:key?', navigateToMovement);

    function navigateToMovement(ctx) {
        var transferKey = ctx.params.key;
        clearMovements();

        // No routing specified
        if (transferKey == undefined) {
            document.title = "Sent Product For Treatment";
            currentMovementKey = null;
            self.currentMovement(null);
            return;
        }

        // New entry
        if (transferKey === 'new') {
            currentMovementKey = null;
            document.title = "Create New Treatment Order";
            var editor = self.treatmentOrderEditor();
            if (editor) {
                self.initNewTreatmentOrder();
            } else {
                var treatmentOrderEditorSubscription = self.treatmentOrderEditor.subscribe(function() {
                    self.initNewTreatmentOrder();
                    treatmentOrderEditorSubscription.dispose();
                    ko.utils.arrayRemoveItem(self.disposables, treatmentOrderEditorSubscription);
                });
                self.disposables.push(treatmentOrderEditorSubscription);
            }

            return;
        }

        // Existing entry
        if (currentMovementKey !== transferKey) {
            currentMovementKey = transferKey;
            self.loadTreatmentByKey(transferKey);
            return;
        }
    }
}

TreatmentOrdersViewModel.prototype.initNewTreatmentOrder = function () {
    this.currentMovement({});
};

TreatmentOrdersViewModel.prototype.loadTreatmentByKey = function (key) {
    var dfd = treatmentService.getTreatmentOrderDetails(key),
        self = this;

    self.isWorking(true);
    self.loadingMessage('Loading movement #' + key + '...');

    dfd.done(function (values) {
        self.currentMovement(values);
        document.title = 'Treatment Order #' + key;
    }).fail(function () {
        showUserMessage("No Treatment Order Found", {
            description: "The Treatment Order # " + key + " is invalid or doesn't exist. If you believe this is an error, please contact system administrators to report the issue."
        });
        self.currentMovement(null);
    }).always(function() {
        self.isWorking(false);
    });

    return dfd;
};

var facilityLocations = {};
TreatmentOrdersViewModel.prototype.getWarehouseLocationsByFacilityAsync = function (facilityKey) {
    var dfd = $.Deferred();
    var locations = facilityLocations[facilityKey];
    if (locations) {
        dfd.resolve(locations);
    } else {
        warehouseLocationsService.getWarehouseLocations(facilityKey)
            .done(function (values) {
                facilityLocations[facilityKey] = values;
                dfd.resolve(values);
            })
            .fail(dfd.reject);
    }
    return dfd;
};

TreatmentOrdersViewModel.prototype.dispose = function () {
    ko.utils.arrayForEach(this.disposables, this.disposeOne);
    ko.utils.objectForEach(this, this.disposeOne);
};

TreatmentOrdersViewModel.prototype.disposeOne = function(propOrValue, value) {
    var disposable = value || propOrValue;

    if (disposable && typeof disposable.dispose === "function") {
        disposable.dispose();
    }
};

var vm = new TreatmentOrdersViewModel();

ko.applyBindings(vm);
ko.punches.enableAll();

page();
