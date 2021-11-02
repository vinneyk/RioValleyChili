namespace RioValleyChili.Services.Utilities.Models
{
    internal class PackScheduleBaseWithCustomerReturn : PackScheduleBaseParametersReturn
    {
        internal CustomerWithProductSpecReturn Customer { get; set; }
    }
}