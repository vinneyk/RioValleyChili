using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class ContactKeyTests : KeyTestsBase<ContactKey, IContactKey>
    {
        private const int expectedCompanyId = 1;
        private const int expectedContactId = 42;
        private const string expectedString = "1-42";
        private const string invalidString = "1-Banana";

        #region Overrides of KeyTestsBase<ContactKey, IContactKey>

        protected override string ExpectedStringValue
        {
            get { return expectedString; }
        }

        protected override string ValidParseInput
        {
            get { return expectedString; }
        }

        protected override string InvalidParseInput
        {
            get { return invalidString; }
        }

        protected override void SetUpValidMock(Mock<IContactKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.CompanyKey_Id).Returns(expectedCompanyId);
            mockKeyInterface.SetupGet(m => m.ContactKey_Id).Returns(expectedContactId);
        }

        protected override ContactKey BuildKey(IContactKey keyInterface)
        {
            return new ContactKey(keyInterface);
        }

        protected override void AssertValidKey(IContactKey resultingKey)
        {
            Assert.AreEqual(expectedCompanyId, resultingKey.CompanyKey_Id);
            Assert.AreEqual(expectedContactId, resultingKey.ContactKey_Id);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = ContactKey.Null;
            var n2 = ContactKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}