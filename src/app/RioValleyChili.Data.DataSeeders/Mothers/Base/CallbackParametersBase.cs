using System;
using RioValleyChili.Data.DataSeeders.Utilities;

namespace RioValleyChili.Data.DataSeeders.Mothers.Base
{
    public abstract class CallbackParametersBase<TCallbackReason> : CallbackParametersBase
        where TCallbackReason : struct
    {
        public Exception Exception { get; set; }
        public string SummaryMessage { get; private set; }
        public DataStringPropertyHelper.Result StringResult { get; private set; }

        public string StringResultMessage
        {
            get { return StringResult == null ? "NO STRING RESULT MESSAGE" : string.Format("{0} [{1}] truncated to length {2}.", StringResult.PropertyName, StringResult.TruncatedString, StringResult.TruncatedLength); }
        }

        public TCallbackReason CallbackReason { get; private set; }

        public override ReasonCategory CallbackReasonCategory { get { return GetReasonCategory(CallbackReason); } }

        private ReasonCategory GetReasonCategory(TCallbackReason reason)
        {
            return SummaryMessage == null ? DerivedGetReasonCategory(reason) : ReasonCategory.Summary;
        }

        protected virtual ReasonCategory DerivedGetReasonCategory(TCallbackReason reason)
        {
            return reason.Equals(ExceptionReason) ? ReasonCategory.Error : ReasonCategory.Informational;
        }

        protected abstract TCallbackReason ExceptionReason { get; }
        protected abstract TCallbackReason SummaryReason { get; }
        protected abstract TCallbackReason StringResultReason { get; }

        protected CallbackParametersBase() { }

        protected CallbackParametersBase(TCallbackReason callbackReason)
        {
            CallbackReason = callbackReason;
        }

        protected CallbackParametersBase(DataStringPropertyHelper.Result result)
        {
            Initialize(result);   
        }

        protected CallbackParametersBase(string summaryMessage)
        {
            Initialize(summaryMessage);
        }

        public override void Initialize(Exception ex)
        {
            CallbackReason = ExceptionReason;
            Exception = ex;
        }

        public override void Initialize(string summaryMessage)
        {
            CallbackReason = SummaryReason;
            SummaryMessage = summaryMessage;
        }

        public override void Initialize(DataStringPropertyHelper.Result stringResult)
        {
            CallbackReason = StringResultReason;
            StringResult = stringResult;
        }
    }

    public abstract class CallbackParametersBase
    {
        public enum ReasonCategory
        {
            Error,
            Summary,
            RecordSkipped,
            Informational
        }

        public virtual ReasonCategory CallbackReasonCategory { get { return ReasonCategory.Informational; } }

        public virtual bool DataLoadSuccess { get { return CallbackReasonCategory != ReasonCategory.Error; } }

        public abstract void Initialize(Exception ex);
        public abstract void Initialize(string summaryMessage);
        public abstract void Initialize(DataStringPropertyHelper.Result stringResult);
    }
}