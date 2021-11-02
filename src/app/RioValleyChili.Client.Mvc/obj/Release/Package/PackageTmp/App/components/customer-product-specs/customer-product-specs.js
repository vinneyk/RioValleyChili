define(['text!components/customer-product-specs/customer-product-specs.html', 'services/productsService', 'services/qualityControlService', 'app', 'ko', 'helpers/koHelpers'],
    function (templateMarkup, productsService, qualityControlService, rvc, ko, koHelpers) {

    //#region models

    function CustomerProductSpec(values) {
        if (!(this instanceof CustomerProductSpec)) return new CustomerProductSpec(values);

        var model = this;
        this.key = values.AttributeKey;
        this.productSpecMinValue = values.ProductSpecMinValue;
        this.productSpecMaxValue = values.ProductSpecMaxValue;
        this.customerSpecMinValue = ko.numericObservable(values.CustomerSpecMinValue, 2);
        this.customerSpecMaxValue = ko.numericObservable(values.CustomerSpecMaxValue, 2);

        this.customerSpecMinValue.extend({
            required: { onlyIf: function () { return model.customerSpecMaxValue() != undefined; } },
            max: model.customerSpecMaxValue,
        });
        this.customerSpecMaxValue.extend({
            required: { onlyIf: function() { return model.customerSpecMinValue() != undefined; } },
            min: model.customerSpecMinValue
        });

        koHelpers.esmHelper(this);

        return this;
    }

    CustomerProductSpec.prototype.toDto = function () {
        return {
            AttributeNameKey: this.key,
            RangeMin: this.customerSpecMinValue(),
            RangeMax: this.customerSpecMaxValue(),
        }
    }

    function CustomerProductSpecs(params) {
        if (!(this instanceof CustomerProductSpecs)) return new CustomerProductSpecs(params);
        params = params || {};

        var model = this;
        var attributeNames = [];

        this.customer = params.customer;
        this.product = params.product;
        this.currentCustomerSpec = ko.observableArray([]);

        //#region computed properties
        this.hasChanges = ko.computed(function() {
            var specs = model.currentCustomerSpec() || [];
            return specs.length && ko.utils.arrayFirst(specs, function(item) {
                return item.hasChanges();
            }) && true;
        });
        this.hasSpecs = ko.computed(function() {
            var specs = model.currentCustomerSpec() || [];
            return specs.length && ko.utils.arrayFirst(specs, function (item) {
                return item.customerSpecMinValue() != undefined || item.customerSpecMaxValue() != undefined;
            }) && true;
        });
        //#endregion

        //#region commands
        this.getProductAndCustomerSpecsCommand = ko.asyncCommand({
            execute: function (complete) {
                var d1 = $.Deferred(), d2 = $.Deferred();
                var productSpec, customerSpec;

                getProductSpec().done(function (data) {
                    productSpec = data.AttributeRanges.toObj(function (o) { return o.AttributeNameKey; });
                    d1.resolve();
                });

                getCustomerSpec()
                    .done(function (data) {
                        customerSpec = data.toObj(function (o) { return o.AttributeShortName; });
                        d2.resolve();
                    })
                    .fail(function(xhr, status, message) {
                        if (xhr.status == 404) {
                            customerSpec = {};
                            d2.resolve();
                        }
                });

                $.when(d1, d2)
                    .done(function () { setCurrentCustomerSpec(productSpec, customerSpec); })
                    .always(complete);
            },
            canExecute: function(isExecuting) {
                return !isExecuting && ko.utils.unwrapObservable(model.product) && ko.utils.unwrapObservable(model.customer) && true;
            }
        });
        this.saveCustomerSpecsCommand = ko.asyncCommand({
            execute: function (complete) {
                if (!model.validate()) {
                    showUserMessage('Please fix all validation errors before proceeding.');
                    return;
                }

                var specs = ko.utils.arrayFilter(model.currentCustomerSpec(), function(item) { return item.hasChanges(); }),
                    companyKey = ko.utils.unwrapObservable(model.customer).CompanyKey,
                    productKey = ko.utils.unwrapObservable(model.product).ProductKey;

                var deferreds = ko.utils.arrayMap(specs, function (s) {
                    var dto = s.toDto();
                    var dfd = toDelete(dto)
                        ? qualityControlService.deleteCustomerProductSpec(companyKey, productKey, dto.AttributeNameKey)
                        : qualityControlService.saveCustomerProductSpec(companyKey, productKey, dto);

                    dfd.done(function () { s.saveEditsCommand.execute(); });
                    return dfd;
                });

                $.when.apply(null, deferreds)
                    .done(function () { showUserMessage('Customer product specs saved successfully.'); })
                    .always(complete);

                function toDelete(o) {
                    return o.RangeMin == undefined && o.RangeMax == undefined;
                }
            },
            canExecute: function(isExecuting) {
                return !isExecuting && model.hasChanges();
            }
        });
        this.revertCustomerSpecsCommand = ko.command({
            execute: function() {
                ko.utils.arrayForEach(model.currentCustomerSpec(), function(item) {
                    item.revertEditsCommand.execute();
                });
            },
            canExecute: function() {
                return model.hasChanges() && !model.saveCustomerSpecsCommand.isExecuting();
            }
        });
        this.clearAllSpecsCustomerCommand = ko.command({
            execute: function() {
                ko.utils.arrayForEach(model.currentCustomerSpec(), function(spec) {
                    spec.customerSpecMinValue(null);
                    spec.customerSpecMaxValue(null);
                });
            },
            canExecute: function() {
                return !model.saveCustomerSpecsCommand.isExecuting() && model.hasSpecs();
            }
        });
        //#endregion

        loadAttributeNames();

        function getProductSpec() {
            return productsService.getProductDetails(rvc.lists.inventoryTypes.Chile.key, ko.utils.unwrapObservable(model.product).ProductKey);
        }
        function getCustomerSpec() {
            return qualityControlService.getCustomerProductSpec(ko.utils.unwrapObservable(model.customer).CompanyKey, ko.utils.unwrapObservable(model.product).ProductKey);
        }
        function loadAttributeNames() {
            productsService.getProductTypeAttributes()
                .done(function (data) { attributeNames = data[rvc.lists.inventoryTypes.Chile.value] || []; });
        }
       
        function setCurrentCustomerSpec(productSpec, customerSpec) {
            model.currentCustomerSpec(
                mapCustomerProductSpecs(attributeNames, productSpec, customerSpec)
            );
        }
    }

    CustomerProductSpecs.prototype.dispose = function () {
        //todo: cleanup
    }
    CustomerProductSpecs.prototype.validate = function () {
        return !ko.utils.arrayFirst(this.currentCustomerSpec(), function (spec) {
            return spec.hasChanges() && ko.validation.group(spec)().length !== 0;
        });
    }

    //#endregion

    return { viewModel: CustomerProductSpecs, template: templateMarkup };

    function mapCustomerProductSpecs(attributeNames, productSpec, customerSpecOverrides) {
        return ko.utils.arrayMap(attributeNames, function (attr) {
            var pSpecAttribute = productSpec[attr.Key] || {};
            var cSpecAttribute = customerSpecOverrides[attr.Key] || {};

            return new CustomerProductSpec({
                AttributeKey: attr.Key,
                ProductSpecMinValue: pSpecAttribute.MinValue,
                ProductSpecMaxValue: pSpecAttribute.MaxValue,
                CustomerSpecMinValue:cSpecAttribute.RangeMin,
                CustomerSpecMaxValue: cSpecAttribute.RangeMax
            });
        });
    }
});