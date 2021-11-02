define(['ko', 'page', 'services/salesService',
    'components/sales/customer-contract-summaries/customer-contract-summaries',
    'components/sales/customer-contract-details/customer-contract-details',
    'components/sales/customer-contract-shipment-summaries/customer-contract-shipment-summaries',
    'App/ko.bindingHandlers.sortableTable',
    'scripts/knockout-projections.min', 'scripts/sh.core'],
    function (ko, page, salesService, contractSummariesComponent, contractDetailsComponent, contractShipmentSummariesComponent) {
      require('App/helpers/koPunchesFilters.js');

      ko.punches.enableAll();

        return init();

    function init() {
        var contractKeyFilter = ko.observable();

        var vm = {
            selectedContract: ko.observable(),
            contractDetails: ko.observable(),
            cloneContractKey: ko.observable(),
            pastContractsForCustomer: ko.observableArray([]),
            searchContractKey: ko.observable(),
            selectedCustomer: ko.observable(),
            customerFilterOptions: ko.observableArray([]),
        };

        var contractSummariesVm = new contractSummariesComponent.viewModel({
            customerFilter: vm.selectedCustomer,
            contractKeyFilter: contractKeyFilter,
        });

        ko.components.register('customer-contract-summaries', {
            viewModel: { instance: contractSummariesVm },
            template: contractSummariesComponent.template,
        });

        var contractDetailsComponentVm = new contractDetailsComponent.viewModel({
            input: vm,
            contract: vm.contractDetails,
            contractKey: vm.selectedContract
        });
        ko.components.register('customer-contract-details', {
            viewModel: { instance: contractDetailsComponentVm },
            template: contractDetailsComponent.template,
        });

        ko.components.register('customer-contract-shipment-summaries', contractShipmentSummariesComponent);

        vm.isUpdating = false;
        vm.isAdding = false;

        vm.isAddrVisible = contractDetailsComponentVm.addressVisible;
        vm.loadingContractDetails = contractDetailsComponentVm.loadingContractDetails;
        vm.loadCustomerContractsCommand = contractSummariesVm.loadCustomerContractsCommand;
        vm.closeAddrCommand = ko.command({
          execute: function() {
            vm.isAddrVisible( false );
          },
          canExecute: function() {
            return true;
          }
        });

        //#region observables
        vm.showShipmentSummariesForContracts = vm.pastContractsForCustomer.filter(function(c) {
            return c.showDraw() === true;
        }).map(function(c) {
            return c.contract;
        });
        vm.isClone = false;
        //#endregion

        vm.loadContractByKeyCommand = ko.asyncCommand({
            execute: function (complete) {
                contractDetailsComponentVm.loadContractByKey(vm.searchContractKey())
                    .always(complete);
            },
            canExecute: function(isExecuting) {
                return !isExecuting && vm.searchContractKey() && true;
            }
        });
        vm.createNewCustomerContractCommand = ko.command({
            execute: function () {
              this.selectedContract('new');
              vm.goToContract('new');
            }
        });
        vm.closeContractCommand = ko.command({
            execute: function() {
                contractDetailsComponentVm.clearContractSelection();
                vm.contractDetails(null);
                vm.selectedContract(null);
                vm.pastContractsForCustomer([]);
                vm.isClone = false;
                vm.goToContract();
            },
            canExecute: function() {
                return contractDetailsComponentVm.contract() && true;
            }
        });
        vm.saveContractCommand = ko.command({
            execute: function () {
                var isNew = !ko.unwrap(contractDetailsComponentVm.contract().CustomerContractKey);

                if (isNew) {
                  vm.isAdding = true;
                } else {
                  vm.isUpdating = true;
                }

                return contractDetailsComponentVm.saveContractCommand.execute().then(
                function(data, textStatus, jqXHR) {
                  var key = contractDetailsComponentVm.contract().CustomerContractKey;

                  vm.goToContract(key);
                },
                function(jqXHR, textStatus, errorThrown) {
                   // Fail
                });
            },
            canExecute: function () {
                return contractDetailsComponentVm.saveContractCommand.canExecute() &&
                  vm.contractDetails() &&
                  true;
            }
        });
        vm.cloneContractCommand = ko.asyncCommand({
            execute: function (complete) {
                var key = vm.cloneContractKey() || vm.selectedContract();
                if (!key) throw new Error("Missing Contract Key");

                var doClone = function () {
                    contractDetailsComponentVm.findContractByKey(key)
                    .then(function (contract) {
                      vm.isClone = true;
                      contractDetailsComponentVm.contract(contract.createNewFromCopy());
                      vm.goToContract('new');
                    })
                    .fail(function (xhr, status, message) {
                        showUserMessage("Unable to copy contract", { description: "The copy failed due to an error retrieving the contract from the database." });
                    })
                    .always(complete);
                };

                // Check if dirty
                var contract = contractDetailsComponentVm.contract();
                if (contract && contract.hasChanges()) {
                    showUserMessage("Save changes before copying?", {
                        description: "The current contract has unsaved changes. Would you like to save before we copy the contract? Click <strong>Yes</strong> to save the changes and then copy the saved contract. Click <strong>No</strong> to discard the changes and copy the contract without the changes. Click <strong>Cancel</strong> to cancel the copy operation.",
                        type: 'yesnocancel',
                        onYesClick: function () {
                            vm.saveContractCommand.execute().then(doClone);
                        },
                        onNoClick: doClone,
                        onCancelClick: complete
                    });
                } else doClone();
            },
            canExecute: function (isExecuting) {
                var isNew = vm.contractDetails() ? vm.contractDetails().isNew : false;
                return !isExecuting && (vm.cloneContractKey() || vm.selectedContract()) && !isNew;
            }
        });
        vm.deleteContractCommand = ko.asyncCommand({
            execute: function (complete) {
                if (showUserMessage("Are you sure you want to delete this contract?", {
                    description: "This operation cannot be undone. To continue with the deletion of the contract, click \"Yes\" otherwise, click \"No\" to abort the deletion.",
                    type: "yesno",
                    onYesClick: function() {
                        salesService.deleteContract(contractDetailsComponentVm.contract().CustomerContractKey)
                            .then(function() {
                                vm.closeContractCommand.execute();
                                showUserMessage("Contract deleted successfully.");
                                //todo: remove summary item
                            })
                            .always(complete);
                    },
                    onNoClick: complete
                }));
            },
            canExecute: function (isExecuting) {
                return !isExecuting && vm.contractDetails() && !vm.contractDetails().isNew && true;
            }
        });

        //#region subscribers
        var contractSubscriptions = [];
        contractDetailsComponentVm.contract.subscribe(function (contract) {
            if (contractSubscriptions.length) {
                ko.utils.arrayForEach(contractSubscriptions, function (sub) {
                  var disposeResult = sub.dispose && sub.dispose();
                });
                contractSubscriptions = [];
            }

            if (!contract) {
                vm.pastContractsForCustomer([]);
                return;
            }

            if (contract.CustomerKey()) loadRecentContractsForCustomer.call(vm, contract.CustomerKey(), contract.copiedFrom);
            else vm.pastContractsForCustomer([]);

            contractSubscriptions.push(contract.CustomerKey.subscribe(function(customerKey) {
                loadRecentContractsForCustomer.call(vm, customerKey);
            }));
        });

        vm.selectedCustomer.subscribe(function () {
            vm.loadCustomerContractsCommand.execute();
        });
        //#endregion
        vm.contractClicked = contractClicked.bind(vm);
        loadCustomers.call(vm);

        vm.goToContract = function(key) {
          if (key === undefined || key === null) {
            page('/');
          } else if (key === 'new') {
            page('/new');
          } else {
            page('/' + key);
          }
        };
        // Routing
        vm.previousKey = '';
        page.base('/Customers/Contracts');
        page('/:key?', navigateToKey);

        function navigateToKey(ctx) {
          var key = ctx.params.key;

          if (key === undefined) {
            document.title = 'Customer Contracts';
            vm.selectedContract();
          } else if (key === 'new') {
            document.title = 'New Contract';
            vm.selectedContract();
            if (!vm.isClone) {
              contractDetailsComponentVm.clearContractSelection();
            } else {
              vm.isClone = false;
            }
          } else {
            if (vm.isUpdating || vm.isAdding) {
              contractDetailsComponentVm.loadContractByKey(key)
              .done(function(data, textStatus, jqXHR) {
                if (vm.isUpdating) {
                  contractSummariesVm.updateContractSummaryItem(key, toContractSummaryItem.call(data));
                } else {
                  contractSummariesVm.addContractSummaryItem(toContractSummaryItem.call(data));
                }
              })
              .fail(function(jqXHR, textStatus, errorThrown) {
              })
              .always(function() {
                vm.isUpdating = false;
                vm.isAdding = false;
              });
            } else {
              vm.selectedContract(key);
            }
          }

          vm.previousKey = key;

          return;
        }

        vm.contractDetails.subscribe(function (x) {
              var title = 'Customer Contracts';

              if (x && x.CustomerContractKey && x.CustomerContractKey !== 'new'){
                title = 'Contract ' + x.CustomerContractKey + ' - ' + x.CustomerName();
                document.title = title;
              }
        });

        ko.applyBindings(vm);
        page();
    }

    function contractClicked(context, args) {
        if (!args.originalEvent) throw new Error("Invalid argument \"args\". Missing event information.");
        var srcElement = args.originalEvent.srcElement || {};
        var $tr = $(srcElement).closest('tr');
        if (!$tr.length) throw new Error("Unable to find \"tr\" element for source element.");

        var selectedContract = ko.contextFor($tr[0]).$data;
        if (selectedContract) {
          this.selectedContract(selectedContract.CustomerContractKey);
          this.goToContract(selectedContract.CustomerContractKey);
        }
    }

    function toContractSummaryItem() {
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
            AverageBasePrice: this.averageBasePrice,
            AverageTotalPrice: this.averageTotalPrice,
            SumQuantity: this.totalQuantityOnContract,
            SumWeight: this.totalPoundsOnContract,
        });
    }

    function loadRecentContractsForCustomer(key, copyee) {
        var vm = this;
        salesService.getContractsForCustomer(key, 3)
            .then(function(data) {
                vm.pastContractsForCustomer(ko.utils.arrayMap(data, function(c) {
                    return {
                        contract: c,
                        showDraw: ko.observable(c.CustomerContractKey == copyee),
                    };
                }));
            });
    }

    function loadCustomers() {
        var vm = this;
        salesService.getCustomers()
            .then(vm.customerFilterOptions);
    }
});
