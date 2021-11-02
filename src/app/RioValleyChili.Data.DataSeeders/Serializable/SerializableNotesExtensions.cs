using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Serializable
{
    public static class SerializableNotesExtensions
    {
        public static List<Note> ToNotes(this IEnumerable<SerializableNote> serializableNotes, INotebookKey notebookKey)
        {
            return serializableNotes != null ? serializableNotes.Select((note, i) => note.ToNote(notebookKey, i + 1)).ToList() : new List<Note>();
        }
    }
}