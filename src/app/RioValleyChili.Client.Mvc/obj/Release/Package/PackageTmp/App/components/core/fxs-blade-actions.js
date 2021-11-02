define(['text!./fxs-blade-actions.html', 'ko', 'scripts/knockout.command'], function (templateMarkup, ko) {

    function ViewModel(params) {
        if (!(this instanceof ViewModel)) return new ViewModel(params);

        var opts = $.extend({}, ViewModel.prototype.defaultOptions, params);

        return {
            closeBladeCommand: ko.command({
                execute: opts.closeBlade,
                canExecute: opts.canCloseBlade,
            }),
            resizeMaxCommand: ko.command({
                execute: opts.resizeMax,
                canExecute: opts.canResizeMax,
            })
        }
    }

    ViewModel.prototype.defaultOptions = {
        closeBlade: function () { console.log('close clicked'); },
        resizeMax: function () { console.log('resize max clicked'); },
        resizeMin: function () { console.log('resize min clicked'); },
        canCloseBlade: function () { return true; },
        canResizeMax: function() { return true; }
    }

    return {
        viewModel: ViewModel,
        template: templateMarkup
    }
})