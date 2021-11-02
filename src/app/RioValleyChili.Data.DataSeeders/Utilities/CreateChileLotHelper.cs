using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities.Interfaces;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Interfaces;
using RioValleyChili.Data.Models.StaticRecords;
using EntityTypes = RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers.LotEntityObjectMother.EntityTypes;
using LotDTO = RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers.LotEntityObjectMother.LotDTO;
using CallbackParameters = RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers.LotEntityObjectMother.CallbackParameters;
using CallbackReason = RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers.LotEntityObjectMother.CallbackReason;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class CreateChileLotHelper
    {
        private readonly SerializedChileLotHelper _serializedChileLotHelper;
        private readonly UnserializedChileLotHelper _unserializedChileLotHelper;

        public CreateChileLotHelper(ILotMother lotMother, IEnumerable<tblLotStatu> lotStatuses)
        {
            var lotStatusDescriptions = lotStatuses.ToDictionary(s => LotStatHelper.GetLotStat(s.LotStatID).Value, s => s.LotStatDesc);
            _serializedChileLotHelper = new SerializedChileLotHelper(lotMother, lotStatusDescriptions);
            _unserializedChileLotHelper = new UnserializedChileLotHelper(lotMother, lotStatusDescriptions);
        }

        public ChileLot CreateChileLot(LotDTO oldLot, Lot newLot, SerializableLot deserialized, ChileProduct chileProduct, IEnumerable<AttributeName> attributeNames, out List<LotAttributeDefect> lotAttributeDefects)
        {
            if(SerializableLot.UpdateLot(newLot, deserialized, oldLot.TestDate != null, out lotAttributeDefects))
            {
                return _serializedChileLotHelper.CreateSerializedChileLot(oldLot, newLot, chileProduct, attributeNames, ref lotAttributeDefects);
            }
            
            return _unserializedChileLotHelper.CreateUnserializedChileLot(oldLot, newLot, chileProduct, attributeNames, out lotAttributeDefects);
        }

        public class AttributeCommonData
        {
            public bool NullTestDate { get; private set; }

            public int TesterId
            {
                get
                {
                    if(_defaultTester && _logMethod != null)
                    {
                        _logMethod(new CallbackParameters(_oldLot, CallbackReason.TesterIDNullUsedDefault)
                            {
                                DefaultEmployeeID = _testerId,
                            });
                        _defaultTester = false;
                    }
                    return _testerId;
                }
            }

            public DateTime DeterminedTestDate
            {
                get
                {
                    if(_defaultTestDate && _logMethod != null)
                    {
                        _logMethod(new CallbackParameters(_oldLot, CallbackReason.TestDateNullCurrentDateUsed));
                        _defaultTestDate = false;
                    }
                    return _determinedTestDate;
                }
            }

            public DateTime? EntryDate
            {
                get
                {
                    if(_nullEntryDate && _logMethod != null)
                    {
                        _logMethod(new CallbackParameters(_oldLot, CallbackReason.EntryDateNull));
                        _nullEntryDate = false;
                    }
                    return _entryDate;
                }
            }

            public AttributeCommonData(LotDTO oldLot, ILotMother lotMother)
            {
                var testerId = oldLot.TesterID == null ? lotMother.DefaultEmployee.EmployeeId : oldLot.TesterID.Value;
                var determinedTestDate = (oldLot.TestDate ?? oldLot.ProductionDate ?? oldLot.EntryDate ?? Models.Helpers.DataConstants.SqlMinDate).Date;
                var entryDate = oldLot.EntryDate.ConvertLocalToUTC();

                _oldLot = oldLot;
                _logMethod = lotMother.Log;

                _testerId = testerId;
                _determinedTestDate = determinedTestDate;
                _entryDate = entryDate;

                _defaultTester = oldLot.TesterID == null;
                _defaultTestDate = determinedTestDate == Models.Helpers.DataConstants.SqlMinDate;
                _nullEntryDate = oldLot.EntryDate == null;
                NullTestDate = oldLot.TestDate == null;
            }

            private readonly LotDTO _oldLot;
            private readonly Action<CallbackParameters> _logMethod;

            private readonly int _testerId;
            private readonly DateTime _determinedTestDate;
            private readonly DateTime? _entryDate;

            private bool _defaultTester;
            private bool _defaultTestDate;
            private bool _nullEntryDate;
        }

        private abstract class ChileLotHelperBase
        {
            protected readonly ILotMother LotMother;

            protected ChileLotHelperBase(ILotMother lotMother)
            {
                LotMother = lotMother;
            }

            protected ChileLot CreateChileLot(LotDTO oldLot, Lot newLot, ChileProduct chileProduct, IEnumerable<AttributeName> attributeNames, ref List<LotAttributeDefect> attributeDefects)
            {
                var newChileLot = new ChileLot
                    {
                        LotDateCreated = newLot.LotDateCreated,
                        LotDateSequence = newLot.LotDateSequence,
                        LotTypeId = newLot.LotTypeId,
                        Lot = newLot,

                        ChileProductId = chileProduct.Id,
                        ChileProduct = chileProduct,
                    };

                return SetLotStatuses(oldLot, newChileLot, attributeNames, ref attributeDefects) ? newChileLot : null;
            }

            [Issue("Setting AllAttributeAreLoBac according to web system logic.",
                References = new[] { "RVCADMIN-1298" })]
            private bool SetLotStatuses(LotDTO oldLot, ChileLot newChileLot, IEnumerable<AttributeName> attributeNames, ref List<LotAttributeDefect> attributeDefects)
            {
                newChileLot.AllAttributesAreLoBac = LotStatusHelper.DetermineChileLotIsLoBac(newChileLot, attributeNames);
                newChileLot.Lot.ProductionStatus = oldLot._BatchStatID < 3 ? LotProductionStatus.Batched : LotProductionStatus.Produced;
                LotMother.SetLotHoldStatus(oldLot.LotStat, newChileLot.Lot);
                newChileLot.Lot.ProductSpecComplete = LotStatusHelper.ChileLotAllProductSpecsEntered(newChileLot);
                newChileLot.Lot.ProductSpecOutOfRange = LotStatusHelper.ChileLotAttributeOutOfRange(newChileLot);

                if(!SetLotQualityStatus(oldLot.LotStat, newChileLot.Lot, ref attributeDefects))
                {
                    LotMother.Log(new CallbackParameters(oldLot, CallbackReason.ChileLotStatusConflict)
                        {
                            ChileLot = newChileLot
                        });
                    return false;
                }

                return true;
            }
            
            [Issue("Will issue resolutions to unresolved lot attribute defects if LotStat is shipable.", 
                References = new[] { "RVCADMIN-1265", "RVCADMIN-1194" })]
            private bool SetLotQualityStatus(LotStat? lotStat, Lot newLot, ref List<LotAttributeDefect> attributeDefects)
            {
                if(lotStat == LotStat.Rejected)
                {
                    newLot.QualityStatus = LotQualityStatus.Rejected;
                    return true;
                }

                if(lotStat.IsCompleted())
                {
                    //todo: get employee from oldLot.TesterID
                    attributeDefects.Where(d => d.LotDefect.Resolution == null).ForEach(d => d.LotDefect.ResolveDefect(LotMother.DefaultEmployee));
                }

                if(lotStat.IsAcceptable())
                {
                    newLot.QualityStatus = LotQualityStatus.Released;
                    return true;
                }

                var unresolvedContaminated = LotStatusHelper.LotHasUnresolvedDefects(newLot, DefectTypeEnum.BacterialContamination);
                if(lotStat == LotStat.Contaminated || unresolvedContaminated)
                {
                    newLot.QualityStatus = LotQualityStatus.Contaminated;
                    return true;
                }

                var unresolvedActionable = LotStatusHelper.LotHasUnresolvedDefects(newLot, DefectTypeEnum.ActionableDefect);
                if(lotStat.IsRequiresAttention() || (newLot.ProductSpecComplete && lotStat == LotStat.InProcess) || unresolvedActionable)
                {
                    newLot.QualityStatus = LotQualityStatus.Pending;
                    return true;
                }

                if(newLot.ProductSpecComplete)
                {
                    newLot.QualityStatus = LotQualityStatus.Released;
                    return true;
                }

                if(lotStat == LotStat.InProcess || lotStat == null)
                {
                    newLot.QualityStatus = LotQualityStatus.Pending;
                    return true;
                }

                return false;
            }
        }

        private class SerializedChileLotHelper : ChileLotHelperBase
        {
            private readonly Dictionary<LotStat, string> _lotStatusDescriptions;

            public SerializedChileLotHelper(ILotMother lotMother, Dictionary<LotStat, string> lotStatusDescriptions)
                : base(lotMother)
            {
                _lotStatusDescriptions = lotStatusDescriptions;
            }

            public ChileLot CreateSerializedChileLot(LotDTO oldLot, Lot newLot, ChileProduct chileProduct, IEnumerable<AttributeName> attributeNames, ref List<LotAttributeDefect> lotAttributeDefects)
            {
                UpdateAttributeDefectTypes(lotAttributeDefects);
                ReconcileLotAttributesAndDefects(oldLot, newLot, chileProduct, new AttributeCommonData(oldLot, LotMother), ref lotAttributeDefects);
                ReconcileInHouseContaminationDefects(oldLot, newLot);
                return CreateChileLot(oldLot, newLot, chileProduct, attributeNames, ref lotAttributeDefects);
            }

            private void UpdateAttributeDefectTypes(IEnumerable<LotAttributeDefect> lotAttributeDefects)
            {
                foreach(var attributeDefect in lotAttributeDefects)
                {
                    AttributeName attributeName;
                    if(LotMother.AttributeNames.TryGetValue(new AttributeNameKey(attributeDefect), out attributeName))
                    {
                        attributeDefect.LotDefect.DefectType = attributeName.DefectType;
                    }
                }
            }

            private void ReconcileInHouseContaminationDefects(LotDTO oldLot, Lot newLot)
            {
                string description = null;
                switch(oldLot.LotStat)
                {
                    case LotStat.See_Desc:
                        description = string.IsNullOrEmpty(oldLot.Notes) ? "Reason Unspecified" : oldLot.Notes;
                        break;

                    case LotStat.Dark_Specs:
                    case LotStat.Smoke_Cont:
                    case LotStat.Hard_BBs:
                    case LotStat.Soft_BBs:
                        description = _lotStatusDescriptions[oldLot.LotStat.Value];
                        break;
                }

                if(description != null)
                {
                    LotMother.AddRead(EntityTypes.LotDefect);
                    if(!newLot.LotDefects.Any(d => d.DefectType == DefectTypeEnum.InHouseContamination && d.Resolution == null && d.Description == description))
                    {
                        newLot.AddNewDefect(DefectTypeEnum.InHouseContamination, description);
                    }
                }
            }

            private void ReconcileLotAttributesAndDefects(LotDTO oldLot, Lot newLot, ChileProduct chileProduct, AttributeCommonData attributeData, ref List<LotAttributeDefect> lotAttributeDefects)
            {
                foreach(var name in StaticAttributeNames.AttributeNames)
                {
                    var getValue = oldLot.AttributeGet(name);
                    if(getValue == null)
                    {
                        continue;
                    }
                    var oldContextValue = getValue();

                    var nameKey = new AttributeNameKey(name);
                    var productAttributeRange = chileProduct.ProductAttributeRanges.FirstOrDefault(r => nameKey.Equals(r));
                    var newContextAttribute = newLot.Attributes.FirstOrDefault(a => nameKey.Equals(a));

                    if(newContextAttribute == null)
                    {
                        if(oldContextValue != null)
                        {
                            AddNewLotAttribute(oldLot, newLot, productAttributeRange, name, attributeData, (double)oldContextValue, ref lotAttributeDefects);
                        }
                    }
                    else
                    {
                        if(oldContextValue != null)
                        {
                            UpdateExistingLotAttribute(newContextAttribute, name, attributeData, productAttributeRange, (double)oldContextValue, ref lotAttributeDefects);
                        }
                        else
                        {
                            RemoveExistingLotAttribute(newContextAttribute, ref lotAttributeDefects);
                        }
                    }
                }
            }

            private void AddNewLotAttribute(ILotAttributes oldAttributes, Lot newLot, ChileProductAttributeRange range, AttributeName attributeName, AttributeCommonData attributeData, double value, ref List<LotAttributeDefect> lotAttributeDefects)
            {
                var attribute = newLot.AddNewAttribute(oldAttributes, attributeName, attributeData, LotMother);
                if(attribute == null)
                {
                    return;
                }

                if(range.ValueOutOfRange(attribute.AttributeValue))
                {
                    LotMother.AddRead(EntityTypes.LotDefect);
                    LotMother.AddRead(EntityTypes.LotAttributeDefect);

                    CreateOrUpdateOpenAttributeDefect(newLot, attributeName, value, range, ref lotAttributeDefects);
                }
                else
                {
                    CloseOpenAttributeDefects(attribute, lotAttributeDefects);
                }
            }

            private void CreateOrUpdateOpenAttributeDefect(Lot lot, AttributeName attribute, double value, IAttributeRange range, ref List<LotAttributeDefect> lotAttributeDefects)
            {
                var nameKey = attribute.ToAttributeNameKey();
                var attributeDefect = lotAttributeDefects.FirstOrDefault(d => d.LotDefect.Resolution == null && nameKey.Equals(d));
                if(attributeDefect == null)
                {
                    var newDefect = lot.AddNewDefect(attribute.DefectType, attribute.Name);
                    attributeDefect = new LotAttributeDefect
                        {
                            LotDateCreated = lot.LotDateCreated,
                            LotDateSequence = lot.LotDateSequence,
                            LotTypeId = lot.LotTypeId,
                            DefectId = newDefect.DefectId,
                            LotDefect = newDefect,
                            AttributeShortName = nameKey.AttributeNameKey_ShortName
                        };
                    lotAttributeDefects.Add(attributeDefect);
                }

                attributeDefect.OriginalAttributeValue = value;
                attributeDefect.OriginalAttributeMinLimit = range.RangeMin;
                attributeDefect.OriginalAttributeMaxLimit = range.RangeMax;

                CloseOpenAttributeDefects(nameKey, lotAttributeDefects.Where(a => a != attributeDefect));
            }

            private void UpdateExistingLotAttribute(LotAttribute existingLotAttribute, AttributeName attributeName, AttributeCommonData attributeData, ChileProductAttributeRange range, double value, ref List<LotAttributeDefect> lotAttributeDefects)
            {
                if(range.ValueOutOfRange(value))
                {
                    CreateOrUpdateOpenAttributeDefect(existingLotAttribute.Lot, attributeName, value, range, ref lotAttributeDefects);
                }
                else
                {
                    CloseOpenAttributeDefects(existingLotAttribute, lotAttributeDefects);
                }

                if(Math.Abs(existingLotAttribute.AttributeValue - value) > 0.0001)
                {
                    existingLotAttribute.AttributeValue = value;
                    existingLotAttribute.Computed = attributeData.NullTestDate;
                    existingLotAttribute.AttributeDate = attributeData.DeterminedTestDate;
                }
            }

            private void RemoveExistingLotAttribute(LotAttribute existingLotAttribute, ref List<LotAttributeDefect> lotAttributeDefects)
            {
                CloseOpenAttributeDefects(existingLotAttribute, lotAttributeDefects);
                existingLotAttribute.Lot.Attributes.Remove(existingLotAttribute);
            }

            private void CloseOpenAttributeDefects(IAttributeNameKey attributeNameKey, IEnumerable<LotAttributeDefect> lotAttributeDefects)
            {
                var nameKey = attributeNameKey.ToAttributeNameKey();
                foreach(var attributeDefect in lotAttributeDefects.Where(a => a.LotDefect.Resolution == null && nameKey.Equals(a)).ToList())
                {
                    attributeDefect.LotDefect.ResolveDefect(LotMother.DefaultEmployee);
                }
            }
        }

        private class UnserializedChileLotHelper : ChileLotHelperBase
        {
            private readonly Dictionary<LotStat, string> _lotStatusDescriptions;

            public UnserializedChileLotHelper(ILotMother lotMother, Dictionary<LotStat, string> lotStatusDescriptions)
                : base(lotMother)
            {
                _lotStatusDescriptions = lotStatusDescriptions;
            }

            public ChileLot CreateUnserializedChileLot(LotDTO oldLot, Lot newLot, ChileProduct chileProduct, IEnumerable<AttributeName> attributeNames, out List<LotAttributeDefect> lotAttributeDefects)
            {
                var attributeCommonData = new AttributeCommonData(oldLot, LotMother);
                newLot.Attributes = new List<LotAttribute>();
                foreach(var attribute in StaticAttributeNames.AttributeNames)
                {
                    var newAttribute = newLot.AddNewAttribute(oldLot, attribute, attributeCommonData, LotMother);
                    if(newAttribute != null)
                    {
                        var wrapper = new ILotAttributesExtensions.LotAttributeWrapper(oldLot, oldLot.OriginalHistory, attribute);
                        newAttribute.Computed = wrapper.Computed;
                    }
                }

                return CreateChileLotAndDefects(oldLot, newLot, chileProduct, attributeNames, out lotAttributeDefects);
            }

            private ChileLot CreateChileLotAndDefects(LotDTO oldLot, Lot newLot, ChileProduct chileProduct, IEnumerable<AttributeName> attributeNames, out List<LotAttributeDefect> lotAttributeDefects)
            {
                lotAttributeDefects = new List<LotAttributeDefect>();
                AddLotAttributeDefects(newLot, chileProduct, DefectTypeEnum.ProductSpec, ref lotAttributeDefects);
                AddLotAttributeDefects(newLot, chileProduct, DefectTypeEnum.ActionableDefect, ref lotAttributeDefects);
                AddLotAttributeDefects(newLot, chileProduct, DefectTypeEnum.BacterialContamination, ref lotAttributeDefects);
                AddInHouseContaminationDefect(oldLot, newLot);

                return CreateChileLot(oldLot, newLot, chileProduct, attributeNames, ref lotAttributeDefects);
            }

            private void AddLotAttributeDefects(Lot newLot, ChileProduct chileProduct, DefectTypeEnum defectType, ref List<LotAttributeDefect> lotAttributeDefects)
            {
                var defectAttributes = LotMother.AttributeNames.Values.Where(a => a.DefectType == defectType);
                var chileProductSpecs = chileProduct.ProductAttributeRanges.Join(defectAttributes,
                                                                                 r => r.AttributeShortName,
                                                                                 a => a.ShortName,
                                                                                 (r, a) => new { Range = r, Attribute = a }).ToList();
                foreach(var productSpec in chileProductSpecs)
                {
                    var attributeRange = productSpec.Range;
                    var attribute = productSpec.Attribute;
                    var lotAttribute = newLot.Attributes.FirstOrDefault(a => a.AttributeShortName == attributeRange.AttributeShortName);
                    if(lotAttribute != null)
                    {
                        if(attributeRange.ValueOutOfRange(lotAttribute.AttributeValue))
                        {
                            LotMother.AddRead(EntityTypes.LotDefect);
                            LotMother.AddRead(EntityTypes.LotAttributeDefect);

                            var lotDefect = newLot.AddNewDefect(defectType, attribute.Name);

                            var lotAttributeDefect = new LotAttributeDefect
                                {
                                    LotDateCreated = lotDefect.LotDateCreated,
                                    LotDateSequence = lotDefect.LotDateSequence,
                                    LotTypeId = lotDefect.LotTypeId,
                                    DefectId = lotDefect.DefectId,
                                    AttributeShortName = attribute.ShortName,
                                    LotDefect = lotDefect,
                                    OriginalAttributeValue = lotAttribute.AttributeValue,
                                    OriginalAttributeMinLimit = attributeRange.RangeMin,
                                    OriginalAttributeMaxLimit = attributeRange.RangeMax
                                };

                            lotAttributeDefects.Add(lotAttributeDefect);
                        }
                    }
                }
            }

            private void AddInHouseContaminationDefect(LotDTO oldLot, Lot newLot)
            {
                if(oldLot.LotStat.IsInHouseContamination())
                {
                    LotMother.AddRead(EntityTypes.LotDefect);
                    newLot.AddNewDefect(DefectTypeEnum.InHouseContamination, _lotStatusDescriptions[oldLot.LotStat.Value]);
                }
            }
        }
    }

    public interface ILotMother
    {
        IDictionary<string, AttributeName> AttributeNames { get; }
        Employee DefaultEmployee { get; }

        void AddRead(EntityTypes entityType);
        void Log(CallbackParameters callbackParameteres);
        void SetLotHoldStatus(LotStat? lotStat, Lot newLot);
    }

    public static class LotExtensions
    {
        public static LotDefect AddNewDefect(this Lot lot, DefectTypeEnum defectType, string description)
        {
            var nextDefectId = lot.LotDefects.Any() ? lot.LotDefects.Max(d => d.DefectId) + 1 : 1;
            var newDefect = new LotDefect
                {
                    LotDateCreated = lot.LotDateCreated,
                    LotDateSequence = lot.LotDateSequence,
                    LotTypeId = lot.LotTypeId,
                    DefectId = nextDefectId,

                    DefectType = defectType,
                    Description = description,
                };
            lot.LotDefects.Add(newDefect);
            return newDefect;
        }

        public static LotAttribute AddNewAttribute(this Lot lot, ILotAttributes oldLot, IAttributeNameKey attributeNameKey, CreateChileLotHelper.AttributeCommonData attributeData, ILotMother lotMother)
        {
            var getter = oldLot.AttributeGet(attributeNameKey);
            if(getter == null)
            {
                return null;
            }

            var setter = oldLot.AttributeSet(attributeNameKey, true);
            var value = setter(getter());
            if(value == null)
            {
                return null;
            }
            
            lotMother.AddRead(EntityTypes.LotAttribute);
            if(attributeData.EntryDate == null)
            {
                return null;
            }

            var lotAttribute = new LotAttribute
                {
                    LotDateCreated = lot.LotDateCreated,
                    LotDateSequence = lot.LotDateSequence,
                    LotTypeId = lot.LotTypeId,
                    AttributeShortName = attributeNameKey.AttributeNameKey_ShortName,

                    AttributeValue = (double)value,
                    AttributeDate = attributeData.DeterminedTestDate,

                    EmployeeId = attributeData.TesterId,
                    TimeStamp = attributeData.EntryDate.Value,
                    Computed = attributeData.NullTestDate
                };

            lot.Attributes.Add(lotAttribute);
            return lotAttribute;
        }

        public static LotDefectResolution ResolveDefect(this LotDefect lotDefect, IEmployeeKey employee, ResolutionTypeEnum? resolutionType = null, string resolutionDescription = null)
        {
            if(lotDefect.Resolution != null) { throw new Exception(string.Format("LotDefect[{0}] already has resolution.", new LotKey(lotDefect))); }
            
            var resolution = new LotDefectResolution
                {
                    EmployeeId = employee.EmployeeKey_Id,
                    TimeStamp = DateTime.UtcNow,

                    LotDateCreated = lotDefect.LotKey_DateCreated,
                    LotDateSequence = lotDefect.LotKey_DateSequence,
                    LotTypeId = lotDefect.LotKey_LotTypeId,
                    DefectId = lotDefect.DefectId,

                    ResolutionType = resolutionType ?? ResolutionTypeEnum.AcceptedByUser,
                    Description = resolutionDescription ?? "Resolved by user in Access."
                };
            lotDefect.Resolution = resolution;
            return resolution;
        }
    }
}