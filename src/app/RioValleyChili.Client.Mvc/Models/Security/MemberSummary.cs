namespace RioValleyChili.Client.Mvc.Models.Security
{
    public class MemberSummary
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
    }
}