require('floatthead/dist/jquery.floatThead.js');
require('App/bindings/ko.bindingHandlers.datePicker');

define(['ko'], function(ko) {
    ko.bindingHandlers.hidden = {
        update: function (element, valueAccessor) {
            ko.bindingHandlers.visible.update(element, function () {
                return !ko.utils.unwrapObservable(valueAccessor());
            });
        }
    }
    ko.bindingHandlers.preventBubble = {
        init: function (element, valueAccessor) {
            var eventName = ko.utils.unwrapObservable(valueAccessor());
            ko.utils.registerEventHandler(element, eventName, function (event) {
                event.cancelBubble = true;
                if (event.stopPropagation) {
                    event.stopPropagation();
                }
            });
        }
    };

    ko.bindingHandlers.fixed = {
      init: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
        var options = ko.unwrap( allBindings().decimalOptions ) || {};
        var precision = options.precision || ko.bindingHandlers.fixed.defaultPrecision;

        if ( $( element ).is('input') ) {
          var hiddenObservable = valueAccessor();

          if ( ko.isObservable( hiddenObservable ) && hiddenObservable() === '' ) {
            hiddenObservable( null );
          }

          var transform = ko.pureComputed({
            read: hiddenObservable,
            write: function( value ) {
              if ( value === '' ) {
                hiddenObservable( null );
              } else {
                var num = parseFloat( value.replace( /[^\d\.\-]/g, '' ) );
                hiddenObservable( num.toFixed( precision ) );
              }
            }
          });

          ko.bindingHandlers.value.init( element, function() { return transform; }, allBindings );
        }
      },
      update: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
        var value = ko.unwrap( valueAccessor() );
        var options = ko.unwrap( allBindings().decimalOptions );
        var precision = options.precision || ko.bindingHandlers.fixed.defaultPrecision;
        var formattedValue = parseFloat( value ).toFixed( precision );

        if ( !isNaN( formattedValue ) ) {
          $( element ).val( options.commas ? ko.bindingHandlers.fixed.withComma( formattedValue ) : formattedValue );
        } else {
          $( element ).val( '' );
        }
      },
      defaultPrecision: 2,
      withComma: function( value ) {
        var vals = value.split('.');
        vals[0] = Number( vals[0] ).toLocaleString();

        return vals.join('.');
      }
    };

    /**
      * Bootstrap Modal Binding
      * Bind to modal wrapper with class `.modal`
      * Refer to http://getbootstrap.com/javascript/#modals for modal structure
      *
      * @param {boolean} valueAccessor - Toggles modal visibility
      */
    ko.bindingHandlers.modal = {
      init: function (element, valueAccessor) {
        $(body).append($(element));
        $(element).remove();
        $(element).modal({
          show: false
        });

        var value = valueAccessor();
        if (ko.isObservable(value)) {
          $(element).on('hide.bs.modal', function() {
            value(false);
          });
        }
      },
      update: function (element, valueAccessor) {
        var value = valueAccessor();
        if (ko.utils.unwrapObservable(value)) {
          $(element).modal('show');
        } else {
          $(element).modal('hide');
        }
      }
    };

    /**
      * @param {Object} valueAccessor - Value to monitor for update function
      */
    ko.bindingHandlers.floatThead = {
      init: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
        if ( element.nodeName.toLowerCase() !== "table" ) {
          throw new Error('The floatThead binding must be set on a table element');
        }

        var $tableSelector = $( element );
        var value = valueAccessor();
        var valueUnwrapped = ko.unwrap( value );

        $tableSelector.parent().addClass('sticky-head-container');
        $tableSelector.addClass('sticky-head');

        if ( ko.isObservable( value ) && (valueUnwrapped == null || (Array.isArray( valueUnwrapped) && value.length === 0)) ) {
          var valueSubscription = value.subscribe( function( newValue ) {
            floatThead( $tableSelector );
            valueSubscription.dispose();
          });
        } else {
          floatThead( $tableSelector );
        }

        ko.utils.domNodeDisposal.addDisposeCallback( element, function() {
          $tableSelector.floatThead('destroy');
        });

        function floatThead( $tableSelector ) {
          $tableSelector.floatThead({
            scrollContainer: function ( $table ) {
              return $table.closest('.sticky-head-container');
            }
          });
        }
      },
      update: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
        var value = valueAccessor();
        var valueUnwrapped = ko.unwrap(value);

        $( element ).floatThead('reflow');
      }
    };

    ko.bindingHandlers.loadingMessage = {
      init: function (element, valueAccessor) {
        $('body').append($(element));
        $(element).hide();
      },
      update: function(element, valueAccessor) {
        var value = valueAccessor();
        var valueUnwrapped = ko.unwrap(value);

        if (!!valueUnwrapped) {
          $(element).fadeIn();
        } else {
          $(element).fadeOut();
        }
      }
    };

    /**
      * @deprecated Use modal instead
      */
    ko.bindingHandlers.dialog = {
        init: function (element, valueAccessor, allBindings, bindingContext) {
            console.debug('dialog has been deprecated, use modal instead');
            var defaultConfig = {
                    modal: true,
                },
                $element = $(element),
                value = valueAccessor(),
                commands = parseCommands();

            initDialog();


            // prevent duplicate binding error?
            ko.cleanNode($element);
            $element.removeAttr('data-bind');
            $element.children(function() {
                this.removeAttr('data-bind');
            });

            var dialogDestroyed = false;
            if (ko.isObservable(value)) {
                var valueSubscriber = value.subscribe(function (val) {
                    if (!dialogDestroyed) $element.dialog(val ? "open" : "close");
                });

                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                    if ($element.dialog('option')) $element.dialog("destroy");
                    dialogDestroyed = true;
                    valueSubscriber.dispose && valueSubscriber.dispose();
                });
            }

            attachKoCommands();

            function initDialog() {
                // Creates empty function for any configured buttons.
                // This will cause the buttons to be displayed while allowing
                // execution to be deferred to the supplied command object.

                var options = ko.utils.extend(allBindings() || {}, defaultConfig);

                var config = {
                    modal: options.modal,
                    height: options.height,
                    width: options.width,
                    position: options.position,
                    buttons: { },
                    close: options.close || options.cancelCommand,
                    title: options.title,
                    autoOpen: ko.utils.peekObservable(value) && true || false,
                    dialogClass: options.cancelCommand ? 'no-close' : '',
                };

                for (var prop in commands) {
                    if (commands.hasOwnProperty(prop))
                        config.buttons[prop] = empty;
                }

                $element.dialog(config);

                function empty() { }
            }

            function parseCommands() {
                var exports = {},
                    bindings = allBindings() || {};

                parseCommand(bindings['cancelCommand'], 'cancelCommand', 'Cancel');
                parseCommand(bindings['okCommand'], 'okCommand', 'Ok');

                var customCommands = getCustomCommands();
                for (var prop in customCommands) {
                    if (customCommands.hasOwnProperty(prop))
                        parseCommand(customCommands[prop], prop, prop);
                }
                return exports;

                function getCustomCommands() {
                    return allBindings().customCommands || allBindings().customCommand || [];
                }
                function parseCommand(cmd, bindingName, mapToButtonName) {
                    if (!cmd) return;
                    if (!cmd.execute) {
                        cmd = ko.command({
                            execute: cmd
                        });
                    }
                    exports[mapToButtonName || bindingName] = cmd;
                }


            }

            function attachKoCommands() {
                var buttonFunctions = $element.dialog("option", "buttons");
                var newButtonsConfig = [];
                for (var funcName in buttonFunctions) {
                    for (var cmdName in commands) {
                        if (cmdName == funcName) {
                            var buttons = $(".ui-dialog-buttonpane button:contains('" + cmdName + "')");

                            $.each(buttons, function (index) {
                                var command = commands[cmdName];

                                ko.bindingHandlers.command.init(
                                    buttons[index],
                                    ko.utils.wrapAccessor(command),
                                    allBindings,
                                    null,
                                    bindingContext);

                                // remove click functionality from the jQuery UI element
                                newButtonsConfig.push({
                                    text: cmdName,
                                    click: empty,
                                });
                            });
                            break;
                        }
                    }
                }

                function empty() { }
            }
        },
    };

    ko.bindingHandlers.cancelKey = {
        init: function (element, valueAccessor, allBindings, viewModel) {
            var delegate = ko.utils.unwrapObservable(valueAccessor());

            if (delegate && typeof delegate !== 'function' && typeof delegate.execute === "function") {
                delegate = delegate.execute;
            }
            if (delegate == undefined) return;

            var cancelKeyCode = 27;
            var elementToRegister = element;

            if (attachToWindow()) {
                var conditionFn = function () { return $(element).is(':visible'); };
                elementToRegister = window;
            }
            ko.utils.registerEventHandler(elementToRegister, 'keydown', buildEventHandler(conditionFn));

            function attachToWindow() {
                var bindings = ko.utils.unwrapObservable(allBindings);
                return bindings && ko.utils.unwrapObservable(bindings.attachToWindow) === true;
            }

            function buildEventHandler(conditionalFn) {
                conditionalFn = conditionalFn || function () { return true; };
                return function (event) {
                    if (event.keyCode == cancelKeyCode && conditionalFn()) {
                        executeCancel(event);
                        event.cancelBubble = true;
                        if (event.stopPropagation) event.stopPropagation();
                    }
                };
            }

            function executeCancel(event) {
                event.preventDefault();
                event.target.blur();
                delegate.call(viewModel);
            }
        }
    };

    ko.bindingHandlers.onblur = {
        init: function (element, valueAccessor) {
            var fn = valueAccessor();
            if (fn && fn.execute) fn = commandWrapper.bind(fn);
            ko.utils.registerEventHandler(element, 'blur', fn);

            function commandWrapper() {
                this.execute();
            }
        }
    };

    ko.bindingHandlers.growToWindowHeight = {
        init: function (element, valueAccessor) {
            var $element = $(element);
            var bindings = $.extend({}, ko.bindingHandlers.growToWindowHeight.DEFAULT_OPTIONS, ko.unwrap(valueAccessor()) || {});

            var $windowHeight = $(window).height();
            var viewportHeight = document.body && document.body.clientHeight
                ? Math.min(document.body.clientHeight, $windowHeight)
                : $windowHeight;
            var windowHeight = viewportHeight - bindings.offset;
            $element.height(windowHeight);
            $element.css('overflow', 'auto');
        },
        DEFAULT_OPTIONS: {
            offset: 0
        }
    };

    ko.bindingHandlers.maxHeight = {
        init: function (element, valueAccessor, allBindings) {
            var $element = $(element);
            $element.addClass('fullWindowHeight');
            constrainHeight.call($element);

            var value = valueAccessor();
            if (ko.isObservable(value)) {
                var sub = value.subscribe(function() {
                    setupStickyTableElements();
                    sub.dispose();
                });
            } else setupStickyTableElements();

            function setupStickyTableElements() {
                prepareTables(element, valueAccessor(), allBindings());
            }
        },
    };

    function constrainHeight() {
        var windowHeight = $(window).height();
        this.height(windowHeight);
        //this.css('max-height', windowHeight);
        this.css('overflow-y', 'scroll');
        this.css('overflow-x', 'scroll');
    }

    function prepareTables(element, value, allBindings) {
        var $element = $(element);
        var opts = allBindings || {};

        if ($element.is('table')) {
            $element = $(element).wrap("<div><div>").parent();
        }

        initStickyTableBinding($element, value, opts, 'stickyTableHeaders');
        //initStickyTableBinding($element, value, opts, 'stickyTableFooters');
    }

    function initStickyTableBinding(element, value, opts, bindingName) {
        var valueAccessor = opts[bindingName]
                ? ko.utils.wrapAccessor(opts[bindingName])
                : ko.utils.wrapAccessor(value || true);

        var $element = $(element);

        var template = getTemplatedChild();
        if (template) {
            if (!opts[bindingName]) return;
            opts.dependsOn = template;
            var bindings = ko.utils.wrapAccessor(opts);
            ko.bindingHandlers[bindingName].init(template, valueAccessor, bindings);
        } else {
            opts.parent = $element;
            $element.find(opts[bindingName] || 'table').each(function () {
                var $this = $(this);
                ko.bindingHandlers[bindingName].init($this, valueAccessor, ko.utils.wrapAccessor(opts));
                removeBinding($this, bindingName);
            });
        }

        function getTemplatedChild() {
            var child = getChild();
            if (!child) return null;
            var childContext = ko.contextFor(child);
            if (!childContext) return null;

            var childBindings = ko.bindingProvider.instance.getBindings(child, childContext);
            return childBindings && childBindings.template ? child : null;

            function getChild() {
                return $element.children(':first')[0]
                    || getVirtualElementChild();
            }
            function getVirtualElementChild() {
                var vChild = ko.virtualElements.firstChild($element[0]);
                return vChild && ko.virtualElements.nextSibling(vChild);
            }
        }

        function removeBinding(table, binding) {
            var dataBind = table.attr('data-bind');
            if (dataBind) {
                var regex = new RegExp(binding + "\:\s?\w+\W?\s?", "i");
                dataBind = dataBind.replace(regex, "");
                table.attr('data-bind', dataBind);
            }
        }
    }

    ko.bindingHandlers.fixCvpOverlay = {
        init: function (element, valueAccessor) {
            var $container = $(element).wrap('<div />').parent();
            //ko.bindingHandlers.fixCvpOverlay.update(element,valueAccessor);
        },
        update: function (element, valueAccessor) {
            valueAccessor().notifySubscribers(); // fix initial overlay
            ko.utils.unwrapObservable(valueAccessor());
            var $cvp = $("#cvp");
            var cvpWidth = $cvp.outerWidth();
            var $element = $(element);
            var inventoryTableWidth = $element.width();

            // When element contains an enumerated child (such as a foreach binding), the
            // width function returns 0. This hacky little fix will set a default width.
            if (inventoryTableWidth == 0) {
                inventoryTableWidth = 5000; // default width
            }

            var $container = $element.parent();
            $container.width(inventoryTableWidth).css({ "padding-right": cvpWidth + 85 });
        }
    };

    ko.bindingHandlers.undo = {
        init: function (element, valueAccessor, allBindings, viewModel) {
            var bindings = {};
            var trackedBindingNames = ['value'];
            var isEditing = ko.computed(function () {
                return ko.utils.unwrapObservable(valueAccessor());
            });
            var elementBindings = ko.bindingProvider.instance.getBindings(element, ko.contextFor(element));
            ko.utils.arrayForEach(trackedBindingNames, function (binding) {
                if (elementBindings[binding]) {
                    bindings[binding] = elementBindings[binding];
                }
            });

            for (var boundProp in bindings) {
                initializeTracking(bindings[boundProp]);
            }

            function revert(propAccessor) {
                var initalValue = propAccessor.changeHistory()[0];
                propAccessor(initalValue);
            }

            function initializeTracking(propAccessor) {
                propAccessor.changeHistory = ko.observableArray([propAccessor()]);

                propAccessor.subscribe(function (newVal) {
                    propAccessor.changeHistory.push(newVal);
                });

                setupRevertTrigger(propAccessor);
            }

            isEditing.subscribe(function (newVal) {
                $cancelButton.each(function (index, button) {
                    ko.bindingHandlers.visible.update(
                        button,
                        ko.utils.wrapAccessor(newVal),
                        allBindings,
                        data
                    );
                });
            });

            function setupRevertTrigger(propAccessor) {
                // eventually, we'll enable actual undo/redo stepping but for now, we just
                // handle both as a revert function.
                var revertCommand = ko.command({
                    execute: function () {
                        revert(propAccessor);
                    },
                    canExecute: function () {
                        return propAccessor.changeHistory().length > 1;
                    }
                });
                propAccessor.revertCommand = revertCommand;
                var trigger = allBindings().undoTrigger || allBindings().revertTrigger;

                $(trigger).each(function (index, button) {
                    ko.bindingHandlers.command.init(
                        button,
                        ko.utils.wrapAccessor(revertCommand),
                        allBindings,
                        viewModel
                    );
                });
            }
        }
    };

    ko.bindingHandlers.pageData = {
        update: function (element, valueAccessor) {
            ko.utils.unwrapObservable(valueAccessor());
            $(element).hide().fadeIn(500);
        }
    };

    ko.bindingHandlers.editableContent = {
        init: function (element, valueAccessor, allBindings, data) {
            var savedState = ko.observable();
            var isEditing = ko.computed(function () {
                return ko.utils.unwrapObservable(valueAccessor());
            });
            var $element = $(element);
            var $cancelButton = $(allBindings().cancelTrigger);
            var $masterCancelButton = $(allBindings().masterCancelTrigger);

            if (!isEditing()) { $element.attr("readonly", "readonly"); }

            ko.bindingHandlers.undo.init(
                element,
                ko.utils.wrapAccessor(function () { return true; }),
                ko.utils.wrapAccessor({ revertTrigger: $cancelButton ? $cancelButton[0] : undefined }),
                data);

            ko.bindingHandlers.click.init(
                element,
                ko.utils.wrapAccessor(beginEdit),
                ko.utils.wrapAccessor({}),
                data);

            //todo: handle blur events (and allow disabling the blur handlers)

            //todo: 1. prevent bubbling, 2. enable canceling when cancel button is not supplied
            ko.bindingHandlers.cancelKey.init(
                element,
                ko.utils.wrapAccessor(function () { $cancelButton.click(); }),
                ko.utils.wrapAccessor({ keydownBubble: false }),
                data
            );


            setupCancelButtons();

            function setupCancelButtons() {
                var cancelCommand = allBindings().cancelEditsCommand;
                if ($cancelButton.length > 0) {
                    $cancelButton.each(function (index, button) {
                        ko.bindingHandlers.command.init(
                            button,
                            ko.utils.wrapAccessor(cancelCommand),
                            function () { return { clickBubble: false }; },
                            data);
                    });
                }

                $masterCancelButton.each(function (index, button) {
                    var context = ko.contextFor(button);
                    if (context) {
                        var commandBinding = ko.bindingProvider.instance.getBindings(button, context).command;
                        if (commandBinding) {
                            if (typeof commandBinding.addCommand != "function") {
                                throw new Error('The masterCancelCommand is only supported with a composableCommand instance.');
                            }

                            commandBinding.addCommand(ko.command({
                                execute: function () { $cancelButton.click(); }
                            }));
                        }
                    }
                });
            }

            function beginEdit() {
                if (isEditing()) return;
                valueAccessor()(true);
                var bindings = ko.bindingProvider.instance.getBindings(element, ko.contextFor(element));
                savedState(ko.utils.unwrapObservable(bindings.value));
            }
        },
        update: function (element, valueAccessor) {
            var isEditing = ko.utils.unwrapObservable(valueAccessor());
            if (isEditing === false) {
                $(element).attr("readonly", "readonly");
            } else {
                $(element).removeAttr("readonly");
            }
        }
    };

    ko.bindingHandlers.editableContentArea = {
        init: function (element, valueAccessor, allBindings, data) {
            var $element = $(element);
            var isEditing = ko.computed(function () {
                return ko.utils.unwrapObservable(valueAccessor());
            });

            function setIsEditingValue(val) {
                valueAccessor()(val);
            }
            var inputElements = $('input', $element).not('[type="button"], [type="submit"]');
            var allChildrenEmpty = ko.computed(function () {
                var firstDefinedValue = ko.utils.arrayFirst(inputElements, function (e) {
                    var ctx = ko.contextFor(e);
                    var binding = ctx ? ko.bindingProvider.instance.getBindings(e, ctx) : undefined;
                    var value = binding ? binding.value() : undefined;
                    return value != undefined;
                });
                return firstDefinedValue === null;
            });
            var isCancelVisible = ko.computed(function () {
                return isEditing() && !allChildrenEmpty();
            });

            valueAccessor().__editableContentArea__inputElements = inputElements;

            var cancelValueAccessor = ko.utils.wrapAccessor(function () { return false; });
            cancelValueAccessor().__editableContentArea__inputElements = inputElements;

            var cancelCommand = ko.command({
                execute: function () {
                    setIsEditingValue(false);
                },
            });

            // currently requires cancelTrigger binding to be provided
            var $cancelButton = $element.find(allBindings().cancelTrigger);

            // initialize visibility
            updateCancelButtonVisibility();

            // update visibility
            isCancelVisible.subscribe(function () {
                updateCancelButtonVisibility();
            });

            allBindings().cancelTrigger = $cancelButton ? $cancelButton[0] : undefined;
            allBindings().cancelEditsCommand = cancelCommand;

            $.each(inputElements, function (index, elem) {
                ko.bindingHandlers.editableContent.init(elem, valueAccessor, allBindings, data);
            });

            function updateCancelButtonVisibility() {
                $cancelButton.each(function (index, button) {
                    ko.bindingHandlers.visible.update(
                        button,
                        function () { return isCancelVisible(); },
                        allBindings,
                        data
                    );
                });
            }
        },
        update: function (element, valueAccessor, allBindings, data) {
            var inputElements = valueAccessor().__editableContentArea__inputElements;
            var isEditing = ko.utils.unwrapObservable(valueAccessor());

            $.each(inputElements, function (index, elem) {
                ko.bindingHandlers.editableContent.update(
                    elem,
                    ko.utils.wrapAccessor(isEditing),
                    allBindings,
                    data
                );
            });
        },
    };



    function initVisibleToggle(element, dataValue, isNot) {
        var $el = $(element);
        if (isNot ? !dataValue : (dataValue && true)) {
            $el.hide();
            $el.slideDown();
        } else {
            $el.slideUp();
        }

        $el = null;
    }

    function updateVisibleToggle(element, dataValue, opts, isNot) {
        var defaults = {
            showDuration: "slow",
            hideDuration: "slow",
            speed: false,
            direction: "down",
        };

        var options = $.extend({}, defaults, opts);
        var $el = $(element);
        if (options && options.speed) options.showDuration = options.hideDuration = options.speed;

        if (isNot ? !dataValue : (dataValue && true)) {
            $el.slideDown(options.showDuration);
        } else {
            $el.slideUp(options.hideDuration);
        }
    }

    ko.bindingHandlers.slideVisible = {
        init: function (element, valueAccessor) {
            initVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), false);
        },
        update: function (element, valueAccessor, allBindings) {
            updateVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), allBindings(), false);
        }
    };

    ko.bindingHandlers.slideCollapsed = {
        init: function(element, valueAccessor) {
            initVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), true);
        },
        update: function (element, valueAccessor, allBindings) {
            updateVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), allBindings(), true);
        }
    }

    ko.bindingHandlers.popup = {
        init: function (element, valueAccessor, allBindings) {
            var $element = $(element);
            $element.addClass('popupWindow');

            var defaults = {
                attachCancelCommandToWindow: true,
            };
            var options = $.extend({}, defaults, allBindings());
            var borderWidth = parseInt($element.css('border-left-width'), 10) || 10; // parseInt trims the 'px' and returns base-10 value

            $(element).on('click', onCloseEvent);

            if (options.closePopupCommand) {
                var cancelKeyOptions = options;
                cancelKeyOptions.attachToWindow = options.attachCancelCommandToWindow;
                ko.bindingHandlers.cancelKey.init(element, ko.utils.wrapAccessor(options.closePopupCommand), ko.utils.wrapAccessor(cancelKeyOptions));
            }

            ko.bindingHandlers.slideIn.init(element, valueAccessor);

            // handle cleanup
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $element.off('click', onCloseEvent);
            });

            // private functions
            function onCloseEvent(e) {
                var hitAreaX = borderWidth + $element.position().left;
                // Reece - Replaced pageX with screenX b/c when
                // clicking a selectBox,  pageX value is relative to
                // selectbox -- not the page.
                if (e.pageX && e.pageX <= hitAreaX) {
                    if (options.closePopupCommand && typeof options.closePopupCommand.execute == "function") {
                        options.closePopupCommand.execute();
                        return;
                    }
                    if (ko.isWriteableObservable(valueAccessor())) {
                        valueAccessor()(false);
                        return;
                    }
                    ko.bindingHandlers.popup.update(element, ko.utils.wrapAccessor(false), allBindings);
                }
            }
        },
        update: function (element, valueAccessor, allBindings) {
            ko.bindingHandlers.slideIn.update(element, valueAccessor, allBindings);
        }
    };

    ko.bindingHandlers.slideIn = {
        init: function (element, valueAccessor) {
            var display = ko.utils.unwrapObservable(valueAccessor());

            var $element = $(element);
            $element.show();
            if (!display) {
                $element.hide();
                //$element.css({ left: $(window).width() });
                $element.css({ left: "100%" });
            }
        },
        update: function (element, valueAccessor) {
            var $element = $(element);
            var display = ko.utils.unwrapObservable(valueAccessor());
            if (display) {
                $element.show();
                $element.animate({ left: 0 });
            } else {
                $element.animate({ left: "100%" });
                $element.hide();
            }
        }
    };

    ko.bindingHandlers.fadeVisible = {
        init: function (element) {
            $(element).hide();
        },
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            if (value) $(element).fadeIn();
            else $(element).fadeOut();
        }
    };

    ko.bindingHandlers.stickyTableHeaders = {
        init: function (element, valueAccessor, allBindings) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            var bindings = allBindings();
            var $table;
            var options = {};
            var completed = false;
            var $element = $(element);

            options.tabs = bindings.tabbedParent;
            options.myTab = bindings.myTab;

            if (typeof value === "string") {
                $table = $element.find(value);
                options.parent = $element;
            } else {
                $table = $(element);
                options.parent = bindings.parent;
            }

            if ($table == undefined) throw new Error("The table element can not be found. Selector: '" + value + "'.");

            bindTable();

            function bindTable() {
                //Enables the jQuery transformation to be deferred until after the dependent object has data
                var dependsOn = bindings['dependsOn'];
                if (dependsOn && deferToDependency()) {
                    return;
                }
                stickyHeaders($table, options);

                function deferToDependency() {
                    var $dependency = typeof dependsOn === "string"
                        ? $element.children(':first')
                        : $(dependsOn);
                    if (!$dependency) return false;

                    var dependencyElement = $dependency[0];
                    var dependencyContext = ko.contextFor(dependencyElement);
                    var dependencyBindings = ko.bindingProvider.instance.getBindings(dependencyElement, dependencyContext);

                    if (dependencyHasTemplate()) {
                        var fnName = '__stickyTableHeaders__updateHeaders__';
                        if (isVirtualElement()) {
                            dependsOn.data = attachAfterRenderBinding.call(dependsOn.data);
                            dependencyContext.$data[fnName] = function () {
                                var table = typeof (value) === "string"
                                    ? $(arguments[0]).filter(value) || $element.find(value)
                                    : value;

                                if (!table.length) {
                                    console.error("The table element could not be found. When attaching stickyTableHeaders within template, the value parameter should contain a selector for the table.");
                                    return;
                                }

                                var context = ko.contextFor(dependsOn);
                                var theadDependency = bindings.stickyTableHeaderDependency;
                                if (typeof theadDependency === "string") theadDependency = context.$data[theadDependency];
                                options.parent = table;
                                if (ko.isObservable(theadDependency)) {
                                    theadDependency.subscribe(function () {
                                        stickyHeaders(table, options);
                                    });
                                } else {
                                    stickyHeaders(table, options);
                                }
                            };
                        } else {
                            var binding = attachAfterRenderBinding.call($dependency.attr('data-bind'));
                            $dependency.attr('data-bind', binding);
                            dependencyContext.$data[fnName] = function () {
                                stickyHeaders($element.find(value), options);
                            };
                        }
                        return true;

                    }

                    return false;

                    function isVirtualElement() {
                        return dependencyElement.nodeType === 8;
                            //virtualNoteBindingValue is apparently only available to the debug version of KO.
                            //&& ko.virtualElements.virtualNodeBindingValue
                            //&& ko.virtualElements.virtualNodeBindingValue(dependsOn);
                    }
                    function dependencyHasTemplate() {
                        return dependencyBindings && dependencyBindings.template;
                    }
                    function attachAfterRenderBinding() {
                        return this.replace(/(template\:\s?\{)/, "$1" + 'afterRender:' + fnName + ',');
                    }
                }
            }

            function stickyHeaders(table, opts) {
                table.each(function () {
                    if (!this.tagName || this.tagName.toLowerCase() !== 'table') {
                        throw new Error("The bound element is not a table element. Element selector: '" + value + "'");
                    }
                });

                opts.floatingElementId = 'stickyTableHeader';
                opts.target = 'thead:first';

                table.stickyTableHeaders(opts);

                table.each(function () { rebind.call(this, opts); });

                var valueSubscription;
                if (ko.isObservable(valueAccessor())) {
                    valueSubscription = valueAccessor().subscribe(function () {
                        table.stickyTableHeaders('option', 'format');
                    });
                }

                var rebuildSubscription;
                if (bindings.rebuildTrigger) {
                    if (!ko.isObservable(bindings.rebuildTrigger))
                        throw new Error("Invalid binding: \"rebuildTrigger\". Must be observable object.");

                    rebuildSubscription = bindings.rebuildTrigger.subscribe(function () {
                        table.stickyTableHeaders("option", "refresh");
                        table.each(function () { rebind.call(this, opts); });
                    });
                }

                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                    table.stickyTableHeaders('destroy');
                    valueSubscription && valueSubscription.dispose();
                    rebuildSubscription && rebuildSubscription.dispose();
                });

                completed = true;
            }

            function rebind(opts) {
                var floatingElements = $('.' + opts.floatingElementId, this);
                var floatingClone = floatingElements[0];

                var context = ko.contextFor(this);
                if (!context || !floatingClone) return;

                ko.cleanNode(floatingClone);
                ko.applyBindings(context.$data, floatingClone);

                var $clone = $(floatingClone);
                var bindings = ko.bindingProvider.instance.getBindings(this, ko.contextFor(this));

                // reformat elements if the clone was templated
                if (bindings.template) {
                    $element.stickyTableHeaders('option', 'format');
                }

                if (bindings.sortableTable) {
                    ko.bindingHandlers.sortableTable.init($clone.parent()[0], ko.utils.wrapAccessor(bindings.sortableTable), ko.utils.wrapAccessor(bindings));
                }

            }
        },
    };

    var templateRegEx = /(?:^|,|\s)template\s*:\s*(?:(?:(?:'|\")([^(?:'|"|\s|\{)]+)\s*(?:'|"))|(?:\{.*name\s*:\s*(?:(?:'|\")([^(?:'|"|\s|\{)]+)(?:'|"|\s))))/i;
    ko.bindingHandlers.stickyTableFooters = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var opts = ko.utils.unwrapObservable(allBindingsAccessor());
            opts.floatingElementId = 'stickyTableFooter';
            opts.target = 'tfoot:first';

            var $element = $(element);

            $element.stickyTableFooters(opts);

            var table = element;

            $(opts.target, table).each(function () {
                var floatingElements = $('.' + opts.floatingElementId, table);
                if (!floatingElements.length) return;
                var floatingClone = floatingElements[0];


                var context = ko.contextFor(this);
                if (!context) return;

                ko.cleanNode(floatingClone);
                ko.applyBindings(context.$data, floatingClone);

                // reformat elements if the clone was templated
                var $clone = $(floatingClone);
                var dataBind = $clone.attr('data-bind');
                if (dataBind) {
                    var matches = dataBind.match(templateRegEx);
                    if (matches && matches.length) {
                        $element.stickyTableFooters('option', 'format');
                    }
                }
            });

            var value = valueAccessor();
            if (ko.isObservable(value)) {
                value.subscribe(function() {
                    $element.stickyTableFooters('option', 'format');
                });
            }

            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $element.stickyTableFooters('destroy');
            });
        },
    };

    ko.bindingHandlers.tooltip = {
        init: function (element, bindingAccessor, allBindings) {
            var value = ko.utils.unwrapObservable(bindingAccessor()),
                bindings = allBindings && allBindings() || {};

            if (typeof value == "number") value = value.toString();
            if (!value || value.length == 0) return;

            var $element = $(element);
            $element.attr('title', value);
            $element.tooltip({
                track: bindings.tooltipTrack,
            });
            //todo: enable updates to the tooltip value
        },
    };

    ko.bindingHandlers.datePickerSm = {
        init: function (element, valueAccessor, allBindings) {
            $(element).wrap('<div class="input-group input-group-sm"></div>');
            $(element).datepicker({
                showOn: 'button',
                buttonText: '<i class="fa fa-calendar"></i>',
                changeMonth: true,
                changeYear: true
            }).next(".ui-datepicker-trigger")
                .addClass("btn btn-default")
                .wrap('<span class="input-group-btn"></span>');

            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                //todo: cleanup wrapper element
                $(element).datepicker('destroy');
            });

            var value = valueAccessor();
            if (ko.isObservable(value)) {
                ko.bindingHandlers.value.init(element, valueAccessor, allBindings);
            }
        }
    };

    ko.bindingHandlers.autoHeightTextarea = {
        init: function (element, valueAccessor) {
        },
        update: function (element, valueAccessor) {
            element.style.height = '0';
            element.style.height = element.scrollHeight + 'px';
        }
    };

    // autocomplete: listOfCompletions
    ko.bindingHandlers.autocomplete = {
        init: function (element, valueAccessor) {
            var disposables = [];
            var value = ko.utils.unwrapObservable(valueAccessor());
            var opts = {
                //minLength: 0,
                change: onChange
            };
            $( element ).wrap('<div class="ui-front"></div>');

            function buildSourceOptions( value ) {
              if ( value.length ) {
                opts.source = ko.utils.arrayMap( value, function(c) {
                  if ( c.Name && !c.label ) {
                    c.label = c.Name;
                  }

                  return c;
                });
              } else {
                opts = $.extend( opts, value );

                if ( !value.source ) {
                  console.log("Invalid parameters for the autocomplete binding. Value must be either an array or and object with a \"source\" property containing an array.");
                  return;

                  //the following line was causing an error when closing a pack schedule after it's been in edit mode.
                  throw new Error("Invalid parameters for the autocomplete binding. Value must be either an array or and object with a \"source\" property containing an array.");
                }

                if ( value.label || value.value ) {
                  var labelProjector = buildProjector( value.label ),
                  valueProjector = value.value ? buildProjector( value.value ) : function() { return value; };

                  opts.source = ko.utils.arrayMap( ko.utils.unwrapObservable( value.source ), function ( item ) {
                    return {
                      label: labelProjector(item),
                      value: valueProjector(item),
                    };
                  });
                } else {
                  opts.source = ko.utils.unwrapObservable(value.source);
                }
              }
            }

            function buildProjector( src ) {
              var prop = ko.utils.unwrapObservable(src);

              if (prop == undefined) {
                throw new Error("Projector property is undefined.");
              }

              return typeof prop === "function" ?
                function (object) { return prop(object); } :
                function(object) { return object[prop]; };
            }

            if ( ko.isObservable( value ) ) {
              disposables.push( value.subscribe(function( optionsSource ) {
                buildSourceOptions( optionsSource );
                $( element ).autocomplete( opts );
              }));
            } else if ( ko.isObservable( value.source ) ) {
              disposables.push( value.source.subscribe(function( optionsSource ) {
                buildSourceOptions( optionsSource );
                $( element ).autocomplete( opts );
              }));
            }

            buildSourceOptions( value );
            $( element ).autocomplete( opts );

            function onChange (e, ui) {
                var bindingContext = ko.contextFor(element);
                if (!bindingContext) return;
                var bindings = ko.bindingProvider.instance.getBindings(element, bindingContext) || {};
                if (!bindings.value) return;

                if (ui.item && ui.item.value) {
                    bindings.value(ui.item.value);
                }
                    // enable new elements to be added to the list
                else if (opts.allowNewValues) bindings.value($(this).val());
                else {
                    bindings.value(null);
                    if (ko.DEBUG) {
                        console.log('The selected value was not found in the options list. To allow new values, include the \"allowNewValues=\'true\'\" value in the \"autocompleteOptions\" binding attribute.');
                    }
                }
            }

            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
              $( element ).autocomplete( 'destroy' );
              ko.utils.arrayForEach( disposables, function( disposable ) {
                disposable.dispose();
              });
            });
        },
    };

    ko.bindingHandlers.tabs = {
        init: function (element, valueAccessor, allBindings) {
            var $element = $(element);
            var value = ko.utils.unwrapObservable(valueAccessor());
            $(element).val(value);

            var options = ko.utils.unwrapObservable(allBindings().tabOptions) || {};

            $element.on("tabsactivate", onTabActivate);
            $element.on("tabscreate", onTabCreate);

            $element.tabs(options);

            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $element.tabs("destroy");
                $element.off('tabsactivate', onTabActivate);
                $element.off('tabscreate', onTabCreate);
            });


            function onTabActivate(event, ui) {
                bindValueFromUI(ui.newTab.text(), ui.newPanel[0]);
            }

            function onTabCreate(event, ui) {
                bindValueFromUI(ui.tab.text(), ui.panel[0]);
            }

            function bindValueFromUI(name, panel) {
                if (!ko.isObservable(valueAccessor())) return;
                valueAccessor()({
                    name: name,
                    data: getDataBoundObjectFor(panel)
                });
            }

            function getDataBoundObjectFor(tabPanel) {
                if (!tabPanel) return undefined;
                var panelContext = ko.contextFor(tabPanel);
                if (!panelContext) return undefined;
                var bindings = ko.bindingProvider.instance.getBindings(tabPanel, panelContext) || {};
                return bindings.with || panelContext.$data;
            }
        },
    };

    ko.bindingHandlers.ajaxStatus = {
        init: function (element, valueAccessor) {
            var value = valueAccessor();
            if (value.ajaxSuccess == undefined
                || value.ajaxFailure == undefined
                || value.ajaxWorking == undefined) throw new Error("The bound value is not valid for use with the ajaxStatus binding.");

            ko.applyBindingsToNode(element, {
                css: {
                    working: value.ajaxWorking,
                    success: value.ajaxSuccess,
                    fail: value.ajaxFailure,
                    ajaxStatus: true,
                }
            });
        }
    };


    // Dragons be here...
    // allows up/down arrows, mouse-click dragging,
    // and mouse-click wheel
    // accepts property 'negative'in allBindings to allow negative numbers
    ko.bindingHandlers.numValue = {
        init: function (element, valueAccessor, allBindings) {
            console.warn("numValue binding handler is being used! This should be replaced with the numericObservable.");
            var num = valueAccessor();
            var bindings = ko.utils.unwrapObservable(allBindings());
            var isChar = function (key) { return key >= 65 && key <= 90; };
            var up = 38, down = 40;
            $(element).keydown(function (evt) {
                var key = evt.keyCode;
                var iVal = parseInt(element.value);
                if (key === up || key === down) {
                    if (key === up) iVal++;
                    else if (key == down && (bindings.negative || iVal > 0)) iVal--;
                }
                else if (isChar(key) && !evt.ctrlKey) evt.preventDefault();
                if (!isNaN(iVal) && iVal != null) num(iVal);
                else num(null);

                return true;
            });
            var isDown = false;
            var lastY = 0;
            var buffer = 10;
            $(element).mousedown(function (e) { isDown = true; return true; });
            $(document).mouseup(function (e) { isDown = false; return true; });
            $(document).mousemove(function (e) {
                if (isDown) {
                    var y = e.pageY;
                    if (!lastY) lastY = y;
                    if (y > lastY + buffer && (bindings.negative || num() > 0)) {
                        num(num() - 1);
                        lastY = y;
                    }
                    else if (y + buffer < lastY) {
                        num(num() + 1);
                        lastY = y;
                    }
                }
            });
            $(document).on("mousewheel", function (evt) {
                if (isDown) {
                    var delta = evt.originalEvent.wheelDelta;
                    if (delta > 0) {
                        num(num() + 1);
                    }
                    else if (delta < 0 && (bindings.negative || num() > 0)) {
                        num(num() - 1);
                    }
                }
            });

            // show validations as well
            return ko.bindingHandlers['validationCore'].init(element, valueAccessor, allBindings);
        },
        update: function (element, valueAccessor, allBindings) {
            var val = ko.utils.unwrapObservable(valueAccessor());
            if (!isNaN(val)) element.value = val;
        }
    };

    ko.bindingHandlers.resizable = {
        init: function (element, valueAccessor) {
            var alsoResizeSelector = ko.unwrap(valueAccessor());
            if (typeof alsoResizeSelector != "string") alsoResizeSelector = '';
            $(element).resizable({
                alsoResize: alsoResizeSelector,
                minWidth: 300,
                minHeight: 100
            });
        }
    };

    ko.bindingHandlers.accordion = {
        init: function (element, valueAccessor) {
        },
        update: function (element, valueAccessor) {
            var opts = ko.utils.unwrapObservable(valueAccessor());
            $(element).accordion(opts);
        }
    };

    ko.bindingHandlers.slimscroll = {
        init: function (element) {
            var $el = $(element);
            $el.slimscroll({
                //alwaysVisible: true,
                railColor: '#222',
                height: "100%"
                //railVisible: true
            });
        }
    };

    // Focuses next .form-control when Enter is pressed
    ko.bindingHandlers.tabOnEnter = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var allBindings = allBindingsAccessor();
            $(element).keypress(function (event) {
                var keyCode = (event.which ? event.which : event.keyCode);
                if (keyCode === 13) {
                    var index = $('.form-control').index(event.target) + 1;
                    var $next = $('.form-control').eq(index);

                    $next.focus();
                    $next.select();
                    return false;
                }
                return true;
            });
        }
    };

    /** Trigger valueAccessor on Enter keypress
      * @param {function} valueAccessor - Function to call
      */
    ko.bindingHandlers.onEnter = {
      init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var allBindings = allBindingsAccessor();
        var value = valueAccessor();
        $(element).keypress(function (event) {
          var keyCode = (event.which ? event.which : event.keyCode);
          if (keyCode === 13) {
            value.call(viewModel);
            return false;
          }
          return true;
        });
      }
    };
});
