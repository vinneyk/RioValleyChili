using System;
using Newtonsoft.Json;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Serializable
{
    [JsonObject(MemberSerialization.Fields)]
    public class SerializableBatchInstruction
    {
        public static string Serialize(Note note)
        {
            return JsonConvert.SerializeObject(new SerializableBatchInstruction(note));
        }

        public static bool DeserializeIntoNote(Note note, string serializedBatchInstruction)
        {
            if(string.IsNullOrWhiteSpace(serializedBatchInstruction))
            {
                return false;
            }

            var deserialized = Deserialize(serializedBatchInstruction);
            note.TimeStamp = deserialized.TimeStamp;

            IEmployeeKey employeeKey;
            if(new EmployeeKey().TryParse(deserialized.EmployeeKey, out employeeKey))
            {
                note.EmployeeId = employeeKey.EmployeeKey_Id;
            }

            return true;
        }

        private SerializableBatchInstruction(Note note)
        {
            TimeStamp = note.TimeStamp;
            EmployeeKey = new EmployeeKey(note);
        }

        private static SerializableBatchInstruction Deserialize(string serializedBatchInstruction)
        {
            return JsonConvert.DeserializeObject<SerializableBatchInstruction>(serializedBatchInstruction);
        }

        public DateTime TimeStamp;
        public string EmployeeKey;
    }
}