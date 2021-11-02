define(['services/serviceCore', 'rvc'], function (core, rvc) {
  var attributeOrder = {
    'AstaC': 0,
    'Asta': 1,
    'H2O': 2,
    'Scan': 3,
    'AB': 4,
    'Gran': 5,
    'Scov': 6,
    'TPC': 7,
    'Yeast': 8,
    'Mold': 9,
    'ColiF': 10,
    'EColi': 11,
    'Sal': 12,
    'Rod Hrs': 13,
    'InsP': 14,
    'Ash': 15,
    'AIA': 16,
    'Ethox': 17,
    'BI': 18,
    'Lead': 19,
    'AToxin': 20,
    'Gluten': 21
  };

  function sortAttributesLists(values) {
    var data = values || {};
    rvc.helpers.forEachInventoryType(sortAttributesForType);
    return data;

    function sortAttributesForType(type) {
      var attrs = data[type.key] || [];
      if (type.key === rvc.lists.inventoryTypes.Chile.key) {
        attrs.push({ Key: 'AstaC', Value: 'Asta Calc' });
      }

      var attrOrder = attributeOrder || {};
      var unorderedIndex = Object.keys(attrOrder).length + 1;
      ko.utils.arrayMap(attrs, function (attr) {
        var order = attrOrder[attr.Key];
        var index = order === undefined ? unorderedIndex++ : order;
        attr.__index = index;
      });

      data[type.key] = attrs.sort(function (a, b) {
        return a.__index === b.__index ?
          0 :
          a.__index < b.__index ?
            -1 :
            1;
      });
    }
  }

  function sortAttributes( attrs ) {
    var sortedAttributes = [];
    var attrOrder = attributeOrder || {};
    var unorderedIndex = Object.keys(attrOrder).length + 1;

    ko.utils.arrayMap( attrs, function (attr) {
      var order = attrOrder[attr.Key];
      var index = order === undefined ?
        unorderedIndex++ :
        order;

      attr.__index = index;
    } );

    sortedAttributes = attrs.sort(function (a, b) {
      return a.__index === b.__index ?
        0 :
        a.__index < b.__index ?
          -1 :
          1;
    });

    return sortedAttributes;
  }

  return {
    buildLotPager: function( options ) {
      options = options || {};
      return core.pagedDataHelper.init({
        urlBase: "/api/lots",
        pageSize: options.pageSize || 50,
        parameters: options.parameters,
        resultCounter: function (data) {
          return data.LotSummaries.length;
        },
        onNewPageSet: options.onNewPageSet
      });
    },
    compositeLots: function( data ) {
      return core.ajaxPut( '/api/lots/addattributes/', data );
    },
    sortAttributes: sortAttributes,
    buildLotUrl: function (key) {
      return ["/api/Lots/", key || ''].join('');
    },
    getAttributeNames: function () {
      var $dfd = $.Deferred();

      core.ajax('/api/attributeNames')
      .done(function (data) {
        try {
          $dfd.resolve(sortAttributesLists(data));
        } catch (e) {
          $dfd.reject();
        }
      })
      .fail($dfd.reject);

      // support compatability with the core.ajax return object
      $dfd.error = $dfd.fail;

      return $dfd;
    },
    getLotData: function( lotKey ) {
      return core.ajax( ''.concat( '/api/lots/' + lotKey.replace(/ /g, '') ) );
    },
    getLotHistory: function( lotKey ) {
      return core.ajax( '/api/lots/' + lotKey + '/history' );
    },
    getIngredientsByProductType: function () {
      return core.ajax('/api/ingredients');
    },
    getProductsByLotType: function (lotType, options) {
      var inventoryType = rvc.lists.lotTypes.findByKey(lotType).inventoryType.key;
      var url = ['/api/products/', inventoryType, '?lotType=', lotType].join('');

      if (options && options.filterProductsWithInventory) {
        url = url.concat("&filterProductsWithInventory=true");
      }

      return core.ajax(url, options);
    },
    getLotsByKey: function( lotKey ) {
      return core.ajax( ''.concat( '/api/lots?startingLotKey=', lotKey ) );
    },
    setLotStatus: function( lotKey, status ) {
      return core.ajaxPut( ''.concat( '/api/lots/', lotKey, '/qualityStatus' ), status );
    },
    removeLotHold: function (lotKey, optionsCallback) {
      return core.ajaxPut("/api/lots/" + lotKey + "/holds", null, optionsCallback);
    },
    setLotHold: function (lotKey, data, optionsCallback) {
      return core.ajaxPut("/api/lots/" + lotKey + "/holds", data, optionsCallback);
    },
    getInputMaterialsDetails: function (key) {
      if (!key) { return; }
      return core.ajax(this.buildLotUrl(key) + '/input');
    },
    getTransactionsDetails: function (key) {
      if (!key) { return; }
      return core.ajax(this.buildLotUrl(key) + '/inventory/transactions');
    },
    saveLabResult: function( lotKey, data ) {
      return core.ajaxPut( ('/api/lots/' + lotKey), data );
    },
    createAllowance: function( lotKey, type, key ) {
      return core.ajaxPut( '/api/lots/' + lotKey.replace( /\s+/g, '' ) + '/allowances/' + type + '/' + key );
    },
    deleteAllowance: function( lotKey, type, key ) {
      return core.ajaxDelete( '/api/lots/' + lotKey.replace( /\s+/g, '' ) + '/allowances/' + type + '/' + key );
    },
    getLotInputTrace: function( lotKey ) {
      if ( lotKey == null ) { throw new Error('Lot trace requires a key'); }

      return core.ajax( '/api/lots/' + lotKey + '/input-trace' );
    },
    getLotOutputTrace: function( lotKey ) {
      if ( lotKey == null ) { throw new Error('Lot trace requires a key'); }

      return core.ajax( '/api/lots/' + lotKey + '/output-trace' );
    },
  };
});
