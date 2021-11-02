using System;
using RioValleyChili.Core;

namespace RioValleyChili.Data.Models.Helpers
{
    public static class AttributeNameFactory
    {
#warning - Need to define what DefectType attributes have. -RI 7/17/13
        public static AttributeName CreateAttributeName(string attributeName, string attributeAbbreviation = null, bool active = true, bool validForChile = true, bool validForAdditive = true, bool validForPackaging = false, double? loBacLimit = null, DefectTypeEnum defectType = DefectTypeEnum.ProductSpec)
        {
            return new AttributeName
                       {
                           Active = active,
                           ShortName = attributeAbbreviation ?? attributeName,
                           Name = attributeName,
                           ValidForChileInventory = validForChile,
                           ValidForAdditiveInventory = validForAdditive,
                           ValidForPackagingInventory = validForPackaging,
                           LoBacLimit = loBacLimit,
                           DefectType = defectType
                       };
        }

        public static AttributeName Init(AttributeName attributeName, params Action<AttributeName>[] inits)
        {
            if(inits != null)
            {
                foreach(var init in inits)
                {
                    init(attributeName);
                }
            }

            return attributeName;
        }
    }
}