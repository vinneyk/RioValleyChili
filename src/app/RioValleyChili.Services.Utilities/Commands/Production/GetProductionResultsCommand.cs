using System;
using System.Linq;
using EF_Split_Projector;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Production
{
    internal class GetProductionResultsCommand
    {
        private readonly Data.Interfaces.UnitsOfWork.IProductionUnitOfWork _productionUnitOfWork;

        internal GetProductionResultsCommand(Data.Interfaces.UnitsOfWork.IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            _productionUnitOfWork = productionUnitOfWork;
        }

        internal IResult<IQueryable<ProductionResultReturn>> Execute(LotKey productionResultKey, bool includeDetails)
        {
            var batches = _productionUnitOfWork.ProductionBatchRepository.All();
            var predicate = (productionResultKey == null ? r => true : productionResultKey.GetPredicate<LotProductionResults>()).And(r => r.Production.ProductionType == ProductionType.ProductionBatch).ExpandAll();
            var selector = includeDetails ? LotProductionResultsProjectors.SplitSelectDetail(batches) : LotProductionResultsProjectors.SplitSelectSummary(batches);

            return new SuccessResult<IQueryable<ProductionResultReturn>>(_productionUnitOfWork.LotProductionResultsRepository.Filter(predicate).SplitSelect(selector));
        }
    }
}