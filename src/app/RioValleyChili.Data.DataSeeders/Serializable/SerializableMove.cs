using System;
using Newtonsoft.Json;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Serializable
{
    [JsonObject(MemberSerialization.Fields)]
    public class SerializableMove
    {
        public SerializableTreatmentOrder TreatmentOrder;
        public SerializableEmployeeIdentifiable Identifiable;

        public static string Serialize(TreatmentOrder treatmentOrder)
        {
            return JsonConvert.SerializeObject(new SerializableMove(treatmentOrder), Formatting.None);
        }

        public static string Serialize(InventoryShipmentOrder order)
        {
            return JsonConvert.SerializeObject(new SerializableMove(order), Formatting.None);
        }

        public static SerializableMove Deserialize(string serialized)
        {
            if(string.IsNullOrWhiteSpace(serialized))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<SerializableMove>(serialized);
        }

        public SerializableMove(TreatmentOrder order) : this(order.InventoryShipmentOrder)
        {
            TreatmentOrder = new SerializableTreatmentOrder(order);
        }

        public SerializableMove(InventoryShipmentOrder order)
        {
            Identifiable = new SerializableEmployeeIdentifiable(order);
        }

        public void SetOrder(InventoryShipmentOrder order)
        {
            order.EmployeeId = Identifiable.EmployeeKey.EmployeeKeyId;
            order.TimeStamp = Identifiable.TimeStamp;
        }

        public void SetOrder(TreatmentOrder order)
        {
            if(TreatmentOrder != null && TreatmentOrder.TreatmentKey != null)
            {
                order.InventoryTreatmentId = TreatmentOrder.TreatmentKey.InventoryTreatmentKeyId;
            }

            SetOrder(order.InventoryShipmentOrder);
        }

        [JsonObject(MemberSerialization.Fields)]
        public class SerializableTreatmentOrder
        {
            [Issue("Normally, storing newContext-specific info like this wouldn't fly, but in this case we can get away with it because we know that newContext InventoryTreatment keys are actually" +
                   "based off of the old context tblTreatment.Id. Still, it freaked me out to see this usage for a bit there. -RI 2016-09-19")]
            public SerializableInventoryTreatmentKey TreatmentKey;

            public SerializableTreatmentOrder(TreatmentOrder order)
            {
                TreatmentKey = new SerializableInventoryTreatmentKey(order);
            }
        }
    }
}