using System;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public static class ContextsHelper
    {
        public static string NewContextConnectionString
        {
            get { return _newContextConnectionString ?? (_newContextConnectionString = ConfigurationManager.ConnectionStrings["RioValleyChiliDataContext"].ConnectionString); }
        }
        private static string _newContextConnectionString;

        public static string OldContextConnectionString
        {
            get { return _oldContextConnectionString ?? (_oldContextConnectionString = ConfigurationManager.ConnectionStrings["RioAccessSQLEntities"].ConnectionString); }
        }
        private static string _oldContextConnectionString;

        public static RioValleyChiliDataContext GetNewContext()
        {
            return new RioValleyChiliDataContext(NewContextConnectionString);
        }

        public static RioAccessSQLEntities GetOldContext()
        {
            return new RioAccessSQLEntities(GetOldContextConnection());
        }

        public static EntityConnection GetOldContextConnection()
        {
            return new EntityConnection(OldContextConnectionString);
        }

        public static void ConsoleOutputSettings()
        {
            Console.WriteLine("Using new context connection string from configuration file: {0}\n", NewContextConnectionString);
            Console.WriteLine("Using old source context connection string from configuration file: {0}\n", OldContextConnectionString);
        }
    }
}