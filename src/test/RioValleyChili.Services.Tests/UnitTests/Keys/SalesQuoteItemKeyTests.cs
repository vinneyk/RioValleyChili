using System;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public class SalesQuoteItemKeyTests : KeyTestsBase<SalesQuoteItemKey, ISalesQuoteItemKey>
    {
        private readonly DateTime expectedDateCreated = new DateTime(2012, 3, 29);
        private const int expectedOrderSequence = 42;
        private const int expectedItemSequence = 99;
        private const string expectedKeyString = "20120329-42-99";
        private const string invalidKeyString = "Harumph Harumph Harumph";

        protected override string ExpectedStringValue { get { return expectedKeyString; } }
        protected override string ValidParseInput { get { return expectedKeyString; } }
        protected override string InvalidParseInput { get { return invalidKeyString; } }

        protected override void SetUpValidMock(Mock<ISalesQuoteItemKey> mockKeyInterface)
        {
            mockKeyInterface.SetupGet(m => m.SalesQuoteKey_DateCreated).Returns(expectedDateCreated);
            mockKeyInterface.SetupGet(m => m.SalesQuoteKey_Sequence).Returns(expectedOrderSequence);
            mockKeyInterface.SetupGet(m => m.SalesQuoteItemKey_ItemSequence).Returns(expectedItemSequence);
        }

        protected override SalesQuoteItemKey BuildKey(ISalesQuoteItemKey keyInterface)
        {
            return new SalesQuoteItemKey(keyInterface);
        }

        protected override void AssertValidKey(ISalesQuoteItemKey resultingKey)
        {
            Assert.AreEqual(expectedDateCreated, resultingKey.SalesQuoteKey_DateCreated);
            Assert.AreEqual(expectedOrderSequence, resultingKey.SalesQuoteKey_Sequence);
            Assert.AreEqual(expectedItemSequence, resultingKey.SalesQuoteItemKey_ItemSequence);
        }
    }
}