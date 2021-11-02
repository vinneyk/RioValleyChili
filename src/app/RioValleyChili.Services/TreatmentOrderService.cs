using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.TreatmentOrderService;
using RioValleyChili.Services.Interfaces.ServiceCompositions;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class TreatmentOrderService : ITreatmentOrderService
    {
        #region Fields and Constructors.

        private readonly TreatmentOrderServiceProvider _treatmentOrderServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public TreatmentOrderService(TreatmentOrderServiceProvider treatmentOrderServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(treatmentOrderServiceProvider == null) { throw new ArgumentNullException("treatmentOrderServiceProvider"); }
            _treatmentOrderServiceProvider = treatmentOrderServiceProvider;

            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }
            _exceptionLogger = exceptionLogger;
        }

        #endregion

        IResult IPickInventoryServiceComponent.SetPickedInventory(string contextKey, ISetPickedInventoryParameters parameters)
        {
            try
            {
                return _treatmentOrderServiceProvider.SetPickedInventory(contextKey, parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        IResult<IPickableInventoryReturn> IPickInventoryServiceComponent.GetPickableInventoryForContext(FilterInventoryForPickingContextParameters parameters)
        {
            try
            {
                return _treatmentOrderServiceProvider.GetInventoryItemsToPickTreatmentOrder((FilterInventoryForShipmentOrderParameters)parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IPickableInventoryReturn>(null, ex.Message);
            }
        }

        public IResult<string> CreateInventoryTreatmentOrder(ICreateTreatmentOrderParameters parameters)
        {
            try
            {
                return _treatmentOrderServiceProvider.CreateInventoryTreatmentOrder(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult UpdateTreatmentOrder(IUpdateTreatmentOrderParameters parameters)
        {
            try
            {
                return _treatmentOrderServiceProvider.UpdateInventoryTreatmentOrder(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult DeleteTreatmentOrder(string orderKey)
        {
            try
            {
                return _treatmentOrderServiceProvider.DeleteTreatmentOrder(orderKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }
        
        public IResult ReceiveOrder(IReceiveTreatmentOrderParameters parameters)
        {
            try
            {
                return _treatmentOrderServiceProvider.ReceiveOrder(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<IEnumerable<IInventoryTreatmentReturn>> GetInventoryTreatments()
        {
            try
            {
                return _treatmentOrderServiceProvider.GetInventoryTreatments();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEnumerable<IInventoryTreatmentReturn>>(null, ex.Message);
            }
        }

        public IResult<ITreatmentOrderDetailReturn> GetTreatmentOrder(string orderKey)
        {
            try
            {
                return _treatmentOrderServiceProvider.GetTreatmentOrder(orderKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ITreatmentOrderDetailReturn>(null, ex.Message);
            }
        }

        public IResult<IQueryable<ITreatmentOrderSummaryReturn>> GetTreatmentOrders()
        {
            try
            {
                return _treatmentOrderServiceProvider.GetTreatmentOrders();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<ITreatmentOrderSummaryReturn>>(null, ex.Message);
            }
        }
    }
}