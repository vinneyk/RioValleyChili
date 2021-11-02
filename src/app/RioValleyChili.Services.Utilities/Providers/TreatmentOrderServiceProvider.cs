using System;
using System.Collections.Generic;
using System.Linq;
using EF_Split_Projector;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.TreatmentOrderService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Conductors;
using RioValleyChili.Services.Utilities.Conductors.Parameters;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Core;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class TreatmentOrderServiceProvider : IUnitOfWorkContainer<IInventoryShipmentOrderUnitOfWork>
    {
        #region Fields and Constructors.

        IInventoryShipmentOrderUnitOfWork IUnitOfWorkContainer<IInventoryShipmentOrderUnitOfWork>.UnitOfWork { get { return _inventoryShipmentOrderUnitOfWork; } }
        private readonly IInventoryShipmentOrderUnitOfWork _inventoryShipmentOrderUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public TreatmentOrderServiceProvider(IInventoryShipmentOrderUnitOfWork inventoryShipmentOrderUnitOfWork, ITimeStamper timeStamper)
        {
            if(inventoryShipmentOrderUnitOfWork == null) { throw new ArgumentNullException("inventoryShipmentOrderUnitOfWork"); }
            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }

            _inventoryShipmentOrderUnitOfWork = inventoryShipmentOrderUnitOfWork;
            _timeStamper = timeStamper;
        }

        #endregion

        public IResult<IPickableInventoryReturn> GetInventoryItemsToPickTreatmentOrder(FilterInventoryForShipmentOrderParameters parameters)
        {
            parameters = parameters ?? new FilterInventoryForShipmentOrderParameters();

            InventoryShipmentOrderKey orderKey;
            InventoryPickOrderItemKey orderItemKey;
            var filterResults = parameters.ParseToPredicateBuilderFilters(out orderKey, out orderItemKey);
            if(!filterResults.Success)
            {
                return filterResults.ConvertTo<IPickableInventoryReturn>();
            }

            var treatmentOrderKey = new TreatmentOrderKey(orderKey);
            var treatmentOrder = _inventoryShipmentOrderUnitOfWork.TreatmentOrderRepository.FindByKey(treatmentOrderKey, o => o.InventoryShipmentOrder.SourceFacility);
            if(treatmentOrder == null)
            {
                return new InvalidResult<IPickableInventoryReturn>(null, string.Format(UserMessages.TreatmentOrderNotFound, treatmentOrderKey));
            }
            var facilityKey = treatmentOrder.InventoryShipmentOrder.SourceFacility.ToFacilityKey();

            var itemsResult = new GetPickableInventoryCommand(_inventoryShipmentOrderUnitOfWork).Execute(filterResults.ResultingObject, _timeStamper.CurrentTimeStamp,
                PickedInventoryValidator.ForTreatmentOrder(facilityKey, treatmentOrder, _inventoryShipmentOrderUnitOfWork), false);

            return itemsResult;
        }

        [SynchronizeOldContext(NewContextMethod.SyncInventoryShipmentOrder)]
        public IResult<string> CreateInventoryTreatmentOrder(ICreateTreatmentOrderParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedParameters = new CreateTreatmentOrderConductorParameters<ICreateTreatmentOrderParameters>(parameters);
            if(!parsedParameters.Result.Success)
            {
                return parsedParameters.Result.ConvertTo<string>();
            }

            var createResult = new CreateTreatmentOrderConductor(_inventoryShipmentOrderUnitOfWork).CreateTreatmentOrder(_timeStamper.CurrentTimeStamp, parsedParameters);
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

        [SynchronizeOldContext(NewContextMethod.SyncInventoryShipmentOrder)]
        public IResult UpdateInventoryTreatmentOrder(IUpdateTreatmentOrderParameters parameters)
        {
            var parsedParameters = new UpdateTreatmentOrderConductorParameters<IUpdateTreatmentOrderParameters>(parameters);
            if(!parsedParameters.Result.Success)
            {
                return parsedParameters.Result.ConvertTo<InventoryShipmentOrderKey>();
            }

            var updateResult = new UpdateTreatmentOrderConductor(_inventoryShipmentOrderUnitOfWork).Update(_timeStamper.CurrentTimeStamp, parsedParameters);
            if(!updateResult.Success)
            {
                return updateResult.ConvertTo<InventoryShipmentOrderKey>();
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            var key = updateResult.ResultingObject.ToInventoryShipmentOrderKey();
            return SyncParameters.Using(new SuccessResult<string>(key), new SyncInventoryShipmentOrderParameters
                {
                    InventoryShipmentOrderKey = key,
                    New = false
                });
        }

        [SynchronizeOldContext(NewContextMethod.DeleteTreatmentOrder)]
        public IResult DeleteTreatmentOrder(string orderKey)
        {
            var keyResult = KeyParserHelper.ParseResult<ITreatmentOrderKey>(orderKey);
            if(!keyResult.Success)
            {
                return keyResult.ConvertTo<string>();
            }

            int? moveNum;
            var result = new DeleteTreatmentOrderConductor(_inventoryShipmentOrderUnitOfWork).Execute(new TreatmentOrderKey(keyResult.ResultingObject), out moveNum);
            if(!result.Success)
            {
                return result.ConvertTo<string>();
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), moveNum);
        }

        [SynchronizeOldContext(NewContextMethod.ReceiveTreatmentOrder)]
        public IResult ReceiveOrder(IReceiveTreatmentOrderParameters parameters)
        {
            var parametersResult = parameters.ToParsedParameters();
            if(!parametersResult.Success)
            {
                return parametersResult;
            }

            var receiveResult = new ReceiveTreatmentOrderConductor(_inventoryShipmentOrderUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parametersResult.ResultingObject);
            if(!receiveResult.Success)
            {
                return receiveResult;
            }

            _inventoryShipmentOrderUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), parametersResult.ResultingObject.TreatmentOrderKey);
        }

        public IResult<IEnumerable<IInventoryTreatmentReturn>> GetInventoryTreatments()
        {
            var select = InventoryTreatmentProjectors.SelectInventoryTreatment();
            var query = _inventoryShipmentOrderUnitOfWork.InventoryTreatmentRepository.All().AsExpandable().Select(select).ToList();
            return new SuccessResult<IEnumerable<IInventoryTreatmentReturn>>(query);
        }

        public IResult<ITreatmentOrderDetailReturn> GetTreatmentOrder(string treatmentOrderKey)
        {
            if(treatmentOrderKey == null) { throw new ArgumentNullException("treatmentOrderKey"); }

            var keyResult = KeyParserHelper.ParseResult<ITreatmentOrderKey>(treatmentOrderKey);
            if(!keyResult.Success)
            {
                return keyResult.ConvertTo<ITreatmentOrderDetailReturn>();
            }

            var context = ((EFUnitOfWorkBase)_inventoryShipmentOrderUnitOfWork).Context;
            context.Configuration.AutoDetectChangesEnabled = false;

            var predicate = new TreatmentOrderKey(keyResult.ResultingObject).FindByPredicate;
            var select = TreatmentOrderProjectors.SplitSelectDetail(_inventoryShipmentOrderUnitOfWork, _timeStamper.CurrentTimeStamp.Date);
            var treatmentOrder = _inventoryShipmentOrderUnitOfWork.TreatmentOrderRepository.Filter(predicate).SplitSelect(select).FirstOrDefault();

            if(treatmentOrder == null)
            {
                return new FailureResult<ITreatmentOrderDetailReturn>(null, string.Format(UserMessages.TreatmentOrderNotFound, treatmentOrderKey));
            }

            return new SuccessResult<ITreatmentOrderDetailReturn>(treatmentOrder);
        }

        public IResult<IQueryable<ITreatmentOrderSummaryReturn>> GetTreatmentOrders()
        {
            var selector = TreatmentOrderProjectors.SelectSummary();
            var query = _inventoryShipmentOrderUnitOfWork.TreatmentOrderRepository.All().AsExpandable().Select(selector);
            return new SuccessResult<IQueryable<ITreatmentOrderSummaryReturn>>(query);
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

            var setResult = new TreatmentOrderPickedInventoryConductor(_inventoryShipmentOrderUnitOfWork).SetPickedInventory(_timeStamper.CurrentTimeStamp, new InventoryShipmentOrderPickInventoryConductor.Parameters
                {
                    User = parameters,
                    OrderKey = orderKey,
                    PickedInventoryParameters = parsedParameters.ResultingObject
                });
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