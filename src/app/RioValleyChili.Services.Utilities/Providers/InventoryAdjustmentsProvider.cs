using System;
using System.Linq;
using EF_Split_Projector;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.InventoryAdjustmentsService;
using RioValleyChili.Services.Interfaces.Returns.InventoryAdjustmentsService;
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
    public class InventoryAdjustmentsProvider : IUnitOfWorkContainer<IInventoryUnitOfWork>
    {
        #region fields and constructors
        IInventoryUnitOfWork IUnitOfWorkContainer<IInventoryUnitOfWork>.UnitOfWork { get { return _inventoryUnitOfWork; } }
        private readonly IInventoryUnitOfWork _inventoryUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public InventoryAdjustmentsProvider(IInventoryUnitOfWork inventoryUnitOfWork, ITimeStamper timeStamper)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }
            _inventoryUnitOfWork = inventoryUnitOfWork;

            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }
            _timeStamper = timeStamper;
        }

        #endregion

        [SynchronizeOldContext(NewContextMethod.CreateInventoryAdjustment)]
        public IResult<string> CreateInventoryAdjustment(ICreateInventoryAdjustmentParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parseResult = parameters.ToParsedParameters();
            if(!parseResult.Success)
            {
                return parseResult.ConvertTo<string>();
            }

            var createResult = new CreateInventoryAdjustmentConductor(_inventoryUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parseResult.ResultingObject);
            if(!createResult.Success)
            {
                return createResult.ConvertTo<string>();
            }

            _inventoryUnitOfWork.Commit();

            var key = new InventoryAdjustmentKey(createResult.ResultingObject);
            return SyncParameters.Using(new SuccessResult<string>(key), key);
        }

        public IResult<IQueryable<IInventoryAdjustmentReturn>> GetInventoryAdjustments(FilterInventoryAdjustmentParameters parameters)
        {
            var parseResult = parameters.ParseToPredicateBuilderFilters();
            if(!parseResult.Success)
            {
                return parseResult.ConvertTo<IQueryable<IInventoryAdjustmentReturn>>();
            }

            var predicateResult = InventoryAdjustmentPredicateBuilder.BuildPredicate(parseResult.ResultingObject);
            if(!predicateResult.Success)
            {
                return predicateResult.ConvertTo<IQueryable<IInventoryAdjustmentReturn>>();
            }
            var results = _inventoryUnitOfWork.InventoryAdjustmentRepository
                .Filter(predicateResult.ResultingObject)
                .SplitSelect(InventoryAdjustmentProjectors.SplitSelect(_inventoryUnitOfWork));

            return new SuccessResult<IQueryable<IInventoryAdjustmentReturn>>(results);
        }

        public IResult<IInventoryAdjustmentReturn> GetInventoryAdjustment(string inventoryAdjustmentKey)
        {
            if(inventoryAdjustmentKey == null) { throw new ArgumentNullException("inventoryAdjustmentKey"); }

            var adjustmentKeyResult = KeyParserHelper.ParseResult<IInventoryAdjustmentKey>(inventoryAdjustmentKey);
            if(!adjustmentKeyResult.Success)
            {
                return adjustmentKeyResult.ConvertTo<IInventoryAdjustmentReturn>();
            }
            var predicate = new InventoryAdjustmentKey(adjustmentKeyResult.ResultingObject).FindByPredicate;

            var select = InventoryAdjustmentProjectors.SplitSelect(_inventoryUnitOfWork);
            var inventoryAdjustment = _inventoryUnitOfWork.InventoryAdjustmentRepository
                .Filter(predicate)
                .SplitSelect(select)
                .ToList().FirstOrDefault();
            if(inventoryAdjustment == null)
            {
                return new InvalidResult<IInventoryAdjustmentReturn>(null, string.Format(UserMessages.InventoryAdjustmentNotFound, inventoryAdjustmentKey));
            }

            return new SuccessResult<IInventoryAdjustmentReturn>(inventoryAdjustment);
        }
    }
}