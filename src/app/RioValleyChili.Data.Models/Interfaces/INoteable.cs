using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models.Interfaces
{
    public interface INoteable : INotebookKey
    {
        Notebook Notebook { get; }
    }
}