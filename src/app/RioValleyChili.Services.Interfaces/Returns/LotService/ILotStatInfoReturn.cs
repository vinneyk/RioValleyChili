using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ILotStatInfoReturn
    {
        LotStat? LotStatEnum { get; }
        string LotStat { get; }
        string LotNotes { get; }
    }

    public static class ILotStatInfoReturnExtensions
    {
        public static string GetLotStatDescription(this ILotStatInfoReturn info)
        {
            var lotStat = info == null ? null : info.LotStatEnum;
            return lotStat == LotStat.See_Desc ? string.Format("{0} ({1})", lotStat.GetLotStatText(), info.LotNotes) : lotStat.GetLotStatText();
        }
    }
}