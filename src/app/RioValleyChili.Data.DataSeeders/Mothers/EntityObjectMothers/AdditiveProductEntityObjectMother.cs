using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Mothers.PocoMothers;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class AdditiveProductEntityObjectMother : EntityMotherLogBase<AdditiveProduct, AdditiveProductEntityObjectMother.CallbackParameters>
    {
        private readonly RioValleyChiliDataContext _newContext;

        public AdditiveProductEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> logginParameters) : base(oldContext, logginParameters)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            _newContext = newContext;
        }

        private enum EntityTypes 
        {
            AdditiveProduct,
            Product
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<AdditiveProduct> BirthRecords()
        {
            _loadCount.Reset();
            
            var productId = _newContext.NextIdentity<Product>(p => p.Id);

            foreach(var additiveProduct in OldContext.CreateObjectSet<tblProduct>().ToList().Where(p => p.PTypeID == (int)LotTypeEnum.Additive && p.ProdGrpID != 11 && p.ProdGrpID != 7))
            {
                _loadCount.AddRead(EntityTypes.AdditiveProduct);
                _loadCount.AddRead(EntityTypes.Product);
                _loadCount.AddLoaded(EntityTypes.AdditiveProduct);
                _loadCount.AddLoaded(EntityTypes.Product);

                yield return new AdditiveProduct
                    {
                        Id = productId,
                        Product = new Product
                            {
                                Id = productId,
                                ProductType = ProductTypeEnum.Additive,
                                IsActive = additiveProduct.InActive == false,
                                Name = additiveProduct.Product,
                                ProductCode = additiveProduct.ProdID.ToString(CultureInfo.InvariantCulture)
                            },
                        AdditiveTypeId = additiveProduct.IngrID ?? AdditiveTypeMother.Unknown.Id
                    };

                productId += 1;
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
            protected override CallbackReason ExceptionReason { get { return AdditiveProductEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return AdditiveProductEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return AdditiveProductEntityObjectMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case AdditiveProductEntityObjectMother.CallbackReason.Exception: return ReasonCategory.Error;
                }
                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}
