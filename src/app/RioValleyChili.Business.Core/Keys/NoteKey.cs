using System;
using System.Globalization;
using System.Linq.Expressions;
using Solutionhead.EntityKey;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Keys
{
    public class NoteKey : EntityKey<INoteKey>.With<DateTime, int, int>, IKey<Note>, INoteKey
    {
        public NoteKey() { }

        public NoteKey(INoteKey noteKey) : base(noteKey) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidNoteKey, inputValue);
        }

        protected override INoteKey ConstructKey(DateTime field0, int field1, int field2)
        {
            return new NoteKey { Field0 = field0, Field1 = field1, Field2 = field2 };
        }

        protected override With<DateTime, int, int> DeconstructKey(INoteKey key)
        {
            return new NoteKey { Field0 = key.NotebookKey_Date, Field1 = key.NotebookKey_Sequence, Field2 = key.NoteKey_Sequence };
        }

        protected override string DateTimeToString(DateTime d)
        {
            return d.ToString("yyyyMMdd");
        }

        protected override bool TryParseDateTime(string s, out object result)
        {
            DateTime dateTime;
            var tryParse = DateTime.TryParseExact(s, "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.None, out dateTime);
            result = dateTime;
            return tryParse;
        }

        public Expression<Func<Note, bool>> FindByPredicate
        {
            get { return n => n.NotebookDate == Field0 && n.NotebookSequence == Field1 && n.Sequence == Field2; }
        }

        public DateTime NotebookKey_Date { get { return Field0; } }

        public int NotebookKey_Sequence { get { return Field1; } }

        public int NoteKey_Sequence { get { return Field2; } }
    }
}