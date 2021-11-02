using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class IntraWarehouseOrder : EmployeeIdentifiableBase, IIntraWarehouseOrderKey, IPickedInventoryKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }
        [Index(IsUnique = true), Column(TypeName = "Money")]
        public virtual decimal TrackingSheetNumber { get; set; }

        [StringLength(20)]
        public virtual string OperatorName { get; set; }
        [Column(TypeName = "Date")]
        public virtual DateTime MovementDate { get; set; }

        #region Navigational Properties.

        [ForeignKey("DateCreated, Sequence")]
        public virtual PickedInventory PickedInventory { get; set; }

        [Obsolete("For data load/sync purposes. -RI 2014/12/29")]
        public virtual DateTime? RinconID { get; set; }

        #endregion

        #region Implementation of IIntraWarehouseOrderKey.

        public DateTime IntraWarehouseOrderKey_DateCreated { get { return DateCreated; } }
        public int IntraWarehouseOrderKey_Sequence { get { return Sequence; } }

        #endregion

        #region Implementation of IPickedInventoryKey.

        public DateTime PickedInventoryKey_DateCreated { get { return DateCreated; } }
        public int PickedInventoryKey_Sequence { get { return Sequence; } }

        #endregion
    }
}