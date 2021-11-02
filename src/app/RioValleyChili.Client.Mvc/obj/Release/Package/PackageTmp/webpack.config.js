var path = require('path');
var webpack = require('webpack');

module.exports = {
    context: appDir('.'),
    progress: true,
    colors: true,
    // Bundles to output
    entry: {
        contracts: 'viewModels/sales/customerContracts',
        companyMaintenance: 'viewModels/sales/companyContactMaintenance',
        customerProductSpecs: 'viewModels/qualityControl/customerProductSpecs',
        dehydratedMaterials: 'viewModels/inventory/dehydratedMaterials',
        facilityMaintenance: 'viewModels/warehouse/facilityMaintenance',
        interWarehouseMovements: 'viewModels/warehouse/interWarehouseMovements',
        inventoryMovements: 'viewModels/warehouse/inventoryMovements',
        inventoryReceiving: 'viewModels/warehouse/inventoryReceiving',
        labResults: 'viewModels/qualityControl/labResults',
        lotHistory: 'viewModels/inventory/lotHistory',
        lotTrace: 'viewModels/qualityControl/lotTrace',
        millWetdown: 'viewModels/production/millAndWetdown',
        packSchedules: 'viewModels/production/packSchedules',
        productionResults: 'viewModels/production/productionResultsViewModel',
        productionSchedules: 'viewModels/production/productionSchedules',
        productmaintenance: 'viewModels/qualityControl/productMaintenance',
        quotes: 'viewModels/sales/quotes',
        receiveChileProduct: 'viewModels/inventory/receiveChileProduct',
        salesOrders: 'viewModels/orders/salesOrders',
        sampleMatching: 'viewModels/qualityControl/sampleMatching.js',
        treatmentOrders: 'viewModels/warehouse/treatmentOrders',
        warehouseInventory: 'viewModels/warehouse/inventory',
        warehouseInventoryAdjustments: 'viewModels/warehouse/inventoryAdjustments',
        warehouseLocations: 'viewModels/warehouse/warehouseLocations',
        vendors: ['ko', 'rvc', 'scripts/globals', scriptDir('knockout.command'), scriptDir('knockout-postbox'), scriptDir('sh.knockout.customObservables'), scriptDir('sh.core'), scriptDir('knockout.punches'), nodeDir('knockout-sortable'), scriptDir('knockout.validation.min')],
    },
    output : {
        path: appDir('build'),
        filename: '[name].bundle.js',
        chunkFilename: '[id].chunk.js',
    },
    resolve: {
        fallback: appDir('node_modules'),
        alias: {
            ko:         'knockout',
            koProjections: scriptDir('knockout-projections.min'),
            jquery:     scriptDir('jquery-2.1.1'),
            'jquery-ui/sortable': scriptDir('jquery-ui-1.10.0.js'),
            'jquery-ui/draggable': scriptDir('jquery-ui-1.10.0.js'),
            text: scriptDir('text'),
            bootstrap:  nodeDir('bootstrap/dist/js/bootstrap'),
            durandal:   scriptDir('durandal'),
            plugins:    scriptDir('durandal/plugins'),

            scripts:    scriptDir('.'),
            Scripts:    scriptDir('.'),
            App:        appDir('.'),
            styles:     path.join(__dirname, 'Content'),

            app:        appDir('rvc.js'),
            rvc:        appDir('rvc.js'),
            tests:      appDir('tests'),
            helpers:    appDir('helpers'),
            components: appDir('components'),
            services:   appDir('services'),
            viewModels: appDir('viewModels'),
            node_modules: nodeDir(''),
        }
    },
    plugins: [
        new webpack.IgnorePlugin(/\.map$/),
        new webpack.ProvidePlugin({
            '$': 'jquery',
            jQuery: 'jquery'
        }),
        //https://github.com/webpack/docs/wiki/optimization
        new webpack.optimize.DedupePlugin(),
        new webpack.optimize.CommonsChunkPlugin('vendors', 'vendors.bundle.js'),
    ],
    module: {
        loaders: [
            { test: /\.html$/, loader: 'raw' },
            { test: /\.css$/, loader: 'style!css' },

            { test: /knockout-latest\.debug\.js$/, loader: 'imports?require=>false&define=>false!exports?ko'},
            { test: /knockout\..*\.js$/, loader: 'imports?require=>false&module=>false'},
            { test: /ko\.extenders\.date\.js$/, loader: 'imports?require=>false'},
            { test: /system\.js$/, loader: 'imports?require=>__webpack_require__'},
            { test: /globals\.js$/, loader: 'expose?jQuery' },
            { test: /knockout-jqAutocomplete\.js$/, loader: 'imports?define=>false&this=>window' },
        ],
        noParse: [
            /knockout\..*\.js$/,
            /knockout-latest\.debug\.js$/,
            /text\.js/,
        ]
    }
};

// __dirname is a node global
function scriptDir(loc) { return path.join(__dirname, 'Scripts', loc); }
function appDir(loc) { return path.join(__dirname, 'App', loc); }
function nodeDir(loc) { return path.join(__dirname, 'node_modules', loc); }
