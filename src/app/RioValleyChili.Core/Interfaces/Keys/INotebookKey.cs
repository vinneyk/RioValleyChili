using System;

namespace RioValleyChili.Core.Interfaces.Keys
{
    public interface INotebookKey
    {
        DateTime NotebookKey_Date { get; }

        int NotebookKey_Sequence { get; }
    }
}