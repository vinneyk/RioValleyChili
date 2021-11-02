using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Extensions
{
    public static class LotAttributeDefectHelpers
    {
        public static IEnumerable<LotAttributeDefect> GetUnresolvedDefects(this IEnumerable<LotAttributeDefect> source, string attributeNameShort = null)
        {
            return source.Where(d => (attributeNameShort == null || d.AttributeShortName == attributeNameShort) && d.LotDefect.Resolution == null);
        }
    }

    internal static class LotAttributeHelpers
    {
        public static bool IsValidResolution(this LotAttribute attribute, ResolutionTypeEnum resolutionType)
        {
            return attribute.AttributeName.IsValidResolution(resolutionType);
        }
    }

    internal static class AttributeNameExtensions
    {
        internal static Dictionary<ProductTypeEnum, IQueryable<AttributeName>> ByProductType(this IQueryable<AttributeName> names)
        {
            if(names == null) { throw new ArgumentNullException("names"); }

            return new Dictionary<ProductTypeEnum, IQueryable<AttributeName>>
                {
                    { ProductTypeEnum.Chile, names.Where(n => n.Active && n.ValidForChileInventory) },
                    { ProductTypeEnum.Additive, names.Where(n => n.Active && n.ValidForAdditiveInventory) },
                    { ProductTypeEnum.Packaging, names.Where(n => n.Active && n.ValidForPackagingInventory) },
                };
        }

        internal static bool IsValidResolution(this AttributeName attributeName, ResolutionTypeEnum resolutionType)
        {
            return attributeName.DefectType.GetValidResolutions().Contains(resolutionType);
        }
    }

    internal static class DefectTypeExtensions
    {
        internal static IEnumerable<ResolutionTypeEnum> GetValidResolutions(this DefectTypeEnum defectType)
        {
            // NOTE: The use of the Treated resolution type seems unnecessary since the InventoryTreatmentForAttributes table enables more proper control of this validity.
            var resolutions = new List<ResolutionTypeEnum>
                {
                    ResolutionTypeEnum.DataEntryCorrection,
                    ResolutionTypeEnum.Retest,
                    ResolutionTypeEnum.InvalidValue
                };

            switch(defectType)
            {
                case DefectTypeEnum.ProductSpec:
                    resolutions.Add(ResolutionTypeEnum.ReworkPerformed);
                    resolutions.Add(ResolutionTypeEnum.AcceptedByUser);
                    resolutions.Add(ResolutionTypeEnum.AcceptedByDataLoad);
                    break;

                case DefectTypeEnum.BacterialContamination:
                    resolutions.Add(ResolutionTypeEnum.Treated);
                    break;

                case DefectTypeEnum.InHouseContamination:
                    break;

                case DefectTypeEnum.ActionableDefect:
                    resolutions.Add(ResolutionTypeEnum.Treated);
                    resolutions.Add(ResolutionTypeEnum.AcceptedByUser);
                    resolutions.Add(ResolutionTypeEnum.AcceptedByDataLoad);
                    break;

                default: throw new ArgumentOutOfRangeException("defectType");
            }

            return resolutions;
        }
    }
}