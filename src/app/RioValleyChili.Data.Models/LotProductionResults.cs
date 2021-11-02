using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class LotProductionResults : LotKeyEmployeeIdentifiableBase, ILocationKey
    {
        [StringLength(25)]
        public virtual string ShiftKey { get; set; }

        [Index, Column(TypeName = "DateTime")]
        public virtual DateTime ProductionBegin { get; set; }
        [Column(TypeName = "DateTime")]
        public virtual DateTime ProductionEnd { get; set; }
        [Column(TypeName = "DateTime")]
        public virtual DateTime DateTimeEntered { get; set; }

        public virtual int ProductionLineLocationId { get; set; }

        #region Navigational Properties

        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual ChileLotProduction Production { get; set; }
        [ForeignKey("ProductionLineLocationId")]
        public virtual Location ProductionLineLocation { get; set; }
        public virtual ICollection<LotProductionResultItem> ResultItems { get; set; }

        #endregion

        #region IProductionLocationKey

        public int LocationKey_Id { get { return ProductionLineLocationId; } }

        #endregion
    }
}