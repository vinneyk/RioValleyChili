using System;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.DataSeeders.Serializable
{
    public class SerializableEmployeeIdentifiable : Serializable.Base<IEmployeeIdentifiableEntity>
    {
        public SerializableEmployeeKey EmployeeKey;
        public DateTime TimeStamp;

        public SerializableEmployeeIdentifiable(IEmployeeIdentifiableEntity source) : base(source) { }

        protected override void InitializeFromSource(IEmployeeIdentifiableEntity source)
        {
            EmployeeKey = new SerializableEmployeeKey(source);
            TimeStamp = source.TimeStamp;
        }
    }
}