using System;
using System.Collections.Generic;
using System.Linq;
using EF_Split_Projector;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Commands.Customer;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Conductors;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Core;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class SalesServiceProvider : IUnitOfWorkContainer<IInventoryShipmentOrderUnitOfWork>
    {
        IInventoryShipmentOrderUnitOfWork IUnitOfWorkContainer<IInventoryShipmentOrderUnitOfWork>.UnitOfWork { get { return _inventoryShipmentOrderUnitOfWork; } }
        private readonly IInventoryShipmentOrderUnitOfWork _inventoryShipmentOrderUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public SalesServiceProvider(IInventoryShipmentOrderUnitOfWork inventoryShipmentOrderUnitOfWork, ITimeStamper timeStamper)
        {
            if(inventoryShipmentOrderUnitOfWork == null) { throw new ArgumentNullException("inventoryShipmentOrderUnitOfWork"); }
            _inventoryShipmentOrderUnitOfWork = inventoryShipmentOrderUnitOfWork;

            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }
            _timeStamper = timeStamper;
        }

        public IResult<IEnumerable<ICompanySummaryReturn>> GetCustomersForBroker(string brokerKey)
        {
            if(brokerKey == null) { throw new ArgumentNullException("brokerKey"); }

            var keyResult = KeyParserHelper.ParseResult<ICompanyKey>(brokerKey);
            if(!keyResult.Success)
            {
                return keyResult.ConvertTo<IEnumerable<ICompanySummaryReturn>>();
            }
            var parsedBrokerKey = new CompanyKey(keyResult.ResultingObject);

            var broker = _inventoryShipmentOrderUnitOfWork.CompanyRepository.FindByKey(parsedBrokerKey, c => c.CompanyTypes);
            if(broker == null)
            {
                return new InvalidResult<IEnumerable<ICompanySummaryReturn>>(null, string.Format(UserMessages.CompanyNotFound, brokerKey));
            }
            if(broker.CompanyTypes.All(t => t.CompanyTypeEnum != CompanyType.Broker))
            {
                return new InvalidResult<IEnumerable<ICompanySummaryReturn>>(null, string.Format(UserMessages.CompanyNotOfType, brokerKey, CompanyType.Broker));
            }

            var predicate = CustomerPredicates.ByBroker(parsedBrokerKey);
            var select = CustomerProjectors.SelectCompanySummary();

            var customers = _inventoryShipmentOrderUnitOfWork.CustomerRepository.Filter(predicate).AsExpandable().Select(select).ToList();

            return new SuccessResult<IEnumerable<ICompanySummaryReturn>>(customers);
        }

        public IResult AssignCustomerToBroker(string brokerKey, string customerKey)
        {
            if(brokerKey == null) { throw new ArgumentNullException("brokerKey"); }
            if(customerKey == null) { throw new ArgumentNullException("customerKey"); }

            var brokerKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(brokerKey);
            if(!brokerKeyResult.Success)
            {
                return brokerKeyResult;
            }

            var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(customerKey);
            if(!customerKeyResult.Success)
            {
                return customerKeyResult;
            }

            var commandResult = new AssignCustomerToBrokerCommand(_inventoryShipmentOrderUnitOfWork).Execute(new CompanyKey(brokerKeyResult.ResultingObject), new CustomerKey(customerKeyResult.ResultingObject));
            if(!commandResult.Success)
            {
                return commandResult;
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            return new SuccessResult();
        }

        public IResult RemoveCustomerFromBroker(string brokerKey, string customerKey)
        {
            if(brokerKey == null) { throw new ArgumentNullException("brokerKey"); }
            if(customerKey == null) { throw new ArgumentNullException("customerKey"); }

            var brokerKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(brokerKey);
            if(!brokerKeyResult.Success)
            {
                return brokerKeyResult;
            }

            var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(customerKey);
            if(!customerKeyResult.Success)
            {
                return customerKeyResult;
            }

            var commandResult = new RemoveCustomerFromBrokerCommand(_inventoryShipmentOrderUnitOfWork).Execute(new CompanyKey(brokerKeyResult.ResultingObject), new CustomerKey(customerKeyResult.ResultingObject));
            if(!commandResult.Success)
            {
                return commandResult;
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            return new SuccessResult();
        }

        [SynchronizeOldContext(NewContextMethod.SyncContract)]
        public IResult<string> CreateCustomerContract(ICreateCustomerContractParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedParametersResult = parameters.ToParsedParameters();
            if(!parsedParametersResult.Success)
            {
                return parsedParametersResult.ConvertTo<string>();
            }

            var commandResult = new CreateCustomerContractConductor(_inventoryShipmentOrderUnitOfWork).Execute(parsedParametersResult.ResultingObject, _timeStamper.CurrentTimeStamp);
            if(!commandResult.Success)
            {
                return commandResult.ConvertTo<string>();
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            var contractKey = commandResult.ResultingObject.ToContractKey();
            return SyncParameters.Using(new SuccessResult<string>(contractKey), new SyncCustomerContractParameters
                {
                    ContractKey = contractKey,
                    New = true
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncContract)]
        public IResult<string> UpdateCustomerContract(IUpdateCustomerContractParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedParametersResult = parameters.ToParsedParameters();
            if(!parsedParametersResult.Success)
            {
                return parsedParametersResult.ConvertTo<string>();
            }

            var commandResult = new UpdateCustomerContractConductor(_inventoryShipmentOrderUnitOfWork).Execute(parsedParametersResult.ResultingObject, _timeStamper.CurrentTimeStamp);
            if(!commandResult.Success)
            {
                return commandResult.ConvertTo<string>();
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            var contractKey = commandResult.ResultingObject.ToContractKey();
            return SyncParameters.Using(new SuccessResult<string>(contractKey), new SyncCustomerContractParameters
                {
                    ContractKey = contractKey,
                    New = false
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncContractsStatus)]
        public IResult SetCustomerContractsStatus(ISetContractsStatusParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var contractKeys = new List<ContractKey>();
            foreach(var key in parameters.ContractKeys.Distinct().Where(k => !string.IsNullOrWhiteSpace(k)))
            {
                var keyResult = KeyParserHelper.ParseResult<IContractKey>(key);
                if(!keyResult.Success)
                {
                    return keyResult;
                }
                contractKeys.Add(new ContractKey(keyResult.ResultingObject));
            }

            if(contractKeys.Any())
            {
                var predicate = contractKeys.Aggregate(PredicateBuilder.False<Contract>(), (c, n) => c.Or(n.FindByPredicate)).ExpandAll();
                var contracts = _inventoryShipmentOrderUnitOfWork.ContractRepository.Filter(predicate).ToList();

                var missingKey = contractKeys.FirstOrDefault(k => contracts.All(c => !k.FindByPredicate.Invoke(c)));
                if(missingKey != null)
                {
                    return new InvalidResult(string.Format(UserMessages.CustomerContractNotFound, missingKey));
                }

                contracts.ForEach(c => c.ContractStatus = parameters.ContractStatus);

                _inventoryShipmentOrderUnitOfWork.Commit();
            }

            return SyncParameters.Using(new SuccessResult(), contractKeys);
        }

        [SynchronizeOldContext(NewContextMethod.DeleteContract)]
        public IResult RemoveCustomerContract(string customerContractKey)
        {
            var contractKey = KeyParserHelper.ParseResult<IContractKey>(customerContractKey);
            if(!contractKey.Success)
            {
                return contractKey.ConvertTo<int?>();
            }

            int? contractId;
            var removeResult = new RemoveContractCommand(_inventoryShipmentOrderUnitOfWork).Execute(contractKey.ResultingObject, out contractId);
            if(!removeResult.Success)
            {
                return removeResult.ConvertTo<int?>();
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), contractId);
        }

        public IResult<ICustomerContractDetailReturn> GetCustomerContract(string customerContractKey)
        {
            if(customerContractKey == null) { throw new ArgumentNullException("customerContractKey"); }

            var contractKeyResult = KeyParserHelper.ParseResult<IContractKey>(customerContractKey);
            if(!contractKeyResult.Success)
            {
                return contractKeyResult.ConvertTo<ICustomerContractDetailReturn>();
            }
            var contractKey = new ContractKey(contractKeyResult.ResultingObject);

            var select = ContractProjectors.SelectDetail();
            var contractDetail = _inventoryShipmentOrderUnitOfWork.ContractRepository.All()
                .Where(contractKey.FindByPredicate)
                .SplitSelect(select).FirstOrDefault();
            if(contractDetail == null)
            {
                return new InvalidResult<ICustomerContractDetailReturn>(null, string.Format(UserMessages.CustomerContractNotFound, customerContractKey));
            }

            return new SuccessResult<ICustomerContractDetailReturn>(contractDetail);
        }

        public IResult<IQueryable<ICustomerContractSummaryReturn>> GetCustomerContracts(FilterCustomerContractsParameters parameters)
        {
            var parsedFiltersResult = parameters.ParseToPredicateBuilderFilters();
            if(!parsedFiltersResult.Success)
            {
                return parsedFiltersResult.ConvertTo<IQueryable<ICustomerContractSummaryReturn>>();
            }

            var predicateResult = ContractPredicateBuilder.BuildPredicate(parsedFiltersResult.ResultingObject);
            if(!predicateResult.Success)
            {
                return predicateResult.ConvertTo<IQueryable<ICustomerContractSummaryReturn>>();
            }

            var select = ContractProjectors.SelectSummary();
            var query = _inventoryShipmentOrderUnitOfWork.ContractRepository.All()
                .Where(predicateResult.ResultingObject)
                .SplitSelect(select);
            return new SuccessResult<IQueryable<ICustomerContractSummaryReturn>>(query);
        }

        public IResult<IEnumerable<string>> GetDistinctContractPaymentTerms()
        {
            var paymentTerms = _inventoryShipmentOrderUnitOfWork.ContractRepository.All()
                .Select(c => c.PaymentTerms)
                .Distinct().ToList()
                .Where(p => !string.IsNullOrWhiteSpace(p)).ToList();

            return new SuccessResult<IEnumerable<string>>(paymentTerms);
        }

        public IResult<ICustomerContractOrdersReturn> GetOrdersForCustomerContract(string customerContractKey)
        {
            var contractKeyResult = KeyParserHelper.ParseResult<IContractKey>(customerContractKey);
            if(!contractKeyResult.Success)
            {
                return contractKeyResult.ConvertTo<ICustomerContractOrdersReturn>();
            }
            var contractKey = new ContractKey(contractKeyResult.ResultingObject);
            var select = ContractProjectors.SelectContractOrders(_inventoryShipmentOrderUnitOfWork);

            var contract = _inventoryShipmentOrderUnitOfWork.ContractRepository.All()
                .AsExpandable()
                .Where(contractKey.FindByPredicate)
                .Select(select).FirstOrDefault();
            if(contract == null)
            {
                return new InvalidResult<ICustomerContractOrdersReturn>(null, string.Format(UserMessages.CustomerContractNotFound, customerContractKey));
            }

            return new SuccessResult<ICustomerContractOrdersReturn>(contract);
        }

        public IResult<IContractShipmentSummaryReturn> GetContractShipmentSummary(string contractKey)
        {
            var contractKeyResult = KeyParserHelper.ParseResult<IContractKey>(contractKey);
            if(!contractKeyResult.Success)
            {
                return contractKeyResult.ConvertTo<IContractShipmentSummaryReturn>();
            }
            var key = new ContractKey(contractKeyResult.ResultingObject);
            var select = ContractProjectors.SplitSelectShipmentSummary();

            var contract = _inventoryShipmentOrderUnitOfWork.ContractRepository
                .All().Where(key.FindByPredicate)
                .SplitSelect(select).FirstOrDefault();
            if(contract == null)
            {
                return new InvalidResult<IContractShipmentSummaryReturn>(null, string.Format(UserMessages.CustomerContractNotFound, contractKey));
            }

            return new SuccessResult<IContractShipmentSummaryReturn>(contract);
        }

        [SynchronizeOldContext(NewContextMethod.CompleteExpiredContracts)]
        public IResult CompleteExpiredContracts()
        {
            var result = new CompleteExpiredContractsCommand(_inventoryShipmentOrderUnitOfWork).Execute();
            if(result.Success)
            {
                _inventoryShipmentOrderUnitOfWork.Commit();
                return SyncParameters.Using(result, result.ResultingObject);
            }

            return result;
        }

        [SynchronizeOldContext(NewContextMethod.SyncSalesOrder)]
        public IResult<string> CreateSalesOrder(ICreateSalesOrderParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedParametersResult = parameters.ToParsedParameters();
            if(!parsedParametersResult.Success)
            {
                return parsedParametersResult.ConvertTo<string>();
            }

            var conductorResult = new CreateSalesOrderConductor(_inventoryShipmentOrderUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parsedParametersResult.ResultingObject);
            if(!conductorResult.Success)
            {
                return conductorResult.ConvertTo<string>();
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            var salesOrderKey = conductorResult.ResultingObject.ToSalesOrderKey();
            return SyncParameters.Using(new SuccessResult<string>(salesOrderKey), new SyncSalesOrderParameters
                {
                    SalesOrderKey = salesOrderKey,
                    New = true
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncSalesOrder)]
        public IResult UpdateSalesOrder(IUpdateSalesOrderParameters parameters)
        {
            var parsedParametersResult = parameters.ToParsedParameters();
            if(!parsedParametersResult.Success)
            {
                return parsedParametersResult;
            }

            var updateResult = new UpdateSalesOrderConductor(_inventoryShipmentOrderUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parsedParametersResult.ResultingObject);
            if(!updateResult.Success)
            {
                return updateResult;
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            var salesOrderKey = updateResult.ResultingObject.ToSalesOrderKey();
            return SyncParameters.Using(new SuccessResult<string>(salesOrderKey), new SyncSalesOrderParameters
                {
                    SalesOrderKey = salesOrderKey,
                    New = false
                });
        }

        [SynchronizeOldContext(NewContextMethod.DeleteSalesOrder)]
        public IResult DeleteSalesOrder(string salesOrderKey)
        {
            var keyResult = KeyParserHelper.ParseResult<ISalesOrderKey>(salesOrderKey);
            if(!keyResult.Success)
            {
                return keyResult;
            }

            int? orderNum;
            var deleteResult = new DeleteSalesOrderConductor(_inventoryShipmentOrderUnitOfWork).Execute(keyResult.ResultingObject, out orderNum);
            if(!deleteResult.Success)
            {
                return deleteResult;
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), orderNum);
        }

        public IResult<ISalesOrderDetailReturn> GetSalesOrder(string salesOrderKey)
        {
            if(salesOrderKey == null) { throw new ArgumentNullException("salesOrderKey"); }

            ISalesOrderDetailReturn salesOrder = null;
            var select = SalesOrderProjectors.SplitSelectDetail(_inventoryShipmentOrderUnitOfWork, _timeStamper.CurrentTimeStamp);

            var orderKeyResult = KeyParserHelper.ParseResult<ISalesOrderKey>(salesOrderKey);
            if(orderKeyResult.Success)
            {
                var orderKey = orderKeyResult.ResultingObject.ToSalesOrderKey();
                salesOrder = _inventoryShipmentOrderUnitOfWork.SalesOrderRepository.Filter(orderKey.FindByPredicate).SplitSelect(select).FirstOrDefault();
            }

            if(salesOrder == null)
            {
                int moveNum;
                if(int.TryParse(salesOrderKey, out moveNum))
                {
                    salesOrder = _inventoryShipmentOrderUnitOfWork.SalesOrderRepository.Filter(o => o.InventoryShipmentOrder.MoveNum == moveNum).SplitSelect(select).FirstOrDefault();
                }

                if(salesOrder == null)
                {
                    return orderKeyResult.Success ? new InvalidResult<ISalesOrderDetailReturn>(null, string.Format(UserMessages.SalesOrderNotFound, salesOrderKey)) : orderKeyResult.ConvertTo<ISalesOrderDetailReturn>();
                }
            }

            return new SuccessResult<ISalesOrderDetailReturn>(salesOrder);
        }

        public IResult<IQueryable<ISalesOrderSummaryReturn>> GetSalesOrders(FilterSalesOrdersParameters parameters)
        {
            var parsedFiltersResult = parameters.ParseToPredicateBuilderFilters();
            if(!parsedFiltersResult.Success)
            {
                return parsedFiltersResult.ConvertTo<IQueryable<ISalesOrderSummaryReturn>>();
            }

            var predicateResult = SalesOrderPredicateBuilder.BuildPredicate(_inventoryShipmentOrderUnitOfWork, parsedFiltersResult.ResultingObject);
            if(!predicateResult.Success)
            {
                return predicateResult.ConvertTo<IQueryable<ISalesOrderSummaryReturn>>();
            }

            var select = SalesOrderProjectors.SplitSelectSummary();
            var query = _inventoryShipmentOrderUnitOfWork.SalesOrderRepository.All()
                .Where(predicateResult.ResultingObject)
                .SplitSelect(select);
            return new SuccessResult<IQueryable<ISalesOrderSummaryReturn>>(query);
        }

        [Issue("Active spec should be that of the CustomerOrderItem, not ContractItem.",
                References = new[] { "RVCADMIN-1177", "https://solutionhead.slack.com/archives/rvc/p1469045866000004" })]
        public IResult<IPickableInventoryReturn> GetInventoryToPickForOrder(FilterInventoryForShipmentOrderParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            InventoryShipmentOrderKey orderKey;
            InventoryPickOrderItemKey orderItemKey;
            var filterResults = parameters.ParseToPredicateBuilderFilters(out orderKey, out orderItemKey);
            if(!filterResults.Success)
            {
                return filterResults.ConvertTo<IPickableInventoryReturn>();
            }
            var salesOrderKey = new SalesOrderKey(orderKey);
            var salesOrderItemKey = orderItemKey == null ? null : new SalesOrderItemKey(orderItemKey);

            var salesOrder = _inventoryShipmentOrderUnitOfWork.SalesOrderRepository.FindByKey(salesOrderKey,
                o => o.InventoryShipmentOrder.SourceFacility,
                o => o.SalesOrderItems.Select(i => i.ContractItem),
                o => o.SalesOrderItems.Select(i => i.InventoryPickOrderItem));
            if(salesOrder == null)
            {
                return new InvalidResult<IPickableInventoryReturn>(null, string.Format(UserMessages.SalesOrderNotFound, orderKey.KeyValue));
            }

            IDictionary<AttributeNameKey, ChileProductAttributeRange> productSpec = null;
            IDictionary<AttributeNameKey, CustomerProductAttributeRange> customerSpec = null;
            SalesOrderItem orderItem = null;
            if(salesOrderItemKey != null)
            {
                var item = salesOrder.SalesOrderItems.FirstOrDefault(salesOrderItemKey.FindByPredicate.Compile());
                if(item == null)
                {
                    return new InvalidResult<IPickableInventoryReturn>(null, string.Format(UserMessages.SalesOrderItemNotFound, salesOrderItemKey));
                }
                orderItem = item;

                var specResult = new GetProductSpecCommand(_inventoryShipmentOrderUnitOfWork).Execute(ChileProductKey.FromProductKey(item.InventoryPickOrderItem),
                    item.Order, out productSpec, out customerSpec);
                if(!specResult.Success)
                {
                    return specResult.ConvertTo<IPickableInventoryReturn>();
                }
            }
            
            var itemsResult = new GetPickableInventoryCommand(_inventoryShipmentOrderUnitOfWork).Execute(filterResults.ResultingObject, _timeStamper.CurrentTimeStamp,
                PickedInventoryValidator.ForSalesOrder(salesOrder.InventoryShipmentOrder.SourceFacility), true);
            if(itemsResult.Success)
            {
                itemsResult.ResultingObject.Initializer = new ValidPickingForOrder(new PickingValidatorContext(productSpec, customerSpec, orderItem == null ? null : orderItem.ContractItem, salesOrder, salesOrder));
            }

            return itemsResult;
        }

        [SynchronizeOldContext(NewContextMethod.SyncSalesOrder)]
        public IResult SetPickedInventoryForSalesOrder(string salesOrderKey, ISetPickedInventoryParameters pickedInventory)
        {
            if(salesOrderKey == null) { throw new ArgumentNullException("salesOrderKey"); }
            if(pickedInventory == null) { throw new ArgumentNullException("pickedInventory"); }

            var orderKeyResult = KeyParserHelper.ParseResult<ISalesOrderKey>(salesOrderKey);
            if(!orderKeyResult.Success)
            {
                return orderKeyResult;
            }

            var pickInventoryResult = new SetSalesOrderPickedInventoryConductor(_inventoryShipmentOrderUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, orderKeyResult.ResultingObject, pickedInventory);
            if(!pickInventoryResult.Success)
            {
                return pickInventoryResult;
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            var key = pickInventoryResult.ResultingObject.ToSalesOrderKey();
            return SyncParameters.Using(new SuccessResult(), new SyncSalesOrderParameters
                {
                    SalesOrderKey = key,
                    New = false
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncCustomerSpecs)]
        public IResult SetCustomerChileProductAttributeRange(ISetCustomerProductAttributeRangesParameters parameters)
        {
            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters;
            }

            var result = new SetCustomerChileProductAttributeRangeCommand(_inventoryShipmentOrderUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parsedParameters.ResultingObject);
            if(!result.Success)
            {
                return result;
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new SynchronizeCustomerProductSpecs
                {
                    CustomerKey = parsedParameters.ResultingObject.CustomerKey,
                    ChileProductKey = parsedParameters.ResultingObject.ChileProductKey
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncCustomerSpecs)]
        public IResult RemoveCustomerChileProductAttributeRanges(IRemoveCustomerChileProductAttributeRangesParameters parameters)
        {
            var customerKey = KeyParserHelper.ParseResult<ICustomerKey>(parameters.CustomerKey);
            if(!customerKey.Success)
            {
                return customerKey;
            }

            var chileProductKey = KeyParserHelper.ParseResult<IChileProductKey>(parameters.ChileProductKey);
            if(!chileProductKey.Success)
            {
                return chileProductKey;
            }

            int? prodId;
            string companyIA;
            var result = new RemoveCustomerChileProductAttributeRangesCommand(_inventoryShipmentOrderUnitOfWork).Execute(customerKey.ResultingObject, chileProductKey.ResultingObject, out prodId, out companyIA);
            if(!result.Success)
            {
                return result;
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new SynchronizeCustomerProductSpecs
                {
                    Delete = prodId == null || string.IsNullOrWhiteSpace(companyIA) ? null :
                    new SerializedSpecKey
                        {
                            ProdID = prodId.Value,
                            Company_IA = companyIA
                        }
                });
        }

        public IResult<ICustomerChileProductAttributeRangesReturn> GetCustomerChileProductAttributeRanges(string customerKey, string chileProductKey)
        {
            var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(customerKey);
            if(!customerKeyResult.Success)
            {
                return customerKeyResult.ConvertTo<ICustomerChileProductAttributeRangesReturn>();
            }

            var chileProductKeyResult = KeyParserHelper.ParseResult<IChileProductKey>(chileProductKey);
            if(!chileProductKeyResult.Success)
            {
                return chileProductKeyResult.ConvertTo<ICustomerChileProductAttributeRangesReturn>();
            }

            var select = CustomerProjectors.SelectProductSpecs(true);
            var filter = customerKeyResult.ResultingObject.ToCustomerKey().FindByPredicate;
            var spec = _inventoryShipmentOrderUnitOfWork.CustomerRepository.Filter(filter).SelectMany(select).FirstOrDefault(s => s.ChileProduct.ProductKeyReturn.ProductKey_ProductId == chileProductKeyResult.ResultingObject.ChileProductKey_ProductId);
            if(spec == null)
            {
                return new InvalidResult<ICustomerChileProductAttributeRangesReturn>(null, string.Format(UserMessages.NoCustomerProductRangesFound, customerKey, chileProductKey));
            }

            return new SuccessResult<ICustomerChileProductAttributeRangesReturn>(spec);
        }

        public IResult<IQueryable<ICustomerChileProductAttributeRangesReturn>> GetCustomerChileProductsAttributeRanges(string customerKey)
        {
            var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(customerKey);
            if(!customerKeyResult.Success)
            {
                return customerKeyResult.ConvertTo<IQueryable<ICustomerChileProductAttributeRangesReturn>>();
            }

            var select = CustomerProjectors.SelectProductSpecs(true);
            var filter = customerKeyResult.ResultingObject.ToCustomerKey().FindByPredicate;
            var specs = _inventoryShipmentOrderUnitOfWork.CustomerRepository.Filter(filter).SelectMany(select);

            return new SuccessResult<IQueryable<ICustomerChileProductAttributeRangesReturn>>(specs);
        }

        public IResult<ICustomerProductCodeReturn> GetCustomerProductCode(string customerKey, string chileProductKey)
        {
            var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(customerKey);
            if(!customerKeyResult.Success)
            {
                return customerKeyResult.ConvertTo<ICustomerProductCodeReturn>();
            }

            var chileProductKeyResult = KeyParserHelper.ParseResult<IChileProductKey>(chileProductKey);
            if(!chileProductKeyResult.Success)
            {
                return chileProductKeyResult.ConvertTo<ICustomerProductCodeReturn>();
            }

            return new GetCustomerProductCodeCommand(_inventoryShipmentOrderUnitOfWork).Execute(new CustomerKey(customerKeyResult.ResultingObject), new ChileProductKey(chileProductKeyResult.ResultingObject));
        }

        public IResult SetCustomerProductCode(string customerKey, string chileProductKey, string code)
        {
            var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(customerKey);
            if(!customerKeyResult.Success)
            {
                return customerKeyResult.ConvertTo<ICustomerProductCodeReturn>();
            }

            var chileProductKeyResult = KeyParserHelper.ParseResult<IChileProductKey>(chileProductKey);
            if(!chileProductKeyResult.Success)
            {
                return chileProductKeyResult.ConvertTo<ICustomerProductCodeReturn>();
            }

            var result = new SetCustomerProductCodeCommand(_inventoryShipmentOrderUnitOfWork).Execute(new CustomerKey(customerKeyResult.ResultingObject), new ChileProductKey(chileProductKeyResult.ResultingObject), code);
            if(result.Success)
            {
                _inventoryShipmentOrderUnitOfWork.Commit();
            }

            return result;
        }

        [SynchronizeOldContext(NewContextMethod.SyncPostInvoice)]
        public IResult PostInvoice(string customerOrderKey)
        {
            var orderKeyResult = KeyParserHelper.ParseResult<ISalesOrderKey>(customerOrderKey);
            if(!orderKeyResult.Success)
            {
                return orderKeyResult;
            }

            var orderKey = orderKeyResult.ResultingObject.ToSalesOrderKey();
            var order = _inventoryShipmentOrderUnitOfWork.SalesOrderRepository.FindByKey(orderKey,
                o => o.InventoryShipmentOrder.ShipmentInformation);
            if(order == null)
            {
                return new InvalidResult(string.Format(UserMessages.SalesOrderNotFound, orderKey));
            }

            if(order.InventoryShipmentOrder.ShipmentInformation.Status != ShipmentStatus.Shipped)
            {
                return new InvalidResult(string.Format(UserMessages.SalesOrderNotShipped, orderKey));
            }

            order.InvoiceDate = order.InvoiceDate ?? DateTime.Now.ToLocalTime().Date;
            order.OrderStatus = SalesOrderStatus.Invoiced;
            
            _inventoryShipmentOrderUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), orderKey);
        }

        public IResult<IQueryable<ISalesQuoteSummaryReturn>> GetSalesQuotes(FilterSalesQuotesParameters parameters)
        {
            var parsedFiltersResult = parameters.ParseToPredicateBuilderFilters();
            if(!parsedFiltersResult.Success)
            {
                return parsedFiltersResult.ConvertTo<IQueryable<ISalesQuoteSummaryReturn>>();
            }

            var predicateResult = SalesQuotePredicateBuilder.BuildPredicate(parsedFiltersResult.ResultingObject);
            if(!predicateResult.Success)
            {
                return predicateResult.ConvertTo<IQueryable<ISalesQuoteSummaryReturn>>();
            }

            var query = _inventoryShipmentOrderUnitOfWork.SalesQuoteRepository.SourceQuery
                .Where(predicateResult.ResultingObject)
                .Select(SalesQuoteProjectors.SelectSummary());

            return new SuccessResult<IQueryable<ISalesQuoteSummaryReturn>>(query);
        }

        public IResult<ISalesQuoteDetailReturn> GetSalesQuote(string salesQuoteKey)
        {
            var parsedKeyResult = KeyParserHelper.ParseResult<ISalesQuoteKey>(salesQuoteKey);
            if(!parsedKeyResult.Success)
            {
                return parsedKeyResult.ConvertTo<ISalesQuoteDetailReturn>();
            }
            var key = parsedKeyResult.ResultingObject.ToSalesQuoteKey();

            var result = _inventoryShipmentOrderUnitOfWork.SalesQuoteRepository
                .Filter(key.FindByPredicate)
                .Select(SalesQuoteProjectors.SelectDetail())
                .FirstOrDefault();
            if(result == null)
            {
                return new InvalidResult<ISalesQuoteDetailReturn>(null, string.Format(UserMessages.SalesQuoteNotFound_Key, key));
            }

            return new SuccessResult<ISalesQuoteDetailReturn>(result);
        }

        public IResult<ISalesQuoteDetailReturn> GetSalesQuote(int salesQuoteNumber)
        {
            var result = _inventoryShipmentOrderUnitOfWork.SalesQuoteRepository
                .Filter(q => q.QuoteNum == salesQuoteNumber)
                .Select(SalesQuoteProjectors.SelectDetail())
                .FirstOrDefault();
            if(result == null)
            {
                return new InvalidResult<ISalesQuoteDetailReturn>(null, string.Format(UserMessages.SalesQuoteNotFound_Num, salesQuoteNumber));
            }

            return new SuccessResult<ISalesQuoteDetailReturn>(result);
        }

        [SynchronizeOldContext(NewContextMethod.SalesQuote)]
        public IResult<string> SetSalesQuote(ISalesQuoteParameters parameters, bool updateExisting)
        {
            var parsedParameters = parameters.ToParsedParameters(updateExisting);
            if(!parsedParameters.Success)
            {
                return parsedParameters.ConvertTo<string>();
            }

            var result = new SetSalesQuoteConductor(_inventoryShipmentOrderUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parsedParameters.ResultingObject);
            if(!result.Success)
            {
                return result.ConvertTo<string>();
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            var salesQuoteKey = result.ResultingObject.ToSalesQuoteKey();
            return SyncParameters.Using(new SuccessResult<string>(salesQuoteKey), new SyncSalesQuoteParameters
                {
                    SalesQuoteKey = salesQuoteKey,
                    New = !updateExisting
                });
        }

        public IResult<ISalesQuoteReportReturn> GetSalesQuoteReport(int salesQuoteNumber)
        {
            var result = _inventoryShipmentOrderUnitOfWork.SalesQuoteRepository
                .Filter(q => q.QuoteNum == salesQuoteNumber)
                .Select(SalesQuoteProjectors.SelectSalesQuoteReport())
                .FirstOrDefault();
            return new SuccessResult<ISalesQuoteReportReturn>(result);
        }
    }
}