var lotService = require('App/services/lotService');
var page = require('page');

require('App/koExtensions');

function LotHistoryViewModel (params) {
    if (!(this instanceof LotHistoryViewModel)) return new LotHistoryViewModel();

    var self = this;

    this.loadingMessage = ko.observable('');

    // Data
    this.inputKey = ko.observable().extend({ lotKey: true });
    this.targetProduct = ko.observable();

    this.inputMaterialsData = {
      input: ko.observableArray( [] )
    };

    this.transactionsData = {
      input: ko.observableArray( [] )
    };

    // Behaviors
    this.goToLotNumber = function () {
        if (self.inputKey()) {
            page('/' + self.inputKey());
        } else {
            page('/');
        }
    };

    function resetData() {
      self.targetProduct( null );
      self.inputMaterialsData.input( [] );
      self.transactionsData.input( [] );
    }

    this.searchLotKeyCommand = ko.asyncCommand({
      execute: function( lotKey, complete ) {
        self.loadingMessage( 'Loading Lot #' + lotKey );
        resetData();

        var getInputMaterials = lotService.getInputMaterialsDetails( lotKey ).then(
        function( data, textStatus, jqXHR ) {
          self.inputMaterialsData.input( data.InputItems );

          return data;
        },
        function( jqXHR, textStatus, errorThrown ) {
          showUserMessage('Failed to load input materials data', {
            description: message
          });
        });

        var getTransactions = lotService.getTransactionsDetails( lotKey ).then(
        function( data, textStatus, jqXHR ) {
          self.transactionsData.input( data );

          return data;
        },
        function( jqXHR, textStatus, errorThrown ) {
          showUserMessage('Failed to load transactions data', {
            description: message
          });
        });

        var checkData = $.when( getInputMaterials, getTransactions ).then(
        function( inputMats, transactions ) {
          var _targetProduct = inputMats.Product;

          if ( !_targetProduct ) {
            showUserMessage( 'Could not load data', {
              description: 'The lot <b>' + lotKey + '</b> does not exist.'
            });
            page.redirect( '/' );
          }

          self.targetProduct( _targetProduct && _targetProduct.ProductCodeAndName );
        },
        function( jqXHR, textStatus, errorThrown ) {
          showUserMessage( 'Could not load data' );
          page( '/' );
        }).always( complete );
      }
    });

    this.searchLotNumber = function ( key ) {
        var targetLotKey = key || self.inputKey();

        self.searchLotKeyCommand.execute( targetLotKey );
    };

    // Page routing
    page.base( '/Warehouse/LotHistory' );

    // Get lot details if key is provided
    this.currentMovementKey = ko.observable( null );
    function navigateToLot( ctx, next ) {
        var lotKey = ctx.params.lotKey;

        if ( lotKey && self.currentMovementKey() !== lotKey ) {
          self.currentMovementKey( lotKey );
          self.inputKey( lotKey );
          self.searchLotNumber( lotKey );
          document.title = 'Inventory Transaction History - #' + lotKey;
        } else if ( self.currentMovementKey() === lotKey ) {
          return;
        } else {
          next();
        }
    }
    page( '/:lotKey', navigateToLot );

    // Else reset UI
    function resetView( ctx, next ) {
      self.currentMovementKey( null );
      resetData();
      document.title = "Inventory Transactions History";
    }
    page( resetView );

    page();

    // Exports
    return this;
}

ko.components.register('loading-screen', require('App/components/common/loading-screen/loading-screen'));
ko.components.register('lot-inventory-inputs', require('App/components/inventory/lot-inventory-inputs/lot-inventory-inputs'));
ko.components.register('lot-inventory-transactions', require('App/components/inventory/lot-inventory-transactions/lot-inventory-transactions'));

var vm = new LotHistoryViewModel();

ko.applyBindings(vm);

