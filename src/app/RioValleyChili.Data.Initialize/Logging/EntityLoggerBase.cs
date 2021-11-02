using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RioValleyChili.Data.DataSeeders.Logging;
using RioValleyChili.Data.DataSeeders.Mothers.Base;

namespace RioValleyChili.Data.Initialize.Logging
{
    public abstract class EntityLoggerBase<TCallbackParameters, TLogReason> : EntityLoggerBase, ILoggerCallback<TCallbackParameters>
        where TCallbackParameters : CallbackParametersBase<TLogReason>
        where TLogReason : struct
    {
        public readonly DataLoadLog<TLogReason> DataLoadLog = new DataLoadLog<TLogReason>();

        /// <summary>
        /// Delegate accepting callback parameteres to log. Pass in null to end and write log.
        /// </summary>
        public Action<TCallbackParameters> Callback { get { return Log; } }

        protected virtual string LogSummaryName { get { return GetType().ToString().Split('.').Last(); } }

        protected EntityLoggerBase(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        protected abstract string GetLogMessage(TCallbackParameters parameters);

        protected static string GetExceptionString(Exception exception)
        {
            return JsonConvert.SerializeObject(exception);
        }

        #region Private Parts

        private readonly string _logFilePath;

        private void LogEntry(TCallbackParameters parameters)
        {
            if(parameters.Exception != null)
            {
                DataLoadLog.Log(parameters, GetExceptionString(parameters.Exception));
            }
            else if(parameters.SummaryMessage != null)
            {
                DataLoadLog.Log(parameters, parameters.SummaryMessage);
            }
            else if(parameters.StringResult != null)
            {
                DataLoadLog.Log(parameters, parameters.StringResultMessage);
            }
            else
            {
                var logMessage = GetLogMessage(parameters);
                if(logMessage == null)
                {
                    throw new ArgumentOutOfRangeException("parameters.CallbackReason");
                }

                DataLoadLog.Log(parameters, logMessage);
            }
        }

        private void Log(TCallbackParameters parameters)
        {
            if(parameters == null)
            {
                EndLog();
            }
            else
            {
                LogEntry(parameters);
                LogSummary.AddEntry(LogSummaryName, parameters);
            }
        }

        private void EndLog()
        {
            using(var writer = new StreamWriter(_logFilePath))
            {
                DataLoadLog.WriteLog(writer);
            }
        }

        #endregion
    }

    public abstract class EntityLoggerBase
    {
        protected static LogSummary LogSummary = new LogSummary();

        public static void LogSummaryMessage(string logName, string summaryMessage)
        {
            LogSummary.AddEntry(CallbackParametersBase.ReasonCategory.Summary, logName, summaryMessage);
        }

        public static void WriteLogSummary(string logFilePath)
        {
            using(var writer = new StreamWriter(logFilePath))
            {
                LogSummary.WriteLogSummary(writer);
            }

            LogSummary = new LogSummary();
        }

        public static void WriteException(Exception exception, string logFilePath)
        {
            using(var writer = new StreamWriter(logFilePath))
            {
                writer.Write(JsonConvert.SerializeObject(exception, Formatting.Indented));
            }
        }
    }
}