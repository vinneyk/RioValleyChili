using System.Collections;
using System.Collections.Generic;
using RioValleyChili.Client.Mvc.SolutionheadLibs.Core;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi
{
    [ExtractIntoSolutionheadLibrary]
    public class ResourceLinkCollection : IDictionary<string, Link>
    {
        private readonly IDictionary<string, Link> _resources;
        private const string SelfRelName = "self";

        public ResourceLinkCollection()
        {
            _resources = new Dictionary<string, Link>();
        }

        public ResourceLinkCollection(IEnumerable<KeyValuePair<string, Link>> initialItems)
        {
            _resources = initialItems.ToDictionary();
        }

        public ResourceLinkCollection(IDictionary<string, Link> initialItems)
        {
            _resources = initialItems;
        }

        public string SelfHRef 
        {
            get { return _resources.ContainsKey(SelfRelName) ? _resources[SelfRelName].HRef : null; }
        }

        public IEnumerator<KeyValuePair<string, Link>> GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, Link> item)
        {
            _resources.Add(item);
        }

        public void Clear()
        {
            _resources.Clear();
        }

        public bool Contains(KeyValuePair<string, Link> item)
        {
            return _resources.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, Link>[] array, int arrayIndex)
        {
            _resources.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, Link> item)
        {
            return _resources.Remove(item);
        }

        public int Count { get { return _resources.Count; } }
        public bool IsReadOnly { get { return _resources.IsReadOnly; } }
        public bool ContainsKey(string key)
        {
            return _resources.ContainsKey(key);
        }

        public void Add(string key, Link value)
        {
            _resources.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _resources.Remove(key);
        }

        public bool TryGetValue(string key, out Link value)
        {
            return _resources.TryGetValue(key, out value);
        }

        public Link this[string key]
        {
            get { return _resources[key]; }
            set { _resources[key] = value; }
        }

        public ICollection<string> Keys { get { return _resources.Keys; }}
        public ICollection<Link> Values { get { return _resources.Values; } }
    }
}