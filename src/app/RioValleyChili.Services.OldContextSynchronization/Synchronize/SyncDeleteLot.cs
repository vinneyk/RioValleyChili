using System;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.DeleteLot)]
    public class SyncDeleteLot : SyncCommandBase<ILotUnitOfWork, LotKey>
    {
        public SyncDeleteLot(ILotUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<LotKey> getInput)
        {
            var lotNumber = LotNumberBuilder.BuildLotNumber(getInput());
            new DeleteTblLotHelper(OldContext).DeleteLots(lotNumber);
            OldContext.SaveChanges();
            Console.WriteLine(ConsoleOutput.DeletedTblLot, lotNumber);
        }
    }
}