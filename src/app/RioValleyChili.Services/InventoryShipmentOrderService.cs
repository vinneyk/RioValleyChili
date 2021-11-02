using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.InventoryShipmentOrderService;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class InventoryShipmentOrderService : IInventoryShipmentOrderService
    {
        #region Fields and Constructors.

        private readonly InventoryShipmentOrderServiceProvider _inventoryShipmentOrderServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public InventoryShipmentOrderService(InventoryShipmentOrderServiceProvider inventoryShipmentOrderServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(inventoryShipmentOrderServiceProvider == null) { throw new ArgumentNullException("inventoryShipmentOrderServiceProvider"); }
            _inventoryShipmentOrderServiceProvider = inventoryShipmentOrderServiceProvider;
            
            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }
            _exceptionLogger = exceptionLogger;
        }

        #endregion

        #region Implementation of IInventoryShipmentOrderService.

        public IResult<IQueryable<IShipmentOrderSummaryReturn>> GetShipments()
        {
            try
            {
                return _inventoryShipmentOrderServiceProvider.GetShipments();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IShipmentOrderSummaryReturn>>(null, ex.Message);
            }
        }

        public IResult SetShipmentInformation(ISetInventoryShipmentInformationParameters parameters)
        {
            try
            {
                return _inventoryShipmentOrderServiceProvider.SetShipmentInformation(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult Post(IPostParameters parameters)
        {
            try
            {
                return _inventoryShipmentOrderServiceProvider.Post(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<IInternalOrderAcknowledgementReturn> GetInhouseShipmentOrderAcknowledgement(string orderKey)
        {
            try
            {
                return _inventoryShipmentOrderServiceProvider.GetInventoryShipmentOrderAcknowledgement(orderKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IInternalOrderAcknowledgementReturn>(null, ex.Message);
            }
        }

        public IResult<ISalesOrderAcknowledgementReturn> GetCustomerOrderAcknowledgement(string orderKey)
        {
            try
            {
                return _inventoryShipmentOrderServiceProvider.GetCustomerOrderAcknowledgement(orderKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ISalesOrderAcknowledgementReturn>(null, ex.Message);
            }
        }

        public IResult<IInventoryShipmentOrderPackingListReturn> GetInventoryShipmentOrderPackingList(string orderKey)
        {
            try
            {
                return _inventoryShipmentOrderServiceProvider.GetInventoryShipmentOrderPackingList(orderKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IInventoryShipmentOrderPackingListReturn>(null, ex.Message);
            }
        }

        public IResult<IInventoryShipmentOrderBillOfLadingReturn> GetInventoryShipmentOrderBillOfLading(string orderKey)
        {
            try
            {
                return _inventoryShipmentOrderServiceProvider.GetInventoryShipmentOrderBillOfLading(orderKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IInventoryShipmentOrderBillOfLadingReturn>(null, ex.Message);
            }
        }

        public IResult<IInventoryShipmentOrderPickSheetReturn> GetInventoryShipmentOrderPickSheet(string orderKey)
        {
            try
            {
                return _inventoryShipmentOrderServiceProvider.GetInventoryShipmentOrderPickSheet(orderKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IInventoryShipmentOrderPickSheetReturn>(null, ex.Message);
            }
        }

        public IResult<IInventoryShipmentOrderCertificateOfAnalysisReturn> GetInventoryShipmentOrderCertificateOfAnalysis(string orderKey)
        {
            try
            {
                return _inventoryShipmentOrderServiceProvider.GetInventoryShipmentOrderCertificateOfAnalysis(orderKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IInventoryShipmentOrderCertificateOfAnalysisReturn>(null, ex.Message);
            }
        }

        public IResult<IPendingOrderDetails> GetPendingOrderDetails(DateTime startDate, DateTime endDate)
        {
            try
            {
                return _inventoryShipmentOrderServiceProvider.GetPendingOrderDetails(startDate, endDate);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IPendingOrderDetails>(null, ex.Message);
            }
        }

        public IResult<ISalesOrderInvoice> GetCustomerOrderInvoice(string orderKey)
        {
            try
            {
                return _inventoryShipmentOrderServiceProvider.GetCustomerOrderInvoice(orderKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ISalesOrderInvoice>(null, ex.Message);
            }
        }

        public IResult VoidOrder(string orderKey)
        {
            try
            {
                return _inventoryShipmentOrderServiceProvider.VoidOrder(orderKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<IEnumerable<string>> GetShipmentMethods()
        {
            try
            {
                return _inventoryShipmentOrderServiceProvider.GetShipmentMethods();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<string>>(null, ex.Message);
            }
        }

        #endregion
    }
}