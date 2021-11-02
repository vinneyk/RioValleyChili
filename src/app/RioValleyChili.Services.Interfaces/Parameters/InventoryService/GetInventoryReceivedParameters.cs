using System;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Parameters.InventoryService
{
    public class GetInventoryReceivedParameters
    {
        public string LotKey { get; set; }
        public LotTypeEnum? LotType { get; set; }
        public DateTime? DateReceivedStart { get; set; }
        public DateTime? DateReceivedEnd { get; set; }
    }
}