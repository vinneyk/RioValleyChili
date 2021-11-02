var directoryService = require('App/services/directoryService');

ko.components.register( 'contact-picker', require('App/components/common/address-book/contact-picker') );

/**
  * @param {boolean} visible - Observable, toggle visibility
  * @param {string} key - Company key to fetch contacts for
  * @param {Object[]} companies - Array of companies for display and selection
  * @param {Object[]} buttons - Buttons to display on picker UI
  * @param {Function} buttons.callback - Callback function to call after successful selection
  * @param {string} buttons.text - Button label text
  */
function AddressBookControllerVM( params ) {
  if ( !(this instanceof AddressBookControllerVM) ) { return new AddressBookControllerVM( params ); }

  var self = this;

  // Constructors
  function Button( opts ) {
    this.text = opts.text;
    this.callback = function () {
      var response = self.inputData.selected();
      response.company = self.company();
      opts.callback( response );
    };
  }

  this.disposables = [];

  // Data
  this.isVisible = ko.isObservable( params.visible ) && params.visible.extend({ notify: 'always' });
  this.isPicking = ko.observable( false );
  this.isLoading = ko.observable( false );

  this.companies = params.companies || ko.observableArray( [] );
  this.company = ko.observable();
  this.companyKey = ko.pureComputed(function() {
    var c = self.company();
    return c && ko.unwrap( c.CompanyKey );
  });

  this.inputData = {
    opts: ko.observableArray( [] ),
    companyKey: this.companyKey,
    selected: ko.observable( null ).extend({ notify: 'always' })
  };

  if ( ko.isObservable( params.key )) {
    this.disposables.push(params.key.subscribe(function(val) {
      setSelectedCompanyByKey(val);
    }));
  }
  if (ko.unwrap(params.key) != null) {
    setTimeout(function() {
      setSelectedCompanyByKey(ko.unwrap(params.key));
    }, 0);
  }
  this.companyKey.subscribe( function( newKey ) {
    if ( newKey != null && self.isPicking() ) {
      // Search for company data
      getContactsById( newKey );
    }
  });

  this.buttons = ko.observableArray( ko.utils.arrayMap( params.buttons, function( btn ) {
    return new Button( btn);
  } ) );

  // Behaviors
  function setSelectedCompanyByKey(keyValue) {
    keyValue = ko.unwrap(keyValue);
    var selected = ko.utils.arrayFirst(self.companies(), function (c) { return c.CompanyKey === keyValue; });
    self.company(selected);
  }
  function hideUI() {
    self.isPicking( false );
    self.isLoading( false );
    self.inputData.selected( null );
    setSelectedCompanyByKey( ko.unwrap(params.key) );
  }

  this.toggleUI = ko.asyncCommand({
    execute: function( complete ) {
      var getContacts = getContactsById( ko.unwrap( self.companyKey )).then(
        function( data, textStatus, jqXHR ) {
          self.isPicking( true );
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  this.cancel = function() {
    self.inputData.selected( null );
    self.isPicking( false );
  };

  function getContactsById( id ) {
    var getContacts = directoryService.getContacts( id ).then(
    function( data, textStatus, jqXHR ) {
      self.inputData.opts( data );
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not get contacts', { description: errorThrown } );
    });

    return getContacts;
  }

  var visibleSub = this.isVisible.subscribe(function( bool ) {
    if ( bool ) {
      self.isLoading( true );

      var _companyKey = ko.unwrap( self.companyKey );
      if ( _companyKey ) {
        var getContacts = getContactsById( _companyKey ).then(
        function( data, textStatus, jqXHR ) {
          self.isPicking( true );
        },
        function( jqXHR, textStatus, errorThrown ) {
          self.isPicking( false );
          showUserMessage( errorThrown );
        }).always(function() {
          self.isLoading( false );
        });
      } else {
        self.isPicking( true );
      }
    } else {
      hideUI();
    }
  });

  this.disposables.push( visibleSub );

  return this;
}

ko.utils.extend( AddressBookControllerVM.prototype, {
  dispose: function() {
    ko.utils.arrayForEach( this.disposables, this.disposeOne );
    ko.utils.objectForEach( this, this.disposeOne );
  },

  // little helper that handles being given a value or prop + value
  disposeOne: function( propOrValue, value ) {
    var disposable = value || propOrValue;

    if ( disposable && typeof disposable.dispose === "function" ) {
      disposable.dispose();
    }
  }
} );

module.exports = {
  viewModel: AddressBookControllerVM,
  template: require('./company-address-label-helper.html')
};
