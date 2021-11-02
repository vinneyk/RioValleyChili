var path = require('path');
var webpack = require('webpack');

console.log(appDir('components'));

module.exports = function(config) {
    config.set({
       
        context: appDir('.'),

        // base path that will be used to resolve all patterns (eg. files, exclude)
        basePath: '',


        // frameworks to use
        // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
        frameworks: ['jasmine'],


        // list of files / patterns to load in the browser
        files: [
            //'app/build/*.js',
            'Test/*Spec.js',
        ],


        // list of files to exclude
        exclude: [
        ],


        // preprocess matching files before serving them to the browser
        // available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
        preprocessors: {
            //'app/build/*': ['webpack', 'sourcemap'],
            'app/components/*/*/*.js*/': ['webpack'],
            'app/*/*.js': ['webpack'],
            'app/*.js': ['webpack'],
            'Test/*Spec.js': ['webpack'],
        },


        webpack: {
            resolve: {  
                extensions: ["", ".js"],
                fallback: appDir('node_modules'),
                alias: {
                    ko: 'knockout',
                    koProjections: scriptDir('knockout-projections.min'),
                    jquery: scriptDir('jquery-2.1.1'),
                    
                    Scripts: scriptDir('.'),
                    App: appDir('.'),
                    styles: path.join(__dirname, 'Content'),
                    tests: testDir(),

                    app: appDir('rvc.js'),
                    rvc: appDir('rvc.js'),
                    helpers: appDir('helpers'),
                    components: appDir('components'),
                    services: appDir('services'),
                    viewModels: appDir('viewModels'),
                },
                //devtool: 'inline-source-map',
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

                        { test: /knockout-latest\.debug\.js$/, loader: 'imports?require=>false&define=>false!exports?ko' },
                        { test: /knockout\..*\.js$/, loader: 'imports?require=>false' },
                        { test: /ko\.extenders\.date\.js$/, loader: 'imports?require=>false' },
                        { test: /system\.js$/, loader: 'imports?require=>__webpack_require__' },
                        { test: /globals\.js$/, loader: 'expose?jQuery' }
                    ],
                    noParse: [
                        /knockout\..*\.js$/,
                        /knockout-latest\.debug\.js$/,
                        /text\.js/,
                    ]
                }
            },
        },


        webpackMiddleware: {
            // webpack-dev-middleware configuration
            noInfo: true
        },


    //plugins: [
    //    //require("karma-webpack")
    //],


    // test results reporter to use
    // possible values: 'dots', 'progress'
    // available reporters: https://npmjs.org/browse/keyword/karma-reporter
    reporters: ['progress'],


    // web server port
    port: 9876,


    // enable / disable colors in the output (reporters and logs)
    colors: true,


    // level of logging
    // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
    logLevel: config.LOG_INFO,


    // enable / disable watching file and executing tests whenever any file changes
    autoWatch: true,


    // start these browsers
    // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
    browsers: ['Chrome'],


    // Continuous Integration mode
    // if true, Karma captures browsers, runs the tests and exits
    singleRun: false
    });
};

function scriptDir(loc) { return path.join(__dirname, 'Scripts', loc); };
function appDir(loc) { return path.join(__dirname, 'App', loc); };
function testDir(loc) { return path.join(__dirname, 'Test', loc || ''); };