using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Logging;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Initialization;
using RioValleyChili.Data.Initialize.Logging;

namespace RioValleyChili.Data.Initialize
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.ExitCode = -1;

            try
            {
                using(var newContext = ContextsHelper.GetNewContext())
                {
                    if(args != null && args.Length > 0 && args[0].Length == 1)
                    {
                        ExecuteSilently(newContext, args);
                    }
                    else
                    {
                        ExecuteNormal(newContext, args);
                    }
                }

                Environment.ExitCode = 0;
            }
            catch(Exception ex)
            {
                if(RVCDataLoadLoggerGate.RVCDataLoadLogger != null)
                {
                    RVCDataLoadLoggerGate.RVCDataLoadLogger.WriteException(ex);
                }

                Console.Write("\n");
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Console.WriteLine("The program encountered an exception. \n\nError message: \n{0}", ex.Message);

                var innermostException = FindInnermostException(ex);
                if(innermostException != null)
                {
                    Console.WriteLine("Innermost exception message:\n{0}", innermostException.Message);
                }

                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Console.Write("\n");
                Console.Write("\n");

                new RVCDataLoadResultObtainer().SetDataLoadResult(new RVCDataLoadResultObtainer.LoadResult
                    {
                        Success = DataLoadResult.Success = false,
                        RanToCompletion = false,
                        TimeStamp = DateTime.Now
                    });

                Console.Read();
            }
        }

        private static void DisplayStartup()
        {
            Console.WriteLine("");
            Console.WriteLine(@"  \\\\\\\\\  \\  \\\\\\\\");
            Console.WriteLine(@"  \\     //   \\  \     \\");
            Console.WriteLine(@"  \\    //     \\  \     \\");
            Console.WriteLine(@"  \\   //       \\  \\\\\\\\");
            Console.WriteLine(@"  \\   \\");
            Console.WriteLine(@"  \\    \\    \\     \\  \\\    \       \       \\\\\   \      //");
            Console.WriteLine(@"  \\     \\    \\    \\  \  \\   \       \       \       \    //");
            Console.WriteLine(@"                \\   \\  \\\\\\   \       \       \\\\    \  //");
            Console.WriteLine(@"                 \\  \\  \     \\  \\\\\\  \\\\\\  \\\\\\  \//");
            Console.WriteLine(@"                  \\ \\                                    //");
            Console.WriteLine(@"                   \\\\                                   //");
            Console.WriteLine(@"                    \\\                                /////");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("RIO VALLEY CHILI INC.");
            Console.WriteLine("DATABASE LOAD PROGRAM");
            Console.WriteLine("****************************************\n");
        }

        private static void ExecuteSilently(RioValleyChiliDataContext context, string[] args)
        {
            var initializationAdapter = GetDataContextInitializationAdapter(args[0][0]);
            ExecuteInitialization(context, initializationAdapter, args);
        }

        private static void ExecuteNormal(RioValleyChiliDataContext context, string[] args)
        {
            DisplayStartup();
            while(true)
            {
                DisplaySeedingOptions();

                var keyInfo = Console.ReadKey();
                Console.WriteLine("\n...................................................\n");

                if (keyInfo.Key == ConsoleKey.Q || keyInfo.Key == ConsoleKey.Enter) { break; }

                var initializationAdapter = GetDataContextInitializationAdapter(keyInfo.KeyChar);

                ExecuteInitialization(context, initializationAdapter, args);

                DisplayLogFileMenu();
            }

            Console.Write("\n\n** Godspeed You! Black Emperor **");
            Console.Read();
        }

        private static void DisplaySeedingOptions()
        {
                Console.WriteLine("DATA SEEDING OPTIONS:");
                Console.WriteLine("0 - [N]o seed, just build data structure.");
                Console.WriteLine("1 - Seed [C]ore data.");
                Console.WriteLine("2 - Seed Co[M]pany data.");
                Console.WriteLine("3 - Seed [P]roduct data.");
                Console.WriteLine("4 - Seed [I]nventory.");
                Console.WriteLine("5 - Seed P[R]oduction data.");
                Console.WriteLine("6 - Seed [O]rder data.");
                Console.WriteLine("7 - Seed Inventory [T]ransactions data.");
                Console.WriteLine("* - Seed ALL data.");
                Console.WriteLine("[Enter] - [Q]uit.");
                Console.WriteLine("");
                Console.Write("ENTER SEEDING OPTION: ");
        }

        private static IDataContextInitializationAdapter<RioValleyChiliDataContext> GetDataContextInitializationAdapter(char keyInfo)
        {
            switch(keyInfo)
            {
                case '1':
                case 'C':
                case 'c':
                    return new RvcMigrationDataContextInitializationAdapter<CoreDbContextDataSeeder>();

                case '2':
                case 'M':
                case 'm':
                    return new RvcMigrationDataContextInitializationAdapter<CompanyDbContactDataSeeder>();

                case '3':
                case 'P':
                case 'p':
                    return new RvcMigrationDataContextInitializationAdapter<ProductsDbContextDataSeeder>();

                case '4':
                case 'I':
                case 'i':
                    return new RvcMigrationDataContextInitializationAdapter<InventoryDbContextDataSeeder>();

                case '5':
                case 'R':
                case 'r':
                    return new RvcMigrationDataContextInitializationAdapter<ProductionDataSeeder>();

                case '6':
                case 'O':
                case 'o':
                    return new RvcMigrationDataContextInitializationAdapter<OrderDbContextDataSeeder>();

                case '7':
                case 'T':
                case 't':
                case '*': //NOTICE: ensure '*' always loads all data; this is used by the daily load script -VK 12/0/13
                    return new RvcMigrationDataContextInitializationAdapter<InventoryTransactionsDataSeeder>();

                default: return new RvcMigrationDataContextInitializationAdapter<DoNothingDbContextDataSeeder<RioValleyChiliDataContext>>();
            }
        }

        private static void ExecuteInitialization(RioValleyChiliDataContext context, IDataContextInitializationAdapter<RioValleyChiliDataContext> initializationAdapter, string[] args)
        {
            ContextsHelper.ConsoleOutputSettings();
            RVCDataLoadLoggerGate.RVCDataLoadLogger = RVCDataLoadLogger.GetDataLoadLogger(ExtractArgs(args, "-LogFolder "));

            if(context.Database.Exists())
            {
                Console.WriteLine("Preserving old data.");
                PreservedData.ObtainData(context);

                Console.WriteLine("Deleting existing database.");
                context.Database.Delete();
            }

            Console.WriteLine("Initializing database.");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            DataLoadResult.Success = true;
            initializationAdapter.InitializeDataContext(ref context);
            new RVCDataLoadResultObtainer().SetDataLoadResult(new RVCDataLoadResultObtainer.LoadResult
                {
                    Success = DataLoadResult.Success,
                    RanToCompletion = true,
                    TimeStamp = DateTime.Now
                });
            stopwatch.Stop();

            Console.WriteLine("\n****************************************");
            Console.WriteLine("DATA LOAD COMPLETE");
            Console.WriteLine("Success: {0}", DataLoadResult.Success);
            Console.WriteLine("Total Run Time: {0:g}", stopwatch.Elapsed);
            Console.WriteLine("****************************************\n");

            if(RVCDataLoadLoggerGate.RVCDataLoadLogger != null)
            {
                RVCDataLoadLoggerGate.RVCDataLoadLogger.WriteLogSummary();
            }
        }

        private static void DisplayLogFileMenu()
        {
            var display = true;
            while(display)
            {
                Console.WriteLine("1 - Open summary log");
                Console.WriteLine("2 - Open logs in explorer");
                Console.WriteLine("0 - Return to main menu");
                Console.Write("ENTER OPTION: ");
                var keyInfo = Console.ReadKey();
                Console.WriteLine("\n...................................................\n");

                switch(keyInfo.KeyChar)
                {
                    case '0': display = false; break;
                    case '1': OpenFileOrDirectory(RVCDataLoadLoggerGate.RVCDataLoadLogger.DataLoadSummaryPath); break;
                    case '2': OpenFileOrDirectory(RVCDataLoadLoggerGate.RVCDataLoadLogger.LogPath, false); break;
                }
            }
        }

        private static void OpenFileOrDirectory(string path, bool file = true)
        {
            var exists = file ? (Func<string, bool>)(File.Exists) : (Directory.Exists);
            if(!exists(path))
            {
                Console.WriteLine("Could not find {0}: [{1}]", file ? "file" : "directory", path);
                return;
            }

            Process.Start(path);
        }

        private static Exception FindInnermostException(Exception ex)
        {
            if(ex == null) { return null; }

            var exception = ex;
            while(exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            return exception == ex ? null : exception;
        }

        private static string ExtractArgs(IEnumerable<string> args, string arg)
        {
            if(args != null && !string.IsNullOrWhiteSpace(arg))
            {
                var match = args.FirstOrDefault(a => a != null && a.StartsWith(arg));
                if(match != null)
                {
                    return match.Replace(arg, "");
                }
            }

            return null;
        }
    }
}
