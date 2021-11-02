using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.LotService;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class LotService : ILotService
    {
        private readonly LotServiceProvider _lotServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public LotService(LotServiceProvider lotServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(lotServiceProvider == null) { throw new ArgumentNullException("lotServiceProvider"); }
            _lotServiceProvider = lotServiceProvider;

            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }
            _exceptionLogger = exceptionLogger;
        }

        public IResult<ILotQualitySummariesReturn> GetLotSummaries(FilterLotParameters parameters = null)
        {
            try
            {
                return _lotServiceProvider.GetLots(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ILotQualitySummariesReturn>(null, ex.Message);
            }
        }

        public IResult<ILotQualitySingleSummaryReturn> GetLotSummary(string lotKey)
        {
            try
            {
                return _lotServiceProvider.GetLot(lotKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ILotQualitySingleSummaryReturn>(null, ex.Message);
            }
        }

        public IResult<ILotStatInfoReturn> SetLotAttributes(ISetLotAttributeParameters parameters)
        {
            try
            {
                return _lotServiceProvider.SetLotAttributes(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ILotStatInfoReturn>(null, ex.Message);
            }
        }

        public IResult AddLotAttributes(IAddLotAttributesParameters parameters)
        {
            try
            {
                return _lotServiceProvider.AddLotAttributes(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult();
            }
        }

        public IResult<ICreateLotDefectReturn> CreateLotDefect(ICreateLotDefectParameters parameters)
        {
            try
            {
                return _lotServiceProvider.CreateLotDefect(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ICreateLotDefectReturn>(null, ex.Message);
            }
        }

        public IResult<ILotStatInfoReturn> RemoveLotDefectResolution(IRemoveLotDefectResolutionParameters parameters)
        {
            try
            {
                return _lotServiceProvider.RemoveLotDefectResolution(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ILotStatInfoReturn>(null, ex.Message);
            }
        }

        public IResult<IResolutionsByDefectTypeReturn> GetDefectResolutions()
        {
            try
            {
                return _lotServiceProvider.GetDefectResolutions();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IResolutionsByDefectTypeReturn>(null, ex.Message);
            }
        }

        public IResult<ILotStatInfoReturn> SetLotHoldStatus(ISetLotHoldStatusParameters parameters)
        {
            try
            {
                return _lotServiceProvider.SetLotHoldStatus(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ILotStatInfoReturn>(null, ex.Message);
            }
        }

        public IResult<ILotStatInfoReturn> SetLotQualityStatus(ISetLotStatusParameters parameters)
        {
            try
            {
                return _lotServiceProvider.SetLotQualityStatus(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ILotStatInfoReturn>(null, ex.Message);
            }
        }

        public IResult<ILabReportReturn> GetLabReport(string lotKey)
        {
            try
            {
                return _lotServiceProvider.GetLabReport(lotKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                throw;
            }
        }

        public IResult<ILabReportReturn> GetLabReport(DateTime minTestDate, DateTime maxTestDate)
        {
            try
            {
                return _lotServiceProvider.GetLabReport(minTestDate, maxTestDate);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                throw;
            }
        }

        public IResult SetLotPackagingReceived(ISetLotPackagingReceivedParameters parameters)
        {
            try
            {
                return _lotServiceProvider.SetLotPackagingReceived(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                throw;
            }
        }

        public IResult AddLotAllowance(ILotAllowanceParameters parameters)
        {
            try
            {
                return _lotServiceProvider.AddLotAllowance(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                throw;
            }
        }

        public IResult RemoveLotAllowance(ILotAllowanceParameters parameters)
        {
            try
            {
                return _lotServiceProvider.RemoveLotAllowance(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                throw;
            }
        }

        public IResult<IEnumerable<ILotOutputTraceReturn>> GetOutputTrace(string lotKey)
        {
            try
            {
                return _lotServiceProvider.GetOutputTrace(lotKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEnumerable<ILotOutputTraceReturn>>(null, ex.Message);
            }
        }

        public IResult<IEnumerable<ILotInputTraceReturn>> GetInputTrace(string lotKey)
        {
            try
            {
                return _lotServiceProvider.GetInputTrace(lotKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEnumerable<ILotInputTraceReturn>>(null, ex.Message);
            }
        }

        public IResult<ILotHistoryReturn> GetLotHistory(string lotKey)
        {
            try
            {
                return _lotServiceProvider.GetLotHistory(lotKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ILotHistoryReturn>(null, ex.Message);
            }
        }
    }
}