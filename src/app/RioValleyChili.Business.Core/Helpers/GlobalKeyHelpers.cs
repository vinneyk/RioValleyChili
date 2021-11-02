using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.StaticRecords;

namespace RioValleyChili.Business.Core.Helpers
{
    public static class GlobalKeyHelpers
    {
        //todo: read values from config on startup rather than hard coding
        public static IFacilityKey RinconFacilityKey = StaticFacilities.Rincon;
        public static IInventoryTreatmentKey NoTreatmentKey = new NoTreatmentKeyHelper();
        public static IAttributeNameKey AstaAttributeNameKey = new AstaAttributeNameKeyHelper();
        public static IAttributeNameKey TPCAttributeNameKey = new TPCAttributeNameKeyHelper();
        public static IAttributeNameKey YeastMoldAttributeNameKey = new YeastMoldAttributeNameKeyHelper();
        public static IAttributeNameKey ColiformAttributeNameKey = new ColiformAttributeNameKeyHelper();
        public static IAttributeNameKey EColiAttributeNameKey = new EColiAttributeNameKeyHelper();
        public static IAttributeNameKey SalmonellaAttributeNameKey = new SalmonellaAttributeNameKeyHelper();

        private class NoTreatmentKeyHelper : IInventoryTreatmentKey
        {
            public int InventoryTreatmentKey_Id { get { return 0; } }
        }

        private class AstaAttributeNameKeyHelper : IAttributeNameKey
        {
            public string AttributeNameKey_ShortName { get { return "Asta"; } }
        }

        private class TPCAttributeNameKeyHelper : IAttributeNameKey
        {
            public string AttributeNameKey_ShortName { get { return "TPC"; } }
        }

        private class YeastMoldAttributeNameKeyHelper : IAttributeNameKey
        {
            public string AttributeNameKey_ShortName { get { return "YeastMold"; } }
        }

        private class ColiformAttributeNameKeyHelper : IAttributeNameKey
        {
            public string AttributeNameKey_ShortName { get { return "Coliform"; } }
        }

        private class EColiAttributeNameKeyHelper : IAttributeNameKey
        {
            public string AttributeNameKey_ShortName { get { return "EColi"; } }
        }

        private class SalmonellaAttributeNameKeyHelper : IAttributeNameKey
        {
            public string AttributeNameKey_ShortName { get { return "Salmonella"; } }
        }
    }
}