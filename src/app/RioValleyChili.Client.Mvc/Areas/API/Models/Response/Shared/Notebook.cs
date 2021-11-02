using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared
{
    public class Notebook
    {
        public string NotebookKey { get; set; }
        public IEnumerable<Note> Notes { get; set; }
    }
}