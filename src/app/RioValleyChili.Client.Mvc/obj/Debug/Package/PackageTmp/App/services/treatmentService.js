var treatmentService = (function () {
  var serviceCore = require('services/serviceCore');
  var dataPager = require('helpers/pagedDataHelper');

  return {
    createTreatmentOrderMovement: createTreatmentOrderMovement,
    getWarehouseDetails: getWarehouseDetails,
    getWarehouses: getWarehouses,
    getTreatmentOrdersDataPager: getTreatmentOrdersDataPager,
    getTreatmentOrderDetails: getTreatmentOrderDetails,
    receiveTreatmentOrderMovement: receiveTreatmentOrderMovement,
    updateTreatmentOrderMovement: updateTreatmentOrderMovement,
    deleteTreatmentOrder: deleteTreatmentOrder,
  };

  function getWarehouseDetails(key) {
    return serviceCore.ajax('/api/facilities/' + key);
  }
  function getWarehouses() {
    return serviceCore.ajax('/api/facilities/');
  }
  function getTreatmentOrdersDataPager(options) {
    options = options || {};
    options.urlBase = buildTreatmentOrderUrl();
    return dataPager.init(options);
  }
  function getTreatmentOrderDetails(key) {
    return serviceCore.ajax(buildTreatmentOrderUrl(key));
  }
  function createTreatmentOrderMovement(values) {
    return serviceCore.ajaxPost(buildTreatmentOrderUrl(), values);
  }
  function updateTreatmentOrderMovement(key, values) {
    return serviceCore.ajaxPut(buildTreatmentOrderUrl(key), values);
  }
  function deleteTreatmentOrder(key) {
    return serviceCore.ajaxDelete(buildTreatmentOrderUrl(key));
  }
  function receiveTreatmentOrderMovement(key, values) {
    return serviceCore.ajaxPost(buildTreatmentOrderUrl(key) + '/Receive', values);
  }

  function buildTreatmentOrderUrl(key) {
    return ['/api/TreatmentOrders/', key || ''].join('');
  }
})();

module.exports = $.extend(treatmentService, require('services/warehouseLocationsService'));
