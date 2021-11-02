var LotSummary = require('App/models/LotSummary');
var lotService = require('App/services/lotService');
var LotAttribute = require('App/models/LotAttribute');

// TODO NJH: Float table header

/**
  * @param {Object} selected - Observable, Contains selected item from table
  * @param {Object[]} filters - Input filters from lot-filters component
  * @param {Object[]} attributeList - Observable, exports current attribute names
  * @param {Object} exports - Observable, container for exposed methods/props
  * @param {Number} pageSize - Determines page size for paged data. Defaults to 100.
  * @param {Function} onNewPageSet - optional, callback function to be executed when the cursor's data page is reset.
  */
function LotSummaryTableViewModel( params ) {
  if ( !(this instanceof LotSummaryTableViewModel) ) { return new LotSummaryTableViewModel( params ); }

  var self = this;

  // Data
  this.allDataLoaded = ko.observable( false );

  var selected = ko.isObservable( params.selected ) ? params.selected : ko.observable( null );

  this.summaries = ko.observableArray( [] );
  this.attributeNames = ko.observableArray( [] );

  var position = {
    index: ko.pureComputed(function() {
      var _selected = selected();
      var results = self.summaries();
      var summaryItemIndex = results.indexOf( _selected );

      if ( summaryItemIndex > -1 ) {
        return summaryItemIndex;
      } else {
        if ( _selected ) {
          var matchedSummary = ko.utils.arrayFirst( results, function( summary ) {
            return summary.LotKey === _selected.LotKey;
          } );
          return matchedSummary ? results.indexOf( matchedSummary ) : -1;
        } else {
          return -1;
        }
      }
    }),
    maxIndex: ko.pureComputed(function() {
      return self.summaries().length - 1;
    })
  };

  // Support for old lot scripts
  this.AttributeNames = this.attributeNames;

  if ( params.attributeList ) {
    var updateAttrList = this.attributeNames.subscribe( function( data ) {
      params.attributeList( data );
    });
  }

  var lotPager = lotService.buildLotPager({
    pageSize: ko.unwrap(params.pageSize) || 100,
    parameters: ko.unwrap( params.filters ),
    resultCounter: function( data ) {
      var summary = data[0].LotSummaries;

      return data[0].LotSummaries.length;
    },
    onNewPageSet: function() {
      self.summaries([]);
      typeof params.onNewPageSet === "function" && params.onNewPageSet();
    }
  });

  // Behaviors
  var getLotsCommand = ko.asyncCommand({
    execute: function ( complete ) {
      var getPage = getNextPage().then(
      function( data, textStatus, jqXHR ) {
        return data;
      }).always( complete );

      return getPage;
    },
    canExecute: function ( isExecuting ) {
      return !isExecuting;
    }
  });

  var searchLotsCommand = ko.asyncCommand({
    execute: function ( complete ) {
      resetPage();

      var getPage = getLotsCommand.execute().then(
      function( data, textStatus, jqXHR ) {
        var filters = ko.unwrap( params.filters );
        var lotKey = filters && filters.startingLotKey();
        var lot = ko.utils.arrayFirst( self.summaries(), function( summary ) {
          if ( summary.LotKey === lotKey ) {
            selected( summary );
          }
        });

        return data;
      }).always( complete );

      return getPage;
    },
    canExecute: function ( isExecuting ) {
      return !isExecuting && getLotsCommand.canExecute();
    }
  });

  function checkOutOfRange( key, value ) {
    var defect = this.Defect;

    if ( !defect ) {
      return 0;
    } else if ( value < defect.AttributeDefect.OriginalMinLimit ) {
      return -1;
    } else if ( value > defect.AttributeDefect.OriginalMaxLimit ) {
      return 1;
    }
  }

  function sortDefects( defects ) {
    return defects.sort(function( a, b ) {
      if ( a.LotDefectKey > b.LotDefectKey ) {
        return 1;
      } else if ( a.LotDefectKey < b.LotDefectKey ) {
        return -1;
      }
      return 0;
    });
  }

  function mapLotSummary( input ) {
    var mappedSummary = new LotSummary( input, checkOutOfRange );

    sortDefects( mappedSummary.Defects );

    var mappedAttributeList = [];

    ko.utils.arrayForEach( self.attributeNames(), function( attrName ) {
      var matchedAttr = ko.utils.arrayFirst( mappedSummary.Attributes, function( attr ) {
        return attr.Key === attrName.Key;
      } );

      if ( matchedAttr ) {
        mappedAttributeList.push( matchedAttr );
      } else {
        var emptyAttr = new LotAttribute({
          Key: attrName.Key
        });

        mappedAttributeList.push( emptyAttr );
      }
    } );

    mappedSummary.Attributes = mappedAttributeList;

    return mappedSummary;
  }

  function getNextPage() {
    var loadNext = lotPager.GetNextPage().then(
    function( data, textStatus, jqXHR ) {
        self.allDataLoaded( lotPager.allDataLoaded() );

        if ( data.splice ) {
          data = data[0];
        }

        if ( !self.attributeNames().length ) {
          self.attributeNames( lotService.sortAttributes( data.AttributeNamesByProductType.Chile ) );
        }

        var mappedLotSummaries = ko.utils.arrayMap( data.LotSummaries, mapLotSummary );
        ko.utils.arrayPushAll( self.summaries(), mappedLotSummaries );
        self.summaries.notifySubscribers( self.summaries() );

        return data;
    });

    return loadNext;
  }

  function selectLotKey( lotKey ) {
    var summary = ko.utils.arrayFirst( self.summaries(), function( summaryData ) {
      return summaryData.LotKey === lotKey;
    });

    if ( summary ) {
      selected( summary );

      return $.when( summary );
    } else {
      return $.Deferred().reject();
    }
  }

  function getLotIndex( lot ) {
    var lotData = ko.unwrap( lot );
    var lotKey = lotData && lotData.LotKey;
    var results = self.summaries();

    if ( lotKey ) {
      var currentLot = ko.utils.arrayFirst( results, function( lot ) {
        return lot.LotKey === lotKey;
      });
      var index = results.indexOf( currentLot );

      return index;
    } else {
      return null;
    }
  }

  function selectPrevLot() {
    var results = self.summaries();
    var index = position.index();

    if ( index > 0 ) {
      return $.when( results[ index - 1 ] );
    }

    return $.Deferred().reject();
  }

  function selectNextLot() {
    var results = self.summaries();
    var index = position.index();
    var indexMax = position.maxIndex();

    if ( index === indexMax ) {
      var fetchMoreResults = getNextPage().then(
      function( data, textStatus, jqXHR ) {
        return self.summaries()[ index + 1 ];
      });

      return fetchMoreResults;
    } else if ( index >= 0 && index < indexMax ) {
      return $.when( results[ index + 1 ] );
    }
  }

  function resetPage() {
    lotPager.resetCursor();
  }

  function updateLot( lotData ) {
    var _selected = selected();
    var lotIndex = getLotIndex( lotData );
    var lots = self.summaries();
    var mappedLot = mapLotSummary( lotData );

    if ( lotIndex != null && lotIndex >= 0 ) {
      self.summaries.splice( lotIndex, 1, mappedLot );
      self.summaries.notifySubscribers();
    }

    if ( lotData.LotKey === (_selected && _selected.LotKey) )  {
      selected( mappedLot );
    }
  }

  function updateLotData( lotKey, data ) {
    var results = self.summaries();
    var result = ko.utils.arrayFirst( results, function( lot ) {
      return lot.LotKey === lotKey;
    });

    if ( result ) {
      ko.utils.arrayForEach( Object.keys( data ), function( key ) {
        if ( ko.isObservable( result[key] ) ) {
          result[key]( data[key] );
        }
      });
    }
  }

  this.selectEntry = function( data, element ) {
    var $element = $( element.target ).closest('tr');

    if ( $element.length ) {
      var context = ko.contextFor( $element[0] ).$data;

      if ( context.hasOwnProperty( 'LotKey' ) ) {
        selected( context );
      }
    }
  };

  params.exports({
    getLotsCommand: getLotsCommand,
    searchLotsCommand: searchLotsCommand,
    position: position,
    lotSummaries: self.summaries,
    selectPrevLot: selectPrevLot,
    selectNextLot: selectNextLot,
    selectLotKey: selectLotKey,
    resetPage: resetPage,
    updateLot: updateLot,
    updateLotData: updateLotData,
  });
}

module.exports = {
  template: require('./lot-summary-table.html') + require('App/templates/lotScriptTemplates.html'),
  viewModel: LotSummaryTableViewModel
};
