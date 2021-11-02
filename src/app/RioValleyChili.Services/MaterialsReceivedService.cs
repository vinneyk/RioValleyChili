using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService;
using RioValleyChili.Services.Interfaces.Returns.MaterialsReceivedService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class MaterialsReceivedService : IMaterialsReceivedService
    {
        private readonly MaterialsServiceProvider _materialsServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public MaterialsReceivedService(MaterialsServiceProvider materialsServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(materialsServiceProvider == null) { throw new ArgumentNullException("materialsServiceProvider"); }
            _materialsServiceProvider = materialsServiceProvider;

            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }
            _exceptionLogger = exceptionLogger;
        }

        public IResult<string> CreateChileMaterialsReceived(ICreateChileMaterialsReceivedParameters parameters)
        {
            try
            {
                return _materialsServiceProvider.CreateChileMaterialsReceived(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult<string> UpdateChileMaterialsReceived(IUpdateChileMaterialsReceivedParameters parameters)
        {
            try
            {
                return _materialsServiceProvider.UpdateChileMaterialsReceived(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult<IQueryable<IChileMaterialsReceivedSummaryReturn>> GetChileMaterialsReceivedSummaries(ChileMaterialsReceivedFilters filters = null)
        {
            try
            {
                return _materialsServiceProvider.GetChileMaterialsReceivedSummaries(filters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IChileMaterialsReceivedSummaryReturn>>(null, ex.Message);
            }
        }

        public IResult<IChileMaterialsReceivedDetailReturn> GetChileMaterialsReceivedDetail(string lotKey)
        {
            try
            {
                return _materialsServiceProvider.GetChileMaterialsReceivedDetail(lotKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IChileMaterialsReceivedDetailReturn>(null, ex.Message);
            }
        }

        public IResult<IEnumerable<string>> GetChileVarieties()
        {
            try
            {
                return _materialsServiceProvider.GetChileVarieties();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEnumerable<string>>(null, ex.Message);
            }
        }

        public IResult<IChileMaterialsReceivedRecapReturn> GetChileRecapReport(string lotKey)
        {
            try
            {
                return _materialsServiceProvider.GetRecapReport(lotKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IChileMaterialsReceivedRecapReturn>(null, ex.Message);
            }
        }
    }
}