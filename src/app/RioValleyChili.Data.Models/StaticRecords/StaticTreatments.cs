namespace RioValleyChili.Data.Models.StaticRecords
{
    public static class StaticInventoryTreatments
    {
        static StaticInventoryTreatments ()
        {
            NoTreatment = new InventoryTreatment
                    {
                        Id = 0,
                        LongName = "Not Treated",
                        ShortName = "NA",
                        Active = true
                    };

            ETO = new InventoryTreatment
            {
                Id = 1,
                LongName = "ETO Treated",
                ShortName = "ET",
                Active = true
            };

            GT = new InventoryTreatment
            {
                Id = 2,
                LongName = "Gamma Treated",
                ShortName = "GT",
                Active = true
            };
        }

        public static InventoryTreatment NoTreatment { get; private set; }
        public static InventoryTreatment ETO { get; private set; }
        public static InventoryTreatment GT { get; private set; }
    }
}