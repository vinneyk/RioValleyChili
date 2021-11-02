using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class NoteKeyTests : KeyTestsBase<NoteKey, INoteKey>
    {
        private readonly DateTime _expectedNotebookDate = new DateTime(2012, 3, 29);
        private const int _expectedNotebookSequence = 1;
        private const int _expectedNoteSequence = 2;
        private const string EXPECTED_STRING_VALUE = "20120329-1-2";
        private const string INVALID_PARSE_INPUT = "20120329-ZZOOooomMM!-2";

        #region Overrides of NewKeyTestsBase<NoteKey>

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

        protected override void SetUpValidMock(Mock<INoteKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.NotebookKey_Date).Returns(_expectedNotebookDate);
            mockKeyInterface.SetupGet(m => m.NotebookKey_Sequence).Returns(_expectedNotebookSequence);
            mockKeyInterface.SetupGet(m => m.NoteKey_Sequence).Returns(_expectedNoteSequence);
        }

        protected override NoteKey BuildKey(INoteKey keyInterface)
        {
            return new NoteKey(keyInterface);
        }

        protected override void AssertValidKey(INoteKey resultingKey)
        {
            Assert.AreEqual(_expectedNotebookDate, resultingKey.NotebookKey_Date);
            Assert.AreEqual(_expectedNotebookSequence, resultingKey.NotebookKey_Sequence);
            Assert.AreEqual(_expectedNoteSequence, resultingKey.NoteKey_Sequence);
        }

        #endregion
    }
}