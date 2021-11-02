using System;
using System.Collections.Generic;
using System.Linq;
using EF_Split_Projector;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseOrderService;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.WarehouseOrderService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Commands.Customer;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Conductors;
using RioValleyChili.Services.Utilities.Conductors.Parameters;
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
    public class WarehouseOrderServiceProvider : IUnitOfWorkContainer<IInventoryShipmentOrderUnitOfWork>
    {
        #region Fields and Constructors.

        IInventoryShipmentOrderUnitOfWork IUnitOfWorkContainer<IInventoryShipmentOrderUnitOfWork>.UnitOfWork { get { return _inventoryShipmentOrderUnitOfWork; } }
        private readonly IInventoryShipmentOrderUnitOfWork _inventoryShipmentOrderUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public WarehouseOrderServiceProvider(IInventoryShipmentOrderUnitOfWork inventoryShipmentOrderUnitOfWork, ITimeStamper timeStamper)
        {
            if(inventoryShipmentOrderUnitOfWork == null) { throw new ArgumentNullException("inventoryShipmentOrderUnitOfWork"); }
            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }

            _inventoryShipmentOrderUnitOfWork = inventoryShipmentOrderUnitOfWork;
            _timeStamper = timeStamper;
        }

        #endregion

        public IResult<IPickableInventoryReturn> GetInventoryItemsToPickWarehouseOrder(FilterInventoryForShipmentOrderParameters parameters)
        {
            parameters = parameters ?? new FilterInventoryForShipmentOrderParameters();

            InventoryShipmentOrderKey orderKey;
            InventoryPickOrderItemKey orderItemKey;
            var filterResults = parameters.ParseToPredicateBuilderFilters(out orderKey, out orderItemKey);
            if(!filterResults.Success)
            {
                return filterResults.ConvertTo<IPickableInventoryReturn>();
            }

            var order = _inventoryShipmentOrderUnitOfWork.InventoryShipmentOrderRepository.FindByKey(orderKey,
                o => o.SourceFacility,
                o => o.InventoryPickOrder.Items.Select(i => i.Customer));
            if(order == null)
            {
                return new InvalidResult<IPickableInventoryReturn>(null, string.Format(UserMessages.InterWarehouseOrderNotFound, orderKey));
            }
            var facilityKey = order.SourceFacility.ToFacilityKey();

            IDictionary<AttributeNameKey, ChileProductAttributeRange> productSpec = null;
            IDictionary<AttributeNameKey, CustomerProductAttributeRange> customerSpec = null;
            Customer customer = null;
            if(orderItemKey != null)
            {
                var item = order.InventoryPickOrder.Items.FirstOrDefault(orderItemKey.FindByPredicate.Compile());
                if(item == null)
                {
                    return new InvalidResult<IPickableInventoryReturn>(null, string.Format(UserMessages.InventoryPickOrderItemNotFound, orderItemKey));
                }
                customer = item.Customer;
                
                var specResult = new GetProductSpecCommand(_inventoryShipmentOrderUnitOfWork).Execute(ChileProductKey.FromProductKey(item),
                    customer, out productSpec, out customerSpec);
                if(!specResult.Success)
                {
                    return specResult.ConvertTo<IPickableInventoryReturn>();
                }
            }
            
            var itemsResult = new GetPickableInventoryCommand(_inventoryShipmentOrderUnitOfWork).Execute(filterResults.ResultingObject, _timeStamper.CurrentTimeStamp,
                PickedInventoryValidator.ForInterWarehouseOrder(facilityKey), true);
            return itemsResult;
        }

        [SynchronizeOldContext(NewContextMethod.SyncInventoryShipmentOrder)]
        public IResult<string> CreateWarehouseOrder(ISetOrderParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedParameters = new SetInventoryShipmentOrderConductorParameters<ISetOrderParameters>(parameters);
            if(!parsedParameters.Result.Success)
            {
                return parsedParameters.Result.ConvertTo<string>();
            }

            var createResult = new CreateInterWarehouseOrderConductor(_inventoryShipmentOrderUnitOfWork).Create(_timeStamper.CurrentTimeStamp, parsedParameters);
            if(!createResult.Success)
            {
                return createResult.ConvertTo<string>();
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            var key = createResult.ResultingObject.ToInventoryShipmentOrderKey();
            return SyncParameters.Using(new SuccessResult<string>(key), new SyncInventoryShipmentOrderParameters
                {
                    InventoryShipmentOrderKey = key,
                    New = true
                });
        }

        public IResult<IQueryable<IInventoryShipmentOrderSummaryReturn>> GetInterWarehouseOrders(FilterInterWarehouseOrderParameters parameters)
        {
            var predicate = parameters.ParseToPredicate();
            if(!predicate.Success)
            {
                return predicate.ConvertTo<IQueryable<IInventoryShipmentOrderSummaryReturn>>();
            }
            var select = InventoryShipmentOrderProjectors.SelectInventoryShipmentOrderSummary();
            var query = _inventoryShipmentOrderUnitOfWork.InventoryShipmentOrderRepository.Filter(predicate.ResultingObject).AsExpandable().Select(select);
            return new SuccessResult<IQueryable<IInventoryShipmentOrderSummaryReturn>>(query);
        }

        public IResult<IInventoryShipmentOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>> GetInterWarehouseOrder(string warehouseOrderKey)
        {
            if(warehouseOrderKey == null) { throw new ArgumentNullException("warehouseOrderKey"); }

            var orderKeyResult = KeyParserHelper.ParseResult<IInventoryShipmentOrderKey>(warehouseOrderKey);
            if(!orderKeyResult.Success)
            {
                return orderKeyResult.ConvertTo<IInventoryShipmentOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>>();
            }

            var predicate = InventoryShipmentOrderPredicates.ByOrderType(InventoryShipmentOrderTypeEnum.InterWarehouseOrder);
            predicate = predicate.And(new InventoryShipmentOrderKey(orderKeyResult.ResultingObject).FindByPredicate).ExpandAll();
            var select = InventoryShipmentOrderProjectors.SplitSelectInventoryShipmentOrderDetail(_inventoryShipmentOrderUnitOfWork, _timeStamper.CurrentTimeStamp.Date, InventoryOrderEnum.TransWarehouseMovements);

            var order = _inventoryShipmentOrderUnitOfWork.InventoryShipmentOrderRepository.Filter(predicate).SplitSelect(select).FirstOrDefault();
            if(order == null)
            {
                return new InvalidResult<IInventoryShipmentOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>>(null, string.Format(UserMessages.InterWarehouseOrderNotFound, warehouseOrderKey));
            }

            return new SuccessResult<IInventoryShipmentOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>>(order);
        }

        [SynchronizeOldContext(NewContextMethod.SyncInventoryShipmentOrder)]
        public IResult UpdateInterWarehouseOrder(IUpdateInterWarehouseOrderParameters parameters)
        {
            var parsedParameters = new UpdateInterWarehouseOrderConductorParameters<IUpdateInterWarehouseOrderParameters>(parameters);
            if(!parsedParameters.Result.Success)
            {
                return parsedParameters.Result;
            }

            var updateResult = new UpdateInterWarehouseOrderConductor(_inventoryShipmentOrderUnitOfWork).Update(_timeStamper.CurrentTimeStamp, parsedParameters);
            if(!updateResult.Success)
            {
                return updateResult;
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            var key = updateResult.ResultingObject.ToInventoryShipmentOrderKey();
            return SyncParameters.Using(new SuccessResult(), new SyncInventoryShipmentOrderParameters
                {
                    InventoryShipmentOrderKey = key,
                    New = false
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncInventoryShipmentOrder)]
        public IResult SetPickedInventory(string contextKey, ISetPickedInventoryParameters parameters)
        {
            var orderKeyResult = KeyParserHelper.ParseResult<IInventoryShipmentOrderKey>(contextKey);
            if(!orderKeyResult.Success)
            {
                return orderKeyResult;
            }
            var orderKey = orderKeyResult.ResultingObject.ToInventoryShipmentOrderKey();

            var parsedParameters = parameters.PickedInventoryItems.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters;
            }

            var setResult = new SetInterWarehouseOrderPickedInventoryConductor(_inventoryShipmentOrderUnitOfWork)
                .UpdatePickedInventory(orderKey, parameters, _timeStamper.CurrentTimeStamp, parsedParameters.ResultingObject);
            if(!setResult.Success)
            {
                return setResult;
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new SyncInventoryShipmentOrderParameters
                {
                    InventoryShipmentOrderKey = orderKey,
                    New = false
                });
        }
    }
}