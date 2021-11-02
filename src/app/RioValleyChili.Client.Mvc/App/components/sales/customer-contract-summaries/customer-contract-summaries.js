define(['./customer-contract-summaries.html', 'services/salesService', 'app', 'ko',
    'scripts/sh.knockout.customObservables', 'scripts/knockout.command', 'scripts/sh.core.js', 'App/koExtensions'],
    function (templateMarkup, salesService, app, ko) {

        //#region model definitions

        function CustomerContractSummary(input) {
            var values = input || {};
            var currencyFormat = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' });

            this.CustomerContractKey = values.CustomerContractKey;
            this.ContractNumber = values.ContractNumber;
            this.ContractDate = new Date(values.ContractDate);
            this.TermBegin = values.TermBegin && new Date(values.TermBegin).toString('M/d/yyyy');
            this.TermEnd = values.TermEnd && new Date(values.TermEnd).toString('M/d/yyyy');
            this.CustomerPurchaseOrder = values.CustomerPurchaseOrder;
            this.ContractType = app.lists.contractTypes.findByKey(values.ContractType || 0).value;
            this.ContractStatus = app.lists.contractStatuses.findByKey(values.ContractStatus || 0).value;
            this.CustomerKey = values.CustomerKey;
            this.CustomerName = values.CustomerName;
            this.ContactName = values.ContactName;
            this.BrokerCompanyKey = values.BrokerCompanyKey;
            this.BrokerCompanyName = values.BrokerCompanyName;
            this.DistributionWarehouseKey = values.DistributionWarehouseKey;
            this.DistributionWarehouseName = values.DistributionWarehouseName;
            this.AverageBasePrice = currencyFormat.format(values.AverageBasePrice || 0);
            this.AverageTotalPrice = currencyFormat.format(values.AverageTotalPrice || 0);
            this.SumQuantity = values.SumQuantity || 0;
            this.SumWeight = values.SumWeight || 0;
        }

        CustomerContractSummary.prototype.defaultValues = {
            AverageBasePrice: 0,
            AverageTotalPrice: 0,
            SumQuantity: 0,
            SumWeight: 0,
        };

        function CustomerContracts(params) {

            this.customerFilter = ko.isObservable(params.customerFilter) ?
              params.customerFilter :
              ko.observable(params.customerFilter);

            var vm = this,
                contractsDataPager = salesService.getContractSummariesDataPager({
                    parameters: { customerKey: vm.customerFilter },
                    onNewPageSet: function() {
                        vm.customerContracts([]);
                    }
                });


            this.contractKeyFilter = ko.isObservable(params.contractKeyFilter) ?
              params.contractKeyFilter :
              ko.observable(params.contractKeyFilter);

            this.customerContracts = ko.observableArray([]);
            this.totalPages = ko.observable();
            this.currentPageNumber = ko.observable();

            // commands
            this.loadCustomerContractsCommand = ko.asyncCommand({
                execute: function (complete) {
                    contractsDataPager.nextPage()
                        .then(loadCustomerContractsSuccess)
                        .fail(loadCustomerContractsFailure)
                        .always(complete);
                },
                canExecute: function(isExecuting) {
                    return !isExecuting;
                }
            }, this);

            this.loadCustomerContractsCommand.execute();


            function loadCustomerContractsSuccess(response) {
                ko.utils.arrayPushAll(vm.customerContracts(), ko.utils.arrayMap(response.Data, mapContractSummary));
                vm.customerContracts.notifySubscribers(vm.customerContracts());
                vm.totalPages(response.TotalPageCount);
                vm.currentPageNumber(response.CurrentPage);
            }
            function loadCustomerContractsFailure(xhr, status, message)
            {
                showUserMessage("Failed to load customer contracts", { description: message });
            }
        }

        CustomerContracts.prototype.findContractSummaryItemByKey = function (key) {
            if (!key) return null;
            return ko.utils.arrayFirst(this.customerContracts(), function(c) {
                return c.CustomerContractKey === key;
            });
        };
        CustomerContracts.prototype.addContractSummaryItem = function (values) {
            if (!values) return null;
            var newItem = values instanceof CustomerContractSummary ? values : new CustomerContractSummary(ko.toJS(values));
            this.customerContracts.unshift(newItem);
            return newItem;
        };
        CustomerContracts.prototype.updateContractSummaryItem = function (key, newValues) {
            var contract = this.findContractSummaryItemByKey(key);
            var newContract = new CustomerContractSummary(ko.toJS(newValues));
            var index = ko.utils.arrayIndexOf(this.customerContracts(), contract);
            if (index > -1) {
                this.customerContracts.splice(index, 1, newContract);
                return newContract;
            } else return this.addContractSummaryItem(newContract);
        };

        //#endregion
        
        return { viewModel: CustomerContracts, template: templateMarkup };

        //#region private methods
        function mapContractSummary(values) {
            return new CustomerContractSummary(values);
        }
        //#endregion
    });
