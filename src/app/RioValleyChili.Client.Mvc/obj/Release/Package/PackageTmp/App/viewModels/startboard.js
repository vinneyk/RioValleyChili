define(['ko', 'services/dashboardService', 'viewModels/dashboardTile'], function (ko, dashboardService, dashboardTileViewModel) {
    var tiles = ko.observableArray([]);

    return {
        activate: activate,
        tiles: tiles,
        journeyPath: ko.observableArray([]),
    }

    function activate() {
        tiles(
            ko.utils.arrayMap(
                dashboardService.getDashboardTiles(),
                dashboardTileViewModel.init));
    }
})