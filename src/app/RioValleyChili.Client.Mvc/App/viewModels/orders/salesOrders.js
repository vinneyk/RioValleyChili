var rvc = require('rvc');
var salesService = require('App/services/salesService');
var productsService = require('App/services/productsService');
var page = require('page');
var PickableInventoryItem = require('App/models/PickableInventoryItem');

require('App/koExtensions');
require('App/helpers/koPunchesFilters');
require('App/helpers/koDeepValidationHelpers');

ko.components.register('loading-screen', require('App/components/common/loading-screen/loading-screen'));
ko.components.register('sales-orders-summary', require('App/components/orders/sales-orders-summary/sales-orders-summary'));
ko.components.register('sales-orders-details', require('App/components/orders/sales-orders-details/sales-orders-details'));
ko.components.register('lot-filters', require('App/components/common/lot-filters/lot-filters'));
ko.components.register('inventory-picker', require('App/components/inventory/inventory-picker/inventory-picker'));

function SalesOrdersViewModel() {
  var self = this;

  this.isInit = ko.observable(false);

  // Data
  var isRedirecting = false;

  this.orderDetails = ko.observable(null);

  this.isLocked = ko.observable(false);
  this.isLoading = ko.observable(false);
  this.isPickingInventory = ko.observable( false );
  this.isNew = ko.pureComputed(function() {
    return self.orderDetails() === 'new';
  });
  this.movementKey = ko.observable(null);
  this.isWorking = ko.pureComputed(function() {
    return !this.isInit() || this.isLoading();
  }, this);

  this.searchString = ko.observable(null);
  this.loadingMessage = ko.observable('');
  this.shipmentMethods = ko.observableArray( [] );

  this.summaryExports = ko.observable();
  this.editorExports = ko.observable();
  this.filtersExports = ko.observable();
  this.pickerExports = ko.observable();

  this.filters = {
    orderStatus: ko.observable(),
    customerKey: ko.observable(),
    brokerKey: ko.observable(),
    orderReceivedRangeStart: ko.observable(),
    orderReceivedRangeEnd: ko.observable()
  };
  this.options = {
    brokers: ko.observableArray(),
    customers: ko.observableArray(),
    facilities: ko.observableArray(),
    products: ko.observableArray(),
    nonInventoryProducts: ko.observableArray(),
    packaging: ko.observableArray(),
    treatments: ko.observableArray(),
    shipmentMethods: this.shipmentMethods
  };

  this.pickerData = null;
  this.checkOutOfRange = function( key, value ) {
    var product = ko.unwrap( self.pickerData.targetProduct );
    var targetRanges = product.AttributeRanges || [];

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

  this.postInvoiceCommand = ko.asyncCommand({
    execute: function( complete ) {
      var orderDetails = self.editorExports().toDto();
      var orderKey = orderDetails.SalesOrderKey;

      var postInvoice = salesService.postSalesInvoice( orderKey ).then(
        function( data, textStatus, jqXHR ) {
        // Update details view
        self.editorExports().setStatusAsInvoiced();

        return data;
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not create invoice', { description: errorThrown } );
      });

      var getUpdatedData = postInvoice.then(function( data ) {
        var getSalesOrder = salesService.getSalesOrder( orderKey ).then(
          function( data, textStatus, jqXHR ) {
          return data;
        });

        return getSalesOrder;
      });

      var updateSummary = getUpdatedData.then(function( data ) {
        // Replace orderStatus with customerOrderStatus for summary table
        data.OrderStatus = data.SalesOrderStatus;

        // Update summary item
        self.summaryExports().updateOrder( data );

        showUserMessage( 'Post successful', { description: 'An invoice has been successfully posted.' } );
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      var editor = self.editorExports();

      return !isExecuting &&
        editor && editor.canPostInvoice();
    }
  });

  this.postOrderCommand = ko.asyncCommand({
    execute: function( complete ) {
      var orderDetails = self.editorExports().toDto();
      var pickedItems = self.editorExports().orderToDto();
      var orderKey = orderDetails.SalesOrderKey;

      var postOrder = salesService.postSalesOrder( orderKey, pickedItems ).then(
        function( data, textStatus, jqXHR ) {
        return data;
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not post order', { description: errorThrown } );
      });

      var getUpdatedData = postOrder.then(function( data ) {
        var getSalesOrder = salesService.getSalesOrder( orderKey ).then(
          function( data, textStatus, jqXHR ) {
          return data;
        });

        return getSalesOrder;
      });

      var updateSummary = getUpdatedData.then(function( data ) {
        // Replace orderStatus with customerOrderStatus for summary table
        data.OrderStatus = data.SalesOrderStatus;

        // Update details view
        self.editorExports().displayShipmentData( data );

        // Update summary item
        self.summaryExports().updateOrder( data );

        showUserMessage( 'Post successful', { description: 'The sales order <b>' + orderDetails.SalesOrderKey +  '</b> has been successfully posted.' } );
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      var editor = self.editorExports();

      return !isExecuting &&
        editor && editor.canPostOrder();
    }
  });

  this.startPicker = ko.command({
    execute: function( data ) {
      // Compile data for inventory picker
      self.pickerData = data;

      // Fetch target attributes for target product
      var product = ko.toJS( self.pickerData.targetProduct );
      var getProductDetails = productsService.getProductDetails(product.ProductType, product.ProductKey).then(
        function( data, textStatus, jqXHR ) {
        self.pickerData.targetProduct = data;


        // Enable picker UI
        self.isPickingInventory( true );

        // Allow knockout component to init before attempting to fetch items
        setTimeout(function() {
          var picker = ko.unwrap( self.pickerExports() );

          // Init item load
          var loadItems = picker && picker.loadInventoryItemsCommand.execute();
        });
      });
    },
    canExecute: function() {
      return true;
    }
  });
  this.loadMorePickerCommand = ko.asyncCommand({
    execute: function( complete ) {
      var picker = ko.unwrap( self.pickerExports() );

      var getItems = picker && picker.loadInventoryItemsCommand.execute().always( complete );
    },
    canExecute: function( isExecuting ) {
      var picker = ko.unwrap(self.pickerExports());
      return !isExecuting && picker && picker.loadInventoryItemsCommand.canExecute();
    }
  });
  this.savePick = ko.asyncCommand({
    execute: function( complete ) {
      var picker = ko.unwrap( self.pickerExports() );

      // Save picked items to API
      var savePicks = picker && picker.saveCommand.execute()
        .done(function(data, textStatus, jqXHR) {
          return data;
        })
        .fail(function() {
          complete();
          return;
        });

      // Get sales order data and update picked items
      var updateItems = savePicks.then(
      function( data, textStatus, jqXHR ) {
        var key = self.movementKey();
        var getOrder = salesService.getSalesOrder( key ).done(
        function( data, textStatus, jqXHR ) {
          var mappedItems = ko.utils.arrayMap( data.PickedInventory.PickedInventoryItems, function( item ) {
            return new PickableInventoryItem( item );
          });

          self.pickerData.pickedInventoryItems( mappedItems );
        });

        self.closePickerCommand.execute();

        return getOrder;
      }).always( complete );

      return updateItems;
    }
  });
  this.isPickerDirty = ko.pureComputed(function() {
    var _picker = self.pickerExports();

    return _picker && _picker() && _picker().isDirty();
  });

  this.savePickCommand = ko.asyncCommand({
    execute: function( complete ) {
      self.savePick.execute().always( complete );
    },
    canExecute: function( isExecuting ) {
      var picker = self.pickerExports();

      return !isExecuting && self.savePick.canExecute() && (!picker || self.isPickerDirty());
    }
  });
  function closePickerUI() {
    // Dispose data
    this.pickerData = null;

    // Disable picker UI
    self.isPickingInventory( false );
  }
  this.closePickerCommand = ko.command({
    execute: function() {
      if ( self.isPickerDirty() ) {
        showUserMessage( 'Save picks before closing?', {
          description: 'There are unsaved picked items. Would you like to save before closing?',
          type: 'yesnocancel',
          onYesClick: function() {
            self.savePickCommand.execute();
          },
          onNoClick: function() {
            closePickerUI();
          },
          onCancelClick: function() {
          },
        });
      } else {
        closePickerUI();
      }
    },
    canExecute: function() {
      return true;
    }
  });
  this.isPickSaving = ko.pureComputed(function() {
    return !self.savePick.canExecute();
  });

  this.orderStatusOptions = ko.utils.arrayMap(Object.keys(rvc.lists.orderStatus), function(key) {
    return rvc.lists.orderStatus[key];
  });

  this.isShowingDetails = ko.pureComputed(function() {
    return typeof self.movementKey() === 'string' && self.orderDetails();
  });
  this.isShowingContactPicker = ko.pureComputed(function() {
    var editor = self.editorExports();

    return editor && ko.unwrap( editor.isPickingContact );
  });

  // Behaviors
  this.closeCommand = ko.asyncCommand({
    execute: function(complete) {
      // TODO NJH: Confirm close if dirty
      page('/');
      complete();
    },
    canExecute: function(isExecuting) {
      return !isExecuting;
    }
  });

  this.getSearchKey = function() {
    var key = self.searchString();

    if (key) {
      getKey(key);
    }
  };
  this.getSelectedKey = function(key) {
    if (key) {
      getKey(key);
    }
  };

  this.getOrders = ko.asyncCommand({
    execute: function(complete) {
      if (self.summaryExports()) {
        self.isLoading(true);
        self.loadingMessage('Loading orders');

        var getOrdersList = self.summaryExports().getOrders().then(
          function(data, textStatus, jqXHR) {
          return data;
        });

        getOrdersList.always(function() {
          self.isLoading(false);
          self.loadingMessage('');
          complete();
        });

        return getOrdersList;
      } else {
        complete();
        return false;
      }
    },
    canExecute: function(isExecuting) {
      return !isExecuting && !self.isWorking();
    }
  });

  this.applyFilters = ko.asyncCommand({
    execute: function(complete) {
      if (self.summaryExports()) {
        self.isLoading(true);
        self.loadingMessage('Loading orders');

        var applySelectedFilters = self.summaryExports().applyFilters().then(null);

        var finishLoading = $.when(applySelectedFilters).always(function() {
          self.isLoading(false);
          self.loadingMessage('');
          complete();
        });

        return applySelectedFilters;
      } else {
        complete();
        return false;
      }
    },
    canExecute: function(isExecuting) {
      return !isExecuting && !self.isWorking();
    }
  });

  this.createNewOrder = ko.command({
    execute: function() {
      page('/new');
    },
    canExecute: function() {
      return true;
    }
  });

  this.closeContactPickerCommand = ko.command({
    execute: function() {
      var editor = self.editorExports();

      return editor && editor.closeContactPicker();
    },
    canExecute: function() {
      return true;
    }
  });
  this.saveOrderCommand = ko.asyncCommand({
    execute: function(complete) {
      var orderDetails = self.editorExports().toDto();
      if (!orderDetails) {
        complete();
        return $.Deferred().reject();
      }
      var key = self.movementKey();
      var _isNew = self.isNew();

      self.isLoading(true);
      self.loadingMessage('Saving Sales Order...');

      var shipMethod = orderDetails.SetShipmentInformation.Transit.ShipmentMethod;
      if ( !_isNew ) {
        var updateOrder = salesService.updateSalesOrder(key, orderDetails).then(
        function( data, textStatus, jqXHR ) {
          if (shipMethod && self.shipmentMethods().indexOf(shipMethod) < 0) {
            self.shipmentMethods.push(shipMethod);
          }

          showUserMessage('Order saved', {
                          description: ''.concat('The sales order <b>', self.orderDetails().MovementKey, '</b> has been saved.')
          });

          return data;
        },
        function( jqXHR, textStatus, errorThrown ) {
          var msg = jqXHR.responseJSON && jqXHR.responseJSON.message || textStatus;
          showUserMessage('Failed to save changes', { description: msg });
        });

        var getUpdatedDetails = updateOrder.then(function ( data ) {
          self.orderDetails(null);

          var getOrderDetails = navigateToKey( key );
          getOrderDetails.done(
            function ( data, textStatus, jqXHR ) {
            data.ShipmentStatus = data.Shipment && data.Shipment.Status;
            setOrderDetails( data );
            self.summaryExports().addOrder( data );

            return data;
          });

          return getOrderDetails;
        }).always(function() {
          self.isLoading(false);
          complete();
        });

        return getUpdatedDetails;
      } else {
        var saveNewOrder = salesService.createSalesOrder( orderDetails ).then(
        function( data, textStatus, jqXHR ) {
          if ( shipMethod && self.shipmentMethods().indexOf( shipMethod ) < 0 ) {
            self.shipmentMethods.push(shipMethod);
          }

          showUserMessage( 'Order created', {
            description: 'The sales order <b>' + data + '</b> has been saved.'
          });

          self.closeCommand.execute();
          self.getSelectedKey( data );

          return data;
        },
        function( jqXHR, textStatus, errorThrown ) {
          var msg = jqXHR.responseJSON && jqXHR.responseJSON.message || textStatus;
          showUserMessage('Failed to save changes', { description: msg });
        });

        var getCreatedDetails = saveNewOrder.then(
        function( data, textStatus, jqXHR ) {
          return salesService.getSalesOrder( data ).then(
          function ( data, textStatus, jqXHR ) {
            data.ShipmentStatus = data.Shipment && data.Shipment.Status;
            self.summaryExports().addOrder( data );

            return data;
          });
        }).always(function() {
          self.isLoading(false);
          complete();
        });

        return getCreatedDetails;
      }
    },
    canExecute: function(isExecuting) {
      var editor = self.editorExports();
      var isValid = editor && editor.isValid();

      return !isExecuting && isValid;
    }
  });

  this.deleteOrderCommand = ko.asyncCommand({
    execute: function( complete ) {
      var key = self.movementKey();

      showUserMessage( 'Delete sales order ""<strong>' + key + '</strong>""?', {
        description: 'Are you sure you want to delete this sales order? This action cannot be undone.',
        type: 'yesno',
        onYesClick: function() {
          var deleteOrder = salesService.deleteSalesOrder( key ).then(
          function( data, textStatus, jqXHR ) {
            self.summaryExports().deleteOrder( key );
            self.closeCommand.execute();
          }).always( complete );
        },
        onNoClick: function() {
          complete();
        }
      });
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  function getKey(key) {
    page('/' + key);
  }

  // Routing
  page.base('/Customers/SalesOrders');
  page('/:key?', navigateToKey);

  function navigateToKey(ctx) {
    var key = typeof ctx === "string" ? ctx : ctx.params.key;

    self.isLocked(true);

    if ( isRedirecting ) {
      isRedirecting = false;
      return;
    }

    self.orderDetails( null );

    if ( !key ) {
      self.movementKey(null);
      return null;
    } else if ( key === 'new' ) {
      self.movementKey('');
      self.isLoading(false);
      self.isLocked(false);
      self.orderDetails('new');
    } else if ( key ) {
      self.isLoading(true);
      self.loadingMessage('Fetching Order ' + key);

      var getPromise = salesService.getSalesOrder(key);
      getPromise.then(
        function(data, textStatus, jqXHR) {
          setOrderDetails(data);

          if (data.MovementKey !== key) {
            isRedirecting = true;
            page('/' + data.MovementKey);
          }

        },
        function(jqXHR, textStatus, errorThrown) {
          showUserMessage('Could not get sales order', { description: errorThrown });

          self.isLoading(false);
          self.loadingMessage('');

          page('/');
        });

      return getPromise;
    }
  }

  function setOrderDetails(data) {
    self.movementKey(data.MovementKey);
    self.isLoading(false);
    self.loadingMessage('');
    self.orderDetails(data);
    self.isLocked(data.IsLocked);
  }

  // Monitors for init status on components
  function init() {
    var getCustomerOptions = salesService.getCustomers().then(
      function(data, textStatus, jqXHR) {
      self.options.customers(data);
    });
    var getBrokerOptions = salesService.getBrokers().then(
      function(data, textStatus, jqXHR) {
      self.options.brokers(data);
    });
    var getFacilityLocationOption = salesService.getWarehouses().then(
      function(data, textStatus, jqXHR) {
      self.options.facilities(data);
    });
    var getProductOptions = productsService.getChileProducts().then(
      function(data, textStatus, jqXHR) {
      self.options.products(data);
    });
    var getMiscProductsOptions = productsService.getProductsByInventoryType('7').then(
    function( data, textStatus, jqXHR ) {
      self.options.nonInventoryProducts( data );
    });
    var getPackagingOptions = productsService.getPackagingProducts().then(
      function(data, textStatus, jqXHR) {
      self.options.packaging(data);
    });
    var getShipmentMethods = salesService.getShipmentMethods().then(
    function( data, textStatus, jqXHR ) {
      self.shipmentMethods( data );
    });

    var checkOptions = $.when(getCustomerOptions,
                              getBrokerOptions,
                              getFacilityLocationOption,
                              getProductOptions,
                              getMiscProductsOptions,
                              getPackagingOptions,
                              getShipmentMethods).then(
      null,
      function(jqXHR, textStatus, errorThrown) {
        showUserMessage('Could not load Sales orders', { description: errorThrown });
      });

    var checkComponentsInit = checkOptions.then(function() {
      var isInitialized = $.Deferred();
      var _initialized = false;

      (function(data, textStatus, jqXHR) {
        ko.computed(function() {
          if (!_initialized) {
            var summary = self.summaryExports();

            if (summary &&
                ko.unwrap(summary.isInit)) {
              _initialized = true;
            isInitialized.resolve(true);
            self.isInit(true);
            }
          }

          return true;
        });
      })();

      return isInitialized;
    });

    return checkComponentsInit;
  }

  $.when( init() ).done(function(data, textStatus, jqXHR) {
    self.isInit(true);
    page();
  });

  // Exports
  return this;
}

ko.punches.enableAll();

ko.applyBindings(new SalesOrdersViewModel());


