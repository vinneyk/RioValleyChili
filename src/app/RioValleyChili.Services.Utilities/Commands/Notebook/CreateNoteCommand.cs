using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.NotebookService;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Notebook
{
    internal class CreateNoteCommand
    {
        private readonly INotebookUnitOfWork _notebookUnitOfWork;

        internal CreateNoteCommand(INotebookUnitOfWork notebookUnitOfWork)
        {
            if(notebookUnitOfWork == null) { throw new ArgumentNullException("notebookUnitOfWork "); }
            _notebookUnitOfWork = notebookUnitOfWork;
        }

        internal IResult<Note> Execute(INotebookKey notebookKey, DateTime timestamp, ICreateNoteParameters parameters)
        {
            if(notebookKey == null) { throw new ArgumentNullException("notebookKey"); }
            if(timestamp == null) { throw new ArgumentNullException("timestamp"); }
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(_notebookUnitOfWork).GetEmployee(parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo((Note) null);
            }

            var notebookKeyFind = new NotebookKey(notebookKey);
            var notebook = _notebookUnitOfWork.NotebookRepository.FindByKey(notebookKeyFind);
            if(notebook == null)
            {
                return new InvalidResult<Note>(null, string.Format(UserMessages.NotebookNotFound, notebookKeyFind));
            }
            return Execute(notebook, timestamp, employeeResult.ResultingObject, parameters.Text);
        }

        internal IResult<Note> Execute(Data.Models.Notebook notebook, DateTime timestamp, Employee employee, string text)
        {
            if(notebook == null) { throw new ArgumentNullException("notebook"); }
            if(timestamp == null) { throw new ArgumentNullException("timestamp"); }
            if(employee == null) { throw new ArgumentNullException("employee"); }
            if(text == null) { throw new ArgumentNullException("text"); }

            var newSequence = new EFUnitOfWorkHelper(_notebookUnitOfWork).GetNextSequence<Note>(n => n.NotebookDate == notebook.NotebookKey_Date && n.NotebookSequence == notebook.NotebookKey_Sequence, n => n.Sequence);
            var newNote = _notebookUnitOfWork.NoteRepository.Add(new Note
                {
                    NotebookDate = notebook.Date,
                    NotebookSequence = notebook.Sequence,
                    Sequence = newSequence,

                    EmployeeId = employee.EmployeeId,
                    TimeStamp = timestamp,
                    Text = text
                });

            return new SuccessResult<Note>(newNote);
        }
    }
}