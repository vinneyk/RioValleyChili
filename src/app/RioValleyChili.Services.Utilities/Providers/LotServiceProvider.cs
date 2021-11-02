using System;
using System.Collections.Generic;
using System.Linq;
using EF_Split_Projector;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.LotService;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Commands.LotCommands;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Conductors;
using RioValleyChili.Services.Utilities.Extensions;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Core;
using Solutionhead.Data;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class LotServiceProvider : IUnitOfWorkContainer<ILotUnitOfWork>
    {
        ILotUnitOfWork IUnitOfWorkContainer<ILotUnitOfWork>.UnitOfWork { get { return _lotUnitOfWork; } }
        private readonly ILotUnitOfWork _lotUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public LotServiceProvider(ILotUnitOfWork lotUnitOfWork, ITimeStamper timeStamper)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }

            _lotUnitOfWork = lotUnitOfWork;
            _timeStamper = timeStamper;
        }

        public IResult<ILotQualitySummariesReturn> GetLots(FilterLotParameters parameters)
        {
            var predicateResult = LotPredicateBuilder.BuildPredicate(_lotUnitOfWork, parameters);
            if(!predicateResult.Success)
            {
                return predicateResult.ConvertTo<ILotQualitySummariesReturn>();
            }

            var selectAttributes = AttributeNameProjectors.SelectActiveAttributeNames(_lotUnitOfWork);
            var selectLotSummary = LotProjectors.SplitSelectLotQualitySummary(_lotUnitOfWork, _timeStamper.CurrentTimeStamp);
            return new SuccessResult<ILotQualitySummariesReturn>(new LotSummariesReturn
                {
                    AttributeNamesAndTypes = selectAttributes.ExpandAll().Invoke().ToList(),
                    LotSummaries = _lotUnitOfWork.LotRepository
                        .Filter(predicateResult.ResultingObject)
                        .OrderByLot()
                        .SplitSelect(selectLotSummary)
                });
        }

        public IResult<ILotQualitySingleSummaryReturn> GetLot(string lotKey)
        {
            if(lotKey == null) { throw new ArgumentNullException("lotKey"); }

            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(lotKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo<ILotQualitySingleSummaryReturn>();
            }

            var predicate = new LotKey(lotKeyResult.ResultingObject).FindByPredicate;
            var select = LotProjectors.SplitSelectSingleLotSummary(_lotUnitOfWork, _timeStamper.CurrentTimeStamp);
            var lotSummary = _lotUnitOfWork.LotRepository
                .Filter(predicate)
                .SplitSelect(select)
                .ToList().SingleOrDefault();
            
            if(lotSummary == null)
            {
                return new InvalidResult<ILotQualitySingleSummaryReturn>(null, string.Format(UserMessages.LotNotFound, lotKey));
            }

            return new SuccessResult<ILotQualitySingleSummaryReturn>(lotSummary);
        }

        [SynchronizeOldContext(NewContextMethod.SyncLot)]
        public IResult<ILotStatInfoReturn> SetLotAttributes(ISetLotAttributeParameters parameters)
        {
            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters.ConvertTo<ILotStatInfoReturn>();
            }

            var currentTimeStamp = _timeStamper.CurrentTimeStamp;
            var setAttributesResult = new SetLotAttributesConductor(_lotUnitOfWork).Execute(currentTimeStamp, parsedParameters.ResultingObject);
            if(!setAttributesResult.Success)
            {
                return setAttributesResult.ConvertTo<ILotStatInfoReturn>();
            }

            #region SetLotStatus to 'Accepted'

#warning Remove if/when 'OverrideOldContextLotAsCompleted' is removed. -RI 6/16/2014

            if(parameters.OverrideOldContextLotAsCompleted)
            {
                var setStatusResult = new SetLotStatusConductor(_lotUnitOfWork).Execute(currentTimeStamp, new SetLotStatusParameters
                    {
                        LotKey = parsedParameters.ResultingObject.LotKey,
                        Parameters = parsedParameters.ResultingObject
                    }, true);
                if(!setStatusResult.Success)
                {
                    return setStatusResult.ConvertTo<ILotStatInfoReturn>();
                }
            }

            #endregion

            _lotUnitOfWork.Commit();

            var syncParameters = new SynchronizeLotParameters
                {
                    LotKey = parsedParameters.ResultingObject.LotKey,
                    OverrideOldContextLotAsCompleted = parameters.OverrideOldContextLotAsCompleted
                };
            return SyncParameters.Using(new SuccessResult<ILotStatInfoReturn>(new LotStatInfoReturn(syncParameters)), syncParameters);
        }

        [SynchronizeOldContext(NewContextMethod.SyncLot)]
        public IResult AddLotAttributes(IAddLotAttributesParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters;
            }

            var setAttributesResult = new SetLotAttributesConductor(_lotUnitOfWork).AddLotAttributes(_timeStamper.CurrentTimeStamp, parsedParameters.ResultingObject);
            if(!setAttributesResult.Success)
            {
                return setAttributesResult;
            }

            _lotUnitOfWork.Commit();

            var syncParameters = new SynchronizeLotParameters
                {
                    LotKeys = parsedParameters.ResultingObject.LotKeys,
                    OverrideOldContextLotAsCompleted = parameters.OverrideOldContextLotAsCompleted
                };
            return SyncParameters.Using(new SuccessResult<ILotStatInfoReturn>(new LotStatInfoReturn(syncParameters)), syncParameters);
        }

        public IResult<IResolutionsByDefectTypeReturn> GetDefectResolutions()
        {
            var defectTypes = Enum.GetValues(typeof(DefectTypeEnum)).Cast<DefectTypeEnum>().ToList();
            var resolutionsByType = defectTypes.ToDictionary(d => d, d => d.GetValidResolutions());
            return new SuccessResult<IResolutionsByDefectTypeReturn>(new ResolutionsByDefectTypeReturn
                {
                    DefectResolutions = resolutionsByType
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncLot)]
        public IResult<ILotStatInfoReturn> SetLotHoldStatus(ISetLotHoldStatusParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters.ConvertTo<ILotStatInfoReturn>();
            }

            var result = new SetLotHoldCommand(_lotUnitOfWork).Execute(parsedParameters.ResultingObject, _timeStamper.CurrentTimeStamp);
            if(!result.Success)
            {
                return result.ConvertTo<ILotStatInfoReturn>();
            }

            _lotUnitOfWork.Commit();

            var synchronizeLotParameters = new SynchronizeLotParameters
                {
                    LotKey = parsedParameters.ResultingObject.LotKey,
                    OverrideOldContextLotAsCompleted = false
                };
            return SyncParameters.Using(new SuccessResult<ILotStatInfoReturn>(new LotStatInfoReturn(synchronizeLotParameters)), synchronizeLotParameters);
        }

        [SynchronizeOldContext(NewContextMethod.SyncLot)]
        public IResult<ICreateLotDefectReturn> CreateLotDefect(ICreateLotDefectParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters.ConvertTo<ICreateLotDefectReturn>();
            }

            var createDefectResult = new CreateInHouseContaminationDefectCommand(_lotUnitOfWork).Execute(parsedParameters.ResultingObject, _timeStamper.CurrentTimeStamp);
            if(!createDefectResult.Success)
            {
                return createDefectResult.ConvertTo<ICreateLotDefectReturn>();
            }

            _lotUnitOfWork.Commit();

            var synchronizeLotParameters = new SynchronizeLotParameters
                {
                    LotKey = parsedParameters.ResultingObject.LotKey,
                    OverrideOldContextLotAsCompleted = false
                };
            return SyncParameters.Using(new SuccessResult<ICreateLotDefectReturn>(
                new CreateLotDefectReturn(createDefectResult.ResultingObject.ToLotDefectKey(), synchronizeLotParameters)),
                synchronizeLotParameters);
        }

        [SynchronizeOldContext(NewContextMethod.SyncLot)]
        public IResult<ILotStatInfoReturn> RemoveLotDefectResolution(IRemoveLotDefectResolutionParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters.ConvertTo<ILotStatInfoReturn>();
            }

            var result = new RemoveLotDefectResolutionConductor(_lotUnitOfWork).RemoveLotDefectResolution(parsedParameters.ResultingObject, _timeStamper.CurrentTimeStamp);
            if(!result.Success)
            {
                return result.ConvertTo<ILotStatInfoReturn>();
            }

            _lotUnitOfWork.Commit();

            var synchronizeLotParameters = new SynchronizeLotParameters
                {
                    LotKey = parsedParameters.ResultingObject.LotDefectKey,
                    OverrideOldContextLotAsCompleted = false
                };
            return SyncParameters.Using(new SuccessResult<ILotStatInfoReturn>(new LotStatInfoReturn(synchronizeLotParameters)), synchronizeLotParameters);
        }

        [SynchronizeOldContext(NewContextMethod.SyncLot)]
        public IResult<ILotStatInfoReturn> SetLotQualityStatus(ISetLotStatusParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters.ConvertTo<ILotStatInfoReturn>();
            }

            var result = new SetLotStatusConductor(_lotUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parsedParameters.ResultingObject);
            if(!result.Success)
            {
                return result.ConvertTo<ILotStatInfoReturn>();
            }

            _lotUnitOfWork.Commit();

            var synchronizeLotParameters = new SynchronizeLotParameters
                {
                    LotKey = parsedParameters.ResultingObject.LotKey,
                    OverrideOldContextLotAsCompleted = false
                };
            return SyncParameters.Using(new SuccessResult<ILotStatInfoReturn>(new LotStatInfoReturn(synchronizeLotParameters)), synchronizeLotParameters);
        }

        public IResult<ILabReportReturn> GetLabReport(string lotKey)
        {
            if(lotKey == null) { throw new ArgumentNullException("lotKey"); }

            var parsedLotKey = KeyParserHelper.ParseResult<ILotKey>(lotKey);
            if(!parsedLotKey.Success)
            {
                return parsedLotKey.ConvertTo<ILabReportReturn>();
            }

            return new GetLabReportConductor(_lotUnitOfWork).GetLabReport(((IKey<ChileLot>) new LotKey(parsedLotKey.ResultingObject)).FindByPredicate);
        }

        public IResult<ILabReportReturn> GetLabReport(DateTime minTestDate, DateTime maxTestDate)
        {
            if(minTestDate > maxTestDate)
            {
                var max = minTestDate;
                minTestDate = maxTestDate;
                maxTestDate = max;
            }

            return new GetLabReportConductor(_lotUnitOfWork).GetLabReport(LotPredicates.FilterByAttributeDateForLabReport(minTestDate, maxTestDate));
        }

        [SynchronizeOldContext(NewContextMethod.SyncLot)]
        public IResult SetLotPackagingReceived(ISetLotPackagingReceivedParameters parameters)
        {
            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters;
            }

            var result = new SetLotPackagingReceivedCommand(_lotUnitOfWork).Execute(parsedParameters.ResultingObject);
            if(!result.Success)
            {
                return result;
            }

            _lotUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new SynchronizeLotParameters
                {
                    LotKey = parsedParameters.ResultingObject.LotKey,
                    OverrideOldContextLotAsCompleted = false
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncLot)]
        public IResult AddLotAllowance(ILotAllowanceParameters parameters)
        {
            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters;
            }

            var result = new LotAllowancesCommand(_lotUnitOfWork).Execute(parsedParameters.ResultingObject, true);
            if(!result.Success)
            {
                return result;
            }

            if(result.State != ResultState.NoWorkRequired)
            {
                _lotUnitOfWork.Commit();
            }

            return SyncParameters.Using(new SuccessResult(), new SynchronizeLotParameters
                {
                    LotKey = parsedParameters.ResultingObject.LotKey,
                    UpdateSerializationOnly = true
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncLot)]
        public IResult RemoveLotAllowance(ILotAllowanceParameters parameters)
        {
            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters;
            }

            var result = new LotAllowancesCommand(_lotUnitOfWork).Execute(parsedParameters.ResultingObject, false);
            if(!result.Success)
            {
                return result;
            }

            if(result.State != ResultState.NoWorkRequired)
            {
                _lotUnitOfWork.Commit();
            }

            return SyncParameters.Using(new SuccessResult(), new SynchronizeLotParameters
                {
                    LotKey = parsedParameters.ResultingObject.LotKey,
                    UpdateSerializationOnly = true
                });
        }

        public IResult<ILotHistoryReturn> GetLotHistory(string lotKey)
        {
            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(lotKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo<ILotHistoryReturn>();
            }

            var parsedLotKey = lotKeyResult.ResultingObject.ToLotKey();
            var result = _lotUnitOfWork.LotRepository
                .FilterByKey(parsedLotKey)
                .SplitSelect(LotProjectors.SplitSelectHistory())
                .FirstOrDefault();
            if(result == null)
            {
                return new InvalidResult<ILotHistoryReturn>(null, string.Format(UserMessages.LotNotFound, parsedLotKey));
            }

            return new SuccessResult<ILotHistoryReturn>(result);
        }

        public IResult<IEnumerable<ILotOutputTraceReturn>> GetOutputTrace(string lotKey)
        {
            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(lotKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo<IEnumerable<ILotOutputTraceReturn>>();
            }

            var traceResult = new LotTraceOutputCommand(_lotUnitOfWork).Execute(lotKeyResult.ResultingObject);
            return traceResult;
        }

        public IResult<IEnumerable<ILotInputTraceReturn>> GetInputTrace(string lotKey)
        {
            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(lotKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo<IEnumerable<ILotInputTraceReturn>>();
            }

            return new LotTraceInputCommand(_lotUnitOfWork).Execute(lotKeyResult.ResultingObject);
        }

    }
}