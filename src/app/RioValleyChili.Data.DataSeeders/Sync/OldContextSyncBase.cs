using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using RioValleyChili.Data.Helpers;

namespace RioValleyChili.Data.DataSeeders.Sync
{
    public abstract class OldContextSyncBase<TNewModel, TOldModel>
        where TOldModel : class
    {
        private readonly ObjectContext _oldContext;
        private readonly ConsoleTicker _consoleTicker;

        protected OldContextSyncBase(ObjectContext oldContext, ConsoleTicker consoleTicker)
        {
            if(oldContext == null) { throw new ArgumentNullException("oldContext"); }
            _oldContext = oldContext;
            _consoleTicker = consoleTicker;
        }

        public void SyncOldModels(IEnumerable<TNewModel> newModels)
        {
            var busyMessage = "";
            if(_consoleTicker != null)
            {
                busyMessage = Message + "...";
                Console.Write(busyMessage + "\r");
            }

            var oldSet = _oldContext.CreateObjectSet<TOldModel>();
            var count = 0;
            foreach(var newModel in newModels)
            {
                if(_consoleTicker != null)
                {
                    _consoleTicker.TickConsole(busyMessage);
                }

                SyncOldContextModel(oldSet.First(GetOldModelPredicate(newModel)), newModel);
                count++;
            }
            _oldContext.SaveChanges();

            if(_consoleTicker != null)
            {
                _consoleTicker.ReplaceCurrentLine(string.Format("{0} Done - {1} records.", busyMessage, count));
            }
        }

        protected virtual string Message { get { return "Synching old models"; } }
        protected abstract Expression<Func<TOldModel, bool>> GetOldModelPredicate(TNewModel newModel);
        protected abstract void SyncOldContextModel(TOldModel oldModel, TNewModel newModel);
    }
}
