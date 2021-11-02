using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class LotHoldDto : ILotHold
    {
        public LotHoldType HoldType { get; set; }
        public string Description { get; set; }
    }
}