using System;
using System.Linq;
using EF_Split_Projector;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.Models.KeyReturns;
using Solutionhead.Services;
using IProductionUnitOfWork = RioValleyChili.Data.Interfaces.UnitsOfWork.IProductionUnitOfWork;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class GetProductionPacketConductor
    {
        private readonly IProductionUnitOfWork _productionUnitOfWork;

        internal GetProductionPacketConductor(IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            _productionUnitOfWork = productionUnitOfWork;
        }

        internal IResult<IProductionPacketReturn> Execute(ILotKey lotKey, DateTime currentDate)
        {
            var productionBatchKey = new LotKey(lotKey);
            var select = ProductionBatchProjectors.SplitSelectProductionPacketFromBatch(_productionUnitOfWork, currentDate.Date, productionBatchKey);
            var productionPacket = _productionUnitOfWork.ProductionBatchRepository
                .Filter(productionBatchKey.GetPredicate<ProductionBatch>())
                .SplitSelect(select)
                .ToList();

            if(productionPacket.Count != 1)
            {
                return new InvalidResult<IProductionPacketReturn>(null, string.Format(UserMessages.ProductionBatchNotFound, productionBatchKey));
            }

            return Process(productionPacket.Single());
        }

        internal IResult<IProductionPacketReturn> Execute(IPackScheduleKey packScheduleKey, DateTime currentDate)
        {
            var parsedPackScheduleKey = new PackScheduleKey(packScheduleKey);
            var productionPacket = _productionUnitOfWork.PackScheduleRepository
                .Filter(parsedPackScheduleKey.FindByPredicate)
                .SplitSelect(PackScheduleProjectors.SplitSelectProductionPacket(_productionUnitOfWork, currentDate.Date))
                .ToList();

            if(productionPacket.Count != 1)
            {
                return new InvalidResult<IProductionPacketReturn>(null, string.Format(UserMessages.PackScheduleNotFound, parsedPackScheduleKey));
            }

            return Process(productionPacket.Single());
        }

        private static IResult<IProductionPacketReturn> Process(IProductionPacketReturn packet)
        {
            foreach(var batch in packet.Batches.OfType<ProductionPacketBatchReturn>())
            {
                var averages = CalculateAttributeWeightedAveragesCommand.CalculateWeightedAverages(batch.PickedItems);
                if(!averages.Success)
                {
                    return averages.ConvertTo<IProductionPacketReturn>(null);
                }

                batch.CalculatedParameters = new ProductionBatchTargetParameters
                    {
                        BatchTargetWeight = batch.PickedItems.Any() ? batch.PickedItems.Sum(i => i.QuantityPicked * i.PackagingProduct.Weight) : 0.0f,
                        BatchTargetAsta = averages.ResultingObject.Where(a => a.Key.AttributeNameKey.AttributeNameKey_ShortName == Constants.ChileAttributeKeys.Asta).Select(a => a.Value).FirstOrDefault(),
                        BatchTargetScan = averages.ResultingObject.Where(a => a.Key.AttributeNameKey.AttributeNameKey_ShortName == Constants.ChileAttributeKeys.Scan).Select(a => a.Value).FirstOrDefault(),
                        BatchTargetScoville = averages.ResultingObject.Where(a => a.Key.AttributeNameKey.AttributeNameKey_ShortName == Constants.ChileAttributeKeys.Scov).Select(a => a.Value).FirstOrDefault(),
                    };
            }

            return new SuccessResult<IProductionPacketReturn>(packet);
        }
    }
}