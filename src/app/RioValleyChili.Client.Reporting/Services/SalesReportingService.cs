using System;
using System.Collections.Generic;
using System.ComponentModel;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Reporting.Models;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Reporting.Services
{
    [DataObject]
    public class SalesReportingService : ReportingService
    {
        private readonly ISalesService _salesService;

        public SalesReportingService() : this(ResolveService<ISalesService>()) { }

        public SalesReportingService(ISalesService salesService)
        {
            if(salesService == null) throw new ArgumentNullException("salesService");
            _salesService = salesService;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public CustomerContract GetCustomerContract(string contractKey)
        {
            return _salesService.GetCustomerContract(contractKey)
                .ResultingObject.Map().To<CustomerContract>();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<CustomerContractItemDrawSummary> GetCustomerContractDrawSummary(string contractKey)
        {
            return _salesService.GetContractShipmentSummary(contractKey)
                .ResultingObject.Map().To<IEnumerable<CustomerContractItemDrawSummary>>();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public SalesQuoteReportModel GetSalesQuote(int salesQuoteNumber)
        {
            return _salesService.GetSalesQuoteReport(salesQuoteNumber).ResultingObject.Map().To<SalesQuoteReportModel>();
        }
    }
}