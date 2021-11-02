using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class RemoveProductionBatchConductor : PickedInventoryConductorBase<Data.Interfaces.UnitsOfWork.IProductionUnitOfWork>
    {
        internal RemoveProductionBatchConductor(Data.Interfaces.UnitsOfWork.IProductionUnitOfWork productionUnitOfWork) : base(productionUnitOfWork) { }

        internal IResult<ResultReason> Execute(LotKey lotKey)
        {
            if(lotKey == null) { throw new ArgumentNullException("lotKey"); }
            
            var productionBatch = UnitOfWork.ProductionBatchRepository.FindByKey(lotKey,
                b => b.InstructionNotebook,
                b => b.InstructionNotebook.Notes,
                b => b.Production.PickedInventory,
                b => b.Production.PickedInventory.Items.Select(i => i.CurrentLocation),
                b => b.Production.Results);
            if(productionBatch == null)
            {
                return new InvalidResult(string.Format(UserMessages.ProductionBatchNotFound, lotKey.KeyValue)).ConvertTo(new ResultReason(Reason.ProductionBatchNotFound, lotKey, null));
            }
            
            var instructionNotebook = productionBatch.InstructionNotebook;
            var pickedInventory = productionBatch.Production.PickedInventory;
            var production = productionBatch.Production;

            if(productionBatch.ProductionHasBeenCompleted)
            {
                return new InvalidResult(string.Format(UserMessages.ProductionBatchAlreadyComplete, lotKey.KeyValue)).ConvertTo(new ResultReason(Reason.ProductionBatchCompleted, lotKey, new LotKey(productionBatch)));
            }

            if(productionBatch.Production.Results != null)
            {
                return new InvalidResult(string.Format(UserMessages.ProductionBatchHasResult, lotKey.KeyValue)).ConvertTo(new ResultReason(Reason.ProductionBatchHasResults, lotKey, new LotKey(productionBatch)));
            }

            var removeLotResult = new RemoveLotCommand(UnitOfWork).Execute(new LotKey(productionBatch));
            if(!removeLotResult.Success)
            {
                return removeLotResult.ConvertTo(new ResultReason(Reason.RemoveLotFailed, lotKey, new LotKey(productionBatch)));
             }

            var removePickedItemsResult = UpdatePickedInventory(null, null, DateTime.UtcNow, pickedInventory, null);
            if(!removePickedItemsResult.Success)
            {
                return removePickedItemsResult.ConvertTo(new ResultReason(Reason.RemovePickedItemFailed, lotKey, new LotKey(productionBatch)));
            }
            
            UnitOfWork.ChileLotProductionRepository.Remove(production);
            UnitOfWork.PickedInventoryRepository.Remove(pickedInventory);

            foreach(var note in instructionNotebook.Notes.ToList())
            {
                UnitOfWork.NoteRepository.Remove(note);
            }
            UnitOfWork.NotebookRepository.Remove(instructionNotebook);

            return new SuccessResult().ConvertTo((ResultReason)null);
        }

        public class ResultReason
        {
            public Reason Reason;
            public string BatchKey;
            public string LotKey;

            public ResultReason(Reason reason, string batchKey, string lotKey)
            {
                Reason = reason;
                BatchKey = batchKey;
                LotKey = lotKey;
            }
        }

        public enum Reason
        {
            ProductionBatchNotFound,
            ProductionBatchCompleted,
            ProductionBatchHasResults,
            RemoveLotFailed,
            RemovePickedItemFailed
        }
    }
}