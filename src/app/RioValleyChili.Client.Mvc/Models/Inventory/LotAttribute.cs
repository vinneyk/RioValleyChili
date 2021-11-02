namespace RioValleyChili.Client.Mvc.Models.Inventory
{
    public class LotAttribute 
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        //public DateTime AttributeDate { get; set; }
        public string AttributeDate { get; set; }
        public bool Computed { get; set; }
    }
}