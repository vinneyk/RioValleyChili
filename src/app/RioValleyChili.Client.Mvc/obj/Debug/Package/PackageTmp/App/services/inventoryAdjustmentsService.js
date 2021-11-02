var core = require('App/services/serviceCore');

/** Service/API methods for Inventory Adjustments */
var inventoryAdjustments = (function InventoryAdjustmentsService() {
  function getInventoryAdjustmentsPager( options ) {
    var _options = options || {};

    return core.pagedDataHelper.init({
      urlBase: _options.baseUrl || "/api/inventoryAdjustments",
      pageSize: _options.pageSize || 50,
      parameters: _options.parameters,
      onNewPageSet: _options.onNewPageSet,
    });
  }

  function getInventoryAdjustment( key ) {
    return core.ajax( '/api/inventoryAdjustments/' + key );
  }

  function postInventoryAdjustment( data ) {
    return core.ajaxPost( '/api/inventoryAdjustments/', data );
  }

  /** Public Methods */
  return {
    getInventoryAdjustmentsPager: getInventoryAdjustmentsPager,
    getInventoryAdjustment: getInventoryAdjustment,
    postInventoryAdjustment: postInventoryAdjustment
  };
})();

module.exports = inventoryAdjustments;
