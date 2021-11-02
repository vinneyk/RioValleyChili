using System;
using System.Collections.Generic;

namespace RioValleyChili.Client.Reporting.Models
{
    public class ProductionAdditiveInputs
    {
        public DateTime ProductionStart { get; set; }
        public DateTime ProductionEnd { get; set; }

        public IEnumerable<ProductionAdditiveInputs_ByDate> ByDates { get; set; }
    }

    public class ProductionAdditiveInputs_ByDate
    {
        public DateTime ProductionDate { get; set; }
        public IEnumerable<ProductionLotAdditiveInputs> Lots { get; set; }
        public IEnumerable<ProductionAdditiveInputs_Totals> Totals { get; set; }
    }

    public class ProductionLotAdditiveInputs
    {
        public string LotKey { get; set; }
        public string Product { get; set; }
        public IEnumerable<ProductionAdditiveInputs_ByAdditiveType> Additives { get; set; }
    }

    public class ProductionAdditiveInputs_ByAdditiveType
    {
        public string AdditiveType { get; set; }
        public IEnumerable<ProductionAdditiveInputPicked> PickedItems { get; set; }
    }

    public class ProductionAdditiveInputPicked
    {
        public string AdditiveType { get; set; }
        public string LotKey { get; set; }
        public int TotalPoundsPicked { get; set; }
        public string UserResultEntered { get; set; }
    }

    public class ProductionAdditiveInputs_Totals
    {
        public string AdditiveType { get; set; }
        public double TotalPoundsPicked { get; set; }
    }
}