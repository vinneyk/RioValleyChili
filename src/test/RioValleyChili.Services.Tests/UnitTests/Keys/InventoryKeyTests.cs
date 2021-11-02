using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class InventoryKeyTests : KeyTestsBase<InventoryKey, IInventoryKey>
    {
        private readonly DateTime _expectedDateCreated = new DateTime(2012, 5, 17);
        private const int EXPECTED_DATE_SEQUENCE = 10;
        private const int EXPECTED_LOT_TYPE_ID = 2;
        private const int EXPECTED_PACKAGING_PRODUCT_ID = 1;
        private const int EXPECTED_WAREHOUSE_LOCATION_ID = 5;
        private const int EXPECTED_TREATMENT_ID = 7;
        private const string EXPECTED_TOTE_KEY = "123TOTE!";
        private const string EXPECTED_KEY = "20120517;10;2;1;5;7;123TOTE!";
        private const string INVALID_PARSE_INPUT = "20120517-10-2-1-5-7-123TOTE!";

        #region Overrides of KeyTestsBase<InventoryKey,IInventoryKey>

        protected override string ExpectedStringValue { get { return EXPECTED_KEY; } }
        protected override string ValidParseInput { get { return EXPECTED_KEY; } }
        protected override string InvalidParseInput { get { return INVALID_PARSE_INPUT; } }

        protected override void SetUpValidMock(Mock<IInventoryKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.LotKey_DateCreated).Returns(_expectedDateCreated);
            mockKeyInterface.SetupGet(m => m.LotKey_DateSequence).Returns(EXPECTED_DATE_SEQUENCE);
            mockKeyInterface.SetupGet(m => m.LotKey_LotTypeId).Returns(EXPECTED_LOT_TYPE_ID);
            mockKeyInterface.SetupGet(m => m.PackagingProductKey_ProductId).Returns(EXPECTED_PACKAGING_PRODUCT_ID);
            mockKeyInterface.SetupGet(m => m.LocationKey_Id).Returns(EXPECTED_WAREHOUSE_LOCATION_ID);
            mockKeyInterface.SetupGet(m => m.InventoryTreatmentKey_Id).Returns(EXPECTED_TREATMENT_ID);
            mockKeyInterface.SetupGet(m => m.InventoryKey_ToteKey).Returns(EXPECTED_TOTE_KEY);
        }

        protected override InventoryKey BuildKey(IInventoryKey keyInterface)
        {
            return new InventoryKey(keyInterface);
        }

        protected override void AssertValidKey(IInventoryKey resultingKey)
        {
            Assert.AreEqual(_expectedDateCreated, resultingKey.LotKey_DateCreated);
            Assert.AreEqual(EXPECTED_DATE_SEQUENCE, resultingKey.LotKey_DateSequence);
            Assert.AreEqual(EXPECTED_LOT_TYPE_ID, resultingKey.LotKey_LotTypeId);
            Assert.AreEqual(EXPECTED_PACKAGING_PRODUCT_ID, resultingKey.PackagingProductKey_ProductId);
            Assert.AreEqual(EXPECTED_WAREHOUSE_LOCATION_ID, resultingKey.LocationKey_Id);
            Assert.AreEqual(EXPECTED_TREATMENT_ID, resultingKey.InventoryTreatmentKey_Id);
            Assert.AreEqual(EXPECTED_TOTE_KEY, resultingKey.InventoryKey_ToteKey);
        }

        #endregion

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = InventoryKey.Null;
            var n2 = InventoryKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}