using System;
using System.Linq;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Utilities;
using Solutionhead.Data;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.DeleteProductionBatch)]
    public class SyncDeleteProductionBatch : SyncCommandBase<IProductionUnitOfWork, LotKey>
    {
        public SyncDeleteProductionBatch(IProductionUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<LotKey> getInput)
        {
            if(getInput == null)
            {
                throw new ArgumentNullException("getInput");
            }
            var lotNumber = LotNumberBuilder.BuildLotNumber(getInput());

            var tblLot = OldContext.tblLots
                .Include
                (
                    l => l.inputBatchItems,
                    l => l.tblBatchInstr,
                    l => l.tblBOMs,
                    l => l.tblLotAttributeHistory,
                    l => l.tblIncomings,
                    l => l.tblOutgoingInputs,
                    l => l.tblPackSch
                )
                .FirstOrDefault(l => l.Lot == lotNumber);
            if(tblLot == null)
            {
                throw new Exception(string.Format("tblLot[{0}] not found.", lotNumber));
            }

            if(tblLot.tblPackSch == null)
            {
                throw new Exception(string.Format("tblPackSch record for tblLot[{0}] not found.", lotNumber));
            }

            var newPackSchedule = UnitOfWork.PackScheduleRepository.SelectPackScheduleForSynch(new PackSchIdKey(tblLot.tblPackSch.PackSchID));
            SynchronizePackScheduleHelper.SynchronizeOldContextPackSchedule(tblLot.tblPackSch, newPackSchedule);

            new DeleteTblLotHelper(OldContext).DeleteLot(tblLot);

            OldContext.SaveChanges();

            Console.WriteLine(ConsoleOutput.RemovedProductionBatch, lotNumber);
        }

        private class PackSchIdKey : IKey<PackSchedule>
        {
            private readonly DateTime _packSchId;

            public PackSchIdKey(DateTime packSchId)
            {
                _packSchId = packSchId;
            }

            public Expression<Func<PackSchedule, bool>> FindByPredicate
            {
                get { return p => p.PackSchID == _packSchId; }
            }
        }
    }
}