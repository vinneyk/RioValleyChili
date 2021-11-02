function FacilityItem( data ) {
  this.FacilityName = data.FacilityName;
  this.ShippingLabel = data.ShippingLabel;

  this.FacilityKey = data.FacilityKey;
  this.Active = data.Active;
  this.COName = data.ShippingLabel.Name;
  this.PhoneNumber = data.ShippingLabel.PhoneNumber;
  this.Email = data.ShippingLabel.Email;

  var _addr = data.ShippingLabel.Address;
  var _addrItems = [];
  _addr.AddressLine1 != null && _addrItems.push( _addr.AddressLine1 );
  _addr.AddressLine2 != null && _addrItems.push( _addr.AddressLine2 );
  _addr.AddressLine3 != null && _addrItems.push( _addr.AddressLine3 );
  _addr.City != null && _addrItems.push( _addr.City );
  _addr.State != null && _addrItems.push( _addr.State );
  _addr.PostalCode != null && _addrItems.push( _addr.PostalCode );

  this.FullAddress = _addrItems.join(', ');
}

/** Facility Maintenance summaries
  * @param {Object} input - Observable, input data for summary table
  * @param {Object} selected - Observable, container for the selected facility
  * @param {Object} exports - Observable, Public methods
  */
function FacilityMaintenanceSummaryVM( params ) {
  if ( !(this instanceof FacilityMaintenanceSummaryVM) ) { return new FacilityMaintenanceSummaryVM( params ); }

  var self = this;

  this.facilities = ko.pureComputed(function() {
    var _facilities = ko.unwrap( params.input ) || [];

    return _facilities.map(function( facility ) {
      return new FacilityItem( facility );
    });
  });

  this.selectedFacility = params.selected;
  this.selectFacility = function( data ) {
    if ( data === self.selectedFacility() ) {
      self.selectedFacility( null );
      return;
    }

    self.selectedFacility( data );
  };

  function updateFacility( facilityData ) {
    var _facilityKey = facilityData.FacilityKey;
    var _facilities = self.facilities();

    var _facility = ko.utils.arrayFirst( _facilities, function( facility ) {
      return facility.FacilityKey === _facilityKey;
    });

    if ( _facility ) {
      var i = _facilities.indexOf( _facility );

      if ( i > -1 ) {
        self.facilities.splice( i, 1, new FacilityItem( facilityData ) );

        return $.Deferred().resolve();
      }
    }

    return addFacility( facilityData );
  }

  function addFacility( facilityData ) {
    self.facilities.unshift( new FacilityItem( facilityData ) );

    return $.Deferred().resolve();
  }

  // Exports
  if ( params && params.exports ) {
    params.exports({
      updateFacility: updateFacility,
      addFacility: addFacility
    });
  }

  return this;
}

module.exports = {
  viewModel: FacilityMaintenanceSummaryVM,
  template: require('./facility-maintenance-summary.html')
};
