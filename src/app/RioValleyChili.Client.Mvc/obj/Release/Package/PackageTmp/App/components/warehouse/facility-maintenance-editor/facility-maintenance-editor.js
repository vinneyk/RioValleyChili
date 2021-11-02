ko.components.register( 'address-editor', require('App/components/warehouse/address-editor/address-editor') );

function FacilityEditor( facilityData ) {
  this.cache = {
    facilityName: facilityData.FacilityName
  };

  this.FacilityKey = facilityData.FacilityKey;
  this.FacilityName = ko.observable( facilityData.FacilityName ).extend({ required: true });
  this.FacilityType = ko.observable( facilityData.FacilityType ).extend({ required: true });
  this.Active = ko.observable( facilityData.Active );

  var _shipLabel = facilityData.ShippingLabel || {};
  this.ShippingLabelName = ko.observable( _shipLabel.Name );
  this.Locations = ko.observableArray( facilityData.Locations );
  this.Phone = ko.observable( _shipLabel.Phone );
  this.EMail = ko.observable( _shipLabel.EMail );

  this.address = {
    input: ko.observable( _shipLabel.Address ),
    exports: ko.observable(),
  };

  this.validation = ko.validatedObservable({
    type: this.FacilityType,
    name: this.FacilityName
  });
}

FacilityEditor.prototype.toDto = function() {
  return ko.toJS({
    isNew: this.FacilityKey != null,
    FacilityKey: this.FacilityKey,
    FacilityType: this.FacilityType,
    FacilityName: this.FacilityName,
    Active: this.Active,
    PhoneNumber: this.Phone,
    EMailAddress: this.EMail,
    ShippingLabelName: this.ShippingLabelName,
    Address: this.address.exports(),
  });
};

function FacilityMaintenanceEditorVM( params ) {
  if ( !(this instanceof FacilityMaintenanceEditorVM) ) { return new FacilityMaintenanceEditorVM( params ); }

  var self = this;
  this.disposables = [];

  this.options = params.options;

  function buildEditor( facilityData ) {
    return new FacilityEditor( facilityData || {} );
  }

  var _editor = buildEditor( ko.toJS( params.input ) );
  this.editor = ko.observable( _editor );

  if ( ko.isObservable( params.input ) ) {
    this.disposables.push( params.input.subscribe(function( newData ) {
      if ( newData ) {
        self.editor( buildEditor( newData ) );
      }
    }) );
  }


  var isValid = ko.pureComputed(function() {
    var _editor = self.editor();

    return _editor && _editor.validation.isValid();
  });

  function toDto() {
    var _editor = self.editor();

    return _editor && _editor.toDto();
  }

  // Exports
  if ( params && params.exports ) {
    params.exports({
      isValid: isValid,
      toDto: toDto
    });
  }

  return this;
}

ko.utils.extend(FacilityMaintenanceEditorVM.prototype, {
  dispose: function() {
    ko.utils.arrayForEach( this.disposables, this.disposeOne );
    ko.utils.objectForEach( this, this.disposeOne );
  },

  disposeOne: function( propOrValue, value ) {
    var disposable = value || propOrValue;

    if ( disposable && typeof disposable.dispose === 'function' ) {
      disposable.dispose();
    }
  }
});

module.exports = {
  viewModel: FacilityMaintenanceEditorVM,
  template: require('./facility-maintenance-editor.html')
};
