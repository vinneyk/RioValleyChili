using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RioValleyChili.Data.DataSeeders.Mothers.Base;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class LogSummary
    {
        private readonly Dictionary<CallbackParametersBase.ReasonCategory, Logs> _categorizedLogs;
        private readonly Dictionary<string, List<string>> _summariesByLog = new Dictionary<string, List<string>>();

        public LogSummary()
        {
            _categorizedLogs = Enum.GetValues(typeof(CallbackParametersBase.ReasonCategory)).Cast<CallbackParametersBase.ReasonCategory>().ToDictionary(v => v, v => new Logs());
        }

        public void AddEntry(CallbackParametersBase.ReasonCategory category, string logName, string logEntry)
        {
            if(category == CallbackParametersBase.ReasonCategory.Summary)
            {
                List<string> logSummaries;
                if(!_summariesByLog.TryGetValue(logName, out logSummaries))
                {
                    _summariesByLog.Add(logName, logSummaries = new List<string>());
                }
                logSummaries.Add(logEntry);
            }
            else
            {
                _categorizedLogs[category].AddEntry(logName, logEntry);
            }
        }

        public void AddEntry<T>(string logName, CallbackParametersBase<T> parameters) where T : struct
        {
            if(parameters.CallbackReasonCategory == CallbackParametersBase.ReasonCategory.Summary)
            {
                List<string> logSummaries;
                if(!_summariesByLog.TryGetValue(logName, out logSummaries))
                {
                    _summariesByLog.Add(logName, logSummaries = new List<string>());
                }
                logSummaries.Add(parameters.SummaryMessage);
            }
            else
            {
                _categorizedLogs[parameters.CallbackReasonCategory].AddEntry(logName, parameters.CallbackReason.ToString());
            }
        }

        public void WriteLogSummary(TextWriter writer)
        {
            writer.WriteLine("Log Summaries:");
            foreach(var summaries in _summariesByLog)
            {
                writer.WriteLine("\tLog [{0}]", summaries.Key);
                summaries.Value.ForEach(s => writer.WriteLine("\t\t{0}", s));
                writer.WriteLine("");
            }
            foreach(var logs in _categorizedLogs.Where(c => c.Key != CallbackParametersBase.ReasonCategory.Summary))
            {
                writer.WriteLine("------------------------------");
                writer.WriteLine("Category [{0}]: {1} total entries.", logs.Key, logs.Value.TotalLogEntries);
                logs.Value.WriteLogs(writer, 1);
                writer.WriteLine("");
            }
        }

        private class Logs
        {
            private readonly Dictionary<string, LogEntries> _logs = new Dictionary<string, LogEntries>();

            private const string IndentString = "\t";

            public int TotalLogEntries
            {
                get { return _logs.Any() ? _logs.Sum(l => l.Value.TotalEntries) : 0; }
            }

            public void AddEntry(string logName, string logEntry)
            {
                LogEntries logEntries;
                if(!_logs.TryGetValue(logName, out logEntries))
                {
                    _logs.Add(logName, logEntries = new LogEntries());
                }
                logEntries.AddEntry(logEntry);
            }

            public void WriteLogs(TextWriter writer, uint indentLevel = 0)
            {
                var indentString = "";
                for(var i = 0; i < indentLevel; ++i)
                {
                    indentString += IndentString;
                }

                foreach(var log in _logs)
                {
                    writer.WriteLine("{0}Log [{1}]: {2} total entries.", indentString, log.Key, log.Value.TotalEntries);
                    log.Value.WriteEntries(writer, indentLevel + 1);
                    writer.WriteLine("");
                }
            }
        }

        private class LogEntries
        {
            private readonly Dictionary<string, int> _entries = new Dictionary<string, int>();

            private const string IndentString = "\t";

            public int TotalEntries 
            {
                get { return _entries.Any() ? _entries.Sum(e => e.Value) : 0; }
            }

            public void AddEntry(string logEntry)
            {
                int count;
                if(!_entries.TryGetValue(logEntry, out count))
                {
                    _entries.Add(logEntry, count = 0);
                }
                _entries[logEntry] = count + 1;
            }

            public void WriteEntries(TextWriter writer, uint indentLevel = 0)
            {
                var indentString = "";
                for(var i = 0; i < indentLevel; ++i)
                {
                    indentString += IndentString;
                }

                foreach(var entry in _entries)
                {
                    writer.WriteLine("{0}[{1}]: {2} entries logged.", indentString, entry.Key, entry.Value);
                }
            }
        }
    }
}