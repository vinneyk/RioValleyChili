var notebooksService = require('App/services/notebooksService.js');

/**
  * @param {Object} data - Note data object
  */
function Instruction(data) {
  this.cache = ko.toJS(data);

  /** Data */
  this.Text = ko.observable(data.Text);
  this.Sequence = data.Sequence;
  this.NoteKey = data.NoteKey;
  this.NoteDate = data.NoteDate || (Date.now()).toISOString();
  this.CreatedByUser = data.CreatedByUser;

  /** State */
  this.isEditing = ko.observable(false);
  this.isWorking = ko.observable(false);
  this.isNew = false;
}

Instruction.prototype.toDto = function() {
  var dto = ko.toJS({
    Text: this.Text(),
  });

  return dto;
};

/**
  * @param {Object} input - Observable, Object w/ instructions notebook data
  * @param {string[]} options - Autocomplete list for instructions editor
  * @param {function} exports - Observable, Container for exposed methods
  */
function InstructionsEditorVM(params) {
  if (!(this instanceof InstructionsEditorVM)) { return new InstructionsEditorVM(params); }

  var self = this;
  var inputData = ko.unwrap(params.input) || {};

  // Data
  this.notebookKey = inputData.NotebookKey;
  this.instructions = ko.observableArray(mapAllInstructions(inputData.Notes));
  this.options = params.options || [];

  this.newInstruction = ko.observable();
  this.isWorking = ko.observable(false);

  // Behaviors
  function mapAllInstructions(notes) {
    var instructions = ko.toJS(notes || []);

    return ko.utils.arrayMap(instructions, mapInstruction);
  }

  function mapInstruction(item) {
    var newInstruction = new Instruction(item);

    return newInstruction;
  }

  /** Instructions editor */
  function removeInstruction(item) {
    var instructions = self.instructions();
    var i = instructions.indexOf(item);

    self.instructions.splice(i, 1);
  }

  function updateNotebook() {
    var Notebook = {
      NotebookKey: self.notebookKey,
      Notes: ko.toJS(self.instructions)
    };

    if ( params && params.input ) {
      params.input(Notebook);
    }
  }

  this.editInstruction = function(instruction) {
    var context = instruction || this;

    context.isEditing(true);
  };

  this.cancelInstructionEdit = function(instruction) {
    var context = instruction || this;

    context.isEditing(false);
    context.Text(context.cache.Text);
  };

  this.deleteInstruction = function(instruction) {
    var context = instruction || this;

    context.isEditing(false);

    showUserMessage('Delete instructions', {
      description: "".concat('Remove "<i>', context.Text(), '</i>" from instructions sheet?'),
      type: 'yesno',
      onYesClick: function() {
        var deleteInstruction = notebooksService.deleteNote(context).then(
        function(data, textStatus, jqXHR) {
          removeInstruction(context);
          updateNotebook();
        },
        function(jqXHR, textStatus, errorThrown) {
          showUserMessage('Delete failed', { description: errorThrown });
        });
      },
      onNoClick: null
    });
  };

  this.saveInstructionEdit = function(instruction) {
    var context = instruction || this;
    var text = ko.unwrap(context.Text);

    if (text == undefined) { return; }

    context.isWorking(true);
    context.NotebookKey = self.notebookKey;

    var updateInstruction = notebooksService.updateNoteText(context).then(
      function (data, textStatus, jqXHR) {
        context.cache.Text = text;
        self.options.push(text);
        updateNotebook();
      },
      function (jqXHR, textStatus, errorThrown) {
        showUserMessage('Save failed', { description: errorThrown });
      }).always(function () {
        context.isEditing(false);
        context.isWorking(false);
      });
  };

  /** New Instruction */
  this.saveNewInstruction = ko.asyncCommand({
    execute: function (complete) {
      var key = self.notebookKey;
      var instructionText = self.newInstruction().trim();

      self.isWorking(true);

      var submitInstruction = notebooksService.insertNote(key, { Text: instructionText })
        .then(function (data, textStatus, jqXHR) {
          var instructionData = mapInstruction(data);
          if (self.options.indexOf(instructionText) === -1) {
            self.options.push(instructionData.Text());
          }
          self.instructions.push(instructionData);
          self.newInstruction("");
          updateNotebook();
        }, function (jqXHR, textStatus, errorThrown) {
          showUserMessage('Save failed', { description: errorThrown });
        }).always(function () {
          self.isWorking(false);
          complete();
        });
    },
    canExecute: function(isExecuting) {
      var instruction = self.newInstruction();
      return !isExecuting && instruction != undefined && instruction.trim() !== '';
    }
  });

  /** Data export */
  function toDto() {
    return ko.utils.arrayMap(self.instructions(), function(instruction) {
      return instruction.Text();
    });
  }

  // Exports
  if (params && params.exports) {
    params.exports({
      toDto: toDto,
      Notebook: {
        Notes: self.instructions,
        NotebookKey: self.notebookKey
      }
    });
  }

  return this;
}

module.exports = {
  viewModel: InstructionsEditorVM,
  template: require('./instructions-editor.html'),
  synchronous: true
};

