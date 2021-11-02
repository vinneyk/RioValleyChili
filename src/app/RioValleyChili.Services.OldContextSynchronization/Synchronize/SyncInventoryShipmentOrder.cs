using System;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using Solutionhead.Data;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SyncInventoryShipmentOrder)]
    public class SyncInventoryShipmentOrder : SyncCommandBase<IInventoryShipmentOrderUnitOfWork, SyncInventoryShipmentOrderParameters>
    {
        public SyncInventoryShipmentOrder(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<SyncInventoryShipmentOrderParameters> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }

            var parameters = getInput();
            var key = parameters.InventoryShipmentOrderKey;

            bool commitNewContext;
            var tblMove = Synchronize(key, parameters.New, out commitNewContext);
            if(commitNewContext)
            {
                UnitOfWork.Commit();
            }

            OldContext.SaveChanges();

            Console.Write(ConsoleOutput.SyncTblMove, tblMove.MoveNum);
        }

        private tblMove Synchronize(IKey<InventoryShipmentOrder> orderKey, bool createdNew, out bool commitNewContext)
        {
            var order = UnitOfWork.InventoryShipmentOrderRepository.FindByKey(orderKey,
                o => o.DestinationFacility,
                o => o.SourceFacility,
                o => o.InventoryPickOrder.Items.Select(i => i.Product),
                o => o.InventoryPickOrder.Items.Select(i => i.Customer.Company),
                o => o.InventoryPickOrder.Items.Select(i => i.PackagingProduct.Product),
                o => o.PickedInventory.Items.Select(i => i.PackagingProduct.Product),
                o => o.PickedInventory.Items.Select(i => i.FromLocation),
                o => o.PickedInventory.Items.Select(i => i.CurrentLocation),
                o => o.ShipmentInformation);

            var oldOrder = GetOrCreateOldContextOrder(order, createdNew, out commitNewContext);
            if(order.OrderType == InventoryShipmentOrderTypeEnum.TreatmentOrder)
            {
                var treatmentOrder = UnitOfWork.TreatmentOrderRepository.FindBy(FindTreatmentOrder(orderKey));
                oldOrder.Serialized = SerializableMove.Serialize(treatmentOrder);
            }
            else
            {
                oldOrder.Serialized = SerializableMove.Serialize(order);
            }

            oldOrder.EmployeeID = order.EmployeeId;

            oldOrder.C2WHID = order.DestinationFacility.WHID ?? OldContextHelper.GetWarehouse(order.DestinationFacility.Name).WHID;
            oldOrder.WHID = order.SourceFacility.WHID ?? OldContextHelper.GetWarehouse(order.SourceFacility.Name).WHID;
            
            SyncSetShipmentInformation.SetOrderShipment(order, oldOrder);
            SetPickOrder(order, oldOrder);
            if(!order.PickedInventory.Archived)
            {
                SetPickedInventory(order, oldOrder, ref commitNewContext);
            }

            return oldOrder;
        }

        private void SetPickedInventory(InventoryShipmentOrder newOrder, tblMove oldOrder, ref bool commitNewContext)
        {
            var MDetail = OldContext.tblMoveDetails.Any() ? OldContext.tblMoveDetails.Max(m => m.MDetail).AddSeconds(1) : DateTime.UtcNow.ConvertUTCToLocal();
            var oldDetailsToRemove = oldOrder.tblMoveDetails.ToList();

            foreach(var newItem in newOrder.PickedInventory.Items)
            {
                var oldItem = oldDetailsToRemove.FirstOrDefault(d => d.MDetail == newItem.DetailID) ?? oldDetailsToRemove.FirstOrDefault();
                if(oldItem == null)
                {
                    oldItem = new tblMoveDetail
                        {
                            MDetail = MDetail
                        };
                    OldContext.tblMoveDetails.AddObject(oldItem);
                    MDetail = MDetail.AddSeconds(1);
                }
                else
                {
                    oldDetailsToRemove.Remove(oldItem);
                }

                oldItem.MoveNum = newOrder.MoveNum.Value;
                oldItem.Lot = LotNumberParser.BuildLotNumber(newItem);
                oldItem.TTypeID = (int?) newOrder.OrderType.ToTransType();
                oldItem.PkgID = OldContextHelper.GetPackaging(newItem.PackagingProduct).PkgID;
                oldItem.Quantity = newItem.Quantity;
                oldItem.NetWgt = (decimal?) newItem.PackagingProduct.Weight;
                oldItem.TtlWgt = (decimal?) (newItem.Quantity * newItem.PackagingProduct.Weight);
                oldItem.LocID = newItem.FromLocation.LocID.Value;
                oldItem.Move2 = newItem.CurrentLocation.LocID.Value;
                oldItem.TrtmtID = OldContextHelper.GetTreatment(newItem).TrtmtID;
                oldItem.EmployeeID = newOrder.PickedInventory.EmployeeId;
                oldItem.CustProductCode = newItem.CustomerProductCode;
                oldItem.CustLot = newItem.CustomerLotCode;

                if(newItem.DetailID != oldItem.MDetail)
                {
                    commitNewContext = true;
                    newItem.DetailID = oldItem.MDetail;
                }
            }

            foreach(var o in oldDetailsToRemove)
            {
                OldContext.tblMoveDetails.DeleteObject(o);
            }
        }

        private void SetPickOrder(InventoryShipmentOrder newOrder, tblMove oldOrder)
        {
            var MOrderDetail = OldContext.tblMoveOrderDetails.Any() ? OldContext.tblMoveOrderDetails.Max(m => m.MOrderDetail).AddSeconds(1) : DateTime.UtcNow.ConvertUTCToLocal();
            var oldDetailsToRemove = oldOrder.tblMoveOrderDetails.ToList();
            
            foreach(var newItem in newOrder.InventoryPickOrder.Items)
            {
                var oldItem = oldDetailsToRemove.FirstOrDefault();
                if(oldItem == null)
                {
                    oldItem = new tblMoveOrderDetail
                        {
                            MOrderDetail = MOrderDetail
                        };
                    OldContext.tblMoveOrderDetails.AddObject(oldItem);
                    MOrderDetail = MOrderDetail.AddSeconds(1);
                }
                else
                {
                    oldDetailsToRemove.Remove(oldItem);
                }

                oldItem.MoveNum = newOrder.MoveNum.Value;
                oldItem.Quantity = newItem.Quantity;
                oldItem.ProdID = OldContextHelper.GetProduct(newItem.Product.ProductCode).ProdID;
                oldItem.PkgID = OldContextHelper.GetPackaging(newItem.PackagingProduct).PkgID;
                oldItem.NetWgt = (decimal?) newItem.PackagingProduct.Weight;
                oldItem.TtlWgt = (decimal?) (newItem.PackagingProduct.Weight * newItem.Quantity);
                oldItem.TrtmtID = OldContextHelper.GetTreatment(newItem).TrtmtID;

                oldItem.EmployeeID = null;
                oldItem.CustProductCode = newItem.CustomerProductCode;
                oldItem.CustLot = newItem.CustomerLotCode;
                oldItem.CustomerID = newItem.Customer != null ? newItem.Customer.Company.Name : null;
            }

            foreach(var o in oldDetailsToRemove)
            {
                OldContext.tblMoveOrderDetails.DeleteObject(o);
            }
        }

        private tblMove GetOrCreateOldContextOrder(InventoryShipmentOrder order, bool createdNew, out bool commitNewContext)
        {
            commitNewContext = false;

            if(createdNew || order.MoveNum == null)
            {
                commitNewContext = true;

                var tblMove = new tblMove
                    {
                        MoveNum = (order.MoveNum = GetNextMoveNum(order.DateCreated.Year)).Value,
                        TTypeID = (int) order.OrderType.ToTransType(),
                        EntryDate = DateTime.UtcNow.ConvertUTCToLocal(),
                        tblMoveOrderDetails = new EntityCollection<tblMoveOrderDetail>(),
                        tblMoveDetails = new EntityCollection<tblMoveDetail>(),
                        Status = (int?) tblOrderStatus.Scheduled
                    };
                OldContext.tblMoves.AddObject(tblMove);

                return tblMove;
            }

            return OldContext.tblMoves
                .Where(m => m.MoveNum == order.MoveNum.Value)
                .Select(m => new
                    {
                        move = m,
                        m.tblMoveOrderDetails,
                        m.tblMoveDetails
                    })
                .ToList()
                .Select(m => m.move).Single();
        }

        private int GetNextMoveNum(int year)
        {
            var startRange = year * 1000;
            var endRange = (year + 1) * 1000;

            var existing = OldContext.tblMoves
                .Where(m => m.MoveNum >= startRange && m.MoveNum < endRange)
                .Select(m => m.MoveNum).ToList();

            var next = existing.DefaultIfEmpty(startRange).Max() + 1;
            return next == endRange ? GetNextMoveNum(year + 1) : next;
        }

        private static Expression<Func<TreatmentOrder, bool>> FindTreatmentOrder(IKey<InventoryShipmentOrder> orderKey)
        {
            var keyPredicate = orderKey.FindByPredicate;
            Expression<Func<TreatmentOrder, bool>> predicate = o => keyPredicate.Invoke(o.InventoryShipmentOrder);
            return predicate.Expand();
        }
    }
}