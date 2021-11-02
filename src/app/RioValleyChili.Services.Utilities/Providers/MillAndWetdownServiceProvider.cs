using System;
using System.Linq;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;
using RioValleyChili.Services.Interfaces.Returns.MillAndWetdownService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Conductors;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Core;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class MillAndWetdownServiceProvider : IUnitOfWorkContainer<IProductionUnitOfWork>
    {
        IProductionUnitOfWork IUnitOfWorkContainer<IProductionUnitOfWork>.UnitOfWork { get { return _productionUnitOfWork; } }
        private readonly IProductionUnitOfWork _productionUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public MillAndWetdownServiceProvider(IProductionUnitOfWork productionUnitOfWork, ITimeStamper timeStamper)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }

            _productionUnitOfWork = productionUnitOfWork;
            _timeStamper = timeStamper;
        }

        [SynchronizeOldContext(NewContextMethod.SyncMillAndWetdown)]
        public IResult<string> CreateMillAndWetdown(ICreateMillAndWetdownParameters parameters)
        {
            var parseResult = parameters.ToParsedParameters();
            if(!parseResult.Success)
            {
                return parseResult.ConvertTo<string>();
            }

            var result = new CreateMillAndWetdownConductor(_productionUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parseResult.ResultingObject);
            if(!result.Success)
            {
                return result.ConvertTo<string>();
            }

            _productionUnitOfWork.Commit();

            var lotKey = new LotKey(result.ResultingObject);
            return SyncParameters.Using(new SuccessResult<string>(lotKey), lotKey);
        }

        [SynchronizeOldContext(NewContextMethod.SyncMillAndWetdown)]
        public IResult<string> UpdateMillAndWetdown(IUpdateMillAndWetdownParameters parameters)
        {
            var parseResult = parameters.ToParsedParameters();
            if(!parseResult.Success)
            {
                return parseResult.ConvertTo<string>();
            }

            var result = new UpdateMillAndWetdownConductor(_productionUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parseResult.ResultingObject);
            if(!result.Success)
            {
                return result.ConvertTo<string>();
            }

            _productionUnitOfWork.Commit();

            var lotKey = new LotKey(result.ResultingObject);
            return SyncParameters.Using(new SuccessResult<string>(lotKey), lotKey);
        }

        [SynchronizeOldContext(NewContextMethod.DeleteLot)]
        public IResult DeleteMillAndWetdown(string lotKey)
        {
            var parseResult = KeyParserHelper.ParseResult<ILotKey>(lotKey);
            if(!parseResult.Success)
            {
                return parseResult;
            }

            var parsedLotKey = parseResult.ResultingObject.ToLotKey();
            var deleteResult = new DeleteMillAndWetdownConductor(_productionUnitOfWork).Execute(parsedLotKey);
            if(!deleteResult.Success)
            {
                return deleteResult;
            }

            _productionUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), parsedLotKey);
        }

        public IResult<IMillAndWetdownDetailReturn> GetMillAndWetdownDetail(string lotKey)
        {
            if(lotKey == null) { throw new ArgumentNullException("lotKey"); }

            var keyResult = KeyParserHelper.ParseResult<ILotKey>(lotKey);
            if(!keyResult.Success)
            {
                return keyResult.ConvertTo<IMillAndWetdownDetailReturn>();
            }

            var predicate = new LotKey(keyResult.ResultingObject).GetPredicate<ChileLotProduction>().And(c => c.ProductionType == ProductionType.MillAndWetdown);
            var select = ChileLotProductionProjectors.SelectDetail(_productionUnitOfWork);
            var entry = _productionUnitOfWork.ChileLotProductionRepository.Filter(predicate).AsExpandable().Select(select).FirstOrDefault();
            if(entry == null)
            {
                return new InvalidResult<IMillAndWetdownDetailReturn>(null, string.Format(UserMessages.MillAndWetdownEntryNotFound, lotKey));
            }

            return new SuccessResult<IMillAndWetdownDetailReturn>(entry);
        }

        public IResult<IQueryable<IMillAndWetdownSummaryReturn>> GetMillAndWetdownSummaries()
        {
            var select = ChileLotProductionProjectors.SelectSummary();
            var query = _productionUnitOfWork.ChileLotProductionRepository.All().Where(l => l.ProductionType == ProductionType.MillAndWetdown).AsExpandable().Select(select);
            return new SuccessResult<IQueryable<IMillAndWetdownSummaryReturn>>(query);
        }
    }
}