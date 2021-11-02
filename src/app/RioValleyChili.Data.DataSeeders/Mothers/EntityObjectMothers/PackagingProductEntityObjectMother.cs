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
    public class PackagingProductEntityObjectMother : EntityMotherLogBase<PackagingProduct, PackagingProductEntityObjectMother.CallbackParameters>
    {
        private readonly RioValleyChiliDataContext _newContext;

        public PackagingProductEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            _newContext = newContext;
        }

        private enum EntityTypes
        {
            PackagingProduct,
            Product
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<PackagingProduct> BirthRecords()
        {
            _loadCount.Reset();
            
            var productId = _newContext.NextIdentity<Product>(p => p.Id);

            foreach(var package in OldContext.CreateObjectSet<tblPackaging>().ToList())
            {
                _loadCount.AddRead(EntityTypes.PackagingProduct);
                _loadCount.AddRead(EntityTypes.Product);
                _loadCount.AddLoaded(EntityTypes.PackagingProduct);
                _loadCount.AddLoaded(EntityTypes.Product);

                yield return new PackagingProduct
                    {
                        Id = productId,
                        Product = new Product
                            {
                                Id = productId,
                                ProductType = ProductTypeEnum.Packaging,
                                IsActive = package.InActive == false,
                                Name = package.Packaging,
                                ProductCode = package.PkgID.ToString(CultureInfo.InvariantCulture),
                            },
                        Weight = (double) (package.NetWgt ?? 0),
                        PackagingWeight = (double) (package.PkgWgt ?? 0),
                        PalletWeight = (double) (package.PalletWgt ?? 0)
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
            protected override CallbackReason ExceptionReason { get { return PackagingProductEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return PackagingProductEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return PackagingProductEntityObjectMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(DataStringPropertyHelper.Result stringResult) : base(stringResult) { }
        }
    }
}
