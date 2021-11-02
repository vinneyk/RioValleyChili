using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Interfaces.Parameters.NotebookService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Notebook
{
    internal class UpdateNoteCommand
    {
        private readonly INotebookUnitOfWork _notebookUnitOfWork;

        internal UpdateNoteCommand(INotebookUnitOfWork notebookUnitOfWork)
        {
            if(notebookUnitOfWork == null) { throw new ArgumentNullException("notebookUnitOfWork"); }
            _notebookUnitOfWork = notebookUnitOfWork;
        }

        internal IResult Execute(NoteKey noteKey, DateTime timestamp, IUpdateNoteParameters parameters)
        {
            if(noteKey == null) { throw new ArgumentNullException("noteKey "); }
            if(timestamp == null) { throw new ArgumentNullException("timestamp"); }
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var note = _notebookUnitOfWork.NoteRepository.FindByKey(noteKey);
            if(note == null)
            {
                return new InvalidResult(string.Format(UserMessages.NoteNotFound, noteKey.KeyValue));
            }

            var employeeResult = new GetEmployeeCommand(_notebookUnitOfWork).GetEmployee(parameters);
            if(!employeeResult.Success)
            {
                return employeeResult;
            }

            note.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            note.TimeStamp = timestamp;
            note.Text = parameters.Text;

            return new SuccessResult();
        }
    }
}