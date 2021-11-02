using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class MotherLoadCount<T> where T : struct
    {
        private readonly Dictionary<T, EntityLoadCount> _loadCounts = Enum.GetValues(typeof(T)).Cast<T>().ToDictionary<T, T, EntityLoadCount>(v => v, v => new EntityLoadCount(v.ToString()));

        public void Reset()
        {
            _loadCounts.ForEach(l => l.Value.Reset());
        }

        public void AddRead(T entity, uint count = 1)
        {
            _loadCounts[entity].AddRead(count);
        }

        public void AddLoaded(T entity, uint count = 1)
        {
            _loadCounts[entity].AddLoaded(count);
        }

        public void LogResults(Action<string> log)
        {
            if(log != null)
            {
                _loadCounts.ForEach(l => log(l.Value));
            }
        }

        private class EntityLoadCount
        {
            private readonly string _entityName;
            private uint _loaded = 0;
            private uint _read = 0;

            public EntityLoadCount(string entityName)
            {
                _entityName = entityName;
            }

            public void Reset()
            {
                _loaded = _read = 0;
            }

            public void AddRead(uint count = 1)
            {
                _read += count;
            }

            public void AddLoaded(uint count = 1)
            {
                _loaded += count;
            }

            public string GetResults()
            {
                var percent = _read > 0 ? string.Format(" = {0:0.00}%", (((float)_loaded * 100.0f) / ((float)_read))) : "";
                return string.Format("{0} records loaded / read: [{1} / {2}]{3}", _entityName, _loaded, _read, percent);
            }

            public override string ToString()
            {
                return GetResults();
            }

            public static implicit operator string(EntityLoadCount loadCount)
            {
                return loadCount == null ? null : loadCount.GetResults();
            }
        }
    }
}