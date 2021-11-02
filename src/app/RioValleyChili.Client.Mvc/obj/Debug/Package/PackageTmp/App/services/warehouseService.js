var warehouseService = (function () {
    var serviceCore = require('services/serviceCore');
    var dataPager = require('helpers/pagedDataHelper');

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

module.exports = $.extend(warehouseService, require('services/warehouseLocationsService'));
