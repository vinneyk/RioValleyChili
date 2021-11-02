using System;
using System.Linq;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Notebook
{
    internal class DeleteNotebookCommand
    {
        private readonly INotebookUnitOfWork _notebookUnitOfWork;

        internal DeleteNotebookCommand(INotebookUnitOfWork notebookUnitOfWork)
        {
            if(notebookUnitOfWork == null) { throw new ArgumentNullException("notebookUnitOfWork"); }
            _notebookUnitOfWork = notebookUnitOfWork;
        }

        internal IResult Delete(Data.Models.Notebook notebook)
        {
            var notes = notebook.Notes.ToList();
            foreach(var note in notes)
            {
                _notebookUnitOfWork.NoteRepository.Remove(note);
            }
            _notebookUnitOfWork.NotebookRepository.Remove(notebook);

            return new SuccessResult();
        }
    }
}