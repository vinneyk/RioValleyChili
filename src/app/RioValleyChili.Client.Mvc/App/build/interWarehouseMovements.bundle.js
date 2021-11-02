webpackJsonp([5],[
/* 0 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {ko.components.register('loading-screen', __webpack_require__(91));
	ko.components.register('inter-warehouse-movement-editor', __webpack_require__(93));
	ko.components.register('inter-warehouse-movement-summary', __webpack_require__(103));
	ko.components.register('post-and-close-inventory-order', __webpack_require__(106));
	ko.components.register('inventory-picker', __webpack_require__(108));
	ko.components.register('inventory-filters', __webpack_require__(118));


	var rvc = __webpack_require__(8),
	    warehouseService = __webpack_require__(6),
	    warehouseLocationsService = __webpack_require__(11),
	    page = __webpack_require__(26);

	__webpack_require__(31);

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

	function InterWarehouseMovementsViewModel() {
	    if (!(this instanceof InterWarehouseMovementsViewModel)) return new InterWarehouseMovementsViewModel();

	    var self = this,
	    inventoryPickerData = ko.observable();

	    // Methods and properties to dispose via .dispose()
	    self.disposables = [];

	    // Initial bindings
	    self.currentMovement = ko.observable();
	    self.warehouses = ko.observableArray();
	    self.destinationFacilityLocations = ko.observableArray();
	    self.searchBoxValue = ko.observable();
	    self.searchKey = ko.observable();
	    self.filtersInput = ko.observable();

	    self.editorExports = ko.observable();
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
	        destinationFacilityKeyFilter: ko.observable(),
	        beginningShipmentDateFilter: ko.observableDate(),
	        endingShipmentDateFilter: ko.observableDate(),
	        orderStatusFilter: ko.observable(),
	        shipmentStatusFilter: ko.observable().extend({ shipmentStatusType: true })
	    };


	    self.isWorking = ko.observable(false);
	    self.loadingMessage = ko.observable('');

	    self.movementKey = ko.observable();
	    self.currentEditorItem = ko.computed(function () {
	        var editor = self.editorExports();
	        var movement = editor && editor.currentMovement();
	        // creating self.movementKey as a computed observable
	        // was resulting in an unexpected behavior in which
	        // the value stopped being updated after loading and closing
	        // a treatment order -- VK 9/25/2015
	        self.movementKey(movement ? movement.MovementKey() : null);
	        return movement;
	    });

	    warehouseService.getWarehouses()
	        .then(function (values) {
	            self.warehouses(values);
	    });

	    self.movementFacility = ko.pureComputed(function () {
	        var movement = self.currentMovement();
	        return  movement &&
	                movement.DestinationFacilityDetails() &&
	                movement.DestinationFacilityDetails().FacilityName;
	    });

	    self.disposables.push(ko.computed(function () {
	        if (self.summaryExports() && self.summaryExports().selectedMovement()) {
	            var key = self.summaryExports().selectedMovement().MovementKey;
	            if (key !== self.movementKey()) self.setSearchKey(key);
	        }
	    }));

	    self.updateItem = function (key) {
	        var dfd = $.Deferred();

	        warehouseService.getInterWarehouseDetails(key)
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
	    self.isEditorControlsShowing = ko.pureComputed(function () {
	        return !self.isPickerShowing() && !self.isPostAndCloseShowing();
	    });
	    self.displayReportLinks = ko.pureComputed(function () {
	        return self.currentEditorItem() && !self.isPickerShowing() && !self.isPostAndCloseShowing();
	    });

	    self.isPickerLoaded = ko.computed(function () {
	        var picker = self.inventoryPickerExports();
	        return picker && picker.isInit() && self.isPickerShowing();
	    });

	    // Bundled data
	    self.inventoryPickerInputs = {
	        args: ko.observable( {} ),
	        pickedInventoryItems: ko.pureComputed(function () {
	            var currentMovement = self.currentMovement(); //raw data
	            return currentMovement && currentMovement.PickedInventory && currentMovement.PickedInventory.PickedInventoryItems;
	        }),
	        pickingContext: rvc.lists.inventoryPickingContexts.TransWarehouseMovements,
	        pickingContextKey: self.movementKey,
	        filters: self.filtersExports,
	        pageSize: 50,
	        targetProduct: ko.observable(  ),
	        targetWeight: ko.observable(  ),
	        customerLotCode: ko.observable(),
	        customerProductCode: ko.observable()
	    };


	    // Behaviors
	    self.createNewMovement = ko.command({
	        execute: function () {
	            page('/new');
	        },
	        canExecute: function () {
	            return true;
	        }
	    });

	    var canMovementBePosted = ko.computed(function () {
	        if (!(self.postAndCloseExports() && !self.isNewMovement())) { return false; }

	        var editor = self.editorExports(),
	            movement = editor && editor.currentMovement(),
	            shipment = movement && movement.Shipment(),
	            pickedInventory = movement && movement.PickedInventory() || [];

	        return editor && shipment && pickedInventory
	            && !editor.isLocked()
	            && pickedInventory.length
	            && shipment.Status < rvc.lists.shipmentStatus.Shipped.key;
	    });

	    self.showPostAndCloseCommand = ko.command({
	        execute: function () {
	            var key = self.currentEditorItem().DestinationFacilityKey();
	            self.getWarehouseLocationsByFacilityAsync(key)
	                .done(function (data) {
	                    self.destinationFacilityLocations(data);
	                    self.isPostAndCloseShowing(true);
	                })
	                .fail(function (xhr, status, message) {
	                    showUserMessage("Failed to load locations for destination facility.", {
	                        description: 'Error: "' + message + '"'
	                    });
	                });
	        },
	        canExecute: function () {
	            return canMovementBePosted();
	        }
	    });
	    self.postAndCloseCommand = ko.asyncCommand({
	        execute: function (complete) {
	            self.postAndCloseExports().postAndCloseAsync()
	                .done(function() {
	                    showUserMessage("Post & close successful", { description: "Inventory has been added to the specified destinations." });
	                    self.updateItem(self.movementKey());
	                    self.cancelPostAndCloseCommand.execute();
	                }).fail(function (xhr, status, message) {
	                    showUserMessage("Failed to post treatment order.", { description: 'Error: "' + message + '"' });
	                }).always(function () {
	                    complete();
	                    self.isWorking(false);
	                });;
	        },
	        canExecute: function () {
	            return canMovementBePosted();
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
	    self.loadInventoryCommand = ko.command({
	        execute: function() {
	            self.inventoryPickerExports().loadInventoryItemsCommand.execute();
	        },
	        canExecute: function() {
	            return self.inventoryPickerExports() && self.inventoryPickerExports().loadInventoryItemsCommand.canExecute();
	        }
	    });

	    self.disposables.push(self.movementKey, self.isWorking);

	    self.saveCommand = ko.asyncCommand({
	        execute: function(complete) {
	            var key = self.isNewMovement() ? null : self.movementKey();
	            if (!self.editorExports().currentMovement.isValid()) {
	                showUserMessage('Validation Errors', { description: 'The Movement contains validation errors. Please correct the errors and retry.' });
	                complete();
	                return;
	            }

	            self.isWorking(true);
	            self.loadingMessage('Saving Movement...');

	            var values = self.editorExports().currentMovement.asDto(),
	                isNew = self.isNewMovement(),
	                dfd = isNew
	                    ? warehouseService.createInterWarehouseMovement(values)
	                    : warehouseService.updateInterWarehouseMovement(key, values);

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
	            return !isExecuting && self.currentEditorItem() && !self.editorExports().isLocked();
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

	    self.pickForItemCommand = ko.command({
	        execute: function(targetProduct) {
	            var input = targetProduct || {};
	            self.isPickerShowing(true);

	            var filters = self.filtersExports();
	            filters.inventoryType(input.filters.inventoryType);
	            filters.lotType(input.filters.lotType);
	            filters.productKey(input.filters.productKey);
	            filters.treatmentKey(input.filters.treatmentKey);
	            filters.packagingProductKey(input.filters.packagingProductKey);

	            self.inventoryPickerInputs.targetProduct( targetProduct.targetProduct );
	            self.inventoryPickerExports().loadInventoryItemsCommand.execute();
	            self.inventoryPickerInputs.targetWeight( targetProduct.targetWeight );
	            //NOTE: WH Transfers are not currently able to maintain their orderItemKey due to limitations in Access data models.
	            //      When the web system is decoupled form Access, this functionality can be restored (pending a refactoring of the association in the web sys models).
	            //self.inventoryPickerInputs.args( { orderItemKey: targetProduct.orderItemKey });
	            //self.inventoryPickerInputs.customerLotCode(input.customerLotCode);
	            //self.inventoryPickerInputs.customerProductCode(input.customerProductCode);
	        },
	        canExecute: function() {
	            var editor = self.editorExports(),
	                        movement = editor && editor.currentMovement(),
	                        shipment = movement && movement.Shipment();
	            return shipment && !self.isNewMovement()
	                && ko.unwrap(movement.OrderStatus) < rvc.lists.orderStatus.Fulfilled.key
	                && ko.unwrap(shipment.Status) < rvc.lists.shipmentStatus.Shipped.key;
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


	    ko.postbox.subscribe('PickedItemsSaved', function (values) {
	        self.isWorking(true);
	        self.loadingMessage('Updating movement data...');
	        self.updateItem(self.movementKey()).always(function () {
	            self.isPickerShowing(false);
	            self.isWorking(false);
	        });
	    });

	    // Routing
	    var currentMovementKey;

	    page.base('/Warehouse/InterWarehouseMovements');
	    page('/:key?', navigateToMovement);

	    function navigateToMovement(ctx) {
	        var transferKey = ctx.params.key;
	        clearMovements();

	        // No routing specified
	        if (ctx.params.key == undefined) {
	            currentMovementKey = undefined;
	            document.title = "Movements Between Facilities";
	            self.isWorking(false);

	            return;
	        }

	        // New entry
	        if (transferKey === 'new') {
	            currentMovementKey = null;
	            document.title = "Create New Inter-Warehouse Movement";

	            var editor = self.editorExports();
	            if (editor) {
	                self.initNewMovementOrder();
	            } else {
	                var editorSubscription = self.editorExports.subscribe(function () {
	                    self.initNewMovementOrder();
	                    editorSubscription.dispose();
	                    ko.utils.arrayRemoveItem(self.disposables, editorSubscription);
	                });
	                self.disposables.push(editorSubscription);
	            }

	            return;
	        }

	        // Existing entry
	        if (currentMovementKey !== transferKey) {
	            currentMovementKey = transferKey;
	            self.loadInterWarehouseMovementByKey(transferKey);
	            return;
	        }
	    }
	}

	InterWarehouseMovementsViewModel.prototype.initNewMovementOrder = function () {
	    this.currentMovement({});
	}
	InterWarehouseMovementsViewModel.prototype.loadInterWarehouseMovementByKey = function (key) {
	    var dfd = warehouseService.getInterWarehouseDetails(key),
	        self = this;

	    self.isWorking(true);
	    self.loadingMessage('Loading movement #' + key + '...');

	    dfd.done(function (values) {
	        self.currentMovement(values);
	        document.title = 'Movement Order #' + key;
	    }).fail(function () {
	        showUserMessage("No Treatment Order Found", {
	            description: "The Movement Order # " + key + " is invalid or doesn't exist. If you believe this is an error, please contact system administrators to report the issue."
	        });
	        self.currentMovement(null);
	    }).always(function () {
	        self.isWorking(false);
	    });

	    return dfd;
	}

	var facilityLocations = {};
	InterWarehouseMovementsViewModel.prototype.getWarehouseLocationsByFacilityAsync = function (facilityKey) {
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
	}

	InterWarehouseMovementsViewModel.prototype.dispose = function () {
	    ko.utils.arrayForEach(this.disposables, this.disposeOne);
	    ko.utils.objectForEach(this, this.disposeOne);
	};

	InterWarehouseMovementsViewModel.prototype.disposeOne = function(propOrValue, value) {
	    var disposable = value || propOrValue;

	    if (disposable && typeof disposable.dispose === "function") {
	        disposable.dispose();
	    }
	};

	var vm = new InterWarehouseMovementsViewModel();

	ko.applyBindings(vm);
	ko.punches.enableAll();

	page();

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 1 */,
/* 2 */,
/* 3 */,
/* 4 */,
/* 5 */
/***/ (function(module, exports, __webpack_require__) {

	var warehouseService = __webpack_require__(6);

	/**
	  * @param {Object} input - Observable, Contact label data
	  * @param {boolean} [locked=false] - Disables editing of contact
	  * @param {Object} exports - Observable, container for exposed methods and properties
	  */
	function ContactLabelEditorViewModel(params) {
	    var self = this,
	        input = ko.toJS(params.input) || {};

	    self.disposables = [];

	    self.isLocked = params.locked || null;

	    self.Name = ko.observable(input.Name);
	    self.Phone = ko.observable(input.Phone);
	    self.EMail = ko.observable(input.EMail);
	    self.Fax = ko.observable(input.Fax);
	    self.Address = ko.observable(input.Address);

	    self.addressExports = ko.observable();

	    if (ko.isObservable(params.input)) {
	      self.disposables.push([
	        params.input.subscribe(function(values) {
	          input = ko.toJS(values || {});

	          if (input.hasOwnProperty('Address')) {
	            self.Name(input.Name);
	            self.Phone(input.Phone);
	            self.EMail(input.EMail);
	            self.Fax(input.Fax);
	            self.Address(input.Address);
	          } else {
	            self.Address(input);
	          }
	        })
	      ]);
	    }

	  // Output
	    if ( params && params.exports ) {
	      params.exports({
	          Name: self.Name,
	          Phone: self.Phone,
	          EMail: self.EMail,
	          Fax: self.Fax,
	          Address: self.addressExports
	      });
	    }
	}

	ko.utils.extend(ContactLabelEditorViewModel, {
	    dispose: function () {
	        ko.utils.arrayForEach(this.disposables, this.disposeOne);
	        ko.utils.objectForEach(this, this.disposeOne);
	    },

	    disposeOne: function(propOrValue, value) {
	        var disposable = value || propOrValue;

	        if (disposable && typeof disposable.dispose === "function") {
	            disposable.dispose();
	        }
	    }
	});

	ko.components.register('address-editor', __webpack_require__(12));

	module.exports = {
	    viewModel: ContactLabelEditorViewModel,
	    template: __webpack_require__(14)
	};


/***/ }),
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
/* 12 */
/***/ (function(module, exports, __webpack_require__) {

	var EditAddressViewModel = function (params) {
	    var self = this,
	        values = ko.toJS(params.input) || {};

	    self.disposables = [];

	    self.isLocked = params.locked || null;

	    // Defines initial observables
	    self.AddressLine1 = ko.observable(values.AddressLine1);
	    self.AddressLine2 = ko.observable(values.AddressLine2);
	    self.AddressLine3 = ko.observable(values.AddressLine3);
	    self.City = ko.observable(values.City);
	    self.State = ko.observable(values.State);
	    self.PostalCode = ko.observable(values.PostalCode);
	    self.Country = ko.observable(values.Country);

	    // Updates observables when input changes
	    self.disposables.push(params.input.subscribe(function (input) {
	        values = ko.toJS(input) || {};

	        self.AddressLine1(values.AddressLine1);
	        self.AddressLine2(values.AddressLine2);
	        self.AddressLine3(values.AddressLine3);
	        self.City(values.City);
	        self.State(values.State);
	        self.PostalCode(values.PostalCode);
	        self.Country(values.Country);
	    }));

	    // Output all values
	    params.output(self);
	};

	ko.utils.extend(EditAddressViewModel, {
	    dispose: function () {
	        ko.utils.arrayForEach(this.disposables, this.disposeOne);
	        ko.utils.objectForEach(this, this.disposeOne);
	    },

	    disposeOne: function(propOrValue, value) {
	        var disposable = value || propOrValue;

	        if (disposable && typeof disposable.dispose === "function") {
	            disposable.dispose();
	        }
	    }
	});

	module.exports = { viewModel: EditAddressViewModel, template: __webpack_require__(13)};


/***/ }),
/* 13 */
/***/ (function(module, exports) {

	module.exports = "<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">Address line 1: </label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"Address\" data-bind=\"value: AddressLine1, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">Address line 2: </label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"Address\" data-bind=\"value: AddressLine2, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">Address line 3: </label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"Address\" data-bind=\"value: AddressLine3, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">City: </label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"City\" data-bind=\"value: City, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">State: </label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"State\" data-bind=\"value: State, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">Postal Code: </label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"Postal Code\" data-bind=\"value: PostalCode, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">Country: </label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"Country\" data-bind=\"value: Country, disable: isLocked\" />\r\n</div>\r\n"

/***/ }),
/* 14 */
/***/ (function(module, exports) {

	module.exports = "<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">Name</label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"Name\" data-bind=\"value: Name, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">Phone</label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"Phone\" data-bind=\"value: Phone, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">Email</label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"Email\" data-bind=\"value: EMail, disable: isLocked\" />\r\n</div>\r\n<div class=\"form-group\">\r\n    <label class=\"sr-only control-label\">Fax</label>\r\n    <input type=\"text\" class=\"form-control\" placeholder=\"Fax\" data-bind=\"value: Fax, disable: isLocked\" />\r\n</div>\r\n<address-editor params=\"input: Address, output: addressExports, locked: isLocked\"></address-editor>\r\n"

/***/ }),
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
/* 20 */,
/* 21 */,
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
/* 25 */,
/* 26 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(process) {  /* globals require, module */

	  'use strict';

	  /**
	   * Module dependencies.
	   */

	  var pathtoRegexp = __webpack_require__(28);

	  /**
	   * Module exports.
	   */

	  module.exports = page;

	  /**
	   * Detect click event
	   */
	  var clickEvent = ('undefined' !== typeof document) && document.ontouchstart ? 'touchstart' : 'click';

	  /**
	   * To work properly with the URL
	   * history.location generated polyfill in https://github.com/devote/HTML5-History-API
	   */

	  var location = ('undefined' !== typeof window) && (window.history.location || window.location);

	  /**
	   * Perform initial dispatch.
	   */

	  var dispatch = true;


	  /**
	   * Decode URL components (query string, pathname, hash).
	   * Accommodates both regular percent encoding and x-www-form-urlencoded format.
	   */
	  var decodeURLComponents = true;

	  /**
	   * Base path.
	   */

	  var base = '';

	  /**
	   * Running flag.
	   */

	  var running;

	  /**
	   * HashBang option
	   */

	  var hashbang = false;

	  /**
	   * Previous context, for capturing
	   * page exit events.
	   */

	  var prevContext;

	  /**
	   * Register `path` with callback `fn()`,
	   * or route `path`, or redirection,
	   * or `page.start()`.
	   *
	   *   page(fn);
	   *   page('*', fn);
	   *   page('/user/:id', load, user);
	   *   page('/user/' + user.id, { some: 'thing' });
	   *   page('/user/' + user.id);
	   *   page('/from', '/to')
	   *   page();
	   *
	   * @param {string|!Function|!Object} path
	   * @param {Function=} fn
	   * @api public
	   */

	  function page(path, fn) {
	    // <callback>
	    if ('function' === typeof path) {
	      return page('*', path);
	    }

	    // route <path> to <callback ...>
	    if ('function' === typeof fn) {
	      var route = new Route(/** @type {string} */ (path));
	      for (var i = 1; i < arguments.length; ++i) {
	        page.callbacks.push(route.middleware(arguments[i]));
	      }
	      // show <path> with [state]
	    } else if ('string' === typeof path) {
	      page['string' === typeof fn ? 'redirect' : 'show'](path, fn);
	      // start [options]
	    } else {
	      page.start(path);
	    }
	  }

	  /**
	   * Callback functions.
	   */

	  page.callbacks = [];
	  page.exits = [];

	  /**
	   * Current path being processed
	   * @type {string}
	   */
	  page.current = '';

	  /**
	   * Number of pages navigated to.
	   * @type {number}
	   *
	   *     page.len == 0;
	   *     page('/login');
	   *     page.len == 1;
	   */

	  page.len = 0;

	  /**
	   * Get or set basepath to `path`.
	   *
	   * @param {string} path
	   * @api public
	   */

	  page.base = function(path) {
	    if (0 === arguments.length) return base;
	    base = path;
	  };

	  /**
	   * Bind with the given `options`.
	   *
	   * Options:
	   *
	   *    - `click` bind to click events [true]
	   *    - `popstate` bind to popstate [true]
	   *    - `dispatch` perform initial dispatch [true]
	   *
	   * @param {Object} options
	   * @api public
	   */

	  page.start = function(options) {
	    options = options || {};
	    if (running) return;
	    running = true;
	    if (false === options.dispatch) dispatch = false;
	    if (false === options.decodeURLComponents) decodeURLComponents = false;
	    if (false !== options.popstate) window.addEventListener('popstate', onpopstate, false);
	    if (false !== options.click) {
	      document.addEventListener(clickEvent, onclick, false);
	    }
	    if (true === options.hashbang) hashbang = true;
	    if (!dispatch) return;
	    var url = (hashbang && ~location.hash.indexOf('#!')) ? location.hash.substr(2) + location.search : location.pathname + location.search + location.hash;
	    page.replace(url, null, true, dispatch);
	  };

	  /**
	   * Unbind click and popstate event handlers.
	   *
	   * @api public
	   */

	  page.stop = function() {
	    if (!running) return;
	    page.current = '';
	    page.len = 0;
	    running = false;
	    document.removeEventListener(clickEvent, onclick, false);
	    window.removeEventListener('popstate', onpopstate, false);
	  };

	  /**
	   * Show `path` with optional `state` object.
	   *
	   * @param {string} path
	   * @param {Object=} state
	   * @param {boolean=} dispatch
	   * @param {boolean=} push
	   * @return {!Context}
	   * @api public
	   */

	  page.show = function(path, state, dispatch, push) {
	    var ctx = new Context(path, state);
	    page.current = ctx.path;
	    if (false !== dispatch) page.dispatch(ctx);
	    if (false !== ctx.handled && false !== push) ctx.pushState();
	    return ctx;
	  };

	  /**
	   * Goes back in the history
	   * Back should always let the current route push state and then go back.
	   *
	   * @param {string} path - fallback path to go back if no more history exists, if undefined defaults to page.base
	   * @param {Object=} state
	   * @api public
	   */

	  page.back = function(path, state) {
	    if (page.len > 0) {
	      // this may need more testing to see if all browsers
	      // wait for the next tick to go back in history
	      history.back();
	      page.len--;
	    } else if (path) {
	      setTimeout(function() {
	        page.show(path, state);
	      });
	    }else{
	      setTimeout(function() {
	        page.show(base, state);
	      });
	    }
	  };


	  /**
	   * Register route to redirect from one path to other
	   * or just redirect to another route
	   *
	   * @param {string} from - if param 'to' is undefined redirects to 'from'
	   * @param {string=} to
	   * @api public
	   */
	  page.redirect = function(from, to) {
	    // Define route from a path to another
	    if ('string' === typeof from && 'string' === typeof to) {
	      page(from, function(e) {
	        setTimeout(function() {
	          page.replace(/** @type {!string} */ (to));
	        }, 0);
	      });
	    }

	    // Wait for the push state and replace it with another
	    if ('string' === typeof from && 'undefined' === typeof to) {
	      setTimeout(function() {
	        page.replace(from);
	      }, 0);
	    }
	  };

	  /**
	   * Replace `path` with optional `state` object.
	   *
	   * @param {string} path
	   * @param {Object=} state
	   * @param {boolean=} init
	   * @param {boolean=} dispatch
	   * @return {!Context}
	   * @api public
	   */


	  page.replace = function(path, state, init, dispatch) {
	    var ctx = new Context(path, state);
	    page.current = ctx.path;
	    ctx.init = init;
	    ctx.save(); // save before dispatching, which may redirect
	    if (false !== dispatch) page.dispatch(ctx);
	    return ctx;
	  };

	  /**
	   * Dispatch the given `ctx`.
	   *
	   * @param {Context} ctx
	   * @api private
	   */
	  page.dispatch = function(ctx) {
	    var prev = prevContext,
	      i = 0,
	      j = 0;

	    prevContext = ctx;

	    function nextExit() {
	      var fn = page.exits[j++];
	      if (!fn) return nextEnter();
	      fn(prev, nextExit);
	    }

	    function nextEnter() {
	      var fn = page.callbacks[i++];

	      if (ctx.path !== page.current) {
	        ctx.handled = false;
	        return;
	      }
	      if (!fn) return unhandled(ctx);
	      fn(ctx, nextEnter);
	    }

	    if (prev) {
	      nextExit();
	    } else {
	      nextEnter();
	    }
	  };

	  /**
	   * Unhandled `ctx`. When it's not the initial
	   * popstate then redirect. If you wish to handle
	   * 404s on your own use `page('*', callback)`.
	   *
	   * @param {Context} ctx
	   * @api private
	   */
	  function unhandled(ctx) {
	    if (ctx.handled) return;
	    var current;

	    if (hashbang) {
	      current = base + location.hash.replace('#!', '');
	    } else {
	      current = location.pathname + location.search;
	    }

	    if (current === ctx.canonicalPath) return;
	    page.stop();
	    ctx.handled = false;
	    location.href = ctx.canonicalPath;
	  }

	  /**
	   * Register an exit route on `path` with
	   * callback `fn()`, which will be called
	   * on the previous context when a new
	   * page is visited.
	   */
	  page.exit = function(path, fn) {
	    if (typeof path === 'function') {
	      return page.exit('*', path);
	    }

	    var route = new Route(path);
	    for (var i = 1; i < arguments.length; ++i) {
	      page.exits.push(route.middleware(arguments[i]));
	    }
	  };

	  /**
	   * Remove URL encoding from the given `str`.
	   * Accommodates whitespace in both x-www-form-urlencoded
	   * and regular percent-encoded form.
	   *
	   * @param {string} val - URL component to decode
	   */
	  function decodeURLEncodedURIComponent(val) {
	    if (typeof val !== 'string') { return val; }
	    return decodeURLComponents ? decodeURIComponent(val.replace(/\+/g, ' ')) : val;
	  }

	  /**
	   * Initialize a new "request" `Context`
	   * with the given `path` and optional initial `state`.
	   *
	   * @constructor
	   * @param {string} path
	   * @param {Object=} state
	   * @api public
	   */

	  function Context(path, state) {
	    if ('/' === path[0] && 0 !== path.indexOf(base)) path = base + (hashbang ? '#!' : '') + path;
	    var i = path.indexOf('?');

	    this.canonicalPath = path;
	    this.path = path.replace(base, '') || '/';
	    if (hashbang) this.path = this.path.replace('#!', '') || '/';

	    this.title = document.title;
	    this.state = state || {};
	    this.state.path = path;
	    this.querystring = ~i ? decodeURLEncodedURIComponent(path.slice(i + 1)) : '';
	    this.pathname = decodeURLEncodedURIComponent(~i ? path.slice(0, i) : path);
	    this.params = {};

	    // fragment
	    this.hash = '';
	    if (!hashbang) {
	      if (!~this.path.indexOf('#')) return;
	      var parts = this.path.split('#');
	      this.path = parts[0];
	      this.hash = decodeURLEncodedURIComponent(parts[1]) || '';
	      this.querystring = this.querystring.split('#')[0];
	    }
	  }

	  /**
	   * Expose `Context`.
	   */

	  page.Context = Context;

	  /**
	   * Push state.
	   *
	   * @api private
	   */

	  Context.prototype.pushState = function() {
	    page.len++;
	    history.pushState(this.state, this.title, hashbang && this.path !== '/' ? '#!' + this.path : this.canonicalPath);
	  };

	  /**
	   * Save the context state.
	   *
	   * @api public
	   */

	  Context.prototype.save = function() {
	    history.replaceState(this.state, this.title, hashbang && this.path !== '/' ? '#!' + this.path : this.canonicalPath);
	  };

	  /**
	   * Initialize `Route` with the given HTTP `path`,
	   * and an array of `callbacks` and `options`.
	   *
	   * Options:
	   *
	   *   - `sensitive`    enable case-sensitive routes
	   *   - `strict`       enable strict matching for trailing slashes
	   *
	   * @constructor
	   * @param {string} path
	   * @param {Object=} options
	   * @api private
	   */

	  function Route(path, options) {
	    options = options || {};
	    this.path = (path === '*') ? '(.*)' : path;
	    this.method = 'GET';
	    this.regexp = pathtoRegexp(this.path,
	      this.keys = [],
	      options);
	  }

	  /**
	   * Expose `Route`.
	   */

	  page.Route = Route;

	  /**
	   * Return route middleware with
	   * the given callback `fn()`.
	   *
	   * @param {Function} fn
	   * @return {Function}
	   * @api public
	   */

	  Route.prototype.middleware = function(fn) {
	    var self = this;
	    return function(ctx, next) {
	      if (self.match(ctx.path, ctx.params)) return fn(ctx, next);
	      next();
	    };
	  };

	  /**
	   * Check if this route matches `path`, if so
	   * populate `params`.
	   *
	   * @param {string} path
	   * @param {Object} params
	   * @return {boolean}
	   * @api private
	   */

	  Route.prototype.match = function(path, params) {
	    var keys = this.keys,
	      qsIndex = path.indexOf('?'),
	      pathname = ~qsIndex ? path.slice(0, qsIndex) : path,
	      m = this.regexp.exec(decodeURIComponent(pathname));

	    if (!m) return false;

	    for (var i = 1, len = m.length; i < len; ++i) {
	      var key = keys[i - 1];
	      var val = decodeURLEncodedURIComponent(m[i]);
	      if (val !== undefined || !(hasOwnProperty.call(params, key.name))) {
	        params[key.name] = val;
	      }
	    }

	    return true;
	  };


	  /**
	   * Handle "populate" events.
	   */

	  var onpopstate = (function () {
	    var loaded = false;
	    if ('undefined' === typeof window) {
	      return;
	    }
	    if (document.readyState === 'complete') {
	      loaded = true;
	    } else {
	      window.addEventListener('load', function() {
	        setTimeout(function() {
	          loaded = true;
	        }, 0);
	      });
	    }
	    return function onpopstate(e) {
	      if (!loaded) return;
	      if (e.state) {
	        var path = e.state.path;
	        page.replace(path, e.state);
	      } else {
	        page.show(location.pathname + location.hash, undefined, undefined, false);
	      }
	    };
	  })();
	  /**
	   * Handle "click" events.
	   */

	  function onclick(e) {

	    if (1 !== which(e)) return;

	    if (e.metaKey || e.ctrlKey || e.shiftKey) return;
	    if (e.defaultPrevented) return;



	    // ensure link
	    // use shadow dom when available
	    var el = e.path ? e.path[0] : e.target;
	    while (el && 'A' !== el.nodeName) el = el.parentNode;
	    if (!el || 'A' !== el.nodeName) return;



	    // Ignore if tag has
	    // 1. "download" attribute
	    // 2. rel="external" attribute
	    if (el.hasAttribute('download') || el.getAttribute('rel') === 'external') return;

	    // ensure non-hash for the same path
	    var link = el.getAttribute('href');
	    if (!hashbang && el.pathname === location.pathname && (el.hash || '#' === link)) return;



	    // Check for mailto: in the href
	    if (link && link.indexOf('mailto:') > -1) return;

	    // check target
	    if (el.target) return;

	    // x-origin
	    if (!sameOrigin(el.href)) return;



	    // rebuild path
	    var path = el.pathname + el.search + (el.hash || '');

	    // strip leading "/[drive letter]:" on NW.js on Windows
	    if (typeof process !== 'undefined' && path.match(/^\/[a-zA-Z]:\//)) {
	      path = path.replace(/^\/[a-zA-Z]:\//, '/');
	    }

	    // same page
	    var orig = path;

	    if (path.indexOf(base) === 0) {
	      path = path.substr(base.length);
	    }

	    if (hashbang) path = path.replace('#!', '');

	    if (base && orig === path) return;

	    e.preventDefault();
	    page.show(orig);
	  }

	  /**
	   * Event button.
	   */

	  function which(e) {
	    e = e || window.event;
	    return null === e.which ? e.button : e.which;
	  }

	  /**
	   * Check if `href` is the same origin.
	   */

	  function sameOrigin(href) {
	    var origin = location.protocol + '//' + location.hostname;
	    if (location.port) origin += ':' + location.port;
	    return (href && (0 === href.indexOf(origin)));
	  }

	  page.sameOrigin = sameOrigin;

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(27)))

/***/ }),
/* 27 */
/***/ (function(module, exports) {

	// shim for using process in browser
	var process = module.exports = {};

	// cached from whatever global is present so that test runners that stub it
	// don't break things.  But we need to wrap it in a try catch in case it is
	// wrapped in strict mode code which doesn't define any globals.  It's inside a
	// function because try/catches deoptimize in certain engines.

	var cachedSetTimeout;
	var cachedClearTimeout;

	function defaultSetTimout() {
	    throw new Error('setTimeout has not been defined');
	}
	function defaultClearTimeout () {
	    throw new Error('clearTimeout has not been defined');
	}
	(function () {
	    try {
	        if (typeof setTimeout === 'function') {
	            cachedSetTimeout = setTimeout;
	        } else {
	            cachedSetTimeout = defaultSetTimout;
	        }
	    } catch (e) {
	        cachedSetTimeout = defaultSetTimout;
	    }
	    try {
	        if (typeof clearTimeout === 'function') {
	            cachedClearTimeout = clearTimeout;
	        } else {
	            cachedClearTimeout = defaultClearTimeout;
	        }
	    } catch (e) {
	        cachedClearTimeout = defaultClearTimeout;
	    }
	} ())
	function runTimeout(fun) {
	    if (cachedSetTimeout === setTimeout) {
	        //normal enviroments in sane situations
	        return setTimeout(fun, 0);
	    }
	    // if setTimeout wasn't available but was latter defined
	    if ((cachedSetTimeout === defaultSetTimout || !cachedSetTimeout) && setTimeout) {
	        cachedSetTimeout = setTimeout;
	        return setTimeout(fun, 0);
	    }
	    try {
	        // when when somebody has screwed with setTimeout but no I.E. maddness
	        return cachedSetTimeout(fun, 0);
	    } catch(e){
	        try {
	            // When we are in I.E. but the script has been evaled so I.E. doesn't trust the global object when called normally
	            return cachedSetTimeout.call(null, fun, 0);
	        } catch(e){
	            // same as above but when it's a version of I.E. that must have the global object for 'this', hopfully our context correct otherwise it will throw a global error
	            return cachedSetTimeout.call(this, fun, 0);
	        }
	    }


	}
	function runClearTimeout(marker) {
	    if (cachedClearTimeout === clearTimeout) {
	        //normal enviroments in sane situations
	        return clearTimeout(marker);
	    }
	    // if clearTimeout wasn't available but was latter defined
	    if ((cachedClearTimeout === defaultClearTimeout || !cachedClearTimeout) && clearTimeout) {
	        cachedClearTimeout = clearTimeout;
	        return clearTimeout(marker);
	    }
	    try {
	        // when when somebody has screwed with setTimeout but no I.E. maddness
	        return cachedClearTimeout(marker);
	    } catch (e){
	        try {
	            // When we are in I.E. but the script has been evaled so I.E. doesn't  trust the global object when called normally
	            return cachedClearTimeout.call(null, marker);
	        } catch (e){
	            // same as above but when it's a version of I.E. that must have the global object for 'this', hopfully our context correct otherwise it will throw a global error.
	            // Some versions of I.E. have different rules for clearTimeout vs setTimeout
	            return cachedClearTimeout.call(this, marker);
	        }
	    }



	}
	var queue = [];
	var draining = false;
	var currentQueue;
	var queueIndex = -1;

	function cleanUpNextTick() {
	    if (!draining || !currentQueue) {
	        return;
	    }
	    draining = false;
	    if (currentQueue.length) {
	        queue = currentQueue.concat(queue);
	    } else {
	        queueIndex = -1;
	    }
	    if (queue.length) {
	        drainQueue();
	    }
	}

	function drainQueue() {
	    if (draining) {
	        return;
	    }
	    var timeout = runTimeout(cleanUpNextTick);
	    draining = true;

	    var len = queue.length;
	    while(len) {
	        currentQueue = queue;
	        queue = [];
	        while (++queueIndex < len) {
	            if (currentQueue) {
	                currentQueue[queueIndex].run();
	            }
	        }
	        queueIndex = -1;
	        len = queue.length;
	    }
	    currentQueue = null;
	    draining = false;
	    runClearTimeout(timeout);
	}

	process.nextTick = function (fun) {
	    var args = new Array(arguments.length - 1);
	    if (arguments.length > 1) {
	        for (var i = 1; i < arguments.length; i++) {
	            args[i - 1] = arguments[i];
	        }
	    }
	    queue.push(new Item(fun, args));
	    if (queue.length === 1 && !draining) {
	        runTimeout(drainQueue);
	    }
	};

	// v8 likes predictible objects
	function Item(fun, array) {
	    this.fun = fun;
	    this.array = array;
	}
	Item.prototype.run = function () {
	    this.fun.apply(null, this.array);
	};
	process.title = 'browser';
	process.browser = true;
	process.env = {};
	process.argv = [];
	process.version = ''; // empty string to avoid regexp issues
	process.versions = {};

	function noop() {}

	process.on = noop;
	process.addListener = noop;
	process.once = noop;
	process.off = noop;
	process.removeListener = noop;
	process.removeAllListeners = noop;
	process.emit = noop;
	process.prependListener = noop;
	process.prependOnceListener = noop;

	process.listeners = function (name) { return [] }

	process.binding = function (name) {
	    throw new Error('process.binding is not supported');
	};

	process.cwd = function () { return '/' };
	process.chdir = function (dir) {
	    throw new Error('process.chdir is not supported');
	};
	process.umask = function() { return 0; };


/***/ }),
/* 28 */
/***/ (function(module, exports, __webpack_require__) {

	var isarray = __webpack_require__(29)

	/**
	 * Expose `pathToRegexp`.
	 */
	module.exports = pathToRegexp
	module.exports.parse = parse
	module.exports.compile = compile
	module.exports.tokensToFunction = tokensToFunction
	module.exports.tokensToRegExp = tokensToRegExp

	/**
	 * The main path matching regexp utility.
	 *
	 * @type {RegExp}
	 */
	var PATH_REGEXP = new RegExp([
	  // Match escaped characters that would otherwise appear in future matches.
	  // This allows the user to escape special characters that won't transform.
	  '(\\\\.)',
	  // Match Express-style parameters and un-named parameters with a prefix
	  // and optional suffixes. Matches appear as:
	  //
	  // "/:test(\\d+)?" => ["/", "test", "\d+", undefined, "?", undefined]
	  // "/route(\\d+)"  => [undefined, undefined, undefined, "\d+", undefined, undefined]
	  // "/*"            => ["/", undefined, undefined, undefined, undefined, "*"]
	  '([\\/.])?(?:(?:\\:(\\w+)(?:\\(((?:\\\\.|[^()])+)\\))?|\\(((?:\\\\.|[^()])+)\\))([+*?])?|(\\*))'
	].join('|'), 'g')

	/**
	 * Parse a string for the raw tokens.
	 *
	 * @param  {String} str
	 * @return {Array}
	 */
	function parse (str) {
	  var tokens = []
	  var key = 0
	  var index = 0
	  var path = ''
	  var res

	  while ((res = PATH_REGEXP.exec(str)) != null) {
	    var m = res[0]
	    var escaped = res[1]
	    var offset = res.index
	    path += str.slice(index, offset)
	    index = offset + m.length

	    // Ignore already escaped sequences.
	    if (escaped) {
	      path += escaped[1]
	      continue
	    }

	    // Push the current path onto the tokens.
	    if (path) {
	      tokens.push(path)
	      path = ''
	    }

	    var prefix = res[2]
	    var name = res[3]
	    var capture = res[4]
	    var group = res[5]
	    var suffix = res[6]
	    var asterisk = res[7]

	    var repeat = suffix === '+' || suffix === '*'
	    var optional = suffix === '?' || suffix === '*'
	    var delimiter = prefix || '/'
	    var pattern = capture || group || (asterisk ? '.*' : '[^' + delimiter + ']+?')

	    tokens.push({
	      name: name || key++,
	      prefix: prefix || '',
	      delimiter: delimiter,
	      optional: optional,
	      repeat: repeat,
	      pattern: escapeGroup(pattern)
	    })
	  }

	  // Match any characters still remaining.
	  if (index < str.length) {
	    path += str.substr(index)
	  }

	  // If the path exists, push it onto the end.
	  if (path) {
	    tokens.push(path)
	  }

	  return tokens
	}

	/**
	 * Compile a string to a template function for the path.
	 *
	 * @param  {String}   str
	 * @return {Function}
	 */
	function compile (str) {
	  return tokensToFunction(parse(str))
	}

	/**
	 * Expose a method for transforming tokens into the path function.
	 */
	function tokensToFunction (tokens) {
	  // Compile all the tokens into regexps.
	  var matches = new Array(tokens.length)

	  // Compile all the patterns before compilation.
	  for (var i = 0; i < tokens.length; i++) {
	    if (typeof tokens[i] === 'object') {
	      matches[i] = new RegExp('^' + tokens[i].pattern + '$')
	    }
	  }

	  return function (obj) {
	    var path = ''
	    var data = obj || {}

	    for (var i = 0; i < tokens.length; i++) {
	      var token = tokens[i]

	      if (typeof token === 'string') {
	        path += token

	        continue
	      }

	      var value = data[token.name]
	      var segment

	      if (value == null) {
	        if (token.optional) {
	          continue
	        } else {
	          throw new TypeError('Expected "' + token.name + '" to be defined')
	        }
	      }

	      if (isarray(value)) {
	        if (!token.repeat) {
	          throw new TypeError('Expected "' + token.name + '" to not repeat, but received "' + value + '"')
	        }

	        if (value.length === 0) {
	          if (token.optional) {
	            continue
	          } else {
	            throw new TypeError('Expected "' + token.name + '" to not be empty')
	          }
	        }

	        for (var j = 0; j < value.length; j++) {
	          segment = encodeURIComponent(value[j])

	          if (!matches[i].test(segment)) {
	            throw new TypeError('Expected all "' + token.name + '" to match "' + token.pattern + '", but received "' + segment + '"')
	          }

	          path += (j === 0 ? token.prefix : token.delimiter) + segment
	        }

	        continue
	      }

	      segment = encodeURIComponent(value)

	      if (!matches[i].test(segment)) {
	        throw new TypeError('Expected "' + token.name + '" to match "' + token.pattern + '", but received "' + segment + '"')
	      }

	      path += token.prefix + segment
	    }

	    return path
	  }
	}

	/**
	 * Escape a regular expression string.
	 *
	 * @param  {String} str
	 * @return {String}
	 */
	function escapeString (str) {
	  return str.replace(/([.+*?=^!:${}()[\]|\/])/g, '\\$1')
	}

	/**
	 * Escape the capturing group by escaping special characters and meaning.
	 *
	 * @param  {String} group
	 * @return {String}
	 */
	function escapeGroup (group) {
	  return group.replace(/([=!:$\/()])/g, '\\$1')
	}

	/**
	 * Attach the keys as a property of the regexp.
	 *
	 * @param  {RegExp} re
	 * @param  {Array}  keys
	 * @return {RegExp}
	 */
	function attachKeys (re, keys) {
	  re.keys = keys
	  return re
	}

	/**
	 * Get the flags for a regexp from the options.
	 *
	 * @param  {Object} options
	 * @return {String}
	 */
	function flags (options) {
	  return options.sensitive ? '' : 'i'
	}

	/**
	 * Pull out keys from a regexp.
	 *
	 * @param  {RegExp} path
	 * @param  {Array}  keys
	 * @return {RegExp}
	 */
	function regexpToRegexp (path, keys) {
	  // Use a negative lookahead to match only capturing groups.
	  var groups = path.source.match(/\((?!\?)/g)

	  if (groups) {
	    for (var i = 0; i < groups.length; i++) {
	      keys.push({
	        name: i,
	        prefix: null,
	        delimiter: null,
	        optional: false,
	        repeat: false,
	        pattern: null
	      })
	    }
	  }

	  return attachKeys(path, keys)
	}

	/**
	 * Transform an array into a regexp.
	 *
	 * @param  {Array}  path
	 * @param  {Array}  keys
	 * @param  {Object} options
	 * @return {RegExp}
	 */
	function arrayToRegexp (path, keys, options) {
	  var parts = []

	  for (var i = 0; i < path.length; i++) {
	    parts.push(pathToRegexp(path[i], keys, options).source)
	  }

	  var regexp = new RegExp('(?:' + parts.join('|') + ')', flags(options))

	  return attachKeys(regexp, keys)
	}

	/**
	 * Create a path regexp from string input.
	 *
	 * @param  {String} path
	 * @param  {Array}  keys
	 * @param  {Object} options
	 * @return {RegExp}
	 */
	function stringToRegexp (path, keys, options) {
	  var tokens = parse(path)
	  var re = tokensToRegExp(tokens, options)

	  // Attach keys back to the regexp.
	  for (var i = 0; i < tokens.length; i++) {
	    if (typeof tokens[i] !== 'string') {
	      keys.push(tokens[i])
	    }
	  }

	  return attachKeys(re, keys)
	}

	/**
	 * Expose a function for taking tokens and returning a RegExp.
	 *
	 * @param  {Array}  tokens
	 * @param  {Array}  keys
	 * @param  {Object} options
	 * @return {RegExp}
	 */
	function tokensToRegExp (tokens, options) {
	  options = options || {}

	  var strict = options.strict
	  var end = options.end !== false
	  var route = ''
	  var lastToken = tokens[tokens.length - 1]
	  var endsWithSlash = typeof lastToken === 'string' && /\/$/.test(lastToken)

	  // Iterate over the tokens and create our regexp string.
	  for (var i = 0; i < tokens.length; i++) {
	    var token = tokens[i]

	    if (typeof token === 'string') {
	      route += escapeString(token)
	    } else {
	      var prefix = escapeString(token.prefix)
	      var capture = token.pattern

	      if (token.repeat) {
	        capture += '(?:' + prefix + capture + ')*'
	      }

	      if (token.optional) {
	        if (prefix) {
	          capture = '(?:' + prefix + '(' + capture + '))?'
	        } else {
	          capture = '(' + capture + ')?'
	        }
	      } else {
	        capture = prefix + '(' + capture + ')'
	      }

	      route += capture
	    }
	  }

	  // In non-strict mode we allow a slash at the end of match. If the path to
	  // match already ends with a slash, we remove it for consistency. The slash
	  // is valid at the end of a path match, not in the middle. This is important
	  // in non-ending mode, where "/test/" shouldn't match "/test//route".
	  if (!strict) {
	    route = (endsWithSlash ? route.slice(0, -2) : route) + '(?:\\/(?=$))?'
	  }

	  if (end) {
	    route += '$'
	  } else {
	    // In non-ending mode, we need the capturing groups to match as much as
	    // possible by using a positive lookahead to the end or next path segment.
	    route += strict && endsWithSlash ? '' : '(?=\\/|$)'
	  }

	  return new RegExp('^' + route, flags(options))
	}

	/**
	 * Normalize the given path string, returning a regular expression.
	 *
	 * An empty array can be passed in for the keys, which will hold the
	 * placeholder key descriptions. For example, using `/user/:id`, `keys` will
	 * contain `[{ name: 'id', delimiter: '/', optional: false, repeat: false }]`.
	 *
	 * @param  {(String|RegExp|Array)} path
	 * @param  {Array}                 [keys]
	 * @param  {Object}                [options]
	 * @return {RegExp}
	 */
	function pathToRegexp (path, keys, options) {
	  keys = keys || []

	  if (!isarray(keys)) {
	    options = keys
	    keys = []
	  } else if (!options) {
	    options = {}
	  }

	  if (path instanceof RegExp) {
	    return regexpToRegexp(path, keys, options)
	  }

	  if (isarray(path)) {
	    return arrayToRegexp(path, keys, options)
	  }

	  return stringToRegexp(path, keys, options)
	}


/***/ }),
/* 29 */
/***/ (function(module, exports) {

	module.exports = Array.isArray || function (arr) {
	  return Object.prototype.toString.call(arr) == '[object Array]';
	};


/***/ }),
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
/* 49 */,
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
/* 93 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {var warehouseService = __webpack_require__(6),
	    page = __webpack_require__(26);

	ko.bindingHandlers.onEnter = {
	    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
	        var allBindings = allBindingsAccessor();
	        $(element).keypress(function (event) {
	            var keyCode = (event.which ?
	                    event.which :
	                    event.keyCode);
	            if (keyCode === 13) {
	                allBindings.onEnter.call(viewModel);
	                return false;
	            }
	            return true;
	        });
	    }
	};

	function InterWarehouseEditorViewModel(params) {
	    // Init
	    var self = this,
	        movement = ko.observable();

	    self.validation = ko.validatedObservable({});
	    self.disposables = [];

	    self.warehouses = ko.observableArray();
	    self.shipmentExports = ko.observable();
	    self.details = ko.observable();
	    self.inventoryPickOrderExports = ko.observable();
	    self.pickedInventoryItemsExports = ko.observable();

	    self.CustomerPO = ko.observable();
	    self.DateOrderReceived = ko.observableDate();
	    self.OrderPlacedBy = ko.observable();
	    self.OrderTakenBy = ko.observable();

	    self.isLocked = ko.observable();
	    self.isNewMovement = false;

	    self.warehouses = ko.isObservable(params.warehouseOptions) ? params.warehouseOptions : ko.observableArray(params.warehouseOptions || []);

	    // Subscriptions and Computed Values
	    self.currentMovement = ko.computed({
	        read: function() {
	            return movement();
	        },
	        write: function(values) {
	            if (values === null || values === undefined) {
	                setMovement(null);
	                return;
	            }

	            var warehouseList = self.warehouses(),
	                data = ko.toJS(values) || {},
	                originKey,
	                destinationKey;

	            if (!data.MovementKey) {
	                self.isNewMovement = true;
	            } else {
	                originKey = data.OriginFacilityKey
	                    ? data.OriginFacilityKey
	                    : data.OriginFacility.FacilityKey;
	                destinationKey = data.DestinationFacilityKey
	                    ? data.DestinationFacilityKey
	                    : data.DestinationFacility.FacilityKey;
	                self.isNewMovement = false;

	                ko.utils.arrayFirst(warehouseList, function(wh) {
	                    if (wh.FacilityKey === originKey) {
	                        data.OriginDetails = wh;
	                    }
	                    if (wh.FacilityKey === destinationKey) {
	                        data.DestinationDetails = wh;
	                    }
	                    return data.OriginDetails && data.DestinationDetails;
	                });

	                if (!data.OriginDetails || !data.DestinationDetails) {
	                    throw new Error("Details data was not set");
	                }
	            }

	            setMovement(data);
	        }
	    });

	    if (ko.isObservable(params.values)) {
	        self.disposables.push(params.values.subscribe(function(vals) {
	            self.currentMovement(vals);
	        }));
	    }

	    function setMovement(input) {
	        if (input === null || input === undefined) {
	            movement(null);
	            self.shipmentExports(null);
	            self.inventoryPickOrderExports(null);
	            self.pickedInventoryItemsExports(null);
	            self.validation(null);
	            return;
	        }

	        var data = input || {},
	            shipment = data.Shipment || {},
	            instructions = shipment.ShippingInstructions || {};

	        var m = {
	            CustomerOrder: ko.observable(data.CustomerOrder),
	            DateCreated: ko.observableDate(data.DateCreated),
	            DateOrderReceived: ko.observableDate(data.DateOrderReceived),
	            DestinationFacilityDetails: ko.observable(data.DestinationDetails),
	            MovementKey: ko.observable(data.MovementKey),
	            OrderRequestedBy: ko.observable(data.OrderRequestedBy),
	            OrderTakenBy: ko.observable(data.OrderTakenBy),
	            OrderStatus: ko.observable(data.OrderStatus),
	            OriginFacilityDetails: ko.observable(data.OriginDetails),
	            PickOrder: ko.observable(data.PickOrder),
	            PickedInventory: ko.observable(data.PickedInventory || {}),
	            PurchaseOrderNumber: ko.observable(data.PurchaseOrderNumber),
	            Shipment: ko.observable(shipment),
	            ShippingInstructions: ko.observable({
	                    SpecialInstructions: ko.observable(instructions.SpecialInstructions),
	                    InternalNotes: ko.observable(instructions.InternalNotes),
	                    ExternalNotes: ko.observable(instructions.ExternalNotes),
	                }
	            ),
	            MoveNum: data.MoveNum,
	            ShipmentDate: ko.observableDate(data.ShipmentDate).extend({ required: false }),
	            PickedInventoryInput: data.PickedInventory,
	            reports: [
	                {
	                    name: 'Order Acknowledgement',
	                    url: data.Links && data.Links['report-wh-acknowledgement'] && data.Links['report-wh-acknowledgement'].HRef
	                }, {
	                    name: 'Inventory Pick List',
	                    url: data.Links && data.Links['report-pick-list'] && data.Links['report-pick-list'].HRef
	                }, {
	                    name: 'Bill of Lading',
	                    url: data.Links && data.Links['report-bol'] && data.Links['report-bol'].HRef
	                }, {
	                    name: 'Packing List',
	                    url: data.Links && data.Links['report-packing-list'] && data.Links['report-packing-list'].HRef
	                }, {
	                    name: 'Certificate of Analysis',
	                    url: data.Links && data.Links['report-coa'] && data.Links['report-coa'].HRef
	                }
	            ]
	        };

	        m.OriginFacilityKey = ko.pureComputed(function() {
	            return m ? m.OriginFacilityDetails() && m.OriginFacilityDetails().FacilityKey : null;
	        });
	        m.DestinationFacilityKey = ko.pureComputed(function() {
	            return m ? m.DestinationFacilityDetails() && m.DestinationFacilityDetails().FacilityKey : null;
	        });
	        m.PickedInventory = ko.pureComputed(function() {
	            var pickedInventoryVm = self.pickedInventoryItemsExports();
	            return (pickedInventoryVm && pickedInventoryVm.PickedItems()) || [];
	        });
	        m.hasPickedInventory = ko.pureComputed(function() {
	            return m.PickedInventory().length;
	        });

	        movement(m);

	        self.isLocked(data.IsLocked);
	    }

	    self.disposables.push(ko.computed(function() {
	        var movement = self.currentMovement(),
	            shipment = self.shipmentExports();
	        if (movement && shipment) {
	            shipment.setShipFromAddress(
	                (movement.OriginFacilityDetails() || {}).ShippingLabel || null
	            );
	            shipment.setFreightBillAddress(
	                (movement.OriginFacilityDetails() || {}).ShippingLabel || null
	            );
	            shipment.setShipToAddress(
	                (movement.DestinationFacilityDetails() || {}).ShippingLabel || null
	            );
	        }
	    }));

	    self.pickCommand = params.pickCommand;

	    self.isLoaded = ko.computed(function() {
	        if (self.currentMovement() === null) {
	            return true;
	        }

	        var requiredData = {
	            shipment: ko.toJS(self.shipmentExports()) || {},
	            pickedInventory: ko.toJS(self.pickedInventoryItemsExports()) || {},
	            pickOrder: ko.toJS(self.inventoryPickOrderExports()) || {}
	        };

	        if (self.currentMovement() &&
	            requiredData.shipment.isInit &&
	            requiredData.pickedInventory.isInit &&
	            requiredData.pickOrder.isInit) {
	            return true;
	        }
	        return false;
	    });


	    self.currentMovement.isValid = function() {
	        var movement = self.currentMovement();
	        if (!movement) return false;
	        var validation = ko.validatedObservable({
	            ShipmentDate: movement.ShipmentDate
	        });

	        if (!validation.isValid()) {
	            validation.errors.showAllMessages();
	            return false;
	        }
	        return true;
	    };

	    self.currentMovement.asDto = function() {
	      var movementData = self.currentMovement();

	      if ( !movementData ) {
	        return null;
	      }

	      var dto = {
	        OriginFacilityKey: movementData.OriginFacilityKey,
	        DestinationFacilityKey: movementData.DestinationFacilityKey,
	        ShipmentDate: movementData.ShipmentDate,
	        PurchaseOrderNumber: movementData.PurchaseOrderNumber,
	        DateOrderReceived: movementData.DateOrderReceived,
	        OrderRequestedBy: movementData.OrderRequestedBy,
	        OrderTakenBy: movementData.OrderTakenBy,
	        Shipment: null,
	        InventoryPickOrderItems: self.inventoryPickOrderExports().toDto(),
	        PickedInventoryItemCodes: self.pickedInventoryItemsExports().toDto(),
	      };

	      var _shippingInstructions = movementData.ShippingInstructions();
	      dto.Shipment = self.shipmentExports().toDto();
	      dto.Shipment.ShippingInstructions.SpecialInstructions = _shippingInstructions.SpecialInstructions;
	      dto.Shipment.ShippingInstructions.InternalNotes = _shippingInstructions.InternalNotes;
	      dto.Shipment.ShippingInstructions.ExternalNotes = _shippingInstructions.ExternalNotes;

	      return ko.toJS( dto );
	    };

	    // Exports
	    params.exports({
	        currentMovement: self.currentMovement,
	        isLocked: self.isLocked,
	        dispose: self.dispose
	    });

	    self.exported = params.exports;
	}

	// Custom disposal logic
	InterWarehouseEditorViewModel.prototype.dispose = function () {
	    ko.utils.arrayForEach(this.disposables, this.disposeOne);
	    ko.utils.objectForEach(this, this.disposeOne);
	    this.exported(null);
	};

	InterWarehouseEditorViewModel.prototype.disposeOne = function (propOrValue, value) {
	    var disposable = value || propOrValue;

	    if (disposable && typeof disposable.dispose === "function") {
	        disposable.dispose();
	    }
	};

	ko.components.register('shipment-editor', __webpack_require__(94));
	ko.components.register('inventory-pick-order', __webpack_require__(96));
	ko.components.register('picked-inventory-items', __webpack_require__(99));

	// Webpack
	module.exports = {
	    viewModel: InterWarehouseEditorViewModel,
	    template: __webpack_require__(102)
	};

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 94 */
/***/ (function(module, exports, __webpack_require__) {

	/**
	  * @param {Object} input - Observable, input data to initialize editor
	  * @param {Object} exports - Observable, container for editor return
	  * @param {boolean} [soldTo=false] - Replace "Ship From" with "Sold To"
	  */

	var warehouseService = __webpack_require__(6);

	function ShipmentEditorViewModel(params) {
	    var self = this,
	        input = ko.toJS(params.input) || {},
	        transit = input.Transit || {},
	        shippingInstructions = input.ShippingInstructions || {};

	    self.isLocked = params.locked || null;
	    self.isInit = ko.observable(false);
	    self.isSoldTo = ko.unwrap( params.soldTo );

	    // Data
	    self.PalletWeight = ko.observable(input.PalletWeight);
	    self.PalletQuantity = ko.numericObservable(input.PalletQuantity);
	    self.Status = ko.observable(input.Status || 0).extend({ shipmentStatusType: true });
	    self.FreightType = ko.observable(transit.FreightType);
	    self.DriverName = ko.observable(transit.DriverName);
	    self.CarrierName = ko.observable(transit.CarrierName);
	    self.TrailerLicenseNumber = ko.observable(transit.TrailerLicenseNumber);
	    self.ContainerSeal = ko.observable(transit.ContainerSeal);
	    self.ShippingInstructions = {
	        ShipTo: ko.observable(shippingInstructions.ShipTo),
	        ShipFrom: ko.observable(shippingInstructions.ShipFromOrSoldTo || shippingInstructions.ShipFrom),
	        FreightBill: ko.observable(shippingInstructions.FreightBill),
	    };

	    self.FreightBillTypes = [
	        'Prepaid',
	        'FOB',
	        'Collect',
	        'COD',
	        '3rd Party',
	        'Delivered',
	        'Other'
	    ];

	    // Exports from address editor component
	    self.ShipFromExport = ko.observable();
	    self.ShipToExport = ko.observable();
	    self.FreightBillExport = ko.observable();

	    self.isInit(true);

	    // Behaviors
	    function setShipToAddress(values) {
	        self.ShippingInstructions.ShipTo(values);
	    }
	    function setShipFromAddress(values) {
	        self.ShippingInstructions.ShipFrom(values);
	    }
	    function setFreightBillAddress(values) {
	        self.ShippingInstructions.FreightBill(values);
	    }
	    function toDto() {
	        return ko.toJS({
	            PalletWeight: self.PalletWeight,
	            PalletQuantity: self.PalletQuantity,
	            Status: self.Status,
	            Transit: {
	                FreightBillType: self.FreightType,
	                DriverName: self.DriverName,
	                CarrierName: self.CarrierName,
	                TrailerLicenseNumber: self.TrailerLicenseNumber,
	                ContainerSeal: self.ContainerSeal,
	            },
	            ShippingInstructions: {
	                ShipFromOrSoldTo: self.ShipFromExport,
	                ShipTo: self.ShipToExport,
	                FreightBill: self.FreightBillExport
	            }
	        });
	    }

	    // Output
	    if (params && params.exports) {
	      params.exports({
	          isInit: self.isInit,
	          toDto: toDto,
	          setShipToAddress: setShipToAddress,
	          setShipFromAddress: setShipFromAddress,
	          setFreightBillAddress: setFreightBillAddress,
	      });
	    }

	    return this;
	}

	ko.components.register('contact-label-editor', __webpack_require__(5));

	module.exports = {
	    viewModel: ShipmentEditorViewModel,
	    template: __webpack_require__(95)
	};


/***/ }),
/* 95 */
/***/ (function(module, exports) {

	module.exports = "<form>\r\n    <div class=\"row\">\r\n        <div class=\"form-group col-md-4\">\r\n            <label class=\"control-label\">Pallet Weight Override</label>\r\n            <input class=\"form-control\" type=\"text\" data-bind=\"value: PalletWeight, disable: isLocked\" />\r\n        </div>\r\n        <div class=\"form-group col-md-4\">\r\n            <label class=\"control-label\">Pallet Quantity</label>\r\n            <input class=\"form-control\" type=\"text\" data-bind=\"value: PalletQuantity, disable: isLocked\" />\r\n        </div>\r\n\r\n        <div class=\"form-group col-md-6 col-lg-4\">\r\n            <label class=\"control-label\">Carrier Name</label>\r\n            <input class=\"form-control\" type=\"text\" data-bind=\"value: CarrierName, disable: isLocked\" />\r\n        </div>\r\n        <div class=\"form-group col-md-6 col-lg-4\">\r\n            <label class=\"control-label\">Driver Name</label>\r\n            <input class=\"form-control\" type=\"text\" data-bind=\"value: DriverName, disable: isLocked\" />\r\n        </div>\r\n        <div class=\"form-group col-md-6 col-lg-4\">\r\n            <label class=\"control-label\">Trailer License Number</label>\r\n            <input class=\"form-control\" type=\"text\" data-bind=\"value: TrailerLicenseNumber, disable: isLocked\" />\r\n        </div>\r\n        <div class=\"form-group col-md-6 col-lg-4\">\r\n            <label class=\"control-label\">Freight Type</label>\r\n            <select class=\"form-control\" data-bind=\"value: FreightType, options: FreightBillTypes, disable: isLocked\"></select>\r\n        </div>\r\n        <div class=\"form-group col-md-6 col-lg-4\">\r\n            <label class=\"control-label\">Container Seal</label>\r\n            <input class=\"form-control\" type=\"text\" data-bind=\"value: ContainerSeal, disable: isLocked\" />\r\n        </div>\r\n        <div class=\"form-group col-md-6 col-lg-4\">\r\n            <label class=\"control-label\">Shipment Status</label>\r\n            <p class=\"form-control-static\" data-bind=\"text: Status.displayValue\"> </p>\r\n        </div>\r\n    </div>\r\n    <div class=\"row\">\r\n        <div class=\"col-md-6 col-lg-4\">\r\n            <div class=\"panel panel-default\">\r\n                <div class=\"panel-heading\">\r\n                    <div class=\"panel-title\">\r\n                        <label data-bind=\"text: isSoldTo ? 'Sold To' : 'Ship From'\"></label>\r\n                    </div>\r\n                </div>\r\n                <div class=\"panel-body\">\r\n                    <contact-label-editor params=\"input: ShippingInstructions.ShipFrom(), exports: ShipFromExport, locked: isLocked\"></contact-label-editor>\r\n                </div>\r\n            </div>\r\n        </div>\r\n        <div class=\"col-md-6 col-lg-4\">\r\n            <div class=\"panel panel-default\">\r\n                <div class=\"panel-heading\">\r\n                    <div class=\"panel-title\">\r\n                        <label>Ship To</label>\r\n                    </div>\r\n                </div>\r\n                <div class=\"panel-body\">\r\n                    <contact-label-editor params=\"input: ShippingInstructions.ShipTo(), exports: ShipToExport, locked: isLocked\"></contact-label-editor>\r\n                </div>\r\n            </div>\r\n        </div>\r\n        <div class=\"col-md-6 col-lg-4\">\r\n            <div class=\"panel panel-default\">\r\n                <div class=\"panel-heading\">\r\n                    <div class=\"panel-title\">\r\n                        <label>Bill To</label>\r\n                    </div>\r\n                </div>\r\n                <div class=\"panel-body\">\r\n                    <contact-label-editor params=\"input: ShippingInstructions.FreightBill(), exports: FreightBillExport, locked: isLocked\"></contact-label-editor>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</form>\r\n"

/***/ }),
/* 96 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {var warehouseService = __webpack_require__(6),
	    productsService = __webpack_require__(24),
	    directoryService = __webpack_require__(16),
	    PickOrderItemFactory = __webpack_require__(97);

	function PickOrderItem( item, options ) {
	  item.customerOptions = options.customerOptions;
	  item.packageOptions = options.packageOptions;
	  item.productOptions = options.productOptions;

	  var mappedItem = new PickOrderItemFactory( item );

	  var _dirtyCheckProps = [ mappedItem.ProductKey, mappedItem.CustomerKey, mappedItem.CustomerProductCode, mappedItem.CustomerLotCode, mappedItem.PackagingKey, mappedItem.TreatmentKey, mappedItem.Quantity ];


	  var _quantity = mappedItem.Quantity();
	  mappedItem.Quantity( _quantity != null ? "" + _quantity : "" );

	  var _lotCode = mappedItem.CustomerLotCode();
	  mappedItem.CustomerLotCode( _lotCode != null ? "" + _lotCode : "" );

	  var _productCode = mappedItem.CustomerProductCode();
	  mappedItem.CustomerProductCode( _productCode != null ? "" + _productCode : "" );

	  mappedItem.dirtyFlag = (function( root, isInitiallyDirty ) {
	    var result = function() {},
	    _initialState = ko.observable( ko.toJSON( root ) ),
	    _isInitiallyDirty = ko.observable( isInitiallyDirty );

	    result.isDirty = ko.computed(function() {
	      return _isInitiallyDirty() || _initialState() !== ko.toJSON( root );
	    });

	    result.reset = function() {
	      _initialState(ko.toJSON( root ));
	      _isInitiallyDirty( false );
	    };

	    return result;
	  })( _dirtyCheckProps, false );

	  return mappedItem;
	}

	InventoryPickOrderViewModel.prototype.setInitialPickOrderItems = function (values) {
	    var self = this;
	    self.init.then(function() {
	        self.PickOrderItems(self.mapPickOrderItems((values && values.PickOrderItems) || []));
	    });
	};

	InventoryPickOrderViewModel.prototype.mapPickOrderItems = function (input) {
	    var self = this;

	    var options = {
	      customerOptions: self.customerOptions,
	      packageOptions: self.packageOptions,
	      productOptions: self.productOptions
	    };

	    var items = ko.utils.arrayMap( input, function( item ) {
	      return new PickOrderItem( item, options );
	    });

	    return items;
	};

	function InventoryPickOrderViewModel(params) {
	    if(!(this instanceof InventoryPickOrderViewModel)) { return new InventoryPickOrderViewModel(params); }

	    var self = this;

	    self.disposables = [];

	    // Initial bindings
	    self.isInit = ko.observable(false);

	    self.customerOptions = [];
	    self.packageOptions = [];
	    self.productOptions = [];

	    self.isLocked = params.locked || null;

	    self.PickOrderItems = ko.observableArray();

	    self.TotalQuantityOnOrder = ko.pureComputed(function () {
	        var total = 0;
	        ko.utils.arrayForEach(self.PickOrderItems(), function (item) {
	            total += Number(item.Quantity());
	        });
	        return total;
	    });
	    self.TotalWeightOnOrder = ko.pureComputed(function () {
	        var total = 0;
	        if (self.PickOrderItems().length > 0) {
	            ko.utils.arrayForEach(self.PickOrderItems(), function (item) {
	                total += Number(item.TotalWeight());
	            });
	        }
	        return total;
	    });

	   self.init = $.when(directoryService.getCustomers(), productsService.getPackagingProducts(), productsService.getChileProducts())
	        .done(function(customers, packaging, products) {
	            if (customers[1] !== "success" || packaging[1] !== "success" || products[1] !== "success") {
	                throw new Error('Failed to load Inventory Pick Order\'s option arrays.');
	            }

	            self.customerOptions = customers[0];
	            self.packageOptions = packaging[0];
	            self.productOptions = products[0];

	            self.isInit(true);
	        });

	    // Behaviors
	    self.addNewItem = function () {
	      var options = {
	          customerOptions: self.customerOptions,
	          packageOptions: self.packageOptions,
	          productOptions: self.productOptions,
	      };

	      var newItem = new PickOrderItem( {}, options );

	      self.PickOrderItems.push( newItem );

	      var items = $(".pick-order-item");
	      var itemsCount = items.length;
	      $(items[itemsCount - 1]).find(".product-select")[0].focus();
	    };

	    self.removeItem = ko.command({
	        execute: function () {
	            self.PickOrderItems.splice(self.PickOrderItems.indexOf(this), 1);
	        },
	        canExecute: function () {
	            return !self.isLocked();
	        }
	    });

	    self.isValid = function () {
	        var isValid = true;
	        for (var i = self.PickOrderItems().length, list = self.PickOrderItems() ; i--;) {
	            if (list[i].validation.isValid()) {
	                isValid = false;
	                break;
	            }
	        }
	        return isValid;
	    };

	    self.toDto = function () {
	      var items = self.PickOrderItems();
	      var dto = ko.utils.arrayMap( items, mapItem );

	      function mapItem( item ) {
	        var itemData = {
	          ProductKey: item.ProductKey,
	          PackagingKey: item.PackagingKey,
	          Quantity: item.Quantity,
	          TreatmentKey: item.TreatmentKey,
	          CustomerLotCode: item.CustomerLotCode,
	          CustomerProductCode: item.CustomerProductCode,
	          CustomerKey: item.CustomerKey
	        };

	        return itemData;
	      }

	      return ko.toJS( dto );
	    };

	    self.disposables.push(self.TotalQuantityOnOrder, self.TotalWeightOnOrder);
	    self.pickForItem = ko.asyncCommand({
	        execute: function ( data, complete ) {
	            var product = this.Product() || {},
	            packaging = this.Packaging() || {},
	            targetProduct = ko.toJS({
	                targetProduct: product,
	                targetWeight: data.TotalWeight,
	                filters: {
	                    inventoryType: product.ProductType,
	                    lotType: product.ChileState,
	                    productKey: this.ProductKey,
	                    packagingProductKey: packaging.ProductKey,
	                    treatmentKey: this.TreatmentKey
	                },
	                customerLotCode: this.CustomerLotCode,
	                customerProductCode: this.CustomerProductCode,
	                orderItemKey: this.OrderItemKey
	            });

	            var getProductSpec = productsService.getProductDetails( product.ProductType, product.ProductKey ).then(
	            function( data, textStatus, jqXHR ) {
	              targetProduct.targetProduct.AttributeRanges = data.AttributeRanges;
	              params.pickForItem(targetProduct);
	            },
	            function() {
	              targetProduct.targetProduct.AttributeRanges = null;
	              params.pickForItem(targetProduct);
	            }).always( complete );
	        },
	        canExecute: function ( isExecuting ) {
	            var enablePicking = ko.unwrap(params.enablePicking);
	            if (enablePicking == undefined) { enablePicking = true; }

	            return !isExecuting &&
	              enablePicking &&
	              typeof params.pickForItem === "function" &&
	              !self.isLocked();
	        }
	    });

	    if (ko.isObservable(params.data)) {
	        self.disposables.push(params.data.subscribe(function(values) {
	            self.setInitialPickOrderItems.call(self, values);
	        }));
	    }
	    self.setInitialPickOrderItems(ko.unwrap(params.data));

	    params.exports(self);
	}

	// Custom disposal logic
	InventoryPickOrderViewModel.prototype.dispose = function () {
	    ko.utils.arrayForEach(this.disposables, this.disposeOne);
	    ko.utils.objectForEach(this, this.disposeOne);
	};

	InventoryPickOrderViewModel.prototype.disposeOne = function (propOrValue, value) {
	    var disposable = value || propOrValue;

	    if (disposable && typeof disposable.dispose === "function") {
	        disposable.dispose();
	    }
	};

	// Webpack
	module.exports = {
	    viewModel: InventoryPickOrderViewModel,
	    template: __webpack_require__(98)
	};

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 97 */
/***/ (function(module, exports, __webpack_require__) {

	var productsService = __webpack_require__(24);
	__webpack_require__(55);

	ko.validation.init({
	    decorateInputElement: true,
	    errorElementClass: 'has-error',
	    insertMessages: false
	});

	function PickOrderItemModel(input) {
	    if (!(this instanceof PickOrderItemModel)) return new PickOrderItemModel(input);

	    var self = this,
	        data = ko.toJS(input) || {};

	    self.disposables = [];

	    // Data
	    self.OrderItemKey = input.OrderItemKey;
	    self.customerOptions = input.customerOptions;
	    self.packageOptions = input.packageOptions;
	    self.productOptions = ko.observableArray(input.productOptions);

	    self.wipProductOptions = ko.observableArray();
	    self.fgProductOptions = ko.observableArray();

	    self.Product = ko.observable();
	    self.Customer = ko.observable();
	    self.CustomerProductCode = ko.observable(data.CustomerProductCode);
	    self.CustomerLotCode = ko.observable(data.CustomerLotCode);
	    self.Packaging = ko.observable();
	    self.TreatmentKey = ko.observable(data.TreatmentKey).extend({ treatmentType: true });
	    self.Quantity = ko.observable(data.Quantity);
	    self.TotalWeight = ko.pureComputed(function () {
	        return (self.Quantity() && self.Packaging()) ?
	            self.Quantity() * self.Packaging().Weight :
	            null;
	    });
	    self.currentTypeFilter = ko.observable('All <span class="caret"></span>');

	    self.CustomerKey = ko.pureComputed(function () {
	        var customer = self.Customer();

	        if (customer && customer.CompanyKey) {
	            return customer.CompanyKey;
	        } else {
	            return null;
	        }
	    });
	    self.ProductKey = ko.pureComputed(function () {
	        var product = self.Product();

	        if (product && product.ProductKey) {
	            return product.ProductKey;
	        } else {
	            return null;
	        }
	    });
	    self.PackagingKey = ko.pureComputed(function () {
	        var packaging = self.Packaging();

	        if (packaging && packaging.ProductKey) {
	            return packaging.ProductKey;
	        } else {
	            return null;
	        }
	    });

	    // Behaviors
	    (function filterProductsList() {
	        for (var i = 0, len = self.productOptions().length, list = self.productOptions(); i < len; i++) {
	            if (list[i].ChileState === 2) {
	                self.wipProductOptions.push(list[i]);
	            } else if (list[i].ChileState === 3) {
	                self.fgProductOptions.push(list[i]);
	            }
	        }
	    }());

	    (function selectObject() {
	        var i,
	            list,
	            target;

	        // Customer
	        if (data.Customer) {
	            for (i = self.customerOptions.length, list = self.customerOptions, target = data.Customer.CompanyKey; i--;) {
	                if (list[i].CompanyKey === target) {
	                    self.Customer(list[i]);
	                    break;
	                }
	            }
	        } else {
	            self.Customer(null);
	        }

	        // Packaging
	        if (data.PackagingProductKey !== undefined) {
	            for (i = self.packageOptions.length, list = self.packageOptions, target = data.PackagingProductKey; i--;) {
	                if (list[i].ProductKey === target) {
	                    self.Packaging(list[i]);
	                    break;
	                }
	            }
	        } else {
	            self.Packaging(null);
	        }

	        // Product
	        if (data.ProductKey !== undefined) {
	            for (i = self.productOptions().length, list = self.productOptions(), target = data.ProductKey; i--;) {
	                if (list[i].ProductKey === target) {
	                    self.Product(list[i]);
	                    break;
	                }
	            }
	        } else {
	            self.Product(null);
	        }
	    }());

	    self.setFG = function () {
	        self.productOptions(self.fgProductOptions());
	        self.currentTypeFilter('FG ' + '<span class="caret"></span>');
	    };
	    self.setWIP = function () {
	        self.productOptions(self.wipProductOptions());
	        self.currentTypeFilter('WIP ' + '<span class="caret"></span>');
	    };

	    // Validation
	    self.validation = ko.validatedObservable({
	        Product: self.Product.extend({ required: true }),
	        Packaging: self.Packaging.extend({ required: true }),
	        TreatmentKey: self.TreatmentKey.extend({ required: true }),
	        Quantity: self.Quantity.extend({ required: true, min: 1 })
	    });

	    // Disposable items
	    self.disposables.push(self.validation);
	}

	// Custom disposal logic
	PickOrderItemModel.prototype.dispose = function () {
	    ko.utils.arrayForEach(this.disposables, this.disposeOne);
	    ko.utils.objectForEach(this, this.disposeOne);
	    self.productOptions([]);

	    self.wipProductOptions([]);
	    self.fgProductOptions([]);
	};

	PickOrderItemModel.prototype.disposeOne = function (propOrValue, value) {
	    var disposable = value || propOrValue;

	    if (disposable && typeof disposable.dispose === "function") {
	        disposable.dispose();
	    }
	};

	// Webpack
	module.exports = PickOrderItemModel;


/***/ }),
/* 98 */
/***/ (function(module, exports) {

	module.exports = "<!-- ko if: !isInit() -->\r\n<div class=\"row\">\r\n    <div class=\"col-sm-12 text-center\">\r\n        <i class=\"fa fa-spinner fa-2x fa-pulse\"></i>\r\n    </div>\r\n</div>\r\n<!-- /ko -->\r\n<!-- ko if: isInit -->\r\n<table class=\"reset table table-condensed table-striped\">\r\n    <thead>\r\n        <tr class=\"small\">\r\n            <th></th>\r\n            <th class=\"col-md-3\">Product</th>\r\n            <th class=\"col-md-2\">Customer</th>\r\n            <th>Customer Product Code</th>\r\n            <th>Customer Lot Code</th>\r\n            <th class=\"col-md-2\">Packaging</th>\r\n            <th class=\"col-md-1\">Treatment</th>\r\n            <th>Quantity</th>\r\n            <th>Total Weight</th>\r\n        </tr>\r\n    </thead>\r\n    <tbody data-bind=\"foreach: PickOrderItems\">\r\n        <tr class=\"pick-order-item\">\r\n            <td class=\"no-wrap\">\r\n              <button class=\"btn btn-link\" data-bind=\"command: $parent.removeItem\"><i class=\"fa fa-times\"></i></button>\r\n              <button class=\"btn btn-primary\" data-bind=\"command: $parent.pickForItem, disable: !OrderItemKey || dirtyFlag.isDirty()\">Pick</button>\r\n            </td>\r\n            <td>\r\n                <div class=\"input-group input-group-sm\" data-bind=\"validationElement: Product\">\r\n                    <div class=\"input-group-btn\">\r\n                        <button type=\"button\" class=\"btn btn-default dropdown-toggle\" data-toggle=\"dropdown\" aria-expanded=\"false\" data-bind=\"html: currentTypeFilter, disable: $parent.isLocked\">Type <span class=\"caret\"></span></button>\r\n                        <ul class=\"dropdown-menu\" role=\"menu\">\r\n                            <li><a href=\"#\" data-bind=\"click: setFG\">FG</a></li>\r\n                            <li><a href=\"#\" data-bind=\"click: setWIP\">WIP</a></li>\r\n                        </ul>\r\n                    </div><!-- /btn-group -->\r\n                    <select class=\"form-control product-select\" data-bind=\"value: Product, options: productOptions, optionsText: 'ProductCodeAndName', optionsCaption: ' ', disable: $parent.isLocked\"></select>\r\n                </div>\r\n            </td>\r\n            <td>\r\n                <div class=\"form-group-sm\">\r\n                    <select class=\"form-control\" data-bind=\"value: Customer, options: customerOptions, optionsText: 'Name', optionsCaption: ' ', disable: $parent.isLocked\"></select>\r\n                </div>\r\n            </td>\r\n            <td>\r\n                <div class=\"form-group-sm\">\r\n                    <input type=\"text\" class=\"form-control\" data-bind=\"value: CustomerProductCode, disable: $parent.isLocked\" />\r\n                </div>\r\n            </td>\r\n            <td>\r\n                <div class=\"form-group-sm\">\r\n                    <input type=\"text\" class=\"form-control\" data-bind=\"value: CustomerLotCode, disable: $parent.isLocked\" />\r\n                </div>\r\n            </td>\r\n            <td>\r\n                <div class=\"form-group-sm\" data-bind=\"validationElement: Packaging\">\r\n                    <select class=\"form-control\" data-bind=\"value: Packaging, options: packageOptions, optionsText: 'ProductName', optionsCaption: ' ', disable: $parent.isLocked\"></select>\r\n                </div>\r\n            </td>\r\n            <td>\r\n                <div class=\"form-group-sm\" data-bind=\"validationElement: TreatmentKey\">\r\n                    <select class=\"form-control\" data-bind=\"value: TreatmentKey, options: TreatmentKey.options, optionsText: 'value', optionsValue: 'key', disable: $parent.isLocked\"></select>\r\n                </div>\r\n            </td>\r\n            <td>\r\n                <div class=\"form-group-sm\" data-bind=\"validationElement: Quantity\">\r\n                    <input type=\"text\" class=\"form-control\" data-bind=\"value: Quantity, disable: $parent.isLocked\" />\r\n                </div>\r\n            </td>\r\n            <td class=\"small\">\r\n                <b data-bind=\"text: TotalWeight | number\"></b>\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n    <tbody data-bind=\"ifnot: $parent.isLocked\">\r\n        <tr>\r\n            <td colspan=\"9\">\r\n                <a class=\"text-button\" href=\"#\" data-bind=\"click: addNewItem\"><i class=\"icon-button fa fa-plus-square\"></i> Add New Item</a>\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>\r\n<!-- /ko -->\r\n"

/***/ }),
/* 99 */
/***/ (function(module, exports, __webpack_require__) {

	var PickedInventoryItemFactory = __webpack_require__(100);

	function PickedInventoryItemsViewModel (params) {
	    // Init
	    var self = this;
	    
	    self.disposables = [];

	    self.isLocked = params.locked || null;
	    self.isInit = ko.observable(false);

	    // Data
	    self.PickedInventoryItems = ko.pureComputed(function () {
	        var data = ko.unwrap(params.input) || {};
	        return ko.utils.arrayMap(ko.unwrap(data.PickedInventoryItems) || [], PickedInventoryItemFactory);
	    });
	    self.TotalQuantityPicked = ko.observable();
	    self.TotalNetWeightOfPickedItems = ko.observable();
	    self.TotalGrossWeightOfPickedItems = ko.observable();

	    self.isInit(true);

	    var calculationsWatcher = ko.computed(function () {
	        if (self.PickedInventoryItems()) {
	            var totalQuantity = 0,
	                totalNetWeight = 0,
	                totalGrossWeight = 0;

	            ko.utils.arrayForEach(self.PickedInventoryItems(), function (item) {
	                totalQuantity += Number(item.QuantityPicked);
	                totalNetWeight += Number(item.NetWeight);
	                totalGrossWeight += Number(item.GrossWeight);
	            });

	            self.TotalQuantityPicked(totalQuantity);
	            self.TotalNetWeightOfPickedItems(totalNetWeight);
	            self.TotalGrossWeightOfPickedItems(totalGrossWeight);
	        }
	    });
	    self.disposables.push(calculationsWatcher);
	    
	    function toDto() {
	        return ko.toJS(self.PickedInventoryItems);
	    }
	    
	    // Exports
	    params.exports({
	        isInit: self.isInit,
	        PickedItems: self.PickedInventoryItems,
	        TotalQuantityPicked: self.TotalQuantityPicked,
	        TotalNetWeightOfPickedItems: self.TotalNetWeightOfPickedItems,
	        TotalGrossWeightOfPickedItems: self.TotalGrossWeightOfPickedItems,
	        toDto: toDto
	    });
	}

	ko.utils.extend(PickedInventoryItemsViewModel, {
	    dispose: function () {
	        ko.utils.arrayForEach(this.disposables, this.disposeOne);
	        ko.utils.objectForEach(this, this.disposeOne);
	    },

	    disposeOne: function(propOrValue, value) {
	        var disposable = value || propOrValue;

	        if (disposable && typeof disposable.dispose === "function") {
	            disposable.dispose();
	        }
	    }
	});

	module.exports = {
	    viewModel: PickedInventoryItemsViewModel,
	    template: __webpack_require__(101)
	};


/***/ }),
/* 100 */
/***/ (function(module, exports) {

	function PickedInventoryItem(input) {
	    if (!(this instanceof PickedInventoryItem)) return new PickedInventoryItem(input);

	    // Init
	    var self = this,
	        input = ko.toJS(input || {});

	    // Data
	    self.ProductName = input.Product.ProductName;
	    self.ProductKey = input.Product.ProductKey;
	    self.ProductCodeAndName = input.Product.ProductCodeAndName;
	    self.PackagingProductName = input.PackagingProduct.ProductName;
	    self.PackagingProductKey = input.PackagingProduct.ProductKey;
	    self.PickedInventoryItemKey = input.PickedInventoryItemKey;
	    self.LocationKey = input.Location.LocationKey;
	    self.LocationName = input.Location.Description;
	    self.TreatmentKey = input.InventoryTreatment.TreatmentKey;
	    self.TreatmentName = input.InventoryTreatment.TreatmentNameShort;
	    self.LotNumber = input.LotKey;
	    self.CustomerProductCode = ko.observable(input.CustomerProductCode);
	    self.CustomerLotCode = ko.observable(input.CustomerLotCode);
	    self.QuantityPicked = input.QuantityPicked;
	    self.NetWeight = self.QuantityPicked * input.PackagingProduct.Weight;
	    self.GrossWeight = self.NetWeight + input.PackagingProduct.PackagingWeight + input.PackagingProduct.PalletWeight;
	}

	module.exports = PickedInventoryItem;


/***/ }),
/* 101 */
/***/ (function(module, exports) {

	module.exports = "<section class=\"table-responsive\">\r\n  <table class=\"reset table table-condensed table-striped table-hover\">\r\n      <thead class=\"small\">\r\n          <tr>\r\n              <th>Product</th>\r\n              <th>Lot Number</th>\r\n              <th>Packaging</th>\r\n              <th>Location</th>\r\n              <th>Treatment</th>\r\n              <th>Customer Product Code</th>\r\n              <th>Customer Lot Code</th>\r\n              <th>Quantity Picked</th>\r\n              <th>Net Weight</th>\r\n              <th>Gross Weight</th>\r\n          </tr>\r\n      </thead>\r\n      <tbody data-bind=\"foreach: PickedInventoryItems\">\r\n          <tr>\r\n              <td class=\"small no-wrap\" data-bind=\"text: ProductCodeAndName\"></td>\r\n              <td class=\"small no-wrap\" data-bind=\"text: LotNumber\"></td>\r\n              <td class=\"small\" data-bind=\"text: PackagingProductName\"></td>\r\n              <td class=\"small\" data-bind=\"text: LocationName\"></td>\r\n              <td class=\"small\" data-bind=\"text: TreatmentName\"></td>\r\n              <td>\r\n                  <div class=\"form-group-sm\">\r\n                      <input type=\"text\" class=\"form-control\" data-bind=\"value: CustomerProductCode, disable: $parent.isLocked\" />\r\n                  </div>\r\n              </td>\r\n              <td>\r\n                  <div class=\"form-group-sm\">\r\n                      <input type=\"text\" class=\"form-control\" data-bind=\"value: CustomerLotCode, disable: $parent.isLocked\" />\r\n                  </div>\r\n              </td>\r\n              <td class=\"small\" data-bind=\"text: QuantityPicked\"></td>\r\n              <td class=\"small\"><b data-bind=\"text: NetWeight | number\"></b></td>\r\n              <td class=\"small\"><b data-bind=\"text: GrossWeight | number\"></b></td>\r\n          </tr>\r\n      </tbody>\r\n      <tfoot>\r\n          <tr>\r\n              <td colspan=\"7\"></td>\r\n              <td data-bind=\"text: TotalQuantityPicked\"></td>\r\n              <td><b data-bind=\"text: TotalNetWeightOfPickedItems | number\"></b></td>\r\n              <td><b data-bind=\"text: TotalGrossWeightOfPickedItems | number\"></b></td>\r\n          </tr>\r\n      </tfoot>\r\n  </table>\r\n</section>\r\n"

/***/ }),
/* 102 */
/***/ (function(module, exports) {

	module.exports = "<div class=\"reset container-fluid\">\r\n  <div class=\"panel panel-default\" data-bind=\"if: currentMovement()\">\r\n    <div class=\"panel-heading\">\r\n      <hgroup>\r\n      <h2>Movement to Another Warehouse</h2>\r\n      <!-- ko if: isNewMovement -->\r\n      <h3>New Movement</h3>\r\n      <!-- /ko -->\r\n      <!-- ko ifnot: isNewMovement -->\r\n        <h3>\r\n          <!-- ko text: currentMovement().MovementKey --><!-- /ko -->\r\n          <span class=\"small\">\r\n            (Move Num: <!-- ko text: currentMovement().MoveNum --><!-- /ko --> )\r\n          </span>\r\n        </h3>\r\n      <!-- /ko -->\r\n      </hgroup>\r\n    </div>\r\n    <div class=\"panel-body\">\r\n      <div class=\"row\">\r\n        <div class=\"form-group col-md-6 col-lg-3\">\r\n          <label class=\"control-label\">Ship From</label>\r\n          <select class=\"form-control\" data-bind=\"value: currentMovement().OriginFacilityDetails, options: warehouses, optionsText: 'FacilityName', optionsCaption: ' ', disable: isLocked\"></select>\r\n        </div>\r\n        <div class=\"form-group col-md-6 col-lg-3\">\r\n          <label class=\"control-label\">Ship To</label>\r\n          <select class=\"form-control\" data-bind=\"value: currentMovement().DestinationFacilityDetails, options: warehouses, optionsText: 'FacilityName', optionsCaption: ' ', disable: isLocked\"></select>\r\n        </div>\r\n        <div class=\"form-group col-md-6 col-lg-3\" data-bind=\"validationElement: currentMovement().ShipmentDate\">\r\n          <label class=\"control-label\">Shipment Date </label>\r\n          <!-- ko if: isLocked -->\r\n          <input type=\"text\" class=\"form-control\" placeholder=\"mm/dd/yyyy\" data-bind=\"value: currentMovement().ShipmentDate, disable: true\">\r\n          <!-- /ko -->\r\n          <!-- ko ifnot: isLocked -->\r\n          <input type=\"text\" class=\"form-control\" placeholder=\"mm/dd/yyyy\" data-bind=\"value: currentMovement().ShipmentDate, datePicker: true\">\r\n          <!-- /ko -->\r\n        </div>\r\n        <div class=\"form-group col-md-6 col-lg-3\">\r\n          <label class=\"control-label\">Date Created </label>\r\n          <input type=\"text\" class=\"form-control\" data-bind=\"value: currentMovement().DateCreated, disable: true\">\r\n        </div>\r\n      </div>\r\n      <div class=\"row\">\r\n        <div class=\"col-md-12\">\r\n          <div data-bind=\"tabs: true\">\r\n            <ul class=\"nav nav-tabs\">\r\n              <li role=\"presentation\"><a href=\"#shipping\">Shipping Information</a></li>\r\n              <li role=\"presentation\"><a href=\"#ordered\">Items Ordered</a></li>\r\n              <li role=\"presentation\"><a href=\"#notes\">Notes</a></li>\r\n              <li role=\"presentation\"><a href=\"#picked\">Items Picked</a></li>\r\n            </ul>\r\n            <div id=\"shipping\" class=\"reset\">\r\n              <div class=\"row\">\r\n                <div class=\"form-group col-md-6\">\r\n                  <label class=\"control-label\">Customer PO</label>\r\n                  <input type=\"text\" class=\"form-control\" data-bind=\"value: currentMovement().PurchaseOrderNumber, disable: isLocked\">\r\n                </div>\r\n                <div class=\"form-group col-md-6\">\r\n                  <label class=\"control-label\">Date Order Received</label>\r\n                  <!-- ko if: isLocked -->\r\n                  <input type=\"text\" class=\"form-control\" data-bind=\"value: currentMovement().DateOrderReceived, disable: true\">\r\n                  <!-- /ko -->\r\n                  <!-- ko ifnot: isLocked -->\r\n                  <input type=\"text\" class=\"form-control\" data-bind=\"value: currentMovement().DateOrderReceived, datePicker: true\">\r\n                  <!-- /ko -->\r\n                </div>\r\n                <div class=\"form-group col-md-6\">\r\n                  <label class=\"control-label\">Order Placed By</label>\r\n                  <input type=\"text\" class=\"form-control\" data-bind=\"value: currentMovement().OrderRequestedBy, disable: isLocked\">\r\n                </div>\r\n                <div class=\"form-group col-md-6\">\r\n                  <label class=\"control-label\">Order Taken By</label>\r\n                  <input type=\"text\" class=\"form-control\" data-bind=\"value: currentMovement().OrderTakenBy, disable: isLocked\">\r\n                </div>\r\n              </div>\r\n              <shipment-editor params=\"input: currentMovement().Shipment, \r\n                exports: shipmentExports, \r\n                locked: isLocked\">\r\n              </shipment-editor>\r\n            </div>\r\n            <div id=\"ordered\">\r\n              <div class=\"table-responsive\">\r\n                <inventory-pick-order \r\n                  params=\"data: currentMovement().PickOrder, \r\n                    pickForItem: pickCommand.execute,\r\n                    enablePicking: pickCommand.canExecute,\r\n                    exports: inventoryPickOrderExports, \r\n                    locked: isLocked\">\r\n                </inventory-pick-order>\r\n              </div>\r\n            </div>\r\n            <div id=\"notes\">\r\n              <div class=\"form-group\">\r\n                <label class=\"control-label\">Special instructions that appear on the pick sheet</label>\r\n                <textarea class=\"form-control\" rows=\"4\" data-bind=\"value: currentMovement().ShippingInstructions().SpecialInstructions, disable: isLocked\"></textarea>\r\n              </div>\r\n              <div class=\"form-group\">\r\n                <label class=\"control-label\">Internal instructions that appear on the pick sheet</label>\r\n                <textarea class=\"form-control\" rows=\"4\" data-bind=\"value: currentMovement().ShippingInstructions().InternalNotes, disable: isLocked\"></textarea>\r\n              </div>\r\n              <div class=\"form-group\">\r\n                <label class=\"control-label\">External instructions that appear on the bill of lading</label>\r\n                <textarea class=\"form-control\" rows=\"4\" data-bind=\"value: currentMovement().ShippingInstructions().ExternalNotes, disable: isLocked\"></textarea>\r\n              </div>\r\n            </div>\r\n            <div id=\"picked\">\r\n                <picked-inventory-items \r\n                    params=\"input: currentMovement().PickedInventoryInput,\r\n                            exports: pickedInventoryItemsExports, \r\n                            locked: isLocked\">\r\n              </picked-inventory-items>\r\n            </div>\r\n          </div>\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </div>\r\n</div>\r\n"

/***/ }),
/* 103 */
/***/ (function(module, exports, __webpack_require__) {

	var warehouseService = __webpack_require__(6),
	    InterWarehouseSummaryFactory = __webpack_require__(104),
	    productsService = __webpack_require__(24),
	    helpers = __webpack_require__(55);

	function InterWarehouseMovementSummaryViewModel(params) {
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
	        return item.Shipment.Status() === 10;
	    });
	    self.unshippedOrders = self.returnedMovementArray.filter(function (item) {
	        return item.Shipment.Status() !== 10;
	    });


	    // Behaviors
	    var pager = warehouseService.getInterWarehouseMovementsDataPager();
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
	        return pager.nextPage()
	            .then(function (values) {
	                var array = self.returnedMovementArray();
	                ko.utils.arrayPushAll(array, buildMovementSummary(values));
	                self.returnedMovementArray.valueHasMutated();
	            })
	            .always(function () {
	                self.isLoading(false);
	            });
	    }

	    self.setSelectedMovement = function () {
	        var context = ko.contextFor(arguments[1].originalEvent.target) || {};
	        context = context.$data || null;
	        if (!(context instanceof InterWarehouseSummaryFactory)) { return; }

	        selectedItem && selectedItem.isSelected(false);
	        context.isSelected(true);
	        selectedItem = context;
	        self.selectedMovement(context);
	    };

	    self.getNextResultsPageCommand = ko.asyncCommand({
	        execute: function(complete) { getNextPage().always(complete); },
	        canExecute: function(isExecuting) { return !isExecuting; }
	    });
	    
	    self.addSummaryItem = function (key) {
	        warehouseService.getInterWarehouseDetails(key)
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
	                    warehouseService.getInterWarehouseDetails(movement.MovementKey)
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

	    params.exports(self);

	    getNextPage();
	}

	InterWarehouseMovementSummaryViewModel.prototype.mapSummaryItem = function(values) {
	    var summary = InterWarehouseSummaryFactory(values);
	    summary.isSelected = ko.observable(false);
	    return summary;
	}
	InterWarehouseMovementSummaryViewModel.prototype.dispose = function () {
	    ko.utils.arrayForEach(this.disposables, this.disposeOne);
	    ko.utils.objectForEach(this, this.disposeOne);
	};

	InterWarehouseMovementSummaryViewModel.prototype.disposeOne = function (propOrValue, value) {
	    var disposable = value || propOrValue;

	    if (disposable && typeof disposable.dispose === "function") {
	        disposable.dispose();
	    }
	};

	module.exports = {
	    viewModel: InterWarehouseMovementSummaryViewModel,
	    template: __webpack_require__(105)
	};


/***/ }),
/* 104 */
/***/ (function(module, exports, __webpack_require__) {

	var warehouseService = __webpack_require__(6);
	__webpack_require__(30);

	function MovementModel(input) {
	    if (!(this instanceof MovementModel)) { return new MovementModel(input); }

	    var self = this;

	    self.disposables = [];
	    
	    self.MovementKey = input.MovementKey;
	    self.MoveNum = input.MoveNum;
	    self.DateCreated = new Date(input.DateCreated).format('m/d/yyyy', true);
	    self.InventoryTreatment = input.InventoryTreatment || {};
	    self.PickOrder = input.PickOrder;
	    self.PickedInventory = input.PickedInventory;
	    self.Shipment = input.Shipment;
	    self.ShipmentDate = new Date(input.ShipmentDate).format('m/d/yyyy', true);
	    self.DestinationFacility = input.DestinationFacility;
	    self.OriginFacility = input.OriginFacility;
	    self.OrderStatus = ko.observable(input.OrderStatus).extend({ orderStatusType: true });
	    self.StatusDisplayText = input.StatusDisplayText;
	    self.EnableReturnFromTreatment = input.EnableReturnFromTreatment;

	    self.Shipment.Status = ko.observable(input.Shipment.Status || 0).extend({ shipmentStatusType: true });
	        
	    if (self.PickOrder && 
	            !self.PickOrder.hasOwnProperty("PoundsOnOrder") &&
	            self.PickOrder.hasOwnProperty("PickOrderItems")) {
	        calculateTotals();
	    }

	    function calculateTotals () {
	        var pickOrderWeight = 0,
	        pickOrderQuantity = 0,
	        pickedWeight = 0,
	        pickedQuantity = 0;

	        (function calculatePickOrderTotalWeight() {
	            for (var i = 0, list = self.PickOrder.PickOrderItems, max = list.length;
	                i < max; i += 1) {
	                if (list[i].Quantity > 0) {
	                    pickOrderWeight += list[i].TotalWeight;
	                    pickOrderQuantity += list[i].Quantity;
	                }
	            }
	        })();

	        (function calculatePickedTotalWeight() {
	            for (var i = 0, list = self.PickedInventory.PickedInventoryItems, max = list.length;
	                    i < max; i += 1) {
	                if (list[i].QuantityPicked > 0) {
	                    pickedQuantity += list[i].QuantityPicked;
	                    pickedWeight += (list[i].QuantityPicked * list[i].PackagingProduct.Weight);
	                }
	            }
	        })();
	        self.PickOrder.TotalQuantity = pickOrderQuantity;
	        self.PickOrder.PoundsOnOrder = pickOrderWeight;
	        self.PickedInventory.TotalQuantityPicked = pickedQuantity;
	        self.PickedInventory.PoundsPicked = pickedWeight;
	    }
	}

	ko.utils.extend(MovementModel, {
	    dispose: function () {
	        ko.utils.arrayForEach(this.disposables, this.disposeOne);
	        ko.utils.objectForEach(this, this.disposeOne);
	    },

	    disposeOne: function(propOrValue, value) {
	        var disposable = value || propOrValue;

	        if (disposable && typeof disposable.dispose === "function") {
	            disposable.dispose();
	        }
	    }
	});

	module.exports = MovementModel;


/***/ }),
/* 105 */
/***/ (function(module, exports) {

	module.exports = "<loading-screen params=\"isVisible: isLoading, displayMessage: loadingMessage\"></loading-screen>\r\n\r\n<div>\r\n    <div class=\"panel panel-default\">\r\n        <div class=\"panel-heading\">\r\n            <h4 class=\"panel-title\"><a id=\"unshipped-orders\"></a>Scheduled Movements</h4>\r\n        </div>\r\n        <section data-bind=\"template: { name: 'movements-table', data: unshippedOrders }\"></section>\r\n    </div>\r\n    <div class=\"panel panel-default\">\r\n        <div class=\"panel-heading\">\r\n            <h4 class=\"panel-title\"><a id=\"shipped-orders\"></a>Posted Movements</h4>\r\n        </div>\r\n        <section data-bind=\"template: { name: 'movements-table', data: shippedOrders }\"></section>\r\n    </div>\r\n</div>\r\n\r\n<!-- Knockout templates -->\r\n<script id=\"movements-table\" type=\"text/html\">\r\n    <table class=\"reset table table-condensed table-hover table-striped\">\r\n        <tr>\r\n            <th>Ship From</th>\r\n            <th>Ship To</th>\r\n            <th>Move Num</th>\r\n            <th>Date Created</th>\r\n            <th>Shipment Date</th>\r\n            <th>Shipment Status</th>\r\n            <th class=\"col-sm-1\">Pounds on Order</th>\r\n            <th class=\"col-sm-1\">Pounds Picked</th>\r\n        </tr>\r\n        <tbody data-bind=\"template: { name: 'movements-tbody', foreach: $data }, click: $parent.setSelectedMovement\"></tbody>\r\n    </table>\r\n</script>\r\n\r\n<script id=\"movements-tbody\" type=\"text/html\">\r\n    <tr data-bind=\"css: { info: isSelected }\">\r\n        <td data-bind=\"text: OriginFacility.FacilityName\"></td>\r\n        <td data-bind=\"text: DestinationFacility.FacilityName\"></td>\r\n        <td data-bind=\"text: MoveNum\"></td>\r\n        <td data-bind=\"text: DateCreated\"></td>\r\n        <td data-bind=\"text: ShipmentDate\"></td>\r\n        <td data-bind=\"text: Shipment.Status.displayValue\"></td>\r\n        <td data-bind=\"text: PickOrder.PoundsOnOrder | number\"></td>\r\n        <td data-bind=\"text: PickedInventory.PoundsPicked | number\"></td>\r\n    </tr>\r\n</script>"

/***/ }),
/* 106 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {var warehouseService = __webpack_require__(6),
	    warehouseLocationsService = __webpack_require__(11);

	__webpack_require__(31);

	ko.validation.init({
	    insertMessages: false,
	    decorateInputElement: true,
	    decorateElementOnModified: false,
	    errorElementClass: 'has-error',
	    errorMessageClass: 'help-block'
	});

	function PostCloseInventoryViewModel(params) {
	    if (!(this instanceof PostCloseInventoryViewModel)) { return new PostCloseInventoryViewModel(params); }

	    var self = this;

	    // Imports
	    self.pickedInventoryItems = params.pickedInventoryItems;
	    self.orderKey = params.orderKey;
	    self.destinationLocationOptions = params.destinationLocationOptions;
	    self.requiresDestinationLocation = params.requiresDestinationLocation || false;
	    self.validation = ko.validatedObservable();

	    // Data
	    self.mappedInventoryItems = ko.computed(function () {
	        var i,
	            isFirst = true,
	            validation = ko.unwrap(self.requiresDestinationLocation) === true ? {} : null;

	        var mapped = ko.utils.arrayMap(self.pickedInventoryItems() || [], function(item) {
	            item.isFirstItem = isFirst;
	            item.DestinationLocation = ko.observable().extend({ required: validation != undefined });
	            if (validation) validation['item' + i] = item.DestinationLocation;
	            if (isFirst) isFirst = false;
	            return item;
	        });
	        self.validation(validation);
	        return mapped;
	    });

	    // Behaviors
	    self.isSingleItemList = ko.pureComputed(function () {
	        return self.destinationLocationOptions().length === 1;
	    });

	    self.setDestinationForAllItemsCommand = ko.command({
	        execute: function() {
	            var destination = this.DestinationLocation();

	            for (var i = self.pickedInventoryItems().length, list = self.pickedInventoryItems(); i--;) {
	                list[i].DestinationLocation(destination);
	            }
	        },
	        canExecute: function() {
	            return true;
	        }
	    });

	    self.setDestinationFromPreviousItemCommand = ko.command({
	        execute: function() {
	            var i = self.pickedInventoryItems().indexOf(this),
	            copyDestination = self.pickedInventoryItems()[i-1].DestinationLocation();

	            if (i > 0) {
	                self.pickedInventoryItems()[i].DestinationLocation(copyDestination);
	            }
	        },
	        canExecute: function() {
	            return true;
	        }
	    });

	    self.disposables = [ self.mappedInventoryItems ];

	    // Exports
	    params.exports({
	        postAndCloseAsync: self.postAndCloseAsync.bind(self)
	    });
	}

	PostCloseInventoryViewModel.prototype.postAndCloseAsync = function () {
	    var self = this,
	        values = { PickedItemDestinations: [] },
	        key = ko.unwrap(self.orderKey);

	    if (self.requiresDestinationLocation && !self.validation.isValid()) {
	        self.validation.errors.showAllMessages();
	        showUserMessage("Failed to Post", {
	            description: 'Please fill in all required fields'
	        });
	        return $.Deferred().reject('Validation errors encountered.');
	    }

	    for (var i = 0, list = self.pickedInventoryItems(), len = list.length; i < len; i++) {
	        values.PickedItemDestinations.push({
	            "PickedInventoryItemKey": list[i].PickedInventoryItemKey,
	            "DestinationLocationKey": list[i].DestinationLocation() ?
	                list[i].DestinationLocation() : null
	        });
	    }

	    return warehouseService.postAndCloseShipmentOrder(key, values);
	}

	PostCloseInventoryViewModel.prototype.dispose = function () {
	    ko.utils.arrayForEach(this.disposables, this.disposeOne);
	    ko.utils.objectForEach(this, this.disposeOne);
	};

	PostCloseInventoryViewModel.prototype.disposeOne = function(propOrValue, value) {
	    var disposable = value || propOrValue;

	    if (disposable && typeof disposable.dispose === "function") {
	        disposable.dispose();
	    }
	};

	module.exports = {
	    viewModel: PostCloseInventoryViewModel,
	    template: __webpack_require__(107)
	};

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 107 */
/***/ (function(module, exports) {

	module.exports = "<div class=\"panel panel-default\">\n<div class=\"panel-heading\">\n  <hgroup>\n  <h2>Post &amp; Close</h2>\n  <h3 data-bind=\"text: orderKey\"></h3>\n  </hgroup>\n</div>\n<table class=\"reset table table-condensed table-hover table-striped\">\n  <thead>\n    <tr>\n      <th>Product</th>\n      <th>Lot Number</th>\n      <th>Customer Name</th>\n      <th>Customer Product Code</th>\n      <th>Customer Lot Code</th>\n      <th>Packaging</th>\n      <th>Treatment</th>\n      <th>Quantity</th>\n      <th>Destination</th>\n      <th colspan=\"2\">Tools</th>\n    </tr>\n  </thead>\n  <tbody data-bind=\"foreach: mappedInventoryItems\">\n    <tr class=\"small\">\n      <td data-bind=\"text: Product.ProductCodeAndName\"></td>\n      <td data-bind=\"text: LotKey\"></td>\n      <td data-bind=\"text: CustomerName\"></td>\n      <td data-bind=\"text: CustomerProductCode\"></td>\n      <td data-bind=\"text: CustomerLotCode\"></td>\n      <td data-bind=\"text: PackagingProduct.ProductName\"></td>\n      <td data-bind=\"text: InventoryTreatment.TreatmentNameShort\"></td>\n      <td data-bind=\"text: QuantityPicked\"></td>\n      <td>\n        <div class=\"form-group-sm\" data-bind=\"validationElement: DestinationLocation\">\n          <!-- ko ifnot: $parent.isSingleItemList -->\n          <select class=\"form-control\" data-bind=\"value: DestinationLocation, options: $parent.destinationLocationOptions, optionsText: 'Description', optionsValue: 'LocationKey', optionsCaption: ' '\"></select>\n          <!-- /ko -->\n          <!-- ko if: $parent.isSingleItemList -->\n          <select class=\"form-control\" data-bind=\"value: DestinationLocation, options: $parent.destinationLocationOptions, optionsText: 'Description', optionsValue: 'LocationKey'\"></select>\n          <!-- /ko -->\n        </div>\n      </td>\n      <td>\n        <button class=\"btn btn-sm btn-default\" data-bind=\"command: $parent.setDestinationForAllItemsCommand\"><i class=\"fa fa-clipboard\"></i> Copy to All</button>\n      </td>\n      <td>\n        <button class=\"btn btn-sm btn-default\" data-bind=\"command: $parent.setDestinationFromPreviousItemCommand, disable: isFirstItem\"><i class=\"fa fa-files-o\"></i> Copy Previous</button>\n      </td>\n    </tr>\n  </tbody>\n</table>\n</div>\n"

/***/ }),
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

/***/ })
]);