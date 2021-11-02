(function(ko) {
    ko.optionalModule = function (options) {
        options = options || {};
        var defaults = {
            createNew: function () { return options.data; },
            evaluateAsNull: function () { return false; }
        };

        options.createNew = options.createNew || defaults.createNew;
        options.evaluateAsNull = options.evaluateAsNull || defaults.evaluateAsNull;

        var backingProperty = ko.observable();
        var isNull = ko.computed(function() {
            return options.data == null || options.evaluateAsNull(options.data);
        });

        setValue(isNull() ? null : options.data);
        backingProperty.createCommand = ko.command({
            canExecute: function(isExecuting) { return !isExecuting && backingProperty() == null; },
            execute: function () { setValue(options.createNew()); },
        });
        backingProperty.deleteCommand = ko.command({
            canExecute: function(isExecuting) { return !isExecuting && backingProperty() != null; },
            execute: function() { backingProperty(null); },
        });
        backingProperty.isNullOrEmpty = isNull;
        
        return backingProperty;
        
        function setValue(value) {
            backingProperty(value);
        }
    };
})(ko);