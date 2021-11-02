using System;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Sync
{
    public class PackScheduleKeySync : OldContextSyncBase<PackSchedule, tblPackSch>
    {
        public PackScheduleKeySync(ObjectContext oldContext, ConsoleTicker consoleTicker) : base(oldContext, consoleTicker) { }

        protected override string Message { get { return "Synchronizing pack schedule keys"; } }

        protected override Expression<Func<tblPackSch, bool>> GetOldModelPredicate(PackSchedule newModel)
        {
            return c => c.PackSchID == newModel.PackSchID;
        }

        protected override void SyncOldContextModel(tblPackSch oldModel, PackSchedule newModel)
        {
            oldModel.SerializedKey = new PackScheduleKey(newModel);
        }
    }
}