using System;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class NoteReturn : INoteReturn
    {
        public string NoteKey { get { return NoteKeyReturn.NoteKey; } }
        public DateTime NoteDate { get; internal set; }
        public string CreatedByUser { get; internal set; }
        public int Sequence { get; internal set; }
        public string Text { get; internal set; }

        internal NoteKeyReturn NoteKeyReturn { get; set; }
    }
}