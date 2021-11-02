using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class RemovePackScheduleConductor
    {
        private readonly Data.Interfaces.UnitsOfWork.IProductionUnitOfWork _productionUnitOfWork;

        internal RemovePackScheduleConductor(Data.Interfaces.UnitsOfWork.IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            _productionUnitOfWork = productionUnitOfWork;
        }

        internal IResult Execute(PackScheduleKey packScheduleKey, string userToken, out DateTime? packSchId)
        {
            packSchId = null;
            if(packScheduleKey == null) { throw new ArgumentNullException("packScheduleKey"); }

            var packSchedule = _productionUnitOfWork.PackScheduleRepository.FindByKey(packScheduleKey, p => p.ProductionBatches);
            if(packSchedule == null)
            {
                return new InvalidResult(string.Format(UserMessages.PackScheduleNotFound, packScheduleKey));
            }

            var removeProductionBatch = new RemoveProductionBatchConductor(_productionUnitOfWork);
            if(packSchedule.ProductionBatches != null)
            {
                foreach(var batch in packSchedule.ProductionBatches.ToList())
                {
                    var result = removeProductionBatch.Execute(new LotKey(batch));
                    if(!result.Success)
                    {
                        if(result.ResultingObject.Reason == RemoveProductionBatchConductor.Reason.ProductionBatchCompleted)
                        {
                            return result.ChangeMessage(string.Format(UserMessages.PackScheduleRemoveFail_LotCompleted, result.ResultingObject.LotKey));
                        }
                        return result;
                    }
                }
            }

            var scheduledPackSchedules = _productionUnitOfWork.ProductionScheduleItemRepository.Filter(s => s.PackScheduleDateCreated == packSchedule.DateCreated && s.PackScheduleSequence == packSchedule.SequentialNumber).ToList();
            foreach(var scheduled in scheduledPackSchedules)
            {
                _productionUnitOfWork.ProductionScheduleItemRepository.Remove(scheduled);
            }

            packSchId = packSchedule.PackSchID;
            _productionUnitOfWork.PackScheduleRepository.Remove(packSchedule);

            return new SuccessResult();
        }
    }
}