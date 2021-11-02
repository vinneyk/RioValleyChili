/** Required Libraries */

var lotService = require('App/services/lotService');
var qualityControlService = require('App/services/qualityControlService');
var directoryService = require('App/services/directoryService');

/** Required components */
ko.components.register( 'contact-label-editor', require('App/components/warehouse/contact-label-editor/contact-label-editor'));
ko.components.register( 'contact-address-label-helper', require('App/components/common/address-book/company-address-label-helper') );

/** Journal entry item constructor */
function JournalEntry( journal ) {
  this.JournalEntryKey = journal.JournalEntryKey;
  this.CreatedByUser = journal.CreatedByUser || {};

  this.Date = ko.observableDate( journal.Date ).extend({ required: true });
  this.Text = ko.observable( journal.Text ).extend({ required: true });

  this.validation = ko.validatedObservable({
    Date: this.Date,
    Text: this.Text
  });
}

function JournalEntryEditor( sampleKey, journalEntries ) {
  var _entries = ko.utils.arrayMap( journalEntries, function( entry ) {
    return new JournalEntry( entry );
  });

  this.JournalEntries = ko.observableArray( _entries );
  this.newEntry = ko.observable();
  this.isNew = ko.pureComputed(function() {
    var entry = this.newEntry();

    return entry && entry.JournalEntryKey == null;
  }, this);

  this.sampleKey = sampleKey;

  this.selectJournal = function( data, element ) {
    // Check for nearby tr element
    var $tr = $( element.target ).closest('.journal-entry-row')[0];

    // If tr is found
    if ( $tr ) {
      // Get context for element
      var journal = ko.toJS( ko.contextFor( $tr ).$data );

      // Build journal editor view
      this.newEntry( new JournalEntry( journal ) );
    }
  }.bind( this );

  this.startNewEntry = ko.command({
    execute: function() {
      this.newEntry( new JournalEntry( {} ) );
    }.bind( this )
  });

  var _journals = this.JournalEntries;
  var _selected = this.newEntry;
  this.createEntry = ko.asyncCommand({
    execute: function( complete ) {
      var _journal = this.newEntry();

      if ( _journal.validation.isValid() ) {
        var _sampleKey = this.sampleKey;

        var createJournalEntry = qualityControlService.createJournalEntry( _sampleKey, _journal ).then(
        function( data ) {
          // Add entry to table
          _journals.splice( 0, 0, new JournalEntry( data ) );

          _selected( new JournalEntry( data ) );

          showUserMessage( 'Journal entry created successfully', {
            description: ''
          });
        },
        function( jqXHR, textStatus, errorThrown ) {
          showUserMessage( 'Could not create journal entry', {
            description: errorThrown
          });
        }).always( complete );

        return createJournalEntry;
      } else {
        showUserMessage( 'Could not create journal entry', {
          description: 'Please ensure all fields have been filled out correctly.'
        });
        complete();
      }
    }.bind( this ),
    canExecute: function( isExecuting ) {
      var entry = this.newEntry();
      return !isExecuting && entry && entry.validation.isValid();
    }.bind( this )
  });

  this.updateEntry = ko.asyncCommand({
    execute: function( complete ) {
      var _journal = this.newEntry();

      if ( _journal.validation.isValid() ) {
        var _sampleKey = this.sampleKey;
        var _journalKey = _journal.JournalEntryKey;

        var createJournalEntry = qualityControlService.updateJournalEntry( _sampleKey, _journalKey, _journal ).then(
        function( data ) {
          // Update entry in table
          var _matchedJournal = ko.utils.arrayFirst( _journals(), function( journal ) {
            return journal.JournalEntryKey === _journalKey;
          });
          var _journalIndex = _journals().indexOf( _matchedJournal );

          _journals.splice( _journalIndex, 1, new JournalEntry( data ) );

          showUserMessage( 'Journal entry created successfully', {
            description: ''
          });
        },
        function( jqXHR, textStatus, errorThrown ) {
          showUserMessage( 'Could not create journal entry', {
            description: errorThrown
          });
        }).always( complete );

        return createJournalEntry;
      } else {
        showUserMessage( 'Could not create journal entry', {
          description: 'Please ensure all fields have been filled out correctly.'
        });
        complete();
      }
    }.bind( this ),
    canExecute: function( isExecuting ) {
      var entry = this.newEntry();
      return !isExecuting && entry && entry.validation.isValid();
    }.bind( this )
  });

  this.cancelEntry = ko.command({
    execute: function() {
      this.newEntry( null );
    }.bind( this ),
    canExecute: function() {
      return true;
    }
  });

  this.removeEntry = function( data ) {
    var _journalKey = data.JournalEntryKey;

    var removeJournalEntry = qualityControlService.deleteJournalEntry( this.sampleKey, data.JournalEntryKey ).then(
    function() {
      var _matchedJournal = ko.utils.arrayFirst( _journals(), function( journal ) {
        return journal.JournalEntryKey === _journalKey;
      });
      var _journalIndex = _journals().indexOf( _matchedJournal );

      _journals.splice( _journalIndex, 1 );

      showUserMessage( 'Journal removed successfully', {
        description: ''
      });
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not remove journal entry', {
        description: errorThrown
      });
    });

    return removeJournalEntry;
  }.bind( this );
}

/** Customer Specs Modal model */
function CustomerSpecAttribute( attrData ) {
  this.Key = attrData.Key;
  this.Name = attrData.Name;
  this.RangeMin = ko.observable( attrData.Min );
  this.RangeMax = ko.observable( attrData.Max );
}

function CustomerSpecsModel( item, specData ) {
  var _spec = ko.unwrap( specData ) || {};

  this.Item = item;
  this.ItemKey = item.ItemKey;
  this.Product = _spec.Product;

  this.Mesh = ko.observable( _spec.Mesh );
  this.AoverB = ko.observable( _spec.AoverB );
  this.Notes = ko.observable( _spec.Notes );

  var _attrs = [
    {
      Key: 'Asta',
      Name: 'Asta'
    },
    {
      Key: 'Moisture',
      Name: 'Moisture'
    },
    {
      Key: 'WaterActivity',
      Name: 'H2O Activity'
    },
    {
      Key: 'Scov',
      Name: 'SHU'
    },
    {
      Key: 'Scan',
      Name: 'Surface Color'
    },
    {
      Key: 'TPC',
      Name: 'TPC'
    },
    {
      Key: 'Yeast',
      Name: 'Yeast'
    },
    {
      Key: 'Mold',
      Name: 'Mold'
    },
    {
      Key: 'Coliforms',
      Name: 'Coliforms'
    },
    {
      Key: 'EColi',
      Name: 'E. Coli.'
    },
    {
      Key: 'Sal',
      Name: 'Salmonella'
    },
  ];
  this.Attributes = ko.utils.arrayMap( _attrs, function( attr ) {
    var _data = {
      Key: attr.Key,
      Name: attr.Name,
      Min: _spec[ attr.Key + 'Min' ],
      Max: _spec[ attr.Key + 'Max' ],
    };

    return new CustomerSpecAttribute( _data );
  });
}

CustomerSpecsModel.prototype.toDto = function() {
  var data = {};

  data.Mesh = this.Mesh;
  data.AoverB = this.AoverB;
  data.Notes = this.Notes;

  ko.utils.arrayForEach( this.Attributes, function( attr ) {
    data[ attr.Key + 'Min' ] = attr.RangeMin();
    data[ attr.Key + 'Max' ] = attr.RangeMax();
  });

  return ko.toJS( data );
};

/** Lab Results Modal model */
function LabResultAttribute( attrData ) {
  this.Key = attrData.Key;
  this.Name = attrData.Name;
  this.Value = ko.observable( attrData.Value );
}
function LabResultsModel( item, labData ) {
  var _labData = ko.unwrap( labData ) || {};

  this.Item = item;
  this.ItemKey = item.ItemKey;
  this.Product = _labData.Product;

  this.AoverB = ko.observable( _labData.AoverB );
  this.Notes = ko.observable( _labData.Notes );

  var _attrs = [
    {
      Key: 'Gran',
      Name: 'Gran'
    },
    {
      Key: 'AvgAsta',
      Name: 'Avg. Asta'
    },
    {
      Key: 'AvgScov',
      Name: 'Scov Avg.'
    },
    {
      Key: 'H2O',
      Name: 'H2O'
    },
    {
      Key: 'Scan',
      Name: 'Scan'
    },
    {
      Key: 'Yeast',
      Name: 'Yeast'
    },
    {
      Key: 'Mold',
      Name: 'Mold'
    },
    {
      Key: 'Coli',
      Name: 'Coliforms'
    },
    {
      Key: 'TPC',
      Name: 'TPC'
    },
    {
      Key: 'EColi',
      Name: 'E. Coli.'
    },
    {
      Key: 'Sal',
      Name: 'Salmonella'
    },
    {
      Key: 'InsPrts',
      Name: 'Insect Parts'
    },
    {
      Key: 'RodHrs',
      Name: 'Rodent Hairs'
    },
  ];
  this.Attributes = ko.utils.arrayMap( _attrs, function( attr ) {
    var _data = {
      Key: attr.Key,
      Name: attr.Name,
      Value: _labData[ attr.Key ]
    };

    return new LabResultAttribute( _data );
  });
}

LabResultsModel.prototype.toDto = function() {
  var data = {};

  data.AoverB = this.AoverB;
  data.Notes = this.Notes;

  ko.utils.arrayForEach( this.Attributes, function( attr ) {
    data[ attr.Key ] = attr.Value();
  });

  return ko.toJS( data );
};


/** Sample Details item constructor */
function SampleDetailItem( detailItem, options ) {
  this.ProductType = ko.observable( detailItem.ProductType != null ? '' + detailItem.ProductType : '7' ).extend({ productType: true });
  this.ProductTypeName = ko.pureComputed(function() {
    var _type = this.ProductType();
    var _opts = this.ProductType.options;
    var _opt = ko.utils.arrayFirst( _opts, function( opt ) {
      return opt.key === _type;
    });

    return _opt.value;
  }, this );

  var _productOptions = options.products;
  this.productOptions = ko.pureComputed(function() {
    var _opts = _productOptions();
    var _productType = +this.ProductType();

    return ko.unwrap( _opts[ _productType ] );
  }, this );

  var _product = ko.utils.arrayFirst( this.productOptions(), function( opt ) {
    return opt.ProductKey === detailItem.ProductKey;
  });
  this.ProductKey = ko.observable( _product );
  this.ProductType.subscribe(function() {
    this.ProductKey( null );
  }, this );

  this.ItemKey = detailItem.ItemKey;
  this.CustomerProductName = ko.observable( detailItem.CustomerProductName );
  this.Quantity = ko.observable( detailItem.Quantity ).extend({ min: 0, required: true });
  this.Description = ko.observable( detailItem.Description );

  this.CustomerSpec = ko.observable( detailItem.CustomerSpec );
  this.LabResults = ko.observable( detailItem.LabResults );

  this.reportLink = detailItem.Links && detailItem.Links[ 'report-sample_match_summary' ].HRef;

  this.isLotValid = ko.observable( true );
  this.loadLotDetails = ko.asyncCommand({
    execute: function( lotKey, complete ) {
      var _lotValid = this.isLotValid;
      var _product = this.ProductKey;
      var _productType = this.ProductType;

      var getDetails = this.getLotDetails( lotKey ).then(
      function( data ) {
        // Lock product selector on success
        _lotValid( true );

        // Set product selector to product return for lot key
        _productType( data.LotSummary.Product.ProductType );
        _product( data.LotSummary.Product.ProductKey );
      }, function() {
        _lotValid( false );
        showUserMessage( 'Lot not found', { description: 'The lot <strong>' + lotKey + '</strong> was not found. Please check the lot number for errors.' });
      }).always( complete );

      return getDetails;
    }.bind( this )
  });
  this.LotKey = ko.observable( detailItem.LotKey || '' ).extend({
    lotKey: this.loadLotDetails.execute,
    validation: {
      validator: function( value ) {
        return value == null || value == '' || this.isLotValid();
      }.bind( this ),
      message: 'Lot key must be valid'
    }
  });
  this.LotKey.subscribe(function() {
    this.isLotValid( false );
  }, this);

  this.enableProductSelector = ko.pureComputed(function() {
    var lotKey = this.LotKey();
    return lotKey == null || lotKey == '';
  }, this);
}

SampleDetailItem.prototype.getLotDetails = function( lotKey ) {
  // Get details for lot key
  return lotService.getLotData( lotKey ).then(
  function( data ) {
    return data;
  },
  function() {
    // Notify user of error on failure
    showUserMessage( 'Could not find lot details', {
      description: 'There were no details found for the lot <b>' + lotKey + '</b>'
    });
  });
};

SampleDetailItem.prototype.toDto = function() {
  return {
    Quantity: this.Quantity,
    Description: this.Description,
    CustomerProductName: this.CustomerProductName,
    ProductKey: this.ProductKey().ProductKey,
    LotKey: this.LotKey,
  };
};

/** Sample Matching editor constructor */
function SampleMatchingEditor( sampleData, options, callbacks ) {
  var self = this;

  this.options = options;
  this.SampleRequestKey = sampleData.SampleRequestKey;

  // Header
  this.CreatedByUser = sampleData.CreatedByUser;
  this.isNew = this.SampleRequestKey == null;
  this.isPickingContact = ko.observable( false );

  // Sample Information tab
  this.Active = ko.observable( this.isNew ? true : sampleData.Active );
  this.Status = ko.observable( sampleData.Status || '0' );
  this.FOB = ko.observable( sampleData.FOB );
  this.ShipVia = ko.observable( sampleData.ShipVia );
  this.Broker = ko.observable( sampleData.Broker && sampleData.Broker.CompanyKey );
  this.DateDue = ko.observableDate( sampleData.DateDue );
  this.DateReceived = ko.observableDate( sampleData.DateReceived );
  this.DateCompleted = ko.observableDate( sampleData.DateCompleted );
  this.Volume = ko.observable( sampleData.Volume );
  this.NotesToPrint = ko.observable( sampleData.NotesToPrint );
  this.Comments = ko.observable( sampleData.Comments );
  this.Links = sampleData.Links;

  // Sample Information contacts
  var _requestedByCompany = ko.observable();
  this.RequestedByCompany = ko.computed({
    read: function() {
      return _requestedByCompany();
    },
    write: function (value) {
      if (value == null) {
        _requestedByCompany( null );
        return;
      }
      var key = value.CompanyKey || value || null;
      if (self.options.companies().length) {
        _requestedByCompany(findCompanyByKey(key));
      } else {
        var _companiesSub = self.options.companies.subscribe(function(companiesValue) {
          if (companiesValue.length) {
            _requestedByCompany(findCompanyByKey(key));
          }
          _companiesSub.dispose();
        });
      }
    }
  });
  this.RequestedByCompany(sampleData.RequestedByCompany);

  function findCompanyByKey(key) {
    return ko.utils.arrayFirst(self.options.companies(), function(c) {
      return c.CompanyKey === key;
    });
  }
  this.RequestedByCompanyName = ko.pureComputed(function() {
    var selectedCompany = self.RequestedByCompany() || { Name: null };
    return selectedCompany.Name;
  });
  this.RequestedByCompanyKey = ko.pureComputed(function() {
    var selectedCompany = self.RequestedByCompany() || { CompanyKey: null };
    return selectedCompany.CompanyKey;
  });
  this.RequestedByShippingLabelInput = ko.observable(sampleData.RequestedByShippingLabel);
  this.RequestedByShippingLabel = ko.observable();

  this.isLoadingCustomer = ko.observable( false );

  self.RequestedByCompanyKey.subscribe(function( key ) {
    if ( key ) {
      self.isLoadingCustomer( true );
      var getCompanyDetails = directoryService.getCompanyData( key ).then(
      function( data ) {
        var _broker = data.CustomerResponse && data.CustomerResponse.Broker.CompanyKey;
        self.Broker( _broker );
      }).always(function() {
        self.isLoadingCustomer( false );
      });

      return getCompanyDetails;
    }
  });

  var setRequestedByContactInfo = function (selectedContact) {
    this.RequestedByShippingLabelInput(selectedContact);

  }.bind( this );

  this.ShipToCompany = ko.observable(sampleData.ShipToCompany);
  this.ShipToShippingLabelInput = ko.observable(sampleData.ShipToShippingLabel);
  this.ShipToShippingLabel = ko.observable();

  this.reportAll = sampleData.Links && sampleData.Links[ 'report-sample_match_summary' ].HRef;

  var setShipToContactInfo = function (selectedContact) {
    selectedContact = selectedContact || { company: null };
    this.ShipToCompany( selectedContact.company.Name );
    this.ShipToShippingLabelInput( selectedContact );
  }.bind( this );

  this.pickerButtons = [
    {
      callback: setRequestedByContactInfo,
      text: 'Requested By'
    },
    {
      callback: setShipToContactInfo,
      text: 'Sold To'
    }
  ];

  this.showContactPicker = ko.command({
    execute: function() {
      this.isPickingContact( true );
    }.bind( this ),
    canExecute: function() {
      return this.RequestedByCompany() != null;
    }.bind( this )
  });

  // Sample Details tab
  var _detailItems = ko.utils.arrayMap( sampleData.Items, function( item ) {
    return new SampleDetailItem( item, self.options );
  });
  this.SampleDetails = ko.observableArray( _detailItems );
  this.addNewSampleDetails = ko.command({
    execute: function() {
      var _details = self.SampleDetails();
      var lastItem = ko.toJS( _details[ _details.length - 1 ] ) || {};

      this.SampleDetails.push( new SampleDetailItem( {
        Quantity: lastItem.Quantity,
        Description: lastItem.Description
      }, self.options ));
    }.bind( this ),
    canExecute: function() {
      return true;
    }
  });
  this.removeSampleDetails = ko.command({
    execute: function( data ) {
      var detailsIndex = this.SampleDetails().indexOf( data );

      this.SampleDetails.splice( detailsIndex, 1 );
    }.bind( this ),
    canExecute: function() {
      return true;
    }
  });

  // Editor modal
  var $modal = $('#specModal');
  this.modalTemplate = ko.observable( null );
  this.modalData = ko.observable();

  var onModalClose = function() {
    this.modalTemplate( null );
    this.modalData( null );
    $modal.off( 'hidden.bs.modal', onModalClose );
  }.bind( this );

  // Customer specs modal
  this.showCustomerSpecsModal = ko.command({
    execute: function (data) {
      if (data.ItemKey == null) {
        showUserMessage("Customer Specs cannot be added for this item until it has been saved.");
        return;
      }

      this.modalData( new CustomerSpecsModel( data, data.CustomerSpec ) );
      this.modalTemplate('customer-product-specs');

      $modal.modal('show');
      $modal.on( 'hidden.bs.modal', onModalClose );
    }.bind( this ),
    canExecute: function() {
      return !this.isNew;
    }.bind( this )
  });
  this.saveCustomerSpecsCommand = ko.asyncCommand({
    execute: function( item, customerSpecs, complete ) {
      var _sampleKey = this.SampleRequestKey;

      var _itemKey = item.ItemKey;
      var _customerSpecsModel = item.Item.CustomerSpec;

      var save = qualityControlService.setCustomerProductSpecs( _sampleKey, _itemKey, customerSpecs ).then(
      function() {
        // Notify user of success
        showUserMessage( 'Customer Specs saved successfully', {
          description: ''
        });

        _customerSpecsModel( customerSpecs );

        // Close modal
        $modal.modal( 'hide' );
      },
      function( jqXHR, textStatus, errorThrown ) {
        // Notify user of failure
        showUserMessage( 'Could not save customer specs', {
          description: errorThrown
        });
      }).always( complete );

      return save;
    }.bind( this ),
    canExecute: function( isExecuting ) {
      return !isExecuting && this.modalTemplate() === 'customer-product-specs';
    }.bind( this )
  });
  this.saveCustomerSpecs = function( data ) {
    var _specData = data.toDto();

    this.saveCustomerSpecsCommand.execute( data, _specData );
  };

  // Lab Results modal
  this.showLabResultsModal = ko.command({
    execute: function (data) {
      if (data.ItemKey == null) {
        showUserMessage("Lab Results cannot be added for this item until it has been saved.");
        return;
      }

      this.modalData( new LabResultsModel( data, data.LabResults ) );
      this.modalTemplate('lab-results');

      $modal.modal('show');
      $modal.on( 'hidden.bs.modal', onModalClose );
    }.bind( this ),
    canExecute: function() {
      return !this.isNew;
    }.bind( this )
  });
  this.saveLabResultsCommand = ko.asyncCommand({
    execute: function( item, labResults, complete ) {
      var _sampleKey = this.SampleRequestKey;

      var _itemKey = item.ItemKey;
      var _labResultsModel = item.Item.LabResults;

      var save = qualityControlService.setLabResults( _sampleKey, _itemKey, labResults ).then(
      function() {
        // Notify user of success
        showUserMessage( 'Lab Results saved successfully', {
          description: ''
        });

        _labResultsModel( labResults );

        // Close modal
        $modal.modal( 'hide' );
      },
      function( jqXHR, textStatus, errorThrown ) {
        // Notify user of failure
        showUserMessage( 'Could not save lab results', {
          description: errorThrown
        });
      }).always( complete );

      return save;
    }.bind( this ),
    canExecute: function( isExecuting ) {
      return !isExecuting && this.modalTemplate() === 'lab-results';
    }.bind( this )
  });
  this.saveLabResults = function( data ) {
    var _specData = data.toDto();

    if ( this.saveLabResultsCommand.canExecute() ) {
      this.saveLabResultsCommand.execute( data, _specData );
    } else {
      showUserMessage( 'Could not save lab results.', {
        description: 'Please ensure all forms have been filled out correctly'
      } );
    }
  };

  // Create new products
  this.startNewProduct = callbacks.startNewProduct;

  // Journal Entries tab
  this.JournalEntriesEditor = new JournalEntryEditor( this.SampleRequestKey, sampleData.JournalEntries );

  // Validation
  this.validation = ko.validatedObservable({
    Items: this.SampleDetails,
    Entries: this.JournalEntries
  });
}

SampleMatchingEditor.prototype.loadReport = function (href) {
  window.open(href);
};

SampleMatchingEditor.prototype.toDto = function() {
  var dto = {
    SampleRequestKey: this.SampleRequestKey,

    DateDue: this.DateDue,
    DateReceived: this.DateReceived,
    DateCompleted: this.DateCompleted,
    Status: this.Status,
    Active: this.Active,
    Comments: this.Comments,
    PrintNotes: this.NotesToPrint,
    Volume: this.Volume,
    BrokerKey: this.Broker,
    RequestedByCompanyKey: this.RequestedByCompanyKey,
    RequestedByShippingLabel: this.RequestedByShippingLabel,
    ShipToCompany: this.ShipToCompany,
    ShipToShippingLabel: this.ShipToShippingLabel,
    ShipmentMethod: this.ShipmentMethod,
    FOB: this.FOB,

    Items: ko.utils.arrayMap( this.SampleDetails(), function( item ) {
      return item.toDto();
    }),

    ShipVia: this.ShipVia,
  };

  return dto;
};

/** Sample Matching editor
  *
  * @param {Object} input - Observable, input data for display
  * @param {Object} options - Observable, options for selects or autocomplete inputs
  * @param {Object} exports - Observable, container for exposed methods and properties
  */
function SampleMatchingEditorVM( params ) {
  if ( !(this instanceof SampleMatchingEditorVM) ) { return new SampleMatchingEditorVM( params ); }

  var self = this;

  // Data
  this.sample = params.input;
  this.sampleEditor = ko.observable();

  // Behaviors
  // Construct new editor UI from input data
  function init( data ) {
    var _data = ko.unwrap( data );

    var _editor = new SampleMatchingEditor( _data, params.options, params.callbacks );
    self.sampleEditor( _editor );
  }

  init( params.input );

  var toDto = function() {
    var editor = self.sampleEditor();

    return editor && editor.toDto();
  };

  var isValid = ko.pureComputed(function() {
    var editor = self.sampleEditor();

    return editor && editor.validation.isValid();
  });

  var isPicking = ko.pureComputed({
    read: function() {
      var editor = self.sampleEditor();

      return editor && editor.isPickingContact();
    },
    write: function( pickingBool ) {
      var editor = self.sampleEditor();

      return editor && editor.isPickingContact( !!pickingBool );
    }
  });

  // Exports
  if ( params && params.exports ) {
    params.exports({
      toDto: toDto,
      isValid: isValid,
      isPicking: isPicking
    });
  }

  return this;
}

module.exports = {
  viewModel: SampleMatchingEditorVM,
  template: require('./sample-matching-editor.html')
};
