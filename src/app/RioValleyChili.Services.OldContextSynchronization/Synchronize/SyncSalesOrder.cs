using System;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using System.Collections.Generic;
using RioValleyChili.Services.OldContextSynchronization.Parameters;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SyncSalesOrder),
        Issue("Added lock to prevent possible tblStagedFG.PK violation due to concurrent execution in production. -RI 2016-09-06",
        References = new[] { "RVCADMIN-1283"})]
    public class SyncSalesOrder : SyncCommandBase<IInventoryShipmentOrderUnitOfWork, SyncSalesOrderParameters>
    {
        public SyncSalesOrder(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        private bool _commitNewContext;
        private readonly object _lock = new object();

        public override void Synchronize(Func<SyncSalesOrderParameters> getInput)
        {
            lock(_lock)
            {
                if(getInput == null) { throw new ArgumentNullException("getInput"); }

                var parameters = getInput();
                var key = parameters.SalesOrderKey;
                var salesOrder = GetSalesOrder(key);
                if(salesOrder == null)
                {
                    throw new Exception(string.Format("Could not find SalesOrder with key[{0}].", key));
                }

                _commitNewContext = false;
                var oldOrder = GetOldOrder(salesOrder, parameters.New);
                oldOrder.SetSalesOrder(salesOrder);
                SyncOrderItems(salesOrder, oldOrder);

                if(_commitNewContext)
                {
                    UnitOfWork.Commit();
                }
                OldContext.SaveChanges();

                Console.WriteLine(ConsoleOutput.SyncTblOrder, salesOrder.InventoryShipmentOrder.MoveNum);
            }
        }

        private SalesOrder GetSalesOrder(SalesOrderKey key)
        {
            var order = UnitOfWork.SalesOrderRepository.FindByKey(key,
                c => c.Customer.Company,
                c => c.Broker,
                c => c.InventoryShipmentOrder.SourceFacility,
                c => c.InventoryShipmentOrder.ShipmentInformation,
                c => c.InventoryShipmentOrder.PickedInventory,
                c => c.SalesOrderItems.Select(i => i.InventoryPickOrderItem.Product),
                c => c.SalesOrderItems.Select(i => i.InventoryPickOrderItem.PackagingProduct.Product),
                c => c.SalesOrderPickedItems.Select(p => p.PickedInventoryItem.CurrentLocation),
                c => c.SalesOrderPickedItems.Select(p => p.PickedInventoryItem.PackagingProduct.Product),
                c => c.SalesOrderPickedItems.Select(p => p.PickedInventoryItem.Lot.ChileLot.Production.Results),
                c => c.SalesOrderPickedItems.Select(p => p.PickedInventoryItem.Lot.Attributes));
            return order;
        }

        private tblOrder GetOldOrder(SalesOrder salesOrder, bool createNew)
        {
            if(!createNew && salesOrder.InventoryShipmentOrder.MoveNum != null)
            {
                return OldContext.tblOrders
                    .Where(o => o.OrderNum == salesOrder.InventoryShipmentOrder.MoveNum.Value)
                    .Select(o => new
                        {
                            tblOrder = o,
                            details = o.tblOrderDetails,
                            staged = o.tblOrderDetails.SelectMany(d => d.tblStagedFGs)
                        })
                    .ToList().Select(o => o.tblOrder).Single();
            }
            
            var tblOrder = new tblOrder
                {
                    OrderNum = OldContext.tblOrders.Select(o => o.OrderNum).DefaultIfEmpty(0).Max() + 1,
                    TTypeID = (int) salesOrder.InventoryShipmentOrder.OrderType.ToTransType(),
                    EntryDate = DateTime.UtcNow.ConvertUTCToLocal(),
                    tblOrderDetails = new EntityCollection<tblOrderDetail>(),
                    s_GUID = Guid.NewGuid()
                };
            OldContext.tblOrders.AddObject(tblOrder);

            salesOrder.InventoryShipmentOrder.MoveNum = tblOrder.OrderNum;
            _commitNewContext = true;

            return tblOrder;
        }

        private void SyncOrderItems(SalesOrder salesOrder, tblOrder tblOrder)
        {
            var detailsToDelete = tblOrder.tblOrderDetails.ToList();

            var nowODetail = DateTime.UtcNow.ConvertUTCToLocal().RoundMillisecondsForSQL();
            var oDetail = OldContext.tblOrderDetails.Any() ? OldContext.tblOrderDetails.Max(o => o.ODetail) : nowODetail;
            if(oDetail < nowODetail)
            {
                oDetail = nowODetail;
            }

            var nowEntryDate = DateTime.UtcNow.ConvertUTCToLocal().RoundMillisecondsForSQL();
            var lastEntryDate = OldContext.tblStagedFGs.Select(f => f.EntryDate).DefaultIfEmpty(nowEntryDate).Max();
            if(lastEntryDate < nowEntryDate)
            {
                lastEntryDate = nowEntryDate;
            }

            foreach(var orderItem in salesOrder.SalesOrderItems)
            {
                var detail = detailsToDelete.FirstOrDefault(d => d.ODetail == orderItem.ODetail);
                if(detail != null)
                {
                    detailsToDelete.Remove(detail);
                }
                else
                {
                    oDetail = oDetail.AddSeconds(1);
                    detail = new tblOrderDetail
                        {
                            ODetail = oDetail,
                            s_GUID = Guid.NewGuid(),
                            tblStagedFGs = new EntityCollection<tblStagedFG>()
                        };
                    tblOrder.tblOrderDetails.Add(detail);
                    orderItem.ODetail = detail.ODetail;
                    _commitNewContext = true;
                }

                var product = OldContextHelper.GetProduct(orderItem.InventoryPickOrderItem.Product.ProductCode);
                var packaging = OldContextHelper.GetPackaging(orderItem.InventoryPickOrderItem.PackagingProduct);
                var treatment = OldContextHelper.GetTreatment(orderItem.InventoryPickOrderItem);
            
                detail.OrderNum = orderItem.Order.InventoryShipmentOrder.MoveNum.Value;
                detail.ProdID = product.ProdID;
                detail.PkgID = packaging.PkgID;
                detail.Quantity = orderItem.InventoryPickOrderItem.Quantity;
                detail.TrtmtID = treatment.TrtmtID;
                detail.Price = (decimal?) orderItem.PriceBase;
                detail.FreightP = (decimal?) orderItem.PriceFreight;
                detail.TrtnmntP = (decimal?) orderItem.PriceTreatment;
                detail.WHCostP = (decimal?) orderItem.PriceWarehouse;
                detail.Rebate = (decimal?) orderItem.PriceRebate;
                detail.KDetailID = orderItem.ContractItem == null ? null : orderItem.ContractItem.KDetailID;
                detail.CustProductCode = orderItem.InventoryPickOrderItem.CustomerProductCode;
                detail.CustLot = orderItem.InventoryPickOrderItem.CustomerLotCode;

                SyncPickedItems(tblOrder, orderItem, ref lastEntryDate, detail);
            }

            foreach(var detail in detailsToDelete)
            {
                foreach(var staged in detail.tblStagedFGs)
                {
                    OldContext.tblStagedFGs.DeleteObject(staged);
                }
                OldContext.tblOrderDetails.DeleteObject(detail);
            }
        }

        private void SyncPickedItems(tblOrder tblOrder, SalesOrderItem orderItem, ref DateTime lastEntryDate, tblOrderDetail detail)
        {
            var stagedToDelete = detail.tblStagedFGs.ToDictionary(s => s.EntryDate);

            foreach(var picked in orderItem.PickedItems ?? new List<SalesOrderPickedItem>())
            {
                tblStagedFG staged;
                if(picked.PickedInventoryItem.DetailID != null && stagedToDelete.TryGetValue(picked.PickedInventoryItem.DetailID.Value, out staged))
                {
                    stagedToDelete.Remove(staged.EntryDate);
                }
                else
                {
                    lastEntryDate = lastEntryDate.AddSeconds(1);
                    staged = new tblStagedFG
                        {
                            EntryDate = lastEntryDate,
                            s_GUID = Guid.NewGuid()
                        };
                    detail.tblStagedFGs.Add(staged);
                    picked.PickedInventoryItem.DetailID = staged.EntryDate;
                    _commitNewContext = true;
                }

                var packaging = OldContextHelper.GetPackaging(picked.PickedInventoryItem.PackagingProduct);
                var treatment = OldContextHelper.GetTreatment(picked.PickedInventoryItem);

                staged.OrderNum = tblOrder.OrderNum;
                staged.ODetail = detail.ODetail;
                staged.Lot = LotNumberParser.BuildLotNumber(picked.PickedInventoryItem);
                staged.TTypeID = (int?) TransType.Batching; //To match logic found in Access (apnd_PickProdTemp) - RI 2016-3-15
                staged.PkgID = packaging.PkgID;
                staged.Quantity = picked.PickedInventoryItem.Quantity;
                staged.NetWgt = packaging.NetWgt;
                staged.TtlWgt = picked.PickedInventoryItem.Quantity * packaging.NetWgt;
                staged.LocID = picked.PickedInventoryItem.CurrentLocation.LocID.Value;
                staged.TrtmtID = treatment.TrtmtID;
                staged.EmployeeID = picked.PickedInventoryItem.PickedInventory.EmployeeId;
                staged.CustProductCode = picked.PickedInventoryItem.CustomerProductCode;
                staged.CustLot = picked.PickedInventoryItem.CustomerLotCode;

                staged.AstaCalc = null;
                var asta = picked.PickedInventoryItem.Lot.Attributes.FirstOrDefault(a => a.AttributeShortName == GlobalKeyHelpers.AstaAttributeNameKey.AttributeNameKey_ShortName);
                if(asta != null)
                {
                    var productionEnd = asta.AttributeDate;
                    if(picked.PickedInventoryItem.Lot.ChileLot != null && picked.PickedInventoryItem.Lot.ChileLot.Production != null && picked.PickedInventoryItem.Lot.ChileLot.Production.Results != null)
                    {
                        productionEnd = picked.PickedInventoryItem.Lot.ChileLot.Production.Results.ProductionEnd;
                    }
                    staged.AstaCalc = AstaCalculator.CalculateAsta(asta.AttributeValue, asta.AttributeDate, productionEnd, DateTime.UtcNow);
                }

                staged.LoBac = false;
                if(picked.PickedInventoryItem.Lot.ChileLot != null)
                {
                    staged.LoBac = picked.PickedInventoryItem.Lot.ChileLot.AllAttributesAreLoBac;
                }
            }

            foreach(var staged in stagedToDelete.Values)
            {
                OldContext.tblStagedFGs.DeleteObject(staged);
            }
        }
    }
}