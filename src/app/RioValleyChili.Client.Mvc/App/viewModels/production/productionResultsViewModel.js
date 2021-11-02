var helpers = require('helpers/koHelpers'),
    ajaxStatusHelper = helpers.ajaxStatusHelper,
    productsService = require('services/productsService'),
    serviceCore = require('services/serviceCore'),
    warehouseLocationsService = require('services/warehouseLocationsService'),
    page = require('page');
require('App/koExtensions');
require('App/helpers/koValidationHelpers');

ko.components.register('modal-message', require('components/common/loading-screen/loading-screen'));
ko.components.register('inventory-by-lot-picker', require('components/inventory/inventory-by-lot-picker/inventory-by-lot-picker'));

var ProductionResultDetailsViewModelFactory = (function () {
    var packagingProductOptions = ko.observableArray([]);
    var productionLineOptions = ko.observableArray([]);
    var warehouseLocationOptions = ko.observableArray([]);

    return {
        init: init,

        getPackagingProductByKey: getPackagingProductByKey,
        getWarehouseLocationByKey: getWarehouseLocationByKey,
        getProductionLineByKey: getProductionLineByKey,
    };


    function init() {
        var productionResults = ko.observable();

        var options = {
          packagingProductOptions: packagingProductOptions,
          productionLineOptions: productionLineOptions,
          warehouseLocationOptions: warehouseLocationOptions,
        };

        var self = {
            productionResults: ko.computed({
                read: function () { return productionResults(); },
                write: function (values) {
                    var old = productionResults();
                    if (old) old.cleanup();

                    if (values == null) {
                        productionResults(null);
                        return;
                    }

                    values.ProductionLine = getProductionLineByKey(values.ProductionLineKey);

                    if (values.PackagingProduct)
                        values.PackagingProduct = getPackagingProductByKey(values.PackagingProduct.PackagingProductKey);

                    if (values.WarehouseLocation)
                        values.WarehouseLocation = getWarehouseLocationByKey(values.WarehouseLocation.LocationKey);

                    var model = new ProductionResults(values, options);
                    model.cancelEdit = cancelEdit;
                    productionResults(model);
                }
            }),
            packagingOptions: packagingProductOptions,
            productionLineOptions: productionLineOptions,
            warehouseLocationOptions: warehouseLocationOptions,
            isEditable: ko.observable(true) // TODO: Remove static value
        };
        var baseDispose = self.productionResults.dispose;
        self.productionResults.dispose = function() {
            var obj = self.productionResults();
            if (obj) obj.dispose();
            baseDispose();
        };

        loadPackagingProductOptions();
        loadProductionLineOptions();
        loadWarehouseLocationOptions();

        return self;
    }

    function cancelEdit(val) {
        getProductionLineByKey(val);
    }
    function loadPackagingProductOptions() {
        productsService.getPackagingProducts()
            .then(function (data) { packagingProductOptions(data); });
    }
    function loadProductionLineOptions() {
        serviceCore.ajax("/api/productionLines")
            .then(function (data) { productionLineOptions(data); });
    }
    function loadWarehouseLocationOptions() {
        warehouseLocationsService.getRinconWarehouseLocations()
            .then(function(data) { warehouseLocationOptions(data); });
    }
    function getProductionLineByKey(key) {
        return ko.utils.arrayFirst(productionLineOptions(), function(item) {
            return key === item.LocationKey;
        });
    }
    function getWarehouseLocationByKey(key) {
        return ko.utils.arrayFirst(warehouseLocationOptions(), function(item) {
            return item.LocationKey === key;
        });
    }
    function getPackagingProductByKey(key) {
        return ko.utils.arrayFirst(packagingProductOptions(), function (item) {
            return item.ProductKey === key;
        });
    }
}());

var ProductionResultsViewModelFactory = (function () {

    ko.applyBindings(create());

    function create() {
        var defaultShiftValue,
            defaultProductionStartDate,
            defaultProductionLineKey;

        var self = {
            modalMessage: ko.observable(),
            searchBatchNumber: ko.observable(),
            productionBatch: ko.observable(),
            recentEdits: ko.observableArray([]),

            packagingOptions: ko.observableArray([]),
            lineNumberOptions: ko.observableArray([]),
            productOptions: ko.observableArray([]),

            productionResultDetailsViewModel: ProductionResultDetailsViewModelFactory.init()
        };

        self.isRecentItems = ko.pureComputed(function() {
            var edits = self.recentEdits();

            return edits && edits.length;
        });

        self.cache = null;

        self.InventoryByLotExports = ko.observable();
        self.additionalInputItemsParameters = {
            PickedInventoryItems: [{}]
        };

        // commands
        self.cancelEditsCommand = ko.command({
            execute: function() {
                self.InventoryByLotExports().reset();
                self.getProductionResultsCommand.execute(self.cache.ProductionBatchKey);
            },
            canExecute: function() {
                var productionResults = self.productionResultDetailsViewModel.productionResults();
                return productionResults;
            }
        });
        self.editProductionResultsCommand = ko.command({
            execute: function() {
                var productionResults = self.productionResultDetailsViewModel.productionResults();

                return productionResults && productionResults.isEditing(true);
            },
            canExecute: function() {
                var productionResults = self.productionResultDetailsViewModel.productionResults();

                return productionResults && !productionResults.isEditing();
            }
        });
        self.searchLotCommand = ko.asyncCommand({
            execute: function (complete) {
                if (lossOfData()) {
                    showUserMessage("Do you want to discard your changes?", {
                        description: "You are about to navigate away from the record before saving your changes. Do you want do discard your changes?",
                        type: 'yesno',
                        onYesClick: function() {
                            executeSearch();
                        },
                        onNoClick: function () {
                            self.searchBatchNumber(null);
                            complete();
                        }
                    });
                    return;
                }
                function lossOfData() {
                    var productionResults = self.productionResultDetailsViewModel.productionResults();
                    return productionResults && productionResults.isEditing();
                }

                function executeSearch() {
                    self.modalMessage('Searching for Production Batch...');
                    var lotNumber = self.searchBatchNumber.formattedLot();
                    if (self.productionBatch() && lotNumber === self.productionBatch().LotNumber) return;

                    self.productionBatch(null);
                    self.searchLotCommand.indicateWorking();
                    self.productionResultDetailsViewModel.productionResults(null);

                    getProductionBatchByLotKey(lotNumber, {
                        successCallback: function (data) {
                            self.searchLotCommand.clearStatus();
                            self.productionBatch(new ProductionBatch(data));
                            self.getProductionResultsCommand.execute(data.ProductionBatchKey);
                            self.cache = data;
                            self.searchBatchNumber(null);
                        },
                        errorCallback: function () {
                            self.searchLotCommand.indicateFailure();
                        },
                        completeCallback: function () {
                            self.modalMessage(null);
                            complete();
                        },
                    });
                }

                executeSearch();
            },
            canExecute: function (isExecuting) {
                var searchLot = self.searchBatchNumber();
                var ok = !isExecuting && searchLot;
                var batch = self.productionBatch();
                return ok && (!batch || batch.LotNumber !== searchLot);
            }
        });
        self.getDefaultInventorySelectionsForLot = function(lot) {
            var keys = [
                    'defaultLocationkey',
                    'defaultPackagingKey',
                    'defaultTreatmentKey'
                ],
                defaults = {};

            ko.utils.arrayForEach(keys, function(item) {
                var data = sessionStorage.getItem('productionResults_' + lot + '_' + item);

                defaults[item] = data !== "undefined" ?
                    data :
                    null;
            });

            return defaults;
        };
        self.setDefaultInventorySelectionsForLot = function(lot, data) {
            var keys = [
                    'defaultLocationkey',
                    'defaultPackagingKey',
                    'defaultTreatmentKey'
                ];

            ko.utils.arrayForEach(keys, function(item) {
                sessionStorage.setItem('productionResults_' + lot + "_" + item,
                                       data[item]);
            });
        };

        self.getProductionResultsCommand = ko.asyncCommand({
            execute: function (batchKey, complete) {
                self.modalMessage('Searching for production results...');
                self.getProductionResultsCommand.indicateWorking();
                getProductionResultsAsync(batchKey, {
                    successCallback: function (data) {
                        self.getProductionResultsCommand.indicateSuccess();
                        self.productionResultDetailsViewModel.productionResults(data);
                    },
                    errorCallback: function (xhr, status, message) {
                        if (xhr.status === 404) {
                            initializeNewProductionResults();
                            self.getProductionResultsCommand.clearStatus();
                        } else {
                            self.getProductionResultsCommand.indicateFailure();
                        }
                    },
                    completeCallback: function () {
                        self.modalMessage(null);
                        complete();
                    },
                });
            },
            canExecute: function (isExecuting) { return !isExecuting; }
        });
        self.saveProductionResultsCommand = ko.asyncCommand({
            execute: function (complete) {
                try {
                    var vm = self.productionResultDetailsViewModel,
                        model = vm.productionResults();

                    if (!model || !model.isValid() || !self.InventoryByLotExports().isValid()) {
                        showUserMessage("The Production Results can not be saved because of validation errors.");
                        complete();
                        return;
                    }

                    self.saveProductionResultsCommand.indicateWorking();
                    self.modalMessage("Saving production results...");
                    var key = model.ProductionResultKey();
                    var values = buildProductionResultsDto();

                    if (key) {
                        updateProductionResults(key, values, {
                            successCallback: function() {
                                successCallback("Production Results were saved successfully.");
                            },
                            errorCallback: function(xhr, status, message) { errorCallback("Failed to save Production Results changes.", message); },
                            completeCallback: completeCallback,
                        });
                    } else {
                        createProductionResultsAsync(values, {
                            successCallback: function() {
                                successCallback("Production Results were created successfully.");
                                defaultShiftValue = values.ShiftKey;
                                defaultProductionLineKey = values.ProductionLine;
                                defaultProductionStartDate = values.ProductionBeginDate;
                            },
                            errorCallback: function(xhr, status, message) { errorCallback("Failed to create Production Results.", message); },
                            completeCallback: completeCallback,
                        });
                    }
                } catch (ex) {
                    complete();
                    showUserMessage('Save failed due to error on page.', { description: ex });
                }

                function successCallback(message) {
                    try {
                        self.saveProductionResultsCommand.clearStatus();
                        storeAsRecent(self.productionBatch().LotNumber);
                        self.InventoryByLotExports().saveData();
                    } catch (ex) { }
                    self.productionBatch(null);
                    self.productionResultDetailsViewModel.productionResults(null);
                    showUserMessage(message);
                }
                function errorCallback(title, description) {
                    self.saveProductionResultsCommand.indicateFailure();
                    showUserMessage(title, { description: description });
                }
                function completeCallback() {
                    complete();
                    self.modalMessage(null);
                }
            },
            canExecute: function (isExecuting) {
                var productionResults = self.productionResultDetailsViewModel.productionResults();
                return !isExecuting &&
                    productionResults;
            }
        });

        // init
        ajaxStatusHelper(self.getProductionResultsCommand);
        ajaxStatusHelper(self.searchLotCommand);
        ajaxStatusHelper(self.saveProductionResultsCommand);
        self.searchBatchNumber.extend({ lotKey: true });

        self.recentEdits(getRecent() || []);

        // functions
        function getRecent() {
            return JSON.parse(sessionStorage.getItem('productionResultsRecent'));
        }

        function storeAsRecent(lot) {
           var recentlyEdited = getRecent();

           if (!lot) {
               throw new Error('Must provide lot number to store as recent edit');
           }
           if (recentlyEdited) {
               deleteDuplicate(lot);
               recentlyEdited.unshift(lot);
           } else {
               recentlyEdited = [lot];
           }

           sessionStorage.setItem('productionResultsRecent', JSON.stringify(recentlyEdited));
           self.recentEdits(recentlyEdited);

           function deleteDuplicate(lot) {
               var index = recentlyEdited.indexOf(lot);

               if (index > -1) {
                   recentlyEdited.splice(index, 1);
               }
           }
        }

        return self;

        // private functions
        function updateProductionResults(key, values, callbackOptions) {
            $.ajax({
                url: '/api/productionResults/' + key,
                data: ko.toJSON(values),
                contentType: 'application/json',
                type: 'PUT',
                success: callbackOptions.successCallback,
                error: callbackOptions.errorCallback,
                complete: callbackOptions.completeCallback,
            });
        }
        function createProductionResultsAsync(values, callbackOptions) {
            $.ajax({
                url: '/api/productionResults/',
                data: ko.toJSON(values),
                contentType: 'application/json',
                type: 'POST',
                success: callbackOptions.successCallback,
                error: callbackOptions.errorCallback,
                complete: callbackOptions.completeCallback,
            });
        }
        function initializeNewProductionResults() {
            var batch = self.productionBatch() || {};

            self.productionResultDetailsViewModel.productionResults({
                ProductionBatchKey: batch.ProductionBatchKey,
                ProductionShiftKey: defaultShiftValue,
                ProductionLineKey: defaultProductionLineKey,
                ProductionStartDate: defaultProductionStartDate,
                ProductionEndDate: defaultProductionStartDate,
            });

            var newResults = self.productionResultDetailsViewModel.productionResults(),
                baseCommand = newResults.addNewItemCommand;

            newResults.addNewItemCommand = ko.command({
                execute: function() {
                    var newItem = baseCommand.execute();

                    setupNewItem(newItem);
                },
                canExecute: baseCommand.canExecute,
            });

            function setupNewItem(item) {
                if (newResults.ResultItems().length === 1) {
                    item.PackagingProduct(ProductionResultDetailsViewModelFactory.getPackagingProductByKey(batch.PackagingProductKey));
                    //todo: set default line number

                    newResults.ProductionLine.subscribe(function(val) {
                        var defaultLocation = getDefaultLocation.call(self, val);
                        item.WarehouseLocation(defaultLocation);
                    });
                }

                item.isComplete = ko.computed(function () {
                    return this.PackagingProduct() &&
                        this.Quantity() > 0 &&
                        this.WarehouseLocation();
                }, item);

                item.isCompleteSubscription = item.isComplete.subscribe(function (val) {
                    if (!val) return;
                    var items = newResults.ResultItems();
                    if (isLastItem()) newResults.addNewItemCommand.execute();

                    function isLastItem() {
                        return ko.utils.arrayIndexOf(items, item) === (items.length - 1);
                    }
                });

                newResults.isEditing(true);

                var oldDispose = item.dispose;
                item.dispose = function () {
                    item.isCompleteSubscription.dispose();
                    item.isComplete.dispose();
                    if (oldDispose) oldDispose();
                };
            }

            newResults.addNewItemCommand.execute();
        }
        function buildProductionResultsDto() {
            function buildResultItemsDto(productionResults) {
                var items = [];

                ko.utils.arrayForEach(productionResults.ResultItems(), function (i) {
                    if (i.Quantity() > 0 && i.isValid()) {
                        items.push(i.toDto());
                    }
                });

                return items;
            }
            function buildAdditionalInputMaterialsDto() {
                var items = [],
                    data = self.InventoryByLotExports(),
                    pickedItems = data.PickedInventoryItems();

                if (pickedItems && pickedItems.length > 0) {
                    if (data.isValid()) {
                        ko.utils.arrayMap(pickedItems, function (item) {
                            var quantityPicked = Number(item.QuantityPicked()) || 0;
                            if (item.InventoryKey && quantityPicked !== 0) {
                                items.push({
                                    InventoryKey: item.InventoryKey(),
                                    QuantityPicked: quantityPicked
                                });
                            }
                        });
                    } else {
                        throw new Error('Invalid data for additional inputs.');
                    }
                }

                return items;
            }

            var model = self.productionResultDetailsViewModel.productionResults();
            return ko.toJS({
                InventoryItems: buildResultItemsDto(model),
                PickedInventoryItemChanges: buildAdditionalInputMaterialsDto(),
                ProductionResultKey: model.ProductionResultKey,
                ProductionShiftKey: model.ProductionShift,
                ProductionLineKey: model.ProductionLineKey,
                ProductionStartTimestamp: model.ProductionStartDateTime,
                ProductionEndTimestamp: model.ProductionEndDateTime,
                ProductionBatchKey: model.ProductionBatchKey,
            });
        }
    }


    //***************************
    // private static functions
    function getProductionBatchByLotKey(lotKey, callbackOptions) {
        $.ajax("/api/productionBatches/lot/" + lotKey, {
            contentType: "application/json",
            type: "GET",
            statusCode: {
                200: callbackOptions.successCallback,
                404: function () {
                    showUserMessage("Batch number not found.", {
                        description: "There were no records found for the batch number <strong>" + lotKey + "</strong>. Please check the batch number for typing errors."
                    });
                },
            },
            error: function (xhr, status, message) {
                if (callbackOptions.errorCallback) callbackOptions.errorCallback();
                showUserMessage("Unable to get production batch information.", { description: message });
            },
            complete: callbackOptions.completeCallback
        });
    }
    function getProductionResultsAsync(batchKey, callbackOptions) {
        $.ajax('/api/productionresults/' + batchKey, {
            contentType: 'application/json',
            type: 'GET',
            statusCode: {
                200: callbackOptions.successCallback,
            },
            error: callbackOptions.errorCallback,
            complete: callbackOptions.completeCallback,
        });
    }
    function getDefaultLocation(line) {
        var locationOptions = this.productionResultDetailsViewModel.warehouseLocationOptions();
        if (!locationOptions.length) return null;
        var locationExp = new RegExp("^P(0?)" + line.Row + "$");
        var location = ko.utils.arrayFirst(locationOptions, function (loc) {
            return locationExp.test(loc.Description);
        });
        return location;
    }
}());


function ProductionBatch(values) {
    var self = {
        LotNumber: values.OutputLotKey,
        BatchType: values.WorkType.Description,
        TargetBatchWeight: values.BatchTargetWeight,
        TargetBatchWeightDisplay: values.BatchTargetWeight + " lbs",
        ProductName: values.ChileProductName,
        ProductKey: values.ChileProductKey,
        ProductionBatchKey: values.ProductionBatchKey,
        PackagingProductName: values.PackagingProduct.ProductName,
        PackagingProductKey: values.PackagingProduct.ProductKey,
    };
    return self;
}
function ProductionResults(values, options) {
    values = values || {};
    var self = {
        options: options,
        InventoryByLotInput: ko.observableArray(),
        ProductionBatchKey: values.ProductionBatchKey,
        ProductionResultKey: ko.observable(values.ProductionResultKey),
        ProductionLine: ko.observable(values.ProductionLine).extend({ required: true }),
        ProductionShift: ko.observable(values.ProductionShiftKey).extend({ required: true }),

        ProductionStartDate: ko.observableDate(values.ProductionStartDate).extend({ required: true }),
        ProductionStartTime: ko.observableTime(values.ProductionStartDate).extend({ required: true }),

        ProductionEndDate: ko.observableDate(values.ProductionEndDate).extend({ required: true }),
        ProductionEndTime: ko.observableTime(values.ProductionEndDate).extend({ required: true }),

        DateEntered: values.DateTimeEntered,
        CreatedByUser: values.User,
        ResultItems: ko.observableArray(parseResultItems()),
        isEditing: ko.observable(false),

        //methods
        cleanup: cleanup,
    };

    // subscribers
    var productionStartSubscription = self.ProductionStartDate.subscribe(function (val) {
        if (self.ProductionEndDate()) return;

        self.ProductionEndDate(val);
    });
    var productionShiftSubscription = self.ProductionShift.subscribe(function(val) {
        var formattedVal = val.trim().toUpperCase();
        if (formattedVal !== val) self.ProductionShift(formattedVal);
    });

    // computed properties
    self.ProductionLineDescription = ko.computed(function() {
        var productionLine = self.ProductionLine();
        return productionLine ? productionLine.GroupName + ' ' + productionLine.Row : null;
    });
    self.ProductionStartDateTime = ko.computed(function() {
        return combineDateTimeFields(self.ProductionStartDate, self.ProductionStartTime);
    });
    self.ProductionStartDateTime.formattedDate = ko.computed(function () {
        var startDateTime = self.ProductionStartDateTime();
        return startDateTime ?
            startDateTime.format('ddd, mmm d, yyyy HH:MM') :
            null;
    });
    self.ProductionEndDateTime = ko.computed(function() {
        return combineDateTimeFields(self.ProductionEndDate, self.ProductionEndTime);
    }).extend({ min: self.ProductionStartDateTime });
    self.ProductionEndDateTime.formattedDate = ko.computed(function () {
        var endDateTime = self.ProductionEndDateTime();
        return endDateTime ?
            endDateTime.format('ddd, mmm d, yyyy HH:MM') :
            null;
    });
    self.TotalProductionTime = ko.computed(function () {
        var end = self.ProductionEndDateTime();
        var start = self.ProductionStartDateTime();
        if (!end || !start) return '0h 0m';
        return formatDateDiff(end, start);

        function formatDateDiff(d1, d2) {
            var dif = (new Date(d1) - new Date(d2)) / 1000;
            return secondsToHM(dif);
        }

        function secondsToHM(secs) {
            var hours = Math.floor(secs / 3600);
            var minutes = Math.floor((secs - (hours * 3600)) / 60);

            var time = hours + 'h ' + minutes + 'm';
            return time;
        }
    }, self);
    self.TotalBatchWeightDisplay = ko.computed(function() {
        var total = 0;
        ko.utils.arrayForEach(this(), function(item) {
            total += ko.utils.unwrapObservable(item.Weight);
        });
        return total + ' lbs';
    }, self.ResultItems);
    self.ProductionLineKey = ko.computed(function() {
        var val = this();
        return (val && val.LocationKey) ? val.LocationKey : null;
    }, self.ProductionLine);

    // commands
    self.addNewItemCommand = ko.command({
        execute: function () {
            var values = {},
                items = self.ResultItems(),
                item;

            if (items.length > 0) {
                values.WarehouseLocation = items[items.length - 1].WarehouseLocation();
            }

            item = new ProductionResultItem(values, options);
            self.ResultItems.push(item);

            return item;
        },
        canExecute: function() { return true; }
    });
    self.removeItemCommand = ko.command({
        execute: function(item) {
            if (!item) return;
            var items = self.ResultItems();
            var index = ko.utils.arrayIndexOf(items, item);

            if (index > -1) {
                self.ResultItems.splice(index, 1);

                if ( self.ResultItems().length === 0 ) {
                  self.addNewItemCommand.execute();
                }
            }
        }
    });

    var errors = ko.validation.group(self, { deep: true });
    self.isValid = ko.computed(function () {
        errors.showAllMessages();

        var errs = errors();

        return errs == null || errs.length === 0;
    });

    return self;

    function combineDateTimeFields(date, time) {
        var dateVal = Date.parse(ko.utils.unwrapObservable(date));
        var timeVal = Date.parse(ko.utils.unwrapObservable(time));

        if (!dateVal || !timeVal) return '';

        var d = new Date(dateVal);
        if (!d.getTime) return '';

        var t = new Date(timeVal);
        if (!t.getTime) return '';

        return new Date(d.format('mm/dd/yyyy') + ' ' + t.format('HH:MM:ss'));
    }
    function parseResultItems() {
        return ko.utils.arrayMap(values.ResultItems, function (item) {
            if (item && item.Treatment) {
                item.TreatmentKey = item.Treatment.TreatmentKey;
            }
            return new ProductionResultItem(item, options);
        }) || [];
    }
    function cleanup() {
        productionShiftSubscription.dispose();
        productionStartSubscription.dispose();
        ko.utils.arrayForEach(ko.unwrap(self.ResultItems), function(i) { i.cleanup(); });
    }
}
function ProductionResultItem(values, options) {
    values = values || {};
    var self = {
        PackagingProduct: ko.observable(),
        TreatmentKey: ko.observable(values.TreatmentKey).extend({ treatmentType: true }),
        Quantity: ko.numericObservable(values.Quantity).extend({ min: 1, required: { onlyIf: isActive } }),
        WarehouseLocation: ko.observable(values.WarehouseLocation && values.WarehouseLocation.LocationKey).extend({ required: { onlyIf: isActive } }),

        // functions
        cleanup: cleanup,
        toDto: buildResultItemDto,
    };

  self.isActive = ko.pureComputed(function() {
    return !isNaN(Number(self.Quantity()));
  });

    (function() {
      var packagingMatch = ko.utils.arrayFirst(options.packagingProductOptions(), function(opt) {
        if (opt.ProductKey === (values.PackagingProduct && values.PackagingProduct.ProductKey)) {
          return true;
        }
      });

      var warehouseMatch = ko.utils.arrayFirst(options.warehouseLocationOptions(), function(opt) {
        if (opt.LocationKey === (values.WarehouseLocation && values.WarehouseLocation.LocationKey)) {
          return true;
        }
      });


      self.PackagingProduct(packagingMatch);
      self.WarehouseLocation(warehouseMatch);
    })();

    self.PackagingProductName = ko.computed(function() {
        var packaging = self.PackagingProduct();
        return packaging ?
            packaging.ProductName :
            '';
    });
    self.WarehouseLocationName = ko.computed(function() {
        var warehouseLocation = self.WarehouseLocation();
        return warehouseLocation ?
            warehouseLocation.Description :
            '';
    });
    self.Weight = ko.computed(function() {
        var packaging = self.PackagingProduct();
        return packaging ?
            packaging.Weight * (self.Quantity() || 0) :
            0;
    });
    self.WeightDisplay = ko.computed(function() {
        return self.Weight() + ' lbs';
    });

    var errors = ko.validation.group(self, { deep: true });
    self.isValid = ko.computed(function() {
        var valid = errors();

        return valid == null || valid.length === 0;
    });

    return self;

    function buildResultItemDto() {
        return {
            PackagingKey: self.PackagingProduct().ProductKey,
            LocationKey: self.WarehouseLocation().LocationKey,
            Quantity: self.Quantity(),
            InventoryTreatmentKey: self.TreatmentKey(),
        };
    }
    function isActive() {
        return self && self.PackagingProduct();
    }
    function cleanup() {
        errors.dispose();
    }
}

