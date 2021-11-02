using System;
using RioValleyChili.Business.Core.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public class InterWarehouseOrderCommandParameters
    {
        public string User { get; set; }

        public DateTime TimeStamp { get; set; }

        public FacilityKey DestinationFacilityKey { get; set; }
    }
}