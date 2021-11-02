requirejs.config({
    paths: {
        'text': '../Scripts/text',
        'durandal': '../Scripts/durandal',
        'plugins': '../Scripts/durandal/plugins',
        'transitions': '../Scripts/durandal/transitions',
        'jq-mousewheel': '../Scripts/jquery.mousewheel',
        'scripts': '../Scripts',
    },
    shim: {
        'jq-mousewheel': ['jquery'],
        'scripts/ko.extenders.date': 'ko',
        'scripts/knockout.editStateManager': ['ko', 'scripts/knockout.command'],
        'App/koBindings': ['ko', 'scripts/knockout.command', 'jquery'],
        'scripts/sh.knockout.customObservables': ['scripts/ko.extenders.date', 'ko'],
        'App/koExtensions': ['ko', 'scripts/knockout.validation'],
        'rvc': ['scripts/sh.core', 'scripts/sh.knockout.customObservables']
    },
    enforceDefine: true,
});

define('jquery', function() { return jQuery; });
define('ko', function() { return ko; });
define('knockout', function () { return ko; });

define(function (require) {
    var system = require('durandal/system'),
        app = require('durandal/app'),
        viewLocator = require('durandal/viewLocator');

    system.debug(true);

    app.configurePlugins({
        router: true,
        dialog: true,
    });

    app.start().then(function () {
        viewLocator.useConvention();

        app.setRoot('viewModels/shell');
    });
});