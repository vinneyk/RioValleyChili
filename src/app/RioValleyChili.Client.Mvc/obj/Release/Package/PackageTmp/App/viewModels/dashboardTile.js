define(['durandal/activator', 'durandal/system', 'ko', 'viewModels/bladeContextBase', 'jquery'], function(activator, system, ko, bladeContextBase, $) {
    function Tile(values) {
        if (!(this instanceof Tile)) return new Tile(values);

        var content = activator.create();
        var selectablePart = getSelectablePart();
        var operations = {
            _activeOperations: [activateModule],
            inProgress: ko.observable(!!values.contentModule),
        };

        return {
            size: values.size,
            container: {
                template: values.template,
                contentModule: values.contentModule,
                partTitle: values.title,
                areaName: values.areaName,
                selectable: selectablePart,
                content: content,
                operations: operations,
            },
            context: bladeContextBase.init({ selectablePart: selectablePart }),
            func: {
                _afterAddChild: function(elements) {
                    console.log({
                        'arguments': arguments,
                        'this': this,
                    });

                    if (elements) {
                        $(elements).hide().fadeIn();
                    }
                }
            }
        }

        function activateModule() {
            if (!values.contentModule) {
                operations.inProgress(false);
                return;
            }

            system
                .acquire(values.contentModule)
                .then(function (module) {
                    content.activateItem(module, { "hi": "hello" })
                        .always(function () {
                            operations.inProgress(false);
                        });
                });
        }
        function getSelectablePart() {
            if (!values.targetModuleName) return null;

            return {
                isSelected: ko.observable(false),
                moduleId: values.targetModuleName,
            }
        }
    }

    var vm = {
        init: init
    }

    return vm;

    function init(input) {
        return new Tile(input || {});
    }

});