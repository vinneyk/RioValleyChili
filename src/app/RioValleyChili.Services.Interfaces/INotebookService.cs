using RioValleyChili.Services.Interfaces.Parameters.NotebookService;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface INotebookService
    {
        IResult<INotebookReturn> GetNotebook(string notebookKey);

        IResult<INoteReturn> AddNote(string notebookKey, ICreateNoteParameters note);

        IResult UpdateNote(string noteKey, IUpdateNoteParameters note);

        IResult DeleteNote(string noteKey);
    }
}