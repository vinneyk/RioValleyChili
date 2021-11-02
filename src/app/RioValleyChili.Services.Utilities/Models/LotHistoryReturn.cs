using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotHistoryReturn : ILotHistoryReturn
    {
        public string LotKey { get { return LotKeyReturn.LotKey; } }
        public DateTime Timestamp { get; internal set; }
        public IUserSummaryReturn Employee { get; internal set; }

        public bool LoBac { get; internal set; }
        public LotHoldType? HoldType { get; internal set; }
        public string HoldDescription { get; internal set; }
        public LotQualityStatus QualityStatus { get; internal set; }
        public LotProductionStatus ProductionStatus { get; internal set; }
        public IInventoryProductReturn Product { get; internal set; }
        public IEnumerable<ILotHistoryAttributeReturn> Attributes { get; internal set; }
        public IEnumerable<ILotHistoryRecordReturn> History
        {
            get
            {
                return _history ?? (_history = SerializedHistory
                    .OrderBy(h => h.Timestamp)
                    .Select(h =>
                        {
                            var deserialized = JsonConvert.DeserializeObject<SerializedLotHistory>(h.Serialized);

                            return new LotHistoryRecordReturn
                                {
                                    Timestamp = deserialized.TimeStamp,
                                    Employee = h.Employee,

                                    LoBac = deserialized.LoBac,
                                    HoldType = deserialized.Hold,
                                    HoldDescription = deserialized.HoldDescription,
                                    QualityStatus = deserialized.QualityStatus,
                                    ProductionStatus = deserialized.ProductionStatus,

                                    Attributes = (deserialized.Attributes ?? new SerializedLotHistoryAttribute[0])
                                        .Select(a => new LotHistoryAttributeReturn
                                            {
                                                AttributeShortName = a.AttributeShortName,
                                                Value = a.AttributeValue,
                                                AttributeDate = a.AttributeDate,
                                                Computed = a.Computed
                                            }).ToList()
                                };
                        }).ToList());
            }
        }

        internal LotKeyReturn LotKeyReturn { get; set; }
        internal IEnumerable<LotSerializedHistoryReturn> SerializedHistory { get; set; }

        private IEnumerable<ILotHistoryRecordReturn> _history;
    }

    internal class LotHistoryAttributeReturn : ILotHistoryAttributeReturn
    {
        public string AttributeShortName { get; internal set; }
        public double Value { get; internal set; }
        public DateTime AttributeDate { get; internal set; }
        public bool Computed { get; internal set; }
    }

    internal class LotHistoryRecordReturn : ILotHistoryRecordReturn
    {
        public DateTime Timestamp { get; internal set; }
        public IUserSummaryReturn Employee { get; internal set; }

        public bool LoBac { get; internal set; }
        public LotHoldType? HoldType { get; internal set; }
        public string HoldDescription { get; internal set; }
        public LotQualityStatus QualityStatus { get; internal set; }
        public LotProductionStatus ProductionStatus { get; internal set; }

        public IEnumerable<ILotHistoryAttributeReturn> Attributes { get; internal set; }
    }

    internal class LotSerializedHistoryReturn
    {
        public DateTime Timestamp { get; internal set; }
        public IUserSummaryReturn Employee { get; internal set; }

        internal string Serialized { get; set; }
    }
}