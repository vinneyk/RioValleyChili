using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Utilities;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.CreatePackSchedule)]
    public class SyncCreatePackSchedule : SyncCommandBase<IProductionUnitOfWork, SyncCreatePackScheduleParameters>
    {
        public SyncCreatePackSchedule(IProductionUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<SyncCreatePackScheduleParameters> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }
            var parameters = getInput();
            var packScheduleKey = parameters.PackScheduleKey;

            var newPackSchedule = UnitOfWork.PackScheduleRepository.SelectPackScheduleForSynch(packScheduleKey);
            var oldPackSchedule = CreateNewPackSchedule(newPackSchedule, parameters.UseSuppliedPSNum);

            OldContext.tblPackSches.AddObject(oldPackSchedule);
            OldContext.SaveChanges();

            newPackSchedule.PackSchID = oldPackSchedule.PackSchID;
            newPackSchedule.PSNum = oldPackSchedule.PSNum;
            UnitOfWork.Commit();

            Console.Write(ConsoleOutput.AddedPackSchedule, oldPackSchedule.PackSchID.ToPackSchIdString());
        }

        private tblPackSch CreateNewPackSchedule(PackSchedule packSchedule, bool useSuppliedPSNum)
        {
            var packSchId = packSchedule.TimeStamp.ConvertLocalToUTC().RoundMillisecondsForSQL();
            var tblPackSch = new tblPackSch
                {
                    SerializedKey = packSchedule.ToPackScheduleKey(),
                    PackSchID = packSchId,
                    PSNum = useSuppliedPSNum && packSchedule.PSNum != null ? packSchedule.PSNum.Value :
                        OldContext.tblPackSches.Select(p => p.PSNum).Where(p => p != null).DefaultIfEmpty(0).Max().Value + 1,
                    PSStatus = 1,
                    SetTrtmtID = 0
                };
            SynchronizePackScheduleHelper.SynchronizeOldContextPackSchedule(tblPackSch, packSchedule);
            return tblPackSch;
        }
    }
}