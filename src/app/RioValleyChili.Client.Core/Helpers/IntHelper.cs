namespace RioValleyChili.Client.Core.Helpers
{
    public static class IntHelper
    {
        public static string ToWeightInPounds(this int value)
        {
            return string.Format("{0:n0} lbs", value);
        }
    }
}