using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class AttributeNameExtensions
    {
        internal static AttributeName SetKey(this AttributeName attributeName, IAttributeNameKey attributeNameKey)
        {
            attributeName.ShortName = attributeNameKey.AttributeNameKey_ShortName;
            return attributeName;
        }

        internal static AttributeName SetValues(this AttributeName attributeName, KeyValuePair<string, string> shortAndLongName, bool active, bool chile = false, bool additive = false, bool packaging = false, DefectTypeEnum? defectType = null)
        {
            return attributeName.SetValues(shortAndLongName.Key, shortAndLongName.Value, active, chile, additive, packaging, defectType);
        }

        internal static AttributeName SetValues(this AttributeName attributeName, string shortName, string name, bool active, bool chile = false, bool additive = false, bool packaging = false, DefectTypeEnum? defectType = null)
        {
            if(shortName != null)
            {
                attributeName.ShortName = shortName;
            }

            if(name != null)
            {
                attributeName.Name = name;
            }

            attributeName.Active = active;
            attributeName.ValidForChileInventory = chile;
            attributeName.ValidForAdditiveInventory = additive;
            attributeName.ValidForPackagingInventory = packaging;

            if(defectType != null)
            {
                attributeName.DefectType = (DefectTypeEnum) defectType;
            }

            return attributeName;
        }
    }
}