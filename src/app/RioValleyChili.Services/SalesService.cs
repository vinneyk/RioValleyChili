using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class SalesService : ISalesService
    {
        private readonly SalesServiceProvider _salesServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public SalesService(SalesServiceProvider salesServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(salesServiceProvider == null) { throw new ArgumentNullException("salesServiceProvider"); }
            _salesServiceProvider = salesServiceProvider;
             
            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }
            _exceptionLogger = exceptionLogger;
        }

        #region Customers and Brokers

        public IResult<IEnumerable<ICompanySummaryReturn>> GetCustomersForBroker(string brokerKey)
        {
            try
            {
                return _salesServiceProvider.GetCustomersForBroker(brokerKey);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEnumerable<ICompanySummaryReturn>>(null, ex.Message);
            }
        }

        public IResult AssignCustomerToBroker(string brokerKey, string customerKey)
        {
            try
            {
                return _salesServiceProvider.AssignCustomerToBroker(brokerKey, customerKey);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult RemoveCustomerFromBroker(string brokerKey, string customerKey)
        {
            try
            {
                return _salesServiceProvider.RemoveCustomerFromBroker(brokerKey, customerKey);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        #endregion

        public IResult<string> CreateCustomerContract(ICreateCustomerContractParameters parameters)
        {
            try
            {
                return _salesServiceProvider.CreateCustomerContract(parameters);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult UpdateCustomerContract(IUpdateCustomerContractParameters parameters)
        {
            try
            {
                return _salesServiceProvider.UpdateCustomerContract(parameters);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult SetCustomerContractsStatus(ISetContractsStatusParameters parameters)
        {
            try
            {
                return _salesServiceProvider.SetCustomerContractsStatus(parameters);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult<ICustomerContractDetailReturn> GetCustomerContract(string customerContractKey)
        {
            try
            {
                return _salesServiceProvider.GetCustomerContract(customerContractKey);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<ICustomerContractDetailReturn>(null, ex.Message);
            }
        }

        public IResult RemoveCustomerContract(string customerContractKey)
        {
            try
            {
                return _salesServiceProvider.RemoveCustomerContract(customerContractKey);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<IQueryable<ICustomerContractSummaryReturn>> GetCustomerContracts(FilterCustomerContractsParameters parameters = null)
        {
            try
            {
                return _salesServiceProvider.GetCustomerContracts(parameters);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<ICustomerContractSummaryReturn>>(null, ex.Message);
            }
        }

        public IResult<IEnumerable<string>> GetDistinctContractPaymentTerms()
        {
            try
            {
                return _salesServiceProvider.GetDistinctContractPaymentTerms();
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEnumerable<string>>(null, ex.Message);
            }
        }

        public IResult<ICustomerContractOrdersReturn> GetOrdersForCustomerContract(string customerContractKey)
        {
            try
            {
                return _salesServiceProvider.GetOrdersForCustomerContract(customerContractKey);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<ICustomerContractOrdersReturn>(null, ex.Message);
            }
        }

        public IResult<IContractShipmentSummaryReturn> GetContractShipmentSummary(string contractKey)
        {
            try
            {
                return _salesServiceProvider.GetContractShipmentSummary(contractKey);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<IContractShipmentSummaryReturn>(null, ex.Message);
            }
        }

        public IResult CompleteExpiredContracts()
        {
            try
            {
                return _salesServiceProvider.CompleteExpiredContracts();
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult<string> CreateSalesOrder(ICreateSalesOrderParameters parameters)
        {
            try
            {
                return _salesServiceProvider.CreateSalesOrder(parameters);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult UpdateSalesOrder(IUpdateSalesOrderParameters parameters)
        {
            try
            {
                return _salesServiceProvider.UpdateSalesOrder(parameters);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult DeleteSalesOrder(string salesOrderKey)
        {
            try
            {
                return _salesServiceProvider.DeleteSalesOrder(salesOrderKey);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<ISalesOrderDetailReturn> GetSalesOrder(string salesOrderKey)
        {
            try
            {
                return _salesServiceProvider.GetSalesOrder(salesOrderKey);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<ISalesOrderDetailReturn>(null, ex.Message);
            }
        }

        public IResult<IQueryable<ISalesOrderSummaryReturn>> GetSalesOrders(FilterSalesOrdersParameters parameters = null)
        {
            try
            {
                return _salesServiceProvider.GetSalesOrders(parameters);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<ISalesOrderSummaryReturn>>(null, ex.Message);
            }
        }
        
        public IResult SetCustomerChileProductAttributeRanges(ISetCustomerProductAttributeRangesParameters parameters)
        {
            try
            {
                return _salesServiceProvider.SetCustomerChileProductAttributeRange(parameters);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult RemoveCustomerChileProductAttributeRanges(IRemoveCustomerChileProductAttributeRangesParameters parameters)
        {
            try
            {
                return _salesServiceProvider.RemoveCustomerChileProductAttributeRanges(parameters);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<ICustomerChileProductAttributeRangesReturn> GetCustomerChileProductAttributeRanges(string customerKey, string chileProductKey)
        {
            try
            {
                return _salesServiceProvider.GetCustomerChileProductAttributeRanges(customerKey, chileProductKey);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<ICustomerChileProductAttributeRangesReturn>(null, ex.Message);
            }
        }

        public IResult<IQueryable<ICustomerChileProductAttributeRangesReturn>> GetCustomerChileProductsAttributeRanges(string customerKey)
        {
            try
            {
                return _salesServiceProvider.GetCustomerChileProductsAttributeRanges(customerKey);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<ICustomerChileProductAttributeRangesReturn>>(null, ex.Message);
            }
        }

        public IResult SetPickedInventory(string contextKey, ISetPickedInventoryParameters parameters)
        {
            try
            {
                return _salesServiceProvider.SetPickedInventoryForSalesOrder(contextKey, parameters);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<IPickableInventoryReturn> GetPickableInventoryForContext(FilterInventoryForPickingContextParameters parameters)
        {
            try
            {
                return _salesServiceProvider.GetInventoryToPickForOrder((FilterInventoryForShipmentOrderParameters)parameters);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<IPickableInventoryReturn>(null, ex.Message);
            }
        }

        public IResult<ICustomerProductCodeReturn> GetCustomerProductCode(string customerKey, string chileProductKey)
        {
            try
            {
                return _salesServiceProvider.GetCustomerProductCode(customerKey, chileProductKey);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<ICustomerProductCodeReturn>(null, ex.Message);
            }
        }

        public IResult SetCustomerProductCode(string customerKey, string chileProductKey, string code)
        {
            try
            {
                return _salesServiceProvider.SetCustomerProductCode(customerKey, chileProductKey, code);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult PostInvoice(string customerOrderKey)
        {
            try
            {
                return _salesServiceProvider.PostInvoice(customerOrderKey);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<IQueryable<ISalesQuoteSummaryReturn>> GetSalesQuotes(FilterSalesQuotesParameters parameters = null)
        {
            try
            {
                return _salesServiceProvider.GetSalesQuotes(parameters);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<ISalesQuoteSummaryReturn>>(null, ex.Message);
            }
        }

        public IResult<ISalesQuoteDetailReturn> GetSalesQuote(int salesQuoteNumber)
        {
            try
            {
                return _salesServiceProvider.GetSalesQuote(salesQuoteNumber);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<ISalesQuoteDetailReturn>(null, ex.Message);
            }
        }

        public IResult<ISalesQuoteDetailReturn> GetSalesQuote(string salesQuoteKey)
        {
            try
            {
                return _salesServiceProvider.GetSalesQuote(salesQuoteKey);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<ISalesQuoteDetailReturn>(null, ex.Message);
            }
        }

        public IResult<string> SetSalesQuote(ISalesQuoteParameters parameters, bool updateExisting = false)
        {
            try
            {
                return _salesServiceProvider.SetSalesQuote(parameters, updateExisting);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult<ISalesQuoteReportReturn> GetSalesQuoteReport(int salesQuoteNumber)
        {
            try
            {
                return _salesServiceProvider.GetSalesQuoteReport(salesQuoteNumber);
            }
            catch(Exception ex)
            {
                ex = ex.GetBaseException();
                _exceptionLogger.LogException(ex);
                return new FailureResult<ISalesQuoteReportReturn>(null, ex.Message);
            }
        }
    }
}