using System;
using System.Collections.Generic;
using System.Linq;
using EF_Split_Projector;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.Models;
using Solutionhead.Services;
using IProductionUnitOfWork = RioValleyChili.Data.Interfaces.UnitsOfWork.IProductionUnitOfWork;

namespace RioValleyChili.Services.Utilities.Commands.Production
{
    internal class GetProductionBatchDetailCommand
    {
        private readonly IProductionUnitOfWork _productionUnitOfWork;

        public GetProductionBatchDetailCommand(IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            _productionUnitOfWork = productionUnitOfWork;
        }

        public IResult<IProductionBatchDetailReturn> Execute(LotKey lotKey, DateTime currentDate)
        {
            var predicate = lotKey.GetPredicate<ProductionBatch>();

            var batchDetails = _productionUnitOfWork.ProductionBatchRepository
                .Filter(predicate)
                .SplitSelect(ProductionBatchProjectors.SplitSelectDetail(_productionUnitOfWork, currentDate)).ToList()
                .FirstOrDefault();
            if(batchDetails == null)
            {
                return new InvalidResult<IProductionBatchDetailReturn>(null, string.Format(UserMessages.ProductionBatchNotFound, lotKey.KeyValue));
            }

            return CreateProductionBatchDetail(batchDetails);
        }

        private static IResult<ProductionBatchDetailReturn> CreateProductionBatchDetail(ProductionBatchDetailReturn parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var additiveIngredientSummaries = parameters.ToChileProductAdditiveIngredientSummaries().ToList();
            var fgMaterialsSummary = parameters.ToFinishedGoodsMaterialSummary();
            var wipMaterialsSummary = parameters.ToWipMaterialsSummary(additiveIngredientSummaries);

            SetPercentPicked(new List<ProductionBatchMaterialsSummary>(additiveIngredientSummaries) { fgMaterialsSummary, wipMaterialsSummary });

            parameters.AdditiveIngredients = additiveIngredientSummaries;
            parameters.FinishedGoodsMaterialsSummary = fgMaterialsSummary;
            parameters.WipMaterialsSummary = wipMaterialsSummary;
            parameters.PackagingMaterialSummaries = parameters.ToPackagingMaterialSummaries();

            return new SuccessResult<ProductionBatchDetailReturn>(parameters);
        }

        private static void SetPercentPicked(List<ProductionBatchMaterialsSummary> materialSummaries)
        {
            var totalPoundsPicked = materialSummaries.Sum(m => m.WeightPicked);
            if(totalPoundsPicked <= 0)
            {
                materialSummaries.ForEach(m => { m.PercentOfPicked = 0.0; });
            }
            else
            {
                materialSummaries.ForEach(m => { m.PercentOfPicked = Math.Max(0.0, m.WeightPicked / totalPoundsPicked); });
            }
        }
    }
}