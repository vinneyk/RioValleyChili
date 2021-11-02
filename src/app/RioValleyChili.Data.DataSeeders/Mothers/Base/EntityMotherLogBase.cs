using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using RioValleyChili.Data.DataSeeders.Utilities;

namespace RioValleyChili.Data.DataSeeders.Mothers.Base
{
    public abstract class EntityMotherLogBase<TEntityResult, TCallbackParameters, TEntityType> : EntityMotherLogBase<TEntityResult, TCallbackParameters>
        where TEntityResult : class
        where TCallbackParameters : CallbackParametersBase, new()
        where TEntityType : struct
    {
        protected EntityMotherLogBase(ObjectContext oldContext, Action<TCallbackParameters> loggingCallback) : base(oldContext, loggingCallback) { }

        protected readonly MotherLoadCount<TEntityType> LoadCount = new MotherLoadCount<TEntityType>();

        protected override void BeginLoad()
        {
            LoadCount.Reset();
        }

        protected override void EndLoad()
        {
            LoadCount.LogResults(l =>
                {
                    var logParameters = new TCallbackParameters();
                    logParameters.Initialize(l);
                    Log(logParameters);
                });
        }
    }

    public abstract class EntityMotherLogBase<TEntityResult, TCallbackParameters> : IMother<TEntityResult>, IProcessedMother<TEntityResult>
        where TEntityResult : class
        where TCallbackParameters : CallbackParametersBase, new()
    {
        protected readonly ObjectContext OldContext;
        private readonly Action<TCallbackParameters> _loggingCallback;

        protected EntityMotherLogBase(ObjectContext oldContext, Action<TCallbackParameters> loggingCallback)
        {
            if(oldContext == null) { throw new ArgumentNullException("oldContext"); }
            OldContext = oldContext;
            _loggingCallback = loggingCallback;
        }

        protected virtual void BeginLoad() { }
        protected virtual void EndLoad() { }
        
        public IEnumerable<TEntityResult> BirthAll(Action consoleCallback = null)
        {
            var results = new List<TEntityResult>();
            ProcessedBirthAll(results.Add);
            return results;
        }

        public void ProcessedBirthAll(Action<TEntityResult> processResult, Action consoleCallback = null)
        {
            if(consoleCallback != null)
            {
                consoleCallback();
            }

            try
            {
                BeginLoad();
                foreach(var record in BirthRecords())
                {
                    if(consoleCallback != null)
                    {
                        consoleCallback();
                    }

                    if(record != null)
                    {
                        LogTruncatedStrings(record);
                        processResult(record);
                    }
                }
                EndLoad();
            }
            catch(Exception ex)
            {
                var exceptionParameters = new TCallbackParameters();
                exceptionParameters.Initialize(ex);
                if(!Log(exceptionParameters))
                {
                    throw;
                }
            }

            Log(null);
        }

        public bool Log(TCallbackParameters callbackParameters)
        {
            if(callbackParameters != null)
            {
                if(!callbackParameters.DataLoadSuccess)
                {
                    DataLoadResult.Success = false;
                }
            }

            if(_loggingCallback != null)
            {
                _loggingCallback(callbackParameters);
                return true;
            }

            return false;
        }

        protected abstract IEnumerable<TEntityResult> BirthRecords();

        private void LogTruncatedStrings(TEntityResult result)
        {
            DataStringPropertyHelper.GetTruncatedStringsInGraph(result).ForEach(r =>
                {
                    var stringTruncateParameters = new TCallbackParameters();
                    stringTruncateParameters.Initialize(r);
                    Log(stringTruncateParameters);
                });
        }
    }
}