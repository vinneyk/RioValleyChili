using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public static class NotesExtensions
    {
        public static string AggregateNotes(this IEnumerable<Note> notes)
        {
            if(notes == null)
            {
                return null;
            }

            return notes
                .OrderBy(n => n.Sequence)
                .Aggregate((string) null, (c, n) => c == null ? n.Text : string.Format("{0} {1}", c, n.Text));
        }
    }
}