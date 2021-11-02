var warehouseLocationsService = (function () {
    var serviceCore = require('services/serviceCore');
    var rvc = require('app');

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