using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class InstructionKeyTests : KeyTestsBase<InstructionKey, IInstructionKey>
    {
        private const int EXPECTED_INSTRUCTION_ID = 1;
        private const string EXPECTED_STRING_VALUE = "1";
        private const string VALID_PARSE_INPUT = "1";
        private const string INVALID_PARSE_INPUT = "THIS IS SPARTA!";

        #region Overrides of KeyTestsBase<InstructionKey,IInstructionKey>

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

        protected override void SetUpValidMock(Mock<IInstructionKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.InstructionKey_InstructionId).Returns(EXPECTED_INSTRUCTION_ID);
        }

        protected override InstructionKey BuildKey(IInstructionKey keyInterface)
        {
            return new InstructionKey(keyInterface);
        }

        protected override void AssertValidKey(IInstructionKey resultingKey)
        {
            Assert.AreEqual(EXPECTED_INSTRUCTION_ID, resultingKey.InstructionKey_InstructionId);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = InstructionKey.Null;
            var n2 = InstructionKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}