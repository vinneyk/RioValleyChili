using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Moq;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Match = System.Text.RegularExpressions.Match;

namespace RioValleyChili.Services.OldContextSynchronization.Tests.Base
{
    public abstract class SynchronizeOldContextIntegrationTestsBase<TService> : ServiceIntegrationTestBase<TService> where TService : class
    {
        protected sealed override bool OldContextSynchronizationEnabled { get { return true; } }
        protected override bool TestHelperDropAndRecreateContext { get { return false; } }
        protected override bool SetupStaticRecords { get { return false; } }

        protected Mock<IKillSwitch> MockKillSwitch;

        private StringBuilder ConsoleStringBuilder { get; set; }
        
        protected override void DerivedSetUp()
        {
            base.DerivedSetUp();
            _consoleOutput = Console.Out;
            Console.SetOut(new StringWriter(ConsoleStringBuilder = new StringBuilder()));
            KillSwitch.Instance = (MockKillSwitch = new Mock<IKillSwitch>()).Object;
            SynchronizationCommandFactory.Factory = null;
        }

        private TextWriter _consoleOutput;
        private void RestoreConsoleOutput()
        {
            Console.SetOut(_consoleOutput);
        }

        protected string GetKeyFromConsoleString(string pattern)
        {
            var consoleString = ConsoleStringBuilder.ToString();
            RestoreConsoleOutput();
            Console.WriteLine(consoleString);
            return GetKeyFromString(consoleString, pattern);
        }

        private string GetKeyFromString(string text, string pattern)
        {
            var match = MatchToConsoleOutput(text, pattern);
            if(!match.Success)
            {
                throw new Exception(string.Format("Could not match text[{0}] with pattern[{1}].", text, pattern));
            }

            return match.Groups[1].Value;
        }

        private Match MatchToConsoleOutput(string text, string pattern)
        {
            var patternToMatch = pattern.Replace("(", "\\(");
            patternToMatch = patternToMatch.Replace(")", "\\)");
            patternToMatch = patternToMatch.Replace("[", "\\[");
            patternToMatch = patternToMatch.Replace("]", "\\]");
            patternToMatch = Regex.Replace(patternToMatch, @"{\d.*?}", @"(.*?)");
            return Regex.Match(text, patternToMatch);
        }

        public override Employee TestUser
        {
            get { return _testUser ?? (_testUser = GetDataLoadUser("DataLoadUser")); }
        }
        private Employee _testUser;

        private Employee GetDataLoadUser(string userName)
        {
            var employee = TestHelper.Context.Employees.FirstOrDefault(e => e.UserName == userName);
            if(employee == null)
            {
                throw new Exception(string.Format("User[{0}] not found in new context.", userName));
            }

            using(var oldContext = new RioAccessSQLEntities())
            {
                if(!oldContext.tblEmployees.Any(e => e.Employee == userName))
                {
                    throw new Exception(string.Format("tblEmployee[{0}] not found in old context.", userName));
                }
            }

            return employee;
        }
    }
}