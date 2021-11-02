define(['./notebook-control.html', 'services/notebooksService', 'ko', 'styles/notebook.css'],
    function (templateMarkup, notebooksService, ko) {
        //document.insertStyleIntoHead(notebookCss);

        function Note(input) {
            if(!(this instanceof Note)) return new Note(input);
            var values = input || {};

            this.Text = ko.observable(values.Text);
            this.CreatedByUser = ko.observable(values.CreatedByUser);
            this.NoteDate = ko.observableDateTime(values.NoteDate || Date.now(), "m/d/yyyy h:MM tt");
            this.NoteKey = values.NoteKey;
        }

        Note.prototype.toDto = function() {
            return {
                NoteKey: this.NoteKey,
                Text: this.Text(),
            }
        }

        function NotebookControlViewModel(params) {
            params = params || {};

            var vm = this;

            this.notebookKey = ko.isObservable(params.notebookKey) ? params.notebookKey : ko.observable(params.notebookKey);
            this.notes = ko.observableArray();
            this.newNote = ko.observable();

            this.insertNoteCommand = ko.asyncCommand({
                execute: function (complete) {
                    var newNote = new Note({
                        Text: vm.newNote(),
                    });

                    return notebooksService.insertNote(vm.notebookKey(), newNote.toDto())
                        .then(function(data) {
                            vm.notes.push(new Note(data));
                            vm.newNote(null);
                        })
                        .always(complete);
                },
                canExecute: function(isExecuting) {
                    return !isExecuting && vm.newNote() && true;
                },
                owner: this,
            });

            this.loadNotebook();
        }

        NotebookControlViewModel.prototype.loadNotebook = function () {
            var notes = this.notes;
            notes([]);

            var key = this.notebookKey();
            if (!key) return;
            
            notebooksService.getNotebookByKey(key)
                .then(function (data) {
                    notes(ko.utils.arrayMap(data, function (item) { return new Note(item); }));
                });
        }
        
        return {
            viewModel: NotebookControlViewModel,
            template: templateMarkup
        }
    });