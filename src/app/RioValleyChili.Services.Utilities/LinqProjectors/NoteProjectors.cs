using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class NoteProjectors
    {
        internal static Expression<Func<Note, NoteKeyReturn>> SelectKey()
        {
            return n => new NoteKeyReturn
                {
                    NotebookKey_Date = n.NotebookDate,
                    NotebookKey_Sequence = n.NotebookSequence,
                    NoteKey_Sequence = n.Sequence
                };
        }

        internal static Expression<Func<Note, NoteReturn>> Select()
        {
            var key = SelectKey();

            return n => new NoteReturn
                {
                    NoteKeyReturn = key.Invoke(n),
                    CreatedByUser = n.Employee.UserName,
                    NoteDate = n.TimeStamp,
                    Sequence = n.Sequence,
                    Text = n.Text
                };
        }
    }
}