namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class UpdateTreatmentOrderDto
    {
        public bool VoidOrder { get; set; }

        public string ReceiveToLocation { get; set; }

        public string TreatmentKey { get; set; }
    }
}