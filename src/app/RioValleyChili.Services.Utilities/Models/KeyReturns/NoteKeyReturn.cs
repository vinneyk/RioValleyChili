using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class NoteKeyReturn : INoteKey
    {
        internal string NotebookKey { get { return new NotebookKey(this).KeyValue; } }

        internal string NoteKey { get { return new NoteKey(this).KeyValue; } }

        public DateTime NotebookKey_Date { get; internal set; }

        public int NotebookKey_Sequence { get; internal set; }

        public int NoteKey_Sequence { get; internal set; }
    }
}