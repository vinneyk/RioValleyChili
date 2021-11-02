define(['services/qualityControlService', 'viewModels/shared/lots', 'viewModels/qualityControl/lotHolds', 'viewModels/shared/notebooks', 'app', 'ko', 'helpers/koHelpers'],
    function (qualityControlService, lots, lotHolds, notebooks, rvc, ko, koHelpers) {
        var details = ko.observable();
        var lotDefectFactory = require('App/models/LotDefect');
        var editableLotAttributeFactory = require('App/models/EditableLotAttribute');
        var lotDefectResolutionFactory = require('App/models/LotDefectResolution');

    var self = {
        LabResult: ko.computed({
            read: function () { return details(); },
            write: function (value) {
                (function (previous) {
                    if (previous != undefined) {
                        if (previous.hasChanges()) {
                            showUserMessage("Do you want to save changes?", {
                                description: "This lot has unsaved changes. Click <strong>\"Yes\"</strong> to save changes and continue navigating away " +
                                    "Click <strong>\"No\"</strong> to clear all changes, or click <strong>\"Cancel\"</strong> to stay on the current lot.",
                                type: 'yesnocancel',
                                onYesClick: function () {
                                    self.saveLotAttributesCommand.execute({
                                        successCallback: replaceLot
                                    });
                                },
                                onNoClick: function () {
                                    self.cancelAllEditsCommand.execute();
                                    replaceLot();
                                },
                                onCancelClick: function () {

                                }
                            });
                            return;
                        }
                    }
                    replaceLot();

                }(details.peek()));

                function replaceLot() {
                    ko.utils.arrayForEach(self.LabResult.__subscriptions || [], function (sub) { return sub.dispose(); });
                    self.LabResult.__subscriptions = [];
                    if (value && value.CustomerKey) {
                        var productKey = value.Product.ProductKey;
                        qualityControlService.getCustomerProductSpec(value.CustomerKey, productKey)
                            .done(function (customerProductSpecs) {
                                if (!customerProductSpecs || !customerProductSpecs.length) return;
                                var specs = customerProductSpecs.toObj(function(s) { return s.AttributeShortName; }, function(s) { return { minValue: s.RangeMin, maxValue: s.RangeMax } });
                                ko.utils.arrayForEach(value.Attributes, function(attribute) {
                                    attribute.CustomerSpec = specs[attribute.Key];
                                });
                            })
                            .always(function() {
                                details(mapLabResult(value));
                            });
                    } else details(value ? mapLabResult(value) : null);
                }
            }
        }),
        ResolveDefect: ko.observable(),
        showResolveDefectDialog: ko.observable(),
        NewDefect: ko.observable(),
        showNewDefectDialog: ko.observable(),
        inhouseDefectOptions: ['Dark Specs', 'Smoke Count', 'Hard BB\'s', 'Soft BB\'s'],
        newLotStatus: ko.observable().extend({ lotQualityStatusType: true }),

        // methods
        initiateDefectResolution: initiateDefectResolution,
    };

    // commands
    self.closeLabResultCommand = ko.command({
        execute: function () {
            self.LabResult(null);
        },
        canExecute: function () {
            return self.LabResult() != null;
        }
    });
    self.saveLotAttributesCommand = ko.asyncCommand({
        execute: function (options, complete) {
            options = options || {};
            var model = self.LabResult.peek();


            var baseCompleteCallback = options.completeCallback;
            options.completeCallback = function () {
                complete();
                if (baseCompleteCallback) baseCompleteCallback();
            };

            if (!isValid()) {
                showUserMessage("Unable to save due to validation errors", { description: "Please correct all validation errors and retry." });
                options.completeCallback();
                return;
            }

            var baseSuccessCallback = options.successCallback;
            options.successCallback = function () {
                ko.utils.arrayForEach(model.Attributes, function (attr) {
                    attr.saveEditsCommand.execute();
                });

                var oldContextOverride = model.OverrideOldContextLotAsCompleted() === true;

                model.OverrideOldContextLotAsCompleted(false);
                if (oldContextOverride) showUserMessage("The Lot has been marked as \"Completed\" in the Access system.");
                else showUserMessage("Lab results saved successfully");

                baseSuccessCallback && baseSuccessCallback();
                self.LabResult.notifySubscribers(model.LotKey, 'lotDataChanged');
            };
            var baseErrorCallback = options.errorCallback;
            options.errorCallback = function (xhr, status, message) {
                showUserMessage("Failed to save lab results", { description: message, mode: 'error' });
                baseErrorCallback && baseErrorCallback();
            };

            qualityControlService.saveLotAttributes(
                model.LotKey,
                model.toDto(),
                options
            );

            function isValid() {
                var result = ko.validation.group(model.Attributes, { deep: true });
                var errors = ko.utils.arrayFilter(result(), function (er) { return er != undefined; });
                return errors.length === 0;
            }
        },
        canExecute: function (isExecuting) {
            if (isExecuting) return false;
            var current = self.LabResult();
            return current && current.hasChanges();
        }
    });
    self.initializeNewInhouseDefectCommand = ko.command({
        execute: function () {
            self.NewDefect(lotDefectFactory({ DefectType: rvc.lists.defectTypes.InHouseContamination.key }));
            self.showNewDefectDialog(true);
        },
        canExecute: function () {
            return self.LabResult();
        },
    });
    self.saveNewInhouseDefectCommand = ko.asyncCommand({
        execute: function (complete) {
            var defect = self.NewDefect();
            var validationResult = ko.validation.group(defect);
            if (validationResult() && validationResult().length) {
                validationResult.showAllMessages();
                self.showNewDefectDialog(true);
                complete();
            }

            var current = self.LabResult();
            defect.LotKey = current.LotKey;

            postNewDefectAsync(defect, {
                successCallback: function (data) {
                    defect.LotDefectKey = data;
                    current.Defects.push(defect);
                    showUserMessage("The defect was created successfully.");
                    self.showNewDefectDialog(false);
                    self.NewDefect(null);
                },
                completeCallback: complete,
                errorCallback: function (xhr, status, message) {
                    console.log('Defect creation failed');
                    console.debug(xhr);
                    showUserMessage("Defect creation failed.", { description: message });
                }
            });
        },
        canExecute: function (isExecuting) {
            return !isExecuting && self.NewDefect();
        },
    });
    self.cancelNewDefectCommand = ko.command({
        execute: function () {
            self.NewDefect(null);
            self.showNewDefectDialog(false);
        },
        canExecute: function () {
            return self.NewDefect();
        }
    });
    self.cancelAllEditsCommand = ko.command({
        execute: function () {
            var current = self.LabResult();
            ko.utils.arrayForEach(current.Attributes, function (attr) {
                attr.revertEditsCommand.execute();
            });
            current.OverrideOldContextLotAsCompleted(false);
        },
        canExecute: function () {
            var current = self.LabResult();
            return current != undefined && current.hasChanges();
        }
    });
    self.updateLotStatus = ko.asyncCommand({
        execute: function (callbackOptions, complete) {
            var qualityStatus = self.newLotStatus();
            var lot = self.LabResult.peek();

            updateQualityStatus(ko.utils.unwrapObservable(lot.LotKey), qualityStatus, {
                successCallback: function () {
                    lot.QualityStatus(qualityStatus);
                    self.newLotStatus(null);
                    self.LabResult.notifySubscribers(lot.LotKey, 'lotDataChanged');
                },
                completeCallback: complete,
            });
        },
        canExecute: function (isExecuting) {
            var lot = details();
            return !isExecuting && self.newLotStatus() != undefined && lot != undefined && !lot.hasChanges();
        }
    });
    self.setLotStatusCommand = ko.command({
        execute: function (status) {
            self.newLotStatus(status);
            self.updateLotStatus.execute();
        }
    });
    self.deleteDefectResolutionCommand = ko.asyncCommand({
        execute: function (resolution, complete) {
            var model = self.LabResult.peek();
            qualityControlService.deleteLotDefectResolution(resolution.Defect.LotDefectKey, {
                successCallback: function () {
                    self.LabResult.notifySubscribers(model.LotKey, 'lotDataChanged');
                    showUserMessage('Defect resolution deleted successfully');
                },
                errorCallback: function (xhr, status, message) {
                    showUsermessage('Failed to delete defect resolution', { description: message });
                },
                completeCallback: function () {
                    complete && complete();
                }
            });
        },
        canExecute: function (isExecuting) {
            return !isExecuting;
        }
    });

    return self;

    function mapLabResult(value) {
        value = value || {};
        if (value.isLabResult === true) return value;

        value.customerValidationDisplay = null;
        value.Attributes = ko.utils.arrayMap(value.Attributes, mapAttribute);
        value.OverrideOldContextLotAsCompleted = ko.observable(false);
        value.toDto = buildLabResultDto.bind(value);
        value.hasChanges = ko.computed(function () {
            return value.OverrideOldContextLotAsCompleted() === true ||
                ko.utils.arrayFirst(value.Attributes, function (attr) {
                    return attr.hasChanges() === true;
                }) != undefined;
        });
        var transitions = ko.utils.arrayFilter(value.ValidLotQualityStatuses, function (status) {
            return value.QualityStatus() !== status;
        });
        value.LotQualityStatusOptions = ko.utils.arrayMap(transitions, function (status) {
            return {
                text: getLotQualityStatusOptionText(),
                value: status,
                isCurrent: ko.computed(function () {
                    return false;
                })
            }
            function getLotQualityStatusOptionText() {
                switch (status) {
                    case rvc.lists.lotQualityStatusTypes.Released.value:
                        return "Release Lot";
                    case rvc.lists.lotQualityStatusTypes.Rejected.value:
                        return "Reject Lot";

                    default:
                        return rvc.lists.lotQualityStatusTypes.toDictionary()[status];
                }
            }
        });
        lotHolds.init(value);

        notebooks.createNotebook({ target: value, NotebookKey: value.QualityControlNotebookKey });
        value.isLabResult = true;

        return value;

        // functions
        function mapAttribute(values) {
            //todo: handle clean up of ESM properties!!
            var attribute = new editableLotAttributeFactory(values);
            attribute.CustomerSpec = values.CustomerSpec;
            if (attribute.CustomerSpec) {
                var val = attribute.Value.peek();
                if (val > attribute.CustomerSpec.maxValue || val < attribute.CustomerSpec.minValue) {
                    value.hasCustomerSpecViolation = true;
                    attribute.CustomerSpec.Defect = lotDefectFactory({
                        DefectType: rvc.lists.defectTypes.CustomerProductSpec,
                        Description: ['(', attribute.CustomerSpec.minValue, ' - ', attribute.CustomerSpec.maxValue, ')'].join(''),
                    });

                    value.customerValidationDisplay = "Customer Spec Violation";
                }

            }

            koHelpers.esmHelper(attribute, {
                initializeAsEditing: isNaN(attribute.Value.peek()),
                canEndEditing: function () {
                    var val = attribute.Value();
                    return (val !== null && !isNaN(val)) && (!ko.utils.unwrapObservable(attribute.hasChanges));
                },
                revertChangesCallback: function () {
                    // the canEndEditing option is preventing the endEditingCommand from being executed
                    attribute.endEditingCommand.execute();
                    if (attribute.Defect) {
                        var resolution = attribute.Defect.Resolution();
                        if (resolution && ko.isObservable(resolution.isEditing)) resolution.isEditing(false);
                    }
                }
            });

            buildAttributeResolutionSubscription();

            return attribute;

            function buildAttributeResolutionSubscription() {
                if (!attribute || !attribute.Defect || !attribute.Defect.AttributeDefect) return;

                var resolutionSubscription = attribute.Value.subscribe(handleDefectiveAttributeValueChanged(attribute.Defect));
                self.LabResult.__subscriptions.push(resolutionSubscription);

                function handleDefectiveAttributeValueChanged(defect) {
                    if (!defect || !defect.AttributeDefect) return null;

                    return function (newValue) {
                        var resolution = defect.Resolution();
                        if (valueResolvesDefect()) {
                            if (!resolution) {
                                initiateDefectResolution(defect, false);
                            }
                        } else {
                            if (resolution && resolution.cancelResolutionCommand) {
                                resolution.cancelResolutionCommand.execute();
                            }
                        }


                        function valueResolvesDefect() {
                            return newValue == undefined || isInRange();

                            function isInRange() {
                                return newValue >= defect.AttributeDefect.OriginalMinLimit && newValue <= defect.AttributeDefect.OriginalMaxLimit;
                            }
                        }
                    };
                }
            }
        }        
    }
    function initiateDefectResolution(defect, setResolveDefect) {
        if (!defect) return;
        var defectResolution = defect.Resolution() || {};
        if (defectResolution instanceof lotDefectResolutionFactory) return;
        if (setResolveDefect === undefined) setResolveDefect = true;

        var resolution = new lotDefectResolutionFactory({
            LotDefectKey: defect.LotDefectKey,
            AttributeDefect: defect.AttributeDefect,
            ResolutionType: ko.utils.unwrapObservable(defectResolution.ResolutionType),
            Description: ko.utils.unwrapObservable(defectResolution.Description),
        });
        resolution.isEditing(true);

        resolution.resolveDefectCommand = ko.asyncCommand({
            execute: function (complete) {
                var result = ko.validation.group(resolution);
                if (result() && result().length) {
                    result.showAllMessages(true);
                    complete();
                    return false;
                }

                postDefectResolutionAsync(resolution, {
                    successCallback: function () {
                        showUserMessage("Defect resolved successfully");
                        resolution.isEditing(false);
                    },
                    errorCallback: function (xhr, status, message) {
                        showUserMessage("Defect resolution failed", { description: message });
                    },
                    completeCallback: complete,
                });
            },
            canExecute: function (isExecuting) {
                return !isExecuting;
            }
        });
        resolution.cancelResolutionCommand = ko.command({
            execute: function () {
                self.ResolveDefect(null);
                self.showResolveDefectDialog(false);
                defect.Resolution(null);
            },
            canExecute: function () {
                return resolution.isEditing();
            }
        });

        defect.resolveDefectCommand = ko.asyncCommand({
            execute: function (complete) {
                var resolvingDefect = self.ResolveDefect();
                var resolution = resolvingDefect.Resolution();

                var result = ko.validation.group(resolution);
                if (result() && result().length) {
                    result.showAllMessages(true);
                    complete();
                    return false;
                }

                postDefectResolutionAsync(resolution, {
                    successCallback: function () {
                        showUserMessage("Defect resolved successfully");
                        resolution.isEditing(false);
                        self.ResolveDefect(null);
                        self.showResolveDefectDialog(false);
                    },
                    errorCallback: function (xhr, status, message) {
                        console.log('defect resolution failed: ' + message);
                        console.debug(xhr);
                        showUserMessage("Defect resolution failed", { description: message });
                    },
                    completeCallback: complete,
                });
            },
            canExecute: function (isExecuting) {
                return !isExecuting && self.ResolveDefect();
            }
        });
        defect.cancelResolutionCommand = ko.command({
            execute: function () {
                defect.Resolution(null);
                self.ResolveDefect(null);
                self.showResolveDefectDialog(false);
            },
            canExecute: function () {
                var myResolution = defect.Resolution();
                return myResolution && myResolution.isEditing();
            }
        });
        defect.Resolution(resolution);

        if (setResolveDefect) {
            self.ResolveDefect(defect);
            self.showResolveDefectDialog(true);
        }
    }
    function buildLabResultDto() {
        var attributes = ko.utils.arrayMap(this.Attributes, buildAttributeDto);

        return {
            Attributes: attributes,
            OverrideOldContextLotAsCompleted: this.OverrideOldContextLotAsCompleted()
        };

        function buildAttributeDto(model) {
            return {
                AttributeKey: model.Key,
                NewValue: model.Value(),
                AttributeDate: model.AttributeDate(),
                Resolution: model.Defect ? model.Defect.Resolution() : null,
            };
        }
    }
    function postDefectResolutionAsync(data, callbackOptions) {
        $.ajax({
            url: '/api/defectResolutions',
            contentType: 'application/json',
            method: 'POST',
            data: ko.toJSON(data),
            success: callbackOptions.successCallback,
            error: callbackOptions.errorCallback,
            complete: callbackOptions.completeCallback,
        });
    }
    function postNewDefectAsync(data, callbackOptions) {
        qualityControlService.createDefect(ko.toJSON(data), callbackOptions);
    }
    function updateQualityStatus(lotKey, qualityStatus, callbackOptions) {
        callbackOptions = callbackOptions || {};
        var callbacks = {
            completeCallback: callbackOptions.completeCallback,
            successCallback: function () {
                showUserMessage("Lot Status saved successfully", {
                    description: "The Lot Status has been updated to <strong>" + rvc.lists.lotQualityStatusTypes.findByKey(qualityStatus).value + "</strong>."
                });
                callbackOptions.successCallback && callbackOptions.successCallback.apply(null, arguments);
            },
            errorCallback: function (xhr, status, message) {
                showUserMessage("Failed to save Lot Status", { description: message });
                callbackOptions.errorCallback && callbackOptions.errorCallback.apply(null, arguments);
            }
        };

        qualityControlService.setLotQualityStatus(lotKey, qualityStatus, callbacks);
    }
});