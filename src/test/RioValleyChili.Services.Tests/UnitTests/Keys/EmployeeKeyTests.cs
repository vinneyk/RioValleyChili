using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class EmployeeKeyTests : KeyTestsBase<EmployeeKey, IEmployeeKey>
    {
        private const int expectedId = 42;
        private const string expectedKeyString = "42";
        private const string invalidKeyString = "42-24";

        #region Overrides of KeyTestsBase<EmployeeKey, IEmployeeKey>

        protected override string ExpectedStringValue
        {
            get { return expectedKeyString; }
        }

        protected override string ValidParseInput
        {
            get { return expectedKeyString; }
        }

        protected override string InvalidParseInput
        {
            get { return invalidKeyString; }
        }

        protected override void SetUpValidMock(Mock<IEmployeeKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.EmployeeKey_Id).Returns(expectedId);
        }

        protected override EmployeeKey BuildKey(IEmployeeKey keyInterface)
        {
            return new EmployeeKey(keyInterface);
        }

        protected override void AssertValidKey(IEmployeeKey resultingKey)
        {
            Assert.AreEqual(expectedId, resultingKey.EmployeeKey_Id);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = EmployeeKey.Null;
            var n2 = EmployeeKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}