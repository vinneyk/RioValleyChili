ko.components.register('inventory-picker', require('App/components/inventory/inventory-picker/inventory-picker'));
ko.components.register('inventory-filters', require('App/components/common/lot-filters/lot-filters'));
ko.components.register('picked-inventory-table', require('App/components/inventory/inventory-picking-table/inventory-picking-table'));
ko.components.register('auto-inventory-picker', require('App/components/inventory/auto-inventory-picker/auto-inventory-picker'));
ko.components.register('instructions-editor', require('App/components/production/instructions-editor/instructions-editor'));

var pickedItemFactory = require('App/models/PickableInventoryItem'),
    lotService = require('services/lotService');

var PickedIngredientSummary, PickedPackagingSummary, ProductionBatch, ProductionBatchSummary;

define([
        'services/productionBatchingService',
        'app',
        'helpers/koHelpers',
        'ko',
        'scripts/knockout.command',
        'scripts/sh.knockout.customObservables'
],
function (productionService, rvc, koHelpers, ko) {
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
});
