using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class InventoryTreatmentForAttributeExtensions
    {
        internal static InventoryTreatmentForAttribute ConstrainKeys(this InventoryTreatmentForAttribute treatmentForAttribute, IInventoryTreatmentKey treatmentKey = null, IAttributeNameKey attributeNameKey = null)
        {
            if(treatmentKey != null)
            {
                treatmentForAttribute.Treatment = null;
                treatmentForAttribute.TreatmentId = treatmentKey.InventoryTreatmentKey_Id;
            }

            if(attributeNameKey != null)
            {
                treatmentForAttribute.AttributeName = null;
                treatmentForAttribute.AttributeShortName = attributeNameKey.AttributeNameKey_ShortName;
            }

            return treatmentForAttribute;
        }
    }
}