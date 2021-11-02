using System;
using System.Data.Entity.Core.Objects;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class PackagingLotInventoryItemEntityObjectMother : LotInventoryEntityObjectMotherBase<PackagingLot>
    {
        #region fields and constructors

        protected override LotTypeEnum LotType { get { return LotTypeEnum.Packaging; } }

        public PackagingLotInventoryItemEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, newContext, loggingCallback) { }

        #endregion

        protected override IProduct GetInventoryPackaging(ViewInventoryLoadSelected inventory)
        {
            return NewContextHelper.NoPackagingProduct.Product;
        }

        protected override IDerivedLot GetLot(LotKey lotKey)
        {
            return NewContextHelper.GetPackagingLotWithProduct(lotKey);
        }
    }
}