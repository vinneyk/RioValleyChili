using System;
using System.Globalization;
using System.Linq.Expressions;
using Solutionhead.EntityKey;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Keys
{
    public class WorkTypeKey : EntityKeyBase.Of<IWorkTypeKey>, IKey<WorkType>, IWorkTypeKey
    {
        #region fields and constructors

        private readonly int _workTypeId;

        public WorkTypeKey() { }

        private WorkTypeKey(int workTypeId)
        {
            _workTypeId = workTypeId;
        }

        public WorkTypeKey(IWorkTypeKey workTypeKey)
            : this(workTypeKey.WorkTypeKey_WorkTypeId) { }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IWorkTypeKey>

        protected override IWorkTypeKey ParseImplementation(string keyValue)
        {
            return new WorkTypeKey(int.Parse(keyValue));
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return UserMessages.InvalidWorkTypeKey;
        }

        public override IWorkTypeKey Default
        {
            get { return Null; }
        }

        protected override IWorkTypeKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IWorkTypeKey key)
        {
            return key.WorkTypeKey_WorkTypeId.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Implementation of IKey<WorkType>

        public Expression<Func<WorkType, bool>> FindByPredicate
        {
            get { return (w => w.Id == _workTypeId); }
        }

        #endregion

        #region Implementation of IWorkTypeKey

        public int WorkTypeKey_WorkTypeId
        {
            get { return _workTypeId; }
        }

        #endregion

        #region Equality Overrides
        
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(obj, this)) return true;
            return obj as IWorkTypeKey != null && Equals(obj as IWorkTypeKey);
        }

        protected bool Equals(IWorkTypeKey other)
        {
            return _workTypeId == other.WorkTypeKey_WorkTypeId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _workTypeId;
            }
        }

        #endregion

        public static IWorkTypeKey Null = new NullWorkTypeKey();

        private class NullWorkTypeKey : IWorkTypeKey
        {
            #region Implementation of IWorkTypeKey

            public int WorkTypeKey_WorkTypeId { get { return 0; } }

            #endregion
        }
    }
}