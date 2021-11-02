using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RioValleyChili.Data.DataSeeders.Mothers.Base;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class DataLoadLog<T> where T : struct
    {
        public List<string> this[CallbackParametersBase.ReasonCategory category, T reason]
        {
            get
            {
                Entries<T> categorizedEntries;
                if(!_categorizedLogs.TryGetValue(category, out categorizedEntries))
                {
                    categorizedEntries = new Entries<T>();
                    _categorizedLogs.Add(category, categorizedEntries);
                }

                return categorizedEntries[reason];
            }
        }

        public Entries<T> this[CallbackParametersBase.ReasonCategory category]
        {
            get
            {
                Entries<T> categories;
                if(!_categorizedLogs.TryGetValue(category, out categories))
                {
                    categories = new Entries<T>();
                    _categorizedLogs.Add(category, categories);
                }

                return categories;
            }
        }

        public void ClearLog()
        {
            _categorizedLogs.Clear();
        }

        public void WriteLog(StreamWriter writer, bool distinct = true)
        {
            var summaryLogs = _categorizedLogs.Where(l => l.Key == CallbackParametersBase.ReasonCategory.Summary).SelectMany(l => l.Value).ToList();
            var detailLogs = _categorizedLogs.Where(l => l.Key != CallbackParametersBase.ReasonCategory.Summary && l.Value.Any()).ToList();

            writer.WriteLine("------------------------------");
            writer.WriteLine("Log Summary:");
            summaryLogs.ForEach(s => s.ForEach(l => writer.WriteLine("\t{0}", l)));
            detailLogs.ForEach(l =>
                {
                    writer.WriteLine("");
                    writer.WriteLine("[{0}]: {1} entries logged.", l.Key, l.Value.SelectMany(v => v).Count());
                    foreach(var logEntries in l.Value.LogEntries.Where(e => e.Value.Any()))
                    {
                        writer.WriteLine("\t[{0}]: {1} entries logged.", logEntries.Key, logEntries.Value.Count);
                    }
                });
            writer.WriteLine("------------------------------");

            writer.WriteLine("");

            detailLogs.ForEach(l =>
                {
                    writer.WriteLine("[{0}]: {1} entries logged.", l.Key, l.Value.SelectMany(v => v).Count());
                    foreach(var logEntries in l.Value.LogEntries.Where(e => e.Value.Any()))
                    {
                        writer.WriteLine("\t[{0}]: {1} entries logged.", logEntries.Key, logEntries.Value.Count);
                        logEntries.Value.ForEach(writer.WriteLine);
                        writer.WriteLine("");
                    }
                    writer.WriteLine("");
                });
        }

        public void Log(CallbackParametersBase<T> parameters, string message)
        {
            this[parameters.CallbackReasonCategory, parameters.CallbackReason].Add(message);
        }

        private readonly Dictionary<CallbackParametersBase.ReasonCategory, Entries<T>> _categorizedLogs = new Dictionary<CallbackParametersBase.ReasonCategory, Entries<T>>();

        public class Entries<T> : IEnumerable<List<string>>
            where T : struct
        {
            public List<string> this[T reason]
            {
                get
                {
                    List<string> logEntries;
                    if(!LogEntries.TryGetValue(reason, out logEntries))
                    {
                        logEntries = new List<string>();
                        LogEntries.Add(reason, logEntries);
                    }
                    return logEntries;
                }
            }

            public readonly Dictionary<T, List<string>> LogEntries = new Dictionary<T, List<string>>();

            public IEnumerator<List<string>> GetEnumerator()
            {
                return LogEntries.Select(l => l.Value).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}