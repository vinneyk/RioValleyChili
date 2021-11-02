using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces
{
    public interface ICreateChileLotProduction : ICreatePickedInventory
    {
        ILotKey LotKey { get; }

        ProductionType ProductionType { get; }
    }
}