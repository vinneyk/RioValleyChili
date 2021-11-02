using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Inventory
{
    internal class RemoveLotCommand
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;

        internal RemoveLotCommand(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        internal IResult Execute(LotKey lotKey)
        {
            if(lotKey == null) { throw new ArgumentNullException("lotKey"); }

            var existingInventory = _lotUnitOfWork.LotRepository.FilterByKey(lotKey).Any(l => l.Inventory.Any());
            if(existingInventory)
            {
                return new FailureResult(string.Format(UserMessages.LotHasExistingInventory, lotKey.KeyValue));
            }

            var lot = _lotUnitOfWork.LotRepository.FindByKey(lotKey,
                l => l.Attributes,
                l => l.LotDefects.Select(d => d.Resolution));
            if(lot == null)
            {
                return new FailureResult(string.Format(UserMessages.LotNotFound, lotKey.KeyValue));
            }

            var additiveLot = _lotUnitOfWork.AdditiveLotRepository.FindByKey(lotKey);
            if(additiveLot != null)
            {
                _lotUnitOfWork.AdditiveLotRepository.Remove(additiveLot);
            }

            var chileLot = _lotUnitOfWork.ChileLotRepository.FindByKey(lotKey);
            if(chileLot != null)
            {
                _lotUnitOfWork.ChileLotRepository.Remove(chileLot);
            }

            var packagingLot = _lotUnitOfWork.PackagingLotRepository.FindByKey(lotKey);
            if(packagingLot != null)
            {
                _lotUnitOfWork.PackagingLotRepository.Remove(packagingLot);
            }
            
            lot.Attributes.ToList().ForEach(a => _lotUnitOfWork.LotAttributeRepository.Remove(a));
            lot.LotDefects.ToList().ForEach(d =>
                {
                    if(d.Resolution != null)
                    {
                        _lotUnitOfWork.LotDefectResolutionRepository.Remove(d.Resolution);
                    }
                    _lotUnitOfWork.LotDefectRepository.Remove(d);
                });

            var lotAttributeDefects = _lotUnitOfWork.LotAttributeDefectRepository.FilterByKey(lotKey);
            foreach(var attributeDefect in lotAttributeDefects)
            {
                _lotUnitOfWork.LotAttributeDefectRepository.Remove(attributeDefect);
            }

            _lotUnitOfWork.LotRepository.Remove(lot);

            return new SuccessResult();
        }
    }
}