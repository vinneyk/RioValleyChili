namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests.Production
{
    public class SetProductionScheduleItemRequest
    {
        public int Index { get; set; }
        public bool FlushBefore { get; set; }
        public string FlushBeforeInstructions { get; set; }
        public bool FlushAfter { get; set; }
        public string FlushAfterInstructions { get; set; }
        public string PackScheduleKey { get; set; }
    }
}