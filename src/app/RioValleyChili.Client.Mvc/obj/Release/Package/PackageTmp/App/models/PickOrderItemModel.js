var productsService = require('services/productsService');
require('App/helpers/koHelpers.js');

ko.validation.init({
    decorateInputElement: true,
    errorElementClass: 'has-error',
    insertMessages: false
});

function PickOrderItemModel(input) {
    if (!(this instanceof PickOrderItemModel)) return new PickOrderItemModel(input);

    var self = this,
        data = ko.toJS(input) || {};

    self.disposables = [];

    // Data
    self.OrderItemKey = input.OrderItemKey;
    self.customerOptions = input.customerOptions;
    self.packageOptions = input.packageOptions;
    self.productOptions = ko.observableArray(input.productOptions);

    self.wipProductOptions = ko.observableArray();
    self.fgProductOptions = ko.observableArray();

    self.Product = ko.observable();
    self.Customer = ko.observable();
    self.CustomerProductCode = ko.observable(data.CustomerProductCode);
    self.CustomerLotCode = ko.observable(data.CustomerLotCode);
    self.Packaging = ko.observable();
    self.TreatmentKey = ko.observable(data.TreatmentKey).extend({ treatmentType: true });
    self.Quantity = ko.observable(data.Quantity);
    self.TotalWeight = ko.pureComputed(function () {
        return (self.Quantity() && self.Packaging()) ?
            self.Quantity() * self.Packaging().Weight :
            null;
    });
    self.currentTypeFilter = ko.observable('All <span class="caret"></span>');

    self.CustomerKey = ko.pureComputed(function () {
        var customer = self.Customer();

        if (customer && customer.CompanyKey) {
            return customer.CompanyKey;
        } else {
            return null;
        }
    });
    self.ProductKey = ko.pureComputed(function () {
        var product = self.Product();

        if (product && product.ProductKey) {
            return product.ProductKey;
        } else {
            return null;
        }
    });
    self.PackagingKey = ko.pureComputed(function () {
        var packaging = self.Packaging();

        if (packaging && packaging.ProductKey) {
            return packaging.ProductKey;
        } else {
            return null;
        }
    });

    // Behaviors
    (function filterProductsList() {
        for (var i = 0, len = self.productOptions().length, list = self.productOptions(); i < len; i++) {
            if (list[i].ChileState === 2) {
                self.wipProductOptions.push(list[i]);
            } else if (list[i].ChileState === 3) {
                self.fgProductOptions.push(list[i]);
            }
        }
    }());

    (function selectObject() {
        var i,
            list,
            target;

        // Customer
        if (data.Customer) {
            for (i = self.customerOptions.length, list = self.customerOptions, target = data.Customer.CompanyKey; i--;) {
                if (list[i].CompanyKey === target) {
                    self.Customer(list[i]);
                    break;
                }
            }
        } else {
            self.Customer(null);
        }

        // Packaging
        if (data.PackagingProductKey !== undefined) {
            for (i = self.packageOptions.length, list = self.packageOptions, target = data.PackagingProductKey; i--;) {
                if (list[i].ProductKey === target) {
                    self.Packaging(list[i]);
                    break;
                }
            }
        } else {
            self.Packaging(null);
        }

        // Product
        if (data.ProductKey !== undefined) {
            for (i = self.productOptions().length, list = self.productOptions(), target = data.ProductKey; i--;) {
                if (list[i].ProductKey === target) {
                    self.Product(list[i]);
                    break;
                }
            }
        } else {
            self.Product(null);
        }
    }());

    self.setFG = function () {
        self.productOptions(self.fgProductOptions());
        self.currentTypeFilter('FG ' + '<span class="caret"></span>');
    };
    self.setWIP = function () {
        self.productOptions(self.wipProductOptions());
        self.currentTypeFilter('WIP ' + '<span class="caret"></span>');
    };

    // Validation
    self.validation = ko.validatedObservable({
        Product: self.Product.extend({ required: true }),
        Packaging: self.Packaging.extend({ required: true }),
        TreatmentKey: self.TreatmentKey.extend({ required: true }),
        Quantity: self.Quantity.extend({ required: true, min: 1 })
    });

    // Disposable items
    self.disposables.push(self.validation);
}

// Custom disposal logic
PickOrderItemModel.prototype.dispose = function () {
    ko.utils.arrayForEach(this.disposables, this.disposeOne);
    ko.utils.objectForEach(this, this.disposeOne);
    self.productOptions([]);

    self.wipProductOptions([]);
    self.fgProductOptions([]);
};

PickOrderItemModel.prototype.disposeOne = function (propOrValue, value) {
    var disposable = value || propOrValue;

    if (disposable && typeof disposable.dispose === "function") {
        disposable.dispose();
    }
};

// Webpack
module.exports = PickOrderItemModel;
