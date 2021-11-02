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
                if(newAttribute.Value.NewValue == null)
                {
                    var result = RemoveLotAttributeIfExists(newAttribute.Key, newAttribute.Value, ref unaccountedAttributes);
                    if(!result.Success)
                    {
                        return result;
                    }
                }
                else
                {
                    var result = UpdateOrCreateLotAttribute(parameters.ChileLot, newAttribute.Key, newAttribute.Value, ref unaccountedAttributes);
                    if(!result.Success)
                    {
                        return result;
                    }
                }
            }

            if(unaccountedAttributes.Any())
            {
                return new InvalidResult(string.Format(UserMessages.NotAllLotAttributesAccountedFor, new LotKey(parameters.ChileLot)));
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

        private static readonly object LotAttributeRepositoryLock = new object();

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

                try
                {
                    lock(LotAttributeRepositoryLock)
                    {
                        lotAttribute = _lotUnitOfWork.LotAttributeRepository.Add(new LotAttribute
                            {
                                LotDateCreated = chileLotKey.LotKey_DateCreated,
                                LotDateSequence = chileLotKey.LotKey_DateSequence,
                                LotTypeId = chileLotKey.LotKey_LotTypeId,

                                AttributeShortName = attributeNameKey.AttributeNameKey_ShortName,
                                AttributeName = attributeName
                            });
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }

            if(!existingAttribute || (Math.Abs(lotAttribute.AttributeValue - newAttribute.NewValue.Value) > EPSILON) || lotAttribute.AttributeDate != newAttribute.AttributeDate)
            {
                var updateResult = UpdateLotAttribute(lotAttribute, newAttribute);
                if(!updateResult.Success)
                {
                    return updateResult;
                }
            }

            return new SuccessResult();
        }

        private IResult UpdateLotAttribute(LotAttribute lotAttribute, IAttributeValueParameters attributeValue)
        {
            var range = _parameters.ChileProductAttributeRanges.FirstOrDefault(r => r.AttributeShortName == lotAttribute.AttributeShortName);
            if(range != null)
            {
                if(range.ValueOutOfRange(attributeValue.NewValue.Value))
                {
                    var updateLotDefectsResult = UpdateOrCreateLotDefects(lotAttribute, attributeValue, range);
                    if(!updateLotDefectsResult.Success)
                    {
                        return updateLotDefectsResult;
                    }
                }
                else
                {
                    var resolveOpenDefectsResult = ResolveOpenDefectsForAttribute(lotAttribute, attributeValue);
                    if(!resolveOpenDefectsResult.Success)
                    {
                        return resolveOpenDefectsResult;
                    }
                }
            }

            lotAttribute.EmployeeId = _parameters.Employee.EmployeeId;
            lotAttribute.TimeStamp = _parameters.TimeStamp;
            lotAttribute.AttributeValue = attributeValue.NewValue.Value;
            lotAttribute.AttributeDate = attributeValue.AttributeDate;
            lotAttribute.Computed = false;

            return new SuccessResult();
        }

        private static readonly object LotDefectLock = new object();

        private IResult UpdateOrCreateLotDefects(LotAttribute lotAttribute, IAttributeValueParameters attributeValue, ChileProductAttributeRange range)
        {
            var unresolvedDefects = _parameters.LotAttributeDefects.Where(d => d.AttributeShortName == lotAttribute.AttributeShortName && d.LotDefect.Resolution == null).ToList();
            if(unresolvedDefects.Any())
            {
                unresolvedDefects.ForEach(d =>
                    {
                        d.OriginalAttributeValue = attributeValue.NewValue.Value;
                        d.OriginalAttributeMinLimit = range.RangeMin;
                        d.OriginalAttributeMaxLimit = range.RangeMax;
                    });
            }
            else
            {
                int defectId;
                lock(LotDefectLock)
                {
                    defectId = new EFUnitOfWorkHelper(_lotUnitOfWork).GetNextSequence<LotDefect>(d => d.LotDateCreated == _parameters.Lot.LotDateCreated && d.LotDateSequence == _parameters.Lot.LotDateSequence && d.LotTypeId == _parameters.Lot.LotTypeId, d => d.DefectId);
                }

                var lotDefect = _lotUnitOfWork.LotDefectRepository.Add(new LotDefect
                    {
                        LotDateCreated = _parameters.Lot.LotDateCreated,
                        LotDateSequence = _parameters.Lot.LotDateSequence,
                        LotTypeId = _parameters.Lot.LotTypeId,
                        DefectId = defectId,
                        DefectType = lotAttribute.AttributeName.DefectType,
                        Description = lotAttribute.AttributeName.Name,
                    });
                _lotUnitOfWork.LotAttributeDefectRepository.Add(new LotAttributeDefect
                    {
                        LotDateCreated = lotDefect.LotDateCreated,
                        LotDateSequence = lotDefect.LotDateSequence,
                        LotTypeId = lotDefect.LotTypeId,
                        DefectId = lotDefect.DefectId,
                        AttributeShortName = lotAttribute.AttributeShortName,
                    
                        OriginalAttributeValue = attributeValue.NewValue.Value,
                        OriginalAttributeMinLimit = range.RangeMin,
                        OriginalAttributeMaxLimit = range.RangeMax,
                        AttributeName = lotAttribute.AttributeName,
                    });
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

            if(attributeValue.Resolution.ResolutionType != ResolutionTypeEnum.Treated)
            {
                return ResolveDefects(unresolvedDefects, _parameters, attributeValue, _lotUnitOfWork);
            }

            IResult result;
            return ValidateTreatmentResolution(_parameters, lotAttribute, out result) 
                ? ResolveDefects(unresolvedDefects, _parameters, attributeValue, _lotUnitOfWork)
                : result;
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
                new InventoryKey(inventoryWithInvalidTreatment).KeyValue,
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
                    new PickedInventoryItemKey(pickedInventoryWithInvalidTreatment).KeyValue,
                    lotAttribute.AttributeName.Name));
            return false;
        }

        private static IResult ResolveDefects(IEnumerable<LotAttributeDefect> unresolvedDefects, Parameters parameters, IAttributeValueParameters attributeValue, ILotUnitOfWork unitOfWork)
        {
            var createResolutionCommand = new CreateLotDefectResolutionCommand(unitOfWork);
            var resolutionResult = unresolvedDefects.Select(
                unresolvedDefect => createResolutionCommand.Execute(new CreateLotDefectResolutionParameters
                    {
                        LotDefect = unresolvedDefect.LotDefect,
                        Employee = parameters.Employee,
                        TimeStamp = parameters.TimeStamp,
                        ResolutionType = attributeValue.Resolution.ResolutionType,
                        Description = attributeValue.Resolution.Description
                    }))
                .FirstOrDefault(r => !r.Success);

            return resolutionResult as IResult ?? new SuccessResult();
        }

        #endregion

        private class Parameters
        {
            internal DateTime TimeStamp { get { return _parameters.TimeStamp; } }
            internal Employee Employee { get { return _parameters.Employee; } }
            internal Lot Lot { get { return _lot ?? (_lot = _parameters.ChileLot.Lot); } }

            internal List<AttributeName> AttributeNames { get { return _parameters.AttributeNames; } }
            internal List<Data.Models.Inventory> Inventory { get { return _inventory ?? (_inventory = Lot.Inventory.ToList()); } }
            internal List<PickedInventoryItem> LotPickedInventoryItems { get { return _parameters.LotUnarchivedPickedInventoryItems; } }
            internal List<LotAttributeDefect> LotAttributeDefects { get { return _parameters.LotAttributeDefects; } }
            internal List<ChileProductAttributeRange> ChileProductAttributeRanges { get { return _chileProductAttributeRanges ?? (_chileProductAttributeRanges = _parameters.ChileLot.ChileProduct.ProductAttributeRanges.ToList()); } }

            internal Parameters(SetLotAttributesParameters parameters)
            {
                if(parameters == null) { throw new ArgumentNullException("parameters"); }
                _parameters = parameters;
            }

            private readonly SetLotAttributesParameters _parameters;

            private Lot _lot;
            private List<Data.Models.Inventory> _inventory;
            private List<ChileProductAttributeRange> _chileProductAttributeRanges;
        }
    }
}