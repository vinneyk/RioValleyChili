require('App/koExtensions');

var rvc = require('rvc'),
    lotService = require('App/services/lotService'),
    productsService = require('App/services/productsService');

function FiltersViewModel (params) {
    var self = this,
    productOptionsByLotType = {},
    packagingProducts = ko.observableArray(),
    ingredientsByProductType = ko.observableArray();

    // Note: Loading screen pops under a popup display regardless of z-index
    this.disposables = [];

    // Status
    this.isLoading = ko.observable(false);
    this.loadingMessage = ko.observable('');
    this.isInit = ko.observable(false);

    // Data Structure
    this.inventoryType = ko.observable().extend({ inventoryType: true });
    this.lotType = ko.observable().extend({ lotType2: true });
    this.ingredientType = ko.observable();
    this.productKey = ko.observable();
    this.packagingProductKey = ko.observable();
    this.treatmentKey = ko.observable().extend({ treatmentType: true });
    this.lotKey = ko.observable().extend({ lotKey: self.lotKey });
    this.lotTypeOptions = ko.pureComputed(function () {
        var inventoryType = self.inventoryType();

        return inventoryType && rvc.lists.lotTypesByInventoryType[inventoryType.key] || [];
    });

    // Data pager handling
    this.fetchNextPageFunction = function () {};
    this.fetchNextPage = function () {
        var dfd = self.fetchNextPageFunction();
        if (dfd) {
            self.isLoading(true);
            self.loadingMessage('Loading items...');
            dfd.always(function () {
                self.isLoading(false);
            });
        }
    };

    this.initSubscription = this.isInit.subscribe(function (data) {
        if (data === true) {
            self.fetchNextPage();
            self.initSubscription.dispose();
        }
    });

    // Filter options
    this.ingredientTypeOptions = ko.pureComputed(function() {
        var productType = self.inventoryType(),
        ingredientTypeDictionary = ingredientsByProductType();

        return (!productType || !ingredientTypeDictionary) ?
            [] :
            ingredientTypeDictionary[productType.key || productType] || [];
    });

    this.productKeyOptions = ko.pureComputed(function() {
        var lotType = self.lotType();
        return lotType && productOptionsByLotType[lotType.value]() || [];
    });

    this.packagingProductKeyOptions = ko.pureComputed(function() {
        return packagingProducts() || [];
    });

    this.hasIngredients = ko.pureComputed(function () {
        return self.inventoryType() &&
            ingredientsByProductType()[self.inventoryType().key] &&
            ingredientsByProductType()[self.inventoryType().key].length > 0;
    });

    // Behaviors
    function loadIngredientOptions() {
        var dfd = lotService.getIngredientsByProductType();

        dfd.done(function (data) {
            ingredientsByProductType(data);
        })
        .error(function(xhr, result, message) {
            showUserMessage('Failed to get ingredient options.', { description: message, type: 'error' });
        });

        return dfd;
    }

    function loadProductOptions() {
        var productDfds = [],
        loadDfd = $.Deferred();

        rvc.helpers.forEachLotType(loadAndPush);
        $.when.apply($, productDfds).done(function () {
            loadDfd.resolve();
        });

        function loadAndPush(type) {
            if (!ko.isObservable(productOptionsByLotType[type.value])) {
                productOptionsByLotType[type.value] = ko.observableArray([]);
            }

            var dfd = lotService.getProductsByLotType(type.key)
                .done(function (data) {
                    productOptionsByLotType[type.value](data);
                });

            productDfds.push(dfd);
            return dfd;
        }

        return loadDfd;
    }

    function loadPackagingProductOptions() {
        var dfd = productsService.getPackagingProducts();

        dfd.done(function (data) {
            packagingProducts(data);
        });

        return dfd;
    }

    $.when(
        lotService.getIngredientsByProductType(),
        loadProductOptions(),
        productsService.getPackagingProducts())
            .done(function (ingredients, products, packaging) {
                ingredientsByProductType(ingredients[0]);
                packagingProducts(packaging[0]);
                init(params.input);
            });

    function init (input) {
        self.inventoryType(undefined);
        self.lotType(undefined);
        self.ingredientType(undefined);
        self.productKey(undefined);
        self.packagingProductKey(undefined);
        self.treatmentKey(undefined);
        self.lotKey(undefined);

        if (input === undefined) {
            return;
        }

        var data = ko.toJS(input) || {},
        i = 0,
        output = {},
        lotReady = $.Deferred(),
        productKeySub = $.Deferred(),
        ingredientTypeSub;


        // Non-dependent values
        output.inventoryType = ko.utils.arrayFirst(self.inventoryType.options, function (item) {
            if (comparator(item.value.key, data.inventoryType)) {
                if (self.lotTypeOptions().length > 0) {
                    lotReady.resolve();
                } else {
                    var lotTypeSub = self.lotTypeOptions.subscribe(function (data) {
                        if (self.lotTypeOptions().length > 0) {
                            lotReady.resolve();
                            lotTypeSub.dispose();
                        }
                    });
                }
                return true;
            }
        });
        output.packagingProductKey = ko.utils.arrayFirst(self.packagingProductKeyOptions(), function (item) {
            if (comparator(item.ProductKey, data.packagingProductKey)) {
                self.packagingProductKey(item);
                return true;
            }
        });
        output.treatmentKey = ko.utils.arrayFirst(self.treatmentKey.options, function (item) {
            if (comparator(item.key, data.treatmentKey)) {
                self.treatmentKey(item);
                return true;
            }
        });

        function comparator (target, key) {
            var targetKey = keyParse(target),
                currentKey = keyParse(key);

            if (currentKey === targetKey) {
                return true;
            }

            return false;

            function keyParse (input) {
                return input === Object(input) ?
                    input.key :
                    input;
            }
        }

        // Dependent variables
        // lotTypeOptions
        self.inventoryType(output.inventoryType);
        self.lotKey(data.lotKey);
        $.when(lotReady).done(function () {
            ko.utils.arrayFirst(self.lotTypeOptions(), function (item) {
                if (comparator(item.key, data.lotType)) {
                    var productSub = self.productKeyOptions.subscribe(function (data) {
                        productKeySub.resolve(true);
                        productSub.dispose();
                    }),
                    ingredientTypeTest = (function () {
                        ingredientTypeSub = $.Deferred();
                        if (self.hasIngredients()) {

                            var ingredientSub = self.productKeyOptions.subscribe(function (data) {
                                ingredientTypeSub.resolve(true);
                                ingredientSub.dispose();
                            });
                        } else {
                            ingredientTypeSub.resolve(false);
                            return;
                        }
                    })();
                    self.lotType(item);

                    return true;
                }
            });

            $.when(productKeySub, ingredientTypeSub)
                .done(function (productSub, ingredientSub) {
                    ko.utils.arrayFirst(self.productKeyOptions(), function (item) {
                        if (comparator(item.ProductKey, data.productKey)) {
                            self.productKey(item);
                        }
                    });

                    if (ingredientSub) {
                        if (typeof data.ingredientType === 'string') {
                            ko.utils.arrayFirst(self.ingredientTypeOptions(), function (item) {
                                if (item.Description === data.ingredientType) {
                                    self.ingredientType(item);
                                    return true;
                                }
                            });
                        } else {
                            ko.utils.arrayFirst(self.ingredientTypeOptions(), function (item) {
                                if (comparator(item.ProductKey, data.ingredientType)) {
                                    self.ingredientType(item);
                                    return true;
                                }
                            });
                        }
                    }
                })
            .always(function () {
                self.fetchNextPage();
            });
        });

    }

    // Exported filters
    this.filters = {};

    this.filters.inventoryType = ko.pureComputed(function () {
        return self.inventoryType() ? self.inventoryType().value.key : null;
    });

    this.filters.lotType = ko.pureComputed(function () {
        return self.lotType() ? self.lotType().key : null;
    });

    this.filters.ingredientType = ko.pureComputed(function () {
        return self.ingredientType() ? self.ingredientType().Key : null;
    });

    this.filters.productKey = ko.pureComputed(function () {
        return self.productKey() ? self.productKey().ProductKey : null;
    });

    this.filters.packagingProductKey = ko.pureComputed(function () {
        return self.packagingProductKey() ? self.packagingProductKey().ProductKey : null;
    });

    this.filters.treatmentKey = ko.pureComputed(function () {
        return self.treatmentKey() ? self.treatmentKey().key : null;
    });

    this.filters.lotKey = self.lotKey;

    // Sets data pager parameters and imports next page function
    this.setDataPagerFilters = function (dataPager, filters, nextPageFunction) {
        dataPager.addParameters(self.filters);

        filters(self.filters);
        self.fetchNextPageFunction = nextPageFunction;
        self.isInit(true);
    };

    params.exports(this);
}

FiltersViewModel.prototype.dispose = function () {
    ko.utils.arrayForEach(this.disposables, this.disposeOne);
    ko.utils.objectForEach(this, this.disposeOne);
};

FiltersViewModel.prototype.disposeOne = function(propOrValue, value) {
    var disposable = value || propOrValue;

    if (disposable && typeof disposable.dispose === "function") {
        disposable.dispose();
    }
};

module.exports = {
    viewModel: FiltersViewModel,
    template: require('./inventory-filters.html')
};
