using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using RioValleyChili.Data.Helpers;

namespace RioValleyChili.Data.Initialization
{
    public abstract class DbContextDataSeederBase<TContext> : IDataContextSeeder<TContext>
        where TContext : DbContext, new()
    {
        public abstract void SeedContext(TContext newContext);

        protected static IList<TEntity> Birth<TEntity>(IMother<TEntity> mother, string message, ConsoleTicker consoleTicker, int? takeLimit = null)
            where TEntity : class
        {
            var tickedMessage = message == null || consoleTicker == null ? null : message + "...";
            var entities = mother.BirthAll(tickedMessage == null ? (Action)null : () => consoleTicker.TickConsole(tickedMessage));
            return takeLimit != null ? entities.Take(takeLimit.Value).ToList() : entities.ToList();
        }

        protected static void ProcessedBirth<TEntity>(IProcessedMother<TEntity> mother, string message, ConsoleTicker consoleTicker, Action<TEntity> processResult)
            where TEntity : class
        {
            var tickedMessage = message == null || consoleTicker == null ? null : message + "...";
            mother.ProcessedBirthAll(processResult, tickedMessage == null ? (Action) null : () => consoleTicker.TickConsole(tickedMessage));
        }

        protected static void LoadRecords<T>(IBulkInsertContext context, IMother<T> mother, string message, ConsoleTicker consoleTicker, int? takeLimit = null)
            where T : class
        {
            var records = Birth(mother, message, consoleTicker, takeLimit);
            if(message != null && consoleTicker != null)
            {
                Console.Write("\r");
            }
            LoadRecords(context, records, message, consoleTicker, takeLimit);
        }

        protected static void LoadRecords<T>(IBulkInsertContext context, IEnumerable<T> records, string message, ConsoleTicker consoleTicker, int? takeLimit = null)
            where T : class
        {
            if(message != null)
            {
                Console.Write(message + "...\r");
            }

            var recordsList = takeLimit != null ? records.Take(takeLimit.Value).ToList() : records.ToList();
            consoleTicker.ResetTicker();
            var count = context.BulkAddAll(recordsList, message != null ? () => consoleTicker.TickConsole(message + "...") : (Action) null);
            consoleTicker.ReplaceCurrentLine("{0}: {1} records.", message, count);
        }

        protected static void LoadRecords<T>(IBulkInsertContext context, List<T> records, string message, ConsoleTicker consoleTicker)
            where T : class
        {
            if(message != null)
            {
                Console.Write(message + "...\r");
            }
            
            consoleTicker.ResetTicker();
            var count = context.BulkAddAll(records, message != null ? () => consoleTicker.TickConsole(message + "...") : (Action)null);
            consoleTicker.ReplaceCurrentLine("{0}: {1} records.", message, count);
        }

        protected virtual void ResetContext(ref TContext context)
        {
            context.Dispose();
            Database.SetInitializer<TContext>(null);
            context = new TContext();
        }

        private static Stopwatch _stopwatch;
        protected static void StartWatch()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        protected static TimeSpan StopWatch()
        {
            if(_stopwatch == null)
            {
                return new TimeSpan();
            }
            _stopwatch.Stop();
            return _stopwatch.Elapsed;
        }
    }
}