define(function() {
    var startModule = 'startboard';

    var routes = [
        {
            route: '',
            title: 'startboard',
            moduleId: 'viewModels/startboard',
            nav: true,
        }
    ];

    var portalFx = {
        Parts: {
            Partsize: {
                Mini: 0,
            }
        }
    }

    return {
        routes: routes,
        startModule: startModule,
    }
})