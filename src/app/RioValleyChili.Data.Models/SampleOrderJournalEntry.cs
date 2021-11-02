using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class SampleOrderJournalEntry : ISampleOrderJournalEntryKey
    {
        [Key, Column(Order = 0)]
        public virtual int SampleOrderYear { get; set; }
        [Key, Column(Order = 1)]
        public virtual int SampleOrderSequence { get; set; }
        [Key, Column(Order = 2)]
        public virtual int EntrySequence { get; set; }

        [Column(TypeName = "date")]
        public virtual DateTime? Date { get; set; }
        public virtual string Text { get; set; }

        public virtual int EmployeeId { get; set; }
        [Obsolete("For referencing old context. - RI 2016/9/13")]
        public DateTime? SamNoteID { get; set; }

        [ForeignKey("SampleOrderYear, SampleOrderSequence")]
        public virtual SampleOrder SampleOrder { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        int ISampleOrderKey.SampleOrderKey_Year { get { return SampleOrderYear; } }
        int ISampleOrderKey.SampleOrderKey_Sequence { get { return SampleOrderSequence; } }
        int ISampleOrderJournalEntryKey.SampleOrderJournalEntryKey_Sequence { get { return EntrySequence; } }
    }
}