using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Core.Helpers.AutoFixture;

namespace RioValleyChili.Tests.Extensions.AutoFixture
{
    [TestFixture]
    public class AutoFixtureCustomizationTests
    {
        public interface IInterface
        {
            string P1 { get; set; }

            int P2 { get; set; }

            IList<string> L1 { get; }

            IList<IInterface2> L2 { get; }

            IInterface3 C1 { get; }
        }

        public interface IInterface2
        {
            string Sub1 { get; }
        }

        public interface IInterface3
        {
            IList<IInterface2> L1 { get; } 
        }

        public class QueryableTest
        {
            public IQueryable<string> Q1 { get; set; }
        }

        [Test]
        public void CreateAnonymousObjectFromInterface()
        {
            // Arrange
            var fixture = new Fixture().Customize(new MockWithAutoPropertiesCustomization());

            // Act
            var obj = fixture.Create<IInterface>();

            // Assert
            Assert.IsNotNull(obj);
            Assert.IsNotEmpty(obj.L1);
            Assert.IsNotEmpty(obj.L2);
            Assert.IsNotNull(obj.C1);
            Assert.IsNotEmpty(obj.C1.L1);
        }

        [Test]
        public void QueryablePropetyTest()
        {
            // Arrange
            var fixture = new Fixture().Customize(new QueryableCustomization());

            // Act
            var obj = fixture.Create<QueryableTest>();

            // Assert
            Assert.IsNotNull(obj);
            Assert.IsNotEmpty(obj.Q1);
        }
    }
}