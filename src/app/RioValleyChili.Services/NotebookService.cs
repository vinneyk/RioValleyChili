using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.NotebookService;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class NotebookService : INotebookService
    {
        private readonly NotebookServiceProvider _notebookServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public NotebookService(NotebookServiceProvider notebookServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(notebookServiceProvider == null) { throw new ArgumentNullException("notebookServiceProvider"); }
            _notebookServiceProvider = notebookServiceProvider;

            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }
            _exceptionLogger = exceptionLogger;
        }

        public IResult<INotebookReturn> GetNotebook(string notebookKey)
        {
            try
            {
                return _notebookServiceProvider.GetNotebook(notebookKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<INotebookReturn>(null, ex.Message);
            }
        }

        public IResult<INoteReturn> AddNote(string notebookKey, ICreateNoteParameters note)
        {
            try
            {
                return _notebookServiceProvider.AddNote(notebookKey, note);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<INoteReturn>(null, ex.Message);
            }
        }

        public IResult UpdateNote(string noteKey, IUpdateNoteParameters note)
        {
            try
            {
                return _notebookServiceProvider.UpdateNote(noteKey, note);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }

        public IResult DeleteNote(string noteKey)
        {
            try
            {
                return _notebookServiceProvider.DeleteNote(noteKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.Message);
            }
        }
    }
}