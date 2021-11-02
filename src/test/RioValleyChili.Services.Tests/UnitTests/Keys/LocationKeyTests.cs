using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class LocationKeyTests : KeyTestsBase<LocationKey, ILocationKey>
    {
        private const int expectedId = 42;
        private const string expectedKeyString = "42";
        private const string invalidKeyString = "All your base.";

        #region Overrides of KeyTestsBase<LocationKey, ILocationKey>

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

        protected override void SetUpValidMock(Mock<ILocationKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.LocationKey_Id).Returns(expectedId);
        }

        protected override LocationKey BuildKey(ILocationKey keyInterface)
        {
            return new LocationKey(keyInterface);
        }

        protected override void AssertValidKey(ILocationKey resultingKey)
        {
            Assert.AreEqual(expectedId, resultingKey.LocationKey_Id);
        }

        #endregion
    }
}