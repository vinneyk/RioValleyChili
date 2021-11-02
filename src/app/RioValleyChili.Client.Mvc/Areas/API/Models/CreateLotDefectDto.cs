using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class CreateLotDefectDto : ICreateLotDefectParameters
    {
        public string UserToken { get; set; }
        public DefectTypeEnum DefectType { get; set; }
        public string LotKey { get; set; }
        public string Description { get; set; }
    }
}