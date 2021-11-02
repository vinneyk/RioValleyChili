using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.DataSeeders.Serializable
{
    public class SerializableInventoryTreatmentKey : Serializable.Base<IInventoryTreatmentKey>, IInventoryTreatmentKey
    {
        public int InventoryTreatmentKeyId;

        public SerializableInventoryTreatmentKey(IInventoryTreatmentKey source) : base(source) { }

        protected override void InitializeFromSource(IInventoryTreatmentKey source)
        {
            InventoryTreatmentKeyId = source.InventoryTreatmentKey_Id;
        }

        public int InventoryTreatmentKey_Id { get { return InventoryTreatmentKeyId; } }
    }
}