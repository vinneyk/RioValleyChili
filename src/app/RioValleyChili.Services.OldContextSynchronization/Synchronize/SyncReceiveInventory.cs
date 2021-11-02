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
    [Sync(NewContextMethod.ReceiveInventory)]
    public class SyncReceiveInventory : SyncCommandBase<IInventoryUnitOfWork, LotKey>
    {
        public SyncReceiveInventory(IInventoryUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<LotKey> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }
            var lotKey = getInput();

            var oldLot = new SyncLotHelper(OldContext, UnitOfWork, OldContextHelper).SynchronizeOldLot(lotKey, false, false);
            var inventory = UnitOfWork.InventoryRepository
                .Filter(i => i.LotDateCreated == lotKey.LotKey_DateCreated && i.LotDateSequence == lotKey.LotKey_DateSequence && i.LotTypeId == lotKey.LotKey_LotTypeId,
                    i => i.PackagingProduct.Product,
                    i => i.Location,
                    i => i.Treatment)
                .ToList();

            foreach(var item in inventory)
            {
                OldContext.tblIncomings.AddObject(CreateIncoming(oldLot, item));
            }

            OldContext.SaveChanges();

            Console.Write(ConsoleOutput.ReceivedInventory, oldLot.Lot);
        }

        private tblIncoming CreateIncoming(tblLot oldLot, Inventory inventory)
        {
            var packaging = OldContextHelper.GetPackaging(inventory.PackagingProduct);
            var treatment = OldContextHelper.GetTreatment(inventory);

            var transType = TransType.Other;
            switch(inventory.LotTypeEnum.ToProductType())
            {
                case ProductTypeEnum.Additive: transType = TransType.Ingredients; break;
                case ProductTypeEnum.Packaging: transType = TransType.Packaging; break;
            }

            return new tblIncoming
                {
                    EmployeeID = oldLot.EmployeeID,
                    EntryDate = oldLot.EntryDate.Value,
                    TTypeID = (int?) transType,

                    Lot = oldLot.Lot,
                    Quantity = inventory.Quantity,
                    PkgID = packaging.PkgID,
                    LocID = inventory.Location.LocID.Value,
                    TrtmtID = treatment.TrtmtID,
                    Tote = inventory.ToteKey,

                    Company_IA = oldLot.Company_IA,
                    NetWgt = packaging.NetWgt,
                    TtlWgt = inventory.Quantity * packaging.NetWgt,
                };
        }
    }
}