var PickOrderItemModel = require('App/models/PickOrderItemModel');
var salesService = require('App/services/salesService');
var rvc = require('rvc');

function SalesOrderItem(item) {
  var mappedItem = new PickOrderItemModel(item);

  // Set null values to "" to fix dirty checker issue
  var _quantity = mappedItem.Quantity();
  mappedItem.isMiscellaneous = item.isMiscellaneous;

  mappedItem.Quantity.rules.remove( function( rules ) {
    return rules.rule === 'min';
  });
  mappedItem.Quantity( _quantity != null ? "" + _quantity : "" );
  mappedItem.Quantity.extend({
    min: {
      onlyIf: function() {
        return !mappedItem.isMiscellaneous();
      },
      params: 0
    }
  });

  var _lotCode = mappedItem.CustomerLotCode();
  mappedItem.CustomerLotCode( _lotCode != null ? "" + _lotCode : "" );

  var _productCode = mappedItem.CustomerProductCode();
  mappedItem.CustomerProductCode( _productCode != null ? "" + _productCode : "" );

  // Create additional properties specific to sales orders
  mappedItem.PriceBase = this.buildUSDComputed(item.PriceBase).extend({ min: 0 });
  mappedItem.PriceFreight = this.buildUSDComputed(item.PriceFreight).extend({ min: 0 });
  mappedItem.PriceTreatment = this.buildUSDComputed(item.PriceTreatment).extend({ min: 0 });
  mappedItem.PriceWarehouse = this.buildUSDComputed(item.PriceWarehouse).extend({ min: 0 });
  mappedItem.PriceRebate = this.buildUSDComputed(item.PriceRebate).extend({ min: 0 });
  mappedItem.Contract = ko.observable(item.ContractKey);

  mappedItem.TotalCostPerLb = ko.pureComputed(function() {
    var price = +mappedItem.PriceBase();
    var freight = +mappedItem.PriceFreight();
    var treatment = +mappedItem.PriceTreatment();
    var warehouse = +mappedItem.PriceWarehouse();
    var rebate = +mappedItem.PriceRebate();
    var totalPrice = price + freight + treatment + warehouse + rebate;

    return totalPrice;
  });

  mappedItem.TotalCost = ko.pureComputed(function() {
    var weight = mappedItem.TotalWeight();
    var costPerLb = mappedItem.TotalCostPerLb();

    return weight * costPerLb;
  });

  if (item.ContractItemKey) {
    mappedItem.ContractItemKey = item.ContractItemKey;
  }

  mappedItem.toDto = this.toDto.bind( mappedItem );

  return mappedItem;
}

SalesOrderItem.prototype.buildUSDComputed = function(value) {
  var _usdValue = ko.observable("0.00").extend({ notify: 'always' });
  var computed = ko.computed({
    read: function() {
      return _usdValue();
    },
    write: function(newValue) {
      var sanitizedValue = String(newValue).replace(/[^0-9.-]/g, '');

      _usdValue((+sanitizedValue || 0).toFixed(2));
    }
  }).extend({ notify: 'always' });

  if (value) {
    computed(value);
  }

  return computed;
};

SalesOrderItem.prototype.toDto = function() {
  var dto = {
    ContractItemKey: this.ContractItemKey,
    ProductKey: this.Product().ProductKey,
    PackagingKey: this.PackagingKey,
    TreatmentKey: this.TreatmentKey().key,
    Quantity: this.Quantity,
    CustomerProductCode: this.CustomerProductCode,
    CustomerLotCode: this.CustomerLotCode,
    PriceBase: this.PriceBase,
    PriceFreight: this.PriceFreight,
    PriceTreatment: this.PriceTreatment,
    PriceWarehouse: this.PriceWarehouse,
    PriceRebate: this.PriceRebate,
  };

  return ko.toJS( dto );
};

/**
  * @param {Object} input -
  * @param {Object} options -
  * @param {Object} exports -
  */
function SalesOrderItemsVM(params) {
  if (!(this instanceof SalesOrderItemsVM)) { return new SalesOrderItemsVM(params); }

  var self = this;

  // Data
  /** Init data */
  var _inputData = ko.toJS(params.input);
  var _items = _inputData.PickOrder && _inputData.PickOrder.PickOrderItems || [];
  var _customer = ko.pureComputed(function() {
    var data = ko.unwrap( params.input.Customer );

    return data;
  });
  this.isMiscellaneous = params.isMiscellaneous;
  this.options = params.options || {};
  this.contracts = ko.observableArray([]);
  this.selectedContracts = ko.observableArray( [] );
  this.pickedItems = params.input.PickedInventory || ko.observableArray( [] );

  /** State */
  this.isSelectingContract = ko.observable(false);

  this.isMiscellaneous.subscribe(function( isMisc ) {
    self.orderItems([]);
  });

  function mapItem(item) {
    /** Provide options arrays for constructor */
    item.packageOptions = self.options.packaging();
    item.customerOptions = self.options.customers();
    item.productOptions = self.options.products();
    item.isMiscellaneous = self.isMiscellaneous;

    var mappedItem = new SalesOrderItem(item);
    var treatment = ko.utils.arrayFirst( mappedItem.TreatmentKey.options, function( opt ) {
      return +opt.key === +item.TreatmentKey;
    } );
    mappedItem.TreatmentKey( treatment );

    /** Find matching options and set properties */
    var product = ko.utils.arrayFirst(self.options.productOptions(), function(opt) {
      return opt.ProductKey === item.ProductKey;
    });

    var packaging = ko.utils.arrayFirst(self.options.packaging(), function(opt) {
      return opt.ProductKey === item.PackagingProductKey;
    });

    mappedItem.Product(product);
    mappedItem.Packaging(packaging);

    mappedItem.pickedItemsForContract = self.pickedItems.filter( function( item ) {
      return ko.unwrap(item.OrderItemKey) === ko.unwrap(mappedItem.OrderItemKey);
    });
    mappedItem.totalPickedWeight = ko.pureComputed(function() {
      var items = ko.unwrap( mappedItem.pickedItemsForContract );
      var weight = 0;

      ko.utils.arrayForEach( items, function( item ) {
        weight += ko.unwrap( item.WeightPicked );
      });

      return weight;
    });

    mappedItem.pickedOverWeight = ko.pureComputed(function() {
      return mappedItem.totalPickedWeight() > mappedItem.TotalWeight();
    });

    var _dirtyCheckProps = [ mappedItem.Product, mappedItem.CustomerProductCode, mappedItem.CustomerLotCode, mappedItem.Packaging, mappedItem.TreatmentKey, mappedItem.Quantity, mappedItem.PriceBase, mappedItem.PriceFreight, mappedItem.PriceTreatment, mappedItem.PriceWarehouse, mappedItem.PriceRebate ];

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

  this.orderItems = ko.observableArray(ko.utils.arrayMap(_items, mapItem));
  var hasItems = ko.pureComputed(function() {
    return self.orderItems().length > 0;
  });

  var _validation = ko.validatedObservable({
    orderItems: self.orderItems
  });

  this.TotalWeight = ko.pureComputed(function() {
    var total = 0;

    ko.utils.arrayForEach(self.orderItems(), function(item) {
      total += item.TotalWeight();
    });

    return total;
  });
  this.TotalCost = ko.pureComputed(function() {
    var total = 0;

    ko.utils.arrayForEach(self.orderItems(), function(item) {
      total += item.TotalCost();
    });

    return total;
  });
  this.TotalPickedWeight = ko.pureComputed(function() {
    var items = ko.unwrap( self.pickedItems );
    var weight = 0;

    ko.utils.arrayForEach( items, function( item ) {
      weight += ko.unwrap( item.WeightPicked );
    });

    return weight;
  });
  this.PickedOverWeight = ko.pureComputed(function() {
    return self.TotalPickedWeight() > self.TotalWeight();
  });


  // Behaviors
  var isValid = ko.pureComputed(function() {
    return _validation.isValid();
  });

  function toDto() {
    if ( isValid ) {
      var mappedItems = ko.utils.arrayMap(ko.toJS(self.orderItems), function( item ) {
        return item.toDto();
      });

      return mappedItems;
    } else {
      _validation.errors.showAllMessages();
      return null;
    }
  }

  this.closeContracts = function() {
    self.isSelectingContract(false);
    self.selectedContracts( [] );
  };

  this.getContracts = ko.asyncCommand({
    execute: function(complete) {
      if (self.contracts().length === 0) {
        var getContracts = salesService.getContractsForCustomer( _customer() ).then(
        function(data, textStatus, jqXHR) {
          return data;
        },
        function(jqXHR, textStatus, errorThrown) {
          showUserMessage('Could not get contracts', { description: errorThrown });
        }).always(complete);

        var getContractDetails = getContracts.then(
        function(data, textStatus, jqXHR) {
          var contracts = [];

          ko.utils.arrayForEach(data, function(contract) {
            var getDetails = salesService.getContractDetails(contract.CustomerContractKey).then(
            function(data, textStatus, jqXHR) {
              return data;
            });

            contracts.push(getDetails);
          });

          return contracts;
        });

        var checkDetails = getContractDetails.then(
        function(data, textStatus, jqXHR) {
          var checkAry = $.when.apply($, data);

          return checkAry;
        }).always(complete);

        var mapDetails = checkDetails.then(
        function() {
          var results = arguments;
          var mappedResults = [];

          ko.utils.arrayForEach(results, function(contract) {
            ko.utils.arrayMap(contract.ContractItems, function(item) {
              var mappedItem = item;

              mappedItem.ContractNumber = contract.ContractNumber;
              mappedItem.CustomerContractKey = contract.CustomerContractKey;
              mappedItem.TermBegin = contract.TermBegin;
              mappedItem.TermEnd = contract.TermEnd;
              mappedItem.ContractStatus = ko.observable( contract.ContractStatus ).extend({ contractStatus: true });
              mappedItem.Treatment = ko.observable( item.TreatmentKey ).extend({ treatmentType: true });
              mappedItem.PriceTotal = item.PriceBase + item.PriceFreight + item.PriceTreatment + item.PriceWarehouse + item.PriceRebate;

              mappedResults.push(mappedItem);
            });
          });

          self.contracts(mappedResults);
          self.isSelectingContract(true);

          return self.contracts;
        });

        return mapDetails;
      } else {
        self.isSelectingContract(true);
        complete();
      }
    },
    canExecute: function(isExecuting) {
      return _customer() && !isExecuting;
    }
  });

  this.addMiscItem = ko.command({
    execute: function() {
      var _newItem = new PickOrderItemModel({});
      var _mappedItem = mapItem( _newItem );

      self.orderItems.push( _mappedItem );
    },
    canExecute: function() {
      return self.isMiscellaneous();
    }
  });

  this.tableMode = ko.pureComputed(function() {
    return self.isMiscellaneous() ? 'non-inventory-item' : 'inventory-item';
  });

  this.selectContractItem = function(viewModel, element) {
    var contractItem = ko.contextFor(element.target).$data;

    if ( contractItem.hasOwnProperty( 'CustomerContractKey' ) ) {
      contractItem.ContractKey = contractItem.CustomerContractKey;
      contractItem.ProductKey = contractItem.ChileProductKey;

      var mappedItem = mapItem(contractItem);
      mappedItem.Quantity( null );

      self.orderItems.push(mappedItem);

      var selectedContracts = self.selectedContracts();
      var contractKey = mappedItem.Contract();
      if ( selectedContracts.indexOf( contractKey ) < 0 ) {
        self.selectedContracts.push( contractKey );
      }
    }
  };

  this.removeItem = ko.command({
    execute: function( data, element ) {
      var items = self.orderItems();
      var target = data;
      var matchedItem = ko.utils.arrayFirst( items, function( item ) {
        return item === target;
      });
      var itemIndex = items.indexOf( matchedItem );

      self.orderItems.splice( itemIndex, 1);
    },
    canExecute: function() {
      return true;
    }
  });

  // Exports
  if (params && params.exports) {
    params.exports({
      toDto: toDto,
      hasItems: hasItems,
      isValid: isValid
    });
  }
  return this;
}

module.exports = {
  viewModel: SalesOrderItemsVM,
  template: require('./sales-order-items.html')
};

