using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    [Table("ChileMaterialsReceived")]
    public class ChileMaterialsReceived : LotKeyEmployeeIdentifiableBase, IChileProductKey, IInventoryTreatmentKey, ICompanyKey
    {
        public virtual ChileMaterialsReceivedType ChileMaterialsReceivedType { get; set; }
        [StringLength(Constants.StringLengths.LoadNumber)]
        public virtual string LoadNumber { get; set; }
        [Column(TypeName = "Date")]
        public virtual DateTime DateReceived { get; set; }

        public virtual int ChileProductId { get; set; }
        public virtual int TreatmentId { get; set; }
        public virtual int SupplierId { get; set; }

        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual ChileLot ChileLot { get; set; }
        [ForeignKey("ChileProductId")]
        public virtual ChileProduct ChileProduct { get; set; }
        [ForeignKey("TreatmentId")]
        public virtual InventoryTreatment InventoryTreatment { get; set; }
        [ForeignKey("SupplierId")]
        public virtual Company Supplier { get; set; }

        public virtual ICollection<ChileMaterialsReceivedItem> Items { get; set; }

        #region Key Interface Implementations
        
        public int ChileProductKey_ProductId { get { return ChileProductId; } }
        public int InventoryTreatmentKey_Id { get { return TreatmentId; } }
        public int CompanyKey_Id { get { return SupplierId; } }

        #endregion
    }
}