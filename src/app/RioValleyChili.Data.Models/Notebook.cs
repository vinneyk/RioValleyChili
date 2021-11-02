using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class Notebook : INotebookKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime Date { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        #region INotebookKey

        public DateTime NotebookKey_Date { get { return Date; } }
        public int NotebookKey_Sequence { get { return Sequence; } }

        #endregion
    }
}