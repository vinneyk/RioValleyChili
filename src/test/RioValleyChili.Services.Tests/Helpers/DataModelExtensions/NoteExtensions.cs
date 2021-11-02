using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class NoteExtensions
    {
        internal static void AssertEqual(this Note note, INoteReturn noteReturn)
        {
            if(note == null) { throw new ArgumentNullException("note"); }
            if(noteReturn == null) { throw new ArgumentNullException("noteReturn"); }

            Assert.AreEqual(new NoteKey(note).KeyValue, noteReturn.NoteKey);
            Assert.AreEqual(note.Employee.UserName, noteReturn.CreatedByUser);
            Assert.LessOrEqual(Math.Abs((note.TimeStamp - noteReturn.NoteDate).Milliseconds), 7);
            Assert.AreEqual(note.Text, noteReturn.Text);
        }

        internal static Note SetNote(this Note note, INotebookKey notebookKey, string text = null)
        {
            if(note == null) { throw new ArgumentNullException("note"); }
            
            if(notebookKey != null)
            {
                note.Notebook = null;
                note.NotebookDate = notebookKey.NotebookKey_Date;
                note.NotebookSequence = notebookKey.NotebookKey_Sequence;
            }

            if(text != null)
            {
                note.Text = text;
            }

            return note;
        }
    }
}