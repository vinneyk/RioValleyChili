using RioValleyChili.Services.Interfaces.Parameters.NotebookService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class CreateNoteParameters : ICreateNoteParameters
    {
        public string UserToken { get; set; }
        public string Text { get; set; }
    }
}