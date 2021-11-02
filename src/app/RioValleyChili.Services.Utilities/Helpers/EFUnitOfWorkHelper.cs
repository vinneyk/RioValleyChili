using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Solutionhead.EntityKey;
using RioValleyChili.Core;
using RioValleyChili.Data;
using Solutionhead.Data;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Helpers
{
    internal class EFUnitOfWorkHelper
    {
        private readonly EFUnitOfWorkBase _unitOfWork;

        internal EFUnitOfWorkHelper(IUnitOfWork unitOfWork)
        {
            if(unitOfWork == null) { throw new ArgumentNullException("unitOfWork"); }

            _unitOfWork = unitOfWork as EFUnitOfWorkBase;
            if(_unitOfWork == null) { throw new ArgumentException("unitOfWork needs to derive from EFUnitOfWorkBase."); }
        }

        [ExtractIntoSolutionheadLibrary("IUnitOfWork")]
        internal IResult EntityHasNoPendingChanges<TKeyInterface, TObject>(EntityKeyBase.Of<TKeyInterface> key, IKey<TObject> keyPredicate)
            where TKeyInterface : class
            where TObject : class
        {
            if(key == null) { throw new ArgumentNullException("key"); }

            TObject entity;
            EntityState? state;
            if(TryGetObjectState(keyPredicate, out state, out entity))
            {
                switch(state)
                {
                    case EntityState.Deleted:
                        return new InvalidResult(string.Format("{0} item with key '{1}' is pending deletion.", typeof(TObject).Name, key.KeyValue));
                    case EntityState.Added:
                        return new InvalidResult(string.Format("{0} item with key '{1}' is pending addition.", typeof(TObject).Name, key.KeyValue));
                    case EntityState.Modified:
                        return new InvalidResult(string.Format("{0} item with key '{1}' is pending modification.", typeof(TObject).Name, key.KeyValue));
                }
            }

            return new SuccessResult();
        }

        [ExtractIntoSolutionheadLibrary("IUnitOfWork")]
        internal IResult EntityHasNoPendingChanges<TKeyInterface, TObject>(EntityKeyBase.Of<TKeyInterface> key, IKey<TObject> keyPredicate, out TObject entity, out EntityState? state)
            where TKeyInterface : class
            where TObject : class
        {
            if(key == null) { throw new ArgumentNullException("key"); }
            
            if(TryGetObjectState(keyPredicate, out state, out entity))
            {
                switch(state)
                {
                    case EntityState.Deleted:
                        return new InvalidResult(string.Format("{0} item with key '{1}' is pending deletion.", typeof(TObject).Name, key.KeyValue));
                    case EntityState.Added:
                        return new InvalidResult(string.Format("{0} item with key '{1}' is pending addition.", typeof(TObject).Name, key.KeyValue));
                    case EntityState.Modified:
                        return new InvalidResult(string.Format("{0} item with key '{1}' is pending modification.", typeof(TObject).Name, key.KeyValue));
                }
            }

            return new SuccessResult();
        }

        [ExtractIntoSolutionheadLibrary("IUnitOfWork")]
        internal List<TObject> GetLocalData<TObject>() where TObject : class
        {
            return _unitOfWork.GetLocalData<TObject>().ToList();
        }

        internal int GetNextSequence<TEntity>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, int>> sequenceSelector) where TEntity : class
        {
            var existingData = _unitOfWork.GetExistingData<TEntity>().Where(predicate);
            var existingSequence = existingData.Select(sequenceSelector).DefaultIfEmpty(0).Max();

            var pendingData = _unitOfWork.GetLocalData<TEntity>().Where(predicate.Compile()).ToList();
            var pendingSequence = pendingData.Select(sequenceSelector.Compile()).DefaultIfEmpty(0).Max();

            return Math.Max(existingSequence, pendingSequence) + 1;
        }

        private bool TryGetObjectState<TObject>(IKey<TObject> key, out EntityState? state, out TObject entity) where TObject : class
        {
            return _unitOfWork.TryGetObjectState(key, out state, out entity);
        }
    }
}