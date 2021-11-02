// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class NotebookProjectors
    {
        internal static Expression<Func<Notebook, NotebookKeyReturn>> SelectKey()
        {
            return n => new NotebookKeyReturn
                {
                    NotebookKey_Date = n.Date,
                    NotebookKey_Sequence = n.Sequence
                };
        }

        internal static Expression<Func<Notebook, NotebookReturn>> Select()
        {
            var key = SelectKey();
            var note = NoteProjectors.Select();

            return n => new NotebookReturn
                {
                    NotebookKeyReturn = key.Invoke(n),
                    Notes = n.Notes.Select(o => note.Invoke(o))
                };
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup