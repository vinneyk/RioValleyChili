using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    public class SetLotWeightedAttributesConductor
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;

        public SetLotWeightedAttributesConductor(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        public IResult Execute(ChileLot chileLot, List<PickedInventoryItem> pickedItems, DateTime timestamp)
        {
            if(chileLot == null) { throw new ArgumentNullException("chileLot"); }
            if(pickedItems == null) { throw new ArgumentNullException("pickedItems"); }

            var actualsRequired = new HashSet<AttributeNameKey>(_lotUnitOfWork.AttributeNameRepository
                .All().Where(a => a.ActualValueRequired)
                .ToList()
                .Select(a => a.ToAttributeNameKey()));
            foreach(var item in pickedItems)
            {
                item.PackagingProduct = _lotUnitOfWork.PackagingProductRepository.FindByKey(item.ToPackagingProductKey());
                item.Lot = _lotUnitOfWork.LotRepository.FindByKey(item.ToLotKey(), i => i.Attributes.Select(a => a.AttributeName));
            }

            var weightedAverages = CalculateAttributeWeightedAveragesCommand.Execute(pickedItems);
            if(!weightedAverages.Success)
            {
                return weightedAverages;
            }

            var attributesToRemove = chileLot.Lot.Attributes.ToList();
            var attributesToSet = weightedAverages.ResultingObject.Where(a => !actualsRequired.Contains(a.Key.Attribute.ToAttributeNameKey())).ToList();
            foreach(var weightedAttribute in attributesToSet)
            {
                var namePredicate = weightedAttribute.Key.AttributeNameKey.FindByPredicate.Compile();
                var lotAttribute = attributesToRemove.FirstOrDefault(a => namePredicate(a.AttributeName));
                if(lotAttribute != null)
                {
                    attributesToRemove.Remove(lotAttribute);
                }
                else
                {
                    lotAttribute = new LotAttribute
                        {
                            LotDateCreated = chileLot.LotDateCreated,
                            LotDateSequence = chileLot.LotDateSequence,
                            LotTypeId = chileLot.LotTypeId,
                            AttributeShortName = weightedAttribute.Key.AttributeNameKey.AttributeNameKey_ShortName,
                            Computed = true
                        };
                    chileLot.Lot.Attributes.Add(lotAttribute);
                }
                
                if(lotAttribute.Computed)
                {
                    lotAttribute.AttributeValue = weightedAttribute.Value;
                    lotAttribute.EmployeeId = chileLot.Production.PickedInventory.EmployeeId;
                    lotAttribute.AttributeDate = (lotAttribute.TimeStamp = timestamp).Date;
                }
            }

            foreach(var attribute in attributesToRemove.Where(a => a.Computed))
            {
                _lotUnitOfWork.LotAttributeRepository.Remove(attribute);
            }

            return new SuccessResult();
        }
    }
}