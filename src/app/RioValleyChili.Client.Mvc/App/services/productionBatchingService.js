define(['services/serviceCore', 'services/lotService', 'services/inventoryService', 'services/directoryService', 'services/productsService', 'helpers/pagedDataHelper', 'app'],
    function (core, lotService, inventoryService, directoryService, productsService, pagedDataHelper, app) {
    return {
        createPackSchedule: core.setupFn(createPackSchedule, psUrl),
        createProductionBatch: function(data, options) {
           return core.ajaxPost(batchUrl(), data, options);
        },
        deletePackSchedule: core.setupFn(deletePackSchedule, psUrl),
        deleteProductionBatch: core.setupFn(deleteProductionBatch, batchUrl),
        getAttributeNames: lotService.getAttributeNames,
        getBatchInstructionOptions: function(options) {
           return core.ajax('/api/productionbatchinstructions', options);
        },
        getBatchPickingInventoryPager: function (batchKey, options) {
            //todo: remove after the production batch UI has been updated to utilize the new inventory picking UI component.
            return inventoryService.getPickableInventoryPager(app.lists.inventoryPickingContexts.ProductionBatch, batchKey, options);
        },
        getChileProducts: productsService.getChileProducts,
        getCustomers: directoryService.getCustomers,
        getProductDetails: productsService.getProductDetails,
        getPackagingProducts: productsService.getPackagingProducts,
        getPackScheduleDetails: core.setupFn(getPackScheduleDetails, psUrl),
        getPackSchedulesPager: function (options) {
            options = options || {};
            options.urlBase = psUrl();
            return pagedDataHelper.init(options);
        },
        getProductionBatchDetails: function(batchKey, options) {
            return core.ajax(batchUrl(batchKey), options);
        },
        getProductionLocations: function (options) {
            return core.ajax("/api/productionLines", options);
        },
        getWorkModes: function () {
            return core.ajax("/api/workModes");
        },
        //todo: refactor into inventoryPickingService
        savePickedInventoryForBatch: function(batchKey, data, options) {
            return core.ajaxPost('/api/' + app.lists.inventoryPickingContexts.ProductionBatch.value.toLowerCase() + '/' + batchKey + '/pick', data, options);
        },
        updatePackSchedule: core.setupFn(updatePackSchedule, psUrl),
        updateProductionBatch: function(batchKey, data, options) {
            return core.ajaxPut(batchUrl(batchKey), data, options);
        },
    }
    
    //#region url functions
    function batchUrl(key) {
        var url = "/api/productionbatches";
        if (key != undefined) url = url + '/' + key;
        return url;
    }

    function psUrl(key) {
        var url = "/api/packSchedules";
        if (key != undefined) url = url + '/' + key;
        return url;
    }
    //#endregion

    //#region service functions
    function createPackSchedule(data) {
        return core.ajaxPost(psUrl(), data);
    }
    function deleteProductionBatch(batchKey, options) {
        return core.ajaxDelete(batchUrl(batchKey), options);
    }
    function deletePackSchedule(key) {
        return core.ajaxDelete(psUrl(key));
    }
    function getPackScheduleDetails(psKey) {
        return core.ajax(psUrl(psKey));
    }
    function updatePackSchedule(psKey, data) {
        return core.ajaxPut(psUrl(psKey), data);
    }
    //#endregion
});