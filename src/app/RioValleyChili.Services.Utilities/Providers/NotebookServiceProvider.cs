using System;
using System.Linq;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Interfaces.Parameters.NotebookService;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Commands.Notebook;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Core;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class NotebookServiceProvider : IUnitOfWorkContainer<EFRVCUnitOfWork>
    {
        EFRVCUnitOfWork IUnitOfWorkContainer<EFRVCUnitOfWork>.UnitOfWork { get { return _notebookUnitOfWork as EFRVCUnitOfWork; } }
        private readonly INotebookUnitOfWork _notebookUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public NotebookServiceProvider(INotebookUnitOfWork notebookUnitOfWork, ITimeStamper timeStamper)
        {
            if(notebookUnitOfWork == null) { throw new ArgumentNullException("notebookUnitOfWork"); }
            _notebookUnitOfWork = notebookUnitOfWork;

            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }
            _timeStamper = timeStamper;
        }

        public IResult<INotebookReturn> GetNotebook(string notebookKey)
        {
            if(notebookKey == null) { throw new ArgumentNullException("notebookKey"); }

            var keyResult = KeyParserHelper.ParseResult<INotebookKey>(notebookKey);
            if(!keyResult.Success)
            {
                return keyResult.ConvertTo((INotebookReturn) null);
            }
            var key = new NotebookKey(keyResult.ResultingObject);

            var predicate = key.FindByPredicate;
            var select = NotebookProjectors.Select();
            
            var notebook = _notebookUnitOfWork.NotebookRepository.Filter(predicate).AsExpandable().Select(select).FirstOrDefault();
            if(notebook == null)
            {
                return new InvalidResult<INotebookReturn>(null, string.Format(UserMessages.NotebookNotFound, notebookKey));
            }

            return new SuccessResult<INotebookReturn>(notebook);
        }

        [SynchronizeOldContext(NewContextMethod.Notebook)]
        public IResult<INoteReturn> AddNote(string notebookKey, ICreateNoteParameters parameters)
        {
            if(notebookKey == null) { throw new ArgumentNullException("notebookKey"); }

            var keyResult = KeyParserHelper.ParseResult<INotebookKey>(notebookKey);
            if(!keyResult.Success)
            {
                return keyResult.ConvertTo<INoteReturn>();
            }
            var key = new NotebookKey(keyResult.ResultingObject);

            var createNoteResult = new CreateNoteCommand(_notebookUnitOfWork).Execute(key, _timeStamper.CurrentTimeStamp, parameters);
            if(!createNoteResult.Success)
            {
                return createNoteResult.ConvertTo<INoteReturn>();
            }

            _notebookUnitOfWork.Commit();
            
            return SyncParameters.Using(new SuccessResult<INoteReturn>(new[] { createNoteResult.ResultingObject }.Select(NoteProjectors.Select().Compile()).FirstOrDefault()), key);
        }

        [SynchronizeOldContext(NewContextMethod.Notebook)]
        public IResult UpdateNote(string noteKey, IUpdateNoteParameters parameters)
        {
            if(noteKey == null) { throw new ArgumentNullException("noteKey"); }
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var keyResult = KeyParserHelper.ParseResult<INoteKey>(noteKey);
            if(!keyResult.Success)
            {
                return keyResult;
            }
            var parsedNoteKey = new NoteKey(keyResult.ResultingObject);

            var updateNoteResult = new UpdateNoteCommand(_notebookUnitOfWork).Execute(parsedNoteKey, _timeStamper.CurrentTimeStamp, parameters);
            if(!updateNoteResult.Success)
            {
                return updateNoteResult;
            }

            _notebookUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new NotebookKey(parsedNoteKey));
        }

        [SynchronizeOldContext(NewContextMethod.Notebook)]
        public IResult DeleteNote(string noteKey)
        {
            if(noteKey == null) { throw new ArgumentNullException("noteKey"); }

            var keyResult = KeyParserHelper.ParseResult<INoteKey>(noteKey);
            if(!keyResult.Success)
            {
                return keyResult;
            }
            var key = new NoteKey(keyResult.ResultingObject);

            var note = _notebookUnitOfWork.NoteRepository.FindByKey(key);
            if(note == null)
            {
                return new InvalidResult(string.Format(UserMessages.NoteNotFound, noteKey));
            }

            _notebookUnitOfWork.NoteRepository.Remove(note);
            _notebookUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new NotebookKey(key));
        }
    }
}