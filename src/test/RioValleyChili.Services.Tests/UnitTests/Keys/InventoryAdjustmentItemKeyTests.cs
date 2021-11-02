using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class InventoryAdjustmentItemKeyTests : KeyTestsBase<InventoryAdjustmentItemKey, IInventoryAdjustmentItemKey>
    {
        private readonly DateTime _expectedTimeStamp = new DateTime(2012, 3, 29);
        private const int _expectedSequence = 42;
        private const int _expectedItemSequence = 33;
        private const string EXPECTED_STRING_VALUE = "20120329-42-33";
        private const string INVALID_PARSE_INPUT = "Urghhh-123-33";

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

        protected override void SetUpValidMock(Mock<IInventoryAdjustmentItemKey> mockKeyInterface)
        {
            mockKeyInterface.Setup(m => m.InventoryAdjustmentKey_AdjustmentDate).Returns(_expectedTimeStamp);
            mockKeyInterface.Setup(m => m.InventoryAdjustmentKey_Sequence).Returns(_expectedSequence);
            mockKeyInterface.Setup(m => m.InventoryAdjustmetItemKey_Sequence).Returns(_expectedItemSequence);
        }

        protected override InventoryAdjustmentItemKey BuildKey(IInventoryAdjustmentItemKey keyInterface)
        {
            return new InventoryAdjustmentItemKey(keyInterface);
        }

        protected override void AssertValidKey(IInventoryAdjustmentItemKey resultingKey)
        {
            Assert.AreEqual(_expectedTimeStamp, resultingKey.InventoryAdjustmentKey_AdjustmentDate);
            Assert.AreEqual(_expectedSequence, resultingKey.InventoryAdjustmentKey_Sequence);
            Assert.AreEqual(_expectedItemSequence, resultingKey.InventoryAdjustmetItemKey_Sequence);
        }

        [Test]
        public void GivenTwoNullInstances_EqualsOperatorDeterminesTrueEquality()
        {
            var n1 = InventoryAdjustmentItemKey.Null;
            var n2 = InventoryAdjustmentItemKey.Null;

            Assert.IsTrue(n1 == n2);
        }
    }
}