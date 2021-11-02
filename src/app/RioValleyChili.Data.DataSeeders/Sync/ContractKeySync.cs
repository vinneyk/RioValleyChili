using System;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Sync
{
    public class ContractKeySync : OldContextSyncBase<Contract, tblContract>
    {
        public ContractKeySync(ObjectContext oldContext, ConsoleTicker consoleTicker) : base(oldContext, consoleTicker) { }

        protected override string Message { get { return "Synchronizing contract keys"; } }

        protected override Expression<Func<tblContract, bool>> GetOldModelPredicate(Contract newModel)
        {
            return c => c.ContractID == newModel.ContractId;
        }

        protected override void SyncOldContextModel(tblContract oldModel, Contract newModel)
        {
            oldModel.SerializedKey = new ContractKey(newModel);
        }
    }
}