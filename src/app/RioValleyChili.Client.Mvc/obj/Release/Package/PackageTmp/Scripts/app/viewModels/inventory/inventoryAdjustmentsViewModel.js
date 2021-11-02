var NewInventoryAdjustmentViewModelFactory = (function (ko) {
    var warehouseLocationsCache = {};

    return {
        create: create
    };

    function create() {
        var model = {
            warehouseOptions: ko.observableArray([]),
            warehouseLocationOptions: ko.observableArray([]),
            packagingOptions: ko.observableArray([]),

            Comment: ko.observable(),
            adjustmentItems: ko.observableArray([]),

            // methods
            validateModel: isValid,
            toDto: buildDto,
        };

        // computed properties
        model.totalAdjustmentWeight = ko.computed(function () {
            var sum = 0;
            ko.utils.arrayForEach(model.adjustmentItems(), function (item) {
                sum += item.AdjustmentWeight();
            });
            return sum;
        }, model);

        // commands
        model.loadWarehouseOptionsComand = ko.asyncCommand({
            execute: function (complete) {
                model.loadWarehouseOptionsComand.indicateWorking();
                rvc.api.warehouse.getWarehouses({
                    successCallback: function (data) {
                        model.loadWarehouseOptionsComand.clearStatus();
                        model.warehouseOptions(data);
                    },
                    errorCallback: function (xhr, status, message) {
                        model.loadWarehouseOptionsComand.indicateFailure();
                        showUserMessage("Error loading warehouse locations.", { description: message });
                    },
                    completeCallback: complete,
                });
            },
            canExecute: function (isExecuting) {
                return !isExecuting;
            }
        });
        model.loadPackagingOptionsComand = ko.asyncCommand({
            execute: function (complete) {
                rvc.api.products.getPackagingProducts({
                    successCallback: function (data) {
                        model.packagingOptions(data);
                    },
                    errorCallback: function (xhr, status, message) {
                        showUserMessage("Error loading packaging products.", { description: message });
                    },
                    completeCallback: complete,
                });
            },
            canExecute: function (isExecuting) {
                return !isExecuting;
            }
        });
        model.removeItemCommand = ko.command({
            execute: removeItem,
        });

        // init
        ajaxStatusHelper.init(model.loadWarehouseOptionsComand);
        
        model.loadWarehouseOptionsComand.execute();
        model.loadPackagingOptionsComand.execute();
        addNewAdjustmentItem();

        model.postAdjustmentAsync = postInventoryAdjustment.bind(model);

        return model;

        // private functions
        function addNewAdjustmentItem() {
            var adjustmentItem = new InventoryAdjustmentItemViewModel({
                warehouseOptions: model.warehouseOptions
            });
            adjustmentItem.isValid.subscribe(itemIsValidChanged);
            model.adjustmentItems.push(adjustmentItem);

            function itemIsValidChanged(val) {
                if (!val) return;
                if (isLastOutputItem()) {
                    addNewAdjustmentItem();
                }

                function isLastOutputItem() {
                    return model.adjustmentItems().length === (ko.utils.arrayIndexOf(model.adjustmentItems(), adjustmentItem) + 1);
                }
            }
        }
        function isValid() {
            var itemsToValidate = getActiveItemsUnwrapped();
            return itemsToValidate.length && ko.utils.arrayFirst(itemsToValidate, function (item) {
                return item.isValid === false;
            }) === null;
        }
        function removeItem(itemToRemove) {
            var index = ko.utils.arrayIndexOf(model.adjustmentItems(), itemToRemove);
            if (index === -1) return;
            var isLastItem = index === (model.adjustmentItems().length - 1);
            model.adjustmentItems.splice(index, 1);
            if (isLastItem) {
                addNewAdjustmentItem();
            }
        }
        function buildDto() {
            var items = ko.utils.arrayMap(getActiveItemsUnwrapped(), function (item) {
                return {
                    LotKey: item.LotKey,
                    WarehouseLocationKey: item.WarehouseLocation.WarehouseLocationKey,
                    PackagingProductKey: item.Packaging.ProductKey,
                    TreatmentKey: item.TreatmentKey,
                    ToteKey: item.ToteKey,
                    Adjustment: item.AdjustmentQuantity,
                };
            });

            return {
                InventoryAdjustments: items,
                Comment: model.Comment(),
            };
        }
        function getActiveItemsUnwrapped() {
            // toJS prevents change notifications
            var activeItems = ko.toJS(model.adjustmentItems);
            activeItems.pop();
            return activeItems;
        }
    }

    // models
    function InventoryAdjustmentItemViewModel(options) {
        if (!(this instanceof arguments.callee))
            return new InventoryAdjustmentItemViewModel(options);

        var model = this;

        this.LotKey = ko.observable();
        this.ProductName = ko.observable();
        this.Warehouse = ko.observable();
        this.WarehouseLocation = ko.observable();
        this.Packaging = ko.observable();
        this.Treatment = ko.observable();
        this.AdjustmentQuantity = ko.numericObservable(0).extend({ required: true });
        this.ToteKey = ko.observable();

        this.isValidLot = ko.observable(false);
        this.availableInventory = ko.observableArray([]);

        this.warehouseOptions = ko.observableArray([]);
        this.warehouseLocationOptions = ko.observableArray([]);
        this.packagingOptions = ko.observableArray([]);
        this.treatmentOptions = ko.observableArray([]);
        
        // computed properties
        model.WarehouseKey = ko.computed(function () { return this.Warehouse() ? this.Warehouse().WarehouseKey : null; }, model);
        model.WarehouseLocationKey = ko.computed(function() {
            return this.WarehouseLocation() ? this.WarehouseLocation().WarehouseLocationKey : null;
        }, model).extend({ required: true });
        model.TreatmentKey = ko.computed(function() {
            var treatment = this.Treatment();
            return treatment
                ? treatment.TreatmentKey || treatment
                : null;
        }, model).extend({ treatmentType: true });
        model.currentInventorySelection = ko.computed(function () {
            if (!model.WarehouseLocationKey()
                || !model.Packaging()
                || !model.TreatmentKey()) return null;

            return ko.utils.arrayFirst(this.availableInventory(), function (item) {
                return model.WarehouseLocationKey() === item.WarehouseLocationKey
                    && model.Packaging().ProductKey === item.PackagingProductKey
                    && model.TreatmentKey() === item.TreatmentKey
                    && (model.ToteKey() || '') === item.ToteKey;
            });
        }, model);

        var validated = ko.validatedObservable(model);
        model.isValid = ko.computed(function () {
            return validated.isValid();
        });
        model.CurrentQuantity = ko.computed(function() {
            var current = this.currentInventorySelection();
            return current ? current.Quantity : '0';
        }, model);
        model.NewQuantity = ko.computed(function () {
            var current = this.currentInventorySelection();
            return current 
                ? current.Quantity + (this.AdjustmentQuantity() || 0)
                : this.AdjustmentQuantity() || 0;
        }, model).extend({ min: 0 });
        model.AdjustmentWeight = ko.computed(function () {
            var qty = this.AdjustmentQuantity();
            var pkg = this.Packaging();
            if (!qty || !pkg) return '0 lbs';
            return (qty * pkg.Weight) + ' lbs';
        }, model);
        model.NewWeight = ko.computed(function () {
            return this.Packaging()
                ? (this.NewQuantity() * this.Packaging().Weight) + ' lbs'
                : '0 lbs';
        }, model);

        // commands
        model.findLotCommand = ko.asyncCommand({
            execute: function(complete) {
                model.findLotCommand.indicateWorking();
                model.isValidLot(null);
                loadLotInventoryAsync(model.LotKey(), complete);
            },
            canExecute: function(isExecuting) {
                return !isExecuting && model.LotKey() && model.LotKey.isComplete();
            }
        });

        model.getWarehouseLocationsCommand = ko.asyncCommand({
            execute: function (complete) {
                var warehouseKey = model.WarehouseKey();
                if (warehouseLocationsCache[warehouseKey]) {
                    model.warehouseLocationOptions(warehouseLocationsCache[warehouseKey]);
                    complete();
                    return;
                }
                
                model.getWarehouseLocationsCommand.indicateWorking();
                rvc.api.warehouse.getWarehouseLocations(warehouseKey, {
                    successCallback: function (data) {
                        model.getWarehouseLocationsCommand.indicateSuccess();
                        warehouseLocationsCache[warehouseKey] = data;
                        model.warehouseLocationOptions(data);
                    },
                    errorCallback: function (xhr, status, message) {
                        model.getWarehouseLocationsCommand.indicateFailure();
                        showUserMessage("Unable to get warehouse locations", { description: message });
                    },
                    completeCallback: complete
                });
            },
            canExecute: function(isExecuting) {
                return !isExecuting && model.WarehouseKey();
            }
        });

        // subscribers
        model.LotKey.subscribe(function (val) {
            model.availableInventory([]);
        });
        model.WarehouseKey.subscribe(function(val) {
            if (val) model.getWarehouseLocationsCommand.execute();
            else model.warehouseLocationOptions([]);
        });

        // init
        ajaxStatusHelper.init(model.findLotCommand);
        ajaxStatusHelper.init(model.getWarehouseLocationsCommand);
        model.LotKey.extend({ lotKey: model.findLotCommand.execute });

        return model;

        function loadLotInventoryAsync(lotKey, completeCallback) {
            rvc.api.inventory.getInventoryByLot(lotKey, {
                successCallback: function (data) {
                    if (!data.InventoryItems.length) {
                        rvc.api.lots.getLotByKey(lotKey, {
                            successCallback: function (lotData) {
                                model.isValidLot(true);
                                model.findLotCommand.indicateSuccess();
                                model.ProductName(lotData.LotSummary.Product.ProductName);
                                buildInventoryOptions.call(model, []);
                            },
                            errorCallback: function (xhr, result, message) {
                                model.isValidLot(false);
                                showUserMessage("Unable to get lot \"" + lot + "\".", { description: message });
                                model.findLotCommand.indicateFailure();
                                complete();
                            },
                        });
                        return;
                    }

                    model.ProductName(data.InventoryItems[0].ProductName);
                    model.findLotCommand.indicateSuccess();
                    model.isValidLot(true);
                    buildInventoryOptions.call(model, data.InventoryItems);
                },
                errorCallback: function (xhr, result, message) {
                    showUserMessage("Unable to get inventory for lot \"" + lotKey + "\".", { description: message });
                    model.findLotCommand.indicateFailure();
                },
                completeCallback: completeCallback,
            });
        }
    }

    // private static functions
    function buildInventoryOptions(inventoryItems) {
        var model = this;
        inventoryItems = inventoryItems || [];
        var warehouseOptions = [];
        var warehouseOptionsCache = {};

        ko.utils.arrayMap(inventoryItems, function (inventoryItem) {
            var warehouse = warehouseOptionsCache[inventoryItem.WarehouseKey];
            if (!warehouse) {
                warehouse = {
                    WarehouseKey: inventoryItem.WarehouseKey,
                    WarehouseName: inventoryItem.WarehouseName,
                    WarehouseLocationsCache: {},
                    WarehouseLocations: [],
                };
                warehouseOptions.push(warehouse);
                warehouseOptionsCache[inventoryItem.WarehouseKey] = warehouse;
                preloadWarehouseLocations(inventoryItem.WarehouseKey);
            }

            var warehouseLocation = warehouse.WarehouseLocationsCache[inventoryItem.WarehouseLocationKey];
            if (!warehouseLocation) {
                warehouseLocation = {
                    WarehouseLocationKey: inventoryItem.WarehouseLocationKey,
                    WarehouseLocationName: inventoryItem.WarehouseLocationName,
                    PackagingOptionsCache: {},
                    PackagingOptions: [],
                };
                warehouse.WarehouseLocations.push(warehouseLocation);
                warehouse.WarehouseLocationsCache[inventoryItem.WarehouseLocationKey] = warehouseLocation;
            }

            var packaging = warehouseLocation.PackagingOptionsCache[inventoryItem.PackagingKey];
            if (!packaging) {
                packaging = {
                    ProductKey: inventoryItem.PackagingProductKey,
                    ProductName: inventoryItem.PackagingProductName,
                    Weight: inventoryItem.PackagingCapacity,
                    TreatmentOptions: [],
                    TreatmentOptionsCache: {},
                };
                warehouseLocation.PackagingOptionsCache[inventoryItem.PackagingKey] = packaging;
                warehouseLocation.PackagingOptions.push(packaging);
            }

            var treatment = packaging.TreatmentOptionsCache[inventoryItem.TreatmentKey];
            if (!treatment) {
                treatment = {
                    TreatmentKey: inventoryItem.TreatmentKey,
                    TreatmentNameShort: inventoryItem.TreatmentNameShort,
                    ToteOptionsCache: {},
                    ToteOptions: [],
                };
                packaging.TreatmentOptionsCache[inventoryItem.TreatmentKey] = treatment;
                packaging.TreatmentOptions.push(treatment);
            }

            if (inventoryItem.ToteKey) {
                var tote = treatment.ToteOptionsCache[inventoryItem.ToteKey];
                if (!tote) {
                    packaging.ToteOptionsCache[inventoryItem.ToteKey] = inventoryItem.ToteKey;
                    packaging.ToteOptions.push(inventoryItem.ToteKey);
                }
            }
        });
        model.warehouseOptions(warehouseOptions);
        model.availableInventory(inventoryItems);
    }
    function preloadWarehouseLocations(warehouseKey) {
        rvc.api.warehouse.getWarehouseLocations(warehouseKey, {
            successCallback: function (data) {
                warehouseLocationsCache[warehouseKey] = data;
            },
        });
    }
    function postInventoryAdjustment(options) {
        var dto = this.toDto();
        $.ajax({
            url: '/api/inventoryadjustments',
            data: ko.toJSON(dto),
            headers: { __RequestVerificationToken: options.antiForgeryToken },
            contentType: 'application/json',
            type: 'POST',
            success: function(data) {
                showUserMessage("The Inventory Adjustment was created successfully!");
                if (options.successCallback) options.successCallback(data);
            },
            error: function(xhr, status, message) {
                showUserMessage("The Inventory Adjustment failed to save.", { description: message });
                if (options.errorCallaback) options.errorCallaback(xhr, status, message);
            },
            complete: options.completeCallback,
        });
    } 
    
}(ko, rvc, ajaxStatusHelper));

var InventoryAdjustmentsViewModelFactory = (function ($, ko, ajaxStatusHelper, notebookFactory) {
    var urlBase = '/api/inventoryadjustments';

    var factory = {
        init: init
    };


    return factory;  

    function init(options) {
        if (!options || !options.antiForgeryToken) throw new Error("The anti-forgery token is required.");
        var antiForgeryToken = options.antiForgeryToken;
        var lastApiUrl = ko.observable();
        var dataPager = PagedDataHelper.init({
            pageSize: 30,
            urlBase: urlBase
        });
        var adjustments = ko.observableArray([]);
        var scrollToNextSummaryItem = ko.observable(false);

        var self = {
            // adjustment summaries
            lotFilter: ko.observable(),
            isResultSetFiltered: ko.observable(),
            allDataLoaded: ko.observable(false),
            dateFilterStart: ko.observable().extend({ isoDate: "m/d/yyyy" }),
            dateFilterEnd: ko.observable().extend({ isoDate: "m/d/yyyy" }),

            // create adjustment
            newAdjustment: ko.observable(),

            // adjustment details
            selectedItem: ko.observable(),
            
            // methods
            animateSummaryItem: rvc.utils.animateNewItem({
                scrollToItem: scrollToNextSummaryItem,
                afterScrollCallback: function() {
                    scrollToNextSummaryItem(false);
                }
            })
        };

        self.lotFilter.extend({ lotKey: true });

        // computed properties
        self.filteredItems = ko.computed(function () {
            return adjustments();
        });
        self.hasFilterParams = ko.computed(function () {
            return self.lotFilter() || self.dateFilterStart() || self.dateFilterEnd();
        });
        
        var apiAdjustmentSummariesUrl = ko.computed(function () {
            var params = "";
            var lot = self.lotFilter(),
                startDate = self.dateFilterStart.formattedDate(),
                endDate = self.dateFilterEnd.formattedDate();

            if (lot) {
                params += "lotKey=" + lot;
            }

            if (startDate) {
                if (params.length) params += "&";
                params += "beginningDateFilter=" + startDate + "&endingDateFilter=" + endDate;
            }

            if (params) params = "?" + params;

            return urlBase + params;
        }, self);

        // commands
        self.loadMoreAdjustmentsCommand = ko.asyncCommand({
            execute: function (completed) {
                self.loadMoreAdjustmentsCommand.indicateWorking();
                getAdjustmentSummariesAsync({
                    successCallback: function() {
                        self.loadMoreAdjustmentsCommand.clearStatus();
                    },
                    errorCallback: function() {
                        self.loadMoreAdjustmentsCommand.indicateFailure();
                    },
                    completeCallback: completed,
                });
            },
            canExecute: function (isExecuting) {
                return !isExecuting && !self.allDataLoaded();
            }
        });
        self.applyFiltersCommand = ko.asyncCommand({
            execute: function (complete) {
                self.applyFiltersCommand.indicateWorking();
                getAdjustmentSummariesAsync({
                    successCallback: function () {
                        self.applyFiltersCommand.clearStatus();
                    },
                    errorCallback: function () {
                        self.applyFiltersCommand.indicateFailure();
                    },
                    completeCallback: complete,
                });
            },
            canExecute: function(isExecuting) {
                return !isExecuting && self.hasFilterParams() && apiAdjustmentSummariesUrl() !== lastApiUrl();
            }
        });
        self.clearFiltersCommand = ko.command({
            execute: function() {
                self.lotFilter(null);
                self.dateFilterStart(null);
                self.dateFilterEnd(null);
                adjustments([]);
                self.allDataLoaded(false);
                self.loadMoreAdjustmentsCommand.execute();
            }
        });
        self.closeSelectionCommand = ko.command({
            execute: function() {
                if (self.selectedItem()) self.selectedItem(null);
            }
        });
        self.initializeNewAdjustmentCommand = ko.command({
            execute: function() {
                initializeNewAdjustment();
            }, 
            canExecute: function() {
                return !self.newAdjustment();
            }
        });
        self.cancelNewAdjustmentCommand = ko.command({
            execute: function () {
                var newAdjustment = self.newAdjustment();
                if (newAdjustment && newAdjustment.hasChanges() && newAdjustment.adjustmentItems().length && newAdjustment.adjustmentItems()[0].LotKey()) {
                    showUserMessage("Do you want to save the adjustment before leaving?", {
                        type: 'yesnocancel',
                        onYesClick: function () {
                            self.newAdjustment().postAdjustmentCommand.execute({
                                successCallback: doExit
                            });
                        },
                        onNoClick: function () {
                            doExit();
                        },
                        onCancelClick: function () {

                        },
                        description: "You are about to navigate away from the adjustment without saving. Do you want to save the adjustment before you leave? Click <strong>Yes</strong> to save the adjustment and leave the page, click <strong>No</strong> to discard the data and leave the page or click <strong>Cancel</strong> to stay on the page without saving."
                    });
                } else doExit();
                
                function doExit() {
                    self.newAdjustment(null);
                }
            },
            canExecute: function() {
                return self.newAdjustment();
            }
        });


        // **************************
        // functions

        self.selectInventoryAdjustment = function (item) {
            self.selectedItem(item || null);
        };
        self.isTargetLot = function (item) {
            if (!item || !item.LotKey || !self.lotFilter()) return false;
            return self.lotFilter.match(item.LotKey);
        };

        // **************************
        // init

        ajaxStatusHelper.init(self.loadMoreAdjustmentsCommand);
        ajaxStatusHelper.init(self.applyFiltersCommand);
        self.loadMoreAdjustmentsCommand.execute();

        ko.applyBindings(self);

        return self;

        function initializeNewAdjustment() {
            var model = NewInventoryAdjustmentViewModelFactory.create();

            // commands
            model.postAdjustmentCommand = ko.asyncCommand({
                execute: function (complete) {
                    model.postAdjustmentCommand.indicateWorking();
                    model.postAdjustmentAsync({
                        antiForgeryToken: antiForgeryToken,
                        successCallback: function (data) {
                            model.saveEditsCommand.execute();
                            model.postAdjustmentCommand.clearStatus();
                            self.cancelNewAdjustmentCommand.execute();
                            scrollToNextSummaryItem(true);
                            var newItem = new Adjustment(data);
                            adjustments.splice(0, 0, newItem);
                            dataPager.incrementSkipCount();
                        },
                        completeCallback: complete,
                    });
                },
                canExecute: function (isExecuting) {
                    return !isExecuting;
                }
            });

            ajaxStatusHelper.init(model.postAdjustmentCommand);
            EsmHelper(model);

            self.newAdjustment(model);
        }
        function getAdjustmentSummariesAsync(callbackOptions) {
            var url = apiAdjustmentSummariesUrl();
            scrollToNextSummaryItem(true);
            dataPager.GetNextPage({
                urlBase: url,
                successCallback: function (data) {
                    if (lastApiUrl() !== url) {
                        lastApiUrl(url);
                        adjustments([]);
                        scrollToNextSummaryItem(false);
                    }

                    self.isResultSetFiltered(self.hasFilterParams());
                    if(callbackOptions.successCallback) callbackOptions.successCallback();
                    
                    self.allDataLoaded(dataPager.AllDataLoaded);
                    if (dataPager.AllDataLoaded) {
                        showUserMessage("All data loaded.");
                    }
                    loadAdjustmentResults(data, adjustments);
                },
                errorCallback: function (data) {
                    if (callbackOptions.errorCallback) callbackOptions.errorCallback();
                    showUserMessage(data.responseText);
                },
                completeCallback: callbackOptions.completeCallback,
            });
        }        
    }

    // *********************
    // Private Functions
    function loadAdjustmentResults(data, container) {
        var mapped = ko.utils.arrayMap(data, function (item) {
            return new Adjustment(item);
        });
        ko.utils.arrayPushAll(container(), mapped);
        container.notifySubscribers(container());
    }

    // *******************************
    // Model Constructors
    function Adjustment(values) {
        var self = {
            InventoryAdjustmentKey: values.InventoryAdjustmentKey,
            AdjustmentDate: ko.observableDateTime(values.TimeStamp, "m/dd/yyyy HH:MM"),
            Items: [],
            Summary: buildSummary(values),
            User: values.User,
            TotalAdjustmentQuantity: 0,
            TotalAdjustmentWeight: 0,
        };

        ko.utils.arrayMap(values.Items, function (item) {
            var adjustment = new AdjustmentItem(item);
            self.Items.push(adjustment);
            self.TotalAdjustmentQuantity += adjustment.AdjustmentQuantity;
            self.TotalAdjustmentWeight += adjustment.AdjustmentWeight();
        });
        
        notebookFactory.init({
            target: self,
            initialValues: values.Notebook,
            getDataOnInit: false,
        });

        return self;

        function buildSummary() {
            return (values.Notebook && values.Notebook.Notes.length)
                ? values.Notebook.Notes[0].Text
                : "";
        }
    }
    function AdjustmentItem(values) {
        values = values || {};
        var self = {
            InventoryAdjustmentItemKey: values.InventoryAdjustmentItemKey,
            AdjustmentQuantity: values.AdjustmentQuantity,
            AdjustmentWeight: ko.observable(values.AdjustmentQuantity * values.PackagingProduct.Weight).extend({ formatNumber: 2 }),
            ProductName: values.InventoryProduct.ProductName,
            Treatment: ko.observable(values.InventoryTreatment.TreatmentKey).extend({ treatmentType: true }),
            LotKey: values.LotKey,
            PackagingName: values.PackagingProduct.ProductName,
            ToteKey: values.ToteKey,
            WarehouseName: values.Location.WarehouseName,
            WarehouseLocationName: values.Location.LocationName,
            WarehouseLocationKey: values.Location.LocationKey,
            WarehouseKey: values.Location.WarehouseKey,
        };

        return self;
    }    
}(jQuery, ko, ajaxStatusHelper, NotebookViewModel));