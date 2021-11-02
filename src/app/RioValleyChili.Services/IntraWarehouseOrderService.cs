using System;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.IntraWarehouseOrderService;
using RioValleyChili.Services.Interfaces.Returns.IntraWarehouseOrderService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class IntraWarehouseOrderService : IIntraWarehouseOrderService
    {
        #region Fields and Constructors.

        private readonly IntraWarehouseOrderServiceProvider _intraWarehouseOrderServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public IntraWarehouseOrderService(IntraWarehouseOrderServiceProvider intraWarehouseOrderServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(intraWarehouseOrderServiceProvider == null) { throw new ArgumentNullException("intraWarehouseOrderServiceProvider"); }
            _intraWarehouseOrderServiceProvider = intraWarehouseOrderServiceProvider;

            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }
            _exceptionLogger = exceptionLogger;
        }

        #endregion

        #region Implementation of IIntraWarehouseOrderService.

        public IResult<string> CreateIntraWarehouseOrder(ICreateIntraWarehouseOrderParameters parameters)
        {
            try
            {
                return _intraWarehouseOrderServiceProvider.CreateIntraWarehouseOrder(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult UpdateIntraWarehouseOrder(IUpdateIntraWarehouseOrderParameters parameters)
        {
            try
            {
                return _intraWarehouseOrderServiceProvider.UpdateIntraWarehouseOrder(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<IQueryable<IIntraWarehouseOrderSummaryReturn>> GetIntraWarehouseOrderSummaries()
        {
            try
            {
                return _intraWarehouseOrderServiceProvider.GetIntraWarehouseOrderSummaries();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IIntraWarehouseOrderSummaryReturn>>(null, ex.Message);
            }
        }

        public IResult<IQueryable<IIntraWarehouseOrderDetailReturn>> GetIntraWarehouseOrders()
        {
            try
            {
                return _intraWarehouseOrderServiceProvider.GetIntraWarehouseOrders();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IIntraWarehouseOrderDetailReturn>>(null, ex.Message);
            }
        }

        #endregion
    }
}