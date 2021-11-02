using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class Note : EmployeeIdentifiableBase, INoteKey
    {
        [Key, Column(Order = 0, TypeName = "date")]
        public virtual DateTime NotebookDate { get; set; }
        [Key, Column(Order = 1)]
        public virtual int NotebookSequence { get; set; }
        [Key, Column(Order = 2)]
        public virtual int Sequence { get; set; }

        public virtual string Text { get; set; }

        [ForeignKey("NotebookDate, NotebookSequence")]
        public virtual Notebook Notebook { get; set; }

        #region INoteKey

        public DateTime NotebookKey_Date { get { return NotebookDate; } }
        public int NotebookKey_Sequence { get { return NotebookSequence; } }
        public int NoteKey_Sequence { get { return Sequence; } }

        #endregion
    }
}