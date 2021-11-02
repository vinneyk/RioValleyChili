using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.Post)]
    public class SyncPost : SyncCommandBase<IInventoryShipmentOrderUnitOfWork, InventoryShipmentOrderKey>
    {
        public SyncPost(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<InventoryShipmentOrderKey> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }

            var key = getInput();
            var order = UnitOfWork.InventoryShipmentOrderRepository.FindByKey(key,
                o => o.PickedInventory.Items.Select(i => i.PackagingProduct.Product),
                o => o.PickedInventory.Items.Select(i => i.FromLocation),
                o => o.PickedInventory.Items.Select(i => i.CurrentLocation));
            if(order == null)
            {
                throw new Exception(string.Format("Could not find InventoryShipmentOrder[{0}].", key));
            }

            if(order.MoveNum == null)
            {
                throw new Exception(string.Format("InventoryShipmentOrder[{0}].MoveNum is null.", key));
            }

            switch(order.OrderType)
            {
                case InventoryShipmentOrderTypeEnum.InterWarehouseOrder:
                case InventoryShipmentOrderTypeEnum.TreatmentOrder:
                    SyncMove(order);
                    break;

                case InventoryShipmentOrderTypeEnum.SalesOrder:
                case InventoryShipmentOrderTypeEnum.MiscellaneousOrder:
                    SyncOrder(order);
                    break;
            }

            OldContext.SaveChanges();

            Console.WriteLine(ConsoleOutput.PostedOrder, order.MoveNum);
        }

        [Issue("There was an issue with mismatched Quantity and TtlWgt signs caused by calculating TtlWgt from item.Quantity instead of supplied quantity." +
               "Modified to use supplied quantity, so that should fix it. -RI 2016-09-19",
               References = new[] { "RVCADMIN-1299" })]
        private tblOutgoing CreateOutgoing(InventoryShipmentOrder order, PickedInventoryItem item, Location location, int quantity, bool forMove)
        {
            return new tblOutgoing
                {
                    EntryDate = DateTime.UtcNow.ConvertUTCToLocal().RoundMillisecondsForSQL(),
                    Lot = LotNumberParser.BuildLotNumber(item),
                    TTypeID = (int?)order.OrderType.ToTransType(),
                    PkgID = OldContextHelper.GetPackaging(item.PackagingProduct).PkgID,
                    Tote = item.ToteKey,
                    Quantity = quantity,
                    NetWgt = (decimal?)item.PackagingProduct.Weight,
                    TtlWgt = (decimal?)(quantity * item.PackagingProduct.Weight),
                    LocID = (int)location.LocID,
                    TrtmtID = OldContextHelper.GetTreatment(item).TrtmtID,
                    EmployeeID = order.EmployeeId,
                    BOMID = 0,
                    BatchLot = 0,
                    MoveNum = forMove ? order.MoveNum : null,
                    MDetail = forMove ? item.DetailID : null,
                    OrderNum = !forMove ? order.MoveNum : null,
                    ODetail = !forMove ? item.DetailID : null,
                    CustProductCode = item.CustomerProductCode
                };
        }

        private void SyncMove(InventoryShipmentOrder order)
        {
            var tblMove = OldContext.tblMoves
                .Where(m => m.MoveNum == order.MoveNum)
                .Select(o => new
                    {
                        order = o,
                        details = o.tblMoveDetails
                    })
                .FirstOrDefault();
            if(tblMove == null)
            {
                throw new Exception(string.Format("Could not find tblMove[{0}]", order.MoveNum));
            }

            var tblMoveDetails = tblMove.details.ToDictionary(d => d.MDetail);
            foreach(var item in order.PickedInventory.Items)
            {
                if(item.DetailID == null)
                {
                    throw new Exception(string.Format("Missing DetailID on PicedInventoryItem for order with MoveNum[{0}]", order.MoveNum));
                }

                var detail = tblMoveDetails[item.DetailID.Value];
                detail.LocID = item.CurrentLocation.LocID.Value;
                detail.Move2 = null;

                OldContext.tblOutgoings.AddObject(CreateOutgoing(order, item, item.FromLocation, item.Quantity, true));
                OldContext.tblOutgoings.AddObject(CreateOutgoing(order, item, item.CurrentLocation, -item.Quantity, true));
            }

            tblMove.order.Status = (int?) tblOrderStatus.Shipped;
        }

        private void SyncOrder(InventoryShipmentOrder order)
        {
            var tblOrder = OldContext.tblOrders
                .Where(o => o.OrderNum == order.MoveNum)
                .Select(o => new
                    {
                        order = o,
                        details = o.tblOrderDetails.Select(d => new
                            {
                                d,
                                staged = d.tblStagedFGs
                            })
                    })
                .FirstOrDefault();
            if(tblOrder == null)
            {
                throw new Exception(string.Format("Could not find tblOrder[{0}]", order.MoveNum));
            }

            var tblStagedFGs = tblOrder.order.tblOrderDetails.SelectMany(d => d.tblStagedFGs)
                .ToDictionary(f => f.EntryDate);
            foreach(var item in order.PickedInventory.Items)
            {
                if(item.DetailID == null)
                {
                    throw new Exception(string.Format("Missing DetailID on PickedInventoryItem for order with MoveNum[{0}]", order.MoveNum));
                }

                var detail = tblStagedFGs[item.DetailID.Value];
                detail.LocID = item.CurrentLocation.LocID.Value;

                OldContext.tblOutgoings.AddObject(CreateOutgoing(order, item, item.FromLocation, item.Quantity, false));
            }

            tblOrder.order.Status = (int?) tblOrderStatus.Shipped;
        }
    }
}