using System;
using System.Linq;
using EF_Split_Projector;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Interfaces.Parameters.IntraWarehouseOrderService;
using RioValleyChili.Services.Interfaces.Returns.IntraWarehouseOrderService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Conductors;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Core;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class IntraWarehouseOrderServiceProvider : IUnitOfWorkContainer<IIntraWarehouseOrderUnitOfWork>
    {
        #region Fields and Constructors.

        IIntraWarehouseOrderUnitOfWork IUnitOfWorkContainer<IIntraWarehouseOrderUnitOfWork>.UnitOfWork { get { return _intraWarehouseOrderUnitOfWork; } }
        private readonly IIntraWarehouseOrderUnitOfWork _intraWarehouseOrderUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public IntraWarehouseOrderServiceProvider(IIntraWarehouseOrderUnitOfWork intraWarehouseOrderUnitOfWork, ITimeStamper timeStamper)
        {
            if(intraWarehouseOrderUnitOfWork == null) { throw new ArgumentNullException("intraWarehouseOrderUnitOfWork"); }
            _intraWarehouseOrderUnitOfWork = intraWarehouseOrderUnitOfWork;

            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }
            _timeStamper = timeStamper;
        }

        #endregion

        [SynchronizeOldContext(NewContextMethod.SyncIntraWarehouseOrder)]
        public IResult<string> CreateIntraWarehouseOrder(ICreateIntraWarehouseOrderParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var createResult = new IntraWarehouseOrderConductor(_intraWarehouseOrderUnitOfWork).Create(_timeStamper.CurrentTimeStamp, parameters);
            if(!createResult.Success)
            {
                return createResult.ConvertTo<string>();
            }

            _intraWarehouseOrderUnitOfWork.Commit();

            var key = createResult.ResultingObject.ToIntraWarehouseOrderKey();
            return SyncParameters.Using(new SuccessResult<string>(key), key);
        }

        [SynchronizeOldContext(NewContextMethod.SyncIntraWarehouseOrder)]
        public IResult UpdateIntraWarehouseOrder(IUpdateIntraWarehouseOrderParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var keyResult = KeyParserHelper.ParseResult<IIntraWarehouseOrderKey>(parameters.IntraWarehouseOrderKey);
            if(!keyResult.Success)
            {
                return keyResult;
            }

            var updateResult = new IntraWarehouseOrderConductor(_intraWarehouseOrderUnitOfWork).Update(_timeStamper.CurrentTimeStamp, keyResult.ResultingObject, parameters);
            if(!updateResult.Success)
            {
                return updateResult;
            }

            _intraWarehouseOrderUnitOfWork.Commit();

            var key = keyResult.ResultingObject.ToIntraWarehouseOrderKey();
            return SyncParameters.Using(new SuccessResult<string>(key), key);
        }

        public IResult<IQueryable<IIntraWarehouseOrderSummaryReturn>> GetIntraWarehouseOrderSummaries()
        {
            var selector = IntraWarehouseOrderProjectors.SelectSummary();
            var query = _intraWarehouseOrderUnitOfWork.IntraWarehouseOrderRepository.All().AsExpandable().Select(selector);
            return new SuccessResult<IQueryable<IntraWarehouseOrderReturn>>(query);
        }

        public IResult<IIntraWarehouseOrderDetailReturn> GetIntraWarehouseOrder(string orderKey)
        {
            if(orderKey == null) { throw new ArgumentNullException("orderKey"); }

            var keyResult = KeyParserHelper.ParseResult<IIntraWarehouseOrderKey>(orderKey);
            if(!keyResult.Success)
            {
                return keyResult.ConvertTo((IIntraWarehouseOrderDetailReturn) null);
            }
            var predicate = new IntraWarehouseOrderKey(keyResult.ResultingObject).FindByPredicate;
            var selector = IntraWarehouseOrderProjectors.SplitSelectDetail(_intraWarehouseOrderUnitOfWork, _timeStamper.CurrentTimeStamp.Date);
            
            var order = _intraWarehouseOrderUnitOfWork.IntraWarehouseOrderRepository.Filter(predicate).SplitSelect(selector).FirstOrDefault();
            if(order == null)
            {
                return new FailureResult<IIntraWarehouseOrderDetailReturn>(null, string.Format(UserMessages.IntraWarehouseOrderNotFound, orderKey));
            }

            return new SuccessResult<IIntraWarehouseOrderDetailReturn>(order);
        }

        public IResult<IQueryable<IIntraWarehouseOrderDetailReturn>> GetIntraWarehouseOrders()
        {
            var select = IntraWarehouseOrderProjectors.SplitSelectDetail(_intraWarehouseOrderUnitOfWork, _timeStamper.CurrentTimeStamp.Date);
            var orders = _intraWarehouseOrderUnitOfWork.IntraWarehouseOrderRepository.All().SplitSelect(select);
            return new SuccessResult<IQueryable<IIntraWarehouseOrderDetailReturn>>(orders);
        }
    }
}