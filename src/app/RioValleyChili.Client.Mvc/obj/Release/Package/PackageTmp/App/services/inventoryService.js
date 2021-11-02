define(['services/serviceCore', 'app'], function (core, app) {
    var warehouseLocationsService = require('services/warehouseLocationsService');

    var buildUrl = core.buildUrl,
        ajax = core.ajax,
        pagedDataHelper = core.pagedDataHelper;

    return {
        getInventoryPager: getInventoryPager,
        getInventoryPagerWithTotals: getInventoryPagerWithTotals,
        getPickableInventoryPager: getPickableInventoryPager,
        getPickableInventory: getPickableInventory,
        getInventory: getInventory,
        getInventoryByLot: getInventoryByLot,
        getInventoryByLotAndWarehouse: getInventoryByLotAndWarehouse,
        getInventoryByTote: getInventoryByTote,
        getChileProductsDataPager: getChileProductsDataPager,
        getChileMaterials: getChileMaterials,
        createChileProductReceivedRecord: createChileProductReceivedRecord,
        updateChileProductReceivedRecord: updateChileProductReceivedRecord,
        getDehydratedMaterials: getDehydratedMaterials,
        getDehydratedMaterialsDataPager: getDehydratedMaterialsDataPager,
        getDehydratedMaterialsReceived: getDehydratedMaterialsReceived,
        getDehydrators: getDehydrators,
        getMillAndWetdownPager: getMillAndWetdownPager,
        getMillAndWetdownDetails: getMillAndWetdownDetails,
        createMillAndWetdownEntry: createMillAndWetdownEntry,
        updateMillAndWetdownEntry: updateMillAndWetdownEntry,
        deleteMillAndWetdownEntry: deleteMillAndWetdownEntry,
        getRinconWarehouseLocations: warehouseLocationsService.getRinconWarehouseLocations,
        getWarehouseLocations: warehouseLocationsService.getWarehouseLocations,
        receiveInventory: receiveInventory,
        saveDehydratedMaterials: saveDehydratedMaterials,
        updateDehydratedMaterials: updateDehydratedMaterials,
        savePickedInventory: savePickedInventoryPublic,
    };

    //#region exports
    function getInventoryPager(options) {
        options = options || {};

        return pagedDataHelper.init({
            urlBase: options.baseUrl || "/api/inventory",
            pageSize: options.pageSize || 50,
            parameters: options.parameters,
            onNewPageSet: options.onNewPageSet,
            onEndOfResults: options.onEndOfResults
        });
    }
    function getInventoryPagerWithTotals(options) {
      options = options || {};

      return pagedDataHelper.init({
        urlBase: options.baseUrl || "/api/inventorytotals",
        pageSize: options.pageSize || 50,
        parameters: options.parameters,
        onNewPageSet: options.onNewPageSet,
        onEndOfResults: options.onEndOfResults,
        resultCounter: function (data) {
          data = data || {};
          data.Items = data.Items || [];
          return data.Items.length || 0;
        }
      });
    }
    function getPickableInventoryPager(pickingContext, contextKey, options) {
        options = options || {};
        if (!options.pageSize) options.pageSize = 100;
        options.urlBase = buildInventoryPickingUrl(pickingContext, contextKey);
        return pagedDataHelper.init(options);
    }
    function getPickableInventory(pickingContext, contextKey, params) {
        var qs = $.param(params);
        return core.ajax(buildInventoryPickingUrl(pickingContext, contextKey) + '?' + qs);
    }
    function savePickedInventoryPublic(pickingContext, contextKey, values) {
        return core.ajaxPost(buildInventoryPickingUrl(pickingContext, contextKey), values);
    }
    function buildInventoryPickingUrl(pickingContext, contextKey) {
        return '/api/inventory/pick-' + pickingContext.value.toLowerCase() + '/' + contextKey;
    }
    function getInventory(/* productType, lotType, productSubType, productKey, warehouseKey */) {
        var args = [];
        for (var a in arguments) {
            args.push(arguments[a]);
        }
        var options = args.pop();
        return ajax(buildUrl(buildInventoryUrl, args), options);
    }
    function getInventoryByLot(lotNumber, options) {
        return ajax(buildUrl(buildInventoryUrl, lotNumber), options);
    }
    function getInventoryByLotAndWarehouse(lotNumber, warehouseKey, options) {
        return ajax(buildUrl(buildLotInventoryByWarehouseUrl, lotNumber, warehouseKey), options);
    }
    function getInventoryByTote(toteKey, options) {
        return ajax("/api/toteinventory/" + toteKey, options);
    }
    function getMillAndWetdownPager(options) {
      options = options || {};

      return pagedDataHelper.init({
        urlBase: options.baseUrl || "/api/millwetdown",
        pageSize: options.pageSize || 50,
        parameters: options.parameters,
        onNewPageSet: options.onNewPageSet,
      });
    }
    function getMillAndWetdownDetails(key) {
      return ajax('/api/millwetdown/' + (key || ''));
    }
    function createMillAndWetdownEntry(entryData) {
      return core.ajaxPost("/api/millwetdown/", entryData);
    }
    function updateMillAndWetdownEntry(lotNumber, entryData) {
      return core.ajaxPut("/api/millwetdown/" + lotNumber, entryData);
    }
    function deleteMillAndWetdownEntry(lotNumber, entryData) {
      return core.ajaxDelete( "/api/millwetdown/" + lotNumber );
    }
    function getDehydrators() {
      var dfd = $.Deferred();

      ajax('/api/companies').done(function(data, textStatus, jqXHR) {
        var dehydrators = ko.utils.arrayFilter(data, function(company) {
          if (company.CompanyTypes.indexOf(app.lists.companyTypes.Dehydrator.key) >= 0) {
            return true;
          }
        });

        dfd.resolve(dehydrators);
      })
      .fail(function(jqXHR, textStatus, errorThrown) {
      });

      return dfd;
    }
    function getChileProductsDataPager(options) {
        options = options || {};

        return pagedDataHelper.init({
            urlBase: options.baseUrl || "/api/chilereceived",
            pageSize: options.pageSize || 50,
            parameters: options.parameters,
            onNewPageSet: options.onNewPageSet,
        });
    }
    function getDehydratedMaterialsDataPager(options) {
        options = options || {};

        return pagedDataHelper.init({
            urlBase: options.baseUrl || "/api/dehydratedmaterialsreceived",
            pageSize: options.pageSize || 50,
            parameters: options.parameters,
            onNewPageSet: options.onNewPageSet,
        });
    }
    function getChileMaterials( lotKey ) {
      return core.ajax( '/api/chilereceived/' + lotKey );
    }
    function createChileProductReceivedRecord( productData ) {
      return core.ajaxPost( '/api/chilereceived', productData );
    }
    function updateChileProductReceivedRecord( productKey, productData ) {
      return core.ajaxPut( '/api/chilereceived/' + productKey, productData );
    }
    function getDehydratedMaterials(lot) {
      return ajax('/api/dehydratedmaterialsreceived/' + (lot || ''));
    }
    function getDehydratedMaterialsReceived() { /* optional parameters: startDate, endDate */
        var args = [];
        for (var a in arguments) {
            args.push(arguments[a]);
        }
        var options = args.pop();
        var url = "/api/dehydratedmaterialsreceived";
        if (args.length > 0) {
            url += buildQueryString(args[0]);
        }

        ajax(url, options);
    }
    function saveDehydratedMaterials(data) {
      return core.ajaxPost('/api/dehydratedmaterialsreceived', data);
    }
    function updateDehydratedMaterials(key, data) {
      return core.ajaxPut('/api/dehydratedmaterialsreceived/' + key, data);
    }
    function receiveInventory(values) {
        var url = buildLotInventoryUrl();
        return core.ajaxPost(url, values);
    }
    //#endregion exports

    function buildLotInventoryUrl(lotNumber) {
        return "/api/inventory/" + lotNumber;
    }
    function buildLotInventoryByWarehouseUrl(lotNumber, warehouseKey) {
        return buildLotInventoryUrl(lotNumber) + "?warehouseKey=" + warehouseKey;
    }
    function buildInventoryUrl(productType) {
        return "/api/inventory/" + (productType || 2);
    }
});
