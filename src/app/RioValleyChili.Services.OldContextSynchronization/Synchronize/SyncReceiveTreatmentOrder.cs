using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.ReceiveTreatmentOrder)]
    public class SyncReceiveTreatmentOrder : SyncCommandBase<ITreatmentOrderUnitOfWork, TreatmentOrderKey>
    {
        public SyncReceiveTreatmentOrder(ITreatmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<TreatmentOrderKey> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }
            var treatmentOrderKey = getInput();

            var treatmentOrder = UnitOfWork.TreatmentOrderRepository.FindByKey(treatmentOrderKey, o => o.InventoryShipmentOrder.PickedInventory.Items.Select(i => i.CurrentLocation));
            if(treatmentOrder == null)
            {
                throw new Exception(string.Format("Could not find TreatmentOrder[{0}].", treatmentOrderKey));
            }

            var oldOrder = OldContext.tblMoves.Where(m => m.MoveNum == treatmentOrder.InventoryShipmentOrder.MoveNum)
                .Select(m => new
                    {
                        tblMove = m,
                        details = m.tblMoveDetails.Select(d => d.tblPackaging)
                    }).FirstOrDefault();
            if(oldOrder == null)
            {
                throw new Exception(string.Format("Could not find tblMove[{0}].", treatmentOrder.InventoryShipmentOrder.MoveNum));
            }

            var oldTreatment = OldContextHelper.GetTreatment(treatmentOrder);
            var firstItem = treatmentOrder.InventoryShipmentOrder.PickedInventory.Items.FirstOrDefault();
            if(firstItem == null)
            {
                throw new Exception("No PickedInventoryItems in TreatmentOrder.");
            }

            foreach(var oldDetail in oldOrder.tblMove.tblMoveDetails)
            {
                OldContext.tblOutgoings.AddObject(CreateOutgoing(treatmentOrder, oldDetail));

                oldDetail.TrtmtID = oldTreatment.TrtmtID;
                oldDetail.LocID = firstItem.CurrentLocation.LocID.Value;
                OldContext.tblOutgoings.AddObject(CreateOutgoing(treatmentOrder, oldDetail, -1));
            }

            oldOrder.tblMove.Returned = treatmentOrder.Returned.ConvertUTCToLocal();
            oldOrder.tblMove.Status = (int?) tblOrderStatus.Treated;

            OldContext.SaveChanges();

            Console.Write(ConsoleOutput.ReceivedTreatmentOrder, oldOrder.tblMove.MoveNum);
        }

        private static tblOutgoing CreateOutgoing(TreatmentOrder order, tblMoveDetail oldDetail, int quantitySign = 1)
        {
            var quantity = oldDetail.Quantity * quantitySign;

            var result = new tblOutgoing
                {
                    EntryDate = DateTime.UtcNow.ConvertUTCToLocal().RoundMillisecondsForSQL(),
                    Lot = oldDetail.Lot,
                    TTypeID = (int?)TransType.FrmTrmt,
                    PkgID = oldDetail.PkgID,
                    Tote = null,
                    Quantity = quantity,
                    NetWgt = oldDetail.tblPackaging.NetWgt,
                    TtlWgt = quantity * oldDetail.tblPackaging.NetWgt,
                    LocID = oldDetail.LocID,
                    TrtmtID = oldDetail.TrtmtID,
                    EmployeeID = order.InventoryShipmentOrder.PickedInventory.EmployeeId,
                    BOMID = 0,
                    BatchLot = 0,
                    MoveNum = order.InventoryShipmentOrder.MoveNum,
                    MDetail = oldDetail.MDetail,
                    CustProductCode = oldDetail.CustProductCode
                };
            return result;
        }
    }
}