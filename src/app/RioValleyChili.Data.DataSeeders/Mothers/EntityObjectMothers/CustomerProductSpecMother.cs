using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class CustomerProductSpecMother : EntityMotherLogBase<CustomerProductAttributeRange, CustomerProductSpecMother.CallbackParameters>
    {
        private readonly NewContextHelper _newContextHelper;

        public CustomerProductSpecMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback)
        {
            _newContextHelper = new NewContextHelper(newContext);
        }

        private enum EntityTypes
        {
            CustomerProductAttributeRange
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<CustomerProductAttributeRange> BirthRecords()
        {
            _loadCount.Reset();

            foreach(var oldContextSpec in OldContext.CreateObjectSet<SerializedCustomerProdSpecs>().ToList())
            {
                var chileProduct = _newContextHelper.GetChileProduct(oldContextSpec.ProdID);
                if(chileProduct == null)
                {
                    Log(new CallbackParameters(CallbackReason.ChileProductNotLoaded)
                        {
                            Spec = oldContextSpec
                        });
                    continue;
                }

                var customer = _newContextHelper.GetCompany(oldContextSpec.Company_IA, CompanyType.Customer);
                if(customer == null)
                {
                    Log(new CallbackParameters(CallbackReason.CustomerNotLoaded)
                        {
                            Spec = oldContextSpec
                        });
                    continue;
                }

                var dataModels = SerializableCustomerSpec.Deserialize(oldContextSpec.Serialized).ToDataModels(chileProduct, customer);
                foreach(var dataModel in dataModels)
                {
                    _loadCount.AddRead(EntityTypes.CustomerProductAttributeRange);
                    _loadCount.AddLoaded(EntityTypes.CustomerProductAttributeRange);
                    yield return dataModel;
                }
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            StringTruncated,
            ChileProductNotLoaded,
            CustomerNotLoaded
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public CallbackParameters() { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) {}
            public CallbackParameters(string summaryMessage) : base(summaryMessage) {}
            public CallbackParameters(DataStringPropertyHelper.Result stringResult) : base(stringResult) {}

            public SerializedCustomerProdSpecs Spec { get; set; }

            protected override CallbackReason ExceptionReason { get { return CustomerProductSpecMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return CustomerProductSpecMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return CustomerProductSpecMother.CallbackReason.StringTruncated; } }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case CustomerProductSpecMother.CallbackReason.ChileProductNotLoaded:
                    case CustomerProductSpecMother.CallbackReason.CustomerNotLoaded:
                        return ReasonCategory.RecordSkipped;
                }

                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}