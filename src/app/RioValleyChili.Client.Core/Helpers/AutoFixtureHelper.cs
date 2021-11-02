using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers.AutoFixture;

namespace RioValleyChili.Client.Core.Helpers
{
    public static class AutoFixtureHelper
    {
        public static IFixture BuildFixture()
        {
            return new Fixture()
                .Customize(new QueryableCustomization())
                .Customize(new MockWithAutoPropertiesCustomization());
        }
    }
}
