using System.Collections;
using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi
{
    [ExtractIntoSolutionheadLibrary]
    public class LinkedResoureCollection<TData> : ILinkedResource<IEnumerable<TData>>, ICollection<TData>
        where TData : ILinkedResource<TData>
    {
        public LinkedResoureCollection() : this (new List<TData>()) { } 

        public LinkedResoureCollection(IEnumerable<TData> items)
        {
            _resources = items as IList<TData> ?? new List<TData>(items);
        }

        private readonly IList<TData> _resources;

        #region ILinkedResource<T> implementation

        public ResourceLinkCollection Links { get; set; }

        public string HRef { get { return Links.SelfHRef; } }

        public IEnumerable<TData> Data { get { return _resources; } }

        #endregion

        #region ICollection<T>, IEnumerator<T>, IEnumerator implementations

        public IEnumerator<TData> GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TData item)
        {
            _resources.Add(item);
        }

        public void Clear()
        {
            _resources.Clear();
        }

        public bool Contains(TData item)
        {
            return _resources.Contains(item);
        }

        public void CopyTo(TData[] array, int arrayIndex)
        {
            _resources.CopyTo(array, arrayIndex);
        }

        public bool Remove(TData item)
        {
            return _resources.Remove(item);
        }

        public int Count
        {
            get { return _resources.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

    }
}