using System;
using System.Collections.Generic;
using System.Linq;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class SerializedData
    {
        public string this[SerializableType type, string key]
        {
            get
            {
                if(string.IsNullOrWhiteSpace(key)) { throw new Exception("key cannot be null or whitespace"); }

                var serialized = GetSerialized(type, key);
                return serialized != null ? serialized.Data : null;
            }

            set
            {
                if(string.IsNullOrWhiteSpace(key)) { throw new Exception("key cannot be null or whitespace"); }

                var serialized = GetSerialized(type, key);
                if(serialized == null)
                {
                    var serializedData = GetSerialized(type);
                    serialized = new Serialized
                        {
                            Type = (int) type,
                            OldKey = key
                        };
                    _context.Serialized.AddObject(serialized);
                    serializedData.Add(key, serialized);
                }

                serialized.Data = value;
            }
        }

        public SerializedData(RioAccessSQLEntities context)
        {
            if(context == null) { throw new ArgumentNullException("context"); }
            _context = context;
        }

        public TDeserialized GetDeserialized<TDeserialized>(SerializableType type, string key)
            where TDeserialized : Serializable.Serializable
        {
            return Serializable.Serializable.Deserialize<TDeserialized>(this[type, key]);
        }

        public Serialized GetSerialized(SerializableType type, string key)
        {
            if(string.IsNullOrWhiteSpace(key)) { throw new Exception("key cannot be null or whitespace"); }

            Serialized data;
            return GetSerialized(type).TryGetValue(key, out data) ? data : null;
        }

        public void Remove(string key, params SerializableType[] types)
        {
            if(string.IsNullOrWhiteSpace(key)) { throw new Exception("key cannot be null or whitespace"); }
            if(types == null) { throw new ArgumentNullException("types"); }

            foreach(var serializedData in types.Select(GetSerialized))
            {
                Serialized serialized;
                if(serializedData.TryGetValue(key, out serialized))
                {
                    serializedData.Remove(key);
                    _context.Serialized.DeleteObject(serialized);
                }
            }
        }

        private readonly RioAccessSQLEntities _context;
        private readonly Dictionary<SerializableType, Dictionary<string, Serialized>> _table = new Dictionary<SerializableType, Dictionary<string, Serialized>>();

        private Dictionary<string, Serialized> GetSerialized(SerializableType type)
        {
            Dictionary<string, Serialized> data;
            if(!_table.TryGetValue(type, out data))
            {
                _table.Add(type, data = _context.Serialized.Where(s => s.Type == (decimal)type).ToDictionary(s => s.OldKey, s => s));
            }
            return data;
        }
    }
}