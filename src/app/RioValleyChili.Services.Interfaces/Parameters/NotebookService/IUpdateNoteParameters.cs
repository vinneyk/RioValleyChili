using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.NotebookService
{
    public interface IUpdateNoteParameters : IUserIdentifiable
    {
        string Text { get; }
    }
}