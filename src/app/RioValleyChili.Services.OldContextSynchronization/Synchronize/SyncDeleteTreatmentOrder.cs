using System;
using System.Linq;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.DeleteTreatmentOrder)]
    public class SyncDeleteTreatmentOrder : SyncCommandBase<IInventoryShipmentOrderUnitOfWork, int?>
    {
        public SyncDeleteTreatmentOrder(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<int?> getInput)
        {
            var moveNum = getInput();

            var order = OldContext
                .tblMoves
                .Include(
                    m => m.tblMoveDetails,
                    m => m.tblMoveOrderDetails)
                .FirstOrDefault(m => m.MoveNum == moveNum);
            if(order == null)
            {
                throw new Exception(string.Format("Could not find tblMove[{0}]", moveNum));
            }

            order.tblMoveOrderDetails.ToList().ForEach(OldContext.tblMoveOrderDetails.DeleteObject);
            order.tblMoveDetails.ToList().ForEach(OldContext.tblMoveDetails.DeleteObject);
            OldContext.tblMoves.DeleteObject(order);

            OldContext.SaveChanges();

            Console.WriteLine(ConsoleOutput.RemovedTblMove, moveNum);

        }
    }
}