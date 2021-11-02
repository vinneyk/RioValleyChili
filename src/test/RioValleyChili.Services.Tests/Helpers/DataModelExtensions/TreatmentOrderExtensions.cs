using System;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class TreatmentOrderExtensions
    {
        internal static TreatmentOrder ClearPickedItems(this TreatmentOrder treatmentOrder)
        {
            if(treatmentOrder == null) { throw new ArgumentNullException("treatmentOrder"); }

            if(treatmentOrder.InventoryShipmentOrder != null)
            {
                if(treatmentOrder.InventoryShipmentOrder.PickedInventory != null)
                {
                    treatmentOrder.InventoryShipmentOrder.PickedInventory.Items = null;
                }

                if(treatmentOrder.InventoryShipmentOrder.InventoryPickOrder != null)
                {
                    if(treatmentOrder.InventoryShipmentOrder.InventoryPickOrder.PickedInventory != null)
                    {
                        treatmentOrder.InventoryShipmentOrder.InventoryPickOrder.PickedInventory.Items = null;
                    }
                }
            }

            return treatmentOrder;
        }
    }
}