using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService;
using RioValleyChili.Services.Interfaces.Returns.MaterialsReceivedService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Conductors;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Core;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class MaterialsServiceProvider : IUnitOfWorkContainer<IMaterialsReceivedUnitOfWork>
    {
        IMaterialsReceivedUnitOfWork IUnitOfWorkContainer<IMaterialsReceivedUnitOfWork>.UnitOfWork { get { return _materialsReceivedUnitOfWork; } }
        private readonly IMaterialsReceivedUnitOfWork _materialsReceivedUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public MaterialsServiceProvider(IMaterialsReceivedUnitOfWork materialsReceivedUnitOfWork, ITimeStamper timeStamper)
        {
            if(materialsReceivedUnitOfWork == null) { throw new ArgumentNullException("materialsReceivedUnitOfWork"); }
            _materialsReceivedUnitOfWork = materialsReceivedUnitOfWork;

            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }
            _timeStamper = timeStamper;
        }

        [SynchronizeOldContext(NewContextMethod.SyncChileMaterialsReceived)]
        public IResult<string> CreateChileMaterialsReceived(ICreateChileMaterialsReceivedParameters parameters)
        {
            var parseResult = parameters.ToParsedParameters();
            if(!parseResult.Success)
            {
                return parseResult.ConvertTo<string>();
            }
            
            var result = new CreateChileMaterialsReceivedConductor(_materialsReceivedUnitOfWork).Execute(parseResult.ResultingObject, _timeStamper.CurrentTimeStamp);
            if(!result.Success)
            {
                return result.ConvertTo<string>();
            }
            
            _materialsReceivedUnitOfWork.Commit();
            
            var lotKey = result.ResultingObject.ToLotKey();
            return SyncParameters.Using(new SuccessResult<string>(lotKey), lotKey);
        }

        [SynchronizeOldContext(NewContextMethod.SyncChileMaterialsReceived)]
        public IResult<string> UpdateChileMaterialsReceived(IUpdateChileMaterialsReceivedParameters parameters)
        {
            var parseResult = parameters.ToParsedParameters();
            if(!parseResult.Success)
            {
                return parseResult.ConvertTo<string>();
            }

            var result = new UpdateChileMaterialsReceivedConductor(_materialsReceivedUnitOfWork).Execute(parseResult.ResultingObject, _timeStamper.CurrentTimeStamp);
            if(!result.Success)
            {
                return result.ConvertTo<string>();
            }

            _materialsReceivedUnitOfWork.Commit();

            var key = result.ResultingObject.ToLotKey();
            return SyncParameters.Using(new SuccessResult<string>(key), key);
        }

        public IResult<IQueryable<IChileMaterialsReceivedSummaryReturn>> GetChileMaterialsReceivedSummaries(ChileMaterialsReceivedFilters filters)
        {
            var filterParametersResult = filters.ToParsedParameters();
            if(!filterParametersResult.Success)
            {
                return filterParametersResult.ConvertTo<IQueryable<IChileMaterialsReceivedSummaryReturn>>();
            }

            var predicateResult = ChileMaterialsReceivedPredicateBuilder.BuildPredicate(filterParametersResult.ResultingObject);
            if(!predicateResult.Success)
            {
                return predicateResult.ConvertTo<IQueryable<IChileMaterialsReceivedSummaryReturn>>();
            }

            var results = _materialsReceivedUnitOfWork.ChileMaterialsReceivedRepository
                .Filter(predicateResult.ResultingObject)
                .Select(ChileMaterialsReceivedProjectors.SelectSummary());
            return new SuccessResult<IQueryable<IChileMaterialsReceivedSummaryReturn>>(results);
        }

        public IResult<IChileMaterialsReceivedDetailReturn> GetChileMaterialsReceivedDetail(string lotKey)
        {
            if(lotKey == null) { throw new ArgumentNullException("lotKey"); }

            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(lotKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo<IChileMaterialsReceivedDetailReturn>();
            }

            var predicate = lotKeyResult.ResultingObject.ToLotKey().GetPredicate<ChileMaterialsReceived>();
            var select = ChileMaterialsReceivedProjectors.SelectDetail();
            
            var detail = _materialsReceivedUnitOfWork.ChileMaterialsReceivedRepository.Filter(predicate).AsExpandable().Select(select).FirstOrDefault();
            if(detail == null)
            {
                return new InvalidResult<IChileMaterialsReceivedDetailReturn>(null, string.Format(UserMessages.ChileMaterialsReceivedNotFound, lotKey));
            }

            return new SuccessResult<IChileMaterialsReceivedDetailReturn>(detail);
        }

        public IResult<IEnumerable<string>> GetChileVarieties()
        {
            var varieties = _materialsReceivedUnitOfWork.ChileMaterialsReceivedItemRepository.All().Select(i => i.ChileVariety).Distinct().ToList();
            return new SuccessResult<IEnumerable<string>>(varieties);
        }

        public IResult<IChileMaterialsReceivedRecapReturn> GetRecapReport(string lotKey)
        {
            var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(lotKey);
            if(!lotKeyResult.Success)
            {
                return lotKeyResult.ConvertTo<IChileMaterialsReceivedRecapReturn>();
            }
;
            var result = _materialsReceivedUnitOfWork.ChileMaterialsReceivedRepository
                .FilterByKey(lotKeyResult.ResultingObject.ToLotKey())
                .Select(ChileMaterialsReceivedProjectors.SelectRecapReport())
                .FirstOrDefault();
            if(result == null)
            {
                return new InvalidResult<IChileMaterialsReceivedRecapReturn>(null, string.Format(UserMessages.ChileMaterialsReceivedNotFound, lotKey));
            }

            return new SuccessResult<IChileMaterialsReceivedRecapReturn>(result);
        }
    }
}