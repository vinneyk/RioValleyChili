define(['plugins/router', 'durandal/app', 'config', 'ko', 'services/authService', 'components/core/fxs-blade-actions', 'jquery', 'jq-mousewheel'],
    function (router, app, config, ko, authService, bladeActionsComponent, $) {
        var currentUser = ko.observable(),
            selectedStartboardItem = ko.observable();

        registerComponents();

        return {
            activate: activate,
            router: router,
            currentUser: currentUser,
            selectedStartboardItem: selectedStartboardItem,
            startboardItemClicked: startboardItemClicked,
        }

        function activate() {
            currentUser(authService.getCurrentUser());

            var $panorama;

            $('body').mousewheel(function (event, delta) {
                if (!$panorama) {
                    $panorama = $(this).find('.fxs-panorama')[0];
                }

                var $content = $(event.originalEvent.srcElement).closest('.fxs-blade-content');
                if ($content.length) {
                    if ($content[0].offsetHeight < $content[0].scrollHeight) {
                        return;
                    }
                }

                $panorama.scrollLeft -= (delta * 30);
                event.preventDefault();
            });

            return router.map(config.routes)
                .buildNavigationModel()
                .mapUnknownRoutes('viewModels/startboard','')
                .activate();
        }

        function startboardItemClicked(object, args) {
            var tile = args.originalEvent.srcElement;
            if (!(tile.context && tile.container)) {
                var closest = $(args.originalEvent.srcElement).closest('.fxs-tile');
                if (closest.length) {
                    tile = closest[0];
                } else {
                    return;
                }
            }

            var tileContext = ko.contextFor(tile).$data;
            if (tileContext.context.isPartClickable() && tileContext.container.selectable) {
                var selected = selectedStartboardItem();
                if (selected) {
                    selected.context.deselectPart();
                }

                tileContext.context.selectPart();
                selectedStartboardItem(tileContext);
            }
        }
        function registerComponents() {
            bladeActionsComponent.viewModel.prototype.defaultOptions.closeBlade = function(context) {
                // deactivate dashboard item if represented by blade being closed
                
                // remove blade from path


                // unload blade

            }
            ko.components.register('fxs-blade-actions', bladeActionsComponent);
        }
    });