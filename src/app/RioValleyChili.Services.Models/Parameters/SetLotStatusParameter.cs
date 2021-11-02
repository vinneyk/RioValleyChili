using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetLotStatusParameter : ISetLotStatusParameters
    {
        public string LotKey { get; set; }
        public LotQualityStatus QualityStatus { get; set; }

        string IUserIdentifiable.UserToken { get; set; }
    }
}