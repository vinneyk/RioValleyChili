namespace RioValleyChili.Core
{
#warning Default Enum value is not being used. 
    public enum ProductTypeEnum : short
    {
        Additive = 4,
        Chile = 1,
        Packaging = 5,
        NonInventory = 7
    }

    public enum SampleOrderStatus
    {
        Pending,
        Sent,
        Rejected,
        Approved,
        SeeJournalEntry
    }

    public static class SampleOrderStatusExtensions
    {
        public static string ToDisplayString(this SampleOrderStatus status)
        {
            switch(status)
            {
                case SampleOrderStatus.SeeJournalEntry: return "See Journal Entry";
                default: return status.ToString();
            }
        }
    }
}