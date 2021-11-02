(function() {
    if (require) {
        define(['ko'], extendKnockout);
    } else extendKnockout(ko);

    function extendKnockout(ko) {
        ko.DEBUG = true;

        ko.EditStateManager = function(objectToTrack, options) {
            var defaultOptions = ko.EditStateManager.defaultOptions;

            options = options || {};

            var include = defaultOptions.include || [];
            if (options.include && options.include.length > 0) {
                ko.utils.arrayPushAll(include, options.include);
            }

            options.initializeAsEditing = options.initializeAsEditing || defaultOptions.initializeAsEditing;
            options.isInitiallyDirty = options.isInitiallyDirty || defaultOptions.isInitiallyDirty;
            options.canSave = options.canSave || defaultOptions.canSave;
            options.canEdit = options.canEdit || defaultOptions.canEdit;
            options.canEndEditing = options.canEndEditing || defaultOptions.canEndEditing;

            var name = options.name || defaultOptions.name;

            var cacheMapping = options.customMappings || {};
            var customRevertFunctions = options.customRevertFunctions || {};
            var ignore = defaultOptions.ignore.concat(options.ignore);

            if (ko.isObservable(objectToTrack) && isArray(objectToTrack())) {
                // object is an observableArray
                objectToTrack = {
                    array: objectToTrack
                };
            }

            var trackedObject = buildTrackedObject();
            var revertUntrackedChanges = options.revertUntrackedChanges || emptyFn;
            var hasUntrackedChanges = options.hasUntrackedChanges || function() { return false; };
            var commitUntrackedChanges = options.commitUntrackedChanges || emptyFn;

            var revertChangesCallback = options.revertChangesCallback || emptyFn;
            var commitChangesCallback = options.commitChangesCallback || emptyFn;
            var beginEditingCallback = options.beginEditingCallback || emptyFn;
            var endEditingCallback = options.endEditingCallback || emptyFn;

            function emptyFn() {}

            var result = function() {},
                isInitiallyDirty = ko.observable(options.isInitiallyDirty),
                cachedState = ko.observable(),
                currentHash = ko.computed(function() {
                    return serialize(trackedObject);
                }),
                preventDirtyCheck = ko.observable(false);

            var isEditing = ko.observable(false);
            var isDirty = ko.computed(function() {
                return preventDirtyCheck() || (isInitiallyDirty() || cachedState() !== currentHash());
            });

            // computed properties
            result.isEditing = ko.computed(function() {
                return isEditing();
            });
            result.isReadonly = ko.computed(function() {
                return !isEditing();
            }, result);
            result.hasChanges = ko.computed(function() {
                return isDirty() || hasUntrackedChanges();
            });
            //#endregion

            //#region commands
            result.toggleEditingCommand = ko.command({
                execute: function() {
                    if (result.isEditing()) result.endEditingCommand.execute();
                    else result.beginEditingCommand.execute();
                }
            });
            result.beginEditingCommand = ko.command({
                canExecute: function() { return options.canEdit() && !isEditing(); },
                execute: function() {
                    beginEditing();
                    beginEditingCallback();
                },
                log_name: name + ".beginEditingCommand",
            });
            result.endEditingCommand = ko.command({
                canExecute: function() { return options.canEndEditing() && isEditing(); },
                execute: function() { endEditing(); },
                log_name: name + ".endEditingCommand",
            });
            result.revertEditsCommand = ko.command({
                execute: function() {
                    rollbackEdits();
                    revertChangesCallback();
                },
                log_name: name + ".revertEditsCommand",
            });
            result.cancelEditsCommand = ko.command({
                execute: function() {
                    result.revertEditsCommand.execute();
                    result.endEditingCommand.execute();
                },
                canExecute: function() { return result.hasChanges() || isEditing(); },
                log_name: name + ".cancelEditsCommand",
            });
            result.saveEditsCommand = ko.command({
                canExecute: function() { return options.canSave(); },
                execute: function() {
                    commitEdits();
                    commitChangesCallback();
                },
                log_name: name + ".saveEditsCommand",
            });
            //#endregion

            result.refreshState = cacheState;
            result.defaultOptions = defaultOptions;

            //#region init
            if (options.initializeAsEditing) {
                beginEditing();
            }
            cacheState();
            //#endregion

            //#region debug
            if (ko.DEBUG) {
                result.LOG = ko.observable(options.enableLogging);

                if (result.LOG()) {
                    cachedState.subscribe(function() {
                        console.log(name + ' > cache value changed.');
                        console.log({
                            cache: cachedState(),
                            currentHash: currentHash(),
                            isEditing: isEditing(),
                            isDirty: isDirty(),
                        });
                    });
                    currentHash.subscribe(function() {
                        console.log(name + ' > current hash value changed.');
                        console.log({
                            cache: cachedState(),
                            currentHash: currentHash(),
                            isEditing: isEditing(),
                            isDirty: isDirty(),
                        });
                    });
                }
            }
            //#endregion

            result.dispose = function() {
                objectToTrack(null);
                objectToTrack = null;
                cacheMapping = null;
                customRevertFunctions = null;
                ignore = null;
                trackedObject = null;
                revertUntrackedChanges = null;
                hasUntrackedChanges = null;
                commitUntrackedChanges = null;
                revertChangesCallback = null;
                commitChangesCallback = null;
                beginEditingCallback = null;
                endEditingCallback = null;
                result = null;
                isInitiallyDirty = null;
                cachedState = null;
                currentHash = null;
                preventDirtyCheck = null;

                isEditing = null;
                isDirty = null;
            }

            return result;

            //#region private functions
            function beginEditing() {
                if (isEditing() === true) return;
                isEditing(true);
            }

            function endEditing() {
                if (!isEditing()) return;
                isEditing(false);
                endEditingCallback();
            }

            function commitEdits() {
                commitUntrackedChanges();
                cacheState();
                endEditing();
                isInitiallyDirty(false);
            }

            function rollbackEdits() {
                preventDirtyCheck(true);
                var cache = deserializeCache();
                recursiveRollback(trackedObject, cache);

                revertUntrackedChanges();
                cacheState();
                preventDirtyCheck(false);

                function recursiveRollback(current, original) {
                    for (var propName in current) {
                        var currentProp = current[propName];
                        if (isEditStateManager(currentProp)) continue;

                        var currentValue = ko.utils.unwrapObservable(currentProp);
                        var originalValue = ko.utils.unwrapObservable(original[propName]);

                        if (typeof currentValue === "function" || currentValue === originalValue) continue;

                        setValue(originalValue);

                        if (ko.DEBUG) {
                            var newValue = ko.utils.unwrapObservable(currentProp);
                            if (original.hasOwnProperty(propName) && newValue !== originalValue && options.customRevertFunctions[propName] == undefined) {
                                console.warn('Revert failure:');
                                console.debug({ message: 'Revert property \"' + propName + '\" failed', 'expected': originalValue, 'actual': newValue });
                            }
                        }
                    }

                    function isEditStateManager(prop) {
                        return prop === result;
                    }

                    function setValue(value) {
                        var revertFn = customRevertFunctions[propName] || defaultRevertFn;

                        if (revertFn.length > 1) revertFn(value, current[propName]);
                        else revertFn(value);
                        
                        function defaultRevertFn(val) {
                            if (ko.isObservable(currentProp)) currentProp(val);
                            else current[propName] = val;
                        }
                    }
                }
            }

            function cacheState() {
                cachedState(serialize(trackedObject));
            }

            function serialize(cacheObject) {
                return ko.toJSON(cacheObject);
            }

            function buildTrackedObject() {
                // Only observable properties are tracked by default. 
                // This may  be  overridden by supplying an 'include' option, however, 
                // the non-observable objects will not trigger changes to the tracked object.
                trackedObject = {};

                for (var prop in objectToTrack) {
                    if (!isExcluded(prop, objectToTrack) && (isObservable(objectToTrack[prop]) || isIncluded(prop))) {
                        trackedObject[prop] = objectToTrack[prop];
                    }
                }

                return trackedObject;
            }

            function deserializeCache() {
                var cache = ko.utils.parseJson(cachedState());
                var hydrated = {};

                for (var prop in cache) {
                    hydrated[prop] = deserializeProperty(prop, cache);
                }

                return hydrated;
            }

            function deserializeProperty(propName, object) {
                return typeof cacheMapping[propName] == "function"
                    ? cacheMapping[propName].call(null, object[propName])
                    : object[propName];
            }

            function isObservable(prop) {
                return ko.isWriteableObservable
                    ? ko.isWriteableObservable(prop)
                    : (typeof prop == "function" && prop.name == "observable");
            }

            function isIncluded(propName) {
                return ko.utils.arrayFirst(include, function(p) {
                    return p === propName;
                }) !== null;
            }

            function isExcluded(propName, obj) {
                var prop = obj[propName];
                return ko.utils.arrayFirst(ignore, function(p) {
                    return typeof p === "string"
                        ? p === propName
                        : p === prop;
                }) !== null;
            }

            function isArray(obj) {
                return obj instanceof Array;
                // Reece -- this wasn't working for me.
                //return Object.prototype.toString(obj) === '[object Array]';
            }
            
//#endregion
        };

        ko.EditStateManager.defaultOptions = (function() {
            var defaultOptions = {
                include: [],
                ignore: ['__ko_mapping__'],
                initializeAsEditing: false,
                isInitiallyDirty: false,
                canSave: function() { return true; },
                name: "[unnamed_esm]",
                canEdit: function() { return true; },
                canEndEditing: function() { return true; },
            };

            return defaultOptions;
        })();

    }
}());