using System;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.InventoryAdjustmentsService;
using RioValleyChili.Services.Interfaces.Returns.InventoryAdjustmentsService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class InventoryAdjustmentsService : IInventoryAdjustmentsService
    {
        #region fields and constructors

        private readonly InventoryAdjustmentsProvider _inventoryAdjustmentsProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public InventoryAdjustmentsService(InventoryAdjustmentsProvider inventoryAdjustmentsProvider, IExceptionLogger exceptionLogger)
        {
            if (inventoryAdjustmentsProvider == null) { throw new ArgumentNullException("inventoryAdjustmentsProvider"); }
            _inventoryAdjustmentsProvider = inventoryAdjustmentsProvider;

            if (exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }
            _exceptionLogger = exceptionLogger;
        }

        #endregion

        #region Implementation of IInventoryManagementService

        public IResult<string> CreateInventoryAdjustment(ICreateInventoryAdjustmentParameters parameters)
        {
            try
            {
                return _inventoryAdjustmentsProvider.CreateInventoryAdjustment(parameters);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult<IQueryable<IInventoryAdjustmentReturn>> GetInventoryAdjustments(FilterInventoryAdjustmentParameters parameters = null)
        {
            try
            {
                return _inventoryAdjustmentsProvider.GetInventoryAdjustments(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IInventoryAdjustmentReturn>>(null, ex.Message);
            }
        }

        public IResult<IInventoryAdjustmentReturn> GetInventoryAdjustment(string inventoryAdjustmentKey)
        {
            try
            {
                return _inventoryAdjustmentsProvider.GetInventoryAdjustment(inventoryAdjustmentKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IInventoryAdjustmentReturn>(null, ex.Message);
            }
        }

        #endregion
    }
}
