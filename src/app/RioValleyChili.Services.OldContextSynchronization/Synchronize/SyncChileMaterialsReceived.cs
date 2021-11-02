using System;
using System.Collections.Generic;
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
    [Sync(NewContextMethod.SyncChileMaterialsReceived)]
    public class SyncChileMaterialsReceived : SyncCommandBase<IMaterialsReceivedUnitOfWork, LotKey>
    {
        public SyncChileMaterialsReceived(IMaterialsReceivedUnitOfWork unitOfWork) : base(unitOfWork) { }

        public sealed override void Synchronize(Func<LotKey> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }

            var key = getInput();
            var received = UnitOfWork.ChileMaterialsReceivedRepository.FindByKey(key,
                r => r.ChileLot.Lot,
                r => r.ChileProduct.Product,
                r => r.Items.Select(i => i.PackagingProduct.Product),
                r => r.Items.Select(i => i.Location));
            
            var oldLot = SyncDehydratedReceived(received);
            OldContext.SaveChanges();

            Console.WriteLine(ConsoleOutput.SyncDehydratedReceived, oldLot.Lot);
        }

        private tblLot SyncDehydratedReceived(ChileMaterialsReceived received)
        {
            var lotNumber = LotNumberBuilder.BuildLotNumber(received);
            var product = OldContextHelper.GetProduct(received.ChileProduct);

            var oldLot = OldContextHelper.OldContext.tblLots
                .Include(l => l.tblIncomings)
                .FirstOrDefault(l => l.Lot == lotNumber.LotNumber)
                ?? AddNewLot(received, lotNumber);

            oldLot.ProdID = product.ProdID;
            oldLot.LoadNum = received.LoadNumber;
            oldLot.Company_IA = received.Supplier.Name;
            oldLot.PurchOrder = received.ChileLot.Lot.PurchaseOrderNumber;
            oldLot.ShipperNum = received.ChileLot.Lot.ShipperNumber;
            oldLot.ProductionDate = received.DateReceived;
            oldLot.Produced = received.DateReceived;
            oldLot.Pellets = received.ChileMaterialsReceivedType == ChileMaterialsReceivedType.Other;

            var varieties = OldContextHelper.OldContext.tblVarieties.ToList();
            SyncIncomingRecords(received, oldLot, varieties);
            return oldLot;
        }

        private tblLot AddNewLot(ChileMaterialsReceived received, LotNumberResult lotNumber)
        {
            var lot = new tblLot
                {
                    Lot = lotNumber,
                    EmployeeID = received.ChileLot.Lot.EmployeeId,
                    EntryDate = received.ChileLot.Lot.TimeStamp.ConvertUTCToLocal().RoundMillisecondsForSQL(),

                    PTypeID = received.LotTypeId,
                    Julian = lotNumber.Julian,
                    BatchNum = received.LotDateSequence,
                    BatchStatID = null, //looks like default value set in old context - RI 2/17/2014
                    TargetWgt = 0, //looks like default value set in old context - RI 2/17/2014
                    Notes = "Old Context Synchronization",
                };
            OldContext.tblLots.AddObject(lot);
            return lot;
        }

        private void SyncIncomingRecords(ChileMaterialsReceived received, tblLot tblLot, ICollection<tblVariety> varieties)
        {
            foreach(var incoming in tblLot.tblIncomings.ToList())
            {
                OldContext.tblIncomings.DeleteObject(incoming);
            }

            var treatment = OldContextHelper.GetTreatment(received);
            var varietyId = varieties.Select(v => v.VarietyID).DefaultIfEmpty(0).Max() + 1;
            foreach(var item in received.Items)
            {
                var packaging = OldContextHelper.GetPackaging(item.PackagingProduct);

                var variety = varieties.FirstOrDefault(v => v.Variety == item.ChileVariety);
                if(variety == null)
                {
                    variety = new tblVariety
                        {
                            VarietyID = varietyId++,
                            Variety = item.ChileVariety,
                            EntryDate = DateTime.Now,
                            InActive = false,
                            SortOrder = varietyId.ToString()
                        };
                    OldContextHelper.OldContext.tblVarieties.AddObject(variety);
                    varieties.Add(variety);
                }

                tblLot.tblIncomings.Add(new tblIncoming
                    {
                        EmployeeID = tblLot.EmployeeID,
                        EntryDate = tblLot.EntryDate.Value,

                        TTypeID = (int?) (received.ChileMaterialsReceivedType == ChileMaterialsReceivedType.Dehydrated ? TransType.DeHy : TransType.Other),
                        Quantity = item.Quantity,
                        TrtmtID = treatment.TrtmtID,
                        Tote = item.ToteKey,
                        VarietyID = variety.VarietyID,
                        Company_IA = tblLot.Company_IA,
                        LocID = item.Location.LocID.Value,
                        DehyLocale = item.GrowerCode,
                        PkgID = packaging.PkgID,
                        NetWgt = packaging.NetWgt,
                        TtlWgt = item.Quantity * packaging.NetWgt
                    });
            }
        }
    }
}