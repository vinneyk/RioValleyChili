using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Utilities.Commands.LotCommands
{
    internal class SetLotAttributesParameters
    {
        internal Employee Employee { get; set; }
        internal List<AttributeName> AttributeNames { get; set; }

        internal Lot Lot { get; set; }
        internal List<LotAttributeDefect> LotAttributeDefects { get; set; }
        internal List<PickedInventoryItem> LotUnarchivedPickedInventoryItems { get; set; }
        internal DateTime TimeStamp { get; set; }

        internal Dictionary<AttributeNameKey, IAttributeValueParameters> NewAttributes
        {
            get { return _newAttributes; }
            set { _newAttributes = value == null ? null : value.ToDictionary(v => v.Key, v => (IAttributeValueParameters)(new AttributeValueParameters(v.Value))); }
        }
        private Dictionary<AttributeNameKey, IAttributeValueParameters> _newAttributes;

        private class AttributeValueParameters : IAttributeValueParameters
        {
            public ILotAttributeInfoParameters AttributeInfo { get; private set; }
            public IDefectResolutionParameters Resolution { get; private set; }

            public AttributeValueParameters(IAttributeValueParameters parameters)
            {
                AttributeInfo = parameters.AttributeInfo;
                Resolution = parameters.Resolution;
            }
        }
    }
}