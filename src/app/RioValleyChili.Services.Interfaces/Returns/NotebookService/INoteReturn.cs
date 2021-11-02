using System;

namespace RioValleyChili.Services.Interfaces.Returns.NotebookService
{
    public interface INoteReturn
    {
        string NoteKey { get; }
        DateTime NoteDate { get; }
        string CreatedByUser { get; }
        int Sequence { get; }
        string Text { get; }
    }
}