using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class WarehouseKeyTests : KeyTestsBase<FacilityKey, IFacilityKey>
    {
        private const int EXPTECTED_WAREHOUSE_ID = 5;
        private const string EXPECTED_STRING_VALUE = "5";
        private const string VALID_PARSE_INPUT = "5";
        private const string INVALID_PARSE_INPUT = "no bueno";

        #region Overrides of KeyTestsBase<WarehouseKey,IWarehouseKey>

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

        protected override void SetUpValidMock(Mock<IFacilityKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.FacilityKey_Id).Returns(EXPTECTED_WAREHOUSE_ID);
        }

        protected override FacilityKey BuildKey(IFacilityKey keyInterface)
        {
            return new FacilityKey(keyInterface);
        }

        protected override void AssertValidKey(IFacilityKey resultingKey)
        {
            Assert.AreEqual(EXPTECTED_WAREHOUSE_ID, resultingKey.FacilityKey_Id);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = FacilityKey.Null;
            var n2 = FacilityKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}