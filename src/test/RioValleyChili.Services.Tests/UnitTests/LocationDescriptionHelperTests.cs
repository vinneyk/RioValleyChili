using NUnit.Framework;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Services.Tests.UnitTests
{
    [TestFixture]
    public class LocationDescriptionHelperTests
    {
        [Test]
        public void Works_as_expected()
        {
            Assert.AreEqual("Line~23", LocationDescriptionHelper.GetDescription("Line", 23));
            Assert.AreEqual("A~1", LocationDescriptionHelper.GetDescription("A", 1));

            string street;
            int row;
            Assert.IsTrue(LocationDescriptionHelper.GetStreetRow("Line", out street, out row));
            Assert.AreEqual("Line", street);
            Assert.AreEqual(0, row);

            Assert.IsTrue(LocationDescriptionHelper.GetStreetRow("Line~23", out street, out row));
            Assert.AreEqual("Line", street);
            Assert.AreEqual(23, row);

            Assert.IsTrue(LocationDescriptionHelper.GetStreetRow("Line~Thing~23", out street, out row));
            Assert.AreEqual("Line~Thing", street);
            Assert.AreEqual(23, row);
        }
    }
}