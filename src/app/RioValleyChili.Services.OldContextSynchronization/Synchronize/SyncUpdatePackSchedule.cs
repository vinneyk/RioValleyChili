using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Utilities;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.UpdatePackSchedule)]
    public class SyncUpdatePackSchedule : SyncCommandBase<IProductionUnitOfWork, PackScheduleKey>
    {
        public SyncUpdatePackSchedule(IProductionUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<PackScheduleKey> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }
            var packScheduleKey = getInput();

            var newPackSchedule = UnitOfWork.PackScheduleRepository.SelectPackScheduleForSynch(packScheduleKey);
            var oldPackSchedule = OldContext.tblPackSches.Select(p => new
                {
                    packSchedule = p,
                    lots = p.tblLots
                }).FirstOrDefault(p => p.packSchedule.PackSchID == newPackSchedule.PackSchID);
            if(oldPackSchedule == null)
            {
                throw new Exception(string.Format("Could not find tblPackSch[{0}] record.", newPackSchedule.PackSchID));
            }
            SynchronizePackScheduleHelper.SynchronizeOldContextPackSchedule(oldPackSchedule.packSchedule, newPackSchedule);

            OldContext.SaveChanges();

            Console.Write(ConsoleOutput.UpdatedPackSchedule, oldPackSchedule.packSchedule.PackSchID.ToPackSchIdString());
        }
    }
}