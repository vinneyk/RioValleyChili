using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Helpers;
using RioValleyChili.Data.Models.Interfaces;
using RioValleyChili.Data.Models.StaticRecords;
using Solutionhead.Services;

namespace RioValleyChili.Business.Core.Helpers
{
    public static class LotStatusHelper
    {
        public static Expression<Func<ChileLot, object>>[] ChileLotIncludePaths
        {
            get
            {
                return new Expression<Func<ChileLot, object>>[]
                    {
                        c => c.ChileProduct.ProductAttributeRanges.Select(r => r.AttributeName),
                        c => c.Lot.Attributes,
                        c => c.Lot.LotDefects.Select(d => d.Resolution)
                    };
            }
        }

        public static IResult UpdateChileLotStatus(ChileLot chileLot, List<AttributeName> attributeNames)
        {
            if(chileLot == null) { throw new ArgumentNullException("chileLot"); }
            if(attributeNames == null) { throw new ArgumentNullException("attributeNames"); }
            
            chileLot.AllAttributesAreLoBac = DetermineChileLotIsLoBac(chileLot, attributeNames);
            chileLot.Lot.ProductSpecComplete = ChileLotAllProductSpecsEntered(chileLot);
            chileLot.Lot.ProductSpecOutOfRange = ChileLotAttributeOutOfRange(chileLot);
            chileLot.Lot.QualityStatus = PreserveQualityStatus(chileLot.Lot.QualityStatus, DetermineLotQualityStatus(chileLot, chileLot.Lot.ProductSpecComplete));

            return new SuccessResult();
        }

        public static bool LotHasUnresolvedDefects(Lot lot, params DefectTypeEnum[] defectTypes)
        {
            var allDefectTypes = defectTypes == null || !defectTypes.Any();
            return lot.LotDefects.Any(d => (allDefectTypes || defectTypes.Contains(d.DefectType)) && d.Resolution == null);
        }

        public static bool ChileLotAllProductSpecsEntered(ChileLot chileLot)
        {
            var lotAttributes = chileLot.Lot.Attributes.ToDictionary(a => a.ToAttributeNameKey());
            return chileLot.ChileProduct.ProductAttributeRanges.All(r =>
                {
                    LotAttribute attribute;
                    if(lotAttributes.TryGetValue(r.ToAttributeNameKey(), out attribute))
                    {
                        return !r.AttributeName.ActualValueRequired || !attribute.Computed;
                    }
                    return false;
                });
        }

        public static bool ChileLotAttributeOutOfRange(ChileLot chileLot, params DefectTypeEnum[] defectTypes)
        {
            var allDefectTypes = defectTypes == null || !defectTypes.Any();
            var defectRanges = chileLot.ChileProduct.ProductAttributeRanges.Where(r => (allDefectTypes || defectTypes.Contains(r.AttributeName.DefectType)));

            return GetAttributeDefects(chileLot.Lot.Attributes.ToDictionary(a => a.ToAttributeNameKey().KeyValue, a => a.AttributeValue),
                defectRanges.ToDictionary(r => r.ToAttributeNameKey().KeyValue, r => (IAttributeRange)r))
                .Any();
        }

        public static List<IAttributeDefect> GetAttributeDefects(IDictionary<string, double> attributes, IDictionary<string, IAttributeRange> attributeRanges)
        {
            var attributesOutOfRange = new List<IAttributeDefect>();
            foreach(var attribute in attributes)
            {
                IAttributeRange range;
                if(attributeRanges.TryGetValue(attribute.Key, out range))
                {
                    attributesOutOfRange.Add(new AttributeRangeResult(range, attribute.Value));
                }
            }

            return attributesOutOfRange.Where(r => r.AbsoluteValueDelta() > 0.0).ToList();
        }

        public static LotQualityStatus DetermineLotQualityStatus(ChileLot chileLot)
        {
            return DetermineLotQualityStatus(chileLot, ChileLotAllProductSpecsEntered(chileLot));
        }

        public static LotQualityStatus DetermineLotQualityStatus(ChileLot chileLot, bool productSpecComplete)
        {
            if(chileLot == null) { throw new ArgumentNullException("chileLot"); }

            if(LotHasUnresolvedDefects(chileLot.Lot, DefectTypeEnum.BacterialContamination))
            {
                return LotQualityStatus.Contaminated;
            }

            return productSpecComplete ? LotQualityStatus.Released : LotQualityStatus.Pending;
        }

        public static LotQualityStatus PreserveQualityStatus(LotQualityStatus currentStatus, LotQualityStatus determinedStatus)
        {
            switch(determinedStatus)
            {
                case LotQualityStatus.Pending:
                    switch(currentStatus)
                    {
                        case LotQualityStatus.Released:
                        case LotQualityStatus.Contaminated:
                            return LotQualityStatus.Pending;
                    }
                    break;

                case LotQualityStatus.Released:
                    switch(currentStatus)
                    {
                        case LotQualityStatus.Pending:
                        case LotQualityStatus.Contaminated:
                            return LotQualityStatus.Released;
                    }
                    break;

                case LotQualityStatus.Contaminated:
                    switch(currentStatus)
                    {
                        case LotQualityStatus.Pending:
                        case LotQualityStatus.Released:
                            return LotQualityStatus.Contaminated;
                    }
                    break;
            }

            return currentStatus;
        }

        public static bool DetermineChileLotIsLoBac(ILotContainer lotContainer, IEnumerable<AttributeName> attributeNames)
        {
            if(lotContainer == null) { throw new ArgumentNullException("lotContainer"); }
            if(attributeNames == null) { throw new ArgumentNullException("attributeNames"); }

            var attributes = (attributeNames as AttributeName[] ?? attributeNames.ToArray()).ToList();
            var scanAttributeNameKey = StaticAttributeNames.Scan.ToAttributeNameKey();
            if(AttributeInLoBacRange(lotContainer, attributes, scanAttributeNameKey))
            {
                return true;
            }

            attributes = attributes.Where(a => a.LoBacLimit != null && !scanAttributeNameKey.Equals(a)).ToList();
            return attributes.All(a => AttributeInLoBacRange(lotContainer, attributes, a));
        }

        private static bool AttributeInLoBacRange(ILotContainer lotContainer, IEnumerable<AttributeName> attributeNames, IAttributeNameKey staticAttributeName)
        {
            var attributeName = attributeNames.SingleOrDefault(n => n.AttributeNameKey_ShortName == staticAttributeName.AttributeNameKey_ShortName);
            if(attributeName == null)
            {
                return false;
            }

            var attribute = lotContainer.Lot.Attributes.SingleOrDefault(c => c.AttributeShortName == attributeName.AttributeNameKey_ShortName);
            if(attribute == null)
            {
                return false;
            }

            return attribute.AttributeValue <= attributeName.LoBacLimit;
        }

        public static IEnumerable<LotQualityStatus> GetValidLotQualityStatuses(ChileLot chileLot)
        {
            return GetValidLotQualityStatuses(chileLot.Lot.QualityStatus, chileLot.Lot.ProductSpecComplete, LotHasUnresolvedDefects(chileLot.Lot, DefectTypeEnum.ActionableDefect), LotHasUnresolvedDefects(chileLot.Lot, DefectTypeEnum.BacterialContamination));
        }

        public static IEnumerable<LotQualityStatus> GetValidLotQualityStatuses(LotQualityStatus currentLotQualityStatus, bool productSpecComplete, bool unresolvedActionableDefects, bool unresolvedContaminationDefects)
        {
            var validStatuses = new List<LotQualityStatus>
                {
                    currentLotQualityStatus,
                    LotQualityStatus.Rejected
                };

            if(!unresolvedContaminationDefects)
            {
                validStatuses.Add(LotQualityStatus.Released);

                if(unresolvedActionableDefects)
                {
                    validStatuses.Add(LotQualityStatus.Contaminated);
                }
                
                if(unresolvedActionableDefects || !productSpecComplete)
                {
                    validStatuses.Add(LotQualityStatus.Pending);
                }
            }
            else
            {
                validStatuses.Add(LotQualityStatus.Contaminated);
            }

            return validStatuses.Distinct().OrderBy(s => s).ToList();
        }

        private class AttributeRangeResult : IAttributeDefect
        {
            public double Value { get; private set; }
            public DefectTypeEnum DefectType { get { return DefectTypeEnum.ProductSpec; } }
            public bool HasResolution { get { return false; } }
            public string AttributeNameKey_ShortName { get { return _range.AttributeNameKey_ShortName; } }
            public double RangeMin { get { return _range.RangeMin; } }
            public double RangeMax { get { return _range.RangeMax; } }

            private readonly IAttributeRange _range;

            public AttributeRangeResult(IAttributeRange range, double value)
            {
                _range = range;
                Value = value;
            }
        }
    }
}