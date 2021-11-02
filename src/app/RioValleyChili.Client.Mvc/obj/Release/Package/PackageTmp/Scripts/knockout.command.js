// By: Hans Fjällemark and John Papa
// https://github.com/CodeSeven/KoLite

(function() {
    if (require) {
        define(['knockout'], factory);
    } else factory(ko);

    function factory(ko) {

        var ResultsFactory = (function () {
            var factory = {
                init: function (target) {
                    if (target == undefined) throw new Error("Target is undefined.");

                    var results = new Results();
                    target.pushError = results.pushError;
                    target.pushSuccess = results.pushSuccess;
                    target.hasErrors = results.hasErrors;
                    target.clearResults = results.clear;
                    target.results = {
                        success: results.success,
                        status: results.status,
                        errors: results.errors,
                        messages: ko.computed(results.results),
                    };
                    target.getAllErrors = getErrorsFromTarget;

                    function getErrorsFromTarget(cmd) {
                        cmd = cmd || target;
                        var targetErrors = cmd.results.errors() || [];
                        ko.utils.arrayMap(cmd.modulesToExecute(), function (module) {
                            targetErrors = targetErrors.concat(getErrorsFromTarget(module));
                        });
                        return targetErrors;
                    }
                }
            };

            return factory;

            function Results() {
                var _results = ko.observableArray([]);

                var result = {
                    results: ko.computed(function () { return _results(); }),
                    pushError: function (message) {
                        pushResult(ko.composableCommand.resultStatus.error, message);
                    },
                    pushSuccess: function (message) {
                        pushResult(ko.composableCommand.resultStatus.success, message);
                    },
                    clear: function () {
                        _results([]);
                    },
                    errors: ko.computed(function () {
                        return ko.utils.arrayFilter(_results(), function (msg) {
                            return msg.state === ko.composableCommand.resultStatus.error;
                        });
                    }),
                };

                result.hasErrors = ko.computed(function () {
                    return this.errors().length > 0;
                }, result);

                result.status = ko.computed(function () {
                    return this.hasErrors()
                        ? ko.composableCommand.resultStatus.error
                        : ko.composableCommand.resultStatus.success;
                }, result);

                result.success = ko.computed(function () {
                    return !this.hasErrors();
                }, result);

                function pushResult(state, message) {
                    _results.push({
                        state: state,
                        message: message,
                        success: state === ko.composableCommand.resultStatus.success,
                        error: state === ko.composableCommand.resultStatus.error,
                    });
                }

                return result;
            };
        }());

        var trueDelegate = function () { return true; }

        ko.composableCommand = function (options) {

            options = options || {};
            options.log_name = options.log_name || "Unnamed";

            if (options.requiresModuleExecution !== undefined) console.warn("The requiresModuleExecution option is not valid. Replace with moduleDependency.");

            var _modules = ko.observableArray([]);
            var executeDelegate = options.execute,
                moduleDependencyDelegate = getModuleDependencyDelegate(
                    options.moduleDependency || ko.composableCommand.moduleDependency.alwaysExecute).bind(this),
                shouldExecuteDelegate = options.shouldExecute || trueDelegate,
                baseCanExecuteDelegate = options.canExecute || trueDelegate,
                hasOwnExecution = executeDelegate != undefined;
            var modulesFromOptions = ko.computed(function () {
                var filtered = ko.utils.arrayFilter(
                    ko.utils.unwrapObservable(options.children),
                    function (cmd) { return cmd != undefined; }) || [];
                return ko.utils.arrayMap(filtered, prepareChildCommand);
            });
            var modules = ko.computed(function () {
                return modulesFromOptions().concat(_modules());
            });
            var activeModules = ko.computed(function () {
                return ko.utils.arrayFilter(modules(), function (cmdModule) {
                    return !cmdModule.ignore();
                });
            });
            var hasActiveModules = ko.computed(function () { return activeModules().length > 0; });
            var activeModulesAreValid = ko.computed(function () {
                return ko.utils.arrayFirst(activeModules(), function (m) {
                    return m.canExecute() !== true;
                }, self) == null;
            });
            var baseCanExecute = ko.computed(function () {
                return baseCanExecuteDelegate();
            });
            var hasLogicToExecute = ko.computed(function () {
                return hasOwnExecution || hasActiveModules();
            });

            var isActive = ko.computed(function () {
                return hasLogicToExecute() && shouldExecuteDelegate();
            });
            var isValid = ko.computed(function () {
                return baseCanExecute() && moduleDependencyDelegate(activeModules()) && activeModulesAreValid();
            });

            var cmd = ko.asyncCommand({
                canExecute: function (isExecuting) {
                    return !isExecuting && isValid();
                },
                execute: function (arg1, arg2, complete) { // the arguments are necessary to preserve the parameters in the asyncCommand
                    if (!complete) throw new Error("Please provide a 'completed' callback function argument.");

                    if (!isValid()) throw new Error("The command being execute is not valid.");
                    if (ko.DEBUG && !isValid()) {
                        console.error("Attempted execution of invalid command:");
                        console.debug(cmd);
                    }

                    if (!isActive()) {
                        if (ko.DEBUG) { console.log(options.log_name + " was ignored"); }
                        return;
                    }

                    cmd.clearResults();

                    var args = [];
                    if (executeDelegate && executeDelegate.length > 1) {
                        args.push(arg1);
                    }

                    if (executeDelegate && executeDelegate.length > 2) {
                        args.push(arg2);
                    }

                    var executionComplete = false;
                    var executingModules = [];
                    var moduleSubscriptions = [];

                    ko.utils.arrayForEach(activeModules(), function (cmdModule) {

                        // The composableCommand's complete callback will only be executed after
                        // after the all commands--this parent and it's children--have been completed.
                        if (cmdModule.isExecuting) {
                            executingModules.push(cmdModule);
                            moduleSubscriptions.push(
                                cmdModule.isExecuting.subscribe(onIsExecutingChanged, cmdModule));
                        }

                        cmdModule.execute.call(cmdModule);
                    });

                    if (executeDelegate) {
                        args.push(onExecuteDelegateComplete);
                        executeDelegate.apply(this, args);
                    } else {
                        onExecuteDelegateComplete();
                    }

                    // private functions

                    function allModulesComplete() {
                        return executingModules.length === 0;
                    }
                    function onIsExecutingChanged(isExecuting) {
                        var mod = this;
                        if (!isExecuting) {
                            var index = ko.utils.arrayIndexOf(executingModules, mod);
                            if (index > -1) {
                                moduleSubscriptions[index].dispose();
                                executingModules.splice(index, 1);
                            }
                            onCommandExecutionComplete();
                        }
                    }
                    function onExecuteDelegateComplete() {
                        executionComplete = true;
                        onCommandExecutionComplete();
                    }
                    function onCommandExecutionComplete() {
                        if (executionComplete && allModulesComplete()) {
                            if (options.executionCompleteCallback) options.executionCompleteCallback();
                            complete();
                        }
                    }

                }
            });

            cmd.ignore = ko.computed(function () {
                return !isActive();
            });
            cmd.modulesToExecute = activeModules;
            cmd.isExecutionValid = isValid;

            if (ko.DEBUG) {
                cmd.__debug = {
                    name: options.log_name,
                    activeChildren: activeModules,
                    inactiveChildren: ko.computed(function () {
                        return ko.utils.arrayFilter(modules(), function (m) {
                            return ko.utils.arrayFirst(
                                activeModules(),
                                function (active) { return active === m; }) === null;
                        });
                    }),
                    allModules: modules,
                    executeDelegate: executeDelegate,
                    isActive: isActive,
                    isValid: isValid,
                };
            }


            ResultsFactory.init(cmd);

            //#region functions

            cmd.addCommand = addChildCommand;
            cmd.removeCommand = function (commandModule) {
                var index = ko.utils.arrayIndexOf(_modules(), commandModule);
                if (index > -1) {
                    _modules.splice(index, index + 1);
                }
            };
            cmd.clearCommands = function () {
                modules([]);
            };

            //#endregion

            //#region debug

            if (ko.DEBUG === true) {
                var debugIndex = 0;
                cmd.LOG = ko.observable(false);
                cmd.debugObject = {
                    canExecute: cmd.canExecute(),
                    ignore: cmd.ignore(),
                    moduleDependencySatisfied: moduleDependencyDelegate(activeModules()),
                    modules: modules(),
                    isExecuting: cmd.isExecuting ? cmd.isExecuting() : 'n/a'
                };
                cmd.debugDisplay = function () {
                    var debugLog = cmd.log_name + " (Log #" + debugIndex++ + ")";
                    debugLog += " { canExecute: " + cmd.canExecute() + ", ignore=" + cmd.ignore()
                        + ", commandModules=" + modules().length + ", moduleDependencySatisfied=" + moduleDependencyDelegate(activeModules())
                        + ", baseCanExecute=" + baseCanExecute() + "}\n";
                    debugLog += "Command Modules: \n";
                    var count = 0;
                    ko.utils.arrayForEach(modules(), function (mod) {
                        count++;
                        debugLog += count.toString() + ". " + mod.log_name + " >> canExecute=" + mod.canExecute() + ", ignore=" + mod.ignore() + "\n";
                    });
                    return debugLog;
                };
                cmd.logDebug = function () {
                    var debugLog = cmd.log_name + " (Log #" + debugIndex++ + "): ";
                    console.debug(debugLog);
                    console.debug(cmd.debugObject);
                };
                modules.subscribe(function () {
                    if (cmd.LOG() === true) {
                        cmd.logDebug();
                    }
                });
                cmd.canExecute.subscribe(function () {
                    if (cmd.LOG()) {
                        cmd.logDebug();
                    }
                });
            }

            //#endregion

            return cmd;

            function addChildCommand(childCommand) {
                if (prepareChildCommand(childCommand)) _modules.push(childCommand);
            }

            function prepareChildCommand(childCmd) {
                if (!childCmd) return null;
                if (!childCmd.execute) throw new Error("The desired command does not define an execute function.");
                childCmd.ignore = childCmd.ignore || ko.computed(function () { return false; });
                return childCmd;
            }

            function each(fn) {
                return function (items) {
                    if (!items) return;
                    if (Object.prototype.toString.call(items) !== '[object Array]') {
                        items = [items];
                    }
                    ko.utils.arrayForEach(items, function (i) { fn.apply(null, [i]); });
                };
            }

            function getModuleDependencyDelegate(option) {
                switch (typeof option) {
                    case "function": return option;
                    case "string": return getShouldExecuteDelegateFromDependencyOption(option);
                    default: throw new Error("The supplied option is not valid.");
                }

                function getShouldExecuteDelegateFromDependencyOption(opt) {
                    switch (opt) {
                        case ko.composableCommand.moduleDependency.allModulesRequired:
                            return allModulesRequiredDelegate;
                        case ko.composableCommand.moduleDependency.atLeastOneModuleRequired:
                            return atLeastOneModuleRequiredDelegate;
                        case ko.composableCommand.moduleDependency.alwaysExecute:
                            return shouldAlwaysExecute;
                        default: throw new Error("The supplied option is not valid.");
                    }
                }
            }

            function allModulesRequiredDelegate(modules) {
                return ko.utils.arrayFirst(modules, function (mod) {
                    return !mod.canExecute();
                }) || true;
            }

            function atLeastOneModuleRequiredDelegate(modules) {
                return modules.length > 0;
            }

            function shouldAlwaysExecute() {
                return true;
            }
        };

        ko.composableCommand.moduleDependency = {
            allModulesRequired: '*',
            atLeastOneModuleRequired: '.',
            alwaysExecute: '?',
        };

        ko.composableCommand.resultStatus = {
            success: 1,
            error: -1,
            none: 0,
        };

        ko.command = function (options) {
            var self = ko.observable(),
                canExecuteDelegate = options.canExecute,
                executeDelegate = options.execute;

            self.canExecute = ko.computed(function () {
                return canExecuteDelegate ? canExecuteDelegate() : true;
            });

            self.execute = function (arg1, arg2) {
                // Needed for anchors since they don't support the disabled state
                if (!self.canExecute()) return

                return executeDelegate.apply(this, [arg1, arg2]);
            };

            if (ko.DEBUG) {
                self.__debug = {
                    executeDelegate: executeDelegate,
                    canExecuteDelegate: canExecuteDelegate,
                };
            }

            return self;
        };

        ko.asyncCommand = function (options) {
            var
                self = ko.observable(),
                canExecuteDelegate = options.canExecute,
                executeDelegate = options.execute,

                completeCallback = function () {
                    self.isExecuting(false);
                };

            self.isExecuting = ko.observable();

            self.canExecute = ko.computed(function () {
                return canExecuteDelegate ? canExecuteDelegate(self.isExecuting()) : !self.isExecuting();
            });

            self.execute = function (arg1, arg2) {
                // Needed for anchors since they don't support the disabled state
                if (!self.canExecute()) return

                var args = []; // Allow for these arguments to be passed on to execute delegate

                if (executeDelegate.length >= 2) {
                    args.push(arg1);
                }

                if (executeDelegate.length >= 3) {
                    args.push(arg2);
                }

                args.push(completeCallback);
                self.isExecuting(true);
                return executeDelegate.apply(this, args);
            };

            return self;
        };

        ko.utils.wrapAccessor = function (accessor) {
            return function () {
                return accessor;
            };
        };

        ko.bindingHandlers.command = {
            init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                var
                    value = valueAccessor(),
                    commands = value.execute ? { click: value } : value,

                    isBindingHandler = function (handler) {
                        return ko.bindingHandlers[handler] !== undefined;
                    },

                    initBindingHandlers = function () {
                        for (var command in commands) {
                            if (!isBindingHandler(command)) {
                                continue;
                            };

                            ko.bindingHandlers[command].init(
                                element,
                                ko.utils.wrapAccessor(commands[command].execute),
                                allBindingsAccessor,
                                viewModel,
                                bindingContext
                            );
                        }
                    },

                    initEventHandlers = function () {
                        var events = {};

                        for (var command in commands) {
                            if (!isBindingHandler(command)) {
                                events[command] = commands[command].execute;
                            }
                        }

                        ko.bindingHandlers.event.init(
                            element,
                            ko.utils.wrapAccessor(events),
                            allBindingsAccessor,
                            viewModel,
                            bindingContext);
                    };

                initBindingHandlers();
                initEventHandlers();
            },

            update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                var commands = valueAccessor();
                var canExecute = commands.canExecute;

                if (!canExecute) {
                    for (var command in commands) {
                        if (commands[command].canExecute) {
                            canExecute = commands[command].canExecute;
                            break;
                        }
                    }
                }

                if (!canExecute) {
                    return;
                }

                ko.bindingHandlers.enable.update(element, canExecute, allBindingsAccessor, viewModel, bindingContext);
            }
        };
    }
}())