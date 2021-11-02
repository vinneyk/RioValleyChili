using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Utilities;
using RioValleyChili.Business.Core.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SyncIntraWarehouseOrder)]
    public class SyncIntraWarehouseOrder : SyncCommandBase<IIntraWarehouseOrderUnitOfWork, IntraWarehouseOrderKey>
    {
        public const string DateTimeFormat = "yyyyMMdd hh:mm:ss.fff tt";

        public SyncIntraWarehouseOrder(IIntraWarehouseOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<IntraWarehouseOrderKey> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }

            var orderKey = getInput();
            var order = UnitOfWork.IntraWarehouseOrderRepository.FindByKey(orderKey,
                o => o.PickedInventory.Items.Select(i => i.PackagingProduct.Product),
                o => o.PickedInventory.Items.Select(i => i.FromLocation),
                o => o.PickedInventory.Items.Select(i => i.CurrentLocation));
            if(order == null)
            {
                throw new Exception(string.Format("IntraWarehouseOrder[{0}] not found in new context.", orderKey));
            }

            bool createdNew;
            var oldOrder = Synchronize(order, out createdNew);
            if(createdNew)
            {
                UnitOfWork.Commit();
            }
            OldContext.SaveChanges();

            Console.Write(ConsoleOutput.SynchronizedIntraWarehouserMovement, oldOrder.RinconID.ToString(DateTimeFormat));
        }

        private tblRincon Synchronize(IntraWarehouseOrder order, out bool createdNew)
        {
            createdNew = false;
            tblRincon oldOrder;
            if(order.RinconID == null)
            {
                createdNew = true;
                oldOrder = new tblRincon
                    {
                        RinconID = OldContext.tblRincons.GetNextUnique(DateTime.UtcNow.ConvertUTCToLocal(), r => r.RinconID)
                    };
                OldContext.tblRincons.AddObject(oldOrder);
                order.RinconID = oldOrder.RinconID;
            }
            else
            {
                var oldOrderSelect = OldContext.tblRincons
                    .Select(r => new
                        {
                            rincon = r,
                            details = r.tblRinconDetails
                        })
                    .FirstOrDefault(r => r.rincon.RinconID == order.RinconID.Value);
                if(oldOrderSelect == null)
                {
                    throw new Exception(string.Format("tblRincon[{0}] not found in old context.", order.RinconID));
                }
                oldOrder = oldOrderSelect.rincon;
            }

            oldOrder.MoveDate = order.MovementDate.Date;
            oldOrder.SheetNum = order.TrackingSheetNumber;
            oldOrder.PrepBy = order.OperatorName;
            oldOrder.Updated = order.TimeStamp.ConvertUTCToLocal();
            oldOrder.EmployeeID = order.EmployeeId;

            if(createdNew)
            {
                SynchronizeItems(order, oldOrder);
            }

            return oldOrder;
        }

        private void SynchronizeItems(IntraWarehouseOrder order, tblRincon oldOrder)
        {
            var currentDetails = order.PickedInventory.Items.Select(i => new RinconDetail(i, oldOrder, OldContextHelper)).ToList();

            SynchronizeItems(currentDetails.Select(d => d.Detail), oldOrder.tblRinconDetails, OldContext.tblRinconDetails, DetailMatchPredicates,
                n => n.RDetailID = OldContext.tblRinconDetails.GetNextUnique(oldOrder.RinconID, d => d.RDetailID),
                (o, n) =>
                    {
                        o.EmployeeID = n.EmployeeID;
                        o.Lot = n.Lot;
                        o.PkgID = n.PkgID;
                        o.TrtmtID = n.TrtmtID;
                        o.Quantity = n.Quantity;
                        o.CurrLocID = n.CurrLocID;
                        o.DestLocID = n.DestLocID;
                    });

            SynchronizeItems(currentDetails.SelectMany(d => d.ToOutgoingRecords()), oldOrder.tblOutgoings, OldContext.tblOutgoings, OutgoingMatchPredicates,
                n => n.ID = OldContext.tblOutgoings.GetNextUnique(o => o.ID),
                (o, n) =>
                {
                    o.EntryDate = n.EntryDate;
                    o.Lot = n.Lot;
                    o.PkgID = n.PkgID;
                    o.Quantity = n.Quantity;
                    o.NetWgt = n.NetWgt;
                    o.TtlWgt = n.TtlWgt;
                    o.LocID = n.LocID;
                    o.TrtmtID = n.TrtmtID;
                    o.EmployeeID = n.EmployeeID;
                    o.CustProductCode = n.CustProductCode;
                });
        }

        private static void SynchronizeItems<T>(IEnumerable<T> newItems, IEnumerable<T> oldItems, IObjectSet<T> set, List<Func<T, T, bool>> matchPredicates,
            Action<T> initializeNew, Action<T, T> updateOld)
            where T : class
        {
            var toDelete = oldItems.ToList();
            foreach(var newItem in newItems)
            {
// ReSharper disable InvokeAsExtensionMethod
                // Surprisingly, will cause compiler error if called as extension method. -RI 2014/12/31
                var topMatch = EnumerableExtensions.RankMatches(newItem, toDelete, matchPredicates.ToArray()).FirstOrDefault();
// ReSharper restore InvokeAsExtensionMethod
                if(topMatch == null)
                {
                    initializeNew(newItem);
                    set.AddObject(newItem);
                }
                else
                {
                    var detail = topMatch.Match;
                    if(topMatch.Rank < matchPredicates.Count)
                    {
                        updateOld(topMatch.Match, newItem);
                    }
                    toDelete.Remove(detail);
                }
            }
            toDelete.ForEach(set.DeleteObject);
        }

        private static readonly List<Func<tblRinconDetail, tblRinconDetail, bool>> DetailMatchPredicates = new List<Func<tblRinconDetail, tblRinconDetail, bool>>
            {
                (n, e) => e.Lot == n.Lot,
                (n, e) => e.PkgID == n.PkgID,
                (n, e) => e.TrtmtID == n.TrtmtID,
                (n, e) => e.Quantity == n.Quantity,
                (n, e) => e.CurrLocID == n.CurrLocID,
                (n, e) => e.DestLocID == n.DestLocID
            };

        private static readonly List<Func<tblOutgoing, tblOutgoing, bool>> OutgoingMatchPredicates = new List<Func<tblOutgoing, tblOutgoing, bool>>
            {
                (n, e) => e.Lot == n.Lot,
                (n, e) => e.PkgID == n.PkgID,
                (n, e) => e.Quantity == n.Quantity,
                (n, e) => e.NetWgt == n.NetWgt,
                (n, e) => e.LocID == n.LocID,
                (n, e) => e.TrtmtID == n.TrtmtID,
                (n, e) => e.CustProductCode == n.CustProductCode,
            };

        private class RinconDetail
        {
            private readonly PickedInventoryItem _newItem;
            private readonly tblRincon _rincon;
            private readonly OldContextHelper _oldContextHelper;
            private tblPackaging _packaging;

            public RinconDetail(PickedInventoryItem newItem, tblRincon rincon, OldContextHelper oldContextHelper)
            {
                _newItem = newItem;
                _rincon = rincon;
                _oldContextHelper = oldContextHelper;
            }

            public tblRinconDetail Detail
            {
                get
                {
                    return _detail ?? (_detail = new tblRinconDetail
                        {
                            RinconID = _rincon.RinconID,
                            EmployeeID = _rincon.EmployeeID,
                            Lot = LotNumberParser.BuildLotNumber(_newItem),
                            PkgID = (_packaging = _oldContextHelper.GetPackaging(_newItem.PackagingProduct)).PkgID,
                            TrtmtID = _oldContextHelper.GetTreatment(_newItem).TrtmtID,
                            Quantity = _newItem.Quantity,
                            CurrLocID = _oldContextHelper.GetLocation(_newItem.FromLocation).LocID,
                            DestLocID = _oldContextHelper.GetLocation(_newItem.CurrentLocation).LocID
                        });
                }
            }
            private tblRinconDetail _detail;

            public IEnumerable<tblOutgoing> ToOutgoingRecords()
            {
                return new List<tblOutgoing>
                    {
                        ToOutgoing(Detail.Quantity, Detail.CurrLocID),
                        ToOutgoing(-Detail.Quantity, Detail.DestLocID),
                    };
            }

            private tblOutgoing ToOutgoing(decimal? quantity, int locId)
            {
                return new tblOutgoing
                    {
                        EntryDate = DateTime.UtcNow.ConvertUTCToLocal().RoundMillisecondsForSQL(),
                        RinconID = Detail.RinconID,
                        EmployeeID = Detail.EmployeeID,
                        Lot = Detail.Lot,
                        TTypeID = (int?) TransType.Rincon,
                        PkgID = Detail.PkgID.Value,
                        Quantity = quantity,
                        NetWgt = _packaging.NetWgt,
                        TtlWgt = quantity * _packaging.NetWgt,
                        LocID = locId,
                        TrtmtID = Detail.TrtmtID,
                        BatchLot = 0,
                        CustProductCode = _newItem.CustomerProductCode
                    };
            }
        }
    }
}