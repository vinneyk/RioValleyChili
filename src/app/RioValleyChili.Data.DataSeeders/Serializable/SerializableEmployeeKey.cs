using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.DataSeeders.Serializable
{
    public class SerializableEmployeeKey : Serializable.Base<IEmployeeKey>, IEmployeeKey
    {
        public int EmployeeKeyId;

        public SerializableEmployeeKey(IEmployeeKey source) : base(source) { }

        protected override void InitializeFromSource(IEmployeeKey source)
        {
            EmployeeKeyId = source.EmployeeKey_Id;
        }

        public int EmployeeKey_Id { get { return EmployeeKeyId; } }
    }
}