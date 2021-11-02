var rvc = require('rvc'),
  salesService = require('App/services/salesService');

require('App/helpers/koHelpers');

function SummaryItem( orderDetails ) {
  var self = this;

  this.Broker = orderDetails.Broker && orderDetails.Broker.Name;
  this.Customer = orderDetails.Customer && orderDetails.Customer.Name;
  this.DateOrderReceived = orderDetails.DateOrderReceived;
  this.MovementKey = orderDetails.MovementKey;
  this.OrderNum = orderDetails.OrderNum;
  this.ShipmentDate = orderDetails.ShipmentDate;
  this.ShipmentStatus = orderDetails.ShipmentStatus;
  this.OrderStatus = orderDetails.OrderStatus;
  var _statusObj = this.orderStatusList[ orderDetails.OrderStatus ];
  this.OrderStatusText = _statusObj && _statusObj.value;
}

SummaryItem.prototype.orderStatusList = (function() {
  var statusObjByKey = {};
  var orderStatusObj = rvc.lists.customerOrderStatus;
  var keys = Object.keys( orderStatusObj );

  ko.utils.arrayForEach( keys, function( key ) {
    var stat = orderStatusObj[key];

    statusObjByKey[stat.key] = stat;
  });

  return statusObjByKey;
})();

/**
  * @param {Function} getKey - Callback to trigger on item selection
  * @param {Object[]} filters - Observables, filters for item search
  * @param {Object} exports - Observable, container for exposed methods
  */
function SalesOrdersSummaryViewModel (params) {
  var self = this;

  var getKey = params.getKey;
  var isInit = ko.observable(false);
  var filters = params.filters;

  // Data
  this.orders = ko.observableArray([]);
  // this.shippedOrders = this.orders.filter(function( order ) {
  //   return order.ShipmentStatus === 10 && order.OrderStatus === 0;
  // });
  // this.unshippedOrders = this.orders.filter(function( order ) {
  //   return order.ShipmentStatus < 10 && order.OrderStatus === 0;
  // });
  // this.invoicedOrders = this.orders.filter(function( order ) {
  //   return order.ShipmentStatus >= 10 && order.OrderStatus === 1;
  // });

  this.shippedOrders = ko.observableArray( [] );
  this.unshippedOrders = ko.observableArray( [] );
  this.invoicedOrders = ko.observableArray( [] );
  this.orders.subscribe(function( orders ) {
    var _shipped = [];
    var _unshipped = [];
    var _invoiced = [];

    ko.utils.arrayForEach( orders, function( order ) {
      if ( order.ShipmentStatus === 10 && order.OrderStatus === 0 ) {
        _shipped.push( order );
      } else if ( order.ShipmentStatus < 10 && order.OrderStatus === 0 ) {
        _unshipped.push( order );
      } else if ( order.ShipmentStatus >= 10 && order.OrderStatus === 1 ) {
        _invoiced.push( order );
      }
    });

    self.shippedOrders( _shipped );
    self.unshippedOrders( _unshipped );
    self.invoicedOrders( _invoiced );
  });

  this.orderStatusList = (function() {
    var statusList = rvc.lists.customerOrderStatus,
      statusKeys = Object.keys(statusList),
      mappedList = {};

    ko.utils.arrayForEach(statusKeys, function(key) {
      mappedList[statusList[key].key] = statusList[key].value;
    });

    return mappedList;
  })();

  // Behaviors
  this.selectKey = function(vm, jQEvent) {
    var data = ko.contextFor(jQEvent.target).$data;

    if (data && data.MovementKey && getKey) {
      getKey(data.MovementKey);
    }
  };

  var pager = salesService.getSalesOrdersDataPager();
  if (filters) {
    pager.addParameters(filters);
  }

  pager.addNewPageSetCallback(resetUI);

  function resetUI() {
    self.orders([]);
  }

  function applyFilters() {
    pager.resetCursor();
    return getOrders();
  }

  function addOrder( orderDetails ) {
    var orders = self.orders();
    var order = new SummaryItem( orderDetails );

    /** Search orders and replace if movement key exists */
    var i;
    var max = orders.length;

    for ( i = 0; i < max; i += 1 ) {
      if ( orders[i].MovementKey === order.MovementKey ) {
        self.orders.splice(i, 1, order);

        return true;
      }
    }

    /** Else prepend to order list */
    self.orders.unshift( order );

    return true;
  }

  function updateOrder( orderDetails ) {
    var orders = self.orders();
    var order = new SummaryItem( orderDetails );

    var i;
    var max = orders.length;

    for ( i = 0; i < max; i += 1 ) {
      if ( orders[i].MovementKey === order.MovementKey ) {
        self.orders.splice(i, 1, order);

        return true;
      }
    }

    console.log( 'Could not find order in summary list' );
  }

  function deleteOrder( key ) {
    var orders = self.orders();

    ko.utils.arrayFirst( orders, function( order ) {
      if ( order.MovementKey === key ) {
        self.orders.splice( orders.indexOf( order ), 1 );
        return true;
      }
    });
  }

  function getOrders() {
    return pager.nextPage().done(function(data, textStatus, jqXHR) {
      var summaryEntries = ko.utils.arrayMap( data, mapResult );
      var ordersList = self.orders();

      self.orders( ordersList.concat(summaryEntries) );
    })
    .fail(function(jqXHR, textStatus, errorThrown) {
      showUserMessage('Could not fetch sales orders', { description: errorThrown });
    })
    .always(function() {
      isInit(true);
    });

    function mapResult( orderDetails ) {
      return new SummaryItem( orderDetails );
    }
  }

  this.addOrder = addOrder;

  if (params.exports) {
    params.exports({
      applyFilters: applyFilters,
      getOrders: getOrders,
      addOrder: addOrder,
      updateOrder: updateOrder,
      deleteOrder: deleteOrder,
      isInit: isInit
    });
  }

  getOrders();

  // Exports
  return this;
}

module.exports = {
  viewModel: SalesOrdersSummaryViewModel,
  template: require('./sales-orders-summary.html')
};
