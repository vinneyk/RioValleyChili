/** Required libraries */
var lotService = require('App/services/lotService');

require('print-this');
require('App/koExtensions');
require('App/helpers/koPunchesFilters');

ko.punches.enableAll();

/** KO components */
ko.components.register( 'loading-screen', require('App/components/common/loading-screen/loading-screen'));

/** Lot Trace VM */
function LotTraceVM() {
  if ( !(this instanceof LotTraceVM) ) { return new LotTraceVM(); }

  var self = this;

  this.searchKey = ko.observable().extend({ lotKey: true });

  // Trace data
  this.traces = {
    inputs: ko.observable(),
    outputs: ko.observable(),
  };

  this.inputsVisible = ko.observable( false );
  this.toggleInputs = function() {
    self.inputsVisible( !self.inputsVisible() );
  };

  this.outputsVisible = ko.observable( false );
  this.toggleOutputs = function() {
    self.outputsVisible( !self.outputsVisible() );
  };

  // Cache checking
  this.lotKey = ko.observable();
  function checkIfCurrentLot( lotKey ) {
    if ( self.lotKey() !== lotKey ) {
      self.lotKey( lotKey );

      self.traces.inputs( null );
      self.traces.outputs( null );
    }
  }

  // Input Trace
  this.traceInput = ko.asyncCommand({
    execute: function( complete ) {
      var lotKey = self.searchKey();

      checkIfCurrentLot( lotKey );

      var getTrace = lotService.getLotInputTrace( lotKey )
      .done(function( data, textStatus, jqXHR ) {
        self.traces.inputs( data );
        self.outputsVisible( false );
        self.inputsVisible( true );
      })
      .fail(function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not trace lot', {
          description: errorThrown
        });
      })
      .always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting && self.searchKey() != null;
    }
  });
  this.loadingInput = ko.pureComputed(function() {
    return self.searchKey() != null && !self.traceInput.canExecute();
  });

  // Output Trace
  this.traceOutput = ko.asyncCommand({
    execute: function( complete ) {
      var lotKey = self.searchKey();

      checkIfCurrentLot( lotKey );

      var getTrace = lotService.getLotOutputTrace( lotKey )
      .done(function( data, textStatus, jqXHR ) {
        self.traces.outputs( data );
        self.inputsVisible( false );
        self.outputsVisible( true );
      })
      .fail(function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not trace lot', {
          description: errorThrown
        });
      })
      .always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting && self.searchKey() != null;
    }
  });
  this.loadingOutput = ko.pureComputed(function() {
    return self.searchKey() != null && !self.traceOutput.canExecute();
  });

  this.printTrace = ko.command({
    execute: function() {
      var _inputs = self.inputsVisible();
      var _outputs = self.outputsVisible();
      var _lotKey = self.lotKey();

      self.inputsVisible( true );
      self.outputsVisible( true );

      setTimeout(function() {
        $('#trace-inputs, #trace-outputs').printThis({
          pageTitle: 'Lot Trace for ' + _lotKey,
          header: '<h1>Lot Trace for ' + _lotKey + '</h1>',
          importCSS: true,
          importStyle: false,
          loadCSS: '/Content/print-trace.css',
          printContainer: true,
          printDelay: 0,
        });

        setTimeout(function() {
          self.inputsVisible( _inputs );
          self.outputsVisible( _outputs );
        }, 1000);
      }, 1000);
    },
    canExecute: function() {
      return self.traces.outputs() || self.traces.inputs();
    }
  });

  // Exports
  return this;
}

var vm = new LotTraceVM();

ko.applyBindings( vm );

module.exports = vm;
