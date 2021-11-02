/** Knockout Components */
ko.components.register( 'production-schedules-summary', require('App/components/production/production-schedules-summary/production-schedules-summary') );
ko.components.register( 'production-schedules-editor', require('App/components/production/production-schedules-editor/production-schedules-editor') );
ko.components.register( 'loading-screen', require('App/components/common/loading-screen/loading-screen') );

/** Required libraries */
var page = require('page');
var productsService = require('App/services/productsService');

require('bootstrap');
require('App/koBindings');
require('App/koExtensions');
require('App/helpers/koPunchesFilters.js');
ko.punches.enableAll();

/** Product Schedules view model */
function ProductionSchedulesVM() {
  if ( !(this instanceof ProductionSchedulesVM) ) { return new ProductionSchedulesVM( params ); }

  var self = this;

  this.isInit = ko.observable( false );

  // Search bar
  this.searchKey = ko.observable();

  this.searchForKey = ko.command({
    execute: function() {
      page( '/' + self.searchKey() );
    },
    canExecute: function() {
      return self.searchKey();
    }
  });

  // Summary component
  this.summaryData = {
    input: ko.observableArray( [] ),
    selected: ko.observable(),
    exports: ko.observable()
  };

  var summaryDataPager = productsService.getProductionSchedulesDataPager({
    onNewPageSet: function() {
      self.summaryData.input( [] );
    }
  });

  this.filters = {
    productionDate: ko.observableDate(),
    productionLineLocationkey: ko.observable(),
  };
  summaryDataPager.addParameters( this.filters );

  this.loadMore = ko.asyncCommand({
    execute: function( complete ) {
      summaryDataPager.nextPage().then(
      function( data, textStatus, jqXHR ) {
        var _summaries = self.summaryData.input;

        ko.utils.arrayPushAll( _summaries, data );
        _summaries.valueHasMutated();
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not load schedules', {
          description: errorThrown
        });
      });
      complete();
    }
  });

  // Editor component
  this.editorData = {
    input: ko.observable(),
    exports: ko.observable()
  };
  this.summaryData.selected.subscribe(function( summaryItem ) {
    if ( summaryItem ) {
      page( '/' + summaryItem.ProductionScheduleKey );
    }
  });

  this.isDirty = ko.pureComputed(function() {
    var _editor = self.editorData.exports();

    return _editor && _editor.isDirty();
  });

  // New production schedule
  this.options = {
    productionLines: ko.observableArray( [] ),
  };
  this.newScheduleDate = ko.observableDate();
  this.newScheduleLine = ko.observable();
  var $scheduleModal = $('#createSchedule');
  $scheduleModal.on( 'hidden.bs.modal', function() {
    self.newScheduleDate( null );
    self.newScheduleLine( null );
  });

  this.startNewSchedule = ko.command({
    execute: function() {
      self.newScheduleDate( Date.now() );
      $scheduleModal.modal('show');
    }
  });

  this.cancelNewSchedule = ko.command({
    execute: function() {
      $scheduleModal.modal('hide');
    }
  });

  this.createNewSchedule = ko.asyncCommand({
    execute: function( complete ) {
      var _scheduleData = ko.toJS({
        ProductionDate: self.newScheduleDate,
        ProductionLineLocationKey: self.newScheduleLine,
      });

      var createSchedule = productsService.createProductionSchedule( _scheduleData ).then(
      function( data, textStatus, jqXHR ) {
        page( '/' + data.ProductionScheduleKey );
        self.summaryData.exports().addSummary( data );
        $scheduleModal.modal('hide');
      },
      function( jqXHR, textStatus, errorThrown ) {
        if ( errorThrown.search('already exists') > -1 ) {
          var _date = new Date( _scheduleData.ProductionDate );

          var _month = "" + (_date.getUTCMonth() + 1);
          _month = _month.length < 2 ?
            "0" + _month :
            _month;

          var _day = "" + _date.getUTCDate();
          _day = _month.length < 2 ?
            "0" + _month :
            _day;

          var _dateStr = _date.getUTCFullYear() + _month + _day;
          var _line = _scheduleData.ProductionLineLocationKey;

          page( '/' + _dateStr + '-' + _line );

          showUserMessage( 'Could not create new schedule', {
            description: errorThrown + ' The existing schedule will be loaded instead.'
          });

          self.cancelNewSchedule.execute();
        } else {
          showUserMessage( 'Could not create new schedule', {
            description: errorThrown
          });
        }
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting && self.newScheduleDate() && self.newScheduleLine();
    }
  });

  // Editor controls
  this.saveSchedule = ko.asyncCommand({
    execute: function( complete ) {
      var _scheduleData = self.editorData.exports().toDto();
      var _scheduleKey = _scheduleData.ProductionScheduleKey;

      var save = productsService.updateProductionSchedule( _scheduleKey, _scheduleData ).then(
      function( data, textStatus, jqXHR ) {
        self.editorData.exports().resetDirtyFlag();
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Failed to update schedule', {
          description: errorThrown
        });
      });

      var checkSave = save.then(
      function( data, textStatus, jqXHR ) {
      }).always( complete );

      return checkSave;
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  this.saveScheduleCommand = ko.asyncCommand({
    execute: function( complete ) {
      return self.saveSchedule.execute().always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting && self.saveSchedule.canExecute() && self.isDirty();
    }
  });

  this.isDeleting = ko.observable( false );
  this.deleteSchedule = ko.asyncCommand({
    execute: function( complete ) {
      var _scheduleData = self.editorData.input();
      var _scheduleKey = _scheduleData.ProductionScheduleKey;

      showUserMessage( 'Delete schedule?', {
        description: 'Are you sure you want to delete this schedule? This action cannot be undone.',
        type: 'yesno',
        onYesClick: function() {
          self.isDeleting( true );
          productsService.deleteProductionSchedule( _scheduleKey ).then(
          function( data, textStatus, jqXHR ) {
            self.summaryData.exports().removeSummary( _scheduleKey );
            page('/');
          },
          function( jqXHR, textStatus, errorThrown ) {
            showUserMessage( 'Could not delete schedule', {
              description: errorThrown
            });
          }).always(function() {
            self.isDeleting( false );
            complete();
          });
        },
        onNoClick: function() {
          complete();
        }
      });
    }
  });

  this.isPrintingReport = false;
  this.goToReport = function( data, element ) {
    function loadReport() {
      window.location.href = data.href;
    }

    if ( self.isDirty() ) {
      showUserMessage( 'Save before printing report?', {
        description: 'There are unsaved changes for the current schedule. Would you like to save them before printing a report?',
        type: 'yesnocancel',
        onYesClick: function() {
          self.isPrintingReport = true;
          self.saveScheduleCommand.execute().then(
          function( data, textStatus, jqXHR ) {
            loadReport();
          },
          function( jqXHR, textStatus, errorThrown ) {
            showUserMessage( 'Failed to save schedule', {
              description: errorThrown
            });
          });
        },
        onNoClick: function() {
          self.isPrintingReport = true;
          loadReport();
        },
        onCancelClick: function() { }
      });
    } else {
      loadReport();
    }
  };

  this.reportLinks = ko.pureComputed(function() {
    var _editor = self.editorData.input();
    var _links = _editor && _editor.Links;

    return [{
      href: _links[ 'report-prod-schedule-day+line' ].HRef,
      label: 'Print Line'
    },
    {
      href: _links[ 'report-prod-schedule-day' ].HRef,
      label: 'Print Day'
    }];
  });


  this.closeEditor = ko.command({
    execute: function() {
      if ( self.isDirty() ) {
        showUserMessage( 'Save before closing?', {
          description: 'The current schedule has unsaved changed. Would you like to save before closing?',
          type: 'yesnocancel',
          onYesClick: function() {
            self.saveScheduleCommand.execute();
          },
          onNoClick: function() {
            self.editorData.exports().resetDirtyFlag();
            page('/');
          },
          onCancelClick: function() { },
        });
      } else {
        page('/');
      }
    }
  });

  // Routing
  page.base('/Production/ProductionSchedules');

  window.onbeforeunload = function(){
    if ( !self.isPrintingReport && self.isDirty() ) {
      return 'Are you sure you want to leave?';
    }

    self.isPrintingReport = false;
  };

  var isRedirecting = false;
  var previousCtx = null;
  function checkIfDirty( ctx, next ) {
    if ( isRedirecting ) {
      isRedirecting = false;
      return;
    }

    if ( self.isDirty() ) {
      showUserMessage( 'Save before navigating?', {
        description: 'Would you like to save before navigating?',
        type: 'yesnocancel',
        onYesClick: function() {
          self.saveScheduleCommand.execute().then(
            function( data, textStatus, jqXHR ) {
            previousCtx = ctx;
            next();
          },
          function( jqXHR, textStatus, errorThrown ) {
            isRedirecting = true;
            page( previousCtx.path );
          });
        },
        onNoClick: function() {
          self.editorData.exports().resetDirtyFlag();

          previousCtx = ctx;
          next();
        },
        onCancelClick: function() {
          isRedirecting = true;

          page( previousCtx.path );
        },
      });
    } else {
      previousCtx = ctx;
      next();
    }
  }
  page( checkIfDirty );

  this.loadScheduleDetails = ko.asyncCommand({
    execute: function( scheduleKey, complete ) {
      var getDetails = productsService.getProductionScheduleDetails( scheduleKey ).always( complete );

      return getDetails;
    }
  });
  function navigateToSchedule( ctx, next ) {
    var _scheduleKey = ctx.params.scheduleKey;

    if ( _scheduleKey === 'new' ) {
      self.editorData.input( {} );
    } else if ( _scheduleKey ) {
      var getDetails = self.loadScheduleDetails.execute( _scheduleKey ).then(
      function( data, textStatus, jqXHR ) {
        self.editorData.input( data );
      },
      function( jqXHR, textStatus, errorThrown ) {
        page.redirect('/');
        showUserMessage( 'Could not load schedule', {
          description: 'The schedule with key <b>' + _scheduleKey + '</b> could not be found',
        });
      });
    } else {
      next();
    }
  }
  page( '/:scheduleKey?', navigateToSchedule );

  function returnToSummaries( ctx, next ) {
    self.editorData.input( null );
    self.summaryData.selected( null );
  }
  page( returnToSummaries );

  // Page initialization
  (function init() {
    var loadProductionLines = productsService.getProductionLocations().then(
    function( data, textStatus, jqXHR ) {
      self.options.productionLines( data );
    });

    var checkOptions = $.when( loadProductionLines ).then(
    function( data, textStatus, jqXHR ) {
      self.isInit( true );
      self.loadMore.execute();
      page();
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Failed to load production schedules UI', {
        description: errorThrown
      });
    });
  })();

  // Exports
  return this;
}

var vm = new ProductionSchedulesVM();

ko.applyBindings( vm );

module.exports = vm;
