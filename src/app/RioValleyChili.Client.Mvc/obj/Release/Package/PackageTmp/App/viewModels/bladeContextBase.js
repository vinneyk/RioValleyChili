define(['durandal/system', 'plugins/router', 'durandal/activator'], function (system, router, activator) {

    //var content = activator.create();

    return {
        init: init
    }

    function init(options) {
        var selectablePart = options.selectablePart;

        var vm = {
            selectPart: function () {
                if (!selectablePart) return;
                selectablePart.isSelected(true);
                if (selectablePart.moduleId) loadModule(selectablePart, vm, options.parentContext);
            },
            deselectPart: function() {
                selectablePart && selectablePart.isSelected(false);
            },
            isPartClickable: ko.computed(function() {
                return selectablePart && !selectablePart.isSelected();
            }),
            path: ko.observableArray([]),
        };

        return vm;
    }

    function loadModule(selectablePart, context, parent) {
        var moduleId = typeof selectablePart === "string"
            ? selectablePart : selectablePart.moduleId;

        // todo: accept slug string for navigate function
        // todo: handle parameterized module loading (ex: lot details by lotKey)
        router.navigate(moduleId, { replace: true, trigger: false });
        
        system
            .acquire(moduleId)
            .then(function (moduleVm) {
                // todo: handle case when moduleId is a view id (string)
                moduleVm.moduleId = moduleId;
                wrapModule(moduleVm, context);
                contextLoadModule(context, moduleVm, parent);
            });
    }

    function wrapModule(module, context) {
        module.loadModule = function(options) {
            loadModule(options, context, module);
        }
    }
    function contextLoadModule(context, module, parent) {
        if (parent) trimPathBranch(parent, context);
        context.path.push(module);
    }
    function trimPathBranch(parent, context) {
        var path = context.path();
        var parentIndex = ko.utils.arrayIndexOf(path, parent);
        if (parentIndex > -1 && parentIndex < path.length - 1) {
            var removedModules = context.path.splice(parentIndex + 1);
            ko.utils.arrayForEach(removedModules, function() {
                //todo: dispose objects
            });
        }

    }
});