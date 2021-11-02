using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.PickInventoryForProductionBatch)]
    public class SyncPickInventoryForProductionBatch : SyncCommandBase<IProductionUnitOfWork, LotKey>
    {
        public SyncPickInventoryForProductionBatch(IProductionUnitOfWork unitOfWork) : base(unitOfWork) { }

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
                b => b.Production.Results,
                b => b.Production.ResultingChileLot.Lot.Attributes.Select(a => a.AttributeName),
                b => b.Production.ResultingChileLot.ChileProduct.Ingredients.Select(i => i.AdditiveType));
            if(productionBatch == null)
            {
                throw new Exception(string.Format("Could not find ProductionBatch[{0}] in new context.", lotKey.KeyValue));
            }

            var lotNumber = LotNumberParser.BuildLotNumber(productionBatch);
            var lotSelect = OldContext.tblLots
                .Where(l => l.Lot == lotNumber)
                .Select(l => new
                    {
                        Lot = l,
                        l.tblBOMs,
                        l.inputBatchItems,
                        l.tblLotAttributeHistory
                    })
                .FirstOrDefault();
            if(lotSelect == null)
            {
                throw new Exception(string.Format("Could not find tblLot[{0}] in old context.", lotNumber));
            }

            new SyncProductionBatchPickedInventoryHelper(UnitOfWork, OldContext).SetBOMBatchItems(productionBatch, lotSelect.Lot);

            OldContext.SaveChanges();

            Console.WriteLine(ConsoleOutput.SetPickedItemsForLot, lotNumber);
        }
    }
}