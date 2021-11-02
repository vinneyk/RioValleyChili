using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class Location : ILocationKey, IFacilityKey
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        public virtual LocationType LocationType { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool Locked { get; set; }

        [Index("IX_FacilityId_Description", 2, IsUnique = true), StringLength(Constants.StringLengths.LocationDescription)]
        public virtual string Description { get; set; }

        [Index("IX_FacilityId_Description", 1, IsUnique = true)]
        public virtual int FacilityId { get; set; }

        [ForeignKey("FacilityId")]
        public virtual Facility Facility { get; set; }

        [Obsolete("For data load/sync purposes. -RI 2014/12/9")]
        public virtual int? LocID { get; set; }

        public virtual ICollection<Inventory> Inventory { get; set; }
        [InverseProperty("CurrentLocation")]
        public virtual ICollection<PickedInventoryItem> PickedInventoryItems { get; set; }

        #region Key Interfaces

        public int LocationKey_Id { get { return Id; } }
        public int FacilityKey_Id { get { return FacilityId; } }

        #endregion
    }
}