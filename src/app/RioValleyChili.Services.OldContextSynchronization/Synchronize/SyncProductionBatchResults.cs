using System;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SyncProductionBatchResults)]
    public class SyncProductionBatchResults : SyncCommandBase<IProductionUnitOfWork, LotKey>
    {
        public SyncProductionBatchResults(IProductionUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<LotKey> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }
            var lotKey = getInput();

            var productionBatch = UnitOfWork.ProductionBatchRepository.FindByKey(lotKey,
                b => b.PackSchedule.WorkType,
                b => b.Production.PickedInventory.Items.Select(i => i.PackagingProduct.Product),
                b => b.Production.PickedInventory.Items.Select(i => i.FromLocation),
                b => b.Production.PickedInventory.Items.Select(i => i.Lot.Attributes),
                b => b.Production.PickedInventory.Items.Select(i => i.Lot.PackagingLot.PackagingProduct.Product),
                b => b.Production.ResultingChileLot.Lot.Attributes.Select(a => a.AttributeName),
                b => b.Production.ResultingChileLot.ChileProduct.Ingredients.Select(i => i.AdditiveType),
                b => b.Production.Results.ProductionLineLocation,
                b => b.Production.Results.ResultItems.Select(i => i.PackagingProduct.Product),
                b => b.Production.Results.ResultItems.Select(i => i.Location));

            var lotNumber = LotNumberBuilder.BuildLotNumber(lotKey);
            var lotSelect = OldContext.tblLots
                .Where(l => l.Lot == lotNumber)
                .Select(l => new
                    {
                        Lot = l,
                        l.inputBatchItems,
                        l.tblOutgoingInputs,
                        l.tblBOMs,
                        l.tblIncomings,
                        l.tblLotAttributeHistory
                    })
                .FirstOrDefault();
            if(lotSelect == null)
            {
                throw new Exception(string.Format("Lot[{0}] not found in old context.", lotNumber));
            }

            new SyncProductionBatchPickedInventoryHelper(UnitOfWork, OldContext).SetBOMBatchItems(productionBatch, lotSelect.Lot);
            new SyncProductionResultsHelper(OldContextHelper).SetResults(productionBatch, lotSelect.Lot);

            OldContext.SaveChanges();
            Console.Write(ConsoleOutput.SyncProductionResults, lotNumber);
        }
    }
}