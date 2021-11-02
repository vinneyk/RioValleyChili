using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class AddLotAttributeParameters
    {
        public IAddLotAttributesParameters Parameters { get; set; }

        public List<LotKey> LotKeys { get; set; }
        public IDictionary<AttributeNameKey, IAttributeValueParameters> Attributes { get; set; }
    }
}