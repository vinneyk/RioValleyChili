using System;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotAttributeReturn : LotAttributeBaseReturn, ILotAttributeReturn
    {
        public DateTime AttributeDate { get; internal set; }
        public bool Computed { get; internal set; }
    }

    internal class LotAttributeParameterReturn : LotAttributeBaseReturn, ILotAttributeParameter
    {
        public string AttributeNameKey_ShortName { get { return Key; } }
        public bool NameActive { get; internal set; }
    }

    internal class LotAttributeBaseReturn
    {
        public string Key { get; internal set; }
        public string Name { get; internal set; }
        public double Value { get; internal set; }
    }
}