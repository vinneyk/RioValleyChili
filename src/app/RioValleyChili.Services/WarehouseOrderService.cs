using System;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseOrderService;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.WarehouseOrderService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class WarehouseOrderService : IWarehouseOrderService
    {
        #region Fields and Constructors.

        private readonly WarehouseOrderServiceProvider _warehouseOrderServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public WarehouseOrderService(WarehouseOrderServiceProvider warehouseOrderServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(warehouseOrderServiceProvider == null) { throw new ArgumentNullException("warehouseOrderServiceProvider"); }
            _warehouseOrderServiceProvider = warehouseOrderServiceProvider;

            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }
            _exceptionLogger = exceptionLogger;
        }
        
        #endregion

        #region Implementation of IWarehouseOrderService.

        public IResult SetPickedInventory(string contextKey, ISetPickedInventoryParameters parameters)
        {
            try
            {
                return _warehouseOrderServiceProvider.SetPickedInventory(contextKey, parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<IPickableInventoryReturn> GetPickableInventoryForContext(FilterInventoryForPickingContextParameters parameters)
        {
            try
            {
                return _warehouseOrderServiceProvider.GetInventoryItemsToPickWarehouseOrder((FilterInventoryForShipmentOrderParameters)parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IPickableInventoryReturn>(null, ex.Message);
            }
        }

        public IResult<string> CreateWarehouseOrder(ISetOrderParameters parameters)
        {
            try
            {
                return _warehouseOrderServiceProvider.CreateWarehouseOrder(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult UpdateInterWarehouseOrder(IUpdateInterWarehouseOrderParameters parameters)
        {
            try
            {
                return _warehouseOrderServiceProvider.UpdateInterWarehouseOrder(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<IInventoryShipmentOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>> GetWarehouseOrder(string orderKey)
        {
            try
            {
                return _warehouseOrderServiceProvider.GetInterWarehouseOrder(orderKey);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IInventoryShipmentOrderDetailReturn<IPickOrderDetailReturn<IPickOrderItemReturn>, IPickOrderItemReturn>>(null, ex.Message);
            }
        }

        public IResult<IQueryable<IInventoryShipmentOrderSummaryReturn>> GetWarehouseOrders(FilterInterWarehouseOrderParameters parameters = null)
        {
            try
            {
                return _warehouseOrderServiceProvider.GetInterWarehouseOrders(parameters);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IInventoryShipmentOrderSummaryReturn>>(null, ex.Message);
            }
        }

        #endregion
    }
}