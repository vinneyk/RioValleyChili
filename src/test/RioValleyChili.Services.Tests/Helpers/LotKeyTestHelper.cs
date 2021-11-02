using System;
using FizzWare.NBuilder;
using Moq;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Tests.Helpers
{
    public static class LotKeyTestHelper
    {

        public static ILotKey BuildLotKey(int seed = 1)
        {
            var gen = new RandomGenerator(seed);
            var mockKey = new Mock<ILotKey>();
            mockKey.SetupGet(m => m.LotKey_DateCreated).Returns((DateTime) gen.DateTime());
            mockKey.SetupGet(m => m.LotKey_DateSequence).Returns((int) gen.Int());
            mockKey.SetupGet(m => m.LotKey_LotTypeId).Returns((int) gen.Int());

            return mockKey.Object;
        }

        public static IInventoryKey BuildInventoryKey(int seed = 1)
        {
            var gen = new RandomGenerator(seed);
            var mockKey = new Mock<IInventoryKey>();
            mockKey.SetupGet(m => m.LotKey_DateCreated).Returns((DateTime) gen.DateTime());
            mockKey.SetupGet(m => m.LotKey_DateSequence).Returns((int) gen.Int());
            mockKey.SetupGet(m => m.LotKey_LotTypeId).Returns((int) gen.Int());
            mockKey.SetupGet(m => m.LocationKey_Id).Returns((int) gen.Int());
            mockKey.SetupGet(m => m.PackagingProductKey_ProductId).Returns((int) gen.Int());

            return mockKey.Object;
        }

        public static ILotKey ParseKey(string keyValue)
        {
            keyValue = keyValue.Replace("--", "-");
            var parts = keyValue.Split('-');
            var dateCreated = DateTime.Parse(parts[0]);
            var dateSequence = int.Parse(parts[1]);
            var lotTypeId = int.Parse(parts[2]);

            var mockLotKey = new Mock<ILotKey>();
            mockLotKey.SetupGet(m => m.LotKey_DateCreated).Returns(dateCreated);
            mockLotKey.SetupGet(m => m.LotKey_DateSequence).Returns(dateSequence);
            mockLotKey.SetupGet(m => m.LotKey_LotTypeId).Returns(lotTypeId);

            return mockLotKey.Object;
        }
    }
}