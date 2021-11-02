using System;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.UpdateProductionBatch)]
    public class SyncUpdateProductionBatch : SyncCommandBase<IProductionUnitOfWork, LotKey>
    {
        public SyncUpdateProductionBatch(IProductionUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<LotKey> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }
            var productionBatchKey = getInput();

            var productionBatch = UnitOfWork.ProductionBatchRepository.FindByKey(productionBatchKey,
                b => b.Production.Results,
                b => b.Production.ResultingChileLot.Lot,
                b => b.PackSchedule.PackagingProduct.Product,
                b => b.PackSchedule.ChileProduct.Product,
                b => b.PackSchedule.ProductionLineLocation,
                b => b.PackSchedule.ProductionBatches.Select(p => p.Production.PickedInventory));
            if(productionBatch == null)
            {
                throw new Exception(string.Format("ProductionBatch[{0}] not found.", productionBatchKey));
            }

            var lotNumber = LotNumberBuilder.BuildLotNumber(productionBatch);
            var lot = OldContext.tblLots.FirstOrDefault(l => l.Lot == lotNumber);
            if(lot == null)
            {
                throw new Exception(string.Format("tblLot[{0}] not found.", lotNumber));
            }
            UpdateLot(lot, productionBatch);

            var oldPackSchedule = OldContext.tblPackSches.FirstOrDefault(p => p.PackSchID == productionBatch.PackSchedule.PackSchID);
            if(oldPackSchedule == null)
            {
                throw new Exception(string.Format("tblPackSch[{0}] could not be found.", productionBatch.PackSchedule.PackSchID));
            }
            oldPackSchedule.Serialized = SerializablePackSchedule.Serialize(productionBatch.PackSchedule);

            var serialized = new SerializedData(OldContext);
            serialized[SerializableType.ChileLotProduction, lotNumber] = new SerializableEmployeeIdentifiable(productionBatch.Production);
            if(productionBatch.Production.Results != null)
            {
                serialized[SerializableType.LotProductionResults, lotNumber] = new SerializableEmployeeIdentifiable(productionBatch.Production.Results);
            }

            OldContext.SaveChanges();

            Console.Write(ConsoleOutput.UpdatedLot, lot.Lot);
        }

        private static void UpdateLot(tblLot lot, ProductionBatch productionBatch)
        {
            lot.TargetWgt = (decimal?) productionBatch.TargetParameters.BatchTargetWeight;
            lot.TgtAsta = (decimal?) productionBatch.TargetParameters.BatchTargetAsta;
            lot.TgtScan = (decimal?) productionBatch.TargetParameters.BatchTargetScan;
            lot.TgtScov = (decimal?) productionBatch.TargetParameters.BatchTargetScoville;
            lot.Notes = productionBatch.Production.ResultingChileLot.Lot.Notes;
        }
    }
}