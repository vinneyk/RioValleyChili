using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using EF_Split_Projector;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Utilities.Commands.Customer;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class GetLabReportConductor
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;

        internal GetLabReportConductor(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        internal IResult<ILabReportReturn> GetLabReport(Expression<Func<ChileLot, bool>> lotPredicate)
        {
            var context = ((EFUnitOfWorkBase) _lotUnitOfWork).Context;
            var adapter = context as IObjectContextAdapter;
            adapter.ObjectContext.CommandTimeout = 600;

            if(lotPredicate == null) { throw new ArgumentNullException("lotPredicate"); }
            
            var select = LotProjectors.SelectLabReportChileLot(_lotUnitOfWork.ProductionBatchRepository.All(), _lotUnitOfWork.LotProductionResultsRepository.All(), _lotUnitOfWork.ChileLotProductionRepository.All());
            var predicate = lotPredicate.And(LotPredicates.FilterForLabReport()).ExpandAll();
            var chileLots = _lotUnitOfWork.ChileLotRepository
                .Filter(predicate)
                .SplitSelect(select)
                .ToList();

            return CreateLabReport(chileLots);
        }

        private IResult<ILabReportReturn> CreateLabReport(List<LabReportChileLotReturn> chileLots)
        {
            if(!chileLots.Any())
            {
                return new InvalidResult<ILabReportReturn>(null, UserMessages.LabReportChileLotsNotFound);
            }

            var chileProductsResult = GetChileProducts(chileLots);
            if(!chileProductsResult.Success)
            {
                return chileProductsResult.ConvertTo<ILabReportReturn>();
            }

            var attributeNames = _lotUnitOfWork.AttributeNameRepository.All().AsNoTracking().ToDictionary(a => a.ShortName, a => a);
            var setupResult = SetupChileLots(chileLots);
            if(!setupResult.Success)
            {
                return setupResult.ConvertTo<ILabReportReturn>();
            }

            return new SuccessResult<ILabReportReturn>(new LabReportReturn
                {
                    AttributeNames = attributeNames.Where(a => a.Value.ValidForChileInventory).Select(a => a.Key).ToList(),
                    ChileLots = chileLots,
                    ChileProducts = chileProductsResult.ResultingObject
                });
        }

        private IResult SetupChileLots(IEnumerable<LabReportChileLotReturn> chileLots)
        {
            var lotToteReturns = new Dictionary<int, DehydratedInputReturn>();

            var dehydratedItems = _lotUnitOfWork.ChileMaterialsReceivedItemRepository.All().AsNoTracking();
            var selectInput = ChileMaterialsReceivedItemProjectors.SelectInput().Expand();

            foreach(var chileLot in chileLots)
            {
                Initialize(chileLot);

                var dehydratedInputs = new List<DehydratedInputReturn>();
                foreach(var pickedLot in chileLot.PickedLots.ToList())
                {
                    DehydratedInputReturn dehydratedInput;
                    var lotToteHash = pickedLot.GetHashCode();
                    if(!lotToteReturns.TryGetValue(lotToteHash, out dehydratedInput))
                    {
                        dehydratedInput = dehydratedItems.Where(ChileMaterialsReceivedItemPredicates.FilterByLotToteReturn(pickedLot).Expand()).Select(selectInput).FirstOrDefault();
                        lotToteReturns.Add(lotToteHash, dehydratedInput);
                    }

                    if(dehydratedInput != null)
                    {
                        dehydratedInputs.Add(dehydratedInput);
                    }
                }

                chileLot.DehydratedInputs = dehydratedInputs;

                var weightedAveragesResult = CalculateAttributeWeightedAveragesCommand.CalculateWeightedAverages(chileLot.PickedLots);
                if(!weightedAveragesResult.Success)
                {
                    return weightedAveragesResult;
                }

                var weightedAverages = weightedAveragesResult.ResultingObject.ToDictionary(a => a.Key.AttributeNameKey.KeyValue, a => a.Value);
                foreach(var lotAttribute in chileLot.WeightedAttributes)
                {
                    double average;
                    lotAttribute.WeightedAverage = weightedAverages.TryGetValue(lotAttribute.Key, out average) ? average : 0.0f;
                }

                SetValidToPick(chileLot);
            }

            return new SuccessResult();
        }

        private IResult<IDictionary<string, ILabReportChileProduct>> GetChileProducts(IEnumerable<LabReportChileLotReturn> chileLots)
        {
            var chileProductIds = chileLots.Select(c => c.ChileProductKeyReturn.ProductKey_ProductId).Distinct().ToArray();
            var select = ProductProjectors.SelectLabReportChileProduct().Expand();
            var chileProducts = _lotUnitOfWork.ChileProductRepository
                .All().AsNoTracking().Where(c => chileProductIds.Contains(c.Id)).Select(select)
                .ToDictionary(c => c.ProductKey, c => (ILabReportChileProduct)c);
            return new SuccessResult<IDictionary<string, ILabReportChileProduct>>(chileProducts);
        }

        private void SetValidToPick(LabReportChileLotReturn chileLot)
        {
            var customerKey = chileLot.PackScheduleBaseReturn != null && chileLot.PackScheduleBaseReturn.Customer != null ? chileLot.PackScheduleBaseReturn.Customer.CustomerKeyReturn : null;
            IDictionary<AttributeNameKey, ChileProductAttributeRange> productSpec;
            IDictionary<AttributeNameKey, CustomerProductAttributeRange> customerSpec;
            if(!new GetProductSpecCommand(_lotUnitOfWork).Execute(ChileProductKey.FromProductKey(chileLot.ChileProductKeyReturn), customerKey, out productSpec, out customerSpec).Success)
            {
                chileLot.ValidToPick = true;
            }

            var lot = _lotUnitOfWork.LotRepository.FindByKey(chileLot.LotKeyReturn.ToLotKey(),
                l => l.ContractAllowances,
                l => l.SalesOrderAllowances,
                l => l.CustomerAllowances,
                l => l.Attributes,
                l => l.AttributeDefects.Select(a => a.LotDefect.Resolution));
            var context = new PickingValidatorContext(productSpec, customerSpec, null, null, customerKey);
            var validator = new PickingValidator(lot).ValidForPicking(context);
            chileLot.ValidToPick = validator.Success;
        }

        private static void Initialize(LabReportChileLotReturn chileLot)
        {
            if(chileLot.WeightedAttributes == null)
            {
                chileLot.WeightedAttributes = new List<WeightedLotAttributeReturn>();
            }

            if(chileLot.PickedLots == null)
            {
                chileLot.PickedLots = new List<PickedLotReturn>();
            }
        }
    }
}