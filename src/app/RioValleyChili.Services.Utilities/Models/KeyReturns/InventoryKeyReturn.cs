using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class InventoryKeyReturn : IInventoryKey
    {
        internal string InventoryKey { get { return new InventoryKey(this).KeyValue; } }

        internal string LotKey { get { return new LotKey(this).KeyValue; } }

        internal string PackagingProductKey { get { return new PackagingProductKey(this).KeyValue; } }

        internal string WarehouseLocationKey { get { return new LocationKey(this).KeyValue; } }

        internal string TreatmentKey { get { return new InventoryTreatmentKey(this).KeyValue; } }

        public int LotKey_LotTypeId { get; internal set; }

        public DateTime LotKey_DateCreated { get; internal set; }

        public int LotKey_DateSequence { get; internal set; }

        public int PackagingProductKey_ProductId { get; internal set; }

        public int LocationKey_Id { get; internal set; }

        public int InventoryTreatmentKey_Id { get; internal set; }

        public string InventoryKey_ToteKey { get; internal set; }
    }
}