using RioValleyChili.Business.Core.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces
{
    internal interface IUpdateProductionResultCommandParameters : IProductionResultCommandParameters
    {
        LotKey ProductionResultKey { get; }
    }
}