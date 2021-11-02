using RioValleyChili.Services.Interfaces.Parameters.NotebookService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateNoteParameters : ICreateNoteParameters
    {
        public string UserToken { get; set; }
        public string Text { get; set; }
    }
}