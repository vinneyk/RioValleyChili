//using System;
//using System.Configuration;
//using System.Reflection;
//using NUnit.Framework;
//using RioValleyChili.Core;

//namespace RioValleyChili.Services.Tests.IntegrationTests
//{
//    [TestFixture]
//    public class DBConnectionStatusTests
//    {
//        [SetUp]
//        public void SetUp()
//        {
//            DBConnectionStatusImplementation.RegisterImplementations();
//        }

//        [Test]
//        public void OldContextConnectionStatus_IsValid_returns_true_if_connection_string_is_not_modified()
//        {
//            Assert.IsTrue(DBConnectionStatus.OldContextIsValid);
//        }

//        [Test]
//        public void OldContextConnectionStatus_IsValid_returns_false_if_bad_login_information_is_used_in_connection_string()
//        {
//            var originalConnectionString = ConfigurationManager.ConnectionStrings["RioAccessSQLEntities"].ConnectionString;
//            SetConnectionString("RioAccessSQLEntities", @"metadata=res://*/OldDataModels.csdl|res://*/OldDataModels.ssdl|res://*/OldDataModels.msl;provider=System.Data.SqlClient;provider connection string='data source=.\SolutionSqlSrv;initial catalog=RioAccessSQL;User ID=badUser;Password=badPass;multipleactiveresultsets=True;App=EntityFramework'");
//            Assert.IsFalse(DBConnectionStatus.OldContextIsValid);
//            SetConnectionString("RioAccessSQLEntities", originalConnectionString);

//            var exception = DBConnectionStatus.OldContextException;
//            if(exception != null)
//            {
//                Console.WriteLine(exception.Message);
//                Assert.Pass(exception.Message);
//            }
//        }

//        [Test]
//        public void NewContextConnectionStatus_IsValid_returns_true_if_connection_string_is_not_modified()
//        {
//            Assert.IsTrue(DBConnectionStatus.NewContextIsValid);
//        }

//        [Test]
//        public void NewContextConnectionStatus_IsValid_returns_false_if_bad_login_information_is_used_in_connection_string()
//        {
//            var originalConnectionString = ConfigurationManager.ConnectionStrings["RioValleyChiliDataContext"].ConnectionString;
//            SetConnectionString("RioValleyChiliDataContext", @"data source=.\SOLUTIONSQLSRV;User ID=badUser;Password=badPass;Initial Catalog=RvcData_Test;MultipleActiveResultSets=True");
//            Assert.IsFalse(DBConnectionStatus.NewContextIsValid);
//            SetConnectionString("RioValleyChiliDataContext", originalConnectionString);

//            var exception = DBConnectionStatus.NewContextException;
//            if(exception != null)
//            {
//                Console.WriteLine(exception.Message);
//                Assert.Pass(exception.Message);
//            }
//        }

//        private void SetConnectionString(string name, string connectionString)
//        {
//            var element = ConfigurationManager.ConnectionStrings[name];
//            var readOnly = typeof(ConfigurationElement).GetField("_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
//            readOnly.SetValue(element, false);
//            element.ConnectionString = connectionString;
//        }
//    }
//}