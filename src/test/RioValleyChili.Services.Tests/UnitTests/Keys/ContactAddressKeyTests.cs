using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class ContactAddressKeyTests : KeyTestsBase<ContactAddressKey, IContactAddressKey>
    {
        private const int expectedCompanyId = 1;
        private const int expectedContactId = 42;
        private const int expectedAddressId = 333;
        private const string expectedString = "1-42-333";
        private const string invalidString = "1-42-monkey-333";

        #region Overrides of KeyTestsBase<ContactAddressKey, IContactAddressKey>

        protected override string ExpectedStringValue { get { return expectedString; } }

        protected override string ValidParseInput  { get { return expectedString; } }

        protected override string InvalidParseInput { get { return invalidString; } }

        protected override void SetUpValidMock(Mock<IContactAddressKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.CompanyKey_Id).Returns(expectedCompanyId);
            mockKeyInterface.SetupGet(m => m.ContactKey_Id).Returns(expectedContactId);
            mockKeyInterface.SetupGet(m => m.ContactAddressKey_Id).Returns(expectedAddressId);
        }

        protected override ContactAddressKey BuildKey(IContactAddressKey keyInterface)
        {
            return new ContactAddressKey(keyInterface);
        }

        protected override void AssertValidKey(IContactAddressKey resultingKey)
        {
            Assert.AreEqual(expectedCompanyId, resultingKey.CompanyKey_Id);
            Assert.AreEqual(expectedContactId, resultingKey.ContactKey_Id);
            Assert.AreEqual(expectedAddressId, resultingKey.ContactAddressKey_Id);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = ContactAddressKey.Null;
            var n2 = ContactAddressKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}