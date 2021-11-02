var salesService = require('App/services/salesService');
var rvc = require('rvc');
var PickedItem = require('App/models/PickableInventoryItem');

var APP_CONSTANTS = {
  INVOICE_NOTE_LENGTH: 600
};

/**
  * @param {Object} input - Observable, input data to initialize editor
  * @param {Object} options - Observable, options to populate select boxes
  * @param {Object} exports - Observable, container for editor return
  */

require('bootstrap');

ko.components.register('shipment-editor', require('App/components/warehouse/shipment-editor/shipment-editor'));
ko.components.register('sales-order-items', require('App/components/sales/sales-order-items/sales-order-items'));
ko.components.register( 'contact-address-label-helper', require('App/components/common/address-book/company-address-label-helper') );
ko.components.register( 'picked-inventory-table', require('App/components/inventory/inventory-picking-table/inventory-picking-table') );

function SalesOrdersDetailsViewModel(params) {
  if (!(this instanceof SalesOrdersDetailsViewModel)) { return new SalesOrdersDetailsViewModel(params); }

  var self = this;

  this.APP_CONSTANTS = APP_CONSTANTS;

  // Data
  var _orderDetails = ko.unwrap(params.input);
  var _isNew = typeof _orderDetails === 'string';

  // TODO NJH: Add shipment method options
  this.isLoadingCustomer = ko.observable( false );
  this.isPickingInventory = ko.observable( false );
  this.options = params.options;
  this.isMiscellaneous = ko.observable( false );
  this.options.productOptions = ko.pureComputed(function() {
    var _isMisc = self.isMiscellaneous();

    return _isMisc ? self.options.nonInventoryProducts() : self.options.products();
  });

  this.changeMiscMode = function() {
    var orderItems = self.exports.orderItemsEditor();

    if ( _isNew ) {
      if ( orderItems && orderItems.hasItems() ) {
        showUserMessage( 'Change editor mode?', {
          description: 'Changing modes will clear the order items table. Are you sure you want to continue?',
          type: 'yesno',
          onYesClick: function() {
            self.isMiscellaneous( !self.isMiscellaneous() );
          },
          onNoClick: function() { }
        });
      } else {
        self.isMiscellaneous( !self.isMiscellaneous() );
      }
    }
  };

  this.movementKey = _isNew ?
    null :
    _orderDetails.MovementKey;

  this.editorData = {
    originFacility: ko.observable().extend({ required: true }),
    broker: ko.observable(),
    preShipmentSampleRequired: ko.observable(),
    shipmentDate: ko.observableDate(),
    purchaseOrderNumber: ko.observable(),
    dateOrderReceived: ko.observableDate().extend({ required: true }),
    orderTakenBy: ko.observable(),
    shipmentMethod: ko.observable(),
    requiredDeliveryDate: ko.observableDate(),
    orderRequestedBy: ko.observable(),
    customerOrderStatus: ko.observable().extend({ customerOrderStatusType: true }),
    shipment: ko.observable(),
    paymentTerms: ko.observable(),
    pickedInventoryItems: ko.observableArray( [] ),
    pickedInventory: ko.observable(  ),
    IsMiscellaneous: self.isMiscellaneous,
    invoice: {
      creditMemo: ko.observable( null ),
      invoiceDate: ko.observableDate( null ),
      invoiceNotes: ko.observable( null ),
      freightCharge: ko.observable( null ),
    },
    notes: {
      internalNotes: ko.observable().extend({ maxLength: APP_CONSTANTS.INVOICE_NOTE_LENGTH }),
      externalNotes: ko.observable().extend({ maxLength: APP_CONSTANTS.INVOICE_NOTE_LENGTH }),
      specialInstructions: ko.observable().extend({ maxLength: APP_CONSTANTS.INVOICE_NOTE_LENGTH }),
    },
    customerKey: ko.observable( null ).extend({ notify: 'always' }),
  };

  this.editorData.customerKey.extend({
    required: {
      onlyIf: function() {
        return !self.editorData.IsMiscellaneous();
      }
    }
  });
  this.editorData.broker.extend({
    required: {
      onlyIf: function() {
        return !self.editorData.IsMiscellaneous();
      }
    }
  });

  this.startInventoryPicker = ko.command({
    execute: function (data, element) {
      var orderKey = ko.unwrap(self.movementKey);
      if (orderKey == null) {
        return;
      }
      if (data.OrderItemKey == null || data.dirtyFlag.isDirty()) {
        showUserMessage("Unsaved Order Item", { description: "Please save the order before picking inventory."});
        return;
      }

      var productData = ko.unwrap( data.Product );
      var packagingData = ko.unwrap( data.Packaging );
      var treatmentData = ko.unwrap( data.TreatmentKey );

      // Set filters
      var filters = params.filters();
      filters.inventoryType( productData.ProductType );
      filters.lotType( productData.ChileState );
      filters.productKey( productData.ProductKey );
      filters.packagingProductKey( packagingData.ProductKey );
      filters.treatmentKey( treatmentData.key );

      // Compile data
      var pickerData = {
        args: {
          orderItemKey: data.OrderItemKey
        },
        pickingContext: rvc.lists.inventoryPickingContexts.CustomerOrder,
        pickingContextKey: orderKey,
        pickedInventoryItems: self.editorData.pickedInventoryItems,
        filters: params.filters,
        targetProduct: data.Product,
        targetWeight: ko.unwrap( data.TotalWeight ),
        customerKey: self.editorData.customerKey,
        customerLotCode: ko.unwrap( data.CustomerLotCode ),
        customerProductCode: ko.unwrap( data.CustomerProductCode ),
      };

      // Start Picker component
      params.startPicker.execute( pickerData );
    },
    canExecute: function() {
      return !_isNew;
    }
  });

  this.editorData.customer = ko.computed({
    read: function() {
      var key = self.editorData.customerKey();
      var customers = ko.unwrap( self.options.customers );
      var customer = ko.utils.arrayFirst( customers, function( customer ) {
        return customer.CompanyKey === key;
      });

      return customer && customer.Name;
    },
    write: function( key ) {
      self.editorData.customerKey( key );
    }
  }).extend({ notify: 'always' });

  if (!_isNew) {
    this.editorData.originFacility(_orderDetails.OriginFacility.FacilityKey);
    this.editorData.customerKey(_orderDetails.Customer && _orderDetails.Customer.CompanyKey);
    this.editorData.broker(_orderDetails.Broker && _orderDetails.Broker.CompanyKey);
    this.editorData.preShipmentSampleRequired(_orderDetails.SampleRequired);
    this.editorData.shipmentDate(getUTCDateString(_orderDetails.ShipmentDate));
    this.editorData.purchaseOrderNumber(_orderDetails.PurchaseOrderNumber);
    this.editorData.dateOrderReceived(getUTCDateString(_orderDetails.DateOrderReceived));
    this.editorData.requiredDeliveryDate(_orderDetails.Shipment.ShippingInstructions.RequiredDeliveryDateTime);
    this.editorData.orderTakenBy(_orderDetails.OrderTakenBy);
    this.editorData.orderRequestedBy(_orderDetails.OrderRequestedBy);
    this.editorData.customerOrderStatus(_orderDetails.SalesOrderStatus);
    this.editorData.shipment(_orderDetails.Shipment);
    this.editorData.shipmentMethod(_orderDetails.Shipment.Transit.ShipmentMethod);
    this.editorData.paymentTerms(_orderDetails.PaymentTerms);
    this.editorData.pickedInventoryItems( ko.utils.arrayMap( _orderDetails.PickedInventory.PickedInventoryItems, function( item ) {
      return new PickedItem( item );
    } ) );
    this.editorData.invoice.creditMemo( _orderDetails.CreditMemo );
    this.editorData.invoice.invoiceDate( _orderDetails.InvoiceDate );
    this.editorData.invoice.invoiceNotes( _orderDetails.InvoiceNotes );
    this.editorData.invoice.freightCharge( _orderDetails.FreightCharge );
    this.editorData.IsMiscellaneous( _orderDetails.IsMiscellaneous );

    // TODO NJH: Replace w/ API provided solution for finding attributes
    var inventoryAttrs = ko.utils.arrayFirst( _orderDetails.PickedInventory.AttributeNamesByProductType, function( attr ) {
      return attr.Key === 1;
    } );

    this.editorData.pickedInventory({
      attributes: inventoryAttrs && inventoryAttrs.Value,
      targetProduct: null,
      inventoryItems: self.editorData.pickedInventoryItems,
      isReadOnly: true
    });
    this.editorData.notes.internalNotes(_orderDetails.Shipment.ShippingInstructions.InternalNotes);
    this.editorData.notes.externalNotes(_orderDetails.Shipment.ShippingInstructions.ExternalNotes);
    this.editorData.notes.specialInstructions(_orderDetails.Shipment.ShippingInstructions.SpecialInstructions);
  } else if ( _isNew ) {
    this.editorData.shipment( {} );
    var onCustomerKeyChange = this.editorData.customerKey.subscribe(function( key ) {
      if ( key ) {
        self.isLoadingCustomer( true );
        var getContracts = salesService.getContractsForCustomer( key ).then(
        function( data, textStatus, jqXHR ) {
          var latestContract = ko.utils.arrayFirst( data, function( contract ) {
            return contract.ContractStatus === 2;
          });

          if ( latestContract ) {
            self.editorData.broker( latestContract.BrokerCompanyKey );
            self.editorData.paymentTerms( latestContract.PaymentTerms );
          }
        }).always(function() {
          self.isLoadingCustomer( false );
        });
      }
    });
  }

  var _validation = ko.validatedObservable({
    origin: self.editorData.originFacility,
    customer: self.editorData.customerKey,
    broker: self.editorData.broker,
    date: self.editorData.shipmentDate,
    internalNotes: self.editorData.notes.internalNotes,
    externalNotes: self.editorData.notes.externalNotes,
    specialInstructions: self.editorData.notes.specialInstructions,
  });

  this.exports = {
    orderItemsEditor: ko.observable(),
    shipmentEditor: ko.observable(),
  };

  this.inputs = {
    salesOrderItems: {
      Customer: self.editorData.customerKey,
      PickOrder: _orderDetails.PickOrder,
      PickedInventory: self.editorData.pickedInventoryItems
    },

    contactPicker: {
      visible: ko.observable( false ),
      buttons: [{
        text: 'Sold To',
        callback: function( contactInfo ) {
          setAddressLabel( 'soldTo', contactInfo );
        }
      },
      {
        text: 'Ship To',
        callback: function( contactInfo ) {
          setAddressLabel( 'shipTo', contactInfo );
        }
      },
      {
        text: 'Bill To',
        callback: function( contactInfo ) {
          setAddressLabel( 'billTo', contactInfo );
        }
      }
      ]
    },

    itemPicker: {
      pickForContract: self.startInventoryPicker,
    }
  };

  var canPostInvoice = ko.pureComputed(function() {
    var shipment = self.editorData.shipment();

    // Order Status: 1 === "Invoiced"
    // Shipment Status: 10 === "Shipped"
    return self.editorData.customerOrderStatus() !== 1 &&
      shipment && shipment.Status === 10;
  });

  var canPostOrder = ko.pureComputed(function() {
    var shipment = self.editorData.shipment();

    // Shipment Status: 10 === "Shipped"
    return shipment && shipment.Status < 10 && (self.editorData.pickedInventoryItems().length > 0 || self.isMiscellaneous());
  });

  this.isNew = ko.pureComputed(function() {
    return _isNew;
  });

  // Behaviors
  function setAddressLabel( label, contactInfo ) {
    var editor = self.exports.shipmentEditor();
    contactInfo.Name = self.editorData.customer();

    if ( label === 'soldTo' ) {
      editor.setShipFromAddress( contactInfo );
    } else if ( label === 'shipTo' ) {
      editor.setShipToAddress( contactInfo );
    } else if ( label === 'billTo' ) {
      editor.setFreightBillAddress( contactInfo );
    }
  }

  this.showContactPicker = ko.command({
    execute: function() {
      self.inputs.contactPicker.visible( true );
    }
  });

  function getUTCDateString(dateStr) {
    var dateObj = new Date(dateStr);
    var newDateStr = "".concat(dateObj.getUTCMonth() + 1, '/', dateObj.getUTCDate(), '/', dateObj.getUTCFullYear());

    return newDateStr;
  }

  var isValid = ko.pureComputed(function() {
    var orderEditor = self.exports.orderItemsEditor();
    var isOrderItemsValid = orderEditor && orderEditor.isValid();

    return _validation.isValid() && isOrderItemsValid;
  });

  function toDto() {
    if ( isValid() ) {
      var orderItems = self.exports.orderItemsEditor().toDto();
      var editor = ko.toJS( self.editorData );
      var notes = ko.toJS( self.editorData.notes );
      var shipment = self.exports.shipmentEditor().toDto();

      /** API Model */
      var dto = {
        CustomerKey: editor.customerKey,
        BrokerKey: editor.broker,
        FacilitySourceKey: editor.originFacility,
        PreShipmentSampleRequired: editor.preShipmentSampleRequired || false,
        InvoiceDate: editor.invoice.invoiceDate || null,
        InvoiceNotes: editor.invoice.invoiceNotes,
        FreightCharge: editor.invoice.freightCharge,
        HeaderParameters: {
          CustomerPurchaseOrderNumber: editor.purchaseOrderNumber,
          ShipmentDate: editor.shipmentDate,
          PaymentTerms: editor.paymentTerms,
          DateOrderReceived: editor.dateOrderReceived,
          OrderRequestedBy: editor.orderRequestedBy,
          OrderTakenBy: editor.orderTakenBy,
        },
        SetShipmentInformation: shipment,
        PickOrderItems: orderItems,
        IsMiscellaneous: editor.IsMiscellaneous
      };

      if ( !_isNew ) {
        dto.SalesOrderKey = self.movementKey;
        dto.CreditMemo = editor.invoice.creditMemo;
      }

      /** */
      dto.SetShipmentInformation.ShippingInstructions.RequiredDeliveryDateTime = editor.requiredDeliveryDate;
      dto.SetShipmentInformation.ShippingInstructions.InternalNotes = notes.internalNotes;
      dto.SetShipmentInformation.ShippingInstructions.ExternalNotes = notes.externalNotes;
      dto.SetShipmentInformation.ShippingInstructions.SpecialInstructions = notes.specialInstructions;
      dto.SetShipmentInformation.Transit.ShipmentMethod = editor.shipmentMethod;

      return dto;
    } else {
      _validation.errors.showAllMessages();
      showUserMessage('Please correct validation errors.');

      return null;
    }
  }

  function orderToDto() {
    var pickedItems = ko.utils.arrayMap( self.editorData.pickedInventoryItems(), function( item ) {
      return {
        PickedInventoryItemKey: item.InventoryKey,
        DestinationLocationKey: null,
      };
    });

    return {
      PickedItemDestionations: pickedItems
    };
  }

  function closeContactPicker() {
    self.inputs.contactPicker.visible( false );
  }

  function setStatusAsInvoiced() {
    self.editorData.customerOrderStatus( 1 );
  }

  function displayShipmentData( orderDetails ) {
    self.editorData.shipment( null );
    self.editorData.shipment( orderDetails.Shipment );
  }

  // Exports
  if (params && params.exports) {
    params.exports({
      isPickingContact: self.inputs.contactPicker.visible,
      closeContactPicker: closeContactPicker,
      toDto: toDto,
      isValid: isValid,
      orderToDto: orderToDto,
      canPostInvoice: canPostInvoice,
      canPostOrder: canPostOrder,
      setStatusAsInvoiced: setStatusAsInvoiced,
      displayShipmentData: displayShipmentData,
      reportLinks: ko.pureComputed(function () {
        var data = ko.unwrap(params.input);
        var links = {
          customer: [],
          warehouse: []
        };

        if (data != null) {

          ko.utils.arrayPushAll(links.customer, [
            {
              name: 'Customer Order Confirmation',
              url: data.Links && data.Links['report-customer-order-confirmation'] && data.Links['report-customer-order-confirmation'].HRef
            },
            {
              name: 'In-House Confirmation',
              url: data.Links && data.Links['report-in-house-confirmation'] && data.Links['report-in-house-confirmation'].HRef
            },
            {
              name: 'Customer Order Invoice',
              url: data.Links && data.Links['report-customer-order-invoice'] && data.Links['report-customer-order-invoice'].HRef
            },
            {
              name: 'In-House Order Invoice',
              url: data.Links && data.Links['report-in-house-invoice'] && data.Links['report-in-house-invoice'].HRef
            }
          ]);

          ko.utils.arrayPushAll(links.warehouse, [{
              name: 'Pick Sheet',
              url: data.Links && data.Links['report-inv_shipment-pick-sheet'] && data.Links['report-inv_shipment-pick-sheet'].HRef
            },
            {
              name: 'COA Worksheet',
              url: data.Links && data.Links['report-inv_shipment-coa'] && data.Links['report-inv_shipment-coa'].HRef
            },
            {
              name: 'Packing List',
              url: data.Links && data.Links['report-inv_shipment-packing-list'] && data.Links['report-inv_shipment-packing-list'].HRef
            },
            {
              name: 'Packing List Barcode',
              url: data.Links && data.Links['report-inv_shipment-packing-list-bar-code'] && data.Links['report-inv_shipment-packing-list-bar-code'].HRef
            },
            {
              name: 'Bill of Lading',
              url: data.Links && data.Links['report-inv_shipment-bol'] && data.Links['report-inv_shipment-bol'].HRef
            }
          ]);
        }

        return links;
      })
    });
  }
  return this;
}

module.exports = {
  viewModel: SalesOrdersDetailsViewModel,
  template: require('./sales-orders-details.html')
};
