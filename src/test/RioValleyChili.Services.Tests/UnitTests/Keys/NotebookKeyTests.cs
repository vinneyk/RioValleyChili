using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class NotebookKeyTests : KeyTestsBase<NotebookKey, INotebookKey>
    {
        private readonly DateTime _expectedNotebookDate = new DateTime(2012, 3, 29);
        private const int _expectedNotebookSequence = 1;
        private const string EXPECTED_STRING_VALUE = "20120329-1";
        private const string INVALID_PARSE_INPUT = "20120329-ZZOOooomMM!";

        #region Overrides of NewKeyTestsBase<NotebookKey>

        protected override string ExpectedStringValue
        {
            get { return EXPECTED_STRING_VALUE; }
        }

        protected override string ValidParseInput
        {
            get { return EXPECTED_STRING_VALUE; }
        }

        protected override string InvalidParseInput
        {
            get { return INVALID_PARSE_INPUT; }
        }

        protected override void SetUpValidMock(Mock<INotebookKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.NotebookKey_Date).Returns(_expectedNotebookDate);
            mockKeyInterface.SetupGet(m => m.NotebookKey_Sequence).Returns(_expectedNotebookSequence);
        }

        protected override NotebookKey BuildKey(INotebookKey keyInterface)
        {
            return new NotebookKey(keyInterface);
        }

        protected override void AssertValidKey(INotebookKey resultingKey)
        {
            Assert.AreEqual(_expectedNotebookDate, resultingKey.NotebookKey_Date);
            Assert.AreEqual(_expectedNotebookSequence, resultingKey.NotebookKey_Sequence);
        }

        #endregion

        [Test]
        public void Feedback_Notebook_Key_is_as_expected()
        {
            var key = new NotebookKey(Data.Models.StaticRecords.StaticNotebooks.FeedbackNotebook).KeyValue;
            Assert.AreEqual("17530101-0", key);
        }
    }
}