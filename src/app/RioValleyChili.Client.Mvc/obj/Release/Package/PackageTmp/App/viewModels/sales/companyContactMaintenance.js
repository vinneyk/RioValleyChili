/** Required ko components */
ko.components.register( 'customer-maintenance-summary', require('App/components/sales/customer-maintenance-summary/customer-maintenance-summary') );
ko.components.register( 'customer-maintenance-editor', require('App/components/sales/customer-maintenance-editor/customer-maintenance-editor') );
ko.components.register( 'product-selector', require('App/components/common/product-selector/product-selector') );

/** Essential libraries */
var directoryService = require('App/services/directoryService');
var rvc = require('rvc');
var page = require('page');

require('bootstrap');
require('App/koExtensions');
require('App/helpers/koValidationHelpers');

/** Company and Contact Maintenance
  */
function CompanyContactMaintenance() {
  if ( !(this instanceof CompanyContactMaintenance) ) { return new CompanyContactMaintenance( params ); }

  var self = this;

  this.isInit = ko.observable( false );

  // Data
  this.filterBrokerKey = ko.observable();
  this.filters = {
    companyType: ko.observable(),
    filterBrokerKey: ko.pureComputed(function() {
      var _broker = self.filterBrokerKey();
      return _broker && _broker.CompanyKey;
    }),
    includeInactive: true
  };

  this.companies = ko.observableArray( [] );
  var companiesPager = directoryService.getCompaniesDataPager({
    onNewPageSet: function() {
      self.companies( [] );
    }
  });
  companiesPager.addParameters( this.filters );
  this.loadMore = ko.asyncCommand({
    execute: function( complete ) {
      companiesPager.nextPage().then(
      function( data, textStatus, jqXHR ) {
        ko.utils.arrayPushAll( self.companies(), data );
        self.companies.valueHasMutated();
      },
      function( jqXHR, textStatus, errorThrown ) {
        // Fail
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  this.options = {
    companyTypes: [],
    brokers: ko.observableArray( [] ),
    companies: ko.observableArray( [] ),
    noteTypes: ko.observableArray( [] )
  };

  this.isCustomer = ko.computed(function() {
    if ( self.filters.companyType() === 0 ) {
      return true;
    } else {
      self.filterBrokerKey( null );
      return false;
    }
  });

  ko.utils.arrayForEach( rvc.lists.companyTypes.buildSelectListOptions(), function( opt ) {
    self.options.companyTypes.push( opt.value );
  });

  this.selectedCompany = ko.observable();
  this.companyName = ko.pureComputed(function() {
    var _company = self.selectedCompany();

    return _company && _company.Name;
  });
  this.summaryData = {
    input: this.companies,
    selected: this.selectedCompany,
    exports: ko.observable()
  };

  this.selectedCompany.subscribe(function( company ) {
    if ( company && company.CompanyKey ) {
      page( '/' + company.CompanyKey );
    }
  });

  this.editorData = {
    input: ko.observable(),
    exports: ko.observable()
  };

  this.startNewCompany = ko.command({
    execute: function() {
      page('/new');
    },
    canExecute: function() {
      return true;
    }
  });

  this.saveCompanyCommand = ko.asyncCommand({
    execute: function( complete ) {
      var _editor = self.editorData.exports();

      // Stop save if data is invalid
      if ( !_editor.isValid() ) {
        showUserMessage( 'Could not save company data', {
          description: 'Please ensure all fields have been entered correctly.'
        });
        complete();
        return;
      }

      var _companyData = _editor.toDto();

      // Call API to create/update company
      var saveCompany;
      if ( !_companyData.CompanyKey ) {
        saveCompany = directoryService.createCompany( _companyData ).then(
        function( data, textStatus, jqXHR ) {
          self.summaryData.exports().addCompany( data );
          self.options.companies.push( data );

          page( '/' + data.CompanyKey );
        });
      } else {
        saveCompany = directoryService.updateCompany( _companyData.CompanyKey, _companyData ).then(
        function( data, textStatus, jqXHR ) {
          self.summaryData.exports().updateCompany( _companyData.CompanyKey, data );

          // Close editor view on success
          page('/');
        });
      }

      saveCompany.then(
      function( data, textStatus, jqXHR ) {
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not save company data', {
          description: errorThrown
        });
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  this.closeEditorCommand = ko.command({
    execute: function() {
      page('/');
    }
  });

  (function init() {
    var getCompanies = directoryService.getCompanies().then(
    function( data, textStatus, jqXHR ) {
      self.options.companies( data );
    });

    var getBrokers = directoryService.getBrokers().then(
    function( data, textStatus, jqXHR ) {
      self.options.brokers( data );
    });

    var getNoteTypes = directoryService.getNoteTypes().then(
    function( data, textStatus, jqXHR ) {
      self.options.noteTypes( data );
    });

    var checkOpts = $.when( getCompanies, getBrokers, getNoteTypes ).then(
    function( data, textStatus, jqXHR ) {
      self.isInit( true );
      page();
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not load company maintenance settings', {
        description: errorThrown
      });
    });
  })();

  page.base('/Customers/CompanyMaintenance');

  function loadCompanyDetails( ctx, next ) {
    var _key = ctx.params.companyKey;
    if ( _key === 'new' ) {
      self.editorData.input( {} );
    } else if ( _key ) {
      var getDetails = directoryService.getCompanyData( _key ).then(
      function( data, textStatus, jqXHR ) {
        self.editorData.input( data );
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not find company' );
        page.redirect( '/' );
      });
    } else {
      next();
    }
  }
  page( '/:companyKey', loadCompanyDetails );

  function goToSummaryView( ctx, next ) {
    self.selectedCompany( null );
    self.editorData.input( null );
  }
  page( goToSummaryView );

  // Exports
  return this;
}

var vm = new CompanyContactMaintenance();

ko.applyBindings( vm );

module.exports = vm;

