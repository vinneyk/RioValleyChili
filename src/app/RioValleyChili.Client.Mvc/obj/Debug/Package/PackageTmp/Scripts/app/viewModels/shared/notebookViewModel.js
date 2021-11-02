function Note(data) {
    if (!(this instanceof Note)) return new Note(data);

    var note = this;

    this.NotebookKey = ko.utils.unwrapObservable(data.NotebookKey);
    this.NoteDate = ko.observableDateTime(ko.utils.unwrapObservable(data.NoteDate) || Date.now(), "m/d/yyyy h:MM tt");
    this.NoteKey = ko.utils.unwrapObservable(data.NoteKey);
    this.User = ko.utils.unwrapObservable(data.CreatedByUser);
    this.Text = ko.observable(ko.utils.unwrapObservable(data.Text) || '');
    this.Sequence = ko.observable(ko.utils.unwrapObservable(data.Sequence));
    
    note.toDto = function () {
        return { Text: note.Text() };
    };
    note.isValid = function () {
        return note.NotebookKey && note.Text();
    };

    return note;
}

var NotebookViewModel = (function () {

    rvc.api.notebooks = {
        getNotebookByKey: getNotebookByKey,
        putNote: putNote,
        postNote: postNote
    }

    return {
        init: init
    };

    function init(options) {
        options = options || {};
        var sortAsc = options.sort == 'asc' || options.sort == undefined;
        var initialValues = options.initialValues || {};
        if (!options.target) throw new Error("Notebook view model requires a target to be specified on the options parameter.");

        var notebookKey = ko.utils.unwrapObservable(initialValues.NotebookKey || options.NotebookKey || options.target.NotebookKey) || null;
        var self = (options.target.notebookViewModel = {});

        self.notes = ko.observableArray();
        self.newNote = ko.observable();
        self.NotebookKey = notebookKey;

        // methods
        self.initializeBindings = function () {
            self.newNote(mapNote());
        };

        // commands
        self.cancelCommand = ko.command({
            execute: function(note) {
                if (note.initial) note.Text(note.initial);
                else self.notes.remove(note);
                note.isEditing(false);
            }
        });
        self.editCommand = ko.command({
            execute: function (note) {
                note.initial = note.Text();
                note.isEditing(true);
            }
        });
        self.loadNotebookCommand = ko.composableCommand({
            execute: function (complete) {
                self.loadNotebookCommand.indicateWorking();
                rvc.api.notebooks.getNotebookByKey(notebookKey, {
                    successCallback: function (data) {
                        mapNotes(data);
                        self.loadNotebookCommand.indicateSuccess();
                        if (options.successCallback) options.successCallback();
                    },
                    errorCallback: function(xhr) {
                        self.loadNotebookCommand.indicateFailure();
                        self.loadNotebookCommand.pushError("Failed to get notes.");
                    },
                    completeCallback: complete
                });
            }
        });
        self.saveNoteCommand = ko.composableCommand({
            execute: function (note, callbackOptions, complete) {
                var callbacks = callbackOptions || {};

                self.saveNoteCommand.indicateWorking();
                if (note.NoteKey)
                    rvc.api.notebooks.putNote(note, {
                        successCallback: function () {
                            self.saveNoteCommand.pushSuccess("Note saved successfully.");
                            note.initial = note.Text();
                            note.isEditing(false);
                            self.saveNoteCommand.indicateSuccess();
                            callbacks.successCallback && callbacks.successCallback();
                        },
                        errorCallback: function (data) {
                            self.saveNoteCommand.pushError("Error saving note.");
                            self.saveNoteCommand.indicateFailure();
                            callbacks.errorCallback && callbacks.errorCallback();
                        },
                        completeCallback: complete,
                    });
                else
                    rvc.api.notebooks.postNote(note, {
                        successCallback: function (data) {
                            var newNote = mapNote(data);
                            if (sortAsc) self.notes.push(newNote);
                            else self.notes.splice(0, 0, newNote);

                            self.saveNoteCommand.pushSuccess("Note created successfully.");
                            self.saveNoteCommand.indicateSuccess();
                            self.initializeBindings();
                            callbacks.successCallback && callbacks.successCallback();
                        },
                        errorCallback: function() {
                            self.saveNoteCommand.indicateFailure();
                            self.saveNoteCommand.pushError('Error creating note.');
                            callbacks.errorCallback && callbacks.errorCallback();
                        },
                        completeCallback: complete,
                    });
            }
        });
        self.deleteNoteCommand = ko.composableCommand({
            execute: function (note, complete) {
                showUserMessage("Are you sure you want to delete this note? This cannot be undone.", {
                    autoClose: false,
                    type: "yesnocancel",
                    onYesClick: confirm
                });

                function confirm() {
                    self.deleteNoteCommand.indicateWorking();
                    deleteNote(note, {
                        successCallback: function() {
                            self.notes.remove(note);
                            self.deleteNoteCommand.indicateSuccess();
                        },
                        errorCallback: function(data) {
                            self.deleteNoteCommand.pushError("Error deleting note.");
                            self.deleteNoteCommand.indicateFailure();
                        },
                        completeCallback: complete
                    });
                }
            }
        });
        self.initNewNoteCommand = ko.command({
            execute: function () {
                return mapNote();
            }
        });
        self.createMultipleNotesCommand = ko.asyncCommand({
            execute: function (notes, complete) {
                var newNotes = ko.toJS(notes);
                var totalNotes = notes.length,
                    processedCount = 0,
                    errors = [];

                insertNotes(newNotes);

                function insertNotes(notesToInsert) {
                    if (!notesToInsert || !notesToInsert.length) return;

                    var attempt = 0;
                    var note = mapNote(notesToInsert.shift());

                    rvc.api.notebooks.postNote(note, {
                        successCallback: function (data) {
                            var newNote = mapNote(data);
                            if (sortAsc) self.notes.push(newNote);
                            else self.notes.splice(0, 0, newNote);
                            processedCount++;
                            insertNotes(notesToInsert);
                        },
                        errorCallback: function () {
                            if (attempt < 3) insertNote(note, attempt + 1);
                            else errors.push("\"" + note + "\"");
                        },
                        completeCallback: function () {
                            if (allNotesProcessed()) {
                                if (errors.length) {
                                    showUserMessage("The following notes failed to save:", {
                                        description: errors.join(', ')
                                    });
                                }
                                complete();
                            }
                        },
                    });

                    function allNotesProcessed() {
                        return totalNotes === processedCount + errors.length;
                    }
                }
                
                function insertNote(note, attempt) {
                    attempt = attempt || 0;
                    rvc.api.notebooks.postNote(note, {
                        successCallback: function (data) {
                            var newNote = mapNote(data);
                            if (sortAsc) self.notes.push(newNote);
                            else self.notes.splice(0, 0, newNote);
                            successCount++;
                        },
                        errorCallback: function () {
                            if (attempt < 3) insertNote(note, attempt + 1);
                            else errors.push(note);
                        },
                        completeCallback: function () {
                            if (allNotesCreated()) {
                                complete();
                            }
                        },
                    });
                }
            },
            canExecute: function(isExecuting) { return !isExecuting; }
        });
        
        // init
        ajaxStatusHelper.init(self.loadNotebookCommand);
        ajaxStatusHelper.init(self.saveNoteCommand);
        ajaxStatusHelper.init(self.deleteNoteCommand);

        self.initializeBindings();
        if (notebookKey && (!options.initialValues || !options.initialValues.Notes)) self.loadNotebookCommand.execute();
        
        var initialNotes = !options.initialValues || !options.initialValues.Notes
            ? []
            : ko.utils.unwrapObservable(options.initialValues.Notes);
        mapNotes(initialNotes);

        return self;

        // private functions

        function mapNotes(data) {
            var notes = ko.utils.arrayMap(data, mapNote).sort(function (a, b) {
                return sortAsc
                    ? a.Sequence() < b.Sequence()
                    : a.Sequence() > b.Sequence();
            });
            self.notes(notes);
        }
        function mapNote(input) {
            var data = typeof input === "string"
                ? { Text: input }
                : input || {};

            data.NotebookKey = notebookKey;
            var note = new Note(data);
            note.isEditing = ko.observable();

            var baseSaveCmd = self.saveNoteCommand;
            note.saveCommand = ko.command({
                execute: function () {
                    baseSaveCmd.execute(note);
                },
                canExecute: function () {
                    return !!note.Text().trim();
                }
            });

            var baseCancelCmd = self.cancelCommand;
            note.cancelCommand = ko.command({
                execute: function () {
                    baseCancelCmd.execute(note);
                }
            });

            var baseEditCmd = self.editCommand;
            note.editCommand = ko.command({
                execute: function() {
                    baseEditCmd.execute(note);
                }
            });

            var baseDeleteCmd = self.deleteNoteCommand;
            note.deleteCommand = ko.command({
                execute: function() {
                    baseDeleteCmd.execute(note);
                }
            });

            return note;
        }
    }

    // private functions

    function getNotebookByKey(key, options) {
        $.ajax("/api/notebooks/" + key + "/notes", {
            success: options.successCallback,
            error: options.errorCallback,
            complete: options.completeCallback,
        });
    }

    function putNote(note, options) {
        $.ajax("/api/notebooks/" + note.NotebookKey + "/notes/" + note.NoteKey, {
            type: 'PUT',
            contentType: "application/json",
            data: ko.toJSON(note.toDto()),
            success: options.successCallback,
            error: options.errorCallback,
            complete: options.completeCallback,
        });
    }

    function postNote(note, options) {
        $.ajax("/api/notebooks/" + note.NotebookKey + "/notes", {
            type: 'POST',
            contentType: "application/json",
            data: ko.toJSON(note.toDto()),
            success: options.successCallback,
            complete: options.completeCallback,
            error: options.errorCallback,
        });
    }

    function deleteNote(note, options) {
        $.ajax("/api/notebooks/" + note.NotebookKey + "/notes/" + note.NoteKey, {
            type: 'DELETE',
            success: options.successCallback,
            error: options.errorCallback,
            complete: options.completeCallback,
        });
    }

}());