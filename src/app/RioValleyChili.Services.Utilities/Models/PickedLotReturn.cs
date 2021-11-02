using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class PickedLotReturn : IWeightedLotAttributes
    {
        internal LotKeyReturn LotKey { get; set; }

        internal string ToteKey { get; set; }

        public double LotWeight { get; internal set; }

        List<ILotAttributeParameter> IWeightedLotAttributes.LotAttributes
        {
            get { return _lotAttributes ?? (_lotAttributes = LotAttributes.ToList()); }
        }
        private List<ILotAttributeParameter> _lotAttributes;

        internal IEnumerable<ILotAttributeParameter> LotAttributes { get; set; }

        public override int GetHashCode()
        {
            return ((LotKey != null ? LotKey.LotKey.GetHashCode() : 0) * 397) ^ (ToteKey != null ? ToteKey.GetHashCode() : 0);
        }
    }
}