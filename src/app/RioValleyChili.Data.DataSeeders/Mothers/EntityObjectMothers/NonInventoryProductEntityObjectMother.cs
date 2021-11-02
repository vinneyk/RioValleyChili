using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class NonInventoryProductEntityObjectMother : EntityMotherLogBase<Product, NonInventoryProductEntityObjectMother.CallbackParameters>
    {
        public NonInventoryProductEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            _newContext = newContext;
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            StringTruncated
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }

            protected override CallbackReason ExceptionReason { get { return NonInventoryProductEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return NonInventoryProductEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return NonInventoryProductEntityObjectMother.CallbackReason.StringTruncated; } }
        }

        protected override IEnumerable<Product> BirthRecords()
        {
            _loadCount.Reset();
            _productId = _newContext.NextIdentity<Product>(p => p.Id);

            foreach(var product in SelectProductsToLoad())
            {
                _loadCount.AddRead(EntityType.Product);

                yield return new Product
                    {
                        Id = _productId,
                        IsActive = product.InActive == false,
                        Name = product.Product,
                        ProductCode = product.ProdID.ToString(CultureInfo.InvariantCulture),
                        ProductType = ProductTypeEnum.NonInventory
                    };

                _loadCount.AddLoaded(EntityType.Product);

                _productId += 1;
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        private int _productId;
        private readonly MotherLoadCount<EntityType> _loadCount = new MotherLoadCount<EntityType>();
        private RioValleyChiliDataContext _newContext;

        private List<tblProduct> SelectProductsToLoad()
        {
            return OldContext.CreateObjectSet<tblProduct>().Where(p => p.ProdGrpID == 7).ToList();
        }

        private enum EntityType
        {
            Product
        }
    }
}