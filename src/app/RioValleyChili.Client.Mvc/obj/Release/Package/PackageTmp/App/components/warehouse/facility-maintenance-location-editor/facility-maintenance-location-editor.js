var warehouseService = require('App/services/warehouseService');

function Location( locationData ) {
  this.cache = locationData;

  // Static Data
  this.LocationKey = locationData.LocationKey;
  this.FacilityKey = locationData.FacilityKey;
  this.FacilityName = locationData.FacilityName;
  this.Active = locationData.Active;
  this.Description = locationData.Description;

  // Editable data
  this.GroupName = ko.observable( locationData.GroupName );
  this.Row = ko.observable( locationData.Row );
  this.Status = ko.observable( locationData.Status );

  this.statusIcon = ko.pureComputed(function() {
    var _status = this.Status();

    var _statusOption = ko.utils.arrayFirst( this.statusOptions, function( opt ) {
      return opt.key === _status;
    } );

    return _statusOption && _statusOption.icon;
  }, this );

  // State
  this.isEditing = ko.observable( locationData.isEditing || locationData.FacilityKey == null );
}

Location.prototype.statusOptions = [{
  icon: 'fa-check',
  key: 1,
  value: 'Available'
},
{
  icon: 'fa-lock',
  key: 2,
  value: 'Locked'
},
{
  icon: 'fa-times',
  key: 3,
  value: 'Inactive'
}];

Location.prototype.toDto = function() {
  return ko.toJS({
    FacilityKey: this.FacilityKey,
    GroupName: this.GroupName,
    Row: this.Row,
    Status: this.Status
  });
};

function LocationEditor( locationsData ) {
  var self = this;

  this.FacilityKey = locationsData.facilityKey;

  var locations = (locationsData.locations || []).map(function( location ) {
    return new Location( location );
  });

  this.sortLocations = function( locations ) {
    locations.sort(function( a, b ) {
      var _a = a.Row();
      var _b = b.Row();

      if ( _a > _b ) {
        return 1;
      } else if ( _a < _b ) {
        return -1;
      } else {
        return 0;
      }
    });
  };
  this.sortLocations( locations );

  this.locations = ko.observableArray( locations );

  var _streets = [];
  ko.utils.arrayForEach( self.locations(), function( location ) {
    var _street = ko.unwrap( location.GroupName );

    if ( _street && _streets.indexOf( _street ) === -1 ) {
      _streets.push( _street );
    }

    _streets.sort();
  });
  this.streetNames = ko.observableArray( _streets );

  this.street = ko.observable( this.streetNames()[0] );
  this.streetLocations = ko.pureComputed(function() {
    var _locations = self.locations().filter(function( location ) {
      return location.cache.GroupName === self.street();
    });

    return _locations;
  });

  this.createStreet = function( streetName ) {
    var initLocation = {
      Row: 1,
      GroupName: streetName,
      Status: 1,
      FacilityKey: self.FacilityKey,
    };

    return self.createLocation( initLocation ).then(
      function( data ) {
      var newLocation = new Location({
        Row: initLocation.Row,
        GroupName: initLocation.GroupName,
        Status: initLocation.Status,
        FacilityKey: initLocation.FacilityKey,
        isEditing: true,
        LocationKey: data
      });
      self.locations.push( newLocation );
      self.streetNames.push( streetName );
      self.streetNames.sort();
      self.street( streetName );
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not create new street', {
        description: errorThrown
      });
    });
  };

  this.save = ko.asyncCommand({
    execute: function( locationData, complete ) {
      var onSuccess = function() {
        this.isEditing( false );
        this.cache.Row = this.Row();
        this.cache.GroupName = this.GroupName();
      }.bind( locationData );

      if ( locationData.FacilityKey != null ) {
        var _updateData = locationData.toDto();

        var updateLocation = self.updateLocation( locationData.LocationKey, _updateData, onSuccess ).always(function() {
          complete();
          this.locations.valueHasMutated();
        }.bind( self ));

        return updateLocation;
      } else {
        var _createData = locationData.toDto();
        _createData.FacilityKey = self.FacilityKey;

        var createLocation = self.createLocation( _createData, onSuccess ).always(function( data ) {
          locationData.LocationKey = data;
          this.locations.valueHasMutated();
          complete();
        }.bind( self ));

        return createLocation;
      }
    }
  });

  this.addLocation = ko.command({
    execute: function() {
      self.locations.push( new Location({
        GroupName: self.street(),
      }) );
    }
  });

  this.saveLocation = function( location ) {
    self.save.execute( location )
    .done(function() {
      self.sortLocations( self.locations );
    });
  };

  this.editLocation = ko.command({
    execute: function( location ) {
      location.isEditing( true );
    }
  });

  this.cancelEditLocation = ko.command({
    execute: function( location ) {
      if ( location.FacilityKey == null ) {
        var i = self.locations().indexOf( location );

        self.locations.splice( i, 1 );
      } else {
        location.isEditing( false );
        location.Row( location.cache.Row );
        location.GroupName( location.cache.GroupName );
        location.Status( location.cache.Status );
      }
    }
  });
}

LocationEditor.prototype.createLocation = function( locationData, successCallback ) {
  var createLocation = warehouseService.createLocation( locationData ).then(function( data ) {
    if ( successCallback ) {
       successCallback();
    }

    return data;
  },
  function( jqXHR, textStatus, errorThrown ) {
    showUserMessage( 'Could not save location', {
      description: errorThrown
    });
  });

  return createLocation;
};

LocationEditor.prototype.updateLocation = function( locationKey, locationData, successCallback ) {
  var updateLocation = warehouseService.updateLocation( locationKey, locationData )
  .then(function( data ) {
    if ( successCallback ) {
       successCallback();
    }

    return data;
  },
  function( jqXHR, textStatus, errorThrown ) {
    showUserMessage( 'Could not save location', {
      description: errorThrown
    });
  });

  return updateLocation;
};

LocationEditor.prototype.saveAll = function() {
  var saves = [];

  var onSuccess = function() {
    this.isEditing( false );
    this.cache.Row = this.Row();
    this.cache.GroupName = this.GroupName();
  };

  var saveLocation = function( location ) {
    if ( location.FacilityKey != null ) {
      var _updateData = location.toDto();

      return this.updateLocation( location.LocationKey, _updateData, onSuccess.bind( location ) );
    } else {
      var _createData = location.toDto();
      _createData.FacilityKey = this.FacilityKey;

      return this.createLocation( _createData, onSuccess.bind( location ) );
    }
  }.bind( this );

  ko.utils.arrayForEach( this.locations(), function( location ) {
    if ( location.isEditing() ) {
      saves.push( saveLocation( location ) );
    }
  });

  var checkSaves = $.when.apply( $, saves ).always(function() {
    this.locations.valueHasMutated();
  }.bind( this ));

  return checkSaves;
};

function FacilityMaintenanceLocationEditorVM( params ) {
  if ( !(this instanceof FacilityMaintenanceLocationEditorVM) ) { return new FacilityMaintenanceLocationEditorVM( params ); }

  var self = this;

  function buildEditor( locationsData ) {
    return new LocationEditor( locationsData || [] );
  }

  this.editor = ko.observable( buildEditor( ko.toJS( params.input ) ) );
  var currentStreet = ko.pureComputed(function() {
    var _editor = self.editor();

    return _editor && _editor.street();
  });

  var isDirty = ko.pureComputed(function() {
    var _editor = self.editor();

    return _editor && ko.utils.arrayFirst( ko.unwrap( _editor.locations() ), function( location ) {
      return location.isEditing();
    } );
  });

  function loadUpdatedData( newLocationsData ) {
    var _editor = self.editor();
    var _street = _editor && _editor.street();

    self.editor( buildEditor( newLocationsData ) );
    self.editor().street( _street );
  }

  function saveAll() {
    var _editor = self.editor();

    return _editor && _editor.saveAll();
  }

  function createStreet( streetName ) {
    return self.editor().createStreet( streetName );
  }

  // Exports
  if ( params && params.exports ) {
    params.exports({
      isDirty: isDirty,
      saveAll: saveAll,
      currentStreet: currentStreet,
      loadUpdatedData: loadUpdatedData,
      createStreet: createStreet,
    });
  }

  return this;
}

module.exports = {
  viewModel: FacilityMaintenanceLocationEditorVM,
  template: require('./facility-maintenance-location-editor.html')
};
