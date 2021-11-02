using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Newtonsoft.Json;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders.Helpers;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.DataSeeders.Utilities.Interfaces;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class LotHistoryEntityObjectMother : EntityMotherLogBase<LotHistory, LotHistoryEntityObjectMother.CallbackParameters>
    {
        private readonly NewContextHelper _newContextHelper;

        public LotHistoryEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback)
        {
            _newContextHelper = new NewContextHelper(newContext);
        }

        public enum EntityTypes
        {
            LotAttributeHistory
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<LotHistory> BirthRecords()
        {
            _loadCount.Reset();

            foreach(var lot in SelectRecords(OldContext))
            {
                LotKey lotKey;
                if(!LotNumberParser.ParseLotNumber(lot.Lot, out lotKey))
                {
                    Log(new CallbackParameters(CallbackReason.InvalidLotNumber)
                        {
                            Lot = lot
                        });
                    continue;
                }

                if(!_newContextHelper.LotLoaded(lotKey))
                {
                    Log(new CallbackParameters(CallbackReason.LotNotLoaded)
                        {
                            Lot = lot
                        });
                    continue;
                }

                var sequence = 0;
                var computedHistory = lot.History.Where(h => h.TestDate == null).OrderBy(h => h.ArchiveDate).FirstOrDefault();

                DateTime? lotHistoryTimeStamp = null;
                foreach(var history in lot.History.OrderBy(h => h.ArchiveDate))
                {
                    _loadCount.AddRead(EntityTypes.LotAttributeHistory);

                    if(history.EmployeeID == null)
                    {
                        Log(new CallbackParameters(CallbackReason.EmployeeIDIsNull)
                            {
                                History = history
                            });
                    }
                    var employeeId = history.TesterID ?? history.EmployeeID ?? _newContextHelper.DefaultEmployee.EmployeeId;

                    string holdDescription;
                    var hold = LotHelper.GetHoldStatus((LotStat?) history.LotStat, out holdDescription);
                    
                    var serializedData = new SerializedLotHistory
                        {
                            TimeStamp = lotHistoryTimeStamp ?? history.EntryDate.ConvertLocalToUTC() ?? lotKey.LotKey_DateCreated,
                            QualityStatus = DetermineLotQualityStatus((LotStat?)history.LotStat),
                            ProductionStatus = LotProductionStatus.Produced,
                            Hold = hold,
                            HoldDescription = holdDescription,
                            LoBac = history.LoBac,

                            Attributes = history.GetAttributes(computedHistory)
                                .Where(a => a.Value != null)
                                .Select(attribute => new SerializedLotHistoryAttribute
                                    {
                                        AttributeShortName = attribute.AttributeNameKey,
                                        AttributeValue = (double)attribute.Value,
                                        AttributeDate = (history.TestDate ?? history.ArchiveDate).Date,
                                        Computed = attribute.Computed
                                    }).ToList()
                        };
                    
                    yield return new LotHistory
                        {
                            LotTypeId = lotKey.LotKey_LotTypeId,
                            LotDateCreated = lotKey.LotKey_DateCreated,
                            LotDateSequence = lotKey.LotKey_DateSequence,
                            Sequence = sequence++,

                            TimeStamp = (lotHistoryTimeStamp = history.ArchiveDate.ConvertLocalToUTC()).Value,
                            EmployeeId = employeeId,

                            Serialized = JsonConvert.SerializeObject(serializedData)
                        };
                    _loadCount.AddLoaded(EntityTypes.LotAttributeHistory);
                }
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        private LotQualityStatus DetermineLotQualityStatus(LotStat? lotStat)
        {
            if(lotStat == LotStat.Rejected)
            {
                return LotQualityStatus.Rejected;
            }

            if(lotStat.IsAcceptable())
            {
                return LotQualityStatus.Released;
            }
            
            if(lotStat == LotStat.Contaminated)
            {
                return LotQualityStatus.Contaminated;
            }
            
            return LotQualityStatus.Pending;
        }

        private static IEnumerable<LotDTO> SelectRecords(ObjectContext objectContext)
        {
            return objectContext.CreateObjectSet<tblLotAttributeHistory>().AsNoTracking()
                .GroupBy(h => h.Lot)
                .Select(g => new LotDTO
                    {
                        Lot = g.Key,
                        History = g
                    });
        }

        public class LotDTO
        {
            public int Lot { get; set; }
            public IEnumerable<tblLotAttributeHistory> History { get; set; }
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            StringTruncated,
            InvalidLotNumber,
            LotNotLoaded,
            EmployeeIDIsNull
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public LotDTO Lot { get; set; }
            public tblLotAttributeHistory History { get; set; }

            protected override CallbackReason ExceptionReason { get { return LotHistoryEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return LotHistoryEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return LotHistoryEntityObjectMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case LotHistoryEntityObjectMother.CallbackReason.EmployeeIDIsNull: return ReasonCategory.Informational;

                    case LotHistoryEntityObjectMother.CallbackReason.InvalidLotNumber:
                    case LotHistoryEntityObjectMother.CallbackReason.LotNotLoaded: return ReasonCategory.RecordSkipped;
                }

                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}