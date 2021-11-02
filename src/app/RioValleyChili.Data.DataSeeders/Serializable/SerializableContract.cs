using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Serializable
{
    [JsonObject(MemberSerialization.Fields)]
    public class SerializableContract
    {
        public static string Serialize(Contract contract)
        {
            return JsonConvert.SerializeObject(new SerializableContract(contract), Formatting.None);
        }

        public static SerializableContract Deserialize(string serializedContract)
        {
            if(string.IsNullOrWhiteSpace(serializedContract))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<SerializableContract>(serializedContract);
        }

        public static void DeserializeIntoContract(Contract contract, string serializedContract)
        {
            var deserialized = Deserialize(serializedContract);
            if(deserialized == null)
            {
                return;
            }
            contract.TimeStamp = deserialized.TimeStamp;

            var restoreNotes = false;
            if(contract.Comments.Notes == null)
            {
                restoreNotes = deserialized.AggregateNotes == null;
            }
            else
            {
                var firstComment = contract.Comments.Notes.FirstOrDefault();
                if(firstComment == null)
                {
                    restoreNotes = deserialized.AggregateNotes == null;
                }
                else
                {
                    restoreNotes = firstComment.Text == deserialized.AggregateNotes;
                }
            }

            if(restoreNotes)
            {
                contract.Comments.Notes = deserialized.Notes.ToNotes(contract);
            }
        }

        private SerializableContract(Contract contract)
        {
            TimeStamp = contract.TimeStamp;
            AggregateNotes = contract.Comments.Notes.AggregateNotes();
            Notes = contract.Comments.Notes.Select(n => new SerializableNote(n)).ToList();
        }

        public DateTime TimeStamp;
        public string AggregateNotes;
        public List<SerializableNote> Notes;
    }
}