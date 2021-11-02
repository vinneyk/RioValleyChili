ko.components.register('lab-result-summary-table', require('App/components/common/lot-summary-table/lot-summary-table'));
ko.components.register('lab-results-editor', require('App/components/quality-control/lab-results-editor/lab-results-editor'));
ko.components.register('lot-filter-controls', require('App/components/common/lot-filters/lot-filters'));
ko.components.register('loading-screen', require('App/components/common/loading-screen/loading-screen'));
ko.components.register('lot-attr-history', require('App/components/quality-control/lot-history/lot-history'));

var lotService = require('App/services/lotService');
var productsService = require('App/services/productsService');
var salesService = require('App/services/salesService');
var app = require('app');
var page = require('page');

require('App/helpers/koValidationHelpers');

ko.punches.interpolationMarkup.enable();

function LotRange() {
  this.startLot = ko.observable().extend({ lotKey: true });
  this.startLot.extend({
    validation: {
      message: 'A start lot is required',
      validator: function( val ) {
        var lot = this.startLot.formattedLot();
        return lot && lot.length >= 12;
      }.bind( this )
    }
  });
  this.startLotIndex = ko.pureComputed(function() {
    var _lot = '' + (this.startLot() || '');
    var _lotBase = _lot.substr( 10, 3 );

    return _lotBase;
  }, this);

  this.endLotIndex = ko.observable( '' );
  this.endLotIndex.extend({
    validation: {
      message: 'End Lot must be higher or equal to Start Lot',
      validator: function( val ) {
        return +val >= +this.startLotIndex();
      }.bind( this ),
      onlyIf: function() {
        var indexVal = this.endLotIndex();

        return indexVal !== '' && indexVal != null;
      }.bind( this )
    }
  });

  this.lotBase = ko.pureComputed(function() {
    var _lot = '' + (this.startLot() || '');
    var _lotBase = _lot.substr( 0, 9 );

    return _lotBase;
  }, this);

  this.lotPlaceholder = ko.pureComputed(function() {
    var lotBaseLength = this.lotBase().length;

    return '## ## ###'.substr( lotBaseLength );
  }, this);

  this.isRangeValid = ko.pureComputed(function() {
    var startIndex = this.startLotIndex();
    var endIndex = this.endLotIndex();

    return startIndex && endIndex ?
      this.startLot.isValid() && this.endLotIndex.isValid() :
      true;
  }, this);

  this.validation = ko.validatedObservable({
    startLot: this.startLot,
    endLot: this.endLotIndex,
    rangeValidation: this.isRangeValid,
  });
}

function CompositeAttribute( data, lastEnteredDate ) {
  this.checked = ko.observable( false );
  this.Key = data.Key;
  this.Value = ko.observable().extend({ min: 0 });
  this.AttributeDate = ko.observableDate().extend({
    required: {
      message: 'A date is required for attributes containing a value',
      onlyIf: function() {
        var val = this.Value();

        return val != null && val !== '';
      }.bind( this )
    }
  });

  this.Value.subscribe( function( val ) {
    if ( val ) {
      this.checked( true );

      var _lastDate = lastEnteredDate();
      if ( !this.AttributeDate() && _lastDate ) {
        this.AttributeDate( _lastDate );
      }
    }
  }, this );

  this.AttributeDate.subscribe( function( newDate ) {
    if ( newDate !== '' && newDate != null ) {
      lastEnteredDate( newDate );
    }
  });

  this.validation = ko.validatedObservable({
    value: this.Value,
    date: this.AttributeDate
  });
}

function CompositeEditor( attrs ) {
  this.lotRanges = ko.observableArray( [] );

  var _lastEnteredDate = ko.observable();
  var _mappedAttrs =  ko.utils.arrayMap( ko.unwrap( attrs ), function( attr ) {
    return new CompositeAttribute( attr, _lastEnteredDate );
  });
  this.attrData = ko.observableArray( _mappedAttrs );
  this.editableAttrs = this.attrData.filter( function( attr ) {
    return attr.Key !== 'AstaC';
  });

  this.selectAllAttrs = function() {
    ko.utils.arrayForEach( this.attrData(), function( attr ) {
      var setAsChecked = attr.Key !== 'AstaC' && attr.checked( true );
    });
  }.bind( this );

  this.deselectAllAttrs = function() {
    ko.utils.arrayForEach( this.attrData(), function( attr ) {
      attr.checked( false );
    });
  }.bind( this );

  var _lotRanges = this.lotRanges;
  this.addLotRange = ko.command({
    execute: function() {
      var lotRange = new LotRange();

      _lotRanges.push( lotRange );
    }
  });

  this.removeLotRange = ko.command({
    execute: function( data ) {
      var index = _lotRanges().indexOf( data );

      _lotRanges.splice( index, 1 );
    },
    canExecute: function() {
      return _lotRanges().length > 1;
    }
  });

  var _attrData = this.attrData;
  this.errors = ko.pureComputed(function() {
    var errors = [];

    var lotErrorCheck = ko.utils.arrayForEach( _lotRanges(), function( lotRange ) {
      if ( !lotRange.validation.isValid() ) {
        lotRange.validation.errors.showAllMessages();
        errors.push( lotRange.validation.errors );
      }
    });

    var attrErrorCheck = ko.utils.arrayForEach( _attrData(), function( attr ) {
      if ( !attr.validation.isValid() ) {
        attr.validation.errors.showAllMessages();
        errors.push( attr.validation.errors );
      }
    });

    return errors;
  }, this);

  this.isValid = ko.pureComputed(function() {
    return this.errors().length === 0;
  }, this);

  this.reset = function() {
    this.lotRanges([ new LotRange() ]);
    ko.utils.arrayForEach( this.attrData(), function( attr ) {
      attr.checked( false );
      attr.Value( '' );
      attr.AttributeDate( '' );
    } );
  }.bind( this );
}

/**
  Lab Results
  */
function LabResultsVM() {
  if ( !(this instanceof LabResultsVM) ) { return new LabResultsVM( params ); }

  var self = this, isLoading = false;

  // Data
  this.isInit = ko.observable( false );
  this.isWorking = ko.observable( null );
  this.isInitialLoad = true;
  this.isNavigating = false;
  this.customers = ko.observableArray( [] );
  this.isShowingCopyLot = ko.observable( false );

  this.copyLotKey = ko.observable( '' ).extend({ lotKey: true });

  this.hasMoreResults = ko.observable( true );

  this.LabResult = ko.observable( null );
  this.attrList = {};
  this.attributeNames = ko.observableArray( [] );
  this.lotKeyToSearch = ko.observable( '' ).extend({ lotKey: true });
  this.isLoadingResults = ko.observable(false);
  this.isLoadingExtendedResults = ko.observable(false);
  this.pagesLoaded = ko.observable(0);

  this.filtersData = {
    filters: ko.observable( null )
  };

  this.compositeData = ko.observable(  );

  this.saveComposite = ko.asyncCommand({
    execute: function( complete ) {
      var compositeData = self.compositeData();

      // Check if Valid
      if ( !compositeData.isValid() ) {
        showUserMessage( 'Save composite failed', {
          description: 'Please ensure all required fields have been entered correctly'
        });

        complete();
        return;
      }

      // Compile Data
      var lots = [];

      // Generate lot numbers from each given range
      ko.utils.arrayForEach( compositeData.lotRanges(), function( lotRange ) {
        compileLotRanges( lots, lotRange.lotBase(), lotRange.startLotIndex(), lotRange.endLotIndex() );
      });

      function compileLotRanges( lots, lotBase, lotStart, lotEnd ) {
        var i;
        var _lotBase = lotBase.replace( /\s+/g, '' );
        var lotKey = '';
        var max = +lotEnd || +lotStart;
        for ( i = +lotStart; i <= max; i += 1 ) {
          lotKey = ("" + i).length > 1 ?
            _lotBase + i :
            _lotBase + '0' + i;

          if ( lots.indexOf( lotKey ) === -1 ) {
            lots.push( lotKey );
          }
        }
      }

      var attrs = ko.utils.arrayFilter( compositeData.attrData(), function( attr ) {
        return attr.checked();
      });
      var mappedAttrs = ko.utils.arrayMap( attrs, function( attr ) {
        var data = {
          AttributeKey: attr.Key,
          AttributeInfo: attr.Value() != null && attr.AttributeDate() != null ?
            {
              Value: attr.Value,
              Date: attr.AttributeDate
            } :
            null
        };

        return data;
      });

      // Save data to api
      var saveCompositeData = lotService.compositeLots({
        LotKeys: lots,
        Attributes: ko.toJS( mappedAttrs )
      }).then(
      function( data, textStatus, jqXHR ) {
        // Display sucess message
        showUserMessage( 'Lab results composite successful', {
          description: ''
        });

        // Close Modal
        self.hideCompositeModal.execute();

        // Load details page for first lot
        page( '/' + lots[0] );

        // Start search from first composite lot to populate summary table
        self.lotKeyToSearch( lots[0] );
        self.searchKey.execute();
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Lab results composite failed', {
          description: errorThrown
        });
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  this.summaryData = {
    selected: ko.observable( null ),
    filters: self.filtersData.filters,
    attrs: ko.observableArray( [] ),
    exports: ko.observable(null),
    pageSize: 100,
    onNewPageSet: function() {
      self.pagesLoaded(0);
    }
  };

  this.editorData = {
    input: {
      setStatus: setLotStatus,
      setQualityHold: setQualityHold,
      createAllowance: createAllowance,
      deleteAllowance: deleteAllowance,
      customers: self.customers
    },
    data: this.LabResult,
    exports: ko.observable(  ),
    isInit: ko.observable( false )
  };

  this.nav = {
    position: ko.pureComputed(function() {
      var summaryComponent = self.summaryData.exports();

      return summaryComponent && summaryComponent.position;
    }),
    isPrevEnabled: ko.pureComputed(function() {
      var posData = self.nav.position();

      return posData && posData.index() > 0 && !self.isWorking();
    }),
    isNextEnabled: ko.pureComputed(function() {
      var posData = self.nav.position();
      var posIndex = posData && posData.index();

      return (self.hasMoreResults() ||
                posData &&
                posIndex < posData.maxIndex()) &&
              posIndex > -1 &&
              !self.isWorking();
    }),
  };

  this.isDirty = ko.pureComputed(function() {
    var editor = self.editorData.exports();

    return editor && editor.isDirty();
  });

  this.lotKeyOnly = ko.pureComputed(function() {
    var isShowingDetails = self.LabResult();
    var isSearchingKey = null;

    if ( isShowingDetails ) {
      return true;
    } else if ( isSearchingKey ) {
      return 'disable';
    } else {
      return false;
    }
  });

  // Behaviors
  this.toggleLotCopy = ko.command({
    execute: function() {
      self.isShowingCopyLot( !self.isShowingCopyLot() );

      if ( !self.isShowingCopyLot() ) {
        self.copyLotKey( '' );
      }
    },
    canExecute: function() {
      return true;
    }
  });

  this.copyFromLot = ko.asyncCommand({
    execute: function( complete ) {
      var lotKey = self.copyLotKey();

      // Get lot data
      var getLotData = lotService.getLotData( lotKey ).then(
      function( data, textStatus, jqXHR ) {
        if ( data.LotSummary.AstaCalc != null ) {
          data.LotSummary.Attributes.push({
            Key: 'AstaC',
            Value: data.LotSummary.AstaCalc,
            Name: 'AstaCalc',
            MaxValue: null,
            MinValue: null
          });
        }

        return data;
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Failed to copy lab result data', {
          description: errorThrown
        });
      });

      var setAttributes = getLotData.then(function( data ) {
        // Replace details attrs w/ new attrs
        var editor = self.editorData.exports();
        var copiedAttrs = data.LotSummary.Attributes;
        copiedAttrs.push({
          Key: 'AstaC',
          Value: data.LotSummary.AstaCalc,
          Name: 'AstaCalc',
          MaxValue: null,
          MinValue: null
        });

        editor.setAttributeValues( copiedAttrs );
        self.isShowingCopyLot( false );
        showUserMessage( 'Copy successful', {
          description: 'Attribute data has been copied from lot <b>' + lotKey + '</b> but changes have not been saved.'
        });
      }).always( complete );

      return setAttributes;
    },
    canExecute: function( isExecuting ) {
      var key = self.copyLotKey();

      return !isExecuting && key != null && key !== '';
    }
  });

  function setQualityHold( lotKey, holdData ) {
    if ( holdData.holdType != null ) {
      var setHold = lotService.setLotHold( lotKey, holdData ).then(
      function( data, textStatus, jqXHR ) {
        updateLotData( lotKey, {
          HoldType: holdData.holdType,
          HoldDescription: holdData.description,
        } );
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Quality Hold creation failed', { description: errorThrown });
      });

      return setHold;
    } else {
      var removeHold = lotService.removeLotHold( lotKey ).then(
        function( data, textStatus, jqXHR ) {
        updateLotData( lotKey, {
          HoldType: null,
          HoldDescription: null,
        } );
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Quality Hold removal failed', { description: errorThrown });
      });

      return removeHold;
    }
  }

  function setLotStatus( lotKey, status ) {
    var statusKey = status;

    var setStatus = lotService.setLotStatus( lotKey, { status: statusKey } ).then(
    function( data, textStatus, jqXHR ) {
      updateLotData( lotKey, {
        QualityStatus: status
      } );
      showUserMessage( 'Override successful', { description: 'The current lab result\'s status has been changed' });

      return data;
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Override failed', { description: errorThrown });
    });

    return setStatus;
  }

  function updateLotData( lotKey, data ) {
    self.summaryData.exports().updateLotData( lotKey, data );
  }

  function loadNextPage() {
    var summaryComp = self.summaryData.exports();

    return summaryComp.getLotsCommand.execute()
      .done(function(data, textStatus, jqXHR) {
        if (data.LotSummaries.length >= self.summaryData.pageSize) {
          self.hasMoreResults(true);
        } else {
          self.hasMoreResults(false);
        }
        return data;
      });
  }

  function getAttributeList() {
    var getNames = lotService.getAttributeNames().then(
    function( data, textStatus, jqXHR ) {
      self.attrList = data;
    });

    return getNames;
  }

  function loadAttributeRanges( productType, productKey ) {
    var loadRanges = productsService.getProductDetails( productType, productKey ).then(
      function( data, textStatus, jqXHR ) {
        return mapAttributeRanges( productType, data.AttributeRanges || [] );
    });

    return loadRanges;
  }

  function mapAttributeRanges( productType, attrRanges ) {
    var attrList = ko.toJS( self.attrList[ productType ] );

    if ( attrList ) {
      ko.utils.arrayForEach( attrList, function( attr ) {
        var ranges = ko.utils.arrayFirst( attrRanges, function( attrRange ) {
          return attrRange.AttributeNameKey === attr.Key;
        });

        attr.MinValue = ranges ? ranges.MinValue : null;
        attr.MaxValue = ranges ? ranges.MaxValue : null;
      });
    }

    return attrList;
  }

  function mapLabResults( lotSummaries ) {
    var mappedSummaries = ko.utils.arrayMap( lotSummaries, function( summary ) {
      if ( summary.hasOwnProperty( 'enableQualityHold' ) ) {
        return summary;
      }

      summary.requiresQualityControlIntervention = ko.computed( function () {
        return summary.QualityStatus() === app.lists.lotQualityStatusTypes.RequiresAttention.key;
      }, summary );

      summary.enableRejection = ko.computed( function () {
        return !summary.requiresQualityControlIntervention() && summary.QualityStatus() !== app.lists.lotQualityStatusTypes.Rejected.key;
      }, summary );

      summary.enableAcceptance = ko.computed( function () {
        return !summary.requiresQualityControlIntervention() &&
          summary.QualityStatus() !== app.lists.lotQualityStatusTypes.Released.key &&
          summary.QualityStatus() !== app.lists.lotQualityStatusTypes.Contaminated.key;
      }, summary );

      summary.enableQualityHold = ko.computed( function () {
        return !summary.requiresQualityControlIntervention() && summary.QualityStatus() !== app.lists.lotQualityStatusTypes.RequiresAttention.key;
      }, summary );
    } );

    return mappedSummaries;
  }

  this.searchKey = ko.asyncCommand({
    execute: function( complete ) {
      var summaryComp = self.summaryData.exports();
      var lotKey = self.lotKeyToSearch();

      self.filtersData.filters().startingLotKey( lotKey );

      self.isLoadingResults(true);
      var getResults = summaryComp.searchLotsCommand.execute().then(
      function( data ) {
        if ( data.LotSummaries.length ) {
          self.hasMoreResults( true );
        } else {
          self.hasMoreResults( false );
        }
      }).always( function() {
        complete();
        self.isLoadingResults(false);
      });
    },
    canExecute: function( isExecuting ) {
      return !isExecuting && self.lotKeyToSearch;
    }
  });

  this.getResults = ko.asyncCommand({
    execute: function( complete ) {
      self.isLoadingResults(true);

      loadNextPage().done( getResultsSuccessCallback )
        .always(function() {
          complete();
          self.isLoadingResults( false );
        });
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  function getResultsSuccessCallback(data, textStatus, jqXHR) {
    var filters = self.filtersData.filters();
    var lotKey = filters && filters.startingLotKey.formattedLot();
    self.pagesLoaded(self.pagesLoaded() + 1);

    if (lotKey) {
      var lot = ko.utils.arrayFirst(data.LotSummaries, function(lot) {
        return lot.LotKey === lotKey;
      });

      if (lot) {
        filters.inventoryType(lot.Product.ProductType);
      }
    }

    //If date range is provided, then keep fetching next page until finished
    if (data.LotSummaries.length >= self.summaryData.pageSize && filters && ko.unwrap(filters.productionStart) && ko.unwrap(filters.productionEnd)) {
      self.isLoadingExtendedResults(true);
      loadNextPage().done(getResultsSuccessCallback);
    } else {
      self.isLoadingExtendedResults(false);
    }
  }

  function scrollTopAndFocusAttrs( Parameters ) {
    $('#lab-result-editor').scrollTop( 0 );
    $('#lab-result-editor-table').find('.form-control').first().focus();
  }

  this.navigatePrev = ko.asyncCommand({
    execute: function( complete ) {
      if ( self.isDirty() ) {
        showUserMessage( 'Would you like to save changes?', {
          description: 'The current lab result has unsaved changes. Would you like to save before navigating? Click "Yes" to save changes and navigate to the next lab result, "No" to navigate without saving changes, or "Cancel" to review changes.',
          type: 'yesnocancel',
          onYesClick: function() {
            self.isNavigating = true;
            var save = self.saveLabResultCommand.execute().then(
            function( data, textStatus, jqXHR ) {
              self.isNavigating = false;
              navPrev();
            });
          },
          onNoClick: function() {
            navPrev();
          },
          onCancelClick: complete,
        } );
      } else {
        navPrev();
      }

      function navPrev() {
        var summaryComp = self.summaryData.exports();

        var selectPrev = summaryComp.selectPrevLot().then(
          function( data, textStatus, jqXHR ) {
          if ( data ) {
            self.summaryData.selected( data );
          }
        }).always( complete );
      }
    },
    canExecute: function( isExecuting ) {
      return !isExecuting && self.nav.isPrevEnabled();
    }
  });

  this.navigateNext = ko.asyncCommand({
    execute: function( complete ) {
      if ( self.isDirty() ) {
        showUserMessage( 'Would you like to save changes?', {
          description: 'The current lab result has unsaved changes. Would you like to save before navigating? Click "Yes" to save changes and navigate to the next lab result, "No" to navigate without saving changes, or "Cancel" to review changes.',
          type: 'yesnocancel',
          onYesClick: function() {
            self.isNavigating = true;
            var save = self.saveLabResultCommand.execute().then(
            function( data, textStatus, jqXHR ) {
              self.isNavigating = false;
              navNext();
            });
          },
          onNoClick: function() {
            navNext();
          },
          onCancelClick: complete,
        } );
      } else {
        navNext();
      }

      function navNext() {
        var summaryComp = self.summaryData.exports();

        self.isWorking( 'Loading lab results...' );
        var selectNext = summaryComp.selectNextLot().then(
        function( data, textStatus, jqXHR ) {
          if ( data ) {
            self.summaryData.selected( data );
          } else {
            self.isWorking( null );
            self.hasMoreResults( false );
          }
        })
        .always( complete );
      }
    },
    canExecute: function( isExecuting ) {
      return !isExecuting && self.nav.isNextEnabled();
    }
  });

  this.showCompositeModal = ko.command({
    execute: function() {
      $('#compositeModal').modal('show');

      self.compositeData().reset();
    },
    canExecute: function() {
      return self.isInit();
    }
  });

  this.hideCompositeModal = ko.command({
    execute: function() {
      $('#compositeModal').modal('hide');
    },
    canExecute: function() {
      return true;
    }
  });

  this.saveLabResultCommand = ko.asyncCommand({
    execute: function( complete ) {
      var resultDto = self.editorData.exports().toDto();

      if ( resultDto ) {
        var saveData = lotService.saveLabResult( resultDto.LotKey, resultDto ).then(
        function( data, textStatus, jqXHR ) {
          showUserMessage( 'Save successful', { description: 'The lab result with lot key ' + resultDto.LotKey + ' was saved.' });
          self.summaryData.exports().updateLot( data );
          return data;
        },
        function( jqXHR, textStatus, errorThrown ) {
          showUserMessage( 'Save failed', {
            description: errorThrown
          } );
        }).always( complete );

        return saveData;
      } else {
        showUserMessage( 'Save failed', { description: 'Please ensure all required data has been entered.'});
        complete();
        return $.Deferred().reject();
      }
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  function createAllowance( lotKey, type, key ) {
    return lotService.createAllowance( lotKey, type, key ).then(
      function( data, textStatus, jqXHR ) {
      showUserMessage( 'Creation successful', { description: 'The allowance for ' + key + ' has been added to lot <b>' + lotKey + '</b>.' } );
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not create allowance', { description: errorThrown });
    });
  }

  function deleteAllowance( lotKey, type, key ) {
    return lotService.deleteAllowance( lotKey.replace( /\s+/g, ''), type, key ).then(
      function( data, textStatus, jqXHR ) {
      showUserMessage( 'Deletion successful', { description: 'The allowance for ' + key + ' has been removed from <b>' + lotKey + '</b>.' } );
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not remove allowance', { description: errorThrown });
    });
  }

  this.closeLabResultCommand = ko.command({
    execute: function() {
      if ( self.isDirty() ) {
        showUserMessage( 'Would you like to save changes?', {
          description: 'The current lab result has unsaved changes. Would you like to save before closing? Click "Yes" to save changes and close, "No" to close without saving changes, or "Cancel" to review changes.',
          type: 'yesnocancel',
          onYesClick: function() {
            self.isNavigating = true;
            var save = self.saveLabResultCommand.execute().then(
            function( data, textStatus, jqXHR ) {
              self.isNavigating = false;
              page('/');
            });
          },
          onNoClick: function() {
            page('/');
          },
          onCancelClick: function() {},
        } );
      } else {
        page('/');
      }
    },
    canExecute: function() {
      return self.saveLabResultCommand.canExecute();
    }
  });

  var summarySelectedSub = this.summaryData.selected.subscribe(function( labResult ) {
    var resultData = ko.toJS( labResult );

    if ( self.isNavigating ) {
      return;
    }

    if ( resultData ) {
      var key = resultData.LotKey;

      page( '/' + key );
    } else {
      page('/');
    }
  });

  // Lot History component
  this.isShowingHistory = ko.observable( false );
  this.lotHistory = ko.observable();

  function fetchLotHistory( lotKey ) {
    return lotService.getLotHistory( lotKey )
    .done(function( data, textStatus, jqXHR ) {
      self.lotHistory( data );
    })
    .fail(function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not get lot history', {
        description: errorThrown
      });
    });
  }

  this.showLotHistory = ko.asyncCommand({
    execute: function( complete ) {
      var _lot = self.editorData.data().LotKey;

      return fetchLotHistory( _lot )
      .done(function( data, textStatus, jqXHR ) {
        self.isShowingHistory( true );
      })
      .always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  this.closeLotHistory = ko.command({
    execute: function() {
      self.isShowingHistory( false );
      self.lotHistory( null );
    }
  });

  (function init() {
    var getAttrs = getAttributeList().then( null );

    var getCustomers = salesService.getCustomers()
    .then(function( data, textStatus, jqXHR ) {
      self.customers( data );

      return data;
    });

    var checkComponents = $.when( getAttrs, getCustomers ).then(
    function( data, textStatus, jqXHR ) {
      var summaryComp = self.summaryData.exports;
      if ( summaryComp() ) {
        enablePage();
      } else {
        var summarySub = summaryComp.subscribe(function( data ) {
          if ( data ) {
            enablePage();
            summarySub.dispose();
          }
        });
      }
    });

    function enablePage() {
      self.isInit( true );

      var _editor = new CompositeEditor( self.attrList[1] );
      self.compositeData( _editor );

      page();
    }
  })();

  page.base('/QualityControl/LabResults');
  page('/:key?', navigateToKey);

  function clearWorkingStatus() {
    self.isWorking( null );
  }

  function navigateToKey( ctx ) {
    var labResult = self.summaryData.selected();
    var key = ko.observable( ctx.params.key ).extend({ lotKey: true });
    key = key.formattedLot();

    self.isNavigating = true;
    self.isShowingCopyLot( false );
    self.isShowingHistory( false );

    if ( self.isInitialLoad && key ) {
      self.filtersData.filters().startingLotKey( key );
      self.getResults.execute();
    }

    self.isInitialLoad = false;

    if ( !key ) {
      self.summaryData.selected( null );
      self.LabResult( null );
      self.isNavigating = false;
    } else if ( key ) {
      var lotKey = labResult && labResult.LotKey;

      // Check if lab result is selected and matches key
      if ( typeof labResult === "object" && lotKey === key ) {
        var prepResult = prepLabResultData( labResult ).done( scrollTopAndFocusAttrs ).always(function() {
            self.isNavigating = false;
          });
      } else {
        // Fetch correct data from summary table
        var selectLot = self.summaryData.exports().selectLotKey( key ).then(
        function( data, textStatus, jqXHR ) {
          return ko.toJS( data );
        });

        var checkLotData = selectLot.then(
        function( data, textStatus, jqXHR ) {
          var prepData = prepLabResultData( data ).done( scrollTopAndFocusAttrs ).always(function() {
            self.isNavigating = false;
            clearWorkingStatus();
          });

          return prepData;
        },
        function( jqXHR, textStatus, errorThrown ) {
          var getLotData = lotService.getLotData( key ).then(
          function( data, textStatus, jqXHR ) {
            var matchedAttrs = ko.utils.arrayFirst( data.AttributeNamesByProductType, function( attr ) {
              return attr.Key === data.LotSummary.Product.ProductType;
            });

            var sortedAttrs = lotService.sortAttributes( matchedAttrs.Value );

            self.summaryData.attrs( sortedAttrs  );
            var prepData = prepLabResultData( data.LotSummary ).then(
            function( data, textStatus, jqXHR ) {
              scrollTopAndFocusAttrs();
              return data;
            });
          },
          function( jqXHR, textStatus, errorThrown ) {
            showUserMessage( 'Could not find lab result' );
          }).always(function() {
            self.isNavigating = false;
          });
        });
      }
    }
  }

  function prepLabResultData( labResult ) {
    var productData = ko.toJS( labResult.Product );
    var productType = productData.ProductType;
    var productKey = productData.ProductKey;

    self.isWorking( 'Loading attributes...' );

    if ( labResult.AstaCalc ) {
      labResult.Attributes.push({
        Key: 'AstaC',
        Value: labResult.AstaCalc,
        Name: 'AstaCalc',
        MaxValue: null,
        MinValue: null
      });
    }

    var loadRanges = loadAttributeRanges( productType, productKey ).then(
    function( data, textStatus, jqXHR ) {
      self.summaryData.attrs( data );
      self.summaryData.selected( labResult );
      self.LabResult( labResult );
    }).always( clearWorkingStatus );

    return loadRanges;
  }

  // Exports
  return this;
}

var vm = new LabResultsVM();

ko.applyBindings( vm );

module.exports = vm;

