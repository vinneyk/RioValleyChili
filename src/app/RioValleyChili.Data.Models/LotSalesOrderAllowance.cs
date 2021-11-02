using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class LotSalesOrderAllowance : LotKeyEntityBase, ISalesOrderKey
    {
        [Key, Column(Order = 3, TypeName = "Date")]
        public virtual DateTime SalesOrderDateCreated { get; set; }
        [Key, Column(Order = 4)]
        public virtual int SalesOrderSequence { get; set; }

        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual Lot Lot { get; set; }
        [ForeignKey("SalesOrderDateCreated, SalesOrderSequence")]
        public virtual SalesOrder SalesOrder { get; set; }

        public DateTime SalesOrderKey_DateCreated { get { return SalesOrderDateCreated; } }
        public int SalesOrderKey_Sequence { get { return SalesOrderSequence; } }
    }
}