function LotHistory( lotData, attrNames ) {
  // Lot info
  this.LotKey = lotData.LotKey;
  this.Product = lotData.Product;

  this.AttributeNames = attrNames || [];

  // Current lot attributes
  this.Employee = lotData.Employee || {};
  this.Timestamp = lotData.Timestamp || null;

  this.LoBac = lotData.LoBac;
  this.HoldType = lotData.HoldType;
  this.HoldDescription = lotData.HoldDescription;
  this.QualityStatus = lotData.QualityStatus;
  this.ProductionStatus = lotData.ProductionStatus;

  // Current attributes
  function mapAttrs( attrNames, attrData ) {
    var _attrVals = {};

    ko.utils.arrayForEach( attrData, function( attr ) {
      _attrVals[ attr.AttributeShortName ] = attr;
    } );

    var mappedAttrs = JSON.parse( ko.toJSON( attrNames ) );
    ko.utils.arrayForEach( mappedAttrs, function( attr ) {
      var _attrData = _attrVals[ attr.Key ] || {};

      attr.AttributeDate = _attrData.AttributeDate;
      attr.Computed = _attrData.Computed;
      attr.Value = _attrData.Value;
    });

    return mappedAttrs;
  }

  this.Attributes = mapAttrs( attrNames, lotData.Attributes );

  // Lot history items
  this.History = ko.utils.arrayMap( lotData.History, function( historyItem ) {
    historyItem.Attributes = mapAttrs( attrNames, historyItem.Attributes );

    return historyItem;
  }) || [];
  this.History.sort(function( a, b ) {
    var _a = a.Timestamp;
    var _b = b.Timestamp;
    if ( _a > _b ) {
      return -1;
    }
    if ( _a < _b ) {
      return 1;
    }

    return 0;
  });
}

/** Lot Attributes History vm
  * @param {Object} input - History data to display in UI
  * @param {Object[]} attrs - Expected attributes to display in history tables
  */
function LotHistoryDetailsVM( params ) {
  if ( !(this instanceof LotHistoryDetailsVM) ) { return new LotHistoryDetailsVM( params ); }

  var self = this;

  this.disposables = [];

  var input = ko.unwrap( params.input );

  this.lotHistory = ko.observable( mapLotHistory( input ) );

  function mapLotHistory( lotData ) {
    var attrNames = params.attrs && params.attrs[ lotData.Product.ProductType ];

    var lotHistory = new LotHistory( ko.unwrap( lotData ), attrNames );

    return lotHistory;
  }

  // Exports
  if ( params && params.exports ) {
    params.exports({
    });
  }

  return this;
}

ko.utils.extend(LotHistoryDetailsVM.prototype, {
    dispose: function() {
        ko.utils.arrayForEach(this.disposables, this.disposeOne);
        ko.utils.objectForEach(this, this.disposeOne);
    },

    // little helper that handles being given a value or prop + value
    disposeOne: function(propOrValue, value) {
        var disposable = value || propOrValue;

        if (disposable && typeof disposable.dispose === "function") {
            disposable.dispose();
        }
    }
});

module.exports = {
  viewModel: LotHistoryDetailsVM,
  template: require('./lot-history.html')
};
