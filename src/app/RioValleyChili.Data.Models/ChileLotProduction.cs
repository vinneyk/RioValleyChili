using System;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class ChileLotProduction : LotKeyEmployeeIdentifiableBase, IPickedInventoryKey
    {
        [Column(TypeName = "Date")]
        public virtual DateTime PickedInventoryDateCreated { get; set; }
        public virtual int PickedInventorySequence { get; set; }

        public virtual ProductionType ProductionType { get; set; }

        [ForeignKey("PickedInventoryDateCreated, PickedInventorySequence")]
        public virtual PickedInventory PickedInventory { get; set; }
        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual LotProductionResults Results { get; set; }
        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual ChileLot ResultingChileLot { get; set; }

        #region IPickedInventoryKey

        public DateTime PickedInventoryKey_DateCreated { get { return PickedInventoryDateCreated; } }
        public int PickedInventoryKey_Sequence { get { return PickedInventorySequence; } }

        #endregion
    }
}