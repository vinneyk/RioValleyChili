using RioValleyChili.Services.Interfaces.Returns.ProductionService;

namespace RioValleyChili.Services.Models.DataTransferObjects
{
    public class WorkTypeDTO : IWorkTypeReturn
    {
        public string WorkTypeKey { get; set; }

        public string Description { get; set; }
    }
}
