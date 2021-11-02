using System;
using System.Linq;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class ProductionResultsService : IProductionResultsService
    {
        #region Fields and Constructors.

        private readonly ProductionResultsProvider _productionResultsProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public ProductionResultsService(ProductionResultsProvider productionResultsProvider, IExceptionLogger exceptionLogger)
        {
            if(productionResultsProvider == null)
            {
                throw new ArgumentNullException("productionResultsProvider");
            }
            if(exceptionLogger == null)
            {
                throw new ArgumentNullException("exceptionLogger");
            }

            _productionResultsProvider = productionResultsProvider;
            _exceptionLogger = exceptionLogger;
        }

        #endregion

        public IResult<IQueryable<IProductionResultSummaryReturn>> GetProductionResultSummaries()
        {
            try
            {
                return _productionResultsProvider.GetProductionResults(new GetObjectParameters { IncludeDetails = false });
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IProductionResultSummaryReturn>>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<IProductionResultDetailReturn> GetProductionResultDetail(string lotKey)
        {
            try
            {
                var result = _productionResultsProvider.GetProductionResults(new GetObjectParameters
                    {
                        Key = lotKey,
                        IncludeDetails = true
                    });

                if(!result.Success)
                {
                    return result.ConvertTo((IProductionResultDetailReturn) null);
                }

                var productionResult = result.ResultingObject.ToList().SingleOrDefault();
                if(productionResult == null)
                {
                    return new InvalidResult<IProductionResultDetailReturn>(null, string.Format(UserMessages.ProductionResultNotFound, lotKey));
                }

                return new SuccessResult<IProductionResultDetailReturn>(productionResult);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IProductionResultDetailReturn>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<IProductionResultDetailReturn> GetProductionResultDetailByProductionBatchKey(string lotKey)
        {
            return GetProductionResultDetail(lotKey);
        }

        public IResult<string> CreateProductionBatchResults(ICreateProductionBatchResultsParameters parameters)
        {
            try
            {
                return _productionResultsProvider.CreateProductionBatchResults(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult UpdateProductionBatchResults(IUpdateProductionBatchResultsParameters parameters)
        {
            try
            {
                return _productionResultsProvider.UpdateProductionBatchResults(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.GetInnermostException().Message);
            }
        }

        public IResult<IProductionRecapReportReturn> GetProductionRecapReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                return _productionResultsProvider.GetProductionRecapReport(startDate, endDate);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IProductionRecapReportReturn>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<IProductionAdditiveInputsReportReturn> GetProductionAdditiveInputsReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                return _productionResultsProvider.GetProductionAdditiveInputsReport(startDate, endDate);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IProductionAdditiveInputsReportReturn>(null, ex.GetInnermostException().Message);
            }
        }
    }
}