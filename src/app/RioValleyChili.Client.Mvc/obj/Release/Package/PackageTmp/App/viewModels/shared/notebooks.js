define(['services/notebooksService', 'ko', 'helpers/koHelpers'], function (notebooksService, ko, koHelpers) {
    registerObjectConstructors();
    return {
        createNotebook: createNotebookViewModel
    };

    function createNotebookViewModel(options) {
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
            execute: function (note) {
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
                notebooksService.getNotebookByKey(notebookKey, {
                    successCallback: function (data) {
                        setNotes(data);
                        self.loadNotebookCommand.indicateSuccess();
                        if (options.successCallback) options.successCallback();
                    },
                    errorCallback: function (xhr) {
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
                    notebooksService.putNote(note, {
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
                    notebooksService.postNote(note, {
                        successCallback: function (data) {
                            var newNote = mapNote(data);
                            if (sortAsc) self.notes.push(newNote);
                            else self.notes.splice(0, 0, newNote);

                            self.saveNoteCommand.pushSuccess("Note created successfully.");
                            self.saveNoteCommand.indicateSuccess();
                            self.initializeBindings();
                            callbacks.successCallback && callbacks.successCallback();
                        },
                        errorCallback: function () {
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
                    notebooksService.deleteNote(note, {
                        successCallback: function () {
                            self.notes.remove(note);
                            self.deleteNoteCommand.indicateSuccess();
                        },
                        errorCallback: function (data) {
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
                    ko.utils.arrayForEach(notesToInsert, function (n) {
                        insertNoteAsync(mapNote(n));
                    });
                }

                function insertNoteAsync(note, attempt) {
                    attempt = attempt || 0;
                    notebooksService.postNote(note, {
                        successCallback: function (data) {
                            var newNote = mapNote(data);
                            if (sortAsc) self.notes.push(newNote);
                            else self.notes.splice(0, 0, newNote);
                            processedCount++;
                        },
                        errorCallback: function () {
                            if (attempt < 3) insertNoteAsync(note, attempt + 1);
                            else errors.push("\"" + note.Text() + "\"");
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
            },
            canExecute: function (isExecuting) { return !isExecuting; }
        });

        // init
        koHelpers.ajaxStatusHelper(self.loadNotebookCommand);
        koHelpers.ajaxStatusHelper(self.saveNoteCommand);
        koHelpers.ajaxStatusHelper(self.deleteNoteCommand);

        self.initializeBindings();
        if (notebookKey && (!options.initialValues || !options.initialValues.Notes)) self.loadNotebookCommand.execute();

        var initialNotes = options.initialValues && options.initialValues.Notes
            ? ko.utils.unwrapObservable(options.initialValues.Notes)
            : [];

        setNotes(initialNotes);

        return self;

        //#region private functions

        function setNotes(data) {
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
                execute: function () {
                    baseEditCmd.execute(note);
                }
            });

            var baseDeleteCmd = self.deleteNoteCommand;
            note.deleteCommand = ko.command({
                execute: function () {
                    baseDeleteCmd.execute(note);
                }
            });

            return note;
        }

        //#endregion
    }

    function registerObjectConstructors() {
        window.Note = function (data) {
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
    }
});
