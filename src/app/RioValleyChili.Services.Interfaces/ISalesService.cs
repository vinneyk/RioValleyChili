using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;
using RioValleyChili.Services.Interfaces.ServiceCompositions;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface ISalesService : IPickInventoryServiceComponent
    {
        #region Customers and Brokers

        IResult<IEnumerable<ICompanySummaryReturn>> GetCustomersForBroker(string brokerKey);

        IResult AssignCustomerToBroker(string brokerKey, string customerKey);

        IResult RemoveCustomerFromBroker(string brokerKey, string customerKey);

        #endregion

        IResult<string> CreateCustomerContract(ICreateCustomerContractParameters parameters);

        IResult UpdateCustomerContract(IUpdateCustomerContractParameters parameters);

        IResult SetCustomerContractsStatus(ISetContractsStatusParameters parameters);

        IResult<ICustomerContractDetailReturn> GetCustomerContract(string customerContractKey);

        IResult RemoveCustomerContract(string customerContractKey);

        IResult<IQueryable<ICustomerContractSummaryReturn>> GetCustomerContracts(FilterCustomerContractsParameters parameters = null);

        IResult<IEnumerable<string>> GetDistinctContractPaymentTerms();

        IResult<ICustomerContractOrdersReturn> GetOrdersForCustomerContract(string customerContractKey);

        IResult<IContractShipmentSummaryReturn> GetContractShipmentSummary(string contractKey);

        IResult CompleteExpiredContracts();

        IResult<string> CreateSalesOrder(ICreateSalesOrderParameters parameters);

        IResult UpdateSalesOrder(IUpdateSalesOrderParameters parameters);

        IResult DeleteSalesOrder(string salesOrderKey);

        IResult<ISalesOrderDetailReturn> GetSalesOrder(string salesOrderKey);

        IResult<IQueryable<ISalesOrderSummaryReturn>> GetSalesOrders(FilterSalesOrdersParameters parameters = null);

        IResult SetCustomerChileProductAttributeRanges(ISetCustomerProductAttributeRangesParameters parameters);

        IResult RemoveCustomerChileProductAttributeRanges(IRemoveCustomerChileProductAttributeRangesParameters parameters);

        IResult<ICustomerChileProductAttributeRangesReturn> GetCustomerChileProductAttributeRanges(string customerKey, string chileProductKey);

        IResult<IQueryable<ICustomerChileProductAttributeRangesReturn>> GetCustomerChileProductsAttributeRanges(string customerKey);
            
        IResult<ICustomerProductCodeReturn> GetCustomerProductCode(string customerKey, string chileProductKey);

        IResult SetCustomerProductCode(string customerKey, string chileProductKey, string code);

        IResult PostInvoice(string customerOrderKey);

        IResult<IQueryable<ISalesQuoteSummaryReturn>> GetSalesQuotes(FilterSalesQuotesParameters parameters = null);

        IResult<ISalesQuoteDetailReturn> GetSalesQuote(int salesQuoteNumber);

        IResult<ISalesQuoteDetailReturn> GetSalesQuote(string salesQuoteKey);

        IResult<string> SetSalesQuote(ISalesQuoteParameters parameters, bool updateExisting = false);

        IResult<ISalesQuoteReportReturn> GetSalesQuoteReport(int salesQuoteNumber);
    }
}