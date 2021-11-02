using System;
using System.ComponentModel;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Reporting.Models;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Reporting.Services
{
    [DataObject]
    public class ProductionResultsReportingService : ReportingService
    {
        private readonly IProductionResultsService _productionResultsService;

        public ProductionResultsReportingService() : this(ResolveService<IProductionResultsService>()) { }

        public ProductionResultsReportingService(IProductionResultsService productionResultsService)
        {
            if(productionResultsService == null) throw new ArgumentNullException("productionResultsService");
            _productionResultsService = productionResultsService;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProductionRecapReportModel GetProductionRecap(DateTime startDate, DateTime endDate)
        {
            var result = _productionResultsService.GetProductionRecapReport(startDate, endDate);
            if(result.Success)
            {
                var mapped = result.ResultingObject.Map().To<ProductionRecapReportModel>();
                return mapped;
            }
            return null;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProductionAdditiveInputs GetProductionAdditiveInputs(DateTime startDate, DateTime endDate)
        {
            var result = _productionResultsService.GetProductionAdditiveInputsReport(startDate, endDate);
            if(result.Success)
            {
                var mapped = result.ResultingObject.Map().To<ProductionAdditiveInputs>();
                return mapped;
            }

            return null;
        }
    }
}