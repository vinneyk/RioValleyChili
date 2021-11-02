using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;

namespace RioValleyChili.Services.Utilities.OldContextSynchronization
{
    public class LotStatInfoReturn : ILotStatInfoReturn
    {
        private readonly SynchronizeLotParameters _parameters;

        public LotStatInfoReturn(SynchronizeLotParameters parameters)
        {
            _parameters = parameters;
        }

        public LotStat? LotStatEnum
        {
            get { return _parameters == null ? null : LotStatHelper.GetLotStat(_parameters.LotStat); }
        }

        public string LotStat
        {
            get { return LotStatEnum.GetLotStatText(); }
        }

        public string LotNotes
        {
            get
            {
                if(_parameters == null || LotStatEnum != Core.LotStat.See_Desc)
                {
                    return null;
                }

                return _parameters.Notes;
            }
        }
    }
}