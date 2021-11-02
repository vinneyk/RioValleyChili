using System;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.CreateProductionBatch)]
    public class SyncCreateProductionBatch : SyncCommandBase<IProductionUnitOfWork, LotKey>
    {
        public SyncCreateProductionBatch(IProductionUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<LotKey> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }
            var productionBatchKey = getInput();

            var productionBatch = UnitOfWork.ProductionBatchRepository.FindByKey(productionBatchKey,
                b => b.Production.ResultingChileLot.Lot,
                b => b.PackSchedule.WorkType,
                b => b.PackSchedule.PackagingProduct.Product,
                b => b.PackSchedule.ChileProduct.Product,
                b => b.PackSchedule.ProductionLineLocation,
                b => b.PackSchedule.Customer.Company,
                b => b.PackSchedule.ProductionBatches.Select(p => p.Production.PickedInventory));
            if(productionBatch == null)
            {
                throw new Exception(string.Format("ProductionBatch[{0}] not found.", productionBatchKey));
            }

            var lotNumber = LotNumberBuilder.BuildLotNumber(productionBatch);
            var lot = OldContext.tblLots.FirstOrDefault(l => l.Lot == lotNumber);
            if(lot != null)
            {
                throw new Exception(string.Format("tblLot[{0}] already exists.", lotNumber));
            }

            OldContext.tblLots.AddObject(CreateNewLot(lotNumber, productionBatch));

            new SyncTblBatchInstr(OldContext).Synchronize(productionBatch.InstructionNotebook, lotNumber);

            var oldPackSchedule = OldContext.tblPackSches.FirstOrDefault(p => p.PackSchID == productionBatch.PackSchedule.PackSchID);
            if(oldPackSchedule == null)
            {
                throw new Exception(string.Format("tblPackSch[{0}] could not be found.", productionBatch.PackSchedule.PackSchID));
            }
            oldPackSchedule.Serialized = SerializablePackSchedule.Serialize(productionBatch.PackSchedule);

            var serialized = new SerializedData(OldContext);
            serialized[SerializableType.ChileLotProduction, lotNumber] = new SerializableEmployeeIdentifiable(productionBatch.Production);

            OldContext.SaveChanges();
            
            Console.Write(ConsoleOutput.AddedLot, lotNumber);
        }

        private tblLot CreateNewLot(LotNumberResult lotNumber, ProductionBatch productionBatch)
        {
            var chileProduct = OldContextHelper.GetProduct(productionBatch.PackSchedule.ChileProduct);
            var productionLine = OldContextHelper.GetProductionLine(productionBatch.PackSchedule.ProductionLineLocation);
            var batchTypeId = BatchTypeIDHelper.GetBatchTypeID(productionBatch.PackSchedule.WorkType.Description);

            return new tblLot
                {
                    Lot = lotNumber,
                    EmployeeID = productionBatch.Production.ResultingChileLot.Lot.EmployeeId,
                    EntryDate = productionBatch.Production.ResultingChileLot.Lot.TimeStamp.ConvertUTCToLocal().RoundMillisecondsForSQL(),

                    Notes = productionBatch.Production.ResultingChileLot.Lot.Notes,
                    
                    PTypeID = productionBatch.LotTypeId,
                    Julian = lotNumber.Julian,
                    BatchNum = productionBatch.LotDateSequence,
                    ProdID = chileProduct.ProdID,

                    PackSchID = productionBatch.PackSchedule.PackSchID,
                    BatchTypeID = (int?) batchTypeId,
                    Company_IA = productionBatch.PackSchedule.Customer == null ? null : productionBatch.PackSchedule.Customer.Company.Name,

                    ProductionDate = productionBatch.PackSchedule.ScheduledProductionDate,
                    ProductionLine = productionLine,
                    BatchProdctnOrder = 0, //Seems constant in old context - RI 2014/4/16

                    SetTrtmtID = 0,
                    LotStat = 0,
                    BatchStatID = (int?) BatchStatID.Scheduled,
                    TargetWgt = (decimal?) productionBatch.TargetParameters.BatchTargetWeight,
                    TgtAsta = (decimal?) productionBatch.TargetParameters.BatchTargetAsta,
                    TgtScan = (decimal?) productionBatch.TargetParameters.BatchTargetScan,
                    TgtScov = (decimal?) productionBatch.TargetParameters.BatchTargetScoville,
                };
        }
    }
}