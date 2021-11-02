webpackJsonp([12],[
/* 0 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {var PackSchedule, PackScheduleSummary;

	__webpack_require__(30);
	ko.components.register('product-selector', __webpack_require__(20));

	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(155),
	       __webpack_require__(160),
	       __webpack_require__(55),
	       __webpack_require__(8),
	       __webpack_require__(9),
	       __webpack_require__(44),
	       __webpack_require__(161),
	       __webpack_require__(24),
	       __webpack_require__(11),
	       __webpack_require__(72)], __WEBPACK_AMD_DEFINE_RESULT__ = function (productionBatchesModule,
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
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));
	ko.components.register('loading-screen', __webpack_require__(91));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 1 */,
/* 2 */,
/* 3 */,
/* 4 */,
/* 5 */,
/* 6 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {var warehouseService = (function () {
	    var serviceCore = __webpack_require__(7);
	    var dataPager = __webpack_require__(10);

	    function getIntraWarehouseMovement() {
	        return serviceCore.ajax(buildIntraWarehouseAPIUrl(arguments[0]));
	    }
	    function createIntraWarehouseMovement(values) {
	        return serviceCore.ajaxPost(buildIntraWarehouseAPIUrl(), values);
	    }
	    function updateIntraWarehouseMovement(key, values) {
	        return serviceCore.ajaxPut(buildIntraWarehouseAPIUrl(key), values);
	    }

	    // Facilities
	    var FACILITY_BASE_URL = '/api/facilities/';
	    function getWarehouseDetails(key) {
	        return serviceCore.ajax( FACILITY_BASE_URL + key);
	    }
	    function getWarehouses() {
	        return serviceCore.ajax( FACILITY_BASE_URL );
	    }
	    function getFacilities() {
	      return serviceCore.ajax( FACILITY_BASE_URL );
	    }
	    function getFacilityDetails( facilityKey ) {
	      return serviceCore.ajax( FACILITY_BASE_URL + facilityKey );
	    }
	    function createFacility( facilityData ) {
	      return serviceCore.ajaxPost( FACILITY_BASE_URL, facilityData );
	    }
	    function updateFacility( facilityKey, facilityData ) {
	      if ( !facilityKey ) {
	        throw new Error('Updating facilities requires a key');
	      }

	      return serviceCore.ajaxPut( FACILITY_BASE_URL + facilityKey, facilityData );
	    }
	    function getFacilityLocations( facilityKey ) {
	      return serviceCore.ajax( FACILITY_BASE_URL + facilityKey + '/locations' );
	    }
	    function freezeStreet( facilityKey, street ) {
	      return serviceCore.ajaxPut( FACILITY_BASE_URL + facilityKey + '/lock?streetname=' + street );
	    }
	    function unfreezeStreet( facilityKey, street ) {
	      return serviceCore.ajaxPut( FACILITY_BASE_URL + facilityKey + '/unlock?streetname=' + street );
	    }

	    var FACILITY_LOCATION_BASE_URL = '/api/facilityLocations/';
	    function createLocation( locationData ) {
	      return serviceCore.ajaxPost( FACILITY_LOCATION_BASE_URL, locationData );
	    }
	    function updateLocation( locationKey, locationData ) {
	      return serviceCore.ajaxPut( FACILITY_LOCATION_BASE_URL + locationKey, locationData );
	    }
	    function getInterWarehouseMovementsDataPager(options) {
	        options = options || {};
	        options.urlBase = buildInterWarhouseUrl();
	        return dataPager.init(options);
	    }
	    function getInterWarehouseDetails(key) {
	        return serviceCore.ajax(buildInterWarhouseUrl(key));
	    }
	    function createInterWarehouseMovement(values) {
	        return serviceCore.ajaxPost(buildInterWarhouseUrl(), values);
	    }
	    function updateInterWarehouseMovement(key, values) {
	        return serviceCore.ajaxPut(buildInterWarhouseUrl(key), values);
	    }
	    function postAndCloseShipmentOrder(key, values) {
	        if(key == undefined) { throw new Error("Invalid argument. Expected key value."); }
	        return serviceCore.ajaxPost(buildShipmentOrderUrl(key) + '/postandclose', values);
	    }

	    function buildIntraWarehouseAPIUrl(key) {
	        return ["/api/IntraWarehouseInventoryMovements/", key || ''].join('');
	    }
	    function buildInterWarhouseUrl(key) {
	        return ['/api/InterWarehouseInventoryMovements/', key || ''].join('');
	    }
	    function buildShipmentOrderUrl(key) {
	        return ['/api/InventoryShipmentOrders/', key || ''].join('');
	    }

	    return {
	        getIntraWarehouseMovementDetails: getIntraWarehouseMovement,
	        getIntraWarehouseMovements: getIntraWarehouseMovement,
	        createIntraWarehouseMovement: createIntraWarehouseMovement,
	        updateIntraWarehouseMovement: updateIntraWarehouseMovement,
	        getWarehouseDetails: getWarehouseDetails,
	        getWarehouses: getWarehouses,
	        getFacilities: getFacilities,
	        getFacilityDetails: getFacilityDetails,
	        createFacility: createFacility,
	        updateFacility: updateFacility,
	        getFacilityLocations: getFacilityLocations,
	        createLocation: createLocation,
	        updateLocation: updateLocation,
	        freezeStreet: freezeStreet,
	        unfreezeStreet: unfreezeStreet,
	        getInterWarehouseMovementsDataPager: getInterWarehouseMovementsDataPager,
	        getInterWarehouseDetails: getInterWarehouseDetails,
	        createInterWarehouseMovement: createInterWarehouseMovement,
	        updateInterWarehouseMovement: updateInterWarehouseMovement,
	        postAndCloseShipmentOrder: postAndCloseShipmentOrder,
	    };
	})();

	module.exports = $.extend(warehouseService, __webpack_require__(11));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 7 */,
/* 8 */,
/* 9 */,
/* 10 */,
/* 11 */
/***/ (function(module, exports, __webpack_require__) {

	var warehouseLocationsService = (function () {
	    var serviceCore = __webpack_require__(7);
	    var rvc = __webpack_require__(8);

	    function getWarehouseLocations(warehouseKey, options) {
	      return serviceCore.ajax(["/api/facilities/", warehouseKey, "/locations/"].join(''), options);
	    }

	    function updateWarehouseLocation(locationKey, values) {
	        return serviceCore.ajaxPut(buildWarehouseLocationsUrl(locationKey), values);
	    }

	    function createWarehouseLocation(values) {
	        return serviceCore.ajaxPost(buildWarehouseLocationsUrl(), values);
	    }

	    function getRinconWarehouseLocations() {
	        return getWarehouseLocations(rvc.lists.rinconWarehouse.WarehouseKey, arguments[0]);
	    }

	    function createWarehouse(values) {
	        return updateWarehouse(null, values);
	    }

	    function updateWarehouse(key, values) {
	        if (key) {
	            return serviceCore.ajaxPut(buildWarehouseUrl(key), values);
	        }
	        return serviceCore.ajaxPost(buildWarehouseUrl(key), values);
	    }

	    function buildWarehouseLocationsUrl(locationKey) {
	      return '/api/facilities/' + (locationKey || '');
	    }

	    function buildWarehouseUrl(key) {
	      return '/api/facilities/' + (key || '');
	    }

	    function freezeFacilityLocationsGroup(facilityKey, groupName) {
	        return serviceCore.ajaxPut(buildWarehouseUrl(facilityKey) + '/street/' + encodeURI(groupName) + '/lock');
	    }

	    function unfreezeFacilityLocationsGroup(facilityKey, groupName) {
	        return serviceCore.ajaxPut(buildWarehouseUrl(facilityKey) + '/street/' + encodeURI(groupName) + '/unlock');
	    }

	    return {
	        getRinconWarehouseLocations: getRinconWarehouseLocations,
	        getWarehouseLocations: getWarehouseLocations,
	        updateWarehouseLocation: updateWarehouseLocation,
	        createWarehouseLocation: createWarehouseLocation,
	        createWarehouse: createWarehouse,
	        updateWarehouse: updateWarehouse,
	        freezeFacilityLocationsGroup: freezeFacilityLocationsGroup,
	        unfreezeFacilityLocationsGroup: unfreezeFacilityLocationsGroup
	    }
	})();

	module.exports = warehouseLocationsService;

/***/ }),
/* 12 */,
/* 13 */,
/* 14 */,
/* 15 */,
/* 16 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(7), __webpack_require__(8)], __WEBPACK_AMD_DEFINE_RESULT__ = function(core, app) {
	    return {
	        getCompanies: curryGetCompaniesDelegate(),
	        getCompaniesDataPager: getCompaniesDataPager,
	        getCompanyDetails: curryGetCompanyByKeyDelegate,
	        getCompanyData: getCompanyData,
	        getCustomers: curryGetCompaniesDelegate(app.lists.companyTypes.Customer.key),
	        getDehydrators: curryGetCompaniesDelegate(app.lists.companyTypes.Dehydrator.key),
	        getBrokers: curryGetCompaniesDelegate(app.lists.companyTypes.Broker.key),
	        getVendors: function (vendorType) { return core.ajax(getCompaniesUrlBuilder(vendorType)); },
	        getVendorDetails: curryGetCompanyByKeyDelegate,
	        createVendor: createVendor,
	        createCompany: createCompany,
	        updateCompany: updateCompany,
	        getContacts: getContacts,
	        createContact: createContact,
	        updateContact: updateContact,
	        deleteContact: deleteContact,
	        getNoteTypes: getNoteTypes,
	        createNote: createNote,
	        updateNote: updateNote,
	        deleteNote: deleteNote
	    };

	    function createCompany( companyData ) {
	      return core.ajaxPost( '/api/companies/', companyData );
	    }

	    function updateCompany( companyKey, companyData ) {
	      return core.ajaxPut( '/api/companies/' + companyKey, companyData );
	    }

	    function createVendor( data ) {
	      var _data = data;

	      // "1" = Supplier
	      _data.VendorTypes = [1];

	      return core.ajaxPost( '/api/vendors/', _data );
	    }

	    function getCompanyData( companyKey ) {
	      return core.ajax( '/api/companies/' + companyKey );
	    }

	    function getCompaniesDataPager( options ) {
	      options = options || {};
	      return core.pagedDataHelper.init({
	        urlBase: "/api/companies",
	        pageSize: options.pageSize || 50,
	        parameters: options.parameters,
	        resultCounter: function (data) {
	          return data.length;
	        },
	        onNewPageSet: options.onNewPageSet
	      });
	    }
	    function curryGetCompaniesDelegate (companyType) {
	        return function () { return core.ajax(getCompaniesUrlBuilder(companyType)); };
	    }

	    function curryGetCompanyByKeyDelegate(companyKey) {
	        return function () {
	          return core.ajax(getCompanyByKeyUrlBuilder(companyKey));
	        };
	    }
	    function getCompanyByKeyUrlBuilder(companyKey) {
	        return function () {
	            return '/api/companies/' + companyKey;
	        };
	    }
	    function getCompaniesUrlBuilder(companyType) {
	        return function () {
	            return '/api/companies' + (companyType == null ? "" : "?companyType=" + companyType);
	        };
	    }

	    function getContacts( companyKey ) {
	      return core.ajax( '/api/companies/' + companyKey + '/contacts' );
	    }

	    function createContact( companyKey, contactData ) {
	      return core.ajaxPost('/api/companies/' + companyKey + '/contacts', contactData );
	    }

	    function updateContact( contactKey, contactData ) {
	      return core.ajaxPut('/api/contacts/' + contactKey, contactData );
	    }

	    function deleteContact( contactKey ) {
	      return core.ajaxDelete('/api/contacts/' + contactKey );
	    }

	    function getNoteTypes() {
	      return core.ajax('/api/profilenotes/types');
	    }
	    function createNote( companyKey, note ) {
	      return core.ajaxPost( '/api/companies/' + companyKey + '/notes/', note );
	    }
	    function updateNote( companyKey, noteId, note ) {
	      return core.ajaxPut( '/api/companies/' + companyKey + '/notes/' + noteId, note );
	    }
	    function deleteNote( companyKey, noteId ) {
	      return core.ajaxDelete( '/api/companies/' + companyKey + '/notes/' + noteId );
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),
/* 17 */,
/* 18 */
/***/ (function(module, exports, __webpack_require__) {

	var rvc = __webpack_require__(8);

	var koPunches = (function () {
	  var self = this;

	  // Behaviors
	  ko.filters.toDate = function (value) {
	    if ( value == null ) {
	      return null;
	    }

	    var dateStr = null,
	      input = new Date(ko.unwrap(value));

	      var month = (input.getUTCMonth() + 1).toString();
	      month = month.length === 2 ? month : "0" + month;

	      var day = input.getUTCDate().toString();
	      day = day.length === 2 ? day : "0" + day;

	    dateStr = month + '/' + day + '/' + input.getUTCFullYear();

	    return dateStr;
	  };

	  ko.filters.toDateTime = function ( value, format ) {
	    var dateObj;

	    if (typeof value === "string") {
	      dateObj = new Date( value );
	    }

	    if ( !(dateObj instanceof Date) ) {
	      throw new Error( 'Invalid input. Expected date but encountered ' + (typeof input) + '.' );
	    }

	    return dateObj.format( format || 'm/d/yyyy hh:MM TT' );
	  };

	  ko.filters.lotKey = function (input) {
	    var value = ko.unwrap(input);

	    if (value == undefined) {
	      return '';
	    }

	    var key = value.toString().replace(/ /g, '');
	    var keyLength = key.length;

	    if (keyLength === 0) {
	      return '';
	    } else if (keyLength <= 2) {
	      return key;
	    } else if (keyLength <= 4) {
	      return [key.substr(0, 2), key.substr(2)].join(' ');
	    } else if (keyLength <= 7) {
	      return [key.substr(0, 2), key.substr(2, 2), key.substr(4,3)].join(' ');
	    } else {
	      return [key.substr(0, 2), key.substr(2, 2), key.substr(4,3), key.substr(7)].join(' ');
	    }
	  };

	  ko.filters.toteKey = function (input) {
	    var value = ko.unwrap(input);

	    if (value == undefined) {
	      return '';
	    }

	    var key = value.toString().replace(/ /g, '');
	    if (key.length === 0) {
	      return '';
	    } else if (key.length <= 2) {
	      return key;
	    } else if (key.length <= 4) {
	      return [key.substr(0, 2), key.substr(2)].join(' ');
	    } else {
	      return [key.substr(0, 2), key.substr(2, 2), key.substr(4)].join(' ');
	    }
	  };

	    ko.filters.secToHrMin = function(value) {
	      var valueNum = +ko.unwrap(value);
	      var isNegative = valueNum < 0;

	      // Parse as positive number, Math.floor rounds negative numbers down
	      // Ex: -0.2 = -1
	      if (!isNaN(valueNum)) {
	        var secondsTotal = isNegative ? -valueNum : valueNum;
	        var hours = Math.floor(secondsTotal / 3600);
	        var minutes = Math.floor((secondsTotal - (3600 * hours)) / 60);

	        return isNegative ?
	          "".concat('-', hours, 'h ', minutes, 'm') :
	          "".concat(hours, 'h ', minutes, 'm');
	      } else {
	        return '0m';
	      }
	    };

	  ko.filters.USD = function(value) {
	    var amt = parseFloat(ko.unwrap(value));

	    return typeof amt === 'number' ?
	      '$' + amt.toFixed(2) :
	      '';
	  };

	  ko.filters.toFixed = function(value, numOfDigits) {
	    var amt = parseFloat(ko.unwrap(value));

	    return typeof amt === 'number' ?
	      amt.toFixed(numOfDigits || 2) :
	      '';
	  };

	  ko.filters.toNumber = function( value ) {
	    var numValue = value != null ? +value : null;

	    if ( numValue != null ) {
	      return numValue.toLocaleString();
	    } else {
	      return null;
	    }
	  };

	  ko.filters.contractStatus = function ( value ) {
	    var input = ko.unwrap( value );
	    var statuses = rvc.lists.contractStatuses;
	    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
	      return statuses[ status ].key === input;
	    });

	    return statuses[ statusKey ].value;
	  };

	  ko.filters.orderStatus = function ( value ) {
	    var input = ko.unwrap( value );
	    var statuses = rvc.lists.orderStatus;
	    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
	      return statuses[ status ].key === input;
	    });

	    return statuses[ statusKey ].value;
	  };

	  ko.filters.sampleStatus = function( value ) {
	    var input = ko.unwrap( value );
	    var statuses = rvc.lists.sampleStatusTypes;
	    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
	      return statuses[ status ].key === input;
	    });

	    return statuses[ statusKey ].value;
	  };

	  ko.filters.statusType = function( value, statusName ) {
	    var input = ko.unwrap( value );

	    if ( input == null ) {
	      return;
	    }

	    var statuses = rvc.lists[ statusName ];
	    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
	      return statuses[ status ].key === input;
	    });

	    return statusKey != null ? statuses[ statusKey ].value : null;
	  };

	  ko.filters.name = function( value ) {
	    var input = ko.unwrap( value );

	    return input && input.Name;
	  };

	  ko.filters.length = function( value ) {
	    if ( typeof value === 'string' || Array.isArray( value ) ) {
	      return value.length;
	    }

	    return 0;
	  };

	  // Exports
	  return this;
	})();

	module.exports = koPunches;


/***/ }),
/* 19 */,
/* 20 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {/**
	  * @param {Object[]} productsSource - Observable, Product options
	  * @param {string} selectedValue - Observable, Selected option
	  * @param {string} optionsDisplay - Property to use for text display
	  * @param {string} optionsValue - Property to use for object value
	  * @param {string} loading - Observable, Toggles input based on status
	  * @param {string} lotType - Observable, optional, enables use of the component without the requirement of providing the source products list and configurations.
	  * @param {bool} disabled - Observable, optional, enables control of the disabled state
	  */

	__webpack_require__(21);
	__webpack_require__(22);
	__webpack_require__(23);
	var productsService = __webpack_require__(24);
	var productsCache = {};

	function ProductSelectorVM(params) {
	  if (!(this instanceof ProductSelectorVM)) { return new ProductSelectorVM(params); }

	  var self = this;
	  var disposables = [];

	  var options = $.extend({}, self.DEFAULT_OPTIONS, params);

	  // Data
	  /** Init options and data */
	  this.options = ko.isObservable( options.productsSource ) ?
	    options.productsSource :
	    ko.observableArray( options.productsSource || [] );
	  this.optionsDisplay = options.optionsDisplay;
	  this.optionsValue = options.optionsValue;
	  this.selectorValue = options.selectedValue;
	  this.loading = options.loading;
	  this.controlId = options.controlId;
	  this.disabled = options.disabled || false;
	  this.enabled = options.enabled || true;

	  var init = this.initAsync(options);

	  init.done(function () {
	    // convert initially selected object into JS object instance
	    var initialValue = options.selectedValue.peek() || null;
	    var valueMember = ko.unwrap(options.optionsValue);
	    if (initialValue != null && valueMember == null) {
	      initialValue = ko.utils.arrayFirst(self.options(), function findItem(opt) {
	        return opt.ProductKey === initialValue.ProductKey;
	      });
	    }
	    self.selectorValue(initialValue);
	  });

	  /** Toggles loading state */
	  this.isLoading = ko.pureComputed(function() {
	    return ko.unwrap(self.loading) || false;
	  });

	  if (ko.isObservable(params.lotType)) {
	    params.lotType.subscribe(function(val) {
	      self.loadProductsByType(val);
	    });
	  }
	  self.loadProductsByType(ko.unwrap(params.lotType));

	  if ( ko.isObservable( options.selectedValue )) {
	    disposables.push( options.selectedValue.subscribe(function(val) {
	      if (ko.utils.arrayIndexOf(self.options(), val) > -1) {
	        return;
	      }

	      var valueMember = ko.unwrap(options.optionsValue);
	      if (valueMember == null && typeof val === "string") {
	        val = ko.utils.arrayFirst(self.options(), function(o) {
	          return o.ProductKey === val;
	        });
	      } else if (valueMember != null) {
	        val = ko.utils.arrayFirst(self.options(), function (o) {
	          return o[valueMember] === val;
	        });
	      } else {
	        val = null;
	      }

	      self.selectorValue(val);
	    }) );
	  }

	  this.dispose = function() {
	    ko.utils.arrayForEach(disposables, function(d) {
	      d.dispose();
	    });
	  };

	  return this;
	}

	module.exports = {
	  viewModel: ProductSelectorVM,
	  template: __webpack_require__(25)
	};

	ProductSelectorVM.prototype.DEFAULT_OPTIONS = {
	  optionsDisplay: 'ProductNameFull',
	  optionsValue: null,
	  loading: ko.observable(false),
	  controlId: null,
	  disabled: false
	};

	ProductSelectorVM.prototype.initAsync = function(options) {
	  var self = this;

	  if (options.productsSource == null) {
	    return self.loadProductsByType(options.lotType);
	  }

	  return $.Deferred().resolve();
	};

	ProductSelectorVM.prototype.loadProductsByType = function (lotType) {
	  lotType = ko.unwrap(lotType);
	  if (lotType == null) {
	    return $.Deferred().reject();
	  }

	  var self = this;
	  self.loading(true);

	  var cache = productsCache[lotType];
	  if (cache != null) {
	    self.options(cache);
	    self.loading(false);
	    return $.Deferred().resolve(cache);
	  }

	  return productsService.getProductsByLotType(ko.unwrap(lotType))
	    .done(function(data) {
	      self.options(data);
	      productsCache[lotType] = data;
	    })
	    .always(function() {
	      self.loading(false);
	    });
	};

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 21 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(jQuery) {/*** IMPORTS FROM imports-loader ***/
	var define = false;
	(function() {

	// knockout-jqautocomplete 0.4.4 | (c) 2016 Ryan Niemeyer |  http://www.opensource.org/licenses/mit-license
	;(function(factory) {
	    if (typeof define === "function" && define.amd) {
	        // AMD anonymous module
	        define(["knockout", "jquery", "jquery-ui/autocomplete"], factory);
	    } else {
	        // No module loader - put directly in global namespace
	        factory(window.ko, jQuery);
	    }
	})(function(ko, $) {
	    var JqAuto = function() {
	        var self = this,
	            unwrap = ko.utils.unwrapObservable; //support older KO versions that did not have ko.unwrap

	        //binding's init function
	        this.init = function(element, valueAccessor, allBindings, data, context) {
	            var existingSelect, existingChange,
	                options = unwrap(valueAccessor()),
	                config = {},
	                filter = typeof options.filter === "function" ? options.filter : self.defaultFilter;

	            //extend with global options
	            ko.utils.extend(config, self.options);
	            //override with options passed in binding
	            ko.utils.extend(config, options.options);

	            //get source from a function (can be remote call)
	            if (typeof options.source === "function" && !ko.isObservable(options.source)) {
	                config.source = function(request, response) {
	                    //provide a wrapper to the normal response callback
	                    var callback = function(data) {
	                        self.processOptions(valueAccessor, null, data, request, response);
	                    };

	                    //call the provided function for retrieving data
	                    options.source.call(context.$data, request.term, callback);
	                };
	            }
	            else {
	                //process local data
	                config.source = self.processOptions.bind(self, valueAccessor, filter, options.source);
	            }

	            //save any passed in select/change calls
	            existingSelect = typeof config.select === "function" && config.select;
	            existingChange = typeof config.change === "function" && config.change;

	            //handle updating the actual value
	            config.select = function(event, ui) {
	                if (ui.item && ui.item.actual) {
	                    options.value(ui.item.actual);

	                    if (ko.isWriteableObservable(options.dataValue)) {
	                        options.dataValue(ui.item.data);
	                    }
	                }

	                if (existingSelect) {
	                    existingSelect.apply(this, arguments);
	                }
	            };

	            //user made a change without selecting a value from the list
	            config.change = function(event, ui) {
	                if (!ui.item || !ui.item.actual) {
	                    options.value(event.target && event.target.value);

	                    if (ko.isWriteableObservable(options.dataValue)) {
	                        options.dataValue(null);
	                    }
	                }

	                if (existingChange) {
	                    existingChange.apply(this, arguments);
	                }
	            };

	            //initialize the widget
	            var widget = $(element).autocomplete(config).data("ui-autocomplete");

	            //render a template for the items
	            if (options.template) {
	                widget._renderItem = self.renderItem.bind(self, options.template, context);
	            }

	            //destroy the widget if KO removes the element
	            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
	                if (widget && typeof widget.destroy === "function") {
	                    widget.destroy();
	                    widget = null;
	                }
	            });
	        };

	        //the binding's update function. keep value in sync with model
	        this.update = function(element, valueAccessor) {
	            var propNames, sources,
	                options = unwrap(valueAccessor()),
	                value = unwrap(options && options.value);

	            if (!value && value !== 0) {
	                value = "";
	            }

	            // find the appropriate value for the input
	            sources = unwrap(options.source);
	            propNames = self.getPropertyNames(valueAccessor);

	            // if there is local data, then try to determine the appropriate value for the input
	            if ($.isArray(sources) && propNames.value) {
	                value = ko.utils.arrayFirst(sources, function (opt) {
	                        return opt[propNames.value] == value;
	                    }
	                ) || value;
	            }

	            if (propNames.input && value && typeof value === "object") {
	                element.value = value[propNames.input];
	            }
	            else {
	                element.value = value;
	            }
	        };

	        //if dealing with local data, the default filtering function
	        this.defaultFilter = function(item, term) {
	            term = term && term.toLowerCase();
	            return (item || item === 0) && ko.toJSON(item).toLowerCase().indexOf(term) > -1;
	        };

	        //filter/map options to be in a format that autocomplete requires
	        this.processOptions = function(valueAccessor, filter, data, request, response) {
	            var item, index, length,
	                items = unwrap(data) || [],
	                results = [],
	                props = this.getPropertyNames(valueAccessor);

	            //filter/map items
	            for (index = 0, length = items.length; index < length; index++) {
	                item = items[index];

	                if (!filter || filter(item, request.term)) {
	                    results.push({
	                        label: props.label ? item[props.label] : item.toString(),
	                        value: props.input ? item[props.input] : item.toString(),
	                        actual: props.value ? item[props.value] : item,
	                        data: item
	                    });
	                }
	            }

	            //call autocomplete callback to display list
	            response(results);
	        };

	        //if specified, use a template to render an item
	        this.renderItem = function(templateName, context, ul, item) {
	            var $li = $("<li></li>").appendTo(ul),
	                itemContext = context.createChildContext(item.data);

	            //apply the template binding
	            ko.applyBindingsToNode($li[0], { template: templateName }, itemContext);

	            //clean up
	            $li.one("remove", ko.cleanNode.bind(ko, $li[0]));

	            return $li;
	        };

	        //retrieve the property names to use for the label, input, and value
	        this.getPropertyNames = function(valueAccessor) {
	            var options = ko.toJS(valueAccessor());

	            return {
	                label: options.labelProp || options.valueProp,
	                input: options.inputProp || options.labelProp || options.valueProp,
	                value: options.valueProp
	            };
	        };

	        //default global options passed into autocomplete widget
	        this.options = {
	            autoFocus: true,
	            delay: 50
	        };
	    };

	    ko.bindingHandlers.jqAuto = new JqAuto();
	});

	}.call(window));
	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 22 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_FACTORY__, __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/*** IMPORTS FROM imports-loader ***/
	var require = false;

	/* http://www.reddnet.net/knockout-js-extender-for-dates-in-iso-8601-format/ 
	 * Knockout extender for dates that are round-tripped in ISO 8601 format
	 *  Depends on knockout.js and date.format.js 
	 *  Includes extensions for the date object that: 
	 *      add Date.toISOString() for browsers that do not nativly implement it
	 *      replaces Date.parse() with version to supports ISO 8601 (IE and Safari do not)
	 *  Includes example of how to use the extended binding
	 */

	(function () {
	    if (require) {
	        !(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9)], __WEBPACK_AMD_DEFINE_FACTORY__ = (extendKo), __WEBPACK_AMD_DEFINE_RESULT__ = (typeof __WEBPACK_AMD_DEFINE_FACTORY__ === 'function' ? (__WEBPACK_AMD_DEFINE_FACTORY__.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__)) : __WEBPACK_AMD_DEFINE_FACTORY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));
	    } else {
	        extendKo(ko);
	    }
	    
	    function extendKo(ko) {
	        ko.extenders.isoDate = function (target, formatString) {
	            target.formattedDate = ko.computed({
	                read: function () {
	                    if (!target()) {
	                        return;
	                    }
	                    var dt = new Date(Date.parse(target()));
	                    // Modified to not change to UTC. Should not effect 
	                    // anything NOT using time formatting.
	                    return dt.format(formatString);
	                    //return dt.format(formatString, true);
	                },
	                write: function (value) {
	                    // Modified from original to enable setting value to null
	                    // previously, when the value was "falsey", assignment was 
	                    // skipped all together. -- VK 5/27/13
	                    if (!value) {
	                        target(null);
	                    } else {
	                        target(new Date(Date.parse(value)).toISOString());
	                    }
	                }
	            });

	            target.asDate = ko.computed(function () {
	                return new Date(target.formattedDate());
	            });

	            //initialize with current value
	            target.formattedDate(target());

	            //return the computed observable
	            return target;
	        };
	    }
	}());


	/** from the mozilla documentation (before they implemented the function in the browser)
	 * https://developer.mozilla.org/index.php?title=en/JavaScript/Reference/Global_Objects/Date&revision=65
	 */
	(function(Date) {
	    if (!Date.prototype.toISOString) {
	        Date.prototype.toISOString = function() {
	            function pad(n) {
	                return n < 10 ? '0' + n : n;
	            }
	            return this.getUTCFullYear() + '-' + pad(this.getUTCMonth() + 1) + '-' + pad(this.getUTCDate()) + 'T' + pad(this.getUTCHours()) + ':' + pad(this.getUTCMinutes()) + ':' + pad(this.getUTCSeconds()) + 'Z';
	        };
	    }
	}(Date));

	/**
	 * Date.parse with progressive enhancement for ISO 8601 <https://github.com/csnover/js-iso8601>
	 * © 2011 Colin Snover <http://zetafleet.com>
	 * Released under MIT license.
	 */
	(function(Date) {
	    var origParse = Date.parse,
	        numericKeys = [1, 4, 5, 6, 7, 10, 11];
	    Date.parse = function(date) {
	        var timestamp, struct, minutesOffset = 0;

	        // ES5 §15.9.4.2 states that the string should attempt to be parsed as a Date Time String Format string
	        // before falling back to any implementation-specific date parsing, so that’s what we do, even if native
	        // implementations could be faster
	        //              1 YYYY                2 MM       3 DD           4 HH    5 mm       6 ss        7 msec        8 Z 9 ±    10 tzHH    11 tzmm
	        if ((struct = /^(\d{4}|[+\-]\d{6})(?:-(\d{2})(?:-(\d{2}))?)?(?:T(\d{2}):(\d{2})(?::(\d{2})(?:\.(\d{3}))?)?(?:(Z)|([+\-])(\d{2})(?::(\d{2}))?)?)?$/.exec(date))) {
	            // avoid NaN timestamps caused by “undefined” values being passed to Date.UTC
	            for (var i = 0, k;
	            (k = numericKeys[i]); ++i) {
	                struct[k] = +struct[k] || 0;
	            }

	            // allow undefined days and months
	            struct[2] = (+struct[2] || 1) - 1;
	            struct[3] = +struct[3] || 1;

	            if (struct[8] !== 'Z' && struct[9] !== 'undefined') {
	                minutesOffset = struct[10] * 60 + struct[11];

	                if (struct[9] === '+') {
	                    minutesOffset = 0 - minutesOffset;
	                }
	            }

	            timestamp = Date.UTC(struct[1], struct[2], struct[3], struct[4], struct[5] + minutesOffset, struct[6], struct[7]);
	        }
	        else {
	            timestamp = origParse ? origParse(date) : NaN;
	        }

	        return timestamp;
	    };
	}(Date));


/***/ }),
/* 23 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {(function() {
	  ko.bindingHandlers.datePicker = {
	    init: function (element, valueAccessor, allBindings) {
	      $(element).wrap('<div class="input-group"></div>');
	      $(element).datepicker({
	        showOn: 'button',
	        buttonText: '<i class="fa fa-calendar"></i>',
	        changeMonth: true,
	        changeYear: true
	      }).next(".ui-datepicker-trigger")
	          .addClass("btn btn-default")
	          .attr( 'tabindex', '-1' )
	          .wrap('<span class="input-group-btn"></span>');

	      ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	        //todo: cleanup wrapper element
	        $(element).datepicker('destroy');
	      });

	      var value = valueAccessor();
	      if (ko.isObservable(value)) {
	        ko.bindingHandlers.value.init(element, valueAccessor, allBindings);
	      }
	    }
	  };
	}());

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 24 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(7), __webpack_require__(8)], __WEBPACK_AMD_DEFINE_RESULT__ = function(core, app) {
	    function getProductsByInventoryType(inventoryType, options) {
	      options = options || {};
	      options = $.extend({
	        filterProductsWithInventory: false,
	        includeInactive: false
	      }, options);

	      var url = ['/api/products/', inventoryType].join('');

	      var qs = [];
	      if (options.lotType != null) {
	        qs.push('lotType=' + options.lotType);
	      }
	      if (options.filterProductsWithInventory) {
	        qs.push('filterProductsWithInventory=true');
	      }
	      if (options.includeInactive) {
	        qs.push('includeInactive=true');
	      }

	      if (qs.length > 0) {
	        url += '?' + qs.join('&');
	      }

	      return core.ajax(url, options);
	    }

	    function getProductsByLotType(lotType, options) {
	      var inventoryType = app.lists.lotTypes.findByKey(lotType).inventoryType.key;
	      options = options || {};
	      options.lotType = lotType;
	      return getProductsByInventoryType(inventoryType, options);
	    }

	    return {
	        getChileProducts: function (chileState) {
	            if (chileState && typeof chileState === "object") chileState = chileState.key;
	            return core.ajax(core.buildUrl(buildChileProductsUrl, chileState));
	        },
	        getPackagingProducts: function (options) { return getProductsByLotType(app.lists.lotTypes.Packaging.key, options); },
	        getProductTypeAttributes: function () {
	            return core.ajax("/api/productTypeAttributes");
	        },
	        getCustomerProducts: function( customerKey ) {
	          return core.ajax( '/api/customers/' + customerKey + '/productspecs' );
	        },
	        getCustomerProductDetails: function( customerKey, productKey ) {
	          return core.ajax( '/api/customers/' + customerKey + '/productSpecs/' + productKey );
	        },
	        createCustomerProductOverride: function( customerKey, productKey, overrideData ) {
	          return core.ajaxPost( '/api/customers/' + customerKey + '/productSpecs/' + productKey, overrideData );
	        },
	        deleteCustomerProductOverride: function( customerKey, productKey ) {
	          return core.ajaxDelete( '/api/customers/' + customerKey + '/productSpecs/' + productKey );
	        },
	        getProductDetails: core.setupFn(getProductDetails, buildProductUrl),
	        getProductsByLotType: getProductsByLotType,
	        getProductsByInventoryType: getProductsByInventoryType,
	        getChileVarieties: function() {
	          return core.ajax('/api/chilevarities');
	        },
	        getChileTypes: function () {
	            return core.ajax("/api/chileTypes");
	        },
	        getAdditiveTypes: function () {
	            return core.ajax("/api/additiveTypes");
	        },
	        getProductionLocations: function() {
	          return core.ajax('/api/productionlines');
	        },
	        createProduct: function( data ) {
	          return core.ajaxPost( '/api/products', data );
	        },
	        updateProduct: function( productCode, data ) {
	          return core.ajaxPut( '/api/products/' + productCode, data );
	        },
	        setProductIngredients: function( productKey, data ) {
	          return core.ajaxPost( '/api/products/' + productKey + '/ingredients', data );
	        },
	        setProductAttributes: function( productKey, data ) {
	          return core.ajaxPost( '/api/products/' + productKey + '/specs', data );
	        },
	        getProductionSchedulesDataPager: function( options ) {
	          options = options || {};

	          return core.pagedDataHelper.init({
	              urlBase: options.baseUrl || "/api/productionschedules",
	              pageSize: options.pageSize || 50,
	              parameters: options.parameters,
	              onNewPageSet: options.onNewPageSet,
	              onEndOfResults: options.onEndOfResults
	          });
	        },
	        getProductionScheduleDetails: function( key ) {
	          return core.ajax( '/api/productionschedules/' + key );
	        },
	        createProductionSchedule: function( data ) {
	          return core.ajaxPost( '/api/productionschedules/', data );
	        },
	        updateProductionSchedule: function( key, data ) {
	          return core.ajaxPut( '/api/productionschedules/' + key, data );
	        },
	        deleteProductionSchedule: function( key ) {
	          return core.ajaxDelete( '/api/productionschedules/' + key );
	        }
	    };

	    //#region function delegates
	    function getProductDetails(lotType, key) {
	        return core.ajax(buildProductUrl(lotType, key));
	    }
	    //#endregion

	    function buildProductUrl(lotType, key) {
	        key = key || '';
	        return '/api/products/' + lotType + (key ? '/' + key : '');
	    }
	    function buildChileProductsUrl(chileState) {
	        return '/api/chileproducts?chileState=' + chileState;
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 25 */
/***/ (function(module, exports) {

	module.exports = "<input type=\"text\" class=\"form-control\" data-bind=\"jqAuto: { value: selectorValue, source: options, labelProp: optionsDisplay, valueProp: optionsValue }, visible: !isLoading(), attr: { 'id': controlId }, enable: enabled, disable: disabled\">\n\n<div class=\"well well-sm\" data-bind=\"visible: isLoading\">\n  <i class=\"fa fa-spinner fa-pulse\"></i>\n</div>\n\n"

/***/ }),
/* 26 */,
/* 27 */,
/* 28 */,
/* 29 */,
/* 30 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(jQuery) {/*!
	 * Bootstrap v3.3.7 (http://getbootstrap.com)
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under the MIT license
	 */

	if (typeof jQuery === 'undefined') {
	  throw new Error('Bootstrap\'s JavaScript requires jQuery')
	}

	+function ($) {
	  'use strict';
	  var version = $.fn.jquery.split(' ')[0].split('.')
	  if ((version[0] < 2 && version[1] < 9) || (version[0] == 1 && version[1] == 9 && version[2] < 1) || (version[0] > 3)) {
	    throw new Error('Bootstrap\'s JavaScript requires jQuery version 1.9.1 or higher, but lower than version 4')
	  }
	}(jQuery);

	/* ========================================================================
	 * Bootstrap: transition.js v3.3.7
	 * http://getbootstrap.com/javascript/#transitions
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // CSS TRANSITION SUPPORT (Shoutout: http://www.modernizr.com/)
	  // ============================================================

	  function transitionEnd() {
	    var el = document.createElement('bootstrap')

	    var transEndEventNames = {
	      WebkitTransition : 'webkitTransitionEnd',
	      MozTransition    : 'transitionend',
	      OTransition      : 'oTransitionEnd otransitionend',
	      transition       : 'transitionend'
	    }

	    for (var name in transEndEventNames) {
	      if (el.style[name] !== undefined) {
	        return { end: transEndEventNames[name] }
	      }
	    }

	    return false // explicit for ie8 (  ._.)
	  }

	  // http://blog.alexmaccaw.com/css-transitions
	  $.fn.emulateTransitionEnd = function (duration) {
	    var called = false
	    var $el = this
	    $(this).one('bsTransitionEnd', function () { called = true })
	    var callback = function () { if (!called) $($el).trigger($.support.transition.end) }
	    setTimeout(callback, duration)
	    return this
	  }

	  $(function () {
	    $.support.transition = transitionEnd()

	    if (!$.support.transition) return

	    $.event.special.bsTransitionEnd = {
	      bindType: $.support.transition.end,
	      delegateType: $.support.transition.end,
	      handle: function (e) {
	        if ($(e.target).is(this)) return e.handleObj.handler.apply(this, arguments)
	      }
	    }
	  })

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: alert.js v3.3.7
	 * http://getbootstrap.com/javascript/#alerts
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // ALERT CLASS DEFINITION
	  // ======================

	  var dismiss = '[data-dismiss="alert"]'
	  var Alert   = function (el) {
	    $(el).on('click', dismiss, this.close)
	  }

	  Alert.VERSION = '3.3.7'

	  Alert.TRANSITION_DURATION = 150

	  Alert.prototype.close = function (e) {
	    var $this    = $(this)
	    var selector = $this.attr('data-target')

	    if (!selector) {
	      selector = $this.attr('href')
	      selector = selector && selector.replace(/.*(?=#[^\s]*$)/, '') // strip for ie7
	    }

	    var $parent = $(selector === '#' ? [] : selector)

	    if (e) e.preventDefault()

	    if (!$parent.length) {
	      $parent = $this.closest('.alert')
	    }

	    $parent.trigger(e = $.Event('close.bs.alert'))

	    if (e.isDefaultPrevented()) return

	    $parent.removeClass('in')

	    function removeElement() {
	      // detach from parent, fire event then clean up data
	      $parent.detach().trigger('closed.bs.alert').remove()
	    }

	    $.support.transition && $parent.hasClass('fade') ?
	      $parent
	        .one('bsTransitionEnd', removeElement)
	        .emulateTransitionEnd(Alert.TRANSITION_DURATION) :
	      removeElement()
	  }


	  // ALERT PLUGIN DEFINITION
	  // =======================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this = $(this)
	      var data  = $this.data('bs.alert')

	      if (!data) $this.data('bs.alert', (data = new Alert(this)))
	      if (typeof option == 'string') data[option].call($this)
	    })
	  }

	  var old = $.fn.alert

	  $.fn.alert             = Plugin
	  $.fn.alert.Constructor = Alert


	  // ALERT NO CONFLICT
	  // =================

	  $.fn.alert.noConflict = function () {
	    $.fn.alert = old
	    return this
	  }


	  // ALERT DATA-API
	  // ==============

	  $(document).on('click.bs.alert.data-api', dismiss, Alert.prototype.close)

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: button.js v3.3.7
	 * http://getbootstrap.com/javascript/#buttons
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // BUTTON PUBLIC CLASS DEFINITION
	  // ==============================

	  var Button = function (element, options) {
	    this.$element  = $(element)
	    this.options   = $.extend({}, Button.DEFAULTS, options)
	    this.isLoading = false
	  }

	  Button.VERSION  = '3.3.7'

	  Button.DEFAULTS = {
	    loadingText: 'loading...'
	  }

	  Button.prototype.setState = function (state) {
	    var d    = 'disabled'
	    var $el  = this.$element
	    var val  = $el.is('input') ? 'val' : 'html'
	    var data = $el.data()

	    state += 'Text'

	    if (data.resetText == null) $el.data('resetText', $el[val]())

	    // push to event loop to allow forms to submit
	    setTimeout($.proxy(function () {
	      $el[val](data[state] == null ? this.options[state] : data[state])

	      if (state == 'loadingText') {
	        this.isLoading = true
	        $el.addClass(d).attr(d, d).prop(d, true)
	      } else if (this.isLoading) {
	        this.isLoading = false
	        $el.removeClass(d).removeAttr(d).prop(d, false)
	      }
	    }, this), 0)
	  }

	  Button.prototype.toggle = function () {
	    var changed = true
	    var $parent = this.$element.closest('[data-toggle="buttons"]')

	    if ($parent.length) {
	      var $input = this.$element.find('input')
	      if ($input.prop('type') == 'radio') {
	        if ($input.prop('checked')) changed = false
	        $parent.find('.active').removeClass('active')
	        this.$element.addClass('active')
	      } else if ($input.prop('type') == 'checkbox') {
	        if (($input.prop('checked')) !== this.$element.hasClass('active')) changed = false
	        this.$element.toggleClass('active')
	      }
	      $input.prop('checked', this.$element.hasClass('active'))
	      if (changed) $input.trigger('change')
	    } else {
	      this.$element.attr('aria-pressed', !this.$element.hasClass('active'))
	      this.$element.toggleClass('active')
	    }
	  }


	  // BUTTON PLUGIN DEFINITION
	  // ========================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.button')
	      var options = typeof option == 'object' && option

	      if (!data) $this.data('bs.button', (data = new Button(this, options)))

	      if (option == 'toggle') data.toggle()
	      else if (option) data.setState(option)
	    })
	  }

	  var old = $.fn.button

	  $.fn.button             = Plugin
	  $.fn.button.Constructor = Button


	  // BUTTON NO CONFLICT
	  // ==================

	  $.fn.button.noConflict = function () {
	    $.fn.button = old
	    return this
	  }


	  // BUTTON DATA-API
	  // ===============

	  $(document)
	    .on('click.bs.button.data-api', '[data-toggle^="button"]', function (e) {
	      var $btn = $(e.target).closest('.btn')
	      Plugin.call($btn, 'toggle')
	      if (!($(e.target).is('input[type="radio"], input[type="checkbox"]'))) {
	        // Prevent double click on radios, and the double selections (so cancellation) on checkboxes
	        e.preventDefault()
	        // The target component still receive the focus
	        if ($btn.is('input,button')) $btn.trigger('focus')
	        else $btn.find('input:visible,button:visible').first().trigger('focus')
	      }
	    })
	    .on('focus.bs.button.data-api blur.bs.button.data-api', '[data-toggle^="button"]', function (e) {
	      $(e.target).closest('.btn').toggleClass('focus', /^focus(in)?$/.test(e.type))
	    })

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: carousel.js v3.3.7
	 * http://getbootstrap.com/javascript/#carousel
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // CAROUSEL CLASS DEFINITION
	  // =========================

	  var Carousel = function (element, options) {
	    this.$element    = $(element)
	    this.$indicators = this.$element.find('.carousel-indicators')
	    this.options     = options
	    this.paused      = null
	    this.sliding     = null
	    this.interval    = null
	    this.$active     = null
	    this.$items      = null

	    this.options.keyboard && this.$element.on('keydown.bs.carousel', $.proxy(this.keydown, this))

	    this.options.pause == 'hover' && !('ontouchstart' in document.documentElement) && this.$element
	      .on('mouseenter.bs.carousel', $.proxy(this.pause, this))
	      .on('mouseleave.bs.carousel', $.proxy(this.cycle, this))
	  }

	  Carousel.VERSION  = '3.3.7'

	  Carousel.TRANSITION_DURATION = 600

	  Carousel.DEFAULTS = {
	    interval: 5000,
	    pause: 'hover',
	    wrap: true,
	    keyboard: true
	  }

	  Carousel.prototype.keydown = function (e) {
	    if (/input|textarea/i.test(e.target.tagName)) return
	    switch (e.which) {
	      case 37: this.prev(); break
	      case 39: this.next(); break
	      default: return
	    }

	    e.preventDefault()
	  }

	  Carousel.prototype.cycle = function (e) {
	    e || (this.paused = false)

	    this.interval && clearInterval(this.interval)

	    this.options.interval
	      && !this.paused
	      && (this.interval = setInterval($.proxy(this.next, this), this.options.interval))

	    return this
	  }

	  Carousel.prototype.getItemIndex = function (item) {
	    this.$items = item.parent().children('.item')
	    return this.$items.index(item || this.$active)
	  }

	  Carousel.prototype.getItemForDirection = function (direction, active) {
	    var activeIndex = this.getItemIndex(active)
	    var willWrap = (direction == 'prev' && activeIndex === 0)
	                || (direction == 'next' && activeIndex == (this.$items.length - 1))
	    if (willWrap && !this.options.wrap) return active
	    var delta = direction == 'prev' ? -1 : 1
	    var itemIndex = (activeIndex + delta) % this.$items.length
	    return this.$items.eq(itemIndex)
	  }

	  Carousel.prototype.to = function (pos) {
	    var that        = this
	    var activeIndex = this.getItemIndex(this.$active = this.$element.find('.item.active'))

	    if (pos > (this.$items.length - 1) || pos < 0) return

	    if (this.sliding)       return this.$element.one('slid.bs.carousel', function () { that.to(pos) }) // yes, "slid"
	    if (activeIndex == pos) return this.pause().cycle()

	    return this.slide(pos > activeIndex ? 'next' : 'prev', this.$items.eq(pos))
	  }

	  Carousel.prototype.pause = function (e) {
	    e || (this.paused = true)

	    if (this.$element.find('.next, .prev').length && $.support.transition) {
	      this.$element.trigger($.support.transition.end)
	      this.cycle(true)
	    }

	    this.interval = clearInterval(this.interval)

	    return this
	  }

	  Carousel.prototype.next = function () {
	    if (this.sliding) return
	    return this.slide('next')
	  }

	  Carousel.prototype.prev = function () {
	    if (this.sliding) return
	    return this.slide('prev')
	  }

	  Carousel.prototype.slide = function (type, next) {
	    var $active   = this.$element.find('.item.active')
	    var $next     = next || this.getItemForDirection(type, $active)
	    var isCycling = this.interval
	    var direction = type == 'next' ? 'left' : 'right'
	    var that      = this

	    if ($next.hasClass('active')) return (this.sliding = false)

	    var relatedTarget = $next[0]
	    var slideEvent = $.Event('slide.bs.carousel', {
	      relatedTarget: relatedTarget,
	      direction: direction
	    })
	    this.$element.trigger(slideEvent)
	    if (slideEvent.isDefaultPrevented()) return

	    this.sliding = true

	    isCycling && this.pause()

	    if (this.$indicators.length) {
	      this.$indicators.find('.active').removeClass('active')
	      var $nextIndicator = $(this.$indicators.children()[this.getItemIndex($next)])
	      $nextIndicator && $nextIndicator.addClass('active')
	    }

	    var slidEvent = $.Event('slid.bs.carousel', { relatedTarget: relatedTarget, direction: direction }) // yes, "slid"
	    if ($.support.transition && this.$element.hasClass('slide')) {
	      $next.addClass(type)
	      $next[0].offsetWidth // force reflow
	      $active.addClass(direction)
	      $next.addClass(direction)
	      $active
	        .one('bsTransitionEnd', function () {
	          $next.removeClass([type, direction].join(' ')).addClass('active')
	          $active.removeClass(['active', direction].join(' '))
	          that.sliding = false
	          setTimeout(function () {
	            that.$element.trigger(slidEvent)
	          }, 0)
	        })
	        .emulateTransitionEnd(Carousel.TRANSITION_DURATION)
	    } else {
	      $active.removeClass('active')
	      $next.addClass('active')
	      this.sliding = false
	      this.$element.trigger(slidEvent)
	    }

	    isCycling && this.cycle()

	    return this
	  }


	  // CAROUSEL PLUGIN DEFINITION
	  // ==========================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.carousel')
	      var options = $.extend({}, Carousel.DEFAULTS, $this.data(), typeof option == 'object' && option)
	      var action  = typeof option == 'string' ? option : options.slide

	      if (!data) $this.data('bs.carousel', (data = new Carousel(this, options)))
	      if (typeof option == 'number') data.to(option)
	      else if (action) data[action]()
	      else if (options.interval) data.pause().cycle()
	    })
	  }

	  var old = $.fn.carousel

	  $.fn.carousel             = Plugin
	  $.fn.carousel.Constructor = Carousel


	  // CAROUSEL NO CONFLICT
	  // ====================

	  $.fn.carousel.noConflict = function () {
	    $.fn.carousel = old
	    return this
	  }


	  // CAROUSEL DATA-API
	  // =================

	  var clickHandler = function (e) {
	    var href
	    var $this   = $(this)
	    var $target = $($this.attr('data-target') || (href = $this.attr('href')) && href.replace(/.*(?=#[^\s]+$)/, '')) // strip for ie7
	    if (!$target.hasClass('carousel')) return
	    var options = $.extend({}, $target.data(), $this.data())
	    var slideIndex = $this.attr('data-slide-to')
	    if (slideIndex) options.interval = false

	    Plugin.call($target, options)

	    if (slideIndex) {
	      $target.data('bs.carousel').to(slideIndex)
	    }

	    e.preventDefault()
	  }

	  $(document)
	    .on('click.bs.carousel.data-api', '[data-slide]', clickHandler)
	    .on('click.bs.carousel.data-api', '[data-slide-to]', clickHandler)

	  $(window).on('load', function () {
	    $('[data-ride="carousel"]').each(function () {
	      var $carousel = $(this)
	      Plugin.call($carousel, $carousel.data())
	    })
	  })

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: collapse.js v3.3.7
	 * http://getbootstrap.com/javascript/#collapse
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */

	/* jshint latedef: false */

	+function ($) {
	  'use strict';

	  // COLLAPSE PUBLIC CLASS DEFINITION
	  // ================================

	  var Collapse = function (element, options) {
	    this.$element      = $(element)
	    this.options       = $.extend({}, Collapse.DEFAULTS, options)
	    this.$trigger      = $('[data-toggle="collapse"][href="#' + element.id + '"],' +
	                           '[data-toggle="collapse"][data-target="#' + element.id + '"]')
	    this.transitioning = null

	    if (this.options.parent) {
	      this.$parent = this.getParent()
	    } else {
	      this.addAriaAndCollapsedClass(this.$element, this.$trigger)
	    }

	    if (this.options.toggle) this.toggle()
	  }

	  Collapse.VERSION  = '3.3.7'

	  Collapse.TRANSITION_DURATION = 350

	  Collapse.DEFAULTS = {
	    toggle: true
	  }

	  Collapse.prototype.dimension = function () {
	    var hasWidth = this.$element.hasClass('width')
	    return hasWidth ? 'width' : 'height'
	  }

	  Collapse.prototype.show = function () {
	    if (this.transitioning || this.$element.hasClass('in')) return

	    var activesData
	    var actives = this.$parent && this.$parent.children('.panel').children('.in, .collapsing')

	    if (actives && actives.length) {
	      activesData = actives.data('bs.collapse')
	      if (activesData && activesData.transitioning) return
	    }

	    var startEvent = $.Event('show.bs.collapse')
	    this.$element.trigger(startEvent)
	    if (startEvent.isDefaultPrevented()) return

	    if (actives && actives.length) {
	      Plugin.call(actives, 'hide')
	      activesData || actives.data('bs.collapse', null)
	    }

	    var dimension = this.dimension()

	    this.$element
	      .removeClass('collapse')
	      .addClass('collapsing')[dimension](0)
	      .attr('aria-expanded', true)

	    this.$trigger
	      .removeClass('collapsed')
	      .attr('aria-expanded', true)

	    this.transitioning = 1

	    var complete = function () {
	      this.$element
	        .removeClass('collapsing')
	        .addClass('collapse in')[dimension]('')
	      this.transitioning = 0
	      this.$element
	        .trigger('shown.bs.collapse')
	    }

	    if (!$.support.transition) return complete.call(this)

	    var scrollSize = $.camelCase(['scroll', dimension].join('-'))

	    this.$element
	      .one('bsTransitionEnd', $.proxy(complete, this))
	      .emulateTransitionEnd(Collapse.TRANSITION_DURATION)[dimension](this.$element[0][scrollSize])
	  }

	  Collapse.prototype.hide = function () {
	    if (this.transitioning || !this.$element.hasClass('in')) return

	    var startEvent = $.Event('hide.bs.collapse')
	    this.$element.trigger(startEvent)
	    if (startEvent.isDefaultPrevented()) return

	    var dimension = this.dimension()

	    this.$element[dimension](this.$element[dimension]())[0].offsetHeight

	    this.$element
	      .addClass('collapsing')
	      .removeClass('collapse in')
	      .attr('aria-expanded', false)

	    this.$trigger
	      .addClass('collapsed')
	      .attr('aria-expanded', false)

	    this.transitioning = 1

	    var complete = function () {
	      this.transitioning = 0
	      this.$element
	        .removeClass('collapsing')
	        .addClass('collapse')
	        .trigger('hidden.bs.collapse')
	    }

	    if (!$.support.transition) return complete.call(this)

	    this.$element
	      [dimension](0)
	      .one('bsTransitionEnd', $.proxy(complete, this))
	      .emulateTransitionEnd(Collapse.TRANSITION_DURATION)
	  }

	  Collapse.prototype.toggle = function () {
	    this[this.$element.hasClass('in') ? 'hide' : 'show']()
	  }

	  Collapse.prototype.getParent = function () {
	    return $(this.options.parent)
	      .find('[data-toggle="collapse"][data-parent="' + this.options.parent + '"]')
	      .each($.proxy(function (i, element) {
	        var $element = $(element)
	        this.addAriaAndCollapsedClass(getTargetFromTrigger($element), $element)
	      }, this))
	      .end()
	  }

	  Collapse.prototype.addAriaAndCollapsedClass = function ($element, $trigger) {
	    var isOpen = $element.hasClass('in')

	    $element.attr('aria-expanded', isOpen)
	    $trigger
	      .toggleClass('collapsed', !isOpen)
	      .attr('aria-expanded', isOpen)
	  }

	  function getTargetFromTrigger($trigger) {
	    var href
	    var target = $trigger.attr('data-target')
	      || (href = $trigger.attr('href')) && href.replace(/.*(?=#[^\s]+$)/, '') // strip for ie7

	    return $(target)
	  }


	  // COLLAPSE PLUGIN DEFINITION
	  // ==========================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.collapse')
	      var options = $.extend({}, Collapse.DEFAULTS, $this.data(), typeof option == 'object' && option)

	      if (!data && options.toggle && /show|hide/.test(option)) options.toggle = false
	      if (!data) $this.data('bs.collapse', (data = new Collapse(this, options)))
	      if (typeof option == 'string') data[option]()
	    })
	  }

	  var old = $.fn.collapse

	  $.fn.collapse             = Plugin
	  $.fn.collapse.Constructor = Collapse


	  // COLLAPSE NO CONFLICT
	  // ====================

	  $.fn.collapse.noConflict = function () {
	    $.fn.collapse = old
	    return this
	  }


	  // COLLAPSE DATA-API
	  // =================

	  $(document).on('click.bs.collapse.data-api', '[data-toggle="collapse"]', function (e) {
	    var $this   = $(this)

	    if (!$this.attr('data-target')) e.preventDefault()

	    var $target = getTargetFromTrigger($this)
	    var data    = $target.data('bs.collapse')
	    var option  = data ? 'toggle' : $this.data()

	    Plugin.call($target, option)
	  })

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: dropdown.js v3.3.7
	 * http://getbootstrap.com/javascript/#dropdowns
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // DROPDOWN CLASS DEFINITION
	  // =========================

	  var backdrop = '.dropdown-backdrop'
	  var toggle   = '[data-toggle="dropdown"]'
	  var Dropdown = function (element) {
	    $(element).on('click.bs.dropdown', this.toggle)
	  }

	  Dropdown.VERSION = '3.3.7'

	  function getParent($this) {
	    var selector = $this.attr('data-target')

	    if (!selector) {
	      selector = $this.attr('href')
	      selector = selector && /#[A-Za-z]/.test(selector) && selector.replace(/.*(?=#[^\s]*$)/, '') // strip for ie7
	    }

	    var $parent = selector && $(selector)

	    return $parent && $parent.length ? $parent : $this.parent()
	  }

	  function clearMenus(e) {
	    if (e && e.which === 3) return
	    $(backdrop).remove()
	    $(toggle).each(function () {
	      var $this         = $(this)
	      var $parent       = getParent($this)
	      var relatedTarget = { relatedTarget: this }

	      if (!$parent.hasClass('open')) return

	      if (e && e.type == 'click' && /input|textarea/i.test(e.target.tagName) && $.contains($parent[0], e.target)) return

	      $parent.trigger(e = $.Event('hide.bs.dropdown', relatedTarget))

	      if (e.isDefaultPrevented()) return

	      $this.attr('aria-expanded', 'false')
	      $parent.removeClass('open').trigger($.Event('hidden.bs.dropdown', relatedTarget))
	    })
	  }

	  Dropdown.prototype.toggle = function (e) {
	    var $this = $(this)

	    if ($this.is('.disabled, :disabled')) return

	    var $parent  = getParent($this)
	    var isActive = $parent.hasClass('open')

	    clearMenus()

	    if (!isActive) {
	      if ('ontouchstart' in document.documentElement && !$parent.closest('.navbar-nav').length) {
	        // if mobile we use a backdrop because click events don't delegate
	        $(document.createElement('div'))
	          .addClass('dropdown-backdrop')
	          .insertAfter($(this))
	          .on('click', clearMenus)
	      }

	      var relatedTarget = { relatedTarget: this }
	      $parent.trigger(e = $.Event('show.bs.dropdown', relatedTarget))

	      if (e.isDefaultPrevented()) return

	      $this
	        .trigger('focus')
	        .attr('aria-expanded', 'true')

	      $parent
	        .toggleClass('open')
	        .trigger($.Event('shown.bs.dropdown', relatedTarget))
	    }

	    return false
	  }

	  Dropdown.prototype.keydown = function (e) {
	    if (!/(38|40|27|32)/.test(e.which) || /input|textarea/i.test(e.target.tagName)) return

	    var $this = $(this)

	    e.preventDefault()
	    e.stopPropagation()

	    if ($this.is('.disabled, :disabled')) return

	    var $parent  = getParent($this)
	    var isActive = $parent.hasClass('open')

	    if (!isActive && e.which != 27 || isActive && e.which == 27) {
	      if (e.which == 27) $parent.find(toggle).trigger('focus')
	      return $this.trigger('click')
	    }

	    var desc = ' li:not(.disabled):visible a'
	    var $items = $parent.find('.dropdown-menu' + desc)

	    if (!$items.length) return

	    var index = $items.index(e.target)

	    if (e.which == 38 && index > 0)                 index--         // up
	    if (e.which == 40 && index < $items.length - 1) index++         // down
	    if (!~index)                                    index = 0

	    $items.eq(index).trigger('focus')
	  }


	  // DROPDOWN PLUGIN DEFINITION
	  // ==========================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this = $(this)
	      var data  = $this.data('bs.dropdown')

	      if (!data) $this.data('bs.dropdown', (data = new Dropdown(this)))
	      if (typeof option == 'string') data[option].call($this)
	    })
	  }

	  var old = $.fn.dropdown

	  $.fn.dropdown             = Plugin
	  $.fn.dropdown.Constructor = Dropdown


	  // DROPDOWN NO CONFLICT
	  // ====================

	  $.fn.dropdown.noConflict = function () {
	    $.fn.dropdown = old
	    return this
	  }


	  // APPLY TO STANDARD DROPDOWN ELEMENTS
	  // ===================================

	  $(document)
	    .on('click.bs.dropdown.data-api', clearMenus)
	    .on('click.bs.dropdown.data-api', '.dropdown form', function (e) { e.stopPropagation() })
	    .on('click.bs.dropdown.data-api', toggle, Dropdown.prototype.toggle)
	    .on('keydown.bs.dropdown.data-api', toggle, Dropdown.prototype.keydown)
	    .on('keydown.bs.dropdown.data-api', '.dropdown-menu', Dropdown.prototype.keydown)

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: modal.js v3.3.7
	 * http://getbootstrap.com/javascript/#modals
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // MODAL CLASS DEFINITION
	  // ======================

	  var Modal = function (element, options) {
	    this.options             = options
	    this.$body               = $(document.body)
	    this.$element            = $(element)
	    this.$dialog             = this.$element.find('.modal-dialog')
	    this.$backdrop           = null
	    this.isShown             = null
	    this.originalBodyPad     = null
	    this.scrollbarWidth      = 0
	    this.ignoreBackdropClick = false

	    if (this.options.remote) {
	      this.$element
	        .find('.modal-content')
	        .load(this.options.remote, $.proxy(function () {
	          this.$element.trigger('loaded.bs.modal')
	        }, this))
	    }
	  }

	  Modal.VERSION  = '3.3.7'

	  Modal.TRANSITION_DURATION = 300
	  Modal.BACKDROP_TRANSITION_DURATION = 150

	  Modal.DEFAULTS = {
	    backdrop: true,
	    keyboard: true,
	    show: true
	  }

	  Modal.prototype.toggle = function (_relatedTarget) {
	    return this.isShown ? this.hide() : this.show(_relatedTarget)
	  }

	  Modal.prototype.show = function (_relatedTarget) {
	    var that = this
	    var e    = $.Event('show.bs.modal', { relatedTarget: _relatedTarget })

	    this.$element.trigger(e)

	    if (this.isShown || e.isDefaultPrevented()) return

	    this.isShown = true

	    this.checkScrollbar()
	    this.setScrollbar()
	    this.$body.addClass('modal-open')

	    this.escape()
	    this.resize()

	    this.$element.on('click.dismiss.bs.modal', '[data-dismiss="modal"]', $.proxy(this.hide, this))

	    this.$dialog.on('mousedown.dismiss.bs.modal', function () {
	      that.$element.one('mouseup.dismiss.bs.modal', function (e) {
	        if ($(e.target).is(that.$element)) that.ignoreBackdropClick = true
	      })
	    })

	    this.backdrop(function () {
	      var transition = $.support.transition && that.$element.hasClass('fade')

	      if (!that.$element.parent().length) {
	        that.$element.appendTo(that.$body) // don't move modals dom position
	      }

	      that.$element
	        .show()
	        .scrollTop(0)

	      that.adjustDialog()

	      if (transition) {
	        that.$element[0].offsetWidth // force reflow
	      }

	      that.$element.addClass('in')

	      that.enforceFocus()

	      var e = $.Event('shown.bs.modal', { relatedTarget: _relatedTarget })

	      transition ?
	        that.$dialog // wait for modal to slide in
	          .one('bsTransitionEnd', function () {
	            that.$element.trigger('focus').trigger(e)
	          })
	          .emulateTransitionEnd(Modal.TRANSITION_DURATION) :
	        that.$element.trigger('focus').trigger(e)
	    })
	  }

	  Modal.prototype.hide = function (e) {
	    if (e) e.preventDefault()

	    e = $.Event('hide.bs.modal')

	    this.$element.trigger(e)

	    if (!this.isShown || e.isDefaultPrevented()) return

	    this.isShown = false

	    this.escape()
	    this.resize()

	    $(document).off('focusin.bs.modal')

	    this.$element
	      .removeClass('in')
	      .off('click.dismiss.bs.modal')
	      .off('mouseup.dismiss.bs.modal')

	    this.$dialog.off('mousedown.dismiss.bs.modal')

	    $.support.transition && this.$element.hasClass('fade') ?
	      this.$element
	        .one('bsTransitionEnd', $.proxy(this.hideModal, this))
	        .emulateTransitionEnd(Modal.TRANSITION_DURATION) :
	      this.hideModal()
	  }

	  Modal.prototype.enforceFocus = function () {
	    $(document)
	      .off('focusin.bs.modal') // guard against infinite focus loop
	      .on('focusin.bs.modal', $.proxy(function (e) {
	        if (document !== e.target &&
	            this.$element[0] !== e.target &&
	            !this.$element.has(e.target).length) {
	          this.$element.trigger('focus')
	        }
	      }, this))
	  }

	  Modal.prototype.escape = function () {
	    if (this.isShown && this.options.keyboard) {
	      this.$element.on('keydown.dismiss.bs.modal', $.proxy(function (e) {
	        e.which == 27 && this.hide()
	      }, this))
	    } else if (!this.isShown) {
	      this.$element.off('keydown.dismiss.bs.modal')
	    }
	  }

	  Modal.prototype.resize = function () {
	    if (this.isShown) {
	      $(window).on('resize.bs.modal', $.proxy(this.handleUpdate, this))
	    } else {
	      $(window).off('resize.bs.modal')
	    }
	  }

	  Modal.prototype.hideModal = function () {
	    var that = this
	    this.$element.hide()
	    this.backdrop(function () {
	      that.$body.removeClass('modal-open')
	      that.resetAdjustments()
	      that.resetScrollbar()
	      that.$element.trigger('hidden.bs.modal')
	    })
	  }

	  Modal.prototype.removeBackdrop = function () {
	    this.$backdrop && this.$backdrop.remove()
	    this.$backdrop = null
	  }

	  Modal.prototype.backdrop = function (callback) {
	    var that = this
	    var animate = this.$element.hasClass('fade') ? 'fade' : ''

	    if (this.isShown && this.options.backdrop) {
	      var doAnimate = $.support.transition && animate

	      this.$backdrop = $(document.createElement('div'))
	        .addClass('modal-backdrop ' + animate)
	        .appendTo(this.$body)

	      this.$element.on('click.dismiss.bs.modal', $.proxy(function (e) {
	        if (this.ignoreBackdropClick) {
	          this.ignoreBackdropClick = false
	          return
	        }
	        if (e.target !== e.currentTarget) return
	        this.options.backdrop == 'static'
	          ? this.$element[0].focus()
	          : this.hide()
	      }, this))

	      if (doAnimate) this.$backdrop[0].offsetWidth // force reflow

	      this.$backdrop.addClass('in')

	      if (!callback) return

	      doAnimate ?
	        this.$backdrop
	          .one('bsTransitionEnd', callback)
	          .emulateTransitionEnd(Modal.BACKDROP_TRANSITION_DURATION) :
	        callback()

	    } else if (!this.isShown && this.$backdrop) {
	      this.$backdrop.removeClass('in')

	      var callbackRemove = function () {
	        that.removeBackdrop()
	        callback && callback()
	      }
	      $.support.transition && this.$element.hasClass('fade') ?
	        this.$backdrop
	          .one('bsTransitionEnd', callbackRemove)
	          .emulateTransitionEnd(Modal.BACKDROP_TRANSITION_DURATION) :
	        callbackRemove()

	    } else if (callback) {
	      callback()
	    }
	  }

	  // these following methods are used to handle overflowing modals

	  Modal.prototype.handleUpdate = function () {
	    this.adjustDialog()
	  }

	  Modal.prototype.adjustDialog = function () {
	    var modalIsOverflowing = this.$element[0].scrollHeight > document.documentElement.clientHeight

	    this.$element.css({
	      paddingLeft:  !this.bodyIsOverflowing && modalIsOverflowing ? this.scrollbarWidth : '',
	      paddingRight: this.bodyIsOverflowing && !modalIsOverflowing ? this.scrollbarWidth : ''
	    })
	  }

	  Modal.prototype.resetAdjustments = function () {
	    this.$element.css({
	      paddingLeft: '',
	      paddingRight: ''
	    })
	  }

	  Modal.prototype.checkScrollbar = function () {
	    var fullWindowWidth = window.innerWidth
	    if (!fullWindowWidth) { // workaround for missing window.innerWidth in IE8
	      var documentElementRect = document.documentElement.getBoundingClientRect()
	      fullWindowWidth = documentElementRect.right - Math.abs(documentElementRect.left)
	    }
	    this.bodyIsOverflowing = document.body.clientWidth < fullWindowWidth
	    this.scrollbarWidth = this.measureScrollbar()
	  }

	  Modal.prototype.setScrollbar = function () {
	    var bodyPad = parseInt((this.$body.css('padding-right') || 0), 10)
	    this.originalBodyPad = document.body.style.paddingRight || ''
	    if (this.bodyIsOverflowing) this.$body.css('padding-right', bodyPad + this.scrollbarWidth)
	  }

	  Modal.prototype.resetScrollbar = function () {
	    this.$body.css('padding-right', this.originalBodyPad)
	  }

	  Modal.prototype.measureScrollbar = function () { // thx walsh
	    var scrollDiv = document.createElement('div')
	    scrollDiv.className = 'modal-scrollbar-measure'
	    this.$body.append(scrollDiv)
	    var scrollbarWidth = scrollDiv.offsetWidth - scrollDiv.clientWidth
	    this.$body[0].removeChild(scrollDiv)
	    return scrollbarWidth
	  }


	  // MODAL PLUGIN DEFINITION
	  // =======================

	  function Plugin(option, _relatedTarget) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.modal')
	      var options = $.extend({}, Modal.DEFAULTS, $this.data(), typeof option == 'object' && option)

	      if (!data) $this.data('bs.modal', (data = new Modal(this, options)))
	      if (typeof option == 'string') data[option](_relatedTarget)
	      else if (options.show) data.show(_relatedTarget)
	    })
	  }

	  var old = $.fn.modal

	  $.fn.modal             = Plugin
	  $.fn.modal.Constructor = Modal


	  // MODAL NO CONFLICT
	  // =================

	  $.fn.modal.noConflict = function () {
	    $.fn.modal = old
	    return this
	  }


	  // MODAL DATA-API
	  // ==============

	  $(document).on('click.bs.modal.data-api', '[data-toggle="modal"]', function (e) {
	    var $this   = $(this)
	    var href    = $this.attr('href')
	    var $target = $($this.attr('data-target') || (href && href.replace(/.*(?=#[^\s]+$)/, ''))) // strip for ie7
	    var option  = $target.data('bs.modal') ? 'toggle' : $.extend({ remote: !/#/.test(href) && href }, $target.data(), $this.data())

	    if ($this.is('a')) e.preventDefault()

	    $target.one('show.bs.modal', function (showEvent) {
	      if (showEvent.isDefaultPrevented()) return // only register focus restorer if modal will actually get shown
	      $target.one('hidden.bs.modal', function () {
	        $this.is(':visible') && $this.trigger('focus')
	      })
	    })
	    Plugin.call($target, option, this)
	  })

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: tooltip.js v3.3.7
	 * http://getbootstrap.com/javascript/#tooltip
	 * Inspired by the original jQuery.tipsy by Jason Frame
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // TOOLTIP PUBLIC CLASS DEFINITION
	  // ===============================

	  var Tooltip = function (element, options) {
	    this.type       = null
	    this.options    = null
	    this.enabled    = null
	    this.timeout    = null
	    this.hoverState = null
	    this.$element   = null
	    this.inState    = null

	    this.init('tooltip', element, options)
	  }

	  Tooltip.VERSION  = '3.3.7'

	  Tooltip.TRANSITION_DURATION = 150

	  Tooltip.DEFAULTS = {
	    animation: true,
	    placement: 'top',
	    selector: false,
	    template: '<div class="tooltip" role="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>',
	    trigger: 'hover focus',
	    title: '',
	    delay: 0,
	    html: false,
	    container: false,
	    viewport: {
	      selector: 'body',
	      padding: 0
	    }
	  }

	  Tooltip.prototype.init = function (type, element, options) {
	    this.enabled   = true
	    this.type      = type
	    this.$element  = $(element)
	    this.options   = this.getOptions(options)
	    this.$viewport = this.options.viewport && $($.isFunction(this.options.viewport) ? this.options.viewport.call(this, this.$element) : (this.options.viewport.selector || this.options.viewport))
	    this.inState   = { click: false, hover: false, focus: false }

	    if (this.$element[0] instanceof document.constructor && !this.options.selector) {
	      throw new Error('`selector` option must be specified when initializing ' + this.type + ' on the window.document object!')
	    }

	    var triggers = this.options.trigger.split(' ')

	    for (var i = triggers.length; i--;) {
	      var trigger = triggers[i]

	      if (trigger == 'click') {
	        this.$element.on('click.' + this.type, this.options.selector, $.proxy(this.toggle, this))
	      } else if (trigger != 'manual') {
	        var eventIn  = trigger == 'hover' ? 'mouseenter' : 'focusin'
	        var eventOut = trigger == 'hover' ? 'mouseleave' : 'focusout'

	        this.$element.on(eventIn  + '.' + this.type, this.options.selector, $.proxy(this.enter, this))
	        this.$element.on(eventOut + '.' + this.type, this.options.selector, $.proxy(this.leave, this))
	      }
	    }

	    this.options.selector ?
	      (this._options = $.extend({}, this.options, { trigger: 'manual', selector: '' })) :
	      this.fixTitle()
	  }

	  Tooltip.prototype.getDefaults = function () {
	    return Tooltip.DEFAULTS
	  }

	  Tooltip.prototype.getOptions = function (options) {
	    options = $.extend({}, this.getDefaults(), this.$element.data(), options)

	    if (options.delay && typeof options.delay == 'number') {
	      options.delay = {
	        show: options.delay,
	        hide: options.delay
	      }
	    }

	    return options
	  }

	  Tooltip.prototype.getDelegateOptions = function () {
	    var options  = {}
	    var defaults = this.getDefaults()

	    this._options && $.each(this._options, function (key, value) {
	      if (defaults[key] != value) options[key] = value
	    })

	    return options
	  }

	  Tooltip.prototype.enter = function (obj) {
	    var self = obj instanceof this.constructor ?
	      obj : $(obj.currentTarget).data('bs.' + this.type)

	    if (!self) {
	      self = new this.constructor(obj.currentTarget, this.getDelegateOptions())
	      $(obj.currentTarget).data('bs.' + this.type, self)
	    }

	    if (obj instanceof $.Event) {
	      self.inState[obj.type == 'focusin' ? 'focus' : 'hover'] = true
	    }

	    if (self.tip().hasClass('in') || self.hoverState == 'in') {
	      self.hoverState = 'in'
	      return
	    }

	    clearTimeout(self.timeout)

	    self.hoverState = 'in'

	    if (!self.options.delay || !self.options.delay.show) return self.show()

	    self.timeout = setTimeout(function () {
	      if (self.hoverState == 'in') self.show()
	    }, self.options.delay.show)
	  }

	  Tooltip.prototype.isInStateTrue = function () {
	    for (var key in this.inState) {
	      if (this.inState[key]) return true
	    }

	    return false
	  }

	  Tooltip.prototype.leave = function (obj) {
	    var self = obj instanceof this.constructor ?
	      obj : $(obj.currentTarget).data('bs.' + this.type)

	    if (!self) {
	      self = new this.constructor(obj.currentTarget, this.getDelegateOptions())
	      $(obj.currentTarget).data('bs.' + this.type, self)
	    }

	    if (obj instanceof $.Event) {
	      self.inState[obj.type == 'focusout' ? 'focus' : 'hover'] = false
	    }

	    if (self.isInStateTrue()) return

	    clearTimeout(self.timeout)

	    self.hoverState = 'out'

	    if (!self.options.delay || !self.options.delay.hide) return self.hide()

	    self.timeout = setTimeout(function () {
	      if (self.hoverState == 'out') self.hide()
	    }, self.options.delay.hide)
	  }

	  Tooltip.prototype.show = function () {
	    var e = $.Event('show.bs.' + this.type)

	    if (this.hasContent() && this.enabled) {
	      this.$element.trigger(e)

	      var inDom = $.contains(this.$element[0].ownerDocument.documentElement, this.$element[0])
	      if (e.isDefaultPrevented() || !inDom) return
	      var that = this

	      var $tip = this.tip()

	      var tipId = this.getUID(this.type)

	      this.setContent()
	      $tip.attr('id', tipId)
	      this.$element.attr('aria-describedby', tipId)

	      if (this.options.animation) $tip.addClass('fade')

	      var placement = typeof this.options.placement == 'function' ?
	        this.options.placement.call(this, $tip[0], this.$element[0]) :
	        this.options.placement

	      var autoToken = /\s?auto?\s?/i
	      var autoPlace = autoToken.test(placement)
	      if (autoPlace) placement = placement.replace(autoToken, '') || 'top'

	      $tip
	        .detach()
	        .css({ top: 0, left: 0, display: 'block' })
	        .addClass(placement)
	        .data('bs.' + this.type, this)

	      this.options.container ? $tip.appendTo(this.options.container) : $tip.insertAfter(this.$element)
	      this.$element.trigger('inserted.bs.' + this.type)

	      var pos          = this.getPosition()
	      var actualWidth  = $tip[0].offsetWidth
	      var actualHeight = $tip[0].offsetHeight

	      if (autoPlace) {
	        var orgPlacement = placement
	        var viewportDim = this.getPosition(this.$viewport)

	        placement = placement == 'bottom' && pos.bottom + actualHeight > viewportDim.bottom ? 'top'    :
	                    placement == 'top'    && pos.top    - actualHeight < viewportDim.top    ? 'bottom' :
	                    placement == 'right'  && pos.right  + actualWidth  > viewportDim.width  ? 'left'   :
	                    placement == 'left'   && pos.left   - actualWidth  < viewportDim.left   ? 'right'  :
	                    placement

	        $tip
	          .removeClass(orgPlacement)
	          .addClass(placement)
	      }

	      var calculatedOffset = this.getCalculatedOffset(placement, pos, actualWidth, actualHeight)

	      this.applyPlacement(calculatedOffset, placement)

	      var complete = function () {
	        var prevHoverState = that.hoverState
	        that.$element.trigger('shown.bs.' + that.type)
	        that.hoverState = null

	        if (prevHoverState == 'out') that.leave(that)
	      }

	      $.support.transition && this.$tip.hasClass('fade') ?
	        $tip
	          .one('bsTransitionEnd', complete)
	          .emulateTransitionEnd(Tooltip.TRANSITION_DURATION) :
	        complete()
	    }
	  }

	  Tooltip.prototype.applyPlacement = function (offset, placement) {
	    var $tip   = this.tip()
	    var width  = $tip[0].offsetWidth
	    var height = $tip[0].offsetHeight

	    // manually read margins because getBoundingClientRect includes difference
	    var marginTop = parseInt($tip.css('margin-top'), 10)
	    var marginLeft = parseInt($tip.css('margin-left'), 10)

	    // we must check for NaN for ie 8/9
	    if (isNaN(marginTop))  marginTop  = 0
	    if (isNaN(marginLeft)) marginLeft = 0

	    offset.top  += marginTop
	    offset.left += marginLeft

	    // $.fn.offset doesn't round pixel values
	    // so we use setOffset directly with our own function B-0
	    $.offset.setOffset($tip[0], $.extend({
	      using: function (props) {
	        $tip.css({
	          top: Math.round(props.top),
	          left: Math.round(props.left)
	        })
	      }
	    }, offset), 0)

	    $tip.addClass('in')

	    // check to see if placing tip in new offset caused the tip to resize itself
	    var actualWidth  = $tip[0].offsetWidth
	    var actualHeight = $tip[0].offsetHeight

	    if (placement == 'top' && actualHeight != height) {
	      offset.top = offset.top + height - actualHeight
	    }

	    var delta = this.getViewportAdjustedDelta(placement, offset, actualWidth, actualHeight)

	    if (delta.left) offset.left += delta.left
	    else offset.top += delta.top

	    var isVertical          = /top|bottom/.test(placement)
	    var arrowDelta          = isVertical ? delta.left * 2 - width + actualWidth : delta.top * 2 - height + actualHeight
	    var arrowOffsetPosition = isVertical ? 'offsetWidth' : 'offsetHeight'

	    $tip.offset(offset)
	    this.replaceArrow(arrowDelta, $tip[0][arrowOffsetPosition], isVertical)
	  }

	  Tooltip.prototype.replaceArrow = function (delta, dimension, isVertical) {
	    this.arrow()
	      .css(isVertical ? 'left' : 'top', 50 * (1 - delta / dimension) + '%')
	      .css(isVertical ? 'top' : 'left', '')
	  }

	  Tooltip.prototype.setContent = function () {
	    var $tip  = this.tip()
	    var title = this.getTitle()

	    $tip.find('.tooltip-inner')[this.options.html ? 'html' : 'text'](title)
	    $tip.removeClass('fade in top bottom left right')
	  }

	  Tooltip.prototype.hide = function (callback) {
	    var that = this
	    var $tip = $(this.$tip)
	    var e    = $.Event('hide.bs.' + this.type)

	    function complete() {
	      if (that.hoverState != 'in') $tip.detach()
	      if (that.$element) { // TODO: Check whether guarding this code with this `if` is really necessary.
	        that.$element
	          .removeAttr('aria-describedby')
	          .trigger('hidden.bs.' + that.type)
	      }
	      callback && callback()
	    }

	    this.$element.trigger(e)

	    if (e.isDefaultPrevented()) return

	    $tip.removeClass('in')

	    $.support.transition && $tip.hasClass('fade') ?
	      $tip
	        .one('bsTransitionEnd', complete)
	        .emulateTransitionEnd(Tooltip.TRANSITION_DURATION) :
	      complete()

	    this.hoverState = null

	    return this
	  }

	  Tooltip.prototype.fixTitle = function () {
	    var $e = this.$element
	    if ($e.attr('title') || typeof $e.attr('data-original-title') != 'string') {
	      $e.attr('data-original-title', $e.attr('title') || '').attr('title', '')
	    }
	  }

	  Tooltip.prototype.hasContent = function () {
	    return this.getTitle()
	  }

	  Tooltip.prototype.getPosition = function ($element) {
	    $element   = $element || this.$element

	    var el     = $element[0]
	    var isBody = el.tagName == 'BODY'

	    var elRect    = el.getBoundingClientRect()
	    if (elRect.width == null) {
	      // width and height are missing in IE8, so compute them manually; see https://github.com/twbs/bootstrap/issues/14093
	      elRect = $.extend({}, elRect, { width: elRect.right - elRect.left, height: elRect.bottom - elRect.top })
	    }
	    var isSvg = window.SVGElement && el instanceof window.SVGElement
	    // Avoid using $.offset() on SVGs since it gives incorrect results in jQuery 3.
	    // See https://github.com/twbs/bootstrap/issues/20280
	    var elOffset  = isBody ? { top: 0, left: 0 } : (isSvg ? null : $element.offset())
	    var scroll    = { scroll: isBody ? document.documentElement.scrollTop || document.body.scrollTop : $element.scrollTop() }
	    var outerDims = isBody ? { width: $(window).width(), height: $(window).height() } : null

	    return $.extend({}, elRect, scroll, outerDims, elOffset)
	  }

	  Tooltip.prototype.getCalculatedOffset = function (placement, pos, actualWidth, actualHeight) {
	    return placement == 'bottom' ? { top: pos.top + pos.height,   left: pos.left + pos.width / 2 - actualWidth / 2 } :
	           placement == 'top'    ? { top: pos.top - actualHeight, left: pos.left + pos.width / 2 - actualWidth / 2 } :
	           placement == 'left'   ? { top: pos.top + pos.height / 2 - actualHeight / 2, left: pos.left - actualWidth } :
	        /* placement == 'right' */ { top: pos.top + pos.height / 2 - actualHeight / 2, left: pos.left + pos.width }

	  }

	  Tooltip.prototype.getViewportAdjustedDelta = function (placement, pos, actualWidth, actualHeight) {
	    var delta = { top: 0, left: 0 }
	    if (!this.$viewport) return delta

	    var viewportPadding = this.options.viewport && this.options.viewport.padding || 0
	    var viewportDimensions = this.getPosition(this.$viewport)

	    if (/right|left/.test(placement)) {
	      var topEdgeOffset    = pos.top - viewportPadding - viewportDimensions.scroll
	      var bottomEdgeOffset = pos.top + viewportPadding - viewportDimensions.scroll + actualHeight
	      if (topEdgeOffset < viewportDimensions.top) { // top overflow
	        delta.top = viewportDimensions.top - topEdgeOffset
	      } else if (bottomEdgeOffset > viewportDimensions.top + viewportDimensions.height) { // bottom overflow
	        delta.top = viewportDimensions.top + viewportDimensions.height - bottomEdgeOffset
	      }
	    } else {
	      var leftEdgeOffset  = pos.left - viewportPadding
	      var rightEdgeOffset = pos.left + viewportPadding + actualWidth
	      if (leftEdgeOffset < viewportDimensions.left) { // left overflow
	        delta.left = viewportDimensions.left - leftEdgeOffset
	      } else if (rightEdgeOffset > viewportDimensions.right) { // right overflow
	        delta.left = viewportDimensions.left + viewportDimensions.width - rightEdgeOffset
	      }
	    }

	    return delta
	  }

	  Tooltip.prototype.getTitle = function () {
	    var title
	    var $e = this.$element
	    var o  = this.options

	    title = $e.attr('data-original-title')
	      || (typeof o.title == 'function' ? o.title.call($e[0]) :  o.title)

	    return title
	  }

	  Tooltip.prototype.getUID = function (prefix) {
	    do prefix += ~~(Math.random() * 1000000)
	    while (document.getElementById(prefix))
	    return prefix
	  }

	  Tooltip.prototype.tip = function () {
	    if (!this.$tip) {
	      this.$tip = $(this.options.template)
	      if (this.$tip.length != 1) {
	        throw new Error(this.type + ' `template` option must consist of exactly 1 top-level element!')
	      }
	    }
	    return this.$tip
	  }

	  Tooltip.prototype.arrow = function () {
	    return (this.$arrow = this.$arrow || this.tip().find('.tooltip-arrow'))
	  }

	  Tooltip.prototype.enable = function () {
	    this.enabled = true
	  }

	  Tooltip.prototype.disable = function () {
	    this.enabled = false
	  }

	  Tooltip.prototype.toggleEnabled = function () {
	    this.enabled = !this.enabled
	  }

	  Tooltip.prototype.toggle = function (e) {
	    var self = this
	    if (e) {
	      self = $(e.currentTarget).data('bs.' + this.type)
	      if (!self) {
	        self = new this.constructor(e.currentTarget, this.getDelegateOptions())
	        $(e.currentTarget).data('bs.' + this.type, self)
	      }
	    }

	    if (e) {
	      self.inState.click = !self.inState.click
	      if (self.isInStateTrue()) self.enter(self)
	      else self.leave(self)
	    } else {
	      self.tip().hasClass('in') ? self.leave(self) : self.enter(self)
	    }
	  }

	  Tooltip.prototype.destroy = function () {
	    var that = this
	    clearTimeout(this.timeout)
	    this.hide(function () {
	      that.$element.off('.' + that.type).removeData('bs.' + that.type)
	      if (that.$tip) {
	        that.$tip.detach()
	      }
	      that.$tip = null
	      that.$arrow = null
	      that.$viewport = null
	      that.$element = null
	    })
	  }


	  // TOOLTIP PLUGIN DEFINITION
	  // =========================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.tooltip')
	      var options = typeof option == 'object' && option

	      if (!data && /destroy|hide/.test(option)) return
	      if (!data) $this.data('bs.tooltip', (data = new Tooltip(this, options)))
	      if (typeof option == 'string') data[option]()
	    })
	  }

	  var old = $.fn.tooltip

	  $.fn.tooltip             = Plugin
	  $.fn.tooltip.Constructor = Tooltip


	  // TOOLTIP NO CONFLICT
	  // ===================

	  $.fn.tooltip.noConflict = function () {
	    $.fn.tooltip = old
	    return this
	  }

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: popover.js v3.3.7
	 * http://getbootstrap.com/javascript/#popovers
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // POPOVER PUBLIC CLASS DEFINITION
	  // ===============================

	  var Popover = function (element, options) {
	    this.init('popover', element, options)
	  }

	  if (!$.fn.tooltip) throw new Error('Popover requires tooltip.js')

	  Popover.VERSION  = '3.3.7'

	  Popover.DEFAULTS = $.extend({}, $.fn.tooltip.Constructor.DEFAULTS, {
	    placement: 'right',
	    trigger: 'click',
	    content: '',
	    template: '<div class="popover" role="tooltip"><div class="arrow"></div><h3 class="popover-title"></h3><div class="popover-content"></div></div>'
	  })


	  // NOTE: POPOVER EXTENDS tooltip.js
	  // ================================

	  Popover.prototype = $.extend({}, $.fn.tooltip.Constructor.prototype)

	  Popover.prototype.constructor = Popover

	  Popover.prototype.getDefaults = function () {
	    return Popover.DEFAULTS
	  }

	  Popover.prototype.setContent = function () {
	    var $tip    = this.tip()
	    var title   = this.getTitle()
	    var content = this.getContent()

	    $tip.find('.popover-title')[this.options.html ? 'html' : 'text'](title)
	    $tip.find('.popover-content').children().detach().end()[ // we use append for html objects to maintain js events
	      this.options.html ? (typeof content == 'string' ? 'html' : 'append') : 'text'
	    ](content)

	    $tip.removeClass('fade top bottom left right in')

	    // IE8 doesn't accept hiding via the `:empty` pseudo selector, we have to do
	    // this manually by checking the contents.
	    if (!$tip.find('.popover-title').html()) $tip.find('.popover-title').hide()
	  }

	  Popover.prototype.hasContent = function () {
	    return this.getTitle() || this.getContent()
	  }

	  Popover.prototype.getContent = function () {
	    var $e = this.$element
	    var o  = this.options

	    return $e.attr('data-content')
	      || (typeof o.content == 'function' ?
	            o.content.call($e[0]) :
	            o.content)
	  }

	  Popover.prototype.arrow = function () {
	    return (this.$arrow = this.$arrow || this.tip().find('.arrow'))
	  }


	  // POPOVER PLUGIN DEFINITION
	  // =========================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.popover')
	      var options = typeof option == 'object' && option

	      if (!data && /destroy|hide/.test(option)) return
	      if (!data) $this.data('bs.popover', (data = new Popover(this, options)))
	      if (typeof option == 'string') data[option]()
	    })
	  }

	  var old = $.fn.popover

	  $.fn.popover             = Plugin
	  $.fn.popover.Constructor = Popover


	  // POPOVER NO CONFLICT
	  // ===================

	  $.fn.popover.noConflict = function () {
	    $.fn.popover = old
	    return this
	  }

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: scrollspy.js v3.3.7
	 * http://getbootstrap.com/javascript/#scrollspy
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // SCROLLSPY CLASS DEFINITION
	  // ==========================

	  function ScrollSpy(element, options) {
	    this.$body          = $(document.body)
	    this.$scrollElement = $(element).is(document.body) ? $(window) : $(element)
	    this.options        = $.extend({}, ScrollSpy.DEFAULTS, options)
	    this.selector       = (this.options.target || '') + ' .nav li > a'
	    this.offsets        = []
	    this.targets        = []
	    this.activeTarget   = null
	    this.scrollHeight   = 0

	    this.$scrollElement.on('scroll.bs.scrollspy', $.proxy(this.process, this))
	    this.refresh()
	    this.process()
	  }

	  ScrollSpy.VERSION  = '3.3.7'

	  ScrollSpy.DEFAULTS = {
	    offset: 10
	  }

	  ScrollSpy.prototype.getScrollHeight = function () {
	    return this.$scrollElement[0].scrollHeight || Math.max(this.$body[0].scrollHeight, document.documentElement.scrollHeight)
	  }

	  ScrollSpy.prototype.refresh = function () {
	    var that          = this
	    var offsetMethod  = 'offset'
	    var offsetBase    = 0

	    this.offsets      = []
	    this.targets      = []
	    this.scrollHeight = this.getScrollHeight()

	    if (!$.isWindow(this.$scrollElement[0])) {
	      offsetMethod = 'position'
	      offsetBase   = this.$scrollElement.scrollTop()
	    }

	    this.$body
	      .find(this.selector)
	      .map(function () {
	        var $el   = $(this)
	        var href  = $el.data('target') || $el.attr('href')
	        var $href = /^#./.test(href) && $(href)

	        return ($href
	          && $href.length
	          && $href.is(':visible')
	          && [[$href[offsetMethod]().top + offsetBase, href]]) || null
	      })
	      .sort(function (a, b) { return a[0] - b[0] })
	      .each(function () {
	        that.offsets.push(this[0])
	        that.targets.push(this[1])
	      })
	  }

	  ScrollSpy.prototype.process = function () {
	    var scrollTop    = this.$scrollElement.scrollTop() + this.options.offset
	    var scrollHeight = this.getScrollHeight()
	    var maxScroll    = this.options.offset + scrollHeight - this.$scrollElement.height()
	    var offsets      = this.offsets
	    var targets      = this.targets
	    var activeTarget = this.activeTarget
	    var i

	    if (this.scrollHeight != scrollHeight) {
	      this.refresh()
	    }

	    if (scrollTop >= maxScroll) {
	      return activeTarget != (i = targets[targets.length - 1]) && this.activate(i)
	    }

	    if (activeTarget && scrollTop < offsets[0]) {
	      this.activeTarget = null
	      return this.clear()
	    }

	    for (i = offsets.length; i--;) {
	      activeTarget != targets[i]
	        && scrollTop >= offsets[i]
	        && (offsets[i + 1] === undefined || scrollTop < offsets[i + 1])
	        && this.activate(targets[i])
	    }
	  }

	  ScrollSpy.prototype.activate = function (target) {
	    this.activeTarget = target

	    this.clear()

	    var selector = this.selector +
	      '[data-target="' + target + '"],' +
	      this.selector + '[href="' + target + '"]'

	    var active = $(selector)
	      .parents('li')
	      .addClass('active')

	    if (active.parent('.dropdown-menu').length) {
	      active = active
	        .closest('li.dropdown')
	        .addClass('active')
	    }

	    active.trigger('activate.bs.scrollspy')
	  }

	  ScrollSpy.prototype.clear = function () {
	    $(this.selector)
	      .parentsUntil(this.options.target, '.active')
	      .removeClass('active')
	  }


	  // SCROLLSPY PLUGIN DEFINITION
	  // ===========================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.scrollspy')
	      var options = typeof option == 'object' && option

	      if (!data) $this.data('bs.scrollspy', (data = new ScrollSpy(this, options)))
	      if (typeof option == 'string') data[option]()
	    })
	  }

	  var old = $.fn.scrollspy

	  $.fn.scrollspy             = Plugin
	  $.fn.scrollspy.Constructor = ScrollSpy


	  // SCROLLSPY NO CONFLICT
	  // =====================

	  $.fn.scrollspy.noConflict = function () {
	    $.fn.scrollspy = old
	    return this
	  }


	  // SCROLLSPY DATA-API
	  // ==================

	  $(window).on('load.bs.scrollspy.data-api', function () {
	    $('[data-spy="scroll"]').each(function () {
	      var $spy = $(this)
	      Plugin.call($spy, $spy.data())
	    })
	  })

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: tab.js v3.3.7
	 * http://getbootstrap.com/javascript/#tabs
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // TAB CLASS DEFINITION
	  // ====================

	  var Tab = function (element) {
	    // jscs:disable requireDollarBeforejQueryAssignment
	    this.element = $(element)
	    // jscs:enable requireDollarBeforejQueryAssignment
	  }

	  Tab.VERSION = '3.3.7'

	  Tab.TRANSITION_DURATION = 150

	  Tab.prototype.show = function () {
	    var $this    = this.element
	    var $ul      = $this.closest('ul:not(.dropdown-menu)')
	    var selector = $this.data('target')

	    if (!selector) {
	      selector = $this.attr('href')
	      selector = selector && selector.replace(/.*(?=#[^\s]*$)/, '') // strip for ie7
	    }

	    if ($this.parent('li').hasClass('active')) return

	    var $previous = $ul.find('.active:last a')
	    var hideEvent = $.Event('hide.bs.tab', {
	      relatedTarget: $this[0]
	    })
	    var showEvent = $.Event('show.bs.tab', {
	      relatedTarget: $previous[0]
	    })

	    $previous.trigger(hideEvent)
	    $this.trigger(showEvent)

	    if (showEvent.isDefaultPrevented() || hideEvent.isDefaultPrevented()) return

	    var $target = $(selector)

	    this.activate($this.closest('li'), $ul)
	    this.activate($target, $target.parent(), function () {
	      $previous.trigger({
	        type: 'hidden.bs.tab',
	        relatedTarget: $this[0]
	      })
	      $this.trigger({
	        type: 'shown.bs.tab',
	        relatedTarget: $previous[0]
	      })
	    })
	  }

	  Tab.prototype.activate = function (element, container, callback) {
	    var $active    = container.find('> .active')
	    var transition = callback
	      && $.support.transition
	      && ($active.length && $active.hasClass('fade') || !!container.find('> .fade').length)

	    function next() {
	      $active
	        .removeClass('active')
	        .find('> .dropdown-menu > .active')
	          .removeClass('active')
	        .end()
	        .find('[data-toggle="tab"]')
	          .attr('aria-expanded', false)

	      element
	        .addClass('active')
	        .find('[data-toggle="tab"]')
	          .attr('aria-expanded', true)

	      if (transition) {
	        element[0].offsetWidth // reflow for transition
	        element.addClass('in')
	      } else {
	        element.removeClass('fade')
	      }

	      if (element.parent('.dropdown-menu').length) {
	        element
	          .closest('li.dropdown')
	            .addClass('active')
	          .end()
	          .find('[data-toggle="tab"]')
	            .attr('aria-expanded', true)
	      }

	      callback && callback()
	    }

	    $active.length && transition ?
	      $active
	        .one('bsTransitionEnd', next)
	        .emulateTransitionEnd(Tab.TRANSITION_DURATION) :
	      next()

	    $active.removeClass('in')
	  }


	  // TAB PLUGIN DEFINITION
	  // =====================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this = $(this)
	      var data  = $this.data('bs.tab')

	      if (!data) $this.data('bs.tab', (data = new Tab(this)))
	      if (typeof option == 'string') data[option]()
	    })
	  }

	  var old = $.fn.tab

	  $.fn.tab             = Plugin
	  $.fn.tab.Constructor = Tab


	  // TAB NO CONFLICT
	  // ===============

	  $.fn.tab.noConflict = function () {
	    $.fn.tab = old
	    return this
	  }


	  // TAB DATA-API
	  // ============

	  var clickHandler = function (e) {
	    e.preventDefault()
	    Plugin.call($(this), 'show')
	  }

	  $(document)
	    .on('click.bs.tab.data-api', '[data-toggle="tab"]', clickHandler)
	    .on('click.bs.tab.data-api', '[data-toggle="pill"]', clickHandler)

	}(jQuery);

	/* ========================================================================
	 * Bootstrap: affix.js v3.3.7
	 * http://getbootstrap.com/javascript/#affix
	 * ========================================================================
	 * Copyright 2011-2016 Twitter, Inc.
	 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
	 * ======================================================================== */


	+function ($) {
	  'use strict';

	  // AFFIX CLASS DEFINITION
	  // ======================

	  var Affix = function (element, options) {
	    this.options = $.extend({}, Affix.DEFAULTS, options)

	    this.$target = $(this.options.target)
	      .on('scroll.bs.affix.data-api', $.proxy(this.checkPosition, this))
	      .on('click.bs.affix.data-api',  $.proxy(this.checkPositionWithEventLoop, this))

	    this.$element     = $(element)
	    this.affixed      = null
	    this.unpin        = null
	    this.pinnedOffset = null

	    this.checkPosition()
	  }

	  Affix.VERSION  = '3.3.7'

	  Affix.RESET    = 'affix affix-top affix-bottom'

	  Affix.DEFAULTS = {
	    offset: 0,
	    target: window
	  }

	  Affix.prototype.getState = function (scrollHeight, height, offsetTop, offsetBottom) {
	    var scrollTop    = this.$target.scrollTop()
	    var position     = this.$element.offset()
	    var targetHeight = this.$target.height()

	    if (offsetTop != null && this.affixed == 'top') return scrollTop < offsetTop ? 'top' : false

	    if (this.affixed == 'bottom') {
	      if (offsetTop != null) return (scrollTop + this.unpin <= position.top) ? false : 'bottom'
	      return (scrollTop + targetHeight <= scrollHeight - offsetBottom) ? false : 'bottom'
	    }

	    var initializing   = this.affixed == null
	    var colliderTop    = initializing ? scrollTop : position.top
	    var colliderHeight = initializing ? targetHeight : height

	    if (offsetTop != null && scrollTop <= offsetTop) return 'top'
	    if (offsetBottom != null && (colliderTop + colliderHeight >= scrollHeight - offsetBottom)) return 'bottom'

	    return false
	  }

	  Affix.prototype.getPinnedOffset = function () {
	    if (this.pinnedOffset) return this.pinnedOffset
	    this.$element.removeClass(Affix.RESET).addClass('affix')
	    var scrollTop = this.$target.scrollTop()
	    var position  = this.$element.offset()
	    return (this.pinnedOffset = position.top - scrollTop)
	  }

	  Affix.prototype.checkPositionWithEventLoop = function () {
	    setTimeout($.proxy(this.checkPosition, this), 1)
	  }

	  Affix.prototype.checkPosition = function () {
	    if (!this.$element.is(':visible')) return

	    var height       = this.$element.height()
	    var offset       = this.options.offset
	    var offsetTop    = offset.top
	    var offsetBottom = offset.bottom
	    var scrollHeight = Math.max($(document).height(), $(document.body).height())

	    if (typeof offset != 'object')         offsetBottom = offsetTop = offset
	    if (typeof offsetTop == 'function')    offsetTop    = offset.top(this.$element)
	    if (typeof offsetBottom == 'function') offsetBottom = offset.bottom(this.$element)

	    var affix = this.getState(scrollHeight, height, offsetTop, offsetBottom)

	    if (this.affixed != affix) {
	      if (this.unpin != null) this.$element.css('top', '')

	      var affixType = 'affix' + (affix ? '-' + affix : '')
	      var e         = $.Event(affixType + '.bs.affix')

	      this.$element.trigger(e)

	      if (e.isDefaultPrevented()) return

	      this.affixed = affix
	      this.unpin = affix == 'bottom' ? this.getPinnedOffset() : null

	      this.$element
	        .removeClass(Affix.RESET)
	        .addClass(affixType)
	        .trigger(affixType.replace('affix', 'affixed') + '.bs.affix')
	    }

	    if (affix == 'bottom') {
	      this.$element.offset({
	        top: scrollHeight - height - offsetBottom
	      })
	    }
	  }


	  // AFFIX PLUGIN DEFINITION
	  // =======================

	  function Plugin(option) {
	    return this.each(function () {
	      var $this   = $(this)
	      var data    = $this.data('bs.affix')
	      var options = typeof option == 'object' && option

	      if (!data) $this.data('bs.affix', (data = new Affix(this, options)))
	      if (typeof option == 'string') data[option]()
	    })
	  }

	  var old = $.fn.affix

	  $.fn.affix             = Plugin
	  $.fn.affix.Constructor = Affix


	  // AFFIX NO CONFLICT
	  // =================

	  $.fn.affix.noConflict = function () {
	    $.fn.affix = old
	    return this
	  }


	  // AFFIX DATA-API
	  // ==============

	  $(window).on('load', function () {
	    $('[data-spy="affix"]').each(function () {
	      var $spy = $(this)
	      var data = $spy.data()

	      data.offset = data.offset || {}

	      if (data.offsetBottom != null) data.offset.bottom = data.offsetBottom
	      if (data.offsetTop    != null) data.offset.top    = data.offsetTop

	      Plugin.call($spy, data)
	    })
	  })

	}(jQuery);

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 31 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9), __webpack_require__(8), __webpack_require__(32), __webpack_require__(34), __webpack_require__(22), __webpack_require__(35)], __WEBPACK_AMD_DEFINE_RESULT__ = function(ko, app) {
	    ko.extenders.timeEntry = function (target) {
	        var pattern = /^(\d)?(\d)?:?(\d)?(\d)?$/;
	        target.formattedTime = ko.computed({
	            read: function () {
	                if (!target()) return '00:00';
	                var val = target();
	                var formatted = val;
	                switch (val.length) {
	                    case 1:
	                        formatted = val.replace(pattern, "0$1");
	                    case 2:
	                        formatted += ":00";
	                        break;
	                    case 3:
	                        formatted = val.replace(pattern, "0$1:$2$3");
	                        break;
	                    case 4:
	                        formatted = val.replace(pattern, "$1$2:$3$4");
	                        break;
	                }
	                return formatted;
	            },
	            write: function (value) {
	                if (typeof value === "string") {
	                    var parsed = Date.parse(value);
	                    if (parsed) {
	                        var d = new Date(parsed);
	                        var hours = d.getHours();
	                        hours = hours < 10 ? ('0' + hours) : hours;
	                        var minutes = d.getMinutes();
	                        minutes = minutes < 10 ? ('0' + minutes) : minutes;
	                        value = hours + ":" + minutes;
	                    }
	                }
	                target(value);
	            }
	        });

	        target.extend({ pattern: { message: "Invalid Date", params: /^([01]\d|2[0-3]):?([0-5]\d)$/ } });
	        target.Hours = ko.computed(function () {
	            if (!target.formattedTime()) return 0;
	            return target.formattedTime().split(":")[0];
	        });
	        target.Mins = ko.computed(function () {
	            if (!target.formattedTime()) return 0;
	            return target.formattedTime().split(":")[1];
	        });
	        target.formattedTime(target());
	        return target;
	    };

	    ko.extenders.toteKey = function (target, callback) {
	        var pattern = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{4})?$/;
	        var isComplete = ko.observable(false);
	        target.formattedTote = ko.computed({
	            read: function () {
	                var value = target();
	                return formatTote(value);
	            },
	            write: function (input) {
	                var value = cleanInput(input);
	                if (target() === value) return;

	                target(value);
	                if (value && value.match(pattern)) {
	                    var formatted = formatTote(value);
	                    if (formatted.length === 10) {
	                        isComplete(true);
	                        if (typeof callback === "function") callback(formatted);
	                    }
	                }
	            },
	        });
	        target.isComplete = ko.computed(function () {
	            return isComplete();
	        });
	        target.getNextTote = function () {
	            var formatted = target.formattedTote();
	            var sequence = parseSequence();
	            if (isNaN(sequence)) return null;
	            sequence++;

	            var sequenceString = formatSequence();
	            return formatted.replace(pattern, '0$2 $4 ' + sequenceString);

	            function parseSequence() {
	                var sections = formatted.split(" ");
	                if (sections.length !== 3) return null;
	                return parseInt(sections[2]);
	            }
	            function formatSequence() {
	                var val = sequence.toString();
	                while (val.length < 4) {
	                    val = "0" + val;
	                }
	                return val;
	            }
	        };
	        target.isMatch = function (val) {
	            var formattedVal = formatTote(ko.utils.unwrapObservable(val));
	            if (!formattedVal) return false;
	            var p = new RegExp("^" + target.formattedTote() + "$");
	            return formattedVal.match(p);
	        };

	        target.extend({ throttle: 800 });
	        target.formattedTote(target());
	        return target;

	        function formatTote(input) {
	            if (input == undefined) return '';
	            if (!input.match(pattern)) return input;
	            input = input.trim();
	            return input.replace(pattern, '0$2 $4 $6').trim().replace("  ", " ");
	        }
	        function cleanInput(input) {
	            if (typeof input == "number") input = input.toString();
	            if (typeof input !== "string") return undefined;
	            return input.replace(/\s/g, '');
	        }
	    };
	    
	    ko.extenders.contractType = function (target) {
	        return new TypeExtension(target, app.lists.contractTypes.toDictionary());
	    };

	    ko.extenders.contractStatus = function (target) {
	        return new TypeExtension(target, app.lists.contractStatuses.toDictionary());
	    };

	    ko.extenders.defectResolutionType = function (target) {
	        return new TypeExtension(target, app.lists.defectResolutionTypes.toDictionary());
	    };

	    ko.extenders.defectType = function (target) {
	        return new TypeExtension(target, app.lists.defectTypes.toDictionary());
	    };
	    
	    ko.extenders.facilityType = function (target) {
	        return new TypeExtension(target, app.lists.facilityTypes.toDictionary());
	    };

	    ko.extenders.inventoryType = function (target) {
	        return new TypeExtension(target, app.lists.inventoryTypes.toObjectDictionary());
	    };

	    ko.extenders.productType = function (target) {
	      var extension = new TypeExtension(target, app.lists.inventoryTypes.toDictionary());
	      extension.trueValue = ko.pureComputed(function() {
	        var raw = target();
	        if (raw == null) return null;
	        return parseInt(raw);
	      });
	      return extension;
	    };

	    ko.extenders.locationStatusType = function (target) {
	        return new TypeExtension(target, app.lists.locationStatusTypes.toDictionary());
	    }

	    ko.extenders.lotHoldType = function (target) {
	        return new TypeExtension(target, app.lists.lotHoldTypes.toDictionary());
	    };

	    ko.extenders.lotQualityStatusType = function (target) {
	        return new TypeExtension(target, app.lists.lotQualityStatusTypes.toDictionary());
	    };

	    ko.extenders.lotType = function (target) {
	        return new TypeExtension(target, app.lists.lotTypes.toDictionary());
	    };

	    ko.extenders.lotType2 = function (target) {
	        return new TypeExtension(target, app.lists.lotTypes.toObjectDictionary());
	    };

	    ko.extenders.productionStatusType = function (target) {
	        return new TypeExtension(target, app.lists.productionStatusTypes.toDictionary());
	    };

	    ko.extenders.chileType = function (target) {
	        var options = {
	            0: 'Other Raw',
	            1: 'Dehydrated',
	            2: 'WIP',
	            3: 'Finished Goods'
	        };
	        return new TypeExtension(target, options);
	    };

	    ko.extenders.treatmentType = function (target) {
	        return new TypeExtension(target, app.lists.treatmentTypes.toDictionary());
	    };

	    ko.extenders.shipmentStatusType = function (target) {
	        return new TypeExtension(target, app.lists.shipmentStatus.toDictionary());
	    };
	    ko.extenders.orderStatusType = function (target) {
	        return new TypeExtension(target, app.lists.orderStatus.toDictionary());
	    };
	    ko.extenders.customerOrderStatusType = function (target) {
	        return new TypeExtension(target, app.lists.customerOrderStatus.toDictionary());
	    };

	    ko.extenders.movementTypes = function (target) {
	        var options = {
	            0: 'Same Warehouse',
	            1: 'Between Warehouses',
	        };
	        return new TypeExtension(target, options);
	    };

	    ko.extenders.inventoryOrderTypes = function (target, defaultOption) {
	        return new TypeExtension(target, app.lists.inventoryOrderTypes, defaultOption);
	    };

	    // Data input binding extension. Converts input to numeric values.
	    ko.extenders.numeric = function (target, precision) {
	        console.warn('Replace numeric binding extender with numericObservable object');
	        var mode = 'readonly', isWriteable = false;
	        if (!ko.isWriteableObservable(target)) {
	            mode = 'writeable';
	            isWriteable = true;
	            //throw new Error('Object must be a writableObservable in order to be used with the numeric binding. For read-only binding, use formatNumber instead.');
	        }

	        target.numericMode = mode;
	        if (isWriteable) return writable();
	        else return readonly();

	        function writable() {
	            applyFormatting(target());
	            target.subscribe(applyFormatting, target);
	            return target;

	            function applyFormatting(value) {
	                value = formatValue(value);
	                if (value === target()) return;
	                setValue(value);
	            }
	            function setValue(value) {
	                target(value);
	            }
	        }

	        function readonly() {
	            target.formattedNumber = ko.computed({
	                read: function () {
	                    return formatValue(target()) || undefined;
	                },
	                write: function (val) {
	                    target(formatValue(val) || undefined);
	                }
	            }, target);
	            return target;
	        }

	        function formatValue(input) {
	            var numVal = parseFloat(input);
	            if (isNaN(numVal)) return undefined;
	            else return parseFloat(numVal.toFixed(precision));
	        }
	    };

	    //Read-only binding for displaying numeric values with a specific decimal precision.
	    //For numeric input bindings, use the numeric binding instead.
	    ko.extenders.formatNumber = function (target, precision) {
	        function formatValue(input) {
	            precision = parseInt(precision) || 0;
	            return precision > 0 ? parseFloat(input).toFixed(precision) : parseInt(input);
	        }

	        target.formattedNumber = ko.computed(function () {
	            return formatValue(target()) || 0;
	        }, target);
	        return target;
	    };

	    //******************************
	    // MAPPING HELPERS

	    ko.mappings = ko.mappings || {};
	    ko.mappings.formattedDate = function (options, format) {
	        var dateString = options.data;
	        var date = null;
	        if (typeof dateString == "string" && dateString.length > 0) {
	            if (dateString.match(/^\/Date\(\d*\)\/$/)) {
	                dateString = dateString.replace(/[^0-9 +]/g, '');
	                dateString = parseInt(dateString);
	            }
	            date = new Date(dateString).toISOString();
	        }
	        var result = ko.observable(date).extend({ isoDate: format || 'm/d/yyyy' });
	        return result;
	    };

	    //****************************************
	    // validation rules
	    ko.validation.rules['isUnique'] = {
	        validator: function (newVal, options) {
	            if (options.predicate && typeof options.predicate !== "function")
	                throw new Error("Invalid option for isUnique validator. The 'predicate' option must be a function.");

	            var array = options.array || options;
	            var count = 0;
	            ko.utils.arrayMap(ko.utils.unwrapObservable(array), function (existingVal) {
	                if (equalityDelegate()(existingVal, newVal)) count++;
	            });
	            return count < 2;

	            function equalityDelegate() {
	                return options.predicate ? options.predicate : function (v1, v2) { return v1 === v2; };
	            }
	        },
	        message: 'This value is a duplicate',
	    };

	    /*
	     * Determines if a field is required or not based on a function or value
	     * Parameter: boolean function, or boolean value
	     * Example
	     *
	     * viewModel = {
	     *   var vm = this;     
	     *   vm.isRequired = ko.observable(false);
	     *   vm.requiredField = ko.observable().extend({ conditional_required: vm.isRequired});
	     * }   
	    */
	    ko.validation.rules['conditional_required'] = {
	        validator: function (val, condition) {
	            var required;
	            if (typeof condition == 'function') {
	                required = condition();
	            } else {
	                required = condition;
	            }

	            if (required) {
	                return !(val == undefined || val.length == 0);
	            } else {
	                return true;
	            }
	        },
	        message: "Field is required"
	    };

	    ko.validation.rules['doesNotEqual'] = {
	        validator: function (v1, v2) {
	            ko.validation.rules['doesNotEqual'].message = "\"" + v1 + "\" is not valid.";
	            return v1 !== v2;
	        },
	    };

	    ko.validation.rules['isValidTreatment'] = {
	        validator: function (val) {
	            return val !== app.lists.treatmentTypes.NotTreated.key
	                && val !== app.lists.treatmentTypes.LowBac.key;
	        },
	        message: "Invalid Treatment"
	    };

	    ko.validation.rules['isTrue'] = {
	        validator: function (value, fnInvalid) {
	            return fnInvalid.apply(value) === true;
	        },
	        message: "The new location is the same as the previous location. There is no need to create a movement if the items don't change location.",
	    };

	    ko.validation.registerExtenders();


	    //******************************************
	    // private functions

	    function TypeExtension(target, options, defaultOption) {
	        if (defaultOption === undefined && options.length) defaultOption = options[0];
	        target.displayValue = ko.computed({
	            read: function () {
	                if (target() == undefined) return '';
	                return getTypeOption(target()) || defaultOption;
	            }
	        });
	        target.options = buildSelectListOptions(options);
	        return target;

	        function buildSelectListOptions(source) {
	            var selectListOptions = [];
	            for (var opt in source) {
	                selectListOptions.push({
	                    key: opt,
	                    value: source[opt]
	                });
	            }
	            return selectListOptions;
	        }
	        function getTypeOption(val) {
	            switch (typeof val) {
	                case "string": return fromString(val);
	                case "number": return fromNumber(val);
	                case "object": return fromObject(val);
	                default: return undefined;
	            }

	            function fromString(s) {
	                return fromNumber(parseInt(s))
	                    || findOptionByName();

	                function findOptionByName() {
	                    for (var prop in options) {
	                        if (options[prop] === s) {
	                            return fromString(prop);
	                        }
	                    }
	                    return undefined;
	                }
	            }
	            function fromNumber(num) {
	                if (isNaN(num)) return undefined;
	                return options[num + ''];
	            }
	            function fromObject(o) {
	                if (!o || o.value == undefined) return undefined;
	                return o.value;
	            }
	        }
	    }

	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),
/* 32 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {__webpack_require__(33);
	__webpack_require__(23);

	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9)], __WEBPACK_AMD_DEFINE_RESULT__ = function(ko) {
	    ko.bindingHandlers.hidden = {
	        update: function (element, valueAccessor) {
	            ko.bindingHandlers.visible.update(element, function () {
	                return !ko.utils.unwrapObservable(valueAccessor());
	            });
	        }
	    }
	    ko.bindingHandlers.preventBubble = {
	        init: function (element, valueAccessor) {
	            var eventName = ko.utils.unwrapObservable(valueAccessor());
	            ko.utils.registerEventHandler(element, eventName, function (event) {
	                event.cancelBubble = true;
	                if (event.stopPropagation) {
	                    event.stopPropagation();
	                }
	            });
	        }
	    };

	    ko.bindingHandlers.fixed = {
	      init: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
	        var options = ko.unwrap( allBindings().decimalOptions ) || {};
	        var precision = options.precision || ko.bindingHandlers.fixed.defaultPrecision;

	        if ( $( element ).is('input') ) {
	          var hiddenObservable = valueAccessor();

	          if ( ko.isObservable( hiddenObservable ) && hiddenObservable() === '' ) {
	            hiddenObservable( null );
	          }

	          var transform = ko.pureComputed({
	            read: hiddenObservable,
	            write: function( value ) {
	              if ( value === '' ) {
	                hiddenObservable( null );
	              } else {
	                var num = parseFloat( value.replace( /[^\d\.\-]/g, '' ) );
	                hiddenObservable( num.toFixed( precision ) );
	              }
	            }
	          });

	          ko.bindingHandlers.value.init( element, function() { return transform; }, allBindings );
	        }
	      },
	      update: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
	        var value = ko.unwrap( valueAccessor() );
	        var options = ko.unwrap( allBindings().decimalOptions );
	        var precision = options.precision || ko.bindingHandlers.fixed.defaultPrecision;
	        var formattedValue = parseFloat( value ).toFixed( precision );

	        if ( !isNaN( formattedValue ) ) {
	          $( element ).val( options.commas ? ko.bindingHandlers.fixed.withComma( formattedValue ) : formattedValue );
	        } else {
	          $( element ).val( '' );
	        }
	      },
	      defaultPrecision: 2,
	      withComma: function( value ) {
	        var vals = value.split('.');
	        vals[0] = Number( vals[0] ).toLocaleString();

	        return vals.join('.');
	      }
	    };

	    /**
	      * Bootstrap Modal Binding
	      * Bind to modal wrapper with class `.modal`
	      * Refer to http://getbootstrap.com/javascript/#modals for modal structure
	      *
	      * @param {boolean} valueAccessor - Toggles modal visibility
	      */
	    ko.bindingHandlers.modal = {
	      init: function (element, valueAccessor) {
	        $(body).append($(element));
	        $(element).remove();
	        $(element).modal({
	          show: false
	        });

	        var value = valueAccessor();
	        if (ko.isObservable(value)) {
	          $(element).on('hide.bs.modal', function() {
	            value(false);
	          });
	        }
	      },
	      update: function (element, valueAccessor) {
	        var value = valueAccessor();
	        if (ko.utils.unwrapObservable(value)) {
	          $(element).modal('show');
	        } else {
	          $(element).modal('hide');
	        }
	      }
	    };

	    /**
	      * @param {Object} valueAccessor - Value to monitor for update function
	      */
	    ko.bindingHandlers.floatThead = {
	      init: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
	        if ( element.nodeName.toLowerCase() !== "table" ) {
	          throw new Error('The floatThead binding must be set on a table element');
	        }

	        var $tableSelector = $( element );
	        var value = valueAccessor();
	        var valueUnwrapped = ko.unwrap( value );

	        $tableSelector.parent().addClass('sticky-head-container');
	        $tableSelector.addClass('sticky-head');

	        if ( ko.isObservable( value ) && (valueUnwrapped == null || (Array.isArray( valueUnwrapped) && value.length === 0)) ) {
	          var valueSubscription = value.subscribe( function( newValue ) {
	            floatThead( $tableSelector );
	            valueSubscription.dispose();
	          });
	        } else {
	          floatThead( $tableSelector );
	        }

	        ko.utils.domNodeDisposal.addDisposeCallback( element, function() {
	          $tableSelector.floatThead('destroy');
	        });

	        function floatThead( $tableSelector ) {
	          $tableSelector.floatThead({
	            scrollContainer: function ( $table ) {
	              return $table.closest('.sticky-head-container');
	            }
	          });
	        }
	      },
	      update: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
	        var value = valueAccessor();
	        var valueUnwrapped = ko.unwrap(value);

	        $( element ).floatThead('reflow');
	      }
	    };

	    ko.bindingHandlers.loadingMessage = {
	      init: function (element, valueAccessor) {
	        $('body').append($(element));
	        $(element).hide();
	      },
	      update: function(element, valueAccessor) {
	        var value = valueAccessor();
	        var valueUnwrapped = ko.unwrap(value);

	        if (!!valueUnwrapped) {
	          $(element).fadeIn();
	        } else {
	          $(element).fadeOut();
	        }
	      }
	    };

	    /**
	      * @deprecated Use modal instead
	      */
	    ko.bindingHandlers.dialog = {
	        init: function (element, valueAccessor, allBindings, bindingContext) {
	            console.debug('dialog has been deprecated, use modal instead');
	            var defaultConfig = {
	                    modal: true,
	                },
	                $element = $(element),
	                value = valueAccessor(),
	                commands = parseCommands();

	            initDialog();


	            // prevent duplicate binding error?
	            ko.cleanNode($element);
	            $element.removeAttr('data-bind');
	            $element.children(function() {
	                this.removeAttr('data-bind');
	            });

	            var dialogDestroyed = false;
	            if (ko.isObservable(value)) {
	                var valueSubscriber = value.subscribe(function (val) {
	                    if (!dialogDestroyed) $element.dialog(val ? "open" : "close");
	                });

	                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                    if ($element.dialog('option')) $element.dialog("destroy");
	                    dialogDestroyed = true;
	                    valueSubscriber.dispose && valueSubscriber.dispose();
	                });
	            }

	            attachKoCommands();

	            function initDialog() {
	                // Creates empty function for any configured buttons.
	                // This will cause the buttons to be displayed while allowing
	                // execution to be deferred to the supplied command object.

	                var options = ko.utils.extend(allBindings() || {}, defaultConfig);

	                var config = {
	                    modal: options.modal,
	                    height: options.height,
	                    width: options.width,
	                    position: options.position,
	                    buttons: { },
	                    close: options.close || options.cancelCommand,
	                    title: options.title,
	                    autoOpen: ko.utils.peekObservable(value) && true || false,
	                    dialogClass: options.cancelCommand ? 'no-close' : '',
	                };

	                for (var prop in commands) {
	                    if (commands.hasOwnProperty(prop))
	                        config.buttons[prop] = empty;
	                }

	                $element.dialog(config);

	                function empty() { }
	            }

	            function parseCommands() {
	                var exports = {},
	                    bindings = allBindings() || {};

	                parseCommand(bindings['cancelCommand'], 'cancelCommand', 'Cancel');
	                parseCommand(bindings['okCommand'], 'okCommand', 'Ok');

	                var customCommands = getCustomCommands();
	                for (var prop in customCommands) {
	                    if (customCommands.hasOwnProperty(prop))
	                        parseCommand(customCommands[prop], prop, prop);
	                }
	                return exports;

	                function getCustomCommands() {
	                    return allBindings().customCommands || allBindings().customCommand || [];
	                }
	                function parseCommand(cmd, bindingName, mapToButtonName) {
	                    if (!cmd) return;
	                    if (!cmd.execute) {
	                        cmd = ko.command({
	                            execute: cmd
	                        });
	                    }
	                    exports[mapToButtonName || bindingName] = cmd;
	                }


	            }

	            function attachKoCommands() {
	                var buttonFunctions = $element.dialog("option", "buttons");
	                var newButtonsConfig = [];
	                for (var funcName in buttonFunctions) {
	                    for (var cmdName in commands) {
	                        if (cmdName == funcName) {
	                            var buttons = $(".ui-dialog-buttonpane button:contains('" + cmdName + "')");

	                            $.each(buttons, function (index) {
	                                var command = commands[cmdName];

	                                ko.bindingHandlers.command.init(
	                                    buttons[index],
	                                    ko.utils.wrapAccessor(command),
	                                    allBindings,
	                                    null,
	                                    bindingContext);

	                                // remove click functionality from the jQuery UI element
	                                newButtonsConfig.push({
	                                    text: cmdName,
	                                    click: empty,
	                                });
	                            });
	                            break;
	                        }
	                    }
	                }

	                function empty() { }
	            }
	        },
	    };

	    ko.bindingHandlers.cancelKey = {
	        init: function (element, valueAccessor, allBindings, viewModel) {
	            var delegate = ko.utils.unwrapObservable(valueAccessor());

	            if (delegate && typeof delegate !== 'function' && typeof delegate.execute === "function") {
	                delegate = delegate.execute;
	            }
	            if (delegate == undefined) return;

	            var cancelKeyCode = 27;
	            var elementToRegister = element;

	            if (attachToWindow()) {
	                var conditionFn = function () { return $(element).is(':visible'); };
	                elementToRegister = window;
	            }
	            ko.utils.registerEventHandler(elementToRegister, 'keydown', buildEventHandler(conditionFn));

	            function attachToWindow() {
	                var bindings = ko.utils.unwrapObservable(allBindings);
	                return bindings && ko.utils.unwrapObservable(bindings.attachToWindow) === true;
	            }

	            function buildEventHandler(conditionalFn) {
	                conditionalFn = conditionalFn || function () { return true; };
	                return function (event) {
	                    if (event.keyCode == cancelKeyCode && conditionalFn()) {
	                        executeCancel(event);
	                        event.cancelBubble = true;
	                        if (event.stopPropagation) event.stopPropagation();
	                    }
	                };
	            }

	            function executeCancel(event) {
	                event.preventDefault();
	                event.target.blur();
	                delegate.call(viewModel);
	            }
	        }
	    };

	    ko.bindingHandlers.onblur = {
	        init: function (element, valueAccessor) {
	            var fn = valueAccessor();
	            if (fn && fn.execute) fn = commandWrapper.bind(fn);
	            ko.utils.registerEventHandler(element, 'blur', fn);

	            function commandWrapper() {
	                this.execute();
	            }
	        }
	    };

	    ko.bindingHandlers.growToWindowHeight = {
	        init: function (element, valueAccessor) {
	            var $element = $(element);
	            var bindings = $.extend({}, ko.bindingHandlers.growToWindowHeight.DEFAULT_OPTIONS, ko.unwrap(valueAccessor()) || {});

	            var $windowHeight = $(window).height();
	            var viewportHeight = document.body && document.body.clientHeight
	                ? Math.min(document.body.clientHeight, $windowHeight)
	                : $windowHeight;
	            var windowHeight = viewportHeight - bindings.offset;
	            $element.height(windowHeight);
	            $element.css('overflow', 'auto');
	        },
	        DEFAULT_OPTIONS: {
	            offset: 0
	        }
	    };

	    ko.bindingHandlers.maxHeight = {
	        init: function (element, valueAccessor, allBindings) {
	            var $element = $(element);
	            $element.addClass('fullWindowHeight');
	            constrainHeight.call($element);

	            var value = valueAccessor();
	            if (ko.isObservable(value)) {
	                var sub = value.subscribe(function() {
	                    setupStickyTableElements();
	                    sub.dispose();
	                });
	            } else setupStickyTableElements();

	            function setupStickyTableElements() {
	                prepareTables(element, valueAccessor(), allBindings());
	            }
	        },
	    };

	    function constrainHeight() {
	        var windowHeight = $(window).height();
	        this.height(windowHeight);
	        //this.css('max-height', windowHeight);
	        this.css('overflow-y', 'scroll');
	        this.css('overflow-x', 'scroll');
	    }

	    function prepareTables(element, value, allBindings) {
	        var $element = $(element);
	        var opts = allBindings || {};

	        if ($element.is('table')) {
	            $element = $(element).wrap("<div><div>").parent();
	        }

	        initStickyTableBinding($element, value, opts, 'stickyTableHeaders');
	        //initStickyTableBinding($element, value, opts, 'stickyTableFooters');
	    }

	    function initStickyTableBinding(element, value, opts, bindingName) {
	        var valueAccessor = opts[bindingName]
	                ? ko.utils.wrapAccessor(opts[bindingName])
	                : ko.utils.wrapAccessor(value || true);

	        var $element = $(element);

	        var template = getTemplatedChild();
	        if (template) {
	            if (!opts[bindingName]) return;
	            opts.dependsOn = template;
	            var bindings = ko.utils.wrapAccessor(opts);
	            ko.bindingHandlers[bindingName].init(template, valueAccessor, bindings);
	        } else {
	            opts.parent = $element;
	            $element.find(opts[bindingName] || 'table').each(function () {
	                var $this = $(this);
	                ko.bindingHandlers[bindingName].init($this, valueAccessor, ko.utils.wrapAccessor(opts));
	                removeBinding($this, bindingName);
	            });
	        }

	        function getTemplatedChild() {
	            var child = getChild();
	            if (!child) return null;
	            var childContext = ko.contextFor(child);
	            if (!childContext) return null;

	            var childBindings = ko.bindingProvider.instance.getBindings(child, childContext);
	            return childBindings && childBindings.template ? child : null;

	            function getChild() {
	                return $element.children(':first')[0]
	                    || getVirtualElementChild();
	            }
	            function getVirtualElementChild() {
	                var vChild = ko.virtualElements.firstChild($element[0]);
	                return vChild && ko.virtualElements.nextSibling(vChild);
	            }
	        }

	        function removeBinding(table, binding) {
	            var dataBind = table.attr('data-bind');
	            if (dataBind) {
	                var regex = new RegExp(binding + "\:\s?\w+\W?\s?", "i");
	                dataBind = dataBind.replace(regex, "");
	                table.attr('data-bind', dataBind);
	            }
	        }
	    }

	    ko.bindingHandlers.fixCvpOverlay = {
	        init: function (element, valueAccessor) {
	            var $container = $(element).wrap('<div />').parent();
	            //ko.bindingHandlers.fixCvpOverlay.update(element,valueAccessor);
	        },
	        update: function (element, valueAccessor) {
	            valueAccessor().notifySubscribers(); // fix initial overlay
	            ko.utils.unwrapObservable(valueAccessor());
	            var $cvp = $("#cvp");
	            var cvpWidth = $cvp.outerWidth();
	            var $element = $(element);
	            var inventoryTableWidth = $element.width();

	            // When element contains an enumerated child (such as a foreach binding), the
	            // width function returns 0. This hacky little fix will set a default width.
	            if (inventoryTableWidth == 0) {
	                inventoryTableWidth = 5000; // default width
	            }

	            var $container = $element.parent();
	            $container.width(inventoryTableWidth).css({ "padding-right": cvpWidth + 85 });
	        }
	    };

	    ko.bindingHandlers.undo = {
	        init: function (element, valueAccessor, allBindings, viewModel) {
	            var bindings = {};
	            var trackedBindingNames = ['value'];
	            var isEditing = ko.computed(function () {
	                return ko.utils.unwrapObservable(valueAccessor());
	            });
	            var elementBindings = ko.bindingProvider.instance.getBindings(element, ko.contextFor(element));
	            ko.utils.arrayForEach(trackedBindingNames, function (binding) {
	                if (elementBindings[binding]) {
	                    bindings[binding] = elementBindings[binding];
	                }
	            });

	            for (var boundProp in bindings) {
	                initializeTracking(bindings[boundProp]);
	            }

	            function revert(propAccessor) {
	                var initalValue = propAccessor.changeHistory()[0];
	                propAccessor(initalValue);
	            }

	            function initializeTracking(propAccessor) {
	                propAccessor.changeHistory = ko.observableArray([propAccessor()]);

	                propAccessor.subscribe(function (newVal) {
	                    propAccessor.changeHistory.push(newVal);
	                });

	                setupRevertTrigger(propAccessor);
	            }

	            isEditing.subscribe(function (newVal) {
	                $cancelButton.each(function (index, button) {
	                    ko.bindingHandlers.visible.update(
	                        button,
	                        ko.utils.wrapAccessor(newVal),
	                        allBindings,
	                        data
	                    );
	                });
	            });

	            function setupRevertTrigger(propAccessor) {
	                // eventually, we'll enable actual undo/redo stepping but for now, we just
	                // handle both as a revert function.
	                var revertCommand = ko.command({
	                    execute: function () {
	                        revert(propAccessor);
	                    },
	                    canExecute: function () {
	                        return propAccessor.changeHistory().length > 1;
	                    }
	                });
	                propAccessor.revertCommand = revertCommand;
	                var trigger = allBindings().undoTrigger || allBindings().revertTrigger;

	                $(trigger).each(function (index, button) {
	                    ko.bindingHandlers.command.init(
	                        button,
	                        ko.utils.wrapAccessor(revertCommand),
	                        allBindings,
	                        viewModel
	                    );
	                });
	            }
	        }
	    };

	    ko.bindingHandlers.pageData = {
	        update: function (element, valueAccessor) {
	            ko.utils.unwrapObservable(valueAccessor());
	            $(element).hide().fadeIn(500);
	        }
	    };

	    ko.bindingHandlers.editableContent = {
	        init: function (element, valueAccessor, allBindings, data) {
	            var savedState = ko.observable();
	            var isEditing = ko.computed(function () {
	                return ko.utils.unwrapObservable(valueAccessor());
	            });
	            var $element = $(element);
	            var $cancelButton = $(allBindings().cancelTrigger);
	            var $masterCancelButton = $(allBindings().masterCancelTrigger);

	            if (!isEditing()) { $element.attr("readonly", "readonly"); }

	            ko.bindingHandlers.undo.init(
	                element,
	                ko.utils.wrapAccessor(function () { return true; }),
	                ko.utils.wrapAccessor({ revertTrigger: $cancelButton ? $cancelButton[0] : undefined }),
	                data);

	            ko.bindingHandlers.click.init(
	                element,
	                ko.utils.wrapAccessor(beginEdit),
	                ko.utils.wrapAccessor({}),
	                data);

	            //todo: handle blur events (and allow disabling the blur handlers)

	            //todo: 1. prevent bubbling, 2. enable canceling when cancel button is not supplied
	            ko.bindingHandlers.cancelKey.init(
	                element,
	                ko.utils.wrapAccessor(function () { $cancelButton.click(); }),
	                ko.utils.wrapAccessor({ keydownBubble: false }),
	                data
	            );


	            setupCancelButtons();

	            function setupCancelButtons() {
	                var cancelCommand = allBindings().cancelEditsCommand;
	                if ($cancelButton.length > 0) {
	                    $cancelButton.each(function (index, button) {
	                        ko.bindingHandlers.command.init(
	                            button,
	                            ko.utils.wrapAccessor(cancelCommand),
	                            function () { return { clickBubble: false }; },
	                            data);
	                    });
	                }

	                $masterCancelButton.each(function (index, button) {
	                    var context = ko.contextFor(button);
	                    if (context) {
	                        var commandBinding = ko.bindingProvider.instance.getBindings(button, context).command;
	                        if (commandBinding) {
	                            if (typeof commandBinding.addCommand != "function") {
	                                throw new Error('The masterCancelCommand is only supported with a composableCommand instance.');
	                            }

	                            commandBinding.addCommand(ko.command({
	                                execute: function () { $cancelButton.click(); }
	                            }));
	                        }
	                    }
	                });
	            }

	            function beginEdit() {
	                if (isEditing()) return;
	                valueAccessor()(true);
	                var bindings = ko.bindingProvider.instance.getBindings(element, ko.contextFor(element));
	                savedState(ko.utils.unwrapObservable(bindings.value));
	            }
	        },
	        update: function (element, valueAccessor) {
	            var isEditing = ko.utils.unwrapObservable(valueAccessor());
	            if (isEditing === false) {
	                $(element).attr("readonly", "readonly");
	            } else {
	                $(element).removeAttr("readonly");
	            }
	        }
	    };

	    ko.bindingHandlers.editableContentArea = {
	        init: function (element, valueAccessor, allBindings, data) {
	            var $element = $(element);
	            var isEditing = ko.computed(function () {
	                return ko.utils.unwrapObservable(valueAccessor());
	            });

	            function setIsEditingValue(val) {
	                valueAccessor()(val);
	            }
	            var inputElements = $('input', $element).not('[type="button"], [type="submit"]');
	            var allChildrenEmpty = ko.computed(function () {
	                var firstDefinedValue = ko.utils.arrayFirst(inputElements, function (e) {
	                    var ctx = ko.contextFor(e);
	                    var binding = ctx ? ko.bindingProvider.instance.getBindings(e, ctx) : undefined;
	                    var value = binding ? binding.value() : undefined;
	                    return value != undefined;
	                });
	                return firstDefinedValue === null;
	            });
	            var isCancelVisible = ko.computed(function () {
	                return isEditing() && !allChildrenEmpty();
	            });

	            valueAccessor().__editableContentArea__inputElements = inputElements;

	            var cancelValueAccessor = ko.utils.wrapAccessor(function () { return false; });
	            cancelValueAccessor().__editableContentArea__inputElements = inputElements;

	            var cancelCommand = ko.command({
	                execute: function () {
	                    setIsEditingValue(false);
	                },
	            });

	            // currently requires cancelTrigger binding to be provided
	            var $cancelButton = $element.find(allBindings().cancelTrigger);

	            // initialize visibility
	            updateCancelButtonVisibility();

	            // update visibility
	            isCancelVisible.subscribe(function () {
	                updateCancelButtonVisibility();
	            });

	            allBindings().cancelTrigger = $cancelButton ? $cancelButton[0] : undefined;
	            allBindings().cancelEditsCommand = cancelCommand;

	            $.each(inputElements, function (index, elem) {
	                ko.bindingHandlers.editableContent.init(elem, valueAccessor, allBindings, data);
	            });

	            function updateCancelButtonVisibility() {
	                $cancelButton.each(function (index, button) {
	                    ko.bindingHandlers.visible.update(
	                        button,
	                        function () { return isCancelVisible(); },
	                        allBindings,
	                        data
	                    );
	                });
	            }
	        },
	        update: function (element, valueAccessor, allBindings, data) {
	            var inputElements = valueAccessor().__editableContentArea__inputElements;
	            var isEditing = ko.utils.unwrapObservable(valueAccessor());

	            $.each(inputElements, function (index, elem) {
	                ko.bindingHandlers.editableContent.update(
	                    elem,
	                    ko.utils.wrapAccessor(isEditing),
	                    allBindings,
	                    data
	                );
	            });
	        },
	    };



	    function initVisibleToggle(element, dataValue, isNot) {
	        var $el = $(element);
	        if (isNot ? !dataValue : (dataValue && true)) {
	            $el.hide();
	            $el.slideDown();
	        } else {
	            $el.slideUp();
	        }

	        $el = null;
	    }

	    function updateVisibleToggle(element, dataValue, opts, isNot) {
	        var defaults = {
	            showDuration: "slow",
	            hideDuration: "slow",
	            speed: false,
	            direction: "down",
	        };

	        var options = $.extend({}, defaults, opts);
	        var $el = $(element);
	        if (options && options.speed) options.showDuration = options.hideDuration = options.speed;

	        if (isNot ? !dataValue : (dataValue && true)) {
	            $el.slideDown(options.showDuration);
	        } else {
	            $el.slideUp(options.hideDuration);
	        }
	    }

	    ko.bindingHandlers.slideVisible = {
	        init: function (element, valueAccessor) {
	            initVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), false);
	        },
	        update: function (element, valueAccessor, allBindings) {
	            updateVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), allBindings(), false);
	        }
	    };

	    ko.bindingHandlers.slideCollapsed = {
	        init: function(element, valueAccessor) {
	            initVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), true);
	        },
	        update: function (element, valueAccessor, allBindings) {
	            updateVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), allBindings(), true);
	        }
	    }

	    ko.bindingHandlers.popup = {
	        init: function (element, valueAccessor, allBindings) {
	            var $element = $(element);
	            $element.addClass('popupWindow');

	            var defaults = {
	                attachCancelCommandToWindow: true,
	            };
	            var options = $.extend({}, defaults, allBindings());
	            var borderWidth = parseInt($element.css('border-left-width'), 10) || 10; // parseInt trims the 'px' and returns base-10 value

	            $(element).on('click', onCloseEvent);

	            if (options.closePopupCommand) {
	                var cancelKeyOptions = options;
	                cancelKeyOptions.attachToWindow = options.attachCancelCommandToWindow;
	                ko.bindingHandlers.cancelKey.init(element, ko.utils.wrapAccessor(options.closePopupCommand), ko.utils.wrapAccessor(cancelKeyOptions));
	            }

	            ko.bindingHandlers.slideIn.init(element, valueAccessor);

	            // handle cleanup
	            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                $element.off('click', onCloseEvent);
	            });

	            // private functions
	            function onCloseEvent(e) {
	                var hitAreaX = borderWidth + $element.position().left;
	                // Reece - Replaced pageX with screenX b/c when
	                // clicking a selectBox,  pageX value is relative to
	                // selectbox -- not the page.
	                if (e.pageX && e.pageX <= hitAreaX) {
	                    if (options.closePopupCommand && typeof options.closePopupCommand.execute == "function") {
	                        options.closePopupCommand.execute();
	                        return;
	                    }
	                    if (ko.isWriteableObservable(valueAccessor())) {
	                        valueAccessor()(false);
	                        return;
	                    }
	                    ko.bindingHandlers.popup.update(element, ko.utils.wrapAccessor(false), allBindings);
	                }
	            }
	        },
	        update: function (element, valueAccessor, allBindings) {
	            ko.bindingHandlers.slideIn.update(element, valueAccessor, allBindings);
	        }
	    };

	    ko.bindingHandlers.slideIn = {
	        init: function (element, valueAccessor) {
	            var display = ko.utils.unwrapObservable(valueAccessor());

	            var $element = $(element);
	            $element.show();
	            if (!display) {
	                $element.hide();
	                //$element.css({ left: $(window).width() });
	                $element.css({ left: "100%" });
	            }
	        },
	        update: function (element, valueAccessor) {
	            var $element = $(element);
	            var display = ko.utils.unwrapObservable(valueAccessor());
	            if (display) {
	                $element.show();
	                $element.animate({ left: 0 });
	            } else {
	                $element.animate({ left: "100%" });
	                $element.hide();
	            }
	        }
	    };

	    ko.bindingHandlers.fadeVisible = {
	        init: function (element) {
	            $(element).hide();
	        },
	        update: function (element, valueAccessor) {
	            var value = ko.utils.unwrapObservable(valueAccessor());
	            if (value) $(element).fadeIn();
	            else $(element).fadeOut();
	        }
	    };

	    ko.bindingHandlers.stickyTableHeaders = {
	        init: function (element, valueAccessor, allBindings) {
	            var value = ko.utils.unwrapObservable(valueAccessor());
	            var bindings = allBindings();
	            var $table;
	            var options = {};
	            var completed = false;
	            var $element = $(element);

	            options.tabs = bindings.tabbedParent;
	            options.myTab = bindings.myTab;

	            if (typeof value === "string") {
	                $table = $element.find(value);
	                options.parent = $element;
	            } else {
	                $table = $(element);
	                options.parent = bindings.parent;
	            }

	            if ($table == undefined) throw new Error("The table element can not be found. Selector: '" + value + "'.");

	            bindTable();

	            function bindTable() {
	                //Enables the jQuery transformation to be deferred until after the dependent object has data
	                var dependsOn = bindings['dependsOn'];
	                if (dependsOn && deferToDependency()) {
	                    return;
	                }
	                stickyHeaders($table, options);

	                function deferToDependency() {
	                    var $dependency = typeof dependsOn === "string"
	                        ? $element.children(':first')
	                        : $(dependsOn);
	                    if (!$dependency) return false;

	                    var dependencyElement = $dependency[0];
	                    var dependencyContext = ko.contextFor(dependencyElement);
	                    var dependencyBindings = ko.bindingProvider.instance.getBindings(dependencyElement, dependencyContext);

	                    if (dependencyHasTemplate()) {
	                        var fnName = '__stickyTableHeaders__updateHeaders__';
	                        if (isVirtualElement()) {
	                            dependsOn.data = attachAfterRenderBinding.call(dependsOn.data);
	                            dependencyContext.$data[fnName] = function () {
	                                var table = typeof (value) === "string"
	                                    ? $(arguments[0]).filter(value) || $element.find(value)
	                                    : value;

	                                if (!table.length) {
	                                    console.error("The table element could not be found. When attaching stickyTableHeaders within template, the value parameter should contain a selector for the table.");
	                                    return;
	                                }

	                                var context = ko.contextFor(dependsOn);
	                                var theadDependency = bindings.stickyTableHeaderDependency;
	                                if (typeof theadDependency === "string") theadDependency = context.$data[theadDependency];
	                                options.parent = table;
	                                if (ko.isObservable(theadDependency)) {
	                                    theadDependency.subscribe(function () {
	                                        stickyHeaders(table, options);
	                                    });
	                                } else {
	                                    stickyHeaders(table, options);
	                                }
	                            };
	                        } else {
	                            var binding = attachAfterRenderBinding.call($dependency.attr('data-bind'));
	                            $dependency.attr('data-bind', binding);
	                            dependencyContext.$data[fnName] = function () {
	                                stickyHeaders($element.find(value), options);
	                            };
	                        }
	                        return true;

	                    }

	                    return false;

	                    function isVirtualElement() {
	                        return dependencyElement.nodeType === 8;
	                            //virtualNoteBindingValue is apparently only available to the debug version of KO.
	                            //&& ko.virtualElements.virtualNodeBindingValue
	                            //&& ko.virtualElements.virtualNodeBindingValue(dependsOn);
	                    }
	                    function dependencyHasTemplate() {
	                        return dependencyBindings && dependencyBindings.template;
	                    }
	                    function attachAfterRenderBinding() {
	                        return this.replace(/(template\:\s?\{)/, "$1" + 'afterRender:' + fnName + ',');
	                    }
	                }
	            }

	            function stickyHeaders(table, opts) {
	                table.each(function () {
	                    if (!this.tagName || this.tagName.toLowerCase() !== 'table') {
	                        throw new Error("The bound element is not a table element. Element selector: '" + value + "'");
	                    }
	                });

	                opts.floatingElementId = 'stickyTableHeader';
	                opts.target = 'thead:first';

	                table.stickyTableHeaders(opts);

	                table.each(function () { rebind.call(this, opts); });

	                var valueSubscription;
	                if (ko.isObservable(valueAccessor())) {
	                    valueSubscription = valueAccessor().subscribe(function () {
	                        table.stickyTableHeaders('option', 'format');
	                    });
	                }

	                var rebuildSubscription;
	                if (bindings.rebuildTrigger) {
	                    if (!ko.isObservable(bindings.rebuildTrigger))
	                        throw new Error("Invalid binding: \"rebuildTrigger\". Must be observable object.");

	                    rebuildSubscription = bindings.rebuildTrigger.subscribe(function () {
	                        table.stickyTableHeaders("option", "refresh");
	                        table.each(function () { rebind.call(this, opts); });
	                    });
	                }

	                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                    table.stickyTableHeaders('destroy');
	                    valueSubscription && valueSubscription.dispose();
	                    rebuildSubscription && rebuildSubscription.dispose();
	                });

	                completed = true;
	            }

	            function rebind(opts) {
	                var floatingElements = $('.' + opts.floatingElementId, this);
	                var floatingClone = floatingElements[0];

	                var context = ko.contextFor(this);
	                if (!context || !floatingClone) return;

	                ko.cleanNode(floatingClone);
	                ko.applyBindings(context.$data, floatingClone);

	                var $clone = $(floatingClone);
	                var bindings = ko.bindingProvider.instance.getBindings(this, ko.contextFor(this));

	                // reformat elements if the clone was templated
	                if (bindings.template) {
	                    $element.stickyTableHeaders('option', 'format');
	                }

	                if (bindings.sortableTable) {
	                    ko.bindingHandlers.sortableTable.init($clone.parent()[0], ko.utils.wrapAccessor(bindings.sortableTable), ko.utils.wrapAccessor(bindings));
	                }

	            }
	        },
	    };

	    var templateRegEx = /(?:^|,|\s)template\s*:\s*(?:(?:(?:'|\")([^(?:'|"|\s|\{)]+)\s*(?:'|"))|(?:\{.*name\s*:\s*(?:(?:'|\")([^(?:'|"|\s|\{)]+)(?:'|"|\s))))/i;
	    ko.bindingHandlers.stickyTableFooters = {
	        init: function (element, valueAccessor, allBindingsAccessor) {
	            var opts = ko.utils.unwrapObservable(allBindingsAccessor());
	            opts.floatingElementId = 'stickyTableFooter';
	            opts.target = 'tfoot:first';

	            var $element = $(element);

	            $element.stickyTableFooters(opts);

	            var table = element;

	            $(opts.target, table).each(function () {
	                var floatingElements = $('.' + opts.floatingElementId, table);
	                if (!floatingElements.length) return;
	                var floatingClone = floatingElements[0];


	                var context = ko.contextFor(this);
	                if (!context) return;

	                ko.cleanNode(floatingClone);
	                ko.applyBindings(context.$data, floatingClone);

	                // reformat elements if the clone was templated
	                var $clone = $(floatingClone);
	                var dataBind = $clone.attr('data-bind');
	                if (dataBind) {
	                    var matches = dataBind.match(templateRegEx);
	                    if (matches && matches.length) {
	                        $element.stickyTableFooters('option', 'format');
	                    }
	                }
	            });

	            var value = valueAccessor();
	            if (ko.isObservable(value)) {
	                value.subscribe(function() {
	                    $element.stickyTableFooters('option', 'format');
	                });
	            }

	            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                $element.stickyTableFooters('destroy');
	            });
	        },
	    };

	    ko.bindingHandlers.tooltip = {
	        init: function (element, bindingAccessor, allBindings) {
	            var value = ko.utils.unwrapObservable(bindingAccessor()),
	                bindings = allBindings && allBindings() || {};

	            if (typeof value == "number") value = value.toString();
	            if (!value || value.length == 0) return;

	            var $element = $(element);
	            $element.attr('title', value);
	            $element.tooltip({
	                track: bindings.tooltipTrack,
	            });
	            //todo: enable updates to the tooltip value
	        },
	    };

	    ko.bindingHandlers.datePickerSm = {
	        init: function (element, valueAccessor, allBindings) {
	            $(element).wrap('<div class="input-group input-group-sm"></div>');
	            $(element).datepicker({
	                showOn: 'button',
	                buttonText: '<i class="fa fa-calendar"></i>',
	                changeMonth: true,
	                changeYear: true
	            }).next(".ui-datepicker-trigger")
	                .addClass("btn btn-default")
	                .wrap('<span class="input-group-btn"></span>');

	            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                //todo: cleanup wrapper element
	                $(element).datepicker('destroy');
	            });

	            var value = valueAccessor();
	            if (ko.isObservable(value)) {
	                ko.bindingHandlers.value.init(element, valueAccessor, allBindings);
	            }
	        }
	    };

	    ko.bindingHandlers.autoHeightTextarea = {
	        init: function (element, valueAccessor) {
	        },
	        update: function (element, valueAccessor) {
	            element.style.height = '0';
	            element.style.height = element.scrollHeight + 'px';
	        }
	    };

	    // autocomplete: listOfCompletions
	    ko.bindingHandlers.autocomplete = {
	        init: function (element, valueAccessor) {
	            var disposables = [];
	            var value = ko.utils.unwrapObservable(valueAccessor());
	            var opts = {
	                //minLength: 0,
	                change: onChange
	            };
	            $( element ).wrap('<div class="ui-front"></div>');

	            function buildSourceOptions( value ) {
	              if ( value.length ) {
	                opts.source = ko.utils.arrayMap( value, function(c) {
	                  if ( c.Name && !c.label ) {
	                    c.label = c.Name;
	                  }

	                  return c;
	                });
	              } else {
	                opts = $.extend( opts, value );

	                if ( !value.source ) {
	                  console.log("Invalid parameters for the autocomplete binding. Value must be either an array or and object with a \"source\" property containing an array.");
	                  return;

	                  //the following line was causing an error when closing a pack schedule after it's been in edit mode.
	                  throw new Error("Invalid parameters for the autocomplete binding. Value must be either an array or and object with a \"source\" property containing an array.");
	                }

	                if ( value.label || value.value ) {
	                  var labelProjector = buildProjector( value.label ),
	                  valueProjector = value.value ? buildProjector( value.value ) : function() { return value; };

	                  opts.source = ko.utils.arrayMap( ko.utils.unwrapObservable( value.source ), function ( item ) {
	                    return {
	                      label: labelProjector(item),
	                      value: valueProjector(item),
	                    };
	                  });
	                } else {
	                  opts.source = ko.utils.unwrapObservable(value.source);
	                }
	              }
	            }

	            function buildProjector( src ) {
	              var prop = ko.utils.unwrapObservable(src);

	              if (prop == undefined) {
	                throw new Error("Projector property is undefined.");
	              }

	              return typeof prop === "function" ?
	                function (object) { return prop(object); } :
	                function(object) { return object[prop]; };
	            }

	            if ( ko.isObservable( value ) ) {
	              disposables.push( value.subscribe(function( optionsSource ) {
	                buildSourceOptions( optionsSource );
	                $( element ).autocomplete( opts );
	              }));
	            } else if ( ko.isObservable( value.source ) ) {
	              disposables.push( value.source.subscribe(function( optionsSource ) {
	                buildSourceOptions( optionsSource );
	                $( element ).autocomplete( opts );
	              }));
	            }

	            buildSourceOptions( value );
	            $( element ).autocomplete( opts );

	            function onChange (e, ui) {
	                var bindingContext = ko.contextFor(element);
	                if (!bindingContext) return;
	                var bindings = ko.bindingProvider.instance.getBindings(element, bindingContext) || {};
	                if (!bindings.value) return;

	                if (ui.item && ui.item.value) {
	                    bindings.value(ui.item.value);
	                }
	                    // enable new elements to be added to the list
	                else if (opts.allowNewValues) bindings.value($(this).val());
	                else {
	                    bindings.value(null);
	                    if (ko.DEBUG) {
	                        console.log('The selected value was not found in the options list. To allow new values, include the \"allowNewValues=\'true\'\" value in the \"autocompleteOptions\" binding attribute.');
	                    }
	                }
	            }

	            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
	              $( element ).autocomplete( 'destroy' );
	              ko.utils.arrayForEach( disposables, function( disposable ) {
	                disposable.dispose();
	              });
	            });
	        },
	    };

	    ko.bindingHandlers.tabs = {
	        init: function (element, valueAccessor, allBindings) {
	            var $element = $(element);
	            var value = ko.utils.unwrapObservable(valueAccessor());
	            $(element).val(value);

	            var options = ko.utils.unwrapObservable(allBindings().tabOptions) || {};

	            $element.on("tabsactivate", onTabActivate);
	            $element.on("tabscreate", onTabCreate);

	            $element.tabs(options);

	            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                $element.tabs("destroy");
	                $element.off('tabsactivate', onTabActivate);
	                $element.off('tabscreate', onTabCreate);
	            });


	            function onTabActivate(event, ui) {
	                bindValueFromUI(ui.newTab.text(), ui.newPanel[0]);
	            }

	            function onTabCreate(event, ui) {
	                bindValueFromUI(ui.tab.text(), ui.panel[0]);
	            }

	            function bindValueFromUI(name, panel) {
	                if (!ko.isObservable(valueAccessor())) return;
	                valueAccessor()({
	                    name: name,
	                    data: getDataBoundObjectFor(panel)
	                });
	            }

	            function getDataBoundObjectFor(tabPanel) {
	                if (!tabPanel) return undefined;
	                var panelContext = ko.contextFor(tabPanel);
	                if (!panelContext) return undefined;
	                var bindings = ko.bindingProvider.instance.getBindings(tabPanel, panelContext) || {};
	                return bindings.with || panelContext.$data;
	            }
	        },
	    };

	    ko.bindingHandlers.ajaxStatus = {
	        init: function (element, valueAccessor) {
	            var value = valueAccessor();
	            if (value.ajaxSuccess == undefined
	                || value.ajaxFailure == undefined
	                || value.ajaxWorking == undefined) throw new Error("The bound value is not valid for use with the ajaxStatus binding.");

	            ko.applyBindingsToNode(element, {
	                css: {
	                    working: value.ajaxWorking,
	                    success: value.ajaxSuccess,
	                    fail: value.ajaxFailure,
	                    ajaxStatus: true,
	                }
	            });
	        }
	    };


	    // Dragons be here...
	    // allows up/down arrows, mouse-click dragging,
	    // and mouse-click wheel
	    // accepts property 'negative'in allBindings to allow negative numbers
	    ko.bindingHandlers.numValue = {
	        init: function (element, valueAccessor, allBindings) {
	            console.warn("numValue binding handler is being used! This should be replaced with the numericObservable.");
	            var num = valueAccessor();
	            var bindings = ko.utils.unwrapObservable(allBindings());
	            var isChar = function (key) { return key >= 65 && key <= 90; };
	            var up = 38, down = 40;
	            $(element).keydown(function (evt) {
	                var key = evt.keyCode;
	                var iVal = parseInt(element.value);
	                if (key === up || key === down) {
	                    if (key === up) iVal++;
	                    else if (key == down && (bindings.negative || iVal > 0)) iVal--;
	                }
	                else if (isChar(key) && !evt.ctrlKey) evt.preventDefault();
	                if (!isNaN(iVal) && iVal != null) num(iVal);
	                else num(null);

	                return true;
	            });
	            var isDown = false;
	            var lastY = 0;
	            var buffer = 10;
	            $(element).mousedown(function (e) { isDown = true; return true; });
	            $(document).mouseup(function (e) { isDown = false; return true; });
	            $(document).mousemove(function (e) {
	                if (isDown) {
	                    var y = e.pageY;
	                    if (!lastY) lastY = y;
	                    if (y > lastY + buffer && (bindings.negative || num() > 0)) {
	                        num(num() - 1);
	                        lastY = y;
	                    }
	                    else if (y + buffer < lastY) {
	                        num(num() + 1);
	                        lastY = y;
	                    }
	                }
	            });
	            $(document).on("mousewheel", function (evt) {
	                if (isDown) {
	                    var delta = evt.originalEvent.wheelDelta;
	                    if (delta > 0) {
	                        num(num() + 1);
	                    }
	                    else if (delta < 0 && (bindings.negative || num() > 0)) {
	                        num(num() - 1);
	                    }
	                }
	            });

	            // show validations as well
	            return ko.bindingHandlers['validationCore'].init(element, valueAccessor, allBindings);
	        },
	        update: function (element, valueAccessor, allBindings) {
	            var val = ko.utils.unwrapObservable(valueAccessor());
	            if (!isNaN(val)) element.value = val;
	        }
	    };

	    ko.bindingHandlers.resizable = {
	        init: function (element, valueAccessor) {
	            var alsoResizeSelector = ko.unwrap(valueAccessor());
	            if (typeof alsoResizeSelector != "string") alsoResizeSelector = '';
	            $(element).resizable({
	                alsoResize: alsoResizeSelector,
	                minWidth: 300,
	                minHeight: 100
	            });
	        }
	    };

	    ko.bindingHandlers.accordion = {
	        init: function (element, valueAccessor) {
	        },
	        update: function (element, valueAccessor) {
	            var opts = ko.utils.unwrapObservable(valueAccessor());
	            $(element).accordion(opts);
	        }
	    };

	    ko.bindingHandlers.slimscroll = {
	        init: function (element) {
	            var $el = $(element);
	            $el.slimscroll({
	                //alwaysVisible: true,
	                railColor: '#222',
	                height: "100%"
	                //railVisible: true
	            });
	        }
	    };

	    // Focuses next .form-control when Enter is pressed
	    ko.bindingHandlers.tabOnEnter = {
	        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
	            var allBindings = allBindingsAccessor();
	            $(element).keypress(function (event) {
	                var keyCode = (event.which ? event.which : event.keyCode);
	                if (keyCode === 13) {
	                    var index = $('.form-control').index(event.target) + 1;
	                    var $next = $('.form-control').eq(index);

	                    $next.focus();
	                    $next.select();
	                    return false;
	                }
	                return true;
	            });
	        }
	    };

	    /** Trigger valueAccessor on Enter keypress
	      * @param {function} valueAccessor - Function to call
	      */
	    ko.bindingHandlers.onEnter = {
	      init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
	        var allBindings = allBindingsAccessor();
	        var value = valueAccessor();
	        $(element).keypress(function (event) {
	          var keyCode = (event.which ? event.which : event.keyCode);
	          if (keyCode === 13) {
	            value.call(viewModel);
	            return false;
	          }
	          return true;
	        });
	      }
	    };
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 33 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(jQuery) {// @preserve jQuery.floatThead 1.4.5 - http://mkoryak.github.io/floatThead/ - Copyright (c) 2012 - 2016 Misha Koryak
	// @license MIT

	/* @author Misha Koryak
	 * @projectDescription lock a table header in place while scrolling - without breaking styles or events bound to the header
	 *
	 * Dependencies:
	 * jquery 1.9.0 + [required] OR jquery 1.7.0 + jquery UI core
	 *
	 * http://mkoryak.github.io/floatThead/
	 *
	 * Tested on FF13+, Chrome 21+, IE8, IE9, IE10, IE11
	 *
	 */
	(function( $ ) {
	  /**
	   * provides a default config object. You can modify this after including this script if you want to change the init defaults
	   * @type {Object}
	   */
	  $.floatThead = $.floatThead || {};
	  $.floatThead.defaults = {
	    headerCellSelector: 'tr:visible:first>*:visible', //thead cells are this.
	    zIndex: 1001, //zindex of the floating thead (actually a container div)
	    position: 'auto', // 'fixed', 'absolute', 'auto'. auto picks the best for your table scrolling type.
	    top: 0, //String or function($table) - offset from top of window where the header should not pass above
	    bottom: 0, //String or function($table) - offset from the bottom of the table where the header should stop scrolling
	    scrollContainer: function($table) { // or boolean 'true' (use offsetParent) | function -> if the table has horizontal scroll bars then this is the container that has overflow:auto and causes those scroll bars
	      return $([]);
	    },
	    responsiveContainer: function($table) { // only valid if scrollContainer is not used (ie window scrolling). this is the container which will control y scrolling at some mobile breakpoints
	      return $([]);
	    },
	    getSizingRow: function($table, $cols, $fthCells){ // this is only called when using IE,
	      // override it if the first row of the table is going to contain colgroups (any cell spans greater than one col)
	      // it should return a jquery object containing a wrapped set of table cells comprising a row that contains no col spans and is visible
	      return $table.find('tbody tr:visible:first>*:visible');
	    },
	    floatTableClass: 'floatThead-table',
	    floatWrapperClass: 'floatThead-wrapper',
	    floatContainerClass: 'floatThead-container',
	    copyTableClass: true, //copy 'class' attribute from table into the floated table so that the styles match.
	    autoReflow: false, //(undocumented) - use MutationObserver api to reflow automatically when internal table DOM changes
	    debug: false, //print possible issues (that don't prevent script loading) to console, if console exists.
	    support: { //should we bind events that expect these frameworks to be present and/or check for them?
	      bootstrap: true,
	      datatables: true,
	      jqueryUI: true,
	      perfectScrollbar: true
	    }
	  };

	  var util = window._;

	  var canObserveMutations = typeof MutationObserver !== 'undefined';


	  //browser stuff
	  var ieVersion = function(){for(var a=3,b=document.createElement("b"),c=b.all||[];a = 1+a,b.innerHTML="<!--[if gt IE "+ a +"]><i><![endif]-->",c[0];);return 4<a?a:document.documentMode}();
	  var isFF = /Gecko\//.test(navigator.userAgent);
	  var isWebkit = /WebKit\//.test(navigator.userAgent);

	  if(!(ieVersion || isFF || isWebkit)){
	    ieVersion = 11; //yey a hack!
	  }

	  //safari 7 (and perhaps others) reports table width to be parent container's width if max-width is set on table. see: https://github.com/mkoryak/floatThead/issues/108
	  var isTableWidthBug = function(){
	    if(isWebkit) {
	      var $test = $('<div>').css('width', 0).append(
	        $('<table>').css('max-width', '100%').append(
	          $('<tr>').append(
	            $('<th>').append(
	              $('<div>').css('min-width', 100).text('X')
	            )
	          )
	        )
	      );
	      $("body").append($test);
	      var ret = ($test.find("table").width() == 0);
	      $test.remove();
	      return ret;
	    }
	    return false;
	  };

	  var createElements = !isFF && !ieVersion; //FF can read width from <col> elements, but webkit cannot

	  var $window = $(window);

	  if(!window.matchMedia) {
	    var _beforePrint = window.onbeforeprint;
	    var _afterPrint = window.onafterprint;
	    window.onbeforeprint = function () {
	      _beforePrint && _beforePrint();
	      $window.triggerHandler("beforeprint");
	    };
	    window.onafterprint = function () {
	      _afterPrint && _afterPrint();
	      $window.triggerHandler("afterprint");
	    };
	  }

	  /**
	   * @param debounceMs
	   * @param cb
	   */
	  function windowResize(eventName, cb){
	    if(ieVersion == 8){ //ie8 is crap: https://github.com/mkoryak/floatThead/issues/65
	      var winWidth = $window.width();
	      var debouncedCb = util.debounce(function(){
	        var winWidthNew = $window.width();
	        if(winWidth != winWidthNew){
	          winWidth = winWidthNew;
	          cb();
	        }
	      }, 1);
	      $window.on(eventName, debouncedCb);
	    } else {
	      $window.on(eventName, util.debounce(cb, 1));
	    }
	  }

	  function getClosestScrollContainer($elem) {
	    var elem = $elem[0];
	    var parent = elem.parentElement;

	    do {
	      var pos = window
	          .getComputedStyle(parent)
	          .getPropertyValue('overflow');

	      if (pos != 'visible') break;

	    } while (parent = parent.parentElement);

	    if(parent == document.body){
	      return $([]);
	    }
	    return $(parent);
	  }


	  function debug(str){
	    window && window.console && window.console.error && window.console.error("jQuery.floatThead: " + str);
	  }

	  //returns fractional pixel widths
	  function getOffsetWidth(el) {
	    var rect = el.getBoundingClientRect();
	    return rect.width || rect.right - rect.left;
	  }

	  /**
	   * try to calculate the scrollbar width for your browser/os
	   * @return {Number}
	   */
	  function scrollbarWidth() {
	    var $div = $('<div>').css({ //borrowed from anti-scroll
	      'width': 50,
	      'height': 50,
	      'overflow-y': 'scroll',
	      'position': 'absolute',
	      'top': -200,
	      'left': -200
	    }).append(
	      $('<div>').css({
	        'height': 100,
	        'width': '100%'
	      })
	    );
	    $('body').append($div);
	    var w1 = $div.innerWidth();
	    var w2 = $('div', $div).innerWidth();
	    $div.remove();
	    return w1 - w2;
	  }
	  /**
	   * Check if a given table has been datatableized (http://datatables.net)
	   * @param $table
	   * @return {Boolean}
	   */
	  function isDatatable($table){
	    if($table.dataTableSettings){
	      for(var i = 0; i < $table.dataTableSettings.length; i++){
	        var table = $table.dataTableSettings[i].nTable;
	        if($table[0] == table){
	          return true;
	        }
	      }
	    }
	    return false;
	  }

	  function tableWidth($table, $fthCells, isOuter){
	    // see: https://github.com/mkoryak/floatThead/issues/108
	    var fn = isOuter ? "outerWidth": "width";
	    if(isTableWidthBug && $table.css("max-width")){
	      var w = 0;
	      if(isOuter) {
	        w += parseInt($table.css("borderLeft"), 10);
	        w += parseInt($table.css("borderRight"), 10);
	      }
	      for(var i=0; i < $fthCells.length; i++){
	        w += $fthCells.get(i).offsetWidth;
	      }
	      return w;
	    } else {
	      return $table[fn]();
	    }
	  }
	  $.fn.floatThead = function(map){
	    map = map || {};
	    if(!util){ //may have been included after the script? lets try to grab it again.
	      util = window._ || $.floatThead._;
	      if(!util){
	        throw new Error("jquery.floatThead-slim.js requires underscore. You should use the non-lite version since you do not have underscore.");
	      }
	    }

	    if(ieVersion < 8){
	      return this; //no more crappy browser support.
	    }

	    var mObs = null; //mutation observer lives in here if we can use it / make it

	    if(util.isFunction(isTableWidthBug)) {
	      isTableWidthBug = isTableWidthBug();
	    }

	    if(util.isString(map)){
	      var command = map;
	      var args = Array.prototype.slice.call(arguments, 1);
	      var ret = this;
	      this.filter('table').each(function(){
	        var $this = $(this);
	        var opts = $this.data('floatThead-lazy');
	        if(opts){
	          $this.floatThead(opts);
	        }
	        var obj = $this.data('floatThead-attached');
	        if(obj && util.isFunction(obj[command])){
	          var r = obj[command].apply(this, args);
	          if(r !== undefined){
	            ret = r;
	          }
	        }
	      });
	      return ret;
	    }
	    var opts = $.extend({}, $.floatThead.defaults || {}, map);

	    $.each(map, function(key, val){
	      if((!(key in $.floatThead.defaults)) && opts.debug){
	        debug("Used ["+key+"] key to init plugin, but that param is not an option for the plugin. Valid options are: "+ (util.keys($.floatThead.defaults)).join(', '));
	      }
	    });
	    if(opts.debug){
	      var v = $.fn.jquery.split(".");
	      if(parseInt(v[0], 10) == 1 && parseInt(v[1], 10) <= 7){
	        debug("jQuery version "+$.fn.jquery+" detected! This plugin supports 1.8 or better, or 1.7.x with jQuery UI 1.8.24 -> http://jqueryui.com/resources/download/jquery-ui-1.8.24.zip")
	      }
	    }

	    this.filter(':not(.'+opts.floatTableClass+')').each(function(){
	      var floatTheadId = util.uniqueId();
	      var $table = $(this);
	      if($table.data('floatThead-attached')){
	        return true; //continue the each loop
	      }
	      if(!$table.is('table')){
	        throw new Error('jQuery.floatThead must be run on a table element. ex: $("table").floatThead();');
	      }
	      canObserveMutations = opts.autoReflow && canObserveMutations; //option defaults to false!
	      var $header = $table.children('thead:first');
	      var $tbody = $table.children('tbody:first');
	      if($header.length == 0 || $tbody.length == 0){
	        $table.data('floatThead-lazy', opts);
	        $table.unbind("reflow").one('reflow', function(){
	          $table.floatThead(opts);
	        });
	        return;
	      }
	      if($table.data('floatThead-lazy')){
	        $table.unbind("reflow");
	      }
	      $table.data('floatThead-lazy', false);

	      var headerFloated = true;
	      var scrollingTop, scrollingBottom;
	      var scrollbarOffset = {vertical: 0, horizontal: 0};
	      var scWidth = scrollbarWidth();
	      var lastColumnCount = 0; //used by columnNum()

	      if(opts.scrollContainer === true){
	        opts.scrollContainer = getClosestScrollContainer;
	      }

	      var $scrollContainer = opts.scrollContainer($table) || $([]); //guard against returned nulls
	      var locked = $scrollContainer.length > 0;
	      var $responsiveContainer = locked ? $([]) : opts.responsiveContainer($table) || $([]);
	      var responsive = isResponsiveContainerActive();

	      var useAbsolutePositioning = null;
	      if(typeof opts.useAbsolutePositioning !== 'undefined'){
	        opts.position = 'auto';
	        if(opts.useAbsolutePositioning){
	          opts.position = opts.useAbsolutePositioning ? 'absolute' : 'fixed';
	        }
	        debug("option 'useAbsolutePositioning' has been removed in v1.3.0, use `position:'"+opts.position+"'` instead. See docs for more info: http://mkoryak.github.io/floatThead/#options")
	      }
	      if(typeof opts.scrollingTop !== 'undefined'){
	        opts.top = opts.scrollingTop;
	        debug("option 'scrollingTop' has been renamed to 'top' in v1.3.0. See docs for more info: http://mkoryak.github.io/floatThead/#options");
	      }
	      if(typeof opts.scrollingBottom !== 'undefined'){
	        opts.bottom = opts.scrollingBottom;
	        debug("option 'scrollingBottom' has been renamed to 'bottom' in v1.3.0. See docs for more info: http://mkoryak.github.io/floatThead/#options");
	      }


	      if (opts.position == 'auto') {
	        useAbsolutePositioning = null;
	      } else if (opts.position == 'fixed') {
	        useAbsolutePositioning = false;
	      } else if (opts.position == 'absolute'){
	        useAbsolutePositioning = true;
	      } else if (opts.debug) {
	        debug('Invalid value given to "position" option, valid is "fixed", "absolute" and "auto". You passed: ', opts.position);
	      }

	      if(useAbsolutePositioning == null){ //defaults: locked=true, !locked=false
	        useAbsolutePositioning = locked;
	      }
	      var $caption = $table.find("caption");
	      var haveCaption = $caption.length == 1;
	      if(haveCaption){
	        var captionAlignTop = ($caption.css("caption-side") || $caption.attr("align") || "top") === "top";
	      }

	      var $fthGrp = $('<fthfoot>').css({
	        'display': 'table-footer-group',
	        'border-spacing': 0,
	        'height': 0,
	        'border-collapse': 'collapse',
	        'visibility': 'hidden'
	      });

	      var wrappedContainer = false; //used with absolute positioning enabled. did we need to wrap the scrollContainer/table with a relative div?
	      var $wrapper = $([]); //used when absolute positioning enabled - wraps the table and the float container
	      var absoluteToFixedOnScroll = ieVersion <= 9 && !locked && useAbsolutePositioning; //on IE using absolute positioning doesn't look good with window scrolling, so we change position to fixed on scroll, and then change it back to absolute when done.
	      var $floatTable = $("<table/>");
	      var $floatColGroup = $("<colgroup/>");
	      var $tableColGroup = $table.children('colgroup:first');
	      var existingColGroup = true;
	      if($tableColGroup.length == 0){
	        $tableColGroup = $("<colgroup/>");
	        existingColGroup = false;
	      }
	      var $fthRow = $('<fthtr>').css({ //created unstyled elements (used for sizing the table because chrome can't read <col> width)
	        'display': 'table-row',
	        'border-spacing': 0,
	        'height': 0,
	        'border-collapse': 'collapse'
	      });
	      var $floatContainer = $('<div>').css('overflow', 'hidden').attr('aria-hidden', 'true');
	      var floatTableHidden = false; //this happens when the table is hidden and we do magic when making it visible
	      var $newHeader = $("<thead/>");
	      var $sizerRow = $('<tr class="size-row" aria-hidden="true"/>');
	      var $sizerCells = $([]);
	      var $tableCells = $([]); //used for sizing - either $sizerCells or $tableColGroup cols. $tableColGroup cols are only created in chrome for borderCollapse:collapse because of a chrome bug.
	      var $headerCells = $([]);
	      var $fthCells = $([]); //created elements

	      $newHeader.append($sizerRow);
	      $table.prepend($tableColGroup);
	      if(createElements){
	        $fthGrp.append($fthRow);
	        $table.append($fthGrp);
	      }

	      $floatTable.append($floatColGroup);
	      $floatContainer.append($floatTable);
	      if(opts.copyTableClass){
	        $floatTable.attr('class', $table.attr('class'));
	      }
	      $floatTable.attr({ //copy over some deprecated table attributes that people still like to use. Good thing people don't use colgroups...
	                         'cellpadding': $table.attr('cellpadding'),
	                         'cellspacing': $table.attr('cellspacing'),
	                         'border': $table.attr('border')
	                       });
	      var tableDisplayCss = $table.css('display');
	      $floatTable.css({
	                        'borderCollapse': $table.css('borderCollapse'),
	                        'border': $table.css('border'),
	                        'display': tableDisplayCss
	                      });
	      if(!locked){
	        $floatTable.css('width', 'auto');
	      }
	      if(tableDisplayCss == 'none'){
	        floatTableHidden = true;
	      }

	      $floatTable.addClass(opts.floatTableClass).css({'margin': 0, 'border-bottom-width': 0}); //must have no margins or you won't be able to click on things under floating table

	      if(useAbsolutePositioning){
	        var makeRelative = function($container, alwaysWrap){
	          var positionCss = $container.css('position');
	          var relativeToScrollContainer = (positionCss == "relative" || positionCss == "absolute");
	          var $containerWrap = $container;
	          if(!relativeToScrollContainer || alwaysWrap){
	            var css = {"paddingLeft": $container.css('paddingLeft'), "paddingRight": $container.css('paddingRight')};
	            $floatContainer.css(css);
	            $containerWrap = $container.data('floatThead-containerWrap') || $container.wrap(
	              $('<div>').addClass(opts.floatWrapperClass).css({
	                'position': 'relative',
	                'clear': 'both'
	              })
	            ).parent();
	            $container.data('floatThead-containerWrap', $containerWrap); //multiple tables inside one scrolling container - #242
	            wrappedContainer = true;
	          }
	          return $containerWrap;
	        };
	        if(locked){
	          $wrapper = makeRelative($scrollContainer, true);
	          $wrapper.prepend($floatContainer);
	        } else {
	          $wrapper = makeRelative($table);
	          $table.before($floatContainer);
	        }
	      } else {
	        $table.before($floatContainer);
	      }


	      $floatContainer.css({
	                            position: useAbsolutePositioning ? 'absolute' : 'fixed',
	                            marginTop: 0,
	                            top:  useAbsolutePositioning ? 0 : 'auto',
	                            zIndex: opts.zIndex,
	                            willChange: 'transform'
	                          });
	      $floatContainer.addClass(opts.floatContainerClass);
	      updateScrollingOffsets();

	      var layoutFixed = {'table-layout': 'fixed'};
	      var layoutAuto = {'table-layout': $table.css('tableLayout') || 'auto'};
	      var originalTableWidth = $table[0].style.width || ""; //setting this to auto is bad: #70
	      var originalTableMinWidth = $table.css('minWidth') || "";

	      function eventName(name){
	        return name+'.fth-'+floatTheadId+'.floatTHead'
	      }

	      function setHeaderHeight(){
	        var headerHeight = 0;
	        $header.children("tr:visible").each(function(){
	          headerHeight += $(this).outerHeight(true);
	        });
	        if($table.css('border-collapse') == 'collapse') {
	          var tableBorderTopHeight = parseInt($table.css('border-top-width'), 10);
	          var cellBorderTopHeight = parseInt($table.find("thead tr:first").find(">*:first").css('border-top-width'), 10);
	          if(tableBorderTopHeight > cellBorderTopHeight) {
	            headerHeight -= (tableBorderTopHeight / 2); //id love to see some docs where this magic recipe is found..
	          }
	        }
	        $sizerRow.outerHeight(headerHeight);
	        $sizerCells.outerHeight(headerHeight);
	      }


	      function setFloatWidth(){
	        var tw = tableWidth($table, $fthCells, true);
	        var $container = responsive ? $responsiveContainer : $scrollContainer;
	        var width = $container.width() || tw;
	        var floatContainerWidth = $container.css("overflow-y") != 'hidden' ? width - scrollbarOffset.vertical : width;
	        $floatContainer.width(floatContainerWidth);
	        if(locked){
	          var percent = 100 * tw / (floatContainerWidth);
	          $floatTable.css('width', percent+'%');
	        } else {
	          $floatTable.outerWidth(tw);
	        }
	      }

	      function updateScrollingOffsets(){
	        scrollingTop = (util.isFunction(opts.top) ? opts.top($table) : opts.top) || 0;
	        scrollingBottom = (util.isFunction(opts.bottom) ? opts.bottom($table) : opts.bottom) || 0;
	      }

	      /**
	       * get the number of columns and also rebuild resizer rows if the count is different than the last count
	       */
	      function columnNum(){
	        var count;
	        var $headerColumns = $header.find(opts.headerCellSelector);
	        if(existingColGroup){
	          count = $tableColGroup.find('col').length;
	        } else {
	          count = 0;
	          $headerColumns.each(function () {
	            count += parseInt(($(this).attr('colspan') || 1), 10);
	          });
	        }
	        if(count != lastColumnCount){
	          lastColumnCount = count;
	          var cells = [], cols = [], psuedo = [], content;
	          for(var x = 0; x < count; x++){
	            content = $headerColumns.eq(x).text();
	            cells.push('<th class="floatThead-col" aria-label="'+content+'"/>');
	            cols.push('<col/>');
	            psuedo.push(
	              $('<fthtd>').css({
	                'display': 'table-cell',
	                'height': 0,
	                'width': 'auto'
	              })
	            );
	          }

	          cols = cols.join('');
	          cells = cells.join('');

	          if(createElements){
	            $fthRow.empty();
	            $fthRow.append(psuedo);
	            $fthCells = $fthRow.find('fthtd');
	          }

	          $sizerRow.html(cells);
	          $sizerCells = $sizerRow.find("th");
	          if(!existingColGroup){
	            $tableColGroup.html(cols);
	          }
	          $tableCells = $tableColGroup.find('col');
	          $floatColGroup.html(cols);
	          $headerCells = $floatColGroup.find("col");

	        }
	        return count;
	      }

	      function refloat(){ //make the thing float
	        if(!headerFloated){
	          headerFloated = true;
	          if(useAbsolutePositioning){ //#53, #56
	            var tw = tableWidth($table, $fthCells, true);
	            var wrapperWidth = $wrapper.width();
	            if(tw > wrapperWidth){
	              $table.css('minWidth', tw);
	            }
	          }
	          $table.css(layoutFixed);
	          $floatTable.css(layoutFixed);
	          $floatTable.append($header); //append because colgroup must go first in chrome
	          $tbody.before($newHeader);
	          setHeaderHeight();
	        }
	      }
	      function unfloat(){ //put the header back into the table
	        if(headerFloated){
	          headerFloated = false;
	          if(useAbsolutePositioning){ //#53, #56
	            $table.width(originalTableWidth);
	          }
	          $newHeader.detach();
	          $table.prepend($header);
	          $table.css(layoutAuto);
	          $floatTable.css(layoutAuto);
	          $table.css('minWidth', originalTableMinWidth); //this looks weird, but it's not a bug. Think about it!!
	          $table.css('minWidth', tableWidth($table, $fthCells)); //#121
	        }
	      }
	      var isHeaderFloatingLogical = false; //for the purpose of this event, the header is/isnt floating, even though the element
	                                           //might be in some other state. this is what the header looks like to the user
	      function triggerFloatEvent(isFloating){
	        if(isHeaderFloatingLogical != isFloating){
	          isHeaderFloatingLogical = isFloating;
	          $table.triggerHandler("floatThead", [isFloating, $floatContainer])
	        }
	      }
	      function changePositioning(isAbsolute){
	        if(useAbsolutePositioning != isAbsolute){
	          useAbsolutePositioning = isAbsolute;
	          $floatContainer.css({
	                                position: useAbsolutePositioning ? 'absolute' : 'fixed'
	                              });
	        }
	      }
	      function getSizingRow($table, $cols, $fthCells, ieVersion){
	        if(createElements){
	          return $fthCells;
	        } else if(ieVersion) {
	          return opts.getSizingRow($table, $cols, $fthCells);
	        } else {
	          return $cols;
	        }
	      }

	      /**
	       * returns a function that updates the floating header's cell widths.
	       * @return {Function}
	       */
	      function reflow(){
	        var i;
	        var numCols = columnNum(); //if the tables columns changed dynamically since last time (datatables), rebuild the sizer rows and get a new count

	        return function(){
	          //Cache the current scrollLeft value so that it can be reset post reflow
	          var scrollLeft = $floatContainer.scrollLeft();
	          $tableCells = $tableColGroup.find('col');
	          var $rowCells = getSizingRow($table, $tableCells, $fthCells, ieVersion);

	          if($rowCells.length == numCols && numCols > 0){
	            if(!existingColGroup){
	              for(i=0; i < numCols; i++){
	                $tableCells.eq(i).css('width', '');
	              }
	            }
	            unfloat();
	            var widths = [];
	            for(i=0; i < numCols; i++){
	              widths[i] = getOffsetWidth($rowCells.get(i));
	            }
	            for(i=0; i < numCols; i++){
	              $headerCells.eq(i).width(widths[i]);
	              $tableCells.eq(i).width(widths[i]);
	            }
	            refloat();
	          } else {
	            $floatTable.append($header);
	            $table.css(layoutAuto);
	            $floatTable.css(layoutAuto);
	            setHeaderHeight();
	          }
	          //Set back the current scrollLeft value on floatContainer
	          $floatContainer.scrollLeft(scrollLeft);
	          $table.triggerHandler("reflowed", [$floatContainer]);
	        };
	      }

	      function floatContainerBorderWidth(side){
	        var border = $scrollContainer.css("border-"+side+"-width");
	        var w = 0;
	        if (border && ~border.indexOf('px')) {
	          w = parseInt(border, 10);
	        }
	        return w;
	      }

	      function isResponsiveContainerActive(){
	        return $responsiveContainer.css("overflow-x") == 'auto';
	      }
	      /**
	       * first performs initial calculations that we expect to not change when the table, window, or scrolling container are scrolled.
	       * returns a function that calculates the floating container's top and left coords. takes into account if we are using page scrolling or inner scrolling
	       * @return {Function}
	       */
	      function calculateFloatContainerPosFn(){
	        var scrollingContainerTop = $scrollContainer.scrollTop();

	        //this floatEnd calc was moved out of the returned function because we assume the table height doesn't change (otherwise we must reinit by calling calculateFloatContainerPosFn)
	        var floatEnd;
	        var tableContainerGap = 0;
	        var captionHeight = haveCaption ? $caption.outerHeight(true) : 0;
	        var captionScrollOffset = captionAlignTop ? captionHeight : -captionHeight;

	        var floatContainerHeight = $floatContainer.height();
	        var tableOffset = $table.offset();
	        var tableLeftGap = 0; //can be caused by border on container (only in locked mode)
	        var tableTopGap = 0;
	        if(locked){
	          var containerOffset = $scrollContainer.offset();
	          tableContainerGap = tableOffset.top - containerOffset.top + scrollingContainerTop;
	          if(haveCaption && captionAlignTop){
	            tableContainerGap += captionHeight;
	          }
	          tableLeftGap = floatContainerBorderWidth('left');
	          tableTopGap = floatContainerBorderWidth('top');
	          tableContainerGap -= tableTopGap;
	        } else {
	          floatEnd = tableOffset.top - scrollingTop - floatContainerHeight + scrollingBottom + scrollbarOffset.horizontal;
	        }
	        var windowTop = $window.scrollTop();
	        var windowLeft = $window.scrollLeft();
	        var scrollContainerLeft = (
	            isResponsiveContainerActive() ?  $responsiveContainer :
	            (locked ? $scrollContainer : $window)
	        ).scrollLeft();

	        return function(eventType){
	          responsive = isResponsiveContainerActive();

	          var isTableHidden = $table[0].offsetWidth <= 0 && $table[0].offsetHeight <= 0;
	          if(!isTableHidden && floatTableHidden) {
	            floatTableHidden = false;
	            setTimeout(function(){
	              $table.triggerHandler("reflow");
	            }, 1);
	            return null;
	          }
	          if(isTableHidden){ //it's hidden
	            floatTableHidden = true;
	            if(!useAbsolutePositioning){
	              return null;
	            }
	          }

	          if(eventType == 'windowScroll'){
	            windowTop = $window.scrollTop();
	            windowLeft = $window.scrollLeft();
	          } else if(eventType == 'containerScroll'){
	            if($responsiveContainer.length){
	              if(!responsive){
	                return; //we dont care about the event if we arent responsive right now
	              }
	              scrollContainerLeft = $responsiveContainer.scrollLeft();
	            } else {
	              scrollingContainerTop = $scrollContainer.scrollTop();
	              scrollContainerLeft = $scrollContainer.scrollLeft();
	            }
	          } else if(eventType != 'init') {
	            windowTop = $window.scrollTop();
	            windowLeft = $window.scrollLeft();
	            scrollingContainerTop = $scrollContainer.scrollTop();
	            scrollContainerLeft =  (responsive ? $responsiveContainer : $scrollContainer).scrollLeft();
	          }
	          if(isWebkit && (windowTop < 0 || windowLeft < 0)){ //chrome overscroll effect at the top of the page - breaks fixed positioned floated headers
	            return;
	          }

	          if(absoluteToFixedOnScroll){
	            if(eventType == 'windowScrollDone'){
	              changePositioning(true); //change to absolute
	            } else {
	              changePositioning(false); //change to fixed
	            }
	          } else if(eventType == 'windowScrollDone'){
	            return null; //event is fired when they stop scrolling. ignore it if not 'absoluteToFixedOnScroll'
	          }

	          tableOffset = $table.offset();
	          if(haveCaption && captionAlignTop){
	            tableOffset.top += captionHeight;
	          }
	          var top, left;
	          var tableHeight = $table.outerHeight();

	          if(locked && useAbsolutePositioning){ //inner scrolling, absolute positioning
	            if (tableContainerGap >= scrollingContainerTop) {
	              var gap = tableContainerGap - scrollingContainerTop + tableTopGap;
	              top = gap > 0 ? gap : 0;
	              triggerFloatEvent(false);
	            } else {
	              top = wrappedContainer ? tableTopGap : scrollingContainerTop;
	              //headers stop at the top of the viewport
	              triggerFloatEvent(true);
	            }
	            left = tableLeftGap;
	          } else if(!locked && useAbsolutePositioning) { //window scrolling, absolute positioning
	            if(windowTop > floatEnd + tableHeight + captionScrollOffset){
	              top = tableHeight - floatContainerHeight + captionScrollOffset; //scrolled past table
	            } else if (tableOffset.top >= windowTop + scrollingTop) {
	              top = 0; //scrolling to table
	              unfloat();
	              triggerFloatEvent(false);
	            } else {
	              top = scrollingTop + windowTop - tableOffset.top + tableContainerGap + (captionAlignTop ? captionHeight : 0);
	              refloat(); //scrolling within table. header floated
	              triggerFloatEvent(true);
	            }
	            left =  scrollContainerLeft;
	          } else if(locked && !useAbsolutePositioning){ //inner scrolling, fixed positioning
	            if (tableContainerGap > scrollingContainerTop || scrollingContainerTop - tableContainerGap > tableHeight) {
	              top = tableOffset.top - windowTop;
	              unfloat();
	              triggerFloatEvent(false);
	            } else {
	              top = tableOffset.top + scrollingContainerTop  - windowTop - tableContainerGap;
	              refloat();
	              triggerFloatEvent(true);
	              //headers stop at the top of the viewport
	            }
	            left = tableOffset.left + scrollContainerLeft - windowLeft;
	          } else if(!locked && !useAbsolutePositioning) { //window scrolling, fixed positioning
	            if(windowTop > floatEnd + tableHeight + captionScrollOffset){
	              top = tableHeight + scrollingTop - windowTop + floatEnd + captionScrollOffset;
	              //scrolled past the bottom of the table
	            } else if (tableOffset.top > windowTop + scrollingTop) {
	              top = tableOffset.top - windowTop;
	              refloat();
	              triggerFloatEvent(false); //this is a weird case, the header never gets unfloated and i have no no way to know
	              //scrolled past the top of the table
	            } else {
	              //scrolling within the table
	              top = scrollingTop;
	              triggerFloatEvent(true);
	            }
	            left = tableOffset.left + scrollContainerLeft - windowLeft;
	          }
	          return {top: Math.round(top), left: Math.round(left)};
	        };
	      }
	      /**
	       * returns a function that caches old floating container position and only updates css when the position changes
	       * @return {Function}
	       */
	      function repositionFloatContainerFn(){
	        var oldTop = null;
	        var oldLeft = null;
	        var oldScrollLeft = null;
	        return function(pos, setWidth, setHeight){
	          if(pos != null && (oldTop != pos.top || oldLeft != pos.left)){
	            if(ieVersion === 8){
	              $floatContainer.css({
	                top: pos.top,
	                left: pos.left
	              });
	            } else {
	              var transform = 'translateX(' + pos.left + 'px) translateY(' + pos.top + 'px)';
	              $floatContainer.css({
	                '-webkit-transform' : transform,
	                '-moz-transform'    : transform,
	                '-ms-transform'     : transform,
	                '-o-transform'      : transform,
	                'transform'         : transform,
	                'top': 0,
	                'left': 0
	              });
	            }
	            oldTop = pos.top;
	            oldLeft = pos.left;
	          }
	          if(setWidth){
	            setFloatWidth();
	          }
	          if(setHeight){
	            setHeaderHeight();
	          }
	          var scrollLeft = (responsive ? $responsiveContainer : $scrollContainer).scrollLeft();
	          if(!useAbsolutePositioning || oldScrollLeft != scrollLeft){
	            $floatContainer.scrollLeft(scrollLeft);
	            oldScrollLeft = scrollLeft;
	          }
	        }
	      }

	      /**
	       * checks if THIS table has scrollbars, and finds their widths
	       */
	      function calculateScrollBarSize(){ //this should happen after the floating table has been positioned
	        if($scrollContainer.length){
	          if(opts.support && opts.support.perfectScrollbar && $scrollContainer.data().perfectScrollbar){
	            scrollbarOffset = {horizontal:0, vertical:0};
	          } else {
	            if($scrollContainer.css('overflow-x') == 'scroll'){
	              scrollbarOffset.horizontal = scWidth;
	            } else {
	              var sw = $scrollContainer.width(), tw = tableWidth($table, $fthCells);
	              var offsetv = sh < th ? scWidth : 0;
	              scrollbarOffset.horizontal = sw - offsetv < tw ? scWidth : 0;
	            }
	            if($scrollContainer.css('overflow-y') == 'scroll'){
	              scrollbarOffset.vertical = scWidth;
	            } else {
	              var sh = $scrollContainer.height(), th = $table.height();
	              var offseth = sw < tw ? scWidth : 0;
	              scrollbarOffset.vertical = sh - offseth < th ? scWidth : 0;
	            }
	          }
	        }
	      }
	      //finish up. create all calculation functions and bind them to events
	      calculateScrollBarSize();

	      var flow;

	      var ensureReflow = function(){
	        flow = reflow();
	        flow();
	      };

	      ensureReflow();

	      var calculateFloatContainerPos = calculateFloatContainerPosFn();
	      var repositionFloatContainer = repositionFloatContainerFn();

	      repositionFloatContainer(calculateFloatContainerPos('init'), true); //this must come after reflow because reflow changes scrollLeft back to 0 when it rips out the thead

	      var windowScrollDoneEvent = util.debounce(function(){
	        repositionFloatContainer(calculateFloatContainerPos('windowScrollDone'), false);
	      }, 1);

	      var windowScrollEvent = function(){
	        repositionFloatContainer(calculateFloatContainerPos('windowScroll'), false);
	        if(absoluteToFixedOnScroll){
	          windowScrollDoneEvent();
	        }
	      };
	      var containerScrollEvent = function(){
	        repositionFloatContainer(calculateFloatContainerPos('containerScroll'), false);
	      };


	      var windowResizeEvent = function(){
	        if($table.is(":hidden")){
	          return;
	        }
	        updateScrollingOffsets();
	        calculateScrollBarSize();
	        ensureReflow();
	        calculateFloatContainerPos = calculateFloatContainerPosFn();
	        repositionFloatContainer = repositionFloatContainerFn();
	        repositionFloatContainer(calculateFloatContainerPos('resize'), true, true);
	      };
	      var reflowEvent = util.debounce(function(){
	        if($table.is(":hidden")){
	          return;
	        }
	        calculateScrollBarSize();
	        updateScrollingOffsets();
	        ensureReflow();
	        calculateFloatContainerPos = calculateFloatContainerPosFn();
	        repositionFloatContainer(calculateFloatContainerPos('reflow'), true);
	      }, 1);

	      /////// printing stuff
	      var beforePrint = function(){
	        unfloat();
	      };
	      var afterPrint = function(){
	        refloat();
	      };
	      var printEvent = function(mql){
	        //make printing the table work properly on IE10+
	        if(mql.matches) {
	          beforePrint();
	        } else {
	          afterPrint();
	        }
	      };

	      var matchMediaPrint;
	      if(window.matchMedia && window.matchMedia('print').addListener){
	        matchMediaPrint = window.matchMedia("print");
	        matchMediaPrint.addListener(printEvent);
	      } else {
	        $window.on('beforeprint', beforePrint);
	        $window.on('afterprint', afterPrint);
	      }
	      ////// end printing stuff


	      if(locked){ //internal scrolling
	        if(useAbsolutePositioning){
	          $scrollContainer.on(eventName('scroll'), containerScrollEvent);
	        } else {
	          $scrollContainer.on(eventName('scroll'), containerScrollEvent);
	          $window.on(eventName('scroll'), windowScrollEvent);
	        }
	      } else { //window scrolling
	        $responsiveContainer.on(eventName('scroll'), containerScrollEvent);
	        $window.on(eventName('scroll'), windowScrollEvent);
	      }

	      $window.on(eventName('load'), reflowEvent); //for tables with images

	      windowResize(eventName('resize'), windowResizeEvent);
	      $table.on('reflow', reflowEvent);
	      if(opts.support && opts.support.datatables && isDatatable($table)){
	        $table
	            .on('filter', reflowEvent)
	            .on('sort',   reflowEvent)
	            .on('page',   reflowEvent);
	      }

	      if(opts.support && opts.support.bootstrap) {
	        $window.on(eventName('shown.bs.tab'), reflowEvent); // people cant seem to figure out how to use this plugin with bs3 tabs... so this :P
	      }
	      if(opts.support && opts.support.jqueryUI) {
	        $window.on(eventName('tabsactivate'), reflowEvent); // same thing for jqueryui
	      }


	      if (canObserveMutations) {
	        var mutationElement = null;
	        if(util.isFunction(opts.autoReflow)){
	          mutationElement = opts.autoReflow($table, $scrollContainer)
	        }
	        if(!mutationElement) {
	          mutationElement = $scrollContainer.length ? $scrollContainer[0] : $table[0]
	        }
	        mObs = new MutationObserver(function(e){
	          var wasTableRelated = function(nodes){
	            return nodes && nodes[0] && (nodes[0].nodeName == "THEAD" || nodes[0].nodeName == "TD"|| nodes[0].nodeName == "TH");
	          };
	          for(var i=0; i < e.length; i++){
	            if(!(wasTableRelated(e[i].addedNodes) || wasTableRelated(e[i].removedNodes))){
	              reflowEvent();
	              break;
	            }
	          }
	        });
	        mObs.observe(mutationElement, {
	          childList: true,
	          subtree: true
	        });
	      }

	      //attach some useful functions to the table.
	      $table.data('floatThead-attached', {
	        destroy: function(){
	          var ns = '.fth-'+floatTheadId;
	          unfloat();
	          $table.css(layoutAuto);
	          $tableColGroup.remove();
	          createElements && $fthGrp.remove();
	          if($newHeader.parent().length){ //only if it's in the DOM
	            $newHeader.replaceWith($header);
	          }
	          triggerFloatEvent(false);
	          if(canObserveMutations){
	            mObs.disconnect();
	            mObs = null;
	          }
	          $table.off('reflow reflowed');
	          $scrollContainer.off(ns);
	          $responsiveContainer.off(ns);
	          if (wrappedContainer) {
	            if ($scrollContainer.length) {
	              $scrollContainer.unwrap();
	            }
	            else {
	              $table.unwrap();
	            }
	          }
	          if(locked){
	            $scrollContainer.data('floatThead-containerWrap', false);
	          } else {
	            $table.data('floatThead-containerWrap', false);
	          }
	          $table.css('minWidth', originalTableMinWidth);
	          $floatContainer.remove();
	          $table.data('floatThead-attached', false);
	          $window.off(ns);
	          if (matchMediaPrint) {
	            matchMediaPrint.removeListener(printEvent);
	          }
	          beforePrint = afterPrint = function(){};

	          return function reinit(){
	            return $table.floatThead(opts);
	          }
	        },
	        reflow: function(){
	          reflowEvent();
	        },
	        setHeaderHeight: function(){
	          setHeaderHeight();
	        },
	        getFloatContainer: function(){
	          return $floatContainer;
	        },
	        getRowGroups: function(){
	          if(headerFloated){
	            return $floatContainer.find('>table>thead').add($table.children("tbody,tfoot"));
	          } else {
	            return $table.children("thead,tbody,tfoot");
	          }
	        }
	      });
	    });
	    return this;
	  };
	})((function(){
	  var $ = window.jQuery;
	  if(typeof module !== 'undefined' && module.exports && !$) {
	    // only use cjs if they dont have a jquery for me to use, and we have commonjs
	    $ = __webpack_require__(1);
	  }
	  return $;
	})());

	/* jQuery.floatThead.utils - http://mkoryak.github.io/floatThead/ - Copyright (c) 2012 - 2016 Misha Koryak
	 * License: MIT
	 *
	 * This file is required if you do not use underscore in your project and you want to use floatThead.
	 * It contains functions from underscore that the plugin uses.
	 *
	 * YOU DON'T NEED TO INCLUDE THIS IF YOU ALREADY INCLUDE UNDERSCORE!
	 *
	 */

	(function($){

	  $.floatThead = $.floatThead || {};

	  $.floatThead._  = window._ || (function(){
	    var that = {};
	    var hasOwnProperty = Object.prototype.hasOwnProperty, isThings = ['Arguments', 'Function', 'String', 'Number', 'Date', 'RegExp'];
	    that.has = function(obj, key) {
	      return hasOwnProperty.call(obj, key);
	    };
	    that.keys = function(obj) {
	      if (obj !== Object(obj)) throw new TypeError('Invalid object');
	      var keys = [];
	      for (var key in obj) if (that.has(obj, key)) keys.push(key);
	      return keys;
	    };
	    var idCounter = 0;
	    that.uniqueId = function(prefix) {
	      var id = ++idCounter + '';
	      return prefix ? prefix + id : id;
	    };
	    $.each(isThings, function(){
	      var name = this;
	      that['is' + name] = function(obj) {
	        return Object.prototype.toString.call(obj) == '[object ' + name + ']';
	      };
	    });
	    that.debounce = function(func, wait, immediate) {
	      var timeout, args, context, timestamp, result;
	      return function() {
	        context = this;
	        args = arguments;
	        timestamp = new Date();
	        var later = function() {
	          var last = (new Date()) - timestamp;
	          if (last < wait) {
	            timeout = setTimeout(later, wait - last);
	          } else {
	            timeout = null;
	            if (!immediate) result = func.apply(context, args);
	          }
	        };
	        var callNow = immediate && !timeout;
	        if (!timeout) {
	          timeout = setTimeout(later, wait);
	        }
	        if (callNow) result = func.apply(context, args);
	        return result;
	      };
	    };
	    return that;
	  })();
	})(jQuery);


	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 34 */
/***/ (function(module, exports) {

	/*** IMPORTS FROM imports-loader ***/
	var require = false;
	var module = false;

	/*=============================================================================
		Author:			Eric M. Barnard - @ericmbarnard								
		License:		MIT (http://opensource.org/licenses/mit-license.php)		
																					
		Description:	Validation Library for KnockoutJS							
		Version:		2.0.2											
	===============================================================================
	*/
	/*globals require: false, exports: false, define: false, ko: false */

	(function (factory) {
		// Module systems magic dance.

		if (typeof require === "function" && typeof exports === "object" && typeof module === "object") {
			// CommonJS or Node: hard-coded dependency on "knockout"
			factory(require("knockout"), exports);
		} else if (typeof define === "function" && define["amd"]) {
			// AMD anonymous module with hard-coded dependency on "knockout"
			define(["knockout", "exports"], factory);
		} else {
			// <script> tag: use the global `ko` object, attaching a `mapping` property
			factory(ko, ko.validation = {});
		}
	}(function ( ko, exports ) {

		if (typeof (ko) === 'undefined') {
			throw new Error('Knockout is required, please ensure it is loaded before loading this validation plug-in');
		}

		// create our namespace object
		ko.validation = exports;

		var kv = ko.validation,
			koUtils = ko.utils,
			unwrap = koUtils.unwrapObservable,
			forEach = koUtils.arrayForEach,
			extend = koUtils.extend;
	;/*global ko: false*/

	var defaults = {
		registerExtenders: true,
		messagesOnModified: true,
		errorsAsTitle: true,            // enables/disables showing of errors as title attribute of the target element.
		errorsAsTitleOnModified: false, // shows the error when hovering the input field (decorateElement must be true)
		messageTemplate: null,
		insertMessages: true,           // automatically inserts validation messages as <span></span>
		parseInputAttributes: false,    // parses the HTML5 validation attribute from a form element and adds that to the object
		writeInputAttributes: false,    // adds HTML5 input validation attributes to form elements that ko observable's are bound to
		decorateInputElement: false,         // false to keep backward compatibility
		decorateElementOnModified: true,// true to keep backward compatibility
		errorClass: null,               // single class for error message and element
		errorElementClass: 'validationElement',  // class to decorate error element
		errorMessageClass: 'validationMessage',  // class to decorate error message
		allowHtmlMessages: false,		// allows HTML in validation messages
		grouping: {
			deep: false,        //by default grouping is shallow
			observable: true,   //and using observables
			live: false		    //react to changes to observableArrays if observable === true
		},
		validate: {
			// throttle: 10
		}
	};

	// make a copy  so we can use 'reset' later
	var configuration = extend({}, defaults);

	configuration.html5Attributes = ['required', 'pattern', 'min', 'max', 'step'];
	configuration.html5InputTypes = ['email', 'number', 'date'];

	configuration.reset = function () {
		extend(configuration, defaults);
	};

	kv.configuration = configuration;
	;kv.utils = (function () {
		var seedId = new Date().getTime();

		var domData = {}; //hash of data objects that we reference from dom elements
		var domDataKey = '__ko_validation__';

		return {
			isArray: function (o) {
				return o.isArray || Object.prototype.toString.call(o) === '[object Array]';
			},
			isObject: function (o) {
				return o !== null && typeof o === 'object';
			},
			isNumber: function(o) {
				return !isNaN(o);	
			},
			isObservableArray: function(instance) {
				return !!instance &&
						typeof instance["remove"] === "function" &&
						typeof instance["removeAll"] === "function" &&
						typeof instance["destroy"] === "function" &&
						typeof instance["destroyAll"] === "function" &&
						typeof instance["indexOf"] === "function" &&
						typeof instance["replace"] === "function";
			},
			values: function (o) {
				var r = [];
				for (var i in o) {
					if (o.hasOwnProperty(i)) {
						r.push(o[i]);
					}
				}
				return r;
			},
			getValue: function (o) {
				return (typeof o === 'function' ? o() : o);
			},
			hasAttribute: function (node, attr) {
				return node.getAttribute(attr) !== null;
			},
			getAttribute: function (element, attr) {
				return element.getAttribute(attr);
			},
			setAttribute: function (element, attr, value) {
				return element.setAttribute(attr, value);
			},
			isValidatable: function (o) {
				return !!(o && o.rules && o.isValid && o.isModified);
			},
			insertAfter: function (node, newNode) {
				node.parentNode.insertBefore(newNode, node.nextSibling);
			},
			newId: function () {
				return seedId += 1;
			},
			getConfigOptions: function (element) {
				var options = kv.utils.contextFor(element);

				return options || kv.configuration;
			},
			setDomData: function (node, data) {
				var key = node[domDataKey];

				if (!key) {
					node[domDataKey] = key = kv.utils.newId();
				}

				domData[key] = data;
			},
			getDomData: function (node) {
				var key = node[domDataKey];

				if (!key) {
					return undefined;
				}

				return domData[key];
			},
			contextFor: function (node) {
				switch (node.nodeType) {
					case 1:
					case 8:
						var context = kv.utils.getDomData(node);
						if (context) { return context; }
						if (node.parentNode) { return kv.utils.contextFor(node.parentNode); }
						break;
				}
				return undefined;
			},
			isEmptyVal: function (val) {
				if (val === undefined) {
					return true;
				}
				if (val === null) {
					return true;
				}
				if (val === "") {
					return true;
				}
			},
			getOriginalElementTitle: function (element) {
				var savedOriginalTitle = kv.utils.getAttribute(element, 'data-orig-title'),
					currentTitle = element.title,
					hasSavedOriginalTitle = kv.utils.hasAttribute(element, 'data-orig-title');

				return hasSavedOriginalTitle ?
					savedOriginalTitle : currentTitle;
			},
			async: function (expr) {
				if (window.setImmediate) { window.setImmediate(expr); }
				else { window.setTimeout(expr, 0); }
			},
			forEach: function (object, callback) {
				if (kv.utils.isArray(object)) {
					return forEach(object, callback);
				}
				for (var prop in object) {
					if (object.hasOwnProperty(prop)) {
						callback(object[prop], prop);
					}
				}
			}
		};
	}());;var api = (function () {

		var isInitialized = 0,
			configuration = kv.configuration,
			utils = kv.utils;

		function cleanUpSubscriptions(context) {
			forEach(context.subscriptions, function (subscription) {
				subscription.dispose();
			});
			context.subscriptions = [];
		}

		function dispose(context) {
			if (context.options.deep) {
				forEach(context.flagged, function (obj) {
					delete obj.__kv_traversed;
				});
				context.flagged.length = 0;
			}

			if (!context.options.live) {
				cleanUpSubscriptions(context);
			}
		}

		function runTraversal(obj, context) {
			context.validatables = [];
			cleanUpSubscriptions(context);
			traverseGraph(obj, context);
			dispose(context);
		}

		function traverseGraph(obj, context, level) {
			var objValues = [],
				val = obj.peek ? obj.peek() : obj;

			if (obj.__kv_traversed === true) {
				return;
			}

			if (context.options.deep) {
				obj.__kv_traversed = true;
				context.flagged.push(obj);
			}

			//default level value depends on deep option.
			level = (level !== undefined ? level : context.options.deep ? 1 : -1);

			// if object is observable then add it to the list
			if (ko.isObservable(obj)) {
				// ensure it's validatable but don't extend validatedObservable because it
				// would overwrite isValid property.
				if (!obj.errors && !utils.isValidatable(obj)) {
					obj.extend({ validatable: true });
				}
				context.validatables.push(obj);

				if (context.options.live && utils.isObservableArray(obj)) {
					context.subscriptions.push(obj.subscribe(function () {
						context.graphMonitor.valueHasMutated();
					}));
				}
			}

			//get list of values either from array or object but ignore non-objects
			// and destroyed objects
			if (val && !val._destroy) {
				if (utils.isArray(val)) {
					objValues = val;
				}
				else if (utils.isObject(val)) {
					objValues = utils.values(val);
				}
			}

			//process recursively if it is deep grouping
			if (level !== 0) {
				utils.forEach(objValues, function (observable) {
					//but not falsy things and not HTML Elements
					if (observable && !observable.nodeType && (!ko.isComputed(observable) || observable.rules)) {
						traverseGraph(observable, context, level + 1);
					}
				});
			}
		}

		function collectErrors(array) {
			var errors = [];
			forEach(array, function (observable) {
				// Do not collect validatedObservable errors
				if (utils.isValidatable(observable) && !observable.isValid()) {
					// Use peek because we don't want a dependency for 'error' property because it
					// changes before 'isValid' does. (Issue #99)
					errors.push(observable.error.peek());
				}
			});
			return errors;
		}

		return {
			//Call this on startup
			//any config can be overridden with the passed in options
			init: function (options, force) {
				//done run this multiple times if we don't really want to
				if (isInitialized > 0 && !force) {
					return;
				}

				//because we will be accessing options properties it has to be an object at least
				options = options || {};
				//if specific error classes are not provided then apply generic errorClass
				//it has to be done on option so that options.errorClass can override default
				//errorElementClass and errorMessage class but not those provided in options
				options.errorElementClass = options.errorElementClass || options.errorClass || configuration.errorElementClass;
				options.errorMessageClass = options.errorMessageClass || options.errorClass || configuration.errorMessageClass;

				extend(configuration, options);

				if (configuration.registerExtenders) {
					kv.registerExtenders();
				}

				isInitialized = 1;
			},

			// resets the config back to its original state
			reset: kv.configuration.reset,

			// recursively walks a viewModel and creates an object that
			// provides validation information for the entire viewModel
			// obj -> the viewModel to walk
			// options -> {
			//	  deep: false, // if true, will walk past the first level of viewModel properties
			//	  observable: false // if true, returns a computed observable indicating if the viewModel is valid
			// }
			group: function group(obj, options) { // array of observables or viewModel
				options = extend(extend({}, configuration.grouping), options);

				var context = {
					options: options,
					graphMonitor: ko.observable(),
					flagged: [],
					subscriptions: [],
					validatables: []
				};

				var result = null;

				//if using observables then traverse structure once and add observables
				if (options.observable) {
					result = ko.computed(function () {
						context.graphMonitor(); //register dependency
						runTraversal(obj, context);
						return collectErrors(context.validatables);
					});
				}
				else { //if not using observables then every call to error() should traverse the structure
					result = function () {
						runTraversal(obj, context);
						return collectErrors(context.validatables);
					};
				}

				result.showAllMessages = function (show) { // thanks @heliosPortal
					if (show === undefined) {//default to true
						show = true;
					}

					result.forEach(function (observable) {
						if (utils.isValidatable(observable)) {
							observable.isModified(show);
						}
					});
				};

				result.isAnyMessageShown = function () {
					var invalidAndModifiedPresent;

					invalidAndModifiedPresent = !!result.find(function (observable) {
						return utils.isValidatable(observable) && !observable.isValid() && observable.isModified();
					});
					return invalidAndModifiedPresent;
				};

				result.filter = function(predicate) {
					predicate = predicate || function () { return true; };
					// ensure we have latest changes
					result();

					return koUtils.arrayFilter(context.validatables, predicate);
				};

				result.find = function(predicate) {
					predicate = predicate || function () { return true; };
					// ensure we have latest changes
					result();

					return koUtils.arrayFirst(context.validatables, predicate);
				};

				result.forEach = function(callback) {
					callback = callback || function () { };
					// ensure we have latest changes
					result();

					forEach(context.validatables, callback);
				};

				result.map = function(mapping) {
					mapping = mapping || function (item) { return item; };
					// ensure we have latest changes
					result();

					return koUtils.arrayMap(context.validatables, mapping);
				};

				/**
				 * @private You should not rely on this method being here.
				 * It's a private method and it may change in the future.
				 *
				 * @description Updates the validated object and collects errors from it.
				 */
				result._updateState = function(newValue) {
					if (!utils.isObject(newValue)) {
						throw new Error('An object is required.');
					}
					obj = newValue;
					if (options.observable) {
						context.graphMonitor.valueHasMutated();
					}
					else {
						runTraversal(newValue, context);
						return collectErrors(context.validatables);
					}
				};
				return result;
			},

			formatMessage: function (message, params, observable) {
				if (utils.isObject(params) && params.typeAttr) {
					params = params.value;
				}
				if (typeof (message) === 'function') {
					return message(params, observable);
				}
				var replacements = unwrap(params) || [];
				if (!utils.isArray(replacements)) {
					replacements = [replacements];
				}
				return message.replace(/{(\d+)}/gi, function(match, index) {
					if (typeof replacements[index] !== 'undefined') {
						return replacements[index];
					}
					return match;
				});
			},

			// addRule:
			// This takes in a ko.observable and a Rule Context - which is just a rule name and params to supply to the validator
			// ie: kv.addRule(myObservable, {
			//		  rule: 'required',
			//		  params: true
			//	  });
			//
			addRule: function (observable, rule) {
				observable.extend({ validatable: true });

				var hasRule = !!koUtils.arrayFirst(observable.rules(), function(item) {
					return item.rule && item.rule === rule.rule;
				});

				if (!hasRule) {
					//push a Rule Context to the observables local array of Rule Contexts
					observable.rules.push(rule);
				}
				return observable;
			},

			// addAnonymousRule:
			// Anonymous Rules essentially have all the properties of a Rule, but are only specific for a certain property
			// and developers typically are wanting to add them on the fly or not register a rule with the 'kv.rules' object
			//
			// Example:
			// var test = ko.observable('something').extend{(
			//	  validation: {
			//		  validator: function(val, someOtherVal){
			//			  return true;
			//		  },
			//		  message: "Something must be really wrong!',
			//		  params: true
			//	  }
			//  )};
			addAnonymousRule: function (observable, ruleObj) {
				if (ruleObj['message'] === undefined) {
					ruleObj['message'] = 'Error';
				}

				//make sure onlyIf is honoured
				if (ruleObj.onlyIf) {
					ruleObj.condition = ruleObj.onlyIf;
				}

				//add the anonymous rule to the observable
				kv.addRule(observable, ruleObj);
			},

			addExtender: function (ruleName) {
				ko.extenders[ruleName] = function (observable, params) {
					//params can come in a few flavors
					// 1. Just the params to be passed to the validator
					// 2. An object containing the Message to be used and the Params to pass to the validator
					// 3. A condition when the validation rule to be applied
					//
					// Example:
					// var test = ko.observable(3).extend({
					//	  max: {
					//		  message: 'This special field has a Max of {0}',
					//		  params: 2,
					//		  onlyIf: function() {
					//					  return specialField.IsVisible();
					//				  }
					//	  }
					//  )};
					//
					if (params && (params.message || params.onlyIf)) { //if it has a message or condition object, then its an object literal to use
						return kv.addRule(observable, {
							rule: ruleName,
							message: params.message,
							params: utils.isEmptyVal(params.params) ? true : params.params,
							condition: params.onlyIf
						});
					} else {
						return kv.addRule(observable, {
							rule: ruleName,
							params: params
						});
					}
				};
			},

			// loops through all kv.rules and adds them as extenders to
			// ko.extenders
			registerExtenders: function () { // root extenders optional, use 'validation' extender if would cause conflicts
				if (configuration.registerExtenders) {
					for (var ruleName in kv.rules) {
						if (kv.rules.hasOwnProperty(ruleName)) {
							if (!ko.extenders[ruleName]) {
								kv.addExtender(ruleName);
							}
						}
					}
				}
			},

			//creates a span next to the @element with the specified error class
			insertValidationMessage: function (element) {
				var span = document.createElement('SPAN');
				span.className = utils.getConfigOptions(element).errorMessageClass;
				utils.insertAfter(element, span);
				return span;
			},

			// if html-5 validation attributes have been specified, this parses
			// the attributes on @element
			parseInputValidationAttributes: function (element, valueAccessor) {
				forEach(kv.configuration.html5Attributes, function (attr) {
					if (utils.hasAttribute(element, attr)) {

						var params = element.getAttribute(attr) || true;

						if (attr === 'min' || attr === 'max')
						{
							// If we're validating based on the min and max attributes, we'll
							// need to know what the 'type' attribute is set to
							var typeAttr = element.getAttribute('type');
							if (typeof typeAttr === "undefined" || !typeAttr)
							{
								// From http://www.w3.org/TR/html-markup/input:
								//   An input element with no type attribute specified represents the
								//   same thing as an input element with its type attribute set to "text".
								typeAttr = "text";
							}
							params = {typeAttr: typeAttr, value: params};
						}

						kv.addRule(valueAccessor(), {
							rule: attr,
							params: params
						});
					}
				});

				var currentType = element.getAttribute('type');
				forEach(kv.configuration.html5InputTypes, function (type) {
					if (type === currentType) {
						kv.addRule(valueAccessor(), {
							rule: (type === 'date') ? 'dateISO' : type,
							params: true
						});
					}
				});
			},

			// writes html5 validation attributes on the element passed in
			writeInputValidationAttributes: function (element, valueAccessor) {
				var observable = valueAccessor();

				if (!observable || !observable.rules) {
					return;
				}

				var contexts = observable.rules(); // observable array

				// loop through the attributes and add the information needed
				forEach(kv.configuration.html5Attributes, function (attr) {
					var ctx = koUtils.arrayFirst(contexts, function (ctx) {
						return ctx.rule && ctx.rule.toLowerCase() === attr.toLowerCase();
					});

					if (!ctx) {
						return;
					}

					// we have a rule matching a validation attribute at this point
					// so lets add it to the element along with the params
					ko.computed({
						read: function() {
							var params = ko.unwrap(ctx.params);

							// we have to do some special things for the pattern validation
							if (ctx.rule === "pattern" && params instanceof RegExp) {
								// we need the pure string representation of the RegExpr without the //gi stuff
								params = params.source;
							}

							element.setAttribute(attr, params);
						},
						disposeWhenNodeIsRemoved: element
					});
				});

				contexts = null;
			},

			//take an existing binding handler and make it cause automatic validations
			makeBindingHandlerValidatable: function (handlerName) {
				var init = ko.bindingHandlers[handlerName].init;

				ko.bindingHandlers[handlerName].init = function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

					init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);

					return ko.bindingHandlers['validationCore'].init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
				};
			},

			// visit an objects properties and apply validation rules from a definition
			setRules: function (target, definition) {
				var setRules = function (target, definition) {
					if (!target || !definition) { return; }

					for (var prop in definition) {
						if (!definition.hasOwnProperty(prop)) { continue; }
						var ruleDefinitions = definition[prop];

						//check the target property exists and has a value
						if (!target[prop]) { continue; }
						var targetValue = target[prop],
							unwrappedTargetValue = unwrap(targetValue),
							rules = {},
							nonRules = {};

						for (var rule in ruleDefinitions) {
							if (!ruleDefinitions.hasOwnProperty(rule)) { continue; }
							if (kv.rules[rule]) {
								rules[rule] = ruleDefinitions[rule];
							} else {
								nonRules[rule] = ruleDefinitions[rule];
							}
						}

						//apply rules
						if (ko.isObservable(targetValue)) {
							targetValue.extend(rules);
						}

						//then apply child rules
						//if it's an array, apply rules to all children
						if (unwrappedTargetValue && utils.isArray(unwrappedTargetValue)) {
							for (var i = 0; i < unwrappedTargetValue.length; i++) {
								setRules(unwrappedTargetValue[i], nonRules);
							}
							//otherwise, just apply to this property
						} else {
							setRules(unwrappedTargetValue, nonRules);
						}
					}
				};
				setRules(target, definition);
			}
		};

	}());

	// expose api publicly
	extend(ko.validation, api);
	;//Validation Rules:
	// You can view and override messages or rules via:
	// kv.rules[ruleName]
	//
	// To implement a custom Rule, simply use this template:
	// kv.rules['<custom rule name>'] = {
	//      validator: function (val, param) {
	//          <custom logic>
	//          return <true or false>;
	//      },
	//      message: '<custom validation message>' //optionally you can also use a '{0}' to denote a placeholder that will be replaced with your 'param'
	// };
	//
	// Example:
	// kv.rules['mustEqual'] = {
	//      validator: function( val, mustEqualVal ){
	//          return val === mustEqualVal;
	//      },
	//      message: 'This field must equal {0}'
	// };
	//
	kv.rules = {};
	kv.rules['required'] = {
		validator: function (val, required) {
			var testVal;

			if (val === undefined || val === null) {
				return !required;
			}

			testVal = val;
			if (typeof (val) === 'string') {
				if (String.prototype.trim) {
					testVal = val.trim();
				}
				else {
					testVal = val.replace(/^\s+|\s+$/g, '');
				}
			}

			if (!required) {// if they passed: { required: false }, then don't require this
				return true;
			}

			return ((testVal + '').length > 0);
		},
		message: 'This field is required.'
	};

	function minMaxValidatorFactory(validatorName) {
	    var isMaxValidation = validatorName === "max";

	    return function (val, options) {
	        if (kv.utils.isEmptyVal(val)) {
	            return true;
	        }

	        var comparisonValue, type;
	        if (options.typeAttr === undefined) {
	            // This validator is being called from javascript rather than
	            // being bound from markup
	            type = "text";
	            comparisonValue = options;
	        } else {
	            type = options.typeAttr;
	            comparisonValue = options.value;
	        }

	        // From http://www.w3.org/TR/2012/WD-html5-20121025/common-input-element-attributes.html#attr-input-min,
	        // if the value is parseable to a number, then the minimum should be numeric
	        if (!isNaN(comparisonValue) && !(comparisonValue instanceof Date)) {
	            type = "number";
	        }

	        var regex, valMatches, comparisonValueMatches;
	        switch (type.toLowerCase()) {
	            case "week":
	                regex = /^(\d{4})-W(\d{2})$/;
	                valMatches = val.match(regex);
	                if (valMatches === null) {
	                    throw new Error("Invalid value for " + validatorName + " attribute for week input.  Should look like " +
	                        "'2000-W33' http://www.w3.org/TR/html-markup/input.week.html#input.week.attrs.min");
	                }
	                comparisonValueMatches = comparisonValue.match(regex);
	                // If no regex matches were found, validation fails
	                if (!comparisonValueMatches) {
	                    return false;
	                }

	                if (isMaxValidation) {
	                    return (valMatches[1] < comparisonValueMatches[1]) || // older year
	                        // same year, older week
	                        ((valMatches[1] === comparisonValueMatches[1]) && (valMatches[2] <= comparisonValueMatches[2]));
	                } else {
	                    return (valMatches[1] > comparisonValueMatches[1]) || // newer year
	                        // same year, newer week
	                        ((valMatches[1] === comparisonValueMatches[1]) && (valMatches[2] >= comparisonValueMatches[2]));
	                }
	                break;

	            case "month":
	                regex = /^(\d{4})-(\d{2})$/;
	                valMatches = val.match(regex);
	                if (valMatches === null) {
	                    throw new Error("Invalid value for " + validatorName + " attribute for month input.  Should look like " +
	                        "'2000-03' http://www.w3.org/TR/html-markup/input.month.html#input.month.attrs.min");
	                }
	                comparisonValueMatches = comparisonValue.match(regex);
	                // If no regex matches were found, validation fails
	                if (!comparisonValueMatches) {
	                    return false;
	                }

	                if (isMaxValidation) {
	                    return ((valMatches[1] < comparisonValueMatches[1]) || // older year
	                        // same year, older month
	                        ((valMatches[1] === comparisonValueMatches[1]) && (valMatches[2] <= comparisonValueMatches[2])));
	                } else {
	                    return (valMatches[1] > comparisonValueMatches[1]) || // newer year
	                        // same year, newer month
	                        ((valMatches[1] === comparisonValueMatches[1]) && (valMatches[2] >= comparisonValueMatches[2]));
	                }
	                break;

	            case "number":
	            case "range":
	                if (isMaxValidation) {
	                    return (!isNaN(val) && parseFloat(val) <= parseFloat(comparisonValue));
	                } else {
	                    return (!isNaN(val) && parseFloat(val) >= parseFloat(comparisonValue));
	                }
	                break;

	            default:
	                if (isMaxValidation) {
	                    return val <= comparisonValue;
	                } else {
	                    return val >= comparisonValue;
	                }
	        }
	    };
	}

	kv.rules['min'] = {
		validator: minMaxValidatorFactory("min"),
		message: 'Please enter a value greater than or equal to {0}.'
	};

	kv.rules['max'] = {
		validator: minMaxValidatorFactory("max"),
		message: 'Please enter a value less than or equal to {0}.'
	};

	kv.rules['minLength'] = {
		validator: function (val, minLength) {
			if(kv.utils.isEmptyVal(val)) { return true; }
			var normalizedVal = kv.utils.isNumber(val) ? ('' + val) : val;
			return normalizedVal.length >= minLength;
		},
		message: 'Please enter at least {0} characters.'
	};

	kv.rules['maxLength'] = {
		validator: function (val, maxLength) {
			if(kv.utils.isEmptyVal(val)) { return true; }
			var normalizedVal = kv.utils.isNumber(val) ? ('' + val) : val;
			return normalizedVal.length <= maxLength;
		},
		message: 'Please enter no more than {0} characters.'
	};

	kv.rules['pattern'] = {
		validator: function (val, regex) {
			return kv.utils.isEmptyVal(val) || val.toString().match(regex) !== null;
		},
		message: 'Please check this value.'
	};

	kv.rules['step'] = {
		validator: function (val, step) {

			// in order to handle steps of .1 & .01 etc.. Modulus won't work
			// if the value is a decimal, so we have to correct for that
			if (kv.utils.isEmptyVal(val) || step === 'any') { return true; }
			var dif = (val * 100) % (step * 100);
			return Math.abs(dif) < 0.00001 || Math.abs(1 - dif) < 0.00001;
		},
		message: 'The value must increment by {0}.'
	};

	kv.rules['email'] = {
		validator: function (val, validate) {
			if (!validate) { return true; }

			//I think an empty email address is also a valid entry
			//if one want's to enforce entry it should be done with 'required: true'
			return kv.utils.isEmptyVal(val) || (
				// jquery validate regex - thanks Scott Gonzalez
				validate && /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$/i.test(val)
			);
		},
		message: 'Please enter a proper email address.'
	};

	kv.rules['date'] = {
		validator: function (value, validate) {
			if (!validate) { return true; }
			return kv.utils.isEmptyVal(value) || (validate && !/Invalid|NaN/.test(new Date(value)));
		},
		message: 'Please enter a proper date.'
	};

	kv.rules['dateISO'] = {
		validator: function (value, validate) {
			if (!validate) { return true; }
			return kv.utils.isEmptyVal(value) || (validate && /^\d{4}[-/](?:0?[1-9]|1[012])[-/](?:0?[1-9]|[12][0-9]|3[01])$/.test(value));
		},
		message: 'Please enter a proper date.'
	};

	kv.rules['number'] = {
		validator: function (value, validate) {
			if (!validate) { return true; }
			return kv.utils.isEmptyVal(value) || (validate && /^-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$/.test(value));
		},
		message: 'Please enter a number.'
	};

	kv.rules['digit'] = {
		validator: function (value, validate) {
			if (!validate) { return true; }
			return kv.utils.isEmptyVal(value) || (validate && /^\d+$/.test(value));
		},
		message: 'Please enter a digit.'
	};

	kv.rules['phoneUS'] = {
		validator: function (phoneNumber, validate) {
			if (!validate) { return true; }
			if (kv.utils.isEmptyVal(phoneNumber)) { return true; } // makes it optional, use 'required' rule if it should be required
			if (typeof (phoneNumber) !== 'string') { return false; }
			phoneNumber = phoneNumber.replace(/\s+/g, "");
			return validate && phoneNumber.length > 9 && phoneNumber.match(/^(1-?)?(\([2-9]\d{2}\)|[2-9]\d{2})-?[2-9]\d{2}-?\d{4}$/);
		},
		message: 'Please specify a valid phone number.'
	};

	kv.rules['equal'] = {
		validator: function (val, params) {
			var otherValue = params;
			return val === kv.utils.getValue(otherValue);
		},
		message: 'Values must equal.'
	};

	kv.rules['notEqual'] = {
		validator: function (val, params) {
			var otherValue = params;
			return val !== kv.utils.getValue(otherValue);
		},
		message: 'Please choose another value.'
	};

	//unique in collection
	// options are:
	//    collection: array or function returning (observable) array
	//              in which the value has to be unique
	//    valueAccessor: function that returns value from an object stored in collection
	//              if it is null the value is compared directly
	//    external: set to true when object you are validating is automatically updating collection
	kv.rules['unique'] = {
		validator: function (val, options) {
			var c = kv.utils.getValue(options.collection),
				external = kv.utils.getValue(options.externalValue),
				counter = 0;

			if (!val || !c) { return true; }

			koUtils.arrayFilter(c, function (item) {
				if (val === (options.valueAccessor ? options.valueAccessor(item) : item)) { counter++; }
			});
			// if value is external even 1 same value in collection means the value is not unique
			return counter < (!!external ? 1 : 2);
		},
		message: 'Please make sure the value is unique.'
	};


	//now register all of these!
	(function () {
		kv.registerExtenders();
	}());
	;// The core binding handler
	// this allows us to setup any value binding that internally always
	// performs the same functionality
	ko.bindingHandlers['validationCore'] = (function () {

		return {
			init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
				var config = kv.utils.getConfigOptions(element);
				var observable = valueAccessor();

				// parse html5 input validation attributes, optional feature
				if (config.parseInputAttributes) {
					kv.utils.async(function () { kv.parseInputValidationAttributes(element, valueAccessor); });
				}

				// if requested insert message element and apply bindings
				if (config.insertMessages && kv.utils.isValidatable(observable)) {

					// insert the <span></span>
					var validationMessageElement = kv.insertValidationMessage(element);

					// if we're told to use a template, make sure that gets rendered
					if (config.messageTemplate) {
						ko.renderTemplate(config.messageTemplate, { field: observable }, null, validationMessageElement, 'replaceNode');
					} else {
						ko.applyBindingsToNode(validationMessageElement, { validationMessage: observable });
					}
				}

				// write the html5 attributes if indicated by the config
				if (config.writeInputAttributes && kv.utils.isValidatable(observable)) {

					kv.writeInputValidationAttributes(element, valueAccessor);
				}

				// if requested, add binding to decorate element
				if (config.decorateInputElement && kv.utils.isValidatable(observable)) {
					ko.applyBindingsToNode(element, { validationElement: observable });
				}
			}
		};

	}());

	// override for KO's default 'value', 'checked', 'textInput' and selectedOptions bindings
	kv.makeBindingHandlerValidatable("value");
	kv.makeBindingHandlerValidatable("checked");
	if (ko.bindingHandlers.textInput) {
		kv.makeBindingHandlerValidatable("textInput");
	}
	kv.makeBindingHandlerValidatable("selectedOptions");


	ko.bindingHandlers['validationMessage'] = { // individual error message, if modified or post binding
		update: function (element, valueAccessor) {
			var obsv = valueAccessor(),
				config = kv.utils.getConfigOptions(element),
				val = unwrap(obsv),
				msg = null,
				isModified = false,
				isValid = false;

			if (obsv === null || typeof obsv === 'undefined') {
				throw new Error('Cannot bind validationMessage to undefined value. data-bind expression: ' +
					element.getAttribute('data-bind'));
			}

			isModified = obsv.isModified && obsv.isModified();
			isValid = obsv.isValid && obsv.isValid();

			var error = null;
			if (!config.messagesOnModified || isModified) {
				error = isValid ? null : obsv.error;
			}

			var isVisible = !config.messagesOnModified || isModified ? !isValid : false;
			var isCurrentlyVisible = element.style.display !== "none";

			if (config.allowHtmlMessages) {
				koUtils.setHtml(element, error);
			} else {
				ko.bindingHandlers.text.update(element, function () { return error; });
			}

			if (isCurrentlyVisible && !isVisible) {
				element.style.display = 'none';
			} else if (!isCurrentlyVisible && isVisible) {
				element.style.display = '';
			}
		}
	};

	ko.bindingHandlers['validationElement'] = {
		update: function (element, valueAccessor, allBindingsAccessor) {
			var obsv = valueAccessor(),
				config = kv.utils.getConfigOptions(element),
				val = unwrap(obsv),
				msg = null,
				isModified = false,
				isValid = false;

			if (obsv === null || typeof obsv === 'undefined') {
				throw new Error('Cannot bind validationElement to undefined value. data-bind expression: ' +
					element.getAttribute('data-bind'));
			}

			isModified = obsv.isModified && obsv.isModified();
			isValid = obsv.isValid && obsv.isValid();

			// create an evaluator function that will return something like:
			// css: { validationElement: true }
			var cssSettingsAccessor = function () {
				var css = {};

				var shouldShow = ((!config.decorateElementOnModified || isModified) ? !isValid : false);

				// css: { validationElement: false }
				css[config.errorElementClass] = shouldShow;

				return css;
			};

			//add or remove class on the element;
			ko.bindingHandlers.css.update(element, cssSettingsAccessor, allBindingsAccessor);
			if (!config.errorsAsTitle) { return; }

			ko.bindingHandlers.attr.update(element, function () {
				var
					hasModification = !config.errorsAsTitleOnModified || isModified,
					title = kv.utils.getOriginalElementTitle(element);

				if (hasModification && !isValid) {
					return { title: obsv.error, 'data-orig-title': title };
				} else if (!hasModification || isValid) {
					return { title: title, 'data-orig-title': null };
				}
			});
		}
	};

	// ValidationOptions:
	// This binding handler allows you to override the initial config by setting any of the options for a specific element or context of elements
	//
	// Example:
	// <div data-bind="validationOptions: { insertMessages: true, messageTemplate: 'customTemplate', errorMessageClass: 'mySpecialClass'}">
	//      <input type="text" data-bind="value: someValue"/>
	//      <input type="text" data-bind="value: someValue2"/>
	// </div>
	ko.bindingHandlers['validationOptions'] = (function () {
		return {
			init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
				var options = unwrap(valueAccessor());
				if (options) {
					var newConfig = extend({}, kv.configuration);
					extend(newConfig, options);

					//store the validation options on the node so we can retrieve it later
					kv.utils.setDomData(element, newConfig);
				}
			}
		};
	}());
	;// Validation Extender:
	// This is for creating custom validation logic on the fly
	// Example:
	// var test = ko.observable('something').extend{(
	//      validation: {
	//          validator: function(val, someOtherVal){
	//              return true;
	//          },
	//          message: "Something must be really wrong!',
	//          params: true
	//      }
	//  )};
	ko.extenders['validation'] = function (observable, rules) { // allow single rule or array
		forEach(kv.utils.isArray(rules) ? rules : [rules], function (rule) {
			// the 'rule' being passed in here has no name to identify a core Rule,
			// so we add it as an anonymous rule
			// If the developer is wanting to use a core Rule, but use a different message see the 'addExtender' logic for examples
			kv.addAnonymousRule(observable, rule);
		});
		return observable;
	};

	//This is the extender that makes a Knockout Observable also 'Validatable'
	//examples include:
	// 1. var test = ko.observable('something').extend({validatable: true});
	// this will ensure that the Observable object is setup properly to respond to rules
	//
	// 2. test.extend({validatable: false});
	// this will remove the validation properties from the Observable object should you need to do that.
	ko.extenders['validatable'] = function (observable, options) {
		if (!kv.utils.isObject(options)) {
			options = { enable: options };
		}

		if (!('enable' in options)) {
			options.enable = true;
		}

		if (options.enable && !kv.utils.isValidatable(observable)) {
			var config = kv.configuration.validate || {};
			var validationOptions = {
				throttleEvaluation : options.throttle || config.throttle
			};

			observable.error = ko.observable(null); // holds the error message, we only need one since we stop processing validators when one is invalid

			// observable.rules:
			// ObservableArray of Rule Contexts, where a Rule Context is simply the name of a rule and the params to supply to it
			//
			// Rule Context = { rule: '<rule name>', params: '<passed in params>', message: '<Override of default Message>' }
			observable.rules = ko.observableArray(); //holds the rule Contexts to use as part of validation

			//in case async validation is occurring
			observable.isValidating = ko.observable(false);

			//the true holder of whether the observable is valid or not
			observable.__valid__ = ko.observable(true);

			observable.isModified = ko.observable(false);

			// a semi-protected observable
			observable.isValid = ko.computed(observable.__valid__);

			//manually set error state
			observable.setError = function (error) {
				var previousError = observable.error.peek();
				var previousIsValid = observable.__valid__.peek();

				observable.error(error);
				observable.__valid__(false);

				if (previousError !== error && !previousIsValid) {
					// if the observable was not valid before then isValid will not mutate,
					// hence causing any grouping to not display the latest error.
					observable.isValid.notifySubscribers();
				}
			};

			//manually clear error state
			observable.clearError = function () {
				observable.error(null);
				observable.__valid__(true);
				return observable;
			};

			//subscribe to changes in the observable
			var h_change = observable.subscribe(function () {
				observable.isModified(true);
			});

			// we use a computed here to ensure that anytime a dependency changes, the
			// validation logic evaluates
			var h_obsValidationTrigger = ko.computed(extend({
				read: function () {
					var obs = observable(),
						ruleContexts = observable.rules();

					kv.validateObservable(observable);

					return true;
				}
			}, validationOptions));

			extend(h_obsValidationTrigger, validationOptions);

			observable._disposeValidation = function () {
				//first dispose of the subscriptions
				observable.isValid.dispose();
				observable.rules.removeAll();
				h_change.dispose();
				h_obsValidationTrigger.dispose();

				delete observable['rules'];
				delete observable['error'];
				delete observable['isValid'];
				delete observable['isValidating'];
				delete observable['__valid__'];
				delete observable['isModified'];
	            delete observable['setError'];
	            delete observable['clearError'];
	            delete observable['_disposeValidation'];
			};
		} else if (options.enable === false && observable._disposeValidation) {
			observable._disposeValidation();
		}
		return observable;
	};

	function validateSync(observable, rule, ctx) {
		//Execute the validator and see if its valid
		if (!rule.validator(observable(), (ctx.params === undefined ? true : unwrap(ctx.params)))) { // default param is true, eg. required = true

			//not valid, so format the error message and stick it in the 'error' variable
			observable.setError(kv.formatMessage(
						ctx.message || rule.message,
						unwrap(ctx.params),
						observable));
			return false;
		} else {
			return true;
		}
	}

	function validateAsync(observable, rule, ctx) {
		observable.isValidating(true);

		var callBack = function (valObj) {
			var isValid = false,
				msg = '';

			if (!observable.__valid__()) {

				// since we're returning early, make sure we turn this off
				observable.isValidating(false);

				return; //if its already NOT valid, don't add to that
			}

			//we were handed back a complex object
			if (valObj['message']) {
				isValid = valObj.isValid;
				msg = valObj.message;
			} else {
				isValid = valObj;
			}

			if (!isValid) {
				//not valid, so format the error message and stick it in the 'error' variable
				observable.error(kv.formatMessage(
					msg || ctx.message || rule.message,
					unwrap(ctx.params),
					observable));
				observable.__valid__(isValid);
			}

			// tell it that we're done
			observable.isValidating(false);
		};

		//fire the validator and hand it the callback
		rule.validator(observable(), unwrap(ctx.params || true), callBack);
	}

	kv.validateObservable = function (observable) {
		var i = 0,
			rule, // the rule validator to execute
			ctx, // the current Rule Context for the loop
			ruleContexts = observable.rules(), //cache for iterator
			len = ruleContexts.length; //cache for iterator

		for (; i < len; i++) {

			//get the Rule Context info to give to the core Rule
			ctx = ruleContexts[i];

			// checks an 'onlyIf' condition
			if (ctx.condition && !ctx.condition()) {
				continue;
			}

			//get the core Rule to use for validation
			rule = ctx.rule ? kv.rules[ctx.rule] : ctx;

			if (rule['async'] || ctx['async']) {
				//run async validation
				validateAsync(observable, rule, ctx);

			} else {
				//run normal sync validation
				if (!validateSync(observable, rule, ctx)) {
					return false; //break out of the loop
				}
			}
		}
		//finally if we got this far, make the observable valid again!
		observable.clearError();
		return true;
	};
	;
	var _locales = {};
	var _currentLocale;

	kv.defineLocale = function(name, values) {
		if (name && values) {
			_locales[name.toLowerCase()] = values;
			return values;
		}
		return null;
	};

	kv.locale = function(name) {
		if (name) {
			name = name.toLowerCase();

			if (_locales.hasOwnProperty(name)) {
				kv.localize(_locales[name]);
				_currentLocale = name;
			}
			else {
				throw new Error('Localization ' + name + ' has not been loaded.');
			}
		}
		return _currentLocale;
	};

	//quick function to override rule messages
	kv.localize = function (msgTranslations) {
		var rules = kv.rules;

		//loop the properties in the object and assign the msg to the rule
		for (var ruleName in msgTranslations) {
			if (rules.hasOwnProperty(ruleName)) {
				rules[ruleName].message = msgTranslations[ruleName];
			}
		}
	};

	// Populate default locale (this will make en-US.js somewhat redundant)
	(function() {
		var localeData = {};
		var rules = kv.rules;

		for (var ruleName in rules) {
			if (rules.hasOwnProperty(ruleName)) {
				localeData[ruleName] = rules[ruleName].message;
			}
		}
		kv.defineLocale('en-us', localeData);
	})();

	// No need to invoke locale because the messages are already defined along with the rules for en-US
	_currentLocale = 'en-us';
	;/**
	 * Possible invocations:
	 * 		applyBindingsWithValidation(viewModel)
	 * 		applyBindingsWithValidation(viewModel, options)
	 * 		applyBindingsWithValidation(viewModel, rootNode)
	 *		applyBindingsWithValidation(viewModel, rootNode, options)
	 */
	ko.applyBindingsWithValidation = function (viewModel, rootNode, options) {
		var node = document.body,
			config;

		if (rootNode && rootNode.nodeType) {
			node = rootNode;
			config = options;
		}
		else {
			config = rootNode;
		}

		kv.init();

		if (config) {
			config = extend(extend({}, kv.configuration), config);
			kv.utils.setDomData(node, config);
		}

		ko.applyBindings(viewModel, node);
	};

	//override the original applyBindings so that we can ensure all new rules and what not are correctly registered
	var origApplyBindings = ko.applyBindings;
	ko.applyBindings = function (viewModel, rootNode) {

		kv.init();

		origApplyBindings(viewModel, rootNode);
	};

	ko.validatedObservable = function (initialValue, options) {
		if (!options && !kv.utils.isObject(initialValue)) {
			return ko.observable(initialValue).extend({ validatable: true });
		}

		var obsv = ko.observable(initialValue);
		obsv.errors = kv.group(kv.utils.isObject(initialValue) ? initialValue : {}, options);
		obsv.isValid = ko.observable(obsv.errors().length === 0);

		if (ko.isObservable(obsv.errors)) {
			obsv.errors.subscribe(function(errors) {
				obsv.isValid(errors.length === 0);
			});
		}
		else {
			ko.computed(obsv.errors).subscribe(function (errors) {
				obsv.isValid(errors.length === 0);
			});
		}

		obsv.subscribe(function(newValue) {
			if (!kv.utils.isObject(newValue)) {
				/*
				 * The validation group works on objects.
				 * Since the new value is a primitive (scalar, null or undefined) we need
				 * to create an empty object to pass along.
				 */
				newValue = {};
			}
			// Force the group to refresh
			obsv.errors._updateState(newValue);
			obsv.isValid(obsv.errors().length === 0);
		});

		return obsv;
	};
	;}));


/***/ }),
/* 35 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;var rvc = __webpack_require__(8);
	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9), __webpack_require__(22)], __WEBPACK_AMD_DEFINE_RESULT__ = function () {
	    ko.extenders.lotKey = function (target, options) {
	        var matchCallback, changedCallback;

	        if (typeof options === "function") {
	            matchCallback = options;
	            changedCallback = null;
	        } else if(options != undefined && typeof options === "object") {
	            matchCallback = options.matchCallback;
	            changedCallback = typeof options.changedCallback === "function" ? options.changedCallback : undefined;
	        }

		    var pattern = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{3})?(\s?)(\d{2}\d*)?$/,
		        newPattern = /^([01])?([0-9]{1})\s{0,}(\d{1,2})?\s{0,}(\d{1,3})?\s{0,}(\d{1,})?(.*)$/,
	            completePattern = /^(\d{2})(\s)(\d{2})(\s)(\d{3})(\s)(\d{2}\d*)$/,
	            isComplete = ko.observable(false);

		    target.formattedLot = ko.pureComputed({
				read: function () {
					return target();
				},
				write: function (value) {
					value = cleanInput(value);
					if (target.peek() === value) return;

					var formatted = formatAsLot(value);
					target(formatted);
				    if (formatted && formatted.match(completePattern)) {
				        isComplete(true);
				        if (typeof matchCallback === "function") matchCallback(formatted);
				        if (typeof changedCallback === "function") changedCallback(value, formatted);
				    } else changedCallback && changedCallback(value, undefined); // the second argument of the changedCallback is only returned when the lot key is complete

				}
			});
	        
			function cleanInput(input) {
				if (typeof input == "number") input = input.toString();
				if (typeof input !== "string") return undefined;
				return input.trim();
			}
			function formatAsLot(input) {
	            var re = /\d+/g,
	            newInput = input.match(re);

				if (newInput === undefined || newInput === null) { return; }
				newInput = newInput.join('');

				return newInput.replace(newPattern, function(match, p1, p2, p3, p4, p5, p6) {
				    if (p1) {
				        return [String(p1) + p2, p3, p4, p5].join(' ');
	                } else if (!p1 && (p2 === "0" || p2 === "1")) {
	                    return [p2, p3, p4, p5].join(' ');
	                } else {
	                    return [String(0) + p2, p3, p4, p5].join(' ');
	                }
	            }).trim();
			}

			target.match = function (valueToCompare) {
				var partialPattern = new RegExp('^' + target.formattedLot() + '$');
				return valueToCompare.match(partialPattern);
			};
			target.isComplete = ko.pureComputed(function () {
				return isComplete();
			}, target);
			target.Date = ko.pureComputed(function () {
			    var formattedLot = this.formattedLot();
				if (formattedLot) {
					var sections = formattedLot.split(" ");
					var days = parseInt(sections[2]);
					var defDate = "1/1/" + (parseInt(sections[1]) >= 90 ? "19" : "20");
					var date = new Date(defDate + sections[1]).addDays(days - 1);
					date.addMinutes(date.getTimezoneOffset());

					return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
				}
			}, target);
			target.formattedDate = ko.pureComputed(function () {
				var date = this();
				if (date && date != 'Invalid Date') return date.format("UTC:m/d/yyyy");
				return '';
			}, target.Date);
			target.LotType = ko.pureComputed(function () {
			  var lot = target.formattedLot();
			  if( lot ) {
			    var sections = lot.split(" ");
			    return Number(sections[0]);
			  }
			});
			target.InventoryTypeKey = ko.pureComputed(function () {
			  var lotType = target.LotType();
	      switch (lotType) {
	        case 1:
	        case 2:
	        case 3:
	        case 11:
	        case 12:
	          return rvc.lists.inventoryTypes.Chile.key;
	        case 4:
	          return rvc.lists.inventoryTypes.Additive.key;
	        case 5:
	          return rvc.lists.inventoryTypes.Packaging.key;
	        default:
	          return null;
			  }
			});
			target.Sequence = ko.pureComputed({
				read: function () {
					if (this.formattedLot()) {
						var sections = this.formattedLot().split(" ");
						if (sections.length === 4)
							return sections[3];
					}
				},
				write: function (newSeq) {
					var val = '';
					if (isComplete()) {
						var reg = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{3})?(\s?)/;
						val = this.formattedLot().match(reg)[0];
						val += newSeq < 10 ? '0' : '';
						val += newSeq;
						this.formattedLot(val);
					}
				}
			}, target);
			target.getNextLot = function () {
				var sequence = parseInt(target.Sequence());
				sequence++;
				if (sequence < 10) sequence = '0' + sequence;
				return target.formattedLot().replace(pattern, '0$2 $4 $6 ' + sequence);
			};

			target.extend({ throttle: 800 });

			target.formattedLot(target.peek());
			return target;
		};
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),
/* 36 */
/***/ (function(module, exports, __webpack_require__) {

	(function (ko) {
	    ko.validation.init({
	        insertMessages: false,
	        decorateInputElement: true,
	        errorElementClass: 'has-error',
	        errorMessageClass: 'help-block'
	    });
	}(__webpack_require__(9)));

/***/ }),
/* 37 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {__webpack_require__(37);

	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(38), __webpack_require__(39), __webpack_require__(9)], __WEBPACK_AMD_DEFINE_RESULT__ = function (tableHeaderClickHelper, propertyGetter, ko) {
	    var sortOption = {};

	    ko.bindingHandlers.sortableTable = {
	        init: function (element, valueAccessor, allBindingsAccessor) {
	            var allBindings = allBindingsAccessor();
	            allBindings.enableClick = canBeSorted;

	            ko.bindingHandlers.clickableTableHeaders.init(
	                element,
	                ko.utils.wrapAccessor(sort),
	                ko.utils.wrapAccessor(allBindings));

	            function sort(th) {
	                $(element).find('thead .' + ko.bindingHandlers.sortableTable.options.sortedCssClass)
	                    .removeClass(ko.bindingHandlers.sortableTable.options.sortedCssClass + ' ' +
	                        ko.bindingHandlers.sortableTable.options.sortedAscCssClass + ' ' +
	                        ko.bindingHandlers.sortableTable.options.sortedDescCssClass);

	                sortData(th, valueAccessor());
	            }
	        }
	    }

	    ko.bindingHandlers.sortableTable.options = {
	        sortedCssClass: 'sorted',
	        sortedAscCssClass: 'asc',
	        sortedDescCssClass: 'desc'
	    }

	    function sortData(thElement, data) {
	        var $th = $(thElement);
	        var sort = $th.attr('data-sort');
	        if (!sort) return;

	        var previousSort = sortOption;
	        var sortDirection = previousSort && previousSort.propertyName === sort
	            ? previousSort.direction * -1
	            : 1;

	        sortOption = {
	            propertyName: sort,
	            direction: sortDirection
	        };

	        // todo: get context for table's tbody in order to prevent duplicate declaration source property as valueAccessor for the body and clickable header

	        var sortFn = dynamicSortFn(sort, sortDirection);
	        if (ko.isObservable(data)) {
	          var dataCache = data();

	            data(dataCache.sort(sortFn));
	        }

	        $th.addClass(ko.bindingHandlers.sortableTable.options.sortedCssClass + ' ' +
	            (sortDirection > 0
	                ? ko.bindingHandlers.sortableTable.options.sortedAscCssClass
	                : ko.bindingHandlers.sortableTable.options.sortedDescCssClass));
	    };

	    function dynamicSortFn( sort, direction ) {
	        if ( !sort ) { return null; }

	        // 1 = Ascending, -1 = Descending
	        if (direction != -1) { direction = 1; }

	        var lt = -1 * direction,
	            gt = 1 * direction;

	        function sortFn( a, b ) {
	            var _a = ko.utils.unwrapObservable( propertyGetter.getValue( a, sort ) );
	            var _b = ko.utils.unwrapObservable( propertyGetter.getValue( b, sort ) );

	            if ( _a < _b || _a == null ) {
	              return lt;
	            } else if ( _a > _b ) {
	              return gt;
	            }

	            return 0;
	        }

	        return sortFn;
	    }

	    function canBeSorted(element) {
	        var $th = $(element);
	        var sort = $th.attr('data-sort');
	        return sort && true;
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__))

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 38 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(1), __webpack_require__(9)], __WEBPACK_AMD_DEFINE_RESULT__ = function($, ko) {
	    ko.bindingHandlers.clickableTableHeaders = {
	        init: function (element, valueAccessor, allBindings) {
	            setTimeout(function () { // allow templated bindings to be rendered
	                setupHandlers(element, valueAccessor, allBindings);
	            }, 0);
	        }
	    }

	    function setupHandlers(element, valueAccessor, allBindings) {
	        setupTableClickElements(element, valueAccessor, allBindings);
	        setupRebindTrigger(element, valueAccessor, allBindings);
	    }

	    var defaultEnableClick = function () { return true; };

	    function setupTableClickElements(element, valueAccessor, allBindings) {
	        var options = allBindings() || {};
	        var enableClick = options.enableClick || defaultEnableClick;
	        var $table = $(element);

	        $table.find('thead th').each(function (index, thElem) {
	            thElem.clickEnabled = enableClick(thElem);
	            
	            if (thElem.clickEnabled) {
	                var $th = $(thElem);
	                $th.css({
	                    cursor: 'pointer'
	                });
	            }

	            thElem = null;
	            $th = null;
	        });

	        var $thead = $table.find('thead');
	        $thead.off('click');
	        $thead.click(function (args) {
	            if (args.target.nodeType !== 1 || args.target.nodeName !== 'TH') return;
	            if (args.target.clickEnabled) valueAccessor()(args.target);
	            args.stopPropagation();
	        });

	        ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
	            $thead.off('click');
	            $thead = null;
	            $table = null;
	        });
	    }
	    function setupRebindTrigger(element, valueAccessor, allBindings) {
	        var options = allBindings() || {};
	        if (options.rebindTrigger) {
	            if (!ko.isObservable(options.rebindTrigger)) throw new Error("The \"rebindTrigger\" binding option is invalid. Expected observable.");
	            options.rebindTrigger.subscribe(function () {
	                setupTableClickElements(element, valueAccessor, allBindings);
	                //todo: clean up old bindings?
	            });
	        }
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

/***/ }),
/* 39 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_RESULT__ = function() {
	    var arrayIndexRegex = /^(\w+)?\[(\d+)\]\.?(.*)/;

	    //Object.prototype.deepSearch = function(propertyName) {
	    //    return getValueFromPropertyName(this, propertyName);
	    //}

	    return {
	        getValue: getValueFromPropertyName
	    }

	    function getValueFromPropertyName(obj, propName) {
	        if (obj == undefined) return undefined;
	        var matches = arrayIndexRegex.exec(propName);
	        if (matches && matches.length) {
	            var index = parseInt(matches[2]);
	            obj = matches[1]
	                ? obj[matches[1]][index]
	                : obj[index];

	            return matches[3]
	                ? getValueFromPropertyName(obj, matches[3])
	                : obj;
	        }

	        var paths = propName.split('.');
	        if (paths.length > 1) {
	            var p = paths.shift();
	            return getValueFromPropertyName(obj[p], paths.join('.'));
	        }

	        return obj[propName];
	    }
	}.call(exports, __webpack_require__, exports, module), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__))


/***/ }),
/* 40 */,
/* 41 */,
/* 42 */,
/* 43 */,
/* 44 */,
/* 45 */,
/* 46 */,
/* 47 */,
/* 48 */,
/* 49 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(7), __webpack_require__(9)], __WEBPACK_AMD_DEFINE_RESULT__ = function (core, ko) {
	    return {
	        getNotebookByKey: getNotebookByKey,
	        insertNote: insertNote,
	        updateNote: putNote,
	        updateNoteText: updateNoteText,
	        deleteNote: deleteNote,

	        // phase these methods out
	        putNote: putNote,
	        postNote: postNote,
	    };

	    //#region exports
	    function getNotebookByKey(key, options) {
	        return core.ajax("/api/notebooks/" + key + "/notes", options);
	    }

	    function putNote(note, options) {
	        options.data = ko.toJSON(note.toDto());
	        return core.ajaxPut("/api/notebooks/" + note.NotebookKey + "/notes/" + note.NoteKey, options);
	    }

	    function updateNoteText(note) {
	        var NoteKey = note.NoteKey;
	        var NotebookKey = note.NotebookKey;
	        var NoteText = note.toDto();

	        return core.ajaxPut("/api/notebooks/" + NotebookKey + "/notes/" + NoteKey, NoteText);
	    }

	    function insertNote(notebookKey, values) {
	        return core.ajaxPost("/api/notebooks/" + notebookKey + "/notes", values);
	    }
	    function postNote(note, options) {
	        console.warn("postNote is obsolete. Use insertNote instead.");
	        options.data = ko.toJSON(note.toDto ? note.toDto() : note);
	        return core.ajaxPost("/api/notebooks/" + note.NotebookKey + "/notes", options);
	    }

	    function deleteNote(note, options) {
	        return core.ajaxDelete("/api/notebooks/" + note.NotebookKey + "/notes/" + note.NoteKey, options);        
	    }
	    //#endregion
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),
/* 50 */,
/* 51 */,
/* 52 */,
/* 53 */,
/* 54 */,
/* 55 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(1), __webpack_require__(9), __webpack_require__(56)], __WEBPACK_AMD_DEFINE_RESULT__ = function ($, ko) {
	    var ajaxStatus = {
	        success: 2,
	        failure: -1,
	        working: 1,
	        none: 0,
	    };

	    setupKoEditStateManager();

	    return {
	        ajaxStatusHelper: initAjaxStatusHelper,
	        animateNewListElement: animateNewListElement,
	        esmHelper: initEsmHelper,
	        getDataForClickedElement: function (options) {
	            options = options || {};
	            options.isDesiredTarget = options.isDesiredTarget || function () { return true; }

	            var data = options.clickArguments[0];

	            if (options.isDesiredTarget(data)) return data;
	            if (options.clickArguments.length < 2) throw new Error("Incorrect number of arguments for click handler.");

	            var targetElement = options.clickArguments[1].originalEvent.target;

	            if (!targetElement) throw new Error("Target element could not be determined.");


	            var context = ko.contextFor(targetElement);
	            if (context && options.isDesiredTarget(context.$data)) {
	                return context.$data;
	            }

	            throw new Error("Unable to identify data for target element.");
	        },
	    };
	   

	    //#region ajaxStatusHelper
	    function initAjaxStatusHelper(target) {
	        if (target == undefined) throw new Error("Target cannot be undefined.");

	        target.ajaxStatus = ko.observable(ajaxStatus.none);
	        target.indicateSuccess = success.bind(target);
	        target.indicateWorking = working.bind(target);
	        target.indicateFailure = failure.bind(target);
	        target.clearStatus = clear.bind(target);

	        // computed properties
	        target.ajaxSuccess = ko.computed(function () {
	            return this.ajaxStatus() === ajaxStatus.success;
	        }, target);
	        target.ajaxFailure = ko.computed(function () {
	            return this.ajaxStatus() === ajaxStatus.failure;
	        }, target);
	        target.ajaxWorking = ko.computed(function () {
	            return this.ajaxStatus() === ajaxStatus.working;
	        }, target);
	        target.ajaxInactive = ko.computed(function () {
	            return this.ajaxStatus() === ajaxStatus.none;
	        }, target);

	        
	        return target;
	    }
	    
	    // functions
	    function clear() {
	        this.ajaxStatus(ajaxStatus.none);
	    }
	    function success() {
	        this.ajaxStatus(ajaxStatus.success);
	    }
	    function working() {
	        this.ajaxStatus(ajaxStatus.working);
	    }
	    function failure() {
	        this.ajaxStatus(ajaxStatus.failure);
	    }
	    //#endregion ajaxStatusHelper

	    //#region animateNewListItem
	    function animateNewListElement(options) {
	        options = options || {};
	        options.paddingTop = options.paddingTop == undefined ? 120 : options.paddingTop;
	        return function(elem) {
	            if (elem.nodeType === 1) {
	                var $elem = $(elem);
	                var origBg = $elem.css('background-color');


	                if (doScroll()) {
	                    var maxHeightContainer = $('.maxHeight-container');
	                    var floatingHeader = maxHeightContainer.find('.tableFloatingHeader');
	                    if (floatingHeader) {
	                        options.paddingTop = floatingHeader.height() + 100; // the 100 shouldn't be necessary but without it, the scrollTop goes off screen...
	                    }

	                    if (maxHeightContainer) {
	                        maxHeightContainer.animate({
	                            scrollTop: (maxHeightContainer.scrollTop() + $elem.position().top) - options.paddingTop // need to allow for floating headers
	                        }, 2000);
	                    } else {
	                        $('html, body').animate({
	                            scrollTop: $elem.offset().top - 100 // need to allow for floating headers
	                        }, 2000);
	                    }

	                    if (options.afterScrollCallback) options.afterScrollCallback();
	                }

	                $elem.css('opacity', 0);
	                $elem.animate({ backgroundColor: "#a6dbed", opacity: 1 }, 800)
	                    .delay(2500)
	                    .animate({ backgroundColor: origBg || 'rgb(255, 255, 255)' }, 1000, function() {
	                        $elem.css('background-color', ''); // remove style from element to allow css to regain control
	                    });
	            };
	        };

	        function doScroll() {
	            var scrollOption = options.scrollToItem;
	            return ko && ko.isObservable(scrollOption)
	                ? scrollOption()
	                : scrollOption;
	        }
	    }
	    //#endregion animateNewListItem

	    //#region Edit State Manager
	    function initEsmHelper(objectToTrack, options) {
	        if (!objectToTrack) throw new Error("Must provide an objectToTrack.");
	        return setup(options);

	        function setup() {
	            var esm = ko.EditStateManager(objectToTrack, options);
	            var propertiesToCopy = ['toggleEditingCommand', 'beginEditingCommand', 'endEditingCommand', 'revertEditsCommand', 'cancelEditsCommand', 'saveEditsCommand', 'isEditing', 'hasChanges'];
	            for (var prop in propertiesToCopy) {
	                if (propertiesToCopy.hasOwnProperty(prop)) {
	                    var propName = propertiesToCopy[prop];
	                    objectToTrack[propName] = esm[propName];
	                }
	            }
	            return esm;
	        }
	    }
	    function setupKoEditStateManager() {
	        ko.EditStateManager.defaultOptions = (function () {
	            var defaultOptions = {
	                include: [],
	                ignore: ['__ko_mapping__'],
	                initializeAsEditing: false,
	                isInitiallyDirty: false,
	                canSave: function () { return true; },
	                name: "[unnamed_esm]",
	                canEdit: function () { return true; },
	                canEndEditing: function () { return true; },
	            };

	            return defaultOptions;
	        })();
	    }
	    //#endregion
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),
/* 56 */
/***/ (function(module, exports) {

	/*** IMPORTS FROM imports-loader ***/
	var require = false;
	var module = false;

	(function() {
	    if (require) {
	        define(['ko'], extendKnockout);
	    } else extendKnockout(ko);

	    function extendKnockout(ko) {
	        ko.DEBUG = true;

	        ko.EditStateManager = function(objectToTrack, options) {
	            var defaultOptions = ko.EditStateManager.defaultOptions;

	            options = options || {};

	            var include = defaultOptions.include || [];
	            if (options.include && options.include.length > 0) {
	                ko.utils.arrayPushAll(include, options.include);
	            }

	            options.initializeAsEditing = options.initializeAsEditing || defaultOptions.initializeAsEditing;
	            options.isInitiallyDirty = options.isInitiallyDirty || defaultOptions.isInitiallyDirty;
	            options.canSave = options.canSave || defaultOptions.canSave;
	            options.canEdit = options.canEdit || defaultOptions.canEdit;
	            options.canEndEditing = options.canEndEditing || defaultOptions.canEndEditing;

	            var name = options.name || defaultOptions.name;

	            var cacheMapping = options.customMappings || {};
	            var customRevertFunctions = options.customRevertFunctions || {};
	            var ignore = defaultOptions.ignore.concat(options.ignore);

	            if (ko.isObservable(objectToTrack) && isArray(objectToTrack())) {
	                // object is an observableArray
	                objectToTrack = {
	                    array: objectToTrack
	                };
	            }

	            var trackedObject = buildTrackedObject();
	            var revertUntrackedChanges = options.revertUntrackedChanges || emptyFn;
	            var hasUntrackedChanges = options.hasUntrackedChanges || function() { return false; };
	            var commitUntrackedChanges = options.commitUntrackedChanges || emptyFn;

	            var revertChangesCallback = options.revertChangesCallback || emptyFn;
	            var commitChangesCallback = options.commitChangesCallback || emptyFn;
	            var beginEditingCallback = options.beginEditingCallback || emptyFn;
	            var endEditingCallback = options.endEditingCallback || emptyFn;

	            function emptyFn() {}

	            var result = function() {},
	                isInitiallyDirty = ko.observable(options.isInitiallyDirty),
	                cachedState = ko.observable(),
	                currentHash = ko.computed(function() {
	                    return serialize(trackedObject);
	                }),
	                preventDirtyCheck = ko.observable(false);

	            var isEditing = ko.observable(false);
	            var isDirty = ko.computed(function() {
	                return preventDirtyCheck() || (isInitiallyDirty() || cachedState() !== currentHash());
	            });

	            // computed properties
	            result.isEditing = ko.computed(function() {
	                return isEditing();
	            });
	            result.isReadonly = ko.computed(function() {
	                return !isEditing();
	            }, result);
	            result.hasChanges = ko.computed(function() {
	                return isDirty() || hasUntrackedChanges();
	            });
	            //#endregion

	            //#region commands
	            result.toggleEditingCommand = ko.command({
	                execute: function() {
	                    if (result.isEditing()) result.endEditingCommand.execute();
	                    else result.beginEditingCommand.execute();
	                }
	            });
	            result.beginEditingCommand = ko.command({
	                canExecute: function() { return options.canEdit() && !isEditing(); },
	                execute: function() {
	                    beginEditing();
	                    beginEditingCallback();
	                },
	                log_name: name + ".beginEditingCommand",
	            });
	            result.endEditingCommand = ko.command({
	                canExecute: function() { return options.canEndEditing() && isEditing(); },
	                execute: function() { endEditing(); },
	                log_name: name + ".endEditingCommand",
	            });
	            result.revertEditsCommand = ko.command({
	                execute: function() {
	                    rollbackEdits();
	                    revertChangesCallback();
	                },
	                log_name: name + ".revertEditsCommand",
	            });
	            result.cancelEditsCommand = ko.command({
	                execute: function() {
	                    result.revertEditsCommand.execute();
	                    result.endEditingCommand.execute();
	                },
	                canExecute: function() { return result.hasChanges() || isEditing(); },
	                log_name: name + ".cancelEditsCommand",
	            });
	            result.saveEditsCommand = ko.command({
	                canExecute: function() { return options.canSave(); },
	                execute: function() {
	                    commitEdits();
	                    commitChangesCallback();
	                },
	                log_name: name + ".saveEditsCommand",
	            });
	            //#endregion

	            result.refreshState = cacheState;
	            result.defaultOptions = defaultOptions;

	            //#region init
	            if (options.initializeAsEditing) {
	                beginEditing();
	            }
	            cacheState();
	            //#endregion

	            //#region debug
	            if (ko.DEBUG) {
	                result.LOG = ko.observable(options.enableLogging);

	                if (result.LOG()) {
	                    cachedState.subscribe(function() {
	                        console.log(name + ' > cache value changed.');
	                        console.log({
	                            cache: cachedState(),
	                            currentHash: currentHash(),
	                            isEditing: isEditing(),
	                            isDirty: isDirty(),
	                        });
	                    });
	                    currentHash.subscribe(function() {
	                        console.log(name + ' > current hash value changed.');
	                        console.log({
	                            cache: cachedState(),
	                            currentHash: currentHash(),
	                            isEditing: isEditing(),
	                            isDirty: isDirty(),
	                        });
	                    });
	                }
	            }
	            //#endregion

	            result.dispose = function() {
	                objectToTrack(null);
	                objectToTrack = null;
	                cacheMapping = null;
	                customRevertFunctions = null;
	                ignore = null;
	                trackedObject = null;
	                revertUntrackedChanges = null;
	                hasUntrackedChanges = null;
	                commitUntrackedChanges = null;
	                revertChangesCallback = null;
	                commitChangesCallback = null;
	                beginEditingCallback = null;
	                endEditingCallback = null;
	                result = null;
	                isInitiallyDirty = null;
	                cachedState = null;
	                currentHash = null;
	                preventDirtyCheck = null;

	                isEditing = null;
	                isDirty = null;
	            }

	            return result;

	            //#region private functions
	            function beginEditing() {
	                if (isEditing() === true) return;
	                isEditing(true);
	            }

	            function endEditing() {
	                if (!isEditing()) return;
	                isEditing(false);
	                endEditingCallback();
	            }

	            function commitEdits() {
	                commitUntrackedChanges();
	                cacheState();
	                endEditing();
	                isInitiallyDirty(false);
	            }

	            function rollbackEdits() {
	                preventDirtyCheck(true);
	                var cache = deserializeCache();
	                recursiveRollback(trackedObject, cache);

	                revertUntrackedChanges();
	                cacheState();
	                preventDirtyCheck(false);

	                function recursiveRollback(current, original) {
	                    for (var propName in current) {
	                        var currentProp = current[propName];
	                        if (isEditStateManager(currentProp)) continue;

	                        var currentValue = ko.utils.unwrapObservable(currentProp);
	                        var originalValue = ko.utils.unwrapObservable(original[propName]);

	                        if (typeof currentValue === "function" || currentValue === originalValue) continue;

	                        setValue(originalValue);

	                        if (ko.DEBUG) {
	                            var newValue = ko.utils.unwrapObservable(currentProp);
	                            if (original.hasOwnProperty(propName) && newValue !== originalValue && options.customRevertFunctions[propName] == undefined) {
	                                console.warn('Revert failure:');
	                                console.debug({ message: 'Revert property \"' + propName + '\" failed', 'expected': originalValue, 'actual': newValue });
	                            }
	                        }
	                    }

	                    function isEditStateManager(prop) {
	                        return prop === result;
	                    }

	                    function setValue(value) {
	                        var revertFn = customRevertFunctions[propName] || defaultRevertFn;

	                        if (revertFn.length > 1) revertFn(value, current[propName]);
	                        else revertFn(value);
	                        
	                        function defaultRevertFn(val) {
	                            if (ko.isObservable(currentProp)) currentProp(val);
	                            else current[propName] = val;
	                        }
	                    }
	                }
	            }

	            function cacheState() {
	                cachedState(serialize(trackedObject));
	            }

	            function serialize(cacheObject) {
	                return ko.toJSON(cacheObject);
	            }

	            function buildTrackedObject() {
	                // Only observable properties are tracked by default. 
	                // This may  be  overridden by supplying an 'include' option, however, 
	                // the non-observable objects will not trigger changes to the tracked object.
	                trackedObject = {};

	                for (var prop in objectToTrack) {
	                    if (!isExcluded(prop, objectToTrack) && (isObservable(objectToTrack[prop]) || isIncluded(prop))) {
	                        trackedObject[prop] = objectToTrack[prop];
	                    }
	                }

	                return trackedObject;
	            }

	            function deserializeCache() {
	                var cache = ko.utils.parseJson(cachedState());
	                var hydrated = {};

	                for (var prop in cache) {
	                    hydrated[prop] = deserializeProperty(prop, cache);
	                }

	                return hydrated;
	            }

	            function deserializeProperty(propName, object) {
	                return typeof cacheMapping[propName] == "function"
	                    ? cacheMapping[propName].call(null, object[propName])
	                    : object[propName];
	            }

	            function isObservable(prop) {
	                return ko.isWriteableObservable
	                    ? ko.isWriteableObservable(prop)
	                    : (typeof prop == "function" && prop.name == "observable");
	            }

	            function isIncluded(propName) {
	                return ko.utils.arrayFirst(include, function(p) {
	                    return p === propName;
	                }) !== null;
	            }

	            function isExcluded(propName, obj) {
	                var prop = obj[propName];
	                return ko.utils.arrayFirst(ignore, function(p) {
	                    return typeof p === "string"
	                        ? p === propName
	                        : p === prop;
	                }) !== null;
	            }

	            function isArray(obj) {
	                return obj instanceof Array;
	                // Reece -- this wasn't working for me.
	                //return Object.prototype.toString(obj) === '[object Array]';
	            }
	            
	//#endregion
	        };

	        ko.EditStateManager.defaultOptions = (function() {
	            var defaultOptions = {
	                include: [],
	                ignore: ['__ko_mapping__'],
	                initializeAsEditing: false,
	                isInitiallyDirty: false,
	                canSave: function() { return true; },
	                name: "[unnamed_esm]",
	                canEdit: function() { return true; },
	                canEndEditing: function() { return true; },
	            };

	            return defaultOptions;
	        })();

	    }
	}());


/***/ }),
/* 57 */,
/* 58 */,
/* 59 */,
/* 60 */,
/* 61 */,
/* 62 */,
/* 63 */,
/* 64 */,
/* 65 */,
/* 66 */,
/* 67 */,
/* 68 */,
/* 69 */,
/* 70 */,
/* 71 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_FACTORY__, __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/*! Knockout projections plugin - version 1.1.0
	------------------------------------------------------------------------------
	Copyright (c) Microsoft Corporation
	All rights reserved.
	Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 
	THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT.
	See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.
	------------------------------------------------------------------------------
	*/
	!function (a) { "use strict"; function b(a, b, c, d, e, f, g) { this.inputItem = b, this.stateArrayIndex = c, this.mappingOptions = e, this.arrayOfState = f, this.outputObservableArray = g, this.outputArray = this.outputObservableArray.peek(), this.isIncluded = null, this.suppressNotification = !1, this.outputArrayIndex = a.observable(d), this.disposeFuncFromMostRecentMapping = null, this.mappedValueComputed = a.computed(this.mappingEvaluator, this), this.mappedValueComputed.subscribe(this.onMappingResultChanged, this), this.previousMappedValue = this.mappedValueComputed.peek() } function c(a, b) { if (!a) return null; switch (a.status) { case "added": return a.index; case "deleted": return a.index + b; default: throw new Error("Unknown diff status: " + a.status) } } function d(a, c, d, e, f, g, h, i, j) { var k = "number" == typeof c.moved, l = k ? d[c.moved] : new b(a, c.value, e, f, g, h, i); return h.splice(e, 0, l), l.isIncluded && j.splice(f, 0, l.mappedValueComputed.peek()), k && (l.stateArrayIndex = e, l.setOutputArrayIndexSilently(f)), l } function e(a, b, c, d, e) { var f = b.splice(c, 1)[0]; f.isIncluded && e.splice(d, 1), "number" != typeof a.moved && f.dispose() } function f(a, b, c) { return a.stateArrayIndex = b, a.setOutputArrayIndexSilently(c), c + (a.isIncluded ? 1 : 0) } function g(a, b) { for (var c = {}, d = 0; d < a.length; d++) { var e = a[d]; "added" === e.status && "number" == typeof e.moved && (c[e.moved] = b[e.moved]) } return c } function h(a, b, c) { return c.length && b[a.index] ? b[a.index].outputArrayIndex.peek() : c.length } function i(a, b, i, j, k, l) { return b.subscribe(function (b) { if (b.length) { for (var m = g(b, i), n = 0, o = b[0], p = 0, q = o && h(o, i, j), r = o.index; o || r < i.length; r++) if (c(o, p) === r) { switch (o.status) { case "added": var s = d(a, o, m, r, q, l, i, k, j); s.isIncluded && q++, p++; break; case "deleted": e(o, i, r, q, j), p--, r--; break; default: throw new Error("Unknown diff status: " + o.status) } n++, o = b[n] } else r < i.length && (q = f(i[r], r, q)); k.valueHasMutated() } }, null, "arrayChange") } function j(a, c) { var d = this, e = [], f = [], g = a.observableArray(f), h = d.peek(); if ("function" == typeof c && (c = { mapping: c }), c.mappingWithDisposeCallback) { if (c.mapping || c.disposeItem) throw new Error("'mappingWithDisposeCallback' cannot be used in conjunction with 'mapping' or 'disposeItem'.") } else if (!c.mapping) throw new Error("Specify either 'mapping' or 'mappingWithDisposeCallback'."); for (var j = 0; j < h.length; j++) { var k = h[j], l = new b(a, k, j, f.length, c, e, g), n = l.mappedValueComputed.peek(); e.push(l), l.isIncluded && f.push(n) } var o = i(a, d, e, f, g, c), p = a.computed(g).extend({ trackArrayChanges: !0 }), q = p.dispose; return p.dispose = function () { o.dispose(), a.utils.arrayForEach(e, function (a) { a.dispose() }), q.call(this, arguments) }, m(a, p), p } function k(a, b) { return j.call(this, a, function (a) { return b(a) ? a : p }) } function l(a) { function b(a, b) { return function () { return b.apply(this, [a].concat(Array.prototype.slice.call(arguments, 0))) } } a[q] = { map: b(a, j), filter: b(a, k) } } function m(a, b) { return a.utils.extend(b, a[q]), b } function n(a) { a.projections = { _exclusionMarker: p }, l(a), m(a, a.observableArray.fn) } function o() { if ("undefined" != typeof module && "undefined" != typeof module.exports) { var b = __webpack_require__(9); n(b), module.exports = b } else  true ? !(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9)], __WEBPACK_AMD_DEFINE_FACTORY__ = (n), __WEBPACK_AMD_DEFINE_RESULT__ = (typeof __WEBPACK_AMD_DEFINE_FACTORY__ === 'function' ? (__WEBPACK_AMD_DEFINE_FACTORY__.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__)) : __WEBPACK_AMD_DEFINE_FACTORY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__)) : "ko" in a && n(a.ko) } var p = {}; b.prototype.dispose = function () { this.mappedValueComputed.dispose(), this.disposeResultFromMostRecentEvaluation() }, b.prototype.disposeResultFromMostRecentEvaluation = function () { if (this.disposeFuncFromMostRecentMapping && (this.disposeFuncFromMostRecentMapping(), this.disposeFuncFromMostRecentMapping = null), this.mappingOptions.disposeItem) { var a = this.mappedValueComputed(); this.mappingOptions.disposeItem(a) } }, b.prototype.mappingEvaluator = function () { null !== this.isIncluded && this.disposeResultFromMostRecentEvaluation(); var a; if (this.mappingOptions.mapping) a = this.mappingOptions.mapping(this.inputItem, this.outputArrayIndex); else { if (!this.mappingOptions.mappingWithDisposeCallback) throw new Error("No mapping callback given."); var b = this.mappingOptions.mappingWithDisposeCallback(this.inputItem, this.outputArrayIndex); if (!("mappedValue" in b)) throw new Error("Return value from mappingWithDisposeCallback should have a 'mappedItem' property."); a = b.mappedValue, this.disposeFuncFromMostRecentMapping = b.dispose } var c = a !== p; return this.isIncluded !== c && (null !== this.isIncluded && this.moveSubsequentItemsBecauseInclusionStateChanged(c), this.isIncluded = c), a }, b.prototype.onMappingResultChanged = function (a) { a !== this.previousMappedValue && (this.isIncluded && this.outputArray.splice(this.outputArrayIndex.peek(), 1, a), this.suppressNotification || this.outputObservableArray.valueHasMutated(), this.previousMappedValue = a) }, b.prototype.moveSubsequentItemsBecauseInclusionStateChanged = function (a) { var b, c, d = this.outputArrayIndex.peek(); if (a) for (this.outputArray.splice(d, 0, null), b = this.stateArrayIndex + 1; b < this.arrayOfState.length; b++) c = this.arrayOfState[b], c.setOutputArrayIndexSilently(c.outputArrayIndex.peek() + 1); else for (this.outputArray.splice(d, 1), b = this.stateArrayIndex + 1; b < this.arrayOfState.length; b++) c = this.arrayOfState[b], c.setOutputArrayIndexSilently(c.outputArrayIndex.peek() - 1) }, b.prototype.setOutputArrayIndexSilently = function (a) { this.suppressNotification = !0, this.outputArrayIndex(a), this.suppressNotification = !1 }; var q = "_ko.projections.cache"; o() }(this);

/***/ }),
/* 72 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(7), __webpack_require__(8)], __WEBPACK_AMD_DEFINE_RESULT__ = function (core, rvc) {
	  var attributeOrder = {
	    'AstaC': 0,
	    'Asta': 1,
	    'H2O': 2,
	    'Scan': 3,
	    'AB': 4,
	    'Gran': 5,
	    'Scov': 6,
	    'TPC': 7,
	    'Yeast': 8,
	    'Mold': 9,
	    'ColiF': 10,
	    'EColi': 11,
	    'Sal': 12,
	    'Rod Hrs': 13,
	    'InsP': 14,
	    'Ash': 15,
	    'AIA': 16,
	    'Ethox': 17,
	    'BI': 18,
	    'Lead': 19,
	    'AToxin': 20,
	    'Gluten': 21
	  };

	  function sortAttributesLists(values) {
	    var data = values || {};
	    rvc.helpers.forEachInventoryType(sortAttributesForType);
	    return data;

	    function sortAttributesForType(type) {
	      var attrs = data[type.key] || [];
	      if (type.key === rvc.lists.inventoryTypes.Chile.key) {
	        attrs.push({ Key: 'AstaC', Value: 'Asta Calc' });
	      }

	      var attrOrder = attributeOrder || {};
	      var unorderedIndex = Object.keys(attrOrder).length + 1;
	      ko.utils.arrayMap(attrs, function (attr) {
	        var order = attrOrder[attr.Key];
	        var index = order === undefined ? unorderedIndex++ : order;
	        attr.__index = index;
	      });

	      data[type.key] = attrs.sort(function (a, b) {
	        return a.__index === b.__index ?
	          0 :
	          a.__index < b.__index ?
	            -1 :
	            1;
	      });
	    }
	  }

	  function sortAttributes( attrs ) {
	    var sortedAttributes = [];
	    var attrOrder = attributeOrder || {};
	    var unorderedIndex = Object.keys(attrOrder).length + 1;

	    ko.utils.arrayMap( attrs, function (attr) {
	      var order = attrOrder[attr.Key];
	      var index = order === undefined ?
	        unorderedIndex++ :
	        order;

	      attr.__index = index;
	    } );

	    sortedAttributes = attrs.sort(function (a, b) {
	      return a.__index === b.__index ?
	        0 :
	        a.__index < b.__index ?
	          -1 :
	          1;
	    });

	    return sortedAttributes;
	  }

	  return {
	    buildLotPager: function( options ) {
	      options = options || {};
	      return core.pagedDataHelper.init({
	        urlBase: "/api/lots",
	        pageSize: options.pageSize || 50,
	        parameters: options.parameters,
	        resultCounter: function (data) {
	          return data.LotSummaries.length;
	        },
	        onNewPageSet: options.onNewPageSet
	      });
	    },
	    compositeLots: function( data ) {
	      return core.ajaxPut( '/api/lots/addattributes/', data );
	    },
	    sortAttributes: sortAttributes,
	    buildLotUrl: function (key) {
	      return ["/api/Lots/", key || ''].join('');
	    },
	    getAttributeNames: function () {
	      var $dfd = $.Deferred();

	      core.ajax('/api/attributeNames')
	      .done(function (data) {
	        try {
	          $dfd.resolve(sortAttributesLists(data));
	        } catch (e) {
	          $dfd.reject();
	        }
	      })
	      .fail($dfd.reject);

	      // support compatability with the core.ajax return object
	      $dfd.error = $dfd.fail;

	      return $dfd;
	    },
	    getLotData: function( lotKey ) {
	      return core.ajax( ''.concat( '/api/lots/' + lotKey.replace(/ /g, '') ) );
	    },
	    getLotHistory: function( lotKey ) {
	      return core.ajax( '/api/lots/' + lotKey + '/history' );
	    },
	    getIngredientsByProductType: function () {
	      return core.ajax('/api/ingredients');
	    },
	    getProductsByLotType: function (lotType, options) {
	      var inventoryType = rvc.lists.lotTypes.findByKey(lotType).inventoryType.key;
	      var url = ['/api/products/', inventoryType, '?lotType=', lotType].join('');

	      if (options && options.filterProductsWithInventory) {
	        url = url.concat("&filterProductsWithInventory=true");
	      }

	      return core.ajax(url, options);
	    },
	    getLotsByKey: function( lotKey ) {
	      return core.ajax( ''.concat( '/api/lots?startingLotKey=', lotKey ) );
	    },
	    setLotStatus: function( lotKey, status ) {
	      return core.ajaxPut( ''.concat( '/api/lots/', lotKey, '/qualityStatus' ), status );
	    },
	    removeLotHold: function (lotKey, optionsCallback) {
	      return core.ajaxPut("/api/lots/" + lotKey + "/holds", null, optionsCallback);
	    },
	    setLotHold: function (lotKey, data, optionsCallback) {
	      return core.ajaxPut("/api/lots/" + lotKey + "/holds", data, optionsCallback);
	    },
	    getInputMaterialsDetails: function (key) {
	      if (!key) { return; }
	      return core.ajax(this.buildLotUrl(key) + '/input');
	    },
	    getTransactionsDetails: function (key) {
	      if (!key) { return; }
	      return core.ajax(this.buildLotUrl(key) + '/inventory/transactions');
	    },
	    saveLabResult: function( lotKey, data ) {
	      return core.ajaxPut( ('/api/lots/' + lotKey), data );
	    },
	    createAllowance: function( lotKey, type, key ) {
	      return core.ajaxPut( '/api/lots/' + lotKey.replace( /\s+/g, '' ) + '/allowances/' + type + '/' + key );
	    },
	    deleteAllowance: function( lotKey, type, key ) {
	      return core.ajaxDelete( '/api/lots/' + lotKey.replace( /\s+/g, '' ) + '/allowances/' + type + '/' + key );
	    },
	    getLotInputTrace: function( lotKey ) {
	      if ( lotKey == null ) { throw new Error('Lot trace requires a key'); }

	      return core.ajax( '/api/lots/' + lotKey + '/input-trace' );
	    },
	    getLotOutputTrace: function( lotKey ) {
	      if ( lotKey == null ) { throw new Error('Lot trace requires a key'); }

	      return core.ajax( '/api/lots/' + lotKey + '/output-trace' );
	    },
	  };
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 73 */,
/* 74 */,
/* 75 */,
/* 76 */,
/* 77 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(7), __webpack_require__(8)], __WEBPACK_AMD_DEFINE_RESULT__ = function (core, app) {
	    var warehouseLocationsService = __webpack_require__(11);

	    var buildUrl = core.buildUrl,
	        ajax = core.ajax,
	        pagedDataHelper = core.pagedDataHelper;

	    return {
	        getInventoryPager: getInventoryPager,
	        getInventoryPagerWithTotals: getInventoryPagerWithTotals,
	        getPickableInventoryPager: getPickableInventoryPager,
	        getPickableInventory: getPickableInventory,
	        getInventory: getInventory,
	        getInventoryByLot: getInventoryByLot,
	        getInventoryByLotAndWarehouse: getInventoryByLotAndWarehouse,
	        getInventoryByTote: getInventoryByTote,
	        getChileProductsDataPager: getChileProductsDataPager,
	        getChileMaterials: getChileMaterials,
	        createChileProductReceivedRecord: createChileProductReceivedRecord,
	        updateChileProductReceivedRecord: updateChileProductReceivedRecord,
	        getDehydratedMaterials: getDehydratedMaterials,
	        getDehydratedMaterialsDataPager: getDehydratedMaterialsDataPager,
	        getDehydratedMaterialsReceived: getDehydratedMaterialsReceived,
	        getDehydrators: getDehydrators,
	        getMillAndWetdownPager: getMillAndWetdownPager,
	        getMillAndWetdownDetails: getMillAndWetdownDetails,
	        createMillAndWetdownEntry: createMillAndWetdownEntry,
	        updateMillAndWetdownEntry: updateMillAndWetdownEntry,
	        deleteMillAndWetdownEntry: deleteMillAndWetdownEntry,
	        getRinconWarehouseLocations: warehouseLocationsService.getRinconWarehouseLocations,
	        getWarehouseLocations: warehouseLocationsService.getWarehouseLocations,
	        receiveInventory: receiveInventory,
	        saveDehydratedMaterials: saveDehydratedMaterials,
	        updateDehydratedMaterials: updateDehydratedMaterials,
	        savePickedInventory: savePickedInventoryPublic,
	    };

	    //#region exports
	    function getInventoryPager(options) {
	        options = options || {};

	        return pagedDataHelper.init({
	            urlBase: options.baseUrl || "/api/inventory",
	            pageSize: options.pageSize || 50,
	            parameters: options.parameters,
	            onNewPageSet: options.onNewPageSet,
	            onEndOfResults: options.onEndOfResults
	        });
	    }
	    function getInventoryPagerWithTotals(options) {
	      options = options || {};

	      return pagedDataHelper.init({
	        urlBase: options.baseUrl || "/api/inventorytotals",
	        pageSize: options.pageSize || 50,
	        parameters: options.parameters,
	        onNewPageSet: options.onNewPageSet,
	        onEndOfResults: options.onEndOfResults,
	        resultCounter: function (data) {
	          data = data || {};
	          data.Items = data.Items || [];
	          return data.Items.length || 0;
	        }
	      });
	    }
	    function getPickableInventoryPager(pickingContext, contextKey, options) {
	        options = options || {};
	        if (!options.pageSize) options.pageSize = 100;
	        options.urlBase = buildInventoryPickingUrl(pickingContext, contextKey);
	        return pagedDataHelper.init(options);
	    }
	    function getPickableInventory(pickingContext, contextKey, params) {
	        var qs = $.param(params);
	        return core.ajax(buildInventoryPickingUrl(pickingContext, contextKey) + '?' + qs);
	    }
	    function savePickedInventoryPublic(pickingContext, contextKey, values) {
	        return core.ajaxPost(buildInventoryPickingUrl(pickingContext, contextKey), values);
	    }
	    function buildInventoryPickingUrl(pickingContext, contextKey) {
	        return '/api/inventory/pick-' + pickingContext.value.toLowerCase() + '/' + contextKey;
	    }
	    function getInventory(/* productType, lotType, productSubType, productKey, warehouseKey */) {
	        var args = [];
	        for (var a in arguments) {
	            args.push(arguments[a]);
	        }
	        var options = args.pop();
	        return ajax(buildUrl(buildInventoryUrl, args), options);
	    }
	    function getInventoryByLot(lotNumber, options) {
	        return ajax(buildUrl(buildInventoryUrl, lotNumber), options);
	    }
	    function getInventoryByLotAndWarehouse(lotNumber, warehouseKey, options) {
	        return ajax(buildUrl(buildLotInventoryByWarehouseUrl, lotNumber, warehouseKey), options);
	    }
	    function getInventoryByTote(toteKey, options) {
	        return ajax("/api/toteinventory/" + toteKey, options);
	    }
	    function getMillAndWetdownPager(options) {
	      options = options || {};

	      return pagedDataHelper.init({
	        urlBase: options.baseUrl || "/api/millwetdown",
	        pageSize: options.pageSize || 50,
	        parameters: options.parameters,
	        onNewPageSet: options.onNewPageSet,
	      });
	    }
	    function getMillAndWetdownDetails(key) {
	      return ajax('/api/millwetdown/' + (key || ''));
	    }
	    function createMillAndWetdownEntry(entryData) {
	      return core.ajaxPost("/api/millwetdown/", entryData);
	    }
	    function updateMillAndWetdownEntry(lotNumber, entryData) {
	      return core.ajaxPut("/api/millwetdown/" + lotNumber, entryData);
	    }
	    function deleteMillAndWetdownEntry(lotNumber, entryData) {
	      return core.ajaxDelete( "/api/millwetdown/" + lotNumber );
	    }
	    function getDehydrators() {
	      var dfd = $.Deferred();

	      ajax('/api/companies').done(function(data, textStatus, jqXHR) {
	        var dehydrators = ko.utils.arrayFilter(data, function(company) {
	          if (company.CompanyTypes.indexOf(app.lists.companyTypes.Dehydrator.key) >= 0) {
	            return true;
	          }
	        });

	        dfd.resolve(dehydrators);
	      })
	      .fail(function(jqXHR, textStatus, errorThrown) {
	      });

	      return dfd;
	    }
	    function getChileProductsDataPager(options) {
	        options = options || {};

	        return pagedDataHelper.init({
	            urlBase: options.baseUrl || "/api/chilereceived",
	            pageSize: options.pageSize || 50,
	            parameters: options.parameters,
	            onNewPageSet: options.onNewPageSet,
	        });
	    }
	    function getDehydratedMaterialsDataPager(options) {
	        options = options || {};

	        return pagedDataHelper.init({
	            urlBase: options.baseUrl || "/api/dehydratedmaterialsreceived",
	            pageSize: options.pageSize || 50,
	            parameters: options.parameters,
	            onNewPageSet: options.onNewPageSet,
	        });
	    }
	    function getChileMaterials( lotKey ) {
	      return core.ajax( '/api/chilereceived/' + lotKey );
	    }
	    function createChileProductReceivedRecord( productData ) {
	      return core.ajaxPost( '/api/chilereceived', productData );
	    }
	    function updateChileProductReceivedRecord( productKey, productData ) {
	      return core.ajaxPut( '/api/chilereceived/' + productKey, productData );
	    }
	    function getDehydratedMaterials(lot) {
	      return ajax('/api/dehydratedmaterialsreceived/' + (lot || ''));
	    }
	    function getDehydratedMaterialsReceived() { /* optional parameters: startDate, endDate */
	        var args = [];
	        for (var a in arguments) {
	            args.push(arguments[a]);
	        }
	        var options = args.pop();
	        var url = "/api/dehydratedmaterialsreceived";
	        if (args.length > 0) {
	            url += buildQueryString(args[0]);
	        }

	        ajax(url, options);
	    }
	    function saveDehydratedMaterials(data) {
	      return core.ajaxPost('/api/dehydratedmaterialsreceived', data);
	    }
	    function updateDehydratedMaterials(key, data) {
	      return core.ajaxPut('/api/dehydratedmaterialsreceived/' + key, data);
	    }
	    function receiveInventory(values) {
	        var url = buildLotInventoryUrl();
	        return core.ajaxPost(url, values);
	    }
	    //#endregion exports

	    function buildLotInventoryUrl(lotNumber) {
	        return "/api/inventory/" + lotNumber;
	    }
	    function buildLotInventoryByWarehouseUrl(lotNumber, warehouseKey) {
	        return buildLotInventoryUrl(lotNumber) + "?warehouseKey=" + warehouseKey;
	    }
	    function buildInventoryUrl(productType) {
	        return "/api/inventory/" + (productType || 2);
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 78 */,
/* 79 */,
/* 80 */,
/* 81 */,
/* 82 */,
/* 83 */,
/* 84 */,
/* 85 */,
/* 86 */,
/* 87 */,
/* 88 */,
/* 89 */,
/* 90 */,
/* 91 */
/***/ (function(module, exports, __webpack_require__) {

	function LoadScreenViewModel(params) {
	    var self = this;

	    self.isVisible = params.isVisible;
	    self.loadMessage = params.displayMessage;
	}

	// Webpack
	module.exports = {
	    viewModel: LoadScreenViewModel,
	    template: __webpack_require__(92)
	};



/***/ }),
/* 92 */
/***/ (function(module, exports) {

	module.exports = "<section class=\"modal-message load-message\" data-bind=\"loadingMessage: isVisible\">\r\n  <span>\r\n    <i class=\"fa fa-spinner fa-2x fa-pulse\"></i><br>\r\n    <!-- ko text: loadMessage --><!-- /ko -->\r\n  </span>\r\n</section>\r\n"

/***/ }),
/* 93 */,
/* 94 */,
/* 95 */,
/* 96 */,
/* 97 */,
/* 98 */,
/* 99 */,
/* 100 */,
/* 101 */,
/* 102 */,
/* 103 */,
/* 104 */,
/* 105 */,
/* 106 */,
/* 107 */,
/* 108 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {var productsService = __webpack_require__(24);

	ko.punches.enableAll();

	var rvc = __webpack_require__(8),
	    lotService = __webpack_require__(72),
	    inventoryService = __webpack_require__(77),
	    pickableInventoryFactory = __webpack_require__(109),
	    loadingMessage = __webpack_require__(91),
	    attributes = ko.observableArray([]),
	    LotAttribute = __webpack_require__(114);

	var initAttributes = $.when(loadAttributeNames());

	var inventoryPickingTableComponent = __webpack_require__(115);
	// must be synchronous for sticky table headers to work.
	inventoryPickingTableComponent.synchronous = true;
	ko.components.register('inventory-picking-table', inventoryPickingTableComponent);

	__webpack_require__(36);
	__webpack_require__(18);
	__webpack_require__(33);

	ko.punches.enableAll();

	/**  Inputs:
	  *
	  * @param {Object} data - Container for input data
	  * @param {Object} data.pickingContext
	  * @param {string} data.pickingContextKey
	  * @param {Object[]} data.pickedInventoryItems
	  * @param {number} data.pageSize
	  * @param {Object} data.filters
	  * @param {Object} data.targetProduct
	  * @param {Object} data.targetWeight
	  * @param {string} data.customerKey
	  * @param {string} data.customerLotCode
	  * @param {string} data.customerProductCode
	  * @param {string} data.args - optional object or observable to include additional properties to be included in the query parameters (for example, picking for sales order requires the inclusion of a contractItemKey)
	  * @param {Function} [dataPager] - Optional pager to call instead of default
	  * @param {function} [checkOutOfRange]
	  * @param {boolean} [hideTheoretical=false] - Disables the calulcation and display of theoretical attrs
	  * @param {Object} exports - Observable, container for exported methods and properties
	  */

	function InventoryPickerViewModel(params) {
	  if (!(this instanceof InventoryPickerViewModel)) { return new InventoryPickerViewModel(params); }

	  var self = this;
	  var input = params.hasOwnProperty('data') ? params.data : params;
	  var data = ko.unwrap(input);

	  self.floatHeader = function () {
	    var $tableSelector = $(arguments[0]).find('table');
	    $tableSelector.floatThead({
	      scrollContainer: function ($table) {
	        return $table.closest('.sticky-head-container');
	      }
	    });
	  };

	  self.reflowTable = function () {
	    var $table = $('table.sticky-head');
	    $table.floatThead('reflow');
	  };

	  self.disposables = [];

	  var targetProduct = data.targetProduct;
	  self.targetProduct = targetProduct;
	  self.targetWeight = data.targetWeight;

	  self.hideTheoretical = params.hideTheoretical;

	  self.isInit = ko.observable(false);
	  self.isLoaded = ko.observable(false);
	  self.isWorking = ko.observable(false);
	  self.isSaving = ko.observable(false);

	  self.loadingMessage = ko.observable('');

	  self.pickingContext = data.pickingContext;
	  self.pickingContextKey = data.pickingContextKey;
	  self.otherArgs = data.args || {};
	  self.customerKey = data.customerKey;
	  self.customerLotCode = data.customerLotCode;
	  self.customerProductCode = data.customerProductCode;

	  self.targetProductName = ko.computed(function () {
	    var product = ko.unwrap(targetProduct) || {};
	    return product.ProductNameFull;
	  });
	  self.targetProductKey = ko.computed(function () {
	    var product = ko.unwrap(targetProduct) || {};
	    return product.ProductKey;
	  });

	  self.customerSpecs = ko.observable({});
	  self.isCustomerSpecAvailable = ko.observable( false );
	  function getCustomerProductSpecs( customerKey, productKey ) {
	    self.customerSpecs({});
	    var _targetProduct = ko.unwrap( targetProduct );
	    var _productSpecs;
	    if ( _targetProduct ) {
	      _productSpecs = {};

	      ko.utils.arrayForEach( _targetProduct.AttributeRanges, function( attrRange ) {
	        _productSpecs[ attrRange.AttributeNameKey ] = attrRange;
	      });
	    }

	    return productsService.getCustomerProductDetails( customerKey, productKey ).then(
	    function( data ) {
	      if ( data.length === 0 ) { return; }

	      var specs = data.map(function( spec ) {
	        spec.MinValue = spec.RangeMin;
	        spec.MaxValue = spec.RangeMax;
	        spec.AttributeNameKey = spec.AttributeShortName;
	        spec.overridden = true;

	        return spec;
	      });

	      var mappedSpecs = {};
	      ko.utils.arrayForEach( specs, function( spec ) {
	        mappedSpecs[ spec.AttributeNameKey ] = spec;
	      });

	      if ( _productSpecs ) {
	        var mergedSpecs = $.extend({}, _productSpecs, mappedSpecs );

	        self.customerSpecs( mergedSpecs );
	      } else {
	        self.customerSpecs( mappedSpecs );
	      }

	      self.isCustomerSpecAvailable( true );
	    },
	    function( jqXHR, textStatus, errorThrown ) {
	      if ( jqXHR.status === 500 ) {
	        showUserMessage( 'Could not get customer specs', {
	          description: errorThrown
	        });
	      }
	    });
	  }

	  self.isUsingCustomerSpec = ko.observable( false );

	  if ( self.customerKey ) {
	    getCustomerProductSpecs( ko.unwrap( self.customerKey ), self.targetProductKey() ).then(
	    function( data, textStatus, jqXHR ) {
	      self.isUsingCustomerSpec( true );
	    });
	  }
	  function checkCustomerOutOfRange( key, value ) {
	    var _customerSpecs = self.customerSpecs();

	    var spec = _customerSpecs[ key ];

	    if ( spec ) {
	      if ( value < spec.MinValue ) {
	        return -1;
	      } else if ( value > spec.MaxValue ) {
	        return 1;
	      }
	    }

	    return 0;
	  }

	  self.checkOutOfRange = ko.pureComputed(function() {
	    return self.isCustomerSpecAvailable() && self.isUsingCustomerSpec() ?
	      checkCustomerOutOfRange :
	      params.checkOutOfRange;
	  });


	  self.inventoryItems = ko.observableArray();
	  var pickedInventoryItems = ko.computed(function() {
	    return ko.utils.arrayFilter(self.inventoryItems(), function(i) {
	      return i.isPicked() === true;
	    });
	  });

	  self.isDirty = ko.pureComputed(function () {
	    var picks = pickedInventoryItems();

	    return ko.utils.arrayFirst(picks, function (item) {
	      return ((item.isInitiallyPicked() &&
	              item.isChanged()) ||
	          (!item.isInitiallyPicked() &&
	              item.isPicked() &&
	              item.QuantityPicked() > 0));
	    }) ? true : false;
	  });
	  self.currentInventoryType = ko.pureComputed(function () {
	    var _input = ko.unwrap( input );
	    var filters = ko.unwrap(_input.filters);
	    return (filters && ko.unwrap(filters.inventoryType)) || rvc.lists.inventoryTypes.Chile.key;
	  });

	  self.showTable = function (table) {
	    var tableType = table.inventoryType.key,
	        targetInventoryType = self.currentInventoryType();

	    if (tableType === targetInventoryType || targetInventoryType == undefined) {
	      self.reflowTable();
	      return true;
	    }
	    return false;
	  };

	  // commands
	  self.saveCommand = ko.asyncCommand({
	    execute: function (complete) {
	      self.loadingMessage('Saving picked items');
	      self.isSaving(true);

	      try {
	        var context = ko.unwrap(self.pickingContext),
	            key = ko.unwrap(self.pickingContextKey),
	            items = pickedInventoryItems(),
	            values = [],
	            args = ko.unwrap(self.otherArgs) || {};

	        // Loops over inventory for picked items
	        for (var i = 0, max = items.length; i < max; i += 1) {
	          var item = items[i];
	          if (!item.validation.isValid()) {
	            showUserMessage('Failed to save items', { description: 'Please enter a valid quantity for all picked items' });
	            complete();
	            return;
	          }

	          var orderItemKey = args.orderItemKey || args.OrderItemKey;
	          var isNewPick = item.OrderItemKey == null && orderItemKey != null;
	          var inScopeForPicking = isNewPick || (args.orderItemKey != null && item.OrderItemKey === args.orderItemKey);

	          if (item.QuantityPicked() > 0 && item.validation.isValid()) {
	            values.push(ko.toJS({
	              InventoryKey: item.InventoryKey,
	              QuantityPicked: item.QuantityPicked,
	              CustomerLotCode: inScopeForPicking ? ko.unwrap( self.customerLotCode ) : item.CustomerLotCode,
	              CustomerProductCode: inScopeForPicking ? ko.unwrap( self.customerProductCode ) : item.CustomerProductCode,
	              OrderItemKey: item.OrderItemKey || orderItemKey || null,
	            }));
	          } else {
	            item.QuantityPicked(null);
	          }
	        }
	      } catch (ex) {
	        self.isSaving(false);
	        complete();
	        showUserMessage("An error occurred while attempting to save inventory. Please contact system administrator.", { description: 'Error description: ' + ex.message });
	      }


	      return inventoryService.savePickedInventory(context, key, values)
	          .done(function () {
	            ko.utils.arrayForEach(pickedInventoryItems(), function (i) {
	              i.isInitiallyPicked(i.QuantityPicked() > 0);
	              i.commit();
	            });

	            ko.postbox.publish('PickedItemsSaved', pickedInventoryItems());
	            self.reflowTable();
	            showUserMessage('Save successful', { description: 'Products have been successfully picked' });
	          })
	          .fail(function (promise, status, message) {
	            showUserMessage('Failed to save items', { description: 'Server gave error: \n' + message });
	          })
	          .always(function () {
	            complete();
	            self.isSaving(false);
	          });
	    },
	    canExecute: function (isExecuting) {
	      return !isExecuting;
	    }
	  });
	  self.revertCommand = ko.command({
	    execute: function () {
	      ko.utils.arrayForEach(pickedInventoryItems(), function (item) { item.revert(); });
	      return true;
	    },
	    canExecute: function () {
	      return true;
	    }
	  });

	  var inventoryCache = {};

	  // Behaviors
	  function mapLotInventoryItemAsPickable(value) {
	    return pickableInventoryFactory( value, self.checkOutOfRange );
	  }
	  function initializeInventoryCache() {
	    var cache = {};
	    ko.utils.arrayForEach(self.inventoryItems.peek(), function (item) {
	      var cacheKey = ko.unwrap(item.Product.ProductType);
	      if (!cache[cacheKey]) { cache[cacheKey] = {}; }
	      cache[cacheKey][item.InventoryKey] = item;
	    });
	    inventoryCache = cache;
	  }

	  function buildDataPager(context, key, otherArgs) {
	    var _input = ko.unwrap( input );
	    var pagerOptions = {
	      pageSize: _input.pageSize ? _input.pageSize : 50,
	      parameters: $.extend({}, ko.unwrap(_input.filters), otherArgs || {}),
	      onNewPageSet: function resetPickableArray() {
	        self.inventoryItems(pickedInventoryItems());
	        initializeInventoryCache();
	      },
	      onEndOfResults: function () {
	        showUserMessage("All Inventory Loaded", { description: 'All inventory is loaded for the current filters. To view more inventory, change the filter selections on the right side of the page.' });
	      }
	    };

	    return context && key ?
	      inventoryService.getPickableInventoryPager(context, key, pagerOptions) :
	      null;
	  }

	  var lotDataPager, contextHold, keyHold;
	  function loadMoreItems() {
	    var dfd = $.Deferred();
	    self.isWorking(true);
	    self.loadingMessage('Loading inventory...');

	    var context = ko.unwrap(self.pickingContext),
	        key = ko.unwrap(self.pickingContextKey);
	    args = ko.unwrap(self.otherArgs);

	    if (!lotDataPager || context !== contextHold || key !== keyHold) {
	      lotDataPager = buildDataPager(context, key, args);
	      contextHold = context;
	      keyHold = key;
	    }

	    if (!lotDataPager) {
	      console.log('Data pager is missing required parameters.');
	      dfd.reject();
	      return dfd;
	    }

	    lotDataPager.GetNextPage().done(function (values) {
	      // Builds inventory item structure
	      var cache = inventoryCache[self.currentInventoryType()];
	      cache = cache || {};
	      var mappedItems = [],
	          cachedItem;

	      ko.utils.arrayForEach(values, function (item) {
	        cachedItem = cache[item.InventoryKey];
	        if (cachedItem) return; //prevent duplicates

	        cachedItem = mapLotInventoryItemAsPickable(item);
	        //build cache (assumes all items are of the same inventory type)
	        cache[item.InventoryKey] = cachedItem;
	        mappedItems.push(cachedItem);
	      });

	      // Pushes data to master inventory list
	      ko.utils.arrayPushAll(self.inventoryItems(), mappedItems);
	      self.inventoryItems.valueHasMutated();
	      self.isWorking(false);
	      self.isLoaded(true);

	      dfd.resolve();
	    })
	    .fail(function () {
	      showUserMessage("Error loading inventory items.", { description: arguments[2] });
	      dfd.reject();
	      self.isWorking(false);
	    });

	    return dfd;
	  }

	  function mapInitiallyPickedItems(pickedItems) {
	    var items = ko.unwrap(pickedItems) || [];
	    var mappedItems = ko.utils.arrayMap(items, mapLotInventoryItemAsPickable);

	    for (var i = 0, max = mappedItems.length; i < max; i += 1) {
	      if (!mappedItems[i].isInitiallyPicked) {
	        mappedItems[i].isInitiallyPicked = ko.observable(true);
	      } else {
	        mappedItems[i].isInitiallyPicked(true);
	      }
	    }
	    return mappedItems;
	  }

	  function consolidateDuplicates(items) {
	    var cache = {},
	        hasError = false;

	    var output = ko.utils.arrayFilter(items, function (item) {
	      return !checkCache(item);
	    });

	    if (hasError) {
	      showUserMessage('Consolidation error', { description: 'Attempted to consolidate multiple entries of same lot but quantity exceeded available product.' });
	    }

	    return output;

	    function checkCache(item) {
	      var cacheKey = item.InventoryKey,
	          cacheItem = cache[cacheKey],
	          qtyAvailable,
	          diff;

	      if (cacheItem) {
	        qtyAvailable = cacheItem.QuantityAvailable.peek();

	        diff = qtyAvailable - item.QuantityPicked.peek();
	        if (diff < 0) {
	          hasError = true;
	        }

	        cacheItem.QuantityPicked(cacheItem.QuantityPicked.peek() + item.QuantityPicked.peek());
	        return true;
	      } else {
	        cache[cacheKey] = item;
	        return false;
	      }
	    }
	  }
	  function setInitiallyPickedInventory() {
	    var pickedItems = ko.unwrap(data.pickedInventoryItems) || [];

	    ko.utils.arrayForEach(pickedItems, function (item) {
	      var isAstaC = ko.utils.arrayFirst(item.Attributes, function (attr) {
	        return attr.Key === "AstaC";
	      });

	      if (!isAstaC && item.AstaCalc) {
	        item.Attributes.push( new LotAttribute({
	          AttributeDate: item.Attributes[0].AttributeDate,
	          Computed: false,
	          Defect: undefined,
	          Key: "AstaC",
	          Name: "AstaC",
	          Value: item.AstaCalc
	        }) );
	      }
	    });

	    if (!(pickedItems.length === 0 && self.inventoryItems.peek().length === 0)) {
	      var consolidatedItems = consolidateDuplicates(mapInitiallyPickedItems(pickedItems));
	      self.inventoryItems(consolidatedItems);
	    }

	    initializeInventoryCache();
	  }

	  if (ko.isObservable(data.pickedInventoryItems)) {
	    var updateOnInputChange = data.pickedInventoryItems.subscribe(function (data) {
	      setInitiallyPickedInventory();
	    });

	    self.disposables.push(updateOnInputChange);
	  }
	  setInitiallyPickedInventory();

	  self.loadInventoryItemsCommand = ko.asyncCommand({
	    execute: function (complete) {
	      return loadMoreItems().always(complete);
	    },
	    canExecute: function (isExecuting) {
	      return !isExecuting && self.isInit();
	    }
	  });

	  initAttributes.then(function () {
	    self.buildInventoryTables(targetProduct)
	        .then(function () {
	          self.isInit(true);
	        });
	  });

	  self.errors = ko.pureComputed(function () {
	    var errorArray = [];

	    ko.utils.arrayForEach(self.inventoryItems(), function (item) {
	      var productType = item.LotType.inventoryType.value;
	      var isValid = item.validation.isValid();

	      if (!isValid && errorArray.indexOf(productType) === -1) {
	        errorArray.push(productType);
	      }
	    });

	    return errorArray;
	  });

	  self.exports = ko.validatedObservable({
	    saveCommand: self.saveCommand,
	    revertCommand: self.revertCommand,
	    isInit: self.isInit,
	    isDirty: self.isDirty,
	    isWorking: self.isWorking,
	    isSaving: self.isSaving,
	    loadInventoryItemsCommand: self.loadInventoryItemsCommand,
	    pickedItems: self.inventoryItems
	  });
	  params.exports(self.exports);
	}

	InventoryPickerViewModel.prototype.buildInventoryTables = function (targetProduct) {
	  var dfd = $.Deferred(),
	      self = this;

	  self.pickingTableViewModels = ko.observableArray([]);
	  self.specWarnings = ko.observableArray([]);

	  var tableVMs = [],
	    exportedInventoryPickingTables = [];

	  rvc.helpers.forEachInventoryType(function (inventoryType) {
	    exportedInventoryPickingTables.push(ko.observable());
	    tableVMs.push({
	      inventoryType: inventoryType,
	      inventoryItems: self.inventoryItems,
	      targetProduct: targetProduct,
	      useCustomerSpec: self.isUsingCustomerSpec,
	      customerSpecs: self.customerSpecs,
	      targetWeight: self.targetWeight,
	      orderItemKey: ko.unwrap( self.otherArgs ).orderItemKey || null,
	      attributes: attributes()[inventoryType.key] || [],
	      exports: exportedInventoryPickingTables[exportedInventoryPickingTables.length - 1]
	    });
	  });

	  ko.utils.arrayPushAll(self.pickingTableViewModels(), tableVMs);
	  self.pickingTableViewModels.valueHasMutated();

	  self.specWarnings = ko.pureComputed(function() {
	    var warnings = [];
	    ko.utils.arrayForEach(exportedInventoryPickingTables, function(pickTableExports) {
	      var pickTable = pickTableExports();
	      if (pickTable == undefined) return;
	      var oos = pickTable.attributesOutOfSpec();
	      if (oos.length) ko.utils.arrayPushAll(warnings, oos);
	    });

	    return warnings;
	  });

	  dfd.resolve();
	  return dfd;
	};

	InventoryPickerViewModel.prototype.events = {
	  pickedItemsSaved: 'PickedInventorySaved'
	};

	function selfOrObservable(candidate) {
	  return ko.isObservable(candidate) ? candidate : ko.observable(candidate);
	}
	function selfOrObservableArray(candidate) {
	  return ko.isObservable(candidate) ? candidate : ko.observableArray(candidate);
	}

	function loadAttributeNames() {
	  if (initAttributes) {
	    return initAttributes;
	  }

	  this.loadAttributeNamesAttempts = this.loadAttributeNamesAttempts || 0;
	  this.loadAttributeNamesAttempts++;
	  var me = this;

	  return lotService.getAttributeNames()
	      .done(function (data) {
	        attributes(data);
	      })
	      .fail(function (xhr, result, message) {
	        if (me.loadAttributeNamesAttempts < 5) me.loadAttributeNames();
	        else showUserMessage('Failed to get attribute name values.', { description: 'There was a problem loading attribute names. Please notify system administrator with the following error message: "' + message + '".', type: 'error' });
	      });
	}

	InventoryPickerViewModel.prototype.dispose = function () {
	  ko.utils.arrayForEach(this.disposables, this.disposeOne);
	  ko.utils.objectForEach(this, this.disposeOne);
	};

	InventoryPickerViewModel.prototype.disposeOne = function (propOrValue, value) {
	  var disposable = value || propOrValue;

	  if (disposable && typeof disposable.dispose === "function") {
	    disposable.dispose();
	  }
	};

	module.exports = {
	  viewModel: InventoryPickerViewModel,
	  template: __webpack_require__(117)
	};

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 109 */
/***/ (function(module, exports, __webpack_require__) {

	var lotInventoryItemFactory = __webpack_require__(110),
	    rvc = __webpack_require__(8);

	function PickableInventoryItem( input, checkOutOfRange ) {
	    if (!(this instanceof PickableInventoryItem)) return new PickableInventoryItem( input, checkOutOfRange );

	    if (input instanceof lotInventoryItemFactory && ko.isObservable(input.QuantityPicked)) {
	        return input;
	    }

	    var inventoryItem = lotInventoryItemFactory( input, checkOutOfRange ),
	        qtyInput = input.QuantityPicked == undefined ? undefined : Number(input.QuantityPicked),
	        quantityPicked = ko.observable(qtyInput),
	        preservedQuantityPicked = ko.observable(Number(input.QuantityPicked) || undefined),
	        quantityOnHand = ko.observable(input.Quantity || 0),
	        maxQuantityPickedValue = ko.pureComputed(function () {
	            return quantityOnHand() + (preservedQuantityPicked() || 0);
	        });

	    inventoryItem.LotType = rvc.lists.lotTypes.fromLotKey(input.LotKey);
	    inventoryItem.isInitiallyPicked = ko.observable(input.isPicked || false);
	    inventoryItem.ValidForPicking = input.ValidForPicking;
	    inventoryItem.checkOutOfRange = checkOutOfRange;
	    inventoryItem.OrderItemKey = input.OrderItemKey;

	    inventoryItem.QuantityPicked = ko.pureComputed({
	        read: function () {
	            return quantityPicked();
	        },
	        write: function (value) {
	            var oldValue = quantityPicked();
	            if (value === oldValue) { return; }

	            var numVal = Number(value);

	            if (isNaN(numVal)) { quantityPicked(null); }
	            else quantityPicked(numVal);

	            ko.postbox.publish('pickedQuantityChanged', { value: numVal, item: inventoryItem, oldValue: oldValue });
	        },
	        owner: inventoryItem
	    });

	    inventoryItem.isPicked = ko.pureComputed(function () {
	        return inventoryItem.isInitiallyPicked() || inventoryItem.QuantityPicked() > 0;
	    });

	    inventoryItem.WeightPicked = ko.pureComputed(function () {
	        var qtyPicked = inventoryItem.QuantityPicked() || 0;
	        return qtyPicked > 0
	            ? calculatePoundsForQuantity(qtyPicked)
	            : '';
	    });
	    inventoryItem.QuantityAvailable = ko.pureComputed(function () {
	        return inventoryItem.QuantityPicked() >= 0 ?
	            maxQuantityPickedValue() - (inventoryItem.QuantityPicked() || 0) :
	            maxQuantityPickedValue();
	    });
	    inventoryItem.TotalWeightAvailable = ko.pureComputed(function () {
	        return (inventoryItem.QuantityAvailable() || 0) * inventoryItem.PackagingCapacity;
	    });
	    inventoryItem.isChanged = ko.computed(function () {
	        return inventoryItem.QuantityPicked() !== preservedQuantityPicked();
	    });

	    inventoryItem.validation = ko.validatedObservable({
	        quantityPicked: inventoryItem.QuantityPicked.extend({
	            min: 0,
	            max: maxQuantityPickedValue
	        })
	    });

	    inventoryItem.setInitialQuantityPicked = function (value) {
	        preservedQuantityPicked(value);
	        quantityPicked(value);
	        inventoryItem.QuantityPicked(value);
	        inventoryItem.commit();
	    };
	    inventoryItem.revert = function () {
	        inventoryItem.QuantityPicked(preservedQuantityPicked());
	    };
	    inventoryItem.commit = function () {
	        quantityOnHand(inventoryItem.QuantityAvailable());
	        preservedQuantityPicked(inventoryItem.QuantityPicked());
	        if (quantityPicked() > 0 && inventoryItem.validation.isValid()) {
	            inventoryItem.isInitiallyPicked(true);
	        } else if (quantityPicked() <= 0 || isNaN(quantityPicked())) {
	            inventoryItem.isInitiallyPicked(false);
	        }
	    };

	    function calculatePoundsForQuantity(quantity) {
	        var qty = parseInt(quantity);
	        if (!qty) return 0;
	        return qty * inventoryItem.PackagingCapacity;
	    }

	    return inventoryItem;
	}

	module.exports = PickableInventoryItem;


/***/ }),
/* 110 */
/***/ (function(module, exports, __webpack_require__) {

	function LotInventoryItem ( values, checkOutOfRange ) {
	    if (!(this instanceof LotInventoryItem)) { return new LotInventoryItem( values, checkOutOfRange ); }

	    var lotSummaryFactory = __webpack_require__(111);

	    this.disposables = [];

	    var base = new lotSummaryFactory( values, checkOutOfRange );
	    var lot = this;

	    for (var prop in base) {
	        if (base.hasOwnProperty(prop))
	            lot[prop] = base[prop];
	    }

	    lot.InventoryKey = values.InventoryKey;
	    lot.ToteKey = values.ToteKey;
	    lot.QuantityOnHand = values.Quantity;
	    lot.PackagingDescription = values.PackagingDescription || values.PackagingProduct.ProductName;
	    lot.PackagingProductKey = values.PackagingProductKey || values.PackagingProduct.ProductKey;
	    lot.PackagingCapacity = values.PackagingProduct?values.PackagingProduct.Weight : values.PackagingCapacity;
	    lot.ReceivedPackagingName = values.ReceivedPackagingName;
	    lot.LocationKey = values.LocationKey || values.Location.LocationKey;
	    lot.LocationName = values.LocationName || values.Location.Description;
	    lot.WarehouseName = values.WarehouseName || values.Location.FacilityName;
	    lot.WarehouseKey = values.WarehouseKey || values.Location.FacilityKey;
	    lot.TotalWeightOnHand = (values.PackagingProduct ? values.PackagingProduct.Weight : values.PackagingCapacity) * values.Quantity;
	    lot.Notes = values.Notes;
	    lot.CustomerProductCode = values.CustomerProductCode;
	    lot.CustomerLotCode = values.CustomerLotCode;

	    return lot;
	}

	LotInventoryItem.prototype.dispose = function () {
	    ko.utils.arrayForEach(this.disposables, this.disposeOne);
	    ko.utils.objectForEach(this, this.disposeOne);
	};

	LotInventoryItem.prototype.disposeOne = function(propOrValue, value) {
	    var disposable = value || propOrValue;

	    if (disposable && typeof disposable.dispose === "function") {
	        disposable.dispose();
	    }
	};



	module.exports = LotInventoryItem;


/***/ }),
/* 111 */
/***/ (function(module, exports, __webpack_require__) {

	var lotDefectFactory = __webpack_require__(112);
	var lotDefectResolutionFactory = __webpack_require__(113);
	var LotAttributeFactory = __webpack_require__(114);
	var rvc = __webpack_require__(8);

	__webpack_require__(71);

	function LotSummary( values, checkOutOfRange ) {
	    if (!(this instanceof LotSummary)) return new LotSummary(values);
	    var self = this;

	    this.LotKey = values.LotKey;
	    this.LotDate = values.LotDate;
	    this.AstaCalc = values.AstaCalc;
	    this.LoBac = values.LoBac;
	    this.HoldType = ko.observable(values.HoldType).extend({ lotHoldType: true });
	    this.HoldDescription = ko.observable(values.HoldDescription);
	    this.Product = values.Product ? JSON.parse(ko.toJSON(values.Product)) : {};
	    this.ProductionStatus = ko.observable(values.ProductionStatus).extend({ productionStatusType: true });
	    this.QualityStatus = ko.observable(values.QualityStatus).extend({ lotQualityStatusType: true });
	    this.ProductSpecStatus = ko.observable(values.ProductSpecStatus);
	    this.Defects = ko.observableArray(ko.utils.arrayMap(values.Defects, function (item) { return lotDefectFactory(item); }));
	    this.Attributes = this.buildAttributes( values.Attributes, checkOutOfRange );
	    this.Treatment = JSON.parse(ko.toJSON(values.InventoryTreatment || values.Treatment || {}));
	    this.CustomerName = values.CustomerName;
	    this.CustomerKey = values.CustomerKey;
	    this.Notes = values.Notes;
	    this.InHouseDefects = self.Defects.filter(function (d) {
	      return ko.unwrap(d.DefectType) === rvc.lists.defectTypes.InHouseContamination.key;
	    });
	    this.OpenInHouseDefects = self.InHouseDefects.filter(function(d) {
	      return !d.isResolved();
	    });

	    this.Product.ProductType = ko.observable(ko.unwrap(values.Product.ProductType)).extend({ inventoryTypes: true });

	    this.QualityControlNotebookKey = values.QualityControlNotebookKey;
	    this.ValidLotQualityStatuses = values.ValidLotQualityStatuses;
	    this.OldContextLotStat = values.OldContextLotStat;
	    this.CustomerAllowances = values.CustomerAllowances;
	    this.CustomerOrderAllowances = values.CustomerOrderAllowances;
	    this.ContractAllowances = values.ContractAllowances;

	    this.tooltipText = ko.computed(function () {
	        return this.LotKey + ' - ' + this.Product.ProductName;
	    }, this);

	    return self;
	}

	LotSummary.prototype.buildAttributes = function( attributeValues, checkOutOfRange ) {
	    if (!attributeValues || !attributeValues.length) return [];

	    var defects = this.Defects();
	    defects = defects && defects.length ? defects.reverse() : [];
	    var attrDefectsCache = buildAttributeDefectsCache(defects);

	    return ko.utils.arrayMap(attributeValues, function (attr) {
	        attr.Defect = attrDefectsCache[attr.Key];
	        return new LotAttributeFactory( attr, checkOutOfRange );
	    });
	};

	function buildAttributeDefectsCache(defects) {
	    var dCache = [];
	    ko.utils.arrayMap(defects, function (d) {
	        if (d.AttributeDefect && d.AttributeDefect.AttributeShortName) {
	            dCache[d.AttributeDefect.AttributeShortName] = d;
	        }
	    });
	    return dCache;
	}

	module.exports = LotSummary;


/***/ }),
/* 112 */
/***/ (function(module, exports, __webpack_require__) {

	function EditableLotDefect(input) {
	    if (input instanceof EditableLotDefect) return input;
	    if (!(this instanceof EditableLotDefect)) return new EditableLotDefect(input);

	    var lotDefectResolutionFactory = __webpack_require__(113);
	    var values = ko.toJS(input) || {};
	    var model = this;

	    this.LotDefectKey = values.LotDefectKey;
	    this.DefectType = ko.observable(ko.utils.unwrapObservable(values.DefectType)).extend({ defectType: true, required: true });
	    this.Description = ko.observable(values.Description).extend({ required: true });
	    this.Resolution = ko.observable(values.Resolution ? lotDefectResolutionFactory(values.Resolution) : undefined);
	    this.AttributeDefect = values.AttributeDefect;
	    this.SummaryText = ko.computed({
	        read: function () {
	            return values.AttributeDefect
	                ? this.DefectType && this.DefectType.displayValue() + " (" + values.AttributeDefect.OriginalMinLimit + " - " + values.AttributeDefect.OriginalMaxLimit + ")"
	                : this.Description();
	        },
	        owner: model,
	    });
	    this.isResolved = ko.computed(function() {
	        return model.Resolution() != undefined;
	    });

	    return this;
	}

	module.exports = EditableLotDefect;

/***/ }),
/* 113 */
/***/ (function(module, exports) {

	function LotDefectResolution(values) {
	    if (!(this instanceof LotDefectResolution)) return new LotDefectResolution(values);
	    this.LotDefectKey = values.LotDefectKey;
	    this.ResolutionType = ko.observable(values.ResolutionType).extend({ defectResolutionType: true, required: true });
	    this.Description = ko.observable(values.Description).extend({ required: true });
	    this.isEditing = ko.observable(false);
	}

	module.exports = LotDefectResolution;

/***/ }),
/* 114 */
/***/ (function(module, exports) {

	function LotAttribute( values, checkOutOfRange ) {
	    if (!(this instanceof LotAttribute)) return new LotAttribute(values);

	    var value = values.Value;

	    this.Key = values.Key;
	    this.Name = values.Name;
	    this.Value = value;
	    this.AttributeDate = values.AttributeDate;
	    this.Defect = values.Defect;
	    this.isValueComputed = values.Computed || false;

	    this.outOfRange = 0;
	    //this.outOfRange = typeof checkOutOfRange === 'function' && !ko.isObservable( checkOutOfRange ) ? checkOutOfRange.call( this, this.Key, this.Value ) : 0;
	    if ( ko.isComputed( checkOutOfRange ) ) {
	      this.outOfRange = ko.pureComputed(function() {
	        var checkerFunction = checkOutOfRange();
	        return typeof checkerFunction === 'function' && checkerFunction.call(this, this.Key, this.Value);
	      }, this);
	    } else if ( typeof checkOutOfRange === 'function' && !ko.isObservable( checkOutOfRange ) ) {
	      this.outOfRange = checkOutOfRange.call( this, this.Key, this.Value );
	    }

	    this.formattedValue = value && value.toLocaleString();

	    return this;
	}

	module.exports = LotAttribute;



/***/ }),
/* 115 */
/***/ (function(module, exports, __webpack_require__) {

	__webpack_require__(71);
	var rvc = __webpack_require__(8);
	var LotAttribute = __webpack_require__(114);

	ko.filters.tryNumber = function (value, fallbackValue) {
	  var val = ko.unwrap(value);
	  return val == null || isNaN(val) ? fallbackValue : val.toLocaleString();
	};
	ko.filters.tryRoundedNumber = function (value, fallbackValue) {
	  var val = Number(ko.unwrap(value));
	  var opts = val >= 10 ?
	    {
	    maximumFractionDigits: 0
	  } :
	    {
	    maximumFractionDigits: 2
	  };
	  return val == null || isNaN(val) ?
	    fallbackValue :
	    val.toLocaleString('en-US', opts);
	};

	__webpack_require__(37);

	/**
	  * @param {object} params Object containing argument parameters
	  * @param {object[]} [attributes] array of attribute ojbects to be displayed in the table header attribute objects can be retrieved from lotService.getAttributeNames
	  * @param {number} [targetWeight] - The target total weight for a pick
	  * @param {object} [targetProduct] the product for which to display product spec information product object is expected to contain AttributeRanges. When target product is undefined, the product spec is not displayed.
	  * @param {bool} [isReadOnly=false] when true, disables the QuantiyPicked field
	  * @param {boolean} hideTheoretical - Disables the calulcation and display of theoretical attrs
	  * @param {object[]} inventoryItems array of items to display (table rows). Values should be in the shape of PickableInventoryItem (app/models)
	  * @param {object} [inventoryType] InventoryType ID to enable filtering display of picked items by type
	  * @param {boolean} [viewOnly=false] - Disables the picker controls and modifies view to be text-only
	  * @param {observable} [exports] Receives the exported model.
	  * @exports {InventoryPickingTable}
	  */
	function InventoryPickingTable(params) {
	  if (!(this instanceof InventoryPickingTable)) { return new InventoryPickingTable(params); }

	  var self = this,
	      input = params.input,
	      attributesToDisplay = ko.computed(function () {
	        return ko.unwrap(input.attributes) || [];
	      });
	  self.inventoryType = ko.pureComputed(function () {
	    var type = ko.unwrap(input.inventoryType);
	    return type == null ?
	      undefined :
	      type.key == null ?
	        type :
	        type.key;
	  });

	  self.disposables = [];

	  var useCustomerSpec = input.useCustomerSpec || ko.observable( false );
	  var targetProductSpec = ko.computed(function () {
	    var tProduct = ko.unwrap(input.targetProduct);
	    if (tProduct == null) {
	      return {};
	    }

	    var productSpec = {};

	    ko.utils.arrayMap( tProduct.AttributeRanges, function (attrRange) {
	      productSpec[attrRange.AttributeNameKey || attrRange.Key] = attrRange;
	    });

	    return productSpec;
	  });

	  self.isReadOnly = input.isReadOnly == null ? false : input.isReadOnly === true;
	  self.isViewOnly = params.viewOnly || false;
	  self.hideTheoretical = params.hideTheoretical;

	  self.targetWeight = ko.pureComputed(function() {
	    return ko.unwrap( input.targetWeight );
	  });
	  self.targetProductName = ko.pureComputed(function () {
	    var tProduct = ko.unwrap(input.targetProduct);

	    return tProduct ? tProduct.ProductCodeAndName : tProduct;
	  });
	  self.attributeHeader = ko.observableArray( [] );

	  var attributeHeaderBuilder = ko.computed(function () {
	    var attrs = attributesToDisplay() || [];
	    var spec = JSON.parse( ko.toJSON( targetProductSpec ) );

	    var _useCustomerSpec = ko.unwrap( useCustomerSpec );
	    var _customerSpecs = _useCustomerSpec ? ko.unwrap( input.customerSpecs ) : null;

	    self.attributeHeader( [] );

	    if ( _customerSpecs && attrs.length ) {

	      var mappedCustomerAttrs = attrs.map(function ( attr ) {
	        var productSpec = spec[ attr.Key ];
	        var customerSpec = _customerSpecs[ attr.Key ];

	        attr.overridden = false;
	        attr.productMinTargetValue = null;
	        attr.productMaxTargetValue = null;
	        attr.minTargetValue = null;
	        attr.maxTargetValue = null;

	        if ( customerSpec ) {
	          attr.productMinTargetValue = productSpec &&
	            (productSpec.hasOwnProperty('MinValue') ?
	             productSpec.MinValue :
	             productSpec.minTargetValue);

	          attr.productMaxTargetValue = productSpec &&
	            (productSpec.hasOwnProperty('MaxValue') ?
	             productSpec.MaxValue :
	             productSpec.maxTargetValue);

	          attr.minTargetValue = customerSpec && customerSpec.MinValue;
	          attr.maxTargetValue = customerSpec && customerSpec.MaxValue;

	          attr.overridden = customerSpec && customerSpec.overridden;
	        }

	        return attr;
	      });

	      self.attributeHeader( mappedCustomerAttrs );

	      return mappedCustomerAttrs;
	    } else if ( spec && attrs.length ) {
	      var mappedSpecAttrs = ko.utils.arrayMap( attrs, function ( attr ) {
	        var currentTarget = spec[ attr.Key ];

	        attr.overridden = false;
	        attr.productMinTargetValue = null;
	        attr.productMaxTargetValue = null;

	        attr.minTargetValue = currentTarget ?
	          (currentTarget.hasOwnProperty('MinValue') ?
	           currentTarget.MinValue :
	           currentTarget.minTargetValue) : undefined;

	        attr.maxTargetValue = currentTarget ?
	          (currentTarget.hasOwnProperty('MaxValue') ?
	           currentTarget.MaxValue :
	           currentTarget.maxTargetValue) : undefined;

	        return attr;
	      });

	      self.attributeHeader( mappedSpecAttrs );

	      return mappedSpecAttrs;
	    }

	    self.attributeHeader( attrs );
	    return attrs;
	  });
	  self.hasProductSpec = ko.computed(function () {
	    return self.attributeHeader().length > 0;
	  });

	  function filterByProductTypeDelegate(item) {
	    return self.inventoryType() == null || ko.unwrap(item.Product.ProductType) === self.inventoryType();
	  }

	  self.allInventoryItems = input.inventoryItems;
	  self.allPickedItems = input.inventoryItems.filter(function (i) { return ko.unwrap(i.isPicked) === true; });
	  self.inventoryItems = input.inventoryItems
	      .filter(filterByProductTypeDelegate)
	      .map(function (item) {
	        item.orderedAttributes = (function () {
	          var itemAttributes = ko.unwrap(item.Attributes);
	          var attrs = (attributesToDisplay());
	          if (!attrs) return [];

	          return ko.utils.arrayMap(attrs, function (attrName) {
	            return ko.utils.arrayFirst(itemAttributes || [], function (attr) {
	              return attr.Key === attrName.Key;
	            }) || new LotAttribute({
	              Key: attrName.Key,
	              Name: attrName.Value,
	              Value: null,
	              formattedValue: '',
	              Defect: {},
	              isValueComputed: false
	            });
	          });
	        })();
	        if (ko.isObservable(item.HoldType) && ko.isObservable(item.HoldType.displayValue)) {
	          item.holdDescription = item.HoldType.displayValue();
	        } else {
	          item.holdDescription = rvc.lists.lotHoldTypes.findByKey(ko.unwrap(item.HoldType));
	          item.holdDescription = item.holdDescription && item.holdDescription.value || '';
	        }
	        return item;
	      });
	  self.pickedItems = self.inventoryItems.filter(function (i) { return ko.unwrap(i.isPicked) === true; });
	  self.initiallyPickedItems = self.pickedItems.filter(function (item) {
	    var orderKey = ko.unwrap( input.orderItemKey );

	    if ( orderKey ) {
	      return ko.unwrap( item.isInitiallyPicked ) === true && item.OrderItemKey === orderKey;
	    }

	    return ko.unwrap(item.isInitiallyPicked) === true;
	  });
	  self.pickableItems = self.inventoryItems.filter(function (item) { return !ko.unwrap(item.isInitiallyPicked); });

	  self.totalPoundsPicked = ko.observable();
	  self.totalQuantityPicked = ko.observable();
	  self.isPickedWeightOverTarget = ko.pureComputed(function() {
	    var picked = +self.totalPoundsPicked();
	    var target = +ko.unwrap( self.targetWeight );

	    return picked > target;
	  });
	  self.isShowingHeader = ko.pureComputed(function () {
	    return ko.unwrap(input.targetProduct) != null;
	  });

	  self.theoreticalAttributeValues = ko.observableArray([]);
	  self.attributesOutOfSpec = ko.computed(function() {
	    var theoreticals = self.theoreticalAttributeValues() || [],
	      pickedItems = self.pickedItems() || [];

	    if (!pickedItems.length || !theoreticals.length) {
	      return [];
	    }

	    var oos = [], index = 0;
	    ko.utils.arrayForEach(self.attributeHeader(), function(attr) {
	      var tValue = theoreticals[index];
	      if (tValue > attr.maxTargetValue || tValue < attr.minTargetValue) {
	        oos.push(attr);
	      }
	      index++;
	    });

	    return oos;
	  });
	  self.totalPoundsOnScreen = ko.pureComputed(function() {
	    var totalPounds = 0;
	    ko.utils.arrayForEach(self.pickableItems(), function (i) {
	      totalPounds += ko.unwrap(i.TotalWeightAvailable);
	    });
	    return totalPounds;
	  });

	  // Subscriptions
	  if (!self.hideTheoretical) {
	    self.disposables.push([
	      self.initiallyPickedItems.subscribe(function() {
	        self.updateTheoreticalAttributeValues();
	      })
	    ]);

	    ko.postbox.subscribe('pickedQuantityChanged', function(item) {
	      self.updateTheoreticalAttributeValues();
	    });

	    self.updateTheoreticalAttributeValues();
	  }

	  if (ko.isObservable(params.exports)) {
	    params.exports({
	      attributesOutOfSpec: self.attributesOutOfSpec,
	      totalPoundsOnScreen: self.totalPoundsOnScreen
	    });
	  }
	  return self;
	}

	InventoryPickingTable.prototype.updateTheoreticalAttributeValues = function () {
	  var self = this,
	      totalQuantity = 0,
	      totalWeight = 0,
	      attributeNames = this.attributeHeader(),
	      theoreticalAttributesContainer = initializeAttributeContainer(attributeNames);

	  var allPickedItems = self.allPickedItems() || [];
	  ko.utils.arrayForEach(allPickedItems, function (item) {
	    totalWeight += ko.unwrap(item.WeightPicked) || 0;
	    totalQuantity += ko.unwrap(item.QuantityPicked);

	    ko.utils.arrayForEach(item.Attributes, function (attr) {
	      if (theoreticalAttributesContainer[attr.Key] != null) {
	        theoreticalAttributesContainer[attr.Key] += ((attr.Value || 0) * ko.unwrap(item.WeightPicked));
	      }
	    });
	  });

	  // Averages all attributes
	  var theoreticalArray = ko.utils.arrayMap(attributeNames, function (currentAttribute) {
	    return totalQuantity > 0 ?
	        (theoreticalAttributesContainer[currentAttribute.Key] / totalWeight) :
	        undefined;
	  });

	  self.totalQuantityPicked(totalQuantity);
	  self.totalPoundsPicked(totalWeight);
	  self.theoreticalAttributeValues(theoreticalArray);

	  function initializeAttributeContainer(attributeNames) {
	    var container = {};
	    ko.utils.arrayForEach(attributeNames, function (attr) {
	      container[attr.Key] = 0;
	    });
	    return container;
	  }
	};

	InventoryPickingTable.prototype.dispose = function () {
	  ko.utils.arrayForEach(this.disposables, this.disposeOne);
	  ko.utils.objectForEach(this, this.disposeOne);
	};

	InventoryPickingTable.prototype.disposeOne = function (propOrValue, value) {
	  var disposable = value || propOrValue;

	  if (disposable && typeof disposable.dispose === "function") {
	    disposable.dispose();
	  }
	};

	module.exports = {
	  viewModel: InventoryPickingTable,
	  template: __webpack_require__(116)
	};


/***/ }),
/* 116 */
/***/ (function(module, exports) {

	module.exports = "<div class=\"table-responsive\">\r\n  <table class=\"reset table table-condensed table-condensed-sm no-wrap\" data-bind=\"validationOptions: {\r\n    errorElementClass: 'danger',\r\n    insertMessages: false,\r\n    decorateElementOnModified: false\r\n    },\r\n    sortableTable: allInventoryItems,\r\n    floatThead: allInventoryItems,\r\n    \">\r\n    <thead>\r\n      <tr>\r\n        <th data-sort=\"Product.ProductCode\">Product<br />Code</th>\r\n        <th data-sort=\"Product.ProductName\">Product Nm.</th>\r\n        <th data-sort=\"LotKey\">Lot</th>\r\n        <th data-sort=\"ToteKey\">Tote</th>\r\n        <th data-sort=\"CustomerName\" class=\"truncate\" style=\"max-width: 120px;\">Customer</th>\r\n        <th data-sort=\"Treatment.TreatmentNameShort\">Trtmt.</th>\r\n        <th data-bind=\"visible: !isViewOnly\">Quantity</th>\r\n        <th data-bind=\"visible: !isViewOnly\">Weight</th>\r\n        <th data-sort=\"QuantityAvailable\">Qty. Avail.</th>\r\n        <th data-sort=\"TotalWeightAvailable\">Weight Avail.</th>\r\n        <th data-sort=\"PackagingDescription\">Packaging</th>\r\n        <th data-sort=\"LocationName\">Location</th>\r\n        <th data-sort=\"WarehouseName\">Warehouse</th>\r\n        <th data-sort=\"HoldType\">Holds</th>\r\n        <!-- ko template: 'lot-attribute-thead-cells' --><!-- /ko -->\r\n        <th>Packaging Received</th>\r\n        <th>Notes</th>\r\n      </tr>\r\n      <!-- ko if: isShowingHeader -->\r\n      <!-- ko template: hideTheoretical ? 'hideTheoretical' : 'showTheoretical' -->\r\n      <!-- /ko -->\r\n      <!-- /ko -->\r\n    </thead>\r\n    <tbody data-bind=\"foreach: $data.initiallyPickedItems\">\r\n      <tr class=\"success reset\" data-bind=\"validationElement: QuantityPicked\">\r\n        <!-- ko template: 'core-lot-tbody-cells-bundle' --><!-- /ko -->\r\n      </tr>\r\n    </tbody>\r\n    <tbody data-bind=\"foreach: { data: $data.pickableItems, afterAdd: $parentContext.$parent.reflowTable }\">\r\n      <tr data-bind=\"validationElement: QuantityPicked\">\r\n        <!-- ko template: 'core-lot-tbody-cells-bundle' --><!-- /ko -->\r\n      </tr>\r\n    </tbody>\r\n  </table>\r\n</div>\r\n\r\n<script id=\"lot-attribute-thead-cells\" type=\"text/html\">\r\n  <th data-sort=\"LoBac\">LoBac</th>\r\n  <!-- ko foreach: attributeHeader -->\r\n  <th data-bind=\"text: $data.Key, attr: { 'data-sort': 'orderedAttributes[' + $index() + '].Value' }\" style=\"white-space: nowrap\"></th><!-- /ko -->\r\n</script>\r\n\r\n<script id=\"core-lot-tbody-cells-bundle\" type=\"text/html\">\r\n  <td style=\"white-space: nowrap;\" class=\"truncate\" data-bind=\"text: Product.ProductCode\"></td>\r\n  <td style=\"white-space: nowrap;\" class=\"truncate\" data-bind=\"text: Product.ProductName\"></td>\r\n  <td style=\"white-space: nowrap\" data-bind=\"text: LotKey\"></td>\r\n  <td style=\"white-space: nowrap\" data-bind=\"text: ToteKey\"></td>\r\n  <td data-bind=\"text: CustomerName\" class=\"truncate\" style=\"max-width: 120px;\"></td>\r\n  <td data-bind=\"text: Treatment.TreatmentNameShort\"></td>\r\n  <td data-bind=\"visible: !$parent.isViewOnly\">\r\n    <input type=\"text\" class=\"form-control input-small\" data-bind=\"value: QuantityPicked, valueUpdate: 'input', disable: $parent.isReadOnly || !ValidForPicking\">\r\n  </td>\r\n  <td data-bind=\"visible: !$parent.isViewOnly, text: WeightPicked | number\"></td>\r\n  <td data-bind=\"text: QuantityAvailable | number\"></td>\r\n  <td data-bind=\"text: TotalWeightAvailable | number\"></td>\r\n  <td data-bind=\"text: PackagingDescription\"></td>\r\n  <td data-bind=\"text: LocationName\"></td>\r\n  <td data-bind=\"text: WarehouseName\"></td>\r\n  <td data-bind=\"text: holdDescription\"></td>\r\n  <td>\r\n    <input type=\"checkbox\" disabled=\"disabled\" tabindex=\"-1\" data-bind=\"checked: LoBac\" />\r\n  </td>\r\n  <!-- ko foreach: orderedAttributes -->\r\n  <td data-bind=\"css: { 'danger': outOfRange }\" style=\"min-width: 30px\">\r\n    <i class=\"fa\" data-bind=\"css: {\r\n      'fa-arrow-up': ko.unwrap( outOfRange ) > 0,\r\n      'fa-arrow-down': ko.unwrap( outOfRange ) < 0\r\n    }\"></i> <!-- ko text: formattedValue --><!-- /ko -->\r\n    </td>\r\n  <!-- /ko -->\r\n  <td data-bind=\"text: ReceivedPackagingName\"></td>\r\n  <td data-bind=\"text: Notes\"></td>\r\n</script>\r\n\r\n<script id=\"showTheoretical\" type=\"text/html\">\r\n  <tr class=\"info\">\r\n    <td colspan=\"15\">\r\n      <span><b data-bind=\"text: targetProductName\"></b></span>\r\n      <!-- ko if: targetWeight -->\r\n      (Pounds picked: <span data-bind=\"text: totalPoundsPicked() | toNumber, css: { 'text-danger': isPickedWeightOverTarget }\"></span>\r\n      <!-- ko if: $parent.orderItemKey -->\r\n      / <span data-bind=\"text: targetWeight() | toNumber\"></span> lbs\r\n      <!-- /ko -->\r\n      )\r\n      <!-- /ko -->\r\n    </td>\r\n    <!-- ko foreach: theoreticalAttributeValues -->\r\n    <td data-bind=\"text: $data | tryRoundedNumber: '-'\"></td><!-- /ko -->\r\n    <td colspan=\"2\"></td>\r\n  </tr>\r\n  <tr class=\"info\">\r\n    <td class=\"text-right\" colspan=\"15\">\r\n      <div class=\"pull-left\">\r\n        <strong data-bind=\"text: $parent.targetProductName\">Target Product</strong>\r\n      </div><span data-bind=\"if: hasProductSpec\">Target Min: </span>\r\n    </td>\r\n    <!-- ko foreach: attributeHeader -->\r\n    <td data-bind=\"text: minTargetValue | tryNumber:'-',\r\n    attr: {\r\n    'title': overridden ? productMinTargetValue : '' \r\n    },\r\n    css: {\r\n      'strong-em': overridden,\r\n      'cursor-help': overridden\r\n    }\"></td><!-- /ko -->\r\n    <td colspan=\"2\"></td>\r\n  </tr>\r\n  <tr class=\"info\">\r\n    <td class=\"text-right\" colspan=\"15\">&nbsp;<span data-bind=\"if: hasProductSpec\">Target Max: </span></td>\r\n    <!-- ko foreach: attributeHeader -->\r\n    <td data-bind=\"text: maxTargetValue | tryNumber:'-',\r\n    attr: {\r\n      'title': overridden ? productMaxTargetValue : ''\r\n    },\r\n    css: {\r\n      'strong-em': overridden,\r\n      'cursor-help': overridden\r\n    }\"></td><!-- /ko -->\r\n    <td colspan=\"2\"></td>\r\n  </tr>\r\n</script>\r\n\r\n<script id=\"hideTheoretical\" type=\"text/html\">\r\n  <tr class=\"info\">\r\n    <td colspan=\"11\">\r\n      <label data-bind=\"text: targetProductName\"></label>\r\n      <!-- ko if: targetWeight -->\r\n      <!-- ko template: 'targetWeightChunk' --><!-- /ko -->\r\n      <!-- /ko -->\r\n    </td>\r\n    <td class=\"text-right\" colspan=\"5\">\r\n      <span data-bind=\"if: hasProductSpec\">Target Min: </span>\r\n    </td>\r\n    <!-- ko foreach: attributeHeader -->\r\n    <td data-bind=\"text: minTargetValue | tryNumber:'-'\"></td><!-- /ko -->\r\n    <td colspan=\"2\"></td>\r\n  </tr>\r\n  <tr class=\"info\">\r\n    <td class=\"text-right\" colspan=\"15\">&nbsp;<span data-bind=\"if: hasProductSpec\">Target Min: </span></td>\r\n    <!-- ko foreach: attributeHeader -->\r\n    <td data-bind=\"text: maxTargetValue | tryNumber:'-'\"></td><!-- /ko -->\r\n    <td colspan=\"2\"></td>\r\n  </tr>\r\n</script>\r\n\r\n<script id=\"targetWeightChunk\" type=\"text/html\">\r\n  <!-- NOTE: This component can't currently get a proper sum of picked items with orderItemKey is in use. -->\r\n\r\n  <!-- ko if: ko.unwrap( $parent.orderItemKey ) && ko.unwrap( targetWeight ) -->\r\n  ( Target Weight:\r\n  <span data-bind=\"text: targetWeight() | toNumber\"></span> lbs )\r\n  <!-- /ko -->\r\n\r\n  <!-- ko if: !ko.unwrap( $parent.orderItemKey ) && ko.unwrap( targetWeight ) -->\r\n  ( Weight picked:\r\n  <span data-bind=\"text: totalPoundsPicked() | toNumber, css: { 'text-danger': isPickedWeightOverTarget }\"></span>\r\n  /\r\n  <span data-bind=\"text: targetWeight() | toNumber\"></span> lbs )\r\n  <!-- /ko -->\r\n</script>\r\n"

/***/ }),
/* 117 */
/***/ (function(module, exports) {

	module.exports = "<section data-bind=\"visible: !isLoaded()\">\r\n  <div class=\"text-center well\">\r\n    <i class=\"fa fa-spinner fa-pulse fa-3x\"></i>\r\n  </div>\r\n</section>\r\n<!-- ko if: isLoaded -->\r\n<div class=\"checkbox\" data-bind=\"visible: isCustomerSpecAvailable\">\r\n  <label>\r\n    <input type=\"checkbox\" data-bind=\"checked: isUsingCustomerSpec, enable: isCustomerSpecAvailable\"> Use Customer Spec\r\n  </label>\r\n</div>\r\n<!-- ko template: {\r\n  name: 'default-lot-inventory-listing-template',\r\n  afterRender: floatHeader\r\n  } -->\r\n<!-- /ko -->\r\n<!-- /ko -->\r\n\r\n<script id=\"default-lot-inventory-listing-template\" type=\"text/html\">\r\n  <section data-bind=\"visible: errors().length > 0\">\r\n    <div class=\"alert alert-danger\" style=\"padding: 5px;\">\r\n      <ul class=\"list-unstyled\" data-bind=\"foreach: errors\">\r\n        <li>\r\n          <b data-bind=\"text: $data\"></b> contains invalid quantities picked\r\n        </li>\r\n      </ul>\r\n    </div>\r\n  </section>\r\n  <section data-bind=\"visible: specWarnings().length > 0\" style=\"padding: 0; margin-bottom: -20px;\">\r\n    <div class=\"alert alert-warning\" style=\"padding: 5px;\">\r\n      <ul class=\"list-unstyled list-inline\">\r\n        <li><b>Attributes out of spec: </b></li>\r\n        <!-- ko foreach: specWarnings -->\r\n        <li>\r\n          <span data-bind=\"text: Key\" class=\"label label-default\"></span> \r\n        </li>\r\n        <!-- /ko -->\r\n      </ul>\r\n    </div>\r\n  </section>\r\n  <section>\r\n    <!-- ko foreach: pickingTableViewModels -->\r\n    <div class=\"panel panel-default\" data-bind=\"visible: $parent.showTable($data)\">\r\n      <div class=\"pick-table-wrap\" data-bind=\"growToWindowHeight: { offset: 110 }\">\r\n        <inventory-picking-table params=\"input: $data, hideTheoretical: $parent.hideTheoretical, exports: $data.exports\"></inventory-picking-table>\r\n      </div>\r\n    </div>\r\n    <!-- /ko -->\r\n  </section>\r\n</script>\r\n"

/***/ }),
/* 118 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {/**
	  * @param {Object} input - Initial settings for filters
	  * @param {string} input.ingredientType
	  * @param {string} input.inventoryType
	  * @param {string} input.lotType
	  * @param {string} input.lotKey
	  * @param {string} input.packagingProductKey
	  * @param {string} input.productKey
	  * @param {string} input.warehouseLocationKey
	  * @param {Object} [options] - Options to populate select boxes with
	  * @param {Object} options.ingredients - Ingredients
	  * @param {Object} options.products - Products, sorted by lot type
	  * @param {Object[]} options.locations - Rincon warehouse locations
	  * @param {Object[]} options.packaging - Packaging products
	  * @param {boolean} [options.filterProductsWithInventory=true] - If not provided options, lot filters will return products with existing inventory
	  * @param {Object} filters - Observable; container for filters
	  * @param {string} mode - Sets filtering mode; 'inventory' or 'qualityControl'
	  * @param {boolean} [disable=false] - Observable; Disables filter input fields
	  * @param {boolean} [lotKeyOnly=false] - Observable; Hide all filters except lotkey
	  * @param {boolean} [startingLotKey] - Use startingLotKey instead of lotKey
	  */


	__webpack_require__(31);
	var disposableHelper = __webpack_require__(119);

	var rvc = __webpack_require__(8),
	    lotService = __webpack_require__(72),
	    productsService = __webpack_require__(24),
	    warehouseService = __webpack_require__(6),
	    warehouseLocationsService = __webpack_require__(11);

	function FiltersViewModel(params) {
	  params = params || {};
	  params.options = params.options || {};
	  var input = ko.unwrap(params.input) || {};
	  var filterProductsWithInventory = params.filterProductsWithInventory || false;

	  if (!(this instanceof FiltersViewModel)) {
	    return new FiltersViewModel(params);
	  }

	  var self = this;
	  var productOptionsByLotType = params.options.products || {};
	  var packagingProducts = ko.observableArray(params.options.packaging || []);
	  var ingredientsByProductType = ko.observable(params.options.ingredients || []);
	  var facilityOptions = ko.observableArray( [] );
	  var warehouseLocationOptions = ko.observableArray(params.options.locations || []);
	  this.isLotKeyOnly = params.lotKeyOnly ?
	    params.lotKeyOnly :
	    false;

	  this.uiTemplate = ko.pureComputed(function() {
	    return ko.unwrap( self.isLotKeyOnly ) ? 'filters-lotkey-only' : 'filters-base';
	  });

	  this.disposables = [];
	  this.includeInventoryFilters = params.mode === 'inventory';
	  this.includeQualityControlFilters = params.mode === 'qualityControl';
	  this.includeInventoryAdjustmentFilters = params.mode === 'inventoryAdjustment';
	  this.enableFacilityFilter = params.enableFacilityFilter || false;
	  this.rinconKey = 2;

	  // Data Structure
	  this.inventoryType = ko.observable(input.inventoryType).extend({ inventoryType: true });

	  this.isInventoryChile = ko.pureComputed(function() {
	    var _invType = self.inventoryType();

	    return _invType && _invType.key === 1;
	  });
	  this.chileTypeOptions = ko.utils.arrayMap( Object.keys( rvc.lists.chileClassifications ), function( opt ) {
	    return rvc.lists.chileClassifications[ opt ];
	  });
	  this.chileType = ko.observable();

	  this.lotType = ko.observable(input.lotType).extend({ lotType: true });
	  this.ingredientType = ko.observable(input.ingredientType);
	  this.productKey = ko.observable(input.productKey);
	  this.packagingProductKey = ko.observable(input.packagingProductKey);
	  this.treatmentKey = ko.observable().extend({ treatmentType: true });
	  this.lotKey = ko.observable(input.lotKey).extend({ lotKey: setInventoryTypeForLotKey });
	  this.facilityKey = ko.observable( input.facilityKey );
	  this.warehouseLocationKey = ko.observable(input.warehouseLocationKey);
	  this.lotTypeOptions = ko.pureComputed(function () {
	    var inventoryType = self.inventoryType();
	    return inventoryType && rvc.lists.lotTypesByInventoryType[inventoryType.value] || [];
	  });

	  this.streetFilter = ko.observable();
	  this.productionStatus = ko.observable().extend({ productionStatusType: true });
	  this.productionStart = ko.observableDate();
	  this.productionEnd = ko.observableDate();
	  this.qualityStatus = ko.observable().extend({ lotQualityStatusType: true });

	  /** Inventory adjustment filters */
	  this.beginningDate = ko.observableDate();
	  this.endingDate = ko.observableDate();

	  this.enableLotTypeFilter = ko.pureComputed(function () {
	    return self.inventoryType() != undefined;
	  });
	  this.enableProductFilter = ko.pureComputed(function () {
	    return self.lotType() != undefined;
	  });

	  this.isRincon = ko.computed(function() {
	    if ( self.facilityKey() === self.rinconKey ) {
	      return true;
	    } else {
	      self.warehouseLocationKey( null );
	      return false;
	    }
	  });
	  this.isAllDisabled = params.disable || false;
	  this.isDisabled = ko.pureComputed(function() {
	    return ko.unwrap( self.isAllDisabled ) || self.lotKey() != null;
	  });

	  this.disposables.push(this.productionStart.subscribe(function(val) {
	    if (val != null && self.productionEnd() == null) {
	      self.productionEnd(Date.now().format('m/d/yyyy'));
	    }
	  }));

	  // Filter options
	  this.ingredientTypeOptions = ko.pureComputed(function () {
	    var productType = self.inventoryType(),
	    ingredientTypeDictionary = ingredientsByProductType();

	    return (!productType || !ingredientTypeDictionary) ?
	        [] :
	        ingredientTypeDictionary[productType.value || productType] || [];
	  });

	  this.productKeyOptions = ko.pureComputed(function () {
	    var lotType = self.lotType();
	    return lotType && productOptionsByLotType[lotType.value]() || [];
	  });

	  this.packagingProductKeyOptions = ko.pureComputed(function () {
	    return packagingProducts() || [];
	  });

	  this.facilityOptions = ko.pureComputed(function() {
	    return facilityOptions() || [];
	  });

	  this.warehouseLocationOptions = ko.pureComputed(function () {
	    var locations = warehouseLocationOptions() || [];
	    var streetFilter = self.streetFilter();
	    if ( streetFilter ) {
	      return ko.utils.arrayFilter(locations, function(l) { return l.GroupName === streetFilter; });
	    } else return locations;
	  });
	  this.streetFilterOptions = ko.pureComputed(function () {
	    var locations = warehouseLocationOptions() || [];
	    return ko.utils.arrayGetDistinctValues(
	      ko.utils.arrayMap(locations, function(l) { return l.GroupName; })
	    ).sort();
	  });

	  this.hasIngredients = ko.pureComputed(function () {
	    return self.ingredientTypeOptions().length > 0;
	  });

	  // Behaviors
	  function loadProductOptions() {
	    var productDfds = [];
	    var options = {
	      filterProductsWithInventory: filterProductsWithInventory
	    };

	    rvc.helpers.forEachLotType(loadAndPush);

	    var checkResults = $.when.apply($, productDfds);

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

	    return checkResults;
	  }

	  this.getFacilityLocations = ko.asyncCommand({
	    execute: function( facilityKey, complete ) {
	      var getLocations = warehouseLocationsService.getWarehouseLocations( facilityKey ).then(
	        function( data, textStatus, jqXHR ) {
	        return data;
	      },
	      function( jqXHR, textStatus, errorThrown ) {
	        showUserMessage( 'Failed to get locations for warehouse', {
	          description: errorThrown
	        });
	      }).always( complete );

	      return getLocations;
	    }
	  });

	  function setInventoryTypeForLotKey() {
	    var inventoryTypeKey = self.lotKey.InventoryTypeKey();

	    var inventoryType = ko.utils.arrayFirst(self.inventoryType.options, function ( opt ) {
	      return opt.value.key === inventoryTypeKey;
	    });

	    self.inventoryType( inventoryType.value );
	  }

	  if ( params.options.hasOwnProperty('ingredients') &&
	             params.options.hasOwnProperty('products') &&
	             params.options.hasOwnProperty('locations') &&
	             params.options.hasOwnProperty('packaging') ) {
	    init(params.input);
	  } else {
	    loadOptions().then(
	    function( ingredients, products, packaging, locations, facilities ) {
	      ingredientsByProductType(ingredients[0] || []);
	      packagingProducts(packaging[0] || []);
	      warehouseLocationOptions(locations[0] || []);

	      if ( facilities ) {
	        facilityOptions(facilities && facilities[0] || []);

	        var rinconKey = ko.utils.arrayFirst( facilities[0], function( facility ) {
	          return facility.FacilityName === 'Rincon';
	        });

	        if ( rinconKey ) {
	          self.facilityKey.subscribe(function( newFacilityKey ) {
	            if ( newFacilityKey && self.getFacilityLocations.canExecute() ) {
	              self.getFacilityLocations.execute( newFacilityKey ).then(
	                function( data, textStatus, jqXHR ) {
	                warehouseLocationOptions( data );
	              });
	            }
	          });

	          self.rinconKey = rinconKey.FacilityKey;
	          self.facilityKey( rinconKey.FacilityKey );
	        }
	      }

	      init(params.input);
	    });
	  }

	  function loadOptions() {
	    var getIngredients = lotService.getIngredientsByProductType().then( null );
	    var getProducts = loadProductOptions().then( null );
	    var getPackaging = productsService.getPackagingProducts().then( null );
	    var getLocations = self.enableFacilityFilter ?
	      $.Deferred().resolve( [] ) :
	      warehouseLocationsService.getRinconWarehouseLocations().then( null );

	    var checkComplete;

	    if ( self.enableFacilityFilter ) {
	      var getFacilities = warehouseService.getWarehouses().then( null );

	      checkComplete = $.when( getIngredients, getProducts, getPackaging, getLocations, getFacilities );
	    } else {
	      checkComplete = $.when( getIngredients, getProducts, getPackaging, getLocations );
	    }

	    return checkComplete;
	  }

	  function init(input) {
	    self.lotType( undefined );
	    self.ingredientType( undefined );
	    self.productKey( undefined );
	    self.warehouseLocationKey( undefined );

	    if ( input == undefined ) {
	      return;
	    }

	    var data = ko.toJS( input ) || {};
	    var lotReady = $.Deferred();
	    var productKeySub = $.Deferred();
	    var ingredientTypeSub = $.Deferred();

	    self.inventoryType( ko.utils.arrayFirst( self.inventoryType.options, function ( item ) {
	      if ( comparator( item.value.key, data.inventoryType ) ) {
	        if ( self.lotTypeOptions().length > 0 ) {
	          lotReady.resolve();
	        } else {
	          var lotTypeSub = self.lotTypeOptions.subscribe(function () {
	            if ( self.lotTypeOptions().length > 0 ) {
	              lotReady.resolve();
	              lotTypeSub.dispose();

	              ko.utils.arrayRemoveItem( self.disposables, lotTypeSub );
	            }
	          });
	          self.disposables.push( lotTypeSub );
	        }
	        return true;
	      }
	    } ) );
	    self.packagingProductKey( ko.utils.arrayFirst( self.packagingProductKeyOptions(), function (item) {
	      return comparator( item.ProductKey, data.packagingProductKey );
	    } ) );
	    self.treatmentKey( ko.utils.arrayFirst( self.treatmentKey.options, function (item) {
	      return comparator( item.key, data.treatmentKey );
	    } ) );

	    function comparator( target, key ) {
	      var targetKey = keyParse( target ),
	          currentKey = keyParse( key );

	      if ( currentKey === targetKey ) {
	        return true;
	      }

	      return false;

	      function keyParse( input ) {
	        return input === Object( input ) ?
	            input.key :
	            input;
	      }
	    }

	    self.lotKey( data.lotKey );

	    var checkLotReady = lotReady.then(
	      function() {
	        ko.utils.arrayFirst( self.lotTypeOptions(), function ( item ) {
	          if ( comparator( item.key, data.lotType ) ) {
	            var productSub = self.productKeyOptions.subscribe(function ( data ) {
	              productKeySub.resolve( true );
	              productSub.dispose();
	              ko.utils.arrayRemoveItem( self.disposables, productSub );
	            });
	            self.disposables.push( productSub );

	            if ( self.hasIngredients() ) {
	              var ingredientSub = self.productKeyOptions.subscribe(function (data) {
	                ingredientTypeSub.resolve( true );
	                ingredientSub.dispose();
	                ko.utils.arrayRemoveItem( self.disposables, ingredientSub );
	              });
	              self.disposables.push( ingredientSub );
	            } else {
	              ingredientTypeSub.resolve( false );
	            }

	            self.lotType(item);

	            return true;
	          }
	        } );
	      }
	    );

	    var checkProductAndIngredientReady = $.when( productKeySub, ingredientTypeSub ).then(
	    function( productsReady, ingredientsReady ) {
	      ko.utils.arrayFirst( self.productKeyOptions(), function (item) {
	        if ( comparator( item.ProductKey, data.productKey ) ) {
	          self.productKey( item );
	        }
	      } );

	      if ( ingredientSub ) {
	        if ( typeof data.ingredientType === 'string' ) {
	          ko.utils.arrayFirst( self.ingredientTypeOptions(), function (item) {
	            if ( item.Description === data.ingredientType ) {
	              self.ingredientType( item );

	              return true;
	            }
	          } );
	        } else {
	          ko.utils.arrayFirst( self.ingredientTypeOptions(), function (item) {
	            if ( comparator( item.ProductKey, data.ingredientType ) ) {
	              self.ingredientType( item );

	              return true;
	            }
	          } );
	        }
	      }
	    });
	  }

	  function getOptionDefaultValue(val) {
	    if (val == undefined) { return null; }
	    if (typeof val === "string") { return val; }
	    if ($.isNumeric(val)) { return val.toString(); }

	    return val.key && (getOptionDefaultValue(val.key) || null);
	  }

	  function getInventoryTypeOptionByKey(val, target) {
	    var key = val == undefined ?
	      null :
	      val.key || val;

	    if (key == undefined) {
	      return null;
	    }

	    var selected = ko.utils.arrayFirst(ko.unwrap(target.options), function (opt) {
	      return opt.value && opt.value.key === key;
	    });

	    return selected == undefined ?
	      null :
	      selected.value;
	  }

	  var filters = {
	    inventoryType: ko.pureComputed({
	      read: function () {
	        var invType = this();
	        return invType && invType.key != undefined ? invType.key : (invType || null);
	      },
	      write: function (val) {
	        this(val == undefined ?
	            null :
	            getInventoryTypeOptionByKey(val, this));
	      },
	      owner: self.inventoryType
	    }),

	    productSubType: ko.pureComputed({
	      read: function () {
	        var _invType = self.inventoryType();
	        
	        switch(_invType.key) {
	          case rvc.lists.inventoryTypes.Chile.key:
	            return self.chileType();
	          default:
	            return null;
	        }
	      },
	      write: function (val) {
	        var _invType = self.inventoryType();

	        self.chileType(null);

	        switch (_invType) {
	          case rvc.lists.inventoryTypes.Chile.key:
	            self.chileType(val);
	            break;
	        }
	      },
	      owner: self
	    }),

	    lotType: ko.pureComputed({
	      read: function () {
	        var lotType = this();
	        return (lotType && lotType.key) ? lotType.key : (lotType || null);
	      },
	      write: function (val) {
	        this(val != undefined ?
	            rvc.lists.lotTypes.findByKey(val.key == undefined ? val : val.key) :
	            null);
	      },
	      owner: self.lotType
	    }),

	    productKey: ko.pureComputed({
	      read: function () {
	        var prod = this();
	        return (prod && prod.ProductKey) ? prod.ProductKey : (prod || null);
	      },
	      write: function (val) {
	        this(val);
	      },
	      owner: self.productKey
	    })
	  };

	  if ( params.startingLotKey ) {
	    filters.startingLotKey = self.lotKey;
	  } else {
	    filters.lotKey = self.lotKey;
	  }

	  if ( this.includeInventoryFilters ) {
	    filters.ingredientType = ko.pureComputed({
	      read: function () {
	        var ingredType = this();
	        return (ingredType && ingredType.key) ? ingredType.key : (ingredType || null);
	      },
	      write: function (val) {
	        var opts = self.ingredientTypeOptions();
	        var selectedValue = null;
	        if (Number(val)) {
	          selectedValue = ko.utils.arrayFirst(opts, function (o) {
	            return o.Key === val;
	          });
	        } else {
	          selectedValue = ko.utils.arrayFirst(opts, function (o) {
	            return o.Description === val;
	          });
	        }
	        this(selectedValue && selectedValue.Key);
	      },
	      owner: self.ingredientType
	    });

	    filters.packagingProductKey = ko.pureComputed({
	      read: function () {
	        var packaging = this();
	        return (packaging && packaging.ProductKey) ? packaging.ProductKey : (packaging || null);
	      },
	      write: function (val) {
	        this(getOptionDefaultValue(val));
	      },
	      owner: self.packagingProductKey
	    });

	    filters.treatmentKey = ko.pureComputed({
	      read: function () {
	        var treatment = this();
	        return (treatment && treatment.key != undefined) ? treatment.key : (treatment || null);
	      },
	      write: function (val) {
	        this(getOptionDefaultValue(val));
	      },
	      owner: self.treatmentKey
	    });

	    filters.warehouseLocationKey = self.warehouseLocationKey;
	    filters.locationGroupName = self.streetFilter;
	  }

	  if ( this.includeQualityControlFilters ) {
	    filters.productionStatus = this.productionStatus;
	    filters.productionStart = this.productionStart;
	    filters.productionEnd = this.productionEnd;
	    filters.qualityStatus = this.qualityStatus;
	  }

	  if ( this.includeInventoryAdjustmentFilters ) {
	    filters.beginningDateFilter = this.beginningDate;
	    filters.endingDateFilter = this.endingDate;
	  }

	  if ( this.enableFacilityFilter ) {
	    filters.warehouseKey = self.facilityKey;
	  }

	  if ( params.exports ) {
	    params.exports( filters );
	  }

	  if ( params.filters ) {
	    params.filters( filters );
	  }

	  return self;
	}

	FiltersViewModel.prototype.dispose = function () {
	  ko.utils.arrayForEach(this.disposables, this.disposeOne);
	  ko.utils.objectForEach(this, this.disposeOne);
	};

	FiltersViewModel.prototype.disposeOne = function (propOrValue, value) {
	  var disposable = value || propOrValue;

	  if (disposable && typeof disposable.dispose === "function") {
	    disposable.dispose();
	  }
	};

	module.exports = {
	  viewModel: FiltersViewModel,
	  template: __webpack_require__(120)
	};

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 119 */
/***/ (function(module, exports) {

	module.exports = {
	    init: function (target, customDisposalFn) {
	        ko.utils.extend(target.prototype, {
	            registerDisposable: function (disposable) {
	                this.__disposables = this.__disposables || [];
	                this.__disposables.push(disposable);
	            },
	            unregisterDisposable: function (disposable) {
	                this.__disposables && this.__disposables.length &&
	                    ko.utils.arrayRemoveItem(this.__disposables, disposable);
	            },
	            disposeOne: function (propOrValue, value) {
	                var disposable = value || propOrValue;

	                if (disposable && typeof disposable.dispose === "function") {
	                    disposable.dispose();
	                }
	            },
	            dispose: function () {
	                ko.utils.arrayForEach(this.__disposables, this.disposeOne);
	                ko.utils.objectForEach(this, this.disposeOne);
	                if (typeof customDisposalFn === "function") customDisposalFn();
	            }
	        });
	    }
	}

/***/ }),
/* 120 */
/***/ (function(module, exports) {

	module.exports = "<!-- ko template: uiTemplate  -->\r\n<!-- /ko -->\r\n\r\n<script id=\"filters-lotkey-only\" type=\"text/html\">\r\n  <div class=\"form-group form-group-sm\">\r\n      <label class=\"control-label sr-only\" for=\"filters-lot\">Lot</label>\r\n      <input class=\"form-control\" id=\"filters-lot\" type=\"text\" placeholder=\"Find by Lot #\" data-bind=\"value: lotKey.formattedLot, valueUpdate: 'input', disable: isAllDisabled\">\r\n  </div>\r\n</script>\r\n\r\n<script id=\"filters-base\" type=\"text/html\">\r\n  <div class=\"form-group form-group-sm\">\r\n      <label class=\"control-label sr-only\" for=\"filters-lot\">Lot</label>\r\n      <input class=\"form-control\" id=\"filters-lot\" type=\"text\" placeholder=\"Find by Lot #\" data-bind=\"value: lotKey.formattedLot, valueUpdate: 'input', disable: isAllDisabled\">\r\n  </div>\r\n  <!-- ko ifnot: includeInventoryAdjustmentFilters -->\r\n  <div class=\"form-group form-group-sm\">\r\n      <label class=\"control-label sr-only\" for=\"filters-inventory-type\">Type</label>\r\n      <select class=\"form-control\" id=\"filters-inventory-type\" name=\"Inventory Type\" data-bind=\"value: inventoryType, options: inventoryType.options, optionsText: 'key', optionsValue: 'value', disable: isDisabled\"></select>\r\n  </div>\r\n  <div class=\"form-group form-group-sm\">\r\n      <label class=\"control-label sr-only\" for=\"filters-lot-type\">Lot Type</label>\r\n      <select class=\"form-control\" id=\"filters-lot-type\" name=\"Lot Type\" data-bind=\"value: lotType, options: lotTypeOptions, optionsText: 'value', optionsCaption: '[Filter by Lot Type]', enable: enableLotTypeFilter, disable: isDisabled\"></select>\r\n  </div>\r\n  <div class=\"form-group form-group-sm\" data-bind=\"slideVisible: isInventoryChile\">\r\n      <label class=\"control-label sr-only\" for=\"filters-chile-type\">Chile Type</label>\r\n      <select class=\"form-control\" id=\"filters-chile-type\" name=\"Chile Type\" data-bind=\"value: chileType, options: chileTypeOptions, optionsText: 'value', optionsValue: 'value', optionsCaption: '[Filter by Chile Type]'\"></select>\r\n  </div>\r\n  <div class=\"form-group form-group-sm\">\r\n      <label class=\"control-label sr-only\" for=\"filters-product-type\">Product</label>\r\n      <select class=\"form-control\" id=\"filters-product-type\" name=\"Product Type\" data-bind=\"value: productKey, options: productKeyOptions, optionsValue: 'ProductKey', optionsText: 'ProductCodeAndName', optionsCaption: '[Filter by Product]', enable: enableProductFilter, disable: isDisabled\"></select>\r\n  </div>\r\n  <!-- /ko -->\r\n  <!-- ko template: { name: 'filters-inventory', if: includeInventoryFilters } -->\r\n  <!-- /ko -->\r\n  <!-- ko template: { name: 'filters-quality-control', if: includeQualityControlFilters } -->\r\n  <!-- /ko -->\r\n  <!-- ko template: { name: 'filters-inventory-adjustment', if: includeInventoryAdjustmentFilters } -->\r\n  <!-- /ko -->\r\n</script>\r\n\r\n<script id=\"filters-inventory-adjustment\" type=\"text/html\">\r\n  <div class=\"form-group\">\r\n    <label class=\"control-label\" for=\"adjustment-start-filter\">Begin Date</label>\r\n    <input type=\"text\" class=\"form-control\" id=\"adjustment-start-filter\" data-bind=\"datePickerSm: beginningDate\"/>\r\n  </div>\r\n\r\n  <div class=\"form-group\">\r\n    <label class=\"control-label\" for=\"adjustment-end-filter\">End Date</label>\r\n    <input type=\"text\" class=\"form-control\" id=\"adjustment-end-filter\" data-bind=\"datePickerSm: endingDate\"/>\r\n  </div>\r\n</script>\r\n\r\n<script id=\"filters-inventory\" type=\"text/html\">\r\n  <div class=\"form-group form-group-sm\">\r\n    <label class=\"control-label sr-only\" for=\"filters-ingredient-type\">Ingredient</label>\r\n    <select class=\"form-control\" id=\"filters-ingredient-type\" name=\"Ingredient Type\" data-bind=\"value: ingredientType, options: ingredientTypeOptions, optionsValue: 'Key', optionsText: 'Description', optionsCaption: '[Filter by Ingredient]', enable: hasIngredients, disable: isDisabled\"></select>\r\n  </div>\r\n  <div class=\"form-group form-group-sm\">\r\n    <label class=\"control-label sr-only\" for=\"filters-packaging-product\">Packaging</label>\r\n    <select class=\"form-control\" id=\"filters-packaging-product\" name=\"Packaging Product\" data-bind=\"value: packagingProductKey, options: packagingProductKeyOptions, optionsValue: 'ProductKey', optionsText: 'ProductName', optionsCaption: '[Filter by Packaging]', disable: isDisabled\"></select>\r\n  </div>\r\n  <div class=\"form-group form-group-sm\">\r\n    <label class=\"control-label sr-only\" for=\"filters-treatment-type\">Treatment</label>\r\n    <select class=\"form-control\" id=\"filters-treatment-type\" name=\"Treatment Type\" data-bind=\"value: treatmentKey, options: treatmentKey.options, optionsText: 'value', optionsValue: 'key', optionsCaption: '[All Treatments]', disable: isDisabled\"></select>\r\n  </div>\r\n  <div class=\"form-group form-group-sm\">\r\n    <label class=\"control-label sr-only\" for=\"filters-warehouse\">Warehouse</label>\r\n    <select class=\"form-control\" id=\"filters-warehouse\" name=\"Warehouse\" data-bind=\"value: facilityKey, options: facilityOptions, optionsText: 'FacilityName', optionsValue: 'FacilityKey', enable: enableFacilityFilter && getFacilityLocations.canExecute()\">\r\n    </select>\r\n  </div>\r\n  <div class=\"form-group form-group-sm\">\r\n    <label class=\"control-label sr-only\" for=\"\">Street</label>\r\n    <select class=\"form-control\" data-bind=\"value: streetFilter, options: streetFilterOptions, optionsCaption: '[Filter by Street]'\"></select>\r\n  </div>\r\n  <div class=\"form-group form-group-sm\">\r\n      <label class=\"control-label sr-only\" for=\"filters-warehouse-location\">Location</label>\r\n      <select class=\"form-control\" id=\"filters-warehouse-location\" name=\"Warehouse Location\" data-bind=\"value: warehouseLocationKey, options: warehouseLocationOptions, optionsValue: 'LocationKey', optionsText: 'Description', optionsCaption: '[All Locations]', disable: isDisabled || !getFacilityLocations.canExecute()\"></select>\r\n  </div>\r\n</script>\r\n<script id=\"filters-quality-control\" type=\"text/html\">\r\n  <div class=\"form-group form-group-sm\">\r\n    <label class=\"control-label sr-only\" for=\"production-status-select\">Production Status</label>\r\n    <select type=\"text\" class=\"form-control\" id=\"production-status-select\" data-bind=\"value: productionStatus, options: productionStatus.options, optionsText: 'value', optionsValue: 'key', optionsCaption: '[Filter by Production Status]', disable: isDisabled\"></select>\r\n  </div>\r\n\r\n  <div class=\"form-group form-group-sm\">\r\n    <label class=\"control-label sr-only\" for=\"quality-status-select\">Quality Status</label>\r\n    <select type=\"text\" class=\"form-control\" id=\"quality-status-select\" data-bind=\"value: qualityStatus, options: qualityStatus.options, optionsText: 'value', optionsValue: 'key', optionsCaption: '[Filter by Quality Status]', disable: isDisabled\"></select>\r\n  </div>\r\n\r\n  <!-- ko ifnot: isDisabled -->\r\n  <div class=\"form-group\">\r\n    <label class=\"control-label\" for=\"production-start-filter\">Production Start</label>\r\n    <input type=\"text\" class=\"form-control\" id=\"production-start-filter\" data-bind=\"datePickerSm: productionStart, disable: isDisabled\"/>\r\n  </div>\r\n\r\n  <div class=\"form-group\">\r\n    <label class=\"control-label\" for=\"production-end-filter\">Production End</label>\r\n    <input type=\"text\" class=\"form-control\" id=\"production-end-filter\" data-bind=\"datePickerSm: productionEnd, disable: isDisabled\"/>\r\n  </div>\r\n  <!-- /ko -->\r\n  <!-- ko if: isDisabled -->\r\n    <div class=\"form-group\">\r\n      <label class=\"control-label\" for=\"production-start-filter\">Production Start</label>\r\n      <input type=\"text\" class=\"form-control input-sm\" id=\"production-start-filter\" data-bind=\"disable: isDisabled\"/>\r\n    </div>\r\n\r\n    <div class=\"form-group\">\r\n      <label class=\"control-label\" for=\"production-end-filter\">Production End</label>\r\n      <input type=\"text\" class=\"form-control input-sm\" id=\"production-end-filter\" data-bind=\"disable: isDisabled\"/>\r\n    </div>\r\n  <!-- /ko -->\r\n</script>\r\n"

/***/ }),
/* 121 */,
/* 122 */,
/* 123 */,
/* 124 */,
/* 125 */,
/* 126 */,
/* 127 */,
/* 128 */,
/* 129 */,
/* 130 */,
/* 131 */,
/* 132 */,
/* 133 */,
/* 134 */,
/* 135 */,
/* 136 */,
/* 137 */,
/* 138 */,
/* 139 */,
/* 140 */,
/* 141 */,
/* 142 */,
/* 143 */,
/* 144 */,
/* 145 */,
/* 146 */,
/* 147 */,
/* 148 */,
/* 149 */,
/* 150 */,
/* 151 */,
/* 152 */,
/* 153 */,
/* 154 */,
/* 155 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {ko.components.register('inventory-picker', __webpack_require__(108));
	ko.components.register('inventory-filters', __webpack_require__(118));
	ko.components.register('picked-inventory-table', __webpack_require__(115));
	ko.components.register('auto-inventory-picker', __webpack_require__(156));
	ko.components.register('instructions-editor', __webpack_require__(158));

	var pickedItemFactory = __webpack_require__(109),
	    lotService = __webpack_require__(72);

	var PickedIngredientSummary, PickedPackagingSummary, ProductionBatch, ProductionBatchSummary;

	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [
	        __webpack_require__(160),
	        __webpack_require__(8),
	        __webpack_require__(55),
	        __webpack_require__(9),
	        __webpack_require__(44),
	        __webpack_require__(43)
	], __WEBPACK_AMD_DEFINE_RESULT__ = function (productionService, rvc, koHelpers, ko) {
	  var _selectedBatch = ko.observable(),
	      _batches = ko.observableArray([]),
	      batchDetailsCache = {},
	      instance = {};

	  // Attributes
	  var attributeNamesByProductType = ko.observable(),
	      ingredientsByProductType = ko.observable();

	  function loadAttributeNames() {
	    return lotService.getAttributeNames()
	        .done(function (data) {
	          attributeNamesByProductType(data);
	        })
	        .error(function (xhr, result, message) {
	          showUserMessage('Failed to get attribute name values.', { description: 'There was a problem loading attribute names. Please notify system administrator with the following error message: "' + message + '".', type: 'error' });
	        });
	  }

	  function loadIngredientOptions() {
	    lotService.getIngredientsByProductType()
	        .done(function (data) { ingredientsByProductType(data); })
	        .error(function (xhr, result, message) {
	          showUserMessage('Failed to get ingredient options.', { description: message, type: 'error' });
	        });
	  }

	  // Init batch VM
	  $.when(loadAttributeNames(), loadIngredientOptions())
	      .done(function () { init(instance); });

	  registerObjectConstructors();
	  return instance;

	  function setProductionBatches(input) {
	    input = input || [];
	    var targetIndex = 0, targetBatchKey;
	    setTargetBatchKey();

	    var batches = ko.utils.arrayMap(input, function (item) {
	      return item instanceof ProductionBatchSummary ?
	        item :
	          mapProductionBatchSummary(item);
	    });

	    _batches(batches);

	    return $.when.apply(null, ko.utils.arrayMap(batches, function (batch) {
	      return loadBatchAsync(batch, trySetSelectedBatch);
	    }));

	    function setTargetBatchKey() {
	      if (targetIndex + 1 > input.length) {
	        targetBatchKey = null;
	        targetIndex = -1;
	        return;
	      }
	      targetBatchKey = ko.utils.unwrapObservable(input[targetIndex].ProductionBatchKey);
	    }

	    function trySetSelectedBatch(batch) {
	      if (ko.utils.unwrapObservable(batch.ProductionBatchKey) === targetBatchKey) {
	        if (batch.ajaxWorking && batch.ajaxWorking()) return;
	        if (batch.ajaxFailure && batch.ajaxFailure()) {
	          targetIndex++;
	          setTargetBatchKey();
	          if (targetBatchKey) trySetSelectedBatch(input[targetIndex]);
	          return;
	        }

	        instance.SelectedProductionBatch(batchDetailsCache[targetBatchKey]);
	      }
	    }
	  }

	  function loadBatchAsync(batch, complete) {
	    var batchSummary = batch instanceof ProductionBatchSummary ?
	      batch :
	        getProductionBatchSummaryItemByKey(batch);

	    if (!batchSummary) return $.Deferred().resolve();

	    batchSummary.indicateWorking();

	    var batchKey = ko.utils.unwrapObservable(batchSummary.ProductionBatchKey);
	    return productionService.getProductionBatchDetails(batchKey)
	        .done(function (data) {
	          var batchDetails = mapProductionBatch(data);
	          batchDetailsCache[batchKey] = batchDetails;
	          batchSummary.indicateSuccess();
	        })
	        .error(function () {
	          batchSummary.indicateFailure();
	          console.log(arguments);
	        })
	        .always(function () {
	          complete && complete(batch);
	        });
	  }

	  function mapProductionBatch(data, isNew) {
	    var batch = new ProductionBatch(data);
	    batch.OverrideLotKey = ko.observable().extend({ lotKey: true });
	    batch.NumberOfPackagingUnits(computePackagingUnitsForWeight(batch.BatchTargetWeight()));
	    batch.Notebook = ko.observable(data.InstructionsNotebook);

	    koHelpers.esmHelper(batch, {
	      isInitiallyDirty: isNew,
	      ignore: ['PickedInventoryItems', 'PickedChileInputs', 'PickedAdditiveIngredients', 'PackagingMaterialSummaries', 'Notebook']
	    });

	    // subscribers
	    batch.BatchTargetWeight.subscribe(function (val) {
	      batch.NumberOfPackagingUnits(computePackagingUnitsForWeight(val));
	    });

	    return batch;
	  }

	  function mapProductionBatchSummary(data) {
	    var batch = new ProductionBatchSummary(data);
	    batch.isSelected = ko.computed(function () {
	      var selectedBatch = instance.SelectedProductionBatch();
	      return selectedBatch && this.ProductionBatchKey() === selectedBatch.ProductionBatchKey();
	    }, batch);

	    koHelpers.ajaxStatusHelper(batch);
	    return batch;
	  }

	  function getProductionBatchSummaryItemByKey(batchKey) {
	    return ko.utils.arrayFirst(_batches(), function (b) {
	      if (b.ProductionBatchKey() === batchKey) {
	        return true;
	      }
	      return false;
	    });
	  }

	  function setSelectedBatchFromClickEvent() {
	    try {
	      var batch = koHelpers.getDataForClickedElement({
	        clickArguments: arguments,
	        isDesiredTarget: function (obj) {
	          return obj instanceof ProductionBatchSummary;
	        }
	      });
	      if (batch) {
	        setSelectedBatchFromSummaryItem(batch);
	      }
	    } catch (ex) {
	      return;
	    }
	  }

	  function setSelectedBatchFromSummaryItem(summary) {
	    instance.SelectedProductionBatch(summary == undefined ? null : batchDetailsCache[summary.ProductionBatchKey()]);
	  }

	  function insertNewBatchIntoUI(batch) {
	    var batchDetail = batchDetailsCache[batch.ProductionBatchKey()];
	    batch.OutputLotKey(ko.utils.unwrapObservable(batchDetail.OutputLotKey));
	    _selectedBatch(batchDetail);
	  }

	  function computePackagingUnitsForWeight(targetWeight) {
	    var wgt = parseFloat(
	      );
	    var capacity = instance.defaultPackagingCapacity();
	    if (isNaN(wgt) || isNaN(capacity)) return NaN;
	    else if (capacity < 0.001) return 0;
	    else return parseInt(wgt / capacity);
	  }

	  function handleNotesClick() {
	    var note;

	    try {
	      note = koHelpers.getDataForClickedElement({
	        clickArguments: arguments,
	        isDesiredTarget: function (obj) {
	          return obj instanceof Note;
	        }
	      });
	    } catch (ex) {
	      return;
	    }

	    if (note) {
	      note.editCommand.execute();
	      instance.SelectedNote(note);
	    }
	  }

	  function loadProductionBatchInstructions() {
	    productionService.getBatchInstructionOptions()
	        .then(function (data) {
	          instance.BatchInstructionOptions(data);
	        })
	        .fail(function () {
	          showUserMessage("Failed to get production batch instruction options.");
	        });
	  }

	  function init(target) {
	    var pickedInventoryItems = ko.observableArray([]);

	    target.inventoryTypes = [];

	    target.pickInventory = ko.observable(false);
	    target.isLocked = ko.observable();
	    target.filtersInput = ko.observable();
	    target.filtersExports = ko.observable();

	    target.autoPickerVm = ko.observable();
	    target.inventoryPicker = ko.observable();
	    target.pickedInventoryExports = ko.observable();
	    target.cancelPickText = ko.pureComputed(function () {
	      var isChanged = target.inventoryPicker() && target.inventoryPicker()().isDirty();
	      return isChanged ? 'Cancel Picked Changes' : 'Back to Batch';
	    });
	    target.totalWeightPicked = ko.pureComputed(function () {
	      var weight = 0,
	        inventoryPicker = target.inventoryPicker() && target.inventoryPicker()(),
	        items = inventoryPicker ?
	          ko.unwrap(inventoryPicker.pickedItems) :
	          null;

	      if (items) {
	        ko.utils.arrayForEach(items, function (item) {
	          if (ko.unwrap(item.QuantityPicked) > 0) {
	            weight += ko.unwrap(item.WeightPicked);
	          }
	        });
	      }

	      return weight;
	    });

	    target.totalWeight = ko.pureComputed(function () {
	      var totalWeight = 0;
	      var options = ko.unwrap(target.pickedInventoryOptions);

	      ko.utils.arrayForEach(options, function (opt) {
	        totalWeight += calcWeight(ko.unwrap(opt.inventoryItems));
	      });

	      function calcWeight(items) {
	        var weight = 0;

	        ko.utils.arrayForEach(items, function (item) {
	          if (item.hasOwnProperty('WeightPicked')) {
	            weight += ko.unwrap(item.WeightPicked);
	          }
	        });

	        return weight;
	      }

	      return totalWeight;
	    });
	        
	    target.PackScheduleKey = ko.observable();
	    target.defaultBatchTargetWeight = ko.numericObservable(0);
	    target.defaultBatchTargetAsta = ko.numericObservable(0);
	    target.defaultBatchTargetScoville = ko.numericObservable(0);
	    target.defaultBatchTargetScan = ko.numericObservable(0);
	    target.defaultPackagingCapacity = ko.numericObservable(0);
	    target.batchPackagingDescription = ko.observable();
	    target.defaultPackaging = ko.observable();
	    target.Customer = ko.observable();
	    target.batchProductKey = ko.observable();
	    target.ProductionBatches = ko.computed({
	      read: function () { return _batches(); }
	    });
	    target.SelectedProductionBatch = ko.computed({
	      read: function () { return _selectedBatch(); },
	      write: function (value) {
	        var currentBatch = _selectedBatch.peek();
	        if (currentBatch && currentBatch.hasChanges()) {
	          showUserMessage('Save production batch changes?', {
	            description: 'The current production batch has unsaved changes. Would you like to save these changes before navigating away from the batch? Click <strong>Yes</strong> to save the batch and continue. Click <strong>No</strong> to undo the changes and continue. Or, click <strong>Cancel</strong> to keep the batch changes without saving.',
	            type: 'yesnocancel',
	            onYesClick: function () {
	              target.saveProductionBatchCommand.execute(function () {
	                setBatch(value);
	              });
	            },
	            onNoClick: function () {
	              currentBatch.cancelEditsCommand.execute();
	              setBatch(value);
	            },
	            onCancelClick: function () { }
	          });
	        } else {
	          setBatch(value);
	        }

	        function setBatch(batchValue) {
	          if (batchValue == undefined) return _selectedBatch(null);
	          else if (!(batchValue instanceof ProductionBatch))
	            return setBatch(mapProductionBatch(batchValue));

	          _selectedBatch(batchValue);

	          var batchKey = ko.utils.unwrapObservable(batchValue.ProductionBatchKey);
	        }
	      }
	    });
	    target.SelectedNote = ko.observable();
	    target.BatchInstructionOptions = ko.observableArray([]);

	    target.pickedInventoryItems = ko.pureComputed(function () {
	      return pickedInventoryItems() || [];
	    });

	    rvc.helpers.forEachInventoryType(function (invType) {
	      var invObj = {};

	      for (var i = 0, list = Object.getOwnPropertyNames(invType), max = list.length; i < max; i += 1) {
	        invObj[list[i]] = invType[list[i]];
	      }

	      invObj.items = ko.pureComputed(function () {
	        var items = target.pickedInventoryItems(),
	        filteredItems = items ?
	            ko.utils.arrayFilter(items, function (item) {
	              return ko.unwrap(item.Product.ProductType) === invObj.key;
	            }) :
	            [];

	        return filteredItems;
	      });
	      target.inventoryTypes.push(invObj);
	    });

	    target.setProductionBatches = setProductionBatches;
	    target.selectBatch = setSelectedBatchFromClickEvent;
	    target.handleNotesClick = handleNotesClick;
	    target.targetProductName = ko.observable();
	    target.targetProduct = ko.observable();
	    target.currentBatchTargetWeight = ko.computed(function () {
	      var batch = target.SelectedProductionBatch();
	      return batch && batch.BatchTargetWeight();
	    });
	    target.setBatchTargets = function (productInfo) {
	      rvc.helpers.forEachInventoryType(function (type) {
	        var attributes = attributeNamesByProductType()[type.key];
	        if (!attributes || !attributes.length) {
	          return;
	        }

	        target.targetProductName(ko.unwrap(productInfo.ProductNameFull));
	        target.targetProduct(ko.unwrap(productInfo));

	        ko.utils.arrayForEach(attributes, function (attribute) {
	          var attributeName = findTargetByAttributeKey(attribute.Key) || {};

	          if (!ko.isObservable(attribute.minTargetValue)) {
	            attribute.minTargetValue = ko.observable(attributeName.MinValue);
	          } else {
	            attribute.minTargetValue(attributeName.MinValue);
	          }

	          if (!ko.isObservable(attribute.maxTargetValue)) {
	            attribute.maxTargetValue = ko.observable(attributeName.MaxValue);
	          } else {
	            attribute.maxTargetValue(attributeName.MaxValue);
	          }
	        });
	      });

	      function findTargetByAttributeKey(key) {
	        return ko.utils.arrayFirst(ko.unwrap(productInfo.AttributeRanges), function (target) {
	          return target.AttributeNameKey === key;
	        });
	      }
	    };
	    target.hasPickedInventoryChanges = ko.pureComputed(function () {
	      return target.inventoryPicker() && target.inventoryPicker()().isDirty();
	    });

	    target.canPickPackaging = ko.computed(function () {
	      return !target.hasPickedInventoryChanges();
	    });
	    target.cleanup = function () {
	      batchDetailsCache = {};
	      _batches([]);
	      _selectedBatch(null);
	    };

	    // computed properties
	    target.defaultNumberOfPackagingUnits = ko.computed(function () {
	      return computePackagingUnitsForWeight(target.defaultBatchTargetWeight());
	    });

	    var isModalShowing = false;
	    target.showNoteEditor = ko.computed({
	      read: function() {
	        var note = target.SelectedNote();

	        return !!(note && note.isEditing());
	      },
	      write: function(value) {
	        target.SelectedNote(!!(value) ? value : null);
	      }
	    });

	    //#region commands
	    target.getProductionBatchDetailsCommand = ko.asyncCommand({
	      execute: function (batchKey, complete) {
	        if (batchKey && typeof batchKey === "object") batchKey = ko.utils.unwrapObservable(batchKey.ProductionBatchKey);
	        if (!batchKey) return;
	        loadBatchAsync(batchKey, complete);
	      },
	      canExecute: function (isExecuting) { return !isExecuting; }
	    });
	    target.isNewBatch = ko.observable(false);
	    target.initializeNewBatchCommand = ko.command({
	      execute: function () {
	        var batches = _batches();
	        var instructionsNotebook = null, batchNotes = '';
	        if (batches.length > 0) {
	          var lastBatchKey = ko.utils.unwrapObservable(batches[batches.length - 1].ProductionBatchKey);
	          var lastBatch = batchDetailsCache[lastBatchKey];
	          instructionsNotebook = lastBatch.Notebook() || [];
	          batchNotes = lastBatch.Notes;
	        }

	        var newBatch = mapProductionBatch({
	          OutputLotKey: '',
	          BatchTargetWeight: target.defaultBatchTargetWeight(),
	          BatchTargetAsta: target.defaultBatchTargetAsta(),
	          BatchTargetScoville: target.defaultBatchTargetScoville(),
	          BatchTargetScan: target.defaultBatchTargetScan(),
	          NumberOfPackagingUnits: target.defaultNumberOfPackagingUnits(),
	          Notes: batchNotes,
	          InstructionsNotebook: instructionsNotebook,
	        }, true);
	        newBatch.isNew = true;
	        newBatch.PackScheduleKey = target.PackScheduleKey();
	        newBatch.beginEditingCommand.execute();
	        target.SelectedProductionBatch(newBatch);
	        target.isNewBatch(true);
	      },
	      canExecute: function () {
	        return true;
	      }
	    });
	    target.saveProductionBatchCommand = ko.asyncCommand({
	      execute: function (successCallback, complete) {
	        var batch = target.SelectedProductionBatch.peek();
	        // todo: check validation before processing

	        var data = ko.toJS(batch);
	        var dfd = $.Deferred();
	        dfd.always(function () { complete(); });

	        data.isNew ?
	            insertNewProductionBatchAsync() :
	            updateProductionBatchAsync();

	        return dfd.promise();

	        function insertNewProductionBatchAsync() {
	          var lastBatch = getLastBatch(),
	              batchDetails;

	          if (lastBatch) {
	            batchDetails = batchDetailsCache[ko.unwrap(lastBatch.ProductionBatchKey)];
	            data.Instructions = ko.utils.arrayMap(lastBatch.Notebook().Notes, function (note) {
	              return note.Text;
	            });
	          }

	          // allow lot number overrides 
	          if (batch.OverrideLotKey.isComplete()) {
	            data.LotType = batch.OverrideLotKey.LotType();
	            data.LotDateCreated = batch.OverrideLotKey.formattedDate();
	            data.LotSequence = batch.OverrideLotKey.Sequence();
	          }

	          productionService.createProductionBatch(data)
	              .done(function (response) {
	                data.ProductionBatchKey = response.ProductionBatchKey;
	                var batch = mapProductionBatchSummary(data);
	                _batches.push(batch);
	                typeof successCallback === "function" && successCallback();

	                return loadBatchAsync(batch)
	                    .done(function () {
	                      insertNewBatchIntoUI(batch);

	                      if (lastBatch) {
	                        var pickSuggestions = ko.toJS(batchDetails.PickedInventoryItems);
	                        ko.utils.arrayForEach(pickSuggestions, function (item) {
	                          item.Quantity -= item.QuantityPicked;
	                        });
	                        pickedInventoryItems(pickSuggestions);
	                        target.beginPickInventoryCommand.execute();
	                      }

	                      dfd.resolve();
	                    })
	                    .error(function () {
	                      showUserMessage('The batch was saved successfully but an error occurred while attempting to update the screen. Please refresh to page to see new batch.');
	                      dfd.reject();
	                    });
	              })
	              .error(function (xhr, status, message) {
	                showUserMessage('Failed to create new production batch', { description: message, mode: 'error' });
	                dfd.reject();
	              });
	        }

	        function getLastBatch() {
	          var batches = _batches();
	          if (!(batches && batches.length)) return null;

	          var lastBatchKey = ko.utils.unwrapObservable(batches[batches.length - 1].ProductionBatchKey);
	          var lastBatch = batchDetailsCache[lastBatchKey];
	          return lastBatch;
	        }

	        function updateProductionBatchAsync() {
	          productionService.updateProductionBatch(data.ProductionBatchKey, data, {
	            successCallback: function () {
	              showUserMessage('Batch <strong>' + data.OutputLotKey + '</strong> was updated successfully.');
	              batch.saveEditsCommand.execute();
	              batch.endEditingCommand.execute();
	              typeof successCallback === "function" && successCallback();
	              dfd.resolve();
	            },
	            errorCallback: function (xhr, status, message) {
	              showUserMessage('Batch <strong>' + data.OutputLotKey + '</strong> failed to save.', { description: message, mode: 'error' });
	              dfd.reject();
	            }
	          });
	        }
	      },
	      canExecute: function (isExecuting) {
	        return !isExecuting && !!(target.SelectedProductionBatch()) && (target.SelectedProductionBatch().hasChanges());
	      }
	    });
	    function updateBatch() {
	      var batch = target.SelectedProductionBatch.peek(),
	          batchKey = ko.utils.unwrapObservable(batch.ProductionBatchKey);

	      return productionService.getProductionBatchDetails(batchKey)
	          .done(function (batchData) {
	            var batch = target.SelectedProductionBatch();

	            batch.setInputMaterialSummary(batchData);
	            batch.PickedInventoryItems = batchData.PickedInventoryItems.map(function (item) {
	              if (item.AstaCalc) {
	                item.Attributes.push({
	                  AttributeDate: item.Attributes[0].AttributeDate,
	                  Computed: false,
	                  Defect: undefined,
	                  Key: "AstaC",
	                  Name: "AstaC",
	                  Value: item.AstaCalc
	                });
	              }

	              item.isInitiallyPicked = true;
	              item.isPicked = true;
	              return item;
	            });

	            // Update cache after save
	            batchDetailsCache[batchKey] = batch;
	            setSelectedBatchFromSummaryItem(batch);
	          })
	          .fail(function (xhr, result, message) {
	            showUserMessage(
	                'Failed to get the updated picked inventory summary for the batch.',
	                {
	                  description: 'Picked inventory items were saved successfully but we were unable to refresh the summary of input materials on the production batch. If you would like to see the updated summary data, please refresh the page.'
	                });
	          });
	    }
	    function savePick(callback) {
	      var picker = target.inventoryPicker(),
	      save;

	      if (picker) {
	        target.isPickerSaving(true);
	        save = picker &&
	            picker().saveCommand.execute();

	        if (save) {
	          save.done(function () {
	            updateBatch()
	                .done(function () {
	                  target.pickInventory(false);
	                  target.isPickerSaving(false);
	                }).fail(function () {
	                  location.reload();
	                });
	          })
	              .fail(function (xhr, status, message) {
	                showUserMessage('Save Failed', { description: message });
	                target.isPickerSaving(false);
	              })
	              .always(function () {
	                if (callback) {
	                  callback();
	                }
	                target.isNewBatch(false);
	              });
	        } else {
	          save = $.Deferred();
	          showUserMessage('Save failed', { description: 'Please enter valid quantities for picked items.' });
	          target.isPickerSaving(false);
	          if (callback) {
	            callback();
	          }

	          save.reject();
	        }
	        return save;
	      }
	    }
	    target.isPickerSaving = ko.observable(false);
	    target.savePickedInventoryCommand = ko.asyncCommand({
	      execute: function (complete) {
	        savePick(complete);
	      },
	      canExecute: function (isExecuting) {
	        return !isExecuting && (target.hasPickedInventoryChanges() || target.isNewBatch());
	      }
	    });
	    target.cancelProductionBatchEditsCommand = ko.command({
	      execute: function () {
	        var batch = target.SelectedProductionBatch();

	        target.isNewBatch(false);

	        if (batch.isNew) {
	          _selectedBatch(null);
	          var batches = _batches() || [];
	          setSelectedBatchFromSummaryItem(batches[0]);
	        } else {
	          batch.cancelEditsCommand.execute();
	        }
	      },
	      canExecute: function () {
	        return target.SelectedProductionBatch() != undefined;
	      }
	    });
	    target.isPickerDeleting = ko.observable(false);
	    target.deleteProductionBatchCommand = ko.asyncCommand({
	      execute: function (complete) {
	        var currentBatch = target.SelectedProductionBatch();
	        var batchKey = currentBatch.ProductionBatchKey();
	        var deleteLot = currentBatch.OutputLotKey();
	        var dfd = $.Deferred();

	        showUserMessage('Delete Production Batch <strong>' + deleteLot + '</strong>?', {
	          type: 'yesno',
	          onYesClick: doDelete,
	          onNoClick: function () { complete(); dfd.resolve(); },
	        });

	        return dfd.promise();

	        function doDelete() {
	          target.isPickerDeleting(true);
	          productionService.deleteProductionBatch(batchKey)
	              .then(function () {
	                var summaryItem = getProductionBatchSummaryItemByKey(batchKey);
	                var index = ko.utils.arrayIndexOf(target.ProductionBatches(), summaryItem);
	                if (index > -1) {
	                  _batches.splice(index, 1);
	                }

	                delete batchDetailsCache[batchKey];

	                var remainingSummaries = _batches();
	                //target.SelectedProductionBatch(remainingSummaries[0] || null);
	                setSelectedBatchFromSummaryItem(remainingSummaries[0]);

	                showUserMessage('Production Batch Deleted', { description: 'Batch was successfully deleted for Lot <strong>' + deleteLot + '</strong>. All related data has been deleted and any picked inventory has been returned.' });
	                dfd.resolve();
	              }).fail(function (xhr, status, message) {
	                showUserMessage('Failed to delete batch.', { description: message, mode: 'error' });
	                dfd.reject();
	              }).always(function () {
	                complete();
	                target.isPickerDeleting(false);
	              });
	        }

	      },
	      canExecute: function (isExecuting) {
	        return !isExecuting && !!target.SelectedProductionBatch();
	      }
	    });

	    target.pickingContextKey = ko.observable('');

	    target.pickedInventoryOptions = [];

	    // Build Picked Items tab's data
	    var mappedPickedItems = pickedInventoryItems.map(pickedItemFactory);

	    function buildPickedInventoryOptions( data ) {
	      target.pickedInventoryOptions.push({
	        inventoryTypeName: "Inputs",
	        isReadOnly: true,
	        inventoryItems: mappedPickedItems.filter(function (i) {
	          return ko.unwrap(i.Product.ProductType) !== rvc.lists.inventoryTypes.Packaging.key;
	        }),
	        targetProduct: target.targetProduct,
	        attributes: attributeNamesByProductType()[rvc.lists.inventoryTypes.Chile.key] || []
	      });
	      target.pickedInventoryOptions.push({
	        inventoryTypeName: rvc.lists.inventoryTypes.Packaging.value,
	        isReadOnly: true,
	        inventoryItems: mappedPickedItems.filter(function (i) {
	          return ko.unwrap(i.Product.ProductType) === rvc.lists.inventoryTypes.Packaging.key;
	        }),
	        targetProduct: null,
	        attributes: attributeNamesByProductType()[rvc.lists.inventoryTypes.Packaging.key] || []
	      });
	    }

	    buildPickedInventoryOptions();

	    mappedPickedItems.subscribe(function( data ) {
	      target.pickedInventoryOptions = [];
	      buildPickedInventoryOptions();
	    });

	    // Init inventory picker component
	    var _customerKey = ko.pureComputed(function() {
	      var _customer = target.Customer();

	      return _customer && _customer.CompanyKey;
	    });

	    target.inventoryPickerOpts = {
	      pickingContext: rvc.lists.inventoryPickingContexts.ProductionBatch,
	      pickingContextKey: target.pickingContextKey,
	      pickedInventoryItems: pickedInventoryItems,
	      pageSize: 50,
	      filters: target.filtersExports,
	      targetProduct: target.targetProduct,
	      customerLotCode: ko.observable(),
	      customerProductCode: ko.observable(),
	    };
	    target.checkOutOfRange = function( key, value ) {
	      var product = ko.unwrap( target.inventoryPickerOpts.targetProduct );
	      var targetRanges = product && product.AttributeRanges || [];

	      var matchedKey = ko.utils.arrayFirst( targetRanges, function( attr ) {
	        return attr.AttributeNameKey === key;
	      });

	      if ( matchedKey ) {
	        if ( value < matchedKey.MinValue ) {
	          return -1;
	        } else if ( value > matchedKey.MaxValue ) {
	          return 1;
	        }
	      }

	      return 0;
	    };
	    target.pickedInventoryInput = {
	      PickedInventoryItems: ko.computed(function () {
	        var batch = target.SelectedProductionBatch();
	        return batch && {
	          PickedInventoryItems: batch.PickedInventoryItems
	        };
	      }),
	      PickedInventoryKey: target.pickingContextKey
	    };

	    ko.computed(function () {
	      if (!target.SelectedProductionBatch()) { return; }

	      var batch = target.SelectedProductionBatch(),
	          batchKey = (batch && ko.unwrap(batch.ProductionBatchKey)) || null,
	          batchDetails = batchDetailsCache[batchKey];

	      target.pickingContextKey(batchKey);

	      if (!batchDetails) { return; }


	      pickedInventoryItems(batchDetails.PickedInventoryItems || []);

	      return batchDetails;
	    });

	    target.isAutoPickerWorking = ko.pureComputed(function () {
	      var picker = target.autoPickerVm();
	      return picker && picker.getInventoryPicks.isExecuting();
	    });
	    target.isPickerInit = ko.pureComputed(function () {
	      var picker = target.inventoryPicker();
	      return picker && picker().isInit();
	    });
	    target.isPickerWorking = ko.pureComputed(function () {
	      var picker = target.inventoryPicker();
	      return picker && picker().isWorking();
	    });
	    target.beginPickInventoryCommand = ko.command({
	      execute: function (opts) {
	        var dfd = $.Deferred();

	        target.pickInventory(true);

	        var computed = ko.computed(function () {
	          var picker = target.inventoryPicker();
	          var filters = target.filtersExports();

	          if (picker && picker().isInit() && filters) {
	            setInventoryFilters();
	            target.loadInventoryCommand.execute();
	            computed && computed.dispose();
	            dfd.resolve();
	          }
	        });

	        function setInventoryFilters() {
	          if (!opts) {
	            return;
	          }

	          var filtersVm = target.filtersExports();
	          if (!filtersVm) return;

	          filtersVm.inventoryType(opts.InventoryType);
	          filtersVm.lotType(opts.LotType);
	          filtersVm.productKey(opts.IngredientName === "Finished Goods" ?
	                               target.batchProductKey() :
	                               opts.ProductKey && opts.ProductKey.key);

	          filtersVm.ingredientType( null );

	          if ( rvc.lists.inventoryTypes.Additive === opts.InventoryType ) {
	            filtersVm.ingredientType( opts.IngredientName );
	          }
	        }

	        return dfd;
	      },
	      canExecute: function () {
	        return target.SelectedProductionBatch() != undefined && !target.isLocked() && !target.pickInventory();
	      }
	    });
	    target.loadInventoryCommand = ko.asyncCommand({
	      execute: function (complete) {
	        var picker = target.inventoryPicker();
	        return picker().loadInventoryItemsCommand.execute()
	            .always(complete);
	      },
	      canExecute: function (isExecuting) {
	        if (isExecuting) return;
	        var picker = target.inventoryPicker();
	        return picker && picker().loadInventoryItemsCommand.canExecute();
	      }
	    });
	    target.cancelPickingInventoryCommand = ko.command({
	      execute: function () {
	        if (target.hasPickedInventoryChanges()) {
	          showUserMessage('Do you want to save your changes?',
	          {
	            description: 'You have unsaved picked inventory changes. Would you like to save your changes before closing the inventory picking view? Click <strong>Yes</strong> to save your changes, <strong>No</strong> to undo changes, or <strong>Cancel</strong> to review your changes before deciding.',
	            type: 'yesnocancel',
	            onYesClick: function () {
	              savePick(executeCancel);
	            },
	            onNoClick: executeCancel,
	          });
	        } else executeCancel();

	        function executeCancel() {
	          target.pickInventory(false);
	          target.inventoryPicker()().revertCommand.execute();
	        }
	      },
	      canExecute: function () {
	        return target.SelectedProductionBatch() != undefined;
	      }
	    });
	    //#endregion

	    instance.rebuild = rebuild.bind(instance);

	    koHelpers.ajaxStatusHelper(target.savePickedInventoryCommand);
	    loadProductionBatchInstructions();


	    ko.postbox.subscribe('AutoPickedItemsSaved', updateBatch);
	  }

	  function rebuild() {
	    _selectedBatch(null);
	    _batches([]);
	    batchDetailsCache = {};

	    for (var prop in this) {
	      if (this.hasOwnProperty(prop)) {
	        delete this[prop];
	      }
	    }
	    init(this);
	  }
	  function registerObjectConstructors() {
	    ProductionBatchSummary = function (input) {
	      if (!(this instanceof ProductionBatchSummary)) return new ProductionBatchSummary(input);

	      var values = ko.toJS(input);
	      this.ProductionBatchKey = ko.observable(values.ProductionBatchKey);
	      this.OutputLotKey = ko.observable(values.OutputLotKey);
	      this.BatchTargetAsta = ko.numericObservable(values.BatchTargetAsta, 0);
	      this.BatchTargetScan = ko.numericObservable(values.BatchTargetScan, 0);
	      this.BatchTargetScoville = ko.numericObservable(values.BatchTargetScoville, 0);
	      this.BatchTargetWeight = ko.numericObservable(values.BatchTargetWeight);
	      this.HasProductionBeenCompleted = ko.observable(values.HasProductionBeenCompleted);
	      this.PackagingProduct = values.PackagingProduct;
	      this.NumberOfPackagingUnits = ko.numericObservable(values.NumberOfPackagingUnits);
	      return this;
	    };

	    ProductionBatch = function (input) {
	      if (!(this instanceof ProductionBatch)) return new ProductionBatch(input);

	      var values = ko.toJS(input) || {};

	      this.isNew = !values.ProductionBatchKey;
	      this.ProductionBatchKey = ko.observable(values.ProductionBatchKey);
	      this.OutputLotKey = ko.observable(values.OutputLotKey);
	      this.BatchTargetAsta = ko.numericObservable(values.BatchTargetAsta).extend({ min: 0 });
	      this.BatchTargetScan = ko.numericObservable(values.BatchTargetScan).extend({ min: 0 });
	      this.BatchTargetScoville = ko.numericObservable(values.BatchTargetScoville).extend({ min: 0 });
	      this.BatchTargetWeight = ko.numericObservable(values.BatchTargetWeight).extend({ min: 0 });
	      this.HasProductionBeenCompleted = ko.observable(values.HasProductionBeenCompleted);
	      this.PackagingProduct = values.PackagingProduct;
	      this.NumberOfPackagingUnits = ko.numericObservable(values.NumberOfPackagingUnits);
	      this.Notes = ko.observable(values.Notes);
	      this.sumTargetWeight = 0;
	      this.sumWeightPicked = 0;
	      this.status = this.isNew ?
	          'Creating' :
	          values.HasProductionBeenCompleted ? 'Produced' : 'Not Produced';

	      input.PickedInventoryItems = input.PickedInventoryItems || [];
	      this.PickedInventoryItems = input.PickedInventoryItems.map(function (item) {
	        if (item.AstaCalc) {
	          item.Attributes.push({
	            AttributeDate: item.Attributes[0].AttributeDate,
	            Computed: false,
	            Defect: undefined,
	            Key: "AstaC",
	            Name: "AstaC",
	            Value: item.AstaCalc
	          });
	        }

	        item.isInitiallyPicked = true;
	        item.isPicked = true;
	        return item;
	      });

	      this.title = values.OutputLotKey || 'New Production Batch';

	      this.PickedAdditiveIngredients = ko.observableArray([]);
	      this.PickedChileInputs = ko.observableArray([]);

	      this.PackagingMaterialSummaries = ko.computed(function () {
	        var _items = {};
	        var items = [];
	        var pickedPackagingItems = instance.pickedInventoryItems().filter(function (item) {
	          return ko.unwrap(item.Product.ProductType) === 5 && ko.unwrap(item.isPicked);
	        }).forEach(function (item) {
	          var index = _items[item.Product.ProductKey];
	          if (!isNaN(index)) {
	            var existing = items[index];
	            var newSum = existing.QuantityPicked() + ko.unwrap(item.QuantityPicked);
	            existing.QuantityPicked(newSum);
	          } else {
	            items.push(new PickedPackagingSummary(item));
	            _items[item.Product.ProductKey] = items.length - 1;
	          }
	        });
	        return items;
	      }, this);

	      this.packagingTotal = ko.computed(function () {
	        var sum = 0;
	        this.PackagingMaterialSummaries().forEach(function (item) {
	          sum += item.QuantityPicked();
	        });
	        return sum;
	      }, this);

	      this.setInputMaterialSummary = setInputMaterialsSummaryFields.bind(this);
	      if (this.isNew)
	        instance.pickedInventoryItems.subscribe(function () {
	          this.setInputMaterialSummary(values);
	        }, this);
	      this.setInputMaterialSummary(values, true);

	      return this;
	    };

	    function setInputMaterialsSummaryFields(batchData, first) {
	      this.PickedAdditiveIngredients(loadPickedIngredients(batchData.AdditiveIngredients));
	      this.PickedChileInputs(batchData.WipMaterialsSummary && batchData.FinishedGoodsMaterialsSummary ?
	          loadPickedIngredients([batchData.WipMaterialsSummary, batchData.FinishedGoodsMaterialsSummary]) :
	          []);
	      this.hasPickedPackaging = ko.computed(function () {
	        return this.PackagingMaterialSummaries().length > 0;
	      }, this);

	      if (first)
	        computeSums.apply(this);

	      function loadPickedIngredients(ingredientValues) {
	        return ko.utils.arrayMap(ingredientValues, function (ing) {
	          return new PickedIngredientSummary(ing);
	        }) || [];
	      }

	      function computeSums() {
	        var batch = this;
	        var source = batch.PickedAdditiveIngredients().concat(batch.PickedChileInputs());

	        ko.utils.arrayMap(source, function (item) {
	          batch.sumTargetWeight += item.TargetWeight;
	          batch.sumWeightPicked += item.WeightPicked;
	        });
	      }
	    }


	    PickedIngredientSummary = function (values) {
	      if (!(this instanceof PickedIngredientSummary)) return new PickedIngredientSummary(values);

	      this.InventoryType = rvc.lists.inventoryTypes.findByKey(values.InventoryType);
	      this.LotType = rvc.lists.lotTypes.findByKey(values.LotType);
	      this.IngredientName = values.IngredientName;
	      this.WeightPicked = values.WeightPicked;
	      this.TargetWeight = parseInt(values.TargetWeight) || 0;
	      this.TargetPercentage = parseFloat((values.TargetPercentage || 0) * 100, 2).toFixed(2);
	      this.PercentOfPicked = parseFloat((values.PercentOfPicked || 0) * 100, 2).toFixed(2);
	      this.TargetPercentageDisplay = this.TargetPercentage + "%";
	      this.PercentOfPickedDisplay = this.PercentOfPicked + "%";

	      return this;
	    };

	    PickedPackagingSummary = function (values) {
	      if (!(this instanceof PickedPackagingSummary)) return new PickedPackagingSummary(values);

	      this.InventoryType = rvc.lists.inventoryTypes.findByKey(5);
	      this.LotType = rvc.lists.lotTypes.findByKey(values.LotType || rvc.lists.lotTypes.fromLotKey(values.LotKey));
	      this.PackagingDescription = values.PackagingDescription || 'No Packaging';
	      this.ProductName = values.Product.ProductName;
	      this.QuantityPicked = ko.observable(ko.unwrap(values.QuantityPicked) || 1);
	      this.TotalQuantityNeeded = values.TotalQuantityNeeded;
	      this.QuantityRemainingToPick = values.QuantityRemainingToPick;
	      this.ProductKey = values.Product.ProductKey;

	      return this;
	    };
	    PickedPackagingSummary.create = function (values) {
	      return new PickedPackagingSummary(values);
	    };
	  }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 156 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {var rvc = __webpack_require__(8),
	    productService = __webpack_require__(24),
	    inventoryService = __webpack_require__(77);

	/**
	* auto-inventory-picker
	* 
	* (All params optional)
	*  
	* @param enable         {Boolean}  Enable `Pick` button (default=true).
	* @param pickedItems    {array}    Existing picked inventory for lot. IMPORTANT: omitting this property will cause existing items to be unpicked!
	* @param inventoryType  {Key}      Setting this makes it read-only.
	* @param product        {Key}      Default value for `product` input.
	* @param quantity       {Integer}  Default value for `quantity` input
	* @param sortFn         {Function} Function to sort inventory by.
	* @param onSuccess      {Function} Success callback.
	* @param onFail         {Function} Error callback.
	* @param pickingContext {string}   REQUIRED. Determines route param value.
	* @param lotKey         {string}   REQUIRED. The lot for which inventory will be picked.
	*/
	function AutoInventoryPicker(params) {
	    if (!(this instanceof AutoInventoryPicker)) { return new AutoInventoryPicker(params); }

	    var self = this;

	    this.sortFn = params.sortFn || sortByDate;
	    this.staticType = params.inventoryType;
	    this.enablePick = params.enable || ko.observable(true);
	    this.lotKey = ko.pureComputed(function() {
	        return ko.unwrap(params.lotKey);
	    });
	    this.pickingContext = ko.pureComputed(function() {
	        return ko.unwrap(params.pickingContext);
	    });
	    this.existingPickedItems = ko.pureComputed(function() {
	        return ko.unwrap(params.pickedItems);
	    });
	    this.isSaving = ko.observable(false);

	    // Inputs
	    this.inventoryType = ko.observable(this.staticType ?
	        rvc.lists.inventoryTypes.findByKey(this.staticType) :
	        null);
	    this.product = ko.observable();
	    this.quantity = ko.numericObservable(params.quantity || 1);

	    // Load productOpts
	    this.inventoryType.subscribe(loadProducts);

	    // Select options
	    this.inventoryOpts = rvc.lists.inventoryTypes.buildSelectListOptions();
	    this.productOpts = ko.observableArray();

	    // Set default product
	    if (ko.unwrap(params.product)) {
	        // Silly timeout for select options binding
	        var s = this.productOpts.subscribe(setTimeout.bind(null, function (items) {
	            var prod = getProductByKey(params.product);
	            self.product(prod);
	            s.dispose(); // only once!
	        }), 0);
	    }

	    loadProducts();

	    // Commands/Behaviors
	    this.getInventoryPicks = ko.asyncCommand({
	        execute: function (complete) {
	            return self.pickInventoryAndSave().always(complete);
	        },
	        canExecute: function (isExecuting) {
	            return !isExecuting && self.enablePick() && self.inventoryType() && self.product() && self.quantity() > 0;
	        }
	    });

	    ko.isObservable(params.exports) && params.exports(self);

	    function loadProducts() {
	        var lotType = self.inventoryType();
	        if (lotType) {
	            productService.getProductsByLotType(lotType.key)
	                .done(self.productOpts)
	                .fail(console.error.bind(console));
	        }
	    }

	    /** Get product from productOpts */
	    function getProductByKey(productKey) {
	        var key = ko.unwrap(productKey),
	            items = self.productOpts();

	        for (var i = 0, max = items.length; i < max; i += 1) {
	            if (items[i].ProductKey == key) {
	                return items[i];
	            }
	        }

	        throw 'Could not find given product by key:' + key;
	    }

	    /** Default sort function */
	    function sortByDate(a, b) {
	        //TODO: if same lot, subtract different (sorting) property instead
	        return new Date(a.LotDateCreated) - new Date(b.LotDateCreated);
	    }
	}

	AutoInventoryPicker.prototype.findItemByKeyDelegate = function(key) {
	     return function(item) {
	         return item.InventoryKey === key;
	     }
	}
	AutoInventoryPicker.prototype.pickInventoryAndSave = function () {
	    var self = this,
	        context = ko.unwrap(self.pickingContext),
	        key = ko.unwrap(self.lotKey),
	        pickedItems = ko.toJS(self.existingPickedItems),
	        $dfd = $.Deferred();

	    self.isSaving(true);

	    self.getInventoryItemPicks()
	        .then(function(data) {
	            ko.utils.arrayForEach(data, function(item) {
	                var existingPick = ko.utils.arrayFirst(pickedItems, self.findItemByKeyDelegate(item.InventoryKey));
	                if (existingPick) {
	                    existingPick.QuantityPicked += item.QuantityPicked;
	                } else {
	                    pickedItems.push(item);
	                }
	            });

	            inventoryService.savePickedInventory(context, key, pickedItems)
	                .done(function () {
	                    showUserMessage('Save successful', { description: 'Products have been successfully picked' });
	                    ko.postbox.publish('AutoPickedItemsSaved', pickedItems);
	                    $dfd.resolve(data, pickedItems);
	                })
	                .fail(function (promise, status, message) {
	                    ko.postbox.publish('AutoPickedItemsSaveFailed', pickedItems);
	                    showUserMessage('Failed to save items', { description: 'Server gave error: \n' + message });
	                    $dfd.reject();
	                })
	                .always(function () {
	                    self.isSaving(false);
	                });
	            
	        })
	        .fail(function() {
	            $dfd.reject();
	        });

	    return $dfd;
	}

	AutoInventoryPicker.prototype.getInventoryItemPicks = function () {
	    var self = this;
	    var context = ko.unwrap(self.pickingContext),
	        key = ko.unwrap(self.lotKey),
	        params = {
	            productKey: self.product().ProductKey,
	            productType: self.inventoryType().key
	        },
	        dfd = $.Deferred();

	    inventoryService.getPickableInventory(context, key, params)
	        .fail(function(xhr, result, msg) {
	            showUserMessage("Failed to fetch inventory", {
	                description: msg,
	                mode: 'error',
	                autoclose: true
	            });
	            dfd.reject();
	        })
	        .done(function(data) {
	            var picks = [],
	                quantityRequested = Number(self.quantity()),
	                totalQuantityPicked = 0,
	                diff = quantityRequested;

	            data.sort(self.sortFn);

	            ko.utils.arrayFirst(data, function(item) {
	                item.QuantityPicked = diff > item.Quantity ? item.Quantity : diff;
	                picks.push(item);

	                totalQuantityPicked += item.QuantityPicked;
	                diff = quantityRequested - totalQuantityPicked;
	                return totalQuantityPicked >= quantityRequested;
	            });

	            dfd.resolve(picks);
	        });

	    return dfd;
	}

	module.exports = {
	    viewModel: AutoInventoryPicker,
	    template: __webpack_require__(157)
	};
	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 157 */
/***/ (function(module, exports) {

	module.exports = "<div class=\"auto-inventory-picker\">\r\n    <!-- ko if: !staticType -->\r\n    <label>Inventory Type</label>\r\n    <select data-bind=\"value: inventoryType, options: inventoryOpts, optionsText: 'key', optionsValue: 'value'\"></select>\r\n    <!-- /ko -->\r\n\r\n    <label>Product</label>\r\n    <select data-bind=\"value: product, options: productOpts, optionsText: 'ProductName'\"></select>\r\n\r\n    <label>Quantity</label>\r\n    <input type=\"number\" data-bind=\"value: quantity, valueUpdate: 'input'\" />\r\n\r\n    <input type=\"button\" value=\"Pick\" data-bind=\"command: getInventoryPicks\" />\r\n</div>\r\n"

/***/ }),
/* 158 */
/***/ (function(module, exports, __webpack_require__) {

	var notebooksService = __webpack_require__(49);

	/**
	  * @param {Object} data - Note data object
	  */
	function Instruction(data) {
	  this.cache = ko.toJS(data);

	  /** Data */
	  this.Text = ko.observable(data.Text);
	  this.Sequence = data.Sequence;
	  this.NoteKey = data.NoteKey;
	  this.NoteDate = data.NoteDate || (Date.now()).toISOString();
	  this.CreatedByUser = data.CreatedByUser;

	  /** State */
	  this.isEditing = ko.observable(false);
	  this.isWorking = ko.observable(false);
	  this.isNew = false;
	}

	Instruction.prototype.toDto = function() {
	  var dto = ko.toJS({
	    Text: this.Text(),
	  });

	  return dto;
	};

	/**
	  * @param {Object} input - Observable, Object w/ instructions notebook data
	  * @param {string[]} options - Autocomplete list for instructions editor
	  * @param {function} exports - Observable, Container for exposed methods
	  */
	function InstructionsEditorVM(params) {
	  if (!(this instanceof InstructionsEditorVM)) { return new InstructionsEditorVM(params); }

	  var self = this;
	  var inputData = ko.unwrap(params.input) || {};

	  // Data
	  this.notebookKey = inputData.NotebookKey;
	  this.instructions = ko.observableArray(mapAllInstructions(inputData.Notes));
	  this.options = params.options || [];

	  this.newInstruction = ko.observable();
	  this.isWorking = ko.observable(false);

	  // Behaviors
	  function mapAllInstructions(notes) {
	    var instructions = ko.toJS(notes || []);

	    return ko.utils.arrayMap(instructions, mapInstruction);
	  }

	  function mapInstruction(item) {
	    var newInstruction = new Instruction(item);

	    return newInstruction;
	  }

	  /** Instructions editor */
	  function removeInstruction(item) {
	    var instructions = self.instructions();
	    var i = instructions.indexOf(item);

	    self.instructions.splice(i, 1);
	  }

	  function updateNotebook() {
	    var Notebook = {
	      NotebookKey: self.notebookKey,
	      Notes: ko.toJS(self.instructions)
	    };

	    if ( params && params.input ) {
	      params.input(Notebook);
	    }
	  }

	  this.editInstruction = function(instruction) {
	    var context = instruction || this;

	    context.isEditing(true);
	  };

	  this.cancelInstructionEdit = function(instruction) {
	    var context = instruction || this;

	    context.isEditing(false);
	    context.Text(context.cache.Text);
	  };

	  this.deleteInstruction = function(instruction) {
	    var context = instruction || this;

	    context.isEditing(false);

	    showUserMessage('Delete instructions', {
	      description: "".concat('Remove "<i>', context.Text(), '</i>" from instructions sheet?'),
	      type: 'yesno',
	      onYesClick: function() {
	        var deleteInstruction = notebooksService.deleteNote(context).then(
	        function(data, textStatus, jqXHR) {
	          removeInstruction(context);
	          updateNotebook();
	        },
	        function(jqXHR, textStatus, errorThrown) {
	          showUserMessage('Delete failed', { description: errorThrown });
	        });
	      },
	      onNoClick: null
	    });
	  };

	  this.saveInstructionEdit = function(instruction) {
	    var context = instruction || this;
	    var text = ko.unwrap(context.Text);

	    if (text == undefined) { return; }

	    context.isWorking(true);
	    context.NotebookKey = self.notebookKey;

	    var updateInstruction = notebooksService.updateNoteText(context).then(
	      function (data, textStatus, jqXHR) {
	        context.cache.Text = text;
	        self.options.push(text);
	        updateNotebook();
	      },
	      function (jqXHR, textStatus, errorThrown) {
	        showUserMessage('Save failed', { description: errorThrown });
	      }).always(function () {
	        context.isEditing(false);
	        context.isWorking(false);
	      });
	  };

	  /** New Instruction */
	  this.saveNewInstruction = ko.asyncCommand({
	    execute: function (complete) {
	      var key = self.notebookKey;
	      var instructionText = self.newInstruction().trim();

	      self.isWorking(true);

	      var submitInstruction = notebooksService.insertNote(key, { Text: instructionText })
	        .then(function (data, textStatus, jqXHR) {
	          var instructionData = mapInstruction(data);
	          if (self.options.indexOf(instructionText) === -1) {
	            self.options.push(instructionData.Text());
	          }
	          self.instructions.push(instructionData);
	          self.newInstruction("");
	          updateNotebook();
	        }, function (jqXHR, textStatus, errorThrown) {
	          showUserMessage('Save failed', { description: errorThrown });
	        }).always(function () {
	          self.isWorking(false);
	          complete();
	        });
	    },
	    canExecute: function(isExecuting) {
	      var instruction = self.newInstruction();
	      return !isExecuting && instruction != undefined && instruction.trim() !== '';
	    }
	  });

	  /** Data export */
	  function toDto() {
	    return ko.utils.arrayMap(self.instructions(), function(instruction) {
	      return instruction.Text();
	    });
	  }

	  // Exports
	  if (params && params.exports) {
	    params.exports({
	      toDto: toDto,
	      Notebook: {
	        Notes: self.instructions,
	        NotebookKey: self.notebookKey
	      }
	    });
	  }

	  return this;
	}

	module.exports = {
	  viewModel: InstructionsEditorVM,
	  template: __webpack_require__(159),
	  synchronous: true
	};



/***/ }),
/* 159 */
/***/ (function(module, exports) {

	module.exports = "<section class=\"table-responsive\">\n  <table class=\"table table-hover\">\n    <thead>\n      <tr>\n        <th class=\"instructions-controls\">\n        </th>\n        <th>\n          Instructions\n        </th>\n      </tr>\n    </thead>\n    <tbody data-bind=\"foreach: instructions\">\n      <tr data-bind=\"template: isEditing() ? 'instruction-edit' : 'instruction-display' \">\n      </tr>\n    </tbody>\n    <tfoot>\n      <tr>\n        <td>\n          <button type=\"button\" class=\"btn btn-link\" data-bind=\"command: saveNewInstruction\"><i class=\"fa fa-fw fa-check\"></i></button>\n        </td>\n        <td><input type=\"text\" class=\"form-control\" data-bind=\"value: newInstruction, valueUpdate: 'afterkeydown', autocomplete: { source: options, allowNewValues: true }, onEnter: saveNewInstruction, disable: isWorking\" placeholder=\"New Instruction\"></td>\n      </tr>\n    </tfoot>\n  </table>\n</section>\n\n<script id=\"instruction-display\" type=\"text/html\">\n  <td>\n    <button type=\"button\" class=\"btn btn-link\" data-bind=\"click: $parent.deleteInstruction\"><i class=\"fa fa-fw fa-trash-o\"></i></button>\n  </td>\n  <td class=\"clickable\" data-bind=\"click: $parent.editInstruction\">\n    <p class=\"form-control-static\" data-bind=\"text: Text\"></p>\n  </td>\n</script>\n\n<script id=\"instruction-edit\" type=\"text/html\">\n  <td data-bind=\"visible: isEditing\">\n    <section class=\"btn-group\">\n      <button type=\"button\" class=\"btn btn-link\" data-bind=\"click: $parent.saveInstructionEdit\"><i class=\"fa fa-fw fa-check\"></i></button>\n      <button type=\"button\" class=\"btn btn-link\" data-bind=\"click: $parent.cancelInstructionEdit\"><i class=\"fa fa-fw fa-times\"></i></button>\n    </section>\n  </td>\n  <td>\n    <input type=\"text\" class=\"form-control\" data-bind=\"value: Text, valueUpdate: 'afterkeydown', autocomplete: { source: $parent.options, allowNewValues: true }, onEnter: $parent.saveInstructionEdit, disable: isWorking\">\n  </td>\n</script>\n"

/***/ }),
/* 160 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(7), __webpack_require__(72), __webpack_require__(77), __webpack_require__(16), __webpack_require__(24), __webpack_require__(10), __webpack_require__(8)], __WEBPACK_AMD_DEFINE_RESULT__ = function (core, lotService, inventoryService, directoryService, productsService, pagedDataHelper, app) {
	    return {
	        createPackSchedule: core.setupFn(createPackSchedule, psUrl),
	        createProductionBatch: function(data, options) {
	           return core.ajaxPost(batchUrl(), data, options);
	        },
	        deletePackSchedule: core.setupFn(deletePackSchedule, psUrl),
	        deleteProductionBatch: core.setupFn(deleteProductionBatch, batchUrl),
	        getAttributeNames: lotService.getAttributeNames,
	        getBatchInstructionOptions: function(options) {
	           return core.ajax('/api/productionbatchinstructions', options);
	        },
	        getBatchPickingInventoryPager: function (batchKey, options) {
	            //todo: remove after the production batch UI has been updated to utilize the new inventory picking UI component.
	            return inventoryService.getPickableInventoryPager(app.lists.inventoryPickingContexts.ProductionBatch, batchKey, options);
	        },
	        getChileProducts: productsService.getChileProducts,
	        getCustomers: directoryService.getCustomers,
	        getProductDetails: productsService.getProductDetails,
	        getPackagingProducts: productsService.getPackagingProducts,
	        getPackScheduleDetails: core.setupFn(getPackScheduleDetails, psUrl),
	        getPackSchedulesPager: function (options) {
	            options = options || {};
	            options.urlBase = psUrl();
	            return pagedDataHelper.init(options);
	        },
	        getProductionBatchDetails: function(batchKey, options) {
	            return core.ajax(batchUrl(batchKey), options);
	        },
	        getProductionLocations: function (options) {
	            return core.ajax("/api/productionLines", options);
	        },
	        getWorkModes: function () {
	            return core.ajax("/api/workModes");
	        },
	        //todo: refactor into inventoryPickingService
	        savePickedInventoryForBatch: function(batchKey, data, options) {
	            return core.ajaxPost('/api/' + app.lists.inventoryPickingContexts.ProductionBatch.value.toLowerCase() + '/' + batchKey + '/pick', data, options);
	        },
	        updatePackSchedule: core.setupFn(updatePackSchedule, psUrl),
	        updateProductionBatch: function(batchKey, data, options) {
	            return core.ajaxPut(batchUrl(batchKey), data, options);
	        },
	    }
	    
	    //#region url functions
	    function batchUrl(key) {
	        var url = "/api/productionbatches";
	        if (key != undefined) url = url + '/' + key;
	        return url;
	    }

	    function psUrl(key) {
	        var url = "/api/packSchedules";
	        if (key != undefined) url = url + '/' + key;
	        return url;
	    }
	    //#endregion

	    //#region service functions
	    function createPackSchedule(data) {
	        return core.ajaxPost(psUrl(), data);
	    }
	    function deleteProductionBatch(batchKey, options) {
	        return core.ajaxDelete(batchUrl(batchKey), options);
	    }
	    function deletePackSchedule(key) {
	        return core.ajaxDelete(psUrl(key));
	    }
	    function getPackScheduleDetails(psKey) {
	        return core.ajax(psUrl(psKey));
	    }
	    function updatePackSchedule(psKey, data) {
	        return core.ajaxPut(psUrl(psKey), data);
	    }
	    //#endregion
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

/***/ }),
/* 161 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_FACTORY__, __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;(function() {
	    //if (require)
	        !(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(1), __webpack_require__(162)], __WEBPACK_AMD_DEFINE_FACTORY__ = (factory), __WEBPACK_AMD_DEFINE_RESULT__ = (typeof __WEBPACK_AMD_DEFINE_FACTORY__ === 'function' ? (__WEBPACK_AMD_DEFINE_FACTORY__.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__)) : __WEBPACK_AMD_DEFINE_FACTORY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));
	    //else factory(jQuery, domGlueWidget);

	    function factory($, domGlueWidget) {
	        $.StickyTableHeaders = function(el, options) {

	            if (options.format) return reformatClone(el, options);
	            if (options.refresh) return refreshClone(el, options);
	            if (options.show) return setVisibility(el, true);
	            if (options.hide) return setVisibility(el, false);

	            var domGlue = getDomGlueWidget(el);
	            if (domGlue) domGlue.cleanup.call(el);

	            var self = this;
	            self.options = $.extend({}, $.StickyTableHeaders.defaultOptions, options);
	            //prepend element ID in attempt to prevent multiple instance with the same floating element id.
	            if (el.id) { self.options.floatingElementId = el.id + '_' + self.options.floatingElementId; }
	            self.options.onParentScroll = curryParentScrolledHandler($(el));
	            self.options.formatClone = updateCloneLayout;

	            self.domGlue = domGlueWidget.init(el, self.options);

	            $(el).data('__stickyTableHeader', self);
	        };

	        $.StickyTableHeaders.defaultOptions = {
	            fixedOffset: 0,
	            offsetTop: 0,
	            parent: window,
	            floatingElementId: 'stickyTableHeader',
	            target: 'thead:first',
	            position: { top: 0, left: 0 }
	        };

	        $.fn.stickyTableHeaders = function (options) {
	            var optParam = options;
	            if (typeof arguments[0] === "string") {
	                var arg0 = arguments[0].toLowerCase();
	                if (arg0 === "option") {
	                    optParam = {};
	                    optParam[arguments[1]] = arguments[2] || true;
	                } else if (arg0 === "destroy") {
	                    return cleanup.call(this);
	                }
	            }

	            return this.each(function () {
	                (new $.StickyTableHeaders(this, optParam));
	            });
	        };
	        
	        function curryParentScrolledHandler($element) {
	            return function (parent, target, clone) {
	                var displayClone = displayFloatingClone(parent);
	                var $parent = $(parent);

	                var display = displayClone ? 'block' : 'hidden';

	                $(clone).css({
	                    'display': display,
	                    'left': -1 * $parent.scrollLeft() + "px",
	                });
	            }

	            function displayFloatingClone(parent) {
	                return headerIsOffScreen(parent) && tableIsOnScreen();
	            }

	            function headerIsOffScreen(parent) {
	                var $parent = $(parent);
	                var parentPosition = $parent.offset() || { top: 0, left: 0 };
	                var position = $element.offset() || { top: 0, left: 0 };
	                var scrollTop = $parent.scrollTop();
	                var $window = $(window);
	                var parentIsWindow = $parent == $window;

	                return scrollTop + parentPosition.top > position.top
	                    || !parentIsWindow && $window.scrollTop() > parentPosition.top;
	            }

	            function tableIsOnScreen() {
	                //return height > 0;
	                return true;
	            }
	        }
	        function updateCloneLayout(orig, clone) {
	            // Copy cell widths and classes from original header
	            $('th', clone).each(function (index) {
	                var $this = $(this);
	                var origCell = $('th', orig).eq(index);
	                $this.removeClass().addClass(origCell.attr('class'));
	                $this.css('width', origCell.width());
	            });

	            // Copy row width from whole table
	            clone.css('width', orig.width());
	        }

	        function reformatClone(el) {
	            var domGlue = getDomGlueWidget(el);
	            if (!domGlue) return;

	            var target = domGlue.getTargetElement();
	            updateCloneLayout(target, domGlue.getCloneElement());
	            domGlue.updateBoundaryPosition();
	        }
	        function refreshClone(el) {
	            var domGlue = getDomGlueWidget(el);
	            if (!domGlue) return;

	            domGlue.rebuildClone();
	            reformatClone(el);
	        }
	        function setVisibility(el, show) {
	            var domGlue = getDomGlueWidget(el);
	            if (!domGlue) return;
	            var clone = domGlue.getCloneElement();
	            if (!clone) return;
	            if (show) clone.show();
	            else cline.hide();
	        }
	        function cleanup() {
	            var widget = getDomGlueWidget(this);
	            widget && widget.cleanup();
	            $.removeData(this, '__stickyTableHeader');
	        }

	        function getDomGlueWidget(el) {
	            var widget = $(el).data('__stickyTableHeader');
	            return widget && widget.domGlue || undefined;
	        }
	    }
	}());

/***/ }),
/* 162 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_FACTORY__, __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/*! Copyright (c) 2014 by Solutionhead Technologies, LLC
	MIT license info: ... */

	(function () {
	    //if (require)
	        !(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(1)], __WEBPACK_AMD_DEFINE_FACTORY__ = (factory), __WEBPACK_AMD_DEFINE_RESULT__ = (typeof __WEBPACK_AMD_DEFINE_FACTORY__ === 'function' ? (__WEBPACK_AMD_DEFINE_FACTORY__.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__)) : __WEBPACK_AMD_DEFINE_FACTORY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));
	    //else factory($);

	    var wrapperClassName = 'domGlueWrapper',
	        boundsClassName = 'domGlueBounds';

	    var defaultOptions = {
	        fixedOffset: 0,
	        offsetTop: 0,
	        offsetBottom: 0,
	        parent: window,
	        floatingElementId: "domGlueFloatingElement",
	        position: {},
	        zIndex: 25,
	        initiallyHidden: false,
	    };

	    function factory($) {
	        return {
	            init: function(el, options) {
	                var self = {};

	                var $el = $(el);
	                
	                options = options || {};
	                self.options = $.extend({}, defaultOptions, options);

	                self.floatingElementId = options.floatingElementId;
	                self.$parent = $(options.parent); //warning: options.parent is a DOM reference
	                self.getParentElement = function () { return $(options.parent); } //warning: options.parent is a DOM reference

	                self.handlers = [];

	                self.cleanupClone = function () {
	                    $('.' + options.floatingElementId, $el).remove();
	                }
	                self.cleanup = function () {
	                    $el = null;
	                    self.cleanupClone();
	                    destroy.call(self);
	                    self = null;
	                }
	                self.updateBoundaryPosition = function () {
	                    var boundary = self.getBoundaryElement();
	                    boundary.updateBoundaryPosition && boundary.updateBoundaryPosition();
	                }

	                init.call(self, $el, options);

	                return self;
	            },
	            defaultOptions: defaultOptions,
	        }


	        function init($el, options) {
	            var self = this;
	            var $parent = self.getParentElement();
	            var $window = $(window);

	            if (!$el.parent("." + wrapperClassName).length) {
	                $el.wrap('<div class="' + wrapperClassName + '"></div>');
	            }

	            self.getTargetElement = function() {
	                return $(options.target, $el);
	            };
	            self.getCloneElement = function() {
	                return $('.' + options.floatingElementId, $el);
	            };
	            self.getBoundaryElement = function() {
	                return self.getTargetElement().siblings('.' + boundsClassName);
	            };


	            self.rebuildClone = function () {
	                self.cleanupClone();
	                $clone = cloneTarget();
	                $clone.appendTo(self.getBoundaryElement());
	            }

	            self.cleanup = function () {
	                $el = null;
	                $clone = null;
	                $parent = null; //remove resize handler, remove parents().scroll handlers
	                target = null;
	                boundary = null;
	                $window.off('resize', updateBoundariesFn);
	                $window = null; //remove resize handler
	            }
	            
	            var target = self.getTargetElement(); 
	            target.addClass('domGlueFloatingElementOriginal');
	            var boundary = initBoundaryElement();

	            var $clone = cloneTarget(); 
	            $clone.appendTo(boundary);

	            target.after(boundary);

	            if (typeof options.onParentScroll === "function") {
	                var parentScrolledEventFromOptions = function (event) {
	                    options.onParentScroll($parent, target, $clone);
	                    event.stopPropagation();
	                }
	                self.handlers.push({ obj: $parent, event: 'scroll', handler: parentScrolledEventFromOptions });
	                $parent.scroll(parentScrolledEventFromOptions);
	            }

	            var updateBoundariesFn = currySetBoundaryPositioningEventHandler(boundary);

	            self.handlers.push({ obj: $parent.parents(), event: 'scroll', handler: updateBoundariesFn });
	            $parent.parents().scroll(updateBoundariesFn);

	            self.handlers.push({ obj: $parent, event: 'resize', handler: updateBoundariesFn });
	            $parent.resize(updateBoundariesFn);

	            if ($parent[0] !== $window[0]) {
	                self.handlers.push({ obj: $window, event: 'resize', handler: updateBoundariesFn });
	                $window.resize(updateBoundariesFn);
	            }

	            setTimeout(function () {
	                setBoundaryPositioning();
	            }, 0); // ensure that the content has been drawn.
	            
	            function currySetBoundaryPositioningEventHandler(boundaryElement) {
	                return function (event) {
	                    setBoundaryPositioning(boundaryElement);
	                    event && event.stopPropagation && event.stopPropagation();
	                }
	            }

	            function initBoundaryElement() {
	                var boundaryElement;
	                boundaryElement = self.getBoundaryElement();
	                if (boundaryElement.length) {
	                    return boundaryElement[0];
	                }

	                boundaryElement = $("<div></div>");
	                boundaryElement.addClass(boundsClassName);
	                boundaryElement.addClass('pass-through');
	                self.updateBoundaryPosition = function() {
	                    setBoundaryPositioning(boundaryElement);
	                }
	                setBoundaryPositioning(boundaryElement);
	                return boundaryElement;
	            }
	            function setBoundaryPositioning() {
	                var parentPosition = $parent.offset() || { top: 0, left: 0 };

	                self.getBoundaryElement().css({
	                    'position': 'fixed',
	                    'z-index': options.zIndex,
	                    'top': parentPosition.top - $(window).scrollTop(),
	                    'left': parentPosition.left,
	                    'display': options.initiallyHidden ? 'none' : 'block',
	                    'height': getHeight(),
	                    'width': getWidth(),
	                    overflow: 'hidden',
	                });

	                function getHeight() {
	                    return $parent[0].clientHeight || $parent.height();
	                }

	                function getWidth() {
	                    return $parent[0].clientWidth || $parent.width();
	                }
	            }
	            function cloneTarget() {
	                var originalEl = self.getTargetElement();
	                var cloneEl = originalEl.clone();
	                cloneEl.addClass(options.floatingElementId);

	                cloneEl.css({
	                    'position': 'absolute',
	                    'top': options.position.top,
	                    'left': options.position.left,
	                    'bottom': options.position.bottom,
	                    'right': options.position.right,
	                });

	                options.formatClone && options.formatClone(originalEl, cloneEl);

	                return cloneEl;
	            }
	        }
	        
	        function destroy() {
	            var self = this;

	            self.$parent = null;

	            for (var i = 0; i < self.handlers.length; i++) {
	                var handle = self.handlers[i];
	                handle.obj.off(handle.event, handle.handler);
	            }

	            self.handlers = [];
	            self.cleanup();
	        }
	    }   
	}());

/***/ })
]);