using RioValleyChili.Services.Interfaces.Parameters.NotebookService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class UpdateNoteParameters : IUpdateNoteParameters
    {
        public string UserToken { get; set; }
        public string Text { get; set; }
    }
}