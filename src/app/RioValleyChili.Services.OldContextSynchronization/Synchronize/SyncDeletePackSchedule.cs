using System;
using System.Linq;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Utilities;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.DeletePackSchedule)]
    public class SyncDeletePackSchedule : SyncCommandBase<IProductionUnitOfWork, DateTime?>
    {
        public SyncDeletePackSchedule(IProductionUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<DateTime?> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }
            var input = getInput();
            if(input == null)
            {
                throw new Exception("PackSchId from method return is null.");
            }
            var packSchId = input.Value;

            var packSch = OldContext.tblPackSches
                .Select(p => new
                    {
                        packSchedule = p,
                        lots = p.tblLots.Select(l => l.Lot).Concat(p.tblBatchItems.Where(i => i.BatchLot != null || i.NewLot != null).Select(i => (i.BatchLot ?? i.NewLot).Value)).Distinct(),
                    })
                .FirstOrDefault(p => p.packSchedule.PackSchID == packSchId);
            if(packSch == null)
            {
                throw new Exception(string.Format("Could not find tblPackSches[{0}]", packSchId));
            }

            foreach(var scheduledItem in OldContext.tblProductionScheduleItems.Where(i => i.PSNum == packSch.packSchedule.PSNum).ToList())
            {
                OldContext.tblProductionScheduleItems.DeleteObject(scheduledItem);
            }

            new DeleteTblLotHelper(OldContext).DeleteLots(packSch.lots.ToArray());
            OldContext.tblPackSches.DeleteObject(packSch.packSchedule);
            OldContext.SaveChanges();
            
            Console.WriteLine(ConsoleOutput.RemovedPackSchedule, packSchId.ToPackSchIdString());
        }
    }
}