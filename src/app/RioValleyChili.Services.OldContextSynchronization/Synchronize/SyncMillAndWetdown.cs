using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SyncMillAndWetdown)]
    public class SyncMillAndWetdown : SyncCommandBase<IChileLotProductionUnitOfWork, LotKey>
    {
        public SyncMillAndWetdown(IChileLotProductionUnitOfWork unitOfWork) : base(unitOfWork) { }

        public sealed override void Synchronize(Func<LotKey> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }
            var key = getInput();
            var production = UnitOfWork.ChileLotProductionRepository.FindByKey(key,
                m => m.ResultingChileLot.ChileProduct.Product,
                m => m.Results.ProductionLineLocation,
                m => m.Results.ResultItems.Select(i => i.PackagingProduct.Product),
                m => m.Results.ResultItems.Select(i => i.Location),
                m => m.PickedInventory.Items.Select(i => i.PackagingProduct.Product),
                m => m.PickedInventory.Items.Select(i => i.FromLocation),
                m => m.PickedInventory.Items.Select(i => i.Treatment));

            bool newLot;
            var tblLot = GetOrCreateLot(production, out newLot);
            SetIncomingRecords(tblLot, production.Results.ResultItems);
            SetOutgoingRecords(tblLot, production);

            OldContext.SaveChanges();
            Console.Write(newLot ? ConsoleOutput.AddedLot : ConsoleOutput.UpdatedLot, tblLot.Lot);
        }

        private tblLot GetOrCreateLot(ChileLotProduction production, out bool newLot)
        {
            newLot = false;
            var lotNumber = LotNumberBuilder.BuildLotNumber(production);
            var tblLot = OldContext.tblLots
                .Include(l => l.tblIncomings, l => l.tblOutgoings)
                .FirstOrDefault(l => l.Lot == lotNumber);
            if(tblLot == null)
            {
                tblLot = new tblLot
                    {
                        Lot = lotNumber,
                        EmployeeID = production.ResultingChileLot.Lot.EmployeeId,
                        EntryDate = production.ResultingChileLot.Lot.TimeStamp.ConvertUTCToLocal().RoundMillisecondsForSQL(),

                        PTypeID = production.LotTypeId,
                        Julian = lotNumber.Julian,
                        BatchNum = production.LotDateSequence,
                        
                        Shift = production.Results.ShiftKey,
                        Notes = "Old Context Synchronization",
                        BatchProdctnOrder = 0,
                        SetTrtmtID = 0,
                        BatchStatID = (int?)BatchStatID.Produced,
                        TargetWgt = 0, //looks like default value set in old context - RI 2014/2/17,
                        tblOutgoings = new EntityCollection<tblOutgoing>(),
                        tblIncomings = new EntityCollection<tblIncoming>()
                    };
                OldContext.tblLots.AddObject(tblLot);
                newLot = true;
            }

            var product = OldContextHelper.GetProduct(production.ResultingChileLot.ChileProduct);
            var productionLine = OldContextHelper.GetProductionLine(production.Results.ProductionLineLocation).Value;
            var productionDate = production.LotDateCreated.Date;
            var batchBegTime = production.Results.ProductionBegin.ConvertUTCToLocal().RoundMillisecondsForSQL();
            var batchEndTime = production.Results.ProductionEnd.ConvertUTCToLocal().RoundMillisecondsForSQL();

            tblLot.ProdID = product.ProdID;
            tblLot.ProductionLine = productionLine;
            tblLot.BatchBegTime = batchBegTime;
            tblLot.BatchEndTime = batchEndTime;
            tblLot.ProductionDate = productionDate;
            tblLot.Produced = productionDate;

            return tblLot;
        }

        private void SetIncomingRecords(tblLot tblLot, IEnumerable<LotProductionResultItem> resultItems)
        {
            foreach(var item in tblLot.tblIncomings.ToList())
            {
                OldContext.tblIncomings.DeleteObject(item);
            }

            foreach(var item in resultItems)
            {
                var packaging = OldContextHelper.GetPackaging(item.PackagingProduct);

                tblLot.tblIncomings.Add(new tblIncoming
                    {
                        EmployeeID = tblLot.EmployeeID,
                        EntryDate = tblLot.EntryDate.Value,

                        Lot = tblLot.Lot,
                        TTypeID = (int?)TransType.MnW,
                        PkgID = packaging.PkgID,
                        Quantity = item.Quantity,
                        NetWgt = packaging.NetWgt,
                        TtlWgt = item.Quantity * packaging.NetWgt,
                        LocID = item.Location.LocID.Value,
                        TrtmtID = 0
                    });
            }
        }

        private void SetOutgoingRecords(tblLot tblLot, ChileLotProduction production)
        {
            foreach(var previousInput in OldContext.tblOutgoings.Where(o => o.NewLot == tblLot.Lot).ToList())
            {
                OldContext.tblOutgoings.DeleteObject(previousInput);
            }

            foreach(var item in production.PickedInventory.Items)
            {
                var sourceLotNumber = LotNumberBuilder.BuildLotNumber(item);
                var packaging = OldContextHelper.GetPackaging(item.PackagingProduct);

                OldContext.tblOutgoings.AddObject(new tblOutgoing
                    {
                        EmployeeID = tblLot.EmployeeID,
                        EntryDate = DateTime.UtcNow.ConvertUTCToLocal().RoundMillisecondsForSQL(),

                        Lot = sourceLotNumber,
                        NewLot = tblLot.Lot,
                        TTypeID = (int?)TransType.DeHy,

                        PkgID = packaging.PkgID,
                        Tote = item.ToteKey,
                        Quantity = item.Quantity,
                        NetWgt = packaging.NetWgt,
                        TtlWgt = item.Quantity * packaging.NetWgt,
                        LocID = item.FromLocation.LocID.Value,
                        TrtmtID = 0
                    });
            }
        }
    }
}