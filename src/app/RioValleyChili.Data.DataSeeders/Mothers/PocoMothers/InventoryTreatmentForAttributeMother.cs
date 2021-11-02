using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;

namespace RioValleyChili.Data.DataSeeders.Mothers.PocoMothers
{
    public class InventoryTreatmentForAttributeMother : IMother<InventoryTreatmentForAttribute>
    {
        public IEnumerable<InventoryTreatmentForAttribute> BirthAll(Action consoleCallback = null)
        {
            var attributesValidForTreatment = new[]
            {
                StaticAttributeNames.Salmonella.ShortName,
                StaticAttributeNames.EColi.ShortName,
                StaticAttributeNames.TPC.ShortName,
                StaticAttributeNames.Yeast.ShortName,
                StaticAttributeNames.Mold.ShortName,
                StaticAttributeNames.ColiForms.ShortName,
            };

            return CreateInventoryTreatmentForAttributes(StaticInventoryTreatments.GT.Id, attributesValidForTreatment)
                .Concat(CreateInventoryTreatmentForAttributes(StaticInventoryTreatments.ETO.Id, attributesValidForTreatment));
        }

        private IEnumerable<InventoryTreatmentForAttribute> CreateInventoryTreatmentForAttributes(int treatmentId,
            string[] attributeShortNames)
        {
            return attributeShortNames.Select(a => new InventoryTreatmentForAttribute
            {
                AttributeShortName = a,
                TreatmentId = treatmentId
            });
        } 
    }
}