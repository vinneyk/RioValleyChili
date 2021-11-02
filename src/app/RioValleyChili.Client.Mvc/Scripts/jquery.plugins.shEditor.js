(function (root, $) {
    $.SHEditor = function (element, options) {
        
        var defaults = {
            edit: function() { },
            save: function() { },
            cancel: function() { },
            canSave: true,
            canEdit: true,
            canCancel: true,
            readonlyViewPane: '.editor-readonly-view',
            editableViewPane: '.editor-editable-view',
            cancelButtonTarget: '.cancel-command',
            saveButtonTarget: '.save-command',
            editButtonTarget: '.edit-command',
            mode: 'view', // , 'edit',
            beginEditCallback: function() { },
            cancelEditCallback: function() { },
        };

        options = $.extend({}, defaults, options);

        var $element = $(element);
        var self = this;
        $.extend(self, options, self);
        self.$editViewPane = $(options.editableViewPane, $element);
        self.$readonlyViewPane = $(options.readonlyViewPane, $element);
        self.$editButton = $(options.editButtonTarget, self.$readonlyViewPane);
        self.$saveButton = $(options.saveButtonTarget, self.$editViewPane);
        self.$cancelButton = $(options.cancelButtonTarget, self.$editViewPane);
        self.forceEndEdit = function (resetValues) {
            if (resetValues) {
                var canCancelHold = self.canCancel;
                var isEditingHold = self.isEditing;
                self.canCancel = true;
                self.isEditing = true;

                cancelCommand();
                
                self.canCancel = canCancelHold;
                self.isEditing = isEditingHold;
            }
            
            if (mode == 'edit') {
                setModeReadonly();
            }
        };

        self.init = function () {
            if (canEdit()) {
                self.$editButton.show();
                self.$editButton.click(editCommand);
            } else {
                self.$editButton.hide();
            }

            if (canCancel()) {
                self.$cancelButton.show();
                self.$cancelButton.click(cancelCommand);
            } else {
                self.$cancelButton.hide();
            }

            if (canSave()) {
                self.$saveButton.show();
                self.$saveButton.click(saveCommand);
            } else {
                self.$saveButton.hide();
            }

            if (self.mode === 'view') {
                setModeReadonly();
            } else {
                setModeEditable();
                beginEdit();
            }

            return self;
        };

        return self.init(options);
        
        // visual state functions
        function switchStates() {
            if (self.$readonlyViewPane.is(':visible')) {
                setModeEditable();
            } else {
                setModeReadonly();
            }
        }
        function setModeReadonly() {
            self.$readonlyViewPane.show();
            self.$editViewPane.hide();
            mode = 'view';
        }
        function setModeEditable() {
            self.$readonlyViewPane.hide();
            self.$editViewPane.show();
            mode = 'edit';
        };

        // command functions
        function canSave() {
            return typeof self.canSave == "boolean"
                ? self.canSave 
                : self.canSave();
        }
        function canEdit() {
            return typeof self.canEdit == "boolean" ?
                self.canEdit :
                self.canEdit();
        }
        function canCancel() {
            return typeof self.canCancel == "boolean" ?
                self.canCancel :
                self.canCancel();
        }
        function beginEdit() {
            var tempData = null;
            if (self.viewModel != null && self.viewModel != {}) {
                // SAVE VIEW MODEL STATE::
                // simplify view model object and remove unwanted properties
                var selfJS = ko.mapping.toJS(self.viewModel);
                var simplified = {};
                ko.mapping.fromJS(selfJS, self.customViewModelMapping || {}, simplified);

                // convert pruned object to JS object
                var rootJS = ko.mapping.toJS(simplified);

                tempData = rootJS;
            }
            
            // convert JS object to JSON
            self.tempData = ko.toJSON(tempData);
            self.savedState = true;

            self.isEditing = true;
            return true;
        }
        function endEdit() {
            if (self.isEditing !== true || self.savedState !== true) { return true; }
            ko.mapping.fromJSON(self.tempData, {}, self.viewModel);
            self.isEditing = false;
            self.savedState = false;
            return true;
        }
        
        // commands
        function saveCommand() {
            if (canSave()) {
                if (self.save == null || self.save() !== false) {
                    switchStates();
                }
            }
        };
        function cancelCommand() {
            // todo: clear fields if viewModel is not provided
            if (canCancel() && endEdit()) {
                if (self.cancel == null || self.cancel() !== false) {
                    switchStates();
                    self.cancelEditCallback();
                }
            }
        };
        function editCommand() {
            if (canEdit() && beginEdit()) {
                if (self.edit == null || self.edit() !== false) {
                    switchStates();
                    self.beginEditCallback();
                }
            }
        };
    };

    $.fn.shEditor = function (options) {
        // this breaks the jQuery chaining but return the initialized control
        if (this.length == 1) {
            return new $.SHEditor(this, options);
        }
        
        return this.each(function() {
            (new $.SHEditor(this, options));
        });
    };

})(window, jQuery);