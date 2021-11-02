var PackSchedule, PackScheduleSummary;

require('bootstrap');
ko.components.register('product-selector', require('App/components/common/product-selector/product-selector'));

define(['viewModels/production/productionBatches',
       'services/productionBatchingService',
       'helpers/koHelpers',
       'rvc',
       'ko',
       'scripts/knockout.command',
       'App/scripts/utils/jquery.plugins.stickyTableHeaders',
       'App/services/productsService',
       'App/services/warehouseLocationsService',
       'App/services/lotService'],
function (productionBatchesModule,
          productionService,
          koHelpers,
          app,
          ko,
          koCmd,
          stickyHeaders,
          productsService,
          warehouseLocationsService,
          lotService) {

    // object constructor exports
    PackSchedule = PackScheduleFactory(ko);
    PackScheduleSummary = PackScheduleSummaryFactory(ko);

    var bypassHistory;
    var instance = {};
    var productOptionsByLotType = {};

    var hash = app.navigation.getHashValue();

    function loadProductOptions() {
        var productDfds = [];
        var loadDfd = $.Deferred();
        var options = {
          filterProductsWithInventory: true
        };

        app.helpers.forEachLotType(loadAndPush);
        $.when.apply($, productDfds).done(function () {
            loadDfd.resolve(productOptionsByLotType);
        });

        function loadAndPush(type) {
            if (!ko.isObservable(productOptionsByLotType[type.value])) {
                productOptionsByLotType[type.value] = ko.observableArray([]);
            }

            var dfd = lotService.getProductsByLotType(type.key, options)
                .done(function (data) {
                    productOptionsByLotType[type.value](data);
                });

            productDfds.push(dfd);
            return dfd;
        }

        return loadDfd;
    }

    instance.options = {};
    function loadOptions() {
      var getIngredients = lotService.getIngredientsByProductType().done(function(data, textStatus, jqXHR) {
        instance.options.ingredients = data;
      });
      var getProducts = loadProductOptions().done(function(data, textStatus, jqXHR) {
        instance.options.products = data;
      });
      var getPackagingProducts = productsService.getPackagingProducts().done(function(data, textStatus, jqXHR) {
        instance.options.packaging = data;
      });
      var getRinconLocations = warehouseLocationsService.getRinconWarehouseLocations().done(function(data, textStatus, jqXHR) {
        instance.options.locations = data;
      });

      var checkOptions = $.when(getIngredients, getProducts, getPackagingProducts, getRinconLocations).then(function(data, textStatus, jqXHR) {
        return arguments;
      });

      return checkOptions;
    }

    loadOptions().done(function() {
      init(instance).done(function( Parameters ) {
        if (hash) loadFromState(hash);

        ko.applyBindings(instance);
      });
    });

    function loadFromState(state) {
        bypassHistory = true;

        if (state == undefined) instance.PackScheduleDetails(null);
        else if (state instanceof PackSchedule || state.__instanceofPackSchedule) instance.PackScheduleDetails(state);
        else if (typeof state === "object" && state.PackScheduleKey) instance.loadDetailsForPackSchedule(state.PackScheduleKey);
        else
            instance.loadDetailsForPackSchedule(state, function(data) {
                if (!data) {
                    // if pack schedule is not found, update history to summary view
                    app.navigation.updateHistory('', "Pack Schedules", null, true);
                }
            });
    }

    function init() {
        var scrollToItem = ko.observable(false);
        var packScheduleDetailsField = ko.observable();

        instance.PackScheduleSummaries = ko.observableArray(),
        instance.PackScheduleDetails = ko.computed({
            read: function() {
                return packScheduleDetailsField();
            },
            write: function(values) {
                setPackScheduleDetails(values);
                updateHistory(values);

                function setPackScheduleDetails(ps) {
                    var prev = packScheduleDetailsField.peek();
                    if (prev && prev.dispose) prev.dispose();

                    if (ps == undefined) return packScheduleDetailsField(null);

                    if (!(ps instanceof PackSchedule)) {
                        return setPackScheduleDetails(new PackSchedule(ps));
                    }

                    if (!ps.productionBatchesViewModel) {
                        productionBatchesModule.PackScheduleKey(values.PackScheduleKey);
                        productionBatchesModule.isLocked(values.IsLocked);
                        productionBatchesModule.setProductionBatches(values.ProductionBatches);
                        productionBatchesModule.batchPackagingDescription(ps.PackagingProductName());
                        productionBatchesModule.defaultPackaging(ps.PackagingProductKey());
                        productionBatchesModule.batchProductKey(ps.ChileProductKey());
                        productionBatchesModule.defaultPackagingCapacity(ps.PackagingWeight());
                        productionBatchesModule.defaultBatchTargetAsta(ps.BatchTargetAsta());
                        productionBatchesModule.defaultBatchTargetWeight(ps.BatchTargetWeight());
                        productionBatchesModule.defaultBatchTargetScoville(ps.BatchTargetScoville());
                        productionBatchesModule.defaultBatchTargetScan(ps.BatchTargetScan());
                        productionBatchesModule.Customer();

                        ps.productionBatchesViewModel = productionBatchesModule;
                    }

                    var options = findChileOptionsForProduct(ps.ChileProductKey()) || {};
                    ps.ChileType = ko.observable(options.ChileType || instance.chileTypeOptions()[0]);
                    ps.ChileProduct = ko.observable(options.ChileProduct);
                    ps.Customer = ko.observable(values.Customer && findCustomerByKey(values.Customer.CompanyKey));
                    ps.productionBatchesViewModel.Customer(values.Customer);

                    ps.PackagingProduct = ko.observable(getPackagingProductOption(ps));

                    ps.setChileType = function(value) {
                        if (arguments[0] === ps.ChileType()) return;
                        ps.ChileType(value);
                    };

                    ps.ChileTypeName = ko.computed(function() {
                        var type = ps.ChileType();
                        return type && type.ChileTypeDescription || '';
                    });

                    //#region subscribers
                    ps.__subscriptions = ps.__subscriptions || [];
                    ps.__subscriptions.push(ps.ChileType.subscribe(function() {
                        ps.ChileProduct(null);
                        ps.ChileProductKey(null);
                        ps.ChileProductName(null);
                    }));
                    ps.__subscriptions.push(ps.ChileProduct.subscribe(function(product) {
                        ps.ChileProductKey(product ? product.ProductKey : null);
                        ps.ChileProductName(product ? product.ProductName : null);
                        product && setBatchTargetsForProduct(product.ProductKey, ps, true);
                    }));
                    ps.__subscriptions.push(ps.WorkTypeKey.subscribe(function(mode) {
                        if (mode == undefined) {
                            ps.WorkType(null);
                            return;
                        }

                        var newMode = ko.utils.arrayFirst(instance.workTypeOptions(), function(o) {
                            return o.WorkTypeKey === mode;
                        });
                        ps.WorkType(newMode.Description);
                    }));
                    ps.__subscriptions.push(ps.ProductionLineKey.subscribe(function(value) {
                        if (value == undefined) {
                            ps.ProductionLineDescription('');
                            return;
                        }

                        ps.ProductionLineDescription((ko.utils.arrayFirst(instance.productionLineOptions(), function(o) {
                            return o.KeyValue === value;
                        }) || {}).Description || '');
                    }));
                    ps.__subscriptions.push(ps.PackagingProduct.subscribe(function(value) {
                        if (value == undefined) {
                            ps.PackagingProductKey(null);
                            ps.PackagingProductName('');
                            ps.PackagingWeight(0);
                            productionBatchesModule.defaultPackagingCapacity(0);
                            productionBatchesModule.batchPackagingDescription('');
                            return;
                        }

                        ps.PackagingProductKey(value.ProductKey);
                        ps.PackagingProductName(value.ProductName);
                        ps.PackagingWeight(value.Weight);
                        productionBatchesModule.defaultPackagingCapacity(value.Weight);
                        productionBatchesModule.batchPackagingDescription(value.ProductName);
                    }));
                    ps.__subscriptions.push(ps.BatchTargetWeight.subscribe(function(value) {
                        productionBatchesModule.defaultBatchTargetWeight(value);
                    }));
                    ps.__subscriptions.push(ps.BatchTargetAsta.subscribe(function(value) {
                        productionBatchesModule.defaultBatchTargetAsta(value);
                    }));
                    ps.__subscriptions.push(ps.BatchTargetScan.subscribe(function(value) {
                        productionBatchesModule.defaultBatchTargetScan(value);
                    }));
                    ps.__subscriptions.push(ps.BatchTargetScoville.subscribe(function(value) {
                        productionBatchesModule.defaultBatchTargetScoville(value);
                    }));
                    ps.__subscriptions.push(ps.Customer.subscribe(function(value) {
                        if (!value) {
                            ps.CustomerName(null);
                            ps.CustomerKey(null);
                            return;
                        }

                        ps.CustomerName(value.Name);
                        ps.CustomerKey(value.CompanyKey);
                        ps.productionBatchesViewModel.Customer(value);
                    }));
                    //#endregion

                    // set up disposal
                    var baseDispose = ps.dispose;
                    ps.dispose = function() {
                        baseDispose && baseDispose();
                        ko.utils.arrayForEach(ps.__subscriptions || [], function(sub) {
                            sub.dispose && sub.dispose();
                        });
                        ps.__subscriptions = [];
                    };

                    koHelpers.esmHelper(ps, {
                        customRevertFunctions: {
                            ChileType: function(value) {
                                if (value == undefined) return null;
                                return ko.utils.arrayFirst(instance.chileTypeOptions(), function(o) {
                                    return o.ChileTypeKey === value.ChileTypeKey;
                                });
                            },
                            ChileProduct: function(value) {
                                if (value == undefined) return null;
                                return (findChileOptionsForProduct(value.ProductKey) || {}).ChileProduct;
                            },
                            PackagingProduct: function(value) {
                                if (value == undefined) return null;
                                return (getPackagingProductOption(value));
                            },
                            Customer: function(value) {
                                if (value == undefined) return null;
                                return (findCustomerByKey(value.CompanyKey));
                            }
                        }
                    });

                    setBatchTargetsForProduct(ps.ChileProductKey(), ps);

                    packScheduleDetailsField(ps);
                }
            }
        });
        instance.loadPackScheduleDetailsState = koHelpers.ajaxStatusHelper({});
        instance.areResultsFiltered = ko.observable(false);

        // filters
        instance.beginningCreatedDateRangeFilter = ko.observableDate();
        instance.endingCreatedDateRangeFilter = ko.observableDate();
        instance.beginningScheduledDateRangeFilter = ko.observableDate();
        instance.endingScheduledDateRangeFilter = ko.observableDate();
        instance.searchPackScheduleKey = ko.observable();

        // lookup lists
        instance.chileTypeOptions = ko.observableArray([]);
        instance.packagingProductOptions = ko.observableArray(instance.options.packaging);
        instance.warehouseLocationOptions = ko.observableArray(instance.options.locations);
        instance.ingredientOptions = ko.observable(instance.options.ingredients);
        instance.productOptions = ko.observable(instance.options.products);
        instance.workTypeOptions = ko.observableArray([]);
        instance.productionLineOptions = ko.observableArray([]);
        instance.customerOptions = ko.observableArray([]);

        // methods
        instance.animateSummaryItem = koHelpers.animateNewListElement({
            scrollToItem: scrollToItem,
            afterScrollCallback: function() {
                scrollToItem(false);
            },
        });
        instance.packScheduleSelectedHandler = function() {
            var selected = getSelectedItemFromClickEvent.apply(null, arguments);
            if (!selected) return;
           loadDetailsForPackSchedule(selected.PackScheduleKey);
        };
        instance.loadDetailsForPackSchedule = loadDetailsForPackSchedule;

        // computed properties
        instance.chileProductOptions = ko.computed(function() {
            var ps = instance.PackScheduleDetails();
            if (!ps) return [];
            return (ps.ChileType() || {}).options || [];
        });
        instance.hasFilters = ko.computed(function() {
            return this.beginningCreatedDateRangeFilter() ||
                this.endingCreatedDateRangeFilter() ||
                this.beginningScheduledDateRangeFilter() ||
                this.endingScheduledDateRangeFilter();
        }, instance);
        instance.hasSelectedBatch = ko.computed(function() {
            var details = this.PackScheduleDetails();
            return details &&
                details.productionBatchesViewModel &&
                details.productionBatchesViewModel.SelectedProductionBatch();
        }, instance);
        instance.showPackScheduleControls = ko.computed(function() {
            var psDetails = this.PackScheduleDetails();
            return psDetails && (!psDetails.productionBatchesViewModel.pickInventory());
        }, instance);
        instance.isAutoPickerWorking = ko.computed(function () {
            var psDetails = this.PackScheduleDetails();
            return psDetails && psDetails.productionBatchesViewModel.isAutoPickerWorking();
        }, instance);
        instance.isPickerWorking = ko.computed(function () {
            var details = instance.PackScheduleDetails();
            return details && details.productionBatchesViewModel.isPickerWorking();
        });
        instance.isPickerSaving = ko.computed(function () {
            var details = instance.PackScheduleDetails();
            return details && details.productionBatchesViewModel.isPickerSaving();
        });
        instance.isPickerDeleting = ko.computed(function () {
            var details = instance.PackScheduleDetails();
            return details && details.productionBatchesViewModel.isPickerDeleting();
        });

        // subscribers
        instance.customerOptions.subscribe(function(val) {
            // if customers take too long to load, this will cause the Customer value to be set
            // retroactively.
            var psDetails = instance.PackScheduleDetails();
            if (psDetails && psDetails.CustomerKey() && !psDetails.Customer()) {
                var isDirty = psDetails.hasChanges.peek();
                psDetails.Customer(findCustomerByKey(psDetails.CustomerKey()));
                if (!isDirty) {
                    psDetails.saveEditsCommand.execute();
                }
            }
        });

        var dataPager = productionService.getPackSchedulesPager({
            successCallback: insertPackSchedules,
            errorCallback: function(xhr, status, message) {
                showUserMessage("Unable to load pack schedules", message);
            },
            pageSize: 50,
            queryParams: {
                beginningCreatedDateRange: instance.beginningCreatedDateRangeFilter,
                endingCreatedDateRange: instance.endingCreatedDateRangeFilter,
                beginningDateScheduledRange: instance.beginningScheduledDateRangeFilter,
                endingDateScheduledRange: instance.endingScheduledDateRangeFilter,
            },
        });
        dataPager.allDataLoaded.subscribe(function(endOfResults) {
            if (endOfResults) {
                showUserMessage("All data is loaded.");
            }
        });

        //#region commands
        instance.loadPackSchedulesCommand = ko.asyncCommand({
            execute: function(complete) {
                return dataPager.GetNextPage()
                    //.done()
                    .always(complete);
            },
            canExecute: function(isExecuting) {
                return !isExecuting && !dataPager.allDataLoaded();
            }
        });
        instance.applyFiltersCommand = ko.command({
            execute: function() {
                instance.PackScheduleSummaries([]);
                dataPager.GetNextPage();
            },
            canExecute: function() {
                return instance.hasFilters() || (instance.areResultsFiltered() && !instance.hasFilters());
            }
        });
        instance.clearFiltersCommand = ko.command({
            execute: function() {
                instance.beginningCreatedDateRangeFilter(null);
                instance.endingCreatedDateRangeFilter(null);
                instance.beginningScheduledDateRangeFilter(null);
                instance.endingScheduledDateRangeFilter(null);
                instance.applyFiltersCommand.execute();
            },
            canExecute: function() {
                return instance.areResultsFiltered() && instance.hasFilters();
            }
        });
        instance.closeSelectedPackScheduleCommand = ko.command({
            execute: function() {
                var packSchedule = instance.PackScheduleDetails();
                if (packSchedule) {
                    var batch = packSchedule.productionBatchesViewModel && packSchedule.productionBatchesViewModel.SelectedProductionBatch();
                    if (batch && batch.hasChanges()) {
                        showUserMessage('Save Production Batch before closing?', {
                            description: 'The Production Batch <strong>' + packSchedule.productionBatchesViewModel.SelectedProductionBatch().OutputLotKey() + '</strong> has unsaved changes. Do you want to save your changes?  Click <strong>Yes</strong> to save changes. Click <strong>No</strong> to discard the changes. Click <strong>Cancel</strong> to stay on the current Pack Schedule without saving.',
                            type: 'yesnocancel',
                            onYesClick: function() {
                                packSchedule.productionBatchesViewModel.saveProductionBatchCommand.execute()
                                    .done(instance.closeSelectedPackScheduleCommand.execute);
                            },
                            onNoClick: function() {
                                if (!batch.isNew) {
                                  batch.cancelEditsCommand.execute();
                                } else {
                                  closePackSchedule();
                                }
                                instance.closeSelectedPackScheduleCommand.execute();
                            },
                            onCancelClick: function() {}
                        });
                        return;
                    } else if (packSchedule.hasChanges()) {
                        showUserMessage('Save Pack Schedule before closing?', {
                            description: 'This Pack Schedule has unsaved changes. Do you want to save your changes before closing the pack schedule? Click <strong>Yes</strong> to save changes. Click <strong>No</strong> to discard the changes. Click <strong>Cancel</strong> to stay on the current Pack Schedule without saving.',
                            type: 'yesnocancel',
                            onYesClick: function() {
                                instance.savePackScheduleCommand.execute()
                                    .done(instance.closeSelectedPackScheduleCommand.execute);

                            },
                            onNoClick: function() {
                                closePackSchedule();
                            },
                            onCancelClick: function() {}
                        });
                        return;
                    }
                }

                closePackSchedule();

                function closePackSchedule() {
                    packSchedule.productionBatchesViewModel.cleanup();
                    instance.PackScheduleDetails(null);
                }
            },
            canExecute: function() { return !!instance.PackScheduleDetails(); }
        });
        instance.toggleSystemDefaultsOverrideCommand = ko.command({
          execute: function () {
            var packSchedule = instance.PackScheduleDetails();
            if (!packSchedule) { return; }
            packSchedule.OverrideSystemDefaults(!packSchedule.OverrideSystemDefaults());
          },
          canExecute: function () {
            var packSchedule = instance.PackScheduleDetails();
            return packSchedule && packSchedule.IsNew();
          }
        })
        instance.initializeNewPackScheduleCommand = ko.command({
            execute: function() {
                instance.PackScheduleDetails({ isNew: true });
                instance.PackScheduleDetails().beginEditingCommand.execute();
            }
        });
        instance.savePackScheduleCommand = ko.asyncCommand({
            execute: function(completed) {
                var ps = instance.PackScheduleDetails.peek();
                try {
                    var result = ko.validation.group(ps);
                    if (result.length > 0) {
                        showUserMessage('Invalid Pack Schedule Data', { description: 'Please correct the validation errors and retry.' });
                        result.showAllMessages(true);
                        completed();
                        return;
                    }
                } catch (ex) {
                    completed();
                    showUserMessage('Error saving Pack Schedule');
                    instance.savePackScheduleCommand.indicateFailure();
                    return;
                }

                instance.savePackScheduleCommand.indicateWorking();
                var key = ps.PackScheduleKey();
                if (key == undefined) {
                    if (ps.OverrideSystemDefaults()) {
                      ps.DateCreated = ps.DateCreatedOverride;
                      ps.Sequence = ps.SequenceOverride;
                      ps.PSNum = ps.PSNumOverride;
                    } 

                    productionService.createPackSchedule(ps)
                        .done(function(newKey) {
                            ps.productionBatchesViewModel.batchProductKey(ps.ChileProductKey());
                            ps.productionBatchesViewModel.PackScheduleKey(newKey);
                            ps.PackScheduleKey(newKey);
                            ps.ChileProductName(ps.ChileProduct().ProductName);
                            ps.DateCreatedOverride(null);
                            ps.SequenceOverride(null);
                            ps.PSNumOverride(null);
                            var ProductionLineKey = ps.ProductionLineKey();
                            var lineOptions = instance.productionLineOptions();
                            var ProductionLine = ko.utils.arrayFirst(lineOptions, function(line) {
                              return line.LocationKey === ProductionLineKey;
                            });
                            ps.ProductionLineDescription(ProductionLine.Description);
                            (function() {
                              var today = Date.now();
                              var dateString = (today.getMonth() + 1) + '/' + today.getDate() + '/' + today.getFullYear();

                              ps.DateCreated = dateString;
                            })();
                            refreshPackScheduleFromServer(ps);
                            instance.PackScheduleSummaries.splice(0, 0, ps);
                            updateHistory(ps, true);
                            success("Pack Schedule Created successfully.");
                        })
                        .fail(function (xhr, status, message) {
                            failure('Error creating Pack Schedule', message);
                        })
                        .always(completed);
                } else {
                    productionService.updatePackSchedule(ps.PackScheduleKey(), ps)
                        .done(function() {
                          var key = ps.PackScheduleKey();
                          var summaries = instance.PackScheduleSummaries();
                          var item = ko.utils.arrayFirst( summaries, function( summary ) {
                            return ko.unwrap(summary.PackScheduleKey) === key;
                          });
                          var itemIndex = summaries.indexOf(item);

                          instance.PackScheduleSummaries.splice(itemIndex, 1, ps);
                          success('Pack Schedule Saved Successfully');
                        })
                        .fail(function(xhr, status, message) {
                          failure('Error updating Pack Schedule', message);
                        })
                        .always(completed);
                }

                function success(message) {
                    showUserMessage(message);
                    instance.savePackScheduleCommand.indicateSuccess();
                    ps.saveEditsCommand.execute();
                }
                function failure(message, description) {
                    showUserMessage(message, { description: description });
                    instance.savePackScheduleCommand.indicateFailure();
                }
            },
            canExecute: function(isExecuting) {
                var ps = instance.PackScheduleDetails();
                return !isExecuting && ps && ps.hasChanges();
            }
        });
        instance.deletePackScheduleCommand = ko.asyncCommand({
            execute: function(complete) {
                var key = instance.PackScheduleDetails().PackScheduleKey();
                showUserMessage('Do you want to delete this Pack Schedule?', {
                    description: 'Are you sure you want to delete this Pack Schedule? You will not be able to undo this operation. To delete, click <strong>Yes</strong> otherwise click <strong>No</strong> to cancel.',
                    type: 'yesno',
                    onYesClick: executeDeletion,
                    onNoClick: complete
                });

                function executeDeletion() {
                    instance.deletePackScheduleCommand.indicateWorking();
                    productionService.deletePackSchedule(key)
                        .done(function() {
                            instance.deletePackScheduleCommand.clearStatus();
                            showUserMessage('Pack Schedule Deleted', { description: 'The Pack Schedule (' + key + ') was successfully deleted.' });
                            removePackScheduleFromSummaryItems(instance.PackScheduleDetails());
                            instance.PackScheduleDetails(null);
                        }).fail(function(xhr, status, message) {
                            instance.deletePackScheduleCommand.clearStatus();
                            showUserMessage('Failed to delete the Pack Schedule', { description: message });
                        }).always(complete);
                }
            },
            canExecute: function(isExecuting) {
                return !isExecuting && instance.PackScheduleDetails() != undefined;
            }
        });
        instance.findPackScheduleCommand = ko.asyncCommand({
            execute: function(complete) {
                loadDetailsForPackSchedule(instance.searchPackScheduleKey(), complete);
            },
            canExecute: function(isExecuting) { return !isExecuting; }
        });
        //#endregion

        var checkOptionsLoaded = $.when(
          instance.loadPackSchedulesCommand.execute(),
          loadChileProductOptions(),
          loadWorkModes(),
          loadCustomerOptions(),
          loadProductionLineOptions()
        ).then(
        function(data, textStatus, jqXHR) {
          return arguments
        });

        koHelpers.ajaxStatusHelper(instance.savePackScheduleCommand);
        koHelpers.ajaxStatusHelper(instance.deletePackScheduleCommand);

        instance.rebuild = rebuild.bind(instance);

        return checkOptionsLoaded;

        function insertPackSchedules(data) {
            instance.areResultsFiltered(instance.hasFilters.peek() != undefined);
            scrollToItem(instance.PackScheduleSummaries().length);
            var mapped = ko.utils.arrayMap(data, function(item) { return new PackScheduleSummary(item); });
            ko.utils.arrayPushAll(instance.PackScheduleSummaries(), mapped);
            instance.PackScheduleSummaries.notifySubscribers(instance.PackScheduleSummaries());
        }

        function loadDetailsForPackSchedule(key, callback) {
            instance.loadPackScheduleDetailsState.indicateWorking();
            productionService.getPackScheduleDetails(ko.utils.unwrapObservable(key))
                .done(function(data) {
                    instance.loadPackScheduleDetailsState.clearStatus();
                    instance.PackScheduleDetails(data);
                    var summaryIndex = getIndexOfPackScheduleSummaryItemByKey(key);
                    if (summaryIndex > -1) {
                        instance.PackScheduleSummaries.splice(summaryIndex, 1, instance.PackScheduleDetails());
                    }
                    callback && callback(data);
                }).fail(function(xhr, status, message) {
                    instance.loadPackScheduleDetailsState.indicateFailure();
                    showUserMessage("Unable to load Pack Schedule details.", { description: message });
                    callback && callback(null);
                });
        }

        function refreshPackScheduleFromServer(target) {
            productionService.getPackScheduleDetails(target.PackScheduleKey())
                .done(function (data) {
                    var hasChanges = target.hasChanges.peek();
                    target.PSNum(data.PSNum);

                    if (target.isNew()) {
                        target.productionBatchesViewModel.initializeNewBatchCommand.execute();
                        target.isNew(false);
                    }

                    if (!hasChanges) {
                        target.saveEditsCommand.execute();
                    }
                });
        }

        function loadChileProductOptions() {
            var chileProductsCache = {};

            var loadChileOptions = productionService.getChileProducts(null).then(
            function(data, textStatus, jqXHR) {
              ko.utils.arrayMap(data, function(product) {
                if (!chileProductsCache[product.ChileTypeKey]) {
                  chileProductsCache[product.ChileTypeKey] = {
                    ChileTypeKey: product.ChileTypeKey,
                    ChileTypeDescription: product.ChileTypeDescription,
                    options: [],
                  };
                }
                chileProductsCache[product.ChileTypeKey].options.push(product);
              });

              var nonObservedArray = instance.chileTypeOptions();
              for (var p in chileProductsCache) {
                nonObservedArray.push(chileProductsCache[p]);
              }
              instance.chileTypeOptions.notifySubscribers();
            },
            function(jqXHR, textStatus, errorThrown) {
              showUserMessage("Unable to load chile product options.", { description: errorThrown });
            });

            return loadChileOptions;
        }

        function findChileOptionsForProduct(chileProductKey) {
            if (ko.isObservable(chileProductKey)) chileProductKey = chileProductKey();
            if (chileProductKey == undefined) return null;

            var options = null;
            ko.utils.arrayFirst(instance.chileTypeOptions(), function(type) {
                var match = ko.utils.arrayFirst(type.options, function(o) {
                    return o.ProductKey === chileProductKey;
                });

                if (match) {
                    options = {
                        ChileType: type,
                        ChileProduct: match,
                    };
                }

                return !!match;
            });
            return options;
        }

        function findCustomerByKey(key) {
            return ko.utils.arrayFirst(instance.customerOptions(), function(c) {
                return c.CompanyKey === key;
            });
        }

        function getPackagingProductOption(ps) {
            ps = ps || {};
            var key = ps;
            if (typeof key === "object") {
                key = ko.utils.unwrapObservable(
                    key.hasOwnProperty("PackagingProductKey") ? key.PackagingProductKey : key.ProductKey);
            }
            if (key == undefined) return null;

            return ko.utils.arrayFirst(instance.packagingProductOptions(), function(o) {
                return o.ProductKey === key;
            });
        }

        function loadWorkModes() {
            var getModes = productionService.getWorkModes().then(
            function(data, textStatus, jqXHR) {
              instance.workTypeOptions(data);
            },
            function(jqXHR, textStatus, errorThrown) {
              showUserMessage("Failed to get Work Modes from database.", { description: errorThrown });
            });

            return getModes;
        }

        function loadProductionLineOptions() {
            var getLineOptions = productionService.getProductionLocations().then(
            function(data, textStatus, jqXHR) {
              instance.productionLineOptions(data);
            },
            function(jqXHR, textStatus, errorThrown) {
              showUserMessage("Failed to get Production Lines from database.", { description: errorThrown });
            });
        }

        function loadCustomerOptions() {
            var loadCustomerOpts = productionService.getCustomers().then(
            function(data, textStatus, jqXHR) {
              data = data || [];
              instance.customerOptions( data.sort(function( a, b ) {
                return a.Name.toUpperCase() < b.Name.toUpperCase() ? -1 : 1;
              }));
            },
            function(jqXHR, textStatus, errorThrown) {
              showUserMessage("Failed to load customer options", { description: errorThrown });
            });

            return loadCustomerOpts;
        }

        function getIndexOfPackScheduleSummaryItemByKey(psKey) {
            var index = -1;
            if (psKey == undefined) return index;
            ko.utils.arrayFirst(instance.PackScheduleSummaries(), function(item) {
                index++;
                return ko.utils.unwrapObservable(item.PackScheduleKey) === psKey;
            });
            return index;
        }

        function removePackScheduleFromSummaryItems(ps) {
            if (!ps) return;
            var psKey = ps;
            if (typeof ps === "object") {
                psKey = ko.utils.unwrapObservable(ps.PackScheduleKey);
            }
            var index = getIndexOfPackScheduleSummaryItemByKey(psKey);

            if (index > -1) {
                instance.PackScheduleSummaries.splice(index, 1);
            }
        }

        function setBatchTargetsForProduct(productKey, target, updateBatchTargets) {
            if (!productKey) return;

            var getTargets = productionService.getProductDetails(app.lists.inventoryTypes.Chile.key, productKey).then(
            function(data, textStatus, jqXHR) {
              productionBatchesModule.setBatchTargets(data);

              if (updateBatchTargets) {
                target.BatchTargetAsta(getTargetValueFromSpec('Asta'));
                target.BatchTargetScoville(getTargetValueFromSpec('Scov'));
                target.BatchTargetScan(getTargetValueFromSpec('Scan'));
              }

              function getTargetValueFromSpec(attrKey) {
                var attr = ko.utils.arrayFirst(data.AttributeRanges, function(a) {
                  return a.AttributeNameKey === attrKey;
                });

                if (!attr) {
                  return null;
                }

                return parseInt((attr.MaxValue + attr.MinValue) / 2);
              }
            },
            function(jqXHR, textStatus, errorThrown) {
              showUserMessage(
                'Failed to get product specifications.',
                { description: errorThrown }
              );
            });
        }
    }

    function updateHistory(values, useReplace) {
        if (bypassHistory === true) {
            bypassHistory = false;
            return;
        }

        var state = JSON.parse(ko.toJSON(values));
        if (values && typeof values === "object") {
            state.__instanceofPackSchedule = true;
        }

        var hash = (state && state.PackScheduleKey) || '';
        app.navigation.updateHistory(hash, "Pack Schedule " + hash, state, useReplace);
    }

    function getSelectedItemFromClickEvent() {
        if (knownObject(arguments[0])) {
            return arguments[0];
        }
        if (arguments.length < 2) return null;

        try {
            var context = ko.contextFor(arguments[1].originalEvent.target);
            if (context && knownObject(context.$data)) {
                return context.$data;
            }
        } catch (ex) {
            console.debug(ex);
        }

        return null;

        function knownObject(o) {
            return o instanceof PackScheduleSummary ||
                o instanceof PackSchedule;
        }
    }

    function rebuild() {
        for (var prop in this) {
            if (this.hasOwnProperty(prop)) {
                delete this[prop];
            }
        }
        init(this);
    }

    function PackScheduleFactory(ko) {
        return function PackSchedule(values) {
            if (!(this instanceof PackSchedule)) return new PackSchedule(values);
            values = values || {};
            values.TargetParameters = values.TargetParameters || {};

            var model = this;

            model.isNew = ko.observable(values.isNew || false);
          
            model.DateCreatedOverride = ko.observableDate();
            model.SequenceOverride = ko.observable();
            model.PSNumOverride = ko.observable();

            model.PackScheduleKey = ko.observable(values.PackScheduleKey);
            model.PSNum = ko.observable(values.PSNum);
            model.WorkType = ko.observable(values.WorkType);
            model.WorkTypeKey = ko.observable(values.WorkTypeKey);
            model.ChileProductKey = ko.observable(values.ChileProductKey).extend({ required: true });
            model.ChileProductName = ko.observable(values.ChileProductName);
            model.PackagingProductKey = ko.observable(values.PackagingProductKey).extend({ required: true });
            model.PackagingProductName = ko.observable(values.PackagingProductName);
            model.PackagingWeight = ko.observable(values.PackagingWeight);
            model.ProductionLineKey = ko.observable(values.ProductionLineKey);
            model.ProductionLineDescription = ko.observable(values.ProductionLineDescription);
            model.DateCreated = ko.observableDate(values.DateCreated);
            model.ProductionDeadline = ko.observableDate(values.ProductionDeadline);
            model.ScheduledProductionDate = ko.observableDate(values.ScheduledProductionDate).extend({ required: true });
            model.SummaryOfWork = ko.observable(values.SummaryOfWork);
            model.BatchTargetWeight = ko.numericObservable(values.TargetParameters.BatchTargetWeight);
            model.BatchTargetAsta = ko.numericObservable(values.TargetParameters.BatchTargetAsta);
            model.BatchTargetScan = ko.numericObservable(values.TargetParameters.BatchTargetScan);
            model.BatchTargetScoville = ko.numericObservable(values.TargetParameters.BatchTargetScoville);
            model.OrderNumber = ko.observable(values.OrderNumber);
            model.CustomerName = ko.observable(values.Customer ? values.Customer.Name : null);
            model.CustomerKey = ko.observable(values.Customer ? values.Customer.CompanyKey : null);
            model.OverrideSystemDefaults = ko.observable(false);

            model.PackScheduleTitle = ko.computed(function () {
                return model.PackScheduleKey() == undefined ?
                  'Create New Pack Schedule' : model.PackScheduleKey() + '  (PS# ' + model.PSNum() + ')';
            });
            model.IsNew = ko.computed(function () {
                return !model.PackScheduleKey();
            });

            return this;
        };
    }

    function PackScheduleSummaryFactory(ko) {
        return function PackScheduleSummary(values) {
            if (!(this instanceof PackScheduleSummary)) return new PackScheduleSummary(values);

            var model = this;

            model.PackScheduleKey = values.PackScheduleKey;
            model.PSNum = values.PSNum;
            model.WorkTypeKey = values.WorkTypeKey;
            model.WorkType = values.WorkType;
            model.ChileProductKey = values.ChileProductKey;
            model.ChileProductName = values.ChileProductName;
            model.ProductionLineKey = values.ProductionLineKey;
            model.ProductionLineDescription = values.ProductionLineDescription;
            model.DateCreated = ko.observableDate(values.DateCreated);
            model.ProductionDeadline = ko.observableDate(values.ProductionDeadline);
            model.ScheduledProductionDate = ko.observableDate(values.ScheduledProductionDate);
            model.PackagingProductName = values.PackagingProductName;
            model.PackagingWeight = values.PackagingWeight;
            model.SummaryOfWork = values.SummaryOfWork;
            model.BatchTargetWeight = values.BatchTargetWeight;
            model.BatchTargetAsta = values.BatchTargetAsta;
            model.BatchTargetScan = values.BatchTargetScan;
            model.BatchTargetScoville = values.BatchTargetScoville;

            return this;
        };
    }
});
ko.components.register('loading-screen', require('App/components/common/loading-screen/loading-screen'));
