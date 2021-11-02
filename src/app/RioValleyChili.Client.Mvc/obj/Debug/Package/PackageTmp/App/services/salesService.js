define(['services/serviceCore', 'services/directoryService', 'jquery'], function (core, directoryService, $) {
    var service = {
        getContractSummariesDataPager: getContractSummariesDataPager,
        getContractDetails: getContractDetails,
        getSalesOrders: getSalesOrders,
        getShipmentMethods: getShipmentMethods,
        getSalesOrdersDataPager: getSalesOrdersDataPager,
        getSalesOrder: getSalesOrder,
        createSalesOrder: createSalesOrder,
        updateSalesOrder: updateSalesOrder,
        deleteSalesOrder: deleteSalesOrder,
        postSalesInvoice: postSalesInvoice,
        postSalesOrder: postSalesOrder,
        getShipmentSummaryForContract: getShipmentSummaryForContract,
        getPaymentTermOptions: getPaymentTermOptions,
        getWarehouses: getWarehouses,
        createContract: createContract,
        deleteContract: deleteContract,
        updateContract: updateContract,
        getContractsForCustomer: getContractsForCustomer,
        getQuotesDataPager: getQuotesDataPager,
        getQuoteDetails: getQuoteDetails,
        createQuote: createQuote,
        updateQuote: updateQuote
    };

    var BASE_URLS = {
      QUOTES: '/api/quotes/'
    };

    return $.extend({}, service, directoryService);

    function getContractSummariesDataPager(args) {
        var options = args || {};
        return core.pagedDataHelper.init({
            urlBase: "/api/contracts",
            pageSize: options.pageSize || 100,
            parameters: options.parameters,
            onNewPageSet: options.onNewPageSet,
            resultCounter: function(response) {
                return response.Data.length;
            }
        });
    }
    function getSalesOrdersDataPager(options) {
        var opts = options || {};
        return core.pagedDataHelper.init({
            urlBase: "/api/salesorders",
            pageSize: opts.pageSize || 100,
            parameters: opts.parameters,
            onNewPageSet: opts.onNewPageSet,
            resultCounter: function(response) {
                return response.length;
            }
        });
    }
    function getContractsForCustomer(key, maxRecords) {
        return core.ajax("/api/customers/" + key + "/contracts?take=" + maxRecords);
    }
    function getContractDetails(key) {
        return core.ajax(buildContractRoute(key));
    }
    function getShipmentSummaryForContract(contractKey) {
        return core.ajax(buildContractRoute(contractKey) + '/shipments');
    }
    function getShipmentMethods() {
      return core.ajax('/api/shipmentMethods');
    }
    function getPaymentTermOptions() {
        return core.ajax("/api/paymentterms");
    }
    function getWarehouses() {
        return core.ajax("/api/facilities");
    }
    function getSalesOrders(filters) {
      if (filters) {
        return core.ajax("/api/salesorders/?" + filters);
      } else {
        return core.ajax("/api/salesorders/");
      }
    }
    function getSalesOrder(key) {
        return core.ajax("/api/salesorders/" + key);
    }
    function createSalesOrder( data ) {
      return core.ajaxPost('/api/salesorders/', data);
    }
    function updateSalesOrder( key, data ) {
      return core.ajaxPut('/api/salesorders/' + key, data);
    }
    function deleteSalesOrder( key ) {
      return core.ajaxDelete('/api/salesorders/' + key);
    }
    function postSalesOrder( key, data ) {
      return core.ajaxPost('/api/InventoryShipmentOrders/' + key + '/PostAndClose', data);
    }
    function postSalesInvoice( key ) {
      return core.ajaxPost('/api/salesorders/' + key + '/postinvoice');
    }
    function createContract(values) {
        return core.ajaxPost(buildContractRoute(), values);
    }
    function deleteContract(key) {
        return core.ajaxDelete(buildContractRoute(key));
    }
    function updateContract(contractKey, values) {
        return core.ajaxPut(buildContractRoute(contractKey), values);
    }

    function buildContractRoute(key) {
        return "/api/contracts/" + (key || "");
    }

    function getQuotesDataPager( options ) {
      var opts = options || {};

      return core.pagedDataHelper.init({
        urlBase: BASE_URLS.QUOTES,
        pageSize: opts.pageSize || 20,
        parameters: opts.parameters,
        onNewPageSet: opts.onNewPageSet,
        resultCounter: function(response) {
          return response.length;
        }
      });
    }

    function getQuoteDetails( quoteKey ) {
      if ( quoteKey == null ) { throw new Error('Quote Details fetching requires a key'); }

      return core.ajax( BASE_URLS.QUOTES + quoteKey );
    }

    function createQuote( quoteData ) {
      return core.ajaxPost( BASE_URLS.QUOTES, quoteData );
    }

    function updateQuote( quoteKey, quoteData ) {
      return core.ajaxPut( BASE_URLS.QUOTES + quoteKey, quoteData );
    }
});
