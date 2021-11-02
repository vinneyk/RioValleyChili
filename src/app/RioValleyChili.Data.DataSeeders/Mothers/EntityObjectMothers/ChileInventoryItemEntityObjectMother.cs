using System;
using System.Data.Entity.Core.Objects;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public abstract class ChileLotInventoryItemEntityObjectMotherBase : LotInventoryEntityObjectMotherBase<ChileLot>
    {
        protected ChileLotInventoryItemEntityObjectMotherBase(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, newContext, loggingCallback) { }

        protected override IDerivedLot GetLot(LotKey lotKey)
        {
            return NewContextHelper.GetChileLotWithProduct(lotKey);
        }
    }

    public sealed class WIPChileLotInventoryItemEntityObjectMother : ChileLotInventoryItemEntityObjectMotherBase
    {
        protected override LotTypeEnum LotType { get { return LotTypeEnum.WIP; } }

        public WIPChileLotInventoryItemEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, newContext, loggingCallback) { }
    }

    public sealed class FinishedGoodsChileLotInventoryItemEntityObjectMother : ChileLotInventoryItemEntityObjectMotherBase
    {
        protected override LotTypeEnum LotType { get { return LotTypeEnum.FinishedGood; } }

        public FinishedGoodsChileLotInventoryItemEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, newContext, loggingCallback) { }
    }

    public sealed class DehyChileLotInventoryItemEntityObjectMother : ChileLotInventoryItemEntityObjectMotherBase
    {
        protected override LotTypeEnum LotType { get { return LotTypeEnum.DeHydrated; } }

        public DehyChileLotInventoryItemEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, newContext, loggingCallback) { }
    }

    public sealed class RawChileLotInventoryItemEntityObjectMother : ChileLotInventoryItemEntityObjectMotherBase
    {
        protected override LotTypeEnum LotType { get { return LotTypeEnum.Raw; } }

        public RawChileLotInventoryItemEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, newContext, loggingCallback) { }
    }

    public sealed class GRPChileLotInventoryItemEntityObjectMother : ChileLotInventoryItemEntityObjectMotherBase
    {
        protected override LotTypeEnum LotType { get { return LotTypeEnum.GRP; } }

        public GRPChileLotInventoryItemEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, newContext, loggingCallback) { }
    }

    public sealed class OtherChileLotInventoryItemEntityObjectMother : ChileLotInventoryItemEntityObjectMotherBase
    {
        protected override LotTypeEnum LotType { get { return LotTypeEnum.Other; } }

        public OtherChileLotInventoryItemEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, newContext, loggingCallback) { }
    }
}
