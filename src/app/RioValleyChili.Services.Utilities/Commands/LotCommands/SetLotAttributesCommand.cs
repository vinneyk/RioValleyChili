using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.LotService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.LotCommands
{
    internal class SetLotAttributeValuesCommand
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;
        private const double EPSILON = 0.00001;

        internal SetLotAttributeValuesCommand(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        private Parameters _parameters;

        internal IResult Execute(SetLotAttributesParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            _parameters = new Parameters(parameters);
            var unaccountedAttributes = _parameters.Lot.Attributes.ToList();
            foreach(var newAttribute in parameters.NewAttributes)
            {
                if(newAttribute.Value.AttributeInfo == null)
                {
                    var result = RemoveLotAttributeIfExists(newAttribute.Key, newAttribute.Value, ref unaccountedAttributes);
                    if(!result.Success)
                    {
                        return result;
                    }
                }
                else
                {
                    var result = UpdateOrCreateLotAttribute(parameters.Lot, newAttribute.Key, newAttribute.Value, ref unaccountedAttributes);
                    if(!result.Success)
                    {
                        return result;
                    }
                }
            }

            if(unaccountedAttributes.Any())
            {
                return new InvalidResult(string.Format(UserMessages.NotAllLotAttributesAccountedFor, parameters.Lot.ToLotKey()));
            }

            return new SuccessResult();
        }

        private IResult RemoveLotAttributeIfExists(AttributeNameKey attributeNameKey, IAttributeValueParameters attributeValue, ref List<LotAttribute> unaccountedAttributes)
        {
            var lotAttribute = unaccountedAttributes.FirstOrDefault(a => a.AttributeName != null && attributeNameKey.FindByPredicate.Invoke(a.AttributeName));
            if(lotAttribute != null)
            {
                var resolveOpenDefectsResult = ResolveOpenDefectsForAttribute(lotAttribute, attributeValue);
                if(!resolveOpenDefectsResult.Success)
                {
                    return resolveOpenDefectsResult;
                }

                unaccountedAttributes.Remove(lotAttribute);
                _lotUnitOfWork.LotAttributeRepository.Remove(lotAttribute);
            }

            return new SuccessResult();
        }

        private IResult UpdateOrCreateLotAttribute(ILotKey chileLotKey, AttributeNameKey attributeNameKey, IAttributeValueParameters newAttribute, ref List<LotAttribute> unaccountedAttributes)
        {
            var existingAttribute = false;
            var lotAttribute = unaccountedAttributes.FirstOrDefault(a => a.AttributeName != null && attributeNameKey.FindByPredicate.Invoke(a.AttributeName));
            if(lotAttribute != null)
            {
                existingAttribute = true;
                unaccountedAttributes.Remove(lotAttribute);
            }
            else
            {
                var attributeName = _parameters.AttributeNames.FirstOrDefault(a => attributeNameKey.FindByPredicate.Invoke(a));
                if(attributeName == null)
                {
                    return new InvalidResult(string.Format(UserMessages.AttributeNameNotFound, attributeNameKey.KeyValue));
                }

                lotAttribute = _lotUnitOfWork.LotAttributeRepository.Add(new LotAttribute
                    {
                        LotDateCreated = chileLotKey.LotKey_DateCreated,
                        LotDateSequence = chileLotKey.LotKey_DateSequence,
                        LotTypeId = chileLotKey.LotKey_LotTypeId,

                        AttributeShortName = attributeNameKey.AttributeNameKey_ShortName,
                        AttributeName = attributeName
                    });
            }

            var newValueInSpec = false;
            if(!existingAttribute ||
                (Math.Abs(lotAttribute.AttributeValue - newAttribute.AttributeInfo.Value) > EPSILON) ||
                lotAttribute.AttributeDate != newAttribute.AttributeInfo.Date)
            {
                var updateResult = UpdateLotAttribute(lotAttribute, newAttribute, out newValueInSpec);
                if(!updateResult.Success)
                {
                    return updateResult;
                }
            }

            if(newValueInSpec || newAttribute.Resolution != null)
            {
                var resolveOpenDefectsResult = ResolveOpenDefectsForAttribute(lotAttribute, newAttribute);
                if(!resolveOpenDefectsResult.Success)
                {
                    return resolveOpenDefectsResult;
                }
            }

            return new SuccessResult();
        }

        private IResult UpdateLotAttribute(LotAttribute lotAttribute, IAttributeValueParameters attributeValue, out bool inSpec)
        {
            var range = _parameters.ProductAttributeRanges.FirstOrDefault(r => r.AttributeNameKey_ShortName == lotAttribute.AttributeShortName);
            inSpec = !range.ValueOutOfRange(attributeValue.AttributeInfo.Value);
            if(!inSpec)
            {
                var updateLotDefectsResult = UpdateOrCreateLotDefects(lotAttribute, attributeValue, range);
                if(!updateLotDefectsResult.Success)
                {
                    return updateLotDefectsResult;
                }
            }

            lotAttribute.EmployeeId = _parameters.Employee.EmployeeId;
            lotAttribute.TimeStamp = _parameters.TimeStamp;
            lotAttribute.AttributeValue = attributeValue.AttributeInfo.Value;
            lotAttribute.AttributeDate = attributeValue.AttributeInfo.Date;
            lotAttribute.Computed = false;

            return new SuccessResult();
        }

        private IResult UpdateOrCreateLotDefects(LotAttribute lotAttribute, IAttributeValueParameters attributeValue, IAttributeRange range)
        {
            var unresolvedDefects = _parameters.LotAttributeDefects.Where(d => d.AttributeShortName == lotAttribute.AttributeShortName && d.LotDefect.Resolution == null).ToList();
            if(unresolvedDefects.Any())
            {
                unresolvedDefects.ForEach(d =>
                    {
                        d.OriginalAttributeValue = attributeValue.AttributeInfo.Value;
                        d.OriginalAttributeMinLimit = range.RangeMin;
                        d.OriginalAttributeMaxLimit = range.RangeMax;
                    });
            }
            else
            {
                var defectId = new EFUnitOfWorkHelper(_lotUnitOfWork).GetNextSequence<LotDefect>(d => d.LotDateCreated == _parameters.Lot.LotDateCreated && d.LotDateSequence == _parameters.Lot.LotDateSequence && d.LotTypeId == _parameters.Lot.LotTypeId, d => d.DefectId);
                var lotDefect = _lotUnitOfWork.LotDefectRepository.Add(new LotDefect
                    {
                        LotDateCreated = _parameters.Lot.LotDateCreated,
                        LotDateSequence = _parameters.Lot.LotDateSequence,
                        LotTypeId = _parameters.Lot.LotTypeId,
                        DefectId = defectId,
                        DefectType = lotAttribute.AttributeName.DefectType,
                        Description = lotAttribute.AttributeName.Name,
                    });
                var attributeDefect = _lotUnitOfWork.LotAttributeDefectRepository.Add(new LotAttributeDefect
                    {
                        LotDateCreated = lotDefect.LotDateCreated,
                        LotDateSequence = lotDefect.LotDateSequence,
                        LotTypeId = lotDefect.LotTypeId,
                        DefectId = lotDefect.DefectId,
                        AttributeShortName = lotAttribute.AttributeShortName,

                        OriginalAttributeValue = attributeValue.AttributeInfo.Value,
                        OriginalAttributeMinLimit = range.RangeMin,
                        OriginalAttributeMaxLimit = range.RangeMax,
                        AttributeName = lotAttribute.AttributeName,

                        LotDefect = lotDefect
                    });

                _parameters.LotAttributeDefects.Add(attributeDefect);
            }

            return new SuccessResult();
        }

        private IResult ResolveOpenDefectsForAttribute(LotAttribute lotAttribute, IAttributeValueParameters attributeValue)
        {
            var unresolvedDefects = _parameters.LotAttributeDefects.GetUnresolvedDefects(lotAttribute.AttributeShortName).ToList();
            
            if(!unresolvedDefects.Any())
            {
                return new SuccessResult();
            }

            if(attributeValue.Resolution == null)
            {
                return new InvalidResult(string.Format(UserMessages.AttributeDefectRequiresResolution, lotAttribute.AttributeShortName));
            }

            if(!lotAttribute.IsValidResolution(attributeValue.Resolution.ResolutionType))
            {
                return new InvalidResult(string.Format(UserMessages.InvalidDefectResolutionType, attributeValue.Resolution.ResolutionType, lotAttribute.AttributeName.DefectType));
            }

            IResult result;
            if(attributeValue.Resolution.ResolutionType == ResolutionTypeEnum.Treated && !ValidateTreatmentResolution(_parameters, lotAttribute, out result))
            {
                return result;
            }

            return ResolveDefects(unresolvedDefects, _parameters, attributeValue.Resolution, _lotUnitOfWork);
        }

        #region Private Static Functions

        private static bool ValidateTreatmentResolution(Parameters parameters, LotAttribute lotAttribute, out IResult result)
        {
            return TreatmentResolutionIsValidForInventory(parameters.Inventory, lotAttribute, out result) &&
                   TreatmentResolutionIsValidForPickedInventoryItems(parameters.LotPickedInventoryItems, lotAttribute, out result);
        }

        private static bool TreatmentResolutionIsValidForInventory(IEnumerable<Data.Models.Inventory> inventory, LotAttribute lotAttribute, out IResult result)
        {
            result = null;
            var inventoryWithInvalidTreatment = inventory.FirstOrDefault(i => lotAttribute.AttributeName.ValidTreatments.All(t => t.TreatmentId != i.TreatmentId));
            if(inventoryWithInvalidTreatment == null)
            {
                return true;
            }

            result = new InvalidResult(string.Format(UserMessages.TreatmentOnInventoryDoesNotResolveDefect,
                inventoryWithInvalidTreatment.Treatment.ShortName,
                inventoryWithInvalidTreatment.ToInventoryKey(),
                lotAttribute.AttributeName.Name));
            return false;
        }

        private static bool TreatmentResolutionIsValidForPickedInventoryItems(IEnumerable<PickedInventoryItem> pickedInventory, LotAttribute lotAttribute, out IResult result)
        {
            result = null;
            var pickedInventoryWithInvalidTreatment = pickedInventory.FirstOrDefault(i => lotAttribute.AttributeName.ValidTreatments.All(t => t.TreatmentId != i.TreatmentId));
            if(pickedInventoryWithInvalidTreatment == null)
            {
                return true;
            }

            result = new InvalidResult(string.Format(UserMessages.TreatmentOnUnarchivedPickedInventoryDoesNotResolveDefect,
                    pickedInventoryWithInvalidTreatment.Treatment.ShortName,
                    pickedInventoryWithInvalidTreatment.ToPickedInventoryItemKey(),
                    lotAttribute.AttributeName.Name));
            return false;
        }

        private static IResult ResolveDefects(IEnumerable<LotAttributeDefect> unresolvedDefects, Parameters parameters, IDefectResolutionParameters resolution, ILotUnitOfWork unitOfWork)
        {
            var createResolutionCommand = new CreateLotDefectResolutionCommand(unitOfWork);
            var resolutionResult = unresolvedDefects.Select(
                unresolvedDefect => createResolutionCommand.Execute(new CreateLotDefectResolutionParameters
                    {
                        LotDefect = unresolvedDefect.LotDefect,
                        Employee = parameters.Employee,
                        TimeStamp = parameters.TimeStamp,
                        ResolutionType = resolution.ResolutionType,
                        Description = resolution.Description
                    }))
                .FirstOrDefault(r => !r.Success);

            return resolutionResult as IResult ?? new SuccessResult();
        }

        #endregion

        private class Parameters
        {
            internal DateTime TimeStamp { get { return _parameters.TimeStamp; } }
            internal Employee Employee { get { return _parameters.Employee; } }
            internal Lot Lot { get { return _lot ?? (_lot = _parameters.Lot); } }

            internal List<AttributeName> AttributeNames { get { return _parameters.AttributeNames; } }
            internal List<Data.Models.Inventory> Inventory { get { return _inventory ?? (_inventory = Lot.Inventory.ToList()); } }
            internal List<PickedInventoryItem> LotPickedInventoryItems { get { return _parameters.LotUnarchivedPickedInventoryItems; } }
            internal List<LotAttributeDefect> LotAttributeDefects { get { return _parameters.LotAttributeDefects; } }
            internal readonly List<IAttributeRange> ProductAttributeRanges = new List<IAttributeRange>();

            internal Parameters(SetLotAttributesParameters parameters)
            {
                if(parameters == null) { throw new ArgumentNullException("parameters"); }
                _parameters = parameters;
                if(_parameters.Lot.ChileLot != null)
                {
                    ProductAttributeRanges.AddRange(_parameters.Lot.ChileLot.ChileProduct.ProductAttributeRanges);
                }
            }

            private readonly SetLotAttributesParameters _parameters;

            private Lot _lot;
            private List<Data.Models.Inventory> _inventory;
        }
    }
}