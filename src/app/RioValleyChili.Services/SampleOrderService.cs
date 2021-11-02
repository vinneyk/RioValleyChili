using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class SampleOrderService : ISampleOrderService
    {
        #region Fields and Constructors.

        private readonly SampleOrderServiceProvider _sampleOrderServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public SampleOrderService(SampleOrderServiceProvider sampleOrderServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(sampleOrderServiceProvider == null) { throw new ArgumentNullException("sampleOrderServiceProvider"); }
            _sampleOrderServiceProvider = sampleOrderServiceProvider;

            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }
            _exceptionLogger = exceptionLogger;
        }

        #endregion

        public IResult<string> SetSampleOrder(ISetSampleOrderParameters parameters)
        {
            try
            {
                return _sampleOrderServiceProvider.SetSampleOrder(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult DeleteSampleOrder(string sampleOrderKey)
        {
            try
            {
                return _sampleOrderServiceProvider.DeleteSampleOrder(sampleOrderKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult SetSampleSpecs(ISetSampleSpecsParameters parameters)
        {
            try
            {
                return _sampleOrderServiceProvider.SetSampleSpecs(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult SetSampleMatch(ISetSampleMatchParameters parameters)
        {
            try
            {
                return _sampleOrderServiceProvider.SetSampleMatch(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<ISampleOrderJournalEntryReturn> SetJournalEntry(ISetSampleOrderJournalEntryParameters parameters)
        {
            try
            {
                return _sampleOrderServiceProvider.SetJournalEntry(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ISampleOrderJournalEntryReturn>(null, ex.Message);
            }
        }

        public IResult DeleteJournalEntry(string journalEntryKey)
        {
            try
            {
                return _sampleOrderServiceProvider.DeleteJournalEntry(journalEntryKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult<IEnumerable<string>> GetCustomerProducNames()
        {
            try
            {
                return _sampleOrderServiceProvider.GetCustomerProductNames();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEnumerable<string>>(null, ex.Message);
            }
        }

        public IResult<ISampleOrderDetailReturn> GetSampleOrder(string sampleOrderKey)
        {
            try
            {
                return _sampleOrderServiceProvider.GetSampleOrder(sampleOrderKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ISampleOrderDetailReturn>(null, ex.Message);
            }
        }

        public IResult<IQueryable<ISampleOrderSummaryReturn>> GetSampleOrders(FilterSampleOrdersParameters parameters = null)
        {
            try
            {
                return _sampleOrderServiceProvider.GetSampleOrders(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<ISampleOrderSummaryReturn>>(null, ex.Message);
            }
        }

        public IResult<ISampleOrderMatchingSummaryReportReturn> GetSampleOrderMatchingSummaryReport(string sampleOrderKey, string itemKey = null)
        {
            try
            {
                return _sampleOrderServiceProvider.GetSampleOrderMatchingSummaryReport(sampleOrderKey, itemKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ISampleOrderMatchingSummaryReportReturn>(null, ex.Message);
            }
        }

        public IResult<ISampleOrderRequestReportReturn> GetSampleOrderRequestReport(string sampleOrderKey)
        {
            try
            {
                return _sampleOrderServiceProvider.GetSampleOrderRequestReport(sampleOrderKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<ISampleOrderRequestReportReturn>(null, ex.Message);
            }
        }
    }
}