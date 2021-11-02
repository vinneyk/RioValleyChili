define(['ko', 'viewModels/qualityControl/lotHolds', 'viewModels/shared/lots', 'viewModels/shared/notebooks'], function (ko, lotHolds, lotsBase, notebooks) {
    var baseUrl = "/api/lots",
        microAttributes = {
            "TPC": true,
            "Yeast": true,
            "Mold": true,
            "EColi": true,
            "ColiF": true,
            "Sal": true
        },
        duplicationModeOptions = [
            { value: 'all', text: 'All Values', fn: function () { return true; } },
            { value: 'micros', text: 'Micros Only', fn: microAttributeFilter }
        ], baseMapLotSummary;

    function microAttributeFilter(attr) {
        return microAttributes[attr.Key || attr] === true;
    }
    var bypassHistory = false;

    return {
        instance: init()
    };

    function init() {
        var scrollToItem = ko.observable(false);
        var self = lotsBase;
        baseMapLotSummary = self.mapLotSummary;

        self.microAttributes = microAttributes;
        self.LabResultSummaries = ko.observableArray([]);
        self.LabResultDetailsViewModel = LabResultDetailsViewModel;
        self.allDataLoaded = ko.observable(false);

        self.defaultTestDate = ko.observableDate(Date.now());
        self.chileTypeFilter = ko.observable(rvc.helpers.chileTypes.FinishedGoods.key).extend({ chileType: true });
        self.productionStartFilter = ko.observableDate();
        self.productionEndFilter = ko.observableDate();
        self.productionStatusFilter = ko.observable(rvc.helpers.productionStatusTypes.Produced.key).extend({ productionStatusType: true });
        self.qualityStatusFilter = ko.observable(rvc.helpers.lotQualityStatusTypes.Undetermined.key).extend({ lotQualityStatusType: true });
        self.startingLotFilter = ko.observable();

        self.duplicationSourceLotKey = ko.observable().extend({ lotKey: true });
        self.duplicationMode = ko.observable(duplicationModeOptions[0]);
        self.duplicationModeOptions = duplicationModeOptions;
        self.displayDuplicationView = ko.observable(false);

        // functions
        self.selectLot = selectLot;
        self.mapLotSummary = mapLabResultLotSummary;

        self.animateNewItem = rvc.utils.animateNewItem({
            scrollToItem: scrollToItem,
            afterScrollCallback: function () {
                scrollToItem(false);
            },
            paddingTop: 200,
        });

        var lotPager = PagedDataHelper.init({
            urlBase: baseUrl,
            pageSize: 50,
            parameters: {
                lotType: self.chileTypeFilter,
                productionStatus: self.productionStatusFilter,
                productionStart: self.productionStartFilter,
                productionEnd: self.productionEndFilter,
                qualityStatus: self.qualityStatusFilter,
                startingLotKey: self.startingLotFilter,
            },
            resultCounter: function (data) {
                return data[0].LotSummaries.length;
            },
            onNewPageSet: function () {
                self.LabResultSummaries([]);
            }
        });

        // computed properties
        self.displaySavingMessage = ko.computed(function () {
            return self.LabResultDetailsViewModel.saveLotAttributesCommand.isExecuting()
                || self.LabResultDetailsViewModel.updateLotStatus.isExecuting()
                || self.LabResultDetailsViewModel.deleteDefectResolutionCommand.isExecuting();
        });
        self.displayLoadingMessage = ko.observable(false);

        // commands
        self.getLabResultsCommand = ko.asyncCommand({
            execute: function (complete) {
                lotPager.GetNextPage({
                    successCallback: function (data) {
                        self.allDataLoaded(lotPager.allDataLoaded);
                        scrollToItem(true);
                        if (data.splice) data = data[0];
                        if (!self.AttributeNames().length) self.AttributeNames(data.AttributeNamesByProductType.Chile);

                        var mappedLotSummaries = ko.utils.arrayMap(data.LotSummaries, self.mapLotSummary);
                        var targetLot = mappedLotSummaries[0];

                        if (self.startingLotFilter() && mappedLotSummaries.length) {
                            var labResultChanged = self.LabResultDetailsViewModel.LabResult.subscribe(function (val) {
                                if (val === targetLot) {
                                    pushLabResultSummaries();
                                    labResultChanged.dispose();
                                }
                            });
                            self.LabResultDetailsViewModel.LabResult(targetLot);
                            if (self.LabResultDetailsViewModel.LabResult.peek() !== targetLot) {
                                labResultChanged.dispose();
                            }
                        } else pushLabResultSummaries();

                        function pushLabResultSummaries() {
                            ko.utils.arrayPushAll(self.LabResultSummaries(), mappedLotSummaries);
                            self.LabResultSummaries.notifySubscribers(self.LabResultSummaries());
                        }
                    },
                    completeCallback: complete,
                });
            },
            canExecute: function (isExecuting) {
                return !isExecuting;
            }
        });
        self.searchLotKeyCommand = ko.asyncCommand({
            execute: function (options, complete) {
                self.displayLoadingMessage(true);
                if (typeof options === "string") {
                    options = {
                        lotKey: options
                    };
                }

                options = options || {};

                if (!options.lotKey) {
                    throw new Error("LotKey required.");
                }

                self.searchLotKeyCommand.indicateWorking();

                findLotByKeyAsync(options.lotKey, {
                    successCallback: function (data) {
                        if (data.ProductionStatus) { // from memory; already mapped
                            setDetailsItem(data);
                            return;
                        }

                        if (!self.AttributeNames().length)
                            self.AttributeNames(data.AttributeNamesByProductType.Chile || []);

                        setDetailsItem(self.mapLotSummary(data.LotSummary));
                    },
                    errorCallback: function (xhr, status, message) {
                        console.log(xhr);
                        self.searchLotKeyCommand.indicateFailure();
                    },
                    completeCallback: function () {
                        self.displayLoadingMessage(false);
                        complete && complete();
                    },
                }, options.bypassLocalCheck);

                function setDetailsItem(item) {
                    self.LabResultDetailsViewModel.LabResult(item);
                    self.searchLotKeyCommand.clearStatus();
                    complete();
                }
            },
            canExecute: function (isExecuting) {
                return !isExecuting;
            }
        });
        self.applyFiltersCommand = ko.command({
            execute: function () {
                self.getLabResultsCommand.execute();
            },
            canExecute: function (isExecuting) {
                return !isExecuting; // && has filter options
            },
        });
        self.saveAttributesCommand = ko.asyncCommand({
            execute: function (complete) {
                var lotKey = self.LabResultDetailsViewModel.LabResult.peek().LotKey;
                self.LabResultDetailsViewModel.saveLotAttributesCommand.execute({
                    successCallback: function () {
                        self.searchLotKeyCommand.execute({
                            lotKey: lotKey,
                            bypassLocalCheck: true,
                        });
                    },
                    completeCallback: function () {
                        complete && complete();
                    },
                });
            },
            canExecute: function (isExecuting) {
                return !isExecuting && self.LabResultDetailsViewModel.saveLotAttributesCommand.canExecute();
            }
        });
        self.openAttributeDuplicationView = ko.command({
            execute: function () {
                self.duplicationSourceLotKey(null);
                self.displayDuplicationView(true);
            },
            canExecute: function () {
                return self.LabResultDetailsViewModel.LabResult();
            }
        });
        self.closeAttributeDuplicationView = ko.command({
            execute: function () {
                self.displayDuplicationView(false);
            },
            canExecute: function () {
                return self.displayDuplicationView();
            }
        });
        self.copyAttributeValuesToSelectedLotCommand = ko.asyncCommand({
            execute: function (lotKey, complete) {
                lotKey = typeof lotKey === "string" ? lotKey : self.duplicationSourceLotKey.formattedLot();
                if (!lotKey) {
                    completeFn();
                    return;
                }

                findLotByKeyAsync(lotKey, {
                    successCallback: function (data) {
                        var sourceLot = data.LotSummary ? self.mapLotSummary(data.LotSummary) : data;
                        (function (source, target, filterFn) {
                            ko.utils.arrayForEach(source.Attributes, function (attr) {
                                if (!filterFn(attr)) return;
                                var targetAttribute = ko.utils.arrayFirst(target.Attributes, function (targetAttr) {
                                    return targetAttr.Key === attr.Key;
                                });
                                if (targetAttribute) {
                                    targetAttribute.Value(ko.utils.unwrapObservable(attr.Value));
                                    targetAttribute.AttributeDate(ko.utils.unwrapObservable(attr.AttributeDate));
                                    targetAttribute.beginEditingCommand.execute();
                                }
                            });
                        }(sourceLot, self.LabResultDetailsViewModel.LabResult(), self.duplicationMode().fn));
                        showUserMessage("Attributes copied successfully");
                    },
                    errorCallback: function (xhr, status, message) {
                        showUserMessage("Unable to copy attributes from lot.", { description: message });
                    },
                    completeCallback: completeFn,
                });

                function completeFn() {
                    complete();
                    self.displayDuplicationView(false);
                }
            },
            canExecute: function (isExecuting) {
                return !isExecuting && self.LabResultDetailsViewModel.LabResult() != undefined;
            }
        });
        self.loadNextLotCommand = ko.command({
            execute: function () {
                var labResults = self.LabResultSummaries.peek();
                var nextIndex = getCurrentIndex() + 1;

                if (nextIndex >= labResults.length) {
                    var sub = self.LabResultSummaries.subscribe(function (values) {
                        loadNext(values);
                        sub.dispose();
                        sub = null;
                    });

                    self.getLabResultsCommand.execute();
                    return;
                }

                loadNext(labResults);

                function loadNext(labResultValues) {
                    if (nextIndex >= labResultValues.length) return;
                    var next = labResultValues[nextIndex];
                    if (next) {
                        self.LabResultDetailsViewModel.LabResult(next);
                    }
                }

            },
            canExecute: function () { return self.LabResultDetailsViewModel.LabResult() != undefined; }
        });
        self.loadPreviousLotCommand = ko.command({
            execute: function () {
                var previousIndex = getCurrentIndex() - 1;
                if (previousIndex < 0) {
                    showUserMessage("This is the first lot from the list.");
                    return;
                }
                var previous = self.LabResultSummaries()[previousIndex];
                self.LabResultDetailsViewModel.LabResult(previous);
            }
        });

        // subscribers
        var currentLabResultSubscriptions = [];
        self.LabResultDetailsViewModel.LabResult.subscribe(function (value) {
            updateHistory(value);

            if (!value) {
                !self.LabResultSummaries().length && self.getLabResultsCommand.execute();
                return;
            }

            var maxDate = null;
            ko.utils.arrayForEach(value.Attributes, function (attr) {
                currentLabResultSubscriptions.push(attr.AttributeDate.subscribe(setDefaultTestDate));
                currentLabResultSubscriptions.push(attr.Value.subscribe(setAttributeDateFromDefault.bind(attr)));

                var testDate = attr.AttributeDate();
                if (testDate && (!maxDate || testDate > maxDate)) {
                    maxDate = testDate;
                }
            });
            if (maxDate) setDefaultTestDate(maxDate);

            function setDefaultTestDate(newDefaultDate) {
                if (newDefaultDate) self.defaultTestDate(newDefaultDate);
            }
            function setAttributeDateFromDefault(attrValue) {
                this.AttributeDate(
                    attrValue == undefined ? null : self.defaultTestDate()
                );
            }
        });
        self.LabResultDetailsViewModel.LabResult.subscribe(function () {
            ko.utils.arrayForEach(currentLabResultSubscriptions, function (sub) {
                sub.dispose();
            });
            currentLabResultSubscriptions = [];
        }, self, 'beforeChange');
        self.LabResultDetailsViewModel.LabResult.subscribe(function (lotKey) {
            self.displayLoadingMessage(true);
            rvc.api.lots.getLotByKey(lotKey, {
                successCallback: updateLot,
                errorCallback: function () {
                    showUserMessage("Failed to get updated lot.", { description: "The Lot changes were saved successfully but we had trouble receiving the updated data from the server. Try refreshing the page if you want to see the updated values." });
                },
                completeCallback: function () {
                    self.displayLoadingMessage(false);
                }
            });

            function updateLot(data) {
                var newLotData = self.mapLotSummary(data.LotSummary);
                var index = 0;
                var current = ko.utils.arrayFirst(self.LabResultSummaries(), function (item) {
                    if (item.LotKey === lotKey) { return true; }
                    index++;
                    return false;
                });
                if (current) {
                    self.LabResultSummaries.splice(index, 1, newLotData);
                }
                self.LabResultDetailsViewModel.LabResult(newLotData);
            }
        }, self, 'lotDataChanged');

        self.startingLotFilter.extend({ lotKey: self.getLabResultsCommand.execute });
        ajaxStatusHelper.init(self.startingLotFilter);
        ajaxStatusHelper.init(self.searchLotKeyCommand);

        var hash = rvc.utils.getHashValue();
        if (hash) {
            updateHistory({ LotKey: hash }, true);
            self.startingLotFilter.formattedLot(hash);
        } else {
            self.getLabResultsCommand.execute();
        }

        registerHashEvent();
        ko.applyBindings(self);
        return self;

        function findLotByKeyAsync(lotKey, callbackOptions, bypassLocalCheck) {
            if (bypassLocalCheck !== true) {
                var fromMemory = ko.utils.arrayFirst(self.LabResultSummaries(), function (item) {
                    return item.LotKey === lotKey;
                });
            }

            if (fromMemory) {
                callbackOptions.successCallback && callbackOptions.successCallback(fromMemory);
                callbackOptions.completeCallback && callbackOptions.completeCallback();
                return;
            }

            rvc.api.lots.getLotByKey(lotKey, callbackOptions);
        }
        function selectLot(lot) {
            self.LabResultDetailsViewModel.LabResult(lot);
        }
        function getCurrentIndex() {
            var lot = self.LabResultDetailsViewModel.LabResult.peek();
            return lot ? ko.utils.arrayIndexOf(self.LabResultSummaries.peek(), lot) : -1;
        }
        function registerHashEvent() {
            window.onpopstate = function (e) {
                if (!e.state) return;
                bypassHistory = true;
                var lot = e.state.LotKey;
                if (lot) {
                    self.startingLotFilter.formattedLot(lot);
                } else {
                    self.LabResultDetailsViewModel.LabResult(null);
                }
            };
        }
    }

    function mapLabResultLotSummary(values) {
        var labResultLot = baseMapLotSummary(values);
        labResultLot.QualityControlNotebookKey = values.QualityControlNotebookKey;
        labResultLot.ValidLotQualityStatuses = values.ValidLotQualityStatuses;

        //computed properties
        labResultLot.requiresQualityControlIntervention = ko.computed(function () {
            return this.QualityStatus() === rvc.helpers.lotQualityStatusTypes.RequiresAttention.key;
        }, labResultLot);
        labResultLot.enableRejection = ko.computed(function () {
            return !this.requiresQualityControlIntervention() && this.QualityStatus() !== rvc.helpers.lotQualityStatusTypes.Rejected.key;
        }, labResultLot);
        labResultLot.enableAcceptance = ko.computed(function () {
            return !this.requiresQualityControlIntervention()
                && this.QualityStatus() !== rvc.helpers.lotQualityStatusTypes.Released.key
                && this.QualityStatus() !== rvc.helpers.lotQualityStatusTypes.Contaminated.key;
        }, labResultLot);
        labResultLot.enableQualityHold = ko.computed(function () {
            return !this.requiresQualityControlIntervention() && this.QualityStatus() !== rvc.helpers.lotQualityStatusTypes.RequiresAttention.key;
        }, labResultLot);

        return labResultLot;
    }
    function updateHistory(value, useReplace) {
        if (bypassHistory === true) {
            bypassHistory = false;
            return;
        }

        var hash = value ? value.LotKey : '';
        if (!useReplace && rvc.utils.getHashValue() === hash) return;

        var args = value
            ? [{ LotKey: value.LotKey }, ("Lab Results " + value.LotKey), "#" + value.LotKey]
            : [{}, "Lab Results", window.location.pathname];

        useReplace === true
            ? rvc.helpers.history.replaceState.apply(null, args)
            : rvc.helpers.history.pushState.apply(null, args);
    }

});