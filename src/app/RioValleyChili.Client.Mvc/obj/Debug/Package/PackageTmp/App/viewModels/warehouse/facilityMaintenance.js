/** Required libraries */
var warehouseService = require('App/services/warehouseService');
var rvc = require('rvc');
var page = require('page');

require('App/koBindings');
require('App/helpers/koValidationHelpers');
require('bootstrap');

/** KO Components*/
ko.components.register( 'fm-summary', require('App/components/warehouse/facility-maintenance-summary/facility-maintenance-summary') );
ko.components.register( 'fm-editor', require('App/components/warehouse/facility-maintenance-editor/facility-maintenance-editor') );
ko.components.register( 'fm-location-editor', require('App/components/warehouse/facility-maintenance-location-editor/facility-maintenance-location-editor') );
ko.components.register( 'loading-screen', require('App/components/common/loading-screen/loading-screen') );

/** Facility Maintenance view model */
function FacilityMaintenanceVM() {
  if ( !(this instanceof FacilityMaintenanceVM) ) { return new FacilityMaintenanceVM(); }

  var self = this;

  this.isInit = ko.observable( false );

  this.options = {
    facilities: ko.observableArray([]),
    facilityTypes: ko.utils.arrayMap( Object.keys( rvc.lists.facilityTypes ), function( opt ) {
      return rvc.lists.facilityTypes[ opt ];
    })
  };

  // Summary view
  this.summaryUI = {
    input: this.options.facilities,
    selected: ko.observable(),
    exports: ko.observable(),
  };

  this.summaryUI.selected.subscribe(function( selectedFacility ) {
    if ( selectedFacility ) {
      editLocations( selectedFacility );
    }
  });

  // Facility info editor
  this.editorUI = {
    input: ko.observable(),
    exports: ko.observable()
  };

  // Facility location editor
  this.locationEditorUI = {
    input: ko.observable(),
    exports: ko.observable()
  };


  this.isEditingInfo = ko.pureComputed(function() {
    return !!self.editorUI.input();
  });

  this.startNewFacility = ko.command({
    execute: function() {
      page('/new');
    }
  });

  this.editFacilityInfo = ko.command({
    execute: function() {
      var _facility = self.currentFacility();
      var _facilityKey = _facility && _facility.FacilityKey;

      if ( _facilityKey != null ) {
        page( '/' + _facilityKey + '/edit' );
      }
    },
    canExecute: function () {
      var _editor = self.locationEditorUI.exports();
      return _editor && !_editor.isDirty();
    }
  });

  this.isSavingInfo = ko.observable( false );
  this.saveFacilityInfo = ko.asyncCommand({
    execute: function( complete ) {
      var _editor = self.editorUI.exports();
      var _data = _editor.toDto();
      var _isNew = _data.FacilityKey == null;

      self.isSavingInfo( true );
      if ( !_isNew ) {
        var updateData = warehouseService.updateFacility( _data.FacilityKey, _data ).then(
        function( data ) {
          return data;
        },
        function( jqXHR, textStatus, errorThrown ) {
          showUserMessage( 'Could not save facility info', {
            description: errorThrown
          });
        });

        var reloadUpdatedSummaries = updateData.then(
        function() {
          return loadFacilities();
        })
        .always(function() {
          self.isSavingInfo( false );
          complete();
        });

        return reloadUpdatedSummaries;
      } else {
        var createFacility = warehouseService.createFacility( _data ).then(
        function( data ) {
          return data;
        },
        function( jqXHR, textStatus, errorThrown ) {
          showUserMessage( 'Could not create new facility', {
            description: errorThrown
          });
        });

        var updateFacilityEditor = createFacility
        .done(function( data ) {
          self.editorUI.input( null );
          page( '/' + data + '/locations' );
        });

        var reloadSummaries = updateFacilityEditor.then(
        function() {
          return loadFacilities();
        })
        .always(function() {
          self.isSavingInfo( false );
          complete();
        });

        return reloadSummaries;
      }
    },
    canExecute: function( isExecuting ) {
      var _editor = self.editorUI.exports();

      return !isExecuting && _editor && _editor.isValid();
    }
  });

  this.closeEditorUI = ko.command({
    execute: function() {
      if ( self.isEditingLocations() ) {
        self.editorUI.input( null );
        return history.back();
      }

      page('/');
    }
  });

  this.isEditingLocations = ko.pureComputed(function() {
    return !!self.locationEditorUI.input();
  });

  function editLocations( facility ) {
    var _facilityKey = facility && facility.FacilityKey;

    page( '/' + _facilityKey + '/locations' );
  }

  this.isSavingAllLocations = ko.observable( false );
  this.saveAllLocations = ko.asyncCommand({
    execute: function( complete ) {
      var _saves = self.locationEditorUI.exports().saveAll();

      self.isSavingAllLocations( true );
      var checkSaves = _saves.then(
      null,
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Location saving failed', {
          description: 'Some locations could not be saved. ' + errorThrown
        });
      })
      .always(function() {
        self.isSavingAllLocations( false );
        complete();
      });

      return checkSaves;
    },
    canExecute: function( isExecuting ) {
      var _editor = self.locationEditorUI.exports();

      return !isExecuting && _editor && _editor.isDirty();
    }
  });

  this.currentFacility = ko.observable();

  this.isFreezing = ko.observable( false );
  this.freezeStreet = ko.asyncCommand({
    execute: function( complete ) {
      var _editor = self.locationEditorUI.exports();
      var _facilityKey = self.currentFacility().FacilityKey;
      var _street = _editor && _editor.currentStreet();

      self.isFreezing( true );
      var freezeStreet = warehouseService.freezeStreet( _facilityKey, _street )
      .done(function( data ) {
        self.locationEditorUI.exports().loadUpdatedData({
          locations: data,
          facilityKey: _facilityKey
        });
      })
      .fail(function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not freeze street', {
          description: errorThrown
        });
      })
      .always(function() {
        self.isFreezing( false );
        complete();
      });

      return freezeStreet;
    },
    canExecute: function( isExecuting ) {
      return !isExecuting && self.currentFacility() != null;
    }
  });

  this.isUnfreezing = ko.observable( false );
  this.unfreezeStreet = ko.asyncCommand({
    execute: function( complete ) {
      var _editor = self.locationEditorUI.exports();
      var _facilityKey = self.currentFacility().FacilityKey;
      var _street = _editor && _editor.currentStreet();

      self.isUnfreezing( true );
      var unfreezeStreet = warehouseService.unfreezeStreet( _facilityKey, _street )
      .done(function( data ) {
        self.locationEditorUI.exports().loadUpdatedData({
          locations: data,
          facilityKey: _facilityKey
        });
      })
      .fail(function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not unfreeze street', {
          description: errorThrown
        });
      })
      .always(function() {
        complete();
        self.isUnfreezing( false );
      });

      return unfreezeStreet;
    },

    canExecute: function( isExecuting ) {
      return !isExecuting && self.currentFacility() != null;
    }
  });
  this.closeLocationEditorUI = ko.command({
    execute: function() {
      page('/');
    }
  });

  // New street modal
  this.newStreetName = ko.observable('');

  this.startNewStreet = ko.command({
    execute: function() {
      $('#new-street-modal').modal('show');
    }
  });

  this.saveNewStreet = ko.asyncCommand({
    execute: function( complete ) {
      var streetName = self.newStreetName();

      return self.locationEditorUI.exports().createStreet( streetName ).then(
      function() {
        self.cancelNewStreet.execute();
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting && self.newStreetName();
    }
  });

  this.cancelNewStreet = ko.command({
    execute: function() {
      $('#new-street-modal').modal('hide');
    }
  });

  $('#new-street-modal').on('hidden.bs.modal', function() {
    self.newStreetName('');
  });

  this.isShowingSummaries = ko.pureComputed(function() {
    return self.editorUI.input() == null && self.locationEditorUI.input() == null;
  });

  // Page routing
  page.base('/Warehouse/FacilityMaintenance');

  function showNewFacility( ctx, next ) {
    var _mode = ctx.params.mode;

    if ( _mode === 'new' ) {
      self.editorUI.input({});
      return;
    }

    next();
  }
  page('/:mode', showNewFacility );

  function getDetails( facilityKey ) {
    return warehouseService.getFacilityDetails( facilityKey )
    .fail(function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not load facility data', {
        description: errorThrown
      });
    });
  }

  this.getFacilityDetails = ko.asyncCommand({
    execute: function( facilityKey, complete ) {
      return getDetails( facilityKey )
      .done(function( data ) {
        self.editorUI.input( data );
      })
      .always( complete );
    }
  });

  this.getFacilityLocations = ko.asyncCommand({
    execute: function( facilityKey, complete ) {
      var getFacilityDetails = getDetails( facilityKey ).then(
      function( data ) {
        self.currentFacility( data );
      });

      var getLocations = getFacilityDetails.then(function() {
        return warehouseService.getFacilityLocations( facilityKey )
        .done(function( data ) {
          self.locationEditorUI.input({
            locations: data,
            facilityKey: facilityKey
          });
        })
        .fail(function( jqXHR, textStatus, errorThrown ) {
          showUserMessage( 'Could not load facility data', {
            description: errorThrown
          });
        })
        .always( complete );
      });

      return getLocations;
    }
  });

  function loadFacilityDetails( ctx, next ) {
    var _facilityKey = ctx.params.facilityKey;
    var _mode = ctx.params.mode;

    var _currentFacility = self.currentFacility();
    if ( _mode === 'locations' && (!_currentFacility || _currentFacility && _currentFacility.FacilityKey != _facilityKey) ) {
      return self.getFacilityLocations.execute( _facilityKey );
    }

    if ( _mode === 'edit' ) {
      return self.getFacilityDetails.execute( _facilityKey );
    }

    next();
  }
  page('/:facilityKey/:mode', loadFacilityDetails );

  function returnToSummaries() {
    self.editorUI.input( null );
    self.locationEditorUI.input( null );
    self.summaryUI.selected( null );
    self.currentFacility( null );
    self.cancelNewStreet.execute();
  }
  page('/', returnToSummaries );

  function loadFacilities() {
    return warehouseService.getFacilities()
    .done(function( data ) {
      self.options.facilities( data );
    });
  }

  (function init() {
    var getFacilities = loadFacilities();

    var checkOpts = $.when( getFacilities )
    .done(function() {
      self.isInit( true );
      page();
    })
    .fail(function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not load Facility Maintenance UI', {
        description: errorThrown
      } );
    });

    return checkOpts;
  })();

  // Exports
  return this;
}

var vm = new FacilityMaintenanceVM();

ko.applyBindings( vm );

module.exports = vm;
