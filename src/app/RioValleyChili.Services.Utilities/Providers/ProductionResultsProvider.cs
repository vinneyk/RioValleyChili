using System;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Commands.Production;
using RioValleyChili.Services.Utilities.Conductors;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Core;
using Solutionhead.Services;
using IProductionUnitOfWork = RioValleyChili.Data.Interfaces.UnitsOfWork.IProductionUnitOfWork;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class ProductionResultsProvider : IUnitOfWorkContainer<IProductionUnitOfWork>
    {
        IProductionUnitOfWork IUnitOfWorkContainer<IProductionUnitOfWork>.UnitOfWork { get { return _productionUnitOfWork; } }
        private readonly IProductionUnitOfWork _productionUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public ProductionResultsProvider(IProductionUnitOfWork productionUnitOfWork, ITimeStamper timeStamper)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }

            _productionUnitOfWork = productionUnitOfWork;
            _timeStamper = timeStamper;
        }

        public IResult<IQueryable<IProductionResultDetailReturn>> GetProductionResults(GetObjectParameters parameters)
        {
            LotKey productionResultKey = null;
            if(parameters != null && parameters.Key != null)
            {
                var keyResult = KeyParserHelper.ParseResult<ILotKey>(parameters.Key);
                if(!keyResult.Success)
                {
                    return keyResult.ConvertTo<IQueryable<IProductionResultDetailReturn>>();
                }
                productionResultKey = new LotKey(keyResult.ResultingObject);
            }
            var includeDetails = productionResultKey != null && parameters.IncludeDetails;
            return new GetProductionResultsCommand(_productionUnitOfWork).Execute(productionResultKey, includeDetails);
        }

        [SynchronizeOldContext(NewContextMethod.SyncProductionBatchResults)]
        public IResult<string> CreateProductionBatchResults(ICreateProductionBatchResultsParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parametersResult = parameters.ToParsedParameters();
            if(!parametersResult.Success)
            {
                return parametersResult.ConvertTo<string>();
            }

            var createResult = new ProductionBatchResultsConductor(_productionUnitOfWork).Create(_timeStamper.CurrentTimeStamp, parametersResult.ResultingObject);
            if(!createResult.Success)
            {
                return createResult.ConvertTo<string>();
            }

            _productionUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult<string>(parametersResult.ResultingObject.LotKey), parametersResult.ResultingObject.LotKey);
        }

        [SynchronizeOldContext(NewContextMethod.SyncProductionBatchResults)]
        public IResult UpdateProductionBatchResults(IUpdateProductionBatchResultsParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters;
            }

            var updateResult = new ProductionBatchResultsConductor(_productionUnitOfWork).Update(_timeStamper.CurrentTimeStamp, parsedParameters.ResultingObject);
            if(!updateResult.Success)
            {
                return updateResult;
            }

            _productionUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), parsedParameters.ResultingObject.LotKey);
        }

        public IResult<IProductionRecapReportReturn> GetProductionRecapReport(DateTime startDate, DateTime endDate)
        {
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, DateTime.Now.Kind);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0, DateTime.Now.Kind);

            var queryStartDate = startDate.ConvertLocalToUTC();
            var queryEndDate = endDate.AddDays(1).ConvertLocalToUTC();

            var select = LotProductionResultsProjectors.SelectProductionRecap(_productionUnitOfWork.ProductionBatchRepository.All());

            var productionResults = _productionUnitOfWork.LotProductionResultsRepository
                .Filter(c => c.ProductionBegin >= queryStartDate && c.ProductionBegin < queryEndDate)
                .Select(select).ToList();

            return new SuccessResult<IProductionRecapReportReturn>(new ProductionRecapReportReturn(startDate, endDate, productionResults));
        }

        public IResult<IProductionAdditiveInputsReportReturn> GetProductionAdditiveInputsReport(DateTime startDate, DateTime endDate)
        {
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, DateTime.Now.Kind);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0, DateTime.Now.Kind);

            var queryStartDate = startDate.ConvertLocalToUTC();
            var queryEndDate = endDate.AddDays(1).ConvertLocalToUTC();

            var select = LotProductionResultsProjectors.SelectAdditiveInputs(_productionUnitOfWork).ExpandAll();
            var results = _productionUnitOfWork.LotProductionResultsRepository
                .All()
                .Select(select)
                .Where(c => c.ProductionDate >= queryStartDate && c.ProductionDate < queryEndDate && c.PickedAdditiveItems.Any())
                .ToList();

            return new SuccessResult<IProductionAdditiveInputsReportReturn>(new ProductionAdditiveInputsReportReturn(startDate, endDate, results));
        }
    }
}