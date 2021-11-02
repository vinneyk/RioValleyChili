var salesService = require('App/services/salesService');

/**
  * @param {Object} input - Input data
  * @param {string} input.lotKey - Target lot key
  * @param {Object[]} input.allowances - Existing allowances
  * @param {Object[]} input.customers - List of customers
  * @param {function} input.createAllowance - Callback for creating allowance, returns promise
  * @param {function} input.deleteAllowance - Callback for deleting allowance, returns promise
  */

ko.punches.enableAll();

function AllowancesEditorVM( params ) {
  if ( !(this instanceof AllowancesEditorVM) ) { return new AllowancesEditorVM( params ); }

  var self = this;
  var input = params.input || {};

  // Data
  this.lotKey = input.lotKey;
  this.allowances = {
    contracts: ko.observableArray( input.allowances.contracts ),
    customers: ko.observableArray( input.allowances.customers ),
    customerOrders: ko.observableArray( input.allowances.customerOrders ),
  };
  this.totalAllowances = ko.pureComputed(function() {
    var allowances = self.allowances;
    return allowances.contracts().length + allowances.customers().length + allowances.customerOrders().length;
  });
  this.customers = ko.unwrap( input.customers );
  this.customerKey = ko.observable( null ).extend({ notify: 'always' });

  this.type = ko.observable( null );
  this.types = ['Contract', 'Order'];

  this.contracts = ko.observableArray( [] );
  this.selectedContract = ko.observable( null );

  this.orders = ko.observableArray( [] );
  this.selectedOrder = ko.observable( null );

  this.customerKey.subscribe(function( customerKey ) {
    self.contracts( [] );
    self.selectedContract( null );
    self.orders( [] );
    self.selectedOrder( null );
    self.isFirstQuery = true;
  });

  var orderPager = salesService.getSalesOrdersDataPager();

  orderPager.addParameters({
    customerKey: self.customerKey
  });

  orderPager.addNewPageSetCallback(function() {
    self.orders( [] );
  });

  this.key = ko.pureComputed(function() {
    var type = self.type();

    if ( type === 'Contract' ) {
      var contract = self.selectedContract();

      return contract && contract.CustomerContractKey;
    } else if ( type === 'Order' ) {
      var order = self.selectedOrder();

      return order && order.MovementKey;
    }
  }).extend({ notify: 'always' });

  this.keyExists = ko.pureComputed(function() {
    var type = self.type();
    var customerKey = null;
    var allowanceKey = null;

    if ( type === 'Contract' ) {
      var contract = self.selectedContract();
      allowanceKey = self.key();

      return ko.utils.arrayFirst( self.allowances.contracts(), function( contract ) {
        return contract.ContractKey === allowanceKey;
      });
    } else if ( type === 'Order' ) {
      var order = self.selectedOrder();
      allowanceKey = self.key();

      return ko.utils.arrayFirst( self.allowances.customerOrders(), function( order ) {
        return order.OrderKey === allowanceKey;
      });
    } else {
      customerKey = self.customerKey();
      return ko.utils.arrayFirst( self.allowances.customers(), function( customer ) {
        return customer.CustomerKey === customerKey;
      });
    }
  });

  this.keyTemplate = ko.pureComputed(function() {
    var type = self.type();

    if ( type === 'Contract' ) {
      return 'allowances-contract';
    } else if ( type === 'Order' ) {
      return 'allowances-order';
    }
  });

  this.customerData = ko.computed({
    read: function() {
      var key = self.customerKey();
      var customers = self.customers;
      var customer = ko.utils.arrayFirst( customers, function( customer ) {
        return customer.CompanyKey === key;
      });

      return customer && customer.Name;
    },
    write: function( key ) {
      self.customerKey( key );
    }
  }).extend({ notify: 'always' });

  // Behaviors
  this.getContracts = ko.asyncCommand({
    execute: function( complete ) {
      var customer = self.customerKey();

      var getContracts = salesService.getContractsForCustomer( customer ).then(
      function( data, textStatus, jqXHR ) {
        self.contracts( data );
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting && self.customerKey();
    }
  });

  this.selectContractItem = function( data, $element ) {
    var $tr = $( $element.target ).closest('tr');

    if ( $tr.length ) {
      var context = ko.contextFor( $tr[0] );

      if ( self.selectedContract() === context.$data ) {
        self.selectedContract( null );
      } else {
        self.selectedContract( context.$data );
      }
    }
  };
  
  this.getOrders = ko.asyncCommand({
    execute: function( complete ) {
      var getOrders = orderPager.nextPage().then(
      function( data, textStatus, jqXHR ) {
        self.orders( self.orders().concat( data ) );
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting && self.customerKey();
    }
  });

  this.selectOrder = function( data, $element ) {
    var $tr = $( $element.target ).closest('tr');

    if ( $tr.length ) {
      var context = ko.contextFor( $tr[0] );

      if ( self.selectedOrder() === context.$data ) {
        self.selectedOrder( null );
      } else {
        self.selectedOrder( context.$data );
      }
    }
  };

  this.createAllowance = ko.asyncCommand({
    execute: function( complete ) {
      var lotKey = self.lotKey;

      var customerKey = self.customerKey();
      var customerName = self.customerData();

      var allowanceType = self.type() || 'Customer';
      var allowanceKey = self.key();
      var allowanceData = {};
      var allowancesForType = null;

      if ( allowanceType === 'Customer' ) {
        allowanceKey = customerKey;
        allowancesForType = self.allowances.customers;
      } else if ( allowanceType === 'Contract' ) {
        allowancesForType = self.allowances.contracts;
        allowanceData = self.selectedContract();
      } else if ( allowanceType === 'Order' ) {
        allowancesForType = self.allowances.customerOrders;
        allowanceData = self.selectedOrder();
      }

      var create = input.createAllowance( lotKey, allowanceType, allowanceKey ).then(
      function( data, textStatus, jqXHR ) {
        if ( allowanceType === 'Customer' ) {
          allowancesForType.push({
            CustomerKey: customerKey,
            CustomerName: customerName,
          });
        } else if ( allowanceType === 'Contract' ) {
          allowancesForType.push({
            CustomerKey: customerKey,
            CustomerName: customerName,
            ContractKey: allowanceKey,
            TermBegin: allowanceData.TermBegin,
            TermEnd: allowanceData.TermEnd,
          });
        } else if ( allowanceType === 'Order' ) {
          allowancesForType.push({
            CustomerKey: customerKey,
            CustomerName: customerName,
            OrderKey: allowanceKey,
            OrderNumber: allowanceData.OrderNum,
          });

          self.type( null );
          self.customerKey( null );
        }
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      var key = self.type() ?
        self.key() :
        self.customerKey();

      return !isExecuting && key != null && key !== '' && !self.keyExists();
    }
  });

  this.deleteAllowance = ko.asyncCommand({
    execute: function( complete ) {
      var lotKey = self.lotKey;
      var allowanceType = null;
      var allowanceKey = null;
      var allowancesForType = null;
      var allowance = this;

      if ( this.hasOwnProperty( 'ContractKey' ) ) {
        allowanceType = 'Contract';
        allowanceKey = this.ContractKey;
        allowancesForType = self.allowances.contracts;
      } else if ( this.hasOwnProperty( 'OrderKey' ) ) {
        allowanceType = 'Order';
        allowanceKey = this.OrderKey;
        allowancesForType = self.allowances.customerOrders;
      } else if ( this.hasOwnProperty( 'CustomerKey' ) ) {
        allowanceType = 'Customer';
        allowanceKey = this.CustomerKey;
        allowancesForType = self.allowances.customers;
      }

      var remove = input.deleteAllowance( lotKey, allowanceType, allowanceKey ).then(
        function( data, textStatus, jqXHR ) {
        var allowanceIndex = allowancesForType().indexOf( allowance );

        if ( allowanceIndex > -1 ) {
          allowancesForType.splice( allowanceIndex, 1 );
        }
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });


  // Exports
  if ( params && params.exports ) {
    params.exports({
      totalAllowances: self.totalAllowances
    });
  }

  return this;
}

module.exports = {
  viewModel: AllowancesEditorVM,
  template: require('./allowances-editor.html')
};
