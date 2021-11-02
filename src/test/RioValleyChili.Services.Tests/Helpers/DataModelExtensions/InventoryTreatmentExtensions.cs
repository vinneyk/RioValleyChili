using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class InventoryTreatmentExtensions
    {
        internal static InventoryTreatment SetKey(this InventoryTreatment treatment, IInventoryTreatmentKey treatmentKey)
        {
            treatment.Id = treatmentKey.InventoryTreatmentKey_Id;
            return treatment;
        }

        internal static void AssertEqual(this InventoryTreatment inventoryTreatment, IInventoryTreatmentReturn inventoryTreatmentReturn)
        {
            if(inventoryTreatment == null) { throw new ArgumentNullException("inventoryTreatment"); }
            if(inventoryTreatmentReturn == null) { throw new ArgumentNullException("inventoryTreatmentReturn"); }

            Assert.AreEqual(new InventoryTreatmentKey(inventoryTreatment).KeyValue, inventoryTreatmentReturn.TreatmentKey);
            Assert.AreEqual(inventoryTreatment.LongName, inventoryTreatmentReturn.TreatmentName);
            Assert.AreEqual(inventoryTreatment.ShortName, inventoryTreatmentReturn.TreatmentNameShort);
        }
    }
}