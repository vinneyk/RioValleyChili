using System;
using System.Collections.Generic;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Notebook
{
    internal class CreateNotebookCommand
    {
        private readonly INotebookUnitOfWork _notebookUnitOfWork;

        internal CreateNotebookCommand(INotebookUnitOfWork notebookUnitOfWork)
        {
            if(notebookUnitOfWork == null) { throw new ArgumentNullException("notebookUnitOfWork"); }
            _notebookUnitOfWork = notebookUnitOfWork;
        }

        internal IResult<Data.Models.Notebook> Execute(DateTime date)
        {
            var currentDate = date.Date;
            var nextSequence = new EFUnitOfWorkHelper(_notebookUnitOfWork).GetNextSequence<Data.Models.Notebook>(n => n.Date == currentDate, n => n.Sequence);

            var newNotebook = _notebookUnitOfWork.NotebookRepository.Add(new Data.Models.Notebook
                {
                    Date = currentDate,
                    Sequence = nextSequence
                });

            return new SuccessResult<Data.Models.Notebook>(newNotebook);
        }

        internal IResult<Data.Models.Notebook> Execute(DateTime timeStamp, Employee employee, IEnumerable<string> notes)
        {
            var notebookResult = Execute(timeStamp);
            if(notebookResult.Success)
            {
                if(notes != null)
                {
                    var createNoteCommand = new CreateNoteCommand(_notebookUnitOfWork);
                    foreach(var note in notes)
                    {
                        var noteResult = createNoteCommand.Execute(notebookResult.ResultingObject, timeStamp, employee, note);
                        if(!noteResult.Success)
                        {
                            return noteResult.ConvertTo((Data.Models.Notebook)null);
                        }
                    }
                }
            }

            return notebookResult;
        }
    }
}
