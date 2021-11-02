using System;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;
using RioValleyChili.Services.Interfaces.Returns.MillAndWetdownService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class MillAndWetdownService : IMillAndWetDownService
    {
        private readonly MillAndWetdownServiceProvider _millAndWetdownServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public MillAndWetdownService(MillAndWetdownServiceProvider millAndWetdownServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(millAndWetdownServiceProvider == null) { throw new ArgumentNullException("millAndWetdownServiceProvider"); }
            _millAndWetdownServiceProvider = millAndWetdownServiceProvider;

            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }
            _exceptionLogger = exceptionLogger;
        }

        public IResult<string> CreateMillAndWetdown(ICreateMillAndWetdownParameters parameters)
        {
            try
            {
                return _millAndWetdownServiceProvider.CreateMillAndWetdown(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult UpdateMillAndWetdown(IUpdateMillAndWetdownParameters parameters)
        {
            try
            {
                return _millAndWetdownServiceProvider.UpdateMillAndWetdown(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult DeleteMillAndWetdown(string lotKey)
        {
            try
            {
                return _millAndWetdownServiceProvider.DeleteMillAndWetdown(lotKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<IMillAndWetdownDetailReturn> GetMillAndWetdownDetail(string lotKey)
        {
            try
            {
                return _millAndWetdownServiceProvider.GetMillAndWetdownDetail(lotKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IMillAndWetdownDetailReturn>(null, ex.Message);
            }
        }

        public IResult<IQueryable<IMillAndWetdownSummaryReturn>> GetMillAndWetdownSummaries()
        {
            try
            {
                return _millAndWetdownServiceProvider.GetMillAndWetdownSummaries();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IMillAndWetdownSummaryReturn>>(null, ex.Message);
            }
        }
    }
}