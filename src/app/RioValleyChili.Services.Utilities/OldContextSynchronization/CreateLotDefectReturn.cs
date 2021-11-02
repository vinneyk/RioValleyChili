using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;

namespace RioValleyChili.Services.Utilities.OldContextSynchronization
{
    public class CreateLotDefectReturn : LotStatInfoReturn, ICreateLotDefectReturn
    {
        public string LotDefectKey { get; set; }

        public CreateLotDefectReturn(string lotDefectKey, SynchronizeLotParameters parameters) : base(parameters)
        {
            LotDefectKey = lotDefectKey;
        }
    }
}