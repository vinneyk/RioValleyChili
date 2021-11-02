define(['./customer-contract-shipment-summaries.html', 'services/salesService', 'helpers/koHelpers', 'ko'],
    function (templateMarkup, salesService, koHelpers, ko) {

        var currencyFormat = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' });

        function CustomerContractShipmentSummariesViewModel(params) {
            if (!(this instanceof CustomerContractShipmentSummariesViewModel)) {
                return new CustomerContractShipmentSummariesViewModel(params);
            }

            var vm = this;

            this.reportItems = ko.observableArray([]);
            this._subscriptions = [];

            var contractsToReport = getContractKeysFromParam();

            if (ko.isObservable(params.contractsToReport)) {
                vm._subscriptions.push(params.contractsToReport.subscribe(function (values) {

                    // convert cache object into array
                    var previousState = [];
                    for (var p in contractsToReport) {
                        if (!contractsToReport.hasOwnProperty(p)) continue;
                        previousState.push(contractsToReport[p]);
                        break;
                    }
                    

                    var differences = ko.utils.compareArrays(previousState || [], values || []);
                    ko.utils.arrayForEach(differences, function(diff) {
                        var key = getContractKey(diff.value);
                        switch(diff.status) {
                            case 'deleted': stopReportingContract(key);
                                break;
                            case 'added' : startReportingContract(key, diff.value);
                                break;
                        }
                    });
                }));
            }

            return this;

            function getContractKeysFromParam() {
                return ko.unwrap(params.contractsToReport).toObj(getContractKey);
            }
            function getContractKey(target) {
                var keyValue = typeof target === "string" ? target
                        : target && ko.unwrap(target.CustomerContractKey);

                if (!keyValue) throw new Error("Unable to determine contract key from parameter target. No CustomerContractKey property was found.");

                return keyValue;
            }
            function stopReportingContract(key) {
                var itemToRemove = ko.utils.arrayFirst(vm.reportItems(), function(item) {
                    return item.ContractKey === key;
                });
                if (itemToRemove) {
                    ko.utils.arrayRemoveItem(vm.reportItems(), itemToRemove);
                    vm.reportItems.notifySubscribers(vm.reportItems());
                }

                delete contractsToReport[key];
            }
            function startReportingContract(key, value) {
                if (contractsToReport[key]) return;

                contractsToReport[key] = value;

                var item = {
                    ContractKey: key,
                    ContractHeader: ko.observable(),
                    shipmentSummaries: ko.observableArray([]),
                    TotalContractPounds: ko.observable(0),
                    TotalPoundsShipped: ko.observable(0),
                    TotalPoundsPending: ko.observable(0),
                    TotalPoundsRemaining: ko.observable(0),
                    TotalContractValueFormatted: ko.observable(0),
                };

                koHelpers.ajaxStatusHelper(item);

                salesService.getShipmentSummaryForContract(key)
                    .then(function (data) {
                        if (!(data && data.length)) return;
                        item.ContractHeader({
                            ContractKey: data[0].ContractKey,
                            ContractNumber: data[0].ContractNumber,
                            ContractStatus: data[0].ContractStatus,
                            ContractBeginDate: data[0].ContractBeginDate,
                            ContractEndDate: data[0].ContractEndDate,
                            reportUrl: data[0].Links["self"] && data[0].Links["self"].HRef,
                        });

                        var totalContractValue = 0,
                            totalContractPounds = 0,
                            totalPoundsShipped = 0,
                            totalPoundsPending = 0,
                            totalPoundsRemaining = 0;

                        ko.utils.arrayPushAll(item.shipmentSummaries(), ko.utils.arrayMap(data, function(i) {
                            totalContractValue += i.ContractItemValue;
                            totalContractPounds += i.ContractItemPounds;
                            totalPoundsShipped += i.TotalPoundsShippedForContractItem;
                            totalPoundsPending += i.TotalPoundsPendingForContractItem;
                            totalPoundsRemaining += i.TotalPoundsRemainingForContractItem;

                            return {
                                ProductName: i.ProductName,
                                PackagingName: i.PackagingName,
                                Treatment: i.Treatment,
                                ContractItemValue: currencyFormat.format(i.ContractItemValue),
                                ContractItemPounds: i.ContractItemPounds.toLocaleString(),
                                TotalPoundsShippedForContractItem: i.TotalPoundsShippedForContractItem.toLocaleString(),
                                TotalPoundsPendingForContractItem: i.TotalPoundsPendingForContractItem.toLocaleString(),
                                TotalPoundsRemainingForContractItem: i.TotalPoundsRemainingForContractItem.toLocaleString(),
                                BasePrice: i.BasePrice,
                                CustomerProductCode: i.CustomerProductCode,
                            };
                        }));

                        item.TotalContractValueFormatted(currencyFormat.format(totalContractValue));
                        item.TotalContractPounds(totalContractPounds.toLocaleString());
                        item.TotalPoundsShipped(totalPoundsShipped.toLocaleString());
                        item.TotalPoundsPending(totalPoundsPending.toLocaleString());
                        item.TotalPoundsRemaining(totalPoundsRemaining.toLocaleString());
                        

                        item.shipmentSummaries.notifySubscribers(item.shipmentSummaries());
                        console.log(item);
                    });

                vm.reportItems.push(item);
            }
        }

        CustomerContractShipmentSummariesViewModel.prototype.dispose = function() {
            ko.utils.arrayForEach(this._subscriptions || [], function(sub) { sub && sub.dispose && sub.dispose(); });
        };

        return { viewModel: CustomerContractShipmentSummariesViewModel, template: templateMarkup };
    });
