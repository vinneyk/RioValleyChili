define(['jquery', 'ko'], function($, ko) {
    ko.bindingHandlers.clickableTableHeaders = {
        init: function (element, valueAccessor, allBindings) {
            setTimeout(function () { // allow templated bindings to be rendered
                setupHandlers(element, valueAccessor, allBindings);
            }, 0);
        }
    }

    function setupHandlers(element, valueAccessor, allBindings) {
        setupTableClickElements(element, valueAccessor, allBindings);
        setupRebindTrigger(element, valueAccessor, allBindings);
    }

    var defaultEnableClick = function () { return true; };

    function setupTableClickElements(element, valueAccessor, allBindings) {
        var options = allBindings() || {};
        var enableClick = options.enableClick || defaultEnableClick;
        var $table = $(element);

        $table.find('thead th').each(function (index, thElem) {
            thElem.clickEnabled = enableClick(thElem);
            
            if (thElem.clickEnabled) {
                var $th = $(thElem);
                $th.css({
                    cursor: 'pointer'
                });
            }

            thElem = null;
            $th = null;
        });

        var $thead = $table.find('thead');
        $thead.off('click');
        $thead.click(function (args) {
            if (args.target.nodeType !== 1 || args.target.nodeName !== 'TH') return;
            if (args.target.clickEnabled) valueAccessor()(args.target);
            args.stopPropagation();
        });

        ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
            $thead.off('click');
            $thead = null;
            $table = null;
        });
    }
    function setupRebindTrigger(element, valueAccessor, allBindings) {
        var options = allBindings() || {};
        if (options.rebindTrigger) {
            if (!ko.isObservable(options.rebindTrigger)) throw new Error("The \"rebindTrigger\" binding option is invalid. Expected observable.");
            options.rebindTrigger.subscribe(function () {
                setupTableClickElements(element, valueAccessor, allBindings);
                //todo: clean up old bindings?
            });
        }
    }
});