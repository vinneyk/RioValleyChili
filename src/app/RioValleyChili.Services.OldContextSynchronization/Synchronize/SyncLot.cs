using System;
using System.Linq;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Parameters;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SyncLot)]
    public class SyncLot : SyncCommandBase<ILotUnitOfWork, SynchronizeLotParameters>
    {
        public SyncLot(ILotUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<SynchronizeLotParameters> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }
            var parameters = getInput();

            var syncLotHelper = new SyncLotHelper(OldContext, UnitOfWork, OldContextHelper);

            var oldLots = parameters.LotKeys.Select(lotKey => 
                {
                    var oldLot = syncLotHelper.SynchronizeOldLot(lotKey, parameters.OverrideOldContextLotAsCompleted, parameters.UpdateSerializationOnly);
                    parameters.LotStat = oldLot.LotStat;
                    parameters.Notes = oldLot.Notes;
                    return oldLot;
                })
            .ToList();

            OldContext.SaveChanges();

            Console.Write(ConsoleOutput.UpdatedLot, string.Join(", ", oldLots.Select(l => l.Lot)));
        }
    }
}