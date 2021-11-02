using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateLotDefectParameters
    {
        internal ICreateLotDefectParameters Parameters { get; set; }

        public LotKey LotKey { get; set; }
    }
}