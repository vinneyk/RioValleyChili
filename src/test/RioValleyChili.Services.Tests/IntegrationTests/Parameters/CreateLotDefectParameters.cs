using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class CreateLotDefectParameters : ICreateLotDefectParameters
    {
        public string UserToken { get; set; }
        public DefectTypeEnum DefectType { get; set; }
        public string LotKey { get; set; }
        public string Description { get; set; }
    }
}