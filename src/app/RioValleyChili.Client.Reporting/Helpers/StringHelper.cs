namespace RioValleyChili.Client.Reporting.Helpers
{
    public static class ReportStringExtensions
    {
        public static string ToReportString(this float f, string format, bool displayZero = true)
        {
            if((!displayZero && f == 0.0f) || float.IsNaN(f) || float.IsInfinity(f))
            {
                return "";
            }

            return f.ToString(format);
        }

        public static string ToReportString(this int i, string format, bool displayZero = true)
        {
            if((!displayZero && i == 0))
            {
                return "";
            }

            return i.ToString(format);
        }
    }
}
