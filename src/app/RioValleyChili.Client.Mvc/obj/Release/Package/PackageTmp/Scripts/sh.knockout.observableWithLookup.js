/// The purpose of this knockout extension is to simplify the binding of select lists.
/// config argument:
/// - keyDelegate (required) - a delegate function for returning the key value of a given option item
/// - value - the initially selected value / object
/// - displayProjector - a delegate function for returning the display representation of the option
/// - defaultDisplayValue - the value to display when no item is selected
/// - lookupOptions - the source for the options. If the argument value is an observable and the desired
///                   option is not found in the array, then setting of the initially selected value 
///                   will be deferred until the next options array is assigned.

(function () {
    if (require) {
        define(['ko'], factory);
    } else {
        factory(ko);
    }

    function factory(ko) {
        if (typeof ko.observableWithLookup !== "undefined") return;
        ko.observableWithLookup = function(config) {
            config = config || {};
            if (typeof config.keyDelegate !== "function") throw new Error('Missing or invalid configuration argument: \"keyDelegate\". Expected function.');

            var initialValue = ko.unwrap(config.value),
                keyValue = getKeyValue(initialValue),
                observable = ko.observable(initialValue);

            if (initialValue != undefined && keyValue == undefined) {
                throw new Error('Unable to get the keyValue for the initial value. Check the keyDelegate');
            }

            observable.display = ko.pureComputed(function() {
                if (typeof config.displayProjector !== "function") return '';
                var selected = observable();
                return selected ? config.displayProjector(selected) : config.defaultDisplayValue || '';
            });
            observable.options = config.lookupOptions;
            observable.keyValue = ko.pureComputed({
                read: function() {
                    return getKeyValue(observable());
                },
                write: function(value) {
                    keyValue = value;
                    this(this.lookup(value));
                },
                owner: observable,
            });

            var _subscriptions = [];

            observable.lookup = lookup;
            observable.dispose = dispose;

            if (ko.isObservable(config.lookupOptions) && !config.lookupOptions.peek().length) {
                // Lookup options may not yet have been loaded. Defer initialization until the values are set.
                var lookupOptionsSubscription = config.lookupOptions.subscribe(function() {
                    lookupOptionsSubscription.dispose();
                    ko.utils.arrayRemoveItem(_subscriptions, lookupOptionsSubscription);

                    // Allow element to be bound before attempting to set the initial value
                    // otherwise the value will be cleared
                    setTimeout(function() {
                        setSelected();
                    }, 0);
                });
                _subscriptions.push(lookupOptionsSubscription);

            } else { // Lookup options are either not observable or have already been loaded. 
                setSelected();
            }

            return observable;

            function lookup(key) {
                return key == undefined
                    ? null
                    : ko.utils.arrayFirst(ko.unwrap(observable.options), equalityDelegate);

                function equalityDelegate(opt) {
                    return getOptionValue(opt) === key;
                }
            }
            function getKeyValue(obj) {
                return typeof obj === "object" ? config.keyDelegate(obj) : obj;
            }
            function getOptionValue(opt) {
                return typeof config.optionValueDelegate == "function" 
                    ? config.optionValueDelegate(opt)
                    : config.keyDelegate(opt);
            }
            function setSelected() {
                var initialSelection = observable.lookup(keyValue);
                observable(initialSelection);
            }

            function dispose() {
                this.options = null;
                this.display.dispose();
                this.display = null;
                this.keyValue.dispose();
                this.keyValue = null;

                ko.utils.arrayForEach(_subscriptions, function(s) { s.dispose(); });
                _subscriptions = [];
            }
        }
    }
}())