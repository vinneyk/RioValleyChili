/** Required components */
ko.components.register( 'contact-label-editor', require('App/components/warehouse/contact-label-editor/contact-label-editor') );
ko.components.register( 'contact-picker', require('App/components/common/address-book/contact-picker') );

/** Required libraries */
var directoryService = require('App/services/directoryService');

require('App/helpers/koPunchesFilters');
ko.punches.enableAll();

function Note( note ) {
  this.NoteKey = note.NoteKey;
  this.DisplayBold = note.DisplayBold;
  this.NoteType = ko.observable( note.NoteType );
  this.Text = ko.observable( note.Text );
  this.TimeStamp = note.TimeStamp;
  this.CreatedByUser = note.CreatedByUser;
}

Note.prototype.toDto = function() {
  return ko.toJS({
    NoteKey: this.NoteKey,
    Type: this.NoteType,
    Text: this.Text,
    Bold: this.DisplayBold,
  });
};

function Editor( input, options ) {
  var self = this;

  this.CompanyKey = input.CompanyKey;

  this.options = options;
  this.isNew = input.CompanyKey == null;

  var _companyTypes = ko.utils.arrayMap( this.options.companyTypes, function( opt ) {
    var _companyTypes = input.CompanyTypes || [];
    return {
      key: opt.key,
      name: opt.value,
      value: ko.observable( _companyTypes.indexOf( opt.key ) > -1 )
    };
  });
  var _broker = ko.utils.arrayFirst( this.options.brokers(), function( broker ) {
    var brokerKey = input.CustomerResponse && input.CustomerResponse.Broker.CompanyKey;

    return broker.CompanyKey === brokerKey;
  });
  this.companyData = {
    CompanyName: ko.observable( input.Name ).extend({ required: true }),
    Active: ko.observable( input.Active ),
    BrokerCompany: ko.observable( _broker ),
    CompanyTypes: ko.observableArray( _companyTypes ),
  };


  this.validation = ko.validatedObservable({
    Name: this.companyData.CompanyName,
    CompanyTypes: this.companyData.CompanyTypes.extend({
      validation: {
        validator: function() {
          var _companyTypes = self.companyData.CompanyTypes();
          var _selectedCompanyType = ko.utils.arrayFirst( _companyTypes, function( companyType ) {
            return companyType.value();
          });

          return !!_selectedCompanyType;
        },
        message: 'Companies must have at least one Company Type selected'
      }
    }),
  });

  this.isCustomer = ko.pureComputed(function() {
    var _customerType = ko.utils.arrayFirst( self.companyData.CompanyTypes(), function( companyType ) {
      return companyType.key === '0' || companyType.key === 0;
    });

    return _customerType && _customerType.value();
  });

  // Contact info
  this.contactEditorData = {
    companyKey: self.CompanyKey,
    options: ko.observableArray( [] ),
    selected: ko.observable(),
  };

  function getContacts( companyKey ) {
    var fetchContacts = directoryService.getContacts( companyKey ).then(
    function( data, textStatus, jqXHR ) {
      self.contactEditorData.options( data );
    },
    function( jqXHR, textStatus, errorThrown ) {
      showUserMessage( 'Could not get company data', {
        description: errorThrown
      });
    });
  }

  if ( !this.isNew ) {
    getContacts( self.CompanyKey );
  }

  // Profile Notes
  this.profileNotes = ko.observableArray( input.CustomerResponse && input.CustomerResponse.CustomerNotes );

  this.noteEditorData = ko.observable();

  this.startNewNote = function() {
    self.noteEditorData( new Note( {} ) );
  };

  this.editNote = function( data, element ) {
    var $tr = $( element.target ).closest('tr')[0];

    // End if tr element does not exist
    if ( !$tr ) {
      return;
    }

    var note = ko.contextFor( $tr ).$data;

    self.noteEditorData( new Note( note ) );
  };

  this.cancelNote = function() {
    self.noteEditorData( null );
  };

  this.saveNote = ko.asyncCommand({
    execute: function( complete ) {
      var _note = self.noteEditorData().toDto();
      var _companyKey = self.CompanyKey;

      var save;
      if ( _note.NoteKey ) {
        save = directoryService.updateNote( _companyKey, _note.NoteKey, _note ).then(
        function( data, textStatus, jqXHR ) {
          // Replace selected note with new data
          var _notes = self.profileNotes();
          var matchedNote = ko.utils.arrayFirst( _notes, function( note ) {
            return note.NoteKey === _note.NoteKey;
          });
          var _noteIndex = _notes.indexOf( matchedNote );
          var _newNote = data;

          self.profileNotes.splice( _noteIndex, 1, _newNote );
        });
      } else {
        save = directoryService.createNote( _companyKey, _note ).then(
        function( data, textStatus, jqXHR ) {
          // Append new note to note list
          self.profileNotes.push( data );
        });
      }

      var finishSave = save.then(
      function( data, textStatus, jqXHR ) {
        self.noteEditorData( null );
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not save note', {
          description: errorThrown
        });
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      var editor = self.noteEditorData();
      return !isExecuting && editor && editor.Text();
    }
  });

  this.deleteNoteCommand = ko.asyncCommand({
    execute: function( _noteKey, complete ) {
      var _companyKey = self.CompanyKey;

      // Call API for delete
      var _deleteNote = directoryService.deleteNote( _companyKey, _noteKey ).then(
      function( data, textStatus, jqXHR ) {
        // Remove note from notes list
        var _notes = self.profileNotes();
        var _note = ko.utils.arrayFirst( _notes, function( note ) {
          return note.NoteKey === _noteKey;
        });
        var _noteIndex = _notes.indexOf( _note );

        self.profileNotes.splice( _noteIndex, 1 );
      },
      function( jqXHR, textStatus, errorThrown ) {
        showUserMessage( 'Could not remove note', {
          description: errorThrown
        });
      }).always( complete );
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });
  this.deleteNote = function( data, element ) {
    var _noteKey = data.NoteKey;

    // Confirm delete
    showUserMessage( 'Delete note?', {
      description: 'Are you sure you want to delete this note? This action cannot be undone.',
      type: 'yesno',
      onYesClick: function() {
        var _noteEditor = self.noteEditorData();

        if ( _noteEditor && _noteEditor.NoteKey === _noteKey ) {
          self.cancelNote();
        }

        self.deleteNoteCommand.execute( _noteKey );
      },
      onNoClick: function() { }
    });
  };

}

Editor.prototype.toDto = function() {
  var _broker = this.companyData.BrokerCompany();
  var dto = {
    CompanyKey: this.CompanyKey,
    CompanyName: this.companyData.CompanyName,
    Active: this.companyData.Active,
    BrokerCompanyKey: _broker && _broker.CompanyKey,
    CompanyTypes: []
  };

  ko.utils.arrayForEach( this.companyData.CompanyTypes(), function( companyType ) {
    if ( companyType.value() ) {
      dto.CompanyTypes.push( companyType.key );
    }
  });

  return ko.toJS( dto );
};

/** Editor view for customer maintenance
  *
  * @param {Object} input - Observable, Customer data to edit
  * @param {Object} options - Options for note editor
  * @param {Object} exports - Observable, Container for exposed methods and properties
  */
function CustomerMaintenanceEditorVM( params ) {
  if ( !(this instanceof CustomerMaintenanceEditorVM) ) { return new CustomerMaintenanceEditorVM( params ); }

  var self = this;

  var _input = ko.unwrap( params.input ) || {};
  var _options = params.options;
  this.isNew = _input.CompanyKey == null;

  this.editor = ko.observable( buildEditor( _input, _options ) );

  params.input.subscribe(function( newData ) {
    if ( newData ) {
      self.editor( buildEditor( newData, _options ) );
    }
  });

  function buildEditor( input, options ) {
    return new Editor( input, options );
  }

  function toDto() {
    var _editor = self.editor();

    return _editor && _editor.toDto();
  }

  var isValid = ko.pureComputed(function() {
    var _editor = self.editor();

    return _editor && _editor.validation.isValid();
  });

  // Exports
  if ( params && params.exports ) {
    params.exports({
      isValid: isValid,
      toDto: toDto
    });
  }

  return this;
}

module.exports = {
  viewModel: CustomerMaintenanceEditorVM,
  template: require('./customer-maintenance-editor.html')
};
