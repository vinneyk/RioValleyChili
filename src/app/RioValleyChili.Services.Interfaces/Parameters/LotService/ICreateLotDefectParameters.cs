using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.LotService
{
    public interface ICreateLotDefectParameters : IUserIdentifiable
    {
        DefectTypeEnum DefectType { get; }
        string LotKey { get; }
        string Description { get; }
    }
}