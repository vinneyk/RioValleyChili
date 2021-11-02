var directoryService = require('App/services/directoryService');

/**
  * @param {string} companyKey - Target company key
  * @param {Object} options - Contact data w/ addresses for display
  * @param {Object} selected - Observable, container for selected address
  */
function AddressBookVM( params ) {
  if ( !(this instanceof AddressBookVM) ) {
    return new AddressBookVM( params );
  }

  var self = this;

  // Data
  var selectedContact = params.selected || ko.observable( null );
  var companyKey = params.companyKey;

  this.contacts = ko.observableArray( [] );
  if ( ko.isObservable( params.options ) ) {
    params.options.subscribe(function( opts ) {
      self.contacts( mapContacts( opts ) );
    });
  }

  function mapContacts( contacts ) {
    var _contacts = ko.unwrap( contacts );

    return ko.utils.arrayMap( _contacts, function( contact ) {
      ko.utils.arrayForEach( contact.Addresses, function( addr ) {
        addr.ContactKey = contact.ContactKey;
        addr.CompanyKey = contact.CompanyKey;
        addr.Name = contact.Name;
        addr.Phone = contact.PhoneNumber || contact.Phone;
        addr.Fax = contact.Fax;
        addr.EMail = contact.EMailAddress || contact.EMail;
        addr.Address.CityStatePost = "".concat( addr.Address.City, ', ', addr.Address.State, ' ', addr.Address.PostalCode );
        addr.isSelected = ko.pureComputed(function() {
          var selected = selectedContact();

          return addr.Address === (selected && selected.Address);
        });
      });

      return contact;
    });
  }

  // Behaviors
  this.select = function( data, element ) {
    if ( data.hasOwnProperty('Address') ) {
      var contactData = data;

      selectedContact( contactData );
    }
  };

  this.contacts( mapContacts( params.options ) );

  // Contact editing
  function Address( addr ) {
    var _address = addr.Address || {};

    this.ContactAddressKey = addr.ContactAddressKey;
    this.AddressDescription = ko.observable( addr.AddressDescription );
    this.Address = {
      AddressLine1: ko.observable( _address.AddressLine1 ),
      AddressLine2: ko.observable( _address.AddressLine2 ),
      AddressLine3: ko.observable( _address.AddressLine3 ),
      City: ko.observable( _address.City ),
      State: ko.observable( _address.State ),
      Country: ko.observable( _address.Country ),
      PostalCode: ko.observable( _address.PostalCode ),
    };

    this.Address.CityStatePost = ko.pureComputed(function() {
      return '' + this.Address.City() + ', ' + this.Address.State() + ' ' + this.Address.PostalCode();

    }, this);
  }

  function ContactInfo( contact ) {
    this.isNew = !contact.ContactKey;

    this.ContactKey = contact.ContactKey;
    this.CompanyKey = companyKey;
    this.Name = ko.observable( contact.Name );
    this.Phone = ko.observable( contact.Phone );
    this.EMail = ko.observable( contact.EMail );

    this.selectedAddress = ko.observable();

    var mapAddr = function( addr ) {
      var mappedAddr = new Address( addr );
      mappedAddr.isSelected = ko.pureComputed(function() {
        var selected = this.selectedAddress();

        return mappedAddr === selected;
      }, this);

      return mappedAddr;
    }.bind( this );

    var _contactAddresses = ko.utils.arrayFirst( self.contacts(), function( contact ) {
      return contact.ContactKey === this.ContactKey;
    }, this);
    var _addressesCache = JSON.parse( ko.toJSON( _contactAddresses && _contactAddresses.Addresses ) );
    var _addresses = ko.utils.arrayMap( _addressesCache, mapAddr);

    this.Addresses = ko.observableArray( _addresses );

    this.selectAddress = function( data, element ) {
      if ( this.selectedAddress() === data ) {
        this.selectedAddress( null );
      } else {
        this.selectedAddress( data );
      }
    }.bind( this );

    if ( contact.selectedAddress ) {
      var _initalAddress = ko.utils.arrayFirst( this.Addresses(), function( addr ) {
        return addr.ContactAddressKey === contact.selectedAddress;
      });

      this.selectedAddress( _initalAddress );
    }

    this.addAddress = function() {
      var _newAddr = mapAddr( {} );

      this.Addresses.push( _newAddr );
      this.selectedAddress( _newAddr );
    }.bind( this );
  }

  ContactInfo.prototype.toDto = function() {
    var _data = {
      Name: this.Name,
      PhoneNumber: this.Phone,
      EmailAddress: this.EMail,
      Addresses: this.Addresses,
      ContactKey: this.ContactKey
    };

    var _addr = ko.toJS( this.Address );
    _data.CompanyKey = companyKey;

    return ko.toJS( _data );
  };

  this.editorData = ko.observable( null );

  this.startNewContact = function Name( Parameters ) {
    self.editorData( new ContactInfo({
      CompanyKey: companyKey
    }) );
  };

  this.editContact = function( contact, element ) {
    var _parentData = ko.toJS( ko.contextFor( element.target ).$parent );

    var _contactData = {};
    _contactData.CompanyKey = companyKey;
    _contactData.ContactKey = _parentData.ContactKey;
    _contactData.Name = _parentData.Name;
    _contactData.Phone = _parentData.Phone;
    _contactData.EMail = _parentData.EMail;
    _contactData.Addresses = _parentData.Addresses;
    _contactData.selectedAddress = _parentData.ContactAddressKey;

    self.editorData( new ContactInfo( _contactData ) );
  };

  function createContact( companyKey, contactData ) {
    return directoryService.createContact( companyKey, contactData ).then(
    function( data, textStatus, jqXHR ) {
      return data;
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not save new contact', {
        description: errorThrown
      });
    });
  }

  function updateContact( contactKey, contactData ) {
    return directoryService.updateContact( contactKey, contactData ).then(
    function( data, textStatus, jqXHR ) {
      return data;
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not update contact info', {
        description: errorThrown
      });
    });
  }

  function deleteContact( contactKey ) {
    return directoryService.deleteContact( contactKey ).then(
    function( data, textStatus, jqXHR ) {
      return data;
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not remove contact', {
        description: errorThrown
      });
    });
  }

  this.saveContact = ko.asyncCommand({
    execute: function( complete ) {
      var _editor = self.editorData();
      var _data = _editor.toDto();
      var _contactKey = _data.ContactKey;
      var _companyKey = _data.CompanyKey;

      var save;
      if ( _editor.isNew ) {
        save = createContact( _companyKey, _data ).then( null );
      } else {
        save = updateContact( _contactKey, _data ).then( null );
      }

      var getContacts = save.then(
      function( data, textStatus, jqXHR ) {
        return directoryService.getContacts( _companyKey ).then(
        function( data, textStatus, jqXHR ) {
          self.contacts( mapContacts( data ) );

          // Close editor
          self.editorData( null );
        });
      }).always( complete );

      return getContacts;
    }
  });

  this.removeContact = ko.asyncCommand({
    execute: function( complete ) {
      var _data = self.editorData();
      var _contactKey = _data.ContactKey;

      showUserMessage( 'Delete contact address?', {
        description: 'Are you sure you want to remove this address? This action cannot be undone.',
        type: 'yesno',
        onYesClick: function() {
          var remove = deleteContact( _contactKey ).then( null );

          var getContacts = remove.then(
            function( data, textStatus, jqXHR ) {
            return directoryService.getContacts( _companyKey ).then(
              function( data, textStatus, jqXHR ) {
              self.contacts( data );

              // Close editor
              self.editorData( null );
            });
          }).always( complete );
        },
        onNoClick: function() { complete(); }
      });
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  this.cancelEdit = function() {
    self.editorData( null );
  };

  return this;
}

module.exports = {
  viewModel: AddressBookVM,
  template: require('./contact-picker.html')
};
