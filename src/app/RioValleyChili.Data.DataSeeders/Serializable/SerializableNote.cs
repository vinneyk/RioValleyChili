using System;
using Newtonsoft.Json;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Serializable
{
    [JsonObject(MemberSerialization.Fields)]
    public class SerializableNote
    {
        public static string Serialize(Note note)
        {
            return JsonConvert.SerializeObject(new SerializableNote(note), Formatting.None);
        }

        public static Note Deserialize(string serializedNote)
        {
            if(string.IsNullOrWhiteSpace(serializedNote))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<SerializableNote>(serializedNote).ToNote();
        }

        public SerializableNote(Note note)
        {
            EmployeeKey = new EmployeeKey(note);
            TimeStamp = note.TimeStamp;
            Text = note.Text;
        }

        public Note ToNote(INotebookKey notebookKey = null, int sequence = 0)
        {
            var note = new Note
                {
                    EmployeeId = new EmployeeKey().Parse(EmployeeKey).EmployeeKey_Id,
                    TimeStamp = TimeStamp,
                    Text = Text
                };

            if(notebookKey != null)
            {
                note.NotebookDate = notebookKey.NotebookKey_Date;
                note.NotebookSequence = notebookKey.NotebookKey_Sequence;
            }

            note.Sequence = sequence;

            return note;
        }

        public string EmployeeKey;
        public DateTime TimeStamp;
        public string Text;
    }
}