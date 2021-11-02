using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Services.OldContextSynchronization.Utilities
{
    public static class RepositoryExtensions
    {
        public static Lot GetLotForSynch(this IRepository<Lot> lotRepository, LotKey lotKey)
        {
            return lotRepository.FindByKey(lotKey, Expressions.SynchLotPaths);
        }

        public static List<LotAttributeDefect> GetLotAttributeDefectsForSynch(this IRepository<LotAttributeDefect> attributeDefectRepository, Lot lot)
        {
            var lotAttributeDefects = attributeDefectRepository.FilterByKey(new LotKey(lot)).ToList();
            lotAttributeDefects.ForEach(d =>
                {
                    d.LotDefect = lot.LotDefects.First(e => e.DefectId == d.DefectId);
                });
            return lotAttributeDefects;
        }

        public static Contract GetContractForSynch(this IRepository<Contract> contractRepository, ContractKey contractKey)
        {
            return contractRepository.FindByKey(contractKey, Expressions.SynchContractPaths);
        }

        public static Contract GetContractForSynch(this IRepository<Contract> contractRepository, INotebookKey commentsNotebookKey)
        {
            return contractRepository.FindBy(Expressions.ContractByCommentsNotebook(commentsNotebookKey), Expressions.SynchContractPaths);
        }
    }
}