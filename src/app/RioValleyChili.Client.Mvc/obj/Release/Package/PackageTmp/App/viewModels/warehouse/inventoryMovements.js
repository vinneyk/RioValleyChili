var inventoryMovementDetailsComponent = require('components/warehouse/intra-warehouse-inventory-movement-details/intra-warehouse-inventory-movement-details');

var rvc = require('app'),
    warehouseService = require('services/warehouseService'),
    productsService = require('services/productsService'),
    page = require('page'),
    helpers = require('App/helpers/koHelpers');

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

function InventoryMovementsViewModel() {
    if (!(this instanceof InventoryMovementsViewModel)) return new InventoryMovementsViewModel();

    var model = this;

    model.init = ko.observable(false);

    $.when(warehouseService.getRinconWarehouseLocations(), 
        productsService.getChileProducts(rvc.lists.chileTypes.FinishedGoods.key),
        productsService.getPackagingProducts())
        .done(function (locations, products, packaging) {
            if (locations[1] !== "success" || products[1] !== "success" || packaging[1] !== "success") {
                throw new Error("Failed to load option arrays for movement.");
            }

            model.warehouseLocationOptions(locations[0] || []);
            model.chileProductOptions(products[0] || []); 
            model.packagingProductOptions(packaging[0] || []); 

            model.init(true);
    });

    model.IntraWarehouseMovementVm = ko.observable();
    model.CurrentMovement = ko.observable();
    model.isNewMovement = ko.observable(false);

    model.warehouseLocationOptions = ko.observableArray([]);
    model.chileProductOptions = ko.observableArray([]);
    model.packagingProductOptions = ko.observableArray([]);

    model.movementItems = ko.observableArray([]);
    model.requestedTrackingSheet = ko.observable();
    model.lookingUpTrackingSheet = ko.observable(false);

    //#region commands
    model.saveMovementCommand = ko.asyncCommand({
        execute: function (complete) {
            var movement = model.IntraWarehouseMovementVm();
            if (movement == undefined) { 
                throw new Error("SelectedMovement can not be undefined."); 
            }
            if (!movement.validation.isValid()) {
                movement.validation.errors.showAllMessages();
                showUserMessage('Invalid movement data', { description: 'Please correct errors before and try again.' });
                complete();
                return;
            }

            model.saveMovementCommand.indicateWorking();

            var packagedData = {
                TrackingSheetNumber: movement.TrackingSheetNumber(),
                OperatorName: movement.OperatorName(),
                MovementDate: movement.MovementDate(),
            };

            if (model.isNewMovement()) {
                packagedData.PickedInventoryItems = mapPickedInventoryItems();
                if (!movement.PickedInventoryVm().isValid()) {
                    if (!movement.validation.isValid()) {
                        showUserMessage('Picked inventory contains errors.', { description: 'Please correct errors before and try again.' });
                        complete();
                        return;
                    }
                }

                warehouseService.createIntraWarehouseMovement(packagedData)
                    .done(function () {
                        showUserMessage("Movement created successfully.", { description: "Tracking sheet number: <strong>" + packagedData.TrackingSheetNumber + "</strong>" });
                        model.initNewMovementCommand.execute();
                        model.saveMovementCommand.clearStatus();
                    })
                    .fail(function (xhr, status, message) {
                        showUserMessage("Failed to crate movement.", { description: message });
                        model.saveMovementCommand.clearStatus();
                    })
                    .always(complete);
            } else {
                warehouseService.updateIntraWarehouseMovement(movement.IntraWarehouseOrderKey(), packagedData)
                    .done(function () {
                        showUsferMessage("Movement updated successfully.", { description: "Tracking sheet number: <strong>" + packagedData.TrackingSheetNumber + "</strong>" });
                        model.saveMovementCommand.clearStatus();
                    })
                    .fail(function (xhr, status, message) {
                        showUserMessage("Failed to crate movement.", { description: message });
                        model.saveMovementCommand.clearStatus();
                    })
                    .always(complete);
            }

            function mapPickedInventoryItems() {
                var items = ko.utils.arrayFilter(movement.PickedInventoryVm().filteredPickedInventoryItems(), function(item) { return item.QuantityPicked() != undefined; });
                return ko.utils.arrayMap(items, function(item) {
                    return {
                        QuantityPicked: item.QuantityPicked(),
                        DestinationLocationKey: item.DestinationWarehouseLocation().LocationKey,
                        InventoryKey: item.InventoryKey()
                    };
                });
            }
        },
        canExecute: function (isExecuting) {
            return !isExecuting && model.IntraWarehouseMovementVm() != undefined && !model.lookingUpTrackingSheet();
        }
    });

    model.initNewMovementCommand = ko.command({
        execute: function (sheetNum) {
            if (!model.isNewMovement()) {
                model.isNewMovement(true);
                page('/new');
            } else {
                model.CurrentMovement({ TrackingSheetNumber: sheetNum });
                addInitialItems(5);
            }

            function addInitialItems(count) {
                function addInitialItemsLoop() {
                    model.IntraWarehouseMovementVm().PickedInventoryVm().clearPickedItemsCommand.execute();
                    for (var i = 0; i < count; i++) {
                        model.IntraWarehouseMovementVm().PickedInventoryVm().addPickedItemCommand.execute();
                    }
                }

                if (!model.IntraWarehouseMovementVm()) {
                    var computed = ko.computed(function () {
                        if (model.IntraWarehouseMovementVm() && model.IntraWarehouseMovementVm().PickedInventoryVm()) {
                            addInitialItemsLoop();
                            computed.dispose();
                        }
                    });
                } else {
                    addInitialItemsLoop();
                }
            }
        },
        canExecute: function () {
            return true;
        }
    });

    var currentMovementKey,
        trackingSheetCache = null;
    model.isPageChanging = false;
    model.trackingSheetMonitor = ko.computed(function() {
        var vm = model.IntraWarehouseMovementVm(),
            trackingSheet = vm && vm.TrackingSheetNumber();

        if (trackingSheetCache === null) {
            return;
        } else if (typeof trackingSheetCache === 'number') {
            trackingSheetCache = trackingSheetCache.toString();
        }

        if (typeof trackingSheet === 'number') {
            trackingSheet = trackingSheet.toString();
        } else if (typeof trackingSheet === 'string') {
            trackingSheet = trackingSheet.trim();
        }

        if (trackingSheet && trackingSheet !== trackingSheetCache) {
            model.lookingUpTrackingSheet(true);
            warehouseService.getIntraWarehouseMovementDetails(trackingSheet)
                .done(function(data, textStatus) {
                    if (textStatus === 'success') {
                        showUserMessage("Tracking sheet already exists", {
                            description: "The tracking sheet number entered already exists. Do you want to load it?",
                            type: 'yesno',
                            onYesClick: function() {
                                trackingSheetCache = trackingSheet;
                                model.isPageChanging = true;
                                page('/' + trackingSheet);
                                model.buildData(data);
                            },
                            onNoClick: function() { }
                        });
                    } else {
                        trackingSheetCache = trackingSheet;
                    }
                })
                .fail(function(xhr, textStatus, message) {
                    if (xhr.status === 404 && !model.isNewMovement()) {
                        model.initTrackingSheetNum = trackingSheet;
                        page('/new');
                    }
                })
                .always(function () {
                    model.lookingUpTrackingSheet(false);
                });
        }
    });


    model.loadTrackingSheet = function () {
        var trackingSheetNum = model.requestedTrackingSheet();
        trackingSheetNum && page('/' + trackingSheetNum);
    };
    //#endregion commands

    helpers.ajaxStatusHelper(model.loadTrackingSheet);
    helpers.ajaxStatusHelper(model.saveMovementCommand);   

    model.init();

    //#region routing

    page.base('/Warehouse/RinconMovements');
    page('/:key?', navigateToMovement);

    //#endregion

    return model;

    function navigateToMovement(ctx) {
        if (model.isPageChanging) { return; }

        var trackingSheetNum;

        if (!ctx) throw new Error("Invalid argument. Expected ctx object.");

        if (!ctx.params || ctx.params.key == undefined || ctx.params.key === 'new') {
            trackingSheetCache = 'new';
            currentMovementKey = null;
            document.title = "Create New Inventory Movement";

            if (!model.IntraWarehouseMovementVm() || !model.IntraWarehouseMovementVm().isNewMovement()) {
                model.initNewMovementCommand.execute(model.initTrackingSheetNum);
                model.initTrackingSheetNum = undefined;
            }
            model.isPageChanging = false;
            return;
        }

        model.isPageChanging = true;
        trackingSheetNum = parseFloat(ctx.params.key);
        if (currentMovementKey !== trackingSheetNum) {
            trackingSheetCache = trackingSheetNum;
            model.loadIntraWarehouseMovementByKey(trackingSheetNum);
            currentMovementKey = trackingSheetNum;
            document.title = 'Inventory Movement #' + trackingSheetNum;

            model.isPageChanging = false;
            return;
        }

    }
}

InventoryMovementsViewModel.prototype.init = function () {
    var model = this;

};

InventoryMovementsViewModel.prototype.loadIntraWarehouseMovementByKey = function (key) {
    var model = this;

    model.loadTrackingSheet.indicateWorking();

    warehouseService.getIntraWarehouseMovementDetails(key)
        .then(function (values) {
            model.buildData(values);
            model.loadTrackingSheet.clearStatus();
        }, function (status, xhr, message) {
            showUserMessage("Failed to retrieve the specified Tracking Sheet", { description: message });
            page('/new');
            model.isNewMovement(true);
            model.loadTrackingSheet.clearStatus();
        });
};

InventoryMovementsViewModel.prototype.buildData = function(data) {
    this.CurrentMovement(data);
    this.isNewMovement(false);
    this.isPageChanging = false;
};

ko.components.register('loading-screen', require('App/components/common/loading-screen/loading-screen')); 
ko.components.register('intra-warehouse-inventory-movement-details', inventoryMovementDetailsComponent);

ko.applyBindings(new InventoryMovementsViewModel());

page();

module.exports = InventoryMovementsViewModel;
