using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class NotebookReturn : INotebookReturn
    {
        public string NotebookKey { get { return NotebookKeyReturn.NotebookKey; } }

        public IEnumerable<INoteReturn> Notes { get; internal set; }

        internal NotebookKeyReturn NotebookKeyReturn { get; set; }
    }
}