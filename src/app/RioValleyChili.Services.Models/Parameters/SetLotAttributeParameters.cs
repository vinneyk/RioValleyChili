using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetLotAttributeParameters : ISetLotAttributeParameters
    {
        public string UserToken { get; set; }
        public string LotKey { get; set; }
        public string Notes { get; set; }
        public IDictionary<string, IAttributeValueParameters> Attributes { get; set; }
        public bool OverrideOldContextLotAsCompleted { get; set; }
    }
}