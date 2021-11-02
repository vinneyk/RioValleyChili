define(['services/serviceCore', 'app'], function(core, app) {
    function getProductsByInventoryType(inventoryType, options) {
      options = options || {};
      options = $.extend({
        filterProductsWithInventory: false,
        includeInactive: false
      }, options);

      var url = ['/api/products/', inventoryType].join('');

      var qs = [];
      if (options.lotType != null) {
        qs.push('lotType=' + options.lotType);
      }
      if (options.filterProductsWithInventory) {
        qs.push('filterProductsWithInventory=true');
      }
      if (options.includeInactive) {
        qs.push('includeInactive=true');
      }

      if (qs.length > 0) {
        url += '?' + qs.join('&');
      }

      return core.ajax(url, options);
    }

    function getProductsByLotType(lotType, options) {
      var inventoryType = app.lists.lotTypes.findByKey(lotType).inventoryType.key;
      options = options || {};
      options.lotType = lotType;
      return getProductsByInventoryType(inventoryType, options);
    }

    return {
        getChileProducts: function (chileState) {
            if (chileState && typeof chileState === "object") chileState = chileState.key;
            return core.ajax(core.buildUrl(buildChileProductsUrl, chileState));
        },
        getPackagingProducts: function (options) { return getProductsByLotType(app.lists.lotTypes.Packaging.key, options); },
        getProductTypeAttributes: function () {
            return core.ajax("/api/productTypeAttributes");
        },
        getCustomerProducts: function( customerKey ) {
          return core.ajax( '/api/customers/' + customerKey + '/productspecs' );
        },
        getCustomerProductDetails: function( customerKey, productKey ) {
          return core.ajax( '/api/customers/' + customerKey + '/productSpecs/' + productKey );
        },
        createCustomerProductOverride: function( customerKey, productKey, overrideData ) {
          return core.ajaxPost( '/api/customers/' + customerKey + '/productSpecs/' + productKey, overrideData );
        },
        deleteCustomerProductOverride: function( customerKey, productKey ) {
          return core.ajaxDelete( '/api/customers/' + customerKey + '/productSpecs/' + productKey );
        },
        getProductDetails: core.setupFn(getProductDetails, buildProductUrl),
        getProductsByLotType: getProductsByLotType,
        getProductsByInventoryType: getProductsByInventoryType,
        getChileVarieties: function() {
          return core.ajax('/api/chilevarities');
        },
        getChileTypes: function () {
            return core.ajax("/api/chileTypes");
        },
        getAdditiveTypes: function () {
            return core.ajax("/api/additiveTypes");
        },
        getProductionLocations: function() {
          return core.ajax('/api/productionlines');
        },
        createProduct: function( data ) {
          return core.ajaxPost( '/api/products', data );
        },
        updateProduct: function( productCode, data ) {
          return core.ajaxPut( '/api/products/' + productCode, data );
        },
        setProductIngredients: function( productKey, data ) {
          return core.ajaxPost( '/api/products/' + productKey + '/ingredients', data );
        },
        setProductAttributes: function( productKey, data ) {
          return core.ajaxPost( '/api/products/' + productKey + '/specs', data );
        },
        getProductionSchedulesDataPager: function( options ) {
          options = options || {};

          return core.pagedDataHelper.init({
              urlBase: options.baseUrl || "/api/productionschedules",
              pageSize: options.pageSize || 50,
              parameters: options.parameters,
              onNewPageSet: options.onNewPageSet,
              onEndOfResults: options.onEndOfResults
          });
        },
        getProductionScheduleDetails: function( key ) {
          return core.ajax( '/api/productionschedules/' + key );
        },
        createProductionSchedule: function( data ) {
          return core.ajaxPost( '/api/productionschedules/', data );
        },
        updateProductionSchedule: function( key, data ) {
          return core.ajaxPut( '/api/productionschedules/' + key, data );
        },
        deleteProductionSchedule: function( key ) {
          return core.ajaxDelete( '/api/productionschedules/' + key );
        }
    };

    //#region function delegates
    function getProductDetails(lotType, key) {
        return core.ajax(buildProductUrl(lotType, key));
    }
    //#endregion

    function buildProductUrl(lotType, key) {
        key = key || '';
        return '/api/products/' + lotType + (key ? '/' + key : '');
    }
    function buildChileProductsUrl(chileState) {
        return '/api/chileproducts?chileState=' + chileState;
    }
});
