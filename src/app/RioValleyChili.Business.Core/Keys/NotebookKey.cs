using System;
using System.Globalization;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class NotebookKey : EntityKey<INotebookKey>.With<DateTime, int>, IKey<Notebook>, INotebookKey
    {
        public NotebookKey() { }

        public NotebookKey(INotebookKey notebookKey) : base(notebookKey) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidNotebookKey, inputValue);
        }

        protected override INotebookKey ConstructKey(DateTime field0, int field1)
        {
            return new NotebookKey { Field0 = field0, Field1 = field1 };
        }

        protected override With<DateTime, int> DeconstructKey(INotebookKey key)
        {
            return new NotebookKey{ Field0 = key.NotebookKey_Date, Field1 = key.NotebookKey_Sequence };
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

        public Expression<Func<Notebook, bool>> FindByPredicate
        {
            get { return n => n.Date == Field0 && n.Sequence == Field1; }
        }

        public DateTime NotebookKey_Date { get { return Field0; } }

        public int NotebookKey_Sequence { get { return Field1; } }
    }

    public static class NotebookKeyExtensions
    {
        public static NotebookKey ToNotebookKey(this INotebookKey k)
        {
            return new NotebookKey(k);
        }
    }
}

