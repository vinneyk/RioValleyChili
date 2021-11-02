using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class CustomerNote : EmployeeIdentifiableBase, ICustomerNoteKey
    {
        [Key, Column(Order = 0)]
        public virtual int CustomerId { get; set; }
        [Key, Column(Order = 1)]
        public virtual int NoteId { get; set; }

        [StringLength(Constants.StringLengths.CustomerNoteType)]
        public virtual string Type { get; set; }
        public virtual string Text { get; set; }
        public virtual bool Bold { get; set; }

        [Obsolete("For data load/sync purposes. -RI 2015/4/13")]
        public virtual DateTime? EntryDate { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        #region ICustomerNoteKey implementation.

        public int CustomerKey_Id { get { return CustomerId; } }
        public int CustomerNoteKey_Id { get { return NoteId; } }

        #endregion
    }
}