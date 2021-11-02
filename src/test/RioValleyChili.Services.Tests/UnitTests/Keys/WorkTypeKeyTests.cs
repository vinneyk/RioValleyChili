using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class WorkTypeKeyTests : KeyTestsBase<WorkTypeKey, IWorkTypeKey>
    {
        private const int EXPTECTED_WORK_TYPE_ID = 1;
        private const string EXPECTED_STRING_VALUE = "1";
        private const string VALID_PARSE_INPUT = "1";
        private const string INVALID_PARSE_INPUT = "no bueno";

        #region Overrides of KeyTestsBase<WorkTypeKey,IWorkTypeKey>

        protected override string ExpectedStringValue
        {
            get { return EXPECTED_STRING_VALUE; }
        }

        protected override string ValidParseInput
        {
            get { return VALID_PARSE_INPUT; }
        }

        protected override string InvalidParseInput
        {
            get { return INVALID_PARSE_INPUT; }
        }

        protected override void SetUpValidMock(Mock<IWorkTypeKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.WorkTypeKey_WorkTypeId).Returns(EXPTECTED_WORK_TYPE_ID);
        }

        protected override WorkTypeKey BuildKey(IWorkTypeKey keyInterface)
        {
            return new WorkTypeKey(keyInterface);
        }

        protected override void AssertValidKey(IWorkTypeKey resultingKey)
        {
            Assert.AreEqual(EXPTECTED_WORK_TYPE_ID, resultingKey.WorkTypeKey_WorkTypeId);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = WorkTypeKey.Null;
            var n2 = WorkTypeKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}