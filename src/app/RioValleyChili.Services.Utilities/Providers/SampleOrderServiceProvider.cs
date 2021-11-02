using System;
using System.Collections.Generic;
using System.Linq;
using EF_Split_Projector;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Conductors;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Core;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class SampleOrderServiceProvider : IUnitOfWorkContainer<ISampleOrderUnitOfWork>
    {
        #region Fields and Constructors.

        ISampleOrderUnitOfWork IUnitOfWorkContainer<ISampleOrderUnitOfWork>.UnitOfWork { get { return _sampleOrderUnitOfWork; } }
        private readonly ISampleOrderUnitOfWork _sampleOrderUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public SampleOrderServiceProvider(ISampleOrderUnitOfWork sampleOrderUnitOfWork, ITimeStamper timeStamper)
        {
            if(sampleOrderUnitOfWork == null) { throw new ArgumentNullException("sampleOrderUnitOfWork"); }
            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }

            _sampleOrderUnitOfWork = sampleOrderUnitOfWork;
            _timeStamper = timeStamper;
        }

        #endregion

        [SynchronizeOldContext(NewContextMethod.SampleOrder)]
        public IResult<string> SetSampleOrder(ISetSampleOrderParameters parameters)
        {
            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters.ConvertTo<string>();
            }

            var conductorResult = new SetSampleOrderConductor(_sampleOrderUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parsedParameters.ResultingObject);
            if(!conductorResult.Success)
            {
                return conductorResult.ConvertTo<string>();
            }

            _sampleOrderUnitOfWork.Commit();

            var sampleOrderKey = conductorResult.ResultingObject.ToSampleOrderKey();
            return SyncParameters.Using(new SuccessResult<string>(sampleOrderKey), new SyncSampleOrderParameters { SampleOrderKey = sampleOrderKey });
        }

        [SynchronizeOldContext(NewContextMethod.SampleOrder)]
        public IResult DeleteSampleOrder(string sampleOrderKey)
        {
            var keyResult = KeyParserHelper.ParseResult<ISampleOrderKey>(sampleOrderKey);
            if(!keyResult.Success)
            {
                return keyResult;
            }

            int? sampleId;
            var result = new DeleteSampleOrderConductor(_sampleOrderUnitOfWork).Execute(keyResult.ResultingObject.ToSampleOrderKey(), out sampleId);
            if(result.Success)
            {
                _sampleOrderUnitOfWork.Commit();
            }

            return SyncParameters.Using(new SuccessResult(), new SyncSampleOrderParameters { DeleteSampleID = sampleId });
        }

        public IResult<ISampleOrderDetailReturn> GetSampleOrder(string sampleOrderKey)
        {
            var keyResult = KeyParserHelper.ParseResult<ISampleOrderKey>(sampleOrderKey);
            if(!keyResult.Success)
            {
                return keyResult.ConvertTo<ISampleOrderDetailReturn>();
            }

            var key = keyResult.ResultingObject.ToSampleOrderKey();
            var predicate = SampleOrderPredicates.ByKey(key);
            var select = SampleOrderProjectors.SelectDetail();

            var result = _sampleOrderUnitOfWork.SampleOrderRepository.Filter(predicate).SplitSelect(select).FirstOrDefault();
            if(result == null)
            {
                return new InvalidResult<ISampleOrderDetailReturn>(null, string.Format(UserMessages.SampleOrderNotFound, key));
            }

            return new SuccessResult<ISampleOrderDetailReturn>(result);
        }

        public IResult<IQueryable<ISampleOrderSummaryReturn>> GetSampleOrders(FilterSampleOrdersParameters parameters)
        {
            var parsedFilterParameters = parameters.ParseToPredicateBuilderFilters();
            if(!parsedFilterParameters.Success)
            {
                return parsedFilterParameters.ConvertTo<IQueryable<ISampleOrderSummaryReturn>>();
            }

            var predicateResult = SampleOrderPredicateBuilder.BuildPredicate(parsedFilterParameters.ResultingObject);
            if(!predicateResult.Success)
            {
                return predicateResult.ConvertTo<IQueryable<ISampleOrderSummaryReturn>>();
            }
            var predicate = predicateResult.ResultingObject;

            var select = SampleOrderProjectors.SelectSummary();
            var query = _sampleOrderUnitOfWork.SampleOrderRepository.All().AsExpandable().Where(predicate).Select(select);

            return new SuccessResult<IQueryable<ISampleOrderSummaryReturn>>(query);
        }

        [SynchronizeOldContext(NewContextMethod.SampleOrder)]
        public IResult SetSampleSpecs(ISetSampleSpecsParameters parameters)
        {
            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters;
            }

            var result = new SetSampleSpecsCommand(_sampleOrderUnitOfWork).Execute(parsedParameters.ResultingObject);
            if(!result.Success)
            {
                return result;
            }

            _sampleOrderUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new SyncSampleOrderParameters { SampleOrderKey = result.ResultingObject.ToSampleOrderKey() });
        }

        [SynchronizeOldContext(NewContextMethod.SampleOrder)]
        public IResult SetSampleMatch(ISetSampleMatchParameters parameters)
        {
            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters;
            }

            var result = new SetSampleMatchCommand(_sampleOrderUnitOfWork).Execute(parsedParameters.ResultingObject);
            if(!result.Success)
            {
                return result;
            }

            _sampleOrderUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new SyncSampleOrderParameters { SampleOrderKey = result.ResultingObject.ToSampleOrderKey() });
        }

        [SynchronizeOldContext(NewContextMethod.SampleOrder)]
        public IResult<ISampleOrderJournalEntryReturn> SetJournalEntry(ISetSampleOrderJournalEntryParameters parameters)
        {
            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters.ConvertTo<ISampleOrderJournalEntryReturn>();
            }

            var result = new SetSampleJournalEntryCommand(_sampleOrderUnitOfWork).Execute(parsedParameters.ResultingObject);
            if(!result.Success)
            {
                return result.ConvertTo<ISampleOrderJournalEntryReturn>();
            }

            _sampleOrderUnitOfWork.Commit();

            var journalEntry = _sampleOrderUnitOfWork.SampleOrderJournalEntryRepository.Filter(result.ResultingObject.ToSampleOrderJournalEntryKey().FindByPredicate)
                .Select(SampleOrderJournalEntryProjectors.Select().ExpandAll())
                .FirstOrDefault();

            return SyncParameters.Using(new SuccessResult<ISampleOrderJournalEntryReturn>(journalEntry),
                new SyncSampleOrderParameters { SampleOrderKey = result.ResultingObject.ToSampleOrderKey() });
        }

        [SynchronizeOldContext(NewContextMethod.SampleOrder)]
        public IResult DeleteJournalEntry(string journalEntryKey)
        {
            var keyResult = KeyParserHelper.ParseResult<ISampleOrderJournalEntryKey>(journalEntryKey);
            if(!keyResult.Success)
            {
                return keyResult;
            }

            var journalEntry = _sampleOrderUnitOfWork.SampleOrderJournalEntryRepository.FindByKey(keyResult.ResultingObject.ToSampleOrderJournalEntryKey());
            if(journalEntry == null)
            {
                return new NoWorkRequiredResult();
            }

            var sampleOrderKey = journalEntry.ToSampleOrderKey();

            _sampleOrderUnitOfWork.SampleOrderJournalEntryRepository.Remove(journalEntry);
            _sampleOrderUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new SyncSampleOrderParameters { SampleOrderKey = sampleOrderKey });
        }

        public IResult<IEnumerable<string>> GetCustomerProductNames()
        {
            var query = _sampleOrderUnitOfWork.SampleOrderItemRepository.All().Select(i => i.CustomerProductName).Distinct();
            return new SuccessResult<IEnumerable<string>>(query);
        }

        public IResult<ISampleOrderMatchingSummaryReportReturn> GetSampleOrderMatchingSummaryReport(string sampleOrderKey, string itemKey)
        {
            var keyResult = KeyParserHelper.ParseResult<ISampleOrderKey>(sampleOrderKey);
            if(!keyResult.Success)
            {
                return keyResult.ConvertTo<ISampleOrderMatchingSummaryReportReturn>();
            }

            SampleOrderItemKey orderItemKey = null;
            if(!string.IsNullOrWhiteSpace(itemKey))
            {
                var itemKeyResult = KeyParserHelper.ParseResult<ISampleOrderItemKey>(itemKey);
                if(!itemKeyResult.Success)
                {
                    return itemKeyResult.ConvertTo<ISampleOrderMatchingSummaryReportReturn>();
                }
                orderItemKey = itemKeyResult.ResultingObject.ToSampleOrderItemKey();
            }

            var result = _sampleOrderUnitOfWork.SampleOrderRepository
                .Filter(keyResult.ResultingObject.ToSampleOrderKey().FindByPredicate)
                .Select(SampleOrderProjectors.SelectMatchingSummaryReport(orderItemKey))
                .FirstOrDefault();

            return new SuccessResult<ISampleOrderMatchingSummaryReportReturn>(result);
        }

        public IResult<ISampleOrderRequestReportReturn> GetSampleOrderRequestReport(string sampleOrderKey)
        {
            var keyResult = KeyParserHelper.ParseResult<ISampleOrderKey>(sampleOrderKey);
            if(!keyResult.Success)
            {
                return keyResult.ConvertTo<ISampleOrderRequestReportReturn>();
            }

            var result = _sampleOrderUnitOfWork.SampleOrderRepository
                .Filter(keyResult.ResultingObject.ToSampleOrderKey().FindByPredicate)
                .Select(SampleOrderProjectors.SelectSampleOrderRequestReport())
                .FirstOrDefault();

            return new SuccessResult<ISampleOrderRequestReportReturn>(result);
        }
    }
}