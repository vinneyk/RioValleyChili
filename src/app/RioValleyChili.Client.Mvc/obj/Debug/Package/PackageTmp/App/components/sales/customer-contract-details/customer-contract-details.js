define(['./customer-contract-details.html', 'services/salesService', 'services/productsService', 'helpers/koHelpers', 'app', 'ko', 'rvc', 'components/common/notebook-control/notebook-control', 'styles/customer-contracts.css', 'components/common/address-book/address-book'],
    function (templateMarkup, salesService, productsService, helpers, app, ko, rvc, notebookControlComponent) {

      require('App/helpers/koValidationHelpers.js');
        var currencyFormat = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' });
        var avgCostFormat = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 3 });

        //#region model definitions

        function Address(input) {
            if (!(this instanceof Address)) return new Address(input);
            var values = input || {};

            this.AddressLine1 = ko.observable(values.AddressLine1 || "");
            this.AddressLine2 = ko.observable(values.AddressLine2 || "");
            this.AddressLine3 = ko.observable(values.AddressLine3 || "");
            this.City = ko.observable(values.City || "");
            this.State = ko.observable(values.State || "");
            this.PostalCode = ko.observable(values.PostalCode || "");
            this.Country = ko.observable(values.Country || "");
        }
        // update values
        Address.prototype.update = function (val) {
            ['AddressLine1', 'AddressLine2', 'AddressLine3', 'City', 'State', 'PostalCode', 'Country']
            .forEach(function (prop) {
                if (prop in val)
                    this[prop](val[prop]);
            }, this);
        };

        Address.prototype.buildDtoObject = function() {
            return ko.toJS(this);
        };

        function CustomerContract(input) {
            if (!(this instanceof CustomerContract)) return new CustomerContract(input);
            var values = input || {};
            var _customer = ko.observable(),
                _broker = ko.observable();

            var vm = this;

            this.CustomerContractKey = values.CustomerContractKey;
            this.ContractNumber = values.ContractNumber;
            this.ContractDate = ko.observableDate(values.ContractDate || Date.now());
            this.TermBegin = ko.observableDate(values.TermBegin);
            this.TermEnd = ko.observableDate(values.TermEnd);
            this.CustomerPurchaseOrder = ko.observable(values.CustomerPurchaseOrder);
            this.ContractType = ko.observable(values.ContractType + '').extend({ contractType: true });
            this.ContractStatus = ko.observable(values.ContractStatus + '').extend({ contractStatus: true });
            this.CustomerKey = ko.observable(values.CustomerKey);
            this.CustomerName = ko.observable(values.CustomerName);
            this.ContactName = ko.observable(values.ContactName);
            this.BrokerCompanyKey = ko.observable(values.BrokerCompanyKey);
            this.BrokerCompanyName = ko.observable(values.BrokerCompanyName);
            this.PaymentTerms = ko.observable(values.PaymentTerms);
            this.FOB = ko.observable(values.FOB);
            this.NotesToPrint = ko.observable(values.NotesToPrint);
            this.CommentsNotebookKey = values.CommentsNotebookKey;

            this.DistributionWarehouseKey = values.DistributionWarehouseKey;
            this.DistributionWarehouseName = values.DistributionWarehouseName;
            this.ContactAddress = new Address(values.ContactAddress);
            this.ContractItems = ko.observableArray(ko.utils.arrayMap(values.ContractItems, ContractLineItem));

            values.Links = values.Links || {};
            this.contractDrawSummaryReportUrl = values.Links["contract-draw-summary-report"] && values.Links["contract-draw-summary-report"].HRef;
            this.contractReportUrl = values.Links["contract-report"] && values.Links["contract-report"].HRef;

            this.customer = ko.computed({
                read: function () {
                  return _customer();
                },
                write: function (value) {
                    if (value === undefined) {
                        _customer(null);
                        return;
                    }
                    vm.CustomerKey((value && value.CompanyKey) || null);
                    vm.CustomerName((value && value.Name) || null);
                    _customer(value);
                },
                owner: this
            });

            this.broker = ko.computed({
                read: _broker,
                write: function(value) {
                    if (value === undefined) {
                        _broker(null);
                        return;
                    }

                    vm.BrokerCompanyKey((value && value.CompanyKey) || null);
                    vm.BrokerCompanyName((value && value.Name) || null);
                    _broker(value);
                }
            });
            this.totalQuantityOnContract = ko.pureComputed(function () {
                var sum = 0;
                ko.utils.arrayMap(this.ContractItems(), function(i) {
                    sum += i.Quantity() || 0;
                });
                return sum;
            }, this);
            this.totalPoundsOnContract = ko.pureComputed(function () {
                var sum = 0;
                ko.utils.arrayMap(this.ContractItems(), function (i) {
                    sum += i.totalWeight() || 0;
                });
                return sum;
            }, this);
            this.averageBasePricePerPound = ko.pureComputed(function () {
                var sum = 0, items = this.ContractItems();
                ko.utils.arrayMap(items, function (i) {
                    sum += (i.PriceBase() || 0) * (i.totalWeight() || 0);
                });
                var totalPounds = this.totalPoundsOnContract();
                return totalPounds ? sum / totalPounds : 0;
            }, this);
            this.averageBasePricePerPound.formattedValue = ko.pureComputed(function () {
                return avgCostFormat.format(this());
            }, this.averageBasePricePerPound);
            this.averageNetPricePerPound = ko.pureComputed(function () {
                var sum = 0, items = this.ContractItems();
                ko.utils.arrayMap(items, function (i) {
                    sum += (i.totalPricePerPound() || 0) * (i.totalWeight() || 0);
                });
                var totalPounds = this.totalPoundsOnContract();
                return totalPounds ? sum / totalPounds : 0;
            }, this);
            this.averageNetPricePerPound.formattedValue = ko.pureComputed(function () {
                return avgCostFormat.format(this());
            }, this.averageNetPricePerPound);
            this.totalValueOfContract = ko.pureComputed(function () {
                var sum = 0;
                ko.utils.arrayMap(this.ContractItems(), function (i) {
                    sum += i.totalValue() || 0;
                });
                return sum;
            }, this);
            this.totalValueOfContract.formattedValue = ko.pureComputed(function() {
                return currencyFormat.format(this());
            }, this.totalValueOfContract);

            this.addContractItemCommand = ko.command({
                execute: function() {
                    vm.ContractItems.push(new ContractLineItem());
                },
            });
            this.removeContractItemCommand = ko.command({
                execute: function(item) {
                    if (!(item instanceof ContractLineItem)) throw new Error("Invalid arguement. Expected ContractLineItem instance.");
                    ko.utils.arrayRemoveItem(vm.ContractItems(), item);
                    vm.ContractItems.notifySubscribers(vm.ContractItems());
                }
            });

            this.isNew = !values.CustomerContractKey;

            this.validation = ko.validatedObservable({
              date: vm.ContractDate.extend({ required: true }),
              customer: vm.customer.extend({ required: true })
            });
        }

        CustomerContract.prototype.updateContractAsync = function() {

        };
        CustomerContract.prototype.adjustBasePriceOfItems = function (adjustment, filter) {
            var adjValue = parseFloat(adjustment);
            if (isNaN(adjValue)) throw new Error("Invalid argument \"adjustment\". Expected numeric value.");

            var items = typeof filter === "function" ?
              ko.utils.arrayFilter(this.ContractItems.peek(), filter) :
              this.ContractItems.peek();

            items.forEach(function (item) { item.PriceBase((parseFloat(item.PriceBase()) || 0) + adjValue); });
        };
        CustomerContract.prototype.buildDtoObject = function() {
            return {
                ContractDate: this.ContractDate(),
                TermBegin: this.TermBegin(),
                TermEnd: this.TermEnd(),
                CustomerKey: this.CustomerKey(),
                ContractType: this.ContractType(),
                ContractStatus: this.ContractStatus(),
                ContactName: this.ContactName(),
                BrokerKey: this.BrokerCompanyKey(),
                ContactAddress: this.ContactAddress.buildDtoObject(),
                FOB: this.FOB(),
                DistributionWarehouseKey: this.DistributionWarehouseKey,
                PaymentTerms: this.PaymentTerms(),
                CustomerPurchaseOrder: this.CustomerPurchaseOrder(),
                NotesToPrint: this.NotesToPrint(),
                ContractItems: ko.utils.arrayMap(this.ContractItems(), function(item) { return item.buildDtoObject(); }),
            };
        };
        CustomerContract.prototype.toContractSummaryItem = function() {
            return ko.toJS({
                CustomerContractKey: this.CustomerContractKey,
                ContractNumber: this.ContractNumber,
                ContractDate: this.ContractDate,
                TermBegin: this.TermBegin,
                TermEnd: this.TermEnd,
                CustomerPurchaseOrder: this.CustomerPurchaseOrder,
                ContractType: this.ContractType,
                ContractStatus: this.ContractStatus,
                CustomerKey: this.CustomerKey,
                CustomerName: this.CustomerName,
                ContactName: this.ContactName,
                BrokerCompanyKey: this.BrokerCompanyKey,
                BrokerCompanyName: this.BrokerCompanyName,
                DistributionWarehouseKey: this.DistributionWarehouseKey,
                DistributionWarehouseName: this.DistributionWarehouseName,
                AverageBasePrice: this.averageBasePricePerPound,
                AverageTotalPrice: this.averageNetPricePerPound,
                SumQuantity: this.totalQuantityOnContract,
                SumWeight: this.totalPoundsOnContract,
            });
        };
        CustomerContract.prototype.createNewFromCopy = function() {
            var copy = new CustomerContract(ko.toJS(this));
            copy.copiedFrom = this.CustomerContractKey;
            copy.CustomerContractKey = null;
            copy.ContractNumber = null;
            copy.CustomerPurchaseOrder(null);
            copy.ContractStatus(app.lists.contractStatuses.Pending.key);
            //copy.NotesToPrint(null);
            copy.CommentsNotebookKey = null;
            copy.ContractDate(Date.now());

            var termBegin = copy.TermBegin();
            if (termBegin) {
                if (!(termBegin instanceof Date)) termBegin = new Date(termBegin);
                copy.TermBegin(new Date(termBegin.getFullYear() + 1, termBegin.getMonth(), termBegin.getDate()));
            }

            var termEnd = copy.TermEnd();
            if (termEnd) {
                if (!(termEnd instanceof Date)) termEnd = new Date(termEnd);
                copy.TermEnd(new Date(termEnd.getFullYear() + 1, termEnd.getMonth(), termEnd.getDate()));
            }

            copy.isNew = true;

            return copy;
        };

        function ContractLineItem(input) {
            if (!(this instanceof ContractLineItem)) return new ContractLineItem(input);
            var values = input || {};

            var self = this;
            var _chileProduct = ko.observable();
            var _packagingProduct = ko.observable();

            this.ChileProductClassificationKey = values.ChileProductClassificationKey;
            this.ContractItemKey = values.ContractItemKey;
            this.ChileProductKey = ko.observable(values.ChileProductKey);
            this.ChileProductName = ko.observable(values.ChileProductName);
            this.PackagingProductKey = ko.observable(values.PackagingProductKey);
            this.PackagingProductName = ko.observable(values.PackagingProductName);
            this.TreatmentKey = ko.observable(values.TreatmentKey).extend({ treatmentType: true });
            this.UseCustomerSpec = ko.observable(values.UseCustomerSpec || false);
            this.CustomerProductCode = ko.observable(values.CustomerProductCode);
            this.Quantity = ko.numericObservable(values.Quantity);
            // this.PriceBase = ko.numericObservable(values.PriceBase, 2);
            this.PriceBase = preciseObservable(values.PriceBase, 2);
            this.PriceFreight = preciseObservable(values.PriceFreight || 0, 2);
            this.PriceTreatment = preciseObservable(values.PriceTreatment || 0, 2);
            this.PriceWarehouse = preciseObservable(values.PriceWarehouse || 0, 2);
            this.PriceRebate = preciseObservable(values.PriceRebate || 0, 2);

            /**
              * @param {(number|observable)} input - Initial value
              * @param {number} precision - Sets precision of output number
              */
            function preciseObservable(input, precision) {
              var value = ko.observable(parseFloat(input) || 0);
              var isPrecise = typeof precision === 'number';

              if (isPrecise) {
                value(value().toFixed(precision));
              }

              var comp = ko.computed({
                read: function() {
                  return value();
                },
                write: function(val) {
                  newVal = parseFloat(val);

                  if (isPrecise) {
                    newVal = newVal.toFixed(precision);
                  }

                  value(newVal);
                  comp.notifySubscribers();
                },
                owner: this
              });

              return comp;
            }

            this.chileProduct = ko.computed({
                read: _chileProduct,
                write: function (value) {
                    var val = value || {};
                    self.ChileProductKey(val.ProductKey || null);
                    self.ChileProductName(val.ProductNameFull || null);
                    self.ChileProductClassificationKey = val.ChileTypeKey;
                    _chileProduct(val);
                }
            });

            this.packagingProduct = ko.computed({
                read: _packagingProduct,
                write: function (value) {
                    var val = value || {};
                    self.PackagingProductKey(val.ProductKey || null);
                    self.PackagingProductName(val.ProductName || null);
                    _packagingProduct(value);
                }
            });

            this.totalWeight = ko.pureComputed(function () {
                var packaging = self.packagingProduct() || {};
                return (self.Quantity() * packaging.Weight) || 0;
            }, this);

            this.totalPricePerPound = ko.pureComputed(function () {
              var base = parseFloat(self.PriceBase() || 0);
              var freight = parseFloat(self.PriceFreight() || 0);
              var treatment = parseFloat(self.PriceTreatment() || 0);
              var warehouse = parseFloat(self.PriceWarehouse() || 0);
              var rebate = parseFloat(self.PriceRebate() || 0);

              return base + freight + treatment + warehouse - rebate;
            }, this);
            this.totalPricePerPound.displayValue = ko.pureComputed(function() {
                return currencyFormat.format(self.totalPricePerPound());
            });

            this.totalValue = ko.pureComputed(function () {
                return self.totalPricePerPound() * self.totalWeight() || 0;
            }, this);
            this.totalValue.displayValue = ko.pureComputed(function() {
                return currencyFormat.format(self.totalValue());
            });
        }

        ContractLineItem.prototype.buildDtoObject = function () {
            return ko.toJS({
                ChileProductKey: this.ChileProductKey,
                PackagingProductKey: this.PackagingProductKey,
                TreatmentKey: this.TreatmentKey,
                UseCustomerSpec: this.UseCustomerSpec,
                CustomerProductCode: this.CustomerProductCode,
                Quantity: this.Quantity,
                PriceBase: this.PriceBase,
                PriceFreight: this.PriceFreight,
                PriceTreatment: this.PriceTreatment,
                PriceWarehouse: this.PriceWarehouse,
                PriceRebate: this.PriceRebate,
            });
        };


        function CustomerContractDetailsViewModel(params) {

            var vm = this,
                _selectedContract = ko.observable();

            // Async Initialization
            this.isInit = ko.observable(false);

            vm.initDef = $.when(salesService.getCustomers(),
                salesService.getBrokers(),
                salesService.getPaymentTermOptions(),
                salesService.getWarehouses(),
                productsService.getChileProducts(app.lists.chileTypes.FinishedGoods.key),
                productsService.getPackagingProducts())
                .done(function (customers, brokers, paymentTermOptions, warehouses, products, packaging) {
                  if (customers[1] !== "success" ||
                      brokers[1] !== "success" ||
                      paymentTermOptions[1] !== "success" ||
                      warehouses[1] !== "success" ||
                      products[1] !== "success" ||
                      packaging[1] !== "success") {
                    throw new Error('Failed to load lookup data.');
                  }
                    var opts,
                        contract;

                    // Customers
                    vm.customerOptions(customers[0]);
                    setInitiallySelectedCusomerForContract.call(vm);

                    // Brokers
                    vm.brokerOptions(brokers[0]);
                    setInitiallySelectedBrokerForContract.call(vm);

                    // Payment Term Options
                    vm.paymentTermOptions(paymentTermOptions[0]);

                    // Warehouses
                    vm.fobOptions(ko.utils.arrayMap(warehouses[0], function(item) { return item.WarehouseName; }));

                    // Packaging
                    vm.packagingProductOptions(packaging[0]);

                    // Products
                    opts = ko.utils.arrayMap(products[0], function (product) {
                        product.ProductCodeAndName = product.ProductCode + ' - ' + product.ProductName;
                        return product;
                    });
                    vm.chileProductOptions(opts.sort(function (a, b) {
                        return a.ProductCode < b.ProductCode ? -1
                            : a.ProductCode === b.ProductCode ? 0 : 1;
                        })
                    );

                    vm.isInit(true);
            });

            this.subscriptions = [];

            var addrVisible = ko.observable(false);

            // Passed into address-book
            var setContactAddress = function( contactInfo ) {
              var contractData = this.contract();
              var contact = contractData.ContactAddress;

              contractData.ContactName( contactInfo.Name );
              contact.AddressLine1( contactInfo.Address.AddressLine1 );
              contact.AddressLine2( contactInfo.Address.AddressLine2 );
              contact.AddressLine3( contactInfo.Address.AddressLine3 );
              contact.City( contactInfo.Address.City );
              contact.State( contactInfo.Address.State );
              contact.PostalCode( contactInfo.Address.PostalCode );
              contact.Country( contactInfo.Address.Country );
            }.bind(this);

            this.contactLabelData = {
              visible: addrVisible,
              buttons: [{
                callback: function( contact ) {
                  setContactAddress( contact );
                  addrVisible( false );
                }.bind(this),
                text: 'Select Contact'
              }]
            };

            this.templateMode = ko.observable("edit-view");
            this.currentTab = ko.observable();
            this.customerOptions = ko.observableArray([]);
            this.brokerOptions = ko.observableArray([]);
            this.paymentTermOptions = ko.observableArray([]);
            this.fobOptions = ko.observableArray([]);
            this.chileProductOptions = ko.observableArray([]);
            this.packagingProductOptions = ko.observableArray([]);
            this.loadingContractDetails = ko.observable(false);
            this.basePriceAdjust = ko.numericObservable(0, 2);
            this.productToAdjust = ko.observable();
            this.chileClassifications = rvc.lists.chileClassifications.buildSelectListOptions();

            this.contract = ko.computed({
                read: _selectedContract,
                write: function(value) {
                    if (!value) return _selectedContract(null);
                    if (!(value instanceof CustomerContract)) {
                        value = new CustomerContract(value);
                    }
                    addrVisible(false);
                    setInitialOptionSectionsForContract.call(vm, value);
                    setInitiallySelectedCusomerForContract.call(vm, value);
                    setInitiallySelectedBrokerForContract.call(vm, value);
                    value.__esm = helpers.esmHelper(value, {
                        customMappings: {
                            ContractItems: function(vals) {
                                return ko.utils.arrayMap(vals, function(item) {
                                    var mapped = new ContractLineItem(item);
                                    setInitiallySelectedOptionsForContractItem.call(vm, mapped);
                                    return mapped;
                                });
                            },
                        },
                        customRevertFunctions: {
                            ContactAddress: function (val, target) {
                                target.update(val);
                            },
                            CustomerKey: function (val) {
                                value.customer(findCustomerOptionByKey.call(vm, val));
                                return val;
                            },
                        },
                        include: ['ContactAddress']
                    });
                    vm.initDef.then(function () {
                        setTimeout(function () { _selectedContract(value); }, 1000);
                    });
                    return value;
                }
            });

            this.showContacts = ko.command({
              execute: function() {
                addrVisible.call(this, true);
              },
              canExecute: function() {
                var contract = vm.contract();
                return contract && ko.unwrap(contract.customer);
              }
            });
            this.hideContacts = addrVisible.bind(this, false);
            this.addressVisible = ko.computed({
              read: function isAddrVisible() {
                return addrVisible() && this.contract();
              },
              write: function setVisibility( val ) {
                if ( !val ) {
                  this.hideContacts();
                }
              },
              owner: this
            });

            if (params.contract && ko.isObservable(params.contract)) {
                vm.subscriptions.push(this.contract.subscribe(params.contract));
            }
            if (params.contractKey && ko.isObservable(params.contractKey)) {
                vm.subscriptions.push(params.contractKey.subscribe(function (key) {
                  var input = params;

                    if (key === 'new') {
                      if (params.input && params.input.isClone === true) {
                        return;
                      } else {
                        vm.contract(new CustomerContract());
                      }
                    } else if (!key) {
                        vm.contract(null);
                    } else {
                        vm.loadContractByKey.call(vm, key);
                    }
                }));
            }

            this.saveContractCommand = ko.asyncCommand({
                execute: function (complete) {
                  var contract = vm.contract();
                    if (contract.validation.isValid()) {
                      var contract = vm.contract();
                      var dto = contract.buildDtoObject();
                      var dfd;

                      if (contract.isNew) {
                          dfd = salesService.createContract(dto);
                          dfd.then(function (data) {
                                  vm.contract().CustomerContractKey = data;
                                  showUserMessage("Contract Created Successfully");
                              })
                              .fail(function (xhr, status, message) { showUserMessage("Failed to create contract.", { description: "An error occurred while attempting to save the contract. " + message }); });
                      } else {
                          dfd = salesService.updateContract(contract.CustomerContractKey, dto);
                          dfd.then(function () { showUserMessage("Contract Saved Successfully"); })
                              .fail(function (xhr, status, message) { showUserMessage("Failed to Update Contract", { description: "An error occurred while attempting to save the contract. " + message }); });
                      }

                      dfd.always(complete);

                      return dfd;
                    } else {
                      var rejectSave = $.Deferred();

                      rejectSave.then(
                      null,
                      function(jqXHR, textStatus, errorThrown) {
                        showUserMessage("Could not save contract", { description: errorThrown });
                      });

                      rejectSave.reject(null, null, 'Please fill in all required fields');

                      contract.validation.errors.showAllMessages();
                      complete();

                      return rejectSave;
                    }
                },
                canExecute: function(isExecuting) {
                    return !isExecuting && vm.contract() && true;
                }
            });

            this.applyAdjustmentsCommand = ko.command({
                execute: function () {
                    var adjustmentAmount = vm.basePriceAdjust();
                    var productType = vm.productToAdjust();
                    var productTypeKey = productType && productType.key;

                    vm.contract().adjustBasePriceOfItems(adjustmentAmount, function (item) {
                        return !productTypeKey || item.ChileProductClassificationKey == productTypeKey;
                    });
                },
                canExecute: function() {
                    return vm.contract() && vm.basePriceAdjust();
                }
            });
        }

        CustomerContractDetailsViewModel.prototype.findContractByKey = function(key) {
            var dfd = $.Deferred();
            salesService.getContractDetails(key)
                .then(function (data) {
                    var contract = new CustomerContract(data);
                    dfd.resolve(contract);
                })
                .fail(function(xhr, status, message) {
                    showUserMessage("Failed to get contract details.", { description: message });
                    dfd.reject.call(null, arguments);
                });
            return dfd;
        };
        CustomerContractDetailsViewModel.prototype.loadContractByKey = function (key) {
            var vm = this;
            vm.loadingContractDetails(true);

            return CustomerContractDetailsViewModel.prototype.findContractByKey(key)
                .then(vm.contract)
                .always(function () { vm.loadingContractDetails(false); });
        };
        CustomerContractDetailsViewModel.prototype.clearContractSelection = function() {
            this.contract(null);
        };

        CustomerContractDetailsViewModel.prototype.dispose = function() {
            var subscriptions = this.subscriptions || [];
            ko.utils.arrayForEach(subscriptions, function (s) { s && s.dispose && s.dispose(); });

            var selectedContract = this.contract();
            if (selectedContract) {
                selectedContract.__esm.dispose();
            }
        };

        function setInitialOptionSectionsForContract(contract) {
            var vm = this;
            ko.utils.arrayForEach(contract.ContractItems(), function (item) {
                setInitiallySelectedOptionsForContractItem.call(vm, item);
            });
        }
        function setInitiallySelectedOptionsForContractItem(item) {
            var vm = this;
            var selectedChileProduct = ko.utils.arrayFirst(vm.chileProductOptions(), function (p) {
                return p.ProductKey === item.ChileProductKey.peek();
            });
            item.chileProduct(selectedChileProduct);

            var selectedPackagingProduct = ko.utils.arrayFirst(vm.packagingProductOptions(), function (p) {
                return p.ProductKey === item.PackagingProductKey.peek();
            });
            item.packagingProduct(selectedPackagingProduct);
        }
        function setInitiallySelectedCusomerForContract(contract) {
            if (!contract) return;
            var vm = this;
            var customerKey = contract.CustomerKey();
            if (customerKey) {
                if (vm.customerOptions().length) {
                    setCustomer(customerKey);
                } else {
                    var sub = vm.customerOptions.subscribe(function() {
                        setCustomer(customerKey, sub);
                        sub = null;
                    });
                    vm.subscriptions.push(sub);
                }
            } else contract.customer(null);

            function setCustomer(keyValue, subscription) {
                var selected = contract.customer() && contract.customer().CompanyKey === keyValue ?
                  contract.customer() :
                  findCustomerOptionByKey.call(vm, keyValue);
                contract.customer(selected || null);

                if (subscription) {
                    subscription.dispose();
                }
            }
        }
        function findCustomerOptionByKey(customerKey) {
            var vm = this;
            return ko.utils.arrayFirst(vm.customerOptions(), function (opt) { return opt.CompanyKey === customerKey; });
        }
        function setInitiallySelectedBrokerForContract(contract) {
            if (!contract) return;
            var vm = this;

            var brokerKey = contract.BrokerCompanyKey();
            if (brokerKey) {
                if (vm.brokerOptions()) {
                    setBroker(brokerKey);
                } else {
                    var sub = vm.brokerOptions().subscribe(function() {
                        setBroker(brokerKey, sub);
                        sub = null;
                    });
                    vm.subscriptions.push(sub);
                }

            } else contract.broker(null);

            function setBroker(keyValue, subscription) {
                var selected = contract.broker() && contract.broker().CompanyKey === brokerKey ?
                  contract.broker() :
                  ko.utils.arrayFirst(vm.brokerOptions(), function (opt) { return opt.CompanyKey === brokerKey; });
                contract.broker(selected || null);

                if (subscription) {
                    subscription.dispose();
                }
            }
        }

        //#endregion

        ko.components.register('notebook-control', notebookControlComponent);
        ko.components.register( 'contact-address-label-helper', require('App/components/common/address-book/company-address-label-helper') );

        return { viewModel: CustomerContractDetailsViewModel, template: templateMarkup };
    });
