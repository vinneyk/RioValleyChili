using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.CreateInventoryAdjustment)]
    public class SyncCreateInventoryAdjustment : SyncCommandBase<IInventoryUnitOfWork, InventoryAdjustmentKey>
    {
        public SyncCreateInventoryAdjustment(IInventoryUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<InventoryAdjustmentKey> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }
            var key = getInput();
            var inventoryAdjustment = UnitOfWork.InventoryAdjustmentRepository.FindByKey(key,
                a => a.Notebook.Notes,
                a => a.Items.Select(i => i.PackagingProduct.Product),
                a => a.Items.Select(i => i.Location),
                a => a.Items.Select(i => i.Treatment),
                a => a.Items.Select(i => i.Lot.PackagingLot.PackagingProduct.Product));
            
            var adjustId = inventoryAdjustment.TimeStamp.ConvertUTCToLocal().RoundMillisecondsForSQL();
            var newAdjustment = new tblAdjust
                {
                    AdjustID = adjustId,
                    EmployeeID = inventoryAdjustment.EmployeeId,
                    Reason = inventoryAdjustment.Notebook.Notes.Select(n => n.Text).FirstOrDefault()
                };

            AddOutgoingRecords(newAdjustment, inventoryAdjustment.Items);
            
            OldContext.tblAdjusts.AddObject(newAdjustment);
            OldContext.SaveChanges();
            Console.WriteLine(ConsoleOutput.AddedAdjust, newAdjustment.AdjustID.ToString(DateTimeExtensions.SQLDateTimeFormat));
        }

        private void AddOutgoingRecords(tblAdjust adjustment, IEnumerable<InventoryAdjustmentItem> items)
        {
            foreach(var item in items)
            {
                var lotNumber = LotNumberBuilder.BuildLotNumber(item);
                var location = OldContextHelper.GetLocation(item.Location);
                var treatment = OldContextHelper.GetTreatment(item.Treatment);

                var packaging = item.PackagingProduct;
                if(item.Lot.PackagingLot != null)
                {
                    packaging = item.Lot.PackagingLot.PackagingProduct;
                }
                var tblPackaging = OldContextHelper.GetPackaging(packaging);

                var outgoing = new tblOutgoing
                    {
                        AdjustID = adjustment.AdjustID,
                        EmployeeID = adjustment.EmployeeID,
                        EntryDate = DateTime.UtcNow.ConvertUTCToLocal().RoundMillisecondsForSQL(),

                        Lot = lotNumber,
                        TTypeID = (int?) TransType.InvAdj,
                        PkgID = tblPackaging.PkgID,
                        Tote = item.ToteKey,
                        Quantity = -item.QuantityAdjustment,
                        NetWgt = tblPackaging.NetWgt,
                        TtlWgt = -item.QuantityAdjustment * tblPackaging.NetWgt,
                        LocID = location.LocID,
                        TrtmtID = treatment.TrtmtID
                    };
                adjustment.tblOutgoings.Add(outgoing);
            }
        }
    }
}