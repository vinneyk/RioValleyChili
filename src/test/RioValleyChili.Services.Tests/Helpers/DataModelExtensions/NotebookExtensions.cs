using System;
using System.Linq;
using LinqKit;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class NotebookExtensions
    {
        internal static void AssertEqual(this Notebook notebook, INotebookReturn notebookReturn)
        {
            if(notebook == null) { throw new ArgumentNullException("notebook"); }
            if(notebookReturn == null) { throw new ArgumentNullException("notebookReturn"); }

            Assert.AreEqual(new NotebookKey(notebook).KeyValue, notebookReturn.NotebookKey);
            if(notebook.Notes != null)
            {
                notebook.Notes.ForEach(n =>
                    {
                        var noteKey = new NoteKey(n);
                        n.AssertEqual(notebookReturn.Notes.Single(r => r.NoteKey == noteKey.KeyValue));
                    });
            }
            else if(notebookReturn.Notes != null)
            {
                Assert.IsEmpty(notebookReturn.Notes);
            }
        }
    }
}