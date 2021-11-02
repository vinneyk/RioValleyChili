using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class InventoryTreatmentMother : EntityMotherLogBase<InventoryTreatment, InventoryTreatmentMother.CallbackParameters>
    {
        public InventoryTreatmentMother(ObjectContext oldContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback) { }

        private enum EntityType
        {
            InventoryTreatment
        }

        private readonly MotherLoadCount<EntityType> _loadCount = new MotherLoadCount<EntityType>();

        protected override IEnumerable<InventoryTreatment> BirthRecords()
        {
            _loadCount.Reset();

            foreach(var treatment in OldContext.CreateObjectSet<tblTreatment>().ToList())
            {
                _loadCount.AddRead(EntityType.InventoryTreatment);
                _loadCount.AddLoaded(EntityType.InventoryTreatment);
                yield return new InventoryTreatment
                    {
                        Id = treatment.TrtmtID,
                        LongName = treatment.TrtmtDesc,
                        ShortName = treatment.Trtmt,
                        Active = treatment.InActive == false,
                    };
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            StringTruncated
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            protected override CallbackReason ExceptionReason { get { return InventoryTreatmentMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return InventoryTreatmentMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return InventoryTreatmentMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
        }
    }
}