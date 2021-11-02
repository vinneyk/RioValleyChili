using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class AddLotAttributesParameters : IAddLotAttributesParameters
    {
        public string UserToken { get; set; }
        public IEnumerable<string> LotKeys { get; set; }
        public IDictionary<string, IAttributeValueParameters> Attributes { get; set; }
        public bool OverrideOldContextLotAsCompleted { get; set; }
    }
}