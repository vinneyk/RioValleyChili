using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.NotebookService
{
    public interface ICreateNoteParameters : IUserIdentifiable
    {
        string Text { get; }
    }
}