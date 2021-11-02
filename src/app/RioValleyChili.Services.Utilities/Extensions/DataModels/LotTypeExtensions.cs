using RioValleyChili.Core;

namespace RioValleyChili.Services.Utilities.Extensions.DataModels
{
    public static class LotTypeExtensions
    {
        public static string ToFullName(this LotTypeEnum lotType)
        {
            return lotType == null ? string.Empty : string.Format("{0} - {1}", (int)lotType, lotType.ToString());
        }
    }
}