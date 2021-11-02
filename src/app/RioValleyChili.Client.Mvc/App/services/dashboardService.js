define([], function() {
    return {
        getDashboardTiles: getDashboardTilesForUser
    }

    function getDashboardTilesForUser() {
        return [
            {
                title: 'Lab Results',
                areaName: 'Quality Control',
                size: null,
                template: 'startboard-tile-standard',
                contentModule: 'viewModels/qualityControl/labResultsStartTile',
                targetModuleName: 'viewModels/qualityControl/labResults',
            },
            {
                title: 'Pack Schedules & Batching',
                areaName: 'Production',
                size: null,
                template: 'startboard-tile-standard',
                targetModuleName: 'viewModels/production/batching',
            },
        ];
    }
});