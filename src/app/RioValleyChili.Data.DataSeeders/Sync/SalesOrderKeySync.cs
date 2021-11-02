using System;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Sync
{
    public class SalesOrderKeySync : OldContextSyncBase<SalesOrder, tblOrder>
    {
        public SalesOrderKeySync(ObjectContext oldContext, ConsoleTicker consoleTicker) : base(oldContext, consoleTicker) { }

        protected override string Message { get { return "Synchronizing SalesOrder keys"; } }

        protected override Expression<Func<tblOrder, bool>> GetOldModelPredicate(SalesOrder newModel)
        {
            return o => o.OrderNum == newModel.InventoryShipmentOrder.MoveNum;
        }

        protected override void SyncOldContextModel(tblOrder oldModel, SalesOrder newModel)
        {
            oldModel.SerializedKey = newModel.ToSalesOrderKey();
        }
    }
}