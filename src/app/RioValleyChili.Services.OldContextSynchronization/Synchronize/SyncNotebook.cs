using System;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Utilities;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.Notebook)]
    public class SyncNotebook : SyncCommandBase<EFRVCUnitOfWork, NotebookKey>
    {
        public SyncNotebook(EFRVCUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<NotebookKey> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }

            var notebookKey = getInput();
            var notebook = UnitOfWork.NotebookRepository.FindByKey(notebookKey, n => n.Notes);
            if(notebook == null)
            {
                throw new Exception(string.Format("Notebook[{0}] not found.", notebookKey));
            }

            var productionBatch = UnitOfWork.ProductionBatchRepository.Filter(b => b.InstructionNotebookDateCreated == notebook.Date && b.InstructionNotebookSequence == notebook.Sequence).FirstOrDefault();
            if(productionBatch != null)
            {
                var lotNumber = LotNumberBuilder.BuildLotNumber(productionBatch);
                new SyncTblBatchInstr(OldContext).Synchronize(notebook, lotNumber);
                OldContext.SaveChanges();

                Console.WriteLine(ConsoleOutput.UpdatedBatchInstructions, lotNumber);
                return;
            }

            var contract = UnitOfWork.ContractRepository.GetContractForSynch(notebookKey);
            if(contract != null)
            {
                new SyncContract(UnitOfWork).Synchronize(contract, false);
            }
        }
    }
}