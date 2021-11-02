using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.DataSeeders.Mothers.Base
{
    public abstract class LotInventoryEntityObjectMotherBase<TLot> : EntityMotherLogBase<Inventory, LotInventoryEntityObjectMotherBase<TLot>.CallbackParameters>
        where TLot : class, IDerivedLot
    {
        #region fields and constructors

        protected abstract LotTypeEnum LotType { get; }
        protected readonly NewContextHelper NewContextHelper;

        protected LotInventoryEntityObjectMotherBase(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            NewContextHelper = new NewContextHelper(newContext);
        }

        #endregion

        protected enum EntityTypes
        {
            Inventory
        }

        protected MotherLoadCount<EntityTypes> LoadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<Inventory> BirthRecords()
        {
            LoadCount.Reset();

            foreach(var inventoryByLot in SelectLotsToLoad())
            {
                var inventory = inventoryByLot.ToList();
                LoadCount.AddRead(EntityTypes.Inventory, (uint) inventory.Count);

                LotKey lotKey;
                if(!LotNumberParser.ParseLotNumber(inventoryByLot.Key, out lotKey))
                {
                    Log(new CallbackParameters(CallbackReason.InvalidLotNumber)
                    {
                        InventoryByLot = inventoryByLot
                    });
                    continue;
                }

                var lot = GetLot(lotKey);
                if(lot == null)
                {
                    Log(new CallbackParameters(CallbackReason.LotNotLoaded)
                        {
                            LotKey = lotKey,
                            InventoryByLot = inventoryByLot
                        });
                    continue;
                }

                foreach(var inventoryItem in inventory)
                {
                    var newInventory = CreateInventory(inventoryItem, lot);
                    if(newInventory == null)
                    {
                        continue;
                    }

                    LoadCount.AddLoaded(EntityTypes.Inventory);
                    yield return newInventory;
                }
            }

            LoadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        protected abstract IDerivedLot GetLot(LotKey lotKey);

        private List<IGrouping<int, ViewInventoryLoadSelected>> SelectLotsToLoad()
        {
            var minDate = DateTime.UtcNow.Month > 7 ? new DateTime(DateTime.UtcNow.Year, 8, 1) : new DateTime(DateTime.UtcNow.Year-1, 8, 1);
            return OldContext.CreateObjectSet<ViewInventoryLoadSelected>()
                .Where(l => 
                    l.tblLot.PTypeID == (int)LotType 
                    && l.Quantity >= 1 
                    && !l.tblLot.tblIncomings.Any(i => i.Tote != null && i.Tote != string.Empty && l.tblLot.EntryDate <= minDate))
                .GroupBy(i => i.Lot).ToList();
        }

        private Inventory CreateInventory(ViewInventoryLoadSelected inventory, ILotKey lotKey)
        {
            if(inventory.Quantity < 1)
            {
                Log(new CallbackParameters(CallbackReason.QuantityLessThanOne)
                    {
                        Inventory = inventory
                    });
                return null;
            }

            var warehouseLocation = NewContextHelper.GetLocation(inventory.LocID);
            if(warehouseLocation == null)
            {
                Log(new CallbackParameters(CallbackReason.WarehouseLocationNotLoaded)
                    {
                        Inventory = inventory
                    });
                return null;
            }

            var packagingProduct = GetInventoryPackaging(inventory);
            if(packagingProduct == null)
            {
                Log(new CallbackParameters(CallbackReason.PackagingNotLoaded)
                    {
                        Inventory = inventory
                    });
                return null;
            }
            
            return new Inventory
                {
                    LotDateCreated = lotKey.LotKey_DateCreated,
                    LotDateSequence = lotKey.LotKey_DateSequence,
                    LotTypeId = lotKey.LotKey_LotTypeId,
                    
                    PackagingProductId = packagingProduct.Id,
                    LocationId = warehouseLocation.Id,
                    TreatmentId = inventory.TrtmtID,
                    ToteKey = inventory.Tote ?? "",

                    Quantity = (int) inventory.Quantity
                };
        }

        protected virtual IProduct GetInventoryPackaging(ViewInventoryLoadSelected inventory)
        {
            var lotPackaging = NewContextHelper.GetPackagingProduct(inventory.PkgID);
            return lotPackaging != null ? lotPackaging.Product : null;
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            StringTruncated,

            InvalidLotNumber,
            WarehouseLocationNotLoaded,
            PackagingNotLoaded,
            QuantityLessThanOne,
            LotNotLoaded,
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public IGrouping<int, ViewInventoryLoadSelected> InventoryByLot { get; set; }
            public LotTypeEnum LotType { get; set; }
            public ViewInventoryLoadSelected Inventory { get; set; }
            public LotKey LotKey { get; set; }

            protected override CallbackReason ExceptionReason { get { return LotInventoryEntityObjectMotherBase<TLot>.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return LotInventoryEntityObjectMotherBase<TLot>.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return LotInventoryEntityObjectMotherBase<TLot>.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case LotInventoryEntityObjectMotherBase<TLot>.CallbackReason.Exception:
                        return ReasonCategory.Error;

                    case LotInventoryEntityObjectMotherBase<TLot>.CallbackReason.InvalidLotNumber:
                    case LotInventoryEntityObjectMotherBase<TLot>.CallbackReason.WarehouseLocationNotLoaded:
                    case LotInventoryEntityObjectMotherBase<TLot>.CallbackReason.PackagingNotLoaded:
                    case LotInventoryEntityObjectMotherBase<TLot>.CallbackReason.QuantityLessThanOne:
                    case LotInventoryEntityObjectMotherBase<TLot>.CallbackReason.LotNotLoaded:
                        return ReasonCategory.RecordSkipped;
                }
                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}