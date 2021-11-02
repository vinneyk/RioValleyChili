using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.NotebookService
{
    public interface INotebookReturn
    {
        string NotebookKey { get; }

        IEnumerable<INoteReturn> Notes { get; } 
    }
}